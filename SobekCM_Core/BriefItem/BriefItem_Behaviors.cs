using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Tools;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Specification of how this item should behave within this library </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("behaviors")]
    public class BriefItem_Behaviors
    {
        private Dictionary<string, BriefItem_BehaviorViewer> viewerTypeToConfig;


        /// <summary> List of page file extenions that should be listed in the downloads tab </summary>
        [DataMember(EmitDefaultValue = false, Name = "pageFileExtensionsForDownload")]
        [XmlElement("pageFileExtensionsForDownload")]
        [ProtoMember(1)]
        public string[] Page_File_Extensions_For_Download { get; set; }

        /// <summary> Complete embed html tag for an embedded video ( i.e., from YouTube for example ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "embeddedVideo")]
        [XmlElement("embeddedVideo")]
        [ProtoMember(2)]
        public string Embedded_Video { get; set; }

        /// <summary> List of viewers attached to this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "viewers")]
        [XmlArray("viewers")]
        [XmlArrayItem("viewer", typeof(BriefItem_BehaviorViewer))]
        [ProtoMember(3)]
        public List<BriefItem_BehaviorViewer> Viewers { get; set; }

        /// <summary> Flag indicates which IP ranges have access to this item, or 0 if this is public, or -1 if private </summary>
        [DataMember(EmitDefaultValue = false, Name = "ipRestriction")]
        [XmlAttribute("ipRestriction")]
        [ProtoMember(4)]
        public short IP_Restriction_Membership { get; set; }

        /// <summary> Flag indicates which IP ranges have access to this item, or 0 if this is public, or -1 if private </summary>
        [DataMember(EmitDefaultValue = false, Name = "dark")]
        [XmlAttribute("dark")]
        [ProtoMember(5)]
        public bool Dark_Flag { get; set; }

        /// <summary> List of all the aggregation codes associated with this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "aggregations")]
        [XmlArray("aggregations")]
        [XmlArrayItem("aggregation", typeof(string))]
        [ProtoMember(6)]
        public List<string> Aggregation_Code_List { get; set; }

        /// <summary> Code for the source institution aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "sourceAggregation")]
        [XmlElement("sourceAggregation")]
        [ProtoMember(7)]
        public string Source_Institution_Aggregation { get; set; }

        /// <summary> Code for the holding location aggregation </summary>
        [DataMember(EmitDefaultValue = false, Name = "holdingAggregation")]
        [XmlElement("holdingAggregation")]
        [ProtoMember(8)]
        public string Holding_Location_Aggregation { get; set; }

        /// <summary> Type for the overall item group (title) </summary>
        [DataMember(EmitDefaultValue = false, Name = "groupType")]
        [XmlAttribute("groupType")]
        [ProtoMember(9)] 
        public string GroupType { get; set; }

        /// <summary> Title associated with the overall item group </summary>
        [DataMember(EmitDefaultValue = false, Name = "groupTitle")]
        [XmlAttribute("groupTitle")]
        [ProtoMember(10)]
        public string GroupTitle { get; set; }

        /// <summary> Gets and sets the name of the main thumbnail file </summary>
        [DataMember(EmitDefaultValue = false, Name = "thumbnail")]
        [XmlAttribute("thumbnail")]
        [ProtoMember(11)] 
        public string Main_Thumbnail { get; set; }

        /// <summary> Flag indicates if checkout is required for this item, in which case this is a single use digital item </summary>
        [DataMember(EmitDefaultValue = false, Name = "singleUse")]
        [XmlAttribute("singleUse")]
        [ProtoMember(12)] 
        public bool Single_Use { get; set; }

        /// <summary> List of wordmark codes attached to this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "wordmarks")]
        [XmlArray("wordmarks")]
        [XmlArrayItem("wordmark", typeof(string))]
        [ProtoMember(13)]
        public List<string> Wordmarks { get; set; }

        /// <summary> Flag indicates if this item is full text searchable </summary>
        [DataMember(EmitDefaultValue = false, Name = "textSearchable")]
        [XmlAttribute("textSearchable")]
        [ProtoMember(14)]
        public bool Full_Text_Searchable { get; set; }

        /// <summary> Allows a specific citation set for this item to be set, possibly
        /// overriding the system default to customize the description appearance </summary>
        [DataMember(EmitDefaultValue = false, Name = "citationSet")]
        [XmlAttribute("citationSet")]
        [ProtoMember(15)]
        public string CitationSet { get; set; }

        /// <summary> Key/value pairs of setting values that can be used to store additional
        /// behavior/setting information for a digital resource </summary>
        [DataMember(EmitDefaultValue = false, Name = "settings")]
        [XmlArray("settings")]
        [XmlArrayItem("setting", typeof(StringKeyValuePair))]
        [ProtoMember(16)]
        public List<StringKeyValuePair> Settings { get; set; }

        [XmlIgnore]
        [IgnoreDataMember]
        private Dictionary<string, StringKeyValuePair> settingLookupDictionary; 

        /// <summary> Constructor for a new instance of the BriefItem_Behaviors class </summary>
        public BriefItem_Behaviors()
        {
            Viewers = new List<BriefItem_BehaviorViewer>();
            viewerTypeToConfig = new Dictionary<string, BriefItem_BehaviorViewer>(StringComparer.OrdinalIgnoreCase);
        }

        /// <summary> Add a key/value pair setting to this item </summary>
        /// <param name="Key"> Key for this setting </param>
        /// <param name="Value"> Value for this setting </param>
        /// <remarks> If a value already exists for the provided key, the value will be changed
        /// to the new value provided to this method. </remarks>
        public void Add_Setting(string Key, string Value)
        {
            // Look for existing key that matches

            // Ensure the list is defined
            if (Settings == null) Settings = new List<StringKeyValuePair>();

            // Ensure the dictionary was built
            if (settingLookupDictionary == null) settingLookupDictionary = new Dictionary<string, StringKeyValuePair>(StringComparer.OrdinalIgnoreCase);
            if (settingLookupDictionary.Count != Settings.Count)
            {
                foreach (StringKeyValuePair setting in Settings)
                    settingLookupDictionary[setting.Key] = setting;
            }

            // Does this key already exist?
            if (settingLookupDictionary.ContainsKey(Key))
                settingLookupDictionary[Key].Value = Value;
            else
            {
                StringKeyValuePair newValue = new StringKeyValuePair(Key, Value);
                Settings.Add(newValue);
                settingLookupDictionary[Key] = newValue;
            }
        }

        /// <summary> Gets a setting value, by key </summary>
        /// <param name="Key"> Key to look for a match from the settings key/value pairs </param>
        /// <returns> Either the setting value, if it exists, or NULL </returns>
        public string Get_Setting(string Key)
        {
            // If the list is undefined, return NULL
            if ((Settings == null) || (Settings.Count == 0))
                return null;

            // Ensure the dictionary was built
            if (settingLookupDictionary == null) settingLookupDictionary = new Dictionary<string, StringKeyValuePair>(StringComparer.OrdinalIgnoreCase);
            if (settingLookupDictionary.Count != Settings.Count)
            {
                foreach (StringKeyValuePair setting in Settings)
                    settingLookupDictionary[setting.Key] = setting;
            }

            // Does this key exist?
            return settingLookupDictionary.ContainsKey(Key) ? settingLookupDictionary[Key].Value : null;
        }

        /// <summary> Gets information about a single viewer for this digital resource </summary>
        /// <param name="ViewerType"> Standard type of the viewer to check </param>
        /// <returns> Information about that viewer, or NULL if it does not exist </returns>
        public BriefItem_BehaviorViewer Get_Viewer(string ViewerType)
        {
            int viewers_count = Viewers != null ? Viewers.Count : 0;

            // Was the dictionary configured?
            if ((viewerTypeToConfig == null) || (viewerTypeToConfig.Count != viewers_count))
            {
                // Ensure the dictionary is defined
                if (viewerTypeToConfig == null)
                    viewerTypeToConfig = new Dictionary<string, BriefItem_BehaviorViewer>( StringComparer.OrdinalIgnoreCase);

                // If there are no viewers, just clear the current config
                if ((Viewers == null) || (Viewers.Count == 0))
                {
                    viewerTypeToConfig.Clear();
                    return null;
                }

                // Add each viewer
                foreach (BriefItem_BehaviorViewer thisViewer in Viewers)
                {
                    viewerTypeToConfig[thisViewer.ViewerType] = thisViewer;
                }
            }

            // Check for non-existence
            if (!viewerTypeToConfig.ContainsKey(ViewerType))
                return null;

            // Return the match
            return viewerTypeToConfig[ViewerType];
        }
    }
}
