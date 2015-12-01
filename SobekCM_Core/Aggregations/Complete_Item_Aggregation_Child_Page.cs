#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.WebContent;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.Aggregations
{
	/// <summary> Basic information about a single child page for an item aggregation </summary>
    [Serializable, DataContract, ProtoContract]
	public class Complete_Item_Aggregation_Child_Page 
	{


		#region Constructors

		/// <summary> Constructor for a new instance of the Item_Aggregation_Complete_Child_Page class </summary>
		/// <param name="Browse_Type">Flag indicates if this is a browse by, browse, or info page</param>
        /// <param name="Source_Data_Type">Source and data type of this browse or info page</param>
		/// <param name="Code">Code for this info or browse page</param>
		/// <param name="Static_HTML_Source">Filename of the static source file for this browse or info page</param>
		/// <param name="Label">Label for this browse or info page which will be displayed on the navigation tab</param>
        public Complete_Item_Aggregation_Child_Page(Item_Aggregation_Child_Visibility_Enum Browse_Type, Item_Aggregation_Child_Source_Data_Enum Source_Data_Type, string Code, string Static_HTML_Source, string Label) 
		{
			// Create the collections for the labels and static html source
			Label_Dictionary = new Dictionary<Web_Language_Enum, string>();
			Source_Dictionary = new Dictionary<Web_Language_Enum, string>();

			// Add the parameter information as the default labels and source
			Label_Dictionary[Web_Language_Enum.DEFAULT] = Label;
			Source_Dictionary[Web_Language_Enum.DEFAULT] = Static_HTML_Source;

			// Save all of these parameters
			this.Code = Code;
			this.Browse_Type = Browse_Type;
            this.Source_Data_Type = Source_Data_Type;

            //// If this is the special ALL or NEW, then the source will be a database table/set
            //if ((Code == "all") || (Code == "new"))
            //{
            //    Data_Type = Result_Data_Type.Table;
            //}
            //else
            //{
            //    Data_Type = Result_Data_Type.Text;
            //}

			// Add the label
			if ( Label.Length > 0 )
				Label_Dictionary[Web_Language_Enum.English] = Label;
		}

		/// <summary> Constructor for a new instance of the Item_Aggregation_Complete_Child_Page class </summary>
        public Complete_Item_Aggregation_Child_Page() 
		{
			// Set code to empty initially
			Code = String.Empty;
		}

		#endregion

		#region Internal methods used while building this object

		/// <summary> Add the label for this browse/info object, by language </summary>
		/// <param name="Label"> Label for this browse/info object </param>
		/// <param name="Language"> Language code </param>
        public void Add_Label(string Label, Web_Language_Enum Language)
		{
		    if (Label_Dictionary == null) Label_Dictionary = new Dictionary<Web_Language_Enum, string>();

			// Save this under the normalized language 
			Label_Dictionary[Language] = Label;
		}

		/// <summary> Add the label for this browse/info object, by language </summary>
		/// <param name="HTML_Source"> Label for this browse/info object </param>
		/// <param name="Language"> Language code </param>
        public void Add_Static_HTML_Source(string HTML_Source, Web_Language_Enum Language)
		{
		    if (Source_Dictionary == null) Source_Dictionary = new Dictionary<Web_Language_Enum, string>();

			// Save this under the normalized language 
			Source_Dictionary[Language] = HTML_Source;
		}

		#endregion

		/// <summary> Code for this info or browse page </summary>
		/// <remarks> This is the code that is used in the URL to specify this info or browse page </remarks>
        [DataMember(Name = "code"), ProtoMember(1)]
		public string Code { get; set; }

		/// <summary> Source of this browse or info page </summary>
        [DataMember(Name = "sourceData"), ProtoMember(2)]
        public Item_Aggregation_Child_Source_Data_Enum Source_Data_Type { get; set; }

		/// <summary> Flag indicates where this child page should appear </summary>
        [DataMember(Name = "browseType"), ProtoMember(3)]
        public Item_Aggregation_Child_Visibility_Enum Browse_Type { get; set; }

		/// <summary> If this is to appear on the main menu, this allows the browses
		/// to be established hierarchically, with this child page either being at the
		/// top, or sitting under another child page </summary>
        [DataMember(Name = "parentCode", EmitDefaultValue=false), ProtoMember(4)]
		public string Parent_Code { get; set; }

		/// <summary> Gets the complete dictionary of labels and languages </summary>
        [DataMember(Name = "labels", EmitDefaultValue = false), ProtoMember(5)]
		public Dictionary<Web_Language_Enum, string> Label_Dictionary { get; private set; }

		/// <summary> Gets the language-specific label, if one exists </summary>
		/// <param name="Language"> Language of the label to retrieve </param>
		/// <returns> Language-specific label </returns>
		public string Get_Label(Web_Language_Enum Language)
		{
            if (Label_Dictionary == null)
                return String.Empty;

			if ( Label_Dictionary.ContainsKey(Language))
				return Label_Dictionary[Language];

			if (Label_Dictionary.ContainsKey(Web_Language_Enum.DEFAULT))
				return Label_Dictionary[Web_Language_Enum.DEFAULT];

			if (Label_Dictionary.ContainsKey(Web_Language_Enum.English))
				return Label_Dictionary[Web_Language_Enum.English];

			if (Label_Dictionary.Count > 0)
				return Label_Dictionary.ElementAt(0).Value;

			return string.Empty;
		}

		/// <summary> Gets the complete dictionary of static HTML sources and languages </summary>
        [DataMember(Name = "staticSources", EmitDefaultValue = false), ProtoMember(6)]
		public Dictionary<Web_Language_Enum, string> Source_Dictionary { get; private set; }

		/// <summary> Gets the language-specific static HTML source file, if one exists </summary>
		/// <param name="Language"> Language of the static HTML source file to retrieve </param>
		/// <returns> Language-specific static HTML source file </returns>
		public string Get_Static_HTML_Source( Web_Language_Enum Language)
		{
            if (Source_Dictionary == null)
                return String.Empty;

			if (Source_Dictionary.ContainsKey(Language))
				return Source_Dictionary[Language];

			if (Source_Dictionary.ContainsKey(Web_Language_Enum.DEFAULT))
				return Source_Dictionary[Web_Language_Enum.DEFAULT];

			if (Source_Dictionary.ContainsKey(Web_Language_Enum.English))
				return Source_Dictionary[Web_Language_Enum.English];

			if (Source_Dictionary.Count > 0)
				return Source_Dictionary.ElementAt(0).Value;

			return string.Empty;
		}

		/// <summary> Removes a language from this child page's dictionaries of 
		/// labels/titles and source files </summary>
		/// <param name="Language_To_Remove"></param>
		public void Remove_Language(Web_Language_Enum Language_To_Remove)
		{
            if ( Source_Dictionary != null ) Source_Dictionary.Remove(Language_To_Remove);
            if ( Label_Dictionary != null ) Label_Dictionary.Remove(Language_To_Remove);
		}

		internal void Write_In_Configuration_XML_File( StreamWriter Writer, string Default_BrowseBy )
		{
			switch (Browse_Type)
			{
                case Item_Aggregation_Child_Visibility_Enum.Metadata_Browse_By:
					Writer.WriteLine(String.Compare(Default_BrowseBy ?? String.Empty, Code, StringComparison.OrdinalIgnoreCase) == 0 ? "  <hi:browse visibility=\"BROWSEBY\" default=\"DEFAULT\">" : "  <hi:browse visibility=\"BROWSEBY\">");
					break;

                case Item_Aggregation_Child_Visibility_Enum.None:
					Writer.WriteLine("  <hi:browse visibility=\"NONE\">");
					break;

                case Item_Aggregation_Child_Visibility_Enum.Main_Menu:
					Writer.WriteLine( String.IsNullOrEmpty(Parent_Code) ? "  <hi:browse visibility=\"MAIN_MENU\">" : "  <hi:browse visibility=\"MAIN_MENU\" parent=\"" + Parent_Code + "\">");
					break;
			}

			// Is this a database option, (usually browse by)?
            if (Source_Data_Type == Item_Aggregation_Child_Source_Data_Enum.Database_Table)
			{
				Writer.WriteLine("    <hi:metadata>" + Code.ToUpper() + "</hi:metadata>");
			}
			else
			{
				// Write the code for this static html sourced browse/info
				Writer.WriteLine("    <hi:code>" + Code.ToLower() + "</hi:code>");

				// Include the titles, or just use the code as the title if no titles given
				Writer.WriteLine("    <hi:titles>");
				if (( Label_Dictionary != null ) && ( Label_Dictionary.Count > 0))
				{
					foreach (KeyValuePair<Web_Language_Enum, string> thisLabel in Label_Dictionary)
					{
						if (thisLabel.Key == Web_Language_Enum.DEFAULT)
							Writer.WriteLine("    <hi:title>" + thisLabel.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:title>");
						else
							Writer.WriteLine("    <hi:title lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(thisLabel.Key) + "\">" + thisLabel.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:title>");
					}
				}
				else
				{
					Writer.WriteLine("    <hi:title>" + Code + "</hi:title>");
				}
				Writer.WriteLine("    </hi:titles>");

				// Include the sources as well
			    if ((Source_Dictionary != null) && (Source_Dictionary.Count > 0))
			    {
			        Writer.WriteLine("    <hi:content>");
			        foreach (KeyValuePair<Web_Language_Enum, string> thisSource in Source_Dictionary)
			        {
			            if (thisSource.Key == Web_Language_Enum.DEFAULT)
			                Writer.WriteLine("    <hi:body>" + thisSource.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:body>");
			            else
			                Writer.WriteLine("    <hi:body lang=\"" + Web_Language_Enum_Converter.Enum_To_Code(thisSource.Key) + "\">" + thisSource.Value.Replace("&", "&amp;").Replace("\"", "&quot;") + "</hi:body>");
			        }
			        Writer.WriteLine("    </hi:content>");
			    }
			}


			Writer.WriteLine("  </hi:browse>");
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
            if ((Source_Data_Type != Item_Aggregation_Child_Source_Data_Enum.Database_Table) || (Source_Dictionary == null) || (Source_Dictionary.Count == 0))
				return null;

			// Get the source file name
			string sourceFile = Get_Static_HTML_Source(Language);
			if (sourceFile.Length == 0)
				return null;

			if ((sourceFile.IndexOf("http://") < 0) && (sourceFile.IndexOf("<%BASEURL%>") < 0))
			{
				if (Tracer != null)
				{
					Tracer.Add_Trace("Item_Aggregation_Child_Page.Get_Static_Text", "Reading browse/info source file from a local directory");
				}

                sourceFile = Base_Network + sourceFile;

				try
				{
					// Ensure the file exists
					if (!File.Exists(sourceFile))
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
				Tracer.Add_Trace("Item_Aggregation_Child_Page.Get_Static_Text", "Reading browse/info source file via http");
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
					Tracer.Add_Trace("Item_Aggregation_Child_Page.Get_Static_Text", "Unable to read data from: " + actualUrl, Custom_Trace_Type_Enum.Error);
				}

				return new HTML_Based_Content( "<div class=\"error_div\">EXCEPTION CAUGHT WHILE TRYING TO READ THE BROWSE/INFO PAGE SOURCE FILE '" + actualUrl + "'.<br /><br />ERROR: " + ee.Message + "</div>", "ERROR: " + ee.Message );
			}
		}
	}
}
