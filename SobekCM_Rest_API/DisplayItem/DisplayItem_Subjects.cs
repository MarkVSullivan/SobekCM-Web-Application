using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SobekCM_Rest_API.DisplayItem
{
    /// <summary> Collection of all the subjects related to this digital resource  </summary>
    [DataContract]
    public class DisplayItem_Subjects
    {
        /// <summary> Collection of all the standard subjects </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Subject_Standard> subjects { get; set; }

        /// <summary> Collection of all the names used as subjects </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Subject_Name> nameAsSubjects { get; set; }

        /// <summary> Collection of all the titles used as subjects </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Subject_Name> titleAsSubjects { get; set; }

        /// <summary> Collection of all the cartographic subjects </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Subject_Name> cartographics { get; set; }

        /// <summary> Collection of all the hierarchical geographic subjects </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<DisplayItem_Subject_Name> hierarchicalGeographics { get; set; }

    }
}
