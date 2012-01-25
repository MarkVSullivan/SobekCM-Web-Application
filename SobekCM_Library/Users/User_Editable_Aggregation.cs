namespace SobekCM.Library.Users
{
    /// <summary> Holds the basic data about objects which may be editable by a user.  These objects are 
    /// institutions, collections, and subcollections.</summary>
    public class User_Editable_Aggregation
    {
        /// <summary> Code for this user editable object </summary>
        public readonly string Code;

        /// <summary> Name for this user editable object </summary>
        public readonly string Name;

        /// <summary> Constructor for a new instance of the User_Editable_Aggregation class </summary>
        /// <param name="Code"> Code for this user editable item aggregation</param>
        /// <param name="Name"> Name for this user editable item aggregation </param>
        /// <param name="CanSelect"> Flag indicates if this user can add items to this item aggregation</param>
        /// <param name="CanEditItems"> Flag indicates if this user can edit any items in this item aggregation</param>
        /// <param name="IsCurator"> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </param>
        /// <param name="OnHomePage"> Flag indicates if this user is an admin over this aggregation, and can edit the aggregation home, browse, and info pages</param>
        public User_Editable_Aggregation(string Code, string Name, bool CanSelect, bool CanEditItems, bool IsCurator, bool OnHomePage  )
        {
            this.Code = Code;
            this.Name = Name;
            this.CanSelect = CanSelect;
            this.CanEditItems = CanEditItems;
            this.IsCurator = IsCurator;
            this.OnHomePage = OnHomePage;
        }

        /// <summary> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </summary>
        public bool IsCurator { get; set; }

        /// <summary> Flag indicates if this user has asked to have this aggregation appear on their personalized home page </summary>
        public bool OnHomePage { get; set; }

        /// <summary> Flag indicates if this user can add items to this item aggregation</summary>
        public bool CanSelect { get; set; }

        /// <summary> Flag indicates if this user can edit any items in this item aggregation</summary>
        public bool CanEditItems { get; set; }
    }
}
