#region Using directives

using System;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Resource_Object.Metadata_Modules.LearningObjects;
using SobekCM.Resource_Object.Behaviors;

#endregion

namespace SobekCM.Resource_Object.Testing
{
    /// <summary> Class is used to create a test METS file which has nearly all of the 
    /// possible elements in a SobekCM mets file. </summary>
    public class Test_Bib_Package
    {
        /// <summary> Create a new instance of the Test_Bib_Package class </summary>
        public Test_Bib_Package()
        {
            // Do nothing here
        }

        /// <summary> Create a test digital resource item  </summary>       
        /// <param name="directory">Directory for the package source directory</param>
        /// <returns>Fully built test bib package</returns>
        public static SobekCM_Item Create(string directory)
        {
            SobekCM_Item testPackage = new SobekCM_Item();

            // Add all the METS header information
            testPackage.METS_Header.Create_Date = new DateTime(2007, 1, 1);
            testPackage.METS_Header.Modify_Date = DateTime.Now;
            testPackage.METS_Header.Creator_Individual = "Mark Sullivan";
            testPackage.METS_Header.Add_Creator_Individual_Notes("Programmer of new SobekCM.Resource_Object");
            testPackage.METS_Header.Add_Creator_Individual_Notes("Adding coordinates");
            testPackage.METS_Header.Creator_Organization = "University of Florida";
            testPackage.METS_Header.Creator_Software = "SobekCM Bib Package Test";
            testPackage.METS_Header.RecordStatus_Enum = METS_Record_Status.COMPLETE;
            testPackage.METS_Header.Add_Creator_Org_Notes("This test package was done to test DLCs new METS package");

            // Add all the MODS elements
            Abstract_Info testAbstract = testPackage.Bib_Info.Add_Abstract("This is a sample abstract", "en");
            testPackage.Bib_Info.Add_Abstract("Tämä on esimerkki abstrakteja", "fin");
            testAbstract.Display_Label = "Summary Abstract";
            testAbstract.Type = "summary";

            testPackage.Bib_Info.Access_Condition.Text = "All rights are reserved by source institution.";
            testPackage.Bib_Info.Access_Condition.Language = "en";
            testPackage.Bib_Info.Access_Condition.Type = "restrictions on use";
            testPackage.Bib_Info.Access_Condition.Display_Label = "Rights";

            testPackage.Bib_Info.Add_Identifier("000123234", "OCLC", "Electronic OCLC");
            testPackage.Bib_Info.Add_Identifier("182-asdsd-28k", "DOI");

            testPackage.Bib_Info.Add_Language("English", String.Empty, "en");
            testPackage.Bib_Info.Add_Language("Finnish");
            testPackage.Bib_Info.Add_Language(String.Empty, "ita", String.Empty);

            testPackage.Bib_Info.Location.Holding_Code = "MVS";
            testPackage.Bib_Info.Location.Holding_Name = "From the Private Library of Mark Sullivan";
            testPackage.Bib_Info.Location.PURL = "http://www.uflib.ufl.edu/ufdc/?b=CA00000000";
            testPackage.Bib_Info.Location.Other_URL = "http://www.fnhm.edu";
            testPackage.Bib_Info.Location.Other_URL_Display_Label = "Specimen Information";
            testPackage.Bib_Info.Location.Other_URL_Note = "Specimen FLAS 125342 Database";
            testPackage.Bib_Info.Location.EAD_URL = "http://digital.uflib.ufl.edu/";
            testPackage.Bib_Info.Location.EAD_Name = "Digital Library Center Finding Guide";

            testPackage.Bib_Info.Main_Entity_Name.Name_Type = Name_Info_Type_Enum.personal;
            testPackage.Bib_Info.Main_Entity_Name.Full_Name = "Brown, B.F.";
            testPackage.Bib_Info.Main_Entity_Name.Terms_Of_Address = "Dr.";
            testPackage.Bib_Info.Main_Entity_Name.Display_Form = "B.F. Brown";
            testPackage.Bib_Info.Main_Entity_Name.Affiliation = "Chemistry Dept., American University";
            testPackage.Bib_Info.Main_Entity_Name.Description = "Chemistry Professor Emeritus";
            testPackage.Bib_Info.Main_Entity_Name.Add_Role("Author");

            Zoological_Taxonomy_Info taxonInfo = new Zoological_Taxonomy_Info();
            testPackage.Add_Metadata_Module(GlobalVar.ZOOLOGICAL_TAXONOMY_METADATA_MODULE_KEY, taxonInfo);
            taxonInfo.Scientific_Name = "Ctenomys sociabilis";
            taxonInfo.Higher_Classification = "Animalia; Chordata; Vertebrata; Mammalia; Theria; Eutheria; Rodentia; Hystricognatha; Hystricognathi; Ctenomyidae; Ctenomyini; Ctenomys";
            taxonInfo.Kingdom = "Animalia";
            taxonInfo.Phylum = "Chordata";
            taxonInfo.Class = "Mammalia";
            taxonInfo.Order = "Rodentia";
            taxonInfo.Family = "Ctenomyidae";
            taxonInfo.Genus = "Ctenomys";
            taxonInfo.Specific_Epithet = "sociabilis";
            taxonInfo.Taxonomic_Rank = "species";
            taxonInfo.Common_Name = "Social Tuco-Tuco";

            Name_Info name1 = new Name_Info();
            name1.Name_Type = Name_Info_Type_Enum.personal;
            name1.Given_Name = "John Paul";
            name1.Terms_Of_Address = "Pope; II";
            name1.Dates = "1920-2002";
            name1.User_Submitted = true;
            testPackage.Bib_Info.Add_Named_Entity(name1);

            Name_Info name2 = new Name_Info();
            name2.Name_Type = Name_Info_Type_Enum.conference;
            name2.Full_Name = "Paris Peace Conference (1919-1920)";
            name2.Dates = "1919-1920";
            testPackage.Bib_Info.Add_Named_Entity(name2);

            Name_Info name3 = new Name_Info();
            name3.Name_Type = Name_Info_Type_Enum.corporate;
            name3.Full_Name = "United States -- Court of Appeals (2nd Court)";
            testPackage.Bib_Info.Add_Named_Entity(name3);

            Name_Info name4 = new Name_Info();
            name4.Name_Type = Name_Info_Type_Enum.personal;
            name4.Full_Name = "Wilson, Mary";
            name4.Display_Form = "Mary 'Weels' Wilson";
            name4.Given_Name = "Mary";
            name4.Family_Name = "Wilson";
            name4.ID = "NAM4";
            name4.Terms_Of_Address = "2nd";
            name4.Add_Role("illustrator");
            name4.Add_Role("cartographer");
            testPackage.Bib_Info.Add_Named_Entity(name4);

            Name_Info donor = new Name_Info();
            donor.Name_Type = Name_Info_Type_Enum.personal;
            donor.Full_Name = "Livingston, Arthur";
            donor.Description = "Gift in honor of Arthur Livingston";
            donor.Terms_Of_Address = "3rd";
            donor.Add_Role("honoree", String.Empty);
            testPackage.Bib_Info.Donor = donor;

            testPackage.Bib_Info.Main_Title.NonSort = "The ";
            testPackage.Bib_Info.Main_Title.Title = "Man Who Would Be King";
            testPackage.Bib_Info.Main_Title.Subtitle = "The story of succession in England";

            Title_Info title1 = new Title_Info("homme qui voulut être roi", Title_Type_Enum.translated);
            title1.NonSort = "L'";
            title1.Language = "fr";
            testPackage.Bib_Info.Add_Other_Title(title1);

            Title_Info title2 = new Title_Info();
            title2.Title = "Man Who Be King";
            title2.Display_Label = "also known as";
            title2.NonSort = "The";
            title2.Title_Type = Title_Type_Enum.alternative;
            testPackage.Bib_Info.Add_Other_Title(title2);

            Title_Info title3 = new Title_Info();
            title3.Title = "Great works of England";
            title3.Authority = "naf";
            title3.Add_Part_Name("Second Portion");
            title3.Add_Part_Number("2nd");
            title3.Title_Type = Title_Type_Enum.uniform;
            title3.User_Submitted = true;
            testPackage.Bib_Info.Add_Other_Title(title3);

            testPackage.Bib_Info.Add_Note("Funded by the NEH", Note_Type_Enum.funding);
            testPackage.Bib_Info.Add_Note("Based on a play which originally appeared in France as \"Un peu plus tard, un peu plus tôt\"").User_Submitted = true;
            testPackage.Bib_Info.Add_Note("Anne Baxter (Louise), Maria Perschy (Angela), Gustavo Rojo (Bill), Reginald Gilliam (Mr. Johnson), [Catherine Elliot?] (Aunt Sallie), Ben Tatar (waiter)", Note_Type_Enum.performers, "Performed By");

            testPackage.Bib_Info.Origin_Info.Add_Place("New York", "nyu", "usa");
            testPackage.Bib_Info.Origin_Info.Date_Issued = "1992";
            testPackage.Bib_Info.Origin_Info.MARC_DateIssued_Start = "1992";
            testPackage.Bib_Info.Origin_Info.MARC_DateIssued_End = "1993";
            testPackage.Bib_Info.Origin_Info.Date_Copyrighted = "1999";
            testPackage.Bib_Info.Origin_Info.Edition = "2nd";

            Publisher_Info newPub = testPackage.Bib_Info.Add_Publisher("Published for the American Vacuum Society by the American Institute of Physics");
            newPub.Add_Place("New York, New York");
            newPub.User_Submitted = true;
            testPackage.Bib_Info.Add_Publisher("University of Florida Press House").Add_Place("Gainesville, FL");
            testPackage.Bib_Info.Add_Manufacturer("Addison Randly Publishing House");

            testPackage.Bib_Info.Original_Description.Extent = "1 sound disc (56 min.) : digital ; 3/4 in.";
            testPackage.Bib_Info.Original_Description.Add_Note("The sleeve of this sound disc was damaged in a fire");
            testPackage.Bib_Info.Original_Description.Add_Note("The disc has a moderate amount of scratches, but still plays");

            testPackage.Bib_Info.Series_Part_Info.Day = "18";
            testPackage.Bib_Info.Series_Part_Info.Day_Index = 18;
            testPackage.Bib_Info.Series_Part_Info.Month = "Syyskuu";
            testPackage.Bib_Info.Series_Part_Info.Month_Index = 9;
            testPackage.Bib_Info.Series_Part_Info.Year = "1992";
            testPackage.Bib_Info.Series_Part_Info.Year_Index = 1992;

            testPackage.Bib_Info.Series_Part_Info.Enum1 = "Volume 12";
            testPackage.Bib_Info.Series_Part_Info.Enum1_Index = 12;
            testPackage.Bib_Info.Series_Part_Info.Enum2 = "Issue 3";
            testPackage.Bib_Info.Series_Part_Info.Enum2_Index = 3;
            testPackage.Bib_Info.Series_Part_Info.Enum3 = "Part 1";
            testPackage.Bib_Info.Series_Part_Info.Enum3_Index = 1;

            testPackage.Behaviors.Serial_Info.Add_Hierarchy(1, 1992, "1992");
            testPackage.Behaviors.Serial_Info.Add_Hierarchy(2, 9, "Syyskuu");
            testPackage.Behaviors.Serial_Info.Add_Hierarchy(3, 18, "18");

            testPackage.Bib_Info.SeriesTitle.Title = "Shakespeare's most famous musicals";

            testPackage.Bib_Info.Add_Target_Audience("young adults");
            testPackage.Bib_Info.Add_Target_Audience("adolescent", "marctarget");

            testPackage.Bib_Info.SobekCM_Type = TypeOfResource_SobekCM_Enum.Newspaper;

            // Add cartographic subject
            Subject_Info_Cartographics newCartographics = testPackage.Bib_Info.Add_Cartographics_Subject();
            newCartographics.Scale = "1:2000";
            newCartographics.Projection = "Conical Projection";
            newCartographics.Coordinates = "E 72°--E 148°/N 13°--N 18°";

            // Add hierarchical geographic subject
            Subject_Info_HierarchicalGeographic hierarchical = testPackage.Bib_Info.Add_Hierarchical_Geographic_Subject();
            hierarchical.Continent = "North America";
            hierarchical.Country = "United States of America";
            hierarchical.State = "Kansas";
            hierarchical.County = "Butler";
            hierarchical.City = "Augusta";

            // Add hierarchical geographic subject
            Subject_Info_HierarchicalGeographic hierarchical2 = testPackage.Bib_Info.Add_Hierarchical_Geographic_Subject();
            hierarchical2.Region = "Arctic Ocean";

            // Add hierarchical geographic subject
            Subject_Info_HierarchicalGeographic hierarchical3 = testPackage.Bib_Info.Add_Hierarchical_Geographic_Subject();
            hierarchical3.Island = "Puerto Rico";
            hierarchical3.Language = "English";
            hierarchical3.Province = "Provincial";
            hierarchical3.Territory = "Puerto Rico";
            hierarchical3.Area = "Intercontinental areas (Western Hemisphere)";

            // Add a name subject
            Subject_Info_Name subjname1 = testPackage.Bib_Info.Add_Name_Subject();
            subjname1.Authority = "lcsh";
            subjname1.Full_Name = "Garcia Lorca, Federico";
            subjname1.Dates = "1898-1936";
            subjname1.Add_Geographic("Russia");
            subjname1.Add_Geographic("Moscow");
            subjname1.Add_Genre("maps");
            subjname1.User_Submitted = true;

            // Add a title information subject
            Subject_Info_TitleInfo subjtitle1 = testPackage.Bib_Info.Add_Title_Subject();
            subjtitle1.Title_Type = Title_Type_Enum.uniform;
            subjtitle1.Authority = "naf";
            subjtitle1.Title = "Missale Carnotense";

            // Add a standard subject
            Subject_Info_Standard subject1 = testPackage.Bib_Info.Add_Subject();
            subject1.Authority = "lcsh";
            subject1.Add_Topic("Real property");
            subject1.Add_Geographic("Mississippi");
            subject1.Add_Geographic("Tippah County");
            subject1.Add_Genre("Maps");


            // Add a standard subject
            Subject_Info_Standard subject2 = testPackage.Bib_Info.Add_Subject();
            subject2.Add_Occupation("Migrant laborers");
            subject2.Add_Genre("School district case files");

            // Add a standard subject
            Subject_Info_Standard subject3 = testPackage.Bib_Info.Add_Subject();
            subject3.Authority = "lctgm";
            subject3.Add_Topic("Educational buildings");
            subject3.Add_Geographic("Washington (D.C.)");
            subject3.Add_Temporal("1890-1910");

            // Add a standard subject
            Subject_Info_Standard subject4 = testPackage.Bib_Info.Add_Subject();
            subject4.Authority = "rvm";
            subject4.Language = "french";
            subject4.Add_Topic("Église catholique");
            subject4.Add_Topic("Histoire");
            subject4.Add_Temporal("20e siècle");

            // Add record information
            testPackage.Bib_Info.Record.Add_Catalog_Language(new Language_Info("English", "eng", "en"));
            testPackage.Bib_Info.Record.Add_Catalog_Language(new Language_Info("French", "fre", "fr"));
            testPackage.Bib_Info.Record.MARC_Creation_Date = "080303";
            testPackage.Bib_Info.Record.Add_MARC_Record_Content_Sources("FUG");
            testPackage.Bib_Info.Record.Record_Origin = "Imported from (OCLC)001213124";


            // Test the items which are in the non-MODS portion of the Bib_Info object
            testPackage.BibID = "MVS0000001";
            testPackage.VID = "00001";
            testPackage.Bib_Info.SortDate = 1234;
            testPackage.Bib_Info.SortTitle = "MAN WHO WOULD BE KING";
            testPackage.Bib_Info.Add_Temporal_Subject(1990, 2002, "Recent history");
            testPackage.Bib_Info.Add_Temporal_Subject(1990, 2002, "Lähihistoria");
            testPackage.Bib_Info.Source.Code = "UF";
            testPackage.Bib_Info.Source.Statement = "University of Florida";

            // Add an affiliation
            Affiliation_Info affiliation1 = new Affiliation_Info();
            affiliation1.University = "University of Florida";
            affiliation1.Campus = "Gainesville Campus";
            affiliation1.College = "College of Engineering";
            affiliation1.Department = "Computer Engineering Department";
            affiliation1.Unit = "Robotics";
            affiliation1.Name_Reference = "NAM4";
            testPackage.Bib_Info.Add_Affiliation(affiliation1);

            // Add a related item
            Related_Item_Info relatedItem1 = new Related_Item_Info();
            relatedItem1.SobekCM_ID = "UF00001234";
            relatedItem1.Relationship = Related_Item_Type_Enum.preceding;
            relatedItem1.Publisher = "Gainesville Sun Publishing House";
            relatedItem1.Add_Note(new Note_Info("Digitized with funding from NEH", Note_Type_Enum.funding));
            relatedItem1.Add_Note(new Note_Info("Gainesville Bee was the precursor to this item"));
            relatedItem1.Main_Title.NonSort = "The";
            relatedItem1.Main_Title.Title = "Gainesville Bee";
            relatedItem1.Add_Identifier("01234353", "oclc");
            relatedItem1.Add_Identifier("002232311", "aleph");
            Name_Info ri_name = new Name_Info();
            ri_name.Full_Name = "Hills, Bryan";
            ri_name.Terms_Of_Address = "Mr.";
            ri_name.Name_Type = Name_Info_Type_Enum.personal;
            ri_name.Add_Role("author");
            relatedItem1.Add_Name(ri_name);
            relatedItem1.URL = @"http://www.uflib.ufl.edu/ufdc/?b=UF00001234";
            relatedItem1.URL_Display_Label = "Full Text";
            testPackage.Bib_Info.Add_Related_Item(relatedItem1);

            // Add another related item
            Related_Item_Info relatedItem2 = new Related_Item_Info();
            relatedItem2.Relationship = Related_Item_Type_Enum.succeeding;
            relatedItem2.SobekCM_ID = "UF00009999";
            relatedItem2.Main_Title.NonSort = "The";
            relatedItem2.Main_Title.Title = "Daily Sun";
            relatedItem2.Add_Identifier("0125437", "oclc");
            relatedItem2.Add_Note("Name change occured in Fall 1933");
            relatedItem2.Start_Date = "Fall 1933";
            relatedItem2.End_Date = "December 31, 1945";
            testPackage.Bib_Info.Add_Related_Item(relatedItem2);

            // Add some processing parameters
            testPackage.Behaviors.Add_Aggregation("JUV");
            testPackage.Behaviors.Add_Aggregation("DLOC");
            testPackage.Behaviors.Add_Aggregation("DLOSA1");
            testPackage.Behaviors.Add_Aggregation("ALICE");
            testPackage.Behaviors.Add_Aggregation("ARTE");

            testPackage.Web.GUID = "GUID!";
            testPackage.Behaviors.Add_Wordmark("DLOC");
            testPackage.Behaviors.Add_Wordmark("UFSPEC");
            testPackage.Behaviors.Main_Thumbnail = "00001thm.jpg";

            // Add some downloads
            testPackage.Divisions.Download_Tree.Add_File("MVS_Complete.PDF");
            testPackage.Divisions.Download_Tree.Add_File("MVS_Complete.MP2");
            testPackage.Divisions.Download_Tree.Add_File("MVS_Part1.MP2");
            testPackage.Divisions.Download_Tree.Add_File("MVS_Part1.PDF");

            // Add some coordinate information
            GeoSpatial_Information geoSpatial = new GeoSpatial_Information();
            testPackage.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoSpatial);
            geoSpatial.Add_Point(29.530151, -82.301459, "Lake Wauberg");
            geoSpatial.Add_Point(29.634352, -82.350640, "Veterinary School");
            Coordinate_Polygon polygon = new Coordinate_Polygon();
            polygon.Label = "University of Florida Campus";
            polygon.Add_Edge_Point(new Coordinate_Point(29.651435, -82.339869, String.Empty));
            polygon.Add_Edge_Point(new Coordinate_Point(29.641216, -82.340298, String.Empty));
            polygon.Add_Edge_Point(new Coordinate_Point(29.629503, -82.371969, String.Empty));
            polygon.Add_Edge_Point(new Coordinate_Point(29.649645, -82.371712, String.Empty));
            polygon.Add_Inner_Point(29.649794, -82.351971, "Stadium");
            polygon.Add_Inner_Point(29.650988, -82.341156, "Library");
            geoSpatial.Add_Polygon(polygon);
            Coordinate_Line line = new Coordinate_Line();
            line.Label = "Waldo Road";
            line.Add_Point(29.652852, -82.310944, "Gainesville");
            line.Add_Point(29.716681, -82.268372, String.Empty);
            line.Add_Point(29.791494, -82.167778, "Waldo");
            geoSpatial.Add_Line(line);


            // Add some performing arts information
            Performing_Arts_Info partInfo = new Performing_Arts_Info();
            testPackage.Add_Metadata_Module("PerformingArts", partInfo);
            partInfo.Performance = "Hamlet";
            partInfo.Performance_Date = "August 12, 1923";
            Performer performer1 = partInfo.Add_Performer("Sullivan, Mark");
            performer1.Sex = "M";
            performer1.LifeSpan = "1873-";
            performer1.Occupation = "actor";
            performer1.Title = "Mr.";

            Performer performer2 = partInfo.Add_Performer("Waldbart, Julia");
            performer2.Sex = "F";
            performer2.LifeSpan = "1876-";
            performer2.Occupation = "actress";
            performer2.Title = "Mrs.";

            // Add some oral history information
            Oral_Interview_Info oralInfo = new Oral_Interview_Info();
            testPackage.Add_Metadata_Module(  "OralInterview", oralInfo);
            oralInfo.Interviewee = "Edwards, Herm";
            oralInfo.Interviewer = "Proctor, Samual";

            // Add some learning object resource information
            LearningObjectMetadata lomInfo = new LearningObjectMetadata();
            testPackage.Add_Metadata_Module( GlobalVar.IEEE_LOM_METADATA_MODULE_KEY, lomInfo );
            lomInfo.AggregationLevel = AggregationLevelEnum.level3;
            lomInfo.Status = StatusEnum.draft;
            LOM_System_Requirements lomReq1 = new LOM_System_Requirements();
            lomReq1.RequirementType = RequirementTypeEnum.operating_system;
            lomReq1.Name.Value = "Windows";
            lomReq1.MinimumVersion = "Windows XP";
            lomReq1.MaximumVersion = "Windows 7";
            lomInfo.Add_SystemRequirements(lomReq1);
            LOM_System_Requirements lomReq2 = new LOM_System_Requirements();
            lomReq2.RequirementType = RequirementTypeEnum.software;
            lomReq2.Name.Value = "Java SDK";
            lomReq2.MinimumVersion = "1.7.1";
            lomReq2.MaximumVersion = "2.09";
            lomInfo.Add_SystemRequirements(lomReq2);
            lomInfo.InteractivityType = InteractivityTypeEnum.mixed;
            lomInfo.Add_LearningResourceType("exercise");
            lomInfo.Add_LearningResourceType("Tutorials", "encdlwebpedagogicaltype");
            lomInfo.InteractivityLevel = InteractivityLevelEnum.high;
            lomInfo.Add_IntendedEndUserRole(IntendedEndUserRoleEnum.learner);
            lomInfo.Add_Context("Undergraduate lower division", "enclearningcontext");
            lomInfo.Add_Context("15", "grade");
            lomInfo.Add_Context("16", "grade");
            lomInfo.Add_Context("5", "group");
            lomInfo.Add_TypicalAgeRange("suitable for children over 7", "en");
            lomInfo.Add_TypicalAgeRange("2-8");
            lomInfo.DifficultyLevel = DifficultyLevelEnum.medium;
            lomInfo.TypicalLearningTime = "PT45M";

            LOM_Classification lomClassification1 = new LOM_Classification();
            lomInfo.Add_Classification(lomClassification1);
            lomClassification1.Purpose.Value = "Discipline";
            LOM_TaxonPath lomTaxonPath1 = new LOM_TaxonPath();
            lomClassification1.Add_TaxonPath(lomTaxonPath1);
            lomTaxonPath1.Add_SourceName("ARIADNE");
            LOM_Taxon lomTaxon1 = new LOM_Taxon();
            lomTaxonPath1.Add_Taxon(lomTaxon1);
            lomTaxon1.ID = "BF120";
            lomTaxon1.Add_Entry("Work_History", "en");
            lomTaxon1.Add_Entry("Historie", "nl");
            LOM_Taxon lomTaxon2 = new LOM_Taxon();
            lomTaxonPath1.Add_Taxon(lomTaxon2);
            lomTaxon2.ID = "BF120.1";
            lomTaxon2.Add_Entry("American Work_History", "en");
            LOM_Taxon lomTaxon3 = new LOM_Taxon();
            lomTaxonPath1.Add_Taxon(lomTaxon3);
            lomTaxon3.ID = "BF120.1.4";
            lomTaxon3.Add_Entry("American Civil War", "en");

            LOM_Classification lomClassification2 = new LOM_Classification();
            lomInfo.Add_Classification(lomClassification2);
            lomClassification2.Purpose.Value = "Educational Objective";

            LOM_TaxonPath lomTaxonPath2 = new LOM_TaxonPath();
            lomClassification2.Add_TaxonPath(lomTaxonPath2);
            lomTaxonPath2.Add_SourceName("Common Core Standards", "en");
            LOM_Taxon lomTaxon4 = new LOM_Taxon();
            lomTaxonPath2.Add_Taxon(lomTaxon4);
            lomTaxon4.ID = "CCS.Math.Content";
            LOM_Taxon lomTaxon5 = new LOM_Taxon();
            lomTaxonPath2.Add_Taxon(lomTaxon5);
            lomTaxon5.ID = "3";
            lomTaxon5.Add_Entry("Grade 3", "en");
            LOM_Taxon lomTaxon6 = new LOM_Taxon();
            lomTaxonPath2.Add_Taxon(lomTaxon6);
            lomTaxon6.ID = "OA";
            lomTaxon6.Add_Entry("Operations and Algebraic Thinking", "en");
            LOM_Taxon lomTaxon7 = new LOM_Taxon();
            lomTaxonPath2.Add_Taxon(lomTaxon7);
            lomTaxon7.ID = "A";
            lomTaxon7.Add_Entry("Represent and solve problems involving multiplication and division.", "en");
            LOM_Taxon lomTaxon8 = new LOM_Taxon();
            lomTaxonPath2.Add_Taxon(lomTaxon8);
            lomTaxon8.ID = "3";
            lomTaxon8.Add_Entry("Use multiplication and division within 100 to solve word problems in situations involving equal groups, arrays, and measurement quantities, e.g., by using drawings and equations with a symbol for the unknown number to represent the problem.", "en");

            LOM_TaxonPath lomTaxonPath3 = new LOM_TaxonPath();
            lomClassification2.Add_TaxonPath(lomTaxonPath3);
            lomTaxonPath3.Add_SourceName("Common Core Standards", "en");
            LOM_Taxon lomTaxon14 = new LOM_Taxon();
            lomTaxonPath3.Add_Taxon(lomTaxon14);
            lomTaxon14.ID = "CCS.Math.Content";
            LOM_Taxon lomTaxon15 = new LOM_Taxon();
            lomTaxonPath3.Add_Taxon(lomTaxon15);
            lomTaxon15.ID = "3";
            lomTaxon15.Add_Entry("Grade 3", "en");
            LOM_Taxon lomTaxon16 = new LOM_Taxon();
            lomTaxonPath3.Add_Taxon(lomTaxon16);
            lomTaxon16.ID = "OA";
            lomTaxon16.Add_Entry("Operations and Algebraic Thinking", "en");
            LOM_Taxon lomTaxon17 = new LOM_Taxon();
            lomTaxonPath3.Add_Taxon(lomTaxon17);
            lomTaxon17.ID = "A";
            lomTaxon17.Add_Entry("Represent and solve problems involving multiplication and division.", "en");
            LOM_Taxon lomTaxon18 = new LOM_Taxon();
            lomTaxonPath3.Add_Taxon(lomTaxon18);
            lomTaxon18.ID = "4";
            lomTaxon18.Add_Entry("Determine the unknown whole number in a multiplication or division equation relating three whole numbers. For example, determine the unknown number that makes the equation true in each of the equations 8 × ? = 48, 5 = _ ÷ 3, 6 × 6 = ?", "en");


            // Add some views and interfaces
            testPackage.Behaviors.Clear_Web_Skins();
            testPackage.Behaviors.Add_Web_Skin("dLOC");
            testPackage.Behaviors.Add_Web_Skin("UFDC");
            testPackage.Behaviors.Add_View(View_Enum.JPEG2000);
            testPackage.Behaviors.Add_View(View_Enum.JPEG);
            testPackage.Behaviors.Add_View(View_Enum.RELATED_IMAGES);
            testPackage.Behaviors.Add_View(View_Enum.HTML, "Full Document", "MVS001214.html");

            // Create the chapters and pages and link them
            Division_TreeNode chapter1 = new Division_TreeNode("Chapter", "First Chapter");
            Page_TreeNode page1 = new Page_TreeNode("First Page");
            Page_TreeNode page2 = new Page_TreeNode("Page 2");
            chapter1.Nodes.Add(page1);
            chapter1.Nodes.Add(page2);
            Division_TreeNode chapter2 = new Division_TreeNode("Chapter", "Last Chapter");
            Page_TreeNode page3 = new Page_TreeNode("Page 3");
            Page_TreeNode page4 = new Page_TreeNode("Last Page");
            chapter2.Nodes.Add(page3);
            chapter2.Nodes.Add(page4);
            testPackage.Divisions.Physical_Tree.Roots.Add(chapter1);
            testPackage.Divisions.Physical_Tree.Roots.Add(chapter2);

            // Create the files
            SobekCM_File_Info file1_1 = new SobekCM_File_Info("2000626_0001.jp2", 2120, 1100);
            SobekCM_File_Info file1_2 = new SobekCM_File_Info("2000626_0001.jpg", 630, 330);
            SobekCM_File_Info file1_3 = new SobekCM_File_Info("2000626_0001.tif");
            SobekCM_File_Info file2_1 = new SobekCM_File_Info("2000626_0002.jp2", 1754, 2453);
            SobekCM_File_Info file2_2 = new SobekCM_File_Info("2000626_0002.jpg", 630, 832);
            SobekCM_File_Info file2_3 = new SobekCM_File_Info("2000626_0002.tif");
            SobekCM_File_Info file3_1 = new SobekCM_File_Info("2000626_0003.jp2", 2321, 1232);
            SobekCM_File_Info file3_2 = new SobekCM_File_Info("2000626_0003.jpg", 630, 342);
            SobekCM_File_Info file3_3 = new SobekCM_File_Info("2000626_0003.tif");
            SobekCM_File_Info file4_1 = new SobekCM_File_Info("2000626_0004.jp2", 2145, 1024);
            SobekCM_File_Info file4_2 = new SobekCM_File_Info("2000626_0004.jpg", 630, 326);
            SobekCM_File_Info file4_3 = new SobekCM_File_Info("2000626_0004.tif");

            // Link the files to the pages
            page1.Files.Add(file1_1);
            page1.Files.Add(file1_2);
            page1.Files.Add(file1_3);
            page2.Files.Add(file2_1);
            page2.Files.Add(file2_2);
            page2.Files.Add(file2_3);
            page3.Files.Add(file3_1);
            page3.Files.Add(file3_2);
            page3.Files.Add(file3_3);
            page4.Files.Add(file4_1);
            page4.Files.Add(file4_2);
            page4.Files.Add(file4_3);

            // Add the DAITSS information
            DAITSS_Info daitssInfo = new DAITSS_Info();
            daitssInfo.Account = "FTU";
            daitssInfo.SubAccount = "CLAS";
            daitssInfo.Project = "UFDC";
            daitssInfo.toArchive = true;
            testPackage.Add_Metadata_Module(GlobalVar.DAITSS_METADATA_MODULE_KEY, daitssInfo);

            PALMM_Info palmmInfo = new PALMM_Info();
            testPackage.Add_Metadata_Module("PALMM", palmmInfo);
            palmmInfo.toPALMM = false;

            // Save this package
            testPackage.Source_Directory = directory;
            return testPackage;
        }
    }
}