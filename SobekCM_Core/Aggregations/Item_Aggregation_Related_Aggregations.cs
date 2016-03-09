#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;
using SobekCM.Core.ApplicationState;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Class contains very basic information about item aggregationPermissions which are related
    /// (either as parent or child) to the main item aggregation </summary>
    /// <remarks> This class is a helper class to the Item_Aggregation class and used by the <see cref="Aggregation_Code_Manager"/>. </remarks>
    [Serializable, DataContract, ProtoContract]
    public class Item_Aggregation_Related_Aggregations : IEquatable<Item_Aggregation_Related_Aggregations>
    {
        /// <summary> Aggregation code for this related aggregation </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary> Description for this aggregation </summary>
        [DataMember(Name = "description", EmitDefaultValue=false)]
        [XmlElement("description")]
        [ProtoMember(2)]
        public string Description { get; set; }

        /// <summary> Aggregation id for this related aggregation </summary>
        [DataMember(Name = "id", EmitDefaultValue = false)]
        [XmlAttribute("id")]
        [ProtoMember(3)]
        public ushort ID { get; set; }

        /// <summary> Type of this related aggregation </summary>
        [DataMember(Name = "type", EmitDefaultValue=false)]
        [XmlAttribute("type")]
        [ProtoMember(4)]
        public string Type { get; set; }

        /// <summary> Constructor for a new instance of the Item_Aggregation_Related_Aggregations class </summary>
        public Item_Aggregation_Related_Aggregations()
        {
            // Paramaterless constructor for serialization
        }

        /// <summary> Constructor for a new instance of the Item_Aggregation_Related_Aggregations class </summary>
        /// <param name="Code"> Aggregation code for this related aggregation</param>
        /// <param name="Name"> Name for this related aggregation</param>
        /// <param name="Type"> Type of this related aggregation</param>
        /// <param name="Hidden"> Flag indicates if this aggregation is hidden</param>
        /// <param name="Active"> Flag indicates if this aggregation is active</param>
        public Item_Aggregation_Related_Aggregations( string Code, string Name, string Type, bool Active, bool Hidden )
        {
            // Save the parameters to the readonly variables
            this.Code = Code;
            this.Name = Name;
            ShortName = Name;
            this.Type = Type;
            this.Hidden = Hidden;
            this.Active = Active;
            ID = 0;
        }

        /// <summary> Constructor for a new instance of the Item_Aggregation_Related_Aggregations class </summary>
        /// <param name="Code"> Aggregation code for this related aggregation</param>
        /// <param name="Name"> Name for this related aggregation</param>
        /// <param name="ShortName"> Short name for this related aggregation </param>
        /// <param name="Type"> Type of this related aggregation</param>
        /// <param name="Hidden"> Flag indicates if this aggregation is hidden</param>
        /// <param name="Active"> Flag indicates if this aggregation is active</param>
        /// <param name="Description">Description for this aggregation</param>
        /// <param name="ID">Primary key for this aggregation from the database </param>
        public Item_Aggregation_Related_Aggregations(string Code, string Name, string ShortName, string Type, bool Active, bool Hidden, string Description, ushort ID )
        {
            // Save the parameters to the readonly variables
            this.Code = Code;
            this.Name = Name;
            this.ShortName = ShortName;
            this.Type = Type;
            this.Hidden = Hidden;
            this.Active = Active;
            if ( !String.IsNullOrEmpty(Description))
                this.Description = Description;
            this.ID = ID;
        }

        /// <summary> Name for this related aggregation </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(5)]
        public string Name { get; set; }

        /// <summary> Shortened name for this related aggregation </summary>
        [DataMember(Name = "shortName", EmitDefaultValue = false)]
        [XmlAttribute("shortName")]
        [ProtoMember(6)]
        public string ShortName { get; set; }

        /// <summary> Flag indicates if this aggregation is hidden </summary>
        [DataMember(Name = "isHidden")]
        [XmlAttribute("isHidden")]
        [ProtoMember(7)]
        public bool Hidden { get; set; }

        /// <summary> Flag indicates if this aggregation is active </summary>
        [DataMember(Name = "isActive")]
        [XmlAttribute("isActive")]
        [ProtoMember(8)]
        public bool Active { get; set; }

        /// <summary> External link for this institution </summary>
        [DataMember(Name = "link", EmitDefaultValue = false)]
        [XmlElement("link")]
        [ProtoMember(9)]
        public string External_Link { get; set; }

        /// <summary> Gets the collection of children item aggregation objects </summary>
        [DataMember(Name = "children", EmitDefaultValue = false)]
        [XmlArray("children")]
        [XmlArrayItem("relatedAggr", typeof(Item_Aggregation_Related_Aggregations))]
        [ProtoMember(10)]
        public List<Item_Aggregation_Related_Aggregations> Children { get; set; }

        /// <summary> Thematic heading, used for placing items on the home page of the entire collection </summary>
        [DataMember(Name = "thematicHeading", EmitDefaultValue = false)]
        [XmlElement("thematicHeading")]
        [ProtoMember(11)]
        public Thematic_Heading Thematic_Heading { get; set; }

        /// <summary> Gets the collection of minimally-reported parent aggregations </summary>
        [DataMember(Name = "parents", EmitDefaultValue = false)]
        [XmlArray("parents")]
        [XmlArrayItem("parent", typeof(Item_Aggregation_Minimal))]
        [ProtoMember(12)]
        public List<Item_Aggregation_Minimal> Parents { get; set; }

        /// <summary> Gets the number of child item aggregations present </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public int Children_Count
        {
            get
            {
                return Children == null ? 0 : Children.Count;
            }
        }

        /// <summary> Gets the number of parent item aggregations present </summary>
        [IgnoreDataMember]
        [XmlIgnore]
        public int Parent_Count
        {
            get
            {
                return Parents == null ? 0 : Parents.Count;
            }
        }


        #region IEquatable<Item_Aggregation_Related_Aggregations> Members

        /// <summary> Checks equality between two Item_Aggregation_Related_Aggregations objects </summary>
        /// <param name="Other"> Other item aggregation object to check</param>
        /// <returns>TRUE if they have the same code, otherwise FALSE</returns>
        public bool Equals(Item_Aggregation_Related_Aggregations Other)
        {
            return (Code == Other.Code);
        }

        #endregion

        /// <summary> Method adds another aggregation as a child of this </summary>
        /// <param name="Child_Aggregation">New child aggregation</param>
        public void Add_Child_Aggregation( Item_Aggregation_Related_Aggregations Child_Aggregation )
        {
            // If the list is currently null, create it
            if (Children == null)
            {
                Children = new List<Item_Aggregation_Related_Aggregations> {Child_Aggregation};
            }
            else
            {
                // If this does not exist, add it
                if (!Children.Contains(Child_Aggregation))
                {
                    Children.Add(Child_Aggregation);
                }
            }
        }

        /// <summary> Method adds a minimally reported aggregation as a parent of this </summary>
        /// <param name="Parent_Aggregation">New child aggregation</param>
        public void Add_Parent_Aggregation(Item_Aggregation_Minimal Parent_Aggregation)
        {
            // If the list is currently null, create it
            if (Parents == null)
            {
                Parents = new List<Item_Aggregation_Minimal> { Parent_Aggregation };
            }
            else
            {
                // If this does not exist, add it
                if (!Parents.Contains(Parent_Aggregation))
                {
                    Parents.Add(Parent_Aggregation);
                }
            }
        }


        /// <summary> Method adds a minimally reported aggregation as a parent of this </summary>
        /// <param name="ParentCode"> Aggregation code for this minimally reported aggregation </param>
        /// <param name="ParentName"> Full name for this minimally reported aggregation </param>
        /// <param name="ParentShortName"> Shortened name for this minimally reported aggregation </param>
        public void Add_Parent_Aggregation(string ParentCode, string ParentName, string ParentShortName)
        {
            // Create the object
            Item_Aggregation_Minimal parentAggregation = new Item_Aggregation_Minimal(ParentCode, ParentName, ParentShortName);

            // If the list is currently null, create it
            if (Parents == null)
            {
                Parents = new List<Item_Aggregation_Minimal> { parentAggregation };
            }
            else
            {
                // If this does not exist, add it
                if (!Parents.Contains(parentAggregation))
                {
                    Parents.Add(parentAggregation);
                }
            }
        }
    }

    /// <summary> Class contains the very minimal amount of data used for some references to aggregations </summary>
    public class Item_Aggregation_Minimal : IEquatable<Item_Aggregation_Minimal>
    {
        /// <summary> Aggregation code for this minimally reported aggregation </summary>
        [DataMember(Name = "code")]
        [XmlAttribute("code")]
        [ProtoMember(1)]
        public string Code { get; set; }

        /// <summary> Full name for this minimally reported aggregation </summary>
        [DataMember(Name = "name", EmitDefaultValue = false)]
        [XmlAttribute("name")]
        [ProtoMember(2)]
        public string Name { get; set; }

        /// <summary> Shortened name for this minimally reported aggregation </summary>
        [DataMember(Name = "shortName", EmitDefaultValue = false)]
        [XmlAttribute("shortName")]
        [ProtoMember(3)]
        public string ShortName { get; set; }

        /// <summary> Constructor for a new instance of the Item_Aggregaiton_Minimal object </summary>
        public Item_Aggregation_Minimal()
        {
            // Parameterless constructor for serialization/deserialization
        }

        /// <summary> Constructor for a new instance of the Item_Aggregaiton_Minimal object </summary>
        /// <param name="Code"> Aggregation code for this minimally reported aggregation </param>
        /// <param name="Name"> Full name for this minimally reported aggregation </param>
        /// <param name="ShortName"> Shortened name for this minimally reported aggregation </param>
        public Item_Aggregation_Minimal(string Code, string Name, string ShortName)
        {
            this.Code = Code;
            this.Name = Name;
            this.ShortName = ShortName;
        }

        /// <summary> Indicates whether the current object is equal to another object of the same type </summary>
        /// <param name="Other"> An object to compare with this object.</param>
        /// <returns> TRUE if the current object is equal to the <paramref name="Other" /> parameter; otherwise, FALSE </returns>
        public bool Equals(Item_Aggregation_Minimal Other)
        {
            return (String.Compare(Other.Code, Code, StringComparison.InvariantCultureIgnoreCase) == 0);
        }

        /// <summary> [Overrides] Returns a hash code for this instance. </summary>
        /// <returns> A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. </returns>
        public override int GetHashCode()
        {
            return ("ItemAggregationMinimal|" + Code ).GetHashCode();
        }
    }
}
