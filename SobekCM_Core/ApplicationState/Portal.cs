#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.ApplicationState
{
    /// <summary> A portal represents a single unique appearance and behavior for this digital library
    /// associated with a specific URL and linked to one or more collections and web skins </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("portal")]
    public class Portal
    {
        /// <summary> Constructor for a new instance of the Portal class </summary>
        /// <param name="ID"> Primary key for this portal in the database </param>
        /// <param name="Name"> Name for the library when viewed through this portal </param>
        /// <param name="Abbreviation"> Abbreviation used for the library when viewed through this portal </param>
        /// <param name="Default_Aggregation"> Default aggregation, or 'all' if all aggregationPermissions are available </param>
        /// <param name="Default_Web_Skin"> Default web skin used when displayed through this portal </param>
        /// <param name="URL_Segment"> URL segment used to determine if a request comes from this portal </param>
        /// <param name="Base_PURL"> Base PURL to used when constructing a PURL for items within this portal, if it is different than the standard base URL </param>
        public Portal( int ID, string Name, string Abbreviation, string Default_Aggregation, string Default_Web_Skin, string URL_Segment, string Base_PURL )
        {
            this.ID = ID;
            this.Name = Name;
            this.Abbreviation = Abbreviation;
            this.Default_Aggregation = Default_Aggregation;
            this.Default_Web_Skin = Default_Web_Skin;
            this.URL_Segment = URL_Segment;
            this.Base_PURL = Base_PURL;
        }

        /// <summary> Constructor for a new instance of the Portal class </summary>
        /// <remarks> Empty constructor for serialization purposes </remarks>
        public Portal()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Gets the primary key for this portal in the database </summary>
        [DataMember(EmitDefaultValue = false, Name = "id")]
        [XmlAttribute("id")]
        [ProtoMember(1)]
        public int ID { get; set; }

        /// <summary> Name for the library when viewed through this portal </summary>
        [DataMember(EmitDefaultValue = false, Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        /// <summary> URL segment used to determine if a request comes from this portal </summary>
        [DataMember(EmitDefaultValue = false, Name = "segment")]
        [XmlAttribute("segment")]
        [ProtoMember(3)]
        public string URL_Segment { get; set; }

        /// <summary> Base PURL to used when constructing a PURL for items within this portal, if it is different than the standard base URL </summary>
        [DataMember(EmitDefaultValue = false, Name = "purl")]
        [XmlAttribute("purl")]
        [ProtoMember(4)]
        public string Base_PURL { get; set; }

        /// <summary> Abbreviation used for the library when viewed through this portal </summary>
        [DataMember(EmitDefaultValue = false, Name = "abbreviation")]
        [XmlAttribute("abbreviation")]
        [ProtoMember(5)]
        public string Abbreviation { get; set; }

        /// <summary> Default aggregation, or 'all' if all aggregationPermissions are available </summary>
        [DataMember(EmitDefaultValue = false, Name = "aggregation")]
        [XmlAttribute("aggregation")]
        [ProtoMember(6)]
        public string Default_Aggregation { get; set; }

        /// <summary> Default web skin used when displayed through this portal </summary>
        [DataMember(EmitDefaultValue = false, Name = "webskin")]
        [XmlAttribute("webskin")]
        [ProtoMember(7)]
        public string Default_Web_Skin { get; set; }

        /// <summary> Aggregations which can appear under this portal </summary>
        [DataMember(EmitDefaultValue = false, Name = "aggregations")]
        [XmlArray("aggregations")]
        [XmlArrayItem("aggregation", typeof(string))]
        [ProtoMember(8)]
        public List<string> PossibleAggregations { get; set;  }

        /// <summary> Web skins which can appear under this portal </summary>
        [DataMember(EmitDefaultValue = false, Name = "webskins")]
        [XmlArray("webskins")]
        [XmlArrayItem("webskin", typeof(string))]
        [ProtoMember(9)]
        public List<string> PossibleSkins { get; set;  }

        /// <summary> Flag indicates if this portal limits the web skins which can be displayed </summary>
        [XmlIgnore]
        public bool Web_Skin_Limiting
        {
            get {
                return (PossibleSkins != null) && (PossibleSkins.Count != 0);
            }
        }

        /// <summary> Flag indicates if this portal limits the aggregationPermissions which can be displayed </summary>
        [XmlIgnore]
        public bool Aggregation_Limiting
        {
            get {
                return (PossibleAggregations != null) && (PossibleAggregations.Count != 0);
            }
        }

        /// <summary> Adds a web skin code to the list of possible web skins that can be displayed
        /// through this portal </summary>
        /// <param name="Web_Skin_Code"> New web skin code to add </param>
        public void Add_Possible_Web_Skin(string Web_Skin_Code )
        {
            // Make sure this collection has been defined
            if (PossibleSkins == null)
            {
                PossibleSkins = new List<string> {Default_Web_Skin.ToLower()};
            }

            if (!PossibleSkins.Contains(Web_Skin_Code.ToLower()))
                PossibleSkins.Add(Web_Skin_Code.ToLower());
        }

        /// <summary> Adds a web skin code to the list of possible web skins that can be displayed
        /// through this portal </summary>
        /// <param name="Aggregation_Code"> New web skin code to add </param>
        public void Add_Possible_Aggregation(string Aggregation_Code )
        {
            // Make sure this collection has been defined
            if (PossibleAggregations == null)
            {
                PossibleAggregations = new List<string> {Default_Aggregation.ToLower()};
            }

            if (!PossibleAggregations.Contains(Aggregation_Code.ToLower()))
                PossibleAggregations.Add(Aggregation_Code.ToLower());
        }

        /// <summary> Flag indicates if the provided web skin code is permitted within this portal </summary>
        /// <param name="Web_Skin_Code"> Web skin code to check </param>
        /// <returns> TRUE if permitted (or if this portal is not web skin limiting), otherwise FALSE </returns>
        public bool Is_Possible_Web_Skin(string Web_Skin_Code)
        {
            // If this collection is not defined, then it is not limiting
            return PossibleSkins == null || PossibleSkins.Contains(Web_Skin_Code.ToLower());
        }

        /// <summary> Flag indicates if the provided aggregation code is permitted within this portal </summary>
        /// <param name="Aggregation_Code"> Aggregation code to check </param>
        /// <returns> TRUE if permitted (or if this portal is not aggregation limiting), otherwise FALSE </returns>
        public bool Is_Possible_Aggregation(string Aggregation_Code)
        {
            // If this collection is not defined, then it is not limiting
            return PossibleAggregations == null || PossibleAggregations.Contains(Aggregation_Code.ToLower());
        }
    }
}
