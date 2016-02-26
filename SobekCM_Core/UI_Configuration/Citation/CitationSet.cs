using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.UI_Configuration.Citation
{
    /// <summary> Single citation set used for display purposes </summary>
    /// <remarks> Generally, within a citation configuration there is only ONE citation set.
    /// If unique citation viewers are used or other custom citations used, having 
    /// multiplc citation sets may be useful. </remarks>
    [Serializable, DataContract, ProtoContract]
    [XmlRoot("CitationSet")]
    public class CitationSet
    {
        private Dictionary<string, CitationFieldSet> fieldSetDictionary;

        /// <summary> Name of this citation set that uniquely identifies it </summary>
        [DataMember(Name = "name")]
        [XmlAttribute("name")]
        [ProtoMember(1)]
        public string Name { get; set; }

        /// <summary> Collection of all the field sets, which contains the actual citation 
        /// elements </summary>
        [DataMember(Name = "fieldSets", EmitDefaultValue = false)]
        [XmlArray("fieldSet")]
        [XmlArrayItem("fieldSet", typeof(CitationFieldSet))]
        [ProtoMember(2)]
        public List<CitationFieldSet> FieldSets { get; set; }

        /// <summary> Constuctor for a new instance of the <see cref="CitationSet"/> class. </summary>
        public CitationSet()
        {
            FieldSets = new List<CitationFieldSet>();
        }

        /// <summary> Add a new field set (or return the existing field set, if it exists) to this citation set </summary>
        /// <param name="FieldSetId"> ID that uniquely defines this field set </param>
        /// <param name="DefaultHeading"> Default heading for this field set </param>
        /// <param name="Order"> Order indicates where this should be added, if this is not pre-existing </param>
        /// <param name="AfterID">  </param>
        /// <returns></returns>
        public CitationFieldSet AddFieldSet(string FieldSetId, string DefaultHeading, string Order, string AfterID)
        {
            // Ensure the field set dictionary is built
            if (fieldSetDictionary == null) fieldSetDictionary = new Dictionary<string, CitationFieldSet>( StringComparer.OrdinalIgnoreCase);

            // Is the field set dictionary built?  If not, build it
            if (fieldSetDictionary.Count != FieldSets.Count)
            {
                foreach (CitationFieldSet fieldSet in FieldSets)
                {
                    fieldSetDictionary[fieldSet.ID] = fieldSet;
                }
            }

            // Is there already a match for this ID?
            if (fieldSetDictionary.ContainsKey(FieldSetId))
            {
                CitationFieldSet existingSet = fieldSetDictionary[FieldSetId];
                existingSet.Heading = DefaultHeading;
                return existingSet;
            }

            // Since there was no match, create the new one
            CitationFieldSet newSet = new CitationFieldSet
            {
                ID = FieldSetId,
                Heading = DefaultHeading
            };

            // Add to the dictionary first 
            fieldSetDictionary[FieldSetId] = newSet;

            // Depending on the order requested, add it
            switch (Order.ToLower())
            {
                case "append":
                    FieldSets.Add(newSet);
                    break;

                case "first":
                    FieldSets.Insert(0, newSet);
                    break;

                case "after":
                    if (fieldSetDictionary.ContainsKey(FieldSetId))
                    {
                        int index = FieldSets.IndexOf(fieldSetDictionary[FieldSetId]) + 1;
                        if (index >= FieldSets.Count)
                            FieldSets.Add(newSet);
                        else
                            FieldSets.Insert(index, newSet);
                    }
                    else
                    {
                        FieldSets.Add(newSet);
                    }
                    break;
            }

            // Return the new field set
            return newSet;
        }
    }
}
