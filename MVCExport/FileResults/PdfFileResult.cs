using iTextSharp.text;
using iTextSharp.text.pdf;
using iTextSharp.tool.xml;
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCExport
{
    public class PdfFileResult<TEntity> : FileResult where TEntity : class
    {
        #region Fields

        private const string DefaultContentType = "application/pdf";
        private Encoding _contentEncoding;
        private object _viewModel;
        private ControllerContext _context;
        private string _viewName;

        #endregion

        #region Properties

        /// <summary>
        /// 
        /// </summary>
        public string ViewName
        {
            get { return _viewName; }
            set { _viewName = value; }
        }

        /// <summary>
        /// 
        /// </summary>
        public ControllerContext Context
        {
            get { return _context; }
            set { _context = value; }
        }

        

        /// <summary>
        /// Data list to be transformed to Excel
        /// </summary>
        public object ViewModel
        {
            get
            {
                return this._viewModel;
            }
             set { _viewModel = value; }
        }
        
        /// <summary>
        /// Content Encoding (default is UTF8).
        /// </summary>
        public Encoding ContentEncoding
        {

            get
            {
                if (this._contentEncoding == null)
                {
                    this._contentEncoding = Encoding.UTF8;
                }

                return this._contentEncoding;
            }

            set { this._contentEncoding = value; }


        }

        /// <summary>
        ///  byte order mark (BOM)  .
        /// </summary>
        public bool HasPreamble { get; set; }

        /// <summary>
        /// Get or Set the response output buffer 
        /// </summary>
        public bool BufferOutput { get; set; }

        #endregion

        #region Ctor
         /// <summary>
         /// 
         /// </summary>
         /// <param name="vieModel"></param>
         /// <param name="context"></param>
         /// <param name="viewName"></param>
         /// <param name="fileDonwloadName"></param>
         /// <param name="contentType"></param>
        public PdfFileResult(object viewModel, ControllerContext context, string viewName, string fileDonwloadName, string contentType)
            : base(contentType)
        {
            if (viewModel == null)
                throw new ArgumentNullException("viewModel");
            this._viewModel = viewModel;

            if (string.IsNullOrEmpty(fileDonwloadName))
                throw new ArgumentNullException("fileDonwloadName");
            this.FileDownloadName = fileDonwloadName;

            if (string.IsNullOrEmpty(viewName))
                throw new ArgumentNullException("viewName");
            this._viewName = viewName;

            if (context==null)
                throw new ArgumentNullException("context");
            this._context = context;
            this.BufferOutput = true;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="viewModel"></param>
        /// <param name="context"></param>
        /// <param name="viewName"></param>
        /// <param name="fileDonwloadName"></param>
        public PdfFileResult(object viewModel, ControllerContext context, string viewName, string fileDonwloadName)
            : this(viewModel, context, viewName, fileDonwloadName,DefaultContentType)
        {

        }
        
        #endregion

        protected override void WriteFile(HttpResponseBase response)
        {
            response.ContentEncoding = this.ContentEncoding;
            response.BufferOutput = this.BufferOutput;
           
            if (HasPreamble)
            {
                var preamble = this.ContentEncoding.GetPreamble();
                response.OutputStream.Write(preamble, 0, preamble.Length);
            }

            this.RenderPDFView(response);
        }

        private void RenderPDFView(HttpResponseBase response)
        {
            string htmlView = this.RenderViewToString(this.Context, this.ViewName, this.ViewModel);
            byte[] streambuffer;
            using (var document = new Document())
            {
                using (var workStream = new MemoryStream())
                {
                    PdfWriter writer = PdfWriter.GetInstance(document, workStream);
                    writer.CloseStream = false;
                    document.Open();
                    using (var reader = new StringReader(htmlView))
                    {
                        XMLWorkerHelper.GetInstance().ParseXHtml(writer, document, reader);
                        document.Close();
                        streambuffer = workStream.ToArray() ;
                        response.OutputStream.Write(streambuffer, 0, streambuffer.Length);
                    }
                }
            }
        }

        private string RenderViewToString(ControllerContext context, String viewPath, object model = null)
        {
            context.Controller.ViewData.Model = model;
            using (var sw = new StringWriter())
            {
                var viewResult = ViewEngines.Engines.FindView(context, viewPath, null);
                var viewContext = new ViewContext(context, viewResult.View, context.Controller.ViewData, context.Controller.TempData, sw);
                viewResult.View.Render(viewContext, sw);
                viewResult.ViewEngine.ReleaseView(context, viewResult.View);
                return sw.GetStringBuilder().ToString();
            }
        }
    }
}