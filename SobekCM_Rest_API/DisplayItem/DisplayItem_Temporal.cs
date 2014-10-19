#region Using directives

using System.Runtime.Serialization;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Stores the temporal subject information about a digital resource </summary>
    [DataContract]
    public class DisplayItem_Temporal
    {
        /// <summary> End year for the year range </summary>
        [DataMember(EmitDefaultValue = false)]
        public int endYear { get; internal set;  }

        /// <summary> Start year for the year range </summary>
        [DataMember(EmitDefaultValue = false)]
        public int startYear { get; internal set;  }

        /// <summary> description of the time period (i.e. 'Second Spanish Colonial Period', etc..) </summary>
        [DataMember(EmitDefaultValue = false)]
        public string timePeriod { get; internal set;  }
    }
}