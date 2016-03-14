using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

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

        /// <summary> List of viewers attached to this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "wordmarks")]
        [XmlArray("wordmarks")]
        [XmlArrayItem("wordmark", typeof(BriefItem_Wordmark))]
        [ProtoMember(13)]
        public List<BriefItem_Wordmark> Wordmarks { get; set; }

        /// <summary> Constructor for a new instance of the BriefItem_Behaviors class </summary>
        public BriefItem_Behaviors()
        {
            Viewers = new List<BriefItem_BehaviorViewer>();
            viewerTypeToConfig = new Dictionary<string, BriefItem_BehaviorViewer>();
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

        /// <summary> Add a single workdmark to these behaviors </summary>
        /// <param name="Code"> Code for this new wordmark, which links back to the collection of wordmarks </param>
        /// <param name="Title"> Title for this new wordmark </param>
        /// <param name="HTML"> HTML for this new wordmark </param>
        /// <param name="Link"> Link for this new wordmark </param>
        public void Add_Wordmark( string Code, string Title, string HTML, string Link )
        {
            // Make sure the collection is built
            if (Wordmarks == null)
                Wordmarks = new List<BriefItem_Wordmark>();

            // Build the new wordmark
            BriefItem_Wordmark newWordmark = new BriefItem_Wordmark
            {
                Code = Code, 
                Title = Title, 
                HTML = HTML, 
                Link = Link
            };

            // Add this wordmark
            Wordmarks.Add(newWordmark);
        }
    }
}
