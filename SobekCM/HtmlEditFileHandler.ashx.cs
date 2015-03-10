using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace SobekCM
{
    /// <summary> Handler for images uploaded through the HTML editor (CKEditor)
    /// during home page or static browse html pages </summary>
    public class HtmlEditFileHandler : IHttpHandler
    {

        public void ProcessRequest(HttpContext context)
        {
            HttpPostedFile uploads = context.Request.Files["upload"];
            string CKEditorFuncNum = context.Request["CKEditorFuncNum"];
            string file = System.IO.Path.GetFileName(uploads.FileName);
            uploads.SaveAs(context.Server.MapPath(".") + "\\Images\\"+ file);
       
            //provide direct URL here
            string url = "http://localhost/CKeditorDemo/Images/"+ file;  
        
            context.Response.Write("<script>window.parent.CKEDITOR.tools.callFunction("+ CKEditorFuncNum + ", \"" + url + "\");</script>");
           context.Response.End();   
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