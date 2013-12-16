using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Resource_Object.Metadata_Modules.LearningObjects
{
    /// <summary> The technical capabilities necessary for using this learning object ( IEEE-LOM 4.4 ) </summary>
    [Serializable]
    public class LOM_System_Requirements
    {
        /// <summary> The technology type required to use this learning object ( IEEE-LOM 4.4.1.1 ) </summary>
        public RequirementTypeEnum RequirementType { get; set; }

        /// <summary> Name of the rqeuired technology to use this learning object ( IEEE-LOM 4.4.1.2 ) </summary>
        public LOM_VocabularyState Name { get; set; }

        /// <summary> Lowest possible version of the required technology to use this learning object ( IEEE-LOM 4.4.1.3 ) </summary>
        public string MinimumVersion { get; set; }

        /// <summary> Highest possible version of the required technology to use this learning object ( IEEE-LOM 4.4.1.4 )  </summary>
        public string MaximumVersion { get; set; }

        /// <summary> Constructor for a new instance of the LOM_System_Requirements class </summary>
        public LOM_System_Requirements()
        {
            RequirementType = RequirementTypeEnum.UNDEFINED;
            Name = new LOM_VocabularyState();
        }
    }
}
