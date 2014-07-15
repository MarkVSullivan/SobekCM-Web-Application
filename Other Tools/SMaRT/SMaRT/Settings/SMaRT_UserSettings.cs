#region Using directives

using System;
using System.Drawing;
using SobekCM.Library.Navigation;
using SobekCM.Library.Search;
using SobekCM.Tools.Settings;

#endregion

namespace SobekCM.Management_Tool.Settings
{
    /// <summary> Enumeration defines what occurs when a user clicks on an item group 
    /// row in the view items form </summary>
    public enum View_Items_Form_Action_On_Click_Enum : byte
    {
        /// <summary> Users is shown a form for the specific item group or item selected </summary>
        Show_Form = 1,

        /// <summary> Item group or item is opened on the web, in the default browser </summary>
        Open_On_Web
    }

    /// <summary> Enumeration defines what occurs when a user performs a search in the discovery
    /// form and has only one matching item group </summary>
    public enum Single_Result_Action_Enum : byte
    {
        /// <summary> Users is shown the result in the grid, just like any other result set </summary>
        Show_In_Grid = 1,

        /// <summary> Item group is opened for display, either online or in the form </summary>
        Show_Details_For_Single_Item
    }

    /// <summary> SMaRT_UserSettings is a static class which holds all of the settings for this 
    /// particular user and assemply in the Isolated Storage. </summary>
    public class SMaRT_UserSettings : IS_UserSettings
    {
        /// <summary> Name of the XML file used to store the QC settings </summary>
        private const string fileName = "SMaRT_UserSettings_Version_A";

        /// <summary> Static constructor for the SMaRT_UserSettings class. </summary>
        static SMaRT_UserSettings()
        {
            Load();
        }

        /// <summary> Preference on the Item Discovery form when a single item group is returned from a search </summary>
        public static Single_Result_Action_Enum Item_Discovery_Form_Single_Result_Action
        {
            get
            {
                int value = Get_Int_Setting("Single_Result_Discovery_Setting");
                switch (value)
                {
                    case 2:
                        return Single_Result_Action_Enum.Show_Details_For_Single_Item;

                    default:
                        return Single_Result_Action_Enum.Show_In_Grid;
                }
            }
            set {
                Add_Setting("Single_Result_Discovery_Setting", value == Single_Result_Action_Enum.Show_Details_For_Single_Item ? 2 : 1);
            }
        }

        /// <summary> Remembers last search precision used during item discovery or ad hoc report generation </summary>
        public static Search_Precision_Type_Enum Search_Precision
        {
            get
            {
                int value = Get_Int_Setting("Search_Precision_Type");
                switch (value)
                {
                    case 2:
                        return Search_Precision_Type_Enum.Contains;

                    case 3:
                        return Search_Precision_Type_Enum.Exact_Match;

                    case 4:
                        return Search_Precision_Type_Enum.Synonmic_Form;

                    default:
                        return Search_Precision_Type_Enum.Inflectional_Form;
                }
            }
            set
            {
                switch (value)
                {
                    case Search_Precision_Type_Enum.Contains:
                        Add_Setting("Search_Precision_Type", 2);
                        break;

                    case Search_Precision_Type_Enum.Exact_Match:
                        Add_Setting("Search_Precision_Type", 3);
                        break;

                    case Search_Precision_Type_Enum.Synonmic_Form:
                        Add_Setting("Search_Precision_Type", 4);
                        break;

                    default:
                        Add_Setting("Search_Precision_Type", 1);
                        break;

                }
            }
        }

        /// <summary> Preference on the View Items form when a single item group row is double clicked </summary>
        public static View_Items_Form_Action_On_Click_Enum Item_Discovery_Form_Action_On_Click
        {
            get
            {
                int value = Get_Int_Setting("Item_Discovery_Form_Action_On_Click");
                switch (value)
                {
                    case 2:
                        return View_Items_Form_Action_On_Click_Enum.Open_On_Web;

                    default:
                        return View_Items_Form_Action_On_Click_Enum.Show_Form;
                }
            }
            set {
                Add_Setting("Item_Discovery_Form_Action_On_Click",
                            value == View_Items_Form_Action_On_Click_Enum.Open_On_Web ? 2 : 1);
            }
        }

        /// <summary> Preference on the View Items form when a single item group row is double clicked </summary>
        public static View_Items_Form_Action_On_Click_Enum Item_Group_Form_Action_On_Click
        {
            get
            {
                int value = Get_Int_Setting("Item_Group_Form_Action_On_Click");
                switch (value)
                {
                    case 2:
                        return View_Items_Form_Action_On_Click_Enum.Open_On_Web;

                    default:
                        return View_Items_Form_Action_On_Click_Enum.Show_Form;
                }
            }
            set {
                Add_Setting("Item_Group_Form_Action_On_Click",
                            value == View_Items_Form_Action_On_Click_Enum.Open_On_Web ? 2 : 1);
            }
        }

        /// <summary> Preference on the Ad Hoc Report form when a single item group row is double clicked </summary>
        public static View_Items_Form_Action_On_Click_Enum Ad_Hoc_Form_Action_On_Click
        {
            get
            {
                int value = Get_Int_Setting("Ad_Hoc_Form_Action_On_Click");
                switch (value)
                {
                    case 2:
                        return View_Items_Form_Action_On_Click_Enum.Open_On_Web;

                    default:
                        return View_Items_Form_Action_On_Click_Enum.Show_Form;
                }
            }
            set {
                Add_Setting("Ad_Hoc_Form_Action_On_Click",
                            value == View_Items_Form_Action_On_Click_Enum.Open_On_Web ? 2 : 1);
            }
        }        

        /// <summary> Flag determines if the user is currently printing in landscape mode when printing a grid of different titles  </summary>
        public static bool Title_Grid_Print_Landscape
        {
            get
            {
                return Get_String_Setting("Print_Landscape") == "TRUE";
            }
            set { Add_Setting("Print_Landscape", value.ToString().ToUpper()); }
        }

        /// <summary> Flag determines if the user is currently printing in landscape mode when printing a grid of individual items within a title </summary>
        public static bool Item_Grid_Print_Landscape
        {
            get {
                return Get_String_Setting("Item_Grid_Print_Landscape") == "TRUE";
            }
            set { Add_Setting("Item_Grid_Print_Landscape", value.ToString().ToUpper()); }
        }

        /// <summary> Last size of the item discovery form </summary>
        public static Size Item_Discovery_Form_Size
        {
            set
            {
                Add_Setting("Item_Discovery_Form_Height", Math.Max(value.Height - 20, 50));
                Add_Setting("Item_Discovery_Form_Width", value.Width);
            }
            get
            {

                string heightString = Get_String_Setting("Item_Discovery_Form_Height");
                string widthString = Get_String_Setting("Item_Discovery_Form_Width");

                int height = -1;
                if (heightString.Length > 0)
                    height = Convert.ToInt32(heightString);
                int width = -1;
                if (widthString.Length > 0)
                    width = Convert.ToInt32(widthString);
                if ((height > 100) && (width > 100))
                    return new Size(width, height);
                
                // else
                return new Size(800, 600);
            }
        }

        /// <summary> Gets a flag indicating if the item discovery form was last maximized </summary>
        public static bool Item_Discovery_Form_Maximized
        {
            get {
                return Get_Int_Setting("Item_Discovery_Form_Maximized") == 1;
            }
            set { Add_Setting("Item_Discovery_Form_Maximized", value ? 1 : 0); }
        }

        /// <summary> Last size of the view item group form </summary>
        public static Size View_Item_Group_Form_Size
        {
            set
            {
                Add_Setting("View_Item_Group_Form_Height", Math.Max(value.Height - 20, 50));
                Add_Setting("View_Item_Group_Form_Width", value.Width);
            }
            get
            {
                string heightString = Get_String_Setting("View_Item_Group_Form_Height");
                string widthString = Get_String_Setting("View_Item_Group_Form_Width");

                int height = -1;
                if (heightString.Length > 0)
                    height = Convert.ToInt32(heightString);
                int width = -1;
                if (widthString.Length > 0)
                    width = Convert.ToInt32(widthString);
                if ((height > 100) && (width > 100))
                    return new Size(width, height);
                
                // else
                return new Size(743, 580);
            }
        }

        /// <summary> Gets a flag indicating if the view item group form was last maximized </summary>
        public static bool View_Item_Group_Form_Maximized
        {
            get {
                return Get_Int_Setting("View_Item_Group_Form_Maximized") == 1;
            }
            set { Add_Setting("View_Item_Group_Form_Maximized", value ? 1 : 0); }
        }

        /// <summary> Last size of the view items form </summary>
        public static Size View_Item_Form_Size
        {
            set
            {
                Add_Setting("View_Item_Form_Height", Math.Max(value.Height - 20, 50));
                Add_Setting("View_Item_Form_Width", value.Width);
            }
            get
            {

                string heightString = Get_String_Setting("View_Item_Form_Height");
                string widthString = Get_String_Setting("View_Item_Form_Width");

                int height = -1;
                if (heightString.Length > 0)
                    height = Convert.ToInt32(heightString);
                int width = -1;
                if (widthString.Length > 0)
                    width = Convert.ToInt32(widthString);
                if ((height > 100) && (width > 100))
                    return new Size(width, height);

                // else..
                return new Size(800, 600);
            }
        }

        /// <summary> Gets a flag indicating if the view items form was last maximized </summary>
        public static bool View_Item_Form_Maximized
        {
            get {
                return Get_Int_Setting("View_Item_Form_Maximized") == 1;
            }
            set { Add_Setting("View_Item_Form_Maximized", value ? 1 : 0); }
        }

        /// <summary> Last size of the edit serial hierarchy form </summary>
        public static Size Edit_Hierarchy_Form_Size
        {
            set
            {
                Add_Setting("Edit_Serial_Hierarchy_Form_Height", Math.Max(value.Height - 20, 50));
                Add_Setting("Edit_Serial_Hierarchy_Form_Width", value.Width);
            }
            get
            {
                string heightString = Get_String_Setting("Edit_Serial_Hierarchy_Form_Height");
                string widthString = Get_String_Setting("Edit_Serial_Hierarchy_Form_Width");

                int height = -1;
                if ( heightString.Length > 0 )
                    height = Convert.ToInt32(heightString);
                int width = -1;
                if ( widthString.Length > 0 )
                    width = Convert.ToInt32(widthString);
                if ((height > 100) && (width > 100))
                    return new Size(width, height);

                // else...
                return new Size(720, 500);
            }
        }

        /// <summary> Gets a flag indicating if the edit serial hierarchy form was last maximized </summary>
        public static bool Edit_Hierarchy_Form_Maximized
        {
            get {
                return Get_Int_Setting("Edit_Serial_Hierarchy_Form_Maximized") == 1;
            }
            set { Add_Setting("Edit_Serial_Hierarchy_Form_Maximized", value ? 1 : 0); }
        }

        /// <summary> Last size of the ad hoc report display form </summary>
        public static Size Ad_Hoc_Report_Form_Size
        {
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Height", Math.Max(value.Height - 20, 50));
                Add_Setting("Ad_Hoc_Report_Form_Width", value.Width);
            }
            get
            {
                string heightString = Get_String_Setting("Ad_Hoc_Report_Form_Height");
                string widthString = Get_String_Setting("Ad_Hoc_Report_Form_Width");

                int height = -1;
                if (heightString.Length > 0)
                    height = Convert.ToInt32(heightString);
                int width = -1;
                if (widthString.Length > 0)
                    width = Convert.ToInt32(widthString);
                if ((height > 100) && (width > 100))
                    return new Size(width, height);

                // else...
                return new Size(706, 484);
            }
        }

        /// <summary> Gets a flag indicating if the ad hoc report display form was last maximized </summary>
        public static bool Ad_Hoc_Report_Form_Maximized
        {
            get {
                return Get_Int_Setting("Ad_Hoc_Report_Form_Maximized") == 1;
            }
            set { Add_Setting("Ad_Hoc_Report_Form_Maximized", value ? 1 : 0); }
        }

        /// <summary> Last size of the tracking box report form </summary>
        public static Size Tracking_Box_Report_Form_Size
        {
            set
            {
                Add_Setting("Tracking_Box_Report_Form_Height", Math.Max(value.Height - 20, 50));
                Add_Setting("Tracking_Box_Report_Form_Width", value.Width);
            }
            get
            {
                string heightString = Get_String_Setting("Tracking_Box_Report_Form_Height");
                string widthString = Get_String_Setting("Tracking_Box_Report_Form_Width");

                int height = -1;
                if (heightString.Length > 0)
                    height = Convert.ToInt32(heightString);
                int width = -1;
                if (widthString.Length > 0)
                    width = Convert.ToInt32(widthString);
                if ((height > 100) && (width > 100))
                    return new Size(width, height);

                // else..
                return new Size(550, 660);
            }
        }

        /// <summary> Gets a flag indicating if the tracking box report form was last maximized </summary>
        public static bool Tracking_Box_Report_Form_Maximized
        {
            get {
                return Get_Int_Setting("Tracking_Box_Report_Form_Maximized") == 1;
            }
            set { Add_Setting("Tracking_Box_Report_Form_Maximized", value ? 1 : 0); }
        }

        /// <summary> Indicates the user's preferred user name, used when retrieving items from archive </summary>
        public static string User_Name
        {
            get
            {
                return Get_String_Setting("User_Name");
            }
            set
            {
                Add_Setting("User_Name", value);
            }
        }

        /// <summary> Indicates the user's preferred email address, used when retrieving items from archive </summary>
        public static string Email_Address
        {
            get
            {
                return Get_String_Setting("Email_Address");
            }
            set
            {
                Add_Setting("Email_Address", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the TITLE field on the results
        /// grid in the main discovery form </summary>
        public static int Discovery_Form_Title_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Discovery_Form_Title_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 350;
            }
            set
            {
                Add_Setting("Discovery_Form_Title_Width", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the AUTHOR field on the results
        /// grid in the main discovery form </summary>
        public static int Discovery_Form_Author_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Discovery_Form_Author_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 350;
            }
            set
            {
                Add_Setting("Discovery_Form_Author_Width", value);
            }
        }

        
        /// <summary> Gets the last width selected by the user for the PUBLISHER field on the results
        /// grid in the main discovery form </summary>
        public static int Discovery_Form_Publisher_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Discovery_Form_Publisher_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 350;
            }
            set
            {
                Add_Setting("Discovery_Form_Publisher_Width", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the TITLE field on the item
        /// grid in the view item group form </summary>
        public static int View_Item_Group_Form_Title_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("View_Item_Group_Form_Title_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 350;
            }
            set
            {
                Add_Setting("View_Item_Group_Form_Title_Width", value);
            }
        }       
      
        /// <summary> Gets the last width selected by the user for the AUTHOR field on the item
        /// grid in the view item group form </summary>
        public static int View_Item_Group_Form_Author_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("View_Item_Group_Form_Author_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 100;
            }
            set
            {
                Add_Setting("View_Item_Group_Form_Author_Width", value);
            }
        }   

        /// <summary> Gets the last width selected by the user for the PUBLISHER field on the item
        /// grid in the view item group form </summary>
        public static int View_Item_Group_Form_Publisher_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("View_Item_Group_Form_Publisher_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 100;
            }
            set
            {
                Add_Setting("View_Item_Group_Form_Publisher_Width", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the PUBLICATION DATE field on the item
        /// grid in the view item group form </summary>
        public static int View_Item_Group_Form_Date_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("View_Item_Group_Form_Date_Width");
                return possible_value > 0 ? Math.Max(possible_value, 5) :75;
            }
            set
            {
                Add_Setting("View_Item_Group_Form_Date_Width", value);
            }
        }  

        /// <summary> Gets the last width selected by the user for the SERIAL HIERARCHY LEVEL 1 field on the item
        /// grid in the view item group form </summary>
        public static int View_Item_Group_Form_Level1_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("View_Item_Group_Form_Level1_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 150;
            }
            set
            {
                Add_Setting("View_Item_Group_Form_Level1_Width", value);
            }
        }  

        /// <summary> Gets the last width selected by the user for the SERIAL HIERARCHY LEVEL 2 field on the item
        /// grid in the view item group form </summary>
        public static int View_Item_Group_Form_Level2_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("View_Item_Group_Form_Level2_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 150;
            }
            set
            {
                Add_Setting("View_Item_Group_Form_Level2_Width", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the SERIAL HIERARCHY LEVEL 3 field on the item
        /// grid in the view item group form </summary>
        public static int View_Item_Group_Form_Level3_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("View_Item_Group_Form_Level3_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 150;
            }
            set
            {
                Add_Setting("View_Item_Group_Form_Level3_Width", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the TITLE field on the item
        /// grid in the ad hoc report form </summary>
        public static int Ad_Hoc_Report_Form_Title_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Ad_Hoc_Report_Form_Title_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 350;
            }
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Title_Width", value);
            }
        }       
      
        /// <summary> Gets the last width selected by the user for the AUTHOR field on the item
        /// grid in the ad hoc report form </summary>
        public static int Ad_Hoc_Report_Form_Author_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Ad_Hoc_Report_Form_Author_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 100;
            }
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Author_Width", value);
            }
        }   

        /// <summary> Gets the last width selected by the user for the PUBLISHER field on the item
        /// grid in the ad hoc report form </summary>
        public static int Ad_Hoc_Report_Form_Publisher_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Ad_Hoc_Report_Form_Publisher_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 100;
            }
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Publisher_Width", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the PUBLICATION DATE field on the item
        /// grid in the ad hoc report form </summary>
        public static int Ad_Hoc_Report_Form_Date_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Ad_Hoc_Report_Form_Date_Width");
                return possible_value > 0 ? Math.Max(possible_value, 5) : 75;
            }
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Date_Width", value);
            }
        }  

        /// <summary> Gets the last width selected by the user for the SERIAL HIERARCHY LEVEL 1 field on the item
        /// grid in the ad hoc report form </summary>
        public static int Ad_Hoc_Report_Form_Level1_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Ad_Hoc_Report_Form_Level1_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 150;
            }
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Level1_Width", value);
            }
        }  

        /// <summary> Gets the last width selected by the user for the SERIAL HIERARCHY LEVEL 2 field on the item
        /// grid in the ad hoc report form </summary>
        public static int Ad_Hoc_Report_Form_Level2_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Ad_Hoc_Report_Form_Level2_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 150;
            }
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Level2_Width", value);
            }
        }

        /// <summary> Gets the last width selected by the user for the SERIAL HIERARCHY LEVEL 3 field on the item
        /// grid in the ad hoc report form </summary>
        public static int Ad_Hoc_Report_Form_Level3_Width
        {
            get
            {
                int possible_value = Get_Int_Setting("Ad_Hoc_Report_Form_Level3_Width");
                return possible_value > 0 ? Math.Max( possible_value, 5 ) : 150;
            }
            set
            {
                Add_Setting("Ad_Hoc_Report_Form_Level3_Width", value);
            }
        }

        /// <summary> Gets the last search term for the first of the search boxes in the discovery panel  </summary>
        public static SobekCM_Search_Object.SobekCM_Term_Enum Discovery_Panel_Search_Term1
        {
            get
            {
                int possible_value = Get_Int_Setting("Discovery_Panel_Search_Term1");
                if ( possible_value >= 0 )
                    return ( SobekCM_Search_Object.SobekCM_Term_Enum) possible_value;

                // else...
                return SobekCM_Search_Object.SobekCM_Term_Enum.Anywhere;
            }
            set
            {
                Add_Setting("Discovery_Panel_Search_Term1", (int) value);
            }
        }

        /// <summary> Gets the last search term for the second of the search boxes in the discovery panel  </summary>
        public static SobekCM_Search_Object.SobekCM_Term_Enum Discovery_Panel_Search_Term2
        {
            get
            {
                int possible_value = Get_Int_Setting("Discovery_Panel_Search_Term2");
                if ( possible_value >= 0 )
                    return ( SobekCM_Search_Object.SobekCM_Term_Enum) possible_value;
                
                // else...
                return SobekCM_Search_Object.SobekCM_Term_Enum.BibID;
            }
            set
            {
                Add_Setting("Discovery_Panel_Search_Term2", (int) value);
            }
        }

        /// <summary> Gets the last search term for the third of the search boxes in the discovery panel  </summary>
        public static SobekCM_Search_Object.SobekCM_Term_Enum Discovery_Panel_Search_Term3
        {
            get
            {
                int possible_value = Get_Int_Setting("Discovery_Panel_Search_Term3");
                if ( possible_value >= 0 )
                    return ( SobekCM_Search_Object.SobekCM_Term_Enum) possible_value;
                // else..
                return SobekCM_Search_Object.SobekCM_Term_Enum.Title;
            }
            set
            {
                Add_Setting("Discovery_Panel_Search_Term3", (int) value);
            }
        }

        /// <summary> Gets the last search term for the fourth of the search boxes in the discovery panel  </summary>
        public static SobekCM_Search_Object.SobekCM_Term_Enum Discovery_Panel_Search_Term4
        {
            get
            {
                int possible_value = Get_Int_Setting("Discovery_Panel_Search_Term4");
                if ( possible_value >= 0 )
                    return ( SobekCM_Search_Object.SobekCM_Term_Enum) possible_value;
                // else...
                return SobekCM_Search_Object.SobekCM_Term_Enum.Tracking_Box;
            }
            set
            {
                Add_Setting("Discovery_Panel_Search_Term4", (int) value);
            }
        }

        /// <summary> Load the individual user settings </summary>
        public static void Load()
        {
            // Try to read the XML file from isolated storage
            Read_XML_File(fileName);


            // Make sure this contains one setting. If not, set them to default
            if (Get_Int_Setting("Item_Discovery_Form_Action_On_Click") == -1)
            {
                // Add the defaults
                Add_Setting("Item_Discovery_Form_Action_On_Click", 1);
                Add_Setting("Item_Group_Form_Action_On_Click", 1);
                Add_Setting("Print_Landscape", "TRUE");
                Add_Setting("Item_Grid_Print_Landscape", "FALSE");
                Add_Setting("Search_Precision_Type", 1);
                Add_Setting("Item_Discovery_Form_Height", 600);
                Add_Setting("Item_Discovery_Form_Width", 800);
                Add_Setting("Item_Discovery_Form_Maximized", 0);
                Add_Setting("View_Item_Group_Form_Height", 580);
                Add_Setting("View_Item_Group_Form_Width", 743);
                Add_Setting("View_Item_Group_Form_Maximized", 0);
                Add_Setting("View_Item_Form_Height", 580);
                Add_Setting("View_Item_Form_Width", 743);
                Add_Setting("View_Item_Form_Maximized", 0);
                Add_Setting("Edit_Serial_Hierarchy_Form_Height", 500);
                Add_Setting("Edit_Serial_Hierarchy_Form_Width", 720);
                Add_Setting("Edit_Serial_Hierarchy_Form_Maximized", 0);
                Add_Setting("Ad_Hoc_Report_Form_Height", 484);
                Add_Setting("Ad_Hoc_Report_Form_Width", 706);
                Add_Setting("Ad_Hoc_Report_Form_Maximized", 0);
                Add_Setting("Tracking_Box_Report_Form_Height", 550);
                Add_Setting("Tracking_Box_Report_Form_Width", 660);
                Add_Setting("Tracking_Box_Report_Form_Maximized", 0);
                Add_Setting("Single_Result_Discovery_Setting", 1 );
                Add_Setting("Discovery_Form_Title_Width", 350);
                Add_Setting("Discovery_Form_Author_Width", 350);
                Add_Setting("Discovery_Form_Publisher_Width", 350);
                Add_Setting("View_Item_Group_Form_Title_Width", 350);
                Add_Setting("View_Item_Group_Form_Author_Width", 100);
                Add_Setting("View_Item_Group_Form_Publisher_Width", 100);
                Add_Setting("View_Item_Group_Form_Level1_Width", 150);
                Add_Setting("View_Item_Group_Form_Level2_Width", 150);
                Add_Setting("View_Item_Group_Form_Level3_Width", 150);
                Add_Setting("Ad_Hoc_Report_Form_Title_Width", 350);
                Add_Setting("Ad_Hoc_Report_Form_Author_Width", 100);
                Add_Setting("Ad_Hoc_Report_Form_Publisher_Width", 100);
                Add_Setting("Ad_Hoc_Report_Form_Level1_Width", 150);
                Add_Setting("Ad_Hoc_Report_Form_Level2_Width", 150);
                Add_Setting("Ad_Hoc_Report_Form_Level3_Width", 150);
                Add_Setting("Discovery_Panel_Search_Term1", 2);
                Add_Setting("Discovery_Panel_Search_Term2", 5);
                Add_Setting("Discovery_Panel_Search_Term3", 23);
                Add_Setting("Discovery_Panel_Search_Term4", 24);
                Add_Setting("Settings_Version", "1.0");
                Save();        
            }
        }

        /// <summary> Save the individual user settings </summary>
        public static void Save()
        {
            // Ask the base class to save the data
            Write_XML_File(fileName);
        }
    }
}

