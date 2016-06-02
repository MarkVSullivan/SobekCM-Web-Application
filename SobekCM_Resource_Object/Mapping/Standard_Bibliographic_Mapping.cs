using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Resource_Object.Metadata_Modules.VRACore;

namespace SobekCM.Resource_Object.Mapping
{
    /// <summary> Standard bibliographic mapping which takes data and field name and maps that data into the SobekCM field
    /// within the main SobekCM Item package </summary>
    /// <remarks> This class implements the <see cref="iBibliographicMapping" /> interface </remarks>
    public class Standard_Bibliographic_Mapping: iBibliographicMapping
    {
        // declare static class variables
        private string month;
        private string day;
        private string year;
        private string date_string;

        /// <summary> Builds a complete date string based on each of the individual components passed in through 
        /// the mappings ( day, month, year ) </summary>
        /// <param name="Package"> Item to add the built date string to </param>
        private void build_date_string(SobekCM_Item Package)
        {
            // check if all the date components exist in the static variables
            if ((month.Length > 0) && (day.Length > 0) && (year.Length > 0))
            {
                date_string = month + day + year;
                string expression = @"\d{" + date_string.Length + "}";
                if (Regex.IsMatch(date_string, expression))
                {
                    // the date string is numeric; separate the date parts with a slash ("/") character.
                    date_string = month + "/" + day + "/" + year;
                }
                else
                {
                    // the date string is not numeric; separate the date parts with a space (" ") character.
                    date_string = month + " " + day + ", " + year;
                }
            }
            else if ((month.Length > 0) && (day.Length > 0))
            {
                // use the month and day parts if the year is missing

                // check if the month part contains all characters and the day part contains all digits
                string month_expression = @"[a-zA-Z]{" + month.Length + "}";
                string day_expression = @"\d{" + day.Length + "}";

                if ((Regex.IsMatch(month, month_expression)) && (Regex.IsMatch(day, day_expression)))
                {
                    // the month part is non-numeric and the day part is numeric; separate the date parts with a space (" ") character.
                    date_string = month + " " + day;
                }
                else
                {
                    // the date parts failed the expression; reset the date_string variable.
                    date_string = String.Empty;
                }
            }
            else if (year.Length > 0)
            {
                // use the year part if the other two parts are missing
                //date_string = year;

                // check if there is a month part
                if (month.Length > 0)
                    // use both the month and the year as the date string
                    date_string = month + " " + year;
                else
                    // use only the year value as the date string
                    date_string = year;
            }
            else
                date_string = String.Empty;

            // copy the date_string value to the bib package field
            Package.Bib_Info.Origin_Info.Date_Issued = date_string;
        }

        /// <summary> Adds a bit of data to a bibliographic package using the mapping </summary>
        /// <param name="Package">Bibliographic package to receive the data</param>
        /// <param name="Data">Text of the data</param>
        /// <param name="Field">Mapped field</param>
        public void Add_Data(SobekCM_Item Package, string Data, string Field)
        {
            // If no field listed, just skip
            if (String.IsNullOrEmpty(Field))
                return;

            // If no data listed, just skip
            if (String.IsNullOrWhiteSpace(Data))
                return;

            // Trim the data
            Data = Data.Trim();

            // Normalize the field name
            string correctName = Field.ToUpper().Replace("#", "").Replace(" ", "").Replace(".", "").Replace(":", "").Replace("\\", "").Replace("/", "").Replace(")", "").Replace("(", "").Trim();
            if (correctName.Length == 0)
            {
                correctName = "None";
            }
            else
            {
                // Find the first number
                int charIndex = 0;
                while ((charIndex < correctName.Length) && (!Char.IsNumber(correctName[charIndex])))
                {
                    charIndex++;
                }

                // If the index stopped before the end (that is, it found a number), 
                // trim the number of the column name
                if ((charIndex < correctName.Length) && (charIndex > 0))
                {
                    correctName = correctName.Substring(0, charIndex);
                }

                // If it was all numbers, just assign NONE
                if (charIndex == 0)
                {
                    correctName = "None";
                }
            }

            // Everything depends on the field which is mapped
            switch (correctName)
            {
                case "NONE":
                    // Do nothing, since no mapping exists
                    break;

                case "ABSTRACT":
                    Package.Bib_Info.Add_Abstract(Data, "en");
                    break;

                case "ACCESSION":
                case "ACCESSIONNUMBER":
                    Package.Bib_Info.Add_Identifier(Data, "Accession Number");
                    break;

                case "ALTERNATETITLE":
                case "ALTTITLE":
                    Package.Bib_Info.Add_Other_Title(Data, Title_Type_Enum.Alternative);
                    break;

                case "ALTERNATETITLELANGUAGE":
                case "ALTTITLELANGUAGE":
                    List<Title_Info> otherTitles = Package.Bib_Info.Other_Titles.Where(ThisTitle => ThisTitle.Title_Type == Title_Type_Enum.Alternative).ToList();
                    if (otherTitles.Count > 0)
                    {
                        otherTitles[otherTitles.Count - 1].Language = Data;
                    }
                    break;

                case "ATTRIBUTION":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.Funding);
                    break;


                case "COLLECTION":
                case "COLLECTIONCODE":
                case "AGGREGATION":
                case "AGGREGATIONCODE":
                case "SUBCOLLECTION":
                case "SUBCOLLECTIONS":
                    Package.Behaviors.Add_Aggregation(Data.ToUpper());
                    break;

                case "CLASSIFICATION":
                    Package.Bib_Info.Add_Classification(Data);
                    break;

                case "CLASSIFICATIONTYPE":
                case "CLASSIFICATIONAUTHORITY":
                    if (Package.Bib_Info.Classifications_Count > 0)
                    {
                        Package.Bib_Info.Classifications[Package.Bib_Info.Classifications_Count - 1].Authority = Data;
                    }
                    break;

                case "CONTRIBUTOR":
                case "CONTRIBUTORS":
                    Package.Bib_Info.Add_Named_Entity(new Name_Info(Data, "contributor"));
                    break;

                case "CREATOR":
                case "CREATORS":
                case "AUTHOR":
                    Package.Bib_Info.Add_Named_Entity(new Name_Info(Data, "creator"));
                    break;

                case "CREATORAFFILIATION":
                case "AUTHORAFFILIATION":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1].Affiliation = Data;
                    }
                    break;

                case "CREATORDATES":
                case "AUTHORDATES":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1].Dates = Data;
                    }
                    break;

                case "CREATORFAMILYNAME":
                case "AUTHORFAMILYNAME":
                case "FAMILYNAME":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Name_Info lastNamedEntity = Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1];
                        if (lastNamedEntity.Family_Name.Length == 0)
                            lastNamedEntity.Family_Name = Data;
                        else
                        {
                            Name_Info newNameEntity = new Name_Info { Family_Name = Data };
                            Package.Bib_Info.Add_Named_Entity(newNameEntity);
                        }
                    }
                    else
                    {
                        Name_Info newNameEntity = new Name_Info { Family_Name = Data };
                        Package.Bib_Info.Add_Named_Entity(newNameEntity);
                    }
                    break;

                case "CREATORGIVENNAME":
                case "AUTHORGIVENNAME":
                case "GIVENNAME":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Name_Info lastNamedEntity = Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1];
                        if (lastNamedEntity.Given_Name.Length == 0)
                            lastNamedEntity.Given_Name = Data;
                        else
                        {
                            Name_Info newNameEntity = new Name_Info { Given_Name = Data };
                            Package.Bib_Info.Add_Named_Entity(newNameEntity);
                        }
                    }
                    else
                    {
                        Name_Info newNameEntity = new Name_Info { Given_Name = Data };
                        Package.Bib_Info.Add_Named_Entity(newNameEntity);
                    }
                    break;

                case "CREATORROLE":
                case "AUTHORROLE":
                case "CREATORROLES":
                case "AUTHORROLES":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Name_Info thisCreator = Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1];
                        if ((thisCreator.Roles.Count == 1) && ((thisCreator.Roles[0].Role == "creator") || (thisCreator.Roles[1].Role == "contributor")))
                            thisCreator.Roles.Clear();
                        Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1].Add_Role(Data);
                    }
                    break;

                case "CULTURALCONTEXT":
                    VRACore_Info vraCoreInfo = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo == null)
                    {
                        vraCoreInfo = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo);
                    }
                    vraCoreInfo.Add_Cultural_Context(Data);
                    break;

                case "DONOR":
                    Package.Bib_Info.Donor.Full_Name = Data;
                    break;

                case "GENRE":
                    Package.Bib_Info.Add_Genre(Data);
                    break;

                case "GENREAUTHORITY":
                    if (Package.Bib_Info.Genres_Count > 0)
                    {
                        Package.Bib_Info.Genres[Package.Bib_Info.Genres_Count - 1].Authority = Data;
                    }
                    break;

                case "HOLDINGLOCATIONCODE":
                case "HOLDINGCODE":
                    Package.Bib_Info.Location.Holding_Code = Data;
                    break;

                case "HOLDINGLOCATIONSTATEMENT":
                case "HOLDINGSTATEMENT":
                    Package.Bib_Info.Location.Holding_Name = Data;
                    break;

                case "IDENTIFIER":
                    Package.Bib_Info.Add_Identifier(Data);
                    break;

                case "IDENTIFIERTYPE":
                    if (Package.Bib_Info.Identifiers_Count > 0)
                    {
                        Package.Bib_Info.Identifiers[Package.Bib_Info.Identifiers_Count - 1].Type = Data;
                    }
                    break;

                case "INSCRIPTION":
                    VRACore_Info vraCoreInfo8 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo8 == null)
                    {
                        vraCoreInfo8 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo8);
                    }
                    vraCoreInfo8.Add_Inscription(Data);
                    break;

                case "LANGUAGE":
                    Package.Bib_Info.Add_Language(Data);
                    break;

                case "PUBLISHER":
                case "PUBLISHERS":
                    Package.Bib_Info.Add_Publisher(Data);
                    break;

                case "PLACEOFPUBLICATION":
                case "PUBLICATIONPLACE":
                case "PUBPLACE":
                case "PUBLICATIONLOCATION":
                case "PLACE":
                    Package.Bib_Info.Origin_Info.Add_Place(Data);
                    break;

                case "RELATEDURLLABEL":
                    Package.Bib_Info.Location.Other_URL_Display_Label = Data;
                    break;

                case "RELATEDURL":
                case "RELATEDURLLINK":
                    Package.Bib_Info.Location.Other_URL = Data;
                    break;

                case "RELATEDURLNOTE":
                case "RELATEDURLNOTES":
                    Package.Bib_Info.Location.Other_URL_Note = Data;
                    break;

                case "SOURCEINSTITUTIONCODE":
                case "SOURCECODE":
                    Package.Bib_Info.Source.Code = Data;
                    break;

                case "SOURCEINSTITUTIONSTATEMENT":
                case "SOURCESTATEMENT":
                case "SOURCE":
                    Package.Bib_Info.Source.Statement = Data;
                    break;

                case "SUBJECTKEYWORD":
                case "SUBJECTKEYWORDS":
                case "SUBJECT":
                case "SUBJECTS":
                    Package.Bib_Info.Add_Subject(Data, String.Empty);
                    break;

                case "SUBJECTKEYWORDAUTHORITY":
                case "SUBJECTAUTHORITY":
                    if (Package.Bib_Info.Subjects_Count > 0)
                    {
                        Package.Bib_Info.Subjects[Package.Bib_Info.Subjects_Count - 1].Authority = Data;
                    }
                    break;

                case "BIBID":
                case "BIB":
                case "BIBLIOGRAHPICID":
                case "BIBLIOGRAPHICIDENTIFIER":
                    Package.Bib_Info.BibID = Data.ToUpper();
                    break;

                case "VID":
                    Package.Bib_Info.VID = Data.PadLeft(5, '0');
                    break;

                case "DATE":
                    try
                    {
                        // first, try converting the string value to a date object
                        Package.Bib_Info.Origin_Info.Date_Issued = Convert.ToDateTime(Data).ToShortDateString();
                    }
                    catch
                    {
                        try
                        {
                            // second, try converting the string value to an integer
                            Package.Bib_Info.Origin_Info.Date_Issued = Convert.ToInt32(Data).ToString();
                        }
                        catch
                        {
                            Package.Bib_Info.Origin_Info.Date_Issued = Data;
                        }
                    }
                    break;

                case "EDITION":
                    Package.Bib_Info.Origin_Info.Edition = Data;
                    break;

                case "FORMAT":
                case "PHYSICALDESCRIPTION":
                    Package.Bib_Info.Original_Description.Extent = Data;
                    break;

                case "NOTE":
                case "NOTES":
                    Package.Bib_Info.Add_Note(Data);
                    break;

                case "RIGHTS":
                    Package.Bib_Info.Access_Condition.Text = Data;
                    break;

                case "BIBSERIESTITLE":
                case "SERIESTITLE":
                    Package.Bib_Info.SeriesTitle.Title = Data;
                    Package.Behaviors.GroupTitle = Data;
                    break;

                case "MATERIALTYPE":
                case "TYPE":
                    string upper_data = Data.ToUpper();
                    if (upper_data.IndexOf("NEWSPAPER", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Newspaper;
                        break;
                    }
                    if ((upper_data.IndexOf("MONOGRAPH", StringComparison.Ordinal) >= 0) || (upper_data.IndexOf("BOOK", StringComparison.Ordinal) >= 0))
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        break;
                    }
                    if (upper_data.IndexOf("SERIAL", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;
                        break;
                    }
                    if (upper_data.IndexOf("AERIAL", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Aerial;
                        if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                            Package.Bib_Info.Original_Description.Extent = "Aerial Photograph";
                        break;
                    }
                    if (upper_data.IndexOf("PHOTO", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        break;
                    }
                    if (upper_data.IndexOf("POSTCARD", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                            Package.Bib_Info.Original_Description.Extent = "Postcard";
                        break;
                    }
                    if (upper_data.IndexOf("MAP", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Map;
                        break;
                    }
                    if (upper_data.IndexOf("TEXT", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        break;
                    }
                    if (upper_data.IndexOf("AUDIO", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Audio;
                        break;
                    }
                    if (upper_data.IndexOf("VIDEO", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Video;
                        break;
                    }
                    if ((upper_data.IndexOf("ARCHIVE", StringComparison.Ordinal) >= 0) || (upper_data.IndexOf("ARCHIVAL", StringComparison.Ordinal) >= 0))
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                        break;
                    }
                    if (upper_data.IndexOf("ARTIFACT", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Artifact;
                        break;
                    }
                    if (upper_data.IndexOf("IMAGE", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        break;
                    }

                    // if there was no match, set type to "UNDETERMINED"
                    Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.UNKNOWN;

                    if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                        Package.Bib_Info.Original_Description.Extent = "Undetermined";
                    break;

                case "BIBUNIFORMTITLE":
                case "UNIFORMTITLE":
                    Package.Bib_Info.Add_Other_Title(Data, Title_Type_Enum.Uniform);
                    Package.Behaviors.GroupTitle = Data;
                    break;

                case "VOLUMETITLE":
                case "TITLE":
                    Package.Bib_Info.Main_Title.Title = Data;
                    break;

                case "TITLELANGUAGE":
                    Package.Bib_Info.Main_Title.Language = Data;
                    break;

                case "ALEPH":
                    Package.Bib_Info.Add_Identifier(Data, "ALEPH");
                    break;

                case "OCLC":
                    Package.Bib_Info.Add_Identifier(Data, "OCLC");
                    break;

                case "LCCN":
                    Package.Bib_Info.Add_Identifier(Data, "LCCN");
                    break;

                case "ISBN":
                    Package.Bib_Info.Add_Identifier(Data, "ISBN");
                    break;

                case "ISSN":
                    Package.Bib_Info.Add_Identifier(Data, "ISSN");
                    break;

                case "SUBTITLE":
                    Package.Bib_Info.Main_Title.Subtitle = Data;
                    break;

                case "VOLUME":
                    Package.Bib_Info.Series_Part_Info.Enum1 = Data;
                    Package.Behaviors.Serial_Info.Add_Hierarchy(Package.Behaviors.Serial_Info.Count + 1, 1, Data);
                    break;

                case "ISSUE":
                    if (Package.Bib_Info.Series_Part_Info.Enum1.Length == 0)
                    {
                        Package.Bib_Info.Series_Part_Info.Enum1 = Data;
                    }
                    else
                    {
                        Package.Bib_Info.Series_Part_Info.Enum2 = Data;
                    }
                    Package.Behaviors.Serial_Info.Add_Hierarchy(Package.Behaviors.Serial_Info.Count + 1, 1, Data);
                    break;

                case "SECTION":
                    if (Package.Bib_Info.Series_Part_Info.Enum2.Length == 0)
                    {
                        if (Package.Bib_Info.Series_Part_Info.Enum1.Length == 0)
                            Package.Bib_Info.Series_Part_Info.Enum1 = Data;
                        else
                            Package.Bib_Info.Series_Part_Info.Enum2 = Data;
                    }
                    else
                    {
                        Package.Bib_Info.Series_Part_Info.Enum3 = Data;
                    }
                    Package.Behaviors.Serial_Info.Add_Hierarchy(Package.Behaviors.Serial_Info.Count + 1, 1, Data);
                    // Do nothing for now
                    break;

                case "YEAR":
                    Package.Bib_Info.Series_Part_Info.Year = Data;

                    if (Data.Length == 1)
                        year = "0" + Data;
                    else
                        year = Data;
                    build_date_string(Package);

                    break;

                case "MONTH":
                    Package.Bib_Info.Series_Part_Info.Month = Data;
                    month = Data;
                    build_date_string(Package);

                    break;

                case "DAY":
                    Package.Bib_Info.Series_Part_Info.Day = Data;
                    day = Data;
                    build_date_string(Package);

                    break;

                case "COORDINATES":
                    GeoSpatial_Information geoInfo = Package.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                    if (geoInfo == null)
                    {
                        geoInfo = new GeoSpatial_Information();
                        Package.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo);
                    }
                    string[] coordinates = Data.Split(", ;".ToCharArray());
                    try
                    {
                        if (coordinates.Length == 2)
                        {
                            geoInfo.Add_Point(Convert.ToDouble(coordinates[0]), Convert.ToDouble(coordinates[1]), String.Empty);
                        }
                        else
                        {
                            coordinates = Data.Split(",;".ToCharArray());
                            if (coordinates.Length == 2)
                            {
                                geoInfo.Add_Point(Convert.ToDouble(coordinates[0]), Convert.ToDouble(coordinates[1]), String.Empty);
                            }
                        }
                    }
                    catch
                    {
                    }
                    break;

                case "LATITUDE":
                case "COORDINATESLATITUDE":
                    GeoSpatial_Information geoInfo2 = Package.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                    if (geoInfo2 == null)
                    {
                        geoInfo2 = new GeoSpatial_Information();
                        Package.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo2);
                    }
                    try
                    {
                        if (geoInfo2.Point_Count == 0)
                            geoInfo2.Add_Point(Convert.ToDouble(Data), 0, String.Empty);
                        else
                            geoInfo2.Points[0].Latitude = Convert.ToDouble(Data);
                    }
                    catch
                    {
                    }
                    break;

                case "LONGITUDE":
                case "COORDINATESLONGITUDE":
                    GeoSpatial_Information geoInfo3 = Package.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                    if (geoInfo3 == null)
                    {
                        geoInfo3 = new GeoSpatial_Information();
                        Package.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo3);
                    }
                    try
                    {
                        if (geoInfo3.Point_Count == 0)
                            geoInfo3.Add_Point(0, Convert.ToDouble(Data.Replace("°", "")), String.Empty);
                        else
                            geoInfo3.Points[0].Longitude = Convert.ToDouble(Data.Replace("°", ""));
                    }
                    catch
                    {
                    }
                    break;

                case "PROJECTION":
                case "MAPPROJECTION":
                    Guarantee_Cartographics(Package).Projection = Data;
                    break;

                case "SCALE":
                case "MAPSCALE":
                    Guarantee_Cartographics(Package).Scale = Data;
                    break;

                //case Mapped_Fields.Spatial_Coverage:
                //    Package.Bib_Info.Hierarchical_Spatials[0].Area = Data;
                //    break;

                case "ICON/WORDMARK":
                case "ICON/WORDMARKS":
                case "ICON":
                case "ICONS":
                case "WORDMARK":
                case "WORDMARKS":
                    Package.Behaviors.Add_Wordmark(Data);
                    break;

                case "WEBSKINS":
                case "WEBSKIN":
                case "SKINS":
                case "SKIN":
                    Package.Behaviors.Add_Web_Skin(Data);
                    break;

                case "TEMPORALCOVERAGE":
                case "TEMPORAL":
                    Package.Bib_Info.Add_Temporal_Subject(-1, -1, Data);
                    break;

                case "AFFILIATIONUNIVERSITY":
                case "UNIVERSITY":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].University = Data;
                    break;

                case "AFFILIATIONCAMPUS":
                case "CAMPUS":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Campus = Data;
                    break;

                case "AFFILIATIONCOLLEGE":
                case "COLLEGE":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].College = Data;
                    break;

                case "AFFILIATIONUNIT":
                case "UNIT":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Unit = Data;
                    break;

                case "AFFILIATIONDEPARTMENT":
                case "DEPARTMENT":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Department = Data;
                    break;

                case "AFFILIATIONINSTITUTE":
                case "INSTITUTE":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Institute = Data;
                    break;

                case "AFFILIATIONCENTER":
                case "CENTER":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Center = Data;
                    break;

                case "AFFILIATIONSECTION":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Section = Data;
                    break;

                case "AFFILIATIONSUBSECTION":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].SubSection = Data;
                    break;

                case "GEOGRAPHYCONTINENT":
                case "CONTINENT":
                    Guarantee_Hierarchical_Spatial(Package).Continent = Data;
                    break;

                case "GEOGRAPHYCOUNTRY":
                case "COUNTRY":
                    Guarantee_Hierarchical_Spatial(Package).Country = Data;
                    break;

                case "GEOGRAPHYPROVINCE":
                case "PROVINCE":
                    Guarantee_Hierarchical_Spatial(Package).Province = Data;
                    break;

                case "GEOGRAPHYREGION":
                case "REGION":
                    Guarantee_Hierarchical_Spatial(Package).Region = Data;
                    break;

                case "GEOGRAPHYSTATE":
                case "STATE":
                    Guarantee_Hierarchical_Spatial(Package).State = Data;
                    break;

                case "GEOGRAPHYTERRITORY":
                case "TERRITORY":
                    Guarantee_Hierarchical_Spatial(Package).Territory = Data;
                    break;

                case "GEOGRAPHYCOUNTY":
                case "COUNTY":
                    Guarantee_Hierarchical_Spatial(Package).County = Data;
                    break;

                case "GEOGRAPHYCITY":
                case "CITY":
                    Guarantee_Hierarchical_Spatial(Package).City = Data;
                    break;

                case "GEOGRAPHYISLAND":
                case "ISLAND":
                    Guarantee_Hierarchical_Spatial(Package).Island = Data;
                    break;

                case "GEOGRAPHYAREA":
                case "AREA":
                    Guarantee_Hierarchical_Spatial(Package).Area = Data;
                    break;

                case "COPYRIGHTDATE":
                case "COPYRIGHT":
                    Package.Bib_Info.Origin_Info.Date_Copyrighted = Data;
                    break;

                case "EADNAME":
                case "EAD":
                    Package.Bib_Info.Location.EAD_Name = Data;
                    break;

                case "EADURL":
                    Package.Bib_Info.Location.EAD_URL = Data;
                    break;

                case "COMMENTS":
                case "INTERNALCOMMENTS":
                case "INTERNAL":
                    Package.Tracking.Internal_Comments = Data;
                    break;

                case "CONTAINERBOX":
                case "BOX":
                    Package.Bib_Info.Add_Container("Box", Data, 1);
                    break;

                case "CONTAINERDIVIDER":
                case "DIVIDER":
                    Package.Bib_Info.Add_Container("Divider", Data, 2);
                    break;

                case "CONTAINERFOLDER":
                case "FOLDER":
                    Package.Bib_Info.Add_Container("Folder", Data, 3);
                    break;

                case "VIEWERS":
                case "VIEWER":
                    Package.Behaviors.Add_View(Data);
                    break;

                case "VISIBILITY":
                    switch (Data.ToUpper())
                    {
                        case "DARK":
                            Package.Behaviors.Dark_Flag = true;
                            Package.Behaviors.IP_Restriction_Membership = -1;
                            break;

                        case "PRIVATE":
                            Package.Behaviors.Dark_Flag = false;
                            Package.Behaviors.IP_Restriction_Membership = -1;
                            break;

                        case "PUBLIC":
                            Package.Behaviors.Dark_Flag = false;
                            Package.Behaviors.IP_Restriction_Membership = 0;
                            break;

                        case "RESTRICTED":
                            Package.Behaviors.Dark_Flag = false;
                            Package.Behaviors.IP_Restriction_Membership = 1;
                            break;
                    }
                    break;

                case "TICKLER":
                    Package.Behaviors.Add_Tickler(Data);
                    break;

                case "TRACKINGBOX":
                    Package.Tracking.Tracking_Box = Data;
                    break;

                case "BORNDIGITAL":
                    if (Data.ToUpper().Trim() == "TRUE")
                        Package.Tracking.Born_Digital = true;
                    break;

                case "MATERIALRECEIVED":
                case "MATERIALRECEIVEDDATE":
                case "MATERIALRECDDATE":
                case "MATERIALRECD":
                    DateTime materialReceivedDate;
                    if (DateTime.TryParse(Data, out materialReceivedDate))
                        Package.Tracking.Material_Received_Date = materialReceivedDate;
                    break;

                case "MATERIAL":
                case "MATERIALS":
                    VRACore_Info vraCoreInfo2 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo2 == null)
                    {
                        vraCoreInfo2 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo2);
                    }
                    vraCoreInfo2.Add_Material(Data, "medium");
                    break;

                case "MEASUREMENT":
                case "MEASUREMENTS":
                    VRACore_Info vraCoreInfo3 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo3 == null)
                    {
                        vraCoreInfo3 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo3);
                    }
                    vraCoreInfo3.Add_Measurement(Data, String.Empty);
                    break;

                case "STATEEDITION":
                    VRACore_Info vraCoreInfo4 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo4 == null)
                    {
                        vraCoreInfo4 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo4);
                    }
                    vraCoreInfo4.Add_State_Edition(Data);
                    break;

                case "STYLE":
                case "PERIOD":
                case "STYLEPERIOD":
                    VRACore_Info vraCoreInfo5 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo5 == null)
                    {
                        vraCoreInfo5 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo5);
                    }
                    vraCoreInfo5.Add_Style_Period(Data);
                    break;

                case "TECHNIQUE":
                    VRACore_Info vraCoreInfo6 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo6 == null)
                    {
                        vraCoreInfo6 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo6);
                    }
                    vraCoreInfo6.Add_Technique(Data);
                    break;
            }
        }


        private static void Guarantee_Affiliation_Collection(SobekCM_Item Package)
        {
            // Add an affiliation, if none exists
            if (Package.Bib_Info.Affiliations_Count == 0)
            {
                Package.Bib_Info.Add_Affiliation(new Affiliation_Info());
            }
        }

        private static Subject_Info_HierarchicalGeographic Guarantee_Hierarchical_Spatial(SobekCM_Item Package)
        {
            // Is there an existing hierarchical?
            if (Package.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info subject in Package.Bib_Info.Subjects)
                {
                    if (subject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                    {
                        return (Subject_Info_HierarchicalGeographic)subject;
                    }
                }
            }

            // Add a spatial, if none exists
            Subject_Info_HierarchicalGeographic hierarchical = new Subject_Info_HierarchicalGeographic();
            Package.Bib_Info.Add_Subject(hierarchical);
            return hierarchical;
        }

        private static Subject_Info_Cartographics Guarantee_Cartographics(SobekCM_Item Package)
        {
            // Is there an existing cartograhics?
            if (Package.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info subject in Package.Bib_Info.Subjects)
                {
                    if (subject.Class_Type == Subject_Info_Type.Cartographics)
                    {
                        return (Subject_Info_Cartographics)subject;
                    }
                }
            }

            // Add a spatial, if none exists
            Subject_Info_Cartographics cartograhics = new Subject_Info_Cartographics();
            Package.Bib_Info.Add_Subject(cartograhics);
            return cartograhics;


        }
    }
}
