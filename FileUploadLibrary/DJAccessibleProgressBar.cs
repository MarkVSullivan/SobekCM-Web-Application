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
using System.Web.UI;
using System.Web.UI.HtmlControls;
using System.Web.UI.WebControls;

namespace darrenjohnstone.net.FileUpload
{
    /// <summary>
    /// Provides a basic progress bar updated by refreshes in an IFrame. This allows
    /// support for browsers without javascript and others where the UI is blocked
    /// during file uploads.
    /// </summary>
    public class DJAccessibleProgressBar : WebControl
    {
        HtmlGenericControl _frame;
        string _progressURL = "UploadProgress.aspx";

        /// <summary>
        /// Gets/sets the URL of the progress page.
        /// </summary>
        public string ProgressURL
        {
            get { return _progressURL; }
            set { _progressURL = value; }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            _frame = new HtmlGenericControl("iframe");
            _frame.Attributes["frameborder"] = "0";
            _frame.Attributes["scrolling"] = "no";
            _frame.Attributes["width"] = "600px";
            _frame.Attributes["height"] = "50px";
            Controls.Add(_frame);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            DJUploadController controller;
   
            controller = DJUploadController.GetController(Page);

            if (controller != null)
            {
                _frame.Attributes["src"] = _progressURL + "?DJUploadStatus=" + controller.UploadID;

                if (controller.ShowProgressBar)
                {
                    Page.ClientScript.RegisterStartupScript(this.GetType(), ID, "up_killProgress('" + ClientID + "')", true);
                }
            }
        }
    }
}
