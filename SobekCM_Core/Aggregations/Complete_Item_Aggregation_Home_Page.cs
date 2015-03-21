using System;
using System.Runtime.Serialization;
using ProtoBuf;
using SobekCM.Core.Configuration;

namespace SobekCM.Core.Aggregations
{
    /// <summary> Information about a single home language-specific home page </summary>
    [Serializable, DataContract, ProtoContract]
    public class Complete_Item_Aggregation_Home_Page
    {
        /// <summary> Constructor for a new instance of the Complete_Item_Aggregation_Home_Page class </summary>
        public Complete_Item_Aggregation_Home_Page()
        {
            // Do nothing (used for serialization)
        }

        /// <summary> Constructor for a new instance of the Complete_Item_Aggregation_Home_Page class </summary>
        /// <param name="Source"> Source file for this home page </param>
        /// <param name="isCustomHome"> Flag indicates if this is a custom home page, which will
        /// override all other home page writing methods, and control the rendered page
        /// from the top to the bottom </param>
        /// <param name="Language"> Language for this home page </param>
        public Complete_Item_Aggregation_Home_Page(string Source, bool isCustomHome, Web_Language_Enum Language)
        {
            this.Source = Source;
            this.isCustomHome = isCustomHome;
            this.Language = Language;
        }

        /// <summary> Source file for this home page </summary>
        [DataMember(Name = "source"), ProtoMember(1)]
        public string Source { get; private set; }

        /// <summary> Flag indicates if this is a custom home page, which will
        /// override all other home page writing methods, and control the rendered page
        /// from the top to the bottom </summary>
        [DataMember(Name = "isCustom"), ProtoMember(2)]
        public bool isCustomHome { get; set; }

        /// <summary> Language for this home page </summary>
        [DataMember(Name = "language"), ProtoMember(3)]
        public Web_Language_Enum Language { get; private set; }
    }
}
