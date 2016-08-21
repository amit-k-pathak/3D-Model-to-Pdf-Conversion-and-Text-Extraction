using System;
using System.Collections.Generic;
using Autodesk.AutoCAD.ApplicationServices;
using Autodesk.AutoCAD.EditorInput;
using Autodesk.AutoCAD.Runtime;
using System.IO;
using System.Collections;
using Autodesk.AutoCAD.PlottingServices;
using Autodesk.AutoCAD.DatabaseServices;
using Autodesk.AutoCAD.Publishing;
using System.Text;
using app = Autodesk.AutoCAD.ApplicationServices.Core.Application;
using Autodesk.AutoCAD.ApplicationServices.Core;



namespace DwgToPdfConverter
{
        /* This class depends on AutoCAD Runtime
        //public class MultiSheetsPdf
        //{
        //    private string dwgFile, pdfFile, dsdFile, outputDir;
        //    private int sheetNum;
        //    IEnumerable<Layout> layouts;

        //    private const string LOG = "publish.log";

        //    public MultiSheetsPdf(string pdfFile, IEnumerable<Layout> layouts)
        //    {
        //        Database db = HostApplicationServices.WorkingDatabase;
        //        this.dwgFile = db.Filename;
        //        this.pdfFile = pdfFile;
        //        this.outputDir = Path.GetDirectoryName(this.pdfFile);
        //        this.dsdFile = Path.ChangeExtension(this.pdfFile, "dsd");
        //        this.layouts = layouts;
        //    }

        //    public void Publish()
        //    {
        //        if (TryCreateDSD())
        //        {
        //            Publisher publisher = Autodesk.AutoCAD.ApplicationServices.Application.Publisher;
        //            PlotProgressDialog plotDlg = new PlotProgressDialog(false, this.sheetNum, true);
        //            publisher.PublishDsd(this.dsdFile, plotDlg);
        //            plotDlg.Destroy();
        //            File.Delete(this.dsdFile);
        //        }
        //    }

        //    private bool TryCreateDSD()
        //    {
        //        using (DsdData dsd = new DsdData())
        //        using (DsdEntryCollection dsdEntries = CreateDsdEntryCollection(this.layouts))
        //        {
        //            if (dsdEntries == null || dsdEntries.Count <= 0) return false;

        //            if (!Directory.Exists(this.outputDir))
        //                Directory.CreateDirectory(this.outputDir);

        //            this.sheetNum = dsdEntries.Count;

        //            dsd.SetDsdEntryCollection(dsdEntries);

        //            dsd.SetUnrecognizedData("PwdProtectPublishedDWF", "FALSE");
        //            dsd.SetUnrecognizedData("PromptForPwd", "FALSE");
        //            dsd.SheetType = SheetType.MultiDwf;
        //            dsd.NoOfCopies = 1;
        //            dsd.DestinationName = this.pdfFile;
        //            dsd.IsHomogeneous = false;
        //            dsd.LogFilePath = Path.Combine(this.outputDir, LOG);

        //            PostProcessDSD(dsd);

        //            return true;
        //        }
        //    }

        //    private DsdEntryCollection CreateDsdEntryCollection(IEnumerable<Layout> layouts)
        //    {
        //        DsdEntryCollection entries = new DsdEntryCollection();

        //        foreach (Layout layout in layouts)
        //        {
        //            DsdEntry dsdEntry = new DsdEntry();
        //            dsdEntry.DwgName = this.dwgFile;
        //            dsdEntry.Layout = layout.LayoutName;
        //            dsdEntry.Title = Path.GetFileNameWithoutExtension(this.dwgFile) + "-" + layout.LayoutName;
        //            dsdEntry.Nps = layout.TabOrder.ToString();
        //            entries.Add(dsdEntry);
        //        }
        //        return entries;
        //    }

        //    private void PostProcessDSD(DsdData dsd)
        //    {
        //        string str, newStr;
        //        string tmpFile = Path.Combine(this.outputDir, "temp.dsd");

        //        dsd.WriteDsd(tmpFile);

        //        using (StreamReader reader = new StreamReader(tmpFile, Encoding.Default))
        //        using (StreamWriter writer = new StreamWriter(this.dsdFile, false, Encoding.Default))
        //        {
        //            while (!reader.EndOfStream)
        //            {
        //                str = reader.ReadLine();
        //                if (str.Contains("Has3DDWF"))
        //                {
        //                    newStr = "Has3DDWF=0";
        //                }
        //                else if (str.Contains("OriginalSheetPath"))
        //                {
        //                    newStr = "OriginalSheetPath=" + this.dwgFile;
        //                }
        //                else if (str.Contains("Type"))
        //                {
        //                    newStr = "Type=6";
        //                }
        //                else if (str.Contains("OUT"))
        //                {
        //                    newStr = "OUT=" + this.outputDir;
        //                }
        //                else if (str.Contains("IncludeLayer"))
        //                {
        //                    newStr = "IncludeLayer=TRUE";
        //                }
        //                else if (str.Contains("PromptForDwfName"))
        //                {
        //                    newStr = "PromptForDwfName=FALSE";
        //                }
        //                else if (str.Contains("LogFilePath"))
        //                {
        //                    newStr = "LogFilePath=" + Path.Combine(this.outputDir, LOG);
        //                }
        //                else
        //                {
        //                    newStr = str;
        //                }
        //                writer.WriteLine(newStr);
        //            }
        //        }
        //        File.Delete(tmpFile);
        //    }

        //    [CommandMethod("PlotPdf")]
        //    public static void PlotPdf()
        //    {
        //        Database db = HostApplicationServices.WorkingDatabase;
        //        short bgp = (short)app.GetSystemVariable("BACKGROUNDPLOT");
        //        try
        //        {
        //            app.SetSystemVariable("BACKGROUNDPLOT", 0);
        //            using (Transaction tr = db.TransactionManager.StartTransaction())
        //            {
        //                List<Layout> layouts = new List<Layout>();
        //                DBDictionary layoutDict = (DBDictionary)db.LayoutDictionaryId.GetObject(OpenMode.ForRead);
                        
        //                foreach (DBDictionaryEntry entry in layoutDict)
        //                {
        //                    //if (entry.Key != "Model") //uncomment to skip model
        //                    {
        //                        layouts.Add((Layout)tr.GetObject(entry.Value, OpenMode.ForRead));
        //                    }
        //                }
        //                layouts.Sort((l1, l2) => l1.TabOrder.CompareTo(l2.TabOrder));

        //                string filename = Path.ChangeExtension(db.Filename, "pdf");

        //                MultiSheetsPdf plotter = new MultiSheetsPdf(filename, layouts);
        //                plotter.Publish();

        //                tr.Commit();
        //            }
        //        }
        //        catch (System.Exception e)
        //        {
        //            Editor ed = app.DocumentManager.MdiActiveDocument.Editor;
        //            ed.WriteMessage("\nError: {0}\n{1}", e.Message, e.StackTrace);
        //        }
        //        finally
        //        {
        //            app.SetSystemVariable("BACKGROUNDPLOT", bgp);
        //        }
        //    }

        //}

        */

        /* This class does not depends on AutoCAD Runtime and uses only core autocad logic*/
        public class MultiSheetsPdfUsingAccoreconsole
        {
            private string _dwgFile, _pdfFile, _dsdFile, _outputDir;
            private int _numberOfsheets;
            IEnumerable<Layout> _layouts;

            private const string _logFile = "publish.log";

            public MultiSheetsPdfUsingAccoreconsole(string pdfFile, IEnumerable<Layout> layouts)
            {
                this._dwgFile = HostApplicationServices.WorkingDatabase.Filename;
                this._pdfFile = pdfFile;
                this._outputDir = Path.GetDirectoryName(this._pdfFile);
                this._dsdFile = Path.ChangeExtension(this._pdfFile, "dsd");
                this._layouts = layouts;
            }

            public void Publish()
            {
                if (TryCreateDSD())
                {
                    //Publisher publisher = Autodesk.AutoCAD.ApplicationServices.ApplicationServices.Application.Publisher;
                    Publisher publisher = Autodesk.AutoCAD.ApplicationServices.Core.Application.Publisher;
                    //publisher.PublishExecute(); --for printing with plotconfigs

                    // Attach necessary event handlers
                    //publisher.BeginPublishingSheet += new BeginPublishingSheetEventHandler(HandleBeginPublishingSheet);
                    //publisher.EndPublish += new EndPublishEventHandler(HandleEndPublish);
                    //publisher.CancelledOrFailedPublishing += new CancelledOrFailedPublishingEventHandler(HandleCancelledOrFailedPublish);

                    PlotProgressDialog plotDlg = new PlotProgressDialog(false, this._numberOfsheets, true);
                    //File.WriteAllText("D:\\sheets.txt", this.sheetNum.ToString());     //test
                    
                    publisher.PublishDsd(this._dsdFile, plotDlg);
                    plotDlg.Destroy();
                    File.Delete(this._dsdFile);
                }
            }

            //private void HandleCancelledOrFailedPublish(object sender, PublishEventArgs e)
            //{
            //    app.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Error : publishing failed/cancelled...", e.ToString());
            //}

            //private void HandleEndPublish(object sender, PublishEventArgs e)
            //{
            //    app.DocumentManager.MdiActiveDocument.Editor.WriteMessage("All publishing tasks finished...", e.ToString());
            //}

            //private void HandleBeginPublishingSheet(object sender, BeginPublishingSheetEventArgs e)
            //{
            //    app.DocumentManager.MdiActiveDocument.Editor.WriteMessage("Error : while publishing...", e.ToString());
            //}

            private bool TryCreateDSD()
            {
                using (DsdData dsd = new DsdData())
                using (DsdEntryCollection dsdEntries = CreateDsdEntryCollection(this._layouts))
                {
                    if (dsdEntries == null || dsdEntries.Count <= 0) return false;

                    if (!Directory.Exists(this._outputDir))
                        Directory.CreateDirectory(this._outputDir);

                    this._numberOfsheets = dsdEntries.Count;

                    dsd.SetDsdEntryCollection(dsdEntries);

                    dsd.SetUnrecognizedData("PwdProtectPublishedDWF", "FALSE");
                    dsd.SetUnrecognizedData("PromptForPwd", "FALSE");
                    //dsd.SheetType = SheetType.MultiDwf;
                    dsd.SheetType = SheetType.MultiPdf; //previously MultiDwf -- check
                    dsd.NoOfCopies = 1;
                    dsd.DestinationName = this._pdfFile;
                    dsd.IsHomogeneous = false;
                    dsd.LogFilePath = Path.Combine(this._outputDir, _logFile);

                    PostProcessDSD(dsd);

                    return true;
                }
            }

            private DsdEntryCollection CreateDsdEntryCollection(IEnumerable<Layout> layouts)
            {
                DsdEntryCollection entries = new DsdEntryCollection();

                foreach (Layout layout in layouts)
                {
                    //layout.Initialize();  --//giving exception
                    DsdEntry dsdEntry = new DsdEntry();
                    dsdEntry.DwgName = this._dwgFile;
                    dsdEntry.Layout = layout.LayoutName;
                    //File.AppendAllText("D:\\layout_names.txt", layout.LayoutName);     //test
                    dsdEntry.Title = Path.GetFileNameWithoutExtension(this._dwgFile) + "-" + layout.LayoutName;
                    dsdEntry.Nps = layout.TabOrder.ToString();
                    entries.Add(dsdEntry);
                }
                return entries;
            }

            private void PostProcessDSD(DsdData dsd)
            {
                string str, newStr;
                string tmpFile = Path.Combine(this._outputDir, "temp.dsd");

                dsd.WriteDsd(tmpFile);

                using (StreamReader reader = new StreamReader(tmpFile, Encoding.Default))
                using (StreamWriter writer = new StreamWriter(this._dsdFile, false, Encoding.Default))
                {
                    while (!reader.EndOfStream)
                    {
                        str = reader.ReadLine();
                        if (str.Contains("Has3DDWF"))
                        {
                            newStr = "Has3DDWF=0";
                        }
                        else if (str.Contains("OriginalSheetPath"))
                        {
                            newStr = "OriginalSheetPath=" + this._dwgFile;
                        }
                        else if (str.Contains("Type"))
                        {
                            newStr = "Type=6";
                        }
                        else if (str.Contains("OUT"))
                        {
                            newStr = "OUT=" + this._outputDir;
                        }
                        else if (str.Contains("IncludeLayer"))
                        {
                            newStr = "IncludeLayer=TRUE";
                        }
                        else if (str.Contains("PromptForDwfName"))
                        {
                            newStr = "PromptForDwfName=FALSE";
                        }
                        else if (str.Contains("LogFilePath"))
                        {
                            newStr = "LogFilePath=" + Path.Combine(this._outputDir, _logFile);
                        }
                        else
                        {
                            newStr = str;
                        }
                        writer.WriteLine(newStr);
                    }
                }
                File.Delete(tmpFile);
            }

            [CommandMethod("PublishAccore")]
            public static void PublishUsingAccoreConsole()
            {
                short bgp = (short)app.GetSystemVariable("BACKGROUNDPLOT");

                try
                {
                    app.SetSystemVariable("BACKGROUNDPLOT", 0);

                    using(Database db = HostApplicationServices.WorkingDatabase)
                    {
                        using (Transaction tr = db.TransactionManager.StartTransaction())
                        {
                            List<Layout> layouts = new List<Layout>();
                            DBDictionary layoutDict = (DBDictionary)db.LayoutDictionaryId.GetObject(OpenMode.ForRead);

                            //int i = 0;    //test

                            foreach (DBDictionaryEntry entry in layoutDict)
                            {
                                // if (entry.Key != "Model") //uncomment to skip model
                                {
                                    layouts.Add((Layout)tr.GetObject(entry.Value, OpenMode.ForRead));
                                    //i++;  //test
                                }
                            }

                            //File.WriteAllText("d:\\debug.txt", i.ToString());  //test
                            layouts.Sort((l1, l2) => l1.TabOrder.CompareTo(l2.TabOrder));

                            string filename = Path.ChangeExtension(db.Filename, "pdf");

                            MultiSheetsPdfUsingAccoreconsole plotter = new MultiSheetsPdfUsingAccoreconsole(filename, layouts);
                            plotter.Publish();

                            tr.Commit();
                        }
                    }
                }
                catch (System.Exception e)
                {
                    app.DocumentManager.MdiActiveDocument.Editor.WriteMessage("\nError: {0}\n{1}", e.Message, e.StackTrace);
                }
                finally
                {
                    app.SetSystemVariable("BACKGROUNDPLOT", bgp);
                }
            }
        }
}

