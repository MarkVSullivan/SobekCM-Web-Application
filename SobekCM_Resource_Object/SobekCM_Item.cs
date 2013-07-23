#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.MARC;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.OAI;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Tracking;
using SobekCM.Resource_Object.Utilities;

#endregion

namespace SobekCM.Resource_Object
{
    /// <summary> Main object represents a single digital resource including all the metadata, divisions, and files. </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class SobekCM_Item : MetadataDescribableBase
    {
        // Variables hold all the main information about a digital resource
        private METS_Header_Info metsInfo;
        private Division_Info divInfo;        
        private Behaviors_Info behaviorInfo;
        private Tracking_Info trackingInfo;
        private Web_Info web;

        private int total_order;
        private double size_mb;
 //       private string main_thumbnail;

        // Flags used while editing a resource in the online SobekCM digital repository 
        private bool using_complex_template;
        private bool analyzed_for_complex_content;
        private bool contains_complex_content;

        // Variables used to store finalized and validation errors
        private string validationErrors = String.Empty;
        private string finalizeErrors = String.Empty;

        /// <summary> Constructor for a new instance of the SobekCM_Item class </summary>
        public SobekCM_Item()
        {
            metsInfo = new METS_Header_Info();
            behaviorInfo = new Behaviors_Info();
            divInfo = new Division_Info();
            bibInfo = new Bibliographic_Info();
            web = new Web_Info(behaviorInfo);
            analyzed_for_complex_content = false;
            contains_complex_content = false;
            using_complex_template = false;
            size_mb = 0;
  //          main_thumbnail = String.Empty;
            
            // If there is a metadata configuration which calls for a metadata
            // extension module to always be used, add it now
            if (Metadata_Configuration.Metadata_Modules_To_Include.Count > 0)
            {
                foreach (Additional_Metadata_Module_Config thisConfig in Metadata_Configuration.Metadata_Modules_To_Include)
                {
                    iMetadata_Module toInclude = thisConfig.Get_Module();
                    if (toInclude != null)
                    {
                        Add_Metadata_Module( thisConfig.Key, toInclude );
                    }
                }
            }
        }

        /// <summary> Constructor for a new instance of the SobekCM_Item class, which imports values from a OAI record </summary>
        public SobekCM_Item(OAI_Repository_DublinCore_Record OAI_Record)
        {
            metsInfo = new METS_Header_Info();
            behaviorInfo = new Behaviors_Info();
            divInfo = new Division_Info();
            bibInfo = new Bibliographic_Info();
            web = new Web_Info(behaviorInfo);
            analyzed_for_complex_content = false;
            contains_complex_content = false;
            using_complex_template = false;
            size_mb = 0;

            // Copy over all the data
            if (OAI_Record.hasCreators)
            {
                foreach (string thisCreator in OAI_Record.Creators)
                {
                    Bib_Info.Add_Named_Entity(thisCreator);
                }
            }
            if (OAI_Record.hasContributors)
            {
                foreach (string thisContributor in OAI_Record.Contributors)
                {
                    Bib_Info.Add_Named_Entity(thisContributor, "Contributor");
                }
            }
            if (OAI_Record.hasCoverages)
            {
                foreach (string thisCoverage in OAI_Record.Coverages)
                {
                    Subject_Info_Standard thisSubject = new Subject_Info_Standard();
                    thisSubject.Add_Geographic(thisCoverage);
                    Bib_Info.Add_Subject(thisSubject);
                }
            }
            if (OAI_Record.hasDates)
            {
                foreach (string thisDate in OAI_Record.Dates)
                {
                    Bib_Info.Origin_Info.Date_Issued = thisDate;
                }
            }
            if (OAI_Record.hasDescriptions)
            {
                foreach (string thisDescription in OAI_Record.Descriptions)
                {
                    Bib_Info.Add_Note(thisDescription);
                }
            }
            if (OAI_Record.hasFormats)
            {
                foreach (string thisFormat in OAI_Record.Formats)
                {
                    Bib_Info.Original_Description.Extent = thisFormat;
                }
            }
            if (OAI_Record.hasIdentifiers)
            {
                foreach (string thisIdentifier in OAI_Record.Identifiers)
                {
                    if (thisIdentifier.IndexOf("http://") == 0)
                    {
                        Bib_Info.Location.Other_URL = thisIdentifier;
                    }
                    else
                    {
                        Bib_Info.Add_Identifier(thisIdentifier);
                    }
                }
            }
            if (OAI_Record.hasLanguages)
            {
                foreach (string thisLanguage in OAI_Record.Languages)
                {
                    Bib_Info.Add_Language(thisLanguage);
                }
            }
            if (OAI_Record.hasPublishers)
            {
                foreach (string thisPublisher in OAI_Record.Publishers)
                {
                    Bib_Info.Add_Publisher(thisPublisher);
                }
            }
            if (OAI_Record.hasRelations)
            {
                foreach (string thisRelation in OAI_Record.Relations)
                {
                    Related_Item_Info newRelatedItem = new Related_Item_Info();
                    newRelatedItem.Main_Title.Title = thisRelation;
                    Bib_Info.Add_Related_Item(newRelatedItem);
                }
            }
            if (OAI_Record.hasRights)
            {
                foreach (string thisRights in OAI_Record.Rights)
                {
                    Bib_Info.Access_Condition.Text = thisRights;
                }
            }
            //if (OAI_Record.hasSources)
            //{
            //    foreach (string thisSource in OAI_Record.Sources)
            //    {
            //        this.Bib_Info.Source.Statement = r.Value.Trim() = thisSource;
            //    }
            //}
            if (OAI_Record.hasSubjects)
            {
                foreach (string thisSubject in OAI_Record.Subjects)
                {
                    if (thisSubject.IndexOf(";") > 0)
                    {
                        string[] splitter = thisSubject.Split(";".ToCharArray());
                        foreach (string thisSplit in splitter)
                        {
                            Bib_Info.Add_Subject(thisSplit.Trim(), String.Empty);
                        }
                    }
                    else
                    {
                        Bib_Info.Add_Subject(thisSubject, String.Empty);
                    }
                }
            }
            if (OAI_Record.hasTitles)
            {
                foreach (string thistitle in OAI_Record.Titles)
                {
                    if (Bib_Info.Main_Title.ToString().Length > 0)
                    {
                        Bib_Info.Add_Other_Title(thistitle, Title_Type_Enum.alternative);
                    }
                    else
                    {
                        Bib_Info.Main_Title.Clear();
                        Bib_Info.Main_Title.Title = thistitle;
                    }
                }
            }
            if (OAI_Record.hasTypes)
            {
                foreach (string thisType in OAI_Record.Types)
                {
                    Bib_Info.SobekCM_Type_String = thisType;
                }
            }

            // If there is a metadata configuration which calls for a metadata
            // extension module to always be used, add it now
            if (Metadata_Configuration.Metadata_Modules_To_Include.Count > 0)
            {
                foreach (Additional_Metadata_Module_Config thisConfig in Metadata_Configuration.Metadata_Modules_To_Include)
                {
                    iMetadata_Module toInclude = thisConfig.Get_Module();
                    if (toInclude != null)
                    {
                        Add_Metadata_Module(thisConfig.Key, toInclude);
                    }
                }
            }
        }

        /// <summary> Size of the entire package on disk </summary>
        public double DiskSize_MB
        {
            get { return size_mb; }
            set { size_mb = value; }
        }


        /// <summary> Gets a flag that indicates if the data in this item contains complex content
        /// which appears to be derived from a MARC record or otherwise have faceted elements
        /// which would be lost in a simple template.</summary>
        public bool Contains_Complex_Content
        {
            get
            {
                if (!analyzed_for_complex_content)
                {
                    if (bibInfo.EncodingLevel.Length > 0)
                    {
                        contains_complex_content = true;
                        analyzed_for_complex_content = true;
                        return true;
                    }

                    if (bibInfo.Subjects_Count > 0)
                    {
                        foreach (Subject_Info thisSubject in bibInfo.Subjects)
                        {
                            if (thisSubject.Class_Type == Subject_Info_Type.TitleInfo)
                            {
                                contains_complex_content = true;
                                analyzed_for_complex_content = true;
                                return true;
                            }

                            if (thisSubject.Class_Type == Subject_Info_Type.Name)
                            {
                                contains_complex_content = true;
                                analyzed_for_complex_content = true;
                                return true;
                            }

                            if (thisSubject.Class_Type == Subject_Info_Type.Standard)
                            {
                                Subject_Info_Standard standSubject = (Subject_Info_Standard) thisSubject;
                                if ((standSubject.Topics_Count > 1) || (standSubject.Genres_Count > 0) || (standSubject.Geographics_Count > 0) || (standSubject.Occupations_Count > 0) || (standSubject.Temporals_Count > 0))
                                {
                                    contains_complex_content = true;
                                    analyzed_for_complex_content = true;
                                    return true;
                                }
                            }

                            if (thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                            {
                                Subject_Info_HierarchicalGeographic hieroSubject = (Subject_Info_HierarchicalGeographic) thisSubject;
                                if ((hieroSubject.Continent.Length > 0) || (hieroSubject.Country.Length > 0) || (hieroSubject.State.Length > 0) || (hieroSubject.County.Length > 0) || (hieroSubject.City.Length > 0) || (hieroSubject.Island.Length > 0))
                                {
                                    contains_complex_content = true;
                                    analyzed_for_complex_content = true;
                                    return true;
                                }
                            }
                        }
                    }

                    if ((Bib_Info.hasMainEntityName) && (Bib_Info.Main_Entity_Name.Roles.Count > 1) || (Bib_Info.Main_Entity_Name.Terms_Of_Address.Length > 0) || (Bib_Info.Main_Entity_Name.Dates.Length > 0) || (Bib_Info.Main_Entity_Name.Affiliation.Length > 0) || (Bib_Info.Main_Entity_Name.Description.Length > 0) || (Bib_Info.Main_Entity_Name.Display_Form.Length > 0) || (Bib_Info.Main_Entity_Name.Family_Name.Length > 0) || (Bib_Info.Main_Entity_Name.Given_Name.Length > 0))
                    {
                        contains_complex_content = true;
                        analyzed_for_complex_content = true;
                        return true;
                    }

                    if (bibInfo.Names_Count > 0)
                    {
                        if (bibInfo.Names.Any(thisName => (thisName.Roles.Count > 1) || (thisName.Terms_Of_Address.Length > 0) || (thisName.Dates.Length > 0) || (thisName.Description.Length > 0) || (thisName.Display_Form.Length > 0) || (thisName.Family_Name.Length > 0) || (thisName.Given_Name.Length > 0)))
                        {
                            contains_complex_content = true;
                            analyzed_for_complex_content = true;
                            return true;
                        }
                    }

                    if ((bibInfo.Main_Title.Authority.Length > 0) || (bibInfo.Main_Title.Display_Label.Length > 0) || (bibInfo.Main_Title.NonSort.Length > 0) || (bibInfo.Main_Title.Part_Names_Count > 0) || (bibInfo.Main_Title.Part_Numbers_Count > 0) || (bibInfo.Main_Title.Subtitle.Length > 0))
                    {
                        contains_complex_content = true;
                        analyzed_for_complex_content = true;
                        return true;
                    }

                    if (bibInfo.Other_Titles_Count > 0)
                    {
                        if (bibInfo.Other_Titles.Any(thisTitle => (thisTitle.Authority.Length > 0) || (thisTitle.Display_Label.Length > 0) || (thisTitle.NonSort.Length > 0) || (thisTitle.Part_Names_Count > 0) || (thisTitle.Part_Numbers_Count > 0) || (thisTitle.Subtitle.Length > 0)))
                        {
                            contains_complex_content = true;
                            analyzed_for_complex_content = true;
                            return true;
                        }
                    }

                    analyzed_for_complex_content = true;
                    contains_complex_content = false;
                }

                return contains_complex_content;
            }
        }

        /// <summary> If this item is user-specific, this stores whether the user has requested to use
        /// the more complex template when editing the item's data </summary>
        public bool Using_Complex_Template
        {
            get { return using_complex_template; }
            set { using_complex_template = value; }
        }

        /// <summary> Gets the appropriate title to use as the bib-level title </summary>
        public string Bib_Title
        {
            get
            {
                if (behaviorInfo.GroupTitle.Length > 0)
                    return behaviorInfo.GroupTitle;

                if (bibInfo.SobekCM_Type == TypeOfResource_SobekCM_Enum.Newspaper)
                {
                    if (bibInfo.Other_Titles_Count > 0)
                    {
                        foreach (Title_Info otherTitle in bibInfo.Other_Titles)
                        {
                            if (otherTitle.Title_Type == Title_Type_Enum.uniform)
                            {
                                return otherTitle.ToString();
                            }
                        }
                    }
                    if ((bibInfo.hasSeriesTitle) && (bibInfo.SeriesTitle.ToString().Length > 0))
                        return bibInfo.SeriesTitle.ToString();
                }
                else
                {
                    if ((bibInfo.hasSeriesTitle) && (bibInfo.SeriesTitle.ToString().Length > 0))
                        return bibInfo.SeriesTitle.ToString();
                    if (bibInfo.Other_Titles_Count > 0)
                    {
                        foreach (Title_Info otherTitle in bibInfo.Other_Titles)
                        {
                            if (otherTitle.Title_Type == Title_Type_Enum.uniform)
                            {
                                return otherTitle.ToString();
                            }
                        }
                    }
                }
                return bibInfo.Main_Title.ToString();
            }
        }


        /// <summary> Sets the (P)URL to be included in the METS file </summary>
        /// <param name="SobekCM_URL"> SobekCM URL for this material </param>
        public void Set_PURL(string SobekCM_URL)
        {
            Bib_Info.Location.PURL = SobekCM_URL + "?b=" + bibInfo.BibID + "&amp;v=" + bibInfo.VID.Replace("VID", "");
        }

        /// <summary> Saves the data stored in this instance of the 
        /// element to the provided bibliographic object </summary>
        /// <param name="serialHierarchyObject">Serial hierarchy object to set to this object </param>
        /// <param name="SerialInfo"> Serial information to set to this object </param>
        public void Set_Serial_Info(Part_Info serialHierarchyObject, Serial_Info SerialInfo)
        {
            behaviorInfo.Set_Serial_Info(SerialInfo);
            Bib_Info.Series_Part_Info = serialHierarchyObject;
        }


        #region Public Properties

        /// <summary> Gets flag which indicates if there is any tracking information in this object </summary>
        public bool hasTrackingInformation
        {
            get {
                return trackingInfo != null;
            }
        }

        /// <summary> Gets the tracking information object which contains status and history information </summary>
        public Tracking_Info Tracking
        {
            get { return trackingInfo ?? (trackingInfo = new Tracking_Info()); }
        }

        /// <summary> Gets the behavior information about how this item behaves in a digital repository </summary>
        public Behaviors_Info Behaviors
        {
            get { return behaviorInfo; }
        }

        /// <summary> Gets the information used by the SobekCM web application during rendering </summary>
        public Web_Info Web
        {
            get { return web; }
        }

        /// <summary> Gets and sets the source directory, where all the files can be found </summary>
        public string Source_Directory
        {
            get { return divInfo.Source_Directory; }
            set { divInfo.Source_Directory = value; }
        }

        /// <summary> Gets and sets the division and file information for this resource </summary>
        public Division_Info Divisions
        {
            get { return divInfo; }
            set { divInfo = value; }
        }

        /// <summary> Gets the METS header specific information associated with this resource </summary>
        public METS_Header_Info METS_Header
        {
            get { return metsInfo; }
        }

        /// <summary> Gets and sets the Bibliographic Identifier (BibID) associated with this resource </summary>
        public string BibID
        {
            get { return bibInfo.BibID; }
            set
            {
                web.BibID = value.ToUpper();
                bibInfo.BibID = value.ToUpper();

                if (value.Length > 0)
                {
                    if (bibInfo.VID.Length > 0)
                    {
                        METS_Header.ObjectID = bibInfo.BibID + "_" + bibInfo.VID;
                    }
                    else
                    {
                        METS_Header.ObjectID = bibInfo.BibID;
                    }
                }
            }
        }

        /// <summary> Gets and sets the Volume Identifier (VID) associated with this resource </summary>
        public string VID
        {
            get { return bibInfo.VID; }
            set
            {
                web.VID = value;
                bibInfo.VID = value;

                if (value.Length > 0)
                {
                    METS_Header.ObjectID = bibInfo.BibID + "_" + bibInfo.VID;
                }
            }
        }

        ///// <summary> Gets and sets the flag indicating this resource is image class </summary>
        //public bool ImageClass
        //{
        //    get { return imageClass; }
        //    set { imageClass = value; }
        //}

        #endregion

        #region Methods to validate a METS package

        /// <summary> Checks that each file in the package is present in the directory. </summary>
        /// <param name="thisPackage">Package to check</param>
        /// <param name="matchCheckSums">Flag indicates whether to match checksums</param>
        /// <returns>TRUE if everything validates, otherwise FALSE </returns>
        public static bool Validate_Files(SobekCM_Item thisPackage, bool matchCheckSums)
        {
            // Build the METS validator and validate the files
            SobekCM_METS_Validator validator = new SobekCM_METS_Validator(thisPackage);
            return validator.Check_Files(thisPackage.Source_Directory, matchCheckSums);
        }

        /// <summary> Checks that each file in the package is present in the directory. </summary>
        /// <param name="matchCheckSums">Flag indicates whether to match checksums</param>
        /// <returns>TRUE if everything validates, otherwise FALSE </returns>
        /// <remarks> This sets the Validation_Errors property of this object </remarks>
        public bool Validate_Files(bool matchCheckSums)
        {
            // Build the METS validator and validate the files
            SobekCM_METS_Validator validator = new SobekCM_METS_Validator(this);
            bool returnVal = validator.Check_Files(Source_Directory, matchCheckSums);

            // Save the validation errors
            validationErrors = returnVal ? String.Empty : validator.ValidationError;

            return returnVal;
        }

        /// <summary> Validates this METS file against the XML Schema </summary>
        /// <param name="thisPackage">Package to validate</param>
        /// <returns>TRUE if succesful, otherwise FALSE </returns>
        public static bool Validate_Against_Schema(SobekCM_Item thisPackage)
        {
            // Build the METS validator object
            METS_Validator_Object validator = new METS_Validator_Object(false);

            // Get the METS file
            string[] files_xml = Directory.GetFiles(thisPackage.Source_Directory, "*.METS_Header.xml");
            string[] files = Directory.GetFiles(thisPackage.Source_Directory, "*.mets");

            // Check if there is one
            if ((files.Length <= 0) && (files_xml.Length <= 0))
            {
                return false;
            }
            return validator.Validate_Against_Schema(files.Length > 0 ? files[0] : files_xml[0]);
        }

        /// <summary> Validates this METS file against the XML Schema </summary>
        /// <returns>TRUE if succesful, otherwise FALSE </returns>
        /// <remarks> This sets the Validation_Errors property of this object </remarks>
        public bool Validate_Against_Schema()
        {
            // Build the METS validator object
            METS_Validator_Object validator = new METS_Validator_Object(false);

            // Get the METS file
            string[] files_xml = Directory.GetFiles(Source_Directory, "*.METS_Header.xml");
            string[] files = Directory.GetFiles(Source_Directory, "*.mets");

            // Check if there is one
            if ((files.Length > 0) || (files_xml.Length > 0))
            {
                bool returnVal = validator.Validate_Against_Schema(files.Length > 0 ? files[0] : files_xml[0]);
                validationErrors = validator.Errors;
                return returnVal;
            }
            
            validationErrors = "METS File Does Not Exist";
            return false;
        }

        #endregion

        #region Method to finalize the METS package

        /// <summary> Gets any errors which occurred during Finalizing </summary>
        public string Finalize_Errors
        {
            get { return finalizeErrors; }
        }

        /// <summary> Gets any errors which occurred during Validation </summary>
        public string Validation_Errors
        {
            get { return validationErrors; }
        }

        /// <summary> Performs some general clean-up functions while finalizing METS files </summary>
        public void General_Cleanup()
        {
            // Make sure 'other' is not listed as a subject keyword scheme
            if (Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in Bib_Info.Subjects)
                {
                    if (thisSubject.Authority.ToUpper().Trim() == "OTHER")
                    {
                        thisSubject.Authority = String.Empty;
                    }
                }
            }

            // Get rid of 'VID' in vid
            if (VID.ToUpper().IndexOf("VID") == 0)
            {
                VID = VID.Substring(3);
            }

            // Get rid of 'VID' in object id
            if (METS_Header.ObjectID.IndexOf("VID") > 0)
            {
                METS_Header.ObjectID = METS_Header.ObjectID.Replace("VID", String.Empty);
            }

            // Make sure no repeats in aggregation
            List<string> subs = new List<string>();
            foreach (Aggregation_Info aggregation in behaviorInfo.Aggregations)
            {
                string thisSub = aggregation.Code;
                if (!subs.Contains(thisSub))
                    subs.Add(thisSub);
            }
            behaviorInfo.Clear_Aggregations();
            foreach (string thisSub in subs)
            {
                behaviorInfo.Add_Aggregation(thisSub);
            }
        }

        /// <summary> Ensure that each page has a name.  If no pages were named, they are just named
        /// from 'Page 1' up.  If some pages are named, any missing pages are named by the division 
        /// type in which they reside. </summary>
        public void Name_All_Pages()
        {
            // Get the collection of all pages
            List<abstract_TreeNode> pageNodes = divInfo.Physical_Tree.Pages_PreOrder;

            // See if any of the pages are named
            bool anyNames = pageNodes.Any(thisNode => thisNode.Label.Length > 0);

            // Name the pages differently, depending on whether or not any pages were named
            if (anyNames)
            {
                // Create names for each page that does not have a name, using the division
                // as the template
                total_order = 1;
                List<abstract_TreeNode> visitedNodes = new List<abstract_TreeNode>();
                foreach (abstract_TreeNode thisNode in Divisions.Physical_Tree.Roots)
                {
                    if (!thisNode.Page)
                    {
                        Division_TreeNode divNode = (Division_TreeNode) thisNode;
                        int order = 1;
                        foreach (abstract_TreeNode subNode in divNode.Nodes)
                        {
                            recursively_add_page_names(subNode, divNode.Type, order++, divNode.Nodes.Count, visitedNodes);
                        }
                    }
                }
            }
            else
            {
                // Should we use 'Page', or something else?
                string precursor = "Page ";
                switch (Bib_Info.SobekCM_Type)
                {
                    case TypeOfResource_SobekCM_Enum.Photograph:
                        precursor = "Photograph #";
                        break;

                    case TypeOfResource_SobekCM_Enum.Map:
                        precursor = "Map Sheet #";
                        break;

                    case TypeOfResource_SobekCM_Enum.Artifact:
                        precursor = "Still Image #";
                        break;

                    case TypeOfResource_SobekCM_Enum.Aerial:
                        precursor = "Tile #";
                        break;
                }

                // No pages have names at all, so just name 'Page 1', 'Page 2', etc...
                int pageNum = 1;
                foreach (abstract_TreeNode thisNode in Divisions.Physical_Tree.Divisions_PreOrder)
                {
                    if ((thisNode.Page) && (thisNode.Label.Length == 0))
                    {
                        thisNode.Label = precursor + pageNum;
                        pageNum++;
                    }
                }
            }
        }

        private void recursively_add_page_names(abstract_TreeNode thisNode, string parentType, int Order, int Sibling_Count, List<abstract_TreeNode> visitedNodes)
        {
            if (thisNode.Page)
            {
                if (thisNode.Label.Length == 0)
                {
                    if ((parentType.ToUpper().IndexOf("CHAPTER") < 0) && (parentType.ToUpper().IndexOf("SUB") != 0))
                    {
                        if (Sibling_Count != 1)
                        {
                            thisNode.Label = parentType + " " + Order;
                        }
                        else
                        {
                            thisNode.Label = parentType;
                        }
                    }
                    else
                    {
                        thisNode.Label = "Unnumbered ( " + total_order + " )";
                    }
                }

                // Move along the total order counter
                if (!visitedNodes.Contains(thisNode))
                {
                    total_order++;
                    visitedNodes.Add(thisNode);
                }
            }
            else
            {
                Division_TreeNode divNode = (Division_TreeNode) thisNode;
                int new_order = 1;
                foreach (abstract_TreeNode subNode in divNode.Nodes)
                {
                    recursively_add_page_names(subNode, divNode.Type, new_order++, divNode.Nodes.Count, visitedNodes);
                }
            }
        }

        /// <summary> Finalize this METS file for the lsat writing before loading to SobekCM </summary>
        /// <param name="calculateAllChecksums">Flag indicates if all checksums should be recalculated</param>
        /// <returns>TRUE if successful, otherwise FALSE</returns>
        public bool Finalize_METS(bool calculateAllChecksums)
        {
            try
            {
                // Build the collections to hold all the file information
                Dictionary<string, Page_TreeNode> allFiles = new Dictionary<string, Page_TreeNode>();
                Dictionary<string, SobekCM_File_Info> jpegFiles = new Dictionary<string, SobekCM_File_Info>();
                Dictionary<string, SobekCM_File_Info> tiffFiles = new Dictionary<string, SobekCM_File_Info>();
                Dictionary<string, SobekCM_File_Info> jp2Files = new Dictionary<string, SobekCM_File_Info>();
                Dictionary<string, SobekCM_File_Info> textFiles = new Dictionary<string, SobekCM_File_Info>();
                Dictionary<string, SobekCM_File_Info> proFiles = new Dictionary<string, SobekCM_File_Info>();

                // Step through each file in the division section of the METS
                string name;
                List<abstract_TreeNode> packagePages = Divisions.Physical_Tree.Pages_PreOrder;
                foreach (Page_TreeNode thisPage in packagePages)
                {
                    foreach (SobekCM_File_Info thisFile in thisPage.Files)
                    {
                        // Make sure the name is long enough
                        if (thisFile.System_Name.Length < 1)
                        {
                            throw new Exception("FILE NAME IN METS IS NOT VALID");
                        }

                        // Calculate the name
                        name = thisFile.File_Name_Sans_Extension;
                        string fullname = thisFile.System_Name.ToUpper();

                        // Is this a text file?
                        if (fullname.IndexOf(".TXT") > 0)
                        {
                            textFiles[name] = thisFile;

                            // Always recalculate the checksums for the text files
                            thisFile.Checksum = String.Empty;
                            thisFile.Size = -1;
                        }

                        // Is this a jpeg file?
                        if (fullname.IndexOf(".JPG") > 0)
                        {
                            jpegFiles[name] = thisFile;
                        }

                        // Is this a TIFF file?
                        if (fullname.IndexOf(".TIF") > 0)
                        {
                            tiffFiles[name] = thisFile;
                        }

                        // Is this a jpeg2000 file?
                        if (fullname.IndexOf(".JP2") > 0)
                        {
                            jp2Files[name] = thisFile;
                        }

                        // Is this a PRO file?
                        if (fullname.IndexOf(".PRO") > 0)
                        {
                            proFiles[name] = thisFile;
                        }

                        // Add the page by name
                        allFiles[name] = thisPage;

                        // Should all files have their checksum cleared?
                        if (calculateAllChecksums)
                        {
                            thisFile.Checksum = String.Empty;
                            thisFile.Size = -1;
                        }
                    }
                }

                // Now, step through each text file in the METS package directory
                string[] textFilesInDir = Directory.GetFiles(Source_Directory, "*.txt");
                string shortName;
                foreach (string thisTextFile in textFilesInDir)
                {
                    // Get the name
                    name = (new FileInfo(thisTextFile).Name);
                    shortName = File_Name_Sans_Extension(name);

                    // Was there any link to this file name, and no text in the METS?
                    if ((allFiles.ContainsKey(shortName)) && (!textFiles.ContainsKey(shortName)))
                    {
                        // Create the new file object
                        SobekCM_File_Info thisFile = new SobekCM_File_Info(name);

                        // Add to the page
                        allFiles[shortName].Files.Add(thisFile);
                    }
                }

                // Now, step through each PRO file in the METS package directory
                string[] proFilesInDir = Directory.GetFiles(Source_Directory, "*.pro");
                foreach (string thisProFile in proFilesInDir)
                {
                    // Get the name
                    name = (new FileInfo(thisProFile).Name);
                    shortName = File_Name_Sans_Extension(name);

                    // Was there any link to this file name, and no PRO in the METS?
                    if ((allFiles.ContainsKey(shortName)) && (!proFiles.ContainsKey(shortName)))
                    {
                        // Create the new file object
                        SobekCM_File_Info thisFile = new SobekCM_File_Info(name);

                        // Add to the page
                        allFiles[shortName].Files.Add(thisFile);
                    }
                }

                // Now, step through each jpeg file in the METS package directory
                bool allHaveThumbs = true;
                textFilesInDir = Directory.GetFiles(Source_Directory, "*.jpg");
                foreach (string thisTextFile in textFilesInDir)
                {
                    if ((thisTextFile.ToUpper().IndexOf("QC.JPG") < 0) && (thisTextFile.ToUpper().IndexOf("QC2.JPG") < 0) && (thisTextFile.ToUpper().IndexOf("THM.JPG") < 0))
                    {
                        // Get the name
                        name = (new FileInfo(thisTextFile).Name);
                        shortName = File_Name_Sans_Extension(name);

                        // Does a JPEG Thumbnail exist as well?
                        if ((allHaveThumbs) && (!File.Exists(Source_Directory + "\\" + name.Replace(".jpg", "thm.jpg"))))
                        {
                            allHaveThumbs = false;
                        }

                        // Was there any link to this file name, and no jpeg in the METS?
                        if ((allFiles.ContainsKey(shortName)) && (!jpegFiles.ContainsKey(shortName)))
                        {
                            // Create the new file object
                            SobekCM_File_Info thisFile = new SobekCM_File_Info(name);

                            // Add to the page
                            allFiles[shortName].Files.Add(thisFile);
                        }
                    }
                }

                // Set the related image, if they all had thumbnails
                if (allHaveThumbs)
                {
                    bool relatedAlreadyThere = Behaviors.Views.Any(thisView => thisView.View_Type == View_Enum.RELATED_IMAGES);

                    if (!relatedAlreadyThere)
                    {
                        Behaviors.Add_View(View_Enum.RELATED_IMAGES);
                    }
                }

                // Now, step through each jpeg2000 file in the METS package directory
                allHaveThumbs = true;
                textFilesInDir = Directory.GetFiles(Source_Directory, "*.jp2");
                foreach (string thisTextFile in textFilesInDir)
                {
                    // Get the name
                    name = (new FileInfo(thisTextFile).Name);
                    shortName = File_Name_Sans_Extension(name);

                    // Does a JPEG Thumbnail exist as well?
                    if ((allHaveThumbs) && (!File.Exists(Source_Directory + "\\" + name.Replace(".jp2", "thm.jpg"))))
                    {
                        allHaveThumbs = false;
                    }

                    // Was there any link to this file name, and no jpeg2000 in the METS?
                    if ((allFiles.ContainsKey(shortName)) && (!jp2Files.ContainsKey(shortName)))
                    {
                        // Create the new file object
                        SobekCM_File_Info thisFile = new SobekCM_File_Info(name);

                        // Add to the page
                        allFiles[shortName].Files.Add(thisFile);
                    }
                }


                // Set the related image, if they all had thumbnails
                if (allHaveThumbs)
                {
                    bool relatedAlreadyThere2 = Behaviors.Views.Any(thisView => thisView.View_Type == View_Enum.RELATED_IMAGES);

                    if (!relatedAlreadyThere2)
                    {
                        Behaviors.Add_View(View_Enum.RELATED_IMAGES);
                    }
                }

                //// If there is one thumbnail, assign it
                //int fileid = 999999;
                //string[] thumbs = Directory.GetFiles( Source_Directory, "*thm.jpg" );
                //if ( thumbs.Length >= 1 )
                //{
                //    string thumbname = ( new FileInfo( thumbs[0] )).Name;

                //    // Assign this file to the main thumbnail metadata field
                //    this.procParam.Main_Thumbnail = thumbname;

                //    Divisions.SobekCM_File_Info thumbFile = this.Divisions.File_By_System_Name(thumbname);

                //    // Also make sure this is in the file section
                //    if ( thumbFile == null )
                //    {
                //        this.Divisions.Add_File( fileid.ToString(), thumbname, String.Empty );
                //        fileid--;
                //    }
                //}

                // If this is destined for PALMM, and there is no PALMM Project, just copy 
                // over the SobekCM Primary code
                PALMM_Info palmmInfo = Get_Metadata_Module("PALMM") as PALMM_Info;
                if (palmmInfo != null)
                {
                    if ((palmmInfo.toPALMM) && (palmmInfo.PALMM_Project.Length == 0) && (behaviorInfo.Aggregation_Count > 0))
                        palmmInfo.PALMM_Project = behaviorInfo.Aggregations[0].Code;
                }

                // Save the old METS
                string[] metsFiles = Directory.GetFiles(Source_Directory, "*.mets*");
                if (metsFiles.Length == 1)
                {
                    try
                    {
                        File.Move(metsFiles[0], metsFiles[0] + ".bak");
                    }
                    catch
                    {
                    }
                }

                // Now, Write the METS file again
                Save_METS();

                // Finally, validate it
                bool returnValue = Validate_Against_Schema();
                if (!returnValue)
                {
                    finalizeErrors = validationErrors;
                }
                else
                {
                    finalizeErrors = String.Empty;
                }
                return returnValue;
            }
            catch (Exception ee)
            {
                finalizeErrors = ee.ToString();
                return false;
            }
        }

        private string File_Name_Sans_Extension(string systemname)
        {
            if (String.IsNullOrEmpty(systemname))
                return string.Empty;
            int first_period_index = systemname.IndexOf('.');
            if (first_period_index < 0)
                return systemname.ToUpper();
            if (first_period_index + 1 >= systemname.Length)
                return systemname.ToUpper().Substring(0, systemname.Length - 1);
            return systemname.Substring(0, first_period_index).ToUpper();
        }

        /// <summary> Adds the license information, used for the dLOC Toolkit which uses license codes to
        /// control the source information in the METS file and allow FTP </summary>
        /// <param name="License"> License key </param>
        public void Set_License(string License)
        {
            behaviorInfo.Calculate_GUID(Bib_Info.BibID, License);
        }

        #endregion

        #region Methods to read data from MXF, METS, SGML, INFO, etc..

        /// <summary> Read from an existing metadata file into the current object</summary>
        /// <param name="fileName">Name of the file</param>
        /// <remarks>The generic reader attempts to determine which form of metadata the file
        /// is currently using, and selects the appropriate reader</remarks>
        public void Read(string fileName)
        {
            Generic_Reader reader = new Generic_Reader();
            reader.Read(fileName, this);
        }

        /// <summary> Read from an existing metadata file and returns a new object</summary>
        /// <param name="fileName">Name of the file</param>
        /// <remarks>The generic reader attempts to determine which form of metadata the file
        /// is currently using, and selects the appropriate reader</remarks>
        /// <returns>Built SobekCM_Item object</returns>
        public static SobekCM_Item Read_File(string fileName)
        {
            SobekCM_Item returnVal = new SobekCM_Item();
            Generic_Reader reader = new Generic_Reader();
            reader.Read(fileName, returnVal);
            return returnVal;
        }

        /// <summary> Read from an existing metadata file and returns a new object </summary>
        /// <param name="directory">Directory of the resource package</param>
        /// <remarks>The generic reader attempts to determine which form of metadata the file
        /// is currently using, and selects the appropriate reader</remarks>
        /// <returns>Built SobekCM_Item object</returns>
        public static SobekCM_Item Read_Directory(string directory)
        {
            Generic_Reader reader = new Generic_Reader();
            return reader.Read_Directory(directory);
        }

        /// <summary> Read from an existing MXF file into the current object</summary>
        /// <param name="fileName">Name of the MXF file</param>
        public bool Read_From_MXF(string fileName)
        {
            string errorMessage;

            MXF_File_ReaderWriter reader = new MXF_File_ReaderWriter();
            return reader.Read_Metadata(fileName, this, null, out errorMessage);
        }

        /// <summary> Read from an existing MXF file and returns a new object</summary>
        /// <param name="fileName">Name of the MXF file</param>
        /// <returns>Built SobekCM_Item object</returns>
        public static SobekCM_Item Read_MXF(string fileName)
        {
            SobekCM_Item returnVal = new SobekCM_Item();
            string errorMessage;

            MXF_File_ReaderWriter reader = new MXF_File_ReaderWriter();
            reader.Read_Metadata(fileName, returnVal, null, out errorMessage);
            return returnVal;
        }

        /// <summary> Read from an existing METS file into the current object</summary>
        /// <param name="fileName">Name of the METS file</param>
        public void Read_From_METS(string fileName)
        {
            // Save this to the METS file format
            METS_File_ReaderWriter writer = new METS_File_ReaderWriter();
            string Error_Message;
            writer.Read_Metadata(fileName, this, null, out Error_Message);
        }

        /// <summary> Read from an existing METS file and returns a new object</summary>
        /// <param name="fileName">Name of the METS file</param>
        /// <returns>Built SobekCM_Item object</returns>
        public static SobekCM_Item Read_METS(string fileName)
        {
            SobekCM_Item returnVal = new SobekCM_Item();

            // Save this to the METS file format
            METS_File_ReaderWriter writer = new METS_File_ReaderWriter();
            string Error_Message;
            writer.Read_Metadata(fileName, returnVal, null, out Error_Message);

            return returnVal;
        }


        /// <summary> Reads the divisions from an existing INFO file into the current object </summary>
        /// <param name="fileName">Name of the INFO file</param>
        public bool Read_Divisions_From_INFO(string fileName)
        {
            string errorMessage;
            INFO_File_ReaderWriter reader = new INFO_File_ReaderWriter();
            return reader.Read_Metadata(fileName, this, null, out errorMessage);
        }

        /// <summary> Reads an existing INFO file and returns a new object </summary>
        /// <param name="fileName">Name of the INFO file</param>
        /// <returns>Built SobekCM_Item object</returns>
        public static SobekCM_Item Read_INFO(string fileName)
        {
            SobekCM_Item returnVal = new SobekCM_Item();

            string errorMessage;
            INFO_File_ReaderWriter reader = new INFO_File_ReaderWriter();
            reader.Read_Metadata(fileName, returnVal, null, out errorMessage);

            return returnVal;
        }

        /// <summary> Reads the bibliographic information from an existing MARC XML file into the current object </summary>
        /// <param name="fileName">Name of the MARC XML file</param>
        public bool Read_From_MARC_XML(string fileName)
        {
            string errorMessage;
            MarcXML_File_ReaderWriter reader = new MarcXML_File_ReaderWriter();
            return reader.Read_Metadata(fileName, this, null, out errorMessage);
        }

        /// <summary> Reads the bibliographic information from an existing MARC XML file and returns a new object </summary>
        /// <param name="fileName">Name of the MARC XML file</param>
        /// <returns>Built SobekCM_Item object</returns>
        public static SobekCM_Item Read_MARC_XML(string fileName)
        {
            SobekCM_Item returnVal = new SobekCM_Item();
            string errorMessage;
            MarcXML_File_ReaderWriter reader = new MarcXML_File_ReaderWriter();
            reader.Read_Metadata(fileName, returnVal, null, out errorMessage);
            return returnVal;
        }

        #endregion

        #region Methods to write data to METS, etc...

        /// <summary> Save this resource as a METS file </summary>
        /// <remarks>Will use either the default namespace, or the same namespace which was used
        /// in any previously read METS files.</remarks>
        public void Save_METS(bool include_daitss)
        {
            METS_Header.ObjectID = BibID + "_" + VID;

            string FileName = Source_Directory + "/" + BibID + "_" + VID + ".mets";

            // If this is juat a project XML, do this differently
            if (Bib_Info.SobekCM_Type == TypeOfResource_SobekCM_Enum.Project)
            {
                Bib_Info.Main_Title.Title = "Project level metadata for '" + BibID + "'";
                FileName = Source_Directory + "/" + BibID + ".pmets";
                METS_Header.ObjectID = BibID;
            }

            // If there is an interview date, use that to calculate the sort safe date
            // Ensure this metadata module extension exists and has data
            Oral_Interview_Info oralInfo = Get_Metadata_Module("OralInterview") as Oral_Interview_Info;
            if ((oralInfo != null) && (oralInfo.Interview_Date.Length > 0) && (bibInfo.SortDate > 0))
            {
                bibInfo.SortDate = bibInfo.sortSafeDate(oralInfo.Interview_Date);
            }

            // Save this to the METS file format
            METS_File_ReaderWriter writer = new METS_File_ReaderWriter();
            string Error_Message;
            writer.Write_Metadata(FileName, this, null, out Error_Message);
        }

        /// <summary> Save this resource as a METS file </summary>
        /// <remarks>Will use either the default namespace, or the same namespace which was used
        /// in any previously read METS files.</remarks>
        public void Save_METS()
        {
            Save_METS(false);
        }

        /// <summary> Save this resource as a METS file </summary>
        /// <remarks>Will use either the default namespace, or the same namespace which was used
        /// in any previously read METS files.</remarks>
        public void Save_METS(string Destination_File)
        {
            METS_Header.ObjectID = BibID + "_" + VID;

            // If there is an interview date, use that to calculate the sort safe date
            // Ensure this metadata module extension exists and has data
            Oral_Interview_Info oralInfo = Get_Metadata_Module("OralInterview") as Oral_Interview_Info;
            if ((oralInfo != null) && (oralInfo.Interview_Date.Length > 0) && (bibInfo.SortDate > 0))
            {
                bibInfo.SortDate = bibInfo.sortSafeDate(oralInfo.Interview_Date);
            }

            // Save this to the METS file format
            METS_File_ReaderWriter writer = new METS_File_ReaderWriter();
            string Error_Message;
            writer.Write_Metadata(Destination_File, this, null, out Error_Message);
        }

        /// <summary> Save this resource as a METS file destined for FCLA's Florida Digital Archive </summary>
        /// <param name="Destination_File"> Name of the resultant METS file </param>
        public void Save_FCLA_METS(string Destination_File)
        {
            // Save this to the METS file format
            METS_Header.ObjectID = BibID + "_" + VID;

            // If there is an interview date, use that to calculate the sort safe date
            // Ensure this metadata module extension exists and has data
            Oral_Interview_Info oralInfo = Get_Metadata_Module("OralInterview") as Oral_Interview_Info;
            if ((oralInfo != null) && (oralInfo.Interview_Date.Length > 0) && (bibInfo.SortDate > 0))
            {
                bibInfo.SortDate = bibInfo.sortSafeDate(oralInfo.Interview_Date);
            }

            // Save this to the METS file format
            METS_File_ReaderWriter writer = new METS_File_ReaderWriter();
            string Error_Message;
            writer.Write_Metadata(Destination_File, this, null, out Error_Message);
        }

        /// <summary> Saves this resource as a SobekCM Service METS file </summary>
        /// <remarks> The SobekCM Service METS file tries to keep the file size as small as possible and also includes service information (image properties) that the standard SobekCM METS file does not include </remarks>
        public void Save_SobekCM_METS()
        {
            // Save this to the METS file format
            METS_File_ReaderWriter writer = new METS_File_ReaderWriter();
            string Error_Message;
            writer.Write_Metadata( Source_Directory + "\\" + BibID + "_" + VID + ".mets.xml", this, null, out Error_Message);
        }

        /// <summary> Save this resource as a bib_level METS file </summary>
        /// <remarks>This is used to save the Bib level mets file from a volume level mets file</remarks>
        public void Save_Bib_Level_METS(string directory)
        {
            // Save the old values
            //string save_vid = VID;
            //METS_Record_Status oldStatus = metsInfo.RecordStatus_Enum;
            //string thumbnail = Behaviors.Main_Thumbnail;
            //string date_issued = Bib_Info.MODS_Origin_Info.Date_Issued;
            //Part_Info thisPartInfo = Bib_Info.Series_Part_Info.Copy();
            //Bib_Info.Series_Part_Info.Clear();
            //if (bibInfo.SobekCM_Type == TypeOfResource_SobekCM_Enum.Newspaper)
            //{
            //    Bib_Info.MODS_Origin_Info.Date_Issued = String.Empty;
            //}

            //// Set new values
            //VID = "*****";
            //metsInfo.RecordStatus_Enum = METS_Record_Status.BIB_LEVEL;
            //METS_Header.ObjectID = BibID;

            //// Save this to the METS file format
            //METS_Writer.Save_SobekCM_MODS_METS(directory + "/" + BibID + ".xml", this, true, false);

            //// Restore the old values
            //VID = save_vid;
            //metsInfo.RecordStatus_Enum = oldStatus;
            //METS_Header.ObjectID = BibID + "_" + VID;
            //Bib_Info.MODS_Origin_Info.Date_Issued = date_issued;
            //Behaviors.Main_Thumbnail = thumbnail;
            //Bib_Info.Series_Part_Info = thisPartInfo;
        }

        /// <summary> Save this resource as a bib_level METS file </summary>
        /// <remarks>This is used to save the Bib level mets file from a volume level mets file</remarks>
        public void Save_Citation_Only_METS()
        {
            //METS_Header.ObjectID = BibID + "_" + VID;

            //// If there is an interview date, use that to calculate the sort safe date
            //// Ensure this metadata module extension exists and has data
            //Oral_Interview_Info oralInfo = Get_Metadata_Module("OralInterview") as Oral_Interview_Info;
            //if ((oralInfo != null) && (oralInfo.Interview_Date.Length > 0) && (bibInfo.SortDate > 0))
            //{
            //    bibInfo.SortDate = bibInfo.sortSafeDate(oralInfo.Interview_Date);
            //}

            //// Set the main page information here
            //if (behaviorInfo.Main_Page.FileName.Length == 0)
            //{
            //    // First try to find the page matchig the thumbnail
            //    string thumb_deriv = String.Empty;
            //    if (behaviorInfo.Main_Thumbnail.Length > 0)
            //    {
            //        thumb_deriv = behaviorInfo.Main_Thumbnail.Replace("thm", "");
            //    }

            //    bool previous_page = false;
            //    bool found_page = false;
            //    string first_page_file = String.Empty;
            //    string first_page_name = String.Empty;
            //    int page_count = 0;

            //    List<abstract_TreeNode> divisions = Divisions.Physical_Tree.Divisions_PreOrder;
            //    behaviorInfo.Main_Page.Next_Page_Exists = false;
            //    Page_TreeNode pageNode;
            //    foreach (abstract_TreeNode thisNode in divisions)
            //    {
            //        if (thisNode.Page)
            //        {
            //            page_count++;

            //            if (found_page)
            //            {
            //                behaviorInfo.Main_Page.Next_Page_Exists = true;
            //            }

            //            pageNode = (Page_TreeNode) thisNode;
            //            foreach (SobekCM_File_Info thisFile in pageNode.Files)
            //            {
            //                if ((thumb_deriv.Length > 0) && (thisFile.System_Name == thumb_deriv))
            //                {
            //                    behaviorInfo.Main_Page.FileName = thisFile.System_Name;
            //                    behaviorInfo.Main_Page.PageName = pageNode.Label;
            //                    behaviorInfo.Main_Page.Previous_Page_Exists = previous_page;
            //                    found_page = true;
            //                }

            //                if (thisFile.System_Name.IndexOf(".jpg") > 0)
            //                {
            //                    first_page_file = thisFile.System_Name;
            //                    first_page_name = pageNode.Label;
            //                }
            //            }

            //            previous_page = true;
            //        }

            //        // Stop early if the page has been found and the next page was also found
            //        if ((found_page) && (behaviorInfo.Main_Page.Next_Page_Exists))
            //        {
            //            break;
            //        }
            //    }

            //    // Was the thumbnail maatched up?
            //    if (behaviorInfo.Main_Page.FileName.Length == 0)
            //    {
            //        behaviorInfo.Main_Page.FileName = first_page_file;
            //        behaviorInfo.Main_Page.PageName = first_page_name;
            //        behaviorInfo.Main_Page.Previous_Page_Exists = false;
            //        if (page_count > 1)
            //        {
            //            behaviorInfo.Main_Page.Next_Page_Exists = true;
            //        }
            //        else
            //        {
            //            behaviorInfo.Main_Page.Next_Page_Exists = false;
            //        }
            //    }
            //}

            //// Save this to the METS file format
            //METS_Writer.Save_SobekCM_MODS_METS(Source_Directory + "/citation_mets.xml", this, true, false);
        }

        /// <summary> Writes all the data about this item in MARC-ish HTML for display online</summary>
        /// <param name="Collections"> List of the names of the collections linked to this item </param>
        /// <param name="Include_Endeca_Tags"> Flag indicates whether to include tags specific to Florida's implementation of Endeca </param>
        /// <param name="System_Name"> Name of the host system, which is added into the MARC record </param>
        /// <param name="System_Abbreviation"> Abbreviation for the host system, which is added into the MARC record </param>
        /// <returns> Complete HTML as a string, ready for display </returns>
        public string Get_MARC_HTML(List<string> Collections, bool Include_Endeca_Tags, string System_Name, string System_Abbreviation)
        {
            MARC_HTML_Writer marcHtmlWriter = new MARC_HTML_Writer();
            return marcHtmlWriter.MARC_HTML(this, MARC_Sobek_Standard_Tags(Collections, Include_Endeca_Tags, System_Name, System_Abbreviation));
        }

        /// <summary> Writes all the data about this item in MARC-ish HTML for display online</summary>
        /// <param name="Collections"> List of the names of the collections linked to this item </param>
        /// <param name="Include_Endeca_Tags"> Flag indicates whether to include tags specific to Florida's implementation of Endeca </param>
        /// <param name="Width"> Width value for use within the rendered tables </param>
        /// <param name="System_Name"> Name of the host system, which is added into the MARC record </param>
        /// <param name="System_Abbreviation"> Abbreviation for the host system, which is added into the MARC record </param>
        /// <returns> Complete HTML as a string, ready for display </returns>
        public string Get_MARC_HTML(List<string> Collections, bool Include_Endeca_Tags, string Width, string System_Name, string System_Abbreviation)
        {
            MARC_HTML_Writer marcHtmlWriter = new MARC_HTML_Writer();
            return marcHtmlWriter.MARC_HTML(this, Width, MARC_Sobek_Standard_Tags(Collections, Include_Endeca_Tags, System_Name, System_Abbreviation));
        }

        #endregion

        #region Method to retrieve this item as a MARC record 

        /// <summary> Gets the collection of MARC tags to be written for this digital resource </summary>
        /// <value> Collection of MARC tags to be written for this digital resource </value>
        public MARC_Record To_MARC_Record()
        {
            // Create the sorted list
            MARC_Record tags = new MARC_Record();

            // Compute the sobekcm type, which will be used for some of these mappings
            TypeOfResource_SobekCM_Enum sobekcm_type = bibInfo.SobekCM_Type;

            // Build a hashtable of all the pertinent genres
            Dictionary<string, string> genreHash = new Dictionary<string, string>();
            if (Bib_Info.Genres_Count > 0)
            {
                foreach (Genre_Info thisGenre in Bib_Info.Genres)
                {
                    if ((thisGenre.Authority == "marcgt") || (thisGenre.Authority == "sobekcm"))
                    {
                        genreHash[thisGenre.Genre_Term] = thisGenre.Genre_Term;
                    }
                }
            }

            // ADD THE 006 FOR ONLINE MATERIAL
            StringBuilder bldr006 = new StringBuilder("m     o  ");
            switch (Bib_Info.SobekCM_Type)
            {
                case TypeOfResource_SobekCM_Enum.Book:
                case TypeOfResource_SobekCM_Enum.Newspaper:
                case TypeOfResource_SobekCM_Enum.Multivolume:
                case TypeOfResource_SobekCM_Enum.Serial:
                    bldr006.Append("d");
                    break;

                case TypeOfResource_SobekCM_Enum.Video:
                case TypeOfResource_SobekCM_Enum.Audio:
                    bldr006.Append("i");
                    break;

                case TypeOfResource_SobekCM_Enum.Photograph:
                case TypeOfResource_SobekCM_Enum.Map:
                case TypeOfResource_SobekCM_Enum.Aerial:
                    bldr006.Append("c");
                    break;

                case TypeOfResource_SobekCM_Enum.Archival:
                    bldr006.Append("m");
                    break;

                default:
                    bldr006.Append(" ");
                    break;
            }
            bldr006.Append(" ");

            // Government publication?
            int govt_publication = 0;
            add_value(genreHash.ContainsKey("multilocal government publication"), 'c', ref govt_publication, 1, bldr006);
            add_value(genreHash.ContainsKey("federal government publication"), 'f', ref govt_publication, 1, bldr006);
            add_value(genreHash.ContainsKey("international intergovernmental publication"), 'i', ref govt_publication, 1, bldr006);
            add_value(genreHash.ContainsKey("local government publication"), 'l', ref govt_publication, 1, bldr006);
            add_value(genreHash.ContainsKey("multistate government publication"), 'm', ref govt_publication, 1, bldr006);
            add_value(genreHash.ContainsKey("government publication"), 'o', ref govt_publication, 1, bldr006);
            add_value(genreHash.ContainsKey("government publication (state, provincial, terriorial, dependent)"), 's', ref govt_publication, 1, bldr006);
            add_value(genreHash.ContainsKey("government publication (autonomous or semiautonomous component)"), 'a', ref govt_publication, 1, bldr006);
            if (govt_publication == 0)
            {
                bldr006.Append(" ");
            }
            bldr006.Append("      ");
            tags.Add_Field(6, "  ", bldr006.ToString());

            // ADD THE 007 FOR ELECTRONIC RESOURCE
            StringBuilder bldr007 = new StringBuilder("cr  n");
            switch (Bib_Info.SobekCM_Type)
            {
                case TypeOfResource_SobekCM_Enum.Audio:
                case TypeOfResource_SobekCM_Enum.Video:
                    bldr007.Append("a");
                    break;

                default:
                    bldr007.Append(" ");
                    break;
            }
            bldr007.Append("---ma mp");
            tags.Add_Field(new MARC_Field(7, "  ", bldr007.ToString()));

            // ADD THE MAIN ENTITY NAME
            if ((Bib_Info.hasMainEntityName) && (Bib_Info.Main_Entity_Name.Full_Name.Length > 0))
            {
                MARC_Field main_entity_marc = Bib_Info.Main_Entity_Name.to_MARC_HTML(false);

                switch (Bib_Info.Main_Entity_Name.Name_Type)
                {
                    case Name_Info_Type_Enum.personal:
                        main_entity_marc.Tag = 100;
                        break;

                    case Name_Info_Type_Enum.corporate:
                        main_entity_marc.Tag = 110;
                        break;

                    case Name_Info_Type_Enum.conference:
                        main_entity_marc.Tag = 111;
                        break;

                    case Name_Info_Type_Enum.UNKNOWN:
                        main_entity_marc.Tag = 720;
                        break;
                }
                tags.Add_Field(main_entity_marc);
            }

            // ADD THE OTHER NAMES
            if (Bib_Info.Names_Count > 0)
            {
                foreach (Name_Info name in Bib_Info.Names)
                {
                    if (name.Full_Name.Length > 0)
                    {
                        MARC_Field name_marc = name.to_MARC_HTML(false);

                        switch (name.Name_Type)
                        {
                            case Name_Info_Type_Enum.personal:
                                name_marc.Tag = 700;
                                break;

                            case Name_Info_Type_Enum.corporate:
                                name_marc.Tag = 710;
                                break;

                            case Name_Info_Type_Enum.conference:
                                name_marc.Tag = 711;
                                break;

                            case Name_Info_Type_Enum.UNKNOWN:
                                name_marc.Tag = 720;
                                break;
                        }

                        tags.Add_Field(name_marc);
                    }
                }
            }

            // ADD THE DONOR
            if ((Bib_Info.hasDonor) && (Bib_Info.Donor.Full_Name.Length > 0))
            {
                MARC_Field donor_marc = Bib_Info.Donor.to_MARC_HTML(false);

                switch (Bib_Info.Donor.Name_Type)
                {
                    case Name_Info_Type_Enum.personal:
                    case Name_Info_Type_Enum.UNKNOWN:
                        donor_marc.Indicators = donor_marc.Indicators[0] + "3";
                        donor_marc.Tag = 796;
                        tags.Add_Field(donor_marc);
                        break;

                    case Name_Info_Type_Enum.corporate:
                    case Name_Info_Type_Enum.conference:
                        donor_marc.Indicators = donor_marc.Indicators[0] + "3";
                        donor_marc.Tag = 797;
                        tags.Add_Field(donor_marc);
                        break;
                }
            }

            // ADD THE 260
            if ((Bib_Info.Origin_Info.Publishers_Count > 0) || (Bib_Info.Origin_Info.Date_Issued.Length > 0) || (Bib_Info.Origin_Info.MARC_DateIssued.Length > 0))
            {
                MARC_Field publisher_tag = new MARC_Field();
                StringBuilder builder260 = new StringBuilder();
                publisher_tag.Tag = 260;
                publisher_tag.Indicators = "  ";
                int pub_count = 0;
                if (Bib_Info.Publishers_Count > 0)
                {
                    foreach (Publisher_Info thisPublisher in Bib_Info.Publishers)
                    {
                        int place_count = 1;
                        bool place_added = false;
                        // ADD ALL THE |a 's for this publisher
                        if (thisPublisher.Places_Count > 0)
                        {
                            foreach (Origin_Info_Place thisPlace in thisPublisher.Places)
                            {
                                if (thisPlace.Place_Text.Length > 0)
                                {
                                    place_added = true;
                                    if (place_count > 1)
                                    {
                                        builder260.Append("; |a " + thisPlace.Place_Text + " ");
                                    }
                                    else
                                    {
                                        builder260.Append("|a " + thisPlace.Place_Text + " ");
                                    }
                                    place_count++;
                                }
                            }
                        }
                        if (!place_added)
                        {
                            builder260.Append("|a [S.l.] ");
                        }
                        builder260.Append(": ");

                        // Add the |b for this publisher
                        string pubName = thisPublisher.Name.Trim();
                        if ((pubName.Length > 2) && (pubName[pubName.Length - 1] == ','))
                        {
                            pubName = pubName.Substring(0, pubName.Length - 1);
                        }

                        builder260.Append("|b " + thisPublisher.Name);
                        pub_count++;

                        if (pub_count == Bib_Info.Publishers_Count)
                        {
                            if ((Bib_Info.Origin_Info.Date_Issued.Length > 0) || (Bib_Info.Manufacturers_Count > 0))
                            {
                                builder260.Append(", ");
                            }
                        }
                        else
                        {
                            builder260.Append(" ; ");
                        }
                    }
                }

                if (Bib_Info.Origin_Info.MARC_DateIssued.Length > 0)
                {
                    builder260.Append("|c " + Bib_Info.Origin_Info.MARC_DateIssued);
                }
                else if (Bib_Info.Origin_Info.Date_Issued.Length > 0)
                {
                    builder260.Append("|c " + Bib_Info.Origin_Info.Date_Issued);
                }

                if (Bib_Info.Manufacturers_Count > 0)
                {
                    foreach (Publisher_Info thisManufacturer in Bib_Info.Manufacturers)
                    {
                        int place_count = 1;
                        bool place_added = false;
                        // ADD ALL THE |e 's for this manufacturer
                        if (thisManufacturer.Places_Count > 0)
                        {
                            foreach (Origin_Info_Place thisPlace in thisManufacturer.Places)
                            {
                                if (thisPlace.Place_Text.Length > 0)
                                {
                                    place_added = true;
                                    if (place_count > 1)
                                    {
                                        builder260.Append("; |e " + thisPlace.Place_Text + " ");
                                    }
                                    else
                                    {
                                        builder260.Append(" |e (" + thisPlace.Place_Text + " ");
                                    }
                                    place_count++;
                                }
                            }
                        }
                        if (!place_added)
                        {
                            builder260.Append("|e ( [S.l.] ");
                        }
                        builder260.Append(": ");

                        // Add the |f for this manufacturer
                        builder260.Append("|f " + thisManufacturer.Name + ")");
                    }
                }
                else
                {
                    builder260.Append(".");
                }

                publisher_tag.Control_Field_Value = builder260.ToString();
                tags.Add_Field(publisher_tag);
            }

            // ADD ALL THE FREQUENCIES
            if (Bib_Info.Origin_Info.Frequencies_Count > 0)
            {
                foreach (Origin_Info_Frequency frequency in Bib_Info.Origin_Info.Frequencies)
                {
                    if (frequency.Authority != "marcfrequency")
                    {
                        MARC_Field frequency_tag = new MARC_Field {Tag = 310, Indicators = "  "};

                        if (frequency.Term.IndexOf("[") < 0)
                        {
                            frequency_tag.Control_Field_Value = "|a " + frequency.Term;
                        }
                        else
                        {
                            if (frequency.Term.IndexOf("[ FORMER") > 0)
                            {
                                frequency_tag.Tag = 321;
                                int square_bracket_index = frequency.Term.IndexOf("[");
                                frequency_tag.Control_Field_Value = "|a " + frequency.Term.Substring(0, square_bracket_index).Trim() + " |b " + frequency.Term.Substring(square_bracket_index + 9).Replace("]", "").Trim();
                            }
                            else
                            {
                                int square_bracket_index2 = frequency.Term.IndexOf("[");
                                frequency_tag.Control_Field_Value = "|a " + frequency.Term.Substring(0, square_bracket_index2).Trim() + " |b " + frequency.Term.Substring(square_bracket_index2 + 1).Replace("]", "").Trim();
                            }
                        }
                        tags.Add_Field(frequency_tag);
                    }
                }
            }

            // Determine if there is a link to another version of this material
            bool another_version_exists = false;
            if (Bib_Info.RelatedItems_Count > 0)
            {
                if (Bib_Info.RelatedItems.Where(relatedItem => relatedItem.URL.Length > 0).Any(relatedItem => relatedItem.Relationship == Related_Item_Type_Enum.otherVersion))
                {
                    another_version_exists = true;
                }
            }


            // ADD ALL THE NOTES
            string statement_of_responsibility = "";
            string electronic_access_note = "";
            if (Bib_Info.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in Bib_Info.Notes)
                {
                    switch (thisNote.Note_Type)
                    {
                        case Note_Type_Enum.statement_of_responsibility:
                            statement_of_responsibility = thisNote.Note;
                            break;

                        case Note_Type_Enum.electronic_access:
                            electronic_access_note = thisNote.Note;
                            break;

                        case Note_Type_Enum.publication_status:
                        case Note_Type_Enum.internal_comments:
                            // DO nothing 
                            break;

                        default:
                            tags.Add_Field(thisNote.to_MARC_HTML());
                            break;
                    }
                }
            }

            // Add an 856 pointing to eitther ufdc or dloc first
            MARC_Field tag856 = new MARC_Field {Tag = 856, Indicators = "40"};
            string url = Bib_Info.Location.PURL;
            if (url.Length == 0)
            {
                url = web.Service_URL;
            }
            string linkText = "Electronic Resource";
            if ((Bib_Info.Type.MODS_Type == TypeOfResource_MODS_Enum.Text))
                linkText = "Click here for full text";
            if (another_version_exists)
            {
                if (electronic_access_note.Length > 0)
                    tag856.Control_Field_Value = "|3 UFDC Version |u " + url + " |y " + linkText + " |z " + electronic_access_note;
                else
                    tag856.Control_Field_Value = "|3 UFDC Version |u " + url + " |y " + linkText;
            }
            else
            {
                if (electronic_access_note.Length > 0)
                    tag856.Control_Field_Value = "|u " + url + " |y " + linkText + " |z " + electronic_access_note;
                else
                    tag856.Control_Field_Value = "|u " + url + " |y " + linkText;
            }
            tags.Add_Field(tag856);

            // ADD THE RELATED ITEMS
            if (Bib_Info.RelatedItems_Count > 0)
            {
                foreach (Related_Item_Info relatedItem in Bib_Info.RelatedItems)
                {
                    // Add the main tag
                    tags.Add_Field(relatedItem.to_MARC_HTML());

                    // If there is a URL listed, add another tag
                    if (relatedItem.URL.Length > 0)
                    {
                        MARC_Field linking_tag = new MARC_Field {Tag = 856, Indicators = "42"};
                        if (relatedItem.Relationship == Related_Item_Type_Enum.otherVersion)
                            linking_tag.Indicators = "41";

                        StringBuilder linking856_builder = new StringBuilder();
                        if (relatedItem.URL_Display_Label.Length > 0)
                        {
                            linking856_builder.Append("|3 " + XML_Writing_Base_Type.Convert_String_To_XML_Safe_Static(relatedItem.URL_Display_Label) + " ");
                        }
                        else
                        {
                            switch (relatedItem.Relationship)
                            {
                                case Related_Item_Type_Enum.host:
                                    linking856_builder.Append("|3 Host material ");
                                    break;

                                case Related_Item_Type_Enum.otherFormat:
                                    linking856_builder.Append("|3 Other format ");
                                    break;

                                case Related_Item_Type_Enum.otherVersion:
                                    linking856_builder.Append("|3 Other version ");
                                    break;

                                case Related_Item_Type_Enum.preceding:
                                    linking856_builder.Append("|3 Preceded by ");
                                    break;

                                case Related_Item_Type_Enum.succeeding:
                                    linking856_builder.Append("|3 Succeeded by ");
                                    break;

                                default:
                                    linking856_builder.Append("|3 Related item ");
                                    break;
                            }
                        }

                        // Add the URL
                        linking856_builder.Append("|u " + XML_Writing_Base_Type.Convert_String_To_XML_Safe_Static(relatedItem.URL) + " ");

                        // Add the title if there is one
                        if ((relatedItem.hasMainTitle) && (relatedItem.Main_Title.Title.Length > 0))
                        {
                            linking856_builder.Append("|y " + XML_Writing_Base_Type.Convert_String_To_XML_Safe_Static(relatedItem.Main_Title.Title));
                        }

                        linking_tag.Control_Field_Value = linking856_builder.ToString().Trim();
                        tags.Add_Field(linking_tag);
                    }
                }
            }

            // ADD THE TARGET AUDIENCE 
            string skipped_marctarget = String.Empty;
            bool done_with_skipped = false;
            if (Bib_Info.Target_Audiences_Count > 0)
            {
                foreach (TargetAudience_Info targetAudience in Bib_Info.Target_Audiences)
                {
                    if (targetAudience.Authority.Length > 0)
                    {
                        // This part is kinda wierd.  basically, one marctarget audience can be included in the 
                        // leader, so it does not need to be included in the 521 as well.  But, if there is 
                        // more than one "marctarget" audience, I want ALL of them to show up here.
                        if (targetAudience.Authority == "marctarget")
                        {
                            if (skipped_marctarget.Length == 0)
                            {
                                skipped_marctarget = targetAudience.Audience;
                            }
                            else
                            {
                                if (!done_with_skipped)
                                {
                                    tags.Add_Field(521, "  ", "|a " + skipped_marctarget + " |b marctarget");
                                    done_with_skipped = true;
                                }

                                tags.Add_Field(521, "  ", "|a " + targetAudience.Audience + " |b " + targetAudience.Authority);
                            }
                        }
                        else
                        {
                            tags.Add_Field(521, "  ", "|a " + targetAudience.Audience + " |b " + targetAudience.Authority);
                        }
                    }
                    else
                    {
                        tags.Add_Field(521, "  ", "|a " + targetAudience.Audience);
                    }
                }
            }

            //// ADD THE ORIGINAL CATALOGING SOURCE INFORMATION
            //if (this.Bib_Info.Record.MARC_Record_Content_Sources.Count > 0)
            //{
            //    MARC_XML_Field origCatSource = new MARC_XML_Field();
            //    StringBuilder builder599 = new StringBuilder();
            //    origCatSource.Tag = "599";
            //    origCatSource.Indicators = "  ";
            //    builder599.Append("|a " + this.Bib_Info.Record.MARC_Record_Content_Sources[0]);
            //    if (this.Bib_Info.Record.MARC_Record_Content_Sources.Count > 1)
            //    {
            //        builder599.Append(" |c " + this.Bib_Info.Record.MARC_Record_Content_Sources[1]);
            //    }
            //    int orig_count = 2;
            //    while (orig_count < this.Bib_Info.Record.MARC_Record_Content_Sources.Count)
            //    {
            //        builder599.Append(" |d " + this.Bib_Info.Record.MARC_Record_Content_Sources[orig_count]);
            //        orig_count++;
            //    }
            //    origCatSource.Field = builder599.ToString();
            //    tags.Add_Field(origCatSource);
            //}

            // Add the NEW cataloging source information
            MARC_Field catSource = new MARC_Field {Tag = 40, Indicators = "  ", Control_Field_Value = "|a FUG |c FUG"};
            if (bibInfo.Record.Description_Standard.Length > 0)
                catSource.Control_Field_Value = catSource.Control_Field_Value + " |e " + bibInfo.Record.Description_Standard.ToLower();
            tags.Add_Field(catSource);


            // ADD THE ABSTRACTS
            if (Bib_Info.Abstracts_Count > 0)
            {
                foreach (Abstract_Info thisAbstract in Bib_Info.Abstracts)
                {
                    tags.Add_Field(thisAbstract.to_MARC_HTML());
                }
            }

            // ADD THE MAIN TITLE
            if (Bib_Info.Main_Title.Title.Length > 0)
            {
                MARC_Field mainTitleTag = Bib_Info.Main_Title.to_MARC_HTML(245, 2, statement_of_responsibility, "[electronic resource]");
                if ((Bib_Info.hasMainEntityName) && (Bib_Info.Main_Entity_Name.Full_Name.Length > 0))
                    mainTitleTag.Indicators = "1" + mainTitleTag.Indicators[1];
                else
                    mainTitleTag.Indicators = "0" + mainTitleTag.Indicators[1];

                tags.Add_Field(mainTitleTag);
            }

            // ADD THE SERIES TITLE
            if ((Bib_Info.hasSeriesTitle) && (Bib_Info.SeriesTitle.Title.Length > 0))
            {
                tags.Add_Field(Bib_Info.SeriesTitle.to_MARC_HTML(490, 0, String.Empty, String.Empty));
            }

            // ADD ALL OTHER TITLES
            if (Bib_Info.Other_Titles_Count > 0)
            {
                foreach (Title_Info thisTitle in Bib_Info.Other_Titles)
                {
                    tags.Add_Field(thisTitle.to_MARC_HTML(-1, -1, String.Empty, String.Empty));
                }
            }

            // ADD THE EXTENT
            if (Bib_Info.Original_Description.Extent.Length > 0)
            {
                int semi_index = Bib_Info.Original_Description.Extent.IndexOf(";");
                int colon_index = Bib_Info.Original_Description.Extent.IndexOf(":");
                if ((semi_index > 0) || (colon_index > 0))
                {
                    string a_300_subfield_string;
                    string b_300_subfield_string = String.Empty;
                    string c_300_subfield_string = String.Empty;

                    if (semi_index > 0)
                    {
                        if (colon_index > 0)
                        {
                            a_300_subfield_string = Bib_Info.Original_Description.Extent.Substring(0, colon_index);
                            b_300_subfield_string = Bib_Info.Original_Description.Extent.Substring(colon_index + 1, semi_index - colon_index - 1);
                            c_300_subfield_string = Bib_Info.Original_Description.Extent.Substring(semi_index + 1);

                            //tags.Add_Field("300", "  ", "|a " + this.Bib_Info.Original_Description.Extent.Substring(0, colon_index + 1) + " |b " + this.Bib_Info.Original_Description.Extent.Substring(colon_index + 1, semi_index - colon_index) + " |c " + this.Bib_Info.Original_Description.Extent.Substring(semi_index + 1));
                        }
                        else
                        {
                            a_300_subfield_string = Bib_Info.Original_Description.Extent.Substring(0, semi_index);
                            c_300_subfield_string = Bib_Info.Original_Description.Extent.Substring(semi_index + 1);

                            // No colon ( colon_index < 0 )
                            //tags.Add_Field("300", "  ", "|a " + this.Bib_Info.Original_Description.Extent.Substring(0, semi_index + 1) + " |c " + this.Bib_Info.Original_Description.Extent.Substring(semi_index + 1));
                        }
                    }
                    else
                    {
                        a_300_subfield_string = Bib_Info.Original_Description.Extent.Substring(0, colon_index);
                        b_300_subfield_string = Bib_Info.Original_Description.Extent.Substring(colon_index + 1);

                        // No semi colon ( semi_index < 0 )
                        //tags.Add_Field("300", "  ", "|a " + this.Bib_Info.Original_Description.Extent.Substring(0, colon_index + 1) + " |b " + this.Bib_Info.Original_Description.Extent.Substring(colon_index + 1));
                    }

                    StringBuilder builder_300 = new StringBuilder("|a " + a_300_subfield_string, a_300_subfield_string.Length + b_300_subfield_string.Length + c_300_subfield_string.Length + 10);
                    if (b_300_subfield_string.Trim().Length > 0)
                    {
                        builder_300.Append(": |b " + b_300_subfield_string);
                    }
                    if (c_300_subfield_string.Trim().Length > 0)
                    {
                        builder_300.Append("; |c " + c_300_subfield_string);
                    }
                    tags.Add_Field(300, "  ", builder_300.ToString());
                }
                else
                {
                    tags.Add_Field(300, "  ", "|a " + Bib_Info.Original_Description.Extent);
                }
            }

            // ADD THE EDITION
            if (Bib_Info.Origin_Info.Edition.Length > 0)
            {
                if (Bib_Info.Origin_Info.Edition.IndexOf(" -- ") > 0)
                {
                    int edition_index = Bib_Info.Origin_Info.Edition.IndexOf(" -- ");
                    tags.Add_Field(250, "  ", "|a " + Bib_Info.Origin_Info.Edition.Substring(0, edition_index) + " /|b " + Bib_Info.Origin_Info.Edition.Substring(edition_index + 4));
                }
                else
                {
                    tags.Add_Field(250, "  ", "|a " + Bib_Info.Origin_Info.Edition);
                }
            }

            // ADD THE CLASSIFICATIONS
            if (Bib_Info.Classifications_Count > 0)
            {
                foreach (Classification_Info thisClassification in Bib_Info.Classifications)
                {
                    tags.Add_Field(thisClassification.to_MARC_Tag());
                }
            }


            // ADD ALL THE IDENTIFIERS
            if (Bib_Info.Identifiers_Count > 0)
            {
                foreach (Identifier_Info thisIdentifier in Bib_Info.Identifiers)
                {
                    switch (thisIdentifier.Type.ToUpper())
                    {
                        case "LCCN":
                            tags.Add_Field(10, "  ", "|a  " + thisIdentifier.Identifier);
                            break;

                        case "ISBN":
                            tags.Add_Field(20, "  ", "|a " + thisIdentifier.Identifier);
                            break;

                        case "ISSN":
                            tags.Add_Field(22, "  ", "|a " + thisIdentifier.Identifier);
                            break;

                        case "OCLC":
                            tags.Add_Field(776, "1 ", "|c Original |w (OCoLC)" + thisIdentifier.Identifier);
                            break;

                        case "NOTIS":
                            tags.Add_Field(035, "9 ", "|a " + thisIdentifier.Identifier + " |b UF");
                            break;

                        case "ALEPH":
                            tags.Add_Field(035, "9 ", "|a " + thisIdentifier.Identifier.PadLeft(9, '0') + " |b UF");
                            break;

                        default:
                            if (thisIdentifier.Type.Length > 0)
                            {
                                tags.Add_Field(024, "7 ", "|a " + thisIdentifier.Identifier + " |2 " + thisIdentifier.Type);
                            }
                            else
                            {
                                tags.Add_Field(024, "8 ", "|a " + thisIdentifier.Identifier);
                            }
                            break;
                    }
                }
            }

            // ADD THE MAIN IDENTIFIER               
            if ((METS_Header.ObjectID.Length > 0) || (Bib_Info.Record.Main_Record_Identifier.Identifier.Length > 0) || ((BibID.Length > 0) && ((VID.Length > 0) || (METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL))))
            {
                if (Bib_Info.Record.Main_Record_Identifier.Identifier.Length > 0)
                {
                    tags.Add_Field(1, "  ", Bib_Info.Record.Main_Record_Identifier.Identifier);
                }
                else
                {
                    if (METS_Header.ObjectID.Length > 0)
                    {
                        tags.Add_Field(1, "  ", METS_Header.ObjectID);
                    }
                    else
                    {
                        if (METS_Header.RecordStatus_Enum != METS_Record_Status.BIB_LEVEL)
                            tags.Add_Field(1, "  ", BibID + "_" + VID);
                        else
                            tags.Add_Field(1, "  ", BibID);
                    }
                }
            }

            // ADD THE LAST MODIFIED DATE
            tags.Add_Field(5, "  ", METS_Header.Modify_Date.Year +
                                    METS_Header.Modify_Date.Month.ToString().PadLeft(2, '0') +
                                    METS_Header.Modify_Date.Day.ToString().PadLeft(2, '0') +
                                    METS_Header.Modify_Date.Hour.ToString().PadLeft(2, '0') +
                                    METS_Header.Modify_Date.Minute.ToString().PadLeft(2, '0') +
                                    METS_Header.Modify_Date.Second.ToString().PadLeft(2, '0') + ".0");

            // ADD THE SUBJECT KEYWORDS
            if (Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in Bib_Info.Subjects)
                {
                    tags.Add_Field(thisSubject.to_MARC_HTML());
                }
            }

            // ADD ANY NON-MARCGT GENRE TERMS AS 655 CODES AS WELL
            if (Bib_Info.Genres_Count > 0)
            {
                foreach (Genre_Info thisGenre in Bib_Info.Genres)
                {
                    tags.Add_Field(thisGenre.to_MARC_HTML());
                }
            }

            // ADD THE TEMPORAL SUBJECT (if the term appears nowehere in the subjects)
            if (Bib_Info.TemporalSubjects_Count > 0)
            {
                foreach (Temporal_Info thisTemporal in Bib_Info.TemporalSubjects)
                {
                    string temporal_upper = thisTemporal.TimePeriod.ToUpper();
                    string years = String.Empty;
                    if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year > 0))
                        years = thisTemporal.Start_Year.ToString() + "-" + thisTemporal.End_Year;
                    if ((thisTemporal.Start_Year > 0) && (thisTemporal.End_Year < 0))
                        years = thisTemporal.Start_Year.ToString() + "-";
                    if ((thisTemporal.Start_Year < 0) && (thisTemporal.End_Year > 0))
                        years = "-" + thisTemporal.End_Year;
                    bool found = false;
                    if (Bib_Info.Subjects_Count > 0)
                    {
                        foreach (Subject_Info thisSubject in Bib_Info.Subjects)
                        {
                            if (thisSubject.Class_Type == Subject_Info_Type.Standard)
                            {
                                Subject_Info_Standard standSubject = (Subject_Info_Standard) thisSubject;
                                if (standSubject.Temporals_Count > 0)
                                {
                                    if (standSubject.Temporals.Any(temporal => (temporal_upper == temporal.ToUpper()) || (temporal == years)))
                                    {
                                        found = true;
                                    }
                                }
                            }


                            if (found)
                                break;
                        }
                    }

                    if (!found)
                    {
                        Subject_Info_Standard tempSubject = new Subject_Info_Standard();
                        if (years.Length > 0)
                        {
                            tempSubject.Add_Temporal(years);
                        }
                        if (thisTemporal.TimePeriod.Length > 0)
                        {
                            tempSubject.Add_Temporal(thisTemporal.TimePeriod);
                        }
                        tags.Add_Field(tempSubject.to_MARC_HTML());
                    }
                }
            }

            // ADD THE TABLE OF CONTENTS
            if (Bib_Info.TableOfContents.Length > 0)
            {
                tags.Add_Field(505, "0 ", "|a " + Bib_Info.TableOfContents);
            }

            // ADD THE RIGHTS 
            if (Bib_Info.Access_Condition.Text.Length > 0)
            {
                tags.Add_Field(String.Compare(Bib_Info.Access_Condition.Type, "use and reproduction", StringComparison.OrdinalIgnoreCase) == 0 ? 540 : 506, "  ", "|a " + Bib_Info.Access_Condition.Text);
            }

            // ADD THE HOLDING LOCATION
            if ((Bib_Info.hasLocationInformation) && (Bib_Info.Location.Holding_Name.Length > 0))
            {
                if (Bib_Info.Location.Holding_Name[Bib_Info.Location.Holding_Name.Length - 1] != '.')
                    tags.Add_Field(535, "1 ", "|a " + Bib_Info.Location.Holding_Name + ".");
                else
                    tags.Add_Field(535, "1 ", "|a " + Bib_Info.Location.Holding_Name);
            }

            // IF THERE IS MORE THAN ONE LANGUAGE, ADD THEM ALL IN THE 041
            if (Bib_Info.Languages_Count > 1)
            {
                StringBuilder marc_coded_languages = new StringBuilder();
                foreach (Language_Info thisLanguage in Bib_Info.Languages)
                {
                    if (thisLanguage.Language_ISO_Code.Length > 0)
                        marc_coded_languages.Append("|a " + thisLanguage.Language_ISO_Code + " ");
                }
                if (marc_coded_languages.Length > 0)
                    tags.Add_Field(041, "  ", marc_coded_languages.ToString().Trim());
            }

            // ADD THE 008 FIELD (FIXED LENGTH DATA ELEMENTS)
            MARC_Field fixedField008 = new MARC_Field {Indicators = "  ", Tag = 008};
            StringBuilder builder008 = new StringBuilder();
            builder008.Append(METS_Header.Create_Date.Year.ToString().Substring(2) + METS_Header.Create_Date.Month.ToString().PadLeft(2, '0') + METS_Header.Create_Date.Day.ToString().PadLeft(2, '0'));

            if ((Bib_Info.Origin_Info.MARC_DateIssued_Start.Length == 0) && (Bib_Info.Origin_Info.MARC_DateIssued_End.Length == 0))
            {
                builder008.Append("n        ");
            }
            else
            {
                if ((sobekcm_type != TypeOfResource_SobekCM_Enum.Newspaper) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Serial) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Multivolume))
                {
                    if ((Bib_Info.Origin_Info.MARC_DateIssued_End.Length == 0) || (Bib_Info.Origin_Info.MARC_DateIssued_End == "0000"))
                    {
                        builder008.Append("s" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + "    ");
                    }
                    else
                    {
                        switch (Bib_Info.Origin_Info.MARC_DateIssued_End.ToUpper())
                        {
                            case "UUUU":
                                builder008.Append("u" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + "uuuu");
                                break;

                            case "9999":
                                builder008.Append("m" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + "9999");
                                break;

                            default:
                                if (Bib_Info.Origin_Info.Date_Reprinted == Bib_Info.Origin_Info.MARC_DateIssued_Start)
                                {
                                    builder008.Append("r" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + Bib_Info.Origin_Info.MARC_DateIssued_End.PadLeft(4, '0'));
                                }
                                else
                                {
                                    if (Bib_Info.Origin_Info.Date_Copyrighted == Bib_Info.Origin_Info.MARC_DateIssued_End)
                                    {
                                        builder008.Append("t" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + Bib_Info.Origin_Info.MARC_DateIssued_End.PadLeft(4, '0'));
                                    }
                                    else
                                    {
                                        builder008.Append("d" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + Bib_Info.Origin_Info.MARC_DateIssued_End.PadLeft(4, '0'));
                                    }
                                }
                                break;
                        }
                    }
                }
                else
                {
                    if (Bib_Info.Origin_Info.MARC_DateIssued_End.Length == 0)
                    {
                        builder008.Append("s" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + "    ");
                    }
                    else
                    {
                        switch (Bib_Info.Origin_Info.MARC_DateIssued_End.ToUpper())
                        {
                            case "UUUU":
                                builder008.Append("u" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + "uuuu");
                                break;

                            case "9999":
                                builder008.Append("c" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + "9999");
                                break;

                            default:
                                builder008.Append("d" + Bib_Info.Origin_Info.MARC_DateIssued_Start.PadLeft(4, '0') + Bib_Info.Origin_Info.MARC_DateIssued_End.PadLeft(4, '0'));
                                break;
                        }
                    }
                }
            }

            // Is there a place listed in marc country?
            string marccountry_code = "xx ";
            if (Bib_Info.Origin_Info.Places_Count > 0)
            {
                foreach (Origin_Info_Place thisPlace in Bib_Info.Origin_Info.Places)
                {
                    if (thisPlace.Place_MarcCountry.Length > 0)
                    {
                        marccountry_code = thisPlace.Place_MarcCountry.PadRight(3, ' ');
                        break;
                    }
                }
            }
            builder008.Append(marccountry_code);

            // Set the default code for the any simple flag characters
            char default_code = '|';
            if (Bib_Info.EncodingLevel.Length > 0)
                default_code = '0';

            #region Map Specific 008 Values

            // Build the Map-Specific 008 Values
            if (sobekcm_type == TypeOfResource_SobekCM_Enum.Map)
            {
                // Add the map relief information
                string map_relief_note = String.Empty;
                if (Bib_Info.Notes_Count > 0)
                {
                    foreach (Note_Info thisNote in Bib_Info.Notes)
                    {
                        if (thisNote.Note_Type == Note_Type_Enum.NONE)
                        {
                            if ((thisNote.Note.IndexOf("Relief shown") >= 0) || (thisNote.Note.IndexOf("Depths shown") >= 0) || (thisNote.Note.IndexOf("Depth shown") >= 0))
                            {
                                map_relief_note = thisNote.Note;
                            }
                        }
                    }
                }
                if (map_relief_note.Length > 0)
                {
                    string[] map_relief_note_splitter = map_relief_note.Split(".".ToCharArray());
                    int map_relief_info = 0;
                    foreach (string map_split in map_relief_note_splitter)
                    {
                        add_value((map_split.IndexOf("contours") >= 0), 'a', ref map_relief_info, 4, builder008);
                        add_value((map_split.IndexOf("shading") >= 0), 'b', ref map_relief_info, 4, builder008);
                        add_value((map_split.IndexOf("Relief") >= 0) && ((map_split.IndexOf("gradient") >= 0) || (map_split.IndexOf("bathymetric") >= 0)), 'c', ref map_relief_info, 4, builder008);
                        add_value((map_split.IndexOf("hachure") >= 0), 'd', ref map_relief_info, 4, builder008);
                        add_value(((map_split.IndexOf("Depth") >= 0) && (map_split.IndexOf("soundings") > 0)) || (map_split.IndexOf("spot depth") >= 0), 'e', ref map_relief_info, 4, builder008);
                        add_value(map_split.IndexOf("form lines") >= 0, 'f', ref map_relief_info, 4, builder008);
                        add_value((map_split.IndexOf("spot height") >= 0), 'g', ref map_relief_info, 4, builder008);
                        add_value((map_split.IndexOf("pictorially") >= 0), 'i', ref map_relief_info, 4, builder008);
                        add_value((map_split.IndexOf("land forms") >= 0), 'j', ref map_relief_info, 4, builder008);
                        add_value((map_split.IndexOf("isolines") >= 0), 'k', ref map_relief_info, 4, builder008);
                    }
                    for (int i = map_relief_info; i < 4; i++)
                        builder008.Append(" ");
                }
                else
                {
                    builder008.Append("    ");
                }

                // Add the projection information
                bool map_projection_handled = false;
                if (Bib_Info.Subjects_Count > 0)
                {
                    foreach (Subject_Info possibleCarto in Bib_Info.Subjects)
                    {
                        if ((possibleCarto.Authority == "marcgt") && (possibleCarto.Class_Type == Subject_Info_Type.Cartographics))
                        {
                            if (((Subject_Info_Cartographics) possibleCarto).Projection.Length == 2)
                            {
                                builder008.Append(((Subject_Info_Cartographics) possibleCarto).Projection);
                                map_projection_handled = true;
                                break;
                            }
                        }
                    }
                }
                if (!map_projection_handled)
                {
                    builder008.Append("  ");
                }

                // Undefined character position
                builder008.Append(" ");

                // Add the type of cartographic material
                int map_type = 0;
                add_value(genreHash.ContainsKey("atlas"), 'e', ref map_type, 1, builder008);
                add_value(genreHash.ContainsKey("globe"), 'd', ref map_type, 1, builder008);
                add_value(genreHash.ContainsKey("single map"), 'a', ref map_type, 1, builder008);
                add_value(genreHash.ContainsKey("map series"), 'b', ref map_type, 1, builder008);
                add_value(genreHash.ContainsKey("map serial"), 'c', ref map_type, 1, builder008);
                if (map_type == 0)
                {
                    builder008.Append(" ");
                }

                // Two undefined positions
                builder008.Append("  ");

                // Government publication?
                int map_govt_publication = 0;
                add_value(genreHash.ContainsKey("multilocal government publication"), 'c', ref map_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("federal government publication"), 'f', ref map_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("international intergovernmental publication"), 'i', ref map_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("local government publication"), 'l', ref map_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("multistate government publication"), 'm', ref map_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication"), 'o', ref map_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (state, provincial, terriorial, dependent)"), 's', ref map_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (autonomous or semiautonomous component)"), 'a', ref map_govt_publication, 1, builder008);
                if (map_govt_publication == 0)
                {
                    builder008.Append(" ");
                }

                // Add form of item
                builder008.Append("o");

                // Undefined position
                builder008.Append(" ");

                // Code if this includes an index
                if (genreHash.ContainsKey("indexed"))
                {
                    builder008.Append("1");
                }
                else
                {
                    builder008.Append(default_code);
                }

                // Undefined position
                builder008.Append(" ");

                // Special format characteristics
                builder008.Append("  ");
            }

            #endregion

            #region Continuing Resource Specific 008 Values

            // Build the Continuing Resource-Specific 008 Values
            if ((sobekcm_type == TypeOfResource_SobekCM_Enum.Newspaper) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Serial))
            {
                // Add the frequency (008/18)
                int newspaper_frequency = 0;
                if (Bib_Info.Origin_Info.Frequencies_Count > 0)
                {
                    foreach (Origin_Info_Frequency thisFrequency in Bib_Info.Origin_Info.Frequencies)
                    {
                        if (thisFrequency.Authority == "marcfrequency")
                        {
                            add_value(thisFrequency.Term == "annual", 'a', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "bimonthly", 'b', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "semiweekly", 'c', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "daily", 'd', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "bieweekly", 'e', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "semiannual", 'f', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "biennial", 'g', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "triennial", 'h', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "three times a week", 'i', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "three times a month", 'j', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "continuously updated", 'k', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "monthly", 'm', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "quarterly", 'q', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "semimonthly", 's', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "three times a year", 't', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "weekly", 'w', ref newspaper_frequency, 1, builder008);
                            add_value(thisFrequency.Term == "other", 'z', ref newspaper_frequency, 1, builder008);
                        }
                    }
                }
                if (newspaper_frequency == 0)
                    builder008.Append(" ");

                // Add the regularity (08/19)
                int newspaper_regularity = 0;
                if (Bib_Info.Origin_Info.Frequencies_Count > 0)
                {
                    foreach (Origin_Info_Frequency thisFrequency in Bib_Info.Origin_Info.Frequencies)
                    {
                        if (thisFrequency.Authority == "marcfrequency")
                        {
                            add_value(thisFrequency.Term == "normalized irregular", 'n', ref newspaper_regularity, 1, builder008);
                            add_value(thisFrequency.Term == "regular", 'r', ref newspaper_regularity, 1, builder008);
                            add_value(thisFrequency.Term == "completely irregular", 'x', ref newspaper_regularity, 1, builder008);
                        }
                    }
                }
                if (newspaper_regularity == 0)
                    builder008.Append("u");

                // undefined (008/20)
                builder008.Append(" ");

                // Add the type of continuing resource (008/21)
                int newspaper_type = 0;
                add_value(genreHash.ContainsKey("database"), 'd', ref newspaper_type, 1, builder008);
                add_value(genreHash.ContainsKey("loose-leaf"), 'l', ref newspaper_type, 1, builder008);
                add_value(genreHash.ContainsKey("newspaper"), 'n', ref newspaper_type, 1, builder008);
                add_value(genreHash.ContainsKey("periodical"), 'p', ref newspaper_type, 1, builder008);
                add_value(genreHash.ContainsKey("series"), 's', ref newspaper_type, 1, builder008);
                add_value(genreHash.ContainsKey("web site"), 'w', ref newspaper_type, 1, builder008);
                if (newspaper_type == 0)
                    builder008.Append(" ");

                // Add original form of item and this item (electronic) (008/22-23)
                builder008.Append(sobekcm_type == TypeOfResource_SobekCM_Enum.Newspaper ? "eo" : " o");

                // Add nature of work (008/24-27)
                int newspaper_nature_of_contents = 0;
                add_value(genreHash.ContainsKey("abstract or summary"), 'a', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("bibliography"), 'b', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("catalog"), 'c', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("dictionary"), 'd', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("directory"), 'r', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("discography"), 'k', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("encyclopedia"), 'e', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("filmography"), 'q', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("handbook"), 'f', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("index"), 'i', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("law report or digest"), 'w', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("legal article"), 'g', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("legal case and case notes"), 'v', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("legislation"), 'l', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("patent"), 'j', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("programmed text"), 'p', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("review"), 'o', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("statistics"), 's', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("survey of literature"), 'n', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("technical reports"), 't', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("theses"), 'm', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("treaty"), 'z', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("offprint"), '2', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("yearbook"), 'y', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("calendar"), '5', ref newspaper_nature_of_contents, 3, builder008);
                add_value(genreHash.ContainsKey("comic/graphic novel"), '6', ref newspaper_nature_of_contents, 3, builder008);
                for (int i = newspaper_nature_of_contents; i < 4; i++)
                {
                    builder008.Append(" ");
                }

                // Code if this is a government document (008/28)
                int newspaper_govt_publication = 0;
                add_value(genreHash.ContainsKey("multilocal government publication"), 'c', ref newspaper_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("federal government publication"), 'f', ref newspaper_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("international intergovernmental publication"), 'i', ref newspaper_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("local government publication"), 'l', ref newspaper_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("multistate government publication"), 'm', ref newspaper_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication"), 'o', ref newspaper_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (state, provincial, terriorial, dependent)"), 's', ref newspaper_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (autonomous or semiautonomous component)"), 'a', ref newspaper_govt_publication, 1, builder008);
                if (newspaper_govt_publication == 0)
                {
                    builder008.Append(" ");
                }

                // Code if this is a conference publication (008/29)
                if (genreHash.ContainsKey("conference publication"))
                {
                    builder008.Append("1");
                }
                else
                {
                    builder008.Append(default_code);
                }

                // Undefined (008/30-32)
                builder008.Append("   ");

                // Code original alphebet or script of title
                builder008.Append(" ");

                // Code the entry convention
                builder008.Append("0");
            }

            #endregion

            #region Book Specific 008 Values

            if (sobekcm_type == TypeOfResource_SobekCM_Enum.Book)
            {
                if (Bib_Info.Original_Description.Extent.Length > 0)
                {
                    int book_illustration_specs = 0;
                    string extent_upper = Bib_Info.Original_Description.Extent.ToUpper();
                    add_value((extent_upper.IndexOf("ILL.") >= 0), 'a', ref book_illustration_specs, 4, builder008);
                    add_value((extent_upper.IndexOf("MAP") >= 0), 'b', ref book_illustration_specs, 4, builder008);
                    add_value(((extent_upper.IndexOf("PORT.") >= 0) || (extent_upper.IndexOf("PORTS.") >= 0)), 'c', ref book_illustration_specs, 4, builder008);
                    add_value((extent_upper.IndexOf("CHART") >= 0), 'd', ref book_illustration_specs, 4, builder008);
                    add_value((extent_upper.IndexOf("PLAN") >= 0), 'e', ref book_illustration_specs, 4, builder008);
                    add_value((extent_upper.IndexOf("PLATE") >= 0), 'f', ref book_illustration_specs, 4, builder008);
                    add_value((extent_upper.IndexOf("MUSIC") >= 0), 'g', ref book_illustration_specs, 4, builder008);
                    add_value(((extent_upper.IndexOf("FACSIM.") >= 0) || (extent_upper.IndexOf("FACSIMS.") >= 0)), 'h', ref book_illustration_specs, 4, builder008);
                    add_value(((extent_upper.IndexOf("COAT OF ARMS") >= 0) || (extent_upper.IndexOf("COATS OF ARMS") >= 0)), 'i', ref book_illustration_specs, 4, builder008);
                    add_value((extent_upper.IndexOf("FORM") >= 0), 'k', ref book_illustration_specs, 4, builder008);
                    add_value((extent_upper.IndexOf("SAMPLE") >= 0), 'l', ref book_illustration_specs, 4, builder008);
                    add_value(((extent_upper.IndexOf("PHOTO.") >= 0) || (extent_upper.IndexOf("PHOTOS.") >= 0)), 'o', ref book_illustration_specs, 4, builder008);

                    // Finish this off
                    if (book_illustration_specs == 0)
                    {
                        builder008.Append("    ");
                    }
                    else
                    {
                        for (int i = book_illustration_specs; i < 4; i++)
                        {
                            builder008.Append(" ");
                        }
                    }
                }
                else
                {
                    builder008.Append("||||");
                }

                // Look for target audience
                int book_target_audiences = 0;
                if (Bib_Info.Target_Audiences_Count > 0)
                {
                    foreach (TargetAudience_Info thisTarget in Bib_Info.Target_Audiences)
                    {
                        if (thisTarget.Authority == "marctarget")
                        {
                            add_value(thisTarget.Audience == "adolescent", 'd', ref book_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "adult", 'e', ref book_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "general", 'g', ref book_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "primary", 'b', ref book_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "pre-adolescent", 'c', ref book_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "juvenile", 'j', ref book_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "preschool", 'a', ref book_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "specialized", 'f', ref book_target_audiences, 1, builder008);
                        }
                    }
                }
                if (book_target_audiences == 0)
                    builder008.Append(" ");

                // Always electronic --> online (1/11/2013)
                builder008.Append("o");

                // Code nature of contents
                int book_nature_of_contents = 0;
                add_value(genreHash.ContainsKey("abstract or summary"), 'a', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("bibliography"), 'b', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("catalog"), 'c', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("dictionary"), 'd', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("directory"), 'r', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("discography"), 'k', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("encyclopedia"), 'e', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("filmography"), 'q', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("handbook"), 'f', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("index"), 'i', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("law report or digest"), 'w', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("legal article"), 'g', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("legal case and case notes"), 'v', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("legislation"), 'l', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("patent"), 'j', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("programmed text"), 'p', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("review"), 'o', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("statistics"), 's', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("survey of literature"), 'n', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("technical reports"), 't', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("theses"), 'm', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("treaty"), 'z', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("offprint"), '2', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("yearbook"), 'y', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("calendar"), '5', ref book_nature_of_contents, 4, builder008);
                add_value(genreHash.ContainsKey("comic/graphic novel"), '6', ref book_nature_of_contents, 4, builder008);


                for (int i = book_nature_of_contents; i < 4; i++)
                {
                    builder008.Append(" ");
                }

                // Code if this is a government document
                int book_govt_publication = 0;
                add_value(genreHash.ContainsKey("multilocal government publication"), 'c', ref book_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("federal government publication"), 'f', ref book_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("international intergovernmental publication"), 'i', ref book_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("local government publication"), 'l', ref book_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("multistate government publication"), 'm', ref book_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication"), 'o', ref book_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (state, provincial, terriorial, dependent)"), 's', ref book_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (autonomous or semiautonomous component)"), 'a', ref book_govt_publication, 1, builder008);
                if (book_govt_publication == 0)
                {
                    builder008.Append(" ");
                }

                // Code if this is a conference publication
                if (genreHash.ContainsKey("conference publication"))
                {
                    builder008.Append("1");
                }
                else
                {
                    builder008.Append(default_code);
                }

                // Code if this is a festschrift
                if (genreHash.ContainsKey("festschrift"))
                {
                    builder008.Append("1");
                }
                else
                {
                    builder008.Append(default_code);
                }

                // Code if this includes an index
                if (genreHash.ContainsKey("indexed"))
                {
                    builder008.Append("1");
                }
                else
                {
                    builder008.Append(default_code);
                }

                // Undefined character spot
                builder008.Append(" ");

                // Code the literary form
                int book_literary_form = 0;
                add_value(genreHash.ContainsKey("drama"), 'd', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("comic strip"), 'a', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("essay"), 'e', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("humor, satire"), 'h', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("letter"), 'i', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("poetry"), 'p', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("novel"), 'f', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("short story"), 'j', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("speech"), 's', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("non-fiction"), '0', ref book_literary_form, 1, builder008);
                add_value(genreHash.ContainsKey("fiction"), '1', ref book_literary_form, 1, builder008);
                if (book_literary_form == 0)
                    builder008.Append("u");

                // Code the biography value
                int book_biography_value = 0;
                add_value(genreHash.ContainsKey("autobiography"), 'a', ref book_biography_value, 1, builder008);
                add_value(genreHash.ContainsKey("individual biography"), 'b', ref book_biography_value, 1, builder008);
                add_value(genreHash.ContainsKey("collective biography"), 'c', ref book_biography_value, 1, builder008);
                if (book_biography_value == 0)
                    builder008.Append(" ");
            }

            #endregion

            #region Visual Material Specific 008 Values

            if ((sobekcm_type == TypeOfResource_SobekCM_Enum.Aerial) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Photograph) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Audio) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Video) || (sobekcm_type == TypeOfResource_SobekCM_Enum.Artifact))
            {
                // Code running time for movies (008/18-20)
                builder008.Append("nnn");

                // Undefined (008/21)
                builder008.Append(" ");

                // Target Audience (008/22)
                int visual_target_audiences = 0;
                if (Bib_Info.Target_Audiences_Count > 0)
                {
                    foreach (TargetAudience_Info thisTarget in Bib_Info.Target_Audiences)
                    {
                        if (thisTarget.Authority == "marctarget")
                        {
                            add_value(thisTarget.Audience == "adolescent", 'd', ref visual_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "adult", 'e', ref visual_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "general", 'g', ref visual_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "primary", 'b', ref visual_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "pre-adolescent", 'c', ref visual_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "juvenile", 'j', ref visual_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "preschool", 'a', ref visual_target_audiences, 1, builder008);
                            add_value(thisTarget.Audience == "specialized", 'f', ref visual_target_audiences, 1, builder008);
                        }
                    }
                }
                if (visual_target_audiences == 0)
                    builder008.Append(" ");

                // Undefined (008/23-27)
                builder008.Append("     ");

                // Is this a government publication? (008/28)
                int visual_govt_publication = 0;
                add_value(genreHash.ContainsKey("multilocal government publication"), 'c', ref visual_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("federal government publication"), 'f', ref visual_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("international intergovernmental publication"), 'i', ref visual_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("local government publication"), 'l', ref visual_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("multistate government publication"), 'm', ref visual_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication"), 'o', ref visual_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (state, provincial, terriorial, dependent)"), 's', ref visual_govt_publication, 1, builder008);
                add_value(genreHash.ContainsKey("government publication (autonomous or semiautonomous component)"), 'a', ref visual_govt_publication, 1, builder008);
                if (visual_govt_publication == 0)
                {
                    builder008.Append(" ");
                }

                // What is the form of this item (always electronic) (008/29)
                builder008.Append("o");

                // Undefined (008/30-32)
                builder008.Append("   ");

                // Type of visual material (008/33)
                int visual_nature_of_contents = 0;
                add_value(genreHash.ContainsKey("art original"), 'a', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("art reproduction"), 'c', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("chart"), 'n', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("diorama"), 'd', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("filmstrip"), 'f', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("flash card"), 'o', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("graphic"), 'k', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("kit"), 'b', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("microscope slide"), 'p', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("model"), 'q', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("motion picture"), 'm', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("picture"), 'i', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("realia"), 'r', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("slide"), 's', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("technical drawing"), 'l', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("toy"), 'w', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("transparency"), 't', ref visual_nature_of_contents, 1, builder008);
                add_value(genreHash.ContainsKey("video recording"), 'v', ref visual_nature_of_contents, 1, builder008);
                if (visual_nature_of_contents == 0)
                {
                    builder008.Append(" ");
                }

                // Technique (008/34)
                builder008.Append(sobekcm_type == TypeOfResource_SobekCM_Enum.Video ? "u" : "n");
            }

            #endregion

            if (sobekcm_type == TypeOfResource_SobekCM_Enum.Archival)
            {
                // Treat as MIXED material
                builder008.Append("     s           ");
            }

            // For any other type, just use spaces
            if ((sobekcm_type != TypeOfResource_SobekCM_Enum.Book) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Map) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Newspaper) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Serial) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Aerial) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Photograph) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Audio) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Video) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Artifact) && (sobekcm_type != TypeOfResource_SobekCM_Enum.Archival))
            {
                builder008.Append("     s           ");
            }

            // Add the language code
            string language_code = "eng";
            if (Bib_Info.Languages_Count > 0)
            {
                foreach (Language_Info thisLanguage in Bib_Info.Languages)
                {
                    if (thisLanguage.Language_ISO_Code.Length > 0)
                    {
                        language_code = thisLanguage.Language_ISO_Code;
                        break;
                    }
                }
            }
            builder008.Append(language_code);

            builder008.Append(" d");

            //if (this.Bib_Info.Record.MARC_Record_Content_Sources.Count > 0)
            //{
            //    builder008.Append("d");
            //}
            //else
            //{
            //    builder008.Append("|");
            //}

            fixedField008.Control_Field_Value = builder008.ToString();
            tags.Add_Field(fixedField008);

            // Now, set the leader
            tags.Leader = MARC_Leader();

            return tags;
        }

        private void add_value(bool add, char char_to_add, ref int current_count, int max_count, StringBuilder builder)
        {
            if (current_count >= max_count)
                return;

            if (add)
            {
                builder.Append(char_to_add);
                current_count++;
            }
        }

        /// <summary> Gets the MARC leader to be written for this digital resource </summary>
        /// <returns> MARC leader formatted as a string </returns>
        private string MARC_Leader()
        {
            const string total_length_string = "00000";
            const string total_directory_string = "00000";

            string type_string = "am";
            switch (Bib_Info.SobekCM_Type)
            {
                case TypeOfResource_SobekCM_Enum.Serial:
                case TypeOfResource_SobekCM_Enum.Newspaper:
                    type_string = "as";
                    break;

                case TypeOfResource_SobekCM_Enum.Map:
                    type_string = "em";
                    break;

                case TypeOfResource_SobekCM_Enum.Archival:
                    type_string = Bib_Info.Type.Collection ? "pc" : "pm";
                    break;

                case TypeOfResource_SobekCM_Enum.Audio:
                    type_string = "im";
                    break;

                case TypeOfResource_SobekCM_Enum.Video:
                    type_string = "gm";
                    break;

                case TypeOfResource_SobekCM_Enum.Photograph:
                case TypeOfResource_SobekCM_Enum.Aerial:
                    type_string = "km";
                    break;

                case TypeOfResource_SobekCM_Enum.Artifact:
                    type_string = "rm";
                    break;
            }

            // Per betsy, do not include the original encoding level.. always '3'
            // Added back when working with the ETDs.  We want to be able to SPECIFY an encoding level,
            // so maybe just not save the old encoding level when importing from MARC records.
            switch (Bib_Info.EncodingLevel.Length)
            {
                case 1:
                    return total_length_string + "n" + type_string + "  22" + total_directory_string + Bib_Info.EncodingLevel.Replace("#", " ") + "a 4500";
                default:
                    return total_length_string + "n" + type_string + "  22" + total_directory_string + "3a 4500";
            }

            //return total_length_string + "n" + type_string + "  22" + total_directory_string + "3a 4500";
        }

        /// <summary> Gets the SobekCM-specific tags to be written for this digital resource </summary>
        /// <param name="Collection_Name"> Item aggregation name this digital resource is associated with </param>
        /// <param name="include_endeca_tags"> Flag indicates whether Endeca-specific tags should also be written </param>
        /// <param name="system_name"> Name of the host system, which is added into the MARC record </param>
        /// <param name="system_abbrev"> Abbreviation for the host system, which is added into the MARC record </param>
        /// <returns> Collection of MARC tags to be written for this digital resource </returns>
        public List<MARC_Field> MARC_Sobek_Standard_Tags(string Collection_Name, bool include_endeca_tags, string system_name, string system_abbrev)
        {
            List<string> collections = new List<string>();
            if (Collection_Name.Trim().Length > 0)
            {
                collections.Add(Collection_Name);
            }
            return MARC_Sobek_Standard_Tags(collections, include_endeca_tags, system_name, system_abbrev);
        }

        /// <summary> Gets the SobekCM-specific tags to be written for this digital resource </summary>
        /// <param name="Collection_Names"> Collection of all the item aggregation names this digital resource is associated with </param>
        /// <param name="include_endeca_tags"> Flag indicates whether Endeca-specific tags should also be written </param>
        /// <param name="system_name"> Name of the host system, which is added into the MARC record </param>
        /// <param name="system_abbrev"> Abbreviation for the host system, which is added into the MARC record </param>
        /// <returns> Collection of MARC tags to be written for this digital resource </returns>
        public List<MARC_Field> MARC_Sobek_Standard_Tags(List<string> Collection_Names, bool include_endeca_tags, string system_name, string system_abbrev)
        {
            // Start to build the list of additional tags
            List<MARC_Field> tag_list = new List<MARC_Field>();

            if (system_name.Length > 0)
            {
                tag_list.Add(new MARC_Field(830, " 0", "|a " + system_name + "."));
            }

            // Was this born digital?  in which case this is NOT an electronic reproduction, so
            // leave out the 533 field
            bool borndigital = Bib_Info.Genres.Any(thisGenre => (thisGenre.Authority == "sobekcm") && (thisGenre.Genre_Term == "born-digital"));
            if (!borndigital)
            {
                MARC_Field tag533 = new MARC_Field {Tag = 533, Indicators = "  "};
                StringBuilder builder533 = new StringBuilder(100);
                builder533.Append("|a Electronic reproduction. ");
                if (Bib_Info.BibID.IndexOf("UF") == 0)
                    builder533.Append("|b Gainesville, Fla. : |c University of Florida, George A. Smathers Libraries, ");

                if (Bib_Info.Source.Code.IndexOf("UF") < 0)
                {
                    builder533.Append("|c " + Bib_Info.Source.Statement + ", ");
                }

                if ((Bib_Info.hasLocationInformation) && (Bib_Info.Location.Holding_Code.IndexOf("UF") < 0) && (Bib_Info.Location.Holding_Code != Bib_Info.Source.Code))
                {
                    builder533.Append("|c " + Bib_Info.Location.Holding_Name + ", ");
                }

                builder533.Append("|d " + METS_Header.Create_Date.Year + ". ");
                if (system_name.Length > 0)
                {
                    builder533.Append("|f (" + system_name + ")");
                }
                foreach (string collection in Collection_Names)
                {
                    if (collection.Trim().Length > 0)
                    {
                        builder533.Append(" |f (" + collection + ")");
                    }
                }
                builder533.Append("|n Mode of access: World Wide Web.  |n System requirements: Internet connectivity; Web browser software.");
                tag533.Control_Field_Value = builder533.ToString();
                tag_list.Add(tag533);
            }

            // Add the collection name as well ( Was getting duplicates here sometimes )
            List<string> added_already = new List<string>();
            foreach (string collection in Collection_Names)
            {
                if (!added_already.Contains(collection.ToUpper().Trim()))
                {
                    if (collection.Trim().Length > 0)
                    {
                        added_already.Add(collection.ToUpper().Trim());
                        tag_list.Add(new MARC_Field(830, " 0", "|a " + collection + "."));
                    }
                }
            }

            // REMOVED PER JIMMIE ( January 2010 )
            //tag_list.Add_Tag("530", "  ", "|a Also available in print.");


            // Add the endeca only tags
            if (include_endeca_tags)
            {
                // Add the thumbnail link (992)
                if ((Behaviors.Main_Thumbnail.Length > 0) && (!Behaviors.Dark_Flag))
                {
                    string thumbnail_link = web.Source_URL + "/" + Behaviors.Main_Thumbnail;
                    tag_list.Add(new MARC_Field(992, "04", "|a " + thumbnail_link.Replace("\\", "/").Replace("//", "/").Replace("http:/", "http://")));
                }

                // Add the 852
                MARC_Field tag852 = new MARC_Field {Tag = 852, Indicators = "  "};
                StringBuilder builder852 = new StringBuilder(100);
                builder852.Append("|a SUS01:;:;: ");
                if (system_abbrev.Length > 0)
                    builder852.Append("|b " + system_abbrev);
                if (Behaviors.Aggregation_Count > 0)
                    builder852.Append(" |c " + Behaviors.Aggregations[0].Name);
                tag852.Control_Field_Value = builder852.ToString();
                tag_list.Add(tag852);

                // Add the collection name in the Endeca spot (997)
                if (Collection_Names.Count > 0)
                {
                    tag_list.Add(new MARC_Field(997, "  ", "|a " + Collection_Names[0]));
                }
            }

            return tag_list;
        }

        #endregion

        #region Method to create the serial hierarchy

        /// <summary> Creates the serial hierarchy if this is a newspaper or serial item </summary>
        public void Create_Serial_Hierarchy()
        {
            if (Behaviors.hasSerialInformation)
            {
                // Set the serial hierarchy, if possible
                if (Behaviors.Serial_Info.Count == 0)
                {
                    switch (Bib_Info.SobekCM_Type)
                    {
                        case TypeOfResource_SobekCM_Enum.Newspaper:
                            if (!Copy_Chronology_To_Serial_Info())
                            {
                                Copy_Enumeration_To_Serial_Info();
                            }
                            break;

                        case TypeOfResource_SobekCM_Enum.Serial:
                            if (!Copy_Enumeration_To_Serial_Info())
                            {
                                Copy_Chronology_To_Serial_Info();
                            }
                            break;
                    }
                }
            }
        }

        private bool Copy_Chronology_To_Serial_Info()
        {
            if (Bib_Info.Origin_Info.Date_Issued.Length > 0)
            {
                try
                {
                    DateTime pubDate = Convert.ToDateTime(Bib_Info.Origin_Info.Date_Issued);
                    Behaviors.Serial_Info.Add_Hierarchy(1, pubDate.Year, pubDate.Year.ToString());
                    string month = pubDate.Month.ToString();
                    switch (pubDate.Month)
                    {
                        case 1:
                            month = "January";
                            break;

                        case 2:
                            month = "February";
                            break;

                        case 3:
                            month = "March";
                            break;

                        case 4:
                            month = "April";
                            break;

                        case 5:
                            month = "May";
                            break;

                        case 6:
                            month = "June";
                            break;

                        case 7:
                            month = "July";
                            break;

                        case 8:
                            month = "August";
                            break;

                        case 9:
                            month = "September";
                            break;

                        case 10:
                            month = "October";
                            break;

                        case 11:
                            month = "November";
                            break;

                        case 12:
                            month = "December";
                            break;
                    }
                    Behaviors.Serial_Info.Add_Hierarchy(2, pubDate.Month, month);
                    Behaviors.Serial_Info.Add_Hierarchy(3, pubDate.Day, pubDate.Day.ToString());

                    return true;
                }
                catch
                {
                    return false;
                }
            }
            return false;
        }

        private bool Copy_Enumeration_To_Serial_Info()
        {
            if (!Bib_Info.hasSeriesPartInfo)
                return false;

            int level = 1;
            if (Bib_Info.Series_Part_Info.Enum1.Length > 0)
            {
                try
                {
                    // Is this volume a number?
                    int volume_number = Convert.ToInt32(Bib_Info.Series_Part_Info.Enum1);

                    // If this is not a year, add 'Volume ' in front of it
                    if ((volume_number < 1000) || (volume_number > 2050))
                        Behaviors.Serial_Info.Add_Hierarchy(level++, volume_number, "Volume " + volume_number);
                    else
                        Behaviors.Serial_Info.Add_Hierarchy(level++, volume_number, volume_number.ToString());
                }
                catch
                {
                    Behaviors.Serial_Info.Add_Hierarchy(level++, 1, Bib_Info.Series_Part_Info.Enum1);
                }
            }

            if (Bib_Info.Series_Part_Info.Enum2.Length > 0)
            {
                try
                {
                    // Is this issue a number?
                    int issue_number = Convert.ToInt32(Bib_Info.Series_Part_Info.Enum2);

                    // If this is not a year, add 'Issue ' in front of it
                    if ((issue_number < 1000) || (issue_number > 2050))
                        Behaviors.Serial_Info.Add_Hierarchy(level++, issue_number, "Issue " + issue_number);
                    else
                        Behaviors.Serial_Info.Add_Hierarchy(level++, issue_number, issue_number.ToString());
                }
                catch
                {
                    Behaviors.Serial_Info.Add_Hierarchy(level++, 1, Bib_Info.Series_Part_Info.Enum2);
                }
            }

            if (Bib_Info.Series_Part_Info.Enum3.Length > 0)
            {
                try
                {
                    // Is this edition a number?
                    int edition_number = Convert.ToInt32(Bib_Info.Series_Part_Info.Enum3);

                    // If this is not a year, add 'Issue ' in front of it
                    if ((edition_number < 1000) || (edition_number > 2050))
                        Behaviors.Serial_Info.Add_Hierarchy(level++, edition_number, "Edition " + edition_number);
                    else
                        Behaviors.Serial_Info.Add_Hierarchy(level++, edition_number, edition_number.ToString());
                }
                catch
                {
                    Behaviors.Serial_Info.Add_Hierarchy(level++, 1, Bib_Info.Series_Part_Info.Enum3);
                }
            }

            return level > 1;
        }

        #endregion
    }
}