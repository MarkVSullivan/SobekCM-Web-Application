#region Using directives

using System;
using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

#endregion

namespace SobekCM.Library.Helpers.UploadiFive
{
	/// <summary> Control is used for uploading files through the UploadiFive library </summary>
	public class UploadiFiveControl : WebControl
	{
		// Stores all the user settings
		private readonly UploadiFive_Settings settings;

		/// <summary> Constructor for a new instance of the UploadiFiveControl class </summary>
		public UploadiFiveControl()
		{
			// Configure the settings object
			settings = new UploadiFive_Settings();
		}
				
		/// <summary> Writes the file input and script necessary for the upload of the files </summary>
		/// <param name="Output"> Stream to write to </param>
		protected override void RenderContents(HtmlTextWriter Output)
		{
			// If there is no current HTTPContext, can't do this...
			if ((UploadPath.Length > 0) && (HttpContext.Current != null))
			{
				// Create a new security token, save in session, and set token GUID in the form data
                UploadiFive_Security_Token newToken = new UploadiFive_Security_Token(UploadPath, AllowedFileExtensions, FileObjName, ServerSideFileName, ReturnToken);
				FormData["token"] = newToken.ThisGuid.ToString();
				HttpContext.Current.Session["#UPLOADIFIVE::" + newToken.ThisGuid.ToString()] = newToken;
			}

			// Add the file input element
			Output.Write("<input id=\"" + FileInputID + "\" name=\"" + FileInputID + "\" ");
			if (FileInputClass.Length > 0)
				Output.Write("class=\"" + FileInputClass + "\" ");
			Output.WriteLine("type=\"file\" />");
			Output.WriteLine();

			// Add the script for this to be added to the ready document event
			Output.WriteLine("<script type=\"text/javascript\">");
			Output.WriteLine("  $(document).ready(function() {");

			// Allow the settings object to write the actual jquery
			settings.Add_To_Stream(Output, String.Empty, String.Empty);

			Output.WriteLine("  });");
			Output.WriteLine("</script>");
			Output.WriteLine();
		}

		#region Properties specific to the ASP.net implementation

		/// <summary> Version being utilized, either the HTML5 version (UploadiFive)
		/// or the FLASH version (Uploadify) </summary>
		/// <value> Default is to use the HTML5 version </value>
		public UploadiFive_Version_Enum Version
		{
			get { return settings.Version; }
			set { settings.Version = value; }
		}

		/// <summary> ID to use for the file input element </summary>
		/// <value> Default value is 'file_upload' </value>
		public string FileInputID
		{
			get { return settings.FileInputID; }
			set { settings.FileInputID = value; }
		}

		/// <summary> Class to use for the file input element </summary>
		/// <value> Default value is 'file_upload' </value>
		public string FileInputClass
		{
			get { return settings.FileInputClass; }
			set { settings.FileInputClass = value; }
		}

		/// <summary> Flag indicates if the secure file handler option should be used </summary>
		/// <remarks> This sets a value in the session state and then checks it upon upload </remarks>
		public bool UseSecureFileHandler
		{
			get { return settings.UseSecureFileHandler; }
			set { settings.UseSecureFileHandler = value; }
		}

		/// <summary> Flag indicates if the form should be submitted once the uploaded queue completes </summary>
		public bool SubmitWhenQueueCompletes
		{
			get { return settings.SubmitWhenQueueCompletes; }
			set { settings.SubmitWhenQueueCompletes = value; }
		}

		/// <summary> Path where the uploaded files should go </summary>
		public string UploadPath
		{
			get { return settings.UploadPath; }
			set { settings.UploadPath = value; }
		}

		/// <summary> List of file extensions allowed </summary>
		public string AllowedFileExtensions
		{
			get { return settings.AllowedFileExtensions; }
			set { settings.AllowedFileExtensions = value; }
		}

		/// <summary> If a user attempts to upload a disallowed file extension, this is the
		/// message that will be popped up to the user </summary>
		/// <remarks> You can use '&lt;extension&gt;' in the string to have the extension
		/// of the attempted file in your message.  Setting this to an empty string will 
		/// cause no alert to happen clientside, but the upload will be cancelled. </remarks>
		/// <value> Default value is 'File types of '&lt;extension&gt;' are not allowed' </value>
		public string DisallowedFileExtenstionMessage
		{
			get { return settings.DisallowedFileExtenstionMessage; }
			set { settings.DisallowedFileExtenstionMessage = value; }
		}

		/// <summary> Flag indicates that if the client does not have HTML5 with their browser, to revert
		/// to the Flash version, if it is available </summary>
		/// <value> Default value is FALSE </value>
		public bool RevertToFlashVersion
		{
			get { return settings.RevertToFlashVersion; }
			set { settings.RevertToFlashVersion = value; }
		}

		/// <summary> If this is set, this class will be used for the button when the HTML5 version 
		/// has reverted to the flash version. </summary>
		/// <value> Can be useful to see if it reverts </value>
		public string RevertedButtonClass
		{
			get { return settings.RevertedButtonClass; }
			set { settings.RevertedButtonClass = value; }
		}

		/// <summary> Message displayed if the user has neither HTML5 on their browse nor Flash installed </summary>
		/// <value> Default value is 'Your browse must either be HTML5-compliant or have Adobe Flash installed to use this upload feature'.</value>
		public string NoHtml5OrFlashMessage
		{
			get { return settings.NoHtml5OrFlashMessage; }
			set { settings.NoHtml5OrFlashMessage = value; }
		}

		/// <summary> Name for the final server-side file, which allows overriding the default name,
		/// which would otherwise match the uploaded name </summary>
		/// <remarks> This can be used to avoid having to manually rename the file after upload </remarks>
		public string ServerSideFileName
		{
			get { return settings.ServerSideFileName; }
			set { settings.ServerSideFileName = value; }
		}

        /// <summary> Return token is used to pass back the information about which file(s) were uploaded </summary>
        public string ReturnToken
        {
            get { return settings.ReturnToken; }
            set { settings.ReturnToken = value; }
        }

		#endregion

		#region Exposing existing Options from UploadiFive

		/// <summary> If set to true, files will automatically upload when added to the queue </summary>
		/// <value> Default value from UploadiFive is TRUE </value>
		public bool? Auto
		{
			get { return settings.Auto; }
			set { settings.Auto = value; }
		}

		/// <summary> A class name to add to the UploadiFive button DOM element </summary>
		public string ButtonClass
		{
			get { return settings.ButtonClass; }
			set { settings.ButtonClass = value; }
		}

		/// <summary> The text to display inside the browse button </summary>
		/// <remarks> This text is rendered as HTML and may contain tags or HTML entities. </remarks>
		public string ButtonText
		{
			get { return settings.ButtonText; }
			set { settings.ButtonText = value; }
		}

		/// <summary> The path to the server-side files that checks whether a files with the 
		/// same name as that being uploaded exists in the destination folder </summary>
		/// <remarks> If the file does not exist, this script should return 0.  If the files 
		/// does exist, the script should return 1.</remarks>
		public string CheckScript
		{
			get { return settings.CheckScript; }
			set { settings.CheckScript = value; }
		}

		/// <summary> If set to false, drag and drop capabilities will not be enabled </summary>
		/// <value> Default value from UploadiFive is TRUE </value>
		public bool? DragAndDrop
		{
			get { return settings.DragAndDrop; }
			set { settings.DragAndDrop = value; }
		}

		/// <summary> The name of the file object to use in your server-side script </summary>
		/// <remarks> For example, in PHP, if this option is set to ‘the_files’, you can 
		/// access the files that have been uploaded using $_FILES['the_files'];</remarks>
		/// <value> Default value from UploadiFive is 'Filedata' </value>
		public string FileObjName
		{
			get { return settings.FileObjName; }
			set { settings.FileObjName = value; }
		}

		/// <summary> The maximum upload size allowed in KB </summary>
		/// <remarks> This option also accepts a unit.  If using a unit, the value must begin 
		/// with a number and end in either KB, MB, or GB.  Set this option to 0 for no limit. </remarks>
		public string FileSizeLimit
		{
			get { return settings.FileSizeLimit; }
			set { settings.FileSizeLimit = value; }
		}

		/// <summary> The type of files allowed for upload </summary>
		/// <remarks> This is taken from the file’s mime type.  To allow all images, set this 
		/// option to ‘image’.  To allow a specific type of image, set this option to ‘image/png’.  
		/// To allow all files, set this value to false.  This option will also accept a 
		/// JSON array to allow a specific set of fileTypes. </remarks>
		/// <value> Default value from UploadiFive is 'false' </value>
		public string FileType
		{
			get { return settings.FileType; }
			set { settings.FileType = value; }
		}

		/// <summary> Additional data to send to the server-side upload script </summary>
		/// <remarks> Data sent via this option will be sent via the headers and can be 
		/// accessed via the $_POST array (if using the ‘post’ method).  So if you send 
		/// something like {‘someKey’ : ‘someValue’}, then you can access it as $_POST['someKey']. </remarks>
		public Dictionary<string, string> FormData
		{
			get { return settings.FormData; }
			set { settings.FormData = value; }
		}

		/// <summary> The height of the browse button in pixels </summary>
		/// <value> Default value from UploadiFive is 30 </value>
		public int? ButtonHeight
		{
			get { return settings.ButtonHeight; }
			set { settings.ButtonHeight = value; }
		}

		/// <summary> The itemTemplate option allows you to specify a special HTML template for 
		/// each item that is added to the queue </summary>
		/// <value> The outtermost item element MUST have the class “uploadifive-queue-item” as 
		/// the code uses this class to perform various tasks.
		/// See instructions on main Uploadify site for more information. </value>
		public string ItemTemplate
		{
			get { return settings.ItemTemplate; }
			set { settings.ItemTemplate = value; }
		}

		/// <summary> The type of method to use when submitting the form  </summary>
		/// <remarks> If set to ’get’, the formData values are sent via the querystring.  If 
		/// set to ‘post’, the fromData values are sent via the headers. </remarks>
		/// <value> Default value is POST </value>
		public UploadiFive_Method_Enum Method
		{
			get { return settings.Method; }
			set { settings.Method = value; }
		}

		/// <summary> Whether or not to allow multiple file selection in the browse dialog window </summary>
		/// <remarks> Setting to true will allow multiple file selection.  This does not affect the 
		/// amount of files that can be added tot he queue.  To limit the queue size to 1, use 
		/// the QueueSizeLimit option. </remarks>
		/// <value> Default value from UploadiFive is TRUE </value>
		public bool? Multi
		{
			get { return settings.Multi; }
			set { settings.Multi = value; }
		}

		/// <summary> The ID of the element you want to use as a file queue </summary>
		/// <remarks> This element will also act as the drop target for files if DragAndDrop (dnd) is 
		/// set to true.  If the value is set to false, a queue will be created and an ID will be 
		/// assigned to it. </remarks>
		/// <value> Default value from UploadiFive is 'false; </value>
		public string QueueID
		{
			get { return settings.QueueID; }
			set { settings.QueueID = value; }
		}

		/// <summary> The maximum number of files you can have in the queue at one time </summary>
		/// <remarks> This does not affect the amount of files that may be uploaded.  To set 
		/// the amount  of files you may upload, use the uploadLimit option.  Set to 0 to set 
		/// the limit to unlimited. </remarks>
		/// <value> Default value from UploadiFive is 0 </value>
		public int? QueueSizeLimit
		{
			get { return settings.QueueSizeLimit; }
			set { settings.QueueSizeLimit = value; }
		}

		/// <summary> Whether or not to remove items that have completed uploading from the queue </summary>
		/// <value> Default value from UploadiFive is FALSE </value>
		public bool? RemoveCompleted
		{
			get { return settings.RemoveCompleted; }
			set { settings.RemoveCompleted = value; }
		}

		/// <summary> The number of files that can be simultaneously uploaded at any given time </summary>
		/// <remarks> Set to 0 to remove the limit.</remarks> 
		public int? SimUploadLimit
		{
			get { return settings.SimUploadLimit; }
			set { settings.SimUploadLimit = value; }
		}

		/// <summary> This is the location of the uploadify.swf file, including the name of the script </summary>
		/// <remarks> Default, if using flash, is 'uploadify/uploadify.swf' </remarks>
		public string Swf
		{
			get { return settings.Swf; }
			set { settings.Swf = value; }
		}

		/// <summary> The number of characters at which to truncate the file name in the queue </summary>
		/// <remarks> Set to 0 to never truncate. </remarks>
		/// <value> Default value from UploadiFive is 0 </value>
		public int? TruncateLength
		{
			get { return settings.TruncateLength; }
			set { settings.TruncateLength = value; }
		}

		/// <summary> The maximum number of files that may be uploaded </summary>
		/// <remarks> Set to 0 to remove any limit.  This does not affect the number of files that 
		/// may be added to the queue.  For that, use the QueueSizeLimit option. </remarks>
		/// <value> Default value from UploadiFive is 0 </value>
		public int? UploadLimit
		{
			get { return settings.UploadLimit; }
			set { settings.UploadLimit = value; }
		}

		/// <summary> The path to the script that will process the uploaded file </summary>
		/// <remarks> To use the provided ASP.net classes/helpers, do NOT set this directly </remarks>
		public string UploadScript
		{
			get { return settings.UploadScript; }
			set { settings.UploadScript = value; }
		}

		/// <summary> The width of the browse button in pixels </summary>
		/// <value> Default value from UploadiFive is 100 </value>
		public int? ButtonWidth
		{
			get { return settings.ButtonWidth; }
			set { settings.ButtonWidth = value; }
		}

		#endregion

	}
}