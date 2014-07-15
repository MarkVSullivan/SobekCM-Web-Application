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

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Implements an internal HTTPWorkerRequest which is replaced in the request
    /// and allows the upload module to spoof the preloaded content. This method is also
    /// acceptable for IIS 7 integrated mode.
    /// </summary>
    internal class UploadWorkerRequest : HttpWorkerRequest
    {
        HttpWorkerRequest _request;
        byte[] _buffer;

        /// <summary>
        /// Constructor.
        /// </summary>
        /// <param name="request">The original worker request.</param>
        /// <param name="buffer">The content minus the uploaded files.</param>
        public UploadWorkerRequest(HttpWorkerRequest request, byte[] buffer)
        {
            _buffer = buffer;
            _request = request;
        }

        #region Altered methods/properties

        /// <summary>
        /// Reads request data from the client (when not preloaded).
        /// </summary>
        /// <param name="buffer">The byte array to read data into.</param>
        /// <param name="size">The maximum number of bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        public override int ReadEntityBody(byte[] buffer, int size)
        {
            // All content is kept in the preloaded entity body so return 0 here
            return 0;
        }

        /// <summary>
        /// Gets the length of the entire HTTP request body.
        /// </summary>
        /// <returns>
        /// An integer containing the length of the entire HTTP request body.
        /// </returns>
        public override int GetTotalEntityBodyLength()
        {
            return _buffer.Length;
        }

        /// <summary>
        /// Gets the portion of the HTTP request body that has currently been read by using the specified buffer data and byte offset.
        /// </summary>
        /// <param name="buffer">The data to read.</param>
        /// <param name="offset">The byte offset at which to begin reading.</param>
        /// <returns>
        /// The portion of the HTTP request body that has been read.
        /// </returns>
        public override int GetPreloadedEntityBody(byte[] buffer, int offset)
        {
            // Return the buffer
            Buffer.BlockCopy(_buffer, 0, buffer, offset, _buffer.Length);
            return _buffer.Length;
        }

        /// <summary>
        /// Returns the portion of the HTTP request body that has already been read.
        /// </summary>
        /// <returns>
        /// The portion of the HTTP request body that has been read.
        /// </returns>
        public override byte[] GetPreloadedEntityBody()
        {
            return _buffer;
        }

        /// <summary>
        /// Gets the length of the portion of the HTTP request body that has currently been read.
        /// </summary>
        /// <returns>
        /// An integer containing the length of the currently read HTTP request body.
        /// </returns>
        public override int GetPreloadedEntityBodyLength()
        {
            return _buffer.Length;
        }

        /// <summary>
        /// Reads request data from the client (when not preloaded) by using the specified buffer to read from, byte offset, and maximum bytes.
        /// </summary>
        /// <param name="buffer">The byte array to read data into.</param>
        /// <param name="offset">The byte offset at which to begin reading.</param>
        /// <param name="size">The maximum number of bytes to read.</param>
        /// <returns>The number of bytes read.</returns>
        public override int ReadEntityBody(byte[] buffer, int offset, int size)
        {
            return 0;
        }

        /// <summary>
        /// Returns the standard HTTP request header that corresponds to the specified index.
        /// </summary>
        /// <param name="index">The index of the header. For example, the <see cref="F:System.Web.HttpWorkerRequest.HeaderAllow"/> field.</param>
        /// <returns>The HTTP request header.</returns>
        public override string GetKnownRequestHeader(int index)
        {
            if (index == HttpWorkerRequest.HeaderContentLength)
            {
                return _buffer.Length.ToString();
            }
            else
            {
                return _request.GetKnownRequestHeader(index);
            }
        }

        /// <summary>
        /// Returns a value indicating whether all request data is available and no further reads from the client are required.
        /// </summary>
        /// <returns>
        /// true if all request data is available; otherwise, false.
        /// </returns>
        public override bool IsEntireEntityBodyIsPreloaded()
        {
            return true;
        }

        #endregion

        #region Core HttpWorkerRequest methods/properties passed to the internal request without change

        /// <summary>
        /// Terminates the connection with the client.
        /// </summary>
        public override void CloseConnection()
        {
            _request.CloseConnection();
        }

        /// <summary>
        /// Used by the runtime to notify the <see cref="T:System.Web.HttpWorkerRequest"/> that request processing for the current request is complete.
        /// </summary>
        public override void EndOfRequest()
        {
            _request.EndOfRequest();
        }

        /// <summary>
        /// Determines whether the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <param name="obj">The <see cref="T:System.Object"/> to compare with the current <see cref="T:System.Object"/>.</param>
        /// <returns>
        /// true if the specified <see cref="T:System.Object"/> is equal to the current <see cref="T:System.Object"/>; otherwise, false.
        /// </returns>
        /// <exception cref="T:System.NullReferenceException">The <paramref name="obj"/> parameter is null.</exception>
        public override bool Equals(object obj)
        {
            return _request.Equals(obj);
        }

        /// <summary>
        /// Sends all pending response data to the client.
        /// </summary>
        /// <param name="finalFlush">true if this is the last time response data will be flushed; otherwise, false.</param>
        public override void FlushResponse(bool finalFlush)
        {
            _request.FlushResponse(finalFlush);
        }

        /// <summary>
        /// Returns the virtual path to the currently executing server application.
        /// </summary>
        /// <returns>
        /// The virtual path of the current application.
        /// </returns>
        public override string GetAppPath()
        {
            return _request.GetAppPath();
        }

        /// <summary>
        /// Returns the physical path to the currently executing server application.
        /// </summary>
        /// <returns>
        /// The physical path of the current application.
        /// </returns>
        public override string GetAppPathTranslated()
        {
            return _request.GetAppPathTranslated();
        }

        /// <summary>
        /// When overridden in a derived class, returns the application pool ID for the current URL.
        /// </summary>
        /// <returns>Always returns null.</returns>
        public override string GetAppPoolID()
        {
            return _request.GetAppPoolID();
        }

        /// <summary>
        /// Gets the number of bytes read in from the client.
        /// </summary>
        /// <returns>
        /// A Long containing the number of bytes read.
        /// </returns>
        public override long GetBytesRead()
        {
            return _request.GetBytesRead();
        }

        /// <summary>
        /// When overridden in a derived class, gets the certification fields (specified in the X.509 standard) from a request issued by the client.
        /// </summary>
        /// <returns>
        /// A byte array containing the stream of the entire certificate content.
        /// </returns>
        public override byte[] GetClientCertificate()
        {
            return _request.GetClientCertificate();
        }

        /// <summary>
        /// Gets the certificate issuer, in binary format.
        /// </summary>
        /// <returns>
        /// A byte array containing the certificate issuer expressed in binary format.
        /// </returns>
        public override byte[] GetClientCertificateBinaryIssuer()
        {
            return _request.GetClientCertificateBinaryIssuer();
        }

        /// <summary>
        /// When overridden in a derived class, returns the <see cref="T:System.Text.Encoding"/> object in which the client certificate was encoded.
        /// </summary>
        /// <returns>
        /// The certificate encoding, expressed as an integer.
        /// </returns>
        public override int GetClientCertificateEncoding()
        {
            return _request.GetClientCertificateEncoding();
        }

        /// <summary>
        /// When overridden in a derived class, gets a PublicKey object associated with the client certificate.
        /// </summary>
        /// <returns>A PublicKey object.</returns>
        public override byte[] GetClientCertificatePublicKey()
        {
            return _request.GetClientCertificatePublicKey();
        }

        /// <summary>
        /// When overridden in a derived class, gets the date when the certificate becomes valid. The date varies with international settings.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> object representing when the certificate becomes valid.
        /// </returns>
        public override DateTime GetClientCertificateValidFrom()
        {
            return _request.GetClientCertificateValidFrom();
        }

        /// <summary>
        /// Gets the certificate expiration date.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.DateTime"/> object representing the date that the certificate expires.
        /// </returns>
        public override DateTime GetClientCertificateValidUntil()
        {
            return _request.GetClientCertificateValidUntil();
        }

        /// <summary>
        /// When overridden in a derived class, returns the ID of the current connection.
        /// </summary>
        /// <returns>Always returns 0.</returns>
        public override long GetConnectionID()
        {
            return _request.GetConnectionID();
        }

        /// <summary>
        /// When overridden in a derived class, returns the virtual path to the requested URI.
        /// </summary>
        /// <returns>The path to the requested URI.</returns>
        public override string GetFilePath()
        {
            return _request.GetFilePath();
        }

        /// <summary>
        /// Returns the physical file path to the requested URI (and translates it from virtual path to physical path: for example, "/proj1/page.aspx" to "c:\dir\page.aspx")
        /// </summary>
        /// <returns>
        /// The translated physical file path to the requested URI.
        /// </returns>
        public override string GetFilePathTranslated()
        {
            return _request.GetFilePathTranslated();
        }

        /// <summary>
        /// Serves as a hash function for a particular type.
        /// </summary>
        /// <returns>
        /// A hash code for the current <see cref="T:System.Object"/>.
        /// </returns>
        public override int GetHashCode()
        {
            return _request.GetHashCode();
        }

        /// <summary>
        /// Returns the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The HTTP verb returned in the request header.
        /// </returns>
        public override string GetHttpVerbName()
        {
            return _request.GetHttpVerbName();
        }

        /// <summary>
        /// Provides access to the HTTP version of the request (for example, "HTTP/1.1").
        /// </summary>
        /// <returns>
        /// The HTTP version returned in the request header.
        /// </returns>
        public override string GetHttpVersion()
        {
            return _request.GetHttpVersion();
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The server IP address returned in the request header.
        /// </returns>
        public override string GetLocalAddress()
        {
            return _request.GetLocalAddress();
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>
        /// The server port number returned in the request header.
        /// </returns>
        public override int GetLocalPort()
        {
            return _request.GetLocalPort();
        }

        /// <summary>
        /// Returns additional path information for a resource with a URL extension. That is, for the path /virdir/page.html/tail, the GetPathInfo value is /tail.
        /// </summary>
        /// <returns>
        /// Additional path information for a resource.
        /// </returns>
        public override string GetPathInfo()
        {
            return _request.GetPathInfo();
        }

        /// <summary>
        /// When overridden in a derived class, returns the HTTP protocol (HTTP or HTTPS).
        /// </summary>
        /// <returns>
        /// HTTPS if the <see cref="M:System.Web.HttpWorkerRequest.IsSecure"/> method is true, otherwise HTTP.
        /// </returns>
        public override string GetProtocol()
        {
            return _request.GetProtocol();
        }

        /// <summary>
        /// Returns the query string specified in the request URL.
        /// </summary>
        /// <returns>The request query string.</returns>
        public override string GetQueryString()
        {
            return _request.GetQueryString();
        }

        /// <summary>
        /// When overridden in a derived class, returns the response query string as an array of bytes.
        /// </summary>
        /// <returns>
        /// An array of bytes containing the response.
        /// </returns>
        public override byte[] GetQueryStringRawBytes()
        {
            return _request.GetQueryStringRawBytes();
        }

        /// <summary>
        /// Returns the URL path contained in the request header with the query string appended.
        /// </summary>
        /// <returns>The raw URL path of the request header.</returns>
        public override string GetRawUrl()
        {
            return _request.GetRawUrl();
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>The client's IP address.</returns>
        public override string GetRemoteAddress()
        {
            return _request.GetRemoteAddress();
        }

        /// <summary>
        /// When overridden in a derived class, returns the name of the client computer.
        /// </summary>
        /// <returns>The name of the client computer.</returns>
        public override string GetRemoteName()
        {
            return _request.GetRemoteName();
        }

        /// <summary>
        /// Provides access to the specified member of the request header.
        /// </summary>
        /// <returns>The client's HTTP port number.</returns>
        public override int GetRemotePort()
        {
            return _request.GetRemotePort();
        }

        /// <summary>
        /// When overridden in a derived class, returns the reason for the request.
        /// </summary>
        /// <returns>
        /// Reason code. The default is ReasonResponseCacheMiss.
        /// </returns>
        public override int GetRequestReason()
        {
            return _request.GetRequestReason();
        }

        /// <summary>
        /// When overridden in a derived class, returns the name of the local server.
        /// </summary>
        /// <returns>The name of the local server.</returns>
        public override string GetServerName()
        {
            return _request.GetServerName();
        }

        /// <summary>
        /// Returns a single server variable from a dictionary of server variables associated with the request.
        /// </summary>
        /// <param name="name">The name of the requested server variable.</param>
        /// <returns>The requested server variable.</returns>
        public override string GetServerVariable(string name)
        {
            return _request.GetServerVariable(name);
        }

        /// <summary>
        /// Returns a nonstandard HTTP request header value.
        /// </summary>
        /// <param name="name">The header name.</param>
        /// <returns>The header value.</returns>
        public override string GetUnknownRequestHeader(string name)
        {
            return _request.GetUnknownRequestHeader(name);
        }

        /// <summary>
        /// Get all nonstandard HTTP header name-value pairs.
        /// </summary>
        /// <returns>An array of header name-value pairs.</returns>
        public override string[][] GetUnknownRequestHeaders()
        {
            return _request.GetUnknownRequestHeaders();
        }

        /// <summary>
        /// Returns the virtual path to the requested URI.
        /// </summary>
        /// <returns>The path to the requested URI.</returns>
        public override string GetUriPath()
        {
            return _request.GetUriPath();
        }

        /// <summary>
        /// When overridden in a derived class, returns the context ID of the current connection.
        /// </summary>
        /// <returns>Always returns 0.</returns>
        public override long GetUrlContextID()
        {
            return _request.GetUrlContextID();
        }

        /// <summary>
        /// When overridden in a derived class, returns the client's impersonation token.
        /// </summary>
        /// <returns>
        /// A value representing the client's impersonation token. The default is 0.
        /// </returns>
        public override IntPtr GetUserToken()
        {
            return _request.GetUserToken();
        }

        /// <summary>
        /// Gets the impersonation token for the request virtual path.
        /// </summary>
        /// <returns>
        /// An unmanaged memory pointer for the token for the request virtual path.
        /// </returns>
        public override IntPtr GetVirtualPathToken()
        {
            return _request.GetVirtualPathToken();
        }

        /// <summary>
        /// Returns a value indicating whether HTTP response headers have been sent to the client for the current request.
        /// </summary>
        /// <returns>
        /// true if HTTP response headers have been sent to the client; otherwise, false.
        /// </returns>
        public override bool HeadersSent()
        {
            return _request.HeadersSent();
        }

        /// <summary>
        /// Returns a value indicating whether the client connection is still active.
        /// </summary>
        /// <returns>
        /// true if the client connection is still active; otherwise, false.
        /// </returns>
        public override bool IsClientConnected()
        {
            return _request.IsClientConnected();
        }

        /// <summary>
        /// Returns a value indicating whether the connection uses SSL.
        /// </summary>
        /// <returns>
        /// true if the connection is an SSL connection; otherwise, false. The default is false.
        /// </returns>
        public override bool IsSecure()
        {
            return _request.IsSecure();
        }

        /// <summary>
        /// Returns the physical path corresponding to the specified virtual path.
        /// </summary>
        /// <param name="virtualPath">The virtual path.</param>
        /// <returns>
        /// The physical path that corresponds to the virtual path specified in the <paramref name="virtualPath"/> parameter.
        /// </returns>
        public override string MapPath(string virtualPath)
        {
            return _request.MapPath(virtualPath);
        }

        /// <summary>
        /// Adds a Content-Length HTTP header to the response for message bodies that are less than or equal to 2 GB.
        /// </summary>
        /// <param name="contentLength">The length of the response, in bytes.</param>
        public override void SendCalculatedContentLength(int contentLength)
        {
            _request.SendCalculatedContentLength(contentLength);
        }

        /// <summary>
        /// Adds a Content-Length HTTP header to the response for message bodies that are greater than 2 GB.
        /// </summary>
        /// <param name="contentLength">The length of the response, in bytes.</param>
        public override void SendCalculatedContentLength(long contentLength)
        {
            _request.SendCalculatedContentLength(contentLength);
        }

        /// <summary>
        /// Adds a standard HTTP header to the response.
        /// </summary>
        /// <param name="index">The header index. For example, <see cref="F:System.Web.HttpWorkerRequest.HeaderContentLength"/>.</param>
        /// <param name="value">The value of the header.</param>
        public override void SendKnownResponseHeader(int index, string value)
        {
            _request.SendKnownResponseHeader(index, value);
        }

        /// <summary>
        /// Adds the contents of the specified file to the response and specifies the starting position in the file and the number of bytes to send.
        /// </summary>
        /// <param name="handle">The handle of the file to send.</param>
        /// <param name="offset">The starting position in the file.</param>
        /// <param name="length">The number of bytes to send.</param>
        public override void SendResponseFromFile(IntPtr handle, long offset, long length)
        {
            _request.SendResponseFromFile(handle, offset, length);
        }

        /// <summary>
        /// Adds the contents of the specified file to the response and specifies the starting position in the file and the number of bytes to send.
        /// </summary>
        /// <param name="filename">The name of the file to send.</param>
        /// <param name="offset">The starting position in the file.</param>
        /// <param name="length">The number of bytes to send.</param>
        public override void SendResponseFromFile(string filename, long offset, long length)
        {
            _request.SendResponseFromFile(filename, offset, length);
        }

        public override void SendResponseFromMemory(byte[] data, int length)
        {
            _request.SendResponseFromMemory(data, length);
        }

        /// <summary>
        /// Adds the specified number of bytes from a block of memory to the response.
        /// </summary>
        /// <param name="data">An unmanaged pointer to the block of memory.</param>
        /// <param name="length">The number of bytes to send.</param>
        public override void SendResponseFromMemory(IntPtr data, int length)
        {
            _request.SendResponseFromMemory(data, length);
        }

        /// <summary>
        /// Specifies the HTTP status code and status description of the response, such as SendStatus(200, "Ok").
        /// </summary>
        /// <param name="statusCode">The status code to send</param>
        /// <param name="statusDescription">The status description to send.</param>
        public override void SendStatus(int statusCode, string statusDescription)
        {
            _request.SendStatus(statusCode, statusDescription);
        }

        /// <summary>
        /// Adds a nonstandard HTTP header to the response.
        /// </summary>
        /// <param name="name">The name of the header to send.</param>
        /// <param name="value">The value of the header.</param>
        public override void SendUnknownResponseHeader(string name, string value)
        {
            _request.SendUnknownResponseHeader(name, value);
        }

        /// <summary>
        /// Registers for an optional notification when all the response data is sent.
        /// </summary>
        /// <param name="callback">The notification callback that is called when all data is sent (out-of-band).</param>
        /// <param name="extraData">An additional parameter to the callback.</param>
        public override void SetEndOfSendNotification(HttpWorkerRequest.EndOfSendNotification callback, object extraData)
        {
            _request.SetEndOfSendNotification(callback, extraData);
        }

        /// <summary>
        /// Returns a <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </summary>
        /// <returns>
        /// A <see cref="T:System.String"/> that represents the current <see cref="T:System.Object"/>.
        /// </returns>
        public override string ToString()
        {
            return _request.ToString();
        }

        /// <summary>
        /// Gets the full physical path to the Machine.config file.
        /// </summary>
        /// <value></value>
        /// <returns>The physical path to the Machine.config file.</returns>
        public override string MachineConfigPath
        {
            get
            {
                return _request.MachineConfigPath;
            }
        }

        /// <summary>
        /// Gets the physical path to the directory where the ASP.NET binaries are installed.
        /// </summary>
        /// <value></value>
        /// <returns>The physical directory to the ASP.NET binary files.</returns>
        public override string MachineInstallDirectory
        {
            get
            {
                return _request.MachineInstallDirectory;
            }
        }

        /// <summary>
        /// Gets the corresponding Event Tracking for Windows trace ID for the current request.
        /// </summary>
        /// <value></value>
        /// <returns>A trace ID for the current ASP.NET request.</returns>
        public override Guid RequestTraceIdentifier
        {
            get
            {
                return _request.RequestTraceIdentifier;
            }
        }

        /// <summary>
        /// Gets the full physical path to the root Web.config file.
        /// </summary>
        /// <value></value>
        /// <returns>The physical path to the root Web.config file.</returns>
        public override string RootWebConfigPath
        {
            get
            {
                return _request.RootWebConfigPath;
            }
        }

        #endregion
    }
}
