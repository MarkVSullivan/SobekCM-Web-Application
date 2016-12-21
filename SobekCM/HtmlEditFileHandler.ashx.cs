using System;
using System.Collections.Generic;
using System.Linq;
using System.IO;
using System.Web;
using System.Web.SessionState;
using SobekCM.Library.Helpers.CKEditor;

namespace SobekCM
{
    /// <summary> Handler for images uploaded through the HTML editor (CKEditor)
    /// during home page or static browse html pages </summary>
    public class HtmlEditFileHandler : IHttpHandler, IReadOnlySessionState
    {

        public void ProcessRequest(HttpContext context)
        {
            // Get the TOKEN from the URL
            if (!String.IsNullOrEmpty(context.Request.QueryString["token"]))
            {
                string token = context.Request.QueryString["token"];
                if (context.Session["#CKEDITOR::" + token] != null)
                {
                    // Get the security token from the session
                    CKEditor_Security_Token tokenObj = context.Session["#CKEDITOR::" + token] as CKEditor_Security_Token;

                    // Get the upload directory from the token and ensure it exists
                    string upload_directory = tokenObj.UploadPath;
                    if (!Directory.Exists(upload_directory))
                        Directory.CreateDirectory(upload_directory);

                    // Save the file
                    HttpPostedFile uploads = context.Request.Files["upload"];
                    string CKEditorFuncNum = context.Request["CKEditorFuncNum"];
                    string file = Path.GetFileName(uploads.FileName);
                    uploads.SaveAs(Path.Combine(upload_directory, file));

                    //provide direct URL here
                    string url = tokenObj.UploadURL + file;

                    // Return the link to the uploaded items
                    context.Response.Write("<script>window.parent.CKEDITOR.tools.callFunction(" + CKEditorFuncNum + ", \"" + url + "\");</script>");
                    context.Response.End();  
                }
            }
        }

        public bool IsReusable
        {
            get
            {
                return false;
            }
        }
    }
}