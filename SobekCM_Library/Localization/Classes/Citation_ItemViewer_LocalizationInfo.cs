namespace SobekCM.Library.Localization.Classes
{
    /// <summary> Localization class holds all the standard terms utilized by the Citation_ItemViewer class </summary>
    public class Citation_ItemViewer_LocalizationInfo : baseLocalizationInfo
    {
        /// <summary> Constructor for a new instance of the Citation_ItemViewer_Localization class </summary>
        public Citation_ItemViewer_LocalizationInfo() : base()
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
                case "XXX Membership":
                    XXXMembership = Value;
                    break;

                case "XXX STATISTICS":
                    XXXSTATISTICS = Value;
                    break;

                case "Citation":
                    Citation = Value;
                    break;

                case "Date":
                    Date = Value;
                    break;

                case "Download":
                    Download = Value;
                    break;

                case "Flash":
                    Flash = Value;
                    break;

                case "For Definitions Of These Terms See The Definitions On The Main Statistics Page":
                    ForDefinitionsOfTheseTermsSeeTheDefinitionsOnTheMainStatisticsPage = Value;
                    break;

                case "JPEG":
                    JPEG = Value;
                    break;

                case "Map":
                    Map = Value;
                    break;

                case "MARC View":
                    MARCView = Value;
                    break;

                case "Material Information":
                    MaterialInformation = Value;
                    break;

                case "Metadata":
                    Metadata = Value;
                    break;

                case "METS Information":
                    METSInformation = Value;
                    break;

                case "Missing Image":
                    MissingImage = Value;
                    break;

                case "Notes":
                    Notes = Value;
                    break;

                case "Record Information":
                    RecordInformation = Value;
                    break;

                case "Related Items":
                    RelatedItems = Value;
                    break;

                case "Standard View":
                    StandardView = Value;
                    break;

                case "Static":
                    Static = Value;
                    break;

                case "Subjects":
                    Subjects = Value;
                    break;

                case "System Admin Information":
                    SystemAdminInformation = Value;
                    break;

                case "Text":
                    Text = Value;
                    break;

                case "The Data Or Metadata About This Digital Resource Is Available In A Variety Of Metadata Formats For More Information About These Formats See The A Href Httpufdcufledusobekcmmetadata Metadata Section A Of The A Href Httpufdcufledusobekcm Technical Aspects A Information":
                    TheDataOrMetadataAboutThisDigitalResourceIsAvailableInAVarietyOfMetadataFormatsForMoreInformationAboutTheseFormatsSeeTheAHrefHttpufdcufledusobekcmmetadataMetadataSectionAOfTheAHrefHttpufdcufledusobekcmTechnicalAspectsAInformation = Value;
                    break;

                case "The Entered Metadata Is Also Converted To MARC XML Format For Interoperability With Other Library Catalog Systems This Represents The Same Data Available In The A Href XXX MARC VIEW A Except This Is A Static XML File This File Follows The A Href Httpwwwlocgovstandardsmarcxml Marcxml Schema A":
                    TheEnteredMetadataIsAlsoConvertedToMARCXMLFormatForInteroperabilityWithOtherLibraryCatalogSystemsThisRepresentsTheSameDataAvailableInTheAHrefXXXMARCVIEWAExceptThisIsAStaticXMLFileThisFileFollowsTheAHrefHttpwwwlocgovstandardsmarcxmlMarcxmlSchemaA = Value;
                    break;

                case "The Fulltext Of This Item Is Also Available In The Established Standard A Href Httpwwwteicorgindexxml Text Encoding Initiative A TEI Downloadable File":
                    TheFulltextOfThisItemIsAlsoAvailableInTheEstablishedStandardAHrefHttpwwwteicorgindexxmlTextEncodingInitiativeATEIDownloadableFile = Value;
                    break;

                case "The Record Above Was Autogenerated From The METS File":
                    TheRecordAboveWasAutogeneratedFromTheMETSFile = Value;
                    break;

                case "This Item Was Has Been Viewed XXX Times Within XXX Visits Below Are The Details For Overall Usage For This Item Within This Library":
                    ThisItemWasHasBeenViewedXXXTimesWithinXXXVisitsBelowAreTheDetailsForOverallUsageForThisItemWithinThisLibrary = Value;
                    break;

                case "This Metadata File Is The Source Metadata File Submitted Along With All The Digital Resource Files This Contains All Of The Citation And Processing Information Used To Build This Resource This File Follows The Established A Href Httpwwwlocgovstandardsmets Metadata Encoding And Transmission Standard A METS And A Href Httpwwwlocgovstandardsmods Metadata Object Description Schema A MODS This METSMODS File Was Just Read When This Item Was Loaded Into Memory And Used To Display All The Information In The Standard View And Marc View Within The Citation":
                    ThisMetadataFileIsTheSourceMetadataFileSubmittedAlongWithAllTheDigitalResourceFilesThisContainsAllOfTheCitationAndProcessingInformationUsedToBuildThisResourceThisFileFollowsTheEstablishedAHrefHttpwwwlocgovstandardsmetsMetadataEncodingAndTransmissionStandardAMETSAndAHrefHttpwwwlocgovstandardsmodsMetadataObjectDescriptionSchemaAMODSThisMETSMODSFileWasJustReadWhenThisItemWasLoadedIntoMemoryAndUsedToDisplayAllTheInformationIn = Value;
                    break;

                case "Thumbnail":
                    Thumbnail = Value;
                    break;

                case "Total":
                    Total = Value;
                    break;

                case "Usage Statistics":
                    UsageStatistics = Value;
                    break;

                case "View Complete METSMODS":
                    ViewCompleteMETSMODS = Value;
                    break;

                case "View Finding Aid EAD":
                    ViewFindingAidEAD = Value;
                    break;

                case "View MARC XML File":
                    ViewMARCXMLFile = Value;
                    break;

                case "View Teitext File":
                    ViewTeitextFile = Value;
                    break;

                case "Views":
                    Views = Value;
                    break;

                case "Visits":
                    Visits = Value;
                    break;

                case "Zoomable":
                    Zoomable = Value;
                    break;

            }
        }
        /// <remarks> %1=abbrevation of system.  Lists aggregations under which this item appears </remarks>
        public string XXXMembership { get; private set; }

        /// <remarks> '%1 STATISTICS' localization string </remarks>
        public string XXXSTATISTICS { get; private set; }

        /// <remarks> 'Citation' localization string </remarks>
        public string Citation { get; private set; }

        /// <remarks> 'Date' localization string </remarks>
        public string Date { get; private set; }

        /// <remarks> 'Download' localization string </remarks>
        public string Download { get; private set; }

        /// <remarks> 'Flash' localization string </remarks>
        public string Flash { get; private set; }

        /// <remarks> For viewing the item usage statistics </remarks>
        public string ForDefinitionsOfTheseTermsSeeTheDefinitionsOnTheMainStatisticsPage { get; private set; }

        /// <remarks> 'JPEG' localization string </remarks>
        public string JPEG { get; private set; }

        /// <remarks> 'Map' localization string </remarks>
        public string Map { get; private set; }

        /// <remarks> Sub-option under the citation on the item-level nav menu </remarks>
        public string MARCView { get; private set; }

        /// <remarks> Heading for basic material information </remarks>
        public string MaterialInformation { get; private set; }

        /// <remarks> Sub-option under the citation on the item-level nav menu </remarks>
        public string Metadata { get; private set; }

        /// <remarks> Heading for information about the last METS file </remarks>
        public string METSInformation { get; private set; }

        /// <remarks> 'Missing Image' localization string </remarks>
        public string MissingImage { get; private set; }

        /// <remarks> Heading for the list of notes </remarks>
        public string Notes { get; private set; }

        /// <remarks> Heading for th Information about the record itself </remarks>
        public string RecordInformation { get; private set; }

        /// <remarks> Heading for the list of any related items </remarks>
        public string RelatedItems { get; private set; }

        /// <remarks> Sub-option under the citation on the item-level nav menu </remarks>
        public string StandardView { get; private set; }

        /// <remarks> 'Static' localization string </remarks>
        public string Static { get; private set; }

        /// <remarks> Heading for the list of subjects </remarks>
        public string Subjects { get; private set; }

        /// <remarks> Keys for this item from the database </remarks>
        public string SystemAdminInformation { get; private set; }

        /// <remarks> 'Text' localization string </remarks>
        public string Text { get; private set; }

        /// <remarks> Main text at the top of the metadata section </remarks>
        public string TheDataOrMetadataAboutThisDigitalResourceIsAvailableInAVarietyOfMetadataFormatsForMoreInformationAboutTheseFormatsSeeTheAHrefHttpufdcufledusobekcmmetadataMetadataSectionAOfTheAHrefHttpufdcufledusobekcmTechnicalAspectsAInformation { get; private set; }

        /// <remarks> Information about the MarcXML file </remarks>
        public string TheEnteredMetadataIsAlsoConvertedToMARCXMLFormatForInteroperabilityWithOtherLibraryCatalogSystemsThisRepresentsTheSameDataAvailableInTheAHrefXXXMARCVIEWAExceptThisIsAStaticXMLFileThisFileFollowsTheAHrefHttpwwwlocgovstandardsmarcxmlMarcxmlSchemaA { get; private set; }

        /// <remarks> Information about the (optional) TEI file </remarks>
        public string TheFulltextOfThisItemIsAlsoAvailableInTheEstablishedStandardAHrefHttpwwwteicorgindexxmlTextEncodingInitiativeATEIDownloadableFile { get; private set; }

        /// <remarks> Note below the MARC records </remarks>
        public string TheRecordAboveWasAutogeneratedFromTheMETSFile { get; private set; }

        /// <remarks> For viewing the item usage statistics </remarks>
        public string ThisItemWasHasBeenViewedXXXTimesWithinXXXVisitsBelowAreTheDetailsForOverallUsageForThisItemWithinThisLibrary { get; private set; }

        /// <remarks> Information about the METS/MODS file </remarks>
        public string ThisMetadataFileIsTheSourceMetadataFileSubmittedAlongWithAllTheDigitalResourceFilesThisContainsAllOfTheCitationAndProcessingInformationUsedToBuildThisResourceThisFileFollowsTheEstablishedAHrefHttpwwwlocgovstandardsmetsMetadataEncodingAndTransmissionStandardAMETSAndAHrefHttpwwwlocgovstandardsmodsMetadataObjectDescriptionSchemaAMODSThisMETSMODSFileWasJustReadWhenThisItemWasLoadedIntoMemoryAndUsedToDisplayAllTheInformationIn { get; private set; }

        /// <remarks> 'Thumbnail' localization string </remarks>
        public string Thumbnail { get; private set; }

        /// <remarks> 'Total' localization string </remarks>
        public string Total { get; private set; }

        /// <remarks> Sub-option under the citation on the item-level nav menu </remarks>
        public string UsageStatistics { get; private set; }

        /// <remarks> Link for the METS/MODS file </remarks>
        public string ViewCompleteMETSMODS { get; private set; }

        /// <remarks> 'View Finding Aid (EAD)' localization string </remarks>
        public string ViewFindingAidEAD { get; private set; }

        /// <remarks> Link for the MarcXML file </remarks>
        public string ViewMARCXMLFile { get; private set; }

        /// <remarks> Link for the (optional) TEI file </remarks>
        public string ViewTeitextFile { get; private set; }

        /// <remarks> 'Views' localization string </remarks>
        public string Views { get; private set; }

        /// <remarks> 'Visits' localization string </remarks>
        public string Visits { get; private set; }

        /// <remarks> 'Zoomable' localization string </remarks>
        public string Zoomable { get; private set; }

    }
}
