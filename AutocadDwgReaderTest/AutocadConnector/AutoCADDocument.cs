using Autodesk.AutoCAD.Interop;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutocadConnector
{
    public class AutoCADDocument
    {
        private Connector _connector;
        private AcadDocument _doc;

        public AutoCADDocument()
        {
            this._connector = null;
            this._doc = null;
        }
         
        public void OpenDocument(string fileName)
        {
            AcadApplication app = null;

            try
            {
                this._connector = new Connector();
                app = _connector.GetAcadApplication();
                this._doc = app.Documents.Open(fileName, Type.Missing, Type.Missing);
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                app = null;
                this._connector = null;
            }
        }

        public void CloseDocument()
        {
            try
            {
                if (!(_doc == null))
                {
                    _doc.Close(Type.Missing, Type.Missing);
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            finally
            {
                this._connector = null;
                this._doc = null;
            }
        }
    }
}
