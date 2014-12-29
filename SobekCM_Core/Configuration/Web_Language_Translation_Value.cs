namespace SobekCM.Core.Configuration
{
    /// <summary> A single translated value, or a pair of the web language and the string itself </summary>
    public class Web_Language_Translation_Value
    {
        /// <summary> Language in which this value is represented </summary>
        public readonly Web_Language_Enum Language;

        /// <summary> Value in provided language </summary>
        public readonly string Value;

        /// <summary> Constructor for a new instance of the Web_Language_Translation_Value class </summary>
        /// <param name="Language"> Language in which this value is represented </param>
        /// <param name="Value"> Value in provided language </param>
        public Web_Language_Translation_Value(Web_Language_Enum Language, string Value)
        {
            this.Language = Language;
            this.Value = Value;
        }
    }
}
