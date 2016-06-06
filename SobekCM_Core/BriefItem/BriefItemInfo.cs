#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.BriefItem
{
    /// <summary> Brief item object used throughout the UI library to display 
    /// item information, item documents, pages, etc.. </summary>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("item")]
    public class BriefItemInfo
    {
        private Dictionary<string, BriefItem_DescriptiveTerm> descriptionTermLookup;

        /// <summary> Bibliographic identifier for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "bibid")]
        [XmlAttribute("bibid")]
        [ProtoMember(1)]
        public string BibID { get; set;  }

        /// <summary> Volume identifier for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "vid")]
        [XmlAttribute("vid")]
        [ProtoMember(2)]
        public string VID { get; set; }

        /// <summary> Title for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "title")]
        [XmlElement("title")]
        [ProtoMember(3)]
        public string Title { get; set;  }

        /// <summary> Namespace definition used within the brief item (generally within the citation)  </summary>
        [DataMember(EmitDefaultValue = false, Name = "namespaces")]
        [XmlArray("namespaces")]
        [XmlArrayItem("namespace", typeof(BriefItem_Namespace))]
        [ProtoMember(4)]
        public List<BriefItem_Namespace> Namespaces { get; set; }

        /// <summary> Description/Citation elements for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        [XmlArray("description")]
        [XmlArrayItem("descriptiveTerm", typeof(BriefItem_DescriptiveTerm))]
        [ProtoMember(5)]
        public List<BriefItem_DescriptiveTerm> Description { get; set; }

        /// <summary> Collection of all the image file groupings ( i.e., "pages" of images of different 
        /// types, such as thumbnails, jpegs, and jpeg2000s ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "images")]
        [XmlArray("images")]
        [XmlArrayItem("fileGroup", typeof(BriefItem_FileGrouping))]
        [ProtoMember(6)]
        public List<BriefItem_FileGrouping> Images { get; set; }

        /// <summary> Collection of all the download file groupings (generally all the downloads
        /// of the same file, including all the different file formats ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "downloads")]
        [XmlArray("downloads")]
        [XmlArrayItem("fileGroup", typeof(BriefItem_FileGrouping))]
        [ProtoMember(7)]
        public List<BriefItem_FileGrouping> Downloads { get; set; }

        /// <summary> Images table of contents, if present </summary>
        [DataMember(EmitDefaultValue = false, Name = "images_toc")]
        [XmlArray("images_toc")]
        [XmlArrayItem("division", typeof(BriefItem_TocElement))]
        [ProtoMember(8)]
        public List<BriefItem_TocElement> Images_TOC { get; set; }

        /// <summary> Downloads table of contents, if present </summary>
        [DataMember(EmitDefaultValue = false, Name = "downloads_toc")]
        [XmlArray("downloads_toc")]
        [XmlArrayItem("division", typeof(BriefItem_TocElement))]
        [ProtoMember(9)]
        public List<BriefItem_TocElement> Downloads_TOC { get; set; }

        /// <summary> Basic resource type, which helps to dictate some default behaviors </summary>
        [DataMember(Name = "type")]
        [XmlElement("type")]
        [ProtoMember(10)]
        public string Type { get; set; }

        /// <summary> Behavior information, on how this item behaves in the SobekCM system </summary>
        [DataMember(Name = "behaviors")]
        [XmlElement("behaviors")]
        [ProtoMember(11)]
        public BriefItem_Behaviors Behaviors { get; set; }

        /// <summary> Behavior information, on how this item behaves in the SobekCM system </summary>
        [DataMember(EmitDefaultValue = false, Name = "extensions")]
        [XmlArray("extensions")]
        [XmlArrayItem("extension", typeof(BriefItem_TocElement))]
        [ProtoMember(12)]
        public List<BriefItem_ExtensionData> Extensions { get; set; }

        /// <summary> Additional information for the item on the web, such as the URL </summary>
        [DataMember(Name = "web")]
        [XmlElement("web")]
        [ProtoMember(13)]
        public BriefItem_Web Web { get; set; }

        /// <summary> Geospatial information tied to this digital resource </summary>
        [DataMember(Name = "geospatial")]
        [XmlElement("geospatial")]
        [ProtoMember(14)]
        public BriefItem_GeoSpatial GeoSpatial { get; set; }

        /// <summary> Data about a brief digital object item that is 
        /// computed once by the user interface and stored in the user interface
        /// cache for subsequent needs </summary>
        /// <remarks> This value is only used by the user interface and is not
        /// serializable </remarks>
        [IgnoreDataMember]
        [XmlIgnore]
        public BriefItem_UI UI { get; set; }

        /// <summary> Constructor for a new instance of the BriefItemInfo class </summary>
        public BriefItemInfo()
        {
            descriptionTermLookup = new Dictionary<string, BriefItem_DescriptiveTerm>(StringComparer.OrdinalIgnoreCase);
            Description = new List<BriefItem_DescriptiveTerm>();
            Behaviors = new BriefItem_Behaviors();

            Type = "UNKNOWN";
        }

        /// <summary> Add a new namespace definition to this object </summary>
        /// <param name="Prefix"> Prefix used for this namespace throughout the object </param>
        /// <param name="URI"> URI for the schema/namespace referred to by the prefix </param>
        public void Add_Namespace(string Prefix, string URI)
        {
            if (Namespaces == null)
                Namespaces = new List<BriefItem_Namespace>();

            Namespaces.Add(new BriefItem_Namespace(Prefix, URI));
        }

        /// <summary> Add a single descriptive element, by term </summary>
        /// <param name="Term"> Normalized term for this metadata element, as employed by the SobekCM system </param>
        /// <param name="Value"> String version of this single value for a metadata term/type </param>
        public BriefItem_DescTermValue Add_Description(string Term, string Value)
        {
            // If the value is NULL or empty, do nothing
            if (String.IsNullOrWhiteSpace(Value))
                return null;

            // Was a value, so look to add it
            BriefItem_DescriptiveTerm currentList;
            if (descriptionTermLookup.TryGetValue(Term, out currentList))
            {
                return currentList.Add_Value(Value);
            }
            else
            {
                BriefItem_DescriptiveTerm newElement = new BriefItem_DescriptiveTerm(Term);
                descriptionTermLookup.Add(Term, newElement);
                Description.Add(newElement);
                return newElement.Add_Value(Value);
                
            }
        }

        /// <summary> Add a single descriptive element, by term </summary>
        /// <param name="Term"> Normalized term for this metadata element, as employed by the SobekCM system </param>
        /// <param name="Value"> String version of this single value for a metadata term/type </param>
        public void Add_Description(string Term, ReadOnlyCollection<string> Value)
        {
            // If the value is NULL or empty, do nothing
            if (( Value == null ) || ( Value.Count == 0 ))
                return;

            // Was a value, so look to add it
            BriefItem_DescriptiveTerm currentList;
            if (descriptionTermLookup.TryGetValue(Term, out currentList))
            {
                foreach( string thisValue in Value )
                    currentList.Add_Value(thisValue);
            }
            else
            {
                BriefItem_DescriptiveTerm newElement = new BriefItem_DescriptiveTerm(Term);
                foreach (string thisValue in Value)
                    newElement.Add_Value(thisValue);
                Description.Add(newElement);
                descriptionTermLookup.Add(Term, newElement);
            }
        }

        /// <summary> Add a fully built descriptive element, by term </summary>
        /// <param name="TermObject"> Fully built descriptive term element, as employed by the SobekCM system </param>
        public void Add_Description(BriefItem_DescriptiveTerm TermObject )
        {
            // Was a value, so look to add it
            if (!descriptionTermLookup.ContainsKey(TermObject.Term))
            {
                descriptionTermLookup.Add(TermObject.Term, TermObject);
                Description.Add(TermObject);
            }
        }

        /// <summary> Checks the description and returns any descriptions linked to the provided term </summary>
        /// <param name="Term"> Key for this term </param>
        /// <returns> Either the information about values matching that term, or NULL </returns>
        public BriefItem_DescriptiveTerm Get_Description(string Term)
        {
            // Ensure the dictionary is built first
            if (descriptionTermLookup == null)
                descriptionTermLookup = new Dictionary<string, BriefItem_DescriptiveTerm>( StringComparer.OrdinalIgnoreCase);

            // Ensure the dictionary count matches the description count
            if (descriptionTermLookup.Count != Description.Count)
            {
                descriptionTermLookup.Clear();
                foreach (BriefItem_DescriptiveTerm thisTerm in Description)
                {
                    descriptionTermLookup[thisTerm.Term] = thisTerm;
                }
            }

            // Now, look to see if it exists
            return descriptionTermLookup.ContainsKey(Term) ? descriptionTermLookup[Term] : null;
        }

        /// <summary> Look for the sequence for a page with a matching filename (without extension) </summary>
        /// <param name="FileName"> Name of the file, without the extension </param>
        /// <returns> Sequence of the matching page, or -1 if no match exists </returns>
        public int Page_Sequence_By_FileName(string FileName)
        {
            // If no pages, then no match
            if ((Images == null) || (Images.Count == 0))
                return -1;

            // Step through looking for matches (this occurs very infrequently, so no dictionary overhead)
            int sequence = 1;
            foreach (BriefItem_FileGrouping page in Images)
            {
                if ((page.Files != null) && (page.Files.Count > 0))
                {
                    foreach (BriefItem_File thisFile in page.Files)
                    {
                        if (thisFile.Name.IndexOf(FileName + ".", StringComparison.OrdinalIgnoreCase) == 0)
                            return sequence;
                    }
                }

                sequence++;
            }

            // No match
            return -1;
        }
    }
}
