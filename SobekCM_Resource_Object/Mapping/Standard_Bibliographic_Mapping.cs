using System;
using System.Collections.Generic;
using System.Data;
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
        /// <returns> TRUE if the field was mapped, FALSE if there was data and no mapping was found </returns>
        public bool Add_Data(SobekCM_Item Package, string Data, string Field)
        {
            // If no field listed, just skip (but not a mapping error, so return TRUE)
            if (String.IsNullOrEmpty(Field))
                return true;

            // If no data listed, just skip (but not a mapping error, so return TRUE)
            if (String.IsNullOrWhiteSpace(Data))
                return true;

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
                    return true;

                case "ABSTRACT":
                    Package.Bib_Info.Add_Abstract(Data, "en");
                    return true;

                case "ACCESSION":
                case "ACCESSIONNUMBER":
                    Package.Bib_Info.Add_Identifier(Data, "Accession Number");
                    return true;

                case "ALTERNATETITLE":
                case "ALTTITLE":
                case "TITLEVARIANT":
                    Package.Bib_Info.Add_Other_Title(Data, Title_Type_Enum.Alternative);
                    return true;

                case "ALTERNATETITLELANGUAGE":
                case "ALTTITLELANGUAGE":
                    List<Title_Info> otherTitles = Package.Bib_Info.Other_Titles.Where(ThisTitle => ThisTitle.Title_Type == Title_Type_Enum.Alternative).ToList();
                    if (otherTitles.Count > 0)
                    {
                        otherTitles[otherTitles.Count - 1].Language = Data;
                    }
                    return true;

                case "TITLETRANSLATION":
                case "TRANSLATEDTITLE":
                    Package.Bib_Info.Add_Other_Title(Data, Title_Type_Enum.Translated);
                    return true;

                case "ATTRIBUTION":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.Funding);
                    return true;

                case "COLLECTION":
                case "COLLECTIONCODE":
                case "AGGREGATION":
                case "AGGREGATIONCODE":
                case "SUBCOLLECTION":
                case "SUBCOLLECTIONS":
                    Package.Behaviors.Add_Aggregation(Data.ToUpper());
                    return true;

                case "CLASSIFICATION":
                    Package.Bib_Info.Add_Classification(Data);
                    return true;

                case "CLASSIFICATIONTYPE":
                case "CLASSIFICATIONAUTHORITY":
                    if (Package.Bib_Info.Classifications_Count > 0)
                    {
                        Package.Bib_Info.Classifications[Package.Bib_Info.Classifications_Count - 1].Authority = Data;
                    }
                    return true;

                case "CONTRIBUTOR":
                case "CONTRIBUTORS":
                    Package.Bib_Info.Add_Named_Entity(new Name_Info(Data, "contributor"));
                    return true;

                case "CREATOR":
                case "CREATORS":
                case "AUTHOR":
                    Package.Bib_Info.Add_Named_Entity(new Name_Info(Data, "creator"));
                    return true;

                case "CREATORPERSONALNAME":
                    Name_Info personalCreator = new Name_Info(Data, "creator");
                    personalCreator.Name_Type = Name_Info_Type_Enum.Personal;
                    Package.Bib_Info.Add_Named_Entity(personalCreator);
                    return true;

                case "CREATORCORPORATENAME":
                    Name_Info corporateCreator = new Name_Info(Data, "creator");
                    corporateCreator.Name_Type = Name_Info_Type_Enum.Corporate;
                    Package.Bib_Info.Add_Named_Entity(corporateCreator);
                    return true;

                case "CREATORAFFILIATION":
                case "AUTHORAFFILIATION":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1].Affiliation = Data;
                    }
                    return true;

                case "CREATORDATES":
                case "AUTHORDATES":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1].Dates = Data;
                    }
                    return true;

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
                    return true;

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
                    return true;

                case "CREATORROLE":
                case "AUTHORROLE":
                case "CREATORROLES":
                case "AUTHORROLES":
                case "CREATORATTRIBUTION":
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Name_Info thisCreator = Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1];
                        if ((thisCreator.Roles.Count == 1) && ((thisCreator.Roles[0].Role == "creator") || (thisCreator.Roles[1].Role == "contributor")))
                            thisCreator.Roles.Clear();
                        Package.Bib_Info.Names[Package.Bib_Info.Names_Count - 1].Add_Role(Data);
                    }
                    return true;

                case "CULTURALCONTEXT":
                case "CULTURE":
                    VRACore_Info vraCoreInfo = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo == null)
                    {
                        vraCoreInfo = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo);
                    }
                    vraCoreInfo.Add_Cultural_Context(Data);
                    return true;

                case "DONOR":
                    Package.Bib_Info.Donor.Full_Name = Data;
                    return true;

                case "GENRE":
                    Package.Bib_Info.Add_Genre(Data);
                    return true;

                case "GENREAUTHORITY":
                    if (Package.Bib_Info.Genres_Count > 0)
                    {
                        Package.Bib_Info.Genres[Package.Bib_Info.Genres_Count - 1].Authority = Data;
                    }
                    return true;

                case "HOLDINGLOCATIONCODE":
                case "HOLDINGCODE":
                    Package.Bib_Info.Location.Holding_Code = Data;
                    return true;

                case "HOLDINGLOCATIONSTATEMENT":
                case "HOLDINGSTATEMENT":
                case "CONTRIBUTINGINSTITUTION":
                case "LOCATIONCURRENT":
                case "LOCATIONCURRENTSITE":
                case "LOCATIONCURRENTREPOSITORY":
                    Package.Bib_Info.Location.Holding_Name = Data;
                    return true;

                case "LOCATIONFORMERSITE":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.OriginalLocation);
                    return true;

                case "IDENTIFIER":
                case "IDNUMBERFORMERREPOSITORY":
                case "IDNUMBERCURRENTREPOSITORY":
                case "IDNUMBERCURRENTRESPOSITORY":
                    Package.Bib_Info.Add_Identifier(Data);
                    return true;

                case "IDENTIFIERTYPE":
                    if (Package.Bib_Info.Identifiers_Count > 0)
                    {
                        Package.Bib_Info.Identifiers[Package.Bib_Info.Identifiers_Count - 1].Type = Data;
                    }
                    return true;

                case "INSCRIPTION":
                    VRACore_Info vraCoreInfo8 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo8 == null)
                    {
                        vraCoreInfo8 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo8);
                    }
                    vraCoreInfo8.Add_Inscription(Data);
                    return true;

                case "LANGUAGE":
                    Package.Bib_Info.Add_Language(Data);
                    return true;

                case "PUBLISHER":
                case "PUBLISHERS":
                    Package.Bib_Info.Add_Publisher(Data);
                    return true;

                case "PLACEOFPUBLICATION":
                case "PUBLICATIONPLACE":
                case "PUBPLACE":
                case "PUBLICATIONLOCATION":
                case "PLACE":
                    Package.Bib_Info.Origin_Info.Add_Place(Data);
                    return true;

                case "RELATEDURLLABEL":
                    Package.Bib_Info.Location.Other_URL_Display_Label = Data;
                    return true;

                case "RELATEDURL":
                case "RELATEDURLLINK":
                    Package.Bib_Info.Location.Other_URL = Data;
                    return true;

                case "RELATEDURLNOTE":
                case "RELATEDURLNOTES":
                    Package.Bib_Info.Location.Other_URL_Note = Data;
                    return true;

                case "SOURCEINSTITUTIONCODE":
                case "SOURCECODE":
                    Package.Bib_Info.Source.Code = Data;
                    return true;

                case "SOURCEINSTITUTIONSTATEMENT":
                case "SOURCESTATEMENT":
                case "SOURCE":
                    Package.Bib_Info.Source.Statement = Data;
                    return true;

                case "SUBJECTKEYWORD":
                case "SUBJECTKEYWORDS":
                case "SUBJECT":
                case "SUBJECTS":
                case "KEYWORDS":
                    Package.Bib_Info.Add_Subject(Data, String.Empty);
                    return true;

                case "SUBJECTKEYWORDAUTHORITY":
                case "SUBJECTAUTHORITY":
                    if (Package.Bib_Info.Subjects_Count > 0)
                    {
                        Package.Bib_Info.Subjects[Package.Bib_Info.Subjects_Count - 1].Authority = Data;
                    }
                    return true;

                case "BIBID":
                case "BIB":
                case "BIBLIOGRAHPICID":
                case "BIBLIOGRAPHICIDENTIFIER":
                    Package.Bib_Info.BibID = Data.ToUpper();
                    return true;

                case "VID":
                    Package.Bib_Info.VID = Data.PadLeft(5, '0');
                    return true;

                case "DATE":
                case "DATECREATION":
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
                    return true;

                case "DATEBEGINNING":
                    Package.Bib_Info.Origin_Info.MARC_DateIssued_Start = Data;
                    return true;

                case "DATECOMPLETION":
                    Package.Bib_Info.Origin_Info.MARC_DateIssued_End = Data;
                    return true;

                case "EDITION":
                    Package.Bib_Info.Origin_Info.Edition = Data;
                    return true;

                case "FORMAT":
                case "PHYSICALDESCRIPTION":
                    Package.Bib_Info.Original_Description.Extent = Data;
                    return true;

                case "NOTE":
                case "NOTES":
                case "DESCRIPTION":
                    Package.Bib_Info.Add_Note(Data);
                    return true;

                case "PROVENANCE":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.Acquisition);
                    return true;

                case "USAGESTATEMENT":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.CitationReference);
                    return true;

                case "CONTACT":
                case "CONTACTNOTES":
                case "CONTACTINFORMATION":
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.NONE, "Contact");
                    return true;

                case "RIGHTS":
                    Package.Bib_Info.Access_Condition.Text = Data;
                    return true;

                case "BIBSERIESTITLE":
                case "SERIESTITLE":
                case "TITLESERIES":
                    Package.Bib_Info.SeriesTitle.Title = Data;
                    Package.Behaviors.GroupTitle = Data;
                    return true;

                case "MATERIALTYPE":
                case "TYPE":
                case "RECORDTYPE":
                    string upper_data = Data.ToUpper();
                    if (upper_data.IndexOf("NEWSPAPER", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Newspaper;
                        return true;
                    }
                    if ((upper_data.IndexOf("MONOGRAPH", StringComparison.Ordinal) >= 0) || (upper_data.IndexOf("BOOK", StringComparison.Ordinal) >= 0))
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        return true;
                    }
                    if (upper_data.IndexOf("SERIAL", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;
                        return true;
                    }
                    if (upper_data.IndexOf("AERIAL", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Aerial;
                        if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                            Package.Bib_Info.Original_Description.Extent = "Aerial Photograph";
                        return true;
                    }
                    if (upper_data.IndexOf("PHOTO", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        return true;
                    }
                    if (upper_data.IndexOf("POSTCARD", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                            Package.Bib_Info.Original_Description.Extent = "Postcard";
                        return true;
                    }
                    if (upper_data.IndexOf("MAP", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Map;
                        return true;
                    }
                    if (upper_data.IndexOf("TEXT", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        return true;
                    }
                    if (upper_data.IndexOf("AUDIO", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Audio;
                        return true;
                    }
                    if (upper_data.IndexOf("VIDEO", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Video;
                        return true;
                    }
                    if ((upper_data.IndexOf("ARCHIVE", StringComparison.Ordinal) >= 0) || (upper_data.IndexOf("ARCHIVAL", StringComparison.Ordinal) >= 0))
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                        return true;
                    }
                    if (upper_data.IndexOf("ARTIFACT", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Artifact;
                        return true;
                    }
                    if (upper_data.IndexOf("IMAGE", StringComparison.Ordinal) >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        return true;
                    }

                    // if there was no match, set type to "UNDETERMINED"
                    Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.UNKNOWN;

                    if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                        Package.Bib_Info.Original_Description.Extent = "Undetermined";
                    return true;

                case "BIBUNIFORMTITLE":
                case "UNIFORMTITLE":
                    Package.Bib_Info.Add_Other_Title(Data, Title_Type_Enum.Uniform);
                    Package.Behaviors.GroupTitle = Data;
                    return true;

                case "VOLUMETITLE":
                case "TITLE":
                    Package.Bib_Info.Main_Title.Title = Data;
                    return true;

                case "TITLELANGUAGE":
                    Package.Bib_Info.Main_Title.Language = Data;
                    return true;

                case "ALEPH":
                    Package.Bib_Info.Add_Identifier(Data, "ALEPH");
                    return true;

                case "OCLC":
                    Package.Bib_Info.Add_Identifier(Data, "OCLC");
                    return true;

                case "LCCN":
                    Package.Bib_Info.Add_Identifier(Data, "LCCN");
                    return true;

                case "ISBN":
                    Package.Bib_Info.Add_Identifier(Data, "ISBN");
                    return true;

                case "ISSN":
                    Package.Bib_Info.Add_Identifier(Data, "ISSN");
                    return true;

                case "SUBTITLE":
                    Package.Bib_Info.Main_Title.Subtitle = Data;
                    return true;

                case "VOLUME":
                    Package.Bib_Info.Series_Part_Info.Enum1 = Data;
                    Package.Behaviors.Serial_Info.Add_Hierarchy(Package.Behaviors.Serial_Info.Count + 1, 1, Data);
                    return true;

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
                    return true;

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
                    return true;

                case "YEAR":
                    Package.Bib_Info.Series_Part_Info.Year = Data;

                    if (Data.Length == 1)
                        year = "0" + Data;
                    else
                        year = Data;
                    build_date_string(Package);

                    return true;

                case "MONTH":
                    Package.Bib_Info.Series_Part_Info.Month = Data;
                    month = Data;
                    build_date_string(Package);

                    return true;

                case "DAY":
                    Package.Bib_Info.Series_Part_Info.Day = Data;
                    day = Data;
                    build_date_string(Package);

                    return true;

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
                    return true;

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
                    return true;

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
                    return true;

                case "PROJECTION":
                case "MAPPROJECTION":
                    Guarantee_Cartographics(Package).Projection = Data;
                    return true;

                case "SCALE":
                case "MAPSCALE":
                    Guarantee_Cartographics(Package).Scale = Data;
                    return true;

                //case Mapped_Fields.Spatial_Coverage:
                //    Package.Bib_Info.Hierarchical_Spatials[0].Area = Data;
                //    return true;

                case "ICON/WORDMARK":
                case "ICON/WORDMARKS":
                case "ICON":
                case "ICONS":
                case "WORDMARK":
                case "WORDMARKS":
                    Package.Behaviors.Add_Wordmark(Data);
                    return true;

                case "WEBSKINS":
                case "WEBSKIN":
                case "SKINS":
                case "SKIN":
                    Package.Behaviors.Add_Web_Skin(Data);
                    return true;

                case "TEMPORALCOVERAGE":
                case "TEMPORAL":
                case "TIMEPERIOD":
                    Package.Bib_Info.Add_Temporal_Subject(-1, -1, Data);
                    return true;

                case "COVERAGE":
                    // Was this a number.. likely, a year?
                    int possible_year;
                    if (( Data.Length >= 4 ) && ( Int32.TryParse(Data.Substring(0,4), out possible_year)))
                        Package.Bib_Info.Add_Temporal_Subject(-1, -1, Data);
                    else
                        Package.Bib_Info.Add_Spatial_Subject(Data);
                    return true;

                case "AFFILIATIONUNIVERSITY":
                case "UNIVERSITY":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].University = Data;
                    return true;

                case "AFFILIATIONCAMPUS":
                case "CAMPUS":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Campus = Data;
                    return true;

                case "AFFILIATIONCOLLEGE":
                case "COLLEGE":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].College = Data;
                    return true;

                case "AFFILIATIONUNIT":
                case "UNIT":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Unit = Data;
                    return true;

                case "AFFILIATIONDEPARTMENT":
                case "DEPARTMENT":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Department = Data;
                    return true;

                case "AFFILIATIONINSTITUTE":
                case "INSTITUTE":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Institute = Data;
                    return true;

                case "AFFILIATIONCENTER":
                case "CENTER":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Center = Data;
                    return true;

                case "AFFILIATIONSECTION":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].Section = Data;
                    return true;

                case "AFFILIATIONSUBSECTION":
                    Guarantee_Affiliation_Collection(Package);
                    Package.Bib_Info.Affiliations[0].SubSection = Data;
                    return true;

                case "GEOGRAPHYCONTINENT":
                case "CONTINENT":
                    Guarantee_Hierarchical_Spatial(Package).Continent = Data;
                    return true;

                case "GEOGRAPHYCOUNTRY":
                case "COUNTRY":
                    Guarantee_Hierarchical_Spatial(Package).Country = Data;
                    return true;

                case "GEOGRAPHYPROVINCE":
                case "PROVINCE":
                    Guarantee_Hierarchical_Spatial(Package).Province = Data;
                    return true;

                case "GEOGRAPHYREGION":
                case "REGION":
                    Guarantee_Hierarchical_Spatial(Package).Region = Data;
                    return true;

                case "GEOGRAPHYSTATE":
                case "STATE":
                    Guarantee_Hierarchical_Spatial(Package).State = Data;
                    return true;

                case "GEOGRAPHYTERRITORY":
                case "TERRITORY":
                    Guarantee_Hierarchical_Spatial(Package).Territory = Data;
                    return true;

                case "GEOGRAPHYCOUNTY":
                case "COUNTY":
                    Guarantee_Hierarchical_Spatial(Package).County = Data;
                    return true;

                case "GEOGRAPHYCITY":
                case "CITY":
                    Guarantee_Hierarchical_Spatial(Package).City = Data;
                    return true;

                case "GEOGRAPHYISLAND":
                case "ISLAND":
                    Guarantee_Hierarchical_Spatial(Package).Island = Data;
                    return true;

                case "GEOGRAPHYAREA":
                case "AREA":
                    Guarantee_Hierarchical_Spatial(Package).Area = Data;
                    return true;

                case "LOCATION":
                    Package.Bib_Info.Add_Spatial_Subject(Data);
                    return true;

                case "COPYRIGHTDATE":
                case "COPYRIGHT":
                    Package.Bib_Info.Origin_Info.Date_Copyrighted = Data;
                    return true;

                case "EADNAME":
                case "EAD":
                    Package.Bib_Info.Location.EAD_Name = Data;
                    return true;

                case "EADURL":
                    Package.Bib_Info.Location.EAD_URL = Data;
                    return true;

                case "COMMENTS":
                case "INTERNALCOMMENTS":
                case "INTERNAL":
                    Package.Tracking.Internal_Comments = Data;
                    return true;

                case "CONTAINERBOX":
                case "BOX":
                    Package.Bib_Info.Add_Container("Box", Data, 1);
                    return true;

                case "CONTAINERDIVIDER":
                case "DIVIDER":
                    Package.Bib_Info.Add_Container("Divider", Data, 2);
                    return true;

                case "CONTAINERFOLDER":
                case "FOLDER":
                    Package.Bib_Info.Add_Container("Folder", Data, 3);
                    return true;

                case "VIEWERS":
                case "VIEWER":
                    Package.Behaviors.Add_View(Data);
                    return true;

                case "VISIBILITY":
                    switch (Data.ToUpper())
                    {
                        case "DARK":
                            Package.Behaviors.Dark_Flag = true;
                            Package.Behaviors.IP_Restriction_Membership = -1;
                            return true;

                        case "PRIVATE":
                            Package.Behaviors.Dark_Flag = false;
                            Package.Behaviors.IP_Restriction_Membership = -1;
                            return true;

                        case "PUBLIC":
                            Package.Behaviors.Dark_Flag = false;
                            Package.Behaviors.IP_Restriction_Membership = 0;
                            return true;

                        case "RESTRICTED":
                            Package.Behaviors.Dark_Flag = false;
                            Package.Behaviors.IP_Restriction_Membership = 1;
                            return true;
                    }
                    return true;

                case "TICKLER":
                    Package.Behaviors.Add_Tickler(Data);
                    return true;

                case "TRACKINGBOX":
                    Package.Tracking.Tracking_Box = Data;
                    return true;

                case "BORNDIGITAL":
                    if (Data.ToUpper().Trim() == "TRUE")
                        Package.Tracking.Born_Digital = true;
                    return true;

                case "MATERIALRECEIVED":
                case "MATERIALRECEIVEDDATE":
                case "MATERIALRECDDATE":
                case "MATERIALRECD":
                    DateTime materialReceivedDate;
                    if (DateTime.TryParse(Data, out materialReceivedDate))
                        Package.Tracking.Material_Received_Date = materialReceivedDate;
                    return true;

                case "MATERIAL":
                case "MATERIALS":
                case "MATERIALMEDIUM":
                    VRACore_Info vraCoreInfo2 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo2 == null)
                    {
                        vraCoreInfo2 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo2);
                    }
                    vraCoreInfo2.Add_Material(Data, "medium");
                    return true;

                case "MATERIALSUPPORT":
                    VRACore_Info vraCoreInfo2b = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo2b == null)
                    {
                        vraCoreInfo2b = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo2b);
                    }
                    vraCoreInfo2b.Add_Material(Data, "support");
                    return true;

                case "MEASUREMENT":
                case "MEASUREMENTS":
                case "MEASUREMENTDIMENSIONS":
                case "MEASUREMENTSDIMENSIONS":
                case "DIMENSIONS":
                    VRACore_Info vraCoreInfo3 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo3 == null)
                    {
                        vraCoreInfo3 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo3);
                    }
                    if (vraCoreInfo3.Measurement_Count == 0)
                        vraCoreInfo3.Add_Measurement(Data, String.Empty);
                    else
                        vraCoreInfo3.Measurements[0].Measurements = Data;
                    return true;

                case "MEASUREMENTFORMAT":
                case "MEASUREMENTUNITS":
                case "MEASUREMENTSFORMAT":
                case "MEASUREMENTSUNITS":
                    VRACore_Info vraCoreInfo3b = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo3b == null)
                    {
                        vraCoreInfo3b = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo3b);
                    }
                    if (vraCoreInfo3b.Measurement_Count == 0)
                        vraCoreInfo3b.Add_Measurement(String.Empty, Data);
                    else
                        vraCoreInfo3b.Measurements[0].Units = Data;
                    return true;



                case "STATEEDITION":
                    VRACore_Info vraCoreInfo4 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo4 == null)
                    {
                        vraCoreInfo4 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo4);
                    }
                    vraCoreInfo4.Add_State_Edition(Data);
                    return true;

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
                    return true;

                case "TECHNIQUE":
                    VRACore_Info vraCoreInfo6 = Package.Get_Metadata_Module("VRACore") as VRACore_Info;
                    if (vraCoreInfo6 == null)
                    {
                        vraCoreInfo6 = new VRACore_Info();
                        Package.Add_Metadata_Module("VRACore", vraCoreInfo6);
                    }
                    vraCoreInfo6.Add_Technique(Data);
                    return true;

                case "TARGETAUDIENCE":
                case "AUDIENCE":
                    Package.Bib_Info.Add_Target_Audience(Data);
                    return true;

                default:
                    // No mapping exists and this is a non-known no-mapping
                    return false;
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
