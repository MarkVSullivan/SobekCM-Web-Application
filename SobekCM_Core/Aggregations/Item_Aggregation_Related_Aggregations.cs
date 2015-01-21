#region Using directives

using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
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
        [DataMember(Name = "code"), ProtoMember(1)]
        public readonly string Code;

        /// <summary> Description for this aggregation </summary>
        [DataMember(Name = "description", EmitDefaultValue=false), ProtoMember(2)]
        public readonly string Description;

        /// <summary> Aggregation id for this related aggregation </summary>
        [DataMember(Name = "id", EmitDefaultValue = false), ProtoMember(3)]
        public readonly ushort ID;

        /// <summary> Type of this related aggregation </summary>
        [DataMember(Name = "type", EmitDefaultValue=false), ProtoMember(4)]
        public readonly string Type;

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
            this.Description = Description;
            this.ID = ID;
        }

        /// <summary> Name for this related aggregation </summary>
        [DataMember(Name = "name", EmitDefaultValue = false), ProtoMember(5)]
        public string Name { get; set; }

        /// <summary> Shortened name for this related aggregation </summary>
        [DataMember(Name = "shortName", EmitDefaultValue = false), ProtoMember(6)]
        public string ShortName { get; set; }

        /// <summary> Flag indicates if this aggregation is hidden </summary>
        [DataMember(Name = "isHidden"), ProtoMember(7)]
        public bool Hidden { get; set; }

        /// <summary> Flag indicates if this aggregation is active </summary>
        [DataMember(Name = "isActive"), ProtoMember(8)]
        public bool Active { get; set; }

        /// <summary> External link for this institution </summary>
        [DataMember(Name = "link", EmitDefaultValue = false), ProtoMember(9)]
        public string External_Link { get; set; }

        /// <summary> Gets the read-only collection of children item aggregation objects </summary>
        /// <remarks> You should check the count of children first using the <see cref="Children_Count"/> before using this property.
        /// Even if there are no children, this property creates a readonly collection to pass back out.</remarks>
        [DataMember(Name = "children", EmitDefaultValue = false), ProtoMember(10)]
        public List<Item_Aggregation_Related_Aggregations> Children { get; private set; }

        /// <summary> Gets the number of child item aggregationPermissions present </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Children"/> property.  Even if 
        /// there are no children, the Children property creates a readonly collection to pass back out.</remarks>
        [IgnoreDataMember]
        public int Children_Count
        {
            get
            {
                return Children == null ? 0 : Children.Count;
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
    }
}
