#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Users
{
    /// <summary> Maintains a list of item aggregationPermissions, along with the rights the current user
    /// has over items in that aggregation and the actual item aggregation pages </summary>
    [DataContract]
    public class User_Aggregation_Permissions : iSerializationEvents
    {
        private readonly Dictionary<string, User_Permissioned_Aggregation> code_to_aggregation;

        /// <summary> Constructor for a new instance of the User_Aggregation_Permissions class </summary>
        public User_Aggregation_Permissions()
        {
            code_to_aggregation = new Dictionary<string, User_Permissioned_Aggregation>();

        }

        /// <summary> Gets the collection of user editable item aggregationPermissions </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<User_Permissioned_Aggregation> Aggregations { get; set; }

        /// <summary> Gets the information about detailed permissions for this user over an aggregation </summary>
        /// <param name="Code"> Code for the aggregation in question </param>
        /// <returns> Object with the permissions details, or NULL </returns>
        public User_Permissioned_Aggregation this[string Code]
        {
            get
            {
                if (code_to_aggregation.ContainsKey(Code.ToUpper()))
                    return code_to_aggregation[Code.ToUpper()];

                return null;
            }

        }

        /// <summary> Clears all of the aggregation information collected here </summary>
        public void Clear()
        {
            if (Aggregations != null) Aggregations.Clear();

            code_to_aggregation.Clear();

            Aggregations = null;
        }

        /// <summary> Adds a new aggregation to the list of aggregationPermissions linked here, with detailed permissions </summary>
        /// <param name="Code">Code for this user editable item aggregation</param>
        /// <param name="Name">Name for this user editable item aggregation </param>
        /// <param name="CanSelect">Flag indicates if this user can add items to this item aggregation</param>
        /// <param name="CanEditMetadata">Flag indicates if this user can edit the metadata for any items in this item aggregation</param>
        /// <param name="CanEditBehaviors"> Flag indicated if this user can edit the behavior for any items in this item aggregation </param>
        /// <param name="CanPerformQc"> Flag indicates if this user can perform quality control for any items in this item aggregation </param>
        /// <param name="CanUploadFiles"> Flag indicates if this user can upload new files to any items in this item aggregation </param>
        /// <param name="CanChangeVisibility"> Flag indicates if this user can change the visibility information for any items in this item aggregation </param>
        /// <param name="CanDelete"> Flag indicates if this user can delete any items in this item aggregation </param>
        /// <param name="IsCurator"> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </param>
        /// <param name="OnHomePage">Flag indicates if this user has asked to have this aggregation appear on their personalized home page</param>
        /// <param name="IsAdmin"> Flag indicates if this user is listed athe admin for this aggregation </param>
        /// <param name="GroupDefined"> Flag indicates if these permissions are defined at the group level </param>
		public void Add(string Code, string Name, bool CanSelect, bool CanEditMetadata, bool CanEditBehaviors, bool CanPerformQc, bool CanUploadFiles, bool CanChangeVisibility, bool CanDelete, bool IsCurator, bool OnHomePage, bool IsAdmin, bool GroupDefined)
        {
            // Create the aggregation object
	        User_Permissioned_Aggregation aggrLink = new User_Permissioned_Aggregation(Code.ToUpper(), Name, CanSelect, false, IsCurator, OnHomePage, IsAdmin);
	        aggrLink.CanEditMetadata = CanEditMetadata;
			aggrLink.CanEditBehaviors = CanEditBehaviors;
			aggrLink.CanPerformQc = CanPerformQc;
			aggrLink.CanUploadFiles = CanUploadFiles;
			aggrLink.CanChangeVisibility = CanChangeVisibility;
			aggrLink.CanDelete = CanDelete;
	        aggrLink.GroupDefined = GroupDefined;

            // Add this
            Add(aggrLink);
        }

        /// <summary> Adds a new aggregation to the list of aggregationPermissions linked here, with detailed permissions </summary>
        /// <param name="AggregationInfo"> Information about the detailed permissions for this user over an aggregation </param>
        public void Add(User_Permissioned_Aggregation AggregationInfo )
        {
            // If this was pre-existing, remove the old one from the list 
            if ( code_to_aggregation.ContainsKey(AggregationInfo.Code.ToUpper()) )
            {
                if (Aggregations.Contains(AggregationInfo))
                    Aggregations.Remove(AggregationInfo);
            }

            if (Aggregations == null)
                Aggregations = new List<User_Permissioned_Aggregation>();

            Aggregations.Add(AggregationInfo);
            code_to_aggregation[AggregationInfo.Code.ToUpper()] = AggregationInfo;
        }


        /// <summary> Class is called by the serializer after an item is unserialized </summary>
        public void PostUnSerialization()
        {
            if (Aggregations != null)
            {
                foreach (User_Permissioned_Aggregation thisAggr in Aggregations)
                {
                    code_to_aggregation[thisAggr.Code.ToUpper()] = thisAggr;
                }
            }
        }
    }
}
