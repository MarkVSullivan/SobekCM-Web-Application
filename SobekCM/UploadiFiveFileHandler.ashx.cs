using System;
using System.IO;
using System.Web;
using System.Web.SessionState;

namespace UploadifyProject
{
	/// <summary> Summary description for UploadiFiveFileHandler </summary>
	public class UploadiFiveFileHandler : IHttpHandler, IReadOnlySessionState
	{

		public void ProcessRequest(HttpContext Context)
		{
			Context.Response.ContentType = "text/plain";
			Context.Response.Expires = -1;
			try
			{
				HttpPostedFile postedFile = Context.Request.Files["Filedata"];
				if (postedFile != null)
				{
					string path = Context.Session["Uploadify_Path"].ToString();
					//string savepath = Context.Server.MapPath(relativePath);
					string filename = Path.GetFileName(postedFile.FileName);
					if (!Directory.Exists(path))
						Directory.CreateDirectory(path);

					postedFile.SaveAs(path + @"\" + filename);
					Context.Response.Write(filename);
					Context.Response.StatusCode = 200;
				}
			}
			catch (Exception ex)
			{
				Context.Response.Write("Error: " + ex.Message);
			}
		}

		public bool IsReusable
		{
			get { return false; }
		}
	}
}