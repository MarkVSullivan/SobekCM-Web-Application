using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SobekCM.Bib_Package.Divisions;

namespace SobekCM.Bib_Package.SobekCM_Info
{
    /// <summary> Class holds all the specific data needed to display this item in the 
    /// UFDC.aspx application </summary>
    [Serializable]
    public class SobekCM_Web_Info : XML_Writing_Base_Type
    {
        private string bibid;
        private string vid;

        private string groupTitle;

        private string assocFilePath;
        private string fileLocation;
        private string imageRoot;

        private Nullable<short> ip_restricted;
        private Nullable<bool> dark_flag;
        private Nullable<bool> checkOutRequired;

        private bool textSearchable;
        private bool suppressEndeca;
        private bool can_be_described;
        private bool exposeFullTextForHarvesting;

        private int itemID;
        private int groupID;
        private Nullable<int> siblings;
        private int pageCount;
        private int divCount;

        private List<Descriptive_Tag> tags;
        private List<string> ticklers;
        private bool show_validation_errors;       
        private string greenstoneCode;
        private List<string> webskins;
        private List<string> download_eligible_files;

        private List<Page_TreeNode> pages_by_seq;
        private Dictionary<string, SobekCM_File_Info> viewer_to_file;        

        private List<View_Object> views;
        private List<View_Object> item_level_page_views;
        private View_Object defaultView;

        private string groupType;

        private List<Related_Titles> related_titles_collection;

        private List<Wordmark_Info> wordmarks;
        private Main_Page_Info mainPage;
        private List<Aggregation_Info> aggregations;

        private string mainThumbnail;
        private string guid;
        private string notifyEmail;

        private Bib_Info.Identifier_Info primaryIdentifier;

        /// <summary> Constructor for a new instance of the SobekCM_Web_Info class </summary>
        public SobekCM_Web_Info()
        {
            textSearchable = false;
            checkOutRequired = false;
            ip_restricted = 0;
            suppressEndeca = true;
            can_be_described = false;
            show_validation_errors = false;
            itemID = -1;
            groupID = -1;
            pageCount = 0;
            divCount = 0;
            dark_flag = false;
            exposeFullTextForHarvesting = true;
            primaryIdentifier = new SobekCM.Bib_Package.Bib_Info.Identifier_Info();
        }

        /// <summary> Flag indicates if this item should be displayed Left-to-Right, rather than the
        /// default Right-to-Left. </summary>
        /// <remarks> This adds support for page turning for languages such as Yiddish </remarks>
        public bool Left_To_Right
        {
            get { return false; }
        }

        #region Wordmark/icon properties and methods

        /// <summary> Dedupes the wordmarks to ensure that no duplication is occurring </summary>
        /// <remarks> Saving duped wordmarks to the database can result in a database exception during the save process </remarks>
        public void Dedupe_Wordmarks()
        {
            if ((wordmarks != null) && ( wordmarks.Count > 1 ))
            {
                List<Wordmark_Info> existing = new List<Wordmark_Info>();
                foreach (Wordmark_Info thisWordmark in wordmarks)
                {
                    existing.Add(thisWordmark);
                }
                wordmarks.Clear();
                List<string> codes = new List<string>();
                foreach (Wordmark_Info thisWordmark in existing)
                {
                    if ((thisWordmark.Code.ToUpper() != "WATERFRONT") && (thisWordmark.Code.ToUpper() != "NEWS") && (!codes.Contains(thisWordmark.Code.ToUpper())))
                    {
                        codes.Add(thisWordmark.Code.ToUpper());
                        wordmarks.Add(thisWordmark);
                    }
                }
            }
        }

        /// <summary> Get the number of wordmarks (or wormdarks) associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Wordmarks"/> property.  Even if 
        /// there are no wordmarks, the Icons property creates a readonly collection to pass back out.</remarks>
        public int Wordmark_Count
        {
            get
            {
                if (wordmarks == null)
                    return 0;
                else
                    return wordmarks.Count;
            }
        }

        /// <summary> Gets the collection of wordmarks (or wordmarks) associated with this resource </summary>
        /// <remarks> You should check the count of wordmarks first using the <see cref="Wordmark_Count"/> property before using this property.
        /// Even if there are no wordmarks, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Wordmark_Info> Wordmarks
        {
            get
            {
                if (wordmarks == null)
                    return new ReadOnlyCollection<Wordmark_Info>(new List<Wordmark_Info>());
                else
                    return new ReadOnlyCollection<Wordmark_Info>(wordmarks);
            }
        }

        /// <summary> Adds a wordmark/icon, if it doesn't exist </summary>
        /// <param name="Wordmark">Wordmark code to add</param>        
        public void Add_Wordmark(string Wordmark)
        {
            if (Wordmark.Trim().Length > 0)
            {
                Add_Wordmark(new Wordmark_Info(Wordmark));
            }
        }

        /// <summary> Adds a wordmark/icon, if it doesn't exist </summary>
        /// <param name="Wordmark">Wordmark code to add</param>
        /// <remarks>This parses the wordmark string for spaces, commas, and semicolons.</remarks>
        public void Add_Wordmarks(string Wordmark)
        {
            if (Wordmark.Length > 0)
            {
                string[] splitIcons = Wordmark.Split(" ,;".ToCharArray());
                foreach (string thisIcon in splitIcons)
                {
                    if (thisIcon.Trim().Length > 0)
                    {
                        if (wordmarks == null)
                            wordmarks = new List<Wordmark_Info>();

                        string trimmedIcon = thisIcon.ToUpper().Replace(".GIF", "").Replace(".JPG", "").Trim();
                        Wordmark_Info newIcon = new Wordmark_Info(trimmedIcon.ToUpper());
                        if (!wordmarks.Contains(newIcon))
                            wordmarks.Add(newIcon);
                    }
                }
            }
        }

        /// <summary> Adds a wordmark/icon, if it doesn't exist </summary>
        /// <param name="Wordmark">Wordmark code to add</param>
        public void Add_Wordmark(Wordmark_Info Wordmark)
        {
            if (wordmarks == null)
                wordmarks = new List<Wordmark_Info>();
            wordmarks.Add(Wordmark);
        }

        /// <summary> Clears all the icon/wordmarks associated with this item </summary>
        public void Clear_Wordmarks()
        {
            if (wordmarks != null)
                wordmarks.Clear();
        }

        #endregion

        #region Aggregation properties and methods

        /// <summary> Adds an aggregation, if it doesn't exist </summary>
        /// <param name="Code">Aggregation code to add</param>
        /// <remarks>This parses the aggregation string for spaces, commas, and semicolons.</remarks>
        public void Add_Aggregation(string Code)
        {
            if (Code.Length > 0)
            {
                string[] splitAggregations = Code.Split(" ,;".ToCharArray());
                foreach (string thisAggregations in splitAggregations)
                {
                    if (thisAggregations.Trim().Length > 0)
                    {
                        if (aggregations == null)
                            aggregations = new List<Aggregation_Info>();

                        // Create this aggregation object
                        Aggregation_Info newAggregation = new Aggregation_Info(Code.Trim().ToUpper(), String.Empty);

                        // If this doesn't exist, add it
                        if (!aggregations.Contains(newAggregation))
                            aggregations.Add(newAggregation);
                    }
                }
            }
        }

        /// <summary> Adds an aggregation, if it doesn't exist </summary>
        /// <param name="Code">Aggregation code to add</param>
        /// <param name="Name">Aggregation name to add</param>
        /// <remarks>This parses the aggregation string for spaces, commas, and semicolons.</remarks>
        public void Add_Aggregation(string Code, string Name )
        {
            if (Code.Length > 0)
            {
                if (aggregations == null)
                    aggregations = new List<Aggregation_Info>();

                // Create this aggregation object
                Aggregation_Info newAggregation = new Aggregation_Info(Code.Trim().ToUpper(), Name);

                // If this doesn't exist, add it
                if (!aggregations.Contains(newAggregation))
                    aggregations.Add(newAggregation);
            }
        }

        /// <summary> Clear all of the aggregations associated with this item </summary>
        public void Clear_Aggregations()
        {
            if (aggregations != null)
                aggregations.Clear();
        }

        /// <summary> Gets the list of all aggregation codes as a single semi-colon delimited string </summary>
        public string Aggregation_Codes
        {
            get
            {
                StringBuilder returnValue = new StringBuilder();
                if (aggregations != null)
                {
                    foreach (Aggregation_Info aggregation in aggregations)
                        returnValue.Append(aggregation.Code + ";");
                }
                if (returnValue.Length > 0)
                {
                    return returnValue.ToString().Substring(0, returnValue.Length - 1);
                }
                return String.Empty;
            }
        }

        /// <summary> Gets the list of all aggregation codes  </summary>
        public ReadOnlyCollection<string> Aggregation_Code_List
        {
            get
            {
                List<string> returnValue = new List<string>();
                if (aggregations != null)
                {
                    foreach (Aggregation_Info aggregation in aggregations)
                        returnValue.Add(aggregation.Code);
                }
                return new ReadOnlyCollection<string>(returnValue);
            }
        }

        /// <summary> Get the number of aggregation codes associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Aggregations"/> property.  Even if 
        /// there are no aggregations, the Aggregations property creates a readonly collection to pass back out.</remarks>
        public int Aggregation_Count
        {
            get
            {
                if (aggregations == null)
                    return 0;
                else
                    return aggregations.Count;
            }
        }

        /// <summary> Gets the readonly list of all aggregation codes </summary>
        ///  <remarks> You should check the count of aggregations first using the <see cref="Aggregation_Count"/> property before using this property.
        /// Even if there are no aggregations, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Aggregation_Info> Aggregations
        {
            get
            {
                if (aggregations != null)
                    return new ReadOnlyCollection<Aggregation_Info>(aggregations);
                else
                    return new ReadOnlyCollection<Aggregation_Info>(new List<Aggregation_Info>());
            }
        }

        #endregion

        #region Web Skin properties and methods

        /// <summary> Clear all the pre-existing web skins from this item </summary>
        public void Clear_Web_Skins()
        {
            if (webskins != null)
                webskins.Clear();
        }

        /// <summary> Add a new web skin code to this item </summary>
        /// <param name="New_Web_Skin"> New web skin code </param>
        public void Add_Web_Skin(string New_Web_Skin)
        {
            if (New_Web_Skin.Length > 0)
            {
                if (webskins == null)
                    webskins = new List<string>();

                if (!webskins.Contains(New_Web_Skin))
                    webskins.Add(New_Web_Skin);
            }
        }

        /// <summary> Get the number of web skins linked to this item  </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Web_Skins"/> property.  Even if 
        /// there are no web skins, the Web_Skins property creates a readonly collection to pass back out.</remarks>
        public int Web_Skin_Count
        {
            get
            {
                if (webskins == null)
                    return 0;
                else
                    return webskins.Count;
            }
        }

        /// <summary> Gets the collection of web skins associated with this resource </summary>
        /// <remarks> You should check the count of web skins first using the <see cref="Web_Skin_Count"/> before using this property.
        /// Even if there are no web skins, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Web_Skins
        {
            get
            {
                if (webskins == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(webskins);
            }
        }


        #endregion

        #region Views properties and methods

        /// <summary> Gets and sets the default view for this item </summary>
        public View_Object Default_View
        {
            get { return defaultView; }
            set { defaultView = value; }
        }

        /// <summary> Gets the collection of SobekCM views associated with this resource </summary>
        /// <remarks> You should check the count of views first using the <see cref="Views_Count"/> before using this property.
        /// Even if there are no views, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<View_Object> Views
        {
            get
            {
                if (views == null)
                    return new ReadOnlyCollection<View_Object>(new List<View_Object>());
                else
                    return new ReadOnlyCollection<View_Object>(views);
            }
        }

        /// <summary> Gets the number of overall SobekCM views associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Views"/> property.  Even if 
        /// there are no views, the Views property creates a readonly collection to pass back out.</remarks>
        public int Views_Count
        {
            get
            {
                if (views == null)
                    return 0;
                else
                    return views.Count;
            }
        }

        /// <summary> Clear all the pre-existing views from this item </summary>
        public void Clear_Views()
        {
            if (views != null)
                views.Clear();
        }


        /// <summary>Add a new SobekCM View to this resource </summary>
        /// <param name="New_View">SobekCM View object</param>
        public void Add_View(View_Object New_View)
        {
            if (views == null)
                views = new List<View_Object>();
            views.Add(New_View);
        }

        /// <summary>Add a new SobekCM View to this resource </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <returns>Built view object</returns>
        public View_Object Add_View(View_Enum View_Type)
        {
            return Add_View(View_Type, String.Empty, String.Empty);
        }

        /// <summary>Add a new SobekCM View to this resource </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <param name="Label">Label for this SobekCM View</param>
        /// <param name="Attributes">Any additional attribures needed for thie SobekCM View</param>
        /// <returns>Built view object</returns>
        public View_Object Add_View(View_Enum View_Type, string Label, string Attributes)
        {
            if (View_Type != View_Enum.None)
            {
                if (views == null)
                    views = new List<View_Object>();

                View_Object newView = new View_Object(View_Type, Label, Attributes);
                views.Add(newView);
                return newView;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Add a new SobekCM View to this resource </summary>
        /// <param name="index">Index where to insert this view</param>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <returns>Built view object</returns>
        public View_Object Insert_View(int index, View_Enum View_Type)
        {
            if (View_Type != View_Enum.None)
            {
                if (views == null)
                    views = new List<View_Object>();

                View_Object newView = new View_Object(View_Type, String.Empty, String.Empty);
                views.Insert(index, newView);
                return newView;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Add a new SobekCM View to this resource </summary>
        /// <param name="index">Index where to insert this view</param>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <param name="Label">Label for this SobekCM View</param>
        /// <param name="Attributes">Any additional attribures needed for thie SobekCM View</param>
        /// <returns>Built view object</returns>
        public View_Object Insert_View(int index, View_Enum View_Type, string Label, string Attributes)
        {
            if (View_Type != View_Enum.None)
            {
                if (views == null)
                    views = new List<View_Object>();

                View_Object newView = new View_Object(View_Type, Label, Attributes);
                views.Insert(index, newView);
                return newView;
            }
            else
            {
                return null;
            }
        }

        /// <summary>Add a new SobekCM View to this resource </summary>
        /// <param name="View_Type">Standard type of SobekCM View</param>
        /// <param name="Label">Label for this SobekCM View</param>
        /// <param name="FileName">Name of the file</param>
        /// <param name="Attributes">Any additional attribures needed for thie SobekCM View</param>
        /// <returns>Built view object</returns>
        public View_Object Add_View(View_Enum View_Type, string Label, string FileName, string Attributes)
        {
            if (View_Type != View_Enum.None)
            {
                if (views == null)
                    views = new List<View_Object>();

                View_Object newView = new View_Object(View_Type, Label, Attributes);
                newView.FileName = FileName;
                views.Add(newView);
                return newView;
            }
            else
            {
                return null;
            }
        }

        /// <summary> Clear the SobekCM item-level page views linked to this resource </summary>
        public void Clear_Item_Level_Page_Views()
        {
            if (item_level_page_views != null)
                item_level_page_views.Clear();
        }

        /// <summary>Add a new SobekCM item-level page view to this resource </summary>
        /// <param name="New_View">SobekCM View object</param>
        public void Add_Item_Level_Page_View(View_Object New_View)
        {
            if (item_level_page_views == null)
                item_level_page_views = new List<View_Object>();
            item_level_page_views.Add(New_View);
        }

        /// <summary> List of page views which are generally accessible for this item </summary>
        /// <remarks>This is used by the SobekCM web application only. <br /><br />
        /// You should check the count of page views first using the <see cref="Item_Level_Page_Views_Count"/> before using this property.
        /// Even if there are no page views, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<View_Object> Item_Level_Page_Views
        {
            get
            {
                if (item_level_page_views == null)
                    return new ReadOnlyCollection<View_Object>(new List<View_Object>());
                else
                    return new ReadOnlyCollection<View_Object>(item_level_page_views);
            }
        }

        /// <summary> Gets the number of page views associated with this resource </summary>
        /// <remarks>This is used by the SobekCM web application only. <br /><br />
        /// This should be used rather than the Count property of the <see cref="Item_Level_Page_Views"/> property.  Even if 
        /// there are no page views, the Item_Level_Page_Views property creates a readonly collection to pass back out.</remarks>
        public int Item_Level_Page_Views_Count
        {
            get
            {
                if (item_level_page_views == null)
                    return 0;
                else
                    return item_level_page_views.Count;
            }
        }

        #endregion

        #region Related title properties and methods

        /// <summary> Gets the number of related titles from the SobekCM database </summary>
        public int Related_Titles_Count
        {
            get
            {
                if (related_titles_collection == null)
                    return 0;
                else
                    return related_titles_collection.Count;
            }
        }

        /// <summary> Gets the related titles collection </summary>
        public List<Related_Titles> All_Related_Titles
        {
            get
            {
                if (related_titles_collection == null)
                    related_titles_collection = new List<Related_Titles>();
                return related_titles_collection;
            }
        }

        #endregion

        #region User Tags/Descriptions properties and methods

        /// <summary> Add a new user tag to this item </summary>
        /// <param name="UserID"> Primary key for the user who entered this tag </param>
        /// <param name="UserName"> Name of the user ( Last Name, Firt Name )</param>
        /// <param name="Description_Tag"> Text of the user-entered descriptive tag </param>
        /// <param name="Date_Added"> Date the tag was added or last modified </param>
        /// <param name="TagID"> Primary key for this tag from the database </param>
        public void Add_User_Tag(int UserID, string UserName, string Description_Tag, DateTime Date_Added, int TagID)
        {
            if (tags == null)
                tags = new List<Descriptive_Tag>();

            foreach (Descriptive_Tag thisTag in tags)
            {
                if (thisTag.TagID == TagID)
                {
                    thisTag.Description_Tag = Description_Tag;
                    thisTag.UserID = UserID;
                    thisTag.Date_Added = DateTime.Now;
                    return;
                }
            }

            tags.Add(new Descriptive_Tag(UserID, UserName, Description_Tag, Date_Added, TagID));
        }


        /// <summary> Delete a user tag from this object, by TagID and UserID </summary>
        /// <param name="TagID"> Primary key for this tag from the database </param>
        /// <param name="UserID">  Primary key for the user who entered this tag </param>
        /// <returns> Returns TRUE is successful, otherwise FALSE </returns>
        /// <remarks> This only deletes the user tag if the UserID for the tag matches the provided userid </remarks>
        public bool Delete_User_Tag(int TagID, int UserID)
        {
            if (tags == null)
                return false;

            Descriptive_Tag tag_to_delete = null;
            foreach (Descriptive_Tag thisTag in tags)
            {
                if ((thisTag.TagID == TagID) && (thisTag.UserID == UserID))
                {
                    tag_to_delete = thisTag;
                    break;
                }
            }
            if (tag_to_delete != null)
            {
                tags.Remove(tag_to_delete);
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary> Gets the collection of SobekCM user tags associated with this resource </summary>
        /// <remarks> You should check the count of user tags first using the <see cref="User_Tags_Count"/> before using this property.
        /// Even if there are no user tags, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Descriptive_Tag> User_Tags
        {
            get
            {
                if (tags == null)
                    return new ReadOnlyCollection<Descriptive_Tag>(new List<Descriptive_Tag>());
                else
                    return new ReadOnlyCollection<Descriptive_Tag>(tags);
            }
        }

        /// <summary> Gets the number of overall SobekCM user tags associated with this resource </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="User_Tags"/> property.  Even if 
        /// there are no user tags, the User_Tags property creates a readonly collection to pass back out.</remarks>
        public int User_Tags_Count
        {
            get
            {
                if (tags == null)
                    return 0;
                else
                    return tags.Count;
            }
        }

        #endregion

        #region Tickler (TKR) properties and methods

        /// <summary> List of ticklers associated with this item </summary>
        /// <remarks> You should check the count of ticklers first using the <see cref="Ticklers_Count"/> before using this property.
        /// Even if there are no ticklers, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Ticklers
        {
            get
            {
                if (ticklers == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(ticklers);
            }
        }

        /// <summary> Gets the number of ticklers associated with this item </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Ticklers"/> property.  Even if 
        /// there are no ticklers, the Ticklers property creates a readonly collection to pass back out.</remarks>
        public int Ticklers_Count
        {
            get
            {
                if (ticklers == null)
                    return 0;
                else
                    return ticklers.Count;
            }
        }

        /// <summary> Clear all the ticklers associated with this item </summary>
        public void Clear_Ticklers()
        {
            if (ticklers != null)
                ticklers.Clear();
        }

        /// <summary> Clear all the ticklers associated with this item </summary>
        /// <param name="Tickler"> New tickler to add to this item </param>
        public void Add_Tickler(string Tickler)
        {
            if (Tickler.Length == 0)
                return;

            if (ticklers == null)
                ticklers = new List<string>();

            if (!ticklers.Contains(Tickler.ToUpper()))
                ticklers.Add(Tickler.ToUpper());
        }

        #endregion

        #region Methods for building and returning information about the files in the (online) directory

        /// <summary> Gets the list of all download eligible files for this package </summary>
        /// <param name="Directory"> Directory to check for the files </param>
        /// <returns> List of all download eligible files </returns>
        /// <remarks> This list excludes all page image type files and the standard SobekCM metadata files </remarks>
        public ReadOnlyCollection<string> Get_Download_Eligible_Files(string Directory)
        {
            if (download_eligible_files == null)
            {
                download_eligible_files = new List<string>();

                try
                {
                    string[] files = System.IO.Directory.GetFiles(Directory);
                    foreach (string thisFile in files)
                    {
                        System.IO.FileInfo thisFileInfo = new System.IO.FileInfo(thisFile);
                        string extension = thisFileInfo.Extension.ToUpper();
                        if ((extension.IndexOf("JPG") < 0) && (extension != ".JP2") && (extension != ".TXT") && (extension != ".PRO") && (extension.IndexOf("METS") < 0) && (extension.IndexOf("HTM") < 0))
                        {
                            // Now exclude specific file names.
                            // Part of the reason I am doing this is so users can make their XML files downloadable
                            string filename = thisFileInfo.Name.ToLower();
                            if ((filename != "marc.xml") && (filename != "doc.xml") && (filename != "citation_mets.xml") && (filename != "ufdc_mets.xml") && (filename.IndexOf(".mets.bak") < 0) && ( filename != bibid.ToLower() + "_" + vid + ".html") && ( filename.IndexOf(".mets") < 0 ))
                            {
                                string file_group_name = filename.Replace(thisFileInfo.Extension.ToLower(), ".*");
                                if ( !download_eligible_files.Contains( file_group_name ))
                                    download_eligible_files.Add(file_group_name);
                            }
                        }
                    }
                }
                catch { }
            }

            return new ReadOnlyCollection<string>(download_eligible_files);
        }

        /// <summary> Gets the list of all possible thumbnail jpeg files for this package </summary>
        /// <param name="Directory"> Directory to check for the files </param>
        /// <returns> List of all possible thumbnail jpeg files </returns>
        /// <remarks> This pulls the file list from the directory each time it is called.  In addition, it
        /// sorts by the filename, since this was not always happening correctly and this is primarily used
        /// for the online metadata update.</remarks>
        public ReadOnlyCollection<string> Get_Thumbnail_Files(string Directory)
        {
            SortedList<string, string> thumbnail_files = new SortedList<string, string>();
            try
            {
                string[] files = System.IO.Directory.GetFiles(Directory, "*thm.jpg");
                foreach (string thisFile in files)
                {
                    System.IO.FileInfo thisFileInfo = new System.IO.FileInfo(thisFile);
                    thumbnail_files[ thisFileInfo.Name] = thisFileInfo.Name;
                }
            }
            catch 
            {

            }
            return new ReadOnlyCollection<string>(thumbnail_files.Values);
        }

        #endregion

        #region Simple properties

        /// <summary> Flag indicates if the full text should be included in the static
        /// files generated for search engine indexing robots </summary>
        public bool Expose_Full_Text_For_Harvesting
        {
            get
            {
                return exposeFullTextForHarvesting;
            }
            set
            {
                exposeFullTextForHarvesting = value;
            }
        }

        /// <summary> Gets or sets the notification email for this record </summary>
        public string NotifyEmail
        {
            get { return notifyEmail ?? String.Empty; }
            set { notifyEmail = value; }
        }

        /// <summary> Gets or sets the GUID for this item </summary>
        public string GUID
        {
            get { return guid ?? String.Empty; }
            set { guid = value; }
        }

        /// <summary> Gets and sets the name of the main thumbnail file </summary>
        public string Main_Thumbnail
        {
            get { return mainThumbnail ?? String.Empty; }
            set { mainThumbnail = value; }
        }

        /// <summary> Flag determines if this item should be suppressed in the Endeca feed of digital resources </summary>
        public bool Suppress_Endeca
        {
            get { return suppressEndeca; }
            set { suppressEndeca = value; }
        }

        /// <summary> Flag determines if this item can be described online by logged in users </summary>
        /// <remarks> This value is set based on the values for all the item aggregations to which this item belongs </remarks>
        public bool Can_Be_Described
        {
            get { return can_be_described; }
            set { can_be_described = value; }
        }

        /// <summary> Flag determines if this item is publicly accessible or only accessible to internal users </summary>
        public bool Publicly_Accessible
        {
            get { return ( ip_restricted >= 0 ); }
        }

        /// <summary> Flag determines if validation errors should be shown </summary>
        public bool Show_Validation_Errors
        {
            get { return show_validation_errors; }
            set { show_validation_errors = value; }
        }

        /// <summary> Flag determines if this item is reserved for single-item use online </summary>
        public bool CheckOut_Required
        {
            get 
            {
                if (checkOutRequired.HasValue)
                    return checkOutRequired.Value;
                else
                    return false;
            }
            set { checkOutRequired = value; }
        }

        /// <summary> Flag indicates if a checkout required value is provided, or if that
        /// flag is currently NULL.</summary>
        /// <remarks> Setting this property to TRUE causes the flag to become NULL. </remarks>
        public bool CheckOut_Required_Is_Null
        {
            get
            {
                return checkOutRequired.HasValue;
            }
            set
            {
                if (value)
                    checkOutRequired = null;
            }
        }

        /// <summary> Flag determines if this item is currently set to DARK and can never be made
        /// public or have files added online </summary>
        public bool Dark_Flag
        {
            get
            {
                if (dark_flag.HasValue)
                    return dark_flag.Value;
                else
                    return false;
            }
            set { dark_flag = value; }
        }

        /// <summary> Flag indicates if a dark flag value is provided, or if that
        /// flag is currently NULL.</summary>
        /// <remarks> Setting this property to TRUE causes the flag to become NULL. </remarks>
        public bool Dark_Flag_Is_Null
        {
            get
            {
                return dark_flag.HasValue;
            }
            set
            {
                if (value)
                    dark_flag = null;
            }
        }

        /// <summary> Bitwise flags determines if this item should be restricted to certain IP ranges </summary>
        public short IP_Restriction_Membership
        {
            get 
            {
                if (ip_restricted.HasValue)
                    return ip_restricted.Value;
                else
                    return -1;
            }
            set { ip_restricted = value; }
        }


        /// <summary> Flag indicates if a value is provided for the current IP Restriction Membership,
        /// or if that value is currently NULL.</summary>
        /// <remarks> Setting this property to TRUE causes the value to become NULL. </remarks>
        public bool IP_Restriction_Membership_Is_Null
        {
            get
            {
                return ip_restricted.HasValue;
            }
            set
            {
                if (value)
                    ip_restricted = null;
            }
        }
        
        /// <summary> Sets the greenstone image root  </summary>
        public string Image_Root
        {
            set { imageRoot = value; }
            get { return imageRoot; }
        }

        /// <summary> Gets the information about the main page for this item </summary>
        public Main_Page_Info Main_Page
        {
            get
            {
                if (mainPage == null)
                    mainPage = new Main_Page_Info();

                return mainPage;
            }
        }


        /// <summary> Gets and sets the group type for this item from the SobekCM Web database </summary>
        public string GroupType
        {
            get { return groupType ?? String.Empty; }
            set { groupType = value; }
        }

        /// <summary> Gets and sets the group title for this item from the SobekCM Web database </summary>
        public string GroupTitle
        {
            get { return groupTitle ?? String.Empty; }
            set { groupTitle = value; }
        }

        /// <summary> Gets and sets the associated file path for this item from the SobekCM Web database </summary>
        public string AssocFilePath
        {
            get { return assocFilePath ?? String.Empty; }
            set { assocFilePath = value; }
        }

        /// <summary> Gets and sets the greenstone code for this item from the SobekCM Web database </summary>
        public string Greenstone_Code
        {
            get { return greenstoneCode ?? String.Empty; }
            set { greenstoneCode = value; }
        }

        /// <summary> Gets and sets the file root for this item from the SobekCM Web database </summary>
        public string File_Root
        {
            get { return fileLocation ?? String.Empty; }
            set { fileLocation = value; }
        }

        /// <summary> Gets and sets the text searchable flag for this item from the SobekCM Web database </summary>
        public bool Text_Searchable
        {
            get { return textSearchable; }
            set { textSearchable = value; }
        }

        /// <summary> Gets and sets the group id for this item from the SobekCM Web database </summary>
        public int GroupID
        {
            get { return groupID; }
            set { groupID = value; }
        }

        /// <summary> Gets and sets the item id for this item from the SobekCM Web database </summary>
        public int ItemID
        {
            get { return itemID; }
            set { itemID = value; }
        }

        /// <summary> Gets and sets the item id for this item from the SobekCM Web database </summary>
        public Nullable<int> Siblings
        {
            get { return siblings; }
            set { siblings = value; }
        }

        /// <summary> Gets and sets the static page count for this item from the SobekCM Web database </summary>
        public int Static_PageCount
        {
            get { return pageCount; }
            set { pageCount = value; }
        }

        /// <summary> Gets and sets the static division count for this item from the SobekCM Web database </summary>
        public int Static_Division_Count
        {
            get { return divCount; }
            set { divCount = value; }
        }

        #endregion

        /// <summary> Sets the type and identifier for the primary alternate identifier associated with this item group </summary>
        /// <param name="Type"> Type of the primary alternate identifier </param>
        /// <param name="Identifier"> Primary alternate identifier </param>
        public void Set_Primary_Identifier(string Type, string Identifier)
        {
            primaryIdentifier.Type = Type;
            primaryIdentifier.Identifier = Identifier;
        }

        /// <summary> Get the primary alternate identifier associated with this item group </summary>
        public Bib_Info.Identifier_Info Primary_Identifier
        {
            get
            {
                return primaryIdentifier;
            }
        }

        /// <summary> Sets the BibID and VID in this child object, which is used to determine the default folder structure </summary>
        /// <param name="BibID"> BibID </param>
        /// <param name="VID"> VID </param>
        public void Set_BibID_VID(string BibID, string VID)
        {
            bibid = BibID;
            vid = VID;
        }

        /// <summary> Gets  the Greenstone source URL </summary>
        public string Source_URL
        {
            get 
            {
                if ((assocFilePath == null) || (assocFilePath.Length == 0))
                {
                    assocFilePath = bibid.Substring(0, 2) + "/" + bibid.Substring(2, 2) + "/" + bibid.Substring(4, 2) + "/" + bibid.Substring(6, 2) + "/" + bibid.Substring(8, 2) + "/" + vid;
                }
                string assocFilePath_URLstyle = assocFilePath.Replace("\\", "/");
                if ((assocFilePath_URLstyle.Length > 0) && (assocFilePath_URLstyle[assocFilePath_URLstyle.Length - 1] == '/'))
                {
                    assocFilePath_URLstyle = assocFilePath_URLstyle.Substring(0, assocFilePath_URLstyle.Length - 1);
                }
                if (imageRoot != null)
                {
                    if (imageRoot.IndexOf(assocFilePath_URLstyle) >= 0)
                        return imageRoot;
                    else
                        return imageRoot + assocFilePath_URLstyle;
                }
                else
                {
                    return assocFilePath_URLstyle;
                }
            }
        }

        /// <summary> Gets the Greenstone URL Query </summary>
        public string Greenstone_URL_Query_String
        {
            get { return "e=d-000-00---0" + greenstoneCode + "--00-1-0--0prompt-10---4------0-1l--1-en-50---20-about---00131-001-1-0utfZz-8-00&d=" + this.bibid.ToUpper() + this.vid; }
        }



        /// <summary> Adds to the collection of pages by sequence </summary>
        /// <param name="Next_Page"> Next page in sequence to add </param>
        public void Add_Pages_By_Sequence(Page_TreeNode Next_Page)
        {
            if (pages_by_seq == null)
                pages_by_seq = new List<Page_TreeNode>();
            pages_by_seq.Add(Next_Page);
        }

        /// <summary> Gets the collection of pages by sequence </summary>
        public ReadOnlyCollection<Page_TreeNode> Pages_By_Sequence
        {
            get
            {
                if (pages_by_seq == null)
                    return new ReadOnlyCollection<Page_TreeNode>(new List<Page_TreeNode>());
                else
                    return new ReadOnlyCollection<Page_TreeNode>(pages_by_seq);
            }
        }

        #region Methods used by the SobekCM web application to get a valid viewer by code 

        /// <summary> Gets the collection of pages by sequence </summary>
        public Dictionary<string, SobekCM_File_Info> Viewer_To_File
        {
            get
            {
                if (viewer_to_file == null)
                    viewer_to_file = new Dictionary<string, SobekCM_File_Info>();


                return viewer_to_file;
            }
        }


        /// <summary> Gets a valid viewer code for this item, based on the requested viewer code and page </summary>
        /// <param name="viewer_code"> Requested viewer code </param>
        /// <param name="page"> Requested page </param>
        /// <returns> Valid viewer code and page </returns>
        /// <remarks> The requested viewer code and page are validated, and if valid, returned.  Otherwise, the closest valid viewer code to the one requested is returned. </remarks>
        public string Get_Valid_Viewer_Code(string viewer_code, int page)
        {
            string lower_code = viewer_code.ToLower();

            // If this is 'RES' for restricted, taht is always valid
            if (viewer_code == "res")
                return "res";

            // Is this in the item level viewer list?
            if (lower_code.Length > 0)
            {
                if (views != null)
                {
                    foreach (View_Object thisView in views)
                    {
                        foreach (string thisCode in thisView.Viewer_Codes)
                        {
                            if (thisCode.ToLower() == lower_code)
                            {
                                return lower_code;
                            }
                        }
                    }
                }

                // Check if this is a Related Items page, which also allows paging
                if (lower_code.IndexOf(View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0]) >= 0)
                {
                    int viewer_page = 0;
                    try
                    {
                        string try_page_get = lower_code.Replace(View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0], "").Trim();
                        if (try_page_get.Length > 0)
                        {
                            viewer_page = Convert.ToInt32(try_page_get);
                        }
                        return View_Object.Viewer_Code_By_Type(View_Enum.RELATED_IMAGES)[0];
                    }
                    catch
                    {

                    }
                }

                // Check each page
                if (this.viewer_to_file != null)
                {
                    if (this.viewer_to_file.ContainsKey(lower_code))
                    {
                        return lower_code;
                    }
                }
            }

            // No match, so just return the default, if there is one
            if ((viewer_code.Length == 0) && (defaultView != null) && (defaultView.Viewer_Codes.Length > 0))
            {
                return defaultView.Viewer_Codes[0];
            }

            // If there are no pages, return the DEFAULT viewer, or FULL CITATION code
            if ((this.viewer_to_file == null) || (viewer_to_file.Count == 0))
            {
                if ((defaultView != null) && (defaultView.Viewer_Codes.Length > 0))
                    return Default_View.Viewer_Codes[0];
                else
                    return View_Object.Viewer_Code_By_Type(View_Enum.CITATION)[0];
            }


            if ((item_level_page_views != null ) && (item_level_page_views.Count == 0))
            {
                item_level_page_views.Add(new View_Object(View_Enum.JPEG2000));
                item_level_page_views.Add(new View_Object(View_Enum.JPEG));
            }

            // Return the first viewer code for this page
            if (( pages_by_seq != null ) && ( item_level_page_views != null ) && (pages_by_seq.Count >= page) && (pages_by_seq[page - 1].Files.Count > 0))
            {
                foreach (View_Object itemTaggedView in item_level_page_views)
                {
                    foreach (SobekCM_File_Info thisFile in pages_by_seq[page - 1].Files)
                    {
                        if (thisFile.Get_Viewer() != null)
                        {
                            if (thisFile.Get_Viewer().View_Type == itemTaggedView.View_Type)
                            {
                                return page.ToString() + thisFile.Get_Viewer().Viewer_Codes[0];
                            }
                        }
                    }
                }
            }

            // Look for FLASH or PDF views if no page was requested
            if (views != null)
            {
                foreach (View_Object thisView in views)
                {
                    if ((thisView.View_Type == View_Enum.PDF) || (thisView.View_Type == View_Enum.FLASH))
                    {
                        return thisView.Viewer_Codes[0];

                    }
                }
            }

            // Return first page viewer code
            if ((pages_by_seq != null) && (item_level_page_views != null) && (pages_by_seq.Count > 0) && (pages_by_seq[0].Files.Count > 0))
            {
                foreach (View_Object itemTaggedView in item_level_page_views)
                {
                    foreach (SobekCM_File_Info thisFile in pages_by_seq[0].Files)
                    {
                        if (thisFile.Get_Viewer() != null)
                        {
                            if (thisFile.Get_Viewer().View_Type == itemTaggedView.View_Type)
                            {
                                return "1" + thisFile.Get_Viewer().Viewer_Codes[0];
                            }
                        }
                    }
                }
            }

            return View_Object.Viewer_Code_By_Type(View_Enum.CITATION)[0];
        }

        /// <summary> Gets the view object from this item based on the requested viewer code  </summary>
        /// <param name="viewer_code"> Viewer code for the viewer requested </param>
        /// <returns> Valid view object from this item, based on requested viewer code, or NULL </returns>
        public View_Object Get_Viewer(string viewer_code)
        {
            string lower_code = viewer_code.ToLower();

            // If this is for the restricted viewer, that is always valid
            if (lower_code == "res")
            {
                return new View_Object(View_Enum.RESTRICTED);
            }

            // If this was for the full citation, jsut return that
            if (lower_code.IndexOf("citation") == 0)
            {
                return new View_Object(View_Enum.CITATION);
            }

            // Is this in the item level viewer list?
            if (views != null)
            {
                foreach (View_Object thisView in views)
                {
                    foreach (string thisCode in thisView.Viewer_Codes)
                    {
                        if (thisCode.ToLower() == lower_code)
                        {
                            return thisView;
                        }
                    }
                }
            }

            // Check each page
            if ((this.viewer_to_file != null) && (this.viewer_to_file.ContainsKey(lower_code)))
            {
                return viewer_to_file[lower_code].Get_Viewer();
            }

            // No match, so just return the default..
            if (defaultView != null) 
            {
                return defaultView;
            }

             // Return first page viewer
            if ((pages_by_seq != null ) && (pages_by_seq.Count > 0) && (pages_by_seq[0].Files.Count > 0))
            {
                SobekCM_File_Info first_page = pages_by_seq[0].Files[0];
                View_Object firstPageViewer = first_page.Get_Viewer();
            }

            // If there is a single view, return the first code for it
            if (( views != null ) && ( views.Count > 0))
            {
                return views[0];
            }

            // Return null
            return null;
        }

        #endregion

        internal void Calculate_GUID(string BibID, string License)
        {
            if ((License.Length > 0) && (BibID.Length == 10))
            {
                byte[] bytIn = System.Text.ASCIIEncoding.ASCII.GetBytes(License);

                // create a MemoryStream so that the process can be done without I/O files
                System.IO.MemoryStream ms = new System.IO.MemoryStream();

                // set the private key
                System.Security.Cryptography.DESCryptoServiceProvider DESProvider = new System.Security.Cryptography.DESCryptoServiceProvider();
                DESProvider.Key = ASCIIEncoding.ASCII.GetBytes("U7+x$Swa");
                DESProvider.IV = ASCIIEncoding.ASCII.GetBytes(BibID.Substring(2));

                // create an Encryptor from the Provider Service instance
                System.Security.Cryptography.ICryptoTransform encrypto = DESProvider.CreateEncryptor();

                // create Crypto Stream that transforms a stream using the encryption
                System.Security.Cryptography.CryptoStream cs = new System.Security.Cryptography.CryptoStream(ms, encrypto, System.Security.Cryptography.CryptoStreamMode.Write);

                // write out encrypted content into MemoryStream
                cs.Write(bytIn, 0, bytIn.Length);
                cs.Close();

                // Write out from the Memory stream to an array of bytes
                byte[] bytOut = ms.ToArray();
                ms.Close();

                // convert into Base64 so that the result can be used in xml
                guid = System.Convert.ToBase64String(bytOut, 0, bytOut.Length);
            }
        }

        /// <summary> Sets the bibliographic identifier </summary>
        internal string BibID
        {
            set { bibid = value; }
        }

        /// <summary> Sets the volume identifier </summary>
        internal string VID
        {
            set { vid = value; }
        }

        #region Methods used to write to METS files's behaviorSec and MODS metadata sections

        /// <summary> Returns the METS formatted XML for all the processing parameters for SobekCM </summary>
        /// <param name="sobekcm_namespace">METS extension schema namespace to use (SobekCM, DLOC, etc..)</param>
        /// <param name="results">  Stream to write the processing information to </param>
        internal void Add_METS_Processing_Metadata(string sobekcm_namespace, System.IO.TextWriter results)
        {
            // Start the Administrative section
            results.Write("<" + sobekcm_namespace + ":procParam>\r\n");

            if (aggregations != null)
            {
                foreach (Aggregation_Info aggregation in aggregations)
                {
                    if (aggregation.Code.Length > 0)
                    {
                        results.Write(toMETS(sobekcm_namespace + ":Aggregation", base.Convert_String_To_XML_Safe(aggregation.Code)));
                    }
                }
            }

            // Add the main page information
            if (mainPage != null)
            {
                mainPage.Add_METS(sobekcm_namespace, results);
            }

            results.Write(toMETS(sobekcm_namespace + ":MainThumbnail", base.Convert_String_To_XML_Safe(mainThumbnail)));

            // Add the icon information
            if (wordmarks != null)
            {
                foreach (Wordmark_Info thisIcon in wordmarks)
                {
                    results.Write("<" + sobekcm_namespace + ":Wordmark>" + base.Convert_String_To_XML_Safe(thisIcon.Code.ToUpper()) + "</" + sobekcm_namespace + ":Wordmark>\r\n");
                }
            }

            // Add the GUID, if there is one
            if (!String.IsNullOrEmpty(guid))
            {
                results.Write(toMETS(sobekcm_namespace + ":GUID", guid));
            }

            // Add the notification email if there is one
            if (String.IsNullOrEmpty(notifyEmail))
            {
                results.Write(toMETS(sobekcm_namespace + ":NotifyEmail", notifyEmail));
            }

            // End the Administrative section
            results.Write("</" + sobekcm_namespace + ":procParam>\r\n");
        }

        /// <summary> Returns a single value in METS formatted XML </summary>
        /// <param name="mets_tag">Tag for the mets element</param>
        /// <param name="mets_value">Value for the mets element</param>
        /// <returns>METS formatted XML</returns>
        internal static string toMETS(string mets_tag, string mets_value)
        {
            if (!String.IsNullOrEmpty(mets_value))
            {
                return "<" + mets_tag + ">" + mets_value + "</" + mets_tag + ">\r\n";
            }
            else
            {
                return String.Empty;
            }
        }

        /// <summary> Returns the METS behavior section associated with this resource </summary>
        internal void Add_BehaviorSec_METS( System.IO.TextWriter behaviorSec, bool package_includes_image_files )
        {
            // Were any views specified?
            if (((views != null) && (views.Count > 0)) || ((item_level_page_views != null ) && (item_level_page_views.Count > 0)))
            {
                // Remove invalid views (i.e., legacy JPEG and JP2 views if no JPEG and/or JP2 )
                if (!package_includes_image_files)
                {
                    // Remove page image views
                    List<View_Object> deletes = new List<View_Object>();
                    if (views != null)
                    {
                        foreach (View_Object thisViewObject in views)
                        {
                            switch (thisViewObject.View_Type)
                            {
                                case View_Enum.JPEG:
                                case View_Enum.JPEG2000:
                                case View_Enum.TEXT:
                                    deletes.Add(thisViewObject);
                                    break;
                            }
                        }
                        foreach (View_Object deleteView in deletes)
                            views.Remove(deleteView);
                    }

                    // Remove from item_level_page_views
                    deletes.Clear();
                    if (item_level_page_views != null)
                    {
                        foreach (View_Object thisViewObject in item_level_page_views)
                        {
                            switch (thisViewObject.View_Type)
                            {
                                case View_Enum.JPEG:
                                case View_Enum.JPEG2000:
                                case View_Enum.TEXT:
                                    deletes.Add(thisViewObject);
                                    break;
                            }
                        }
                        foreach (View_Object deleteView in deletes)
                            item_level_page_views.Remove(deleteView);
                    }
                }

                // Start the behavior section for views
                behaviorSec.Write("<METS:behaviorSec ID=\"VIEWS\" LABEL=\"Options available to the user for viewing this item\" >\r\n");

                // Add each view behavior
                List<View_Enum> views_added = new List<View_Enum>();
                int view_count = 1;
                if (views != null)
                {
                    foreach (View_Object thisView in views)
                    {
                        if (!views_added.Contains(thisView.View_Type))
                        {
                            // Add this METS data
                            thisView.Add_METS(behaviorSec, view_count++);
                            views_added.Add(thisView.View_Type);
                        }
                    }
                }

                // Add each item level page view
                if (item_level_page_views != null)
                {
                    foreach (View_Object thisView in item_level_page_views)
                    {
                        if (!views_added.Contains(thisView.View_Type))
                        {
                            // Add this METS data
                            thisView.Add_METS(behaviorSec, view_count++);
                            views_added.Add(thisView.View_Type);
                        }
                    }
                }

                // End this behavior section
                behaviorSec.Write("</METS:behaviorSec>\r\n");
            }
 
            // Add all the webskins
            // Start the behavior section for views
            if ((webskins != null) && (webskins.Count > 0))
            {
                behaviorSec.Write("<METS:behaviorSec ID=\"INTERFACES\" LABEL=\"Banners or webskins which this resource can appear under\" >\r\n");

                // Add each behavior
                int interface_count = 1;
                foreach (string thisInterface in webskins)
                {
                    // Start this behavior
                    if (interface_count == 1)
                    {
                        behaviorSec.Write("<METS:behavior GROUPID=\"INTERFACES\" ID=\"INT1\" LABEL=\"Default Interface\">\r\n");
                    }
                    else
                    {
                        behaviorSec.Write("<METS:behavior GROUPID=\"INTERFACES\" ID=\"INT" + interface_count.ToString() + "\" LABEL=\"Alternate Interface\">\r\n");
                    }

                    // Increment the count to prepare for the next one
                    interface_count++;

                    // Add the actual behavior mechanism
                    behaviorSec.Write("<METS:mechanism LABEL=\"" + thisInterface + " Interface\" LOCTYPE=\"OTHER\" OTHERLOCTYPE=\"SobekCM Procedure\" xlink:type=\"simple\" xlink:title=\"" + thisInterface + "_Interface_Loader\" />\r\n");

                    // End this behavior
                    behaviorSec.Write("</METS:behavior>\r\n");
                }

                // End this behavior section
                behaviorSec.Write("</METS:behaviorSec>\r\n");
            }
        }

        #endregion
    }
}
