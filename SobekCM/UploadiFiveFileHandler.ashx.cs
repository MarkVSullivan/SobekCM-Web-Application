using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.SessionState;
using SobekCM.Library.UploadiFive;

namespace SobekCM
{
	/// <summary> Handler for the uploading files </summary>
	public class UploadiFiveFileHandler : IHttpHandler, IReadOnlySessionState
	{
		/// <summary> Process this request </summary>
		/// <param name="Context"></param>
		public void ProcessRequest(HttpContext Context)
		{
			Context.Response.ContentType = "text/plain";
			Context.Response.Expires = -1;

			// Try to get the security token key
			string tokenKey = Context.Request["token"];
			if (tokenKey == null)
			{
				Context.Response.Write("No token provided with this request");
				Context.Response.StatusCode = 401;
				return;
			}

			// Try to get the matching token object from the session
			UploadiFive_Security_Token tokenObj = Context.Session["#UPLOADIFIVE::" + tokenKey] as UploadiFive_Security_Token;
			if (tokenObj == null)
			{
				Context.Response.Write("No matching server-side token found for this request");
				Context.Response.StatusCode = 401;
				return;
			}

			try
			{
				// Get the posted file from the appropriate file key
				HttpPostedFile postedFile = Context.Request.Files[ tokenObj.FileObjName ];
				if (postedFile != null)
				{
					// Get the path from the token and ensure it exists
					string path = tokenObj.UploadPath;
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					// Get the filename for the uploaded file
					string filename = Path.GetFileName(postedFile.FileName);

					// Are there file extension restrictions?
					if ( !String.IsNullOrEmpty(tokenObj.AllowedFileExtensions))
					{
						string extension = Path.GetExtension(postedFile.FileName).ToLower();
						List<string> allowed = tokenObj.AllowedFileExtensions.Split("|,".ToCharArray()).ToList();
						if (!allowed.Contains(extension))
						{
							Context.Response.Write("Invalid extension");
							Context.Response.StatusCode = 401;
							return;
						}
					}

					// Save this file locally
					postedFile.SaveAs(path + @"\" + filename);

					// Post a successful status
					Context.Response.Write(filename);
					Context.Response.StatusCode = 200;
				}
			}
			catch (Exception ex)
			{
				Context.Response.Write("Error: " + ex.Message);
				Context.Response.StatusCode = 500;
			}
		}

		public bool IsReusable
		{
			get { return true; }
		}
	}
}