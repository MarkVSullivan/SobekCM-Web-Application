#region Using directives

using System;

#endregion

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Contains information about a single finding guide container related to a digital resource (and ostensibly an EAD )</summary>
    [Serializable]
    public class DisplayItem_Container 
    {
        /// <summary> Gets the name of this container </summary>
        public string name { get; internal set; }

        /// <summary> Gets the type of container this represents (i.e., Box, Folder, etc..)</summary>
        public string type { get; internal set; }

        /// <summary> Gets the level within the container list that this container resides </summary>
        public int? level { get; internal set; }

    }
}