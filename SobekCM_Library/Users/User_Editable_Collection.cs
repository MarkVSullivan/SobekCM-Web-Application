#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.Users
{
    /// <summary> Maintains a list of item aggregations, along with the rights the current user
    /// has over items in that aggregation and the actual item aggregation pages </summary>
    public class User_Editable_Collection
    {
        private readonly List<string> canEditCodes;
        private readonly List<User_Editable_Aggregation> collection;

        /// <summary> Constructor for a new instance of the User_Editable_Collection class </summary>
        public User_Editable_Collection()
        {
            collection = new List<User_Editable_Aggregation>();
            canEditCodes = new List<string>();
        }

        /// <summary> Gets the collection of user editable item aggregations </summary>
        public ReadOnlyCollection<User_Editable_Aggregation> Collection
        {
            get { return new ReadOnlyCollection<User_Editable_Aggregation>(collection); }
        }

        /// <summary> Clears all of the aggregation information collected here </summary>
        public void Clear()
        {
            collection.Clear();
            canEditCodes.Clear();
        }

        /// <summary> Add a new element to this collection </summary>
        /// <param name="Code">Code for this user editable item aggregation</param>
        /// <param name="Name">Name for this user editable item aggregation </param>
        /// <param name="CanSelect">Flag indicates if this user can add items to this item aggregation</param>
        /// <param name="CanEditItems">Flag indicates if this user can edit any items in this item aggregation</param>
        /// <param name="IsCurator"> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </param>
        /// <param name="OnHomePage">Flag indicates if this user has asked to have this aggregation appear on their personalized home page</param>
        public void Add(string Code, string Name, bool CanSelect, bool CanEditItems, bool IsCurator, bool OnHomePage)
        {
            collection.Add(new User_Editable_Aggregation(Code.ToUpper(), Name, CanSelect, CanEditItems, IsCurator, OnHomePage));
            if (CanEditItems)
            {
                canEditCodes.Add(Code.ToUpper());
            }
        }

        /// <summary> Checks to see if this code exists and can be edited </summary>
        /// <param name="Code">Code for this user editable item aggregation</param>
        /// <returns>TRUE if the element exists and can be edited, otherwise FALSE</returns>
        public bool Can_Edit(string Code)
        {
            return canEditCodes.Contains(Code.ToUpper());
        }
    }
}
