using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Core.Configuration;


namespace SobekCM.Library.CKEditor
{
    public class CKEditor
    {
        /// <summary> Constructor for a new instance of the CKEditor class </summary>
		public CKEditor()
		{

		}

        /// <summary> Add the file input and the necessary script section, with
        /// all the options specfiedi here, directly to the streamwriter </summary>
        /// <param name="Output"> Writer to write to the stream </param>
        public void Add_To_Stream(TextWriter Output)
        {
            // If there is no current HTTPContext, can't do this...
            if ((UploadPath.Length > 0) && (HttpContext.Current != null))
            {
                // Create a new security token, save in session, and set token GUID in the form data
                CKEditor_Security_Token newToken = new CKEditor_Security_Token(UploadPath);
                string token = newToken.ThisGuid.ToString();
                HttpContext.Current.Session["#CKEDITOR::" + newToken.ThisGuid] = newToken;
            }

            Output.WriteLine("  <script type=\"text/javascript\" src=\"" + BaseUrl + "default/scripts/ckeditor/ckeditor.js\"></script>");

            Output.WriteLine("  <script type=\"text/javascript\">");

            Output.WriteLine("    $(document).ready(function () { ");
            Output.WriteLine("          CKEDITOR.replace( '" + TextAreaID + "', {");
            Output.WriteLine("               extraPlugins: 'divarea',");
            if (Language == Web_Language_Enum.English)
                Output.WriteLine("               language: 'en',");
            if (Language == Web_Language_Enum.Spanish)
                Output.WriteLine("               language: 'es',");
            if (Language == Web_Language_Enum.French)
                Output.WriteLine("               language: 'fr',");
            if (Language == Web_Language_Enum.German)
                Output.WriteLine("               language: 'de',");
            if (Language == Web_Language_Enum.Dutch)
                Output.WriteLine("               language: 'nl',");
            Output.WriteLine("               extraPlugins: 'autogrow',");
            Output.WriteLine("               autoGrow_maxHeight: 800,");
            Output.WriteLine("               removePlugins: 'resize',");
            Output.WriteLine("               magicline_color: 'blue',");
            Output.Write("               extraPlugins: 'tableresize'");

            // Is there an endpoint defined for looking at uploaded files?
            if (!String.IsNullOrEmpty(ImageBrowser_ListUrl))
            {
                Output.WriteLine(",");
                Output.WriteLine("               extraPlugins : 'imagebrowser',");
                Output.WriteLine("               imageBrowser_listUrl: '" + ImageBrowser_ListUrl + "'");
            }
            else
            {
                Output.WriteLine();
            }
            Output.WriteLine("			});");
            Output.WriteLine("    });");

            Output.WriteLine("  </script>");
        }


        /// <summary> Returns the HTML to add a file input and the necessary
        /// script section to enable UploadiFive with these options </summary>
        /// <returns> HTML as a string </returns>
        public string HTML_To_Write()
        {
            StringBuilder builder = new StringBuilder(500);
            TextWriter writer = new StringWriter(builder);
            Add_To_Stream(writer);
            writer.Close();
            return builder.ToString();
        }

        /// <summary> Base URL for the system </summary>
        public string BaseUrl { get; set;  }

        /// <summary> URL for the JSON for the Image Browser plug-in, which tells which 
        /// files are on the server when the user browser the server while adding an image to the HTML </summary>
        public string ImageBrowser_ListUrl { get; set; }

        /// <summary> Path where the uploaded files should go </summary>
        public string UploadPath { get; set; }

        /// <summary> ID of the existing text area where the HTML to edit resides </summary>
        public string TextAreaID {  get; set;  }

        /// <summary> Language to use for the interface </summary>
        public Web_Language_Enum Language { get; set;  }

        /// <summary> URL for the file upload handler on the server </summary>
        public string FileBrowser_ImageUploadUrl { get; set; }
    }
}
