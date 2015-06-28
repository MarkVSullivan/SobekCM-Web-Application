#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Basic information about any child page to an item aggregation, regardless of whether this is a
    /// static html page, or a browse pulled from the database  </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("childPage")]
    public class Item_Aggregation_Child_Page
    {
        #region Constructors

		/// <summary> Constructor for a new instance of the Item_Aggregation_Child_Page class </summary>
		/// <param name="Browse_Type">Flag indicates if this is a browse by, browse, or info page</param>
        /// <param name="Source_Data_Type">Source and data type of this browse or info page</param>
		/// <param name="Code">Code for this info or browse page</param>
		/// <param name="Static_HTML_Source">Filename of the static source file for this browse or info page</param>
		/// <param name="Label">Label for this browse or info page which will be displayed on the navigation tab</param>
        public Item_Aggregation_Child_Page(Item_Aggregation_Child_Visibility_Enum Browse_Type, Item_Aggregation_Child_Source_Data_Enum Source_Data_Type, string Code, string Static_HTML_Source, string Label) 
		{
			// Save all of these parameters
			this.Code = Code;
			this.Browse_Type = Browse_Type;
            this.Source_Data_Type = Source_Data_Type;
            this.Label = Label;
            if ( !String.IsNullOrEmpty(Static_HTML_Source))
    		    Source = Static_HTML_Source;

		    //// If this is the special ALL or NEW, then the source will be a database table/set
		    //if ((Code == "all") || (Code == "new"))
		    //{
		    //    Data_Type = Result_Data_Type.Table;
		    //}
		    //else
		    //{
		    //    Data_Type = Result_Data_Type.Text;
		    //}

		}

		/// <summary> Constructor for a new instance of the Item_Aggregation_Child_Page class </summary>
        public Item_Aggregation_Child_Page() 
		{
			// Set code to empty initially
			Code = String.Empty;
		}

		#endregion

        /// <summary> Code for this info or browse page </summary>
        /// <remarks> This is the code that is used in the URL to specify this info or browse page </remarks>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary> Source and data type of this browse or info page </summary>
        [DataMember(Name = "sourceData")]
        [XmlAttribute("sourceData")]
        [ProtoMember(2)]
        public Item_Aggregation_Child_Source_Data_Enum Source_Data_Type { get; set; }

        /// <summary> Flag indicates where this child page should appear </summary>
        [DataMember(Name = "browseType")]
        [XmlAttribute("browseType")]
        [ProtoMember(3)]
        public Item_Aggregation_Child_Visibility_Enum Browse_Type { get; set; }

        /// <summary> If this is to appear on the main menu, this allows the browses
        /// to be established hierarchically, with this child page either being at the
        /// top, or sitting under another child page </summary>
        [DataMember(Name = "parentCode", EmitDefaultValue = false)]
        [XmlAttribute("parentCode")]
        [ProtoMember(4)]
        public string Parent_Code { get; set; }

        /// <summary> Gets the complete dictionary of labels and languages </summary>
        [DataMember(Name = "label", EmitDefaultValue = false)]
        [XmlAttribute("label")]
        [ProtoMember(5)]
        public string Label { get; set; }

        /// <summary> Source for this child page </summary>
        [DataMember(Name = "source", EmitDefaultValue = false)]
        [XmlAttribute("source")]
        [ProtoMember(6)]
        public string Source { get; set;}
    }
}
