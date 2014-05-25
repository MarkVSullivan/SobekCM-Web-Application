namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the abstractHtmlSubwriter class </summary>
    public class abstractHtmlSubwriter_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the abstractHtmlSubwriter_Localization class </summary>
        public abstractHtmlSubwriter_LocalizationInfo() : base()
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
                case "ALEPH Number":
                    ALEPHNumber = Value;
                    break;

                case "Anywhere":
                    Anywhere = Value;
                    break;

                case "Attribution":
                    Attribution = Value;
                    break;

                case "Author":
                    Author = Value;
                    break;

                case "City":
                    City = Value;
                    break;

                case "Country":
                    Country = Value;
                    break;

                case "County":
                    County = Value;
                    break;

                case "Donor":
                    Donor = Value;
                    break;

                case "Frequency":
                    Frequency = Value;
                    break;

                case "Genre":
                    Genre = Value;
                    break;

                case "Identifier":
                    Identifier = Value;
                    break;

                case "Language":
                    Language = Value;
                    break;

                case "Notes":
                    Notes = Value;
                    break;

                case "OCLC Number":
                    OCLCNumber = Value;
                    break;

                case "Place Of Publication":
                    PlaceOfPublication = Value;
                    break;

                case "Publisher":
                    Publisher = Value;
                    break;

                case "Spatial Coverage":
                    SpatialCoverage = Value;
                    break;

                case "State":
                    State = Value;
                    break;

                case "Subject Keywords":
                    SubjectKeywords = Value;
                    break;

                case "Target Audience":
                    TargetAudience = Value;
                    break;

                case "Tickler":
                    Tickler = Value;
                    break;

                case "Title":
                    Title = Value;
                    break;

                case "Tracking Box":
                    TrackingBox = Value;
                    break;

                case "Type":
                    Type = Value;
                    break;

            }
        }
        /// <remarks> 'ALEPH Number' localization string </remarks>
        public string ALEPHNumber { get; private set; }

        /// <remarks> 'Anywhere' localization string </remarks>
        public string Anywhere { get; private set; }

        /// <remarks> 'Attribution' localization string </remarks>
        public string Attribution { get; private set; }

        /// <remarks> 'Author' localization string </remarks>
        public string Author { get; private set; }

        /// <remarks> 'City' localization string </remarks>
        public string City { get; private set; }

        /// <remarks> 'Country' localization string </remarks>
        public string Country { get; private set; }

        /// <remarks> 'County' localization string </remarks>
        public string County { get; private set; }

        /// <remarks> 'Donor' localization string </remarks>
        public string Donor { get; private set; }

        /// <remarks> 'Frequency' localization string </remarks>
        public string Frequency { get; private set; }

        /// <remarks> 'Genre' localization string </remarks>
        public string Genre { get; private set; }

        /// <remarks> 'Identifier' localization string </remarks>
        public string Identifier { get; private set; }

        /// <remarks> 'Language' localization string </remarks>
        public string Language { get; private set; }

        /// <remarks> 'Notes' localization string </remarks>
        public string Notes { get; private set; }

        /// <remarks> 'OCLC Number' localization string </remarks>
        public string OCLCNumber { get; private set; }

        /// <remarks> 'Place of Publication' localization string </remarks>
        public string PlaceOfPublication { get; private set; }

        /// <remarks> 'Publisher' localization string </remarks>
        public string Publisher { get; private set; }

        /// <remarks> 'Spatial Coverage' localization string </remarks>
        public string SpatialCoverage { get; private set; }

        /// <remarks> 'State' localization string </remarks>
        public string State { get; private set; }

        /// <remarks> 'Subject Keywords' localization string </remarks>
        public string SubjectKeywords { get; private set; }

        /// <remarks> 'Target Audience' localization string </remarks>
        public string TargetAudience { get; private set; }

        /// <remarks> 'Tickler' localization string </remarks>
        public string Tickler { get; private set; }

        /// <remarks> 'Title' localization string </remarks>
        public string Title { get; private set; }

        /// <remarks> 'Tracking Box' localization string </remarks>
        public string TrackingBox { get; private set; }

        /// <remarks> 'Type' localization string </remarks>
        public string Type { get; private set; }

    }
}
