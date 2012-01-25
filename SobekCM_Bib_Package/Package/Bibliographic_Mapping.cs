using System;
using System.Text.RegularExpressions;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;

namespace SobekCM.Bib_Package
{
	/// <summary> Enumeration is used to map to certain values in the <see cref="SobekCM_Item"/>. </summary>
	public enum Mapped_Fields
	{
		/// <summary> No mapping is present </summary>
		None = -1,

		/// <summary> Abstract </summary>
		Abstract = 1,

        /// <summary> Accession Number </summary>
        Accession_Number,

        /// <summary> Affiliation.University </summary>
        Affiliation_University,

        /// <summary> Affiliation.Campus </summary>
        Affiliation_Campus,

        /// <summary> Affiliation.College </summary>
        Affiliation_College,

        /// <summary> Affiliation.Unit </summary>
        Affiliation_Unit,

        /// <summary> Affiliation.Department </summary>
        Affiliation_Department,

        /// <summary> Affiliation.Institute </summary>
        Affiliation_Institute,

        /// <summary> Affiliation.Center </summary>
        Affiliation_Center,

        /// <summary> Affiliation.Section </summary>
        Affiliation_Section,

        /// <summary> Affiliation.Subsection </summary>
        Affiliation_Subsection,

        /// <summary> SobekCM aggregation code </summary>
        Aggregation_Code,

        /// <summary> Aleph Record Number ( Local catalog number ) </summary>
        Aleph,

		/// <summary> Alternate Title </summary>
		Alternate_Title,

		/// <summary> Attribution Statement </summary>
		Attribution,

        /// <summary> Bibliographic identifier (BibID) </summary>
        BibID,

        /// <summary> Born digital flag </summary>
        Born_Digital_Flag,

        /// <summary> Box container </summary>
        Container_Box,

        /// <summary> Folder container </summary>
        Container_Folder,

        /// <summary> Divider container </summary>
        Container_Divider,

		/// <summary> Contributor name </summary>
		Contributor,

        /// <summary> Coordinates </summary>
        Coordinates,

        /// <summary> Copyright Date </summary>
        Copyright_Date,

		/// <summary> Creator name </summary>
		Creator,

        /// <summary> Creator affiliation </summary>
        Creator_Affiliation,

        /// <summary> Cultural context </summary>
        Cultural_Context,

        /// <summary> Date, creation or publication date </summary>
        Date,

        /// <summary> Date Field: Day </summary>
        Day,

		/// <summary> Donor </summary>
		Donor,

        /// <summary> Name of the EAD </summary>
        EAD_Name,

        /// <summary> URL of the EAD </summary>
        EAD_URL,

        /// <summary> Edition </summary>
        Edition,

        /// <summary> Format </summary>
        Format,

		/// <summary> Genre </summary>
		Genre,

        /// <summary> Geography.Continent </summary>
        Geography_Continent,

        /// <summary> Geography.Country </summary>
        Geography_Country,

        /// <summary> Geography.Province </summary>
        Geography_Province,

        /// <summary> Geography.Region </summary>
        Geography_Region,

        /// <summary> Geography.State </summary>
        Geography_State,

        /// <summary> Geography.Territory </summary>
        Geography_Territory,

        /// <summary> Geography.County </summary>
        Geography_County,

        /// <summary> Geography.City </summary>
        Geography_City,

        /// <summary> Geography.Island </summary>
        Geography_Island,

        /// <summary> Geography.Area </summary>
        Geography_Area,

		/// <summary> Holding Location Code </summary>
		Holding_Code,

		/// <summary> Holding Location Statement </summary>
		Holding_Statement,

        /// <summary> Icon/Wordmarks </summary>
        Icon_Wordmarks,

		/// <summary> Identifier </summary>
		Identifier,

        /// <summary> Inscription </summary>
        Inscription,

        /// <summary> Internal Comments </summary>
        Internal_Comments,

        /// <summary> ISBN Record Number </summary>
        ISBN,

        /// <summary> ISSN Record Number </summary>
        ISSN,

        /// <summary> Issue Number </summary>
        Issue,

		/// <summary> Language of the resource </summary>
		Language,

        /// <summary> Latitude </summary>
        Latitude,

        /// <summary> LCCN Record Number </summary>
        LCCN,

        /// <summary> Longitude </summary>
        Longitude,

        /// <summary> Materials </summary>
        Materials,

        /// <summary> Material Received Date </summary>
        Material_Received_Date,

        /// <summary> Measurements </summary>
        Measurements,

        /// <summary> Date Field: Month </summary>
        Month,

        /// <summary> Notes </summary>
        Note,

        /// <summary> OCLC Record Number </summary>
        OCLC,

        /// <summary> Projection </summary>
        Projection,

		/// <summary> Publisher </summary>
		Publisher,

        /// <summary> Place of Publication </summary>
        Pub_Place,

        /// <summary> Rights </summary>
        Rights,

        /// <summary> Scale </summary>
        Scale,

        /// <summary> Section Number </summary>
        Section,

        /// <summary> Series Title </summary>
        Series_Title,

		/// <summary> Source institution code </summary>
		Source_Code,

		/// <summary> Source institution statement </summary>
		Source_Statement,

        /// <summary> State/edition</summary>
        State_Edition,

        /// <summary> Style/Period</summary>
        Style_Period,

		/// <summary> Subject keyword </summary>
		Subject_Keyword,

        /// <summary> Sub Title </summary>
        Sub_Title,

        /// <summary> Technique </summary>
        Technique,

        /// <summary> Temporal Coverage </summary>
        Temporal_Coverage,

        /// <summary> Tickler field  </summary>
        Tickler,

        /// <summary> Title </summary>
        Title,

        /// <summary> Tracking Box </summary>
        Tracking_Box,

        /// <summary> Resource Type </summary>
        Type,

        /// <summary> Uniform Title </summary>
        Uniform_Title,

		/// <summary> Volume identifier (VID) </summary>
		VID,

        /// <summary> Visibility </summary>
        Visibility,

        /// <summary> Volume Number </summary>
        Volume,

        /// <summary> Icon / Wordmark </summary>
        Wordmark,

        /// <summary> Date Field: Year </summary>
        Year
	}

	/// <summary> This object allows data to be dynamically mapped to the correct fields
	/// in a Bibliographic Package. </summary>
	/// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class Bibliographic_Mapping
	{
        // declare static class variables
        private static string month;
        private static string day;
        private static string year;
        private static string date_string;


		/// <summary> Constructor for a new instance of the Bibliographic_Mapping class </summary>
		public Bibliographic_Mapping()
		{
			// Empty constructor
		}
       
        /// <summary> Clears all of the static class variables used to retain state between different mappings for 
        /// complex values which are passed in one at a time.</summary>
        public static void clear_static_variables()
        {
            month = String.Empty;
            day = String.Empty;
            year = String.Empty;
            date_string = String.Empty;
        }

        /// <summary> Builds a complete date string based on each of the individual components passed in through 
        /// the mappings ( day, month, year ) </summary>
        /// <param name="Package"> Item to add the built date string to </param>
        public static void build_date_string(SobekCM_Item Package)
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

        /// <summary> Converts a string to a mapped field enumeration value </summary>
        /// <param name="Field_As_String"> String version of the mapped field </param>
        /// <returns> Enumeration value </returns>
        public static Mapped_Fields String_To_Mapped_Field(string Field_As_String)
        {
            string correctName = Field_As_String.ToUpper().Replace("#", "").Replace(" ","").Replace(".","").Replace(":","").Replace("\\","").Replace("/","").Trim();
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

            switch (correctName)
            {
                case "ABSTRACT":
                    return Mapped_Fields.Abstract;

                case "ACCESSION":
                case "ACCESSIONNUMBER":
                    return Mapped_Fields.Accession_Number;

                case "AFFILIATIONUNIVERSITY":
                case "UNIVERSITY":
                    return Mapped_Fields.Affiliation_University;

                case "AFFILIATIONCAMPUS":
                case "CAMPUS":
                    return Mapped_Fields.Affiliation_Campus;

                case "AFFILIATIONCOLLEGE":
                case "COLLEGE":
                    return Mapped_Fields.Affiliation_College;

                case "AFFILIATIONUNIT":
                case "UNIT":
                    return Mapped_Fields.Affiliation_Unit;

                case "AFFILIATIONDEPARTMENT":
                case "DEPARTMENT":
                    return Mapped_Fields.Affiliation_Department;

                case "AFFILIATIONINSTITUTE":
                case "INSTITUTE":
                    return Mapped_Fields.Affiliation_Institute;

                case "AFFILIATIONCENTER":
                case "CENTER":
                    return Mapped_Fields.Affiliation_Center;

                case "AFFILIATIONSECTION":
                    return Mapped_Fields.Affiliation_Section;

                case "AFFILIATIONSUBSECTION":
                    return Mapped_Fields.Affiliation_Subsection;

                case "COLLECTION":
                case "COLLECTIONCODE":
                case "AGGREGATION":
                case "AGGREGATIONCODE":
                case "SUBCOLLECTION":
                case "SUBCOLLECTIONS":
                    return Mapped_Fields.Aggregation_Code;
                    
                case "ALEPH":
                    return Mapped_Fields.Aleph;

                case "ALTERNATETITLE":
                case "ALTTITLE":
                    return Mapped_Fields.Alternate_Title;

                case "ATTRIBUTION":
                    return Mapped_Fields.Attribution;

                case "BIBID":
                case "BIB":
                case "BIBLIOGRAHPICID":
                case "BIBLIOGRAPHICIDENTIFIER":
                    return Mapped_Fields.BibID;

                case "BORNDIGITAL":
                    return Mapped_Fields.Born_Digital_Flag;

                case "CONTAINERBOX":
                case "BOX":
                    return Mapped_Fields.Container_Box;

                case "CONTAINERFOLDER":
                case "FOLDER":
                    return Mapped_Fields.Container_Folder;

                case "CONTAINERDIVIDER":
                case "DIVIDER":
                    return Mapped_Fields.Container_Divider;

                case "CONTRIBUTOR":
                case "CONTRIBUTORS":
                    return Mapped_Fields.Contributor;

                case "COORDINATES":
                    return Mapped_Fields.Coordinates;

                case "COPYRIGHTDATE":
                case "COPYRIGHT":
                    return Mapped_Fields.Copyright_Date;

                case "CREATOR":
                case "CREATORS":
                case "AUTHOR":
                    return Mapped_Fields.Creator;

                case "CREATORAFFILIATION":
                case "AUTHORAFFILIATION":
                    return Mapped_Fields.Creator_Affiliation;

                case "CULTURALCONTEXT":
                    return Mapped_Fields.Cultural_Context;

                case "DATE":
                    return Mapped_Fields.Date;

                case "DAY":
                    return Mapped_Fields.Day;

                case "DONOR":
                    return Mapped_Fields.Donor;

                case "EADNAME":
                case "EAD":
                    return Mapped_Fields.EAD_Name;

                case "EADURL":
                    return Mapped_Fields.EAD_URL;

                case "EDITION":
                    return Mapped_Fields.Edition;

                case "FORMAT":
                case "PHYSICALDESCRIPTION":
                    return Mapped_Fields.Format;

                case "GENRE":
                    return Mapped_Fields.Genre;

                case "GEOGRAPHYCONTINENT":
                case "CONTINENT":
                    return Mapped_Fields.Geography_Continent;

                case "GEOGRAPHYCOUNTRY":
                case "COUNTRY":
                    return Mapped_Fields.Geography_Country;

                case "GEOGRAPHYPROVINCE":
                case "PROVINCE":
                    return Mapped_Fields.Geography_Province;

                case "GEOGRAPHYREGION":
                case "REGION":
                    return Mapped_Fields.Geography_Region;

                case "GEOGRAPHYSTATE":
                case "STATE":
                    return Mapped_Fields.Geography_State;

                case "GEOGRAPHYTERRITORY":
                case "TERRITORY":
                    return Mapped_Fields.Geography_Territory;

                case "GEOGRAPHYCOUNTY":
                case "COUNTY":
                    return Mapped_Fields.Geography_County;

                case "GEOGRAPHYCITY":
                case "CITY":
                    return Mapped_Fields.Geography_City;

                case "GEOGRAPHYISLAND":
                case "ISLAND":
                    return Mapped_Fields.Geography_Island;

                case "GEOGRAPHYAREA":
                case "AREA":
                    return Mapped_Fields.Geography_Area;

                case "HOLDINGLOCATIONCODE":
                case "HOLDINGCODE":
                    return Mapped_Fields.Holding_Code;

                case "HOLDINGLOCATIONSTATEMENT":
                case "HOLDINGSTATEMENT":
                    return Mapped_Fields.Holding_Statement;

                case "ICON/WORDMARK":
                case "ICON/WORDMARKS":
                case "ICON":
                case "ICONS":
                case "WORDMARK":
                case "WORDMARKS":
                    return Mapped_Fields.Icon_Wordmarks;

                case "IDENTIFIER":
                    return Mapped_Fields.Identifier;

                case "INSCRIPTION":
                    return Mapped_Fields.Inscription;

                case "COMMENTS":
                case "INTERNALCOMMENTS":
                case "INTERNAL":
                    return Mapped_Fields.Internal_Comments;

                case "ISBN":
                    return Mapped_Fields.ISBN;

                case "ISSN":
                    return Mapped_Fields.ISSN;

                case "ISSUE":
                    return Mapped_Fields.Issue;

                case "LANGUAGE":
                    return Mapped_Fields.Language;

                case "LATITUDE":
                    return Mapped_Fields.Latitude;

                case "LCCN":
                    return Mapped_Fields.LCCN;

                case "LONGITUDE":
                    return Mapped_Fields.Longitude;

                case "MATERIAL":
                case "MATERIALS":
                    return Mapped_Fields.Materials;

                case "MATERIALRECEIVED":
                case "MATERIALRECEIVEDDATE":
                case "MATERIALRECDDATE":
                case "MATERIALRECD":
                    return Mapped_Fields.Material_Received_Date;

                case "MEASUREMENT":
                case "MEASUREMENTS":
                    return Mapped_Fields.Measurements;

                case "MONTH":
                    return Mapped_Fields.Month;

                case "NOTE":
                case "NOTES":
                    return Mapped_Fields.Note;

                case "OCLC":
                    return Mapped_Fields.OCLC;

                case "PROJECTION":
                case "MAPPROJECTION":                    
                    return Mapped_Fields.Projection;

                case "PUBLISHER":
                case "PUBLISHERS":
                    return Mapped_Fields.Publisher;

                case "PLACEOFPUBLICATION":
                case "PUBLICATIONPLACE":
                case "PUBPLACE":
                case "PUBLICATIONLOCATION":
                case "PLACE":
                    return Mapped_Fields.Pub_Place;

                case "RIGHTS":
                    return Mapped_Fields.Rights;

                case "SCALE":
                case "MAPSCALE":
                    return Mapped_Fields.Scale;

                case "SECTION":
                    return Mapped_Fields.Section;

                case "BIB(SERIES)TITLE":
                case "SERIESTITLE":
                    return Mapped_Fields.Series_Title;

                case "SOURCEINSTITUTIONCODE":
                case "SOURCECODE":
                    return Mapped_Fields.Source_Code;

                case "SOURCEINSTITUTIONSTATEMENT":
                case "SOURCESTATEMENT":
                case "SOURCE":
                    return Mapped_Fields.Source_Statement;

                case "STATEEDITION":
                    return Mapped_Fields.State_Edition;

                case "STYLE":
                case "PERIOD":
                case "STYLEPERIOD":
                    return Mapped_Fields.Style_Period;

                case "SUBJECTKEYWORD":
                case "SUBJECTKEYWORDS":
                case "SUBJECT":
                case "SUBJECTS":
                    return Mapped_Fields.Subject_Keyword;

                case "SUBTITLE":
                    return Mapped_Fields.Sub_Title;

                case "TECHNIQUE":
                    return Mapped_Fields.Technique;

                case "TEMPORALCOVERAGE":
                case "TEMPORAL":
                    return Mapped_Fields.Temporal_Coverage;

                case "TICKLER":
                    return Mapped_Fields.Tickler;

                case "VOLUMETITLE":
                case "TITLE":
                    return Mapped_Fields.Title;

                case "TRACKINGBOX":
                    return Mapped_Fields.Tracking_Box;

                case "MATERIALTYPE":
                case "TYPE":
                    return Mapped_Fields.Type;

                case "BIB(UNIFORM)TITLE":
                case "UNIFORMTITLE":
                    return Mapped_Fields.Uniform_Title;

                case "VID":
                    return Mapped_Fields.VID;

                case "VISIBILITY":
                    return Mapped_Fields.Visibility;

                case "VOLUME":
                    return Mapped_Fields.Volume;

                case "YEAR":
                    return Mapped_Fields.Year;

                default:
                    return Mapped_Fields.None;
            }
        }

        /// <summary> Converts the mapped field enumeration to a string </summary>
        /// <param name="Field"> Mapped field enumeration </param>
        /// <returns> Mapped field name as a string </returns>
        public static string Mapped_Field_To_String(Mapped_Fields Field)
        {
            // Everything depends on the field which is mapped
            switch (Field)
            {
                case Mapped_Fields.None:
                    return "None";

                case Mapped_Fields.Abstract:
                    return "Abstract";

                case Mapped_Fields.Accession_Number:
                    return "Accession Number";

                case Mapped_Fields.Affiliation_University:
                    return "Affiliation.University";

                case Mapped_Fields.Affiliation_Campus:
                    return "Affiliation.Campus";

                case Mapped_Fields.Affiliation_College:
                    return "Affiliation.College";

                case Mapped_Fields.Affiliation_Unit:
                    return "Affiliation.Unit";

                case Mapped_Fields.Affiliation_Department:
                    return "Affiliation.Department";

                case Mapped_Fields.Affiliation_Institute:
                    return "Affiliation.Institute";

                case Mapped_Fields.Affiliation_Center:
                    return "Affiliation.Center";

                case Mapped_Fields.Affiliation_Section:
                    return "Affiliation.Section";

                case Mapped_Fields.Affiliation_Subsection:
                    return "Affiliation.SubSection";

                case Mapped_Fields.Aggregation_Code:
                    return "Aggregation Code";

                case Mapped_Fields.Aleph:
                    return "ALEPH";

                case Mapped_Fields.Alternate_Title:
                    return "Alternate Title";

                case Mapped_Fields.Attribution:
                    return "Attribution";

                case Mapped_Fields.BibID:
                    return "BibID";

                case Mapped_Fields.Born_Digital_Flag:
                    return "Born Digital";

                case Mapped_Fields.Container_Box:
                    return "Container.Box";

                case Mapped_Fields.Container_Folder:
                    return "Container.Folder";

                case Mapped_Fields.Container_Divider:
                    return "Container.Divider";

                case Mapped_Fields.Contributor:
                    return "Contributor";

                case Mapped_Fields.Coordinates:
                    return "Coordinates";

                case Mapped_Fields.Copyright_Date:
                    return "Copyright Date";

                case Mapped_Fields.Creator:
                    return "Creator";

                case Mapped_Fields.Creator_Affiliation:
                    return "Creator Affiliation";

                case Mapped_Fields.Cultural_Context:
                    return "Cultural Context";

                case Mapped_Fields.Date:
                    return "Date";

                case Mapped_Fields.Day:
                    return "Day";

                case Mapped_Fields.Donor:
                    return "Donor";

                case Mapped_Fields.EAD_Name:
                    return "EAD Name";

                case Mapped_Fields.EAD_URL:
                    return "EAD URL";

                case Mapped_Fields.Edition:
                    return "Edition";

                case Mapped_Fields.Format:
                    return "Format";

                case Mapped_Fields.Genre:
                    return "Genre";

                case Mapped_Fields.Geography_Continent:
                    return "Geography.Continent";

                case Mapped_Fields.Geography_Country:
                    return "Geography.Country";

                case Mapped_Fields.Geography_Province:
                    return "Geography.Province";

                case Mapped_Fields.Geography_Region:
                    return "Geography.Region";

                case Mapped_Fields.Geography_State:
                    return "Geography.State";

                case Mapped_Fields.Geography_Territory:
                    return "Geography.Territory";

                case Mapped_Fields.Geography_County:
                    return "Geography.County";

                case Mapped_Fields.Geography_City:
                    return "Geography.City";

                case Mapped_Fields.Geography_Island:
                    return "Geography.Island";

                case Mapped_Fields.Geography_Area:
                    return "Geography.Area";

                case Mapped_Fields.Holding_Code:
                    return "Holding Location Code";

                case Mapped_Fields.Holding_Statement:
                    return "Holding Location Statement";

                case Mapped_Fields.Icon_Wordmarks:
                case Mapped_Fields.Wordmark:
                    return "Icon";

                case Mapped_Fields.Identifier:
                    return "Identifier";

                case Mapped_Fields.Inscription:
                    return "Inscription";

                case Mapped_Fields.Internal_Comments:
                    return "Internal Comments";

                case Mapped_Fields.ISBN:
                    return "ISBN";

                case Mapped_Fields.ISSN:
                    return "ISSN";

                case Mapped_Fields.Issue:
                    return "Issue";

                case Mapped_Fields.Language:
                    return "Language";

                case Mapped_Fields.Latitude:
                    return "Latitude";

                case Mapped_Fields.LCCN:
                    return "LCCN";

                case Mapped_Fields.Longitude:
                    return "Longitude";

                case Mapped_Fields.Materials:
                    return "Materials";

                case Mapped_Fields.Material_Received_Date:
                    return "Material Recd Date";
            
                case Mapped_Fields.Measurements:
                    return "Measurements";

                case Mapped_Fields.Month:
                    return "Month";

                case Mapped_Fields.Note:
                    return "Note";

                case Mapped_Fields.OCLC:
                    return "OCLC";

                case Mapped_Fields.Projection:
                    return "Projection";

                case Mapped_Fields.Publisher:
                    return "Publisher";

                case Mapped_Fields.Pub_Place:
                    return "Publication Place";

                case Mapped_Fields.Rights:
                    return "Rights";

                case Mapped_Fields.Scale:
                    return "Scale";

                case Mapped_Fields.Section:
                    return "Section";

                case Mapped_Fields.Series_Title:
                    return "Series Title";

                case Mapped_Fields.Source_Code:
                    return "Source Institution Code";

                case Mapped_Fields.Source_Statement:
                    return "Source Institution Statement";

                case Mapped_Fields.State_Edition:
                    return "State/Edition";

                case Mapped_Fields.Style_Period:
                    return "Style/Period";

                case Mapped_Fields.Subject_Keyword:
                    return "Subject Keyword";

                case Mapped_Fields.Sub_Title:
                    return "SubTitle";

                case Mapped_Fields.Technique:
                    return "Technique";

                case Mapped_Fields.Temporal_Coverage:
                    return "Temporal Converage";

                case Mapped_Fields.Tickler:
                    return "Tickler";

                case Mapped_Fields.Title:
                    return "Title";

                case Mapped_Fields.Tracking_Box:
                    return "Tracking Box";

                case Mapped_Fields.Uniform_Title:
                    return "Uniform Title";
                    
                case Mapped_Fields.Type:
                    return "Material Type"; 

                case Mapped_Fields.VID:
                    return "VID";

                case Mapped_Fields.Visibility:
                    return "Visibility";

                case Mapped_Fields.Volume:
                    return "Volume";

                case Mapped_Fields.Year:
                    return "Year";
            }

            return "UNKNOWN";
        }

		/// <summary> Adds a bit of data to a bibliographic package using the mapping </summary>
		/// <param name="Package">Bibliographic package to receive the data</param>
		/// <param name="Data">Text of the data</param>
		/// <param name="Field">Mapped field</param>
		public static void Add_Data( SobekCM_Item Package, string Data, Mapped_Fields Field )
		{
            Data = Data.Trim();
            if (Data.Length == 0)
                return;

			// Everything depends on the field which is mapped
			switch( Field )
			{
				case Mapped_Fields.None:
					// Do nothing, since no mapping exists
					break;

				case Mapped_Fields.Abstract:
					Package.Bib_Info.Add_Abstract(Data, "en" );
					break;
                case Mapped_Fields.Accession_Number:
                    Package.Bib_Info.Add_Identifier(Data, "Accession Number");
                    break;
				case Mapped_Fields.Alternate_Title:
					Package.Bib_Info.Add_Other_Title( Data, Title_Type_Enum.alternative );
					break;
				case Mapped_Fields.Attribution:
                    Package.Bib_Info.Add_Note(Data, Note_Type_Enum.funding);
					break;
				case Mapped_Fields.Aggregation_Code:
                    Package.SobekCM_Web.Add_Aggregation( Data.ToUpper() );                    
					break;
				case Mapped_Fields.Contributor:
                    Package.Bib_Info.Add_Named_Entity(new Name_Info(Data, "contributor"));
					break;
				case Mapped_Fields.Creator:
                    Package.Bib_Info.Add_Named_Entity(new Name_Info(Data, "creator"));
					break;
                case Mapped_Fields.Creator_Affiliation:
                    if (Package.Bib_Info.Names_Count > 0)
                    {
                        Package.Bib_Info.Names[0].Affiliation = Data;
                    }
                    break;
                case Mapped_Fields.Cultural_Context:
                    Package.Bib_Info.VRACore.Add_Cultural_Context(Data);
                    break;
				case Mapped_Fields.Donor:
					Package.Bib_Info.Donor.Full_Name = Data;
					break;
				case Mapped_Fields.Genre:
                    Subject_Info_Standard genre = new Subject_Info_Standard();
                    genre.Add_Genre(Data);
                    Package.Bib_Info.Add_Subject(genre);
					break;
				case Mapped_Fields.Holding_Code:
                    Package.Bib_Info.Location.Holding_Code = Data;
					break;
				case Mapped_Fields.Holding_Statement:
					Package.Bib_Info.Location.Holding_Name = Data;
					break;
				case Mapped_Fields.Identifier:
					Package.Bib_Info.Add_Identifier( Data );
					break;
                case Mapped_Fields.Inscription:
                    Package.Bib_Info.VRACore.Add_Inscription(Data);
                    break;
				case Mapped_Fields.Language:
                    Package.Bib_Info.Add_Language(Data);
					break;               
                case Mapped_Fields.Publisher:
                    Package.Bib_Info.Add_Publisher(Data);
                    break;
                case Mapped_Fields.Pub_Place:
                    Package.Bib_Info.Origin_Info.Add_Place(Data);
                    break;
				case Mapped_Fields.Source_Code:
					Package.Bib_Info.Source.Code = Data;
					break;
				case Mapped_Fields.Source_Statement:
					Package.Bib_Info.Source.Statement = Data;
					break;
				case Mapped_Fields.Subject_Keyword:
                    Package.Bib_Info.Add_Subject(Data, String.Empty);
					break;
				case Mapped_Fields.BibID:
					Package.Bib_Info.BibID = Data.ToUpper();
					break;
				case Mapped_Fields.VID:
					Package.Bib_Info.VID = Data.PadLeft(5, '0');
					break;
				case Mapped_Fields.Date:
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
                            Package.Bib_Info.Origin_Info.Date_Issued = System.Convert.ToInt32(Data).ToString();
                        }
                        catch
                        {
                            Package.Bib_Info.Origin_Info.Date_Issued = Data;
                        }
                    }
                    break;
				case Mapped_Fields.Edition:
                    Package.Bib_Info.Origin_Info.Edition = Data;
					break;
				case Mapped_Fields.Format:
                    Package.Bib_Info.Original_Description.Extent = Data;
					break;
				case Mapped_Fields.Note:
                    Package.Bib_Info.Add_Note(Data);
					break;
				case Mapped_Fields.Rights:
                    Package.Bib_Info.Access_Condition.Text = Data;
					break;				
				case Mapped_Fields.Series_Title:
					Package.Bib_Info.SeriesTitle.Title = Data;
                    Package.SobekCM_Web.GroupTitle = Data;
					break;
				case Mapped_Fields.Type:
                    string upper_data = Data.ToUpper();
                    if (upper_data.IndexOf("NEWSPAPER") >= 0 )
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Newspaper;
                        break;
                    }
                    if ((upper_data.IndexOf("MONOGRAPH") >= 0 ) || ( upper_data.IndexOf("BOOK") >= 0 ))
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        break;
                    }
                    if (upper_data.IndexOf("SERIAL") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Serial;
                        break;
                    }
                    if (upper_data.IndexOf("AERIAL") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Aerial;
                        if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                            Package.Bib_Info.Original_Description.Extent = "Aerial Photograph";
                        break;
                    }
                    if (upper_data.IndexOf("PHOTO") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        break;
                    }
                    if (upper_data.IndexOf("POSTCARD") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                            Package.Bib_Info.Original_Description.Extent = "Postcard";
                        break;
                    }
                    if (upper_data.IndexOf("MAP") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Map;
                        break;
                    }
                    if (upper_data.IndexOf("TEXT") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Book;
                        break;
                    }
                    if (upper_data.IndexOf("AUDIO") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Audio;
                        break;
                    }
                    if (upper_data.IndexOf("VIDEO") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Video;
                        break;
                    }
                    if ((upper_data.IndexOf("ARCHIVE") >= 0) || ( upper_data.IndexOf("ARCHIVAL") >= 0 ))
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Archival;
                        break;
                    }
                    if (upper_data.IndexOf("ARTIFACT") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Artifact;
                        break;
                    }
                    if (upper_data.IndexOf("IMAGE") >= 0)
                    {
                        Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Photograph;
                        break;
                    }

                    // if there was no match, set type to "UNDETERMINED"
                    Package.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.UNKNOWN;

                    if (Package.Bib_Info.Original_Description.Extent.Length == 0)
                        Package.Bib_Info.Original_Description.Extent = "Undetermined";
					break;
				case Mapped_Fields.Uniform_Title:
                    Package.Bib_Info.Add_Other_Title(Data, Title_Type_Enum.uniform);
                    Package.SobekCM_Web.GroupTitle = Data;
					break;
				case Mapped_Fields.Title:
					Package.Bib_Info.Main_Title.Title = Data;
					break;
				case Mapped_Fields.Aleph:
                    Package.Bib_Info.Add_Identifier(Data, "ALEPH");
                    break;
				case Mapped_Fields.OCLC:
                    Package.Bib_Info.Add_Identifier(Data, "OCLC");
					break;
                case Mapped_Fields.LCCN:
                    Package.Bib_Info.Add_Identifier(Data, "LCCN");
                    break;
                case Mapped_Fields.ISBN:
                    Package.Bib_Info.Add_Identifier(Data, "ISBN");
                    break;
                case Mapped_Fields.ISSN:
                    Package.Bib_Info.Add_Identifier(Data, "ISSN");
                    break;
                case Mapped_Fields.Sub_Title:
                    Package.Bib_Info.Main_Title.Subtitle = Data;
                    break;
                case Mapped_Fields.Volume:                   
                   Package.Bib_Info.Series_Part_Info.Enum1 = Data;
                   Package.Serial_Info.Add_Hierarchy(Package.Serial_Info.Count + 1, 1, Data);
                   break;
                case Mapped_Fields.Issue:
                    if (Package.Bib_Info.Series_Part_Info.Enum1.Length == 0)
                    {
                        Package.Bib_Info.Series_Part_Info.Enum1 = Data;
                    }
                    else
                    {
                        Package.Bib_Info.Series_Part_Info.Enum2 = Data;
                    }
                    Package.Serial_Info.Add_Hierarchy(Package.Serial_Info.Count + 1, 1, Data);
                    break;
                case Mapped_Fields.Section:
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
                    Package.Serial_Info.Add_Hierarchy(Package.Serial_Info.Count + 1, 1, Data);
                    // Do nothing for now
                    break;

                case Mapped_Fields.Year:
                    Package.Bib_Info.Series_Part_Info.Year = Data;

                    if (Data.Length == 1)
                        year = "0" + Data;
                    else
                        year = Data;
                    build_date_string(Package);
                                        
                    break;

                case Mapped_Fields.Month:
                    Package.Bib_Info.Series_Part_Info.Month = Data;             
                    month = Data;
                    build_date_string(Package);
                                       
                    break;

                case Mapped_Fields.Day:
                    Package.Bib_Info.Series_Part_Info.Day = Data;                     
                    day = Data;
                    build_date_string(Package);
                   
                    break;                     
               			          
                case Mapped_Fields.Coordinates:                   	              
                    string[] coordinates = Data.Split(", ;".ToCharArray());
                    try
                    {
                        if (coordinates.Length == 2)
                        {
                            Package.Bib_Info.Coordinates.Add_Point( Convert.ToDouble(coordinates[0]), Convert.ToDouble(coordinates[1]), String.Empty);
                        }
                        else
                        {
                            coordinates = Data.Split(",;".ToCharArray());
                            if (coordinates.Length == 2)
                            {
                                Package.Bib_Info.Coordinates.Add_Point(Convert.ToDouble(coordinates[0]), Convert.ToDouble(coordinates[1]), String.Empty);
                            }
                        }
                    }
                    catch { }
                    break;

                case Mapped_Fields.Latitude:
                    try
                    {
                        if (Package.Bib_Info.Coordinates.Point_Count == 0)
                            Package.Bib_Info.Coordinates.Add_Point(Convert.ToDouble(Data), 0, String.Empty);
                        else
                            Package.Bib_Info.Coordinates.Points[0].Latitude = Convert.ToDouble(Data);
                    }
                    catch { }
                    break;

                case Mapped_Fields.Longitude:
                    try
                    {
                    if (Package.Bib_Info.Coordinates.Point_Count == 0)
                        Package.Bib_Info.Coordinates.Add_Point(0, Convert.ToDouble(Data.Replace("°", "")), String.Empty);
                    else
                        Package.Bib_Info.Coordinates.Points[0].Longitude = Convert.ToDouble(Data.Replace("°", ""));
                    }
                    catch { }
                    break;

                case Mapped_Fields.Projection:
                    Guarantee_Cartographics(Package).Projection = Data;
					break;        
                case Mapped_Fields.Scale:
                    Guarantee_Cartographics(Package).Scale = Data;
				    break;
                //case Mapped_Fields.Spatial_Coverage:
                //    Package.Bib_Info.Hierarchical_Spatials[0].Area = Data;
                //    break;
                case Mapped_Fields.Icon_Wordmarks:
                case Mapped_Fields.Wordmark:
                //    Package.Processing_Parameters.Icons.Add(Data, String.Empty);
                    Package.SobekCM_Web.Add_Wordmark(Data);     
				    break;
                case Mapped_Fields.Temporal_Coverage:
                    Package.Bib_Info.Add_Temporal_Subject(-1, -1, Data );
					break;                        
                case Mapped_Fields.Affiliation_University:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].University = Data;
					break;                    
                case Mapped_Fields.Affiliation_Campus:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].Campus = Data;
					break;                          
                case Mapped_Fields.Affiliation_College:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].College = Data;
					break;                         
                case Mapped_Fields.Affiliation_Unit:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].Unit = Data;
					break;
                case Mapped_Fields.Affiliation_Department:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].Department = Data;
					break; 
                case Mapped_Fields.Affiliation_Institute:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].Institute = Data;
					break;                           
                case Mapped_Fields.Affiliation_Center:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].Center = Data;
					break;        
                case Mapped_Fields.Affiliation_Section:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].Section = Data;
					break; 
                case Mapped_Fields.Affiliation_Subsection:
                    Guarantee_Affiliation_Collection(Package);
					Package.Bib_Info.Affiliations[0].SubSection = Data;
					break;                          
                case Mapped_Fields.Geography_Continent:
                    Guarantee_Hierarchical_Spatial(Package).Continent = Data;
					break;                                           
                case Mapped_Fields.Geography_Country:
                    Guarantee_Hierarchical_Spatial(Package).Country = Data;               
					break;                    
                case Mapped_Fields.Geography_Province:
                    Guarantee_Hierarchical_Spatial(Package).Province = Data;              
					break;                          
                case Mapped_Fields.Geography_Region:
                    Guarantee_Hierarchical_Spatial(Package).Region = Data;      
					break;                
                case Mapped_Fields.Geography_State:
                    Guarantee_Hierarchical_Spatial(Package).State = Data;               
                    break;
                case Mapped_Fields.Geography_Territory:
                    Guarantee_Hierarchical_Spatial(Package).Territory = Data;                   
                    break;
                case Mapped_Fields.Geography_County:
                    Guarantee_Hierarchical_Spatial(Package).County = Data;                  
					break;               
                case Mapped_Fields.Geography_City:
                    Guarantee_Hierarchical_Spatial(Package).City = Data;                   
					break;                
                case Mapped_Fields.Geography_Island:
                    Guarantee_Hierarchical_Spatial(Package).Island = Data;                  
					break;
                case Mapped_Fields.Geography_Area:
                    Guarantee_Hierarchical_Spatial(Package).Area = Data;
                    break;
                case Mapped_Fields.Copyright_Date:
                    Package.Bib_Info.Origin_Info.Date_Copyrighted = Data;
                    break;
                case Mapped_Fields.EAD_Name:
                    Package.Bib_Info.Location.EAD_Name = Data;
                    break;
                case Mapped_Fields.EAD_URL:
                    Package.Bib_Info.Location.EAD_URL = Data;
                    break;
                case Mapped_Fields.Internal_Comments:
                    Package.Tracking.Internal_Comments = Data;
                    break;
                case Mapped_Fields.Container_Box:
                    Package.Bib_Info.Add_Container("Box", Data, 1);
                    break;
                case Mapped_Fields.Container_Divider:
                    Package.Bib_Info.Add_Container("Divider", Data, 2);
                    break;
                case Mapped_Fields.Container_Folder:
                    Package.Bib_Info.Add_Container("Folder", Data, 3);
                    break;
                case Mapped_Fields.Visibility:
                    switch (Data.ToUpper())
                    {
                        case "DARK":
                            Package.SobekCM_Web.Dark_Flag = true;
                            Package.SobekCM_Web.IP_Restriction_Membership = -1;
                            break;

                        case "PRIVATE":
                            Package.SobekCM_Web.Dark_Flag = false;
                            Package.SobekCM_Web.IP_Restriction_Membership = -1;
                            break;

                        case "PUBLIC":
                            Package.SobekCM_Web.Dark_Flag = false;
                            Package.SobekCM_Web.IP_Restriction_Membership = 0;
                            break;

                        case "RESTRICTED":
                            Package.SobekCM_Web.Dark_Flag = false;
                            Package.SobekCM_Web.IP_Restriction_Membership = 1;
                            break;
                    }
                    break;

                case Mapped_Fields.Tickler:
                    Package.SobekCM_Web.Add_Tickler(Data);
                    break;

                case Mapped_Fields.Tracking_Box:
                    Package.Tracking.Tracking_Box = Data;
                    break;

                case Mapped_Fields.Born_Digital_Flag:
                    if (Data.ToUpper().Trim() == "TRUE")
                        Package.Tracking.Born_Digital = true;
                    break;

                case Mapped_Fields.Material_Received_Date:
                    try
                    {
                        DateTime materialReceivedDate = Convert.ToDateTime(Data);
                    }
                    catch
                    {

                    }
                    break;

                case Mapped_Fields.Materials:
                    Package.Bib_Info.VRACore.Add_Material(Data, "medium");
                    break;

                case Mapped_Fields.Measurements:
                    Package.Bib_Info.VRACore.Add_Measurement(Data, String.Empty);
                    break;

                case Mapped_Fields.State_Edition:
                    Package.Bib_Info.VRACore.Add_State_Edition(Data);
                    break;

                case Mapped_Fields.Style_Period:
                    Package.Bib_Info.VRACore.Add_Style_Period(Data);
                    break;

                case Mapped_Fields.Technique:
                    Package.Bib_Info.VRACore.Add_Technique(Data);
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
