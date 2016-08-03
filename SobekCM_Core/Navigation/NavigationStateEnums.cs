namespace SobekCM.Core.Navigation
{

    /// <summary> Writer type which determines what type of response is requested </summary>
    /// <remarks> This generally corresponds to which type of Main Writer is used, and which ASP application replies<br /><br />
    /// This value generally comes from the first character of the mode value in the URL query string ( m=X... ) </remarks>
    public enum Writer_Type_Enum : byte
    {
        /// <summary> Response should be in HTML </summary>
        HTML = 1,

        /// <summary> Simple writer just echoes through an existing HTML page through this application 
        /// with very little logic (used for robot search engines mostly) </summary>
        HTML_Echo,

        /// <summary> Response should be in HTML, but the user is logged in</summary>
        /// <remarks>This seperate writer type is implemented to force the user's system to refresh when
        /// the user's logon state changes.</remarks>
        HTML_LoggedIn,

        /// <summary> Response should be in microsoft compliant dataset format </summary>
        /// <remarks>This type of request is forwarded to sobekcm_data.aspx <br /><br />
        /// Generally, this is achieved by just using the standard write to xml routines in a DataSet</remarks>
        DataSet,

        /// <summary> Response should be a portion of a datatable, in JSON response </summary>
        /// <remarks> This is used to provide data server-side for the jQuery Datatable plug-in mostly </remarks>
        Data_Provider,

        /// <summary> Response should be in simplified JSON (Javan Simple Object Notation) format </summary>
        /// <remarks>This type of request is forwarded to sobekcm_data.aspx <br /><br />
        /// This is used to provide support to the iPhone application</remarks>
        JSON,

        /// <summary> Response should be compliant with the OAI-PMH standard </summary>
        /// <remarks>This type of request is forwarded to sobekcm_oai.aspx </remarks>
        OAI,

        /// <summary> Response should be in HTML, but be simple text, not formatted </summary>
        Text,

        /// <summary> Response should be in simplified XML format </summary>
        /// <remarks>This type of request is forwarded to sobekcm_data.aspx </remarks>
        XML
    };

    /// <summary> Display mode which determines what major category of action is being requested </summary>
    /// <remarks> For HTML rendering, these roughly correspond to which HTML subwriter to use.<br /><br />
    /// This value generally comes from the second character of the mode value in the URL query string ( m=.X... ) </remarks>
    public enum Display_Mode_Enum : byte
    {
        /// <summary>No display mode set</summary>
        None,

        /// <summary>An error was encountered during processing</summary>
        Error,

        /// <summary> System and portal administrator tasks </summary>
        Administrative,

        /// <summary> Aggregation views, such as viewing the home page, child pages, etc.. </summary>
        Aggregation,

        /// <summary>Request to send an email and contact the digital library team</summary>
        Contact,

        /// <summary>Displays the congratulations screen after the user submits a contact us email</summary>
        Contact_Sent,

        /// <summary>Display internal information about this digital library</summary>
        Internal,

        /// <summary>Just reload the information about each of the items into the cache</summary>
        Item_Cache_Reload,

        /// <summary>Display a single item</summary>
        Item_Display,

        /// <summary>Renders a single item for printing </summary>
        Item_Print,

        /// <summary>Used to redirect from a legacy URL to a new URL, during URL transition times</summary>
        Legacy_URL,

        /// <summary> This holds the mapsearch beta enum</summary>
        MapSearchBeta,

        /// <summary>Display is something from the authenticated portion of mySobek</summary>
        My_Sobek,

        /// <summary>Simple preferences (not related to mySobek) are displayed, allowing user to select language, etc..</summary>
        Preferences,

        /// <summary> Display a user's public folder </summary>
        Public_Folder,

        /// <summary> Reporting module </summary>
        Reports,

        /// <summary>Full memory reset was requested</summary>
        Reset,

        /// <summary>Display results of a search against items within an aggregation</summary>
        Results,

        /// <summary>Search items within an aggregation</summary>
        Search,

        /// <summary>Simply works as a content management system, displaying static html pages within an interface</summary>
        Simple_HTML_CMS,

        /// <summary>Display statistical information about item counts and usage</summary>
        Statistics
    };

    /// <summary> Type of search type to display </summary>
    /// <remarks> This roughly corresponds to the collection viewer used by the collection html subwriter</remarks>
    public enum Search_Type_Enum : byte
    {
        /// <summary> No search type specified, not applicable </summary>
        NONE,

        /// <summary> Advanced search type allows boolean searching with four different search fields </summary>
        Advanced,

        /// <summary> Basic search type allows metadata searching with one search field</summary>
        Basic,

        /// <summary> dLOC-specific full text search against the text of all documents in an item aggregation </summary>
        dLOC_Full_Text,

        /// <summary> Full text searches against the text of all the documents in an item aggregation </summary>
        Full_Text,

        /// <summary> Map searching employs a map to allow the user to select a rectangle of interest</summary>
        Map,

        /// <summary> Map searching employs a map to allow the user to select a rectangle of interest</summary>
        Map_Beta,

        /// <summary> Newspaper search type allows searching with one search field and suggests several metadata fields to search (i.e., newspaper title, full text, location, etc..)</summary>
        Newspaper,

    };

    /// <summary> Preciseness or exactness of the search </summary>
    public enum Search_Precision_Type_Enum : byte
    {
        /// <summary> Searches for inflectional forms of the search word(s), for example plural, past tense, etc.. </summary>
        Inflectional_Form = 1,

        /// <summary> Searches for synonmic forms of the search word(s) by using a thesaurus </summary>
        Synonmic_Form,

        /// <summary> Results must contain the actual word, with no stemming or alternative searches </summary>
        Contains,

        /// <summary> Results must exactly match the search term (which does not use the full-text indexing) </summary>
        Exact_Match
    }

    /// <summary> Format to display search or browse results </summary>
    /// <remarks> This roughly corresponds to the result viewer used by the results (or collection) html subwriter</remarks>
    public enum Result_Display_Type_Enum : byte
    {
        /// <summary> No result display type specified, not applicable </summary>
        NONE,

        /// <summary> Default result display type means that not particular type was selected
        /// and the item aggregation default is utilized </summary>
        Default,

        /// <summary> Displays the results in the bookshelf view, which allows the user to remove the item
        /// from the bookshelf, move the item, or edit the user notes </summary>
        Bookshelf,

        /// <summary> Display the results in a brief metadata format (with thumbnails) </summary>
        Brief,

        /// <summary> Allows results to be exported as CSV or excel files </summary>
        Export,

        /// <summary> Display the full citation of a single result</summary>
        Full_Citation,

        /// <summary> Display the main image of a single result</summary>
        Full_Image,

        /// <summary> Display the results according to their main coordinate information</summary>
        Map,

        /// <summary> Display the results according to their main coordinate information</summary>
        Map_Beta,

        /// <summary> Static text-type browse/info mode </summary>
        Static_Text,

        /// <summary> Display the results in a simple table format </summary>
        Table,

        /// <summary> Display the results in a thumbnail format</summary>
        Thumbnails
    };

    /// <summary> Type of administrative information requested for display </summary>
    public enum Internal_Type_Enum : byte
    {
        /// <summary> No internal type specified, not applicable </summary>
        NONE,

        /// <summary> Gets the complete list of all aggregations </summary>
        Aggregations_List,

        /// <summary> Display list of aggregations of one particular type</summary>
        Aggregations_Tree,

        /// <summary> Gets list of recent failures encountered during building </summary>
        Build_Failures,

        /// <summary> Display list of all items in memory; global, cache, and session </summary>
        Cache,

        /// <summary> Display list of all new and modified items in last week (or so) </summary>
        New_Items,

        /// <summary> Gets thumbnails of all current wordmarks, along with the wordmark code </summary>
        /// <remarks> This is used during online metadata editing to aid the user in selecting a wordmark</remarks>
        Wordmarks
    };

    /// <summary> Type of home page to display (for the main library home page)</summary>
    public enum Home_Type_Enum : byte
    {

        /// <summary> Display icons with full descriptions </summary>
        Descriptions,

        /// <summary> Display standard list with icons [DEFAULT] </summary>
        List,

        /// <summary> Displays the list of all the institutional partners </summary>
        Partners_List,

        /// <summary> Displays the thumbnails of all the institutional partners </summary>
        Partners_Thumbnails,

        /// <summary> Display the personalized home page for logged on users </summary>
        Personalized,

        /// <summary> Display the hierarchical tree view, initially fully collapsed </summary>
        Tree,
    };

    /// <summary> Type of statistical information to display </summary>
    public enum Statistics_Type_Enum : byte
    {
        /// <summary> No statistics type specified, not applicable </summary>
        NONE,

        /// <summary> Displays the item count for an arbitrary date and the growth from the first arbitrary date and the second date </summary>
        Item_Count_Arbitrary_View,

        /// <summary> Displays the current number of items in each aggregation, as well as growth during the last FYTD </summary>
        Item_Count_Growth_View,

        /// <summary> Displays the current number of items in each aggregation </summary>
        Item_Count_Standard_View,

        /// <summary> Displays the current number of items in each aggregation, in comma-seperate value text</summary>
        Item_Count_Text,

        /// <summary> Displays list of recent searches performed against this digital library</summary>
        Recent_Searches,

        /// <summary> Displays the overall usage at the aggregation-level by date, in comma-seperate value text</summary>
        Usage_By_Date_Text,

        /// <summary> Displays the overall usage at the aggregation-level by date</summary>
        Usage_Collections_By_Date,

        /// <summary> Displays the overall usage by date of a single item aggregation</summary>
        Usage_Collection_History,

        /// <summary> Displays the overall usage by date of a single item aggregation, in comma-seperate value text</summary>
        Usage_Collection_History_Text,

        /// <summary> Displays the definitions of the terms used in the statistical screens</summary>
        Usage_Definitions,

        /// <summary> Displays the most often used items within a single item aggregation</summary>
        Usage_Items_By_Collection,

        /// <summary> Displays the overall usage at the item-level by date</summary>
        Usage_Item_Views_By_Date,

        /// <summary> Displays the most often used titles within a single item aggregation</summary>
        Usage_Titles_By_Collection,

        /// <summary> Displays the overall usage against the overall architecture</summary>
        Usage_Overall
    };

    /// <summary> Type of mySobek display or action requested by the user </summary>
    public enum My_Sobek_Type_Enum : byte
    {
        /// <summary> No mySobek type specified, not applicable </summary>
        NONE,

        /// <summary> Allows system administrators the ability to delete an item online </summary>
        Delete_Item,

        /// <summary> Edit the behaviors of an existing item group within this digital library </summary>
        Edit_Group_Behaviors,

        /// <summary> Edit the serial hierarchy for all items under an existing item group </summary>
        Edit_Group_Serial_Hierarchy,

        /// <summary> Edits the behaviors of an existing item within this digital library </summary>
        Edit_Item_Behaviors,

        /// <summary> Edit an existing item through the online metadata editing process </summary>
        Edit_Item_Metadata,

        /// <summary> Edit an existing item 's permissions ( visibility, embargo, etc.. ) </summary>
        Edit_Item_Permissions,

        /// <summary> Edit the files related to a digial resource, by deleting or uploading new files </summary>
        File_Management,

        /// <summary> View a current folder (or bookshelf) or perform folder management on saved items </summary>
        Folder_Management,

        /// <summary> Page is used as the landing page when coming back from Shibboleth authentication </summary>
        Shibboleth_Landing,

        /// <summary> Add a new volume to an existing item group  </summary>
        Group_Add_Volume,

        /// <summary> Auto-fill multiple new volumes to an existing item group </summary>
        Group_AutoFill_Volumes,

        /// <summary> Mass update the behaviors for all the items within a particular item group </summary>
        Group_Mass_Update_Items,

        /// <summary> Display the mySobek home page, with possible courses of actions </summary>
        Home,

        /// <summary>Enter the tracking information for a given item using the physical tracking sheet </summary>
        Item_Tracking,

        /// <summary> Logout of mySobek and return to non-authenticated (public) mode </summary>
        Log_Out,

        /// <summary> Log on to mySobek and enter authenticated (private) mode </summary>
        Logon,

        /// <summary> Submit a new item through the online submittal process </summary>
        New_Item,

        /// <summary> Change your current password (or temporary password) </summary>
        New_Password,

        /// <summary> Edit the page images related to a digial resource, by deleting or uploading new images </summary>
        Page_Images_Management,

        /// <summary> Edit user-based preferences and user information </summary>
        Preferences,

        /// <summary> List of all saved searches for this user </summary>
        Saved_Searches,

        /// <summary> Provides way to view all user lists entered by a user or by aggregation </summary>
        User_Tags,

        /// <summary> Provides a list of all items linked to a user along with usage statistics for a given month/year </summary>
        User_Usage_Stats
    };

    /// <summary> Type of admin display or action requested by the system or portal administrator </summary>
    public enum Admin_Type_Enum : byte
    {
        /// <summary> No admin type specified, not applicable </summary>
        NONE,

        /// <summary> Adds a single collection to this instance, via the wizard </summary>
        Add_Collection_Wizard,

        /// <summary> Allows all the information and behaviors for a single aggregation to be viewed / edited </summary>
        Aggregation_Single,

        /// <summary> Provides list of all existing aggregationPermissions and allows admin to enter a new aggregation </summary>
        Aggregations_Mgmt,

        /// <summary> Provides list of all aggregation aliases and allows admin to perform some very basic tasks </summary>
        Aliases,

        /// <summary> Allows a single builder folder to be either added or edited online </summary>
        Builder_Folder_Mgmt,

        /// <summary> Gives the current SobekCM status and allows an authenticated system admin to temporarily halt the builder remotely via a database flag </summary>
        Builder_Status,

        /// <summary> Provides list of all existing default metadata files and allows admin to enter a new default or edit an existing default </summary>
        Default_Metadata,

        /// <summary> Administrative home page with links to all the Admin tasks </summary>
        Home,

        /// <summary> Provides list of the IP restriction lists and allows admins to edit the single IPs within the range(s) </summary>
        IP_Restrictions,

        /// <summary> Allows admin to perform some limited cache reset functions </summary>
        Reset,

        /// <summary> Allows admins to view and edit system-wide settings from the database </summary>
        Settings,

        /// <summary> Detailed editing of a single web skin </summary>
        Skins_Single,

        /// <summary> Provides list of all existing web skins and allows admin to enter a new web skin or edit an existing web skin </summary>
        Skins_Mgmt,

        /// <summary> Administrative features related to the TEI plug-in </summary>
        TEI,

        /// <summary> Allows the system administrator to add new thematic headings to the main home page </summary>
        Thematic_Headings,

        /// <summary> Allows admin to perform some limited actions against the URL Portals data </summary>
        URL_Portals,

        /// <summary> Provides list of all users and allows admin to perform some very basic tasks </summary>
        Users,

        /// <summary> Allows for editing and viewing of user groups </summary>
        User_Groups,

        /// <summary> Provides top-level reports regarding permissions granted to users </summary>
        User_Permissions_Reports,

        /// <summary> Form to add a new web content page to the system </summary>
        WebContent_Add_New,

        /// <summary> View recent updates to the top-level static HTML web content pages </summary>
        WebContent_History,

        /// <summary> Manage the list of all top-level static HTML web content pages in this instance </summary>
        WebContent_Mgmt,

        /// <summary> Allows all the information and behaviors about a single web content page to be edited </summary>
        WebContent_Single,

        /// <summary> View the usage stats for the top-level static HTML web content pages in this instance </summary>
        WebContent_Usage,

        /// <summary> Provides list of all existing wordmarks/icons and allows admin to enter a new wordmark or edit an existing wordmark </summary>
        Wordmarks
    };

    /// <summary> Type of aggregation display </summary>
    public enum Aggregation_Type_Enum : byte
    {
        /// <summary> No aggregation type specified, not applicable </summary>
        NONE,

        /// <summary>Browse item metadata values within an aggregation </summary>
        Browse_By,

        /// <summary>Browse items within an aggregation or static html collection pages</summary>
        Browse_Info,

        /// <summary> View all the coordinates linked to this item aggregation on a map </summary>
        Browse_Map,

        /// <summary> View all the coordinates linked to this item aggregation on a map </summary>
        Browse_Map_Beta,

        /// <summary>Ability to edit the static html aggregation child pages</summary>
        Child_Page_Edit,

        /// <summary>Display the home page for an aggregation (and default search option)</summary>
        Home,

        /// <summary>Display the home page for an aggregation (and default search option) with ability to edit the home page HTML</summary>
        Home_Edit,

        /// <summary> Display the item count information for a single item aggregation  </summary>
        Item_Count,

        /// <summary> Menu for aggregation curators and admins gives access to all the
        /// special aggregation views they can use </summary>
        Manage_Menu,

        /// <summary> Display the list of private items for a single item aggregation </summary>
        Private_Items,

        /// <summary> Display the usage statistics for a single item aggregation </summary>
        Usage_Statistics,

        /// <summary> Displays all the users with their permissions, either individually assigned 
        /// or assigned through a group, that have special permissions assigned for this aggregation </summary>
        User_Permissions,

        /// <summary> List of all changes against the aggregation itself, such as design or
        /// other administrative changes </summary>
        Work_History,
    }


    /// <summary> Flag is used to determine whether the table of contents should be displayed in the item viewer </summary>
    public enum TOC_Display_Type_Enum : byte
    {
        /// <summary> Display is not explicitly stated, so use the last value in the session state </summary>
        Undetermined = 1,

        /// <summary> Display the table of contents </summary>
        Show,

        /// <summary> Hide the table of contents </summary>
        Hide
    };

    /// <summary> Flag is used to determine whether the trace route should be included at the end of the html page </summary>
    public enum Trace_Flag_Type_Enum : byte
    {
        /// <summary> No data collection on whether to show the trace route</summary>
        Unspecified = 0,

        /// <summary> Do not display the trace route information </summary>
        No = 1,

        /// <summary> Show the trace route information; it was explicitely requested in the URL </summary>
        Explicit,

        /// <summary> Show the trace route information, although it was not requested in the URL </summary>
        /// <remarks> Trace route can be requested either by IP address, or by currently logged on user</remarks>
        Implied
    };

    /// <summary> Type of top-level web content display </summary>
    public enum WebContent_Type_Enum : byte
    {
        /// <summary> No web content display type specified, not applicable </summary>
        NONE,

        /// <summary> Viewer used to verify a delete request </summary>
        Delete_Verify,

        /// <summary> Standard public display of the web content </summary>
        Display,

        /// <summary> Edit of the web content </summary>
        Edit,

        /// <summary> Menu for gives access to all the special actions 
        /// that can be performed against a web content page </summary>
        Manage_Menu,

        /// <summary> Milestones for the web content </summary>
        Milestones,

        /// <summary> Permissions admin for the web content </summary>
        Permissions,

        /// <summary> Usage report for a single web content page </summary>
        Usage
    }
}
