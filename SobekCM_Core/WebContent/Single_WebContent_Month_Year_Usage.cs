#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.WebContent
{
    /// <summary> Object contains the basic usage information for a single web content
    /// page for a single year/month </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("webContentUsage")]
    public class Single_WebContent_Month_Year_Usage
    {
        /// <summary> Year for which this is reporting usage </summary>
        [DataMember(Name = "year")]
        [XmlAttribute("year")]
        [ProtoMember(1)]
        public short Year { get; set; }

        /// <summary> Month for which this is reporting usage </summary>
        [DataMember(Name = "month")]
        [XmlAttribute("month")]
        [ProtoMember(2)]
        public short Month { get; set; }

        /// <summary> Total number of (non-robotic) hits on this page </summary>
        [DataMember(Name = "hits")]
        [XmlAttribute("hits")]
        [ProtoMember(3)]
        public int Hits { get; set; }

        /// <summary> Total number of (non-robotic) hits on this page or on any child pages </summary>
        [DataMember(EmitDefaultValue = false, Name = "hitsHierarchical")]
        [XmlIgnore]
        [ProtoMember(4)]
        public int? HitsHierarchical { get; set; }

        /// <summary> Total number of (non-robotic) hits on this page or on any child pages </summary>
        /// <remarks> This is for the XML serialization portions </remarks>
        [IgnoreDataMember]
        [XmlAttribute("hitsHierarchical")]
        public string HitsHierarchical_AsString
        {
            get { return HitsHierarchical.HasValue ? HitsHierarchical.ToString() : null; }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    HitsHierarchical = temp;
            }
        }

        /// <summary> Consructor for a new instance of the Single_WebContent_Month_Year_Usage class </summary>
        public Single_WebContent_Month_Year_Usage()
        {
            // Empty constructor for serialization purposes
        }

        /// <summary> Consructor for a new instance of the Single_WebContent_Month_Year_Usage class </summary>
        /// <param name="Year"> Year for which this is reporting usage </param>
        /// <param name="Month"> Month for which this is reporting usage </param>
        /// <param name="Hits"> Total number of (non-robotic) hits on this page </param>
        /// <param name="HitsHierarchical"> Total number of (non-robotic) hits on this page or on any child pages </param>
        public Single_WebContent_Month_Year_Usage(short Year, short Month, int Hits, int HitsHierarchical)
        {
            this.Year = Year;
            this.Month = Month;
            this.Hits = Hits;
            if (HitsHierarchical != Hits)
                this.HitsHierarchical = HitsHierarchical;
        }
    }
}
