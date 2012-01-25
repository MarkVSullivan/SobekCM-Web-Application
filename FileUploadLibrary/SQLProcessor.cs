#region License
/*

File Upload HTTP module for ASP.Net (v 2.0)
Copyright (C) 2007-2008 Darren Johnstone (http://darrenjohnstone.net)

This library is free software; you can redistribute it and/or
modify it under the terms of the GNU Lesser General Public
License as published by the Free Software Foundation; either
version 2.1 of the License, or (at your option) any later version.

This library is distributed in the hope that it will be useful,
but WITHOUT ANY WARRANTY; without even the implied warranty of
MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the GNU
Lesser General Public License for more details.

You should have received a copy of the GNU Lesser General Public
License along with this library; if not, write to the Free Software
Foundation, Inc., 51 Franklin Street, Fifth Floor, Boston, MA  02110-1301  USA

*/
#endregion

using System;
using System.Collections.Generic;
using System.Text;
using System.Web;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Data;
using System.Data.SqlClient;
using System.IO;
using System.Runtime.Serialization;

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Implements the IFileSystemProcessor interface to provide a processor for streaming uploads into a SQL database.
    /// </summary>
    [Serializable()]
    public class SQLProcessor : IFileProcessor
    {
        #region Declarations

        [NonSerialized()]
        SqlConnection _connection;

        [NonSerialized()]
        SqlTransaction _tran;

        [NonSerialized()]
        string _fileName;

        [NonSerialized()]
        string _contentType;

        string _tableName = "UploadedFile";

        [NonSerialized()]
        string _connectionString = null;

        string _connectionConfig = null;

        [NonSerialized()]
        bool _errorState;

        [NonSerialized()]
        Dictionary<string, string> _headerItems;

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the name of the connection string config setting.
        /// </summary>
        /// <value>The connection string config name.</value>
        public string ConnectionConfig
        {
            get { return _connectionConfig; }
            set { _connectionConfig = value; }
        }

        /// <summary>
        /// Gets or sets the connection string.
        /// </summary>
        /// <value>The connection string.</value>
        public string ConnectionString
        {
            get { return _connectionString; }
            set { _connectionString = value; }
        }

        /// <summary>
        /// Gets/sets the table name to store the files in.
        /// </summary>
        public string TableName
        {
            get { return _tableName; }
            set { _tableName = value; }
        }

        #endregion

        #region Constructor

        /// <summary>
        /// Constructor.
        /// </summary>
        public SQLProcessor()
        {
        }

        #endregion

        #region Data access

        /// <summary>
        /// Gets the connection string.
        /// </summary>
        /// <returns>The connection string.</returns>
        string GetConnectionString()
        {
            return _connectionConfig != null ? System.Configuration.ConfigurationManager.ConnectionStrings[_connectionConfig].ConnectionString : _connectionString;
        }

        /// <summary>
        /// Creates a SQL command object which performs an initial insert
        /// of the blob row into the database.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="contentType">Content type.</param>
        /// <returns>A SQL command which creates the initial row in the database.</returns>
        protected virtual SqlCommand CreateInitialInsertCommand(string fileName, string contentType)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "INSERT INTO " + TableName + " (FileName, ContentType, FileContents) " +
                "Values(@FileName, @ContentType, 0x0);" +
                "SELECT @Identity = SCOPE_IDENTITY();" +
                "SELECT @Pointer = TEXTPTR(FileContents) FROM UploadedFile WHERE Id = @Identity";

            cmd.Parameters.Add(new SqlParameter("@FileName", System.IO.Path.GetFileName(fileName)));
            cmd.Parameters.Add(new SqlParameter("@ContentType", contentType));

            SqlParameter idParm = cmd.Parameters.Add("@Identity", SqlDbType.Int);
            idParm.Direction = ParameterDirection.Output;

            SqlParameter ptrParm = cmd.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
            ptrParm.Direction = ParameterDirection.Output;

            return cmd;
        }

        /// <summary>
        /// Creates a SQL command object which appends incoming bytes onto the blob field.
        /// </summary>
        /// <param name="pointer">SQL pointer to the blob.</param>
        /// <param name="offset">Offset in the blob.</param>
        /// <param name="bytes">The bytes to write.</param>
        /// <returns>A SQL command object which appends the bytes to the blob field.</returns>
        protected virtual SqlCommand CreateBlobAppendCommand(byte[] pointer, long offset, byte[] bytes)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "UPDATETEXT " + TableName + ".FileContents @Pointer @Offset 0 @Bytes";

            SqlParameter ptrParm = cmd.Parameters.Add("@Pointer", SqlDbType.Binary, 16);
            ptrParm.Value = pointer;
            
            SqlParameter bytesParam = cmd.Parameters.Add("@Bytes", SqlDbType.Image, bytes.Length);
            bytesParam.Value = bytes;
            
            SqlParameter offsetParm = cmd.Parameters.Add("@Offset", SqlDbType.Int);
            offsetParm.Value = offset;

            return cmd;
        }

        /// <summary>
        /// Creates a SQL command which reads the file name, content type, and image column of a file based on the ID.
        /// </summary>
        /// <param name="id">The ID to retrieve.</param>
        /// <returns>A SQL command object which gets the image from the database.</returns>
        protected virtual SqlCommand CreateSelectCommand(int id)
        {
            SqlCommand cmd = new SqlCommand();

            cmd.CommandText = "SELECT FileName, ContentType, FileContents FROM " + TableName + " WHERE Id=@ID";
            cmd.Parameters.Add(new SqlParameter("@ID", id));

            return cmd;
        }

        /// <summary>
        /// Closes the connection and cleans up.
        /// </summary>
        /// <param name="commit">True to commit the transaction.</param>
        void CleanUp(bool commit)
        {
            if (_tran != null)
            {
                if (commit)
                {
                    _tran.Commit();
                }
                else
                {
                    _tran.Rollback();
                }

                _tran = null;
            }

            if (_connection != null)
            {
                _connection.Close();
                _connection.Dispose();
                _connection = null;
            }
        }



        #endregion

        #region IFileProcessor Members

        byte[] _pointer = null;
        long _blobOffset = 0;
        int _rowId;

        /// <summary>
        /// Starts a new file.
        /// </summary>
        /// <param name="fileName">File name.</param>
        /// <param name="contentType">The content type of the file.</param>
        /// <param name="headerItems">A dictionary of items pulled from the header of the field.</param>
        /// <param name="previousFields">A dictionary of previous fields.</param>
        /// <returns>An optional object used to identify the item in the storage container.</returns>
        public object StartNewFile(string fileName, string contentType, Dictionary<string, string> headerItems, Dictionary<string, string> previousFields)
        {
            SqlCommand insertCommand;

            _rowId = -1;
            _errorState = false;

            _fileName = fileName;
            _headerItems = headerItems;
            _contentType = contentType;
            _blobOffset = 0;

            try
            {
                _connection = new SqlConnection(GetConnectionString());
                _connection.Open();
                _tran = _connection.BeginTransaction();

                insertCommand = CreateInitialInsertCommand(fileName, contentType);
                insertCommand.Connection = _connection;
                insertCommand.Transaction = _tran;
                insertCommand.ExecuteNonQuery();

                _pointer = (byte[])insertCommand.Parameters["@Pointer"].Value;
                _rowId = (int)insertCommand.Parameters["@Identity"].Value;

            }
            catch(Exception ex)
            {
                _errorState = true;
                CleanUp(false);
                throw ex;
            }

            return _rowId;
        }

        /// <summary>
        /// Writes to the output file.
        /// </summary>
        /// <param name="buffer">Buffer to write from.</param>
        /// <param name="offset">Offset in the buffer to write from.</param>
        /// <param name="count">Count of bytes to write.</param>
        public void Write(byte[] buffer, int offset, int count)
        {
            SqlCommand blobCommand;
            byte[] toWrite;
            
            if (_errorState) return;

            toWrite = new byte[count];
            Buffer.BlockCopy(buffer, offset, toWrite, 0, count);

            blobCommand = CreateBlobAppendCommand(_pointer, _blobOffset, toWrite);

            try
            {
                blobCommand.Connection = _connection;
                blobCommand.Transaction = _tran;
                blobCommand.ExecuteNonQuery();
                _blobOffset += count;
            }
            catch(Exception ex)
            {
                _errorState = true;
                CleanUp(false);
                throw ex;
            }
        }

        /// <summary>
        /// Gets the file name and content type of the file.
        /// </summary>
        /// <param name="id">The ID of the file to get.</param>
        /// <param name="fileName">File name.</param>
        /// <param name="contentType">Content type.</param>
        /// <returns>True if the file is found, otherwise false.</returns>
        public virtual bool GetFileDetails(int id, out string fileName, out string contentType)
        {
            SqlConnection conn = null;
            SqlCommand selectCommand = CreateSelectCommand(id);
            SqlDataReader reader = null;

            try
            {
                conn = new SqlConnection(GetConnectionString());
                conn.Open();
                selectCommand.Connection = conn;
                reader = selectCommand.ExecuteReader(CommandBehavior.SingleRow);

                if (reader.Read())
                {
                    fileName = reader.GetString(0);
                    contentType = reader.GetString(1);
                    return true;
                }
                else
                {
                    contentType = null;
                    fileName = null;
                    return false;
                }
            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Gets the file from the database and writes it to a stream.
        /// </summary>
        /// <param name="stream">Stream to write to.</param>
        /// <param name="id">The id of the record to get.</param>
        /// <param name="blockSize">The size of blocks to stream the data in.</param>
        public virtual void SaveFileToStream(Stream stream, int id, int blockSize)
        {
            SqlConnection conn = null;
            SqlCommand selectCommand = CreateSelectCommand(id);
            SqlDataReader reader = null;

            try
            {
                long count;
                long index = 0;
                byte[] buffer = new byte[blockSize];

                conn = new SqlConnection(GetConnectionString());
                conn.Open();
                selectCommand.Connection = conn;
                reader = selectCommand.ExecuteReader(CommandBehavior.SequentialAccess);

                while (reader.Read())
                {
                    count = reader.GetBytes(2, index, buffer, 0, blockSize);

                    while (count > 0)
                    {
                        stream.Write(buffer, 0, (int)count);
                        index += count;
                        count = reader.GetBytes(2, index, buffer, 0, blockSize);
                    }
                }

            }
            finally
            {
                if (reader != null)
                {
                    reader.Close();
                    reader.Dispose();
                }

                if (conn != null)
                {
                    conn.Close();
                    conn.Dispose();
                }
            }
        }

        /// <summary>
        /// Ends current file processing.
        /// </summary>
        public void EndFile()
        {
            if (_errorState) return;

            CleanUp(true);
        }

        /// <summary>
        /// Returns the name of the file that is currently being processed.
        /// Null if there is no file.
        /// </summary>
        /// <returns>The file name.</returns>
        public string GetFileName()
        {
            return _fileName;
        }

        /// <summary>
        /// Returns the container identifier.
        /// </summary>
        /// <returns>The container identifier.</returns>
        public virtual object GetIdentifier()
        {
            return _rowId;
        }

        /// <summary>
        /// Gets the header items.
        /// </summary>
        public Dictionary<string, string> GetHeaderItems()
        {
            return _headerItems;
        }

        #endregion

        #region IDisposable Members

        /// <summary>
        /// Performs application-defined tasks associated with freeing, releasing, or resetting unmanaged resources.
        /// </summary>
        public void Dispose()
        {
            CleanUp(false);
        }

        #endregion
    }
}
