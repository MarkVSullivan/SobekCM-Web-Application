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

	    private bool can_edit_items;

        /// <summary> Constructor for a new instance of the User_Editable_Aggregation class </summary>
        /// <param name="Code"> Code for this user editable item aggregation</param>
        /// <param name="Name"> Name for this user editable item aggregation </param>
        /// <param name="CanSelect"> Flag indicates if this user can add items to this item aggregation</param>
        /// <param name="CanEditItems"> Flag indicates if this user can edit any items in this item aggregation</param>
        /// <param name="IsCurator"> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </param>
        /// <param name="OnHomePage"> Flag indicates if this user is an admin over this aggregation, and can edit the aggregation home, browse, and info pages</param>
		/// <param name="IsAdmin"> Flag indicates if this user is listed as the administrator for this aggregation </param>
        public User_Editable_Aggregation(string Code, string Name, bool CanSelect, bool CanEditItems, bool IsCurator, bool OnHomePage, bool IsAdmin  )
        {
            this.Code = Code;
            this.Name = Name;
            this.CanSelect = CanSelect;
            this.CanEditItems = CanEditItems;
            this.IsCurator = IsCurator;
            this.OnHomePage = OnHomePage;
	        this.IsAdmin = IsAdmin;
	        GroupDefined = false;
        }

        /// <summary> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </summary>
        public bool IsCurator { get; set; }

		/// <summary> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </summary>
		public bool IsAdmin { get; set; }

        /// <summary> Flag indicates if this user has asked to have this aggregation appear on their personalized home page </summary>
        public bool OnHomePage { get; set; }

        /// <summary> Flag indicates if this user can add items to this item aggregation</summary>
        public bool CanSelect { get; set; }

		/// <summary> Flag indicates if this user can edit the metadata for items in this aggregation</summary>
		public bool CanEditMetadata { get; set; }

		/// <summary> Flag indicates if this user can edit the behavior for items in this aggregation</summary>
		public bool CanEditBehaviors { get; set; }

		/// <summary> Flag indicates if this user can perform quality control for items in this aggregation</summary>
		public bool CanPerformQc { get; set; }

		/// <summary> Flag indicates if this user can upload files for items in this aggregation</summary>
		public bool CanUploadFiles { get; set; }

		/// <summary> Flag indicates if this user can change the visibility of items ( PRIVATE, PUBLIC, etc.. ) in this aggregation</summary>
		public bool CanChangeVisibility { get; set; }

		/// <summary> Flag indicates if this user can delete any items in this aggregation</summary>
		public bool CanDelete { get; set;  }

		/// <summary> Flag indicates that this is a group defined link  </summary>
		public bool GroupDefined { get; set;  }

		/// <summary> Flag indicates if this user can edit any items in this item aggregation</summary>
		public bool CanEditItems 
		{
			get { return can_edit_items; }
			set
			{
				can_edit_items = value;
				CanEditMetadata = value;
				CanEditBehaviors = value;
				CanPerformQc = value;
				CanUploadFiles = value;
				CanChangeVisibility = value;
				CanDelete = value;
			}
		}
    }
}
