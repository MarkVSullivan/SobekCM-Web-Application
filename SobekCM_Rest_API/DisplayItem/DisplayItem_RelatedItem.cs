#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;
using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Represents information about a related item </summary>
    [DataContract]
    public class DisplayItem_RelatedItem 
    {
        /// <summary> Gets or sets the end date for the related item </summary>
        [DataMember(EmitDefaultValue = false)]
        public string endDate;

        /// <summary> Gets the list of identifiers associated with related item </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Identifier> identifiers { get; internal set; }

        /// <summary> Gets the number of names associated with this related item  </summary> 
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Name> names { get; internal set; }

        /// <summary> Gets the list of notes associated with related item </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Note> notes { get; internal set; }

        /// <summary> Gets or sets the name of the main publisher of this related item </summary>
        [DataMember(EmitDefaultValue = false)]
        public string publisher { get; internal set; }

        /// <summary> Gets or sets the type of relationship this item has to the described item ( i.e., 'otherVersion', 'otherFormat', 'preceding', 'succeeding', etc.. ) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string relationship { get; internal set; }

        /// <summary> Gets or sets the start date for the related item </summary>
        [DataMember(EmitDefaultValue = false)]
        public string startDate { get; internal set; }

        /// <summary> Gets the main title for this related item /summary>
        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_Title title { get; internal set; }

        /// <summary> Gets or sets the SobekCM ID for the related item if hosted in the same library </summary>
        [DataMember(EmitDefaultValue = false)]
        public string localId { get; internal set; }

        /// <summary> Gets or sets the URL information for the related item </summary>
        [DataMember(EmitDefaultValue = false)]
        public DisplayItem_URL url { get; internal set; }
    }
}