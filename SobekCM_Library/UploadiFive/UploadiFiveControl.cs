using System.Collections.Generic;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;

namespace SobekCM.Library.UploadiFive
{
	public class UploadiFiveControl : WebControl
	{
		/// <summary> Constructor for a new instance of the UploadiFiveControl class </summary>
		public UploadiFiveControl()
		{
			// Set some defaults
			Method = UploadiFive_Method_Enum.Post;
			FileInputID = "file_upload";
			FileInputClass = "file_upload";
			UseSecureFileHandler = false;
			SubmitWhenQueueCompletes = false;
			UploadPath = string.Empty;

			// Declare the dictionary object
			FormData = new Dictionary<string, string>();
		}

		#region Properties specific to the ASP.net implementation

		/// <summary> ID to use for the file input element </summary>
		/// <value> Default value is 'file_upload' </value>
		public string FileInputID { get; set; }

		/// <summary> Class to use for the file input element </summary>
		/// <value> Default value is 'file_upload' </value>
		public string FileInputClass { get; set; }

		/// <summary> Flag indicates if the secure file handler option should be used </summary>
		/// <remarks> This sets a value in the session state and then checks it upon upload </remarks>
		public bool UseSecureFileHandler { get; set; }

		/// <summary> Flag indicates if the form should be submitted once the uploaded queue completes </summary>
		public bool SubmitWhenQueueCompletes { get; set; }

		/// <summary> Relative path where the uploaded files should go </summary>
		public string UploadPath { get; set; }

		/// <summary> List of file extensions allowed </summary>
		public string AllowedFileExtensions { get; set; }


		#endregion
				
		protected override void RenderContents(HtmlTextWriter Output)
		{
			if ((UploadPath.Length > 0) && (HttpContext.Current != null))
			{
				HttpContext.Current.Session["Uploadify_Path"] = UploadPath;
			}

			Output.Write("<input id=\"" + FileInputID + "\" name=\"" + FileInputID + "\" ");
			if (FileInputClass.Length > 0)
				Output.Write("class=\"" + FileInputClass + "\" ");
			Output.WriteLine("type=\"file\" />");
			Output.WriteLine();

			Output.WriteLine("<script type=\"text/javascript\">");
			Output.WriteLine("  $(document).ready(function() {");
			Output.WriteLine("    $('#" + FileInputID + "').uploadifive({");
			Output.WriteLine("      'uploadScript': '" + UploadScript + "',");
			Output.WriteLine("      'removeCompleted': true,");
			Output.WriteLine("      'onQueueComplete': function (uploads) { $('#" + FileInputID + "').closest(\"form\").submit(); }");
			Output.WriteLine("    });");
			Output.WriteLine("  });");
			Output.WriteLine("</script>");
			Output.WriteLine();
		}

		#region Exposing existing Options from UploadiFive 

		/// <summary> If set to true, files will automatically upload when added to the queue </summary>
		/// <value> Default value from UploadiFive is TRUE </value>
		public bool? Auto { get; set; }

		/// <summary> A class name to add to the UploadiFive button DOM element </summary>
		public string ButtonClass { get; set; }

		/// <summary> The text to display inside the browse button </summary>
		/// <remarks> This text is rendered as HTML and may contain tags or HTML entities. </remarks>
		public string ButtonText { get; set; }

		/// <summary> The path to the server-side files that checks whether a files with the 
		/// same name as that being uploaded exists in the destination folder </summary>
		/// <remarks> If the file does not exist, this script should return 0.  If the files 
		/// does exist, the script should return 1.</remarks>
		public string CheckScript { get; set; }

		/// <summary> If set to false, drag and drop capabilities will not be enabled </summary>
		/// <value> Default value from UploadiFive is TRUE </value>
		public bool? DragAndDrop { get; set; }

		/// <summary> The name of the file object to use in your server-side script </summary>
		/// <remarks> For example, in PHP, if this option is set to ‘the_files’, you can 
		/// access the files that have been uploaded using $_FILES['the_files'];</remarks>
		/// <value> Default value from UploadiFive is 'Filedata' </value>
		public string FileObjName { get; set;  }

		/// <summary> The maximum upload size allowed in KB </summary>
		/// <remarks> This option also accepts a unit.  If using a unit, the value must begin 
		/// with a number and end in either KB, MB, or GB.  Set this option to 0 for no limit. </remarks>
		public string FileSizeLimit { get; set; }

		/// <summary> The type of files allowed for upload </summary>
		/// <remarks> This is taken from the file’s mime type.  To allow all images, set this 
		/// option to ‘image’.  To allow a specific type of image, set this option to ‘image/png’.  
		/// To allow all images, set this value to false.  This option will also accept a 
		/// JSON array to allow a specific set of fileTypes. </remarks>
		/// <value> Default value from UploadiFive is 'false' </value>
		public string FileType { get; set; }

		/// <summary> Additional data to send to the server-side upload script </summary>
		/// <remarks> Data sent via this option will be sent via the headers and can be 
		/// accessed via the $_POST array (if using the ‘post’ method).  So if you send 
		/// something like {‘someKey’ : ‘someValue’}, then you can access it as $_POST['someKey']. </remarks>
		public Dictionary<string, string> FormData { get; set; }

		/// <summary> The height of the browse button in pixels </summary>
		/// <value> Default value from UploadiFive is 30 </value>
		public int? Button_Height { get; set; }

		/// <summary> The itemTemplate option allows you to specify a special HTML template for 
		/// each item that is added to the queue </summary>
		/// <value> The outtermost item element MUST have the class “uploadifive-queue-item” as 
		/// the code uses this class to perform various tasks.
		/// See instructions on main Uploadify site for more information. </value>
		public string ItemTemplate { get; set; }

		/// <summary> The type of method to use when submitting the form  </summary>
		/// <remarks> If set to ’get’, the formData values are sent via the querystring.  If 
		/// set to ‘post’, the fromData values are sent via the headers. </remarks>
		/// <value> Default value is POST </value>
		public UploadiFive_Method_Enum Method { get; set; }

		/// <summary> Whether or not to allow multiple file selection in the browse dialog window </summary>
		/// <remarks> Setting to true will allow multiple file selection.  This does not affect the 
		/// amount of files that can be added tot he queue.  To limit the queue size to 1, use 
		/// the QueueSizeLimit option. </remarks>
		/// <value> Default value from UploadiFive is TRUE </value>
		public bool? Multi { get; set; }

		/// <summary> The ID of the element you want to use as a file queue </summary>
		/// <remarks> This element will also act as the drop target for files if DragAndDrop (dnd) is 
		/// set to true.  If the value is set to false, a queue will be created and an ID will be 
		/// assigned to it. </remarks>
		/// <value> Default value from UploadiFive is 'false; </value>
		public string QueueID { get; set; }

		/// <summary> The maximum number of files you can have in the queue at one time </summary>
		/// <remarks> This does not affect the amount of files that may be uploaded.  To set 
		/// the amount  of files you may upload, use the uploadLimit option.  Set to 0 to set 
		/// the limit to unlimited. </remarks>
		/// <value> Default value from UploadiFive is 0 </value>
		public int? QueueSizeLimit { get; set; }

		/// <summary> Whether or not to remove items that have completed uploading from the queue </summary>
		/// <value> Default value from UploadiFive is FALSE </value>
		public bool? RemoveCompleted { get; set; }

		/// <summary> The number of files that can be simultaneously uploaded at any given time </summary>
		/// <remarks> Set to 0 to remove the limit.</remarks> 
		public int? SimUploadLimit { get; set; }

		/// <summary> The number of characters at which to truncate the file name in the queue </summary>
		/// <remarks> Set to 0 to never truncate. </remarks>
		/// <value> Default value from UploadiFive is 0 </value>
		public int? TruncateLength { get; set; }

		/// <summary> The maximum number of files that may be uploaded </summary>
		/// <remarks> Set to 0 to remove any limit.  This does not affect the number of files that 
		/// may be added to the queue.  For that, use the QueueSizeLimit option. </remarks>
		/// <value> Default value from UploadiFive is 0 </value>
		public int? UploadLimit { get; set;  }

		/// <summary> The path to the script that will process the uploaded file </summary>
		/// <remarks> To use the provided ASP.net classes/helpers, do NOT set this directly </remarks>
		public string UploadScript { get; set; }

		/// <summary> The width of the browse button in pixels </summary>
		/// <value> Default value from UploadiFive is 100 </value>
		public int? Button_Width { get; set; }

		#endregion


	}
}