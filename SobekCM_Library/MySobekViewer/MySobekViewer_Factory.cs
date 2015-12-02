using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.Items;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.Navigation;
using SobekCM.Engine_Library.Database;
using SobekCM.Library.AdminViewer;

namespace SobekCM.Library.MySobekViewer
{
    /// <summary> Factory class returns the appropriate mySobek viewer </summary>
    public static class MySobekViewer_Factory
    {
        /// <summary> Returns the appropriate mySobek viewer, based on requst and system settings </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request  </param>
        /// <returns> Built mySobek viewer </returns>
        public static iMySobek_Admin_Viewer Get_MySobekViewer(RequestCache RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("MySobekViewer_Factory.Get_MySobekViewer", "Building the mySobek viewer object");

            switch (RequestSpecificValues.Current_Mode.My_Sobek_Type)
            {
                case My_Sobek_Type_Enum.Home:
                    return new Home_MySobekViewer(RequestSpecificValues);
 
                case My_Sobek_Type_Enum.New_Item:
                    return new New_Group_And_Item_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Folder_Management:
                    return new Folder_Mgmt_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Saved_Searches:
                    return new Saved_Searches_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Preferences:
                    return new Preferences_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Logon:
                    return new Logon_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.New_Password:
                    return new NewPassword_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Delete_Item:
                    return new Delete_Item_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Edit_Item_Behaviors:
                    return new Edit_Item_Behaviors_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Edit_Item_Metadata:
                    return new Edit_Item_Metadata_MySobekViewer(RequestSpecificValues.Current_Item, RequestSpecificValues);

                case My_Sobek_Type_Enum.Edit_Item_Permissions:
                    return new Edit_Item_Permissions_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.File_Management:
                    return new File_Management_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Edit_Group_Behaviors:
                    return new Edit_Group_Behaviors_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Edit_Group_Serial_Hierarchy:
                    return new Edit_Serial_Hierarchy_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Item_Tracking:
                    return new Track_Item_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Group_Add_Volume:
                    return new Group_Add_Volume_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Group_AutoFill_Volumes:
                    return new Group_AutoFill_Volume_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Group_Mass_Update_Items:
                    return new Mass_Update_Items_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.Page_Images_Management:
                    return new Page_Image_Upload_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.User_Tags:
                    return new User_Tags_MySobekViewer(RequestSpecificValues);

                case My_Sobek_Type_Enum.User_Usage_Stats:
                    return new User_Usage_Stats_MySobekViewer(RequestSpecificValues);
            }

            return null;
        }
    }
}
