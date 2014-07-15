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
    /// Controller for uploads. Must be placed before all upload controls on the page.
    /// </summary>
    public class DJUploadController : WebControl
    {
        #region Declarations

        internal static string UPLOAD_ID_TAG = "::DJ_UPLOAD_ID::";
        internal static string UPLOAD_DEFAULT_PARAMETER_TAG = "::DJ_DEFAULT_UPLOAD_PARAMETER::";

        HiddenField _uploadID;
        HiddenField _parameters;
        string _scriptPath;
        string _cssPath;
        string _imagePath;
        string _progressUrl;
        UploadStatus _status;
        bool _showCancelButton;
        string _allowedFileExtentions;
        bool _showProgressBar = true;
        bool _enableManualProcessing = true;
        IFileProcessor _processor;
        UploadConfigurationSection _settings;

        string DEFAULT_IMAGE_PATH = "default/scripts/upload_images";
        string DEFAULT_CSS_PATH = "default/scripts/upload_styles";
        string DEFAULT_JS_PATH = "default/scripts/upload_scripts";
        string DEFAULT_PROGRESS_URL = "UploadProgress.aspx";

        #endregion

        /// <summary>
        /// Gets/sets the default file processor.
        /// </summary>
        public IFileProcessor DefaultFileProcessor
        {
            get { return _processor; }
            set
            {
                _processor = value as IFileProcessor;

                if (_processor == null)
                {
                    throw new ArgumentException("File processor must implement IFileProcessor");
                }
            }
        }

        /// <summary>
        /// Gets or sets the allowed file extensions (a comma separated list .pdf,.zip,.gif).
        /// </summary>
        /// <value>The allowed file extensions.</value>
        public string AllowedFileExtensions
        {
            get { return _allowedFileExtentions; }
            set { _allowedFileExtentions = value; }
        }

        /// <summary>
        /// Gets or sets the upload status.
        /// </summary>
        /// <value>The upload status.</value>
        public UploadStatus Status
        {
            get { return _status; }
            internal set { _status = value; }
        }

        /// <summary>
        /// Gets or sets the path to the script file.
        /// </summary>
        /// <value>The script path.</value>
        public string ScriptPath
        {
            get { return _scriptPath; }
            set { _scriptPath  = value; }
        }

        /// <summary>
        /// Gets or sets the path to the css file.
        /// </summary>
        /// <value>The image path.</value>
        public string CSSPath
        {
            get { return _cssPath; }
            set { _cssPath = value; }
        }

        /// <summary>
        /// Gets or sets the url of the progress page.
        /// </summary>
        /// <value>The URL of the progress page.</value>
        public string ProgressUrl
        {
            get { return _progressUrl; }
            set { _progressUrl = value; }
        }

        /// <summary>
        /// Gets or sets the image path.
        /// </summary>
        /// <value>The image path.</value>
        public string ImagePath
        {
            get { return _imagePath; }
            set { _imagePath = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the cancel button should be shown.
        /// </summary>
        /// <value><c>true</c> if the cancel button should be shown; otherwise, <c>false</c>.</value>
        public bool ShowCancelButton
        {
            get { return _showCancelButton; }
            set { _showCancelButton = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether the progress bar should be shown.
        /// </summary>
        /// <value><c>true</c> if the progress bar should be shown; otherwise, <c>false</c>.</value>
        public bool ShowProgressBar
        {
            get { return _showProgressBar; }
            set { _showProgressBar = value; }
        }

        /// <summary>
        /// Gets or sets a value indicating whether manual processing should be allowed if the
        /// upload module is not installed.
        /// </summary>
        /// <value><c>true</c> if manual processing is allowed; otherwise, <c>false</c>.</value>
        public bool EnableManualProcessing
        {
            get { return _enableManualProcessing; }
            set { _enableManualProcessing = value; }
        }

        /// <summary>
        /// Gets the upload ID.
        /// </summary>
        public string UploadID
        {
            get { return _uploadID == null ? null : _uploadID.Value; }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="DJUploadController"/> class.
        /// </summary>
        public DJUploadController()
        {
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Init"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnInit(EventArgs e)
        {
            base.OnInit(e);
            _settings = UploadConfigurationSection.GetConfig();

            if (String.IsNullOrEmpty(ImagePath))
            {
                ImagePath = _settings == null ? DEFAULT_IMAGE_PATH : _settings.ImagePath;
            }

            ImagePath = ImagePath.TrimEnd('/') + "/";

            if (String.IsNullOrEmpty(CSSPath))
            {
                CSSPath = _settings == null ? DEFAULT_CSS_PATH : _settings.CSSPath;
            }

            CSSPath = CSSPath.TrimEnd('/') + "/";

            if (String.IsNullOrEmpty(ScriptPath))
            {
                ScriptPath = _settings == null ? DEFAULT_JS_PATH : _settings.ScriptPath;
            }

            if (String.IsNullOrEmpty(ProgressUrl))
            {
                ProgressUrl = _settings == null ? DEFAULT_PROGRESS_URL : _settings.ProgressUrl;
            }

            if (_settings != null)
            {
                AllowedFileExtensions = _settings.AllowedFileExtensions;
                ShowCancelButton = _settings.ShowCancelButton;
                ShowProgressBar = _settings.ShowProgressBar;
                EnableManualProcessing = _settings.EnableManualProcessing;
            }

            ScriptPath = ScriptPath.TrimEnd('/') + "/";

            if (UploadManager.Instance.ModuleInstalled)
            {
                _status = UploadManager.Instance.Status;
                UploadManager.Instance.Status = null;
            }
        }

        /// <summary>
        /// Adds a style sheet reference to the page header.
        /// </summary>
        /// <param name="name">The name of the file to link.</param>
        void AddStyleLink(string name)
        {
            //HtmlLink link = new HtmlLink();
            //link.Attributes.Add("type", "text/css");
            //link.Attributes.Add("rel", "stylesheet");
            //link.Attributes.Add("href", CSSPath + name);
            //Page.Header.Controls.Add(link);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.Load"/> event.
        /// </summary>
        /// <param name="e">The <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Page.IsPostBack && !UploadManager.Instance.ModuleInstalled && EnableManualProcessing)
            {
                ManualProcessUploads();
            }

            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "FU_Script", ScriptPath + "fileupload.js");
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "FU_Script1", ScriptPath + "prototype.js");
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "FU_Script2", ScriptPath + "scriptaculous.js?load=effects");
            Page.ClientScript.RegisterClientScriptInclude(this.GetType(), "FU_Script3", ScriptPath + "modalbox.js");
            Page.ClientScript.RegisterStartupScript(this.GetType(), "FU_Init", "up_createDynamicStyles('" + CSSPath + "'); up_initFileUploads('" + ImagePath + "');", true);
            AddStyleLink("modalbox.css");
            AddStyleLink("uploadstyles.css"); // Always add modalbox.css first as uploadstyles.css has overrides

            EnsureChildControls();

            _uploadID.Value = UPLOAD_ID_TAG + Guid.NewGuid().ToString();

            if (ShowProgressBar && UploadManager.Instance.ModuleInstalled)
            {
                Page.ClientScript.RegisterOnSubmitStatement(this.GetType(), "FU_Submit", "up_BeginUpload('" + _uploadID.ClientID + "'," + (ShowCancelButton ? "true" : "false") + ", '" + ProgressUrl + "')");
            }
        }

        /// <summary>
        /// Gets the file processor associated with a control.
        /// </summary>
        /// <returns>The file processor or null if none is found.</returns>
        IFileProcessor GetProcessorForControl(Control c)
        {
            if (c.Parent.Parent.Parent is DJFileUpload)
            {
                return ((DJFileUpload)c.Parent.Parent.Parent).FileProcessor;
            }
            else
            {
                return null;
            }
        }

        /// <summary>
        /// Processes all upload controls in a control collection when the module is not installed.
        /// </summary>
        /// <param name="cc">Control collection.</param>
        /// <param name="defaultProcessor">The default processor.</param>
        /// <param name="status">The upload status.</param>
        void ProcessUploadControls(ControlCollection cc, IFileProcessor defaultProcessor, UploadStatus status)
        {
            foreach (Control c in cc)
            {
                System.Web.UI.WebControls.FileUpload fu = c as System.Web.UI.WebControls.FileUpload;

                if (fu != null && fu.HasFile)
                {
                    IFileProcessor controlProcessor = GetProcessorForControl(fu);
                    IFileProcessor processor = controlProcessor == null ? defaultProcessor : controlProcessor;

                    try
                    {
                        processor.StartNewFile(fu.FileName, fu.PostedFile.ContentType, null, null);
                        processor.Write(fu.FileBytes, 0, fu.FileBytes.Length);
                        processor.EndFile();

                        status.UploadedFiles.Add(new UploadedFile(fu.FileName, processor.GetIdentifier(), null));
                    }
                    catch(Exception ex)
                    {
                        status.ErrorFiles.Add(new UploadedFile(fu.FileName, processor.GetIdentifier(), null, ex));
                    }
                }

                if (c.HasControls())
                {
                    ProcessUploadControls(c.Controls, defaultProcessor, status);
                }
            }
        }

        /// <summary>
        /// Processes file uploads through the processor when the upload module is not installed.
        /// </summary>
        void ManualProcessUploads()
        {
            IFileProcessor processor;
            
            processor = _processor == null ? UploadManager.Instance.GetProcessor() : _processor;

            _status = new UploadStatus(-1);

            if (processor != null)
            {
                ProcessUploadControls(Page.Controls, processor, _status);
            }
        }

        /// <summary>
        /// Called by the ASP.NET page framework to notify server controls that use composition-based implementation to create any child controls they contain in preparation for posting back or rendering.
        /// </summary>
        protected override void CreateChildControls()
        {
            base.CreateChildControls();

            _uploadID = new HiddenField();
            Controls.Add(_uploadID);

            // Create the parameter field
            _parameters = new HiddenField();
            Controls.Add(_parameters);
        }

        /// <summary>
        /// Raises the <see cref="E:System.Web.UI.Control.PreRender"/> event.
        /// </summary>
        /// <param name="e">An <see cref="T:System.EventArgs"/> object that contains the event data.</param>
        protected override void OnPreRender(EventArgs e)
        {
            base.OnPreRender(e);

            if (_processor != null)
            {
                _parameters.Value = UPLOAD_DEFAULT_PARAMETER_TAG + UploadManager.Instance.SerializeProcessor(_processor);
            }
        }

        /// <summary>
        /// Gets the controller from a control collection recursively.
        /// </summary>
        /// <param name="cc">Control collection.</param>
        /// <returns>Controller or null if not found.</returns>
        public static DJUploadController GetControllerFromControls(ControlCollection cc)
        {
            DJUploadController res = null;

            foreach (object o in cc)
            {
                res = o as DJUploadController;

                if (res != null)
                {
                    break;
                }

                Control c = o as Control;

                if (c != null && c.HasControls())
                {
                    res = GetControllerFromControls(c.Controls);

                    if (res != null)
                    {
                        break;
                    }
                }
            }

            return res;
        }

        /// <summary>
        /// Gets the upload controller.
        /// </summary>
        /// <param name="page">The page to check.</param>
        /// <returns>The upload controller.</returns>
        public static DJUploadController GetController(Page page)
        {
            DJUploadController res = null;

            res = GetControllerFromControls(page.Controls);

            if (res == null)
            {
                throw new Exception("An instance of the DJUploadController control must be placed at the beginning of the page before other controls.");
            }

            return res;
        }
    }
}
