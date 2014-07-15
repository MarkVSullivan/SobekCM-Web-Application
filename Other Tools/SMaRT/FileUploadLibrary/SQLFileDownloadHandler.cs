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
    /// An HTTP handler which allows files to be downloaded from a SQL database.
    /// </summary>
    public class SQLFileDownloadHandler : IHttpHandler
    {
        SQLProcessor _processor;

        #region Constructor

        /// <summary>
        /// Initializes a new instance of the <see cref="SQLFileDownloadHandler"/> class.
        /// </summary>
        public SQLFileDownloadHandler()
        {
            _processor = UploadManager.Instance.GetProcessor() as SQLProcessor;

            if (_processor == null)
            {
                throw new Exception("The default processor must be of type SQLProcessor for downloads.");
            }
        }

        #endregion

        #region IHttpHandler Members

        /// <summary>
        /// Gets a value indicating whether another request can use the <see cref="T:System.Web.IHttpHandler"/> instance.
        /// </summary>
        /// <value></value>
        /// <returns>true if the <see cref="T:System.Web.IHttpHandler"/> instance is reusable; otherwise, false.</returns>
        public bool IsReusable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// Enables processing of HTTP Web requests by a custom HttpHandler that implements the <see cref="T:System.Web.IHttpHandler"/> interface.
        /// </summary>
        /// <param name="context">An <see cref="T:System.Web.HttpContext"/> object that provides references to the intrinsic server objects (for example, Request, Response, Session, and Server) used to service HTTP requests.</param>
        public void ProcessRequest(HttpContext context)
        {
            int id;
            string contentType;
            string fileName;

            if (int.TryParse(context.Request["id"], out id))
            {
                if (_processor.GetFileDetails(id, out fileName, out contentType))
                {
                    context.Response.ContentType = contentType;

                    if (context.Request["attach"] == "yes")
                    {
                        context.Response.AddHeader("Content-Disposition", "attachment; filename=\"" + fileName + "\"");
                    }
                    context.Response.Flush();
                    _processor.SaveFileToStream(context.Response.OutputStream, id, UploadManager.Instance.BufferSize);
                    context.Response.Flush();
                }
            }
        }

        #endregion
    }
}
