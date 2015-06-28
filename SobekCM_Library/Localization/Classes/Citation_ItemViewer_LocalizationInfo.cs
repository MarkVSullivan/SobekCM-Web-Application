namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Citation_ItemViewer class </summary>
    public class Citation_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Citation_ItemViewer_Localization class </summary>
        public Citation_ItemViewer_LocalizationInfo()
        {
            // Set the source class name this localization file serves
            ClassName = "Citation_ItemViewer";
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
                case "Standard View":
                    StandardView = Value;
                    break;

                case "MARC View":
                    MARCView = Value;
                    break;

                case "Metadata":
                    Metadata = Value;
                    break;

                case "Usage Statistics":
                    UsageStatistics = Value;
                    break;

                case "This Item Was Has Been Viewed XXX Times Within XXX Visits Below Are The Details For Overall Usage For This Item Within This Library":
                    ThisItemWasHasBeenViewedXXXTimesWithinXXX = Value;
                    break;

                case "For Definitions Of These Terms See The Definitions On The Main Statistics Page":
                    ForDefinitionsOfTheseTermsSeeTheDefinitions = Value;
                    break;

                case "Date":
                    Date = Value;
                    break;

                case "Views":
                    Views = Value;
                    break;

                case "Visits":
                    Visits = Value;
                    break;

                case "JPEG":
                    JPEG = Value;
                    break;

                case "Zoomable":
                    Zoomable = Value;
                    break;

                case "Citation":
                    Citation = Value;
                    break;

                case "Thumbnail":
                    Thumbnail = Value;
                    break;

                case "Text":
                    Text = Value;
                    break;

                case "Flash":
                    Flash = Value;
                    break;

                case "Map":
                    Map = Value;
                    break;

                case "Download":
                    Download = Value;
                    break;

                case "Static":
                    Static = Value;
                    break;

                case "XXX STATISTICS":
                    XXXSTATISTICS = Value;
                    break;

                case "Total":
                    Total = Value;
                    break;

                case "View Finding Aid EAD":
                    ViewFindingAidEAD = Value;
                    break;

                case "Missing Image":
                    MissingImage = Value;
                    break;

                case "The Data Or Metadata About This Digital Resource Is Available In A Variety Of Metadata Formats For More Information About These Formats See The A Href Httpufdcufledusobekcmmetadata Metadata Section A Of The A Href Httpufdcufledusobekcm Technical Aspects A Information":
                    TheDataOrMetadataAboutThisDigitalResourceI = Value;
                    break;

                case "View Complete METSMODS":
                    ViewCompleteMETSMODS = Value;
                    break;

                case "This Metadata File Is The Source Metadata File Submitted Along With All The Digital Resource Files This Contains All Of The Citation And Processing Information Used To Build This Resource This File Follows The Established A Href Httpwwwlocgovstandardsmets Metadata Encoding And Transmission Standard A METS And A Href Httpwwwlocgovstandardsmods Metadata Object Description Schema A MODS This METSMODS File Was Just Read When This Item Was Loaded Into Memory And Used To Display All The Information In The Standard View And Marc View Within The Citation":
                    ThisMetadataFileIsTheSourceMetadataFileSub = Value;
                    break;

                case "View MARC XML File":
                    ViewMARCXMLFile = Value;
                    break;

                case "The Entered Metadata Is Also Converted To MARC XML Format For Interoperability With Other Library Catalog Systems This Represents The Same Data Available In The A Href XXX MARC VIEW A Except This Is A Static XML File This File Follows The A Href Httpwwwlocgovstandardsmarcxml Marcxml Schema A":
                    TheEnteredMetadataIsAlsoConvertedToMARCXML = Value;
                    break;

                case "View Teitext File":
                    ViewTeitextFile = Value;
                    break;

                case "The Fulltext Of This Item Is Also Available In The Established Standard A Href Httpwwwteicorgindexxml Text Encoding Initiative A TEI Downloadable File":
                    TheFulltextOfThisItemIsAlsoAvailableInThe = Value;
                    break;

                case "The Record Above Was Autogenerated From The METS File":
                    TheRecordAboveWasAutogeneratedFromTheMETSF = Value;
                    break;

                case "Material Information":
                    MaterialInformation = Value;
                    break;

                case "Notes":
                    Notes = Value;
                    break;

                case "Subjects":
                    Subjects = Value;
                    break;

                case "Record Information":
                    RecordInformation = Value;
                    break;

                case "Related Items":
                    RelatedItems = Value;
                    break;

                case "XXX Membership":
                    XXXMembership = Value;
                    break;

                case "System Admin Information":
                    SystemAdminInformation = Value;
                    break;

                case "METS Information":
                    METSInformation = Value;
                    break;

            }
        }
        /// <remarks> 'Standard View' localization string </remarks>
        public string StandardView { get; private set; }

        /// <remarks> 'MARC View' localization string </remarks>
        public string MARCView { get; private set; }

        /// <remarks> 'Metadata' localization string </remarks>
        public string Metadata { get; private set; }

        /// <remarks> 'Usage Statistics' localization string </remarks>
        public string UsageStatistics { get; private set; }

        /// <remarks> 'This item was has been viewed %1 times within %2 visits.  Below are the details for overall usage for this item within this library.' localization string </remarks>
        public string ThisItemWasHasBeenViewedXXXTimesWithinXXX { get; private set; }

        /// <remarks> '"For definitions of these terms, see the definitions on the main statistics page."' localization string </remarks>
        public string ForDefinitionsOfTheseTermsSeeTheDefinitions { get; private set; }

        /// <remarks> 'Date' localization string </remarks>
        public string Date { get; private set; }

        /// <remarks> 'Views' localization string </remarks>
        public string Views { get; private set; }

        /// <remarks> 'Visits' localization string </remarks>
        public string Visits { get; private set; }

        /// <remarks> 'JPEG' localization string </remarks>
        public string JPEG { get; private set; }

        /// <remarks> 'Zoomable' localization string </remarks>
        public string Zoomable { get; private set; }

        /// <remarks> 'Citation' localization string </remarks>
        public string Citation { get; private set; }

        /// <remarks> 'Thumbnail' localization string </remarks>
        public string Thumbnail { get; private set; }

        /// <remarks> 'Text' localization string </remarks>
        public string Text { get; private set; }

        /// <remarks> 'Flash' localization string </remarks>
        public string Flash { get; private set; }

        /// <remarks> 'Map' localization string </remarks>
        public string Map { get; private set; }

        /// <remarks> 'Download' localization string </remarks>
        public string Download { get; private set; }

        /// <remarks> 'Static' localization string </remarks>
        public string Static { get; private set; }

        /// <remarks> '%1 STATISTICS' localization string </remarks>
        public string XXXSTATISTICS { get; private set; }

        /// <remarks> 'Total' localization string </remarks>
        public string Total { get; private set; }

        /// <remarks> 'View Finding Aid (EAD)' localization string </remarks>
        public string ViewFindingAidEAD { get; private set; }

        /// <remarks> 'Missing Image' localization string </remarks>
        public string MissingImage { get; private set; }

        /// <remarks> '"The data (or metadata) about this digital resource is available in a variety of metadata formats. For more information about these formats, see the a href=http://ufdc.ufl.edu/sobekcm/metadata"">Metadata Section(/a) of the (a href=""http://ufdc.ufl.edu/sobekcm/"")Technical Aspects(/a) information."' localization string </remarks>
        public string TheDataOrMetadataAboutThisDigitalResourceI { get; private set; }

        /// <remarks> 'View Complete METS/MODS' localization string </remarks>
        public string ViewCompleteMETSMODS { get; private set; }

        /// <remarks> '"This metadata file is the source metadata file submitted along with all the digital resource files. This contains all of the citation and processing information used to build this resource. This file follows the established (a href=""http://www.loc.gov/standards/mets/"")Metadata Encoding and Transmission Standard(/a) (METS) and (a href=""http://www.loc.gov/standards/mods/"")Metadata Object Description Schema(/a) (MODS). This METS/MODS file was just read when this item was loaded into memory and used to display all the information in the standard view and marc view within the citation."' localization string </remarks>
        public string ThisMetadataFileIsTheSourceMetadataFileSub { get; private set; }

        /// <remarks> 'View MARC XML File' localization string </remarks>
        public string ViewMARCXMLFile { get; private set; }

        /// <remarks> '"The entered metadata is also converted to MARC XML format, for interoperability with other library catalog systems.  This represents the same data available in the (a href=""%1"")MARC VIEW(/a) except this is a static XML file.  This file follows the (a href=""http://www.loc.gov/standards/marcxml/"")MarcXML Schema(/a)."' localization string </remarks>
        public string TheEnteredMetadataIsAlsoConvertedToMARCXML { get; private set; }

        /// <remarks> 'View TEI/Text File' localization string </remarks>
        public string ViewTeitextFile { get; private set; }

        /// <remarks> '"The full-text of this item is also available in the established standard (a href=""http://www.tei-c.org/index.xml"")Text Encoding Initiative(/a) (TEI) downloadable file."' localization string </remarks>
        public string TheFulltextOfThisItemIsAlsoAvailableInThe { get; private set; }

        /// <remarks> 'The record above was auto-generated from the METS file.' localization string </remarks>
        public string TheRecordAboveWasAutogeneratedFromTheMETSF { get; private set; }

        /// <remarks> 'Material Information' localization string </remarks>
        public string MaterialInformation { get; private set; }

        /// <remarks> 'Notes' localization string </remarks>
        public string Notes { get; private set; }

        /// <remarks> 'Subjects' localization string </remarks>
        public string Subjects { get; private set; }

        /// <remarks> 'Record Information' localization string </remarks>
        public string RecordInformation { get; private set; }

        /// <remarks> 'Related Items' localization string </remarks>
        public string RelatedItems { get; private set; }

        /// <remarks> '%1 Membership' localization string </remarks>
        public string XXXMembership { get; private set; }

        /// <remarks> 'System Admin Information' localization string </remarks>
        public string SystemAdminInformation { get; private set; }

        /// <remarks> 'METS Information' localization string </remarks>
        public string METSInformation { get; private set; }

    }
}
