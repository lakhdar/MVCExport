using System;
using System.Collections.Generic;
using System.Data.OleDb;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Hosting;
using System.Web.Mvc;

namespace MVCExport
{
    public class XlsFileResult<TEntity> : FileResult where TEntity : class
    {
        #region Fields

        private const string DefaultContentType = "application/vnd.ms-excel";
        private string _tempPath;
        private string _tableName;



        private Encoding _contentEncoding;
        private IEnumerable<string> _headers;
        private IEnumerable<PropertyInfo> _sourceProperties;
        private IEnumerable<TEntity> _dataSource;
        private Func<TEntity, IEnumerable<string>> _map;

        #endregion

        #region Properties

        public string TableName
        {
            get
            {

                if (string.IsNullOrEmpty(_tableName))
                {
                    _tableName = typeof(TEntity).Name;
                }

                _tableName = _tableName.Trim().Replace(" ", "_");
                if (_tableName.Length > 30)
                {
                    _tableName = _tableName.Substring(0, 30);
                }

                return _tableName;
            }
            set { _tableName = value; }
        }

        public string TempPath
        {
            get
            {
                if (string.IsNullOrEmpty(_tempPath))
                {
                    _tempPath = HostingEnvironment.MapPath(Path.Combine(@"~/App_Data", this.FileDownloadName));
                }
                return _tempPath;
            }
            set
            {
                _tempPath = Path.Combine(value, this.FileDownloadName);
            }
        }

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
        public XlsFileResult(IEnumerable<TEntity> source, string fileDonwloadName, string contentType)
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
        public XlsFileResult(IEnumerable<TEntity> source, string fileDonwloadName)
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
        public XlsFileResult(IEnumerable<TEntity> source, Func<TEntity, IEnumerable<string>> map, IEnumerable<string> headers, string fileDonwloadName)
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

            this.RenderResponse(response);
        }

        private void RenderResponse(HttpResponseBase response)
        {
            if (File.Exists(this.TempPath))
            {
                File.Delete(this.TempPath);
            }
            string sexcelconnectionstring = GetConnectionString(this.TempPath);
            using (System.Data.OleDb.OleDbConnection oledbconn = new System.Data.OleDb.OleDbConnection(sexcelconnectionstring))
            {
                oledbconn.Open();
                this.createTable(oledbconn);
                this.InsertData(oledbconn);
            }

            var streambuffer = this.ContentEncoding.GetBytes(File.ReadAllText(this.TempPath));

            response.OutputStream.Write(streambuffer, 0, streambuffer.Length);
        }

        private IEnumerable<string> GetEntityValues(TEntity obj)
        {
            IEnumerable<string> ds = null;
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

        private string GetConnectionString(string FileName, bool hasHeaders = true)
        {
            string HDR = hasHeaders ? "Yes" : "No";
            return Path.GetExtension(FileName).Equals(".xlsx") ?
                "Provider=Microsoft.ACE.OLEDB.12.0;Data Source=" + FileName + ";Extended Properties=\"Excel 12.0;HDR=" + HDR + ";IMEX=0\"" :
                "Provider=Microsoft.Jet.OLEDB.4.0;Data Source=" + FileName + ";Extended Properties=\"Excel 8.0;HDR=" + HDR + ";IMEX=0\"";
        }

        private void createTable(OleDbConnection con)
        {
            string tyed = string.Join(",", this.Headers.Select(x => x + " " + "VARCHAR"));
            string commandText = string.Format("CREATE TABLE [{0}]({1});", this.TableName, tyed);
            OleDbCommand oledbcmd = new OleDbCommand(commandText,con);
            oledbcmd.ExecuteNonQuery();
        }

        private void InsertData(OleDbConnection con)
        {
            OleDbDataAdapter oleAdap = new OleDbDataAdapter("SELECT * FROM [" + this.TableName + "]", con);
            OleDbCommandBuilder oleCmdBuilder = new OleDbCommandBuilder(oleAdap);
            oleCmdBuilder.QuotePrefix = "[";
            oleCmdBuilder.QuoteSuffix = "]";
            OleDbCommand cmd = oleCmdBuilder.GetInsertCommand();
            foreach (TEntity row in this.DataSource)
            {
                var pVals = GetEntityValues(row);
                int index = 0;
                foreach (OleDbParameter param in cmd.Parameters)
                {
                    param.Value = pVals.ElementAt(index);
                    index++;
                }

                cmd.ExecuteNonQuery();
            }
        }

        private void InsertDataQuery(OleDbConnection cn)
        {
            StringBuilder sbSql = new StringBuilder();
            sbSql.Length = 0;
            sbSql.Insert(0, "INSERT INTO [" + this.TableName + "]");
            sbSql.Append(" (");
            sbSql.Append(string.Join(",", this.Headers));
            sbSql.Append(")");
            sbSql.Append(string.Join(" UNION ALL ", this.DataSource.Select(x => "  SELECT  " + string.Join(",", GetEntityValues(x)) + " ")));
            sbSql.Append(";");
            OleDbCommand cmd = new OleDbCommand(sbSql.ToString(), cn);
            cmd.ExecuteNonQuery();
        }
    }
}