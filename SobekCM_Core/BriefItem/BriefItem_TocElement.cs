#region Using directives

using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Single element from within a TOC (table of contents) </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItem_TocElement
    {
        /// <summary> Name of this element, for display purposes </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Name;

        /// <summary> Sequence within the list of file groupings that this element should link to </summary>
        [DataMember(Name = "sequence")]
        [XmlAttribute("sequence")]
        [ProtoMember(2)]
        public int Sequence;

        /// <summary> Level within the TOC, if this is a hierarchically organized TOC </summary>
        [DataMember(EmitDefaultValue = false, Name = "level")]
        [XmlIgnore]
        [ProtoMember(3)]
        public int? Level;

        /// <summary> Level within the TOC, if this is a hierarchically organized TOC </summary>
        /// <remarks> This is used for XML serialization primarily.  </remarks>
        [IgnoreDataMember]
        [XmlAttribute("level")]
        public string Level_AsString
        {
            get
            {
                if (Level.HasValue)
                    return Level.ToString();
                else
                    return null;
            }
            set
            {
                int temp;
                if (Int32.TryParse(value, out temp))
                    Level = temp;
            }
        }

        /// <summary> Constructor for a new instance of the BriefItem_TocElement class </summary>
        public BriefItem_TocElement()
        {
            // Does nothing - needed for deserialization
        }

        /// <summary> Shorter version of the name for this toc element, which is either the short label or the type </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public string Shortened_Name
        {
            get
            {
                return shorten(Name);
            }
        }

        private static string shorten(string LongLabel)
        {
            const int SHORT_LENGTH = 30;
            if (LongLabel.Length > SHORT_LENGTH)
            {
                // See if there is a space somewhere convenient
                int spaceLocation = LongLabel.IndexOf(" ", SHORT_LENGTH, StringComparison.Ordinal);
                if (spaceLocation >= 0)
                {
                    return LongLabel.Substring(0, spaceLocation) + "...";
                }

                spaceLocation = LongLabel.IndexOf(" ", SHORT_LENGTH - 5, StringComparison.Ordinal);
                if ((spaceLocation >= 0) && (spaceLocation <= SHORT_LENGTH + 5))
                {
                    return LongLabel.Substring(0, spaceLocation) + "...";
                }

                return LongLabel.Substring(0, SHORT_LENGTH) + "...";
            }

            return LongLabel;
        }
    }
}
