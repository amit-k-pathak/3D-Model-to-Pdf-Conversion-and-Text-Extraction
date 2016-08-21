using System;
using System.IO;
using System.Reflection;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Runtime.InteropServices;
using System.Configuration;


namespace PWPFileProcessingLib
{
    public class PWPPDFMgr
    {
        private PDFLibrary pdfLib = new PDFLibrary(@"DebenuPDFLibrary64DLL1113.dll");
        private bool isLibraryLoaded = false;
        public PWPPDFMgr()
        {
            
        }
        ~PWPPDFMgr()
        {
            if(isLibraryLoaded == true)
            {
                pdfLib.ReleaseLibrary();
                isLibraryLoaded = false;
            }
        }
        
        private bool LoadPDFLib()
        {
            bool result = true;
            try
            {
                if (isLibraryLoaded == false)
                {
                    if (pdfLib.LibraryLoaded())
                    {
                        // Create an instance of the class and give it the path to the DLL
                        string LicKey = (!string.IsNullOrEmpty(System.Configuration.ConfigurationSettings.AppSettings["DebenuLicenceKey"]) ? System.Configuration.ConfigurationSettings.AppSettings["DebenuLicenceKey"].ToString() : "jc8am89q6n57ue8q87xd7nt4y");
                        pdfLib.UnlockKey(LicKey);

                        if (pdfLib.Unlocked() == 0)
                        {
                            //LogManager.Trace("PWPPDFMgr::LoadPDFLib", TraceCategoryType.Error, "License unlock failed.  Please update your key in the form1.cs source code", "");
                            result = false;
                        }
                        else
                        {
                            isLibraryLoaded = true;
                        }
                    }
                    else
                    {
                        //LogManager.Trace("PWPPDFMgr::LoadPDFLib", TraceCategoryType.Error, "Could not locate the Debenu PDF Library DLL, please check the path");
                        result = false;
                    }
                }
            }
            catch(Exception Ex)
            {
                //LogManager.Trace("PWPPDFMgr::LoadPDFLib", Ex1, "Could not locate the Debenu PDF Library DLL, please check the path");
                result = false;
            }
            return result;
        }

        public bool ExtractPDFPages(string srcpath, string destpath, int PagesCnt, out string err)//, int primaryid, int secondaryid, float ordinalNum)
        {
            bool result = false;
            int opsResult = -1;
            int FileHandle = -1;
            int count = PagesCnt;
            int errorcode = 0;
            err = "";
            try
            {
                if (isLibraryLoaded == false)
                {
                    result = LoadPDFLib();
                    if (result == false)
                    {
                        return result;
                    }
                }

                //Page count is 0 - findout at runtime page count
                if (PagesCnt == -1)
                {
                    FileHandle = pdfLib.DAOpenFileReadOnly(srcpath, "");
                    if (FileHandle == 0)
                    {
                        errorcode = pdfLib.LastErrorCode();
                        err += "Debenu Error Code " + errorcode;
                        //LogManager.Trace("PWPPDFMgr::ExtractPDFPages", TraceCategoryType.Error, "DAOpenFileReadOnly failed - Unable to open file-srcpath", srcpath, "errorcode", errorcode.ToString());
                        return result;
                    }
                    count = pdfLib.DAGetPageCount(FileHandle);

                    if (FileHandle != -1)
                    {
                        pdfLib.DACloseFile(FileHandle);
                    }
                }


                string pagefilename = string.Empty;
                string pagewisepath = string.Empty;
                string pagesRange = string.Empty;
                //PWPDocumentInfo docpageInfo = null;

                if(!Directory.Exists(Path.Combine(destpath, Path.GetFileName(srcpath))))
                {
                    Directory.CreateDirectory(Path.Combine(destpath, Path.GetFileNameWithoutExtension(srcpath) ));
                    pagefilename = Path.Combine(destpath, Path.GetFileNameWithoutExtension(srcpath));
                }

                for (int idx = 1; idx <= count; idx++)
                {
                    pagewisepath = string.Empty;
                    pagesRange = string.Empty;

                    pagewisepath = Path.Combine(pagefilename, "Page_" + idx + ".pdf");
                    pagesRange = idx.ToString() + "-" + idx.ToString();
                    opsResult = pdfLib.ExtractFilePages(srcpath, "", pagewisepath, pagesRange);
                    if (opsResult == 0)
                    {
                        errorcode = pdfLib.LastErrorCode();
                        err += "Debenu Error Code " + errorcode;
                        break;
                    }
                }
            }
            catch(Exception ex)
            {
                result = false;
                err += ex.Message;
            }
            return result;
        }

        public bool MergePDFFiles(List<string> pdfFiles, string outPdfFileName, out string err)
        {
            bool result = false;
            int count = 0;
            err = "";

            try
            {
                if (isLibraryLoaded == false)
                {
                    result = LoadPDFLib();
                    if (result == false)
                    {
                        return result;
                    }
                }

                if (pdfFiles != null)
                {
                    foreach (string fileName in pdfFiles)
                        pdfLib.AddToFileList("PDFList", fileName);

                    if (string.IsNullOrWhiteSpace(outPdfFileName))
                        outPdfFileName = "MergedPdf.pdf";

                    count = pdfLib.MergeFileList("PDFList", Path.Combine(Path.GetDirectoryName(pdfFiles[0]), outPdfFileName));

                   
                    //int FileHandle = pdfLib.DAOpenFileReadOnly(Path.Combine(Path.GetDirectoryName(pdfFiles[0]), outPdfFileName), "");
                    //if (FileHandle == 0)
                    //{
                    //    err += "Debenu Error Code " + pdfLib.LastErrorCode().ToString();
                    //    //LogManager.Trace("PWPPDFMgr::ExtractPDFPages", TraceCategoryType.Error, "DAOpenFileReadOnly failed - Unable to open file-srcpath", srcpath, "errorcode", errorcode.ToString());
                    //    return result;
                    //}

                    //pdfLib.ClearPageLabels();
                    //pdfLib.AddPageLabels(0, 2, 1, "Page ");

                    //if (FileHandle != -1)
                    //{
                    //    pdfLib.DACloseFile(FileHandle);
                    //}

                    result = (count == pdfFiles.Count);
                }
            }
            catch (Exception ex)
            {
                err += ex.Message + "\n";
                err += pdfLib.LastErrorCode();
            }
            return result;
        }

        public bool ExtractAllTextFromDwg(string inPdfFileName, string outTxtFileName, out string err)
        {
            bool result = false;
            string annotType = "";
            string contents = "";
            err = "";

            try
            {
                if (isLibraryLoaded == false)
                {
                    result = LoadPDFLib();
                    if (result == false)
                    {
                        return result;
                    }
                }

                int docId = pdfLib.LoadFromFile(inPdfFileName, "");

                if (docId == 0)   //failed
                {
                    err += pdfLib.LastErrorCode();
                    return false;
                }

                pdfLib.SelectDocument(docId);

                for (int pageNo = 1; pageNo <= pdfLib.PageCount(); ++pageNo)    //Model contains no text
                {
                    pdfLib.SelectPage(pageNo);

                    for (int i = 1; i < pdfLib.AnnotationCount(); ++i)
                    {
                        annotType = pdfLib.GetAnnotStrProperty(i, 101);

                        if (annotType.Equals("Square"))
                        {
                            contents += pdfLib.GetAnnotStrProperty(i, 102) + "\n"; //102 = contents
                        }
                    }
                }

                if (!string.IsNullOrWhiteSpace(contents))
                {
                    File.WriteAllText(outTxtFileName, contents);
                }
            }
            catch (Exception ex)
            {
                err += ex.Message + "\n";
                err += pdfLib.LastErrorCode();
            }
            return result;
        }
    }
    
}
