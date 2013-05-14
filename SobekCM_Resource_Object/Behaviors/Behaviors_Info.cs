#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Resource_Object.Behaviors
{
    /// <summary> Class holds all the non-descriptive behavior information, such as under which
    /// aggregations this item should appear, which web skins, etc..</summary>
    /// <remarks>  Depending on the currently METS-writing profile, much of this data 
    /// can also be written into a METS file, which differentiates this object from the
    /// Web_Info object, which is utilized by the web server to assist with some rendering
    /// and to generally improve performance by calculating and saving several values
    /// when building the resource object.  </remarks>
    [Serializable]
    public class Behaviors_Info : XML_Writing_Base_Type
    {
        private List<Aggregation_Info> aggregations;
        private List<string> ticklers;
        private List<View_Object> views;
        private View_Object defaultView;
        private List<string> webskins;
        private List<Wordmark_Info> wordmarks;
        private Identifier_Info primaryIdentifier;
        private List<Descriptive_Tag> tags;    

        private bool can_be_described;
        private Nullable<bool> checkOutRequired;
        private Nullable<bool> dark_flag;
        private bool exposeFullTextForHarvesting;
        private Nullable<short> ip_restricted;
        private string mainThumbnail;
        private string notifyEmail;
        private bool textSearchable;

        private string embeddedVideo;
        
        private string groupTitle;
        private string groupType;
        private string guid;

        private List<View_Object> item_level_page_views;
        private Main_Page_Info mainPage;
        private bool suppressEndeca;
        private Serial_Info serialInfo;

        
        /// <summary> Constructor for a new instance of the Behaviors_Info class </summary>
        public Behaviors_Info()
        {
            textSearchable = false;
            checkOutRequired = false;
            ip_restricted = 0;
            suppressEndeca = true;
            can_be_described = false;
            dark_flag = false;
            exposeFullTextForHarvesting = true;
            primaryIdentifier = new Identifier_Info();
        }

        /// <summary> Flag indicates if this item should be displayed Left-to-Right, rather than the
        /// default Right-to-Left. </summary>
        /// <remarks> This adds support for page turning for languages such as Yiddish </remarks>
        public bool Left_To_Right
        {
            get { return false; }
        }

        /// <summary> Gets flag which indicates if there is any serial hierarchy information in this object </summary>
        public bool hasSerialInformation
        {
            get
            {
                if (serialInfo == null)
                    return false;
                else
                    return true;
            }
        }

        /// <summary> Gets the serial hierarchy information associated with this resource </summary>
        public Serial_Info Serial_Info
        {
            get
            {
                if (serialInfo == null)
                    serialInfo = new Serial_Info();

                return serialInfo;
            }
        }

        /// <summary> Get the primary alternate identifier associated with this item group </summary>
        public Identifier_Info Primary_Identifier
        {
            get { return primaryIdentifier; }
        }

        #region Methods used to write to METS files's behaviorSec and MODS metadata sections

        /// <summary> Returns the METS formatted XML for all the processing parameters for SobekCM </summary>
        /// <param name="sobekcm_namespace">METS extension schema namespace to use (SobekCM, DLOC, etc..)</param>
        /// <param name="results">  Stream to write the processing information to </param>
        internal void Add_METS_Processing_Metadata(string sobekcm_namespace, TextWriter results)
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

            // Add ticklers
            if ((ticklers != null) && (ticklers.Count > 0))
            {
                foreach (string thisTickler in ticklers)
                {
                    results.Write(toMETS(sobekcm_namespace + ":Tickler", thisTickler));
                }
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
        internal void Add_BehaviorSec_METS(TextWriter behaviorSec, bool package_includes_image_files)
        {
            // Were any views specified?
            if (((views != null) && (views.Count > 0)) || ((item_level_page_views != null) && (item_level_page_views.Count > 0)))
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

        #region Wordmark/icon properties and methods

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

        /// <summary> Dedupes the wordmarks to ensure that no duplication is occurring </summary>
        /// <remarks> Saving duped wordmarks to the database can result in a database exception during the save process </remarks>
        public void Dedupe_Wordmarks()
        {
            if ((wordmarks != null) && (wordmarks.Count > 1))
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
        public void Add_Aggregation(string Code, string Name)
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

        #endregion

        #region Web Skin properties and methods

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

        /// <summary> Checks to see if the viewer type is in use for this digital resource </summary>
        /// <param name="viewerEnum"> Viewer type to check for </param>
        /// <returns> TRUE if this item has a viewer of that type, otherwise FALSE </returns>
        public bool Has_Viewer_Type( View_Enum viewerEnum )
        {
            if (views == null) return false;
            return views.Any(thisView => thisView.View_Type == viewerEnum);
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

        #endregion

        #region User Tags/Descriptions properties and methods

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

        #region Simple properties

        /// <summary> Flag indicates if the full text should be included in the static
        /// files generated for search engine indexing robots </summary>
        public bool Expose_Full_Text_For_Harvesting
        {
            get { return exposeFullTextForHarvesting; }
            set { exposeFullTextForHarvesting = value; }
        }

        /// <summary> Gets or sets the notification email for this record </summary>
        public string NotifyEmail
        {
            get { return notifyEmail ?? String.Empty; }
            set { notifyEmail = value; }
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
            get { return (ip_restricted >= 0); }
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
            get { return checkOutRequired.HasValue; }
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
            get { return dark_flag.HasValue; }
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
            get { return ip_restricted.HasValue; }
            set
            {
                if (value)
                    ip_restricted = null;
            }
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

        /// <summary> Gets and sets the text searchable flag for this item from the SobekCM Web database </summary>
        public bool Text_Searchable
        {
            get { return textSearchable; }
            set { textSearchable = value; }
        }

        /// <summary> Gets and sets the embedded video code </summary>
        public string Embedded_Video
        {
            get { return embeddedVideo ?? String.Empty; }
            set { embeddedVideo = value; }
        }

        #endregion

        /// <summary> Saves the data stored in this instance of the 
        /// element to the provided bibliographic object </summary>
        /// <param name="serialHierarchyObject">Serial hierarchy object to set to this object </param>
        /// <param name="SerialInfo"> Serial information to set to this object </param>
        public void Set_Serial_Info(Serial_Info SerialInfo)
        {
            serialInfo = SerialInfo;
        }

        /// <summary> Sets the type and identifier for the primary alternate identifier associated with this item group </summary>
        /// <param name="Type"> Type of the primary alternate identifier </param>
        /// <param name="Identifier"> Primary alternate identifier </param>
        public void Set_Primary_Identifier(string Type, string Identifier)
        {
            primaryIdentifier.Type = Type;
            primaryIdentifier.Identifier = Identifier;
        }

        internal void Calculate_GUID(string BibID, string License)
        {
            if ((License.Length > 0) && (BibID.Length == 10))
            {
                byte[] bytIn = ASCIIEncoding.ASCII.GetBytes(License);

                // create a MemoryStream so that the process can be done without I/O files
                MemoryStream ms = new MemoryStream();

                // set the private key
                DESCryptoServiceProvider DESProvider = new DESCryptoServiceProvider();
                DESProvider.Key = ASCIIEncoding.ASCII.GetBytes("U7+x$Swa");
                DESProvider.IV = ASCIIEncoding.ASCII.GetBytes(BibID.Substring(2));

                // create an Encryptor from the Provider Service instance
                ICryptoTransform encrypto = DESProvider.CreateEncryptor();

                // create Crypto Stream that transforms a stream using the encryption
                CryptoStream cs = new CryptoStream(ms, encrypto, CryptoStreamMode.Write);

                // write out encrypted content into MemoryStream
                cs.Write(bytIn, 0, bytIn.Length);
                cs.Close();

                // Write out from the Memory stream to an array of bytes
                byte[] bytOut = ms.ToArray();
                ms.Close();

                // convert into Base64 so that the result can be used in xml
                guid = Convert.ToBase64String(bytOut, 0, bytOut.Length);
            }
        }
    }
}