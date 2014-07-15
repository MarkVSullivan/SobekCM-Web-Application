#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.WebContent;

#endregion

namespace SobekCM.Library.Aggregations
{
	/// <summary> Basic information about a single browse or info page for an item aggregation </summary>
	/// <remarks> Browse pages appear as tabs in the collection viewer, while information pages must be manually linked in the collection text </remarks>
	[Serializable]
	public class Item_Aggregation_Browse_Info 
	{
		#region Browse_Info_Type enum

		/// <summary> Tells whether this is a browse to appear on the home page, a browse on the Browse By.. portion,
		/// or an information page with not explicit links </summary>
		public enum Browse_Info_Type : byte
		{
			/// <summary> This is a browse which appears on a special Browse By.. tab and screen </summary>
			Browse_By = 1,

			/// <summary> This is a browse which appears on its own tab on the home page </summary>
			Browse_Home,

			/// <summary> This is an info page which does not have its link automatically advertised </summary>
			Info
		}

		#endregion

		#region Result_Data_Type enum

		/// <summary> Specifies the data type of this browse or info page </summary>
		public enum Result_Data_Type : byte
		{
			/// <summary> This browse or info page is returned as a table of items </summary>
			Table = 1,

			/// <summary> This browse or info page is returned as text, formatted as HTML </summary>
			Text
		};

		#endregion

		#region Source_Type enum

		/// <summary> Specifies the source of this browse or info page </summary>
		public enum Source_Type : byte
		{
			/// <summary> This browse or info page is pulled from the database </summary>
			Database = 1,

			/// <summary> This browse or info page is pulled from a static (usually HTML) file </summary>
			Static_HTML
		};

		#endregion

		private Browse_Info_Type browseType;
		private string code;
		private Result_Data_Type dataType;
        private readonly Dictionary<Web_Language_Enum, string> labelsByLanguage;
		private Source_Type source;
		private readonly Dictionary<Web_Language_Enum, string> staticHtmlSourceByLanguage;

		/// <summary> Constructor for a new instance of the Item_Aggregation_Browse_Info class </summary>
		/// <param name="Browse_Type">Flag indicates if this is a browse by, browse, or info page</param>
		/// <param name="Source">Source of this browse or info page</param>
		/// <param name="Code">Code for this info or browse page</param>
		/// <param name="Static_HTML_Source">Filename of the static source file for this browse or info page</param>
		/// <param name="Label">Label for this browse or info page which will be displayed on the navigation tab</param>
		public Item_Aggregation_Browse_Info(Browse_Info_Type Browse_Type, Source_Type Source, string Code, string Static_HTML_Source, string Label) 
		{
			// Create the collections for the labels and static html source
			labelsByLanguage = new Dictionary<Web_Language_Enum, string>();
			staticHtmlSourceByLanguage = new Dictionary<Web_Language_Enum, string>();

			// Add the parameter information as the default labels and source
			labelsByLanguage[Web_Language_Enum.DEFAULT] = Label;
			staticHtmlSourceByLanguage[Web_Language_Enum.DEFAULT] = Static_HTML_Source;

			// Save all of these parameters
			code = Code;
			browseType = Browse_Type;
			source = Source;

			// If this is the special ALL or NEW, then the source will be a database table/set
			if ((code == "all") || (code == "new"))
			{
				dataType = Result_Data_Type.Table;
			}
			else
			{
				dataType = Result_Data_Type.Text;
			}
		}

		/// <summary> Constructor for a new instance of the Item_Aggregation_Browse_Info class </summary>
		public Item_Aggregation_Browse_Info() 
		{
			// Create the collections for the labels and static html source
			labelsByLanguage = new Dictionary<Web_Language_Enum, string>();
			staticHtmlSourceByLanguage = new Dictionary<Web_Language_Enum, string>();

			// Set code to empty initially
			code = String.Empty;
		}

		#region Internal methods used while building this object 

		/// <summary> Add the label for this browse/info object, by language </summary>
		/// <param name="Label"> Label for this browse/info object </param>
		/// <param name="Language"> Language code </param>
		internal void Add_Label(string Label, Web_Language_Enum Language)
		{
			// Save this under the normalized language 
			labelsByLanguage[Language] = Label;
		}

		/// <summary> Add the label for this browse/info object, by language </summary>
		/// <param name="HTML_Source"> Label for this browse/info object </param>
		/// <param name="Language"> Language code </param>
        internal void Add_Static_HTML_Source(string HTML_Source, Web_Language_Enum Language)
		{
			// Save this under the normalized language 
			staticHtmlSourceByLanguage[Language] = HTML_Source;
		}

		#endregion

		/// <summary> Code for this info or browse page </summary>
		/// <remarks> This is the code that is used in the URL to specify this info or browse page </remarks>
		public string Code
		{
			get { return code ?? String.Empty; }
			set { code = value.ToLower(); }
		}

		/// <summary> Source of this browse or info page </summary>
		public Source_Type Source
		{
			get { return source; }
			set { source = value; }
		}

		/// <summary> Data type of this browse or info page </summary>
		public Result_Data_Type Data_Type
		{
			get { return dataType; }
			set { dataType = value; }
		}

		/// <summary> Flag indicates if this is a browse by, browse, or info page </summary>
		public Browse_Info_Type Browse_Type
		{
			get { return browseType; }
			set { browseType = value; }
		}

        /// <summary> Gets the complete dictionary of labels and languages </summary>
        public Dictionary<Web_Language_Enum, string> Label_Dictionary
        {
            get { return labelsByLanguage; }
        }        

		/// <summary> Gets the language-specific label, if one exists </summary>
		/// <param name="Language"> Language of the label to retrieve </param>
		/// <returns> Language-specific label </returns>
		public string Get_Label(Web_Language_Enum Language)
		{
			if ( labelsByLanguage.ContainsKey(Language))
				return labelsByLanguage[Language];

			if (labelsByLanguage.ContainsKey(Web_Language_Enum.DEFAULT))
				return labelsByLanguage[Web_Language_Enum.DEFAULT];

			if (labelsByLanguage.ContainsKey(Web_Language_Enum.English))
				return labelsByLanguage[Web_Language_Enum.English];

			if (labelsByLanguage.Count > 0)
				return labelsByLanguage.ElementAt(0).Value;

			return string.Empty;
		}

        /// <summary> Gets the complete dictionary of static HTML sources and languages </summary>
        public Dictionary<Web_Language_Enum, string> Source_Dictionary
        {
            get { return staticHtmlSourceByLanguage; }
        }        

		/// <summary> Gets the language-specific static HTML source file, if one exists </summary>
		/// <param name="Language"> Language of the static HTML source file to retrieve </param>
		/// <returns> Language-specific static HTML source file </returns>
		public string Get_Static_HTML_Source( Web_Language_Enum Language)
		{
			if (staticHtmlSourceByLanguage.ContainsKey(Language))
				return staticHtmlSourceByLanguage[Language];

			if (staticHtmlSourceByLanguage.ContainsKey(Web_Language_Enum.DEFAULT))
				return staticHtmlSourceByLanguage[Web_Language_Enum.DEFAULT];

			if (staticHtmlSourceByLanguage.ContainsKey(Web_Language_Enum.English))
				return staticHtmlSourceByLanguage[Web_Language_Enum.English];

			if (staticHtmlSourceByLanguage.Count > 0)
				return staticHtmlSourceByLanguage.ElementAt(0).Value;

			return string.Empty;
		}

		internal void Write_In_Configuration_XML_File( System.IO.StreamWriter Writer, string Default_BrowseBy )
		{
			string type = "browse";
			if ( browseType == Browse_Info_Type.Info)
				type = "info";

			if (browseType == Browse_Info_Type.Browse_By)
			{
				Writer.WriteLine(String.Compare(Default_BrowseBy, code, true) == 0 ? "  <hi:browse location=\"BROWSEBY\" default=\"DEFAULT\">" : "  <hi:browse location=\"BROWSEBY\">");
			}
			else
			{
                Writer.WriteLine("  <hi:" + type + ">");
			}

			// Is this a database option, (usually browse by)?
			if (source == Source_Type.Database)
			{
				Writer.WriteLine("    <hi:metadata>" + code.ToUpper() + "</hi:metadata>");
			}
			else
			{
				// Write the code for this static html sourced browse/info
				Writer.WriteLine("    <hi:code>" + code.ToLower() + "</hi:code>");

				// Include the titles, or just use the code as the title if no titles given
				Writer.WriteLine("    <hi:titles>");
				if (labelsByLanguage.Count > 0)
				{
					foreach (KeyValuePair<Web_Language_Enum, string> thisLabel in labelsByLanguage)
					{
						if (thisLabel.Key == Web_Language_Enum.DEFAULT)
							Writer.WriteLine("    <hi:title>" + thisLabel.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:title>");
						else
							Writer.WriteLine("    <hi:title lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(thisLabel.Key) + "\">" + thisLabel.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:title>");
					}
				}
				else
				{
					Writer.WriteLine("    <hi:title>" + code + "</hi:title>");
				}
				Writer.WriteLine("    </hi:titles>");

				// Include the sources as well
				Writer.WriteLine("    <hi:content>");
				foreach (KeyValuePair<Web_Language_Enum, string> thisSource in staticHtmlSourceByLanguage )
				{
					if (thisSource.Key == Web_Language_Enum.DEFAULT)
						Writer.WriteLine("    <hi:body>" + thisSource.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:body>");
					else
						Writer.WriteLine("    <hi:body lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(thisSource.Key) + "\">" + thisSource.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:body>");
				}
				Writer.WriteLine("    </hi:content>");
			}


			Writer.WriteLine("  </hi:" + type + ">");
		}


		/// <summary> Gets the static HTML_based content  associated with this browse/info file (if it is TEXT type )</summary>
		/// <param name="Language"> Language of the static-html request, used to find the appropriate source file </param>
		/// <param name="Base_URL"> Currently used Base URL to reach this system </param>
        /// <param name="Base_Network"> Base network location from which this static content would be located from </param>
		/// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
		/// <returns> Static HTML-based content to read from a  html source file </returns>
		/// <remarks> This actually reads the HTML file each time this is requested </remarks>
		public HTML_Based_Content Get_Static_Content( Web_Language_Enum Language, string Base_URL, string Base_Network, Custom_Tracer Tracer)
		{
			if ((Data_Type != Result_Data_Type.Text) || (Source != Source_Type.Static_HTML) || (staticHtmlSourceByLanguage.Count == 0))
				return null;

			// Get the source file name
			string sourceFile = Get_Static_HTML_Source(Language);
			if (sourceFile.Length == 0)
				return null;

			if ((sourceFile.IndexOf("http://") < 0) && (sourceFile.IndexOf("<%BASEURL%>") < 0))
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Item_Aggregation_Browse_Info.Get_Static_Text", "Reading browse/info source file from a local directory");
				}

                sourceFile = Base_Network + sourceFile;

				try
				{
					// Ensure the file exists
					if (!System.IO.File.Exists(sourceFile))
					{
						return new HTML_Based_Content("<div class=\"error_div\">INVALID OR MISSING BROWSE FILE '" + sourceFile + ".<br /><br />PLEASE CHECK YOUR URL.</div>", "ERROR: Missing File");
					}

					// Read this file from the network
					return HTML_Based_Content_Reader.Read_HTML_File(sourceFile, true, Tracer);
				}
				catch (Exception ee)
				{
					return new HTML_Based_Content( "<div class=\"error_div\">EXCEPTION CAUGHT WHILE TRYING TO READ THE BROWSE/INFO PAGE SOURCE FILE '" + sourceFile + "'.<br /><br />ERROR: " + ee.Message + "</div>", "ERROR: " + ee.Message );
				}
			}
			
			if (Tracer != null)
			{
				Tracer.Add_Trace("Item_Aggregation_Browse_Info.Get_Static_Text", "Reading browse/info source file via http");
			}

			string actualUrl = sourceFile.Replace("<%BASEURL%>", Base_URL);
			try
			{
				// Read this file from the web
				return HTML_Based_Content_Reader.Read_Web_Document(actualUrl, true, Tracer );
			}
			catch ( Exception ee )
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Item_Aggregation_Browse_Info.Get_Static_Text", "Unable to read data from: " + actualUrl, Custom_Trace_Type_Enum.Error);
				}

				return new HTML_Based_Content( "<div class=\"error_div\">EXCEPTION CAUGHT WHILE TRYING TO READ THE BROWSE/INFO PAGE SOURCE FILE '" + actualUrl + "'.<br /><br />ERROR: " + ee.Message + "</div>", "ERROR: " + ee.Message );
			}
		}
	}
}
