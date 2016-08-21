using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.Windows;
using Newtonsoft.Json;
namespace MyNamespace5
{
    internal class MeshData
    {
        public double _dist;
        public string _handle;
        public Extents3d _exts;
        public Point3dCollection _vertices;
        public Int32Collection _faces;
        public string _color;

        public MeshData()
        {
            _dist = 1.0;
            _handle = String.Empty;
            _exts = new Extents3d();
            _vertices = new Point3dCollection();
            _faces = new Int32Collection();
            _color = String.Empty;
        }

        public MeshData(double  dist, string handle, Extents3d exts, Point3dCollection vertices, Int32Collection faces, string color)
        {
            _dist = dist;
            _handle = handle;
            _exts = exts;
            _vertices = vertices;
            _faces = faces;
            _color = color;
        }
    }

  internal class Utils
  {
    // Helper to get the document a palette was launched from
    // in the case where the active document is null
 
    internal static Document GetActiveDocument(
      DocumentCollection dm, Document launchDoc = null
    )
    {
      // If we're called from an HTML document, the active
      // document may be null
 
      var doc = dm.MdiActiveDocument;
      if (doc == null)
      {
        doc = launchDoc;
      }
      return doc;
    }
 
    internal static string GetSolids(
      Document launchDoc, Point3d camPos)
    {
      var doc =
        Utils.GetActiveDocument(
          Application.DocumentManager,
          launchDoc
        );
 
      // If we didn't find a document, return
 
      if (doc == null)
        return "";
 
      // We could probably get away without locking the document
      // - as we only need to read - but it's good practice to
      // do it anyway
 
      using (var dl = doc.LockDocument())
      {
        var db = doc.Database;
        var ed = doc.Editor;
 
        var ids = new ObjectIdCollection();
 
        using (
          var tr = doc.TransactionManager.StartOpenCloseTransaction()
        )
        {
          // Start by getting the modelspace
 
          var ms =
            (BlockTableRecord)tr.GetObject(
              SymbolUtilityServices.GetBlockModelSpaceId(db),
              OpenMode.ForRead
            );
 
          // If in palette mode we can get the camera from the
          // Editor, otherwise we rely on what was provided when
          // the HTML document was launched
 
          if (launchDoc == null)
          {
            var view = ed.GetCurrentView();
            camPos = view.Target + view.ViewDirection;
          }
 
          // Get each Solid3d in modelspace and add its extents
          // to the sorted list keyed off the distance from the
          // closest face of the solid (not necessarily true,
          // but this only really is a crude approximation)
 
          foreach (var id in ms)
          {
            ids.Add(id);
          }
          tr.Commit();
        }
 
        var sols = SolidInfoForCollection(doc, camPos, ids);
 
        return SolidsString(sols);
      }
    }

    internal static List<MeshData> SolidInfoForCollection(Document doc, Point3d camPos, ObjectIdCollection ids)
    {
        var sols = new List<MeshData>();
 
        using (var tr = doc.TransactionManager.StartOpenCloseTransaction())
        {
            foreach (ObjectId id in ids)
            {
                Entity ent = tr.GetObject(id, OpenMode.ForRead) as Entity;

                // Entity handle to name the Three.js objects
                String hand = ent.Handle.ToString();
                Autodesk.AutoCAD.Colors.EntityColor clr = ent.EntityColor;

                // Entity color to use for the Three.js objects
                long b, g, r;
                if (ent.ColorIndex < 256)
                {
                    System.Byte byt = System.Convert.ToByte(ent.ColorIndex);
                    int rgb = Autodesk.AutoCAD.Colors.EntityColor.LookUpRgb(byt);
                    b = (rgb & 0xffL);
                    g = (rgb & 0xff00L) >> 8;
                    r = rgb >> 16;
                }
                else
                {
                    b = 127;
                    g = 127;
                    r = 127;
                }
                String entColor = "0x" + String.Format("{0:X2}{1:X2}{2:X2}", r, g, b);

                // Entity extents
                Extents3d ext = ent.GeometricExtents;
                var tmp = ext.MinPoint + 0.5 * (ext.MaxPoint - ext.MinPoint);
                Vector3d dir = ext.MaxPoint - ext.MinPoint;
                var mid = new Point3d(ext.MinPoint.X, tmp.Y, tmp.Z);
                var dist = camPos.DistanceTo(mid);

                if (id.ObjectClass.Name.Equals("AcDbSubDMesh"))
                {
                    // Already a Mesh. Get the face info and clean it up
                    // a bit to export it as a THREE.Face3 which only has three vertices

                    var mesh = ent as SubDMesh;

                    Point3dCollection threeVertices = new Point3dCollection();
                    Autodesk.AutoCAD.Geometry.Int32Collection threeFaceInfo = new Autodesk.AutoCAD.Geometry.Int32Collection();

                    Point3dCollection vertices = mesh.Vertices;

                    int[] faceArr = mesh.FaceArray.ToArray();
                    int[] edgeArr = mesh.EdgeArray.ToArray();

                    Autodesk.AutoCAD.Geometry.Int32Collection faceVertices = new Autodesk.AutoCAD.Geometry.Int32Collection();

                    int verticesInFace = 0;
                    int facecnt = 0;
                    for (int x = 0; x < faceArr.Length; facecnt++, x = x + verticesInFace + 1)
                    {
                        faceVertices.Clear();

                        verticesInFace = faceArr[x];
                        for (int y = x + 1; y <= x + verticesInFace; y++)
                        {
                            faceVertices.Add(faceArr[y]);
                        }

                        // Merging of mesh faces can result in faces with multiple vertices
                        // A simple collinearity check can help remove those redundant vertices
                        bool continueCollinearCheck = false;
                        do
                        {
                            continueCollinearCheck = false;
                            for (int index = 0; index < faceVertices.Count; index++)
                            {
                                int v1 = index;
                                int v2 = (index + 1) >= faceVertices.Count ? (index + 1) - faceVertices.Count : index + 1;
                                int v3 = (index + 2) >= faceVertices.Count ? (index + 2) - faceVertices.Count : index + 2;

                                // Check collinear
                                Point3d p1 = vertices[faceVertices[v1]];
                                Point3d p2 = vertices[faceVertices[v2]];
                                Point3d p3 = vertices[faceVertices[v3]];

                                Vector3d vec1 = p1 - p2;
                                Vector3d vec2 = p2 - p3;
                                if (vec1.IsCodirectionalTo(vec2))
                                {
                                    faceVertices.RemoveAt(v2);
                                    continueCollinearCheck = true;
                                    break;
                                }
                            }
                        } while (continueCollinearCheck);

                        if (faceVertices.Count == 3)
                        {
                            Point3d p1 = vertices[faceVertices[0]];
                            Point3d p2 = vertices[faceVertices[1]];
                            Point3d p3 = vertices[faceVertices[2]];

                            if (!threeVertices.Contains(p1))
                                threeVertices.Add(p1);
                            threeFaceInfo.Add(threeVertices.IndexOf(p1));

                            if (!threeVertices.Contains(p2))
                                threeVertices.Add(p2);
                            threeFaceInfo.Add(threeVertices.IndexOf(p2));

                            if (!threeVertices.Contains(p3))
                                threeVertices.Add(p3);
                            threeFaceInfo.Add(threeVertices.IndexOf(p3));
                        }
                        else if (faceVertices.Count == 4)
                        { // A face with 4 vertices, lets split it to 
                            // make it easier later to create a THREE.Face3 
                            Point3d p1 = vertices[faceVertices[0]];
                            Point3d p2 = vertices[faceVertices[1]];
                            Point3d p3 = vertices[faceVertices[2]];

                            if (!threeVertices.Contains(p1))
                                threeVertices.Add(p1);
                            threeFaceInfo.Add(threeVertices.IndexOf(p1));

                            if (!threeVertices.Contains(p2))
                                threeVertices.Add(p2);
                            threeFaceInfo.Add(threeVertices.IndexOf(p2));

                            if (!threeVertices.Contains(p3))
                                threeVertices.Add(p3);
                            threeFaceInfo.Add(threeVertices.IndexOf(p3));

                            p1 = vertices[faceVertices[2]];
                            p2 = vertices[faceVertices[3]];
                            p3 = vertices[faceVertices[0]];

                            if (!threeVertices.Contains(p1))
                                threeVertices.Add(p1);
                            threeFaceInfo.Add(threeVertices.IndexOf(p1));

                            if (!threeVertices.Contains(p2))
                                threeVertices.Add(p2);
                            threeFaceInfo.Add(threeVertices.IndexOf(p2));

                            if (!threeVertices.Contains(p3))
                                threeVertices.Add(p3);
                            threeFaceInfo.Add(threeVertices.IndexOf(p3));
                        }
                        else
                        {
                            Application.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Face with more than 4 vertices will need triangulation to import in Three.js ");
                        }
                    }

                    sols.Add(new MeshData(dist, hand, ext, threeVertices, threeFaceInfo, entColor));
                }
                else if(id.ObjectClass.Name.Equals("AcDb3dSolid"))
                {
                    // Mesh the solid to export to Three.js
                    Autodesk.AutoCAD.DatabaseServices.Solid3d sld = tr.GetObject(id, OpenMode.ForRead) as Autodesk.AutoCAD.DatabaseServices.Solid3d;

                    MeshFaceterData mfd = new MeshFaceterData();
                    // Only triangles
                    mfd.FaceterMeshType = 2; 

                    // May need change based on how granular we want the mesh to be.
                    mfd.FaceterMaxEdgeLength = dir.Length * 0.025;

                    MeshDataCollection md = SubDMesh.GetObjectMesh(sld, mfd);

                    Point3dCollection threeVertices = new Point3dCollection();
                    Autodesk.AutoCAD.Geometry.Int32Collection threeFaceInfo = new Autodesk.AutoCAD.Geometry.Int32Collection();

                    Point3dCollection vertices = md.VertexArray;

                    int[] faceArr = md.FaceArray.ToArray();

                    Autodesk.AutoCAD.Geometry.Int32Collection faceVertices = new Autodesk.AutoCAD.Geometry.Int32Collection();

                    int verticesInFace = 0;
                    int facecnt = 0;
                    for (int x = 0; x < faceArr.Length; facecnt++, x = x + verticesInFace + 1)
                    {
                        faceVertices.Clear();

                        verticesInFace = faceArr[x];
                        for (int y = x + 1; y <= x + verticesInFace; y++)
                        {
                            faceVertices.Add(faceArr[y]);
                        }

                        if (faceVertices.Count == 3)
                        {
                            Point3d p1 = vertices[faceVertices[0]];
                            Point3d p2 = vertices[faceVertices[1]];
                            Point3d p3 = vertices[faceVertices[2]];

                            if (!threeVertices.Contains(p1))
                                threeVertices.Add(p1);
                            threeFaceInfo.Add(threeVertices.IndexOf(p1));

                            if (!threeVertices.Contains(p2))
                                threeVertices.Add(p2);
                            threeFaceInfo.Add(threeVertices.IndexOf(p2));

                            if (!threeVertices.Contains(p3))
                                threeVertices.Add(p3);
                            threeFaceInfo.Add(threeVertices.IndexOf(p3));
                        }
                    }

                    sols.Add(new MeshData(dist, hand, ext, threeVertices, threeFaceInfo, entColor));

                    tr.Commit();
                }
            }
        }
        return sols;
    }
 
    // Helper function to build a JSON string containing a
    // sorted extents list
 
    internal static string SolidsString(
      List<MeshData> lst)
    {
      var sb = new StringBuilder("{\"retCode\":0, \"result\":[");
 
      var first = true;
      foreach (var data in lst)
      {
        if (!first)
          sb.Append(",");
 
        first = false;
        var hand = data._handle;
        var ext = data._exts;
        var vertices = data._vertices;
        var faces = data._faces;
        var color = data._color;

        sb.AppendFormat(
          "{{\"min\":{0},\"max\":{1},\"handle\":\"{2}\",\"vertices\":{3},\"faces\":{4},\"color\":\"{5}\"}}",
          JsonConvert.SerializeObject(ext.MinPoint),
          JsonConvert.SerializeObject(ext.MaxPoint),
          hand,
          JsonConvert.SerializeObject(vertices),
          JsonConvert.SerializeObject(faces),
          color
        );
      }
      sb.Append("]}");
 
      return sb.ToString();
    }
 
    // Helper function to build a JSON string containing a
    // list of handles
 
    internal static string GetHandleString(ObjectIdCollection _ids)
    {
      var sb = new StringBuilder("{\"handles\":[");
      bool first = true;
      foreach (ObjectId id in _ids)
      {
        if (!first)
        {
          sb.Append(",");
        }
 
        first = false;
 
        sb.AppendFormat(
          "{{\"handle\":\"{0}\"}}",
          id.Handle.ToString()
        );
      }
      sb.Append("]}");
      return sb.ToString();
    }
 
    // Helper function to show a palette
 
    internal static PaletteSet ShowPalette(
      PaletteSet ps, Guid guid, string cmd, string title, Uri uri,
      bool reload = false
    )
    {
      // If the reload flag is true we'll force an unload/reload
      // (this isn't strictly needed - given our refresh function -
      // but I've left it in for possible future use)
 
      if (reload && ps != null)
      {
        // Close the palette and make sure we process windows
        // messages, otherwise sizing is a problem
 
        ps.Close();
        System.Windows.Forms.Application.DoEvents();
        ps.Dispose();
        ps = null;
      }
 
      if (ps == null)
      {
        ps = new PaletteSet(cmd, guid);
      }
      else
      {
        if (ps.Visible)
          return ps;
      }
 
      if (ps.Count != 0)
      {
        ps.Remove(0);
      }
 
      ps.Add(title, uri);
      ps.Visible = true;
 
      return ps;
    }
 
    internal static Matrix3d Dcs2Wcs(AbstractViewTableRecord v)
    {
      return
        Matrix3d.Rotation(-v.ViewTwist, v.ViewDirection, v.Target) *
        Matrix3d.Displacement(v.Target - Point3d.Origin) *
        Matrix3d.PlaneToWorld(v.ViewDirection);
    }
 
    internal static Extents3d ScreenExtents(
      AbstractViewTableRecord vtr
    )
    {
      // Get the centre of the screen in WCS and use it
      // with the diagonal vector to add the corners to the
      // extents object
 
      var ext = new Extents3d();
      var vec = new Vector3d(0.5 * vtr.Width, 0.5 * vtr.Height, 0);
      var ctr =
        new Point3d(vtr.CenterPoint.X, vtr.CenterPoint.Y, 0);
      var dcs = Utils.Dcs2Wcs(vtr);
      ext.AddPoint((ctr + vec).TransformBy(dcs));
      ext.AddPoint((ctr - vec).TransformBy(dcs));
 
      return ext;
    }
 
    // Helper function to get the path to our HTML files
 
    internal static string GetHtmlPath()
    {
      // Use this approach if loading the HTML from the same
      // location as your .NET module
 
      //var asm = Assembly.GetExecutingAssembly();
      //return Path.GetDirectoryName(asm.Location) + "\\";

        return @"D:\KB_P\ThreeIntegration - Part1\MyTest1_Net\files\";
    }
  }
}
