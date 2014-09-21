#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.Aggregations
{
    /// <summary> Class contains very basic information about item aggregationPermissions which are related
    /// (either as parent or child) to the main item aggregation </summary>
    /// <remarks> This class is a helper class to the <see cref="Item_Aggregation"/> class. </remarks>
    [Serializable]
    public class Item_Aggregation_Related_Aggregations : IEquatable<Item_Aggregation_Related_Aggregations>
    {
        /// <summary> Aggregation code for this related aggregation </summary>
        public readonly string Code;

        /// <summary> Description for this aggregation </summary>
        public readonly string Description;

        /// <summary> Aggregation id for this related aggregation </summary>
        public readonly ushort ID;

        /// <summary> Type of this related aggregation </summary>
        public readonly string Type;

        /// <summary> Stores the pointer to the child aggregation </summary>
        private List<Item_Aggregation_Related_Aggregations> children;

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
            External_Link = String.Empty;
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
            External_Link = String.Empty;
        }

        /// <summary> Name for this related aggregation </summary>
        public string Name { get; set; }

        /// <summary> Shortened name for this related aggregation </summary>
        public string ShortName { get; set; }

        /// <summary> Flag indicates if this aggregation is hidden </summary>
        public bool Hidden { get; set; }

        /// <summary> Flag indicates if this aggregation is active </summary>
        public bool Active { get; set; }

        /// <summary> External link for this institution </summary>
        public string External_Link { get; set; }

        /// <summary> Gets the number of child item aggregationPermissions present </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Children"/> property.  Even if 
        /// there are no children, the Children property creates a readonly collection to pass back out.</remarks>
        public int Children_Count
        {
            get
            {
                if (children == null)
                    return 0;
                return children.Count;
            }
        }

        /// <summary> Gets the read-only collection of children item aggregation objects </summary>
        /// <remarks> You should check the count of children first using the <see cref="Children_Count"/> before using this property.
        /// Even if there are no children, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Item_Aggregation_Related_Aggregations> Children
        {
            get
            {
                if (children == null)
                    return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(new List<Item_Aggregation_Related_Aggregations>());

                return new ReadOnlyCollection<Item_Aggregation_Related_Aggregations>(children);
            }
        }

        #region IEquatable<Item_Aggregation_Related_Aggregations> Members

        /// <summary> Checks equality between two Item_Aggregation_Related_Aggregations objects </summary>
        /// <param name="other"> Other item aggregation object to check</param>
        /// <returns>TRUE if they have the same code, otherwise FALSE</returns>
        public bool Equals(Item_Aggregation_Related_Aggregations other)
        {
            return (Code == other.Code);
        }

        #endregion

        /// <summary> Method adds another aggregation as a child of this </summary>
        /// <param name="Child_Aggregation">New child aggregation</param>
        internal void Add_Child_Aggregation( Item_Aggregation_Related_Aggregations Child_Aggregation )
        {
            // If the list is currently null, create it
            if (children == null)
            {
                children = new List<Item_Aggregation_Related_Aggregations> {Child_Aggregation};
            }
            else
            {
                // If this does not exist, add it
                if (!children.Contains(Child_Aggregation))
                {
                    children.Add(Child_Aggregation);
                }
            }
        }
    }
}
