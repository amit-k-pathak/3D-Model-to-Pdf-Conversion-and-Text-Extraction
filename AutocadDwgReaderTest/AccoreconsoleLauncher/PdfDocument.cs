using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace AccoreconsoleLauncher
{
    public class PdfDocument
    {
        protected int fileIndex;

        #region Structs and Enums

        protected unsafe struct DllPageOutput
        {
            public byte* img_data;
            public int img_width;
            public int img_height;
            public int img_n;
            public int totalNumberofPages;
            public int numberAnnotations;
            public float resolution;
            public int rectangleX0;
            public int rectangleY0;
            public int rectangleX1;
            public int rectangleY1;
            public int flag;
        }

        protected struct UIRenderParameters
        {
            public float resolution;
            public int rectangleX0;
            public int rectangleY0;
            public int rectangleX1;
            public int rectangleY1;
        }

        protected enum TileDimension : int
        {
            TILE512 = 512,
            TILE1024 = 1024,
            TILE2048 = 2048,
            TILE4096 = 4096,
            TILE8192 = 8192
        };

        public struct fz_rect
        {
            public float x0, y0;
            public float x1, y1;
        }

        protected unsafe struct copyInformation
        {
	        public char *copyStr;
	        public int length;
        };

        private enum ERROR : int
        {
            MUPDF_SUCCESS = 0,

	        MUPDF_ABNORMAL_TERMINATION = -1,

	        MUPDF_ERROR_DLL_INITIALIZATION_FAILED = -2,

	        MUPDF_ERROR_FILE_NOT_FOUND = -3,

	        MUPDF_ERROR_FILE_OPENING_FAILED = -4,
	
	        MUPDF_ERROR_FILE_FORMAT_UNKNOWN = -5,

	        MUPDF_ERROR_FILE_IS_CORRUPT = -6,

	        MUPDF_ERROR_FILE_PASSWORD_PROTECTED = -7,

	        MUPDF_ERROR_PAGE_NOT_FOUND = -8,

	        MUPDF_ERROR_FILE_SAVING_FAILED = -9
        }

        //protected enum DocumentType : int
        //{
        //    NONE = 0,
        //    DRAWING = 1,
        //    SPECS = 2
        //}

        //protected enum Rotation : int
        //{
        //    None = 0,
        //    Rotate90ClockWise = 90,
        //    Rotate90AntiClockWise = -90
        //}

        #endregion

        #region DLL entry point functions

        private const string DLL = "MuPDF_Basic_Operation_Dll_1.dll";

        [DllImport(DLL, EntryPoint = "initializeDll")]
        private static extern ERROR initializeDll();

        [DllImport(DLL, EntryPoint = "createPdfReference")]
        private static extern ERROR createPdfReference(int pdfFileIndex, char[] fileName);

        [DllImport(DLL, EntryPoint = "loadPdfPage")]
        private static extern ERROR loadPdfPage(int pdfFileIndex, int pageNumber);

        [DllImport(DLL, EntryPoint = "countPage")]
        private unsafe static extern int countPage(int pdfFileIndex);

        [DllImport(DLL, EntryPoint = "closeDocument")]
        private static extern void closeDocument(int pdfFileIndex);

        [DllImport(DLL, EntryPoint = "extractAndSaveText")]
        private static extern ERROR extractAndSaveText(char[] fileName, int pdfFileNumber, fz_rect rect);

        [DllImport(DLL, EntryPoint = "getDefaultPdfPageDimension")]
        protected static extern fz_rect getDefaultPdfPageDimension(int pdfFileIndex, int pageNumber);

        [DllImport(DLL, EntryPoint = "loadPdfPageForTiles")]
        protected static extern int loadPdfPageForTiles(int pdfFileIndex, int pageNumber);

        [DllImport(DLL, EntryPoint = "renderPdfPage")]
        protected unsafe static extern DllPageOutput renderPdfPage(int pdfFileIndex, int pageNumber, UIRenderParameters* input);

        [DllImport(DLL, EntryPoint = "mergePdfFiles")]
        private static extern ERROR mergePdfFiles(char[] fileName, int numDocs);

        #endregion

        #region Public Methods

        public PdfDocument()
        {
            this.fileIndex = 0;
        }

        private int getDllPdfIndex()
        {
            return fileIndex;   //increment not required as it's not multithreaded
        }

        public void InitDll()
        {
            ERROR err;
            err = (ERROR)initializeDll();

            if (err == ERROR.MUPDF_ERROR_DLL_INITIALIZATION_FAILED)
            {
                Console.Error.WriteLine("MUPDF ERROR : Failed to initialise Dll...");
                throw new Exception("Failed to initialise Dll...\n");
            }
        }

        public int LoadPdfDocument(string pdfFileName)
        {
            ERROR err;
            int dllPdfIndex = getDllPdfIndex();

            err = (ERROR)createPdfReference(dllPdfIndex, (pdfFileName + "\0").ToCharArray());

            
            switch (err)
            {
                case ERROR.MUPDF_ERROR_FILE_NOT_FOUND:
                    Console.Error.WriteLine("MUPDF ERROR : PDF File not found..." + pdfFileName);
                    throw new Exception("PDF File not found..." + pdfFileName);

                case ERROR.MUPDF_ERROR_FILE_FORMAT_UNKNOWN:
                       Console.Error.WriteLine("MUPDF ERROR : PDF Loading failed : Unrecognized format or File formatting Error in file..." + pdfFileName);
                       throw new Exception("Unrecognized format or File formatting Error..." + pdfFileName);

                case ERROR.MUPDF_ERROR_FILE_OPENING_FAILED:
                case ERROR.MUPDF_ERROR_FILE_IS_CORRUPT:
                       Console.Error.WriteLine("MUPDF ERROR : PDF Loading failed : File is corrupt..." + pdfFileName);
                       throw new Exception("Can not open pdf document..." + pdfFileName);
                
                case ERROR.MUPDF_ERROR_FILE_PASSWORD_PROTECTED:
                       Console.Error.WriteLine("MUPDF ERROR : PDF Loading failed : PDF File is password protected : " + pdfFileName);
                       throw new Exception("Can not open pdf document..." + pdfFileName);
                
            }

            return dllPdfIndex;
        }

        public void LoadPdfPage(int dllPdfIndex, string pdfFileName, int pageNumber)
        {
            ERROR err;

            err = (ERROR)loadPdfPage(dllPdfIndex, pageNumber);

            if (err == ERROR.MUPDF_ERROR_PAGE_NOT_FOUND)
            {
                Console.Error.WriteLine("MUPDF ERROR : PDF Page Load failed : Specified Pagenumber: " + pageNumber + " not found in file " + pdfFileName);
                throw new Exception("Specified Pagenumber: " + pageNumber + " not found in file :" + pdfFileName + "\n");
            }
        }

        public fz_rect GetPageDimension(int dllPdfIndex, int pageNumber)
        {
            fz_rect pageRect;
            pageRect = getDefaultPdfPageDimension(dllPdfIndex, pageNumber);
            return pageRect;
        }

        public int GetTotalPageCount(int dllPdfIndex)
        {
            return countPage(dllPdfIndex);
        }

        public void GetAllText(int dllPdfIndex, string savePath, fz_rect pageRect)
        {
            ERROR err;
            err = extractAndSaveText((savePath + "\0").ToCharArray(), dllPdfIndex, pageRect);

            if (err == ERROR.MUPDF_ABNORMAL_TERMINATION)
            {
                //throw new Exception("Failed to extract text from pdf file...");
            }
        }

        public void MergePdfFiles(List<string> pdfFilesToMerge, string outPdfFileName)
        {
            ERROR err;
            
            err = mergePdfFiles((pdfFilesToMerge[0] + "\0").ToCharArray(), 1);

            if (err == ERROR.MUPDF_ABNORMAL_TERMINATION)
            {
                throw new Exception("Merging Failed...");
            }
        }

        public void SplitPdf(string pdfFileName, int fromPage, int toPage)
        {
            //To-Do
        }

        public void ClosePdfDocument(int dllPdfIndex)
        {
            closeDocument(dllPdfIndex);
        }

        #endregion
    }
}
