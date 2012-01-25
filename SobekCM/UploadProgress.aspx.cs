using System;
using System.Collections;
using System.Configuration;
using System.Data;
using System.Linq;
using System.Web;
using System.Web.Security;
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;
using System.Web.UI.WebControls.WebParts;
using System.Xml.Linq;

using darrenjohnstone.net.FileUpload;

public partial class UploadProgress : System.Web.UI.Page
{
    protected void Page_Load(object sender, EventArgs e)
    {

    }

    protected void Add_Stylesheet_Links()
    {
        string base_url = HttpContext.Current.Request.Url.AbsoluteUri.ToLower().Replace("sobekcm.aspx", "");
        base_url = base_url.Substring(0, base_url.ToLower().IndexOf("uploadprogress.aspx"));
        HttpContext.Current.Response.Output.WriteLine("    <link rel=\"stylesheet\" type=\"text/css\" href=\"" + base_url + "default/scripts/upload_styles/modalbox.css\" />");
        HttpContext.Current.Response.Output.WriteLine("    <link rel=\"stylesheet\" type=\"text/css\" href=\"" + base_url + "default/scripts/upload_styles/uploadstyles.css\" />");
    }

    protected override void OnInit(EventArgs e)
    {
        base.OnInit(e);

        Response.AppendHeader("Cache-Control", "no-cache");
        Response.AppendHeader("Cache-Control", "private");
        Response.AppendHeader("Cache-Control", "no-store");
        Response.AppendHeader("Cache-Control", "must-revalidate");
        Response.AppendHeader("Cache-Control", "max-stale=0");
        Response.AppendHeader("Cache-Control", "post-check=0");
        Response.AppendHeader("Cache-Control", "pre-check=0");
        Response.AppendHeader("Pragma", "no-cache");
        Response.AppendHeader("Keep-Alive", "timeout=3, max=993");
        Response.AppendHeader("Expires", "Mon, 26 Jul 1997 05:00:00 GMT");
    }

    /// <summary>
    /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
    /// </summary>
    /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
    protected override void OnPreRender(EventArgs e)
    {
        UploadStatus status;

        base.OnPreRender(e);

        status = UploadManager.Instance.Status;

        if (status != null)
        {
            upProgressBar.Width = new Unit(status.ProgressPercent, UnitType.Percentage);

            if (status.ProgressPercent > 0)
            {
                lblStatus.Text = "Now uploading: " + status.CurrentFile + " " + status.ProgressPercent.ToString() + "%";
            }
            else
            {
                lblStatus.Text = "Waiting for uploads";
            }
        }
        else
        {
            lblStatus.Text = "Waiting for uploads";
        }
    }
}
