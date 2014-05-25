namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Html_Echo_MainWriter class </summary>
    public class Html_Echo_MainWriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Html_Echo_MainWriter_Localization class </summary>
        public Html_Echo_MainWriter_LocalizationInfo() : base()
        {
            // Do nothing
        }

        /// <summary> Adds a localization string ( with key and value ) to this localization class </summary>
        /// <param name="Key"> Key for the new localization string being saved </param>
        /// <param name="Value"> Value for this localization string </param>
        /// <remarks> This overrides the base class's implementation </remarks>
        public override void Add_Localization_String(string Key, string Value)
        {
            // First, add to the localization string dictionary
            base.Add_Localization_String(Key, Value);

            // Assign to custom properties depending on the key
            switch (Key)
            {
                case "ERROR READING THE SOURCE FILE":
                    ERRORREADINGTHESOURCEFILE = Value;
                    break;

            }
        }
        /// <remarks> 'ERROR READING THE SOURCE FILE' localization string </remarks>
        public string ERRORREADINGTHESOURCEFILE { get; private set; }

    }
}
