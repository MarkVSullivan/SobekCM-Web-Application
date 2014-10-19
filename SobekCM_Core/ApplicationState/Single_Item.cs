#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.ApplicationState
{
    /// <summary> Stores the basic information about an item which is cached, to allow
    /// for correct display in search results and browses. </summary>
    /// <remarks> One of these objects is cached for each item in the digital library </remarks>
    [DataContract]
    public class Single_Item
    {
        /// <summary> Value determines if this item is restricted to certain IP addresses </summary>
        [DataMember]
        public readonly short IP_Range_Membership;

        /// <summary> Title of this item, for display during search engine indexing </summary>
        /// <remarks> This allows the website to correctly title the resulting page, without having to do a database lookup </remarks>
        [DataMember]
        public readonly string Title;

        /// <summary> Volume identifier for this item within the larger title/item group </summary>
        [DataMember]
        public readonly string VID;

        /// <summary> Constructor for a completely-built instance of the Single_Item class </summary>
        /// <param name="VID"> Volume identifier for this item within the larger title/item group</param>
        /// <param name="IP_Range_Membership"> Value determines if this item is restricted to certain IP addresses</param>
        /// <param name="Title"> Title for this item, for display during search engine optimization </param>
        public Single_Item(string VID, short IP_Range_Membership, string Title )
        {
            this.VID = VID;
            this.IP_Range_Membership = IP_Range_Membership;
            this.Title = Title;
        }

        /// <summary> Flag indicates if this item is publicly available or should not appear for the public </summary>
        public bool Public
        {
            get { return IP_Range_Membership >= 0; }
        }
    }
}
