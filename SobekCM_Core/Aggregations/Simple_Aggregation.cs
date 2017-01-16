using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization;
using System.Text;
using System.Xml.Serialization;
using ProtoBuf;
using Jil;
using SobekCM.Core.Configuration.Localization;

namespace SobekCM.Core.Aggregations
{
    /// <summary> Simple language-specific item aggregation data </summary>
    /// <remarks> This is not actually used by the SobekCM code, but is provided as a smaller, simpler
    /// object to be used by external libraries and javascript. </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("aggregation.simple")]
    public class Simple_Aggregation
    {

        /// <summary> Constructor for a new instance of the simple aggregation class </summary>
        public Simple_Aggregation()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Constructor for a new instance of the simple aggregation class </summary>
        /// <param name="FullAggregation"> Full (language-specific) aggregation object </param>
        public Simple_Aggregation(Item_Aggregation FullAggregation)
        {
            // Assign the simple properties
            Language = Web_Language_Enum_Converter.Enum_To_Code(FullAggregation.Language);
            ID = FullAggregation.ID;
            Code = FullAggregation.Code;
            Name = FullAggregation.Name;
            BannerImage = FullAggregation.BannerImage;
            Last_Item_Added = FullAggregation.Last_Item_Added;

            // Assign search fields
            if ((FullAggregation.Search_Fields != null) && (FullAggregation.Search_Fields.Count > 0))
            {
                Search_Fields = new List<Item_Aggregation_Metadata_Type>();
                foreach (Item_Aggregation_Metadata_Type searchField in FullAggregation.Search_Fields)
                {
                    Search_Fields.Add(new Item_Aggregation_Metadata_Type(searchField.DisplayTerm, searchField.SobekCode ));
                }
            }

            // Assign browseable fields
            if ((FullAggregation.Browseable_Fields != null) && (FullAggregation.Browseable_Fields.Count > 0))
            {
                Browseable_Fields = new List<Item_Aggregation_Metadata_Type>();
                foreach (Item_Aggregation_Metadata_Type searchField in FullAggregation.Browseable_Fields)
                {
                    Browseable_Fields.Add(new Item_Aggregation_Metadata_Type(searchField.DisplayTerm, searchField.SobekCode));
                }
            }

            // Assign searches and views
            if ((FullAggregation.Views_And_Searches != null) && (FullAggregation.Views_And_Searches.Count > 0))
            {
                Views_And_Searches = new List<string>();
                foreach (Item_Aggregation_Views_Searches_Enum viewEnum in FullAggregation.Views_And_Searches)
                {
                    Views_And_Searches.Add(viewEnum.ToString("G"));
                }
            }

            // Assign the name and code of each child page
            if ((FullAggregation.Child_Pages != null) && (FullAggregation.Child_Pages.Count > 0))
            {
                Child_Pages = new List<Simple_Aggregation_Child_Page>();
                foreach (Item_Aggregation_Child_Page thisChild in FullAggregation.Child_Pages)
                {
                    Child_Pages.Add(new Simple_Aggregation_Child_Page( thisChild.Code, thisChild.Label ));
                }
            }
        }


        /// <summary> Language this item aggregation represents  </summary>
        [DataMember(Name = "language")]
        [XmlAttribute("language")]
        [ProtoMember(1)]
        public string Language { get; set; }

        /// <summary> ID for this item aggregation object </summary>
        /// <remarks> The AggregationID for the ALL aggregation is set to -1 by the stored procedure </remarks>
        [DataMember(Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(2)]
        public int ID { get; set; }

        /// <summary> Code for this item aggregation object </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(3)]
        public string Code { get; set; }

        /// <summary> Full name of this item aggregation </summary>
        [DataMember(Name = "name")]
        [XmlElement("name")]
        [ProtoMember(4)]
        public string Name { get; set; }

        /// <summary> Filename for the banner used on most of the aggregation pages, and perhaps the front page as well </summary>
        [DataMember(EmitDefaultValue = false, Name = "bannerImg")]
        [XmlElement("bannerImg")]
        [ProtoMember(5)]
        public string BannerImage { get; set; }

        /// <summary> Date the last item was added to this collection </summary>
        /// <remarks> If there is no record of this, the date of 1/1/2000 is returned </remarks>
        [DataMember(Name = "lastItemAdded")]
        [XmlElement("lastItemAdded")]
        [ProtoMember(6)]
        public DateTime Last_Item_Added { get; set; }

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus should appear in the advanced search drop downs  </summary>
        [DataMember(Name = "searchFields")]
        [XmlArray("searchFields")]
        [XmlArrayItem("metadataType", typeof(Item_Aggregation_Metadata_Type))]
        [ProtoMember(7)]
        public List<Item_Aggregation_Metadata_Type> Search_Fields { get; set; }

        /// <summary> Returns the list of the primary identifiers for all metadata fields which have data and thus could appear in the metadata browse </summary>
        [DataMember(Name = "browseableFields")]
        [XmlArray("browseableFields")]
        [XmlArrayItem("metadataType", typeof(Item_Aggregation_Metadata_Type))]
        [ProtoMember(8)]
        public List<Item_Aggregation_Metadata_Type> Browseable_Fields { get; set; }

        /// <summary> Read-only list of collection views and searches for this item aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "viewsAndSearches")]
        [XmlArray("viewsAndSearches")]
        [XmlArrayItem("view")]
        [ProtoMember(9)]
        public List<string> Views_And_Searches { get; set; }

        /// <summary> Collection of all child pages </summary>
        [DataMember(EmitDefaultValue = false, Name = "childPages")]
        [XmlArray("childPages")]
        [XmlArrayItem("childPage", typeof(Simple_Aggregation_Child_Page))]
        [ProtoMember(10)]
        public List<Simple_Aggregation_Child_Page> Child_Pages { get; set; }

        /// <summary> Return this simple aggregation as JSON </summary>
        /// <returns> This simple aggregation, as a JSON string </returns>
        public string ToJSON()
        {
            return JSON.Serialize(this, Options.ISO8601ExcludeNulls);
        }

        /// <summary> Return this simple aggregation as XML </summary>
        /// <returns> This simple aggregation, as a XML string </returns>
        public string ToXML()
        {
            XmlSerializer x = new XmlSerializer(this.GetType());

            using (StringWriter textWriter = new StringWriter())
            {
                x.Serialize(textWriter, this);
                return textWriter.ToString();
            }
        }

    }
}
