using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace MVCExport
{

    /// <summary>
    /// CSV file result impementation
    /// </summary>
    /// <typeparam name="TEntity">Entity list to transform to CSV</typeparam>
    public class CsvFileResult<TEntity> : FileResult where TEntity : class
    {
        #region Fields

        private const string DefaultContentType = "text/csv";
        private string _delimiter;
        private string _lineBreak;
        private Encoding _contentEncoding;
        private IEnumerable<string> _headers;
        private IEnumerable<PropertyInfo> _sourceProperties;
        private IEnumerable<TEntity> _dataSource;
        private Func<TEntity, IEnumerable<string>> _map;

        #endregion

        #region Properties

        public Func<TEntity, IEnumerable<string>> Map
        {
            get
            {

                return _map;
            }
            set { _map = value; }
        }
        public IEnumerable<TEntity> DataSource
        {
            get
            {
                return this._dataSource;
            }
        }
        /// <summary>
        /// CSV delimiter default ,
        /// </summary>
        public string Delimiter
        {
            get
            {
                if (string.IsNullOrEmpty(this._delimiter))
                {
                    this._delimiter = CultureInfo.CurrentCulture.TextInfo.ListSeparator;
                }

                return this._delimiter;
            }

            set { this._delimiter = value; }
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
                    this._contentEncoding = Encoding.Unicode;
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
        /// Line  delimiter \n
        /// </summary>
        public string LineBreak
        {
            get
            {
                if (string.IsNullOrEmpty(this._lineBreak))
                {
                    this._lineBreak = Environment.NewLine;
                }

                return this._lineBreak;
            }

            set { this._lineBreak = value; }
        }



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
        public CsvFileResult(IEnumerable<TEntity> source, string fileDonwloadName, string contentType)
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
        public CsvFileResult(IEnumerable<TEntity> source, string fileDonwloadName)
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
        public CsvFileResult(IEnumerable<TEntity> source, string fileDonwloadName, Func<TEntity, IEnumerable<string>> map, IEnumerable<string> headers)
            : this(source, fileDonwloadName, DefaultContentType)
        {
            this._headers = headers;
            this._map = map;
        }

        #endregion

        #region override

        protected override void WriteFile(HttpResponseBase response)
        {
            response.ContentEncoding = this.ContentEncoding;
            response.BufferOutput = this.BufferOutput;
            var streambuffer = ContentEncoding.GetBytes(this.GetCSVData());
            if (HasPreamble)
            {
                var preamble = this.ContentEncoding.GetPreamble();
                response.OutputStream.Write(preamble, 0, preamble.Length);
            }

            response.OutputStream.Write(streambuffer, 0, streambuffer.Length);
        }

        #endregion

        #region local routines

        private string GetCSVHeader()
        {
            string csv = "";
            csv = String.Join(this.Delimiter, this.Headers.Select(x => this.FormatCSV(x)));

            return csv;
        }


        private string GetCSVData()
        {
            string csv = GetCSVHeader();
            Func<TEntity, string> expr = x => this.Map == null ? this.FormatPropertiesCSV(x) : this.FormatMapCSV(x);
            csv += this.LineBreak + String.Join(this.LineBreak, this.DataSource.Select(expr));
            return csv;
        }

        private string FormatCSV(string str)
        {
            str = (str ?? "").Replace(this.Delimiter, "\"" + this.Delimiter + "\"");
            str = str.Replace(this.LineBreak, "\"" + this.LineBreak + "\"");
            str = str.Replace("\"", "\"\"");

            return String.Format("\"{0}\"", str);
        }

        private string FormatPropertiesCSV(TEntity obj)
        {
            string csv = "";

            foreach (var pi in this.SourceProperties)
            {
                string val = GetPropertyValue(pi, obj);
                csv += FormatCSV(val) + this.Delimiter;
            }

            csv = csv.TrimEnd(this.Delimiter.ToCharArray());
            return csv;
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

        private string FormatMapCSV(TEntity obj)
        {
            return String.Join(this.Delimiter, this.Map(obj).Select(x => FormatCSV(x)));
        }


        #endregion

    }
}