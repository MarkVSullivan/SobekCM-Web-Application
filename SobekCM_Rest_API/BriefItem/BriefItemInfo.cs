using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;
using ProtoBuf;

namespace SobekCM.Rest_API.BriefItem
{
    /// <summary> Brief item object used throughout the UI library to display 
    /// item information, item documents, pages, etc.. </summary>
    [Serializable, DataContract, ProtoContract]
    public class BriefItemInfo
    {
        private Dictionary<string, BriefItem_CitationElement> citationElementLookup;

            /// <summary> Namespace definition used within the brief item (generally within the citation)  </summary>
        [DataMember(EmitDefaultValue = false, Name = "namespaces")]
        [ProtoMember(1)]
        public List<BriefItem_Namespace> Namespaces { private get; set; }

        /// <summary> Citation elements for this item </summary>
        [DataMember(Name = "citation")]
        [ProtoMember(2)]
        public List<BriefItem_CitationElement> Citation { private get; set; }

        /// <summary> Constructor for a new instance of the BriefItemInfo class </summary>
        public BriefItemInfo()
        {
            citationElementLookup = new Dictionary<string, BriefItem_CitationElement>(StringComparer.OrdinalIgnoreCase);
            Citation = new List<BriefItem_CitationElement>();
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

        /// <summary> Add a single citation, by term </summary>
        /// <param name="Term"> Normalized term for this metadata element, as employed by the SobekCM system </param>
        /// <param name="Value"> String version of this single value for a metadata term/type </param>
        public BriefItem_CitationElementValue Add_Citation(string Term, string Value)
        {
            // If the value is NULL or empty, do nothing
            if (String.IsNullOrWhiteSpace(Value))
                return null;

            // Was a value, so look to add it
            BriefItem_CitationElement currentList;
            if ( citationElementLookup.TryGetValue(Term, out currentList))
            {
                return currentList.Add_Value(Value);
            }
            else
            {
                BriefItem_CitationElement newElement = new BriefItem_CitationElement(Term);
                citationElementLookup.Add(Term, newElement);
                Citation.Add(newElement);
                return newElement.Add_Value(Value);
                
            }
        }

        /// <summary> Add a single citation, by term </summary>
        /// <param name="Term"> Normalized term for this metadata element, as employed by the SobekCM system </param>
        /// <param name="Value"> String version of this single value for a metadata term/type </param>
        public void Add_Citation(string Term, ReadOnlyCollection<string> Value)
        {
            // If the value is NULL or empty, do nothing
            if (( Value == null ) || ( Value.Count == 0 ))
                return;

            // Was a value, so look to add it
            BriefItem_CitationElement currentList;
            if (citationElementLookup.TryGetValue(Term, out currentList))
            {
                foreach( string thisValue in Value )
                    currentList.Add_Value(thisValue);
            }
            else
            {
                BriefItem_CitationElement newElement = new BriefItem_CitationElement(Term);
                foreach (string thisValue in Value)
                    newElement.Add_Value(thisValue);
                Citation.Add(newElement);
                citationElementLookup.Add(Term, newElement);
            }
        }
    }
}
