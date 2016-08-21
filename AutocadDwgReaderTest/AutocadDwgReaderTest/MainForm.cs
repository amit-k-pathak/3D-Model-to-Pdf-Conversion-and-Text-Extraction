using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using DwgFileReader;
using DwgToPdfConverter;
using AutocadConnector;
using AccoreconsoleLauncher;
using System.Threading;
using System.IO;
using System.Diagnostics;

namespace AutocadDwgReaderTest
{
    public partial class MainForm : Form
    {
        #region Private Variables

        private enum Filter : int
        {
            DwgDxfFile,
            PdfFile,
            DwfFile
        }

        private bool _proceed;

        private string _fileName;

        private BackgroundWorker _worker;

        private bool _flag;

        Stopwatch watch ;

        #endregion

        #region Constructors

        public MainForm()
        {
            InitializeComponent();
            this.MaximizeBox = false;
            this._proceed = false;
            this._worker = null;
            this._fileName = "";
            this._flag = false;
        }

        #endregion

        #region Private Methods

        #region worker

        private void Init()
        {
            if (this._worker == null)
            {
                this._worker = new BackgroundWorker();
                this._worker.WorkerReportsProgress = true;
                this._worker.WorkerSupportsCancellation = false;
                this._worker.DoWork += new DoWorkEventHandler(HandleDoWork);
                this._worker.ProgressChanged += new ProgressChangedEventHandler(HandleProgressChanged);
                this._worker.RunWorkerCompleted += new RunWorkerCompletedEventHandler(HandleWorkerCompleted);
                this.watch = new Stopwatch();
            }
        }

        private void StartWorker(Accoreconsole console)
        {
            Init();

            if (!_worker.IsBusy)
            {
                watch.Start();
                this._worker.RunWorkerAsync(console);
            }
        }

        private void HandleDoWork(object sender, DoWorkEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            Accoreconsole console = e.Argument as Accoreconsole;
            
            int i = 0;
            string msg = "";

            if (!(worker == null))
            {
                if (!(console == null))
                {
                    console.Process(_fileName);
                    msg = console.GetLog();
                    worker.ReportProgress(++i, msg); 
                }
            }
        }

        private void HandleWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;
            
            if (!(worker == null))
            {
                if (e.Error == null)
                {
                    this._flag = true;
                    WriteLog("Seems like, all done !!", LogMessageType.Information);
                    watch.Stop();
                    WriteLog("Total Time : " + watch.ElapsedMilliseconds.ToString() + " ms", LogMessageType.Information);
                }
                else
                    WriteLog("failed : " + e.Error.Data, LogMessageType.Error);
            }
        }

        private void HandleProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            BackgroundWorker worker = sender as BackgroundWorker;

            if (!(worker == null))
            {
                string msg = e.UserState as string;

                if (!string.IsNullOrWhiteSpace(msg))
                {
                    string[] str = msg.Split('\n');
                    foreach(string s in str) 
                        WriteLog(s, LogMessageType.Information);
                }
            }
        }

        #endregion

        private void exitToolStripMainMenu_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void exitToolStripMenuItem_Click(object sender, EventArgs e)
        {
            this.Close();
        }

        private void openDwgFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
                //string contents = "";
                ChangeFilter(Filter.DwgDxfFile);
                MultiselectFiles(false);
                
                if (openFileDialog.ShowDialog() == DialogResult.OK)
                {
                    //Commands cmd = new Commands();
                    //contents = cmd.ReadDwgFile(openFileDialog.FileName);

                    ////Commands.ExtractObjectsFromFile();
                    //if (!String.IsNullOrEmpty(contents))
                    //{
                    //    MessageBox.Show("File reading successful.", "Read DWG File", MessageBoxButtons.OK, MessageBoxIcon.Information);
                    //    //listBoxFileContents.Items.Add(contents);
                    //}
                    //else
                    //{
                    //    MessageBox.Show("File reading failed.", "Read DWG File", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    //}
                    if (openFileDialog.FileName.Trim().Length > 0)
                    {
                        this._fileName = openFileDialog.FileName;
                        MessageBox.Show("Got file name...", "Message", MessageBoxButtons.OK);
                    }
                }
        }

        private void convertToPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsAutocadRunning())
            {
                //DWGtoPDF.BuildList(@"C:\Users\amitp\Downloads\3D Files\dwg\New folder", true, false);
                //DWGtoPDF.Converttopdf();
            }
        }

        private void convertToMultisheetPDFToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (IsAutocadRunning())
            {
                MultiSheetsPdf.PlotPdf();
            }
        }

        private bool IsAutocadRunning()
        {
            if (_proceed == false)
            {
                MessageBox.Show("AutoCAD is not running, connect to AutoCAD first (connect->connect to AutoCAD)", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteLog("AutoCAD is not running.", LogMessageType.Error);
            }

            return _proceed;
        }

        private void WriteLog(string msg, LogMessageType type)
        {
            string format = LogWriter.WriteLog(msg, type);

            switch (type)
            {
                case LogMessageType.Error:
                    listBoxFileContents.Items.Add(format);
                break;

                case LogMessageType.Information:
                listBoxFileContents.Items.Add(format);
                break;
            }
            listBoxFileContents.Refresh();
        }

        private void connectToAutoCADToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string autocadInfo = "";

            try
            {
                    Connector connect = new Connector();

                    WriteLog("Trying to launch AutoCAD...", LogMessageType.Information);

                    _proceed = connect.ConnectToAutoCAD(out autocadInfo);

                    AutoCADDocument doc = new AutoCADDocument();

                    doc.OpenDocument(_fileName);

                    doc.CloseDocument();

                    if (_proceed == true)
                    {
                        MessageBox.Show("AutoCAD is running..." + autocadInfo, "Success", MessageBoxButtons.OK, MessageBoxIcon.Information);
                        WriteLog("AutoCAD is running now..." + autocadInfo, LogMessageType.Information);
                    }
                    else
                    {
                        MessageBox.Show("failed to launch AutoCAD..." + autocadInfo, "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
                        WriteLog("Could not launch AutoCAD......" + autocadInfo, LogMessageType.Error);
                    }
               
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Could not launch AutoCAD...", MessageBoxButtons.OK, MessageBoxIcon.Error);
                WriteLog("Could not launch AutoCAD......" + ex.Message, LogMessageType.Error);
            }
        }

        private void openDxfFileToolStripMenuItem_Click(object sender, EventArgs e)
        {
            //To-Do
        }

        private void accToolStripMenuItem_Click(object sender, EventArgs e)
        {
            
            if (!String.IsNullOrWhiteSpace(this._fileName))
            {
                Accoreconsole console = new Accoreconsole();
                StartWorker(console);
            }
            else
            {
                MessageBox.Show("Invalid file name !!", "Message", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            
             
        }

        private void ChangeFilter(Filter filter)
        {
            switch (filter)
            {
                case Filter.DwgDxfFile:
                    openFileDialog.Filter = "DWG Files|*.dwg|DXF Files|*.dxf";
                    break;

                case Filter.PdfFile:
                    openFileDialog.Filter = "PDF Files|*.pdf";
                    break;

                case Filter.DwfFile:
                    openFileDialog.Filter = "DWF Files|*.dwf";
                    break;
            }
        }

        private void MultiselectFiles(bool val)
        {
            openFileDialog.Multiselect = val;
        }

        private void mergePDFsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string outPdfFileName = "MergedPdf.pdf";
            string error = "";
            ChangeFilter(Filter.PdfFile);
            MultiselectFiles(true);

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                if (openFileDialog.FileNames.Length >= 2)
                {
                    List<string> pdfFileNames = new List<string>();
                    pdfFileNames.AddRange(openFileDialog.FileNames);
                    Accoreconsole console = new Accoreconsole();
                    WriteLog("Task : Merge PDFs", LogMessageType.Information);
                    WriteLog("PDF File/Files : ", LogMessageType.Information);

                    for (int i = 0; i < pdfFileNames.Count; ++i)
                        WriteLog(i + 1 + " : " + pdfFileNames[i], LogMessageType.Information);

                    try
                    {
                        if (console.mergerPdfFiles(pdfFileNames, outPdfFileName, out error))
                            WriteLog("Success : saved...", LogMessageType.Information);
                        else
                            WriteLog("Failed to merge pdf files : " + error, LogMessageType.Error);

                    }
                    catch (Exception ex)
                    {
                        WriteLog("Failed to save pdf file : " + error, LogMessageType.Error);
                        WriteLog(ex.Message, LogMessageType.Error);
                    }
                }
            }
            else
            {
                WriteLog("Select 2 or more pdf files...", LogMessageType.Error);
            }
        }

        private void buttonSaveLog_Click(object sender, EventArgs e)
        {
            string contents = "";
            if (listBoxFileContents.Items.Count >= 1)
            {
                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    WriteLog("Task : Save Log", LogMessageType.Information);
                    WriteLog("Save File : " + saveFileDialog.FileName, LogMessageType.Information);

                    foreach (string str in listBoxFileContents.Items)
                    {
                        contents += str + "\n";
                    }
                    File.WriteAllText(saveFileDialog.FileName, contents);

                    if (File.Exists(saveFileDialog.FileName))
                        WriteLog("Log file saved..." + saveFileDialog.FileName, LogMessageType.Information);
                    else
                        WriteLog("Couldn't save log file..." + saveFileDialog.FileName, LogMessageType.Information);
                }
            }
        }

        private void extractAllVectorContentsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            string error = "";

            ChangeFilter(Filter.PdfFile);
            MultiselectFiles(false);
            openFileDialog.Title = "Select PDF File";

            if (openFileDialog.ShowDialog() == DialogResult.OK)
            {
                WriteLog("Task : Extract all vector contents form TrueView exported PDF", LogMessageType.Information);
                WriteLog("Open PDF File : " + openFileDialog.FileName, LogMessageType.Information);

                if (saveFileDialog.ShowDialog() == DialogResult.OK)
                {
                    WriteLog("Output Text File : " + saveFileDialog.FileName, LogMessageType.Information);

                    Accoreconsole console = new Accoreconsole();

                    try
                    {
                        if (console.ExtractAllText(openFileDialog.FileName, saveFileDialog.FileName, out error))
                        {
                            WriteLog("Success : saved...", LogMessageType.Information);
                        }
                        else
                        {
                            WriteLog("Failed to extract text : " + error, LogMessageType.Error);
                        }
                    }
                    catch (Exception ex)
                    {
                        WriteLog("Failed to save text file : " + error, LogMessageType.Error);
                        WriteLog(ex.Message, LogMessageType.Error);
                    }
                }
            }
        }

        #endregion
    }

    internal class LogWriter
    {
        public static string WriteLog(string msg, LogMessageType type)
        {
            string formattedOutput = string.Empty;

            switch (type)
            {
                case LogMessageType.Error:
                    formattedOutput = "[" + DateTime.Now.ToString() + "]" + " - Error - " + msg;
                    break;

                case LogMessageType.Information:
                    formattedOutput = "[" + DateTime.Now.ToString() + "]" + " - Info - " + msg;
                    break;
            }

            return formattedOutput;
        }
    }
}
