namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Export_View_ResultsViewer class </summary>
    public class Export_View_ResultsViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Export_View_ResultsViewer_Localization class </summary>
        public Export_View_ResultsViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Export_View_ResultsViewer";
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
                case "This Option Allows You To Export The List Of Results Which Match Your Search Or Browse To An Excel Document Or CSV File For Download":
                    ThisOptionAllowsYouToExportTheListOfResul = Value;
                    break;

                case "Select The File Type Below To Create The Report":
                    SelectTheFileTypeBelowToCreateTheReport = Value;
                    break;

                case "Excel Spreadsheet File XLS":
                    ExcelSpreadsheetFileXLS = Value;
                    break;

                case "Commaseperated Value Text File CSV":
                    CommaseperatedValueTextFileCSV = Value;
                    break;

            }
        }
        /// <remarks> 'This option allows you to export the list of results which match your search or browse to an excel document or CSV file for download.' localization string </remarks>
        public string ThisOptionAllowsYouToExportTheListOfResul { get; private set; }

        /// <remarks> 'Select the file type below to create the report:' localization string </remarks>
        public string SelectTheFileTypeBelowToCreateTheReport { get; private set; }

        /// <remarks> 'Excel Spreadsheet file (XLS)' localization string </remarks>
        public string ExcelSpreadsheetFileXLS { get; private set; }

        /// <remarks> 'Comma-seperated value text file (CSV)' localization string </remarks>
        public string CommaseperatedValueTextFileCSV { get; private set; }

    }
}
