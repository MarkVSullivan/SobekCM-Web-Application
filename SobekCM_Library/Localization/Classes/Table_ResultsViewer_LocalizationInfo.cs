namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Table_ResultsViewer class </summary>
    public class Table_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Table_ResultsViewer_Localization class </summary>
        public Table_ResultsViewer_LocalizationInfo() : base()
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
                case "Date":
                    Date = Value;
                    break;

                case "No":
                    No = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

            }
        }
        /// <remarks> 'Date' localization string </remarks>
        public string Date { get; private set; }

        /// <remarks> "Short for number, which is the first column of the results when shown as a table.  Number is just the result number, starting with 1." </remarks>
        public string No { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

    }
}
