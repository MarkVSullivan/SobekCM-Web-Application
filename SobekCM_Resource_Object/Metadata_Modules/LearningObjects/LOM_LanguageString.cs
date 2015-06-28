#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.LearningObjects
{
    /// <summary> Basic data type used within the learning object metadata to store the 
    /// language of a string and the string </summary>
    [Serializable]
    public class LOM_LanguageString
    {
        /// <summary> Language of the string </summary>
        public string Language { get; set; }

        /// <summary> Value of the string itself </summary>
        public string Value { get; set; }

        /// <summary> Constructor for a new instance of the LOM_LanguageString class </summary>
        public LOM_LanguageString()
        {
            Language = String.Empty;
            Value = String.Empty;
        }

        /// <summary> Constructor for a new instance of the LOM_LanguageString class </summary>
        /// <param name="Value"> Value of the string itself </param>
        /// <param name="Language"> Language of the string </param>
        public LOM_LanguageString( string Value, string Language )
        {
            this.Language = Language;
            this.Value = Value;
        }
    }
}
