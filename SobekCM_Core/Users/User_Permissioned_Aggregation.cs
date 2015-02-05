#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Users
{
    /// <summary> Holds the basic data about aggregationPermissions which may be editable by a user.  These objects are 
    /// institutions, collections, subcollections, etc.. </summary>
    [DataContract]
    public class User_Permissioned_Aggregation
    {
        /// <summary> Code for this user editable object </summary>
        [DataMember]
        public readonly string Code;

        /// <summary> Name for this user editable object </summary>
        [DataMember]
        public readonly string Name;

        /// <summary> Constructor for a new instance of the User_Permissioned_Aggregation class </summary>
        /// <param name="Code"> Code for this user editable item aggregation</param>
        /// <param name="Name"> Name for this user editable item aggregation </param>
        /// <param name="CanSelect"> Flag indicates if this user can add items to this item aggregation</param>
        /// <param name="CanEditItems"> Flag indicates if this user can edit any items in this item aggregation</param>
        /// <param name="IsCurator"> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </param>
        /// <param name="OnHomePage"> Flag indicates if this user is an admin over this aggregation, and can edit the aggregation home, browse, and info pages</param>
		/// <param name="IsAdmin"> Flag indicates if this user is listed as the administrator for this aggregation </param>
        public User_Permissioned_Aggregation(string Code, string Name, bool CanSelect, bool CanEditItems, bool IsCurator, bool OnHomePage, bool IsAdmin  )
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
        [DataMember]
        public bool IsCurator { get; set; }

		/// <summary> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </summary>
        [DataMember]
		public bool IsAdmin { get; set; }

        /// <summary> Flag indicates if this user has asked to have this aggregation appear on their personalized home page </summary>
        [DataMember]
        public bool OnHomePage { get; set; }

        /// <summary> Flag indicates if this user can add items to this item aggregation</summary>
        [DataMember]
        public bool CanSelect { get; set; }

		/// <summary> Flag indicates if this user can edit the metadata for items in this aggregation</summary>
        [DataMember]
		public bool CanEditMetadata { get; set; }

		/// <summary> Flag indicates if this user can edit the behavior for items in this aggregation</summary>
        [DataMember]
		public bool CanEditBehaviors { get; set; }

		/// <summary> Flag indicates if this user can perform quality control for items in this aggregation</summary>
        [DataMember]
		public bool CanPerformQc { get; set; }

		/// <summary> Flag indicates if this user can upload files for items in this aggregation</summary>
        [DataMember]
		public bool CanUploadFiles { get; set; }

		/// <summary> Flag indicates if this user can change the visibility of items ( PRIVATE, PUBLIC, etc.. ) in this aggregation</summary>
        [DataMember]
		public bool CanChangeVisibility { get; set; }

		/// <summary> Flag indicates if this user can delete any items in this aggregation</summary>
        [DataMember]
		public bool CanDelete { get; set;  }

		/// <summary> Flag indicates that this is a group defined link  </summary>
        [DataMember]
		public bool GroupDefined { get; set;  }

		/// <summary> Flag indicates if this user can edit any items in this item aggregation</summary>
        [IgnoreDataMember]
		public bool CanEditItems 
		{
		    get
		    {
		        return CanEditMetadata && CanEditBehaviors && CanPerformQc && CanUploadFiles && CanChangeVisibility && CanDelete;
		    }
			set
			{
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
