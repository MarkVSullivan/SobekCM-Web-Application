using System;
using System.IO;
using SobekCM.Core.Configuration;
using SobekCM.Library.Localization.Classes;

namespace SobekCM.Library.Localization
{
    public class SobekCM_LocalizationInfo
    {
		/// <summary> Language of this localization information </summary>
		public Web_Language_Enum Language { get; set; }

		#region Private members that contain the localization strings for each class 

        /// <summary> Localization string information for the General class </summary>
        public General_LocalizationInfo General { get ; private set; }

        /// <summary> Localization string information for the Html_MainWriter class </summary>
        public Html_MainWriter_LocalizationInfo Html_MainWriter { get ; private set; }

        /// <summary> Localization string information for the URL_Email_Helper class </summary>
        public URL_Email_Helper_LocalizationInfo URL_Email_Helper { get ; private set; }

        /// <summary> Localization string information for the Item_Email_Helper class </summary>
        public Item_Email_Helper_LocalizationInfo Item_Email_Helper { get ; private set; }

        /// <summary> Localization string information for the Usage_Stats_Email_Helper class </summary>
        public Usage_Stats_Email_Helper_LocalizationInfo Usage_Stats_Email_Helper { get ; private set; }

        /// <summary> Localization string information for the Aggregation_HtmlSubwriter class </summary>
        public Aggregation_HtmlSubwriter_LocalizationInfo Aggregation_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Aggregation_Nav_Bar_HTML_Factory class </summary>
        public Aggregation_Nav_Bar_HTML_Factory_LocalizationInfo Aggregation_Nav_Bar_HTML_Factory { get ; private set; }

        /// <summary> Localization string information for the Advanced_Search_AggregationViewer class </summary>
        public Advanced_Search_AggregationViewer_LocalizationInfo Advanced_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Basic_Search_AggregationViewer class </summary>
        public Basic_Search_AggregationViewer_LocalizationInfo Basic_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the dLOC_Search_AggregationViewer class </summary>
        public dLOC_Search_AggregationViewer_LocalizationInfo dLOC_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Full_Text_Search_AggregationViewer class </summary>
        public Full_Text_Search_AggregationViewer_LocalizationInfo Full_Text_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Item_Count_AggregationViewer class </summary>
        public Item_Count_AggregationViewer_LocalizationInfo Item_Count_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Map_Browse_AggregationViewer class </summary>
        public Map_Browse_AggregationViewer_LocalizationInfo Map_Browse_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Map_Search_AggregationViewer class </summary>
        public Map_Search_AggregationViewer_LocalizationInfo Map_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Metadata_Browse_AggregationViewer class </summary>
        public Metadata_Browse_AggregationViewer_LocalizationInfo Metadata_Browse_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Newspaper_Search_AggregationViewer class </summary>
        public Newspaper_Search_AggregationViewer_LocalizationInfo Newspaper_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the No_Search_AggregationViewer class </summary>
        public No_Search_AggregationViewer_LocalizationInfo No_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Private_Items_AggregationViewer class </summary>
        public Private_Items_AggregationViewer_LocalizationInfo Private_Items_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Rotating_Highlight_Search_AggregationViewer class </summary>
        public Rotating_Highlight_Search_AggregationViewer_LocalizationInfo Rotating_Highlight_Search_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Usage_Statistics_AggregationViewer class </summary>
        public Usage_Statistics_AggregationViewer_LocalizationInfo Usage_Statistics_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Contact_HtmlSubwriter class </summary>
        public Contact_HtmlSubwriter_LocalizationInfo Contact_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Error_HtmlSubwriter class </summary>
        public Error_HtmlSubwriter_LocalizationInfo Error_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Internal_HtmlSubwriter class </summary>
        public Internal_HtmlSubwriter_LocalizationInfo Internal_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the LegacyUrl_HtmlSubwriter class </summary>
        public LegacyUrl_HtmlSubwriter_LocalizationInfo LegacyUrl_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Preferences_HtmlSubwriter class </summary>
        public Preferences_HtmlSubwriter_LocalizationInfo Preferences_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Print_Item_HtmlSubwriter class </summary>
        public Print_Item_HtmlSubwriter_LocalizationInfo Print_Item_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Public_Folder_HtmlSubwriter class </summary>
        public Public_Folder_HtmlSubwriter_LocalizationInfo Public_Folder_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Search_Results_HtmlSubwriter class </summary>
        public Search_Results_HtmlSubwriter_LocalizationInfo Search_Results_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Statistics_HtmlSubwriter class </summary>
        public Statistics_HtmlSubwriter_LocalizationInfo Statistics_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Item_HtmlSubwriter class </summary>
        public Item_HtmlSubwriter_LocalizationInfo Item_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the AddRemove_Fragment_ItemViewer class </summary>
        public AddRemove_Fragment_ItemViewer_LocalizationInfo AddRemove_Fragment_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Describe_Fragment_ItemViewer class </summary>
        public Describe_Fragment_ItemViewer_LocalizationInfo Describe_Fragment_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the PrintForm_Fragment_ItemViewer class </summary>
        public PrintForm_Fragment_ItemViewer_LocalizationInfo PrintForm_Fragment_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Send_Fragment_ItemViewer class </summary>
        public Send_Fragment_ItemViewer_LocalizationInfo Send_Fragment_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Share_Fragment_ItemViewer class </summary>
        public Share_Fragment_ItemViewer_LocalizationInfo Share_Fragment_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Checked_Out_ItemViewer class </summary>
        public Checked_Out_ItemViewer_LocalizationInfo Checked_Out_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Citation_ItemViewer class </summary>
        public Citation_ItemViewer_LocalizationInfo Citation_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Download_ItemViewer class </summary>
        public Download_ItemViewer_LocalizationInfo Download_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the EAD_Container_List_ItemViewer class </summary>
        public EAD_Container_List_ItemViewer_LocalizationInfo EAD_Container_List_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the EAD_Description_ItemViewer class </summary>
        public EAD_Description_ItemViewer_LocalizationInfo EAD_Description_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the EmbeddedVideo_ItemViewer class </summary>
        public EmbeddedVideo_ItemViewer_LocalizationInfo EmbeddedVideo_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the GnuBooks_PageTurner_ItemViewer class </summary>
        public GnuBooks_PageTurner_ItemViewer_LocalizationInfo GnuBooks_PageTurner_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Google_Map_ItemViewer class </summary>
        public Google_Map_ItemViewer_LocalizationInfo Google_Map_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the HTML_Map_ItemViewer class </summary>
        public HTML_Map_ItemViewer_LocalizationInfo HTML_Map_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the HTML_ItemViewer class </summary>
        public HTML_ItemViewer_LocalizationInfo HTML_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Aware_JP2_ItemViewer class </summary>
        public Aware_JP2_ItemViewer_LocalizationInfo Aware_JP2_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the MultiVolumes_ItemViewer class </summary>
        public MultiVolumes_ItemViewer_LocalizationInfo MultiVolumes_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the PDF_ItemViewer class </summary>
        public PDF_ItemViewer_LocalizationInfo PDF_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the QC_ItemViewer class </summary>
        public QC_ItemViewer_LocalizationInfo QC_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Related_Images_ItemViewer class </summary>
        public Related_Images_ItemViewer_LocalizationInfo Related_Images_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the YouTube_Embedded_Video_ItemViewer class </summary>
        public YouTube_Embedded_Video_ItemViewer_LocalizationInfo YouTube_Embedded_Video_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Text_Search_ItemViewer class </summary>
        public Text_Search_ItemViewer_LocalizationInfo Text_Search_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Tracking_ItemViewer class </summary>
        public Tracking_ItemViewer_LocalizationInfo Tracking_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the abstract_ResultsViewer class </summary>
        public abstract_ResultsViewer_LocalizationInfo abstract_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the PagedResults_HtmlSubwriter class </summary>
        public PagedResults_HtmlSubwriter_LocalizationInfo PagedResults_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Bookshelf_View_ResultsViewer class </summary>
        public Bookshelf_View_ResultsViewer_LocalizationInfo Bookshelf_View_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the Brief_ResultsViewer class </summary>
        public Brief_ResultsViewer_LocalizationInfo Brief_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the Export_View_ResultsViewer class </summary>
        public Export_View_ResultsViewer_LocalizationInfo Export_View_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the Map_ResultsWriter class </summary>
        public Map_ResultsWriter_LocalizationInfo Map_ResultsWriter { get ; private set; }

        /// <summary> Localization string information for the No_Results_ResultsViewer class </summary>
        public No_Results_ResultsViewer_LocalizationInfo No_Results_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the Table_ResultsViewer class </summary>
        public Table_ResultsViewer_LocalizationInfo Table_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the Thumbnail_ResultsViewer class </summary>
        public Thumbnail_ResultsViewer_LocalizationInfo Thumbnail_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the MySobek_HtmlSubwriter class </summary>
        public MySobek_HtmlSubwriter_LocalizationInfo MySobek_HtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Saved_Searches_MySobekViewer class </summary>
        public Saved_Searches_MySobekViewer_LocalizationInfo Saved_Searches_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Preferences_MySobekViewer class </summary>
        public Preferences_MySobekViewer_LocalizationInfo Preferences_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the NewPassword_MySobekViewer class </summary>
        public NewPassword_MySobekViewer_LocalizationInfo NewPassword_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Logon_MySobekViewer class </summary>
        public Logon_MySobekViewer_LocalizationInfo Logon_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Home_MySobekViewer class </summary>
        public Home_MySobekViewer_LocalizationInfo Home_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Folder_Mgmt_MySobekViewer class </summary>
        public Folder_Mgmt_MySobekViewer_LocalizationInfo Folder_Mgmt_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Aggregation_Single_AdminViewer class </summary>
        public Aggregation_Single_AdminViewer_LocalizationInfo Aggregation_Single_AdminViewer { get ; private set; }

        /// <summary> Localization string information for the Google_Map_ResultsViewer class </summary>
        public Google_Map_ResultsViewer_LocalizationInfo Google_Map_ResultsViewer { get ; private set; }

        /// <summary> Localization string information for the User_Usage_Stats_MySobekViewer class </summary>
        public User_Usage_Stats_MySobekViewer_LocalizationInfo User_Usage_Stats_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the User_Tags_MySobekViewer class </summary>
        public User_Tags_MySobekViewer_LocalizationInfo User_Tags_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Page_Image_Upload_MySobekViewer class </summary>
        public Page_Image_Upload_MySobekViewer_LocalizationInfo Page_Image_Upload_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the New_Group_And_Item_MySobekViewer class </summary>
        public New_Group_And_Item_MySobekViewer_LocalizationInfo New_Group_And_Item_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Mass_Update_Items_MySobekViewer class </summary>
        public Mass_Update_Items_MySobekViewer_LocalizationInfo Mass_Update_Items_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Group_AutoFill_Volume_MySobekViewer class </summary>
        public Group_AutoFill_Volume_MySobekViewer_LocalizationInfo Group_AutoFill_Volume_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Group_Add_Volume_MySobekViewer class </summary>
        public Group_Add_Volume_MySobekViewer_LocalizationInfo Group_Add_Volume_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the File_Management_MySobekViewer class </summary>
        public File_Management_MySobekViewer_LocalizationInfo File_Management_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Edit_Serial_Hierarchy_MySobekViewer class </summary>
        public Edit_Serial_Hierarchy_MySobekViewer_LocalizationInfo Edit_Serial_Hierarchy_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Edit_Item_Metadata_MySobekViewer class </summary>
        public Edit_Item_Metadata_MySobekViewer_LocalizationInfo Edit_Item_Metadata_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Edit_Item_Behaviors_MySobekViewer class </summary>
        public Edit_Item_Behaviors_MySobekViewer_LocalizationInfo Edit_Item_Behaviors_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Edit_Group_Behaviors_MySobekViewer class </summary>
        public Edit_Group_Behaviors_MySobekViewer_LocalizationInfo Edit_Group_Behaviors_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Delete_Item_MySobekViewer class </summary>
        public Delete_Item_MySobekViewer_LocalizationInfo Delete_Item_MySobekViewer { get ; private set; }

        /// <summary> Localization string information for the Text_MainWriter class </summary>
        public Text_MainWriter_LocalizationInfo Text_MainWriter { get ; private set; }

        /// <summary> Localization string information for the Html_Echo_MainWriter class </summary>
        public Html_Echo_MainWriter_LocalizationInfo Html_Echo_MainWriter { get ; private set; }

        /// <summary> Localization string information for the TOC_ItemViewer class </summary>
        public TOC_ItemViewer_LocalizationInfo TOC_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Text_ItemViewer class </summary>
        public Text_ItemViewer_LocalizationInfo Text_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Street_ItemViewer class </summary>
        public Street_ItemViewer_LocalizationInfo Street_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the ManageMenu_ItemViewer class </summary>
        public ManageMenu_ItemViewer_LocalizationInfo ManageMenu_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the JPEG2000_ItemViewer class </summary>
        public JPEG2000_ItemViewer_LocalizationInfo JPEG2000_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the JPEG_ItemViewer class </summary>
        public JPEG_ItemViewer_LocalizationInfo JPEG_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Google_Coordinate_Entry_ItemViewer class </summary>
        public Google_Coordinate_Entry_ItemViewer_LocalizationInfo Google_Coordinate_Entry_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the Feature_ItemViewer class </summary>
        public Feature_ItemViewer_LocalizationInfo Feature_ItemViewer { get ; private set; }

        /// <summary> Localization string information for the abstractItemViewer class </summary>
        public abstractItemViewer_LocalizationInfo abstractItemViewer { get ; private set; }

        /// <summary> Localization string information for the MainMenus_Helper_HtmlSubWriter class </summary>
        public MainMenus_Helper_HtmlSubWriter_LocalizationInfo MainMenus_Helper_HtmlSubWriter { get ; private set; }

        /// <summary> Localization string information for the Item_Nav_Bar_HTML_Factory class </summary>
        public Item_Nav_Bar_HTML_Factory_LocalizationInfo Item_Nav_Bar_HTML_Factory { get ; private set; }

        /// <summary> Localization string information for the abstractHtmlSubwriter class </summary>
        public abstractHtmlSubwriter_LocalizationInfo abstractHtmlSubwriter { get ; private set; }

        /// <summary> Localization string information for the Static_Browse_Info_AggregationViewer class </summary>
        public Static_Browse_Info_AggregationViewer_LocalizationInfo Static_Browse_Info_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Item_Count_Aggregation_Viewer class </summary>
        public Item_Count_Aggregation_Viewer_LocalizationInfo Item_Count_Aggregation_Viewer { get ; private set; }

        /// <summary> Localization string information for the Basic_Search_YearRange_AggregationViewer class </summary>
        public Basic_Search_YearRange_AggregationViewer_LocalizationInfo Basic_Search_YearRange_AggregationViewer { get ; private set; }

        /// <summary> Localization string information for the Advanced_Search_YearRange_AggregationViewer class </summary>
        public Advanced_Search_YearRange_AggregationViewer_LocalizationInfo Advanced_Search_YearRange_AggregationViewer { get ; private set; }


		#endregion

		#region Constructor that configures all strings to the default english

        /// <summary> Constructor for a new instance of the SobekCM_LocalizationInfo class </summary>
        /// <remarks> This sets all the terms for localization to the system default, before any resource file is read </remarks>
        public SobekCM_LocalizationInfo()
        {
            // Set a hardwired default language for this localization initially
            // This will be replaced by the actual value
            Language = Web_Language_Enum.English;

            // Initialize all the child localization objects
            //Initialize the General_Localization class
            General = new General_LocalizationInfo();
            General.Add_Localization_String( "My", "my");
            General.Add_Localization_String( "Close", "Close");
            General.Add_Localization_String( "Search", "Search");
            General.Add_Localization_String( "Help", "Help");
            General.Add_Localization_String( "Submit", "Submit");
            General.Add_Localization_String( "Save", "Save");
            General.Add_Localization_String( "Cancel", "Cancel");
            General.Add_Localization_String( "Sort By", "Sort by");
            General.Add_Localization_String( "First Page", "First Page");
            General.Add_Localization_String( "Previous Page", "Previous Page");
            General.Add_Localization_String( "Next Page", "Next Page");
            General.Add_Localization_String( "Last Page", "Last Page");
            General.Add_Localization_String( "First", "First");
            General.Add_Localization_String( "Previous", "Previous");
            General.Add_Localization_String( "Next", "Next");
            General.Add_Localization_String( "Last", "Last");
            General.Add_Localization_String( "January", "January");
            General.Add_Localization_String( "February", "February");
            General.Add_Localization_String( "March", "March");
            General.Add_Localization_String( "April", "April ");
            General.Add_Localization_String( "May", "May ");
            General.Add_Localization_String( "June", "June");
            General.Add_Localization_String( "July", "July ");
            General.Add_Localization_String( "August", "August");
            General.Add_Localization_String( "September", "September");
            General.Add_Localization_String( "October", "October");
            General.Add_Localization_String( "November", "November");
            General.Add_Localization_String( "December", "December");
            General.Add_Localization_String( "Unknown", "Unknown");
            General.Add_Localization_String( "Error", "Error");
            General.Add_Localization_String( "Total", "Total");
            General.Add_Localization_String( "Bibid", "BibID");
            General.Add_Localization_String( "VID", "VID");
            General.Add_Localization_String( "One", "one");
            General.Add_Localization_String( "Two", "two");
            General.Add_Localization_String( "Three", "three");
            General.Add_Localization_String( "Four", "four");
            General.Add_Localization_String( "Five", "five");
            General.Add_Localization_String( "Six", "six");
            General.Add_Localization_String( "Seven", "seven");
            General.Add_Localization_String( "Eight", "eight");
            General.Add_Localization_String( "Nine", "nine");
            General.Add_Localization_String( "Ten", "ten");
            General.Add_Localization_String( "Eleven", "eleven");
            General.Add_Localization_String( "Twelve", "twelve");
            General.Add_Localization_String( "Print", "Print");
            General.Add_Localization_String( "Send", "Send");
            General.Add_Localization_String( "Share", "Share");
            General.Add_Localization_String( "Describe", "Describe");
            General.Add_Localization_String( "Add", "Add");
            General.Add_Localization_String( "Remove", "Remove");
            General.Add_Localization_String( "Missing Image", "Missing Image");
            General.Add_Localization_String( "Missing Banner", "Missing Banner");
            General.Add_Localization_String( "My Library", "My Library");
            General.Add_Localization_String( "My Account", "My Account");
            General.Add_Localization_String( "Internal", "Internal");
            General.Add_Localization_String( "System Admin", "System Admin");
            General.Add_Localization_String( "Portal Admin", "Portal Admin");
            General.Add_Localization_String( "ACTIONS", "ACTIONS");
            General.Add_Localization_String( "Delete", "delete");
            General.Add_Localization_String( "View", "view");
            General.Add_Localization_String( "XXX Is A Required Field", "{0} is a required field");
            General.Add_Localization_String( "The Following Errors Were Detected", "The following errors were detected:");

            //Initialize the Html_MainWriter_Localization class
            Html_MainWriter = new Html_MainWriter_LocalizationInfo();
            Html_MainWriter.Add_Localization_String( "XXX Item", "%1 Item");
            Html_MainWriter.Add_Localization_String( "XXX Home", "%1 Home");
            Html_MainWriter.Add_Localization_String( "XXX Search", "%1 Search");
            Html_MainWriter.Add_Localization_String( "XXX Search Results", "%1 Search Results");
            Html_MainWriter.Add_Localization_String( "XXX Preferences", "%1 Preferences");
            Html_MainWriter.Add_Localization_String( "XXX Contact Us", "%1 Contact Us");
            Html_MainWriter.Add_Localization_String( "XXX Contact Sent", "%1 Contact Sent");
            Html_MainWriter.Add_Localization_String( "XXX Error", "%1 Error");
            Html_MainWriter.Add_Localization_String( "Log Out", "Log Out");
            Html_MainWriter.Add_Localization_String( "Xxxs XXX", "%1's %2 ");

            //Initialize the URL_Email_Helper_Localization class
            URL_Email_Helper = new URL_Email_Helper_LocalizationInfo();
            URL_Email_Helper.Add_Localization_String( "XXX Wanted You To See These Search Results On XXX", "%1 wanted you to see these search results on %2.");
            URL_Email_Helper.Add_Localization_String( "XXX Wanted You To See These Search Results On XXX And Included The Following Comments", "%1 wanted you to see these search results on %2 and included the following comments.");
            URL_Email_Helper.Add_Localization_String( "XXX Wanted You To See This Page On XXX", "%1 wanted you to see this page on %2.");
            URL_Email_Helper.Add_Localization_String( "XXX Wanted You To See This Page On XXX And Included The Following Comments", "%1 wanted you to see this page on %2 and included the following comments.");
            URL_Email_Helper.Add_Localization_String( "Item From XXX", "Item from %1");
            URL_Email_Helper.Add_Localization_String( "Search Results From XXX", "Search results from %1");
            URL_Email_Helper.Add_Localization_String( "Page From XXX", "Page from %1");
            URL_Email_Helper.Add_Localization_String( "XXX XXX From XXX", "\"%1,%2 from %3\"");
            URL_Email_Helper.Add_Localization_String( "XXX Wanted You To See This XXX On XXX And Included The Following Comments", "%1 wanted you to see this %2 on %3 and included the following comments");
            URL_Email_Helper.Add_Localization_String( "XXX Wanted You To See This On XXX", "%1 wanted you to see this on %2");
            URL_Email_Helper.Add_Localization_String( "Title XXX", "Title: %1");

            //Initialize the Item_Email_Helper_Localization class
            Item_Email_Helper = new Item_Email_Helper_LocalizationInfo();
            Item_Email_Helper.Add_Localization_String( "XXX Wanted You To See This Item On XXX", "%1 wanted you to see this item on %2.");
            Item_Email_Helper.Add_Localization_String( "XXX Wanted You To See This Item On XXX And Included The Following Comments", "%1 wanted you to see this item on %2 and included the following comments.");
            Item_Email_Helper.Add_Localization_String( "ITEM INFORMATION", "ITEM INFORMATION");
            Item_Email_Helper.Add_Localization_String( "BLOCKED THUMBNAIL IMAGE", "BLOCKED THUMBNAIL IMAGE");
            Item_Email_Helper.Add_Localization_String( "Uniform Title XXX", "Uniform Title: %1");
            Item_Email_Helper.Add_Localization_String( "Alternate Title XXX", "Alternate Title: %1");
            Item_Email_Helper.Add_Localization_String( "Translated Title XXX", "Translated Title: %1");
            Item_Email_Helper.Add_Localization_String( "Abbreviated Title XXX", "Abbreviated Title: %1");
            Item_Email_Helper.Add_Localization_String( "Creator XXX", "Creator: %1");
            Item_Email_Helper.Add_Localization_String( "Publisher XXX", "Publisher: %1");
            Item_Email_Helper.Add_Localization_String( "Date XXX", "Date: %1");
            Item_Email_Helper.Add_Localization_String( "Description XXX", "Description: %1");
            Item_Email_Helper.Add_Localization_String( "Subject XXX", "Subject: %1");
            Item_Email_Helper.Add_Localization_String( "Genre XXX", "Genre: %1");
            Item_Email_Helper.Add_Localization_String( "Spatial Coverage XXX", "Spatial Coverage: %1");
            Item_Email_Helper.Add_Localization_String( "Rights XXX", "Rights: %1");
            Item_Email_Helper.Add_Localization_String( "Series Title XXX", "Series Title: %1");

            //Initialize the Usage_Stats_Email_Helper_Localization class
            Usage_Stats_Email_Helper = new Usage_Stats_Email_Helper_LocalizationInfo();
            Usage_Stats_Email_Helper.Add_Localization_String( "Usage Statistics For Your Materials XXX", "Usage statistics for your materials ( %1 )");
            Usage_Stats_Email_Helper.Add_Localization_String( "Below Are The Details For Your Top 10 Items See The Link Below To View Usage Statistics For All XXX Of Your Items", "Below are the details for your top 10 items.  See the link below to view usage statistics for all %1 of your items.");
            Usage_Stats_Email_Helper.Add_Localization_String( "XXX Views", "%1 views");
            Usage_Stats_Email_Helper.Add_Localization_String( "Total To Date Since XXX", "Total to date ( since %1 )");
            Usage_Stats_Email_Helper.Add_Localization_String( "You Are Receiving This Message Because You Are A Contributor To A Digital Library Or A Collection Supported By The UF Libraries Including The Institutional Repository IRUF The UF Digital Collections UFDC The Digital Library Of The Caribbean Dloc And Many Others If You Do Not Wish To Receive Future Messages Please A Href Httpufdcufledumypreferences Edit Your Account Preferences A Online Or Send An Email To A Href Mailtoufdcuflibufledu Ufdcuflibufledu A P P Strong Usage Statistics For Your Materials DATE Strong P P NAME P P Thank You For Sharing Materials That Will Be Accessible Online And For Supporting Worldwide Open Access To Scholarly Creative And Other Works This Is A Usage Report For The Shared Materials P P Your Items Have Been Viewed TOTAL Times Since They Were Added And Were Viewed MONTHLY Times This Month P ITEMS P Em A Href Httpufdcufledumystats YEAR MONTH D Click Here To See The Usage Statistics For All Of Your Items Gtgt A Em P P Thank You For Sharing These Materials Please Contact Us With Any Questions A Href Mailtoufdcuflibufledu Ufdcuflibufledu A Or 3522732900", "\"You are receiving this message because you are a contributor to a digital library or a collection supported by the UF Libraries, including the Institutional Repository (IR@UF), the UF Digital Collections (UFDC), the Digital Library of the Caribbean (dLOC), and many others. If you do not wish to receive future messages, please <a href=\"http://ufdc.ufl.edu/my/preferences\">edit your account preferences</a> online or send an email to <a href=\"mailto:ufdc@uflib.ufl.edu\">ufdc@uflib.ufl.edu</a>. </p>\"\" + \"\"<p><strong>Usage statistics for your materials ( <%DATE%> )</strong></p>\"\" + \"\"<p><%NAME%>,</p>\"\" + \"\"<p>Thank you for sharing materials that will be accessible online and for supporting worldwide open access to scholarly, creative, and other works.  This is a usage report for the shared materials.</p>\"\" + \"\"<p>Your items have been viewed <%TOTAL%> times since they were added and were viewed <%MONTHLY%> times this month</p>\"\" + \"\"<%ITEMS%>\"\" + \"\"<p><em><a href=\"http://ufdc.ufl.edu/my/stats/<%YEAR%><%MONTH%>d\">Click here to see the usage statistics for all of your items. &gt;&gt;</a></em></p>\"\" + \"\"<p>Thank you for sharing these materials.  Please contact us with any questions ( <a href=\"mailto:ufdc@uflib.ufl.edu\">ufdc@uflib.ufl.edu</a> or 352-273-2900).\"");

            //Initialize the Aggregation_HtmlSubwriter_Localization class
            Aggregation_HtmlSubwriter = new Aggregation_HtmlSubwriter_LocalizationInfo();
            Aggregation_HtmlSubwriter.Add_Localization_String( "Aggregation_Htmlsubwriter", "Aggregation_HtmlSubwriter");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Removed Aggregation From Your Home Page", "Removed aggregation from your home page");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Your Email Has Been Sent", "Your email has been sent");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Hide Internal Header", "Hide Internal Header");
            Aggregation_HtmlSubwriter.Add_Localization_String( "View Private Items", "View Private Items");
            Aggregation_HtmlSubwriter.Add_Localization_String( "View Item Count", "View Item Count");
            Aggregation_HtmlSubwriter.Add_Localization_String( "View Usage Statistics", "View Usage Statistics");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Edit Administrative Information", "Edit Administrative Information");
            Aggregation_HtmlSubwriter.Add_Localization_String( "View Administrative Information", "View Administrative Information");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Help Regarding This Header", "Help regarding this header");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Perform Search", "Perform search");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Show Internal Header", "Show Internal Header");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Close", "Close");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Send This Collection To A Friend", "Send this collection to a friend");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Enter The Email Information Below", "Enter the email information below");
            Aggregation_HtmlSubwriter.Add_Localization_String( "To", "To:");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Comments", "Comments:");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Format", "Format:");
            Aggregation_HtmlSubwriter.Add_Localization_String( "HTML", "HTML");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Plain Text", "Plain text");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Send", "Send");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Cancel", "Cancel");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Show Collection Groups", "Show Collection Groups");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Show Collections", "Show Collections");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Show Subcollections", "Show SubCollections");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Hide Collection Groups", "Hide Collection Groups");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Hide Collections", "Hide Collections");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Hide Subcollections", "Hide SubCollections");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Select Collection Groups To Include In Search", "Select collection groups to include in search");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Select Collections To Include In Search", "Select collections to include in search");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Select Subcollections To Include In Search", "Select subcollections to include in search");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Print This Page", "Print this page");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Send This To Someone", "Send this to someone");
            Aggregation_HtmlSubwriter.Add_Localization_String( "PRINT", "PRINT");
            Aggregation_HtmlSubwriter.Add_Localization_String( "SEND", "SEND");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Remove This From My Collections Home Page", "Remove this from my collections home page");
            Aggregation_HtmlSubwriter.Add_Localization_String( "REMOVE", "REMOVE");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Add This To My Collections Home Page", "Add this to my collections home page");
            Aggregation_HtmlSubwriter.Add_Localization_String( "ADD", "ADD");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Save This To My Collections Home Page", "Save this to my collections home page");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Share This Collection", "Share this collection");
            Aggregation_HtmlSubwriter.Add_Localization_String( "All Collections", "All collections");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Collapse All", "Collapse All");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Expand All", "Expand All");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Do Not Apply Changes", "Do not apply changes");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Save Changes To This Aggregation Home Page Text", "Save changes to this aggregation home page text");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Save", "Save");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Edit This Home Text", "Edit this home text");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Edit Content", "edit content");
            Aggregation_HtmlSubwriter.Add_Localization_String( "LIST VIEW", "LIST VIEW");
            Aggregation_HtmlSubwriter.Add_Localization_String( "BRIEF VIEW", "BRIEF VIEW");
            Aggregation_HtmlSubwriter.Add_Localization_String( "TREE VIEW", "TREE VIEW");
            Aggregation_HtmlSubwriter.Add_Localization_String( "THUMBNAIL VIEW", "THUMBNAIL VIEW");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Welcome To Your Personalized XXX Home Page This Page Displays Any Collections You Have Added As Well As Any Of Your Bookshelves You Have Made Public", "\"Welcome to your personalized %1 home page. This page displays any collections you have added, as well as any of your bookshelves you have made public.\"");
            Aggregation_HtmlSubwriter.Add_Localization_String( "You Do Not Have Any Collections Added To Your Home Page", "You do not have any collections added to your home page.");
            Aggregation_HtmlSubwriter.Add_Localization_String( "To Add A Collection Use The XXX Button From That Collections Home Page", "\"To add a collection, use the %1 button from that collection's home page.\"");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Make Private", "make private");
            Aggregation_HtmlSubwriter.Add_Localization_String( "My Public Bookshelves", "My Public Bookshelves");
            Aggregation_HtmlSubwriter.Add_Localization_String( "My Links", "My Links");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Go To My XXX Home", "Go to my %1 home");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Go To My Bookshelf", "Go to my bookshelf");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Go To My Saved Searches", "Go to my saved searches");
            Aggregation_HtmlSubwriter.Add_Localization_String( "My Library", "My Library");
            Aggregation_HtmlSubwriter.Add_Localization_String( "My Saved Searches", "My Saved Searches");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Partners And Contributing Institutions", "Partners and Contributing Institutions");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Home", "Home");
            Aggregation_HtmlSubwriter.Add_Localization_String( "All Items", "All Items");
            Aggregation_HtmlSubwriter.Add_Localization_String( "New Items", "New Items");
            Aggregation_HtmlSubwriter.Add_Localization_String( "My Collections", "My Collections");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Browse Partners", "Browse Partners");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Browse By", "Browse By");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Map Browse", "Map Browse");
            Aggregation_HtmlSubwriter.Add_Localization_String( "Partners Collaborating And Contributing To Digital Collections And Libraries Include", "Partners collaborating and contributing to digital collections and libraries include:");

            //Initialize the Aggregation_Nav_Bar_HTML_Factory_Localization class
            Aggregation_Nav_Bar_HTML_Factory = new Aggregation_Nav_Bar_HTML_Factory_LocalizationInfo();
            Aggregation_Nav_Bar_HTML_Factory.Add_Localization_String( "Advanced Search", "Advanced Search");
            Aggregation_Nav_Bar_HTML_Factory.Add_Localization_String( "Basic Search", "Basic Search");
            Aggregation_Nav_Bar_HTML_Factory.Add_Localization_String( "Map Search", "Map Search");
            Aggregation_Nav_Bar_HTML_Factory.Add_Localization_String( "Newspaper Search", "Newspaper Search");
            Aggregation_Nav_Bar_HTML_Factory.Add_Localization_String( "Text Search", "Text Search");
            Aggregation_Nav_Bar_HTML_Factory.Add_Localization_String( "Display Text", "Display Text");

            //Initialize the Advanced_Search_AggregationViewer_Localization class
            Advanced_Search_AggregationViewer = new Advanced_Search_AggregationViewer_LocalizationInfo();
            Advanced_Search_AggregationViewer.Add_Localization_String( "Search For", "Search for:");
            Advanced_Search_AggregationViewer.Add_Localization_String( "In", "in");
            Advanced_Search_AggregationViewer.Add_Localization_String( "Search", "Search");
            Advanced_Search_AggregationViewer.Add_Localization_String( "Search Options", "Search Options");
            Advanced_Search_AggregationViewer.Add_Localization_String( "Precision", "Precision");
            Advanced_Search_AggregationViewer.Add_Localization_String( "Contains Exactly The Search Terms", "Contains exactly the search terms");
            Advanced_Search_AggregationViewer.Add_Localization_String( "Contains Any Form Of The Search Terms", "Contains any form of the search terms");
            Advanced_Search_AggregationViewer.Add_Localization_String( "Contains The Search Term Or Terms Of Similar Meaning", "Contains the search term or terms of similar meaning");
            Advanced_Search_AggregationViewer.Add_Localization_String( "And", "and");
            Advanced_Search_AggregationViewer.Add_Localization_String( "Or", "or");
            Advanced_Search_AggregationViewer.Add_Localization_String( "And Not", "and not");

            //Initialize the Basic_Search_AggregationViewer_Localization class
            Basic_Search_AggregationViewer = new Basic_Search_AggregationViewer_LocalizationInfo();
            Basic_Search_AggregationViewer.Add_Localization_String( "Search Collection", "Search Collection");
            Basic_Search_AggregationViewer.Add_Localization_String( "Include Nonpublic Items", "Include non-public items");

            //Initialize the dLOC_Search_AggregationViewer_Localization class
            dLOC_Search_AggregationViewer = new dLOC_Search_AggregationViewer_LocalizationInfo();
            dLOC_Search_AggregationViewer.Add_Localization_String( "Search Full Text", "Search full text");
            dLOC_Search_AggregationViewer.Add_Localization_String( "Include Newspapers", "Include newspapers?");
            dLOC_Search_AggregationViewer.Add_Localization_String( "Go", "Go");

            //Initialize the Full_Text_Search_AggregationViewer_Localization class
            Full_Text_Search_AggregationViewer = new Full_Text_Search_AggregationViewer_LocalizationInfo();
            Full_Text_Search_AggregationViewer.Add_Localization_String( "Search Full Text", "Search full text");
            Full_Text_Search_AggregationViewer.Add_Localization_String( "Go", "Go");

            //Initialize the Item_Count_AggregationViewer_Localization class
            Item_Count_AggregationViewer = new Item_Count_AggregationViewer_LocalizationInfo();
            Item_Count_AggregationViewer.Add_Localization_String( "Resource Count In Aggregation", "Resource Count in Aggregation");
            Item_Count_AggregationViewer.Add_Localization_String( "Below Is The Number Of Titles And Items For All Items Within This Aggregation Including Currently Online Items As Well As Items In Process", "\"Below is the number of titles and items for all items within this aggregation, including currently online items as well as items in process.\"");
            Item_Count_AggregationViewer.Add_Localization_String( "Last Milestone", "Last Milestone");
            Item_Count_AggregationViewer.Add_Localization_String( "Title Count", "Title Count");
            Item_Count_AggregationViewer.Add_Localization_String( "Item Count", "Item Count");
            Item_Count_AggregationViewer.Add_Localization_String( "Page Count", "Page Count");
            Item_Count_AggregationViewer.Add_Localization_String( "File Count", "File Count");

            //Initialize the Map_Browse_AggregationViewer_Localization class
            Map_Browse_AggregationViewer = new Map_Browse_AggregationViewer_LocalizationInfo();
            Map_Browse_AggregationViewer.Add_Localization_String( "XXX Issues", "( %1 issues )");
            Map_Browse_AggregationViewer.Add_Localization_String( "XXX Volumes", "( %1 volumes )");
            Map_Browse_AggregationViewer.Add_Localization_String( "Click Here For More Information About This Title", "Click here for more information about this title");
            Map_Browse_AggregationViewer.Add_Localization_String( "Click Here For More Information About These XXX Titles", "Click here for more information about these %1 titles");
            Map_Browse_AggregationViewer.Add_Localization_String( "Select A Point Below To View The Items From Or About That Location", "Select a point below to view the items from or about that location.");
            Map_Browse_AggregationViewer.Add_Localization_String( "Press The SHIFT Button And Then Drag A Box On The Map To Zoom In", "\"Press the SHIFT button, and then drag a box on the map to zoom in.\"");

            //Initialize the Map_Search_AggregationViewer_Localization class
            Map_Search_AggregationViewer = new Map_Search_AggregationViewer_LocalizationInfo();
            Map_Search_AggregationViewer.Add_Localization_String( "Use The I Select Area I Button And Click To Select Opposite Corners To Draw A Search Box On The Map", " 1. Use the <i>Select Area</i> button and click to select opposite corners to draw a search box on the map ");
            Map_Search_AggregationViewer.Add_Localization_String( "Use One Of The Methods Below To Define Your Geographic Search", "1. Use one of the methods below to define your geographic search:");
            Map_Search_AggregationViewer.Add_Localization_String( "Press The I Search I Button To See Results", "2. Press the <i>Search</i> button to see results");
            Map_Search_AggregationViewer.Add_Localization_String( "A Enter An Address And Press I Find Address I To Locate I Or I", "\"a. Enter an address and press <i>Find Address</i> to locate, <i>or</i>\"");
            Map_Search_AggregationViewer.Add_Localization_String( "Actions", "Actions");
            Map_Search_AggregationViewer.Add_Localization_String( "Address", "Address");
            Map_Search_AggregationViewer.Add_Localization_String( "Address This Is The Nearest Address Of The Point You Selected", "Address: This is the nearest address of the point you selected.");
            Map_Search_AggregationViewer.Add_Localization_String( "Applied", "Applied");
            Map_Search_AggregationViewer.Add_Localization_String( "Apply", "Apply");
            Map_Search_AggregationViewer.Add_Localization_String( "Apply Changes Make Changes Public", "Apply Changes (Make Changes Public)");
            Map_Search_AggregationViewer.Add_Localization_String( "B Press The I Select Area I Button And Click To Select Two Opposite Corners I Or I", "\"b. Press the <i>Select Area</i> button and click to select two opposite corners, <i>or</i>\"");
            Map_Search_AggregationViewer.Add_Localization_String( "Blocklot Toggle Blocklot Map Layer", "Block/Lot: Toggle Block/Lot Map Layer");
            Map_Search_AggregationViewer.Add_Localization_String( "C Press The I Select Point I Button And Click To Select A Single Point", "c. Press the <i>Select Point</i> button and click to select a single point");
            Map_Search_AggregationViewer.Add_Localization_String( "Cancel Editing", "Cancel Editing");
            Map_Search_AggregationViewer.Add_Localization_String( "Canceling", "Canceling...");
            Map_Search_AggregationViewer.Add_Localization_String( "Cannot Convert", "Cannot Convert");
            Map_Search_AggregationViewer.Add_Localization_String( "Cannot Zoom In Further", "Cannot Zoom in further");
            Map_Search_AggregationViewer.Add_Localization_String( "Cannot Zoom Out Further", "Cannot Zoom out Further");
            Map_Search_AggregationViewer.Add_Localization_String( "Center On Current Location", "Center on Current Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Center On Your Current Position", "Center On Your Current Position");
            Map_Search_AggregationViewer.Add_Localization_String( "Circle", "Circle");
            Map_Search_AggregationViewer.Add_Localization_String( "Circle Place A Circle", "Circle: Place a Circle");
            Map_Search_AggregationViewer.Add_Localization_String( "Clear Point Of Interest Set", "Clear Point Of Interest Set");
            Map_Search_AggregationViewer.Add_Localization_String( "Click Here To View Instructions For This Search Interface", "Click here to view instructions for this search interface");
            Map_Search_AggregationViewer.Add_Localization_String( "Clockwise", "Clockwise");
            Map_Search_AggregationViewer.Add_Localization_String( "Complete Editing", "Complete Editing");
            Map_Search_AggregationViewer.Add_Localization_String( "Completed", "Completed");
            Map_Search_AggregationViewer.Add_Localization_String( "Controls", "Controls");
            Map_Search_AggregationViewer.Add_Localization_String( "Controls Toggle Map Controls", "Controls: Toggle Map Controls");
            Map_Search_AggregationViewer.Add_Localization_String( "Convert This To A Map Overlay", "Convert This To a Map Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Convert To Overlay", "Convert to Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Converted Item To Overlay", "Converted Item to Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Coordinate Data Removed For", "Coordinate Data removed for");
            Map_Search_AggregationViewer.Add_Localization_String( "Coordinates Copied To Clipboard", "Coordinates Copied to Clipboard");
            Map_Search_AggregationViewer.Add_Localization_String( "Coordinates Viewer Frozen", "Coordinates Viewer Frozen");
            Map_Search_AggregationViewer.Add_Localization_String( "Coordinates Viewer Unfrozen", "Coordinates Viewer Unfrozen");
            Map_Search_AggregationViewer.Add_Localization_String( "Coordinates This Is The Selected Latitude And Longitude Of The Point You Selected", "Coordinates: This is the selected Latitude and Longitude of the point you selected.");
            Map_Search_AggregationViewer.Add_Localization_String( "Could Not Find Location Either The Format You Entered Is Invalid Or The Location Is Outside Of The Map Bounds", "Could not find location. Either the format you entered is invalid or the location is outside of the map bounds.");
            Map_Search_AggregationViewer.Add_Localization_String( "Could Not Find Within Bounds", "Could not find within bounds");
            Map_Search_AggregationViewer.Add_Localization_String( "Counterclockwise", "Counter-Clockwise");
            Map_Search_AggregationViewer.Add_Localization_String( "Custom", "Custom");
            Map_Search_AggregationViewer.Add_Localization_String( "Default Pan Map To Default", "Default: Pan Map To Default");
            Map_Search_AggregationViewer.Add_Localization_String( "Delete", "Delete");
            Map_Search_AggregationViewer.Add_Localization_String( "Delete Coordinate Data For Overlay", "Delete Coordinate Data for Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Delete Geographic Location", "Delete Geographic Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Delete POI", "Delete POI");
            Map_Search_AggregationViewer.Add_Localization_String( "Delete Search Result", "Delete Search Result");
            Map_Search_AggregationViewer.Add_Localization_String( "Deleted", "Deleted");
            Map_Search_AggregationViewer.Add_Localization_String( "Description Optional", "Description (Optional)");
            Map_Search_AggregationViewer.Add_Localization_String( "Did Not Reset Page", "Did not Reset Page");
            Map_Search_AggregationViewer.Add_Localization_String( "Documentation", "Documentation");
            Map_Search_AggregationViewer.Add_Localization_String( "Down Pan Map Down", "Down: Pan Map Down");
            Map_Search_AggregationViewer.Add_Localization_String( "Edit Location", "Edit Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Edit Location By Dragging Existing Marker", "Edit Location By Dragging Existing Marker");
            Map_Search_AggregationViewer.Add_Localization_String( "Edit This Overlay", "Edit this Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Edit This POI", "Edit this POI");
            Map_Search_AggregationViewer.Add_Localization_String( "Editing", "Editing");
            Map_Search_AggregationViewer.Add_Localization_String( "Enter Address Ie 12 Main Street Gainesville Florida", "\"Enter address ( i.e., 12 Main Street, Gainesville Florida )\"");
            Map_Search_AggregationViewer.Add_Localization_String( "Error Addign Other Listeners", "Error Addign other Listeners");
            Map_Search_AggregationViewer.Add_Localization_String( "ERROR Failed Adding Textual Content", "ERROR Failed Adding Textual Content");
            Map_Search_AggregationViewer.Add_Localization_String( "ERROR Failed Adding Titles", "ERROR Failed Adding Titles");
            Map_Search_AggregationViewer.Add_Localization_String( "Error Description Cannot Contain A Or", "Error: Description cannot contain a ~ or |");
            Map_Search_AggregationViewer.Add_Localization_String( "Error Overlay Image Source Cannot Contain A Or", "Error: Overlay image source cannot contain a ~ or |");
            Map_Search_AggregationViewer.Add_Localization_String( "File", "File");
            Map_Search_AggregationViewer.Add_Localization_String( "Find A Location", "Find a Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Find Address", "Find Address");
            Map_Search_AggregationViewer.Add_Localization_String( "Find Location", "Find Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Finding Your Location", "Finding your location");
            Map_Search_AggregationViewer.Add_Localization_String( "Geocoder Failed Due To", "geocoder failed due to:");
            Map_Search_AggregationViewer.Add_Localization_String( "Geolocation Service Failed", "Geolocation Service Failed.");
            Map_Search_AggregationViewer.Add_Localization_String( "Help", "Help");
            Map_Search_AggregationViewer.Add_Localization_String( "Hide Coordinates", "Hide Coordinates");
            Map_Search_AggregationViewer.Add_Localization_String( "Hiding", "Hiding");
            Map_Search_AggregationViewer.Add_Localization_String( "Hiding Overlays", "Hiding Overlays");
            Map_Search_AggregationViewer.Add_Localization_String( "Hiding Pois", "Hiding POIs");
            Map_Search_AggregationViewer.Add_Localization_String( "Hybrid", "Hybrid");
            Map_Search_AggregationViewer.Add_Localization_String( "Hybrid Toggle Hybrid Map Layer", "Hybrid: Toggle Hybrid Map Layer");
            Map_Search_AggregationViewer.Add_Localization_String( "In Zoom Map In", "In: Zoom Map In");
            Map_Search_AggregationViewer.Add_Localization_String( "Item Geographic Location Deleted", "Item Geographic Location Deleted");
            Map_Search_AggregationViewer.Add_Localization_String( "Item Location Converted To Listing Overlays", "Item Location Converted to Listing Overlays");
            Map_Search_AggregationViewer.Add_Localization_String( "Item Relocation Reset", "Item Relocation Reset!");
            Map_Search_AggregationViewer.Add_Localization_String( "Item Saved", "Item Saved!");
            Map_Search_AggregationViewer.Add_Localization_String( "Latitude", "Latitude");
            Map_Search_AggregationViewer.Add_Localization_String( "Layers", "Layers");
            Map_Search_AggregationViewer.Add_Localization_String( "Left Pan Map Left", "Left: Pan Map Left");
            Map_Search_AggregationViewer.Add_Localization_String( "Line", "Line");
            Map_Search_AggregationViewer.Add_Localization_String( "Line Place A Line", "Line: Place a Line");
            Map_Search_AggregationViewer.Add_Localization_String( "Locate", "Locate");
            Map_Search_AggregationViewer.Add_Localization_String( "Locate Find A Location On The Map", "Locate: Find A Location On The Map");
            Map_Search_AggregationViewer.Add_Localization_String( "Longitude", "Longitude");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage Location", "Manage Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage Location Details", "Manage Location Details");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage Map Coverage", "Manage Map Coverage");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage Overlay", "Manage Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage Overlays", "Manage Overlays");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage POI", "Manage POI");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage Points Of Interest", "Manage Points of Interest");
            Map_Search_AggregationViewer.Add_Localization_String( "Manage Pois", "Manage POIs");
            Map_Search_AggregationViewer.Add_Localization_String( "Map Controls", "Map Controls");
            Map_Search_AggregationViewer.Add_Localization_String( "Marker", "Marker");
            Map_Search_AggregationViewer.Add_Localization_String( "Marker Place A Point", "Marker: Place a Point");
            Map_Search_AggregationViewer.Add_Localization_String( "More Help", "more help");
            Map_Search_AggregationViewer.Add_Localization_String( "Nearest Address", "Nearest Address");
            Map_Search_AggregationViewer.Add_Localization_String( "Not Cleared", "Not Cleared");
            Map_Search_AggregationViewer.Add_Localization_String( "Nothing Happened", "Nothing Happened!");
            Map_Search_AggregationViewer.Add_Localization_String( "Nothing To Delete", "Nothing to Delete");
            Map_Search_AggregationViewer.Add_Localization_String( "Nothing To Hide", "Nothing to Hide");
            Map_Search_AggregationViewer.Add_Localization_String( "Nothing To Reset", "Nothing to Reset");
            Map_Search_AggregationViewer.Add_Localization_String( "Nothing To Save", "Nothing to Save");
            Map_Search_AggregationViewer.Add_Localization_String( "Nothing To Search", "Nothing to Search");
            Map_Search_AggregationViewer.Add_Localization_String( "Nothing To Toggle", "Nothing to Toggle");
            Map_Search_AggregationViewer.Add_Localization_String( "Out Zoom Map Out", "Out: Zoom Map Out");
            Map_Search_AggregationViewer.Add_Localization_String( "Overall Geographic Data Deleted", "Overall Geographic Data Deleted");
            Map_Search_AggregationViewer.Add_Localization_String( "Overlay Editing Turned Off", "Overlay Editing Turned Off");
            Map_Search_AggregationViewer.Add_Localization_String( "Overlay Editing Turned On", "Overlay Editing Turned On");
            Map_Search_AggregationViewer.Add_Localization_String( "Overlay Saved", "Overlay Saved!");
            Map_Search_AggregationViewer.Add_Localization_String( "Overlays Reset", "Overlays Reset!");
            Map_Search_AggregationViewer.Add_Localization_String( "Pan", "Pan");
            Map_Search_AggregationViewer.Add_Localization_String( "Place", "Place");
            Map_Search_AggregationViewer.Add_Localization_String( "POI Set Cleared", "POI Set Cleared!");
            Map_Search_AggregationViewer.Add_Localization_String( "POI Set Saved", "POI Set Saved!");
            Map_Search_AggregationViewer.Add_Localization_String( "Point 1", "Point 1");
            Map_Search_AggregationViewer.Add_Localization_String( "Point 2", "Point 2");
            Map_Search_AggregationViewer.Add_Localization_String( "Polygon", "Polygon");
            Map_Search_AggregationViewer.Add_Localization_String( "Polygon Place A Polygon", "Polygon: Place a Polygon");
            Map_Search_AggregationViewer.Add_Localization_String( "Rectangle", "Rectangle");
            Map_Search_AggregationViewer.Add_Localization_String( "Rectangle Place A Rectangle", "rectangle: Place a rectangle");
            Map_Search_AggregationViewer.Add_Localization_String( "Removed", "Removed");
            Map_Search_AggregationViewer.Add_Localization_String( "Report A Problem", "Report a Problem");
            Map_Search_AggregationViewer.Add_Localization_String( "Report A Sobek Error", "Report a Sobek Error");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset", "Reset");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset All Changes", "Reset All Changes");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset All Overlay Changes", "Reset All Overlay Changes");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Location", "Reset Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Location Changes", "Reset Location Changes");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Me", "Reset Me");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Overlays", "Reset Overlays");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Pois", "Reset POIs");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Click To Reset Rotation", "Reset: Click to Reset Rotation");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Reset Map To Defaults", "Reset: Reset Map To Defaults");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Reset Map Type", "Reset: Reset Map Type");
            Map_Search_AggregationViewer.Add_Localization_String( "Reset Reset Zoom Level", "Reset: Reset Zoom Level");
            Map_Search_AggregationViewer.Add_Localization_String( "Reseting Page", "Reseting Page");
            Map_Search_AggregationViewer.Add_Localization_String( "Resetting Overlays", "Resetting Overlays");
            Map_Search_AggregationViewer.Add_Localization_String( "Resetting Pois", "Resetting POIs");
            Map_Search_AggregationViewer.Add_Localization_String( "Returned To Bounds", "Returned to Bounds!");
            Map_Search_AggregationViewer.Add_Localization_String( "Right Pan Map Right", "Right: Pan Map Right");
            Map_Search_AggregationViewer.Add_Localization_String( "Roadmap", "Roadmap");
            Map_Search_AggregationViewer.Add_Localization_String( "Roadmap Toggle Road Map Layer", "Roadmap: Toggle Road Map Layer");
            Map_Search_AggregationViewer.Add_Localization_String( "Rotate", "Rotate");
            Map_Search_AggregationViewer.Add_Localization_String( "Rotate Edit The Rotation Value", "Rotate: Edit the rotation value");
            Map_Search_AggregationViewer.Add_Localization_String( "Satellite", "Satellite");
            Map_Search_AggregationViewer.Add_Localization_String( "Satellite Toggle Satellite Map Layer", "Satellite: Toggle Satellite Map Layer");
            Map_Search_AggregationViewer.Add_Localization_String( "Save", "Save");
            Map_Search_AggregationViewer.Add_Localization_String( "Save Description", "Save description");
            Map_Search_AggregationViewer.Add_Localization_String( "Save Location Changes", "Save Location Changes");
            Map_Search_AggregationViewer.Add_Localization_String( "Save Overlay Changes", "Save Overlay Changes");
            Map_Search_AggregationViewer.Add_Localization_String( "Save Point Of Interest Set", "Save Point Of Interest Set");
            Map_Search_AggregationViewer.Add_Localization_String( "Save This Description", "save this Description");
            Map_Search_AggregationViewer.Add_Localization_String( "Save To Temporary File", "Save to Temporary File");
            Map_Search_AggregationViewer.Add_Localization_String( "Saved", "Saved");
            Map_Search_AggregationViewer.Add_Localization_String( "Saving", "Saving...");
            Map_Search_AggregationViewer.Add_Localization_String( "Search Coordinates", "Search Coordinates");
            Map_Search_AggregationViewer.Add_Localization_String( "Select Area", "Select Area");
            Map_Search_AggregationViewer.Add_Localization_String( "Select Point", "Select Point");
            Map_Search_AggregationViewer.Add_Localization_String( "Select The Area To Draw The Overlay", "Select the area to Draw the Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Selected Latlong", "Selected Lat/Long");
            Map_Search_AggregationViewer.Add_Localization_String( "Show Coordinates", "Show Coordinates");
            Map_Search_AggregationViewer.Add_Localization_String( "Showing", "Showing");
            Map_Search_AggregationViewer.Add_Localization_String( "Showing Overlays", "Showing Overlays");
            Map_Search_AggregationViewer.Add_Localization_String( "Showing Pois", "Showing POIs");
            Map_Search_AggregationViewer.Add_Localization_String( "Tenth Degree Left Click To Rotate A Tenth Degree Counterclockwise", "Tenth Degree Left: Click to Rotate a Tenth Degree Counter-Clockwise");
            Map_Search_AggregationViewer.Add_Localization_String( "Tenth Degree Right Click To Rotate A Tenth Degree Clockwise", "Tenth Degree Right: Click to Rotate a Tenth Degree Clockwise");
            Map_Search_AggregationViewer.Add_Localization_String( "Terrain", "Terrain");
            Map_Search_AggregationViewer.Add_Localization_String( "Terrain Toggle Terrain Map Layer", "Terrain: Toggle Terrain Map Layer");
            Map_Search_AggregationViewer.Add_Localization_String( "This Will Delete All Of The Pois Are You Sure", "\"This will delete all of the POIs, are you sure?\"");
            Map_Search_AggregationViewer.Add_Localization_String( "This Will Delete The Geogarphic Coodinate Data For This Item Are You Sure", "\"This will delete the geogarphic coodinate data for this item, are you sure?\"");
            Map_Search_AggregationViewer.Add_Localization_String( "This Will Delete The Geographic Coordinate Data For This Overlay Are You Sure", "\"This will delete the geographic coordinate data for this overlay, are you sure?\"");
            Map_Search_AggregationViewer.Add_Localization_String( "Toggle All Map Overlays", "Toggle All Map Overlays");
            Map_Search_AggregationViewer.Add_Localization_String( "Toggle All Overlays On Map", "Toggle All Overlays On Map");
            Map_Search_AggregationViewer.Add_Localization_String( "Toggle All Pois On Map", "Toggle All POIs On Map");
            Map_Search_AggregationViewer.Add_Localization_String( "Toggle On Map", "Toggle on Map");
            Map_Search_AggregationViewer.Add_Localization_String( "Toggle Pois On Map", "Toggle POIs on Map");
            Map_Search_AggregationViewer.Add_Localization_String( "Toolbar", "Toolbar");
            Map_Search_AggregationViewer.Add_Localization_String( "Toolbar Toggle The Toolbar", "Toolbar: Toggle the Toolbar");
            Map_Search_AggregationViewer.Add_Localization_String( "Toolbox", "Toolbox");
            Map_Search_AggregationViewer.Add_Localization_String( "Toolbox Toggle Toolbox", "Toolbox: Toggle Toolbox");
            Map_Search_AggregationViewer.Add_Localization_String( "Transparency", "Transparency");
            Map_Search_AggregationViewer.Add_Localization_String( "Transparency Set The Transparency Of This Overlay", "Transparency: Set the transparency of this Overlay");
            Map_Search_AggregationViewer.Add_Localization_String( "Up Pan Map Up", "Up: Pan Map Up");
            Map_Search_AggregationViewer.Add_Localization_String( "Use Search Result As Location", "Use Search Result As Location");
            Map_Search_AggregationViewer.Add_Localization_String( "Using Search Results As Location", "Using Search Results as Location");
            Map_Search_AggregationViewer.Add_Localization_String( "View", "View");
            Map_Search_AggregationViewer.Add_Localization_String( "Warning This Will Erase Any Changes You Have Made Do You Still Want To Proceed", "Warning! This will erase any changes you have made. Do you still want to proceed?");
            Map_Search_AggregationViewer.Add_Localization_String( "Working", "Working...");
            Map_Search_AggregationViewer.Add_Localization_String( "Zoom", "Zoom");
            Map_Search_AggregationViewer.Add_Localization_String( "Zoom In", "Zoom In");
            Map_Search_AggregationViewer.Add_Localization_String( "Zoom Out", "Zoom Out");

            //Initialize the Metadata_Browse_AggregationViewer_Localization class
            Metadata_Browse_AggregationViewer = new Metadata_Browse_AggregationViewer_LocalizationInfo();
            Metadata_Browse_AggregationViewer.Add_Localization_String( "Browse By XXX", "Browse by %1");
            Metadata_Browse_AggregationViewer.Add_Localization_String( "Browse By", "Browse By:");
            Metadata_Browse_AggregationViewer.Add_Localization_String( "Public Browses", "Public Browses");
            Metadata_Browse_AggregationViewer.Add_Localization_String( "Internal Browses", "Internal Browses");
            Metadata_Browse_AggregationViewer.Add_Localization_String( "Select A Metadata Field To Browse By From The List On The Left", "Select a metadata field to browse by from the list on the left");
            Metadata_Browse_AggregationViewer.Add_Localization_String( "NO MATCHING VALUES", "NO MATCHING VALUES");

            //Initialize the Newspaper_Search_AggregationViewer_Localization class
            Newspaper_Search_AggregationViewer = new Newspaper_Search_AggregationViewer_LocalizationInfo();
            Newspaper_Search_AggregationViewer.Add_Localization_String( "Search For", "Search for:");
            Newspaper_Search_AggregationViewer.Add_Localization_String( "In", "in");
            Newspaper_Search_AggregationViewer.Add_Localization_String( "Full Citation", "Full Citation");
            Newspaper_Search_AggregationViewer.Add_Localization_String( "Full Text", "Full Text");
            Newspaper_Search_AggregationViewer.Add_Localization_String( "Newspaper Title", "Newspaper Title");
            Newspaper_Search_AggregationViewer.Add_Localization_String( "Location", "Location");
            Newspaper_Search_AggregationViewer.Add_Localization_String( "Go", "Go");

            //Initialize the No_Search_AggregationViewer_Localization class
            No_Search_AggregationViewer = new No_Search_AggregationViewer_LocalizationInfo();
            No_Search_AggregationViewer.Add_Localization_String( "XXX Home", "%1 Home");

            //Initialize the Private_Items_AggregationViewer_Localization class
            Private_Items_AggregationViewer = new Private_Items_AggregationViewer_LocalizationInfo();
            Private_Items_AggregationViewer.Add_Localization_String( "Bibid VID", "BibID / VID");
            Private_Items_AggregationViewer.Add_Localization_String( "Title VID", "Title / VID");
            Private_Items_AggregationViewer.Add_Localization_String( "Last Activity Date Most Recent First", "Last Activity Date (most recent first)");
            Private_Items_AggregationViewer.Add_Localization_String( "Last Milestone Date Most Recent First", "Last Milestone Date (most recent first)");
            Private_Items_AggregationViewer.Add_Localization_String( "Last Activity Date Oldest First", "Last Activity Date (oldest first)");
            Private_Items_AggregationViewer.Add_Localization_String( "Last Milestone Date Oldest First", "Last Milestone Date (oldest first)");
            Private_Items_AggregationViewer.Add_Localization_String( "Private And Dark Items", "Private and Dark Items");
            Private_Items_AggregationViewer.Add_Localization_String( "ERROR PULLING INFORMATION FROM DATABASE", "ERROR PULLING INFORMATION FROM DATABASE");
            Private_Items_AggregationViewer.Add_Localization_String( "This Collection Does Not Include Any PRIVATE Or DARK Items", "This collection does not include any PRIVATE or DARK items.");
            Private_Items_AggregationViewer.Add_Localization_String( "Below Is The List Of All Items Linked To This Aggregation Which Are Either Private In Process Or Dark", "Below is the list of all items linked to this aggregation which are either private (in process) or dark.");
            Private_Items_AggregationViewer.Add_Localization_String( "There Is Only One Matching Item", "There is only one matching item.");
            Private_Items_AggregationViewer.Add_Localization_String( "There Are A Total Of XXX Titles", "There are a total of %1 titles.");
            Private_Items_AggregationViewer.Add_Localization_String( "There Are A Total Of XXX Items In One Title", "There are a total of %1 items in one title.");
            Private_Items_AggregationViewer.Add_Localization_String( "There Are A Total Of XXX Items In XXX Titles", "There are a total of %1 items in %2 titles.");
            Private_Items_AggregationViewer.Add_Localization_String( "Title", "Title");
            Private_Items_AggregationViewer.Add_Localization_String( "Last Activity", "Last Activity");
            Private_Items_AggregationViewer.Add_Localization_String( "Last Milestone", "Last Milestone");
            Private_Items_AggregationViewer.Add_Localization_String( "Dated XXX", "Dated %1");
            Private_Items_AggregationViewer.Add_Localization_String( "Record Created", "record created");
            Private_Items_AggregationViewer.Add_Localization_String( "Scanned", "scanned");
            Private_Items_AggregationViewer.Add_Localization_String( "Processed", "processed");
            Private_Items_AggregationViewer.Add_Localization_String( "Quality Control", "quality control");
            Private_Items_AggregationViewer.Add_Localization_String( "Online Completed", "online completed");
            Private_Items_AggregationViewer.Add_Localization_String( "Unknown Milestone", "unknown milestone");
            Private_Items_AggregationViewer.Add_Localization_String( "Days Ago", "% days ago");
            Private_Items_AggregationViewer.Add_Localization_String( "First Page", "First Page");
            Private_Items_AggregationViewer.Add_Localization_String( "Previous Page", "Previous Page");
            Private_Items_AggregationViewer.Add_Localization_String( "Next Page", "Next Page");
            Private_Items_AggregationViewer.Add_Localization_String( "Last Page", "Last Page");
            Private_Items_AggregationViewer.Add_Localization_String( "XXX Volumes Out Of XXX Total Volumes", "%1 volumes out of %2 total volumes");

            //Initialize the Rotating_Highlight_Search_AggregationViewer_Localization class
            Rotating_Highlight_Search_AggregationViewer = new Rotating_Highlight_Search_AggregationViewer_LocalizationInfo();
            Rotating_Highlight_Search_AggregationViewer.Add_Localization_String( "Search Collection", "Search Collection:");

            //Initialize the Usage_Statistics_AggregationViewer_Localization class
            Usage_Statistics_AggregationViewer = new Usage_Statistics_AggregationViewer_LocalizationInfo();
            Usage_Statistics_AggregationViewer.Add_Localization_String( "History Of Collectionlevel Usage", "History of Collection-Level Usage");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "History Of Item Usage", "History of Item Usage");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Most Accessed Titles", "Most Accessed Titles");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Most Accessed Items", "Most Accessed Items");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Definitions Of Terms Used", "Definitions of Terms Used");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Collection Views", "Collection Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Item Views", "Item Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Top Titles", "Top Titles");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Top Items", "Top Items");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Definitions", "Definitions");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Usage History For This Collection Is Displayed Below This History Includes Just The Toplevel Views Of The Collection", "Usage history for this collection is displayed below. This history includes just the top-level views of the collection.");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Definitions Page Provides More Details About The Statistics And Words Used Below", "Definitions page provides more details about the statistics and words used below.");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Date", "Date");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Total Views", "Total Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Visits", "Visits");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Main Pages", "Main Pages");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Browses", "Browses");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Search Results", "Search Results");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Title Views", "Title Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Usage History For The Items Within This Collection Are Displayed Below", "Usage history for the items within this collection are displayed below.");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "JPEG Views", "JPEG Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Zoomable Views", "Zoomable Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Citation Views", "Citation Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Thumbnail Views", "Thumbnail Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Text Searches", "Text Searches");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Flash Views", "Flash Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Map Views", "Map Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Download Views", "Download Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Static Views", "Static Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "The Most Commonly Utilized Items For This Collection Appear Below", "The most commonly utilized items for this collection appear below.");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Title", "Title");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Views", "Views");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "The Most Commonly Utilized Titles By Collection Appear Below", "The most commonly utilized titles by collection appear below.");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "XXX Statistics", "%1 Statistics");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Collection Hierarchy", "Collection Hierarchy");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Collection Groups", "Collection Groups");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Collections", "Collections");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Subcollections", "SubCollections");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Titles And Items", "Titles and Items");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "Bibid", "BibID");
            Usage_Statistics_AggregationViewer.Add_Localization_String( "VID", "VID");

            //Initialize the Contact_HtmlSubwriter_Localization class
            Contact_HtmlSubwriter = new Contact_HtmlSubwriter_LocalizationInfo();
            Contact_HtmlSubwriter.Add_Localization_String( "The Following Information Is Collected To Allow Us Better Serve Your Needs", "The following information is collected to allow us better serve your needs.");
            Contact_HtmlSubwriter.Add_Localization_String( "PERSONAL INFORMATION", "PERSONAL INFORMATION");
            Contact_HtmlSubwriter.Add_Localization_String( "Name", "Name:");
            Contact_HtmlSubwriter.Add_Localization_String( "Email", "Email:");
            Contact_HtmlSubwriter.Add_Localization_String( "XXX Submission", "[ %1 Submission ]");
            Contact_HtmlSubwriter.Add_Localization_String( "Submit", "Submit");
            Contact_HtmlSubwriter.Add_Localization_String( "Cancel", "Cancel");
            Contact_HtmlSubwriter.Add_Localization_String( "Your Email Has Been Sent", "Your email has been sent.");
            Contact_HtmlSubwriter.Add_Localization_String( "Click Here To Return To The Digital Collection Home", "Click here to return to the digital collection home");
            Contact_HtmlSubwriter.Add_Localization_String( "Click Here To Close This Tab In Your Browser", "Click here to close this tab in your browser");
            Contact_HtmlSubwriter.Add_Localization_String( "Contact Us", "Contact Us");
            Contact_HtmlSubwriter.Add_Localization_String( "Please Complete The Following Required Fields", "Please complete the following required fields:");
            Contact_HtmlSubwriter.Add_Localization_String( "Enter A Subject Here", "Enter a subject here:");
            Contact_HtmlSubwriter.Add_Localization_String( "Describe Your Question Or Problem Here", "Describe your question or problem here:");
            Contact_HtmlSubwriter.Add_Localization_String( "May We Contact You If So Please Provide The Following Information", "\"May we contact you?  If so, please provide the following information:\"");
            Contact_HtmlSubwriter.Add_Localization_String( "Enter Your Name Here", "Enter your name here:");
            Contact_HtmlSubwriter.Add_Localization_String( "Enter Your Email Address Here", "Enter your e-mail address here:");

            //Initialize the Error_HtmlSubwriter_Localization class
            Error_HtmlSubwriter = new Error_HtmlSubwriter_LocalizationInfo();
            Error_HtmlSubwriter.Add_Localization_String( "The Item Indicated Was Not Valid", "The item indicated was not valid.");
            Error_HtmlSubwriter.Add_Localization_String( "Click Here To Report An Error", "Click here to report an error.");
            Error_HtmlSubwriter.Add_Localization_String( "Unknown Error Occurred", "Unknown error occurred.");
            Error_HtmlSubwriter.Add_Localization_String( "We Apologize For The Inconvenience", "We apologize for the inconvenience.");
            Error_HtmlSubwriter.Add_Localization_String( "Click Here To Return To The Library", "Click here to return to the library.");
            Error_HtmlSubwriter.Add_Localization_String( "Click Here To Report The Problem", "Click here to report the problem.");

            //Initialize the Internal_HtmlSubwriter_Localization class
            Internal_HtmlSubwriter = new Internal_HtmlSubwriter_LocalizationInfo();
            Internal_HtmlSubwriter.Add_Localization_String( "Collection Hierarchy", "Collection Hierarchy");
            Internal_HtmlSubwriter.Add_Localization_String( "Active And Inactive Collections", "Active and Inactive Collections");
            Internal_HtmlSubwriter.Add_Localization_String( "New Items", "New Items");
            Internal_HtmlSubwriter.Add_Localization_String( "Newly Added Or Modified Items", "Newly Added or Modified Items");
            Internal_HtmlSubwriter.Add_Localization_String( "Memory Management", "Memory Management");
            Internal_HtmlSubwriter.Add_Localization_String( "Current Memory Profile", "Current Memory Profile");
            Internal_HtmlSubwriter.Add_Localization_String( "Wordmarks", "Wordmarks");
            Internal_HtmlSubwriter.Add_Localization_String( "Build Failures", "Build Failures");
            Internal_HtmlSubwriter.Add_Localization_String( "Build Failure Logs", "Build Failure Logs");
            Internal_HtmlSubwriter.Add_Localization_String( "Internal Users Only", "Internal Users Only");
            Internal_HtmlSubwriter.Add_Localization_String( "You Are Not Authorized To Access This View", "You are not authorized to access this view.");
            Internal_HtmlSubwriter.Add_Localization_String( "Click Here To Return To The Digital Library Home Page", "Click here to return to the digital library home page.");
            Internal_HtmlSubwriter.Add_Localization_String( "The Data Below Shows Errors Which Occurred While Loading New Items Through The Builder These Can Be Displayed By Month And Year Below By Selecting The Start And End Month These Failures Will Continue To Display Until They Are Manually Cleared Or Until The Item Successfully Loads After The Failure Or Warning", "The data below shows errors which occurred while loading new items through the builder.  These can be displayed by month and year below by selecting the start and end month.  These failures will continue to display until they are manually cleared or until the item successfully loads after the failure or warning.");
            Internal_HtmlSubwriter.Add_Localization_String( "Selected Date Range", "Selected Date Range");
            Internal_HtmlSubwriter.Add_Localization_String( "The Failures And Warnings Which Were Encountered During Build Are Searchable Below By Month", "\"The failures and warnings which were encountered during build are searchable below, by month:\"");
            Internal_HtmlSubwriter.Add_Localization_String( "From", "From:");
            Internal_HtmlSubwriter.Add_Localization_String( "To", "To:");
            Internal_HtmlSubwriter.Add_Localization_String( "To Change The Date Shown Choose Your Dates Above And Hit The GO Button", "\"To change the date shown, choose your dates above and hit the GO button.\"");
            Internal_HtmlSubwriter.Add_Localization_String( "Build Failures And Warnings", "Build Failures and Warnings");
            Internal_HtmlSubwriter.Add_Localization_String( "No Uncleared Warnings Or Failures For The Selected Date Range", "No uncleared warnings or failures for the selected date range.");
            Internal_HtmlSubwriter.Add_Localization_String( "METS Type", "METS Type");
            Internal_HtmlSubwriter.Add_Localization_String( "Description", "Description");
            Internal_HtmlSubwriter.Add_Localization_String( "GLOBAL VALUES", "GLOBAL VALUES");
            Internal_HtmlSubwriter.Add_Localization_String( "APPLICATION STATE VALUES", "APPLICATION STATE VALUES");
            Internal_HtmlSubwriter.Add_Localization_String( "LOCALLY CACHED OBJECTS", "LOCALLY CACHED OBJECTS");
            Internal_HtmlSubwriter.Add_Localization_String( "REMOTELY CACHED OBJECTS", "REMOTELY CACHED OBJECTS");
            Internal_HtmlSubwriter.Add_Localization_String( "SESSION STATE VALUES", "SESSION STATE VALUES");
            Internal_HtmlSubwriter.Add_Localization_String( "INSTANCE NAME", "INSTANCE NAME");
            Internal_HtmlSubwriter.Add_Localization_String( "KEY", "KEY");
            Internal_HtmlSubwriter.Add_Localization_String( "OBJECT", "OBJECT");
            Internal_HtmlSubwriter.Add_Localization_String( "UNKNOWN OBJECT TYPE", "UNKNOWN OBJECT TYPE");
            Internal_HtmlSubwriter.Add_Localization_String( "None", "( none )");
            Internal_HtmlSubwriter.Add_Localization_String( "Select One Of The Aggregation Types Below To View Information About All Aggregations Of That Type", "Select one of the aggregation types below to view information about all aggregations of that type.");
            Internal_HtmlSubwriter.Add_Localization_String( "All Aggregation Types", "All Aggregation Types");
            Internal_HtmlSubwriter.Add_Localization_String( "Parent Aggregations", "Parent Aggregations");
            Internal_HtmlSubwriter.Add_Localization_String( "Child Aggregations", "Child Aggregations");
            Internal_HtmlSubwriter.Add_Localization_String( "Active Aggregations", "Active Aggregations");
            Internal_HtmlSubwriter.Add_Localization_String( "Inactive Aggregations", "Inactive Aggregations");
            Internal_HtmlSubwriter.Add_Localization_String( "Below Is The Complete Master List Of All Aggregations Within This Library This Includes All Active Aggregations As Well As All Hidden Or Inactive Collections", "\"Below is the complete master list of all aggregations within this library.  This includes all active aggregations, as well as all hidden or inactive collections.\"");
            Internal_HtmlSubwriter.Add_Localization_String( "Click Here To Sort By DATE ADDED", "Click here to sort by DATE ADDED");
            Internal_HtmlSubwriter.Add_Localization_String( "Click Here To Sort By CODE", "Click here to sort by CODE");
            Internal_HtmlSubwriter.Add_Localization_String( "Sobekcm Code", "SobekCM Code");
            Internal_HtmlSubwriter.Add_Localization_String( "Type", "Type");
            Internal_HtmlSubwriter.Add_Localization_String( "Name", "Name");
            Internal_HtmlSubwriter.Add_Localization_String( "Date Added", "Date Added");
            Internal_HtmlSubwriter.Add_Localization_String( "METS TYPE", "METS TYPE");
            Internal_HtmlSubwriter.Add_Localization_String( "USER", "USER");
            Internal_HtmlSubwriter.Add_Localization_String( "ALL", "ALL");
            Internal_HtmlSubwriter.Add_Localization_String( "ONLINE EDITS", "ONLINE EDITS");
            Internal_HtmlSubwriter.Add_Localization_String( "ONLINE SUBMITS", "ONLINE SUBMITS");
            Internal_HtmlSubwriter.Add_Localization_String( "VISIBILITY CHANGES", "VISIBILITY CHANGES");
            Internal_HtmlSubwriter.Add_Localization_String( "BULK LOADED", "BULK LOADED");
            Internal_HtmlSubwriter.Add_Localization_String( "POSTPROCESSED", "POST-PROCESSED");
            Internal_HtmlSubwriter.Add_Localization_String( "NO NEW ITEMS", "NO NEW ITEMS");
            Internal_HtmlSubwriter.Add_Localization_String( "There Have Been An Unusually Large Number Of Updates Over The Last Week", "There have been an unusually large number of updates over the last week.");
            Internal_HtmlSubwriter.Add_Localization_String( "Select The Update Type Tab Above To View The Details", "Select the update type tab above to view the details.");
            Internal_HtmlSubwriter.Add_Localization_String( "NO INFORMATION FOR YOUR SELECTION", "NO INFORMATION FOR YOUR SELECTION");

            //Initialize the LegacyUrl_HtmlSubwriter_Localization class
            LegacyUrl_HtmlSubwriter = new LegacyUrl_HtmlSubwriter_LocalizationInfo();
            LegacyUrl_HtmlSubwriter.Add_Localization_String( "Deprecated URL Detected", "Deprecated URL detected");
            LegacyUrl_HtmlSubwriter.Add_Localization_String( "The URL You Entered Is A Legacy URL Support For This URL Will End Shortly", "The URL you entered is a legacy URL.  Support for this URL will end shortly.");
            LegacyUrl_HtmlSubwriter.Add_Localization_String( "Please Update Your Records To The New URL Below", "Please update your records to the new URL below:");

            //Initialize the Preferences_HtmlSubwriter_Localization class
            Preferences_HtmlSubwriter = new Preferences_HtmlSubwriter_LocalizationInfo();
            Preferences_HtmlSubwriter.Add_Localization_String( "Preferences", "Preferences");
            Preferences_HtmlSubwriter.Add_Localization_String( "Language", "Language:");
            Preferences_HtmlSubwriter.Add_Localization_String( "Return", "Return");
            Preferences_HtmlSubwriter.Add_Localization_String( "Default View", "Default View:");
            Preferences_HtmlSubwriter.Add_Localization_String( "Default Sort", "Default Sort:");

            //Initialize the Print_Item_HtmlSubwriter_Localization class
            Print_Item_HtmlSubwriter = new Print_Item_HtmlSubwriter_LocalizationInfo();
            Print_Item_HtmlSubwriter.Add_Localization_String( "Title", "Title:");
            Print_Item_HtmlSubwriter.Add_Localization_String( "URL", "URL:");
            Print_Item_HtmlSubwriter.Add_Localization_String( "Site", "Site:");

            //Initialize the Public_Folder_HtmlSubwriter_Localization class
            Public_Folder_HtmlSubwriter = new Public_Folder_HtmlSubwriter_LocalizationInfo();
            Public_Folder_HtmlSubwriter.Add_Localization_String( "Public Bookshelf", "Public Bookshelf");
            Public_Folder_HtmlSubwriter.Add_Localization_String( "XXX Home", "%1 Home");

            //Initialize the Search_Results_HtmlSubwriter_Localization class
            Search_Results_HtmlSubwriter = new Search_Results_HtmlSubwriter_LocalizationInfo();
            Search_Results_HtmlSubwriter.Add_Localization_String( "Modify Your Search", "Modify your search");
            Search_Results_HtmlSubwriter.Add_Localization_String( "New Search", "New Search");

            //Initialize the Statistics_HtmlSubwriter_Localization class
            Statistics_HtmlSubwriter = new Statistics_HtmlSubwriter_LocalizationInfo();
            Statistics_HtmlSubwriter.Add_Localization_String( "Item Count", "Item Count");
            Statistics_HtmlSubwriter.Add_Localization_String( "Usage Statistics", "Usage Statistics");
            Statistics_HtmlSubwriter.Add_Localization_String( "Recent Searches", "Recent Searches");
            Statistics_HtmlSubwriter.Add_Localization_String( "Resource Count In XXX", "Resource count in %1");
            Statistics_HtmlSubwriter.Add_Localization_String( "Recent Searches In XXX", "Recent searches in %1");
            Statistics_HtmlSubwriter.Add_Localization_String( "Usage Statistics For XXX", "Usage statistics for %1");
            Statistics_HtmlSubwriter.Add_Localization_String( "Standard View", "Standard View");
            Statistics_HtmlSubwriter.Add_Localization_String( "FYTD Growth View", "FYTD Growth View");
            Statistics_HtmlSubwriter.Add_Localization_String( "Arbitrary Dates", "Arbitrary Dates");
            Statistics_HtmlSubwriter.Add_Localization_String( "Overall Stats", "Overall Stats");
            Statistics_HtmlSubwriter.Add_Localization_String( "Collections By Date", "Collections by Date");
            Statistics_HtmlSubwriter.Add_Localization_String( "Item Views By Date", "Item Views by Date");
            Statistics_HtmlSubwriter.Add_Localization_String( "Collection History", "Collection History");
            Statistics_HtmlSubwriter.Add_Localization_String( "Top Titles", "Top Titles");
            Statistics_HtmlSubwriter.Add_Localization_String( "Definitions", "Definitions");
            Statistics_HtmlSubwriter.Add_Localization_String( "Browse Partners", "Browse Partners");
            Statistics_HtmlSubwriter.Add_Localization_String( "Advanced Search", "Advanced Search");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Most Commonly Accessed Items By Collection Appear Below", "The most commonly accessed items by collection appear below.");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Definitions Page Provides More Details About The Statistics And Words Used Below", "The Definitions page provides more details about the statistics and words used below.");
            Statistics_HtmlSubwriter.Add_Localization_String( "Selected Collection", "Selected Collection");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Most Commonly Accessed Items Below Are Displayed For The Following Collection", "The most commonly accessed items below are displayed for the following collection:");
            Statistics_HtmlSubwriter.Add_Localization_String( "From", "From:");
            Statistics_HtmlSubwriter.Add_Localization_String( "To Change The Collection Shown Choose The Collection Above And Hit The GO Button", "\"To change the collection shown, choose the collection above and hit the GO button.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "Most Accessed Items", "Most Accessed Items");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Data Below Shows The Most Commonly Accessed Items Within The Collection Above", "The data below shows the most commonly accessed items within the collection above.");
            Statistics_HtmlSubwriter.Add_Localization_String( "Click Here To View The Most Commonly Accessed TITLES", "Click here to view the most commonly accessed TITLES");
            Statistics_HtmlSubwriter.Add_Localization_String( "Views", "Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Most Commonly Utilized Titles By Collection Appear Below", "The most commonly utilized titles by collection appear below.");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Most Commonly Accessed Titles Below Are Displayed Is For The Following Collection", "The most commonly accessed titles below are displayed is for the following collection:");
            Statistics_HtmlSubwriter.Add_Localization_String( "Most Accessed Titles", "Most Accessed Titles");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Data Below Shows The Most Commonly Accessed Titles Within The Collection Above", "The data below shows the most commonly accessed titles within the collection above.");
            Statistics_HtmlSubwriter.Add_Localization_String( "Click Here To View The Most Commonly Accessed ITEMS", "Click here to view the most commonly accessed ITEMS");
            Statistics_HtmlSubwriter.Add_Localization_String( "Below Is The Collection And Itemlevel Details For Your Provided Date Range In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel", "\"Below is the collection and item-level details for your provided date range in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "Select Collection", "Select Collection");
            Statistics_HtmlSubwriter.Add_Localization_String( "By Name", "By Name");
            Statistics_HtmlSubwriter.Add_Localization_String( "Below Are The Details About The Specialized Views For Items Aggregated At The Collection Levels", "Below are the details about the specialized views for items aggregated at the collection levels.");
            Statistics_HtmlSubwriter.Add_Localization_String( "For The Number Of Times Collections Were Viewed Or Searched See Collections By Date", "\"For the number of times collections were viewed or searched, see Collections by Date.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "Selected Date Range", "Selected Date Range");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Usage For All Items Appears Below For The Following Date Range", "The usage for all items appears below for the following date range:");
            Statistics_HtmlSubwriter.Add_Localization_String( "To", "To:");
            Statistics_HtmlSubwriter.Add_Localization_String( "To Change The Date Shown Choose Your Dates Above And Hit The GO Button", "\"To change the date shown, choose your dates above and hit the GO button.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "Summary By Collection", "Summary by Collection");
            Statistics_HtmlSubwriter.Add_Localization_String( "NO USAGE STATISTICS EXIST FOR YOUR SELECTION", "NO USAGE STATISTICS EXIST FOR YOUR SELECTION");
            Statistics_HtmlSubwriter.Add_Localization_String( "Export As CSV", "Export as CSV");
            Statistics_HtmlSubwriter.Add_Localization_String( "JPEG Views", "JPEG Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Zoomable Views", "Zoomable Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Citation Views", "Citation Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Thumbnail Views", "Thumbnail Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Text Searches", "Text Searches");
            Statistics_HtmlSubwriter.Add_Localization_String( "Flash Views", "Flash Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Map Views", "Map Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Download Views", "Download Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Static Views", "Static Views");
            Statistics_HtmlSubwriter.Add_Localization_String( "Collections", "Collections");
            Statistics_HtmlSubwriter.Add_Localization_String( "Type", "Type");
            Statistics_HtmlSubwriter.Add_Localization_String( "Terms", "Terms");
            Statistics_HtmlSubwriter.Add_Localization_String( "Time", "Time");
            Statistics_HtmlSubwriter.Add_Localization_String( "Title", "Title");
            Statistics_HtmlSubwriter.Add_Localization_String( "NUMBER", "NUMBER");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Data Below Shows Details For The Views That Occurred At Each Level Of The Collection Hierarchy Collection Groups Collections Subcollections", "\"The data below shows details for the views that occurred at each level of the collection hierarchy (Collection Groups, Collections, SubCollections).\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "These Statistics Show The Number Of Times Users", "These statistics show the number of times users:");
            Statistics_HtmlSubwriter.Add_Localization_String( "Navigated To The Collection Main Pages", "navigated to the collection main pages");
            Statistics_HtmlSubwriter.Add_Localization_String( "Browsed The Items Or Information About The Collection", "browsed the items or information about the collection");
            Statistics_HtmlSubwriter.Add_Localization_String( "Performed Searches Against The Items Contained In The Collection", "performed searches against the items contained in the collection");
            Statistics_HtmlSubwriter.Add_Localization_String( "Viewed Titles And Items Contained Within The Collection", "viewed titles and items contained within the collection");
            Statistics_HtmlSubwriter.Add_Localization_String( "For The Specialized Itemlevel View Details See Item Views By Date", "\"For the specialized item-level view details, see Item Views by Date.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Usage For All The Collections Appears Below For The Following Data Range", "The usage for all the collections appears below for the following data range:");
            Statistics_HtmlSubwriter.Add_Localization_String( "GROUP CODE", "GROUP CODE");
            Statistics_HtmlSubwriter.Add_Localization_String( "COLL CODE", "COLL CODE");
            Statistics_HtmlSubwriter.Add_Localization_String( "SUB CODE", "SUB CODE");
            Statistics_HtmlSubwriter.Add_Localization_String( "TOTAL VIEWS", "TOTAL VIEWS");
            Statistics_HtmlSubwriter.Add_Localization_String( "VISITS", "VISITS");
            Statistics_HtmlSubwriter.Add_Localization_String( "MAIN PAGES", "MAIN PAGES");
            Statistics_HtmlSubwriter.Add_Localization_String( "BROWSES", "BROWSES");
            Statistics_HtmlSubwriter.Add_Localization_String( "SEARCH RESULTS", "SEARCH RESULTS");
            Statistics_HtmlSubwriter.Add_Localization_String( "TITLE VIEWS", "TITLE VIEWS");
            Statistics_HtmlSubwriter.Add_Localization_String( "ITEM VIEWS", "ITEM VIEWS");
            Statistics_HtmlSubwriter.Add_Localization_String( "ALL COLLECTION GROUPS", "ALL COLLECTION GROUPS");
            Statistics_HtmlSubwriter.Add_Localization_String( "Usage History For A Single Collection Is Displayed Below This History Includes Views Of The Collection And Views Of The Items Within The Collections", "Usage history for a single collection is displayed below. This history includes views of the collection and views of the items within the collections.");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Usage Displayed Is For The Following Collection", "The usage displayed is for the following collection:");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Data Below Shows The Collection History For The Selected Collection The First Table Shows The Summary Of All Views Of This Collection And Items Contained In The Collection The Second Table Includes The Details For Specialized Itemlevel Views", "The data below shows the collection history for the selected collection.  The first table shows the summary of all views of this collection and items contained in the collection.  The second table includes the details for specialized item-level views.");
            Statistics_HtmlSubwriter.Add_Localization_String( "Below Is The History Collection Information Included In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel", "\"Below is the history collection information included in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "MONTH", "MONTH");
            Statistics_HtmlSubwriter.Add_Localization_String( "Titles And Items", "Titles and Items");
            Statistics_HtmlSubwriter.Add_Localization_String( "Below Are The Most Up To Date Numbers For Overall Utilization Of XXX The First Table Shows The Summary Of Views Against All Collections The Second Table Includes The Details For Specialized Itemlevel Views", "Below are the most up to date numbers for overall utilization of %1.  The first table shows the summary of views against all collections.  The second table includes the details for specialized item-level views.");
            Statistics_HtmlSubwriter.Add_Localization_String( "NO ITEM COUNT AVAILABLE", "NO ITEM COUNT AVAILABLE");
            Statistics_HtmlSubwriter.Add_Localization_String( "Below Are The Number Of Items In Each Collection And Subcollection", "Below are the number of items in each collection and subcollection.");
            Statistics_HtmlSubwriter.Add_Localization_String( "TITLES", "TITLES");
            Statistics_HtmlSubwriter.Add_Localization_String( "ITEMS", "ITEMS");
            Statistics_HtmlSubwriter.Add_Localization_String( "PAGES", "PAGES");
            Statistics_HtmlSubwriter.Add_Localization_String( "FYTD TITLES", "FYTD TITLES");
            Statistics_HtmlSubwriter.Add_Localization_String( "FYTD ITEMS", "FYTD ITEMS");
            Statistics_HtmlSubwriter.Add_Localization_String( "FYTD PAGES", "FYTD PAGES");
            Statistics_HtmlSubwriter.Add_Localization_String( "List View", "List View");
            Statistics_HtmlSubwriter.Add_Localization_String( "Brief View", "Brief View");
            Statistics_HtmlSubwriter.Add_Localization_String( "Tree View", "Tree View");
            Statistics_HtmlSubwriter.Add_Localization_String( "Bibid", "BibID");
            Statistics_HtmlSubwriter.Add_Localization_String( "VID", "VID");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Following Terms Are Defined Below", "The following terms are defined below:");
            Statistics_HtmlSubwriter.Add_Localization_String( "Collection Hierarchy", "Collection Hierarchy");
            Statistics_HtmlSubwriter.Add_Localization_String( "Collection Groups", "Collection Groups");
            Statistics_HtmlSubwriter.Add_Localization_String( "Subcollections", "SubCollections");
            Statistics_HtmlSubwriter.Add_Localization_String( "NO STATISTICS FOUND FOR THAT DATE RANGE", "NO STATISTICS FOUND FOR THAT DATE RANGE");
            Statistics_HtmlSubwriter.Add_Localization_String( "Below Is The Item Count With Fiscal Year To Date Information Included In Commaseperated Value Form To Use The Data Below Cut And Paste It Into A CSV Or Text File The Resulting File Can Be Opened In A Variety Of Applications Including Openoffice And Microsoft Excel", "\"Below is the item count with fiscal year to date information included in comma-seperated value form.  To use the data below, cut and paste it into a CSV or text file.  The resulting file can be opened in a variety of applications, including OpenOffice and Microsoft Excel.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "Defined Terms", "Defined Terms");
            Statistics_HtmlSubwriter.Add_Localization_String( "CODES", "CODES");
            Statistics_HtmlSubwriter.Add_Localization_String( "NAME", "NAME");
            Statistics_HtmlSubwriter.Add_Localization_String( "TOTAL", "TOTAL");
            Statistics_HtmlSubwriter.Add_Localization_String( "This Option Allows The Complete Title Count Item Count And Page Count To Be Viewed For A Previous Time And To Additionally See The Growth Between Two Arbitrary Dates", "\"This option allows the complete title count, item count, and page count to be viewed for a previous time and to additionally see the growth between two arbitrary dates.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Title Count Item Count And Page Count Appear Below For The Following Two Arbitrary Dates", "\"The title count, item count, and page count appear below for the following two arbitrary dates:\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "To Change The Dates Shown Choose Your Dates Above And Hit The GO Button", "\"To change the dates shown, choose your dates above and hit the GO button.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "Item Count And Growth", "Item Count and Growth");
            Statistics_HtmlSubwriter.Add_Localization_String( "Below Are The Number Of Titles Items And Pages In Each Collection And Subcollection The Quotinitialquot Values Are The Values That Were Present At Midnight On XXX Before Any New Items Were Processed That Day", "\"Below are the number of titles, items, and pages in each collection and subcollection.  The &quot;INITIAL&quot; values are the values that were present at midnight on %1 before any new items were processed that day.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "The Quotaddedquot Values Are The Number Of Titles Items And Pages That Were Added Between XXX And XXX Inclusive", "\" The &quot;ADDED&quot; values are the number of titles, items, and pages that were added between %1 and %2 inclusive.\"");
            Statistics_HtmlSubwriter.Add_Localization_String( "INITIAL TITLES", "INITIAL TITLES");
            Statistics_HtmlSubwriter.Add_Localization_String( "INITIAL ITEMS", "INITIAL ITEMS");
            Statistics_HtmlSubwriter.Add_Localization_String( "INITIAL PAGES", "INITIAL PAGES");

            //Initialize the Item_HtmlSubwriter_Localization class
            Item_HtmlSubwriter = new Item_HtmlSubwriter_LocalizationInfo();
            Item_HtmlSubwriter.Add_Localization_String( "Error Encountered While Sending Email", "Error encountered while sending email");
            Item_HtmlSubwriter.Add_Localization_String( "Error Encountered While Trying To Save To Your Bookshelf", "Error encountered while trying to save to your bookshelf");
            Item_HtmlSubwriter.Add_Localization_String( "Your Email Has Been Sent", "Your email has been sent");
            Item_HtmlSubwriter.Add_Localization_String( "Item Was Saved To Your Bookshelf", "Item was saved to your bookshelf.");
            Item_HtmlSubwriter.Add_Localization_String( "ERROR Encountered While Trying To Save To Your Bookshelf", "ERROR encountered while trying to save to your bookshelf.");
            Item_HtmlSubwriter.Add_Localization_String( "Item Was Removed From Your Bookshelves", "Item was removed from your bookshelves.");
            Item_HtmlSubwriter.Add_Localization_String( "ERROR Encountered While Trying To Remove Item From Your Bookshelves", "ERROR encountered while trying to remove item from your bookshelves.");
            Item_HtmlSubwriter.Add_Localization_String( "MATCHING PAGES", "MATCHING PAGES");
            Item_HtmlSubwriter.Add_Localization_String( "MATCHING TILES", "MATCHING TILES");
            Item_HtmlSubwriter.Add_Localization_String( "SHOW TABLE OF CONTENTS", "SHOW TABLE OF CONTENTS");
            Item_HtmlSubwriter.Add_Localization_String( "HIDE", "HIDE");
            Item_HtmlSubwriter.Add_Localization_String( "TABLE OF CONTENTS", "TABLE OF CONTENTS");
            Item_HtmlSubwriter.Add_Localization_String( "HIDE TABLE OF CONTENTS", "HIDE TABLE OF CONTENTS");
            Item_HtmlSubwriter.Add_Localization_String( "View Work Log", "View Work Log");
            Item_HtmlSubwriter.Add_Localization_String( "Private Resource", "Private Resource");
            Item_HtmlSubwriter.Add_Localization_String( "Public Resource", "Public Resource");
            Item_HtmlSubwriter.Add_Localization_String( "Accession Number", "Accession number");
            Item_HtmlSubwriter.Add_Localization_String( "Usage Statistics", "Usage Statistics");
            Item_HtmlSubwriter.Add_Localization_String( "MARC View", "MARC View");
            Item_HtmlSubwriter.Add_Localization_String( "Metadata", "Metadata");
            Item_HtmlSubwriter.Add_Localization_String( "Tree View", "Tree View");
            Item_HtmlSubwriter.Add_Localization_String( "Thumbnails", "Thumbnails");
            Item_HtmlSubwriter.Add_Localization_String( "List View", "List View");
            Item_HtmlSubwriter.Add_Localization_String( "Manage Download Files", "Manage Download Files");
            Item_HtmlSubwriter.Add_Localization_String( "Edit Item Behaviors", "Edit Item Behaviors");
            Item_HtmlSubwriter.Add_Localization_String( "Manage Pages And Divisions", "Manage Pages and Divisions");
            Item_HtmlSubwriter.Add_Localization_String( "Manage Geospatial Data", "Manage Geo-Spatial Data");
            Item_HtmlSubwriter.Add_Localization_String( "Beta", "beta");
            Item_HtmlSubwriter.Add_Localization_String( "View Tracking Sheet", "View Tracking Sheet");
            Item_HtmlSubwriter.Add_Localization_String( "Mass Update Item Behaviors", "Mass Update Item Behaviors");
            Item_HtmlSubwriter.Add_Localization_String( "First Page", "First Page");
            Item_HtmlSubwriter.Add_Localization_String( "Edit Item Group Behaviors", "Edit Item Group Behaviors");
            Item_HtmlSubwriter.Add_Localization_String( "Previous Page", "Previous Page");
            Item_HtmlSubwriter.Add_Localization_String( "Next Page", "Next Page");
            Item_HtmlSubwriter.Add_Localization_String( "Last Page", "Last Page");
            Item_HtmlSubwriter.Add_Localization_String( "Next", "Next");
            Item_HtmlSubwriter.Add_Localization_String( "Last", "Last");
            Item_HtmlSubwriter.Add_Localization_String( "First", "First");
            Item_HtmlSubwriter.Add_Localization_String( "Previous", "Previous");
            Item_HtmlSubwriter.Add_Localization_String( "Edit Metadata", "Edit Metadata");
            Item_HtmlSubwriter.Add_Localization_String( "Edit Behaviors", "Edit Behaviors");
            Item_HtmlSubwriter.Add_Localization_String( "Change Access Restriction", "Change Access Restriction");
            Item_HtmlSubwriter.Add_Localization_String( "Public Item", "Public Item");
            Item_HtmlSubwriter.Add_Localization_String( "Private Item", "Private Item");
            Item_HtmlSubwriter.Add_Localization_String( "Dark Item", "Dark Item");
            Item_HtmlSubwriter.Add_Localization_String( "Restricted Item", "Restricted Item");
            Item_HtmlSubwriter.Add_Localization_String( "View Work History", "View Work History");
            Item_HtmlSubwriter.Add_Localization_String( "Manage Files", "Manage Files");
            Item_HtmlSubwriter.Add_Localization_String( "Perform QC", "Perform QC");
            Item_HtmlSubwriter.Add_Localization_String( "Perform Quality Control", "Perform Quality Control");
            Item_HtmlSubwriter.Add_Localization_String( "Comments", "Comments");
            Item_HtmlSubwriter.Add_Localization_String( "Save New Internal Comments", "Save new internal comments");
            Item_HtmlSubwriter.Add_Localization_String( "SET ACCESS RESTRICTIONS", "SET ACCESS RESTRICTIONS");
            Item_HtmlSubwriter.Add_Localization_String( "Make Item Public", "Make item public");
            Item_HtmlSubwriter.Add_Localization_String( "Add IP Restriction To This Item", "Add IP restriction to this item");
            Item_HtmlSubwriter.Add_Localization_String( "Make Item Private", "Make item private");
            Item_HtmlSubwriter.Add_Localization_String( "Delete This Item", "Delete this item");
            Item_HtmlSubwriter.Add_Localization_String( "Cancel Changes", "Cancel changes");
            Item_HtmlSubwriter.Add_Localization_String( "Add Volume", "Add Volume");
            Item_HtmlSubwriter.Add_Localization_String( "Autofill Volumes", "Auto-Fill Volumes");
            Item_HtmlSubwriter.Add_Localization_String( "Edit Serial Hierarchy", "Edit Serial Hierarchy");
            Item_HtmlSubwriter.Add_Localization_String( "Mass Update Volumes", "Mass Update Volumes");
            Item_HtmlSubwriter.Add_Localization_String( "Hide Internal Header", "Hide Internal Header");
            Item_HtmlSubwriter.Add_Localization_String( "Show Internal Header", "Show Internal Header");
            Item_HtmlSubwriter.Add_Localization_String( "Container List", "Container List");
            Item_HtmlSubwriter.Add_Localization_String( "DARK ITEM", "DARK ITEM");
            Item_HtmlSubwriter.Add_Localization_String( "PRIVATE ITEM", "PRIVATE ITEM");
            Item_HtmlSubwriter.Add_Localization_String( "Digitization Of This Item Is Currently In Progress", "Digitization of this item is currently in progress.");
            Item_HtmlSubwriter.Add_Localization_String( "Go To", "Go To:");

            //Initialize the AddRemove_Fragment_ItemViewer_Localization class
            AddRemove_Fragment_ItemViewer = new AddRemove_Fragment_ItemViewer_LocalizationInfo();
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Add This Item To Your Bookshelf", "Add this Item to your Bookshelf");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Enter Notes For This Item In Your Bookshelf", "Enter notes for this item in your bookshelf");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Bookshelf", "Bookshelf:");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "My Bookshelf", "My Bookshelf");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Notes", "Notes:");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Open Bookshelf In New Window", "Open bookshelf in new window");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Cancel", "Cancel");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Save", "Save");
            AddRemove_Fragment_ItemViewer.Add_Localization_String( "Send", "Send");

            //Initialize the Describe_Fragment_ItemViewer_Localization class
            Describe_Fragment_ItemViewer = new Describe_Fragment_ItemViewer_LocalizationInfo();
            Describe_Fragment_ItemViewer.Add_Localization_String( "Add Item Description", "Add Item Description");
            Describe_Fragment_ItemViewer.Add_Localization_String( "Enter A Description Or Notes To Add To This Item", "Enter a description or notes to add to this item");
            Describe_Fragment_ItemViewer.Add_Localization_String( "Notes", "Notes:");
            Describe_Fragment_ItemViewer.Add_Localization_String( "Close", "Close");

            //Initialize the PrintForm_Fragment_ItemViewer_Localization class
            PrintForm_Fragment_ItemViewer = new PrintForm_Fragment_ItemViewer_LocalizationInfo();
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Print Options", "Print Options");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Include Brief Citation", "Include brief citation?");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Full Citation", "Full citation");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Citation Only", "Citation only");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Print Thumbnails", "Print thumbnails");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Print Current View", "Print current view");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Print Current Page", "Print current page");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Print All Pages", "Print all pages");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Print A Range Of Pages", "Print a range of pages");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "From", "from");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "To", "to");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Print", "Print");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Close", "Close");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Cancel", "Cancel");
            PrintForm_Fragment_ItemViewer.Add_Localization_String( "Select The Options Below To Print This Item", "Select the options below to print this item");

            //Initialize the Send_Fragment_ItemViewer_Localization class
            Send_Fragment_ItemViewer = new Send_Fragment_ItemViewer_LocalizationInfo();
            Send_Fragment_ItemViewer.Add_Localization_String( "Send This Item To A Friend", "Send this Item to a Friend");
            Send_Fragment_ItemViewer.Add_Localization_String( "Enter The Email Information Below", "Enter the email information below");
            Send_Fragment_ItemViewer.Add_Localization_String( "To", "To:");
            Send_Fragment_ItemViewer.Add_Localization_String( "Comments", "Comments:");
            Send_Fragment_ItemViewer.Add_Localization_String( "Format", "Format:");
            Send_Fragment_ItemViewer.Add_Localization_String( "HTML", "HTML:");
            Send_Fragment_ItemViewer.Add_Localization_String( "Text", "Text:");
            Send_Fragment_ItemViewer.Add_Localization_String( "Send", "Send");
            Send_Fragment_ItemViewer.Add_Localization_String( "You Must Wait XXX Seconds Between Sending Emails", "You must wait %1 seconds between sending emails.");
            Send_Fragment_ItemViewer.Add_Localization_String( "You Cannot Send To More Than XXX Email Addresses Simultaneously", "You cannot send to more than %1 email addresses simultaneously.");
            Send_Fragment_ItemViewer.Add_Localization_String( "You Have Reached Your Daily Quota For Emails", "You have reached your daily quota for emails.");
            Send_Fragment_ItemViewer.Add_Localization_String( "Close", "Close");
            Send_Fragment_ItemViewer.Add_Localization_String( "Plain Text", "Plain Text");
            Send_Fragment_ItemViewer.Add_Localization_String( "Cancel", "Cancel");

            //Initialize the Share_Fragment_ItemViewer_Localization class
            Share_Fragment_ItemViewer = new Share_Fragment_ItemViewer_LocalizationInfo();
            Share_Fragment_ItemViewer.Add_Localization_String( "Add To Favorites", "Add to favorites");

            //Initialize the Checked_Out_ItemViewer_Localization class
            Checked_Out_ItemViewer = new Checked_Out_ItemViewer_LocalizationInfo();
            Checked_Out_ItemViewer.Add_Localization_String( "The Item You Have Requested Contains Copyright Material And Is Reserved For Singleuse Br Br Someone Has Currently Checked Out This Digital Copy For Viewing Br Br Please Try Again In Several Minutes", "The item you have requested contains copyright material and is reserved for single-use.  <br /><br />Someone has currently checked out this digital copy for viewing.  <br /><br />Please try again in several minutes.");

            //Initialize the Citation_ItemViewer_Localization class
            Citation_ItemViewer = new Citation_ItemViewer_LocalizationInfo();
            Citation_ItemViewer.Add_Localization_String( "Standard View", "Standard View");
            Citation_ItemViewer.Add_Localization_String( "MARC View", "MARC View");
            Citation_ItemViewer.Add_Localization_String( "Metadata", "Metadata");
            Citation_ItemViewer.Add_Localization_String( "Usage Statistics", "Usage Statistics");
            Citation_ItemViewer.Add_Localization_String( "This Item Was Has Been Viewed XXX Times Within XXX Visits Below Are The Details For Overall Usage For This Item Within This Library", "This item was has been viewed %1 times within %2 visits.  Below are the details for overall usage for this item within this library.");
            Citation_ItemViewer.Add_Localization_String( "For Definitions Of These Terms See The Definitions On The Main Statistics Page", "\"For definitions of these terms, see the definitions on the main statistics page.\"");
            Citation_ItemViewer.Add_Localization_String( "Date", "Date");
            Citation_ItemViewer.Add_Localization_String( "Views", "Views");
            Citation_ItemViewer.Add_Localization_String( "Visits", "Visits");
            Citation_ItemViewer.Add_Localization_String( "JPEG", "JPEG");
            Citation_ItemViewer.Add_Localization_String( "Zoomable", "Zoomable");
            Citation_ItemViewer.Add_Localization_String( "Citation", "Citation");
            Citation_ItemViewer.Add_Localization_String( "Thumbnail", "Thumbnail");
            Citation_ItemViewer.Add_Localization_String( "Text", "Text");
            Citation_ItemViewer.Add_Localization_String( "Flash", "Flash");
            Citation_ItemViewer.Add_Localization_String( "Map", "Map");
            Citation_ItemViewer.Add_Localization_String( "Download", "Download");
            Citation_ItemViewer.Add_Localization_String( "Static", "Static");
            Citation_ItemViewer.Add_Localization_String( "XXX STATISTICS", "%1 STATISTICS");
            Citation_ItemViewer.Add_Localization_String( "Total", "Total");
            Citation_ItemViewer.Add_Localization_String( "View Finding Aid EAD", "View Finding Aid (EAD)");
            Citation_ItemViewer.Add_Localization_String( "Missing Image", "Missing Image");
            Citation_ItemViewer.Add_Localization_String( "The Data Or Metadata About This Digital Resource Is Available In A Variety Of Metadata Formats For More Information About These Formats See The A Href Httpufdcufledusobekcmmetadata Metadata Section A Of The A Href Httpufdcufledusobekcm Technical Aspects A Information", "\"The data (or metadata) about this digital resource is available in a variety of metadata formats. For more information about these formats, see the <a href=\"\"http://ufdc.ufl.edu/sobekcm/metadata\"\">Metadata Section</a> of the <a href=\"\"http://ufdc.ufl.edu/sobekcm/\"\">Technical Aspects</a> information.\"");
            Citation_ItemViewer.Add_Localization_String( "View Complete METSMODS", "View Complete METS/MODS");
            Citation_ItemViewer.Add_Localization_String( "This Metadata File Is The Source Metadata File Submitted Along With All The Digital Resource Files This Contains All Of The Citation And Processing Information Used To Build This Resource This File Follows The Established A Href Httpwwwlocgovstandardsmets Metadata Encoding And Transmission Standard A METS And A Href Httpwwwlocgovstandardsmods Metadata Object Description Schema A MODS This METSMODS File Was Just Read When This Item Was Loaded Into Memory And Used To Display All The Information In The Standard View And Marc View Within The Citation", "\"This metadata file is the source metadata file submitted along with all the digital resource files. This contains all of the citation and processing information used to build this resource. This file follows the established <a href=\"\"http://www.loc.gov/standards/mets/\"\">Metadata Encoding and Transmission Standard</a> (METS) and <a href=\"\"http://www.loc.gov/standards/mods/\"\">Metadata Object Description Schema</a> (MODS). This METS/MODS file was just read when this item was loaded into memory and used to display all the information in the standard view and marc view within the citation.\"");
            Citation_ItemViewer.Add_Localization_String( "View MARC XML File", "View MARC XML File");
            Citation_ItemViewer.Add_Localization_String( "The Entered Metadata Is Also Converted To MARC XML Format For Interoperability With Other Library Catalog Systems This Represents The Same Data Available In The A Href XXX MARC VIEW A Except This Is A Static XML File This File Follows The A Href Httpwwwlocgovstandardsmarcxml Marcxml Schema A", "\"The entered metadata is also converted to MARC XML format, for interoperability with other library catalog systems.  This represents the same data available in the <a href=\"\"%1\"\">MARC VIEW</a> except this is a static XML file.  This file follows the <a href=\"\"http://www.loc.gov/standards/marcxml/\"\">MarcXML Schema</a>.\"");
            Citation_ItemViewer.Add_Localization_String( "View Teitext File", "View TEI/Text File");
            Citation_ItemViewer.Add_Localization_String( "The Fulltext Of This Item Is Also Available In The Established Standard A Href Httpwwwteicorgindexxml Text Encoding Initiative A TEI Downloadable File", "\"The full-text of this item is also available in the established standard <a href=\"\"http://www.tei-c.org/index.xml\"\">Text Encoding Initiative</a> (TEI) downloadable file.\"");
            Citation_ItemViewer.Add_Localization_String( "The Record Above Was Autogenerated From The METS File", "The record above was auto-generated from the METS file.");
            Citation_ItemViewer.Add_Localization_String( "Material Information", "Material Information");
            Citation_ItemViewer.Add_Localization_String( "Notes", "Notes");
            Citation_ItemViewer.Add_Localization_String( "Subjects", "Subjects");
            Citation_ItemViewer.Add_Localization_String( "Record Information", "Record Information");
            Citation_ItemViewer.Add_Localization_String( "Related Items", "Related Items");
            Citation_ItemViewer.Add_Localization_String( "XXX Membership", "%1 Membership");
            Citation_ItemViewer.Add_Localization_String( "System Admin Information", "System Admin Information");
            Citation_ItemViewer.Add_Localization_String( "METS Information", "METS Information");

            //Initialize the Download_ItemViewer_Localization class
            Download_ItemViewer = new Download_ItemViewer_LocalizationInfo();
            Download_ItemViewer.Add_Localization_String( "This Item Has The Following Downloads", "This item has the following downloads:");
            Download_ItemViewer.Add_Localization_String( "This Item Is Only Available As The Following Downloads", "This item is only available as the following downloads:");
            Download_ItemViewer.Add_Localization_String( "To Download Right Click On The Tile Name Below Select Save Link As And Save The JPEG2000 To Your Local Computer", "\"To download, right click on the tile name below, select 'Save Link As...' and save the JPEG2000 to your local computer.\"");
            Download_ItemViewer.Add_Localization_String( "To Download Right Click On The Tile Name Below Select Save Target As And Save The JPEG2000 To Your Local Computer", "\"To download, right click on the tile name below, select 'Save Target As...' and save the JPEG2000 to your local computer. \"");
            Download_ItemViewer.Add_Localization_String( "The Following Tiles Are Available For Download", "The following tiles are available for download:");

            //Initialize the EAD_Container_List_ItemViewer_Localization class
            EAD_Container_List_ItemViewer = new EAD_Container_List_ItemViewer_LocalizationInfo();
            EAD_Container_List_ItemViewer.Add_Localization_String( "Container List", "Container List");

            //Initialize the EAD_Description_ItemViewer_Localization class
            EAD_Description_ItemViewer = new EAD_Description_ItemViewer_LocalizationInfo();
            EAD_Description_ItemViewer.Add_Localization_String( "Archival Description", "Archival Description");

            //Initialize the EmbeddedVideo_ItemViewer_Localization class
            EmbeddedVideo_ItemViewer = new EmbeddedVideo_ItemViewer_LocalizationInfo();
            EmbeddedVideo_ItemViewer.Add_Localization_String( "Streaming Video", "Streaming Video");

            //Initialize the GnuBooks_PageTurner_ItemViewer_Localization class
            GnuBooks_PageTurner_ItemViewer = new GnuBooks_PageTurner_ItemViewer_LocalizationInfo();
            GnuBooks_PageTurner_ItemViewer.Add_Localization_String( "Return To Item", "Return to Item");
            GnuBooks_PageTurner_ItemViewer.Add_Localization_String( "Zoom", "Zoom");
            GnuBooks_PageTurner_ItemViewer.Add_Localization_String( "Book Turner Presentations Require A Javascriptenabled Browser", "Book Turner presentations require a Javascript-enabled browser.");

            //Initialize the Google_Map_ItemViewer_Localization class
            Google_Map_ItemViewer = new Google_Map_ItemViewer_LocalizationInfo();
            Google_Map_ItemViewer.Add_Localization_String( "The Following Results Match Your Geographic Search And Also Appear On The Navigation Bar To The Left", "The following results match your geographic search and also appear on the navigation bar to the left:");
            Google_Map_ItemViewer.Add_Localization_String( "Sheet", "Sheet");
            Google_Map_ItemViewer.Add_Localization_String( "Tile", "Tile");
            Google_Map_ItemViewer.Add_Localization_String( "Zoom To Extent", "Zoom to extent");
            Google_Map_ItemViewer.Add_Localization_String( "Zoom To Matches", "Zoom to matches");
            Google_Map_ItemViewer.Add_Localization_String( "There Were No Matches Within This Item For Your Geographic Search", "There were no matches within this item for your geographic search.");
            Google_Map_ItemViewer.Add_Localization_String( "Click Here To Search Other Items In The Current Collection", "Click here to search other items in the current collection");
            Google_Map_ItemViewer.Add_Localization_String( "Modify Item Search", "Modify item search");
            Google_Map_ItemViewer.Add_Localization_String( "Modify Search Within Flight", "Modify search within flight");
            Google_Map_ItemViewer.Add_Localization_String( "Select The I Select Area I Button Below To Draw A Search Box On The Map Or Enter An Address And Press Find Address", "Select the <i>Select Area</i> button below to draw a search box on the map or enter an address and press Find Address.");
            Google_Map_ItemViewer.Add_Localization_String( "Search", "Search");
            Google_Map_ItemViewer.Add_Localization_String( "Search All Flights", "Search all flights");
            Google_Map_ItemViewer.Add_Localization_String( "Search Entire Collection", "Search entire collection");
            Google_Map_ItemViewer.Add_Localization_String( "Press The I Search I Button To See Results", "Press the <i>Search</i> button to see results");
            Google_Map_ItemViewer.Add_Localization_String( "Address", "Address:");
            Google_Map_ItemViewer.Add_Localization_String( "Enter Address Ie 12 Main Street Gainesville Florida", "\"Enter address ( i.e., 12 Main Street, Gainesville Florida )\"");
            Google_Map_ItemViewer.Add_Localization_String( "Find Address", "Find Address");
            Google_Map_ItemViewer.Add_Localization_String( "PRESS TO SELECT AREA", "PRESS TO SELECT AREA");
            Google_Map_ItemViewer.Add_Localization_String( "PRESS TO SELECT POINT", "PRESS TO SELECT POINT");
            Google_Map_ItemViewer.Add_Localization_String( "SELECT THE FIRST POINT", "SELECT THE FIRST POINT");
            Google_Map_ItemViewer.Add_Localization_String( "SELECT THE SECOND POINT", "SELECT THE SECOND POINT");
            Google_Map_ItemViewer.Add_Localization_String( "SELECT A POINT ON THE MAP", "SELECT A POINT ON THE MAP");

            //Initialize the HTML_Map_ItemViewer_Localization class
            HTML_Map_ItemViewer = new HTML_Map_ItemViewer_LocalizationInfo();
            HTML_Map_ItemViewer.Add_Localization_String( "Click On A Sheet In The Map To View A Sheet", "Click on a sheet in the map to view a sheet");

            //Initialize the HTML_ItemViewer_Localization class
            HTML_ItemViewer = new HTML_ItemViewer_LocalizationInfo();
            HTML_ItemViewer.Add_Localization_String( "Unable To Pull Html View For Item", "Unable to pull html view for item");
            HTML_ItemViewer.Add_Localization_String( "We Apologize For The Inconvenience", "We apologize for the inconvenience.");
            HTML_ItemViewer.Add_Localization_String( "Click Here To Report The Problem", "Click here to report the problem.");
            HTML_ItemViewer.Add_Localization_String( "Unable To Pull Html View For Item XXX", "Unable to pull html view for item %1");

            //Initialize the Aware_JP2_ItemViewer_Localization class
            Aware_JP2_ItemViewer = new Aware_JP2_ItemViewer_LocalizationInfo();
            Aware_JP2_ItemViewer.Add_Localization_String( "Zoom Out", "Zoom Out");
            Aware_JP2_ItemViewer.Add_Localization_String( "Zoom To Level", "Zoom to Level");
            Aware_JP2_ItemViewer.Add_Localization_String( "Current Zoom", "Current Zoom");
            Aware_JP2_ItemViewer.Add_Localization_String( "Zoom In", "Zoom In");
            Aware_JP2_ItemViewer.Add_Localization_String( "Rotate Clockwise", "Rotate Clockwise");
            Aware_JP2_ItemViewer.Add_Localization_String( "Rotate Counter Clockwise", "Rotate Counter Clockwise");
            Aware_JP2_ItemViewer.Add_Localization_String( "Pan Up", "Pan Up");
            Aware_JP2_ItemViewer.Add_Localization_String( "Pan Down", "Pan Down");
            Aware_JP2_ItemViewer.Add_Localization_String( "Pan Left", "Pan Left");
            Aware_JP2_ItemViewer.Add_Localization_String( "Pan Right", "Pan Right");
            Aware_JP2_ItemViewer.Add_Localization_String( "Click On Thumbnail To Recenter Image", "Click on Thumbnail to Recenter Image");
            Aware_JP2_ItemViewer.Add_Localization_String( "THUMBNAIL", "THUMBNAIL");
            Aware_JP2_ItemViewer.Add_Localization_String( "Small Size View", "Small size view");
            Aware_JP2_ItemViewer.Add_Localization_String( "Medium Size View", "Medium size view");
            Aware_JP2_ItemViewer.Add_Localization_String( "Mediumlarge Size View", "Medium-large size view");
            Aware_JP2_ItemViewer.Add_Localization_String( "Large Size View", "Large size view");
            Aware_JP2_ItemViewer.Add_Localization_String( "To Download Right Click Here Select Save Target As And Save The JPEG2000 To Your Local Computer", "\"To download, right click here, select 'Save Target As...' and save the JPEG2000 to your local computer.\"");
            Aware_JP2_ItemViewer.Add_Localization_String( "JPEG2000 IMAGE NOT FOUND IN DATABASE", "JPEG2000 IMAGE NOT FOUND IN DATABASE!");

            //Initialize the MultiVolumes_ItemViewer_Localization class
            MultiVolumes_ItemViewer = new MultiVolumes_ItemViewer_LocalizationInfo();
            MultiVolumes_ItemViewer.Add_Localization_String( "All Issues", "All Issues");
            MultiVolumes_ItemViewer.Add_Localization_String( "Related Maps", "Related Maps");
            MultiVolumes_ItemViewer.Add_Localization_String( "MISSING THUMBNAIL", "MISSING THUMBNAIL");
            MultiVolumes_ItemViewer.Add_Localization_String( "Related Map Sets", "Related Map sets");
            MultiVolumes_ItemViewer.Add_Localization_String( "Related Flights", "Related Flights");
            MultiVolumes_ItemViewer.Add_Localization_String( "All Volumes", "All Volumes");
            MultiVolumes_ItemViewer.Add_Localization_String( "Tree View", "Tree View");
            MultiVolumes_ItemViewer.Add_Localization_String( "Thumbnails", "Thumbnails");
            MultiVolumes_ItemViewer.Add_Localization_String( "List View", "List View");
            MultiVolumes_ItemViewer.Add_Localization_String( "Level 1", "Level 1");
            MultiVolumes_ItemViewer.Add_Localization_String( "Level 2", "Level 2");
            MultiVolumes_ItemViewer.Add_Localization_String( "Level 3", "Level 3");
            MultiVolumes_ItemViewer.Add_Localization_String( "Access", "Access");
            MultiVolumes_ItemViewer.Add_Localization_String( "Public", "public");
            MultiVolumes_ItemViewer.Add_Localization_String( "Private", "private");
            MultiVolumes_ItemViewer.Add_Localization_String( "Restricted", "restricted");
            MultiVolumes_ItemViewer.Add_Localization_String( "Dark", "dark");

            //Initialize the PDF_ItemViewer_Localization class
            PDF_ItemViewer = new PDF_ItemViewer_LocalizationInfo();
            PDF_ItemViewer.Add_Localization_String( "Download This PDF", "Download this PDF");

            //Initialize the QC_ItemViewer_Localization class
            QC_ItemViewer = new QC_ItemViewer_LocalizationInfo();
            QC_ItemViewer.Add_Localization_String( "Thumbnails Per Page", "% Thumbnails per page");
            QC_ItemViewer.Add_Localization_String( "All Thumbnails", "All Thumbnails");
            QC_ItemViewer.Add_Localization_String( "Go To Thumbnail", "Go to thumbnail:");
            QC_ItemViewer.Add_Localization_String( "Page", "Page");
            QC_ItemViewer.Add_Localization_String( "Division", "Division");
            QC_ItemViewer.Add_Localization_String( "Pagination", "Pagination");
            QC_ItemViewer.Add_Localization_String( "Small Thumbnails", "Small thumbnails");
            QC_ItemViewer.Add_Localization_String( "Medium Thumbnails", "Medium thumbnails");
            QC_ItemViewer.Add_Localization_String( "Large Thumbnails", "Large thumbnails");
            QC_ItemViewer.Add_Localization_String( "Overcropped", "Overcropped");
            QC_ItemViewer.Add_Localization_String( "Image Quality Error", "Image Quality Error");
            QC_ItemViewer.Add_Localization_String( "Technical Specification Error", "Technical Specification Error");
            QC_ItemViewer.Add_Localization_String( "Other Specify", "Other (specify)");
            QC_ItemViewer.Add_Localization_String( "Undercropped", "Undercropped");
            QC_ItemViewer.Add_Localization_String( "Orientation Error", "Orientation error");
            QC_ItemViewer.Add_Localization_String( "Skew Error", "Skew error");
            QC_ItemViewer.Add_Localization_String( "Blur Needed", "Blur needed");
            QC_ItemViewer.Add_Localization_String( "Unblur Needed", "Unblur needed");
            QC_ItemViewer.Add_Localization_String( "Thumbnail Size", "Thumbnail size");
            QC_ItemViewer.Add_Localization_String( "Missing Icon", "Missing icon");
            QC_ItemViewer.Add_Localization_String( "Standard Cursor", "Standard cursor");
            QC_ItemViewer.Add_Localization_String( "Choose Main Thumbnail", "Choose main thumbnail");
            QC_ItemViewer.Add_Localization_String( "Move Multiple Pages", "Move multiple pages");
            QC_ItemViewer.Add_Localization_String( "Delete Multiple Pages", "Delete multiple pages");
            QC_ItemViewer.Add_Localization_String( "Resource", "Resource");
            QC_ItemViewer.Add_Localization_String( "Edit", "Edit");
            QC_ItemViewer.Add_Localization_String( "Save", "Save");
            QC_ItemViewer.Add_Localization_String( "Complete", "Complete");
            QC_ItemViewer.Add_Localization_String( "Cancel", "Cancel");
            QC_ItemViewer.Add_Localization_String( "Clear Pagination", "Clear Pagination");
            QC_ItemViewer.Add_Localization_String( "Clear All Reorder Pages", "Clear All & Reorder Pages");
            QC_ItemViewer.Add_Localization_String( "Settings", "Settings");
            QC_ItemViewer.Add_Localization_String( "Automatic Numbering", "Automatic numbering");
            QC_ItemViewer.Add_Localization_String( "No Automatic Numbering", "No automatic numbering");
            QC_ItemViewer.Add_Localization_String( "Within Same Division", "Within same division");
            QC_ItemViewer.Add_Localization_String( "Entire Document", "Entire document");
            QC_ItemViewer.Add_Localization_String( "Drag Drop Pages", "Drag & drop pages");
            QC_ItemViewer.Add_Localization_String( "Enabled", "Enabled");
            QC_ItemViewer.Add_Localization_String( "Enabled With Confirmation", "Enabled with confirmation");
            QC_ItemViewer.Add_Localization_String( "Disabled", "Disabled");
            QC_ItemViewer.Add_Localization_String( "View", "View");
            QC_ItemViewer.Add_Localization_String( "View METS", "View METS");
            QC_ItemViewer.Add_Localization_String( "View QC History", "View QC History");
            QC_ItemViewer.Add_Localization_String( "View Directory", "View Directory");
            QC_ItemViewer.Add_Localization_String( "Help", "Help");
            QC_ItemViewer.Add_Localization_String( "Check For The Beginning Of A New Division Type", "Check for the beginning of a new division type");
            QC_ItemViewer.Add_Localization_String( "View Technical Image Information", "View technical image information");
            QC_ItemViewer.Add_Localization_String( "Delete This Page And Related Files", "Delete this page and related files");
            QC_ItemViewer.Add_Localization_String( "Open This Page In A New Window", "Open this page in a new window");
            QC_ItemViewer.Add_Localization_String( "Mark An Error On This Page Image", "Mark an error on this page image");
            QC_ItemViewer.Add_Localization_String( "Missing Thumbnail", "Missing thumbnail");
            QC_ItemViewer.Add_Localization_String( "MOVE SELECTED PAGES", "MOVE SELECTED PAGES");
            QC_ItemViewer.Add_Localization_String( "Before", "Before");
            QC_ItemViewer.Add_Localization_String( "After", "After");
            QC_ItemViewer.Add_Localization_String( "PREVIEW", "PREVIEW");
            QC_ItemViewer.Add_Localization_String( "Cancel This Move", "Cancel this move");
            QC_ItemViewer.Add_Localization_String( "SUBMIT", "SUBMIT");
            QC_ItemViewer.Add_Localization_String( "CANCEL", "CANCEL");
            QC_ItemViewer.Add_Localization_String( "FILE ERROR", "FILE ERROR");
            QC_ItemViewer.Add_Localization_String( "Recapture Required", "Recapture required");
            QC_ItemViewer.Add_Localization_String( "Processing Required", "Processing required");
            QC_ItemViewer.Add_Localization_String( "No File Error", "No File error");
            QC_ItemViewer.Add_Localization_String( "Save This Error", "Save this error");
            QC_ItemViewer.Add_Localization_String( "Critical Volume Error", "Critical Volume error");
            QC_ItemViewer.Add_Localization_String( "No Volume Error", "No Volume Error");
            QC_ItemViewer.Add_Localization_String( "Invalid Images", "Invalid Images");
            QC_ItemViewer.Add_Localization_String( "Incorrect Volume", "Incorrect Volume");
            QC_ItemViewer.Add_Localization_String( "Save The Resource And Apply Your Changes", "Save the resource and apply your changes");
            QC_ItemViewer.Add_Localization_String( "Upload New Page Image Files", "Upload new page image files");
            QC_ItemViewer.Add_Localization_String( "Comments", "Comments:");
            QC_ItemViewer.Add_Localization_String( "Go To", "Go to:");

            //Initialize the Related_Images_ItemViewer_Localization class
            Related_Images_ItemViewer = new Related_Images_ItemViewer_LocalizationInfo();
            Related_Images_ItemViewer.Add_Localization_String( "Thumbnails Per Page", "% Thumbnails per page");
            Related_Images_ItemViewer.Add_Localization_String( "All Thumbnails", "All Thumbnails");
            Related_Images_ItemViewer.Add_Localization_String( "Go To Thumbnail", "Go to thumbnail:");
            Related_Images_ItemViewer.Add_Localization_String( "Page", "Page");
            Related_Images_ItemViewer.Add_Localization_String( "Small Thumbnails", "Small thumbnails");
            Related_Images_ItemViewer.Add_Localization_String( "Medium Thumbnails", "Medium thumbnails");
            Related_Images_ItemViewer.Add_Localization_String( "Large Thumbnails", "Large thumbnails");
            Related_Images_ItemViewer.Add_Localization_String( "Switch To Small Thumbnails", "Switch to small thumbnails");
            Related_Images_ItemViewer.Add_Localization_String( "Switch To Medium Thumbnails", "Switch to medium thumbnails");
            Related_Images_ItemViewer.Add_Localization_String( "Switch To Large Thumbnails", "Switch to large thumbnails");
            Related_Images_ItemViewer.Add_Localization_String( "Small", "Small");
            Related_Images_ItemViewer.Add_Localization_String( "Medium", "Medium");
            Related_Images_ItemViewer.Add_Localization_String( "Large", "Large");
            Related_Images_ItemViewer.Add_Localization_String( "MISSING THUMBNAIL", "MISSING THUMBNAIL");

            //Initialize the YouTube_Embedded_Video_ItemViewer_Localization class
            YouTube_Embedded_Video_ItemViewer = new YouTube_Embedded_Video_ItemViewer_LocalizationInfo();
            YouTube_Embedded_Video_ItemViewer.Add_Localization_String( "Streaming Video", "Streaming Video");

            //Initialize the Text_Search_ItemViewer_Localization class
            Text_Search_ItemViewer = new Text_Search_ItemViewer_LocalizationInfo();
            Text_Search_ItemViewer.Add_Localization_String( "Search This Document", "Search this document");
            Text_Search_ItemViewer.Add_Localization_String( "Your Search Within This Document For", "Your search within this document for ");
            Text_Search_ItemViewer.Add_Localization_String( "AND", "AND");
            Text_Search_ItemViewer.Add_Localization_String( "OR", "OR ");
            Text_Search_ItemViewer.Add_Localization_String( "AND NOT", "AND NOT");
            Text_Search_ItemViewer.Add_Localization_String( "Resulted In", "resulted in");
            Text_Search_ItemViewer.Add_Localization_String( "Matching Pages", "matching pages");
            Text_Search_ItemViewer.Add_Localization_String( "No Matching Pages", "no matching pages");
            Text_Search_ItemViewer.Add_Localization_String( "You Can Expand Your Results By Searching For", "You can expand your results by searching for");
            Text_Search_ItemViewer.Add_Localization_String( "You Can Restrict Your Results By Searching For", "You can restrict your results by searching for");
            Text_Search_ItemViewer.Add_Localization_String( "First Page", "First Page");
            Text_Search_ItemViewer.Add_Localization_String( "Previous Page", "Previous Page");
            Text_Search_ItemViewer.Add_Localization_String( "Next Page", "Next Page");
            Text_Search_ItemViewer.Add_Localization_String( "Last Page", "Last Page");
            Text_Search_ItemViewer.Add_Localization_String( "First", "First");
            Text_Search_ItemViewer.Add_Localization_String( "Previous", "Previous");
            Text_Search_ItemViewer.Add_Localization_String( "Next", "Next");
            Text_Search_ItemViewer.Add_Localization_String( "Last", "Last");

            //Initialize the Tracking_ItemViewer_Localization class
            Tracking_ItemViewer = new Tracking_ItemViewer_LocalizationInfo();
            Tracking_ItemViewer.Add_Localization_String( "Tracking Information", "Tracking Information");
            Tracking_ItemViewer.Add_Localization_String( "Milestones", "Milestones");
            Tracking_ItemViewer.Add_Localization_String( "History", "History");
            Tracking_ItemViewer.Add_Localization_String( "Media", "Media");
            Tracking_ItemViewer.Add_Localization_String( "Archives", "Archives");
            Tracking_ItemViewer.Add_Localization_String( "Directory", "Directory");
            Tracking_ItemViewer.Add_Localization_String( "ITEM HAS NO HISTORY", "ITEM HAS NO HISTORY");
            Tracking_ItemViewer.Add_Localization_String( "ITEM HISTORY", "ITEM HISTORY");
            Tracking_ItemViewer.Add_Localization_String( "Workflow Name", "Workflow Name");
            Tracking_ItemViewer.Add_Localization_String( "Completed Date", "Completed Date");
            Tracking_ItemViewer.Add_Localization_String( "User", "User");
            Tracking_ItemViewer.Add_Localization_String( "Location Notes", "Location / Notes");
            Tracking_ItemViewer.Add_Localization_String( "ITEM IS NOT ARCHIVED TO MEDIA", "ITEM IS NOT ARCHIVED TO MEDIA");
            Tracking_ItemViewer.Add_Localization_String( "CDDVD ARCHIVE", "CD/DVD ARCHIVE");
            Tracking_ItemViewer.Add_Localization_String( "CD Number", "CD Number");
            Tracking_ItemViewer.Add_Localization_String( "File Range", "File Range");
            Tracking_ItemViewer.Add_Localization_String( "Images", "Images");
            Tracking_ItemViewer.Add_Localization_String( "Size", "Size");
            Tracking_ItemViewer.Add_Localization_String( "Date Burned", "Date Burned");
            Tracking_ItemViewer.Add_Localization_String( "ITEM HAS NO ARCHIVE INFORMATION", "ITEM HAS NO ARCHIVE INFORMATION");
            Tracking_ItemViewer.Add_Localization_String( "ARCHIVED FILE INFORMATION", "ARCHIVED FILE INFORMATION");
            Tracking_ItemViewer.Add_Localization_String( "Filename", "Filename");
            Tracking_ItemViewer.Add_Localization_String( "Last Write Date", "Last Write Date");
            Tracking_ItemViewer.Add_Localization_String( "Archived Date", "Archived Date");
            Tracking_ItemViewer.Add_Localization_String( "PAGE FILES", "PAGE FILES");
            Tracking_ItemViewer.Add_Localization_String( "Name", "Name");
            Tracking_ItemViewer.Add_Localization_String( "Date Modified", "Date Modified");
            Tracking_ItemViewer.Add_Localization_String( "Type", "Type");
            Tracking_ItemViewer.Add_Localization_String( "METADATA FILES", "METADATA FILES");
            Tracking_ItemViewer.Add_Localization_String( "OTHER FILES", "OTHER FILES");
            Tracking_ItemViewer.Add_Localization_String( "JPEG Image", "JPEG image");
            Tracking_ItemViewer.Add_Localization_String( "Thumbnail Image", "Thumbnail image");
            Tracking_ItemViewer.Add_Localization_String( "Archival TIFF Image", "Archival TIFF image");
            Tracking_ItemViewer.Add_Localization_String( "JPEG2000 Zoomable Image", "JPEG2000 Zoomable image");
            Tracking_ItemViewer.Add_Localization_String( "Adobe Acrobat Document", "Adobe Acrobat Document");
            Tracking_ItemViewer.Add_Localization_String( "Text File", "Text file");
            Tracking_ItemViewer.Add_Localization_String( "Microsoft Office Excel Worksheet", "Microsoft Office Excel Worksheet");
            Tracking_ItemViewer.Add_Localization_String( "Microsoft Office Word Document", "Microsoft Office Word Document");
            Tracking_ItemViewer.Add_Localization_String( "Microsoft Office Powerpoint Presentation", "Microsoft Office Powerpoint Presentation");
            Tracking_ItemViewer.Add_Localization_String( "Shockwave Flash Object", "Shockwave Flash Object");
            Tracking_ItemViewer.Add_Localization_String( "Citationonly METS File", "Citation-only METS File");
            Tracking_ItemViewer.Add_Localization_String( "MARC XML File", "MARC XML File");
            Tracking_ItemViewer.Add_Localization_String( "Sobekcm Service METS File", "SobekCM Service METS File");
            Tracking_ItemViewer.Add_Localization_String( "XML Document", "XML Document");
            Tracking_ItemViewer.Add_Localization_String( "Usersubmitted METS File", "User-submitted METS File");
            Tracking_ItemViewer.Add_Localization_String( "Previous METS File Version", "Previous METS File Version");
            Tracking_ItemViewer.Add_Localization_String( "Backup File", "Backup File");
            Tracking_ItemViewer.Add_Localization_String( "FDA Ingest Report", "FDA Ingest Report");
            Tracking_ItemViewer.Add_Localization_String( "DIGITIZATION MILESTONES", "DIGITIZATION MILESTONES");
            Tracking_ItemViewer.Add_Localization_String( "Digital Acquisition", "Digital Acquisition");
            Tracking_ItemViewer.Add_Localization_String( "Postacquisition Processing", "Post-Acquisition Processing");
            Tracking_ItemViewer.Add_Localization_String( "Quality Control Performed", "Quality Control Performed");
            Tracking_ItemViewer.Add_Localization_String( "Online Complete", "Online Complete");
            Tracking_ItemViewer.Add_Localization_String( "PHYSICAL MATERIAL MILESTONES", "PHYSICAL MATERIAL MILESTONES");
            Tracking_ItemViewer.Add_Localization_String( "Materials Received", "Materials Received");
            Tracking_ItemViewer.Add_Localization_String( "Disposition Date", "Disposition Date");
            Tracking_ItemViewer.Add_Localization_String( "Tracking Box", "Tracking Box");
            Tracking_ItemViewer.Add_Localization_String( "Born Digital", "Born Digital");
            Tracking_ItemViewer.Add_Localization_String( "Disposition Advice", "Disposition Advice");
            Tracking_ItemViewer.Add_Localization_String( "Locally Stored On CD Or Tape", "Locally Stored on CD or Tape");
            Tracking_ItemViewer.Add_Localization_String( "Archived Remotely FDA", "Archived Remotely (FDA)");
            Tracking_ItemViewer.Add_Localization_String( "NOT ARCHIVED", "NOT ARCHIVED");
            Tracking_ItemViewer.Add_Localization_String( "PHYSICAL MATERIAL RELATED FIELDS", "PHYSICAL MATERIAL RELATED FIELDS");
            Tracking_ItemViewer.Add_Localization_String( "ARCHIVING MILESTONES", "ARCHIVING MILESTONES");

            //Initialize the abstract_ResultsViewer_Localization class
            abstract_ResultsViewer = new abstract_ResultsViewer_LocalizationInfo();
            abstract_ResultsViewer.Add_Localization_String( "Issue", "issue");
            abstract_ResultsViewer.Add_Localization_String( "Issues", "issues");
            abstract_ResultsViewer.Add_Localization_String( "Flight Line", "flight line");
            abstract_ResultsViewer.Add_Localization_String( "Flight Lines", "flight lines");
            abstract_ResultsViewer.Add_Localization_String( "Map Set", "map set");
            abstract_ResultsViewer.Add_Localization_String( "Map Sets", "map sets");
            abstract_ResultsViewer.Add_Localization_String( "Photograph Set", "photograph set");
            abstract_ResultsViewer.Add_Localization_String( "Photograph Sets", "photograph sets");
            abstract_ResultsViewer.Add_Localization_String( "Video", "video");
            abstract_ResultsViewer.Add_Localization_String( "Videos", "videos");
            abstract_ResultsViewer.Add_Localization_String( "Audio Item", "audio item");
            abstract_ResultsViewer.Add_Localization_String( "Audio Items", "audio items");
            abstract_ResultsViewer.Add_Localization_String( "Audio", "audio");
            abstract_ResultsViewer.Add_Localization_String( "Audios", "audios");
            abstract_ResultsViewer.Add_Localization_String( "Artifact", "artifact");
            abstract_ResultsViewer.Add_Localization_String( "Artifacts", "artifacts");
            abstract_ResultsViewer.Add_Localization_String( "Item", "item");
            abstract_ResultsViewer.Add_Localization_String( "Items", "items");

            //Initialize the PagedResults_HtmlSubwriter_Localization class
            PagedResults_HtmlSubwriter = new PagedResults_HtmlSubwriter_LocalizationInfo();
            PagedResults_HtmlSubwriter.Add_Localization_String( "Search Has Been Saved To Your Saved Searches", "Search has been saved to your saved searches.");
            PagedResults_HtmlSubwriter.Add_Localization_String( "ERROR Encountered While Saving Your Search", "ERROR encountered while saving your search!");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Your Email Has Been Sent", "Your email has been sent");
            PagedResults_HtmlSubwriter.Add_Localization_String( "ERROR Encountered While Saving", "ERROR encountered while saving!");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Title", "Title");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Rank", "Rank");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Bibid Ascending", "BibID Ascending");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Bibid Descending", "BibID Descending");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Date Ascending", "Date Ascending");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Date Descending", "Date Descending");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Date Added", "Date Added");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Bookshelf View", "Bookshelf View");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Brief View", "Brief View");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Map View", "Map View");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Table View", "Table View");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Thumbnail View", "Thumbnail View");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Export", "Export");
            PagedResults_HtmlSubwriter.Add_Localization_String( "XXX XXX Of XXX Matching Titles", "{0} - {1} of {2} matching titles");
            PagedResults_HtmlSubwriter.Add_Localization_String( "XXX XXX Of XXX Matching Coordinates", "{0} - {1} of {2} matching coordinates");
            PagedResults_HtmlSubwriter.Add_Localization_String( "XXX XXX Of XXX Matching Flights", "{0} - {1} of {2} matching flights");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Public Bookshelf", "Public Bookshelf");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Browse", "Browse");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Search", "Search");
            PagedResults_HtmlSubwriter.Add_Localization_String( "XXX Items For Export", "{1} items for export");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Add This To Your Saved Searches", "Add this to your Saved Searches");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Enter Notes For This Public Bookshelf", "Enter notes for this public bookshelf");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Enter Notes For This Browse", "Enter notes for this browse");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Enter Notes For This Search", "Enter notes for this search");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Description", "Description:");
            PagedResults_HtmlSubwriter.Add_Localization_String( "And", "and");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Or", "or");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Not", "not");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In No Matching Records", "resulted in no matching records.");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In One Matching Record", "resulted in one matching record.");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In XXX Matching Records", "resulted in {0} matching records.");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In One Item In", "resulted in one item in ");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In XXX Items In", "resulted in {0} items in");
            PagedResults_HtmlSubwriter.Add_Localization_String( "One Title", "one title");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Titles", "titles");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In No Matching Flights", "resulted in no matching flights.");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In One Matching Flight", "resulted in one matching flight.");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In XXX Matching Flights", "resulted in {0} matching flights.");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In One Flight In", "resulted in one flight in ");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Resulted In XXX Flights In", "resulted in {0} flights in ");
            PagedResults_HtmlSubwriter.Add_Localization_String( "One County", "one county");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Counties", "counties");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Your Search Of XXX For", "Your search of {0} for ");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Your Geographic Search Of XXX", "Your geographic search of {0}");
            PagedResults_HtmlSubwriter.Add_Localization_String( "In", "in ");
            PagedResults_HtmlSubwriter.Add_Localization_String( "NARROW RESULTS BY", "NARROW RESULTS BY");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Show More", "Show More");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Show Less", "Show Less");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Sort Alphabetically", "Sort alphabetically");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Sort By Frequency", "Sort by frequency");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Sort By", "Sort by");
            PagedResults_HtmlSubwriter.Add_Localization_String( "BRIEF", "BRIEF");
            PagedResults_HtmlSubwriter.Add_Localization_String( "THUMB", "THUMB");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Enter The Email Inforamtion Below", "Enter the email inforamtion below");
            PagedResults_HtmlSubwriter.Add_Localization_String( "To", "To:");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Comments", "Comments:");
            PagedResults_HtmlSubwriter.Add_Localization_String( "HTML", "HTML");
            PagedResults_HtmlSubwriter.Add_Localization_String( "Plain Text", "Plain Text");
            PagedResults_HtmlSubwriter.Add_Localization_String( "UNRECOGNIZED SEARCH", "UNRECOGNIZED SEARCH");

            //Initialize the Bookshelf_View_ResultsViewer_Localization class
            Bookshelf_View_ResultsViewer = new Bookshelf_View_ResultsViewer_LocalizationInfo();
            Bookshelf_View_ResultsViewer.Add_Localization_String( "TITLE NOTES", "TITLE / NOTES");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Remove All Checked Items From Your Bookshelf", "Remove all checked items from your bookshelf");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Remove", "remove");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Move All Checked Items To A New Bookshelf", "Move all checked items to a new bookshelf");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Move", "move");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Select Or Unselect All Items In This Bookshelf", "Select or unselect all items in this bookshelf");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Remove This Item From Your Bookshelf", "Remove this item from your bookshelf");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Move This Item To A New Bookshelf", "Move this item to a new bookshelf");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Send This Item To A Friend", "Send this item to a friend");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Send", "send");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Edit This Item", "Edit this item");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Edit", "edit");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Select Or Unselect This Item", "Select or unselect this item");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Edit Note", "edit note");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "Add Note", "add note");
            Bookshelf_View_ResultsViewer.Add_Localization_String( "ACTIONS", "ACTIONS");

            //Initialize the Brief_ResultsViewer_Localization class
            Brief_ResultsViewer = new Brief_ResultsViewer_LocalizationInfo();
            Brief_ResultsViewer.Add_Localization_String( "Access Restricted", "Access Restricted");
            Brief_ResultsViewer.Add_Localization_String( "MISSING THUMBNAIL", "MISSING THUMBNAIL");

            //Initialize the Export_View_ResultsViewer_Localization class
            Export_View_ResultsViewer = new Export_View_ResultsViewer_LocalizationInfo();
            Export_View_ResultsViewer.Add_Localization_String( "This Option Allows You To Export The List Of Results Which Match Your Search Or Browse To An Excel Document Or CSV File For Download", "This option allows you to export the list of results which match your search or browse to an excel document or CSV file for download.");
            Export_View_ResultsViewer.Add_Localization_String( "Select The File Type Below To Create The Report", "Select the file type below to create the report:");
            Export_View_ResultsViewer.Add_Localization_String( "Excel Spreadsheet File XLS", "Excel Spreadsheet file (XLS)");
            Export_View_ResultsViewer.Add_Localization_String( "Commaseperated Value Text File CSV", "Comma-seperated value text file (CSV)");

            //Initialize the Map_ResultsWriter_Localization class
            Map_ResultsWriter = new Map_ResultsWriter_LocalizationInfo();
            Map_ResultsWriter.Add_Localization_String( "Varies", "( varies )");
            Map_ResultsWriter.Add_Localization_String( "The Following XXX Matches In XXX Sets Share The Same Coordinate Information", "The following {0} matches in {1} sets share the same coordinate information");
            Map_ResultsWriter.Add_Localization_String( "The Following XXX Matches Share The Same Coordinate Information", "The following {0} matches share the same coordinate information");
            Map_ResultsWriter.Add_Localization_String( "The Followingxxx Matches In XXX Sets Have No Coordinate Information", "The following{0} matches in {1} sets have no coordinate information");
            Map_ResultsWriter.Add_Localization_String( "The Following XXX Matches Have No Coordinate Information", "The following {0} matches have no coordinate information");
            Map_ResultsWriter.Add_Localization_String( "The Following XXX Titles Have The Same Coordinate Point", "The following {0} titles have the same coordinate point");

            //Initialize the No_Results_ResultsViewer_Localization class
            No_Results_ResultsViewer = new No_Results_ResultsViewer_LocalizationInfo();
            No_Results_ResultsViewer.Add_Localization_String( "Your Search Returned No Results", "Your search returned no results.");
            No_Results_ResultsViewer.Add_Localization_String( "XXX Result Found In XXX", "{0} result found in {1}");
            No_Results_ResultsViewer.Add_Localization_String( "The Following Matches Were Found", "The following matches were found:");
            No_Results_ResultsViewer.Add_Localization_String( "XXX Found In The University Of Florida Library Catalog", "%1 found in the University of Florida Library Catalog");
            No_Results_ResultsViewer.Add_Localization_String( "One Found In The University Of Florida Library Catalog", "One found in the University of Florida Library Catalog");
            No_Results_ResultsViewer.Add_Localization_String( "Consider Searching One Of The Following", "Consider searching one of the following:");
            No_Results_ResultsViewer.Add_Localization_String( "Online Resource", "Online Resource");
            No_Results_ResultsViewer.Add_Localization_String( "Physical Holdings", "Physical Holdings");

            //Initialize the Table_ResultsViewer_Localization class
            Table_ResultsViewer = new Table_ResultsViewer_LocalizationInfo();
            Table_ResultsViewer.Add_Localization_String( "No", "No. ");
            Table_ResultsViewer.Add_Localization_String( "Title", "Title");
            Table_ResultsViewer.Add_Localization_String( "Date", "Date");

            //Initialize the Thumbnail_ResultsViewer_Localization class
            Thumbnail_ResultsViewer = new Thumbnail_ResultsViewer_LocalizationInfo();
            Thumbnail_ResultsViewer.Add_Localization_String( "Issue", "issue");
            Thumbnail_ResultsViewer.Add_Localization_String( "Issues", "issues");
            Thumbnail_ResultsViewer.Add_Localization_String( "Volume", "volume");
            Thumbnail_ResultsViewer.Add_Localization_String( "Volumes", "volumes");
            Thumbnail_ResultsViewer.Add_Localization_String( "Access Restricted", "Access Restricted");

            //Initialize the MySobek_HtmlSubwriter_Localization class
            MySobek_HtmlSubwriter = new MySobek_HtmlSubwriter_LocalizationInfo();
            MySobek_HtmlSubwriter.Add_Localization_String( "Welcome Back XXX", "\"Welcome back, {0}\"");
            MySobek_HtmlSubwriter.Add_Localization_String( "XXX Home", "%1 Home");

            //Initialize the Saved_Searches_MySobekViewer_Localization class
            Saved_Searches_MySobekViewer = new Saved_Searches_MySobekViewer_LocalizationInfo();
            Saved_Searches_MySobekViewer.Add_Localization_String( "My Saved Searches", "My Saved Searches");
            Saved_Searches_MySobekViewer.Add_Localization_String( "You Do Not Have Any Saved Searches Or Browses Br Br To Add A Search Or Browse Use The ADD Button While Viewing The Results Of Your Search Or Browse", "\"You do not have any saved searches or browses.<br /><br />To add a search or browse, use the ADD button while viewing the results of your search or browse.\"");
            Saved_Searches_MySobekViewer.Add_Localization_String( "SAVED SEARCH", "SAVED SEARCH");
            Saved_Searches_MySobekViewer.Add_Localization_String( "Click To Delete This Saved Search", "Click to delete this saved search");
            Saved_Searches_MySobekViewer.Add_Localization_String( "Click To View This Search", "Click to view this search");
            Saved_Searches_MySobekViewer.Add_Localization_String( "ACTIONS", "ACTIONS");
            Saved_Searches_MySobekViewer.Add_Localization_String( "Delete", "delete");

            //Initialize the Preferences_MySobekViewer_Localization class
            Preferences_MySobekViewer = new Preferences_MySobekViewer_LocalizationInfo();
            Preferences_MySobekViewer.Add_Localization_String( "Register For XXX", "Register for {0}");
            Preferences_MySobekViewer.Add_Localization_String( "Edit Your Account Preferences", "Edit Your Account Preferences");
            Preferences_MySobekViewer.Add_Localization_String( "Account Information", "Account Information");
            Preferences_MySobekViewer.Add_Localization_String( "Username", "UserName");
            Preferences_MySobekViewer.Add_Localization_String( "Personal Information", "Personal Information");
            Preferences_MySobekViewer.Add_Localization_String( "Family Names", "Family Name(s)");
            Preferences_MySobekViewer.Add_Localization_String( "Given Names", "Given Name(s)");
            Preferences_MySobekViewer.Add_Localization_String( "Lastfamily Names", "Last/Family Name(s)");
            Preferences_MySobekViewer.Add_Localization_String( "Firstgiven Names", "First/Given Name(s)");
            Preferences_MySobekViewer.Add_Localization_String( "Default Metadata", "Default Metadata");
            Preferences_MySobekViewer.Add_Localization_String( "Nickname", "Nickname");
            Preferences_MySobekViewer.Add_Localization_String( "Email", "Email");
            Preferences_MySobekViewer.Add_Localization_String( "Send Me Monthly Usage Statistics For My Items", "Send me monthly usage statistics for my items");
            Preferences_MySobekViewer.Add_Localization_String( "Current Affiliation Information", "Current Affiliation Information");
            Preferences_MySobekViewer.Add_Localization_String( "Organizationuniversity", "Organization/University");
            Preferences_MySobekViewer.Add_Localization_String( "College", "College");
            Preferences_MySobekViewer.Add_Localization_String( "Department", "Department");
            Preferences_MySobekViewer.Add_Localization_String( "Unit", "Unit");
            Preferences_MySobekViewer.Add_Localization_String( "Selfsubmittal Preferences", "Self-Submittal Preferences");
            Preferences_MySobekViewer.Add_Localization_String( "Send Me An Email When I Submit New Items", "Send me an email when I submit new items");
            Preferences_MySobekViewer.Add_Localization_String( "Template", "Template");
            Preferences_MySobekViewer.Add_Localization_String( "Project", "Project");
            Preferences_MySobekViewer.Add_Localization_String( "Default Rights", "Default Rights");
            Preferences_MySobekViewer.Add_Localization_String( "These Are The Default Rights You Give For Sharing Repurposing Or Remixing Your Item To Other Users You Can Set This With Each New Item You Submit But This Will Be The Default That Appears", "\"(These are the default rights you give for sharing, repurposing, or remixing your item to other users. You can set this with each new item you submit, but this will be the default that appears.)\"");
            Preferences_MySobekViewer.Add_Localization_String( "You May Also Select A A Title Explanation Of Different Creative Commons Licenses Href Httpcreativecommonsorgaboutlicenses Creative Commons License A Option Below", "\"You may also select a <a title=\"\"Explanation of different creative commons licenses.\"\" href=\"\"http://creativecommons.org/about/licenses/\"\">Creative Commons License</a> option below.\"");
            Preferences_MySobekViewer.Add_Localization_String( "Other Preferences", "Other Preferences");
            Preferences_MySobekViewer.Add_Localization_String( "Language", "Language");
            Preferences_MySobekViewer.Add_Localization_String( "Password", "Password");
            Preferences_MySobekViewer.Add_Localization_String( "Confirm Password", "Confirm Password");
            Preferences_MySobekViewer.Add_Localization_String( "Username Must Be At Least Eight Digits", "Username must be at least eight digits");
            Preferences_MySobekViewer.Add_Localization_String( "Select And Confirm A Password", "Select and confirm a password");
            Preferences_MySobekViewer.Add_Localization_String( "Passwords Do Not Match", "Passwords do not match");
            Preferences_MySobekViewer.Add_Localization_String( "Password Must Be At Least Eight Digits", "Password must be at least eight digits");
            Preferences_MySobekViewer.Add_Localization_String( "Ufids Are Always Eight Digits", "UFIDs are always eight digits");
            Preferences_MySobekViewer.Add_Localization_String( "Ufids Are Always Numeric", "UFIDs are always numeric");
            Preferences_MySobekViewer.Add_Localization_String( "A Valid Email Is Required", "A valid email is required");
            Preferences_MySobekViewer.Add_Localization_String( "Rights Statement Truncated To 1000 Characters", "Rights statement truncated to 1000 characters.");
            Preferences_MySobekViewer.Add_Localization_String( "An Account For That Email Address Already Exists", "An account for that email address already exists.");
            Preferences_MySobekViewer.Add_Localization_String( "That Username Is Taken Please Choose Another", "That username is taken.  Please choose another.");
            Preferences_MySobekViewer.Add_Localization_String( "Registration For XXX Is Free And Open To The Public Enter Your Information Below To Be Instantly Registered", "Registration for {0} is free and open to the public.  Enter your information below to be instantly registered.");
            Preferences_MySobekViewer.Add_Localization_String( "Account Information Name And Email Are Required For Each New Account", "\"Account information, name, and email are required for each new account.\"");
            Preferences_MySobekViewer.Add_Localization_String( "Already Registered Xxxlog Onxxx", "Already registered?  {0}Log on{1}.");
            Preferences_MySobekViewer.Add_Localization_String( "The Following Errors Were Detected", "The following errors were detected:");
            Preferences_MySobekViewer.Add_Localization_String( "XXX Optionally Provides Access Through Gatorlink", "%1 (optionally provides access through Gatorlink)");
            Preferences_MySobekViewer.Add_Localization_String( "I Would Like To Be Able To Submit Materials Online Once Your Application To Submit Has Been Approved You Will Receive Email Notification", "\"I would like to be able to submit materials online. (Once your application to submit has been approved, you will receive email notification)\"");
            Preferences_MySobekViewer.Add_Localization_String( "CANCEL", "CANCEL");

            //Initialize the NewPassword_MySobekViewer_Localization class
            NewPassword_MySobekViewer = new NewPassword_MySobekViewer_LocalizationInfo();
            NewPassword_MySobekViewer.Add_Localization_String( "Change Your Password", "Change your password");
            NewPassword_MySobekViewer.Add_Localization_String( "Select And Confirm A New Password", "Select and confirm a new password");
            NewPassword_MySobekViewer.Add_Localization_String( "New Passwords Do Not Match", "New passwords do not match");
            NewPassword_MySobekViewer.Add_Localization_String( "Password Must Be At Least Eight Digits", "Password must be at least eight digits");
            NewPassword_MySobekViewer.Add_Localization_String( "The New Password Cannot Match The Old Password", "The new password cannot match the old password");
            NewPassword_MySobekViewer.Add_Localization_String( "Unable To Change Password Verify Current Password", "Unable to change password.  Verify current password.");
            NewPassword_MySobekViewer.Add_Localization_String( "You Are Required To Change Your Password To Continue", "You are required to change your password to continue.");
            NewPassword_MySobekViewer.Add_Localization_String( "Please Enter Your Existing Password And Your New Password", "Please enter your existing password and your new password.");
            NewPassword_MySobekViewer.Add_Localization_String( "Existing Password", "Existing Password:");
            NewPassword_MySobekViewer.Add_Localization_String( "New Password", "New Password:");
            NewPassword_MySobekViewer.Add_Localization_String( "Confirm New Password", "Confirm New Password:");
            NewPassword_MySobekViewer.Add_Localization_String( "The Following Errors Were Detected", "The following errors were detected:");

            //Initialize the Logon_MySobekViewer_Localization class
            Logon_MySobekViewer = new Logon_MySobekViewer_LocalizationInfo();
            Logon_MySobekViewer.Add_Localization_String( "Logon To XXX", "Logon to {0}");
            Logon_MySobekViewer.Add_Localization_String( "The Feature You Are Trying To Access Requires A Valid Logon", "The feature you are trying to access requires a valid logon.");
            Logon_MySobekViewer.Add_Localization_String( "Please Choose The Appropriate Logon Directly Below", "Please choose the appropriate logon directly below.");
            Logon_MySobekViewer.Add_Localization_String( "If You Have A Valid XXX Logon Xxxsign On With XXX Authentication Herexxx", "\"<b>If you have a valid {0} logon</b>, {1}Sign on with {0} authentication here{2}.\"");
            Logon_MySobekViewer.Add_Localization_String( "Not Registered Yet Xxxregister Nowxxx Or Xxxcontact Usxxx", "<b>Not registered yet?</b> {0}Register now{1} or {2}Contact Us{1}.");
            Logon_MySobekViewer.Add_Localization_String( "LOG IN", "LOG IN");
            Logon_MySobekViewer.Add_Localization_String( "Username Or Email", "Username or email:");
            Logon_MySobekViewer.Add_Localization_String( "Password", "Password:");
            Logon_MySobekViewer.Add_Localization_String( "Remember Me", "Remember me");
            Logon_MySobekViewer.Add_Localization_String( "Not Registered Yet Xxxregister Nowxxx", "Not registered yet?  {0}Register now{1}");
            Logon_MySobekViewer.Add_Localization_String( "Forgot Your Username Or Password Please Xxxcontact Usxxx", "Forgot your username or password?  Please {0}contact us{1}.");

            //Initialize the Home_MySobekViewer_Localization class
            Home_MySobekViewer = new Home_MySobekViewer_LocalizationInfo();
            Home_MySobekViewer.Add_Localization_String( "Welcome To XXX XXX", "\"Welcome to {0}, {1}\"");
            Home_MySobekViewer.Add_Localization_String( "Welcome Back XXX", "\"Welcome back, {0}\"");
            Home_MySobekViewer.Add_Localization_String( "Welcome To XXX This Feature Allows You To Add Items To Your Bookshelves Organize Your Bookshelves And Email Your Bookshelves To Friends", "\"Welcome to {0}.  This feature allows you to add items to your bookshelves, organize your bookshelves, and email your bookshelves to friends.\"");
            Home_MySobekViewer.Add_Localization_String( "What Would You Like To Do Today", "What would you like to do today?");
            Home_MySobekViewer.Add_Localization_String( "Start A New Item", "Start a new item");
            Home_MySobekViewer.Add_Localization_String( "Online Submittals Are Temporarily Disabled", "Online submittals are temporarily disabled");
            Home_MySobekViewer.Add_Localization_String( "View All My Submitted Items", "View all my submitted items");
            Home_MySobekViewer.Add_Localization_String( "View Usage For My Items", "View usage for my items");
            Home_MySobekViewer.Add_Localization_String( "View My Descriptive Tags", "View my descriptive tags");
            Home_MySobekViewer.Add_Localization_String( "View And Organize My Bookshelves", "View and organize my bookshelves");
            Home_MySobekViewer.Add_Localization_String( "View My Saved Searches", "View my saved searches");
            Home_MySobekViewer.Add_Localization_String( "Edit My Account Preferences", "Edit my account preferences");
            Home_MySobekViewer.Add_Localization_String( "Track Item Scanningprocessing", "Track Item Scanning/Processing");
            Home_MySobekViewer.Add_Localization_String( "Return To XXX", "Return to {0}");
            Home_MySobekViewer.Add_Localization_String( "Return To Previous XXX Page", "Return to previous %1 page");
            Home_MySobekViewer.Add_Localization_String( "Log Out", "Log Out");
            Home_MySobekViewer.Add_Localization_String( "Comments Or Recommendations Please Xxxcontact Usxxx", "Comments or recommendations?  Please {0}contact us{1}.");
            Home_MySobekViewer.Add_Localization_String( "If You Would Like To Contribute Materials Through The Online System Please Xxxcontact Usxxx As Well", "\"If you would like to contribute materials through the online system, please {0}contact us{1} as well.\"");

            //Initialize the Folder_Mgmt_MySobekViewer_Localization class
            Folder_Mgmt_MySobekViewer = new Folder_Mgmt_MySobekViewer_LocalizationInfo();
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "My Library", "My Library");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Move Item Between Bookshelves", "Move Item Between Bookshelves");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Select New Bookshelf For This Item", "Select new bookshelf for this item");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Bookshelf", "Bookshelf:");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Addedit Notes For Bookshelf Item", "Add/Edit Notes for Bookshelf Item");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Enter Notes For This Item In Your Bookshelf", "Enter notes for this item in your bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Notes", "Notes:");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "New Bookshelf", "New Bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Enter The Information For Your New Bookshelf", "Enter the information for your new bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Name", "Name:");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Parent", "Parent:");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Manage My Library", "Manage my library");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "View My Collections Home Page", "View my collections home page");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "My Collections Home", "My Collections Home");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "My Saved Searches", "My Saved Searches");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "View My Saved Searches", "View my saved searches");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "This Bookshelf Is Currently Empty", "This bookshelf is currently empty");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Manage My Bookshelves", "Manage My Bookshelves");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Click To Add A New Bookshelf", "Click to add a new bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Add New Bookshelf", "Add New Bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Refresh Bookshelves List", "Refresh Bookshelves List");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "BOOKSHELF NAME", "BOOKSHELF NAME");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Click To Delete This Bookshelf", "Click to delete this bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Make This Bookshelf Private", "Make this bookshelf private");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Make This Bookshelf Public", "Make this bookshelf public");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Click To Manage This Bookshelf", "Click to manage this bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "You Cannot Delete Bookshelves Which Contain Other Bookshelves", "You cannot delete bookshelves which contain other bookshelves");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "You Cannot Delete Your Last Bookshelf", "You cannot delete your last bookshelf");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Make Private", "make private");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Make Public", "make public");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Manage", "manage");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Public Folder", "Public folder");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Enter The Email Information Below", "Enter the email information below");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "To", "To:");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Comments", "Comments");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "Format", "Format:");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "CLOSE", "CLOSE");
            Folder_Mgmt_MySobekViewer.Add_Localization_String( "ACTIONS", "ACTIONS");

            //Initialize the Aggregation_Single_AdminViewer_Localization class
            Aggregation_Single_AdminViewer = new Aggregation_Single_AdminViewer_LocalizationInfo();
            Aggregation_Single_AdminViewer.Add_Localization_String( "Error Saving Aggregation Information", "Error saving aggregation information!");
            Aggregation_Single_AdminViewer.Add_Localization_String( "NO DEFAULT", "( NO DEFAULT)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "None Top Level", "(none - top level)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "ACTION", "ACTION");
            Aggregation_Single_AdminViewer.Add_Localization_String( "ACTIONS", "ACTIONS");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Active", "Active?");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Add", "Add");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Add New Banner", "Add new banner");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Add New Home Page", "Add new home page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Additional Dublin Core Metadata To Include In OAIPMH Set List", "Additional dublin core metadata to include in OAI-PMH set list:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Advanced Search", "Advanced Search");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Advanced Search With Year Range", "Advanced Search (with Year Range)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Africa", "Africa");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Aggregationlevel Custom Stylesheet CSS", "Aggregation-level Custom Stylesheet (CSS)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Appearance", "Appearance");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Appearance Options", "Appearance Options");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Asia", "Asia");
            Aggregation_Single_AdminViewer.Add_Localization_String( "BACK", "BACK");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Banner Type", "Banner Type:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Banners", "Banners");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Basic Information", "Basic Information");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Basic Search With Year Range", "Basic Search (with Year Range)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Behavior", "Behavior:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Brief View", "Brief View");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Browse By", "Browse By");
            Aggregation_Single_AdminViewer.Add_Localization_String( "CANCEL", "CANCEL");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Caribbean", "Caribbean");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Child Pages", "Child Pages");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Child Pages Are Pages Related To The Aggregation And Allow Additional Information To Be Presented Within The Same Aggregational Branding These Can Appear In The Aggregation Main Menu With Any Metadata Browses Pulled From The Database Or You Can Set Them To For No Automatic Visibility In Which Case They Are Only Accessible By Links In The Home Page Or Other Child Pages", "\"Child pages are pages related to the aggregation and allow additional information to be presented within the same aggregational branding.  These can appear in the aggregation main menu, with any metadata browses pulled from the database, or you can set them to for no automatic visibility, in which case they are only accessible by links in the home page or other child pages.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Click Here To View The Help Page", "click here to view the help page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Click To Delete This Subcollection", "Click to delete this subcollection");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Close This Child Page Details And Return To Main Admin Pages", "Close this child page details and return to main admin pages");
            Aggregation_Single_AdminViewer.Add_Localization_String( "CODE", "CODE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Collection", "Collection");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Collection Button", "Collection Button:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Collection Visibility", "Collection Visibility");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Contact Email", "Contact Email:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Copy From Existing", "Copy from existing");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Copy From Existing Home", "Copy from existing home:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Custom Home Page", "Custom Home Page:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Custom Stylesheet", "Custom Stylesheet:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Default", "default");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Default Browse", "Default Browse:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Default Result View", "Default Result View:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Delete", "delete");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Delete This XXX Version", "Delete this %1 version");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Delete This Banner", "Delete this banner");
            Aggregation_Single_AdminViewer.Add_Localization_String( "DELETE THIS HIGHLIGHT", "DELETE THIS HIGHLIGHT");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Deleted XXX Subcollection", "Deleted %1 subcollection");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Description", "Description:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Disable This Aggregationlevel Stylesheet", "Disable this aggregation-level stylesheet");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Dloc Full Text Search", "dLOC Full Text Search");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Do Not Apply Changes", "Do not apply changes");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit", "Edit");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit Child Page Detail", "Edit Child Page Detail");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit This Aggregationlevel Stylesheet", "Edit this aggregation-level stylesheet");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit This Child Page", "Edit this child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit This Child Page In XXX", "Edit this child page in %1");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit This Home Page", "Edit this home page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit This Home Page In XXX", "Edit this home page in %1");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Edit This Subcollection", "Edit this subcollection");
            Aggregation_Single_AdminViewer.Add_Localization_String( "ENABLE", "ENABLE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Enable An Aggregationlevel Stylesheet", "Enable an aggregation-level stylesheet");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Enabled Flag", "Enabled Flag:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Error Adding New Version To This Child Page", "Error adding new version to this child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "ERROR Saving The New Item Aggregation To The Database", "ERROR saving the new item aggregation to the database");
            Aggregation_Single_AdminViewer.Add_Localization_String( "ERROR Invalid Entry For New Item Aggregation", "ERROR: Invalid entry for new item aggregation");
            Aggregation_Single_AdminViewer.Add_Localization_String( "ERROR Invalid Entry For New Item Child Page", "ERROR: Invalid entry for new item child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Europe", "Europe");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Exhibit", "Exhibit");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Existing Banners", "Existing Banners:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Existing Child Pages", "Existing Child Pages:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Existing Home Pages", "Existing Home Pages:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Existing Subcollections", "Existing Subcollections:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Existing Versions", "Existing Versions:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "External Link", "External Link:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Florida", "Florida");
            Aggregation_Single_AdminViewer.Add_Localization_String( "For More Information About The Settings On This Tab", "\"For more information about the settings on this tab,\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Full Text Search", "Full Text Search");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Full View", "Full View");
            Aggregation_Single_AdminViewer.Add_Localization_String( "General", "General");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Highlights", "Highlights");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Highlights Type", "Highlights Type:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Home Page", "Home Page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Home Page Left", "Home Page - Left");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Home Page Right", "Home Page - Right");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Home Page Text", "Home Page Text");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Image", "Image");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Include All New Item Browses", "Include All / New Item Browses");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Include In OAIPMH As A Set", "Include in OAI-PMH as a set?");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Include Map Browse", "Include Map Browse");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Institutional Division", "Institutional Division");
            Aggregation_Single_AdminViewer.Add_Localization_String( "LANGUAGE", "LANGUAGE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "LANGUAGES", "LANGUAGE(S)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Language", "Language:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Link", "Link");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Main Menu", "Main Menu");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Map Search Beta", "Map Search (Beta)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Map Search Legacy", "Map Search (Legacy)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Map Search Default Area", "Map Search Default Area:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Metadata", "Metadata");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Metadata Browses", "Metadata Browses");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Middle East", "Middle East");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Name Full", "Name (full)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Name Short", "Name (short):");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Aggregation Code Must Be Twenty Characters Long Or Less", "New aggregation code must be twenty characters long or less");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Banner", "New Banner:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Child Page Code Must Be Twenty Characters Long Or Less", "New child page code must be twenty characters long or less");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Child Page", "New Child Page:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Code Must Be Unique XXX Already Exists", "New code must be unique... %1 already exists");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Collection Home Page Text Goes Here", "New collection home page text goes here.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Home Page Text In XXX Goes Here", "New home page text in %1 goes here");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Home Page", "New Home Page:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Item Aggregation XXX Saved Successfully", "New item aggregation %1 saved successfully");
            Aggregation_Single_AdminViewer.Add_Localization_String( "New Subcollection", "New Subcollection:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "No Custom Aggregationlevel Stylesheet", "No custom aggregation-level stylesheet");
            Aggregation_Single_AdminViewer.Add_Localization_String( "No Html Source Files", "No html source files");
            Aggregation_Single_AdminViewer.Add_Localization_String( "NONE", "NONE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "North America", "North America");
            Aggregation_Single_AdminViewer.Add_Localization_String( "NOTE You May Need To Refresh Your Browser For Your Changes To Take Affect", "NOTE: You may need to refresh your browser for your changes to take affect.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "OAIPMH Settings", "OAI-PMH Settings");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Other Display Types", "Other Display Types:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "PARENT", "PARENT");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Parent Codes", "Parent Code(s)");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Parent", "Parent:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Result Views", "Result Views:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Results", "Results");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Results Options", "Results Options");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Rotating", "Rotating");
            Aggregation_Single_AdminViewer.Add_Localization_String( "SAVE", "SAVE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Save Changes To This Item Aggregation", "Save changes to this item Aggregation");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Save New Child Page", "Save new child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Save New Version Of This Child Page", "Save new version of this child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Search", "Search");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Search Options", "Search Options");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Search Types", "Search Types:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Selected Image File", "Selected image file");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Show In Parent Collection Home Page", "Show in parent collection home page?");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Some Of The Files May Be In Use", "Some of the files may be in use");
            Aggregation_Single_AdminViewer.Add_Localization_String( "SOURCE FILE", "SOURCE FILE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "South America", "South America");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Standard", "Standard");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Static", "Static");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Subcollections", "SubCollections");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Table View", "Table View");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Text", "Text:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "That Code Is A Systemreserved Keyword Try A Different Code", "That code is a system-reserved keyword.  Try a different code.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "The Code For This Browse Is XXX", "The code for this browse is: %1");
            Aggregation_Single_AdminViewer.Add_Localization_String( "The Information In This Section Controls How Search Results Or Item Browses Appears The Facet Options Control Which Metadata Values Appear To The Left Of The Results To Allow Users To Narrow Their Results The Search Results Values Determine Which Options Are Available For Viewing The Results And What Are The Aggregation Defaults Finally The Result Fields Determines Which Values Are Displayed With Each Individual Result In The Result Set", "\"The information in this section controls how search results or item browses appears.  The facet options control which metadata values appear to the left of the results, to allow users to narrow their results.  The search results values determine which options are available for viewing the results and what are the aggregation defaults.  Finally, the result fields determines which values are displayed with each individual result in the result set.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "The Information In This Section Is The Basic Information About The Aggregation Such As The Full Name The Shortened Name Used For Breadcrumbs The Description And The Email Contact", "\"The information in this section is the basic information about the aggregation, such as the full name, the shortened name used for breadcrumbs, the description, and the email contact. \"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "The Metadata Browses Can Be Used To Expose All The Metadata Of The Resources Within This Aggregation For Public Browsing Select The Metadata Fields You Would Like Have Available Below", "The metadata browses can be used to expose all the metadata of the resources within this aggregation for public browsing.  Select the metadata fields you would like have available below.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "The Values In This Section Determine If The Collection Is Currently Visible At All Whether It Is Eligible To Appear On The Collection List At The Bottom Of The Parent Page And The Collection Button Used In That Case Thematic Headings Are Used To Place This Collection On The Main Home Page", "\"The values in this section determine if the collection is currently visible at all, whether it is eligible to appear on the collection list at the bottom of the parent page, and the collection button used in that case.  Thematic headings are used to place this collection on the main home page.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Thematic Heading", "Thematic Heading:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "These Options Control How Searching Works Within This Aggregation Such As Which Search Options Are Made Publicly Available", "\"These options control how searching works within this aggregation, such as which search options are made publicly available.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "These Three Settings Have The Most Profound Affects On The Appearance Of This Aggregation By Forcing It To Appear Under A Particular Web Skin Allowing A Custom Aggregationlevel Stylesheet Or Completely Overriding The Systemgenerated Home Page For A Custom Home Page HTML Source File", "\"These three settings have the most profound affects on the appearance of this aggregation, by forcing it to appear under a particular web skin, allowing a custom aggregation-level stylesheet, or completely overriding the system-generated home page for a custom home page HTML source file.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "This Aggregation Currently Has No Child Pages", "This aggregation currently has no child pages");
            Aggregation_Single_AdminViewer.Add_Localization_String( "This Aggregation Currently Has No Subcollections", "This aggregation currently has no subcollections");
            Aggregation_Single_AdminViewer.Add_Localization_String( "THIS BANNER IMAGE IS MISSING", "THIS BANNER IMAGE IS MISSING");
            Aggregation_Single_AdminViewer.Add_Localization_String( "This Page Allows You To Edit The Basic Information About A Single Child Page And Add The Ability To Display This Child Page In Alternate Languages", "This page allows you to edit the basic information about a single child page and add the ability to display this child page in alternate languages.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "This Section Controls All The Languagespecific And Default Text Which Appears On The Home Page", "This section controls all the language-specific (and default) text which appears on the home page.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "This Section Shows All The Existing Languagespecific Banners For This Aggregation And Allows You Upload New Banners For This Aggregation", "This section shows all the existing language-specific banners for this aggregation and allows you upload new banners for this aggregation.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Thumbnail View", "Thumbnail View");
            Aggregation_Single_AdminViewer.Add_Localization_String( "TITLE", "TITLE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Title Default", "Title (default):");
            Aggregation_Single_AdminViewer.Add_Localization_String( "To Change Browse To A 50X50 Pixel GIF File And Then Select UPLOAD", "\"To change, browse to a 50x50 pixel GIF file, and then select UPLOAD\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "To Edit This Log On As The Aggregation Admin And Hover Over This Text To Edit It", "\"To edit this, log on as the aggregation admin and hover over this text to edit it.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "To Upload Browse To A GIF PNG JPEG Or BMP File And Then Select UPLOAD", "\"To upload, browse to a GIF, PNG, JPEG, or BMP file, and then select UPLOAD\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Tooltip", "Tooltip:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "TYPE", "TYPE");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Unable To Remove Subcollection Directory", "Unable to remove subcollection directory");
            Aggregation_Single_AdminViewer.Add_Localization_String( "United States", "United States");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Upload New Banner Image", "Upload New Banner Image:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View", "View");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View Banner Image File", "View banner image file");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View CSS File", "View CSS file");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View Source File", "View source file");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View This Child Page", "View this child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View This Child Page In XXX", "View this child page in %1");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View This Home Page", "View this home page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View This Home Page In XXX", "View this home page in %1");
            Aggregation_Single_AdminViewer.Add_Localization_String( "View This Subcollection", "View this subcollection");
            Aggregation_Single_AdminViewer.Add_Localization_String( "VISIBILITY", "VISIBILITY");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Visibility", "Visibility:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "Web Skin", "Web Skin:");
            Aggregation_Single_AdminViewer.Add_Localization_String( "World", "World");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Can Edit The Contents Of The Aggregationlevel Custom Stylesheet Css File Here Click SAVE When Complete To Return To The Main Aggregation Administration Screen", "You can edit the contents of the aggregation-level custom stylesheet (css) file here.  Click SAVE when complete to return to the main aggregation administration screen.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Can Use OAIPMH To Expose All The Metadata Of The Resources Within This Aggregation For Automatic Harvesting By Other Repositories Additionally You Can Choose To Attach Metadata To The Collectionlevel OAIPMH Record This Should Be Coded As Dublin Core Tags", "\"You can use OAI-PMH to expose all the metadata of the resources within this aggregation for automatic harvesting by other repositories.  Additionally, you can choose to attach metadata to the collection-level OAI-PMH record.  This should be coded as dublin core tags.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Can View Existing Subcollections Or Add New Subcollections To This Aggregation From This Tab You Will Have Full Curatorial Rights Over Any New Subcollections You Add Currently Only System Administrators Can DELETE Subcollections", "\"You can view existing subcollections or add new subcollections to this aggregation from this tab.  You will have full curatorial rights over any new subcollections you add.  Currently, only system administrators can DELETE subcollections.\"");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must Enter A CODE For This Child Page", "You must enter a CODE for this child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must Enter A CODE For This Item Aggregation", "You must enter a CODE for this item aggregation");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must Enter A DESCRIPTION For This New Aggregation", "You must enter a DESCRIPTION for this new aggregation");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must Enter A LABEL For This Child Page", "You must enter a LABEL for this child page");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must Enter A NAME For This New Aggregation", "You must enter a NAME for this new aggregation");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must SAVE Your Changes Before You Can View Or Edit Newly Added Child Page Versions", "You must SAVE your changes before you can view or edit newly added child page versions");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must SAVE Your Changes Before You Can View Or Edit Newly Added Home Pages", "You must SAVE your changes before you can view or edit newly added home pages.");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must Select A TYPE For This New Aggregation", "You must select a TYPE for this new aggregation");
            Aggregation_Single_AdminViewer.Add_Localization_String( "You Must Select A VISIBILITY For This Child Page", "You must select a VISIBILITY for this child page");

            //Initialize the Google_Map_ResultsViewer_Localization class
            Google_Map_ResultsViewer = new Google_Map_ResultsViewer_LocalizationInfo();
            Google_Map_ResultsViewer.Add_Localization_String( "The Following XXX Matches In XXX Sets Share The Same Coordinate Information", "The following %1 matches in  %2 sets share the same coordinate information");
            Google_Map_ResultsViewer.Add_Localization_String( "The Following XXX Matches Share The Same Coordinate Information", "The following %1  matches share the same coordinate information");
            Google_Map_ResultsViewer.Add_Localization_String( "The Following XXX Matches In XXX Sets Have No Coordinate Information", "The following  %1 matches  in %2  sets have no coordinate information");
            Google_Map_ResultsViewer.Add_Localization_String( "The Following XXX Matches Have No Coordinate Information", "The following %1  matches have no coordinate information");
            Google_Map_ResultsViewer.Add_Localization_String( "The Following XXX Titles Have The Same Coordinate Point", "The following %1 titles have the same coordinate point");

            //Initialize the User_Usage_Stats_MySobekViewer_Localization class
            User_Usage_Stats_MySobekViewer = new User_Usage_Stats_MySobekViewer_LocalizationInfo();
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "Below Is A List Of Items Associated With Your Account Including Usage Statistics Total Views And Visits Represents The Total Amount Of Usage Since The Item Was Added To The Library And The Monthly Views And Visits Is The Usage In The Selected Month", "Below is a list of items associated with your account including usage statistics.  Total views and visits represents the total amount of usage since the item was added to the library and the monthly views and visits is the usage in the selected month.");
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "For More Information About These Terms See The", "\"For more information about these terms, see the\"");
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "Definitions On The Main Statistics Page", "definitions on the main statistics page");
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "Select Any Column To Resort This Data", "Select any column to re-sort this data");
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "TITLE", "TITLE");
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "TOTAL VIEWS", "TOTAL VIEWS");
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "TOTAL VISITS", "TOTAL VISITS");
            User_Usage_Stats_MySobekViewer.Add_Localization_String( "MONTHLY VIEWS", "MONTHLY VIEWS");

            //Initialize the User_Tags_MySobekViewer_Localization class
            User_Tags_MySobekViewer = new User_Tags_MySobekViewer_LocalizationInfo();
            User_Tags_MySobekViewer.Add_Localization_String( "As A Digital Collection Manager Or Administrator You Can Use This Screen To View Descriptive Tags Added To Collections Of Interest As Well As View The Descriptive Tags You Have Added To Items", "\"As a digital collection manager or administrator, you can use this screen to view descriptive tags added to collections of interest, as well as view the descriptive tags you have added to items.\"");
            User_Tags_MySobekViewer.Add_Localization_String( "Tags By Aggregation", "Tags By Aggregation");
            User_Tags_MySobekViewer.Add_Localization_String( "Choose An Aggregation Below To View All Tags For That Aggregation", "Choose an aggregation below to view all tags for that aggregation:");
            User_Tags_MySobekViewer.Add_Localization_String( "All Aggregations", "All Aggregations");
            User_Tags_MySobekViewer.Add_Localization_String( "Your Descriptive Tags", "Your Descriptive Tags");
            User_Tags_MySobekViewer.Add_Localization_String( "You Have Not Added Any Descriptive Tags To Any Items", "You have not added any descriptive tags to any items");
            User_Tags_MySobekViewer.Add_Localization_String( "You Have Added The Following XXX Descriptive Tags", "You have added the following %1 descriptive tags");
            User_Tags_MySobekViewer.Add_Localization_String( "Added By You On XXX", "Added by you on %1");
            User_Tags_MySobekViewer.Add_Localization_String( "View All By This User", "view all by this user");
            User_Tags_MySobekViewer.Add_Localization_String( "Added By XXX On XXX", "Added by %1 on %2");
            User_Tags_MySobekViewer.Add_Localization_String( "Tags By User", "Tags By User");
            User_Tags_MySobekViewer.Add_Localization_String( "This User Has Not Added Any Descriptive Tags To Any Items Or It Is Not A Valid Userid", "This user has not added any descriptive tags to any items or it is not a valid userid.");

            //Initialize the Page_Image_Upload_MySobekViewer_Localization class
            Page_Image_Upload_MySobekViewer = new Page_Image_Upload_MySobekViewer_LocalizationInfo();
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "Upload Page Images", "Upload Page Images");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "Upload The Page Images For Your Item You Will Then Be Directed To Manage The Pages And Divisions To Ensure The New Page Images Appear In The Correct Order And Are Reflected In The Table Of Contents", "Upload the page images for your item.  You will then be directed to manage the pages and divisions to ensure the new page images appear in the correct order and are reflected in the table of contents.");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "Click Here To Add Download Files Instead", "Click here to add download files instead.");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "The Following New Page Images Will Be Added To The Item Once You Click SUBMIT", "The following new page images will be added to the item once you click SUBMIT");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "FILENAME", "FILENAME");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "SIZE", "SIZE");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "DATE UPLOADED", "DATE UPLOADED");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "ACTION", "ACTION");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "Cancel This And Remove The Recentely Uploaded Images", "Cancel this and remove the recentely uploaded images");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "Submit The Recently Uploaded Page Images And Complete The Process", "Submit the recently uploaded page images and complete the process");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "Once All Images Are Uploaded Press SUBMIT To Finish This Item", "\"Once all images are uploaded, press SUBMIT to finish this item\"");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "The Following Extensions Are Accepted", "The following extensions are accepted");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "SUBMIT", "SUBMIT");
            Page_Image_Upload_MySobekViewer.Add_Localization_String( "CANCEL", "CANCEL");

            //Initialize the New_Group_And_Item_MySobekViewer_Localization class
            New_Group_And_Item_MySobekViewer = new New_Group_And_Item_MySobekViewer_LocalizationInfo();
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Permissions Agreement", "Permissions Agreement");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "User", "User");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Date", "Date");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Upload Files", "Upload Files");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Upload The Related Files For Your New Item You Can Also Provide Labels For Each File Once They Are Uploaded", "\"Upload the related files for your new item.  You can also provide labels for each file, once they are uploaded.\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Upload Files Or Enter URL", "Upload Files or Enter URL");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Upload The Related Files Or Enter A URL For Your New Item You Can Also Provide Labels For Each File Once They Are Uploaded", "\"Upload the related files or enter a URL for your new item.  You can also provide labels for each file, once they are uploaded\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Enter A URL For This New Item", "Enter a URL for this new item.");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Enter URL", "Enter URL");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Template", "Template");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Default Metadata", "Default Metadata");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Step 1 Of XXX Grant Of Permission", "Step 1 of %1: Grant of Permission");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "You Must Read And Accept The Below Permissions To Continue", "You must read and accept the below permissions to continue.");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "You Must Read And Accept The Above Permissions Agreement To Continue", "You must read and accept the above permissions agreement to continue.");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "CANCEL", "CANCEL");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "ACCEPT", "ACCEPT");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Step 1 Of XXX Confirm Template And Project", "Step 1 of %1: Confirm Template and Project ");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "You May Also Change Your Current XXX For This Submission", "You may also change your current %1 for this submission.");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "To Change Your Default XXX Select", "\"To change your default %1, select\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "My Account", "My Account");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Above", "above");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Item Description", "Item Description");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Enter The Basic Information To Describe Your New Item", "Enter the basic information to describe your new item");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "The Following Errors Were Detected", "The following errors were detected:");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "BACK", "BACK");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "CLEAR", "CLEAR");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "FILENAME", "FILENAME");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "SIZE", "SIZE");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "DATE UPLOADED", "DATE UPLOADED");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "ACTION", "ACTION");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "The Following Files Are Already Uploaded For This Package And Will Be Included As Downloads", "The following files are already uploaded for this package and will be included as downloads");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Once The URL Is Entered Press SUBMIT To Finish This Item", "\"Once the URL is entered, press SUBMIT to finish this item.\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Once You Enter Any Files Andor URL Press SUBMIT To Finish This Item", "\"Once you enter any files and/or URL, press SUBMIT to finish this item\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Once All Files Are Uploaded Press SUBMIT To Finish This Item", "\"Once all files are uploaded, press SUBMIT to finish this item\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Once Complete Press SUBMIT To Finish This Item", "\"Once complete, press SUBMIT to finish this item\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "SUBMIT", "SUBMIT");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Add A New Item For This Package", "Add a new item for this package");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "CONGRATULATIONS", "CONGRATULATIONS");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Your Item Has Been Successfully Added To The Digital Library And Will Appear Immediately", "Your item has been successfully added to the digital library and will appear immediately.");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Search Indexes May Take A Couple Minutes To Build At Which Time This Item Will Be Discoverable Through The Search Interface", "\"Search indexes may take a couple minutes to build, at which time this item will be discoverable through the search interface\"");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "An Email Has Been Sent To You With The New Item Information", "An email has been sent to you with the new item information.");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Ooops We Encountered A Problem", "Ooops!! We encountered a problem!");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "An Email Has Been Sent To The Programmer Who Will Attempt To Correct Your Issue You Should Be Contacted Within The Next 2448 Hours Regarding This Issue", "An email has been sent to the programmer who will attempt to correct your issue.  You should be contacted within the next 24-48 hours regarding this issue");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Return To My Home", "Return to my home");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "View This Item", "View this item");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Edit This Item", "Edit this item");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "Add Another Item", "Add another item");
            New_Group_And_Item_MySobekViewer.Add_Localization_String( "View All My Submitted Items", "View all my submitted items");

            //Initialize the Mass_Update_Items_MySobekViewer_Localization class
            Mass_Update_Items_MySobekViewer = new Mass_Update_Items_MySobekViewer_LocalizationInfo();
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "Change The Behavior Of All Items Belonging To This Group", "Change the behavior of all items belonging to this group");
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "Enter Any Behaviors You Would Like To Have Propogate Through All The Items Within This Item Groupand Press The SAVE Button When Complete", "Enter any behaviors you would like to have propogate through all the items within this item group.and press the SAVE button when complete.");
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "Clicking On The Green Plus Button", "Clicking on the green plus button ( ");
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "Will Add Another Instance Of The Element If The Element Is Repeatable", "\" ) will add another instance of the element, if the element is repeatable.\"");
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "Click", "Click");
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "Here For Detailed Instructions", "here for detailed instructions");
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "On Mass Updating Behaviors Online", "on mass updating behaviors online.");
            Mass_Update_Items_MySobekViewer.Add_Localization_String( "MASS UPDATE", "MASS UPDATE");

            //Initialize the Group_AutoFill_Volume_MySobekViewer_Localization class
            Group_AutoFill_Volume_MySobekViewer = new Group_AutoFill_Volume_MySobekViewer_LocalizationInfo();
            Group_AutoFill_Volume_MySobekViewer.Add_Localization_String( "AUTOFILL NEW VOLUMES", "AUTO-FILL NEW VOLUMES");
            Group_AutoFill_Volume_MySobekViewer.Add_Localization_String( "Implementation For This Feature Is Currently Pending", "Implementation for this feature is currently pending.");

            //Initialize the Group_Add_Volume_MySobekViewer_Localization class
            Group_Add_Volume_MySobekViewer = new Group_Add_Volume_MySobekViewer_LocalizationInfo();
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "No Base Volume Selected", "No base volume selected!");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "EXCEPTION CAUGHT", "EXCEPTION CAUGHT!");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "NEW VOLUME", "NEW VOLUME");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "Add A New Volume To This Existing Titleitem Group", "Add a new volume to this existing title/item group");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "Only Enter Data That You Wish To Override The Data In The Existing Base Volume", "Only enter data that you wish to override the data in the existing base volume.");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "Click", "Click");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "Here For Detailed Instructions", "here for detailed instructions");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "In Addition The Following Actions Are Available", "\"In addition, the following actions are available:\"");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "SAVE EDIT ITEM", "SAVE & EDIT ITEM");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "SAVE ADD FILES", "SAVE & ADD FILES");
            Group_Add_Volume_MySobekViewer.Add_Localization_String( "SAVE ADD ANOTHER", "SAVE & ADD ANOTHER");

            //Initialize the File_Management_MySobekViewer_Localization class
            File_Management_MySobekViewer = new File_Management_MySobekViewer_LocalizationInfo();
            File_Management_MySobekViewer.Add_Localization_String( "ERROR CAUGHT WHILE SAVING DIGITAL RESOURCE", "ERROR CAUGHT WHILE SAVING DIGITAL RESOURCE");
            File_Management_MySobekViewer.Add_Localization_String( "ERROR ENCOUNTERED DURING ONLINE FILE MANAGEMENT", "ERROR ENCOUNTERED DURING ONLINE FILE MANAGEMENT");
            File_Management_MySobekViewer.Add_Localization_String( "Error During File Management For XXX", "Error during file management for %1");
            File_Management_MySobekViewer.Add_Localization_String( "Manage Downloads", "Manage Downloads");
            File_Management_MySobekViewer.Add_Localization_String( "Upload The Download Files For Your Item You Can Also Provide Labels For Each File Once They Are Uploaded", "\"Upload the download files for your item.  You can also provide labels for each file, once they are uploaded.\"");
            File_Management_MySobekViewer.Add_Localization_String( "Click Here To Upload Page Images Instead", "Click here to upload page images instead.");
            File_Management_MySobekViewer.Add_Localization_String( "Add A New File For This Package", "Add a new file for this package");

            //Initialize the Edit_Serial_Hierarchy_MySobekViewer_Localization class
            Edit_Serial_Hierarchy_MySobekViewer = new Edit_Serial_Hierarchy_MySobekViewer_LocalizationInfo();
            Edit_Serial_Hierarchy_MySobekViewer.Add_Localization_String( "EDIT SERIAL HIERARCHY", "EDIT SERIAL HIERARCHY");
            Edit_Serial_Hierarchy_MySobekViewer.Add_Localization_String( "Implementation For This Feature Is Currently Pending", "Implementation for this feature is currently pending.");

            //Initialize the Edit_Item_Metadata_MySobekViewer_Localization class
            Edit_Item_Metadata_MySobekViewer = new Edit_Item_Metadata_MySobekViewer_LocalizationInfo();
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Edit Project", "Edit Project");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Edit Item", "Edit Item");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Edit This Item", "Edit this item");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Enter The Data For This Item Below And Press The SAVE Button When All Your Edits Are Complete", "Enter the data for this item below and press the SAVE button when all your edits are complete.");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Clicking On The Green Plus Button", "Clicking on the green plus button ( ");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Will Add Another Instance Of The Element If The Element Is Repeatable", "\" ) will add another instance of the element, if the element is repeatable.\"");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "You Are Using The Full Editing Form Because This Item Contains Complex Elements Or Was Derived From MARC", "You are using the full editing form because this item contains complex elements or was derived from MARC.");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "You Are Using The Full Editing Form Click", "You are using the full editing form.  Click");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Here To Return To The Simplified Version", "here to return to the simplified version");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "You Are Using The Simplified Editing Form Click", "You are using the simplified editing form.  Click");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "To Open Detailed Edit Forms Click On The Linked Metadata Values", "\"To open detailed edit forms, click on the linked metadata values.\"");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Click", "Click");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Here For Detailed Instructions", "here for detailed instructions");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "On Editing Metadata Online", " on editing metadata online.");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Edit This Project", "Edit this project");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Enter The Default Data For This Project Below And Press The SAVE Button When All Your Edits Are Complete", "Enter the default data for this project below and press the SAVE button when all your edits are complete.");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Clicking On The Blue Plus Signs", "Clicking on the blue plus signs ( ");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Click On The Element Names For Detailed Information Inluding Definitions Best Practices And Technical Information", "\"Click on the element names for detailed information inluding definitions, best practices, and technical information.\"");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "You Are Using The Full Editing Form Because You Are Editing A Project", "You are using the full editing form because you are editing a project.");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "Standard View", "Standard View");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "MARC View", "MARC View");
            Edit_Item_Metadata_MySobekViewer.Add_Localization_String( "METS View", "METS View");

            //Initialize the Edit_Item_Behaviors_MySobekViewer_Localization class
            Edit_Item_Behaviors_MySobekViewer = new Edit_Item_Behaviors_MySobekViewer_LocalizationInfo();
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "Edit Item Behaviors", "Edit Item Behaviors");
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "Edit This Items Behaviors Within This Library", "Edit this item's behaviors within this library");
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "Enter The Data For This Item Below And Press The SAVE Button When All Your Edits Are Complete", "Enter the data for this item below and press the SAVE button when all your edits are complete.");
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "Clicking On The Green Plus Button", "Clicking on the green plus button ( ");
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "Will Add Another Instance Of The Element If The Element Is Repeatable", "\") will add another instance of the element, if the element is repeatable.\"");
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "Click", "Click");
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "Here For Detailed Instructions", "here for detailed instructions");
            Edit_Item_Behaviors_MySobekViewer.Add_Localization_String( "On Editing Behaviors Online", "on editing behaviors online");

            //Initialize the Edit_Group_Behaviors_MySobekViewer_Localization class
            Edit_Group_Behaviors_MySobekViewer = new Edit_Group_Behaviors_MySobekViewer_LocalizationInfo();
            Edit_Group_Behaviors_MySobekViewer.Add_Localization_String( "Edit The Behaviors Associated With This Item Group Within This Library", "Edit the behaviors associated with this item group within this library");
            Edit_Group_Behaviors_MySobekViewer.Add_Localization_String( "Enter The Data For This Item Group Below And Press The SAVE Button When All Your Edits Are Complete", "Enter the data for this item group below and press the SAVE button when all your edits are complete.");
            Edit_Group_Behaviors_MySobekViewer.Add_Localization_String( "Clicking On The Green Plus Button", "Clicking on the green plus button ( ");
            Edit_Group_Behaviors_MySobekViewer.Add_Localization_String( "Will Add Another Instance Of The Element If The Element Is Repeatable", "\" ) will add another instance of the element, if the element is repeatable.\"");
            Edit_Group_Behaviors_MySobekViewer.Add_Localization_String( "Click", "Click");
            Edit_Group_Behaviors_MySobekViewer.Add_Localization_String( "Here For Detailed Instructions", "here for detailed instructions");
            Edit_Group_Behaviors_MySobekViewer.Add_Localization_String( "On Editing Behaviors Online", " on editing behaviors online.");

            //Initialize the Delete_Item_MySobekViewer_Localization class
            Delete_Item_MySobekViewer = new Delete_Item_MySobekViewer_LocalizationInfo();
            Delete_Item_MySobekViewer.Add_Localization_String( "Delete Item", "Delete Item");
            Delete_Item_MySobekViewer.Add_Localization_String( "Enter DELETE In The Textbox Below And Select GO To Complete This Deletion", "Enter DELETE in the textbox below and select GO to complete this deletion.");
            Delete_Item_MySobekViewer.Add_Localization_String( "Confirm Delete Of This Item", "Confirm delete of this item");
            Delete_Item_MySobekViewer.Add_Localization_String( "DELETE SUCCESSFUL", "DELETE SUCCESSFUL");
            Delete_Item_MySobekViewer.Add_Localization_String( "DELETE FAILED", "DELETE FAILED");
            Delete_Item_MySobekViewer.Add_Localization_String( "Insufficient User Permissions To Perform Delete", "Insufficient user permissions to perform delete");
            Delete_Item_MySobekViewer.Add_Localization_String( "Item Indicated Does Not Exists", "Item indicated does not exists");
            Delete_Item_MySobekViewer.Add_Localization_String( "Error While Performing Delete In Database", "Error while performing delete in database");
            Delete_Item_MySobekViewer.Add_Localization_String( "DELETE PARTIALLY SUCCESSFUL", "DELETE PARTIALLY SUCCESSFUL");
            Delete_Item_MySobekViewer.Add_Localization_String( "Unable To Move All Files To The RECYCLE BIN Folder", "Unable to move all files to the RECYCLE BIN folder");

            //Initialize the Text_MainWriter_Localization class
            Text_MainWriter = new Text_MainWriter_LocalizationInfo();
            Text_MainWriter.Add_Localization_String( "INVALID ITEM INDICATED", "INVALID ITEM INDICATED");
            Text_MainWriter.Add_Localization_String( "TEXT WRITER UNKNOWN MODE", "TEXT WRITER - UNKNOWN MODE");
            Text_MainWriter.Add_Localization_String( "Invalid Group", "Invalid Group");
            Text_MainWriter.Add_Localization_String( "Group", "Group");
            Text_MainWriter.Add_Localization_String( "Items", "Items");
            Text_MainWriter.Add_Localization_String( "Icons", "Icons");
            Text_MainWriter.Add_Localization_String( "Skins", "Skins");
            Text_MainWriter.Add_Localization_String( "GROUP Table", "GROUP table");
            Text_MainWriter.Add_Localization_String( "Grouptitle", "GroupTitle");
            Text_MainWriter.Add_Localization_String( "Bibid", "BibID");
            Text_MainWriter.Add_Localization_String( "Type", "Type");
            Text_MainWriter.Add_Localization_String( "Default_Collection", "Default_Collection");
            Text_MainWriter.Add_Localization_String( "File_Root", "File_Root");
            Text_MainWriter.Add_Localization_String( "Greenstone_Code", "Greenstone_Code");
            Text_MainWriter.Add_Localization_String( "Title", "Title");
            Text_MainWriter.Add_Localization_String( "Itemid", "ItemID");
            Text_MainWriter.Add_Localization_String( "Level1_Text", "Level1_Text");
            Text_MainWriter.Add_Localization_String( "Level1_Index", "Level1_Index");
            Text_MainWriter.Add_Localization_String( "Level2_Text", "Level2_Text");
            Text_MainWriter.Add_Localization_String( "Level2_Index", "Level2_Index");
            Text_MainWriter.Add_Localization_String( "Level3_Text", "Level3_Text");
            Text_MainWriter.Add_Localization_String( "Level3_Index", "Level3_Index");
            Text_MainWriter.Add_Localization_String( "Level4_Text", "Level4_Text");
            Text_MainWriter.Add_Localization_String( "Level4_Index", "Level4_Index");
            Text_MainWriter.Add_Localization_String( "Level5_Text", "Level5_Text");
            Text_MainWriter.Add_Localization_String( "Level5_Index", "Level5_Index");
            Text_MainWriter.Add_Localization_String( "ICON Table", "ICON table");
            Text_MainWriter.Add_Localization_String( "Icon_URL", "Icon_URL");
            Text_MainWriter.Add_Localization_String( "Link", "Link");
            Text_MainWriter.Add_Localization_String( "Icon_Name", "Icon_Name");
            Text_MainWriter.Add_Localization_String( "INTERFACE Table", "INTERFACE table");

            //Initialize the Html_Echo_MainWriter_Localization class
            Html_Echo_MainWriter = new Html_Echo_MainWriter_LocalizationInfo();
            Html_Echo_MainWriter.Add_Localization_String( "ERROR READING THE SOURCE FILE", "ERROR READING THE SOURCE FILE");

            //Initialize the TOC_ItemViewer_Localization class
            TOC_ItemViewer = new TOC_ItemViewer_LocalizationInfo();
            TOC_ItemViewer.Add_Localization_String( "Table Of Contents", "Table of Contents");

            //Initialize the Text_ItemViewer_Localization class
            Text_ItemViewer = new Text_ItemViewer_LocalizationInfo();
            Text_ItemViewer.Add_Localization_String( "Unknown Error While Retrieving Text", "Unknown error while retrieving text");
            Text_ItemViewer.Add_Localization_String( "No Text File Exists For This Page", "No text file exists for this page");
            Text_ItemViewer.Add_Localization_String( "No Text Is Recorded For This Page", "No text is recorded for this page");

            //Initialize the Street_ItemViewer_Localization class
            Street_ItemViewer = new Street_ItemViewer_LocalizationInfo();
            Street_ItemViewer.Add_Localization_String( "UNABLE TO LOAD STREETS FROM DATABASE", "UNABLE TO LOAD STREETS FROM DATABASE");
            Street_ItemViewer.Add_Localization_String( "Click", "Click");
            Street_ItemViewer.Add_Localization_String( "Here", "here");
            Street_ItemViewer.Add_Localization_String( "To Report This Issue", " to report this issue.");
            Street_ItemViewer.Add_Localization_String( "Index Of Streets", "Index of Streets");

            //Initialize the ManageMenu_ItemViewer_Localization class
            ManageMenu_ItemViewer = new ManageMenu_ItemViewer_LocalizationInfo();
            ManageMenu_ItemViewer.Add_Localization_String( "Manage This Item", "Manage this Item");
            ManageMenu_ItemViewer.Add_Localization_String( "Beta", "(beta)");
            ManageMenu_ItemViewer.Add_Localization_String( "Add A New Related Volume To This Item Group", "\"Add a new, related volume to this item group.\"");
            ManageMenu_ItemViewer.Add_Localization_String( "Add Geospatial Information For This Item This Can Be As Simple As A Location For A Photograph Or Can Be An Overlay For A Map Points Lines And Polygons Of Interest Can Also Be Drawn", "\"Add geo-spatial information for this item.  This can be as simple as a location for a photograph or can be an overlay for a map.  Points, lines, and polygons of interest can also be drawn.\"");
            ManageMenu_ItemViewer.Add_Localization_String( "Add New Volume", "Add New Volume");
            ManageMenu_ItemViewer.Add_Localization_String( "Change The Way This Item Behaves In This Library Including Which Aggregations It Appears Under The Wordmarks To The Left And Which Viewer Types Are Publicly Accessible", "\"Change the way this item behaves in this library, including which aggregations it appears under, the wordmarks to the left, and which viewer types are publicly accessible.\"");
            ManageMenu_ItemViewer.Add_Localization_String( "Edit Item Behaviors", "Edit Item Behaviors");
            ManageMenu_ItemViewer.Add_Localization_String( "Edit Item Group Behaviors", "Edit Item Group Behaviors");
            ManageMenu_ItemViewer.Add_Localization_String( "Edit Item Metadata", "Edit Item Metadata");
            ManageMenu_ItemViewer.Add_Localization_String( "Edit The Information About This Item Which Appears In The Citationdescription This Is Basic Information About The Original Item And This Digital Manifestation", "Edit the information about this item which appears in the citation/description.  This is basic information about the original item and this digital manifestation.");
            ManageMenu_ItemViewer.Add_Localization_String( "How Would You Like To Manage This Item Group Today", "How would you like to manage this item group today?");
            ManageMenu_ItemViewer.Add_Localization_String( "How Would You Like To Manage This Item Today", "How would you like to manage this item today?");
            ManageMenu_ItemViewer.Add_Localization_String( "In Addition The Following Changes Can Be Made At The Item Group Level", "\"In addition, the following changes can be made at the item group level:\"");
            ManageMenu_ItemViewer.Add_Localization_String( "Manage Download Files", "Manage Download Files");
            ManageMenu_ItemViewer.Add_Localization_String( "Manage Geospatial Data", "Manage Geo-Spatial Data");
            ManageMenu_ItemViewer.Add_Localization_String( "Manage Pages And Divisions Quality Control", "Manage Pages and Divisions (Quality Control)");
            ManageMenu_ItemViewer.Add_Localization_String( "Manage This Item Group", "Manage this Item Group");
            ManageMenu_ItemViewer.Add_Localization_String( "Mass Update Item Behaviors", "Mass Update Item Behaviors");
            ManageMenu_ItemViewer.Add_Localization_String( "Reorder Page Images Name Pages Assign Divisions And Delete And Add New Page Images To This Item", "\"Reorder page images, name pages, assign divisions, and delete and add new page images to this item.\"");
            ManageMenu_ItemViewer.Add_Localization_String( "Set The Title Under Which All Of These Items Appear In Search Results And Set The Web Skins Under Which All These Items Should Appear", "Set the title under which all of these items appear in search results and set the web skins under which all these items should appear.");
            ManageMenu_ItemViewer.Add_Localization_String( "This Allows Itemlevel Behaviors To Be Set For All Items Within This Item Group Including Which Aggregations It Appears Under The Wordmarks To The Left And Which Viewer Types Are Publicly Accessible", "\"This allows item-level behaviors to be set for all items within this item group, including which aggregations it appears under, the wordmarks to the left, and which viewer types are publicly accessible.\"");
            ManageMenu_ItemViewer.Add_Localization_String( "This Can Be Used For Printing The Tracking Sheet For This Item Which Can Be Used As Part Of The Builtin Digitization Workflow", "\"This can be used for printing the tracking sheet for this item, which can be used as part of the built-in digitization workflow.\"");
            ManageMenu_ItemViewer.Add_Localization_String( "Upload New Files For Download Or Remove Existing Files That Are Attached To This Item For Download This Generally Includes Everything Except For The Page Images", "Upload new files for download or remove existing files that are attached to this item for download.  This generally includes everything except for the page images.");
            ManageMenu_ItemViewer.Add_Localization_String( "View The History Of All Work Performed On This Item From This View You Can Also See Any Digitization Milestones And Digital Resource File Information", "\"View the history of all work performed on this item.  From this view, you can also see any digitization milestones and digital resource file information.\"");
            ManageMenu_ItemViewer.Add_Localization_String( "View Tracking Sheet", "View Tracking Sheet");
            ManageMenu_ItemViewer.Add_Localization_String( "View Work History", "View Work History");

            //Initialize the JPEG2000_ItemViewer_Localization class
            JPEG2000_ItemViewer = new JPEG2000_ItemViewer_LocalizationInfo();
            JPEG2000_ItemViewer.Add_Localization_String( "THUMBNAIL", "THUMBNAIL");

            //Initialize the JPEG_ItemViewer_Localization class
            JPEG_ItemViewer = new JPEG_ItemViewer_LocalizationInfo();
            JPEG_ItemViewer.Add_Localization_String( "Click On Image Below To Switch To Zoomable Version", "Click on image below to switch to zoomable version");
            JPEG_ItemViewer.Add_Localization_String( "Click On Image To Switch To Zoomable Version", "Click on image to switch to zoomable version");
            JPEG_ItemViewer.Add_Localization_String( "MISSING IMAGE", "MISSING IMAGE");

            //Initialize the Google_Coordinate_Entry_ItemViewer_Localization class
            Google_Coordinate_Entry_ItemViewer = new Google_Coordinate_Entry_ItemViewer_LocalizationInfo();
            Google_Coordinate_Entry_ItemViewer.Add_Localization_String( "Loading", "Loading...");

            //Initialize the Feature_ItemViewer_Localization class
            Feature_ItemViewer = new Feature_ItemViewer_LocalizationInfo();
            Feature_ItemViewer.Add_Localization_String( "Index Of Features", "Index of Features");
            Feature_ItemViewer.Add_Localization_String( "UNABLE TO LOAD FEATURES FROM DATABASE", "UNABLE TO LOAD FEATURES FROM DATABASE");

            //Initialize the abstractItemViewer_Localization class
            abstractItemViewer = new abstractItemViewer_LocalizationInfo();
            abstractItemViewer.Add_Localization_String( "Unnumbered", "Unnumbered ");
            abstractItemViewer.Add_Localization_String( "Page", "Page ");

            //Initialize the MainMenus_Helper_HtmlSubWriter_Localization class
            MainMenus_Helper_HtmlSubWriter = new MainMenus_Helper_HtmlSubWriter_LocalizationInfo();
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Home", "Home");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "View Items", "View Items");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "View All Items", "View All Items");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "View Recently Added Items", "View Recently Added Items");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "MY COLLECTIONS", "MY COLLECTIONS");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "BROWSE PARTNERS", "BROWSE PARTNERS");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "BROWSE BY", "BROWSE BY");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "MAP BROWSE", "MAP BROWSE");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "List View", "List View");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Brief View", "Brief View");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Tree View", "Tree View");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Browse Partners", "Browse Partners");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Added Automatically", " ( Added Automatically )");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "SEARCH OPTIONS", "SEARCH OPTIONS");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "BOOKSHELF VIEW", "BOOKSHELF VIEW");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "BRIEF VIEW", "BRIEF VIEW");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "MAP VIEW", "MAP VIEW");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "TABLE VIEW", "TABLE VIEW");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "THUMBNAIL VIEW", "THUMBNAIL VIEW");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "EXPORT", "EXPORT");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Save", "Save");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Send", "Send");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Print", "Print");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Advanced Search", "Advanced Search");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "My Library", "My Library");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "My Account", "My Account");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Internal", "Internal");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "System Admin", "System Admin");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Portal Admin", "Portal Admin");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Collection Hierarchy", "Collection Hierarchy");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "New Items", "New Items");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Memory Management", "Memory Management");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Wordmarks", "Wordmarks");
            MainMenus_Helper_HtmlSubWriter.Add_Localization_String( "Build Failures", "Build Failures");

            //Initialize the Item_Nav_Bar_HTML_Factory_Localization class
            Item_Nav_Bar_HTML_Factory = new Item_Nav_Bar_HTML_Factory_LocalizationInfo();
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "ALL ISSUES", "ALL ISSUES");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "RELATED MAPS", "RELATED MAPS");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "RELATED FLIGHTS", "RELATED FLIGHTS");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "ALL VOLUMES", "ALL VOLUMES");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "CITATION", "CITATION");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "DATA STRUCTURE", "DATA STRUCTURE");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "REPORTS", "REPORTS");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "EXPLORE DATA", "EXPLORE DATA");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "DOWNLOADS", "DOWNLOADS");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "FEATURES", "FEATURES");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "FLASH VIEW", "FLASH VIEW");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "MAP SEARCH", "MAP SEARCH");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "SEARCH RESULTS", "SEARCH RESULTS");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "MAP COVERAGE", "MAP COVERAGE");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "MAP IT", "MAP IT!");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "STANDARD", "STANDARD");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "PAGE IMAGE WITH TEXT", "PAGE IMAGE WITH TEXT");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "ZOOMABLE", "ZOOMABLE");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "PDF VIEWER", "PDF VIEWER");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "THUMBNAILS", "THUMBNAILS");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "SEARCH", "SEARCH");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "STREETS", "STREETS");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "PAGE TEXT", "PAGE TEXT");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "RESTRICTED", "RESTRICTED");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "CONTAINER LIST", "CONTAINER LIST");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "PAGE TURNER", "PAGE TURNER");
            Item_Nav_Bar_HTML_Factory.Add_Localization_String( "VIDEO", "VIDEO");

            //Initialize the abstractHtmlSubwriter_Localization class
            abstractHtmlSubwriter = new abstractHtmlSubwriter_LocalizationInfo();
            abstractHtmlSubwriter.Add_Localization_String( "OCLC Number", "OCLC Number");
            abstractHtmlSubwriter.Add_Localization_String( "ALEPH Number", "ALEPH Number");
            abstractHtmlSubwriter.Add_Localization_String( "Anywhere", "Anywhere");
            abstractHtmlSubwriter.Add_Localization_String( "Title", "Title");
            abstractHtmlSubwriter.Add_Localization_String( "Author", "Author");
            abstractHtmlSubwriter.Add_Localization_String( "Subject Keywords", "Subject Keywords");
            abstractHtmlSubwriter.Add_Localization_String( "Country", "Country");
            abstractHtmlSubwriter.Add_Localization_String( "State", "State");
            abstractHtmlSubwriter.Add_Localization_String( "County", "County");
            abstractHtmlSubwriter.Add_Localization_String( "City", "City");
            abstractHtmlSubwriter.Add_Localization_String( "Place Of Publication", "Place of Publication");
            abstractHtmlSubwriter.Add_Localization_String( "Spatial Coverage", "Spatial Coverage");
            abstractHtmlSubwriter.Add_Localization_String( "Type", "Type");
            abstractHtmlSubwriter.Add_Localization_String( "Language", "Language");
            abstractHtmlSubwriter.Add_Localization_String( "Publisher", "Publisher");
            abstractHtmlSubwriter.Add_Localization_String( "Genre", "Genre");
            abstractHtmlSubwriter.Add_Localization_String( "Target Audience", "Target Audience");
            abstractHtmlSubwriter.Add_Localization_String( "Donor", "Donor");
            abstractHtmlSubwriter.Add_Localization_String( "Attribution", "Attribution");
            abstractHtmlSubwriter.Add_Localization_String( "Tickler", "Tickler");
            abstractHtmlSubwriter.Add_Localization_String( "Notes", "Notes");
            abstractHtmlSubwriter.Add_Localization_String( "Identifier", "Identifier");
            abstractHtmlSubwriter.Add_Localization_String( "Frequency", "Frequency");
            abstractHtmlSubwriter.Add_Localization_String( "Tracking Box", "Tracking Box");

            //Initialize the Static_Browse_Info_AggregationViewer_Localization class
            Static_Browse_Info_AggregationViewer = new Static_Browse_Info_AggregationViewer_LocalizationInfo();
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Show Header Data Advanced", "show header data (advanced)");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "The Data Below Describes The Content Of This Static Child Page And Is Used By Some Search Engine Indexing Algorithms By Default It Will Not Show In Text Of The Page But Will Be Included In The Head Tag Of The Page", "\"The data below describes the content of this static child page and is used by some search engine indexing algorithms.  By default, it will not show in text of the page, but will be included in the head tag of the page.\"");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Title", "Title:");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Author", "Author:");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Date", "Date:");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Description", "Description:");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Keywords", "Keywords:");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "HTML Head Info", "HTML Head Info:");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Edit This Home Text", "Edit this home text");
            Static_Browse_Info_AggregationViewer.Add_Localization_String( "Edit Content", "edit content");

            //Initialize the Item_Count_Aggregation_Viewer_Localization class
            Item_Count_Aggregation_Viewer = new Item_Count_Aggregation_Viewer_LocalizationInfo();
            Item_Count_Aggregation_Viewer.Add_Localization_String( "Resource Count In Aggregation", "Resource Count in Aggregation");
            Item_Count_Aggregation_Viewer.Add_Localization_String( "Below Is The Number Of Titles And Items For All Items Within This Aggregation Including Currently Online Items As Well As Items In Process", "\"Below is the number of titles and items for all items within this aggregation, including currently online items as well as items in process.\"");
            Item_Count_Aggregation_Viewer.Add_Localization_String( "LAST MILESTONE", "LAST MILESTONE");
            Item_Count_Aggregation_Viewer.Add_Localization_String( "TITLE COUNT", "TITLE COUNT");
            Item_Count_Aggregation_Viewer.Add_Localization_String( "ITEM COUNT", "ITEM COUNT");
            Item_Count_Aggregation_Viewer.Add_Localization_String( "PAGE COUNT", "PAGE COUNT");
            Item_Count_Aggregation_Viewer.Add_Localization_String( "FILE COUNT", "FILE COUNT");

            //Initialize the Basic_Search_YearRange_AggregationViewer_Localization class
            Basic_Search_YearRange_AggregationViewer = new Basic_Search_YearRange_AggregationViewer_LocalizationInfo();
            Basic_Search_YearRange_AggregationViewer.Add_Localization_String( "Search Collection", "Search Collection");
            Basic_Search_YearRange_AggregationViewer.Add_Localization_String( "Limit By Year", "Limit by Year");

            //Initialize the Advanced_Search_YearRange_AggregationViewer_Localization class
            Advanced_Search_YearRange_AggregationViewer = new Advanced_Search_YearRange_AggregationViewer_LocalizationInfo();
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Search For", "Search for:");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "In", "in");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Search", "Search");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Search Options", "Search Options");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Precision", "Precision");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Contains Exactly The Search Terms", "Contains exactly the search terms");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Contains Any Form Of The Search Terms", "Contains any form of the search terms");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Contains The Search Term Or Terms Of Similar Meaning", "Contains the search term or terms of similar meaning");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Limit By Year", "Limit by Year");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "And", "and");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "Or", "or");
            Advanced_Search_YearRange_AggregationViewer.Add_Localization_String( "And Not", "and not");

        }

		#endregion

        /// <summary> Write the localization XML source file from the data within this localization object </summary>
        /// <param name="File"> Filename for the resulting XML file </param>
        /// <returns> TRUE if successful, otherise FALSE </returns>
		public bool Write_Localization_XML(string File)
		{
			try
			{
				// Open the file and write to it
				StreamWriter writer = new StreamWriter(File, false);
				writer.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"yes\" ?>");
				writer.WriteLine("<localization lang=\"" + Web_Language_Enum_Converter.Enum_To_Name(Language) + "\">");

				// Add the inforamtion for each localization object
				General.Write_Localization_XML(writer);
				Html_MainWriter.Write_Localization_XML(writer);
				URL_Email_Helper.Write_Localization_XML(writer);
				Item_Email_Helper.Write_Localization_XML(writer);
				Usage_Stats_Email_Helper.Write_Localization_XML(writer);
				Aggregation_HtmlSubwriter.Write_Localization_XML(writer);
				Aggregation_Nav_Bar_HTML_Factory.Write_Localization_XML(writer);
				Advanced_Search_AggregationViewer.Write_Localization_XML(writer);
				Basic_Search_AggregationViewer.Write_Localization_XML(writer);
				dLOC_Search_AggregationViewer.Write_Localization_XML(writer);
				Full_Text_Search_AggregationViewer.Write_Localization_XML(writer);
				Item_Count_AggregationViewer.Write_Localization_XML(writer);
				Map_Browse_AggregationViewer.Write_Localization_XML(writer);
				Map_Search_AggregationViewer.Write_Localization_XML(writer);
				Metadata_Browse_AggregationViewer.Write_Localization_XML(writer);
				Newspaper_Search_AggregationViewer.Write_Localization_XML(writer);
				No_Search_AggregationViewer.Write_Localization_XML(writer);
				Private_Items_AggregationViewer.Write_Localization_XML(writer);
				Rotating_Highlight_Search_AggregationViewer.Write_Localization_XML(writer);
				Usage_Statistics_AggregationViewer.Write_Localization_XML(writer);
				Contact_HtmlSubwriter.Write_Localization_XML(writer);
				Error_HtmlSubwriter.Write_Localization_XML(writer);
				Internal_HtmlSubwriter.Write_Localization_XML(writer);
				LegacyUrl_HtmlSubwriter.Write_Localization_XML(writer);
				Preferences_HtmlSubwriter.Write_Localization_XML(writer);
				Print_Item_HtmlSubwriter.Write_Localization_XML(writer);
				Public_Folder_HtmlSubwriter.Write_Localization_XML(writer);
				Search_Results_HtmlSubwriter.Write_Localization_XML(writer);
				Statistics_HtmlSubwriter.Write_Localization_XML(writer);
				Item_HtmlSubwriter.Write_Localization_XML(writer);
				AddRemove_Fragment_ItemViewer.Write_Localization_XML(writer);
				Describe_Fragment_ItemViewer.Write_Localization_XML(writer);
				PrintForm_Fragment_ItemViewer.Write_Localization_XML(writer);
				Send_Fragment_ItemViewer.Write_Localization_XML(writer);
				Share_Fragment_ItemViewer.Write_Localization_XML(writer);
				Checked_Out_ItemViewer.Write_Localization_XML(writer);
				Citation_ItemViewer.Write_Localization_XML(writer);
				Download_ItemViewer.Write_Localization_XML(writer);
				EAD_Container_List_ItemViewer.Write_Localization_XML(writer);
				EAD_Description_ItemViewer.Write_Localization_XML(writer);
				EmbeddedVideo_ItemViewer.Write_Localization_XML(writer);
				GnuBooks_PageTurner_ItemViewer.Write_Localization_XML(writer);
				Google_Map_ItemViewer.Write_Localization_XML(writer);
				HTML_Map_ItemViewer.Write_Localization_XML(writer);
				HTML_ItemViewer.Write_Localization_XML(writer);
				Aware_JP2_ItemViewer.Write_Localization_XML(writer);
				MultiVolumes_ItemViewer.Write_Localization_XML(writer);
				PDF_ItemViewer.Write_Localization_XML(writer);
				QC_ItemViewer.Write_Localization_XML(writer);
				Related_Images_ItemViewer.Write_Localization_XML(writer);
				YouTube_Embedded_Video_ItemViewer.Write_Localization_XML(writer);
				Text_Search_ItemViewer.Write_Localization_XML(writer);
				Tracking_ItemViewer.Write_Localization_XML(writer);
				abstract_ResultsViewer.Write_Localization_XML(writer);
				PagedResults_HtmlSubwriter.Write_Localization_XML(writer);
				Bookshelf_View_ResultsViewer.Write_Localization_XML(writer);
				Brief_ResultsViewer.Write_Localization_XML(writer);
				Export_View_ResultsViewer.Write_Localization_XML(writer);
				Map_ResultsWriter.Write_Localization_XML(writer);
				No_Results_ResultsViewer.Write_Localization_XML(writer);
				Table_ResultsViewer.Write_Localization_XML(writer);
				Thumbnail_ResultsViewer.Write_Localization_XML(writer);
				MySobek_HtmlSubwriter.Write_Localization_XML(writer);
				Saved_Searches_MySobekViewer.Write_Localization_XML(writer);
				Preferences_MySobekViewer.Write_Localization_XML(writer);
				NewPassword_MySobekViewer.Write_Localization_XML(writer);
				Logon_MySobekViewer.Write_Localization_XML(writer);
				Home_MySobekViewer.Write_Localization_XML(writer);
				Folder_Mgmt_MySobekViewer.Write_Localization_XML(writer);
				Aggregation_Single_AdminViewer.Write_Localization_XML(writer);
				Google_Map_ResultsViewer.Write_Localization_XML(writer);
				User_Usage_Stats_MySobekViewer.Write_Localization_XML(writer);
				User_Tags_MySobekViewer.Write_Localization_XML(writer);
				Page_Image_Upload_MySobekViewer.Write_Localization_XML(writer);
				New_Group_And_Item_MySobekViewer.Write_Localization_XML(writer);
				Mass_Update_Items_MySobekViewer.Write_Localization_XML(writer);
				Group_AutoFill_Volume_MySobekViewer.Write_Localization_XML(writer);
				Group_Add_Volume_MySobekViewer.Write_Localization_XML(writer);
				File_Management_MySobekViewer.Write_Localization_XML(writer);
				Edit_Serial_Hierarchy_MySobekViewer.Write_Localization_XML(writer);
				Edit_Item_Metadata_MySobekViewer.Write_Localization_XML(writer);
				Edit_Item_Behaviors_MySobekViewer.Write_Localization_XML(writer);
				Edit_Group_Behaviors_MySobekViewer.Write_Localization_XML(writer);
				Delete_Item_MySobekViewer.Write_Localization_XML(writer);
				Text_MainWriter.Write_Localization_XML(writer);
				Html_Echo_MainWriter.Write_Localization_XML(writer);
				TOC_ItemViewer.Write_Localization_XML(writer);
				Text_ItemViewer.Write_Localization_XML(writer);
				Street_ItemViewer.Write_Localization_XML(writer);
				ManageMenu_ItemViewer.Write_Localization_XML(writer);
				JPEG2000_ItemViewer.Write_Localization_XML(writer);
				JPEG_ItemViewer.Write_Localization_XML(writer);
				Google_Coordinate_Entry_ItemViewer.Write_Localization_XML(writer);
				Feature_ItemViewer.Write_Localization_XML(writer);
				abstractItemViewer.Write_Localization_XML(writer);
				MainMenus_Helper_HtmlSubWriter.Write_Localization_XML(writer);
				Item_Nav_Bar_HTML_Factory.Write_Localization_XML(writer);
				abstractHtmlSubwriter.Write_Localization_XML(writer);
				Static_Browse_Info_AggregationViewer.Write_Localization_XML(writer);
				Item_Count_Aggregation_Viewer.Write_Localization_XML(writer);
				Basic_Search_YearRange_AggregationViewer.Write_Localization_XML(writer);
				Advanced_Search_YearRange_AggregationViewer.Write_Localization_XML(writer);

				// Close the file
				writer.WriteLine("</localization>");
				writer.Flush();
				writer.Close();

				return true;
			}
			catch (Exception ee)
			{
				return false;
			}
		}
    }
}
