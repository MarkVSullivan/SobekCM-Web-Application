using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Resource_Object.Metadata_Modules.LearningObjects
{
    /// <summary> Basic data type used within the learning object metadata to store the 
    /// source of a string and the string </summary>
    [Serializable]
    public class LOM_VocabularyState
    {
        /// <summary> Source or vocabulary from which the string is derived </summary>
        public string Source { get; set; }

        /// <summary> Value of the string itself </summary>
        public string Value { get; set; }

        /// <summary> Constructor for a new instance of the LOM_VocabularyState class </summary>
        public LOM_VocabularyState()
        {
            Source = String.Empty;
            Value = String.Empty;
        }

        /// <summary> Constructor for a new instance of the LOM_VocabularyState class </summary>
        /// <param name="Value"> Value of the string itself </param>
        /// <param name="Source"> Source or vocabulary from which the string is derived </param>
        public LOM_VocabularyState( string Value, string Source )
        {
            this.Value = Value;
            this.Source = Source;
        }
    }
}
