using System;
using System.IO;
using System.Text;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using Autodesk.AutoCAD.Runtime;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Geometry;
using Autodesk.AutoCAD.EditorInput;
using acApp = Autodesk.AutoCAD.ApplicationServices.Core.Application;

// To-Do : Take care of loading performances
namespace DwgTextExtracter
{
    internal enum MessageType : int
    {
        Information,
        Error
    }

    // AutoCAD database tables
    internal enum TableType : int
    {
        BlockTable,
        LayerTable,
        DimStyleTable
    }

    internal enum RecordType : int
    {
        BlockTableRecord,
        LayerTableRecord,
        DimStyleTableRecord
    }

    internal class Logger
    {
        public static void WriteLog(string msg, MessageType type)
        {
            Editor ed = acApp.DocumentManager.MdiActiveDocument.Editor;
            
            switch(type)
                {
                    case MessageType.Information :
                        ed.WriteMessage("\nInformation : " +  msg);
                    break;

                    case MessageType.Error:
                        ed.WriteMessage("\nError : " + msg);
                    break;
                }
            
        }
    }

    /* This class does not depends on AutoCAD Runtime and uses only core autocad logic
        and so can be */
    public class TextExtracter
    {   
        #region Extract all object data to xml
        //Previous code, extracting only MText contents from dwg file
        //const string _sourcePath =
        //  @"C:\Users\amitp\Downloads\3D Files\dwg\Sample DWG file\";
        //const string _destPath =
        //  @"C:\Users\amitp\Downloads\3D Files\dwg\Sample DWG file\";
        //const string _inputFileName =
        //  "001. Laterial Support For Non-Load Bearing Partition.dwg";
        //const string _outputFileName = 
        //  "extracted_text_from_dwg.txt";

        //[CommandMethod("extd")]
        //public void extractAllDataToXml()   //dumps all dwg contents to xml file
        //{
        //    if (!System.IO.File.Exists(_sourcePath + _inputFileName))
        //    {
        //        Document doc = acApp.DocumentManager.MdiActiveDocument;
        //        Editor ed = doc.Editor;
        //        ed.WriteMessage("\nFile does not exist.");
        //        return;
        //    }

        //    // Create some settings for the extraction

        //    IDxExtractionSettings es = new DxExtractionSettings();

        //    IDxDrawingDataExtractor de = es.DrawingDataExtractor;

        //    de.Settings.ExtractFlags = ExtractFlags.ModelSpaceOnly | ExtractFlags.XrefDependent | ExtractFlags.Nested;

        //    // Add a single file to the settings

        //    IDxFileReference fr = new DxFileReference(_sourcePath, _sourcePath + _inputFileName);

        //    de.Settings.DrawingList.AddFile(fr);

        //    // Scan the drawing for object types & their properties

        //    de.DiscoverTypesAndProperties(_sourcePath);

        //    List<IDxTypeDescriptor> types = de.DiscoveredTypesAndProperties;

        //    // Select all the types and properties for extraction
        //    // by adding them one-by-one to these two lists

        //    List<string> selTypes = new List<string>();
        //    List<string> selProps = new List<string>();

        //    foreach (IDxTypeDescriptor type in types)
        //    {
        //        selTypes.Add(type.GlobalName);

        //        foreach (IDxPropertyDescriptor pr in type.Properties)
        //        {
        //            if (!selProps.Contains(pr.GlobalName))
        //                selProps.Add(pr.GlobalName);
        //        }
        //    }

        //    // Pass this information to the extractor

        //    de.Settings.SetSelectedTypesAndProperties(
        //      types,
        //      selTypes,
        //      selProps
        //    );

        //    // Now perform the extraction itself

        //    de.ExtractData(_sourcePath);

        //    // Get the results of the extraction

        //    System.Data.DataTable dataTable = de.ExtractedData;

        //    // Output the extracted data to an XML file

        //    if (dataTable.Rows.Count > 0)
        //    {
        //        dataTable.TableName = "My_Data_Extract";
        //        dataTable.WriteXml(_destPath + _outputFileName);
        //    }
        //}

        #endregion

        #region previous code to extract MText only
        //[CommandMethod("extt")]
        //public void getMTextObj()           //dumps all text contents to text file
        //{                                   //giving error for files not containing text --check
        //    string contents = "";

        //    try
        //    {
        //        using (Database db = HostApplicationServices.WorkingDatabase)
        //        {
        //            Editor ed = acApp.DocumentManager.MdiActiveDocument.Editor;
        //            TypedValue[] filterlist = new TypedValue[1];

        //            ed.WriteMessage("\nEXTT : Attempting to extract text...\n");

        //            //select Mtext entities
        //            filterlist[0] = new TypedValue(0, "MTEXT");

        //            SelectionFilter filter = new SelectionFilter(filterlist);

        //            //we can add custom data to be extracted here
        //            PromptSelectionResult selRes = ed.SelectAll(filter);

        //            if (selRes.Status != PromptStatus.OK)
        //            {
        //                ed.WriteMessage("\nEXTT : Error during text extraction...\n");
        //                return;
        //            }

        //            ObjectId[] MTextObjIds = selRes.Value.GetObjectIds();

        //            if (!(MTextObjIds == null))
        //            {
        //                if (MTextObjIds.Length > 0)
        //                {
        //                    using (var Transaction = acApp.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
        //                    {
        //                        MText MTextObj;

        //                        foreach (ObjectId MTextObjId in MTextObjIds)
        //                        {
        //                            MTextObj = Transaction.GetObject(MTextObjId, OpenMode.ForRead) as MText;

        //                            if (MTextObj != null)
        //                            {
        //                                contents += MTextObj.Text + "\n";
        //                            }
        //                        }

        //                        if (!String.IsNullOrEmpty(contents))
        //                        {
        //                            File.WriteAllText(Path.ChangeExtension(db.Filename, "txt"), contents);
        //                        }
        //                    }
        //                    //Transaction.Commit(); // no changes so no need to commit
        //                }
        //            }
        //            ed.WriteMessage("\nEXTT : Command executed successfully...\n");
        //        }
        //    }
        //    catch (System.Exception ex)
        //    {
        //          Logger.WriteLog("EXTT : unexpected Error occured : " + ex.Message + "\n" + ex.StackTrace, MessageType.Error);
        //    }
        //}
        #endregion

        #region Under developement, extarct MText + DBText + Attibutereferences

        [CommandMethod("extt")]
        public void getMTextObj()           //dumps all text contents to text file
        {                                   //giving error for files not containing text --check
            string contents = "";

            try
            {
                using (Database db = HostApplicationServices.WorkingDatabase)
                {
                    Editor ed = acApp.DocumentManager.MdiActiveDocument.Editor;
                    TypedValue[] filterlist = new TypedValue[1];

                    Logger.WriteLog("COMMAND - EXTT : Attempting to extract text...", MessageType.Information);

                    //select Mtext entities
                    //filterlist[0] = new TypedValue((int)DxfCode.LayerName, "View Labels");  //Test
                    filterlist[0] = new TypedValue((int)DxfCode.Start, "MTEXT");  
                    SelectionFilter filter = new SelectionFilter(filterlist);

                    //we can add custom data to be extracted here
                    PromptSelectionResult selRes = ed.SelectAll(filter);

                    if (selRes.Status != PromptStatus.OK)
                    {
                        Logger.WriteLog("COMMAND - EXTT : Error during text extraction...", MessageType.Error);
                        //return;       //test
                    }

                    //ObjectId[] MTextObjIds = selRes.Value.GetObjectIds();

                    //if (!(MTextObjIds == null))       //test
                    {
                        //if (MTextObjIds.Length > 0)   //test
                        {
                            using (Transaction tr = acApp.DocumentManager.MdiActiveDocument.Database.TransactionManager.StartTransaction())
                            {                              
#if !_TEST
                                LayerTable lt = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);

                                foreach(ObjectId id in lt)
                                    {
                                        LayerTableRecord ltr = (LayerTableRecord)tr.GetObject(id, OpenMode.ForRead);

                                        //if (ltr.Name.Equals("0")) // iterate all layers
                                        {
                                            contents += ltr.Name + "\t\n";
                                            ObjectId[] ids = GetTextFromLayerOrBlock(tr, ltr.Name, true);

                                            if (ids == null)
                                            {
                                                contents += "NULL\n";
                                            }
                                            else
                                            {
                                                for (int i = 0; i < ids.Length; ++i)
                                                {
                                                    DBObject obj = tr.GetObject(ids[i], OpenMode.ForRead);

                                                    contents += "\n Layer Object Details : " + obj.GetType().ToString() + " " + obj.GetRXClass().DxfName + "\n";

                                                    switch (obj.GetRXClass().DxfName)
                                                    {
                                                        //Autodesk.AutoCAD.DatabaseServices.Table
                                                        case "ACAD_TABLE" :

                                                            Table tb = obj as Table;

                                                            for (int row = 0; row < tb.Rows.Count; ++row)
                                                            {
                                                                for (int col = 0; col < tb.Rows.Count; ++col)
                                                                {
                                                                    var cell = tb.Cells[row, col];
                                                                    contents += cell.Value + "\t";  //formatting lost
                                                                    //contents += cell.GetValue(FormatOption.FormatOptionNone) + "\t";
                                                                }
                                                            }

                                                        break;

                                                        //Autodesk.AutoCAD.DatabaseServices.MText
                                                        case "MTEXT" :
                                                            MText mtext = obj as MText;

                                                            if (mtext != null)
                                                            {
                                                                //if(mtext.Text != String.Empty)
                                                                    contents += "MTEXT : " + mtext.Text + "\n";     
                                                            }
                                                        break;

                                                        //Autodesk.AutoCAD.DatabaseServices.DBText
                                                        case "TEXT" :
                                                            DBText dbtext = obj as DBText;

                                                            if (dbtext != null)
                                                            {
                                                                //if(dbtext.TextString != string.Empty)
                                                                    contents += "DBTEXT : " + dbtext.TextString + "\n";
                                                            }
                                                        break;

                                                        //Autodesk.AutoCAD.DatabaseServices.BlockReference
                                                        case "INSERT" :
                                                            BlockReference br = obj as BlockReference;

                                                            foreach (ObjectId attrId in br.AttributeCollection)
                                                            {
                                                                AttributeReference attRef = (AttributeReference)tr.GetObject(attrId, OpenMode.ForRead);
                                                                
                                                                string str = ("\n  Attribute Tag: " + attRef.Tag + "\n    Attribute String: " + attRef.TextString + "\n");
                                                                contents += str;
                                                            }
                                                            
                                                        break;
                                                    }

                    
                                                    
                                                }
                                            }
                                            //break;
                                        }
                                        
                                    }

                                    //MText MTextObj;

                                    //foreach (ObjectId MTextObjId in MTextObjIds)
                                    //{
                                    //    MTextObj = tr.GetObject(MTextObjId, OpenMode.ForRead) as MText;

                                    //    if (MTextObj != null)
                                    //    {
                                    //        contents += MTextObj.Text + "\n";
                                    //    }
                                    //}

                                    
                                    
                                    if (!String.IsNullOrEmpty(contents))
                                    {
                                        File.WriteAllText(Path.ChangeExtension(db.Filename, "txt"), contents);
                                    }
                                }
                                //tr.Commit(); // no changes so no need to commit

#else                               
                                // Open the blocktable, get the modelspace
                                //BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                                //BlockTableRecord btrModel = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.ModelSpace], OpenMode.ForRead);
                                
                                //BlockTableRecord btrPaper = (BlockTableRecord)tr.GetObject(bt[BlockTableRecord.PaperSpace], OpenMode.ForRead);

                                // Iterate through it, dumping objects

                                // Iterate through it, dumping objects
                                //foreach (ObjectId objId in btr)
                                //{
                                //    Entity ent = (Entity)tr.GetObject(objId, OpenMode.ForRead);
                                //    string typeString = ent.GetType().ToString();
                                  
                                //    //if(ent.GetType() == Type.GetType("MText"))
                                //    if(typeString.Equals("MTEXT"))
                                //    {
                                //        MText txt = ent as MText;
                                //        contents += txt.Contents;
                                //    }
                                //    else if (ent.GetType() == Type.GetType("DBText"))
                                //    {
                                //        DBText txt = ent as DBText;
                                //        contents += txt.TextString;
                                //    }

                                //}
                                //contents += "*****************MODEL SPACE TEXT***********************\n";

                                //foreach (ObjectId id in btrModel)
                                //{
                                //    DBObject obj = tr.GetObject(id, OpenMode.ForRead) as MText;

                                //    MText mtext = obj as MText;

                                //    if (mtext != null)
                                //    {
                                //        contents += "\nMTEXT-->" + mtext.Text + "\n";
                                //        continue;
                                //    }

                                //    DBText dbtext = obj as DBText;

                                //    if (dbtext != null)
                                //    {
                                //        contents += "\nDBTEXT-->" + dbtext.TextString + "\n";
                                //    }
                                //}

                                //contents += "*****************PAPER SPACE TEXT***********************\n";

                                //foreach (ObjectId id in btrPaper)
                                //{
                                //    DBObject obj = tr.GetObject(id, OpenMode.ForRead) as MText;

                                //    MText mtext = obj as MText;
                                    
                                //    if (mtext != null)
                                //    {
                                //        contents += "\nMTEXT-->" + mtext.Text + "\n";
                                //        continue;
                                //    }

                                //    DBText dbtext = obj as DBText;

                                //    if (dbtext != null)
                                //    {
                                //        contents += "\nDBTEXT-->" + dbtext.TextString + "\n";
                                //    }
                                //}


                                IterateBlockTable(db, tr, out contents);

                                //IterateAutoCADSymbolTables(db, tr, out contents, TableType.BlockTable, RecordType.BlockTableRecord);

                                if (!String.IsNullOrEmpty(contents))
                                {
                                    File.WriteAllText(Path.ChangeExtension(db.Filename, "txt"), contents);
                                }
                            }

#endif
                        }
                    }

                    Logger.WriteLog("COMMAND - EXTT : Command executed successfully...", MessageType.Information);

                }
            }
            catch (System.Exception ex)
            {
                Logger.WriteLog("COMMAND - EXTT : Error occured : " +  ex.Message + "\n" + ex.StackTrace, MessageType.Error);
            }
        }

        #endregion

        private ObjectId[] GetTextFromLayerOrBlock(Transaction tr, string layerOrBlockName, bool isLayer)
        {
            //textOnLayer = "";
            ObjectId[] ids = null;

            TypedValue[] val = new TypedValue[1];

            val[0] = new TypedValue(isLayer ? (int)DxfCode.LayerName : (int)DxfCode.BlockName, layerOrBlockName);
            
            Editor ed = acApp.DocumentManager.MdiActiveDocument.Editor;

            PromptSelectionResult result = ed.SelectAll(new SelectionFilter(val));

            if (result.Status == PromptStatus.OK)
            {
                ids = result.Value.GetObjectIds();
            }
            else
            {
                Logger.WriteLog("COMMAND - EXTT : Unexpected error occured while retrieving text from layer/block : " + layerOrBlockName, MessageType.Error);
            }

            return ids;
        }

        private void IterateBlockTable(Database db, Transaction tr, out string contents)
        {
            contents = "";

            Logger.WriteLog("Inside IterateBlockTable()...", MessageType.Information);

            if (db != null && tr != null)
            {
                BlockTable bt = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);

                Logger.WriteLog("Got BlockTable...", MessageType.Information);
                
                foreach (ObjectId blockid in bt)
                {
                    BlockTableRecord btr = (BlockTableRecord)tr.GetObject(blockid, OpenMode.ForRead);

                    contents += "\n BLOCK NAME : " + btr.Name + "\n";
                    ObjectId[] ids = GetTextFromLayerOrBlock(tr, btr.Name, false);

                    if (ids == null)
                    {
                        contents += "BLOCK CONTAINS NO OBJECTS\n";
                    }
                    else
                    {
                        for (int i = 0; i < ids.Length; ++i)
                        {
                            DBObject obj = (DBObject)tr.GetObject(ids[i], OpenMode.ForRead);
                            contents += "\n Block Object Details : " + obj.GetRXClass().DxfName + "\n";

                            switch (obj.GetRXClass().DxfName)
                            {
                                //Autodesk.AutoCAD.DatabaseServices.Table
                                case "ACAD_TABLE":

                                    Table tb = obj as Table;

                                    for (int row = 0; row < tb.Rows.Count; ++row)
                                    {
                                        for (int col = 0; col < tb.Rows.Count; ++col)
                                        {
                                            var cell = tb.Cells[row, col];
                                            contents += cell.Value + "\t";  //formatting lost
                                            //contents += cell.GetValue(FormatOption.FormatOptionNone) + "\t";
                                        }
                                    }

                                    break;

                                //Autodesk.AutoCAD.DatabaseServices.MText
                                case "MTEXT":
                                    MText mtext = obj as MText;

                                    if (mtext != null)
                                        contents += "MTEXT : " + mtext.Text + "\n";
                                    break;

                                //Autodesk.AutoCAD.DatabaseServices.DBText
                                case "TEXT":
                                    DBText dbtext = obj as DBText;

                                    if (dbtext != null)
                                        contents += "DBTEXT : " + dbtext.TextString + "\n";
                                    break;

                                //Autodesk.AutoCAD.DatabaseServices.BlockReference
                                case "INSERT":
                                    BlockReference br = obj as BlockReference;

                                    foreach (ObjectId attrId in br.AttributeCollection)
                                    {
                                        AttributeReference attRef = (AttributeReference)tr.GetObject(attrId, OpenMode.ForRead);

                                        string str = ("\n  Attribute Tag: " + attRef.Tag + "\n    Attribute String: " + attRef.TextString + "\n" + "MText Attribute: " + attRef.MTextAttribute.Text);
                                        contents += str;
                                    }

                                    break;
                            }
                        }
                    }
                }

            }

        }

        private SymbolTable GetSymbolTable(Database db, Transaction tr, TableType type)
        {
            SymbolTable st = null;

            switch (type)
            {
                case TableType.BlockTable:
                    st = (BlockTable)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                break;

                case TableType.LayerTable:
                    st = (LayerTable)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                break;

                case TableType.DimStyleTable:
                    st = (DimStyleTable)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                break;
            }

            return st;
        }

        private SymbolTableRecord GetSymbolTableRecord(Database db, Transaction tr, RecordType type)
        {
            SymbolTableRecord st = null;

            switch (type)
            {
                case RecordType.BlockTableRecord:
                    st = (BlockTableRecord)tr.GetObject(db.BlockTableId, OpenMode.ForRead);
                    break;

                case RecordType.LayerTableRecord:
                    st = (LayerTableRecord)tr.GetObject(db.LayerTableId, OpenMode.ForRead);
                    break;

                case RecordType.DimStyleTableRecord:
                    st = (DimStyleTableRecord)tr.GetObject(db.DimStyleTableId, OpenMode.ForRead);
                    break;
            }

            return st;
        }

        //Iterate BlockTables, LayerTables, DimStyleTables
        private void IterateAutoCADSymbolTables(Database db, Transaction tr, out string contents, TableType type, RecordType rtype)
        {
            contents = "";

            Logger.WriteLog("Inside IterateSymbolTable()...", MessageType.Information);

            if (db != null && tr != null)
            {
                SymbolTable bt = GetSymbolTable(db, tr, type);

                Logger.WriteLog("Got SymbolTable...", MessageType.Information);

                try
                {

                    foreach (ObjectId blockid in bt)
                    {
                        SymbolTableRecord btr = GetSymbolTableRecord(db, tr, rtype);

                        contents += "\n" + btr.GetRXClass().DxfName + " NAME : " + btr.Name + "\n";
                        ObjectId[] ids = GetTextFromLayerOrBlock(tr, btr.Name, false);

                        if (ids == null)
                        {
                            contents += "BLOCK CONTAINS NO OBJECTS\n";
                        }
                        else
                        {
                            for (int i = 0; i < ids.Length; ++i)
                            {
                                DBObject obj = (DBObject)tr.GetObject(ids[i], OpenMode.ForRead);
                                contents += "\n Object Details : " + obj.GetRXClass().DxfName + "\n";

                                switch (obj.GetRXClass().DxfName)
                                {
                                    //Autodesk.AutoCAD.DatabaseServices.Table
                                    case "ACAD_TABLE":

                                        Table tb = obj as Table;

                                        if (tb != null)
                                        {

                                            for (int row = 0; row < tb.Rows.Count; ++row)
                                            {
                                                for (int col = 0; col < tb.Rows.Count; ++col)
                                                {
                                                    var cell = tb.Cells[row, col];
                                                    contents += cell.Value + "\t";  //formatting lost
                                                    //contents += cell.GetValue(FormatOption.FormatOptionNone) + "\t";
                                                }
                                            }
                                        }

                                        break;

                                    //Autodesk.AutoCAD.DatabaseServices.MText
                                    case "MTEXT":
                                        MText mtext = obj as MText;

                                        if (mtext != null)
                                            contents += "MTEXT : " + mtext.Text + "\n";
                                        break;

                                    //Autodesk.AutoCAD.DatabaseServices.DBText
                                    case "TEXT":
                                        DBText dbtext = obj as DBText;

                                        if (dbtext != null)
                                            contents += "DBTEXT : " + dbtext.TextString + "\n";
                                        break;

                                    //Autodesk.AutoCAD.DatabaseServices.BlockReference
                                    case "INSERT":
                                        BlockReference br = obj as BlockReference;

                                        if (br != null)
                                        {
                                            foreach (ObjectId attrId in br.AttributeCollection)
                                            {
                                                AttributeReference attRef = (AttributeReference)tr.GetObject(attrId, OpenMode.ForRead);

                                                string str = ("\n  Attribute Tag: " + attRef.Tag + "\n    Attribute String: " + attRef.TextString + "\n" + "MText Attribute: " + attRef.MTextAttribute.Text);
                                                contents += str;
                                            }
                                        }

                                        break;
                                }
                            }
                        }
                    }
                }
                catch (System.Exception ex)
                {
                    Logger.WriteLog("COMMAND - EXTT : " + ex.Message + "\n" + ex.StackTrace, MessageType.Error);
                }

            }


        }
    }
}

