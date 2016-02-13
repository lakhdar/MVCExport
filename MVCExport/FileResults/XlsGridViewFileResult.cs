using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MVCExport
{
    public class XlsGridViewFileResult<TEntity> : FileResult where TEntity : class
    {
        #region Fields

        private const string DefaultContentType = "application/vnd.ms-excel";
        private Encoding _contentEncoding;
        private IEnumerable<string> _headers;
        private IEnumerable<PropertyInfo> _sourceProperties;
        private IEnumerable<TEntity> _dataSource;
        private Func<TEntity, IEnumerable<string>> _map;

        #endregion

        #region Properties


        /// <summary>
        /// Custom properties transformation
        /// </summary>
        public Func<TEntity, IEnumerable<string>> Map
        {
            get
            {
                return _map;
            }

            set { _map = value; }
        }

        /// <summary>
        /// Data list to be transformed to Excel
        /// </summary>
        public IEnumerable<TEntity> DataSource
        {
            get
            {
                return this._dataSource;
            }
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
        /// the first line of the CSV file, column headers
        /// </summary>
        public IEnumerable<string> Headers
        {
            get
            {
                if (this._headers == null)
                {
                    this._headers = typeof(TEntity).GetProperties().Select(x => x.Name);
                }

                return this._headers;
            }

            set { this._headers = value; }
        }

        /// <summary>
        /// Object's properties to convert to excel  
        /// </summary>
        public IEnumerable<PropertyInfo> SourceProperties
        {
            get
            {
                if (this._sourceProperties == null)
                {
                    this._sourceProperties = typeof(TEntity).GetProperties();
                }

                return this._sourceProperties;
            }
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
        /// Creats new instance of CsvFileResult{TEntity}
        /// </summary>
        /// <param name="source">List of data to be transformed to csv</param>
        /// <param name="fileDonwloadName">CSV file name</param>
        /// <param name="contentType">Http response content type</param>
        public XlsGridViewFileResult(IEnumerable<TEntity> source, string fileDonwloadName, string contentType)
            : base(contentType)
        {
            if (source == null)
                throw new ArgumentNullException("source");
            this._dataSource = source;

            if (string.IsNullOrEmpty(fileDonwloadName))
                throw new ArgumentNullException("fileDonwloadName");
            this.FileDownloadName = fileDonwloadName;

            this.BufferOutput = true;
        }

        /// <summary>
        /// Creats new instance of CsvFileResult{TEntity}
        /// </summary>
        /// <param name="source">List of data to be transformed to csv</param>
        /// <param name="fileDonwloadName">CSV file name</param>
        public XlsGridViewFileResult(IEnumerable<TEntity> source, string fileDonwloadName)
            : this(source, fileDonwloadName, DefaultContentType)
        {

        }

        /// <summary>
        /// Creats new instance of CsvFileResult{TEntity}
        /// </summary>
        /// <param name="source">List of data to be transformed to csv</param>
        /// <param name="fileDonwloadName">CSV file name</param>
        /// <param name="map">Custom transformation delegate</param>
        /// <param name="headers">Columns headers</param>
        public XlsGridViewFileResult(IEnumerable<TEntity> source,  Func<TEntity, IEnumerable<string>> map, IEnumerable<string> headers,string fileDonwloadName)
            : this(source, fileDonwloadName, DefaultContentType)
        {
            this._headers = headers;
            this._map = map;
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
        
            this.RenderGridView(response);
        }

        private void RenderGridView(HttpResponseBase response)
        {
            var grid = new System.Web.UI.WebControls.GridView();
            if (this.Map != null)
            {
                grid.DataSource = this.DataSource.Select(this.Map);
            }
            else
            {
                grid.DataSource = this.DataSource;
            }

            grid.DataBind();

            StringWriter sw = new StringWriter();
            HtmlTextWriter htw = new HtmlTextWriter(sw);

            grid.RenderControl(htw);
            response.Write(sw.ToString());
        }

        private IEnumerable<string> GetEntityValues(TEntity obj)
        {
            IEnumerable<string> ds = null; ;
            if (this.Map != null)
            {
                ds = this.Map(obj);
            }
            else
            {
              ds = this.SourceProperties.Select(x => this.GetPropertyValue(x, obj));
               
            }
            return ds;
        }

        private string GetPropertyValue(PropertyInfo pi, object source)
        {
            try
            {
                var result = pi.GetValue(source, null);
                return (result == null) ? "" : result.ToString();
            }
            catch (Exception)
            {
                return "Can not obtain the value";
            }
        }

         
    }
}