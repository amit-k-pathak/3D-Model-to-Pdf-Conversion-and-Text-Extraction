using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.Runtime;
using Newtonsoft.Json;
using Autodesk.AutoCAD.Windows;
using System.IO;
using System.Windows;


namespace JsonExporter
{
    public class Converter
    {
            private Autodesk.AutoCAD.Windows.PaletteSet _3ps = null;

            private static Document _launchDoc = null;

            [JavaScriptCallback("GetSolids")]
            public string GetSolids(string jsonArgs)
            {
                var doc = GetActiveDocument(Application.DocumentManager);

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

                    // Capture our Extents3d objects in a list

                    var sols = new List<Extents3d>();

                    using (var tr = doc.TransactionManager.StartTransaction())
                    {
                        // Start by getting the modelspace

                        var ms =
                          (BlockTableRecord)tr.GetObject(
                            SymbolUtilityServices.GetBlockModelSpaceId(db),
                            OpenMode.ForRead
                          );

                        // Get each Solid3d in modelspace and add its extents
                        // to the list

                        foreach (var id in ms)
                        {
                            var obj = tr.GetObject(id, OpenMode.ForRead);
                            var sol = obj as Solid3d;
                            if (sol != null)
                            {
                                sols.Add(sol.GeometricExtents);
                            }
                        }
                        tr.Commit();
                    }
                    return GetSolidsString(sols);
                }
            }

            private Document GetActiveDocument(DocumentCollection dm)
            {
                // If we're called from an HTML document, the active
                // document may be null

                var doc = dm.MdiActiveDocument;
                if (doc == null)
                {
                    doc = _launchDoc;
                }
                return doc;
            }

            // Helper function to build a JSON string containing our
            // sorted extents list

            private string GetSolidsString(List<Extents3d> lst)
            {
                var sb = new StringBuilder("{\"retCode\":0, \"result\":[");

                bool first = true;
                foreach (var ext in lst)
                {
                    if (first)
                        first = false;
                    else
                        sb.Append(",");

                    sb.Append(
                      string.Format(
                        "{{\"min\":{0},\"max\":{1}}}",
                        JsonConvert.SerializeObject(ext.MinPoint),
                        JsonConvert.SerializeObject(ext.MaxPoint)
                      )
                    );
                }
                sb.Append("]}");

                File.WriteAllText(@"D:\3dsolids.json", sb.ToString());     //test

                return sb.ToString();
            }

            [CommandMethod("THREEDOC")]
            public static void ThreeDocument()
            {
                _launchDoc = Application.DocumentManager.MdiActiveDocument;
                _launchDoc.BeginDocumentClose +=
                  (s, e) => { _launchDoc = null; };

                Application.DocumentWindowCollection.AddDocumentWindow("Three.js Document", GetHtmlPathThree()
                );
            }

            [CommandMethod("THREE")]
            public void ThreePalette()
            {
                _launchDoc = null;

                _3ps =
                  ShowPalette(
                    _3ps,
                    new Guid("9CEE43FF-FDD7-406A-89B2-6A48D4169F71"),
                    "THREE",
                    "Three.js Examples",
                    GetHtmlPathThree()
                  );
            }

            // Helper function to show a palette

            private PaletteSet ShowPalette(PaletteSet ps, Guid guid, string cmd, string title, Uri uri)
            {
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
                    ps[0].PaletteSet.Remove(0);
                }

                ps.Add(title, uri);
                ps.Visible = true;

                return ps;
            }

            // Helper function to get the path to our HTML files

            private static string GetHtmlPath()
            {
                // Use this approach if loading the HTML from the same
                // location as your .NET module

                //var asm = Assembly.GetExecutingAssembly();
                //return Path.GetDirectoryName(asm.Location) + "\\";

                return "http://through-the-interface.typepad.com/files/";
            }

            private static Uri GetHtmlPathThree()
            {
                return new Uri(GetHtmlPath() + "threesolids.html");
            }
    }
}
