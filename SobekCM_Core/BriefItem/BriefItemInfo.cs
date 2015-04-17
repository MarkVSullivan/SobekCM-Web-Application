using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Core.BriefItem
{
    /// <summary> Brief item object used throughout the UI library to display 
    /// item information, item documents, pages, etc.. </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItemInfo
    {
        private readonly Dictionary<string, BriefItem_DescriptiveTerm> descriptionTermLookup;

        /// <summary> Namespace definition used within the brief item (generally within the citation)  </summary>
        [DataMember(EmitDefaultValue = false, Name = "namespaces")]
        [ProtoMember(1)]
        public List<BriefItem_Namespace> Namespaces { get; set; }

        /// <summary> Description/Citation elements for this item </summary>
        [DataMember(EmitDefaultValue = false, Name = "description")]
        [ProtoMember(2)]
        public List<BriefItem_DescriptiveTerm> Description { get; set; }

        /// <summary> Collection of all the image file groupings ( i.e., "pages" of images of different 
        /// types, such as thumbnails, jpegs, and jpeg2000s ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "images")]
        [ProtoMember(3)]
        public List<BriefItem_FileGrouping> Images { get; set; }

        /// <summary> Collection of all the download file groupings (generally all the downloads
        /// of the same file, including all the different file formats ) </summary>
        [DataMember(EmitDefaultValue = false, Name = "downloads")]
        [ProtoMember(4)]
        public List<BriefItem_FileGrouping> Downloads { get; set; }

        /// <summary> Images table of contents, if present </summary>
        [DataMember(EmitDefaultValue = false, Name = "images_toc")]
        [ProtoMember(5)]
        public List<BriefItem_TocElement> Images_TOC { get; set; }

        /// <summary> Downloads table of contents, if present </summary>
        [DataMember(EmitDefaultValue = false, Name = "downloads_toc")]
        [ProtoMember(6)]
        public List<BriefItem_TocElement> Downloads_TOC { get; set; }


        // behaviors

        /// <summary> Constructor for a new instance of the BriefItemInfo class </summary>
        public BriefItemInfo()
        {
            descriptionTermLookup = new Dictionary<string, BriefItem_DescriptiveTerm>(StringComparer.OrdinalIgnoreCase);
            Description = new List<BriefItem_DescriptiveTerm>();
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
    }
}
