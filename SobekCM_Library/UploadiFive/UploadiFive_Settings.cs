using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace SobekCM.Library.UploadiFive
{
	/// <summary> Class holds all the individual setting values for the uploaders </summary>
	/// <remarks> This helper class is utilized internally by both the <see cref="UploadiFive"/> and <see cref="UploadiFiveControl"/> classes.  
	/// This class contains all the properties and does some of the actual HTML writing as well </remarks>
	internal class UploadiFive_Settings
	{

		#region Properties specific to the ASP.net implementation

		/// <summary> Version being utilized, either the HTML5 version (UploadiFive)
		/// or the FLASH version (Uploadify) </summary>
		/// <value> Default is to use the HTML5 version </value>
		public UploadiFive_Version_Enum Version { get; set; }

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

		/// <summary> Path where the uploaded files should go </summary>
		public string UploadPath { get; set; }

		/// <summary> List of file extensions allowed </summary>
		public string AllowedFileExtensions { get; set; }

		/// <summary> If a user attempts to upload a disallowed file extension, this is the
		/// message that will be popped up to the user </summary>
		/// <remarks> You can use '&lt;extension&gt;' in the string to have the extension
		/// of the attempted file in your message.  Setting this to an empty string will 
		/// cause no alert to happen clientside, but the upload will be cancelled. </remarks>
		/// <value> Default value is 'File types of '&lt;extension&gt;' are not allowed' </value>
		public string DisallowedFileExtenstionMessage { get; set; }

		/// <summary> Flag indicates that if the client does not have HTML5 with their browser, to revert
		/// to the Flash version, if it is available </summary>
		/// <value> Default value is FALSE </value>
		public bool RevertToFlashVersion { get; set; }

		/// <summary> If this is set, this class will be used for the button when the HTML5 version 
		/// has reverted to the flash version. </summary>
		/// <value> Can be useful to see if it reverts </value>
		public string RevertedButtonClass { get; set; }

		/// <summary> Message displayed if the user has neither HTML5 on their browse nor Flash installed </summary>
		/// <value> Default value is 'Your browse must either be HTML5-compliant or have Adobe Flash installed to use this upload feature'.</value>
		public string NoHtml5OrFlashMessage { get; set; }

		#endregion

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
		public string FileObjName { get; set; }

		/// <summary> The maximum upload size allowed in KB </summary>
		/// <remarks> This option also accepts a unit.  If using a unit, the value must begin 
		/// with a number and end in either KB, MB, or GB.  Set this option to 0 for no limit. </remarks>
		public string FileSizeLimit { get; set; }

		/// <summary> The type of files allowed for upload </summary>
		/// <remarks> This is taken from the file’s mime type.  To allow all images, set this 
		/// option to ‘image’.  To allow a specific type of image, set this option to ‘image/png’.  
		/// To allow all files, set this value to false.  This option will also accept a 
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
		public int? ButtonHeight { get; set; }

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

		/// <summary> This is the location of the uploadify.swf file, including the name of the script </summary>
		/// <remarks> Default, if using flash, is 'uploadify/uploadify.swf' </remarks>
		public string Swf { get; set; }

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
		public int? UploadLimit { get; set; }

		/// <summary> The path to the script that will process the uploaded file </summary>
		/// <remarks> To use the provided ASP.net classes/helpers, do NOT set this directly.  For the FLASH version, 
		/// this actually maps to the 'uploader' value. </remarks>
		public string UploadScript { get; set; }

		/// <summary> The width of the browse button in pixels </summary>
		/// <value> Default value from UploadiFive is 100 </value>
		public int? ButtonWidth { get; set; }

		#endregion

		/// <summary> Constructor for a new instance of the UploadiFive_Settings class </summary>
		public UploadiFive_Settings()
		{
			// Set some defaults
			Method = UploadiFive_Method_Enum.Post;
			FileInputID = "file_upload";
			FileInputClass = "file_upload";
			UseSecureFileHandler = false;
			SubmitWhenQueueCompletes = false;
			UploadPath = string.Empty;
			UploadScript = "UploadiFiveFileHandler.ashx";
			FileObjName = "Filedata";
			DisallowedFileExtenstionMessage = "File types of '<extension>' are not allowed";
			Version = UploadiFive_Version_Enum.HTML5;
			RevertToFlashVersion = false;

			// Declare the dictionary object
			FormData = new Dictionary<string, string>();
		}

		/// <summary> Add the file input and the necessary script section, with
		/// all the options specfiedi here, directly to the streamwriter </summary>
		/// <param name="Output"> Writer to write to the stream </param>
		/// <param name="Extra_Indent"> Extra indent to begin each line with (purely for formatting) </param>
		/// <param name="Invalid_Message"> Message to display on fallback, particularly when HTML5 and flash both fail </param>
		public void Add_To_Stream(TextWriter Output, string Extra_Indent, String Invalid_Message )
		{
			if (Version == UploadiFive_Version_Enum.HTML5)
				Output.WriteLine(Extra_Indent + "    $('#" + FileInputID + "').uploadifive({");
			else
				Output.WriteLine(Extra_Indent + "    $('#" + FileInputID + "').uploadify({");
			

			// Add all the uploadifive options
			if (Auto.HasValue)
				Output.WriteLine(Extra_Indent + "      'auto': " + Auto.Value.ToString().ToLower() + ",");
			if (!string.IsNullOrEmpty(ButtonClass))
				Output.WriteLine(Extra_Indent + "      'buttonClass': '" + ButtonClass + "',");
			if (!string.IsNullOrEmpty(ButtonText))
				Output.WriteLine(Extra_Indent + "      'buttonText': '" + ButtonText + "',");
			if (!string.IsNullOrEmpty(CheckScript))
				Output.WriteLine(Extra_Indent + "      'checkScript': '" + CheckScript + "',");
			if (DragAndDrop.HasValue)
				Output.WriteLine(Extra_Indent + "      'dnd': " + DragAndDrop.Value.ToString().ToLower() + ",");
			if (!string.IsNullOrEmpty(FileObjName))
				Output.WriteLine(Extra_Indent + "      'fileObjName': '" + FileObjName + "',");
			if (!string.IsNullOrEmpty(FileSizeLimit))
				Output.WriteLine(Extra_Indent + "      'fileSizeLimit': '" + FileSizeLimit + "',");
			if (!string.IsNullOrEmpty(FileType))
				Output.WriteLine(Extra_Indent + "      'fileType': '" + FileType + "',");

			// Add the form data
			if (FormData.Count > 0)
			{
				Output.Write(Extra_Indent + "      'formData': { ");
				bool first = true;
				foreach (KeyValuePair<string, string> thisData in FormData)
				{
					// After the first one, start with the comma seperation
					if (first)
						first = false;
					else
						Output.Write(", ");

					Output.Write("'" + thisData.Key + "' : '" + thisData.Value + "'");
				}
				Output.WriteLine(" },");
			}

			// Finish the uploadifive options
			if (ButtonHeight.HasValue)
				Output.WriteLine(Extra_Indent + "      'height': " + ButtonHeight.Value + ",");
			if (!string.IsNullOrEmpty(ItemTemplate))
				Output.WriteLine(Extra_Indent + "      'itemTemplate': '" + ItemTemplate + "',");
			if (Method == UploadiFive_Method_Enum.Get)
				Output.WriteLine(Extra_Indent + "      'method': 'get',");
			if (Multi.HasValue)
				Output.WriteLine(Extra_Indent + "      'multi': " + Multi.Value.ToString().ToLower() + ",");
			if (!string.IsNullOrEmpty(QueueID))
				Output.WriteLine(Extra_Indent + "      'queueID': '" + QueueID + "',");
			if (QueueSizeLimit.HasValue)
				Output.WriteLine(Extra_Indent + "      'queueSizeLimit': " + QueueSizeLimit.Value + ",");
			if (RemoveCompleted.HasValue)
				Output.WriteLine(Extra_Indent + "      'removeCompleted': " + RemoveCompleted.Value.ToString().ToLower() + ",");
			if (TruncateLength.HasValue)
				Output.WriteLine(Extra_Indent + "      'truncateLength': " + TruncateLength.Value + ",");
			if (UploadLimit.HasValue)
				Output.WriteLine(Extra_Indent + "      'uploadLimit': " + UploadLimit.Value + ",");
			if (ButtonWidth.HasValue)
				Output.WriteLine(Extra_Indent + "      'width': " + ButtonWidth.Value + ",");

			// Add some event handlers
			if (SubmitWhenQueueCompletes)
				Output.WriteLine(Extra_Indent + "      'onQueueComplete': function (uploads) { $('#" + FileInputID + "').closest(\"form\").submit(); },");

			// Is there a file extension restriction here?
			if (!string.IsNullOrEmpty(AllowedFileExtensions))
			{
				// Build the json array of possible file extensions
				string[] split = AllowedFileExtensions.Split(",|".ToCharArray());
				StringBuilder jsonArrayBuilder = new StringBuilder(AllowedFileExtensions.Length * 2);
				bool first = true;
				foreach (string thisSplit in split)
				{
					if (first)
					{
						jsonArrayBuilder.Append("\"" + thisSplit.Trim().ToLower() + "\"");
						first = false;
					}
					else
					{
						jsonArrayBuilder.Append(", \"" + thisSplit.Trim().ToLower() + "\"");
					}
				}

				// Now, add the event
				if (Version == UploadiFive_Version_Enum.HTML5)
					Output.WriteLine(Extra_Indent + "      'onAddQueueItem' : function(file) {");
				else
					Output.WriteLine(Extra_Indent + "      'onSelect' : function(file) {");

				Output.WriteLine(Extra_Indent + "                             var extArray = JSON.parse('[ " + jsonArrayBuilder + " ]');");
				Output.WriteLine(Extra_Indent + "                             var fileName = file.name;");
				Output.WriteLine(Extra_Indent + "                             var ext = fileName.substring(fileName.lastIndexOf('.')).toLowerCase();");
				Output.WriteLine(Extra_Indent + "                             var isExtValid = false;");
				Output.WriteLine(Extra_Indent + "                             for(var i = 0; i < extArray.length; i++) { ");
				Output.WriteLine(Extra_Indent + "                                 if ( ext == extArray[i] ) { isExtValid = true; break; }");
				Output.WriteLine(Extra_Indent + "                             }");

				if (Version == UploadiFive_Version_Enum.HTML5)
				{
					if (DisallowedFileExtenstionMessage.Length > 0)
						Output.WriteLine(Extra_Indent + "                             if ( !isExtValid ) {  alert(\"" + DisallowedFileExtenstionMessage + "\".replace('<extension>', ext)); $('#" + FileInputID + "').uploadifive('cancel', file);  }");
					else
						Output.WriteLine(Extra_Indent + "                             if ( !isExtValid ) {  $('#" + FileInputID + "').uploadifive('cancel', file);  }");
				}
				else
				{
					if (DisallowedFileExtenstionMessage.Length > 0)
						Output.WriteLine(Extra_Indent + "                             if ( !isExtValid ) {  alert(\"" + DisallowedFileExtenstionMessage + "\".replace('<extension>', ext)); $('#" + FileInputID + "').uploadify('cancel', '*');  }");
					else
						Output.WriteLine(Extra_Indent + "                             if ( !isExtValid ) {  $('#" + FileInputID + "').uploadify('cancel', '*');  }");
				}

				Output.WriteLine(Extra_Indent + "                         },");

			}


			// Set the upload script and finish this
			if (Version == UploadiFive_Version_Enum.HTML5)
				Output.Write(Extra_Indent + "      'uploadScript': '" + UploadScript + "'");
			else
			{
				if ( String.IsNullOrEmpty(Swf))
					Output.WriteLine(Extra_Indent + "      'swf': 'uploadify/uploadify.swf',");
				else
					Output.WriteLine(Extra_Indent + "      'swf': '" + Swf + "',");

				Output.Write(Extra_Indent + "      'uploader': '" + UploadScript + "'");
			}

			if ((Version == UploadiFive_Version_Enum.HTML5) && (RevertToFlashVersion))
			{
				// ENd the last line, with a paranthesis
				Output.WriteLine(",");
				Output.WriteLine(Extra_Indent + "      'onFallback': function() {");

				Output.WriteLine(Extra_Indent + "                           // Revert to flash version if no HTML5");

				// Switch to SWF version 
				Version = UploadiFive_Version_Enum.Flash;
				string buttonCss = ButtonClass;
				if (!String.IsNullOrEmpty(RevertedButtonClass))
					ButtonClass = RevertedButtonClass;
				try
				{
					Add_To_Stream(Output, Extra_Indent + "                       ", NoHtml5OrFlashMessage);
				}
				catch (Exception)
				{
					// Just want to ensure the setting is returned
				}
				finally
				{
					Version = UploadiFive_Version_Enum.HTML5;
					ButtonClass = buttonCss;
				}

				Output.WriteLine(Extra_Indent + "                    }");
			}
			else
			{
				if (NoHtml5OrFlashMessage.Length > 0)
				{
					// ENd the last line, with a paranthesis
					Output.WriteLine(",");
					Output.WriteLine(Extra_Indent + "      'onFallback': function() { alert('" + NoHtml5OrFlashMessage.Replace("'","") + "'); }");
				}
				else
				{
					// End the last line
					Output.WriteLine();
				}

			}

			Output.WriteLine(Extra_Indent + "    });");
			Output.WriteLine();
		}

	}
}
