using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using settingsReader = System.Configuration.ConfigurationSettings;

namespace AccoreconsoleLauncher
{
    internal class DwgModelsAndLayouts    
    {
        public bool isModel;
        public string name;
    }
    
    public class Accoreconsole
    {
        private static bool _readConfig;
        private static string _accoreExeFile;
        private static string _basePdfGenScrFile;
        private static string _modelAndLayoutGenScrFile;
        private static int _timeOut;
        private bool _isModel;
        private static string _basePdfGenScrFileModel;
        private static string _basePdfGenScrFileLayout;
     
        private LogWriter _writer;
        private List<string> _dwgModelsAndLayouts;
        private List<DwgModelsAndLayouts> _dwgModelsAndLayoutsDynamic;
        private int _tracker;
        private int _detected;
        private string _log;
        
        public Accoreconsole()
        {
            this._writer = new LogWriter();
            this._dwgModelsAndLayouts = new List<string>();
            this._tracker = -1;
            _readConfig = false;
            this._log = "";
            this._isModel = false;
            this._detected = 0;
            ReadConfig();
        }

        private void ReadConfig()
        {
            try
            {
                if (_readConfig == false)
                {
                    _accoreExeFile = (settingsReader.AppSettings["AccoreconsolFileName"] == null) ?
                                               "accoreconsole.exe" :
                                               settingsReader.AppSettings["AccoreconsolFileName"];

                    _basePdfGenScrFile = (settingsReader.AppSettings["BasePdfFGenScriptFile"] == null) ?
                                               "C:\\temp\\PDFGen_EN - Trueview - BaseScr.scr" :
                                               settingsReader.AppSettings["BasePdfFGenScriptFile"];

                    _basePdfGenScrFileModel = (settingsReader.AppSettings["BasePdfFGenScriptFileModel"] == null) ?
                                               "C:\\temp\\PDFGen_EN - Trueview - Model.scr" :
                                               settingsReader.AppSettings["BasePdfFGenScriptFileModel"];

                    _basePdfGenScrFileLayout = (settingsReader.AppSettings["BasePdfFGenScriptFileLayout"] == null) ?
                                               "C:\\temp\\PDFGen_EN - Trueview - Layout.scr" :
                                               settingsReader.AppSettings["BasePdfFGenScriptFileLayout"];

                    _modelAndLayoutGenScrFile = (settingsReader.AppSettings["ModelAndLayoutGenScript"] == null) ?
                                               "C:\\temp\\GetModelAndLayouts.scr" :
                                               settingsReader.AppSettings["ModelAndLayoutGenScript"];

                    _timeOut = (settingsReader.AppSettings["TimeOut"] == null) ?
                                               30000 :
                                               Convert.ToInt32(settingsReader.AppSettings["TimeOut"]);

                    _readConfig = true;

                    //Console.WriteLine(_accoreExeFile);

                }
            }
            catch (Exception ex)
            {
                _writer.LogOutput(ex.Message + "\n" + ex.StackTrace);
            }
        }

        private Process CreateProcess(string application, string args)
        {
            Process p = new Process();
            p.StartInfo.FileName = application;
            p.StartInfo.Arguments = args;


            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            //p.StartInfo.RedirectStandardInput = true;

            p.OutputDataReceived += new DataReceivedEventHandler(ReadOutputDataHandler);
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(ProcessExitedHandler);

            p.StartInfo.RedirectStandardError = true;  
            p.ErrorDataReceived += new DataReceivedEventHandler(ReadErrorDataHandler);

            p.StartInfo.CreateNoWindow = true;
            return p;
        }

        private Process CreatePdfConvertorProcess(string application, string args)
        {
            Process p = new Process();
            p.StartInfo.FileName = application;
            p.StartInfo.Arguments = args;

            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;

            p.OutputDataReceived += new DataReceivedEventHandler(PdfConvertorReadOutputDataHandler);
            p.EnableRaisingEvents = true;
            p.Exited += new EventHandler(PdfConvertorProcessExitedHandler);

            p.StartInfo.RedirectStandardError = true;
            p.ErrorDataReceived += new DataReceivedEventHandler(PdfConvertorReadErrorDataHandler);

            p.StartInfo.CreateNoWindow = true;
            return p;
        }

        private void PdfConvertorReadErrorDataHandler(object sender, DataReceivedEventArgs e)
        {
            
        }

        private void PdfConvertorProcessExitedHandler(object sender, EventArgs e)
        {
            Process p = sender as Process;

            if (p != null)
            {
                //if (p.HasExited)  // a necessity because modelAndScript generator process is getting killed
                //{
                //    if (p.ExitCode == 0)
                //    {
                //        _writer.LogOutput("accoreconsole executed successfully...");
                //    }
                //    else
                //    {
                //        _writer.LogOutput("accoreconsole exe failed...");
                //    }
                //}
            }
        }

        private void PdfConvertorReadOutputDataHandler(object sendingProcess, DataReceivedEventArgs receivedOutput)
        {
            Process p = sendingProcess as Process;

            try
            {

                if (!(p == null))
                {
                    if (!String.IsNullOrWhiteSpace(receivedOutput.Data))
                    {
                        String str = receivedOutput.Data.Replace("\0", "");
                        str = str.Replace("\b", "");

                        if (!string.IsNullOrWhiteSpace(str))
                        {
                            if (this._detected == 0)
                            {
                                //if (str.Contains("[Display/Extents/Limits/View/Window]"))   //"[Display/Extents/Layout/View/Window]")
                                //    this._detected = 1;

                                if (str.Contains("Limits")) //Limits option = Model, Layout option = Layout
                                    this._detected = 1;
                            }

                            if (str.Contains("_quit") || str.Contains("Enter plot scale") || this._detected == 1)
                            {
                                _tracker = Int32.MaxValue;

                                if (!p.HasExited)
                                {
                                    p.Kill();
                                }
                            }

                        }

                        //_writer.LogOutput(str + "\n");
                    }
                }
            }
            catch (Exception ex)
            {
                _writer.LogLocal("Exception occured..." + ex.Message + "\n" + ex.StackTrace, ref _log);
            }
        }

        private void ReadErrorDataHandler(object sender, DataReceivedEventArgs errorData)
        {
            Process p = sender as Process;

            if (!(p == null))
            {
                if (!String.IsNullOrWhiteSpace(errorData.Data))
                {
                    _writer.LogOutput(errorData.Data + "\n");
                }
            }
        }

        private void ProcessExitedHandler(object sender, EventArgs e)
        {
            Process p = sender as Process;

            if (p != null)
            {
                //if (p.HasExited)  // a necessity because modelAndScript generator process is getting killed
                //{
                //    if (p.ExitCode == 0)
                //    {
                //        _writer.LogOutput("accoreconsole executed successfully...");
                //    }
                //    else
                //    {
                //        _writer.LogOutput("accoreconsole exe failed...");
                //    }
                //}
            }
        }

        private void ReadOutputDataHandler(object sendingProcess, DataReceivedEventArgs receivedOutput)
        {
            Process p = sendingProcess as Process;

            if (!(p == null))
            {
                if (!String.IsNullOrWhiteSpace(receivedOutput.Data))
                {
                    String str = receivedOutput.Data.Replace("\0", "");
                    str = str.Replace("\b", "");

                    if (!string.IsNullOrWhiteSpace(str))
                    {  
                        if (_tracker == 1)
                        {
                            if (!str.Contains("Enter a layout name or [?]"))
                            {
                                _dwgModelsAndLayouts.Add(str);
                            }
                            else
                            {
                                _tracker = 0;
                            }
                        }

                        if (str.Contains("Layout(s) in drawing:"))
                        {
                            _tracker = 1;
                        }

                        if (str.Contains("_quit") || _tracker == 0)
                        {
                            _tracker = Int32.MaxValue;
                            if (!p.HasExited)
                            {
                                p.Kill();
                            }
                        }
                    }

                    _writer.LogOutput(str + "\n");
                }
            }
        }

        private void WriteData(Process accore)
        {
            //In case we want to redirect std input as well...
            StreamWriter writer = accore.StandardInput;
            StreamReader reader = accore.StandardOutput;

            if (accore != null)
            {
                while (reader.ReadLine().Contains("plot area"))
                {
                    this._isModel = false;
                    if (!reader.ReadLine().Contains("Layout"))
                        this._isModel = true;
                }

                if (this._isModel)
                {
                    writer.WriteLine("Extents");
                    
                    while (!reader.ReadLine().Contains("scale"));
                    writer.WriteLine("Fit");
                    
                    while(!reader.ReadLine().Contains("offset"));
                    writer.WriteLine("Center");

                    while(!reader.ReadLine().Contains("styles"));
                    writer.WriteLine("Yes");
                    
                    while(!reader.ReadLine().Contains("plot style table"));
                    writer.WriteLine(".");
                    
                    while(!reader.ReadLine().Contains("lineweights"));
                    writer.WriteLine("Yes");
                    
                    while(!reader.ReadLine().Contains("shade"));
                    writer.WriteLine("\"" + "As displayed" + "\"");
                    
                    while(!reader.ReadLine().Contains("file name"))
                    writer.WriteLine("");
                  
                    while(!reader.ReadLine().Contains("Save"));
                    writer.WriteLine("N");
                  
                    while (!reader.ReadLine().Contains("Proceed")) ;
                    writer.WriteLine("Y");
                }
                else
                {
                    writer.WriteLine("Layout");

                    while (!reader.ReadLine().Contains("scale")) ;
                    writer.WriteLine("Fit");

                    while (!reader.ReadLine().Contains("offset")) ;
                    writer.WriteLine("Center");

                    while (!reader.ReadLine().Contains("styles")) ;
                    writer.WriteLine("Yes");

                    while (!reader.ReadLine().Contains("plot style table")) ;
                    writer.WriteLine(".");

                    while (!reader.ReadLine().Contains("lineweights?")) ;
                    writer.WriteLine("Yes");

                    while (!reader.ReadLine().Contains("cale lineweights")) ;
                    writer.WriteLine("Yes");

                    while (!reader.ReadLine().Contains("file name"))
                        writer.WriteLine("");

                    while (!reader.ReadLine().Contains("paper space")) ;
                    writer.WriteLine("No");

                    while (!reader.ReadLine().Contains("Hide paper")) ;
                    writer.WriteLine("No");

                    while (!reader.ReadLine().Contains("file name"))
                        writer.WriteLine("");

                    while (!reader.ReadLine().Contains("Save")) ;
                    writer.WriteLine("N");

                    while (!reader.ReadLine().Contains("Proceed")) ;
                    writer.WriteLine("Y");
                }


                writer.Dispose();
                reader.Dispose();
            }
        }

        public string GetLog()
        {
            return _log;
        }

        //public void Process(string dwgFile)
        //{
        //    string exeFile = Path.Combine(AccoreConsoleHelper.GetTrueView2016InstallPath(), _accoreExeFile);
        //    string applicationArguments = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, _modelAndLayoutGenScrFile);

        //    //AccoreConsoleHelper.RunAccoreconsoleApp(exePath, dwgFile, scriptPath, 20000); //Threaded
        //    _writer.LogLocal("------------LOGGING STARTED------------", ref _log);
        //    _writer.LogLocal("Received....", ref _log);
        //    _writer.LogLocal("File Name : " + dwgFile, ref _log);
        //    _writer.LogLocal("TrueView Accoreconsole : " + exeFile, ref _log);
        //    _writer.LogLocal("Model&LayoutGenScr : " + _modelAndLayoutGenScrFile, ref _log);
        //    _writer.LogLocal("Base Script : " + _basePdfGenScrFile, ref _log);
        //    _writer.LogLocal("Trying to launch Model&LayoutGenScr Process: ", ref _log);

        //    try
        //    {

        //        using (Process accore = CreateProcess(exeFile, applicationArguments))
        //        {
        //            accore.Start();
        //            accore.BeginOutputReadLine();
        //            accore.BeginErrorReadLine();
        //            accore.WaitForExit(_timeOut);

        //            _writer.LogLocal("Ok...", ref _log);

        //            string str = File.ReadAllText(_basePdfGenScrFile);
        //            string content = str;
        //            List<string> scriptFilePerModelOrLayout = new List<string>();

        //            while (!(_dwgModelsAndLayouts.Count >= 0) || _tracker != Int32.MaxValue) ;

        //            _writer.LogLocal("Model & Layout List : " + _dwgModelsAndLayouts.Count, ref _log);

        //            for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
        //                _writer.LogLocal(_dwgModelsAndLayouts[i], ref _log);

        //            for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
        //            {
        //                scriptFilePerModelOrLayout.Add(Path.Combine(Path.GetDirectoryName(_basePdfGenScrFile), Path.GetFileNameWithoutExtension(dwgFile) + "-" + _dwgModelsAndLayouts[i]) + ".scr");
        //                _writer.LogLocal("Generating script file for : " + _dwgModelsAndLayouts[i], ref _log);

        //                if (AccoreConsoleHelper.GenerateScriptFile(scriptFilePerModelOrLayout[i], str, _dwgModelsAndLayouts[i]))
        //                    _writer.LogLocal("success: " + scriptFilePerModelOrLayout[i], ref _log);
        //                else
        //                    _writer.LogLocal("could not generate script file : " + scriptFilePerModelOrLayout[i], ref _log);
        //            }

        //            string pdfFile = "";
        //            string textFile = "";
        //            string args = "";
        //            PdfDocument doc = new PdfDocument();
        //            string[] files = new string[_dwgModelsAndLayouts.Count];

        //            int index = 1;
        //            string error = "";
        //            string stdOut = "";

        //            for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
        //            {
        //                //AccoreConsoleHelper.RunAccoreconsoleApp(exeFile, dwgFile, path, 20000);
        //                args = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, scriptFilePerModelOrLayout[i]);

        //                using (Process proc = CreateProcess(exeFile, args))
        //                {
        //                    proc.StartInfo.RedirectStandardOutput = true;
        //                    proc.StartInfo.CreateNoWindow = true;  /////
        //                    proc.Start();


        //                    using(StreamReader sr = new StreamReader(proc.StandardOutput.BaseStream))
        //                    {
        //                        while (!sr.EndOfStream)
        //                        {
        //                            stdOut = sr.ReadLine().Replace("\0", "");
        //                            stdOut = stdOut.Replace("\b", "");

        //                            if (stdOut.Contains("Enter plot area"))
        //                            {
        //                                if (!stdOut.Contains("Layout"))
        //                                {
        //                                                                        // This is model
        //                                }
        //                            }
        //                        }
        //                    }

        //                    proc.WaitForExit(_timeOut); //one by one exec, for safety

        //                    pdfFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".pdf");

        //                    if (!File.Exists(pdfFile))
        //                    {
        //                        //_writer.LogOutput("Failed to generate : " + pdfFile);
        //                        string file = "";
        //                        file = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "No.pdf");
        //                        if (File.Exists(file))
        //                        {
        //                            //index = i;
        //                            File.Copy(file, pdfFile);
        //                            File.Delete(file);
        //                            _writer.LogLocal("Pdf file generated : " + pdfFile, ref _log);
        //                            files[0] = pdfFile;
        //                        }
        //                        else
        //                            _writer.LogLocal("Failed to generate pdf file: " + pdfFile, ref _log);
        //                    }
        //                    else
        //                    {
        //                        files[index++] = pdfFile;
        //                        _writer.LogLocal("Pdf file generated : " + pdfFile, ref _log);
        //                        int fileIndex = doc.LoadPdfDocument(pdfFile);
        //                        textFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".txt");
        //                        doc.LoadPdfPage(fileIndex, pdfFile, 0);
        //                        //doc.GetAllText(fileIndex, textFile, doc.GetPageDimension(fileIndex, 0));

        //                        if (!File.Exists(textFile))
        //                            _writer.LogLocal("Failed to generate text file : " + textFile, ref _log);
        //                        else
        //                            _writer.LogLocal("Text file generated : " + textFile, ref _log);

        //                        doc.ClosePdfDocument(fileIndex);

        //                    }
        //                }
        //            }

        //            string modelOrLayoutPdfFile = "";

        //            //Merget outputs
                   
        //            _writer.LogLocal("Meging Reordered File list... : ", ref _log);

        //            for (int i = 0; i < files.Length; i++)
        //            {
        //                _writer.LogLocal("Meging Files... : " + files[i], ref _log);
        //            }

        //            if (mergerPdfFiles(files.ToList(), Path.ChangeExtension(dwgFile, ".pdf"), out error))
        //                _writer.LogLocal("Meging Success : " + Path.ChangeExtension(dwgFile, ".pdf"), ref _log);

        //            //doc.MergePdfFiles(files.ToList(), ""); 

        //            for (int i = 0; i < scriptFilePerModelOrLayout.Count; ++i)
        //            {
        //                modelOrLayoutPdfFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".pdf");
        //                if (File.Exists(modelOrLayoutPdfFile))
        //                    File.Delete(modelOrLayoutPdfFile);

        //                if (File.Exists(scriptFilePerModelOrLayout[i]))
        //                    File.Delete(scriptFilePerModelOrLayout[i]);
        //            }
        //        }

        //        _writer.LogLocal("------------LOGGING ENDED------------", ref _log);
        //    }
        //    catch (Exception ex)
        //    {
        //        throw ex;
        //    }
        //}

        public void Process(string dwgFile)
        {
            string exeFile = Path.Combine(AccoreConsoleHelper.GetTrueView2016InstallPath(), _accoreExeFile);
            string applicationArguments = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, _modelAndLayoutGenScrFile);
     
            //AccoreConsoleHelper.RunAccoreconsoleApp(exePath, dwgFile, scriptPath, 20000); //Threaded
            _writer.LogLocal("------------LOGGING STARTED------------", ref _log);
            _writer.LogLocal("Received....", ref _log);
            _writer.LogLocal("File Name : " + dwgFile, ref _log);
            _writer.LogLocal("TrueView Accoreconsole : " + exeFile, ref _log);
            _writer.LogLocal("Model&LayoutGenScr : " + _modelAndLayoutGenScrFile, ref _log);
            _writer.LogLocal("Base Script : " + _basePdfGenScrFile, ref _log);
            _writer.LogLocal("Trying to launch Model&LayoutGenScr Process: ", ref _log);

            try
            {
                using (Process accore = CreateProcess(exeFile, applicationArguments))
                {
                    accore.Start();
                    accore.BeginOutputReadLine();
                    accore.BeginErrorReadLine();
                    accore.WaitForExit(_timeOut);

                    _writer.LogLocal("Model&LayoutGenScr Process : Ok", ref _log);

                    string content = File.ReadAllText(_basePdfGenScrFile);
                    string str;
                    List<string> scriptFilePerModelOrLayout = new List<string>();
                    _dwgModelsAndLayoutsDynamic = new List<DwgModelsAndLayouts>();

                    while (!(_dwgModelsAndLayouts.Count >= 0) || _tracker != Int32.MaxValue) ;

                    _writer.LogLocal("Model & Layout List : " + _dwgModelsAndLayouts.Count, ref _log);

                    for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
                        _writer.LogLocal(_dwgModelsAndLayouts[i], ref _log);

                    for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
                    {
                        scriptFilePerModelOrLayout.Add(Path.Combine(Path.GetDirectoryName(_basePdfGenScrFile), Path.GetFileNameWithoutExtension(dwgFile) + "-" + _dwgModelsAndLayouts[i]) + ".scr");
                        _writer.LogLocal("Generating script file for : " + _dwgModelsAndLayouts[i], ref _log);

                        string arguments = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, scriptFilePerModelOrLayout[i]);
                        this._detected = 0;

                        _writer.LogLocal("Trying to launch Model&LayoutDetectorScr Process : " + _dwgModelsAndLayouts[i], ref _log);

                        if (AccoreConsoleHelper.GenerateScriptFile(scriptFilePerModelOrLayout[i], content, _dwgModelsAndLayouts[i]))
                            _writer.LogLocal("success: " + scriptFilePerModelOrLayout[i], ref _log);
                        else
                            _writer.LogLocal("could not generate script file : " + scriptFilePerModelOrLayout[i], ref _log);

                        //Detect Model or Layout here
                        using(Process p = CreatePdfConvertorProcess(exeFile, arguments))
                        {
                            p.Start();
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();
                            p.WaitForExit(_timeOut);

                            _writer.LogLocal("Model&LayoutDetectorScr Process : Ok" + _dwgModelsAndLayouts[i], ref _log);

                            while (_tracker != Int32.MaxValue);
                        }

                        DwgModelsAndLayouts modelOrLayout = new DwgModelsAndLayouts();
                        modelOrLayout.isModel = this._detected == 1? true : false;
                        modelOrLayout.name = scriptFilePerModelOrLayout[i];
                        _dwgModelsAndLayoutsDynamic.Add(modelOrLayout);

                        _writer.LogLocal("Is Model : " + _dwgModelsAndLayoutsDynamic[i].isModel, ref _log);
                         
                        //Layout
                        if (!_dwgModelsAndLayoutsDynamic[i].isModel)
                        {
                            str = File.ReadAllText(_basePdfGenScrFileLayout);
                        }
                        else //Model
                        {
                            str = File.ReadAllText(_basePdfGenScrFileModel);
                        }


                        if (AccoreConsoleHelper.GenerateScriptFile(scriptFilePerModelOrLayout[i], str, _dwgModelsAndLayouts[i]))
                            _writer.LogLocal("success: " + scriptFilePerModelOrLayout[i], ref _log);
                        else
                            _writer.LogLocal("could not generate script file : " + scriptFilePerModelOrLayout[i], ref _log);

                    }

                    string pdfFile = "";
                    string textFile = "";
                    string args = "";
                    PdfDocument doc = new PdfDocument();
                    string[] files = new string[_dwgModelsAndLayouts.Count];

                    int index = 1;
                    string error = "";

                    for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
                    {
                        //AccoreConsoleHelper.RunAccoreconsoleApp(exeFile, dwgFile, path, 20000);
                        args = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, scriptFilePerModelOrLayout[i]);

                        using (Process proc = CreatePdfConvertorProcess(exeFile, args))
                        {
                            proc.Start();
              
                            proc.WaitForExit(_timeOut); //one by one exec, for safety

                            pdfFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".pdf");

                            if (File.Exists(pdfFile))
                            {
                                if(_dwgModelsAndLayoutsDynamic[i].isModel)
                                    files[0] = pdfFile;
                                else
                                    files[index++] = pdfFile;
                                _writer.LogLocal("Pdf file generated : " + pdfFile, ref _log);
                                //int fileIndex = doc.LoadPdfDocument(pdfFile);
                                //textFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".txt");
                                //doc.LoadPdfPage(fileIndex, pdfFile, 0);
                                ////doc.GetAllText(fileIndex, textFile, doc.GetPageDimension(fileIndex, 0));

                                //if (!File.Exists(textFile))
                                //    _writer.LogLocal("Failed to generate text file : " + textFile, ref _log);
                                //else
                                //    _writer.LogLocal("Text file generated : " + textFile, ref _log);

                                //doc.ClosePdfDocument(fileIndex);

                            }
                            else
                            {
                                _writer.LogLocal("Failed to generate pdf file: " + pdfFile, ref _log);

                            }
                        }
                    }

                    string modelOrLayoutPdfFile = "";

                    //Merget outputs

                    _writer.LogLocal("Merging Reordered File list... : ", ref _log);

                    for (int i = 0; i < files.Length; i++)
                    {
                        _writer.LogLocal("Merging Files... : " + files[i], ref _log);
                    }

                    if (mergerPdfFiles(files.ToList(), Path.ChangeExtension(dwgFile, ".pdf"), out error))
                        _writer.LogLocal("Merging Success : " + Path.ChangeExtension(dwgFile, ".pdf"), ref _log);

                    //doc.MergePdfFiles(files.ToList(), ""); 

                    for (int i = 0; i < scriptFilePerModelOrLayout.Count; ++i)
                    {
                        modelOrLayoutPdfFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".pdf");
                        if (File.Exists(modelOrLayoutPdfFile))
                            File.Delete(modelOrLayoutPdfFile);

                        if (File.Exists(scriptFilePerModelOrLayout[i]))
                            File.Delete(scriptFilePerModelOrLayout[i]);
                    }
                }


                _writer.LogLocal("------------LOGGING ENDED------------", ref _log);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public ConversionStatus ConvertDwgToPdf(string dwgFile)
        {
            ConversionStatus status = ConversionStatus.ConversionFailed;
            string exeFile = Path.Combine(AccoreConsoleHelper.GetTrueView2016InstallPath(), _accoreExeFile);
            //Console.WriteLine(exeFile);
            string applicationArguments = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, _modelAndLayoutGenScrFile);
            //Console.WriteLine(applicationArguments);
            //AccoreConsoleHelper.RunAccoreconsoleApp(exePath, dwgFile, scriptPath, 20000); //Threaded
            _writer.LogLocal("------------LOGGING STARTED------------", ref _log);
            _writer.LogLocal("Received....", ref _log);
            _writer.LogLocal("File Name : " + dwgFile, ref _log);
            _writer.LogLocal("TrueView Accoreconsole : " + exeFile, ref _log);
            _writer.LogLocal("Model&LayoutGenScr : " + _modelAndLayoutGenScrFile, ref _log);
            _writer.LogLocal("Base Script : " + _basePdfGenScrFile, ref _log);
            _writer.LogLocal("Trying to launch Model&LayoutGenScr Process: ", ref _log);

            try
            {
                using (Process accore = CreateProcess(exeFile, applicationArguments))
                {
                    accore.Start();
                    accore.BeginOutputReadLine();
                    accore.BeginErrorReadLine();
                    accore.WaitForExit(_timeOut);

                    _writer.LogLocal("Model&LayoutGenScr Process : Ok\n", ref _log);

                    string content = File.ReadAllText(_basePdfGenScrFile);
                    string str;
                    List<string> scriptFilePerModelOrLayout = new List<string>();
                    _dwgModelsAndLayoutsDynamic = new List<DwgModelsAndLayouts>();

                    while (!(_dwgModelsAndLayouts.Count >= 0) || _tracker != Int32.MaxValue) ;

                    _writer.LogLocal("Model & Layout List : " + _dwgModelsAndLayouts.Count, ref _log);

                    for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
                        _writer.LogLocal(_dwgModelsAndLayouts[i], ref _log);

                    for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
                    {
                        scriptFilePerModelOrLayout.Add(Path.Combine(Path.GetDirectoryName(_basePdfGenScrFile), Path.GetFileNameWithoutExtension(dwgFile) + "-" + _dwgModelsAndLayouts[i]) + ".scr");
                        _writer.LogLocal("Generating script file for : " + _dwgModelsAndLayouts[i], ref _log);

                        string arguments = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, scriptFilePerModelOrLayout[i]);
                        this._detected = 0;

                        _writer.LogLocal("Trying to launch Model&LayoutDetectorScr Process : " + _dwgModelsAndLayouts[i], ref _log);

                        if (AccoreConsoleHelper.GenerateScriptFile(scriptFilePerModelOrLayout[i], content, _dwgModelsAndLayouts[i]))
                            _writer.LogLocal("success: " + scriptFilePerModelOrLayout[i], ref _log);
                        else
                            _writer.LogLocal("could not generate script file : " + scriptFilePerModelOrLayout[i], ref _log);

                        //Detect Model or Layout here
                        using (Process p = CreatePdfConvertorProcess(exeFile, arguments))
                        {
                            p.Start();
                            p.BeginOutputReadLine();
                            p.BeginErrorReadLine();
                            p.WaitForExit(_timeOut);

                            _writer.LogLocal("Model&LayoutDetectorScr Process : Ok" + _dwgModelsAndLayouts[i], ref _log);

                            while (_tracker != Int32.MaxValue) ;
                        }

                        DwgModelsAndLayouts modelOrLayout = new DwgModelsAndLayouts();
                        modelOrLayout.isModel = this._detected == 1 ? true : false;
                        modelOrLayout.name = scriptFilePerModelOrLayout[i];
                        _dwgModelsAndLayoutsDynamic.Add(modelOrLayout);

                        _writer.LogLocal("Is Model : " + _dwgModelsAndLayoutsDynamic[i].isModel, ref _log);

                        //Layout
                        if (!_dwgModelsAndLayoutsDynamic[i].isModel)
                        {
                            str = File.ReadAllText(_basePdfGenScrFileLayout);
                        }
                        else //Model
                        {
                            str = File.ReadAllText(_basePdfGenScrFileModel);
                        }


                        if (AccoreConsoleHelper.GenerateScriptFile(scriptFilePerModelOrLayout[i], str, _dwgModelsAndLayouts[i]))
                            _writer.LogLocal("success: " + scriptFilePerModelOrLayout[i], ref _log);
                        else
                            _writer.LogLocal("could not generate script file : " + scriptFilePerModelOrLayout[i], ref _log);

                    }

                    string pdfFile = "";
                    //string textFile = "";
                    string args = "";
                    PdfDocument doc = new PdfDocument();
                    string[] files = new string[_dwgModelsAndLayouts.Count];

                    int index = 1;
                    string error = "";

                    for (int i = 0; i < _dwgModelsAndLayouts.Count; ++i)
                    {
                        //AccoreConsoleHelper.RunAccoreconsoleApp(exeFile, dwgFile, path, 20000);
                        args = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", dwgFile, scriptFilePerModelOrLayout[i]);

                        using (Process proc = CreatePdfConvertorProcess(exeFile, args))
                        {
                            proc.Start();

                            proc.WaitForExit(_timeOut); //one by one exec, for safety

                            if (!proc.HasExited)
                                proc.Kill();

                            //pdfFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".pdf");
                            pdfFile = Path.GetDirectoryName(dwgFile) + "\\" + Path.GetFileNameWithoutExtension(scriptFilePerModelOrLayout[i]) + ".pdf";

                            if (File.Exists(pdfFile))
                            {
                                if (_dwgModelsAndLayoutsDynamic[i].isModel)
                                    files[0] = pdfFile;
                                else
                                    files[index++] = pdfFile;
                                _writer.LogLocal("Pdf file generated : " + pdfFile, ref _log);
                                //int fileIndex = doc.LoadPdfDocument(pdfFile);
                                //textFile = Path.ChangeExtension(scriptFilePerModelOrLayout[i], ".txt");
                                //doc.LoadPdfPage(fileIndex, pdfFile, 0);
                                ////doc.GetAllText(fileIndex, textFile, doc.GetPageDimension(fileIndex, 0));

                                //if (!File.Exists(textFile))
                                //    _writer.LogLocal("Failed to generate text file : " + textFile, ref _log);
                                //else
                                //    _writer.LogLocal("Text file generated : " + textFile, ref _log);

                                //doc.ClosePdfDocument(fileIndex);

                            }
                            else
                            {
                                _writer.LogLocal("Failed to generate pdf file: " + pdfFile, ref _log);

                            }
                        }
                    }

                    string modelOrLayoutPdfFile = "";

                    //Merget outputs

                    _writer.LogLocal("Merging Reordered File list... : ", ref _log);

                    for (int i = 0; i < files.Length; i++)
                    {
                        _writer.LogLocal("Merging Files... : " + files[i], ref _log);
                    }

                    string targetPdfFile = Path.GetFileNameWithoutExtension(dwgFile) + "_Conv.pdf";

                    if (mergerPdfFilesDemo(files.ToList(), targetPdfFile, out error))
                    {
                        _writer.LogLocal("Merging Success : " + Path.GetDirectoryName(dwgFile) + "\\" + targetPdfFile, ref _log);
                        status = ConversionStatus.ConversionSuccess;
                    }
                    else
                    {
                        _writer.LogLocal("Merging Failed : " + Path.GetDirectoryName(dwgFile) + "\\" + targetPdfFile, ref _log);
                    }

                    //doc.MergePdfFiles(files.ToList(), ""); 

                    for (int i = 0; i < scriptFilePerModelOrLayout.Count; ++i)
                    {
                        modelOrLayoutPdfFile = Path.ChangeExtension(files[i], ".pdf");
                        if (File.Exists(modelOrLayoutPdfFile))
                            File.Delete(modelOrLayoutPdfFile);

                        if (File.Exists(scriptFilePerModelOrLayout[i]))
                            File.Delete(scriptFilePerModelOrLayout[i]);
                    }
                }
                
                return status;
            }
            catch (Exception ex)
            {
                _writer.LogLocal("Exception : "+ex.Message + "\n" + ex.StackTrace, ref _log);
                return ConversionStatus.ConversionFailed;
            }
            finally
            {
                _writer.LogLocal("------------LOGGING ENDED------------", ref _log);
            }
        }

        public bool mergerPdfFiles(List<string> pdfFileList, string outPdfFileName, out string error)
        {
            error = "";
            PWPFileProcessingLib.PWPPDFMgr mgr = new PWPFileProcessingLib.PWPPDFMgr();

            //test
            //PdfDocument doc = new PdfDocument();
            //doc.MergePdfFiles(pdfFileList, "test_mupdf.pdf");

            return mgr.MergePDFFiles(pdfFileList, outPdfFileName, out error);
        }

        public bool mergerPdfFilesDemo(List<string> pdfFileList, string outPdfFileName, out string error)
        {
            error = "";
           // Console.WriteLine(outPdfFileName);
            PWPFileProcessingLib.PWPPDFMgr mgr = new PWPFileProcessingLib.PWPPDFMgr();
            return mgr.MergePDFFiles(pdfFileList, outPdfFileName, out error);
        }

        public bool ExtractAllText(string inputPdfFile, string outTxtFileName, out string error)
        {
            error = "";
            PWPFileProcessingLib.PWPPDFMgr mgr = new PWPFileProcessingLib.PWPPDFMgr();
            return mgr.ExtractAllTextFromDwg(inputPdfFile, outTxtFileName, out error);
        }
    }

    internal class AccoreConsoleHelper
    {
        public static string GetTrueView2016InstallPath()
        {
            String TrueViewVer = @"R14\";
            String ReleaseNumber = "dwgviewr-F001:409";
            String TrueViewInstallPath = GetRegistryValueHKLM(@"Software\Autodesk\DWG TrueView\" + TrueViewVer + ReleaseNumber, "Location");

            if (string.IsNullOrWhiteSpace(TrueViewInstallPath))
                TrueViewInstallPath = @"C:\Program Files\Autodesk\DWG TrueView 2016 - English\";
            return TrueViewInstallPath;
        }

        static string GetRegistryValueHKLM(string subKey, string keyName)
        {
            string value = string.Empty;
            
            RegistryKey registrySubKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry64);

            RegistryKey key = registrySubKey.OpenSubKey(subKey);

            if (key != null)
            {
                object regKey = key.GetValue(keyName);
                
                if (regKey != null)
                {
                    value = regKey.ToString();
                }
            }
            return value;
        }

        // Create TrueView script file
        public static bool GenerateScriptFile(String filePath, String content, string modelOrLayout)
        {
            string dirPath = Path.GetDirectoryName(filePath);

            if (!Directory.Exists(dirPath))
                Directory.CreateDirectory(dirPath);

            DeleteIfExists(filePath);

            content = content.Replace("Model", "\"" + modelOrLayout + "\"");

            if (File.Exists(filePath))
                File.Delete(filePath);

            //File.WriteAllText(scriptFilePerModelOrLayout[i], content);

            try
            {
                using (TextWriter tw = new StreamWriter(filePath, false))
                {
                    tw.Write(content);
                    tw.Flush();
                    tw.Close();
                }
            }
            catch (Exception ex)
            {

            }

            return File.Exists(filePath);
        }

        public static bool DeleteIfExists(String filePath)
        {
            try
            {
                if (File.Exists(filePath))
                    File.Delete(filePath);
            }
            catch
            {
                return false;
            }
            return true;
        }

        public static Thread RunAccoreconsoleApp(string ApplicationPath, string drawingFilePath, string scriptFilePath, int maxWaitInMilliSeconds)
        {
            Thread workerthread = new Thread(delegate()
            {
                LogWriter writer = new LogWriter();

                if (!File.Exists(ApplicationPath))
                {
                    writer.LogOutput("Location of accoreconsole.exe not found !");
                }
                else
                {
                    Process[] processes = Process.GetProcessesByName(Path.GetFileNameWithoutExtension(ApplicationPath));

                    foreach (Process proc in processes)
                    {
                        proc.Kill();
                    }

                    // Prepare the arguments
                    string ApplicationArguments = string.Format("/i \"{0}\" /s \"{1}\" /l en-US", drawingFilePath, scriptFilePath);

                    Process ProcessObj = new Process();
                    ProcessObj.StartInfo.FileName = ApplicationPath;
                    ProcessObj.StartInfo.Arguments = ApplicationArguments;

                    //if (Properties.Settings.Default.HideConsoleWindow == false)
                    if(false)
                    {// Display the accoreconsole window
                        ProcessObj.StartInfo.UseShellExecute = true;
                        ProcessObj.StartInfo.CreateNoWindow = false;
                    }
                    else
                    {// Do not display the accoreconsole window
                        ProcessObj.StartInfo.UseShellExecute = false;
                        ProcessObj.StartInfo.CreateNoWindow = true;
                        ProcessObj.StartInfo.RedirectStandardOutput = true;
                    }

                    // Launch
                    ProcessObj.Start();
                    

                    // Wait for the exe to complete.
                    ProcessObj.WaitForExit(maxWaitInMilliSeconds);

                    // Get the console output of accoreconsole.exe and log it to a file.
                    //if (Properties.Settings.Default.HideConsoleWindow)
                    if(true)
                    {
                        //writer.LogOutput(ProcessObj.StandardOutput.ReadToEnd());
                    }
                    else
                    {
                        //writer.LogOutput("Console output not available if \"Hide Console Window\" setting is unchecked.");
                    }
                }
            });

            workerthread.Start();
            return workerthread;
        }

        public static void RunAccoreConsoleApp()
        {

        }

        static void OpenDefaultViewer(string filePath)
        {
            System.Diagnostics.ProcessStartInfo Print = new System.Diagnostics.ProcessStartInfo();
            Print.Verb = "open";
            Print.WindowStyle = ProcessWindowStyle.Maximized;
            Print.FileName = filePath;
            Print.UseShellExecute = true;
            try
            {
                System.Diagnostics.Process.Start(Print);
            }
            catch (Exception)
            {
                //System.Windows.Forms.MessageBox.Show("Generated file : " + filePath);
            }
        }

        static string AppPath
        {
            get
            {
                return Path.GetDirectoryName(Directory.GetCurrentDirectory());
            }
        }
    }

    internal class LogWriter
    {
        private string _logFilePath;

        public LogWriter()
        {
            this._logFilePath = Path.Combine("C:\\temp\\", "LastRun.log");
        }

        public void LogOutput(string content)
        {
            string dirPath = Path.GetDirectoryName(this._logFilePath);
            
            if (Directory.Exists(dirPath) == false)
                Directory.CreateDirectory(dirPath);

            //DeleteIfExists(logFilePath);

            try
            {
                using (TextWriter tw = new StreamWriter(this._logFilePath, true))
                {
                    tw.Write(content);
                    tw.Flush();
                    tw.Close();
                }
            }
            catch (Exception ex)
            {

            }
        }

        public string GetLogText()
        {
            string sContent = string.Empty;
           
            if (File.Exists(this._logFilePath))
            {
                using (TextReader tr = new StreamReader(this._logFilePath))
                {
                    sContent = tr.ReadToEnd();
                    sContent = sContent.Replace("\b", "");
                    tr.Close();
                }
            }
            return sContent;
        }

        public void LogLocal(string msg, ref string _log)
        {
            if(!string.IsNullOrWhiteSpace(msg))
                _log += msg + "\n";
        }
    }

    public enum ConversionStatus
    {
        ConversionSuccess = 0,
        ConversionFailed = -1
    }
}
