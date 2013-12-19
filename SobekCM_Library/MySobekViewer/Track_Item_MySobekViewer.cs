#region Using directives
using System;
using System.Data;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
using System.Collections.Generic;
using System.Data.SqlTypes;
#endregion

namespace SobekCM.Library.MySobekViewer
{
    class Track_Item_MySobekViewer : abstract_MySobekViewer
    {
       private Dictionary<string, User_Object> user_list;
        private List<string> scanners_list;
        private  string barcodeString;
        private  int itemID;
        private string encodedItemID;
        private string checksum;
        private string BibID;
        private string VID;
        private string error_message = String.Empty;
        private int stage=1;
        private string hidden_request ;
        private string hidden_value;
        private string title;
        private string equipment;
        private int current_workflow_id = -1;

        private string start_Time;
        private string end_Time;
        private bool close_error = false;
        private DateTime this_workflow_date;

        private DataTable tracking_users;
        private DataTable open_workflows_from_DB;
      
        private Dictionary<string,Tracking_Workflow> current_workflows;
        private Dictionary<string, Tracking_Workflow> current_workflows_no_durations;
        
        private User_Object current_selected_user;

        private readonly int page;

        /// <summary> Constructor for a new instance of the Track_Item_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Track_Item_MySobekViewer(User_Object User, SobekCM_Navigation_Object Current_Mode, Custom_Tracer Tracer) 
            :  base(User)
          {
                    Tracer.Add_Trace("Track_Item_MySobekViewer.Constructor", String.Empty);

                    currentMode = Current_Mode;
                    

                     //If there is no user, go back
                    if (user == null)
                    {
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                        HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                    }

     
            //Initialize variables
            tracking_users = new DataTable();
            user_list = new Dictionary<string, User_Object>();
            scanners_list = new List<string>();

            //Determine the page
            page = 1;

            if ((HttpContext.Current.Session["Selected_Tab"] != null) && !(String.IsNullOrEmpty(HttpContext.Current.Session["Selected_Tab"].ToString())) && HttpContext.Current.Session["Selected_Tab"] == "2")
                page = 2;

            string sub_page = HttpContext.Current.Request.Form["tracking_new_page"] ?? "";
            if (sub_page == "2")
            {
                page = 2;
                HttpContext.Current.Session["Selected_Tab"] = "2";
            }
            else if (sub_page == "1")
            {
                page = 1;
                HttpContext.Current.Session["Selected_Tab"] = "1";
            }

            
            //Get the list of users who are possible Scanning/Processing technicians from the DB
            tracking_users = Database.SobekCM_Database.Tracking_Get_Users_Scanning_Processing();
            
            foreach (DataRow row in tracking_users.Rows)
            {
                User_Object temp_user = new User_Object();
                temp_user.UserName = row["UserName"].ToString();
                temp_user.Given_Name = row["FirstName"].ToString();
                temp_user.Family_Name = row["LastName"].ToString();
                temp_user.Email = row["EmailAddress"].ToString();
                user_list.Add(temp_user.UserName, temp_user);
            }
           
            if(!user_list.ContainsKey(User.UserName))
                user_list.Add(User.UserName, User);

            //Get the list of scanning equipment
            DataTable scanners = new DataTable();
            scanners = Database.SobekCM_Database.Tracking_Get_Scanners_List();
            foreach (DataRow row in scanners.Rows)
            {
                scanners_list.Add(row["ScanningEquipment"].ToString());
            }

            //See if there were any hidden requests
            hidden_request = HttpContext.Current.Request.Form["Track_Item_behaviors_request"] ?? String.Empty;
            hidden_value = HttpContext.Current.Request.Form["Track_Item_hidden_value"] ?? String.Empty;


            //Get the equipment value
            if (HttpContext.Current.Session["Equipment"]!=null && !String.IsNullOrEmpty(HttpContext.Current.Session["Equipment"].ToString()))
                equipment = HttpContext.Current.Session["Equipment"].ToString();

            else
            {
                equipment = scanners_list[0];
                HttpContext.Current.Session["Equipment"] = equipment;
            }
            
            //Check the hidden value to see if equipment was previously changed
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["hidden_equipment"]))
            {
                equipment = HttpContext.Current.Request.Form["hidden_equipment"];
                HttpContext.Current.Session["equipment"] = equipment;
            }


            //Also get the currently selected user
            if (HttpContext.Current.Session["Selected_User"] != null)
                current_selected_user = (User_Object)HttpContext.Current.Session["Selected_User"];

            else
            {
                current_selected_user = User;
                HttpContext.Current.Session["Selected_User"] = current_selected_user;
            }
            
            //Check if the selected user has been changed
            if (!String.IsNullOrEmpty(HttpContext.Current.Request.Form["hidden_selected_username"]) )
            {
               current_selected_user = new User_Object();
                string temp = HttpContext.Current.Request.Form["hidden_selected_username"];
                current_selected_user = user_list[temp];

                HttpContext.Current.Session["Selected_User"] = current_selected_user;
            }

            //Fetch the dictionaries of current work from the session
            current_workflows = (HttpContext.Current.Session["Tracking_Current_Workflows"]) as Dictionary<string, Tracking_Workflow>;
            current_workflows_no_durations = (HttpContext.Current.Session["Tracking_Current_Workflows_No_Duration"]) as Dictionary<string, Tracking_Workflow>;

            //else create new ones
            if (current_workflows == null && page == 1)
            {
                current_workflows = new Dictionary<string, Tracking_Workflow>();
            }
            else if (current_workflows_no_durations == null && page == 2)
            {
                current_workflows_no_durations = new Dictionary<string, Tracking_Workflow>();
            }
                
            //If there is a valid item currently selected
            if (!String.IsNullOrEmpty(BibID) && !String.IsNullOrEmpty(VID))
            {
                if (page == 1)
                {
                    //Get the the form field values from the first tab
                    start_Time = Convert.ToDateTime(HttpContext.Current.Request.Form["txtStartTime"]).ToString("HH:mm");
                    end_Time = Convert.ToDateTime(HttpContext.Current.Request.Form["txtEndTime"]).ToString("HH:mm");
                    this_workflow_date = Convert.ToDateTime(HttpContext.Current.Request.Form["txtStartDate"]);
                }
                else if (page == 2)
                {
                    //Get the form values from the second tab
                    this_workflow_date = Convert.ToDateTime(HttpContext.Current.Request.Form["txtStartDate2"]);
                }
            }

            
            switch (hidden_request.ToLower())
            {
                case "decode_barcode":
                    barcodeString = hidden_value;
                    //Decode the scanned barcode 
                    if (!String.IsNullOrEmpty(barcodeString))
                    {
                        //Find a match for a number within the string, which corresponds to the event
                        Match val = Regex.Match(barcodeString, @"\d");
                        if (val.Success)
                        {
                            int len = barcodeString.IndexOf(val.Value);
                            Int32.TryParse(val.Value, out stage);

                            //Extract the item ID & checksum from the barcode string
                            encodedItemID = barcodeString.Substring(0, len);
                            checksum = barcodeString.Substring(len + 1, (barcodeString.Length - len - 1));

                            //Verify that the checksum is valid
                            bool isValidChecksum = Is_Valid_Checksum(encodedItemID, val.Value, checksum);
                            if (!isValidChecksum)
                                error_message = "Barcode error- checksum error";

                            //Save the item information for this itemID
                            bool IsValidItem = Get_Item_Info_from_Barcode(encodedItemID);
                            if (!IsValidItem)
                                error_message = "Barcode error - invalid ItemID referenced";
                            else
                            {
                                Get_Bib_VID_from_ItemID(itemID);
                            }

                        }
                        else
                        {
                            error_message = "Invalid barcode scanned!";
                        }
                    }
                    break;

                case "read_manual_entry":
                    //Get the related hidden values for the selected manual entry fields
                    string hidden_bibID = HttpContext.Current.Request.Form["hidden_BibID"] ?? String.Empty;
                    string hidden_VID =  HttpContext.Current.Request.Form["hidden_VID"] ?? String.Empty;
                    string hidden_event_num = HttpContext.Current.Request.Form["hidden_event_num"] ?? String.Empty;
                    if (String.IsNullOrEmpty(hidden_bibID) || String.IsNullOrEmpty(hidden_VID) || String.IsNullOrEmpty(hidden_event_num))
                    {
                        error_message = "You must enter a BibID and a VID!";
                    }
                    else
                    {
                        Int32.TryParse(hidden_event_num, out stage);
                        BibID = hidden_bibID;
                        VID = hidden_VID;
                        try
                        {
                            itemID = Resource_Object.Database.SobekCM_Database.Get_ItemID(BibID, VID);
                            Get_Bib_VID_from_ItemID(itemID);
                        }
                        catch (Exception ee)
                        {
                            error_message = "Invalid BibID or VID!";
                        }


                    }
                    break;


                case "save":
                    int thisWorkflowId = Convert.ToInt32(HttpContext.Current.Request.Form["Track_Item_hidden_value"]);
                    int hidden_itemID = Convert.ToInt32(HttpContext.Current.Request.Form["hidden_itemID"]);
                    current_workflow_id = thisWorkflowId;
                    itemID = hidden_itemID;
                    Get_Bib_VID_from_ItemID(hidden_itemID);
                    Add_or_Update_Workflow(thisWorkflowId, hidden_itemID);
               
                    break;

                case "delete":
                    int WorkflowId = Convert.ToInt32(HttpContext.Current.Request.Form["Track_Item_hidden_value"]);
                    current_workflow_id = WorkflowId;
                    Database.SobekCM_Database.Tracking_Delete_Workflow(WorkflowId);
                    break;
                    
                default:
                    break;
               }

            //Get the table of any previously opened workflows for this item
            if (!String.IsNullOrEmpty(itemID.ToString()) && itemID != 0 && page==1)
            {
                    DataView temp_open_workflows_all_users = new DataView(Database.SobekCM_Database.Tracking_Get_Open_Workflows(itemID,stage));

                    //Filter the open workflows associated with the currently selected user
                    open_workflows_from_DB = temp_open_workflows_all_users.ToTable().Clone();

                    foreach (DataRowView rowView in temp_open_workflows_all_users)
                    {
                        DataRow newRow = open_workflows_from_DB.NewRow();
                        newRow.ItemArray = rowView.Row.ItemArray;
                        string username_column = rowView["WorkPerformedBy"].ToString();
                        string start_event_column = rowView["Start_Event_Number"].ToString();
                        string current_start = (stage == 1 || stage == 2) ? "1" : "3";
                        if (username_column == current_selected_user.UserName && !(String.IsNullOrEmpty(start_event_column)) && (start_event_column==current_start) )
                            open_workflows_from_DB.Rows.Add(newRow);
                    }
                HttpContext.Current.Session["Open_Workflows"] = open_workflows_from_DB;

            }
            else if (!String.IsNullOrEmpty(itemID.ToString()) && itemID != 0 && page==2)
            {
                open_workflows_from_DB = HttpContext.Current.Session["Open_Workflows"] as DataTable;
            }


            //If a valid Bib, VID workflow was entered, add this to the current session
            if (String.IsNullOrEmpty(error_message) && !String.IsNullOrEmpty(BibID) && !String.IsNullOrEmpty(VID) && hidden_request!="save")
            {
                Add_New_Workflow();
            }

          }


        /// <summary> Save or Update Workflow when a 'Save' button for a tracking entry is clicked </summary>
        /// <param name="thisWorkflowId"></param>
        private void Add_or_Update_Workflow(int thisWorkflowId, int thisItemID)
        {
                   string thisWorkflowId_string = thisWorkflowId.ToString();
                    string new_start_time="";
                    string new_end_time="";
                    DateTime new_date;
                    int selected_ddl_workflow;
                    int this_event = -1;
                    int start_event_num=-1;
                    int end_event_num=-1;
                    int start_end_event_num=-1;

                    if (thisWorkflowId == -1)
                    {
                        thisWorkflowId_string = "-1";
                    }
                    if (page == 1)
                    {
                        new_date = Convert.ToDateTime(HttpContext.Current.Request.Form["txtStartDate" + thisWorkflowId_string]);
                        selected_ddl_workflow = Convert.ToInt32(HttpContext.Current.Request.Form["ddlEvent" + thisWorkflowId_string]);
                    }
                    else
                    {
                        new_date = Convert.ToDateTime(HttpContext.Current.Request.Form["txtStartDate2"]);
                        selected_ddl_workflow = Convert.ToInt32(HttpContext.Current.Request.Form["ddlEvent2"]);
                    }
                  
                    if (page == 1)
                    {
                        new_start_time = String.IsNullOrEmpty(HttpContext.Current.Request.Form["txtStartTime" + thisWorkflowId_string]) ? "" :Convert.ToDateTime(HttpContext.Current.Request.Form["txtStartTime" + thisWorkflowId_string]).ToString("HH:mm");
                        new_end_time = String.IsNullOrEmpty(HttpContext.Current.Request.Form["txtEndTime" + thisWorkflowId_string]) ? "" : Convert.ToDateTime(HttpContext.Current.Request.Form["txtEndTime" + thisWorkflowId_string]).ToString("HH:mm");
                            if (selected_ddl_workflow == 1)
                            {
                                this_event = 1;
                                start_event_num = 1;
                                if (!String.IsNullOrEmpty(new_end_time))
                                {
                                    end_event_num = 2;
                                    this_event = 2;
                                }
                            }
                            else
                            {
                                this_event = 3;
                                start_event_num = 3;
                                if (!String.IsNullOrEmpty(new_end_time))
                                {
                                    end_event_num = 4;
                                    this_event = 4;
                                }
                            }
                        }

                         //If this is from the second tab, set the appropriate start_end_event number
                        else
                        {
                            new_start_time = new_date.ToString("HH:mm");
                            new_end_time = new_date.ToString("HH:mm");
                            if (selected_ddl_workflow == 1)
                            {
                                this_event = 2;
                                start_end_event_num = 2;
                            }
                            else
                            {
                                this_event = 4;
                                start_end_event_num = 4;
                            }
                        }

                     //Create a new workflow object for this workflow
                            Tracking_Workflow this_workflow = new Tracking_Workflow();
                            SqlDateTime start_time_to_save = DateTime.Parse(new_date.ToShortDateString() + " " + new_start_time);
                            SqlDateTime end_time_to_save = String.IsNullOrEmpty(new_end_time) ? SqlDateTime.Null : DateTime.Parse(new_date.ToShortDateString() + " " + new_end_time);
                            if (page == 2)
                                end_time_to_save = start_time_to_save;

                            this_workflow.BibID = BibID;
                            this_workflow.VID = VID;
                            this_workflow.Equipment = equipment;
                            this_workflow.Date = new_date.ToString("yyyy-MM-dd");
                            this_workflow.Saved = true;
                            this_workflow.Title = title;
                            this_workflow.StartTime = Convert.ToDateTime(new_start_time).ToString("HH:mm");
                            if (page == 1)
                            {
                                this_workflow.EndTime = String.IsNullOrEmpty(new_end_time) ? DateTime.MinValue.ToShortTimeString() : Convert.ToDateTime(new_end_time).ToString("HH:mm");
                            }
                            else
                            {
                                this_workflow.EndTime = this_workflow.StartTime;
                            }
                            this_workflow.Workflow_type = this_event;
                            this_workflow.thisUser = current_selected_user;


                            //Fetch the workflow dictionaries from the session if present
                            current_workflows = (HttpContext.Current.Session["Tracking_Current_Workflows"]) as Dictionary<string, Tracking_Workflow>;
                            current_workflows_no_durations = (HttpContext.Current.Session["Tracking_Current_Workflows_No_Duration"]) as Dictionary<string, Tracking_Workflow>;

                            //else create new ones
                            if (current_workflows == null && page == 1)
                            {
                                current_workflows = new Dictionary<string, Tracking_Workflow>();
                            }
                            else if (current_workflows_no_durations == null && page == 2)
                            {
                                current_workflows_no_durations = new Dictionary<string, Tracking_Workflow>();
                            }



                    //If this has not been previously saved, create a new entry in the DB
                        if (thisWorkflowId == -1)
                        {
                            //If this is a 'Start' event, use the event number, since there can be at max one event start  per item per user per day
                            //else use the unique Guid. This ensures there can be any number of 'close' events for an item per user per day
                            string guidObj = Guid.NewGuid().ToString();

                            string stage_from_event = (page==1) ? ((this_event == 1 || this_event == 3) ? this_event.ToString() : guidObj) : guidObj;

                        //    string key = page + thisItemID + this_workflow.Date+stage_from_event + current_selected_user.UserName;
                            string key = current_workflow_id.ToString();

                            if (page == 1)
                            {
                                //Save this to the DB
                                this_workflow.WorkflowID = Database.SobekCM_Database.Tracking_Save_New_Workflow(thisItemID, current_selected_user.UserName, equipment, start_time_to_save, end_time_to_save, this_event, start_event_num, end_event_num, start_end_event_num);
                              
                                current_workflow_id = this_workflow.WorkflowID;
                                key = current_workflow_id.ToString();

                                if (current_workflows.ContainsKey("-1"))
                                    current_workflows.Remove("-1");

                                //Add this to the dictionary
                                current_workflows.Add(key, this_workflow);
                            }

                            else if (page == 2)
                            {

                                //Save this workflow to the database
                                this_workflow.WorkflowID = Database.SobekCM_Database.Tracking_Save_New_Workflow(thisItemID, current_selected_user.UserName, equipment, start_time_to_save, end_time_to_save, stage, start_event_num, end_event_num, start_end_event_num);

                                current_workflow_id = this_workflow.WorkflowID;
                                key = current_workflow_id.ToString();

                                if (current_workflows_no_durations.ContainsKey("-1"))
                                    current_workflows_no_durations.Remove("-1");

                                //Add this to the dictionary
                                current_workflows_no_durations.Add(key, this_workflow);

                            }

                  
                        }
                        //Else update this workflow in the database
                        else
                        {
                            string guidObj = Guid.NewGuid().ToString();
                            string stage_from_event = (page == 1) ? ((this_event == 1 || this_event == 3) ? this_event.ToString() : guidObj) : guidObj;
//                            string stage_from_event = (this_event == 1 || this_event == 3) ? this_event.ToString() : thisWorkflowId.ToString();

                            //string key = page + thisItemID + this_workflow.Date + stage_from_event + current_selected_user.UserName;
                            string key = thisWorkflowId.ToString();
                            current_workflow_id = thisWorkflowId;

                            Database.SobekCM_Database.Tracking_Update_Workflow(thisWorkflowId, thisItemID, current_selected_user.UserName, start_time_to_save, end_time_to_save, equipment, this_event, start_event_num, end_event_num);                            

                            if (page == 1)
                            {
                                if (this_event == 2 || this_event == 4)
                                {
                                    string opened_key_string = this_event == 2 ? "1" : "3";
                           //         string opened_key = page + thisItemID + this_workflow.Date + opened_key_string + current_selected_user.UserName;
                                    string opened_key = key;
      

                                    //Remove the old value from the dictionary if present
                                    if (current_workflows.ContainsKey(opened_key))
                                        current_workflows.Remove(opened_key);
                                }
                            

                                //Add this to the dictionary
                                current_workflows.Add(key, this_workflow);
                            }

                            else if (page == 2)
                            {

                                //Remove the old value from the dictionary if present
                                if (current_workflows_no_durations.ContainsKey(key))
                                    current_workflows_no_durations.Remove(key);

                                //Add this to the dictionary
                                current_workflows_no_durations.Add(key, this_workflow);

                            }

                          
                        }

                        //Save the dictionaries back to the session
                        HttpContext.Current.Session["Tracking_Current_Workflows"] = current_workflows;
                        HttpContext.Current.Session["Tracking_Current_Workflows_No_Duration"] = current_workflows_no_durations;
        }


       /// <summary>
        /// Add new workflow to the dictionary and the session of all current workflows
       /// </summary>
        private void Add_New_Workflow()
        {
           //Fetch the workflow dictionaries from the session if present
             current_workflows = (HttpContext.Current.Session["Tracking_Current_Workflows"]) as Dictionary<string, Tracking_Workflow>;
             current_workflows_no_durations = (HttpContext.Current.Session["Tracking_Current_Workflows_No_Duration"]) as Dictionary<string, Tracking_Workflow>;
            
            //else create new ones
            if (current_workflows == null && page==1)
            {
                current_workflows = new Dictionary<string, Tracking_Workflow>();
            }
            else if (current_workflows_no_durations == null && page == 2)
            {
                current_workflows_no_durations = new Dictionary<string, Tracking_Workflow>();
            }

            Tracking_Workflow this_workflow = new Tracking_Workflow();
            this_workflow.BibID = BibID;
            this_workflow.VID = VID;
            this_workflow.Equipment = equipment;
            this_workflow.thisUser = current_selected_user;
           

            //Add the date and time
            string currentTime = DateTime.Now.ToString("HH:mm");


            if (stage == 1 || stage == 3)
            {
                start_Time = currentTime;
                end_Time = null;
                
                //If there are previously opened workflows, return. We will not save this entry yet, till the previous ones are resolved 
                if ((page==1)&&!(open_workflows_from_DB == null || open_workflows_from_DB.Rows.Count == 0))
                    return;
            }
            else if ((stage == 2 || stage == 4) &&(page==1))
            {
               
                    start_Time = null;
                    end_Time = currentTime;

                  if ( open_workflows_from_DB != null && open_workflows_from_DB.Rows.Count > 0)
                    {
                        int count = 0;
                        //Look for the matching opened workflow to close
                        foreach (DataRow row in open_workflows_from_DB.Rows)
                        {
                            if (Convert.ToDateTime(row["DateStarted"]).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                            {
                                count++;
                                if (count > 1)
                                {
                                    error_message = "More than one unclosed workflow entry for today!";
                                    return;
                                }
                                start_Time = Convert.ToDateTime(row["DateStarted"]).ToString("HH:mm");
                                int temp;
                                Int32.TryParse(row["ProgressID"].ToString(), out temp);
                                this_workflow.WorkflowID = temp;

                            }
                        }
                      
                      //If there were no workflows opened today, do not save this
                      if (count == 0)
                        {
                            error_message = "No open workflow to close!";
                            close_error = true;
                            return;
                        }
                    }
                    else
                  {
                      close_error = true;
                        return;
                    }

              }

            this_workflow.StartTime = start_Time;
            this_workflow.EndTime = end_Time;
            this_workflow.Date = DateTime.Now.ToString("yyyy-MM-dd");
           
            this_workflow.itemID = itemID;
            this_workflow.Saved = false;
            this_workflow.Title = title;
            this_workflow.Workflow_type = stage;
           

            //Combine the times and dates to single SqlDateTime variables to save to the database
           SqlDateTime start_date_time = String.IsNullOrEmpty(start_Time) ? SqlDateTime.Null : DateTime.Parse(this_workflow.Date + " " + start_Time);
           SqlDateTime end_date_time = String.IsNullOrEmpty(end_Time) ? SqlDateTime.Null : DateTime.Parse(this_workflow.Date + " " + end_Time);
          
            //Determine the start, end & single point event numbers
            int start_event_num=-1;
           int end_event_num=-1;
           int start_end_event_num=-1;
           if (page == 1)
           {
               if (stage == 1 || stage == 3)
               {
                   start_event_num = stage;
                   end_event_num = -1;
                   start_end_event_num = -1;
               }
               else if (stage == 2)
               {
                   start_event_num = 1;
                   end_event_num = 2;
                   start_end_event_num = -1;
               }
               else if (stage == 4)
               {
                   start_event_num = 3;
                   end_event_num = 4;
                   start_end_event_num = -1;
               }
           }

           else if (page == 2)
           {
                  start_event_num = -1;
                   end_event_num = -1;
                   start_end_event_num = stage;
            }
           string guidObj =  Guid.NewGuid().ToString();


           string stage_from_event = (page==1)?((stage == 1 || stage == 3) ? stage.ToString() : guidObj.ToString()) : guidObj ;

        //   string key = page + itemID + this_workflow.Date+stage_from_event + current_selected_user.UserName;
           string key = current_workflow_id.ToString();
           
            if ((page==1) && !current_workflows.ContainsKey(key) && (stage==1 || stage==3))
            {
                //Save this workflow to the database
                current_workflow_id = Database.SobekCM_Database.Tracking_Save_New_Workflow(itemID, current_selected_user.UserName, equipment, start_date_time, end_date_time, stage, start_event_num, end_event_num, start_end_event_num);

                //Add the workflow id obtained after saving to the database 
                this_workflow.WorkflowID = current_workflow_id;

                key = current_workflow_id.ToString();
                
                //Add this to the dictionary
                 current_workflows.Add(key,this_workflow);
            }
            else if ((page == 1) && (stage == 2 || stage == 4))
            {
                //determine the key with which this workflow was opened
                string opened_key_event = (stage == 2) ? "1" : "3";
        //        string opened_key = page + itemID + this_workflow.Date + opened_key_event + current_selected_user.UserName;
                string opened_key = current_workflow_id.ToString();


                //Remove the old opened value from the dictionary if present
                if (current_workflows.ContainsKey(opened_key))
                    current_workflows.Remove(opened_key);

                //Update this data in the database
                Database.SobekCM_Database.Tracking_Update_Workflow(this_workflow.WorkflowID, itemID, current_selected_user.UserName, start_date_time, end_date_time, equipment, stage, start_event_num, end_event_num);

                //Add the new row to the dictionary with the updated "end" information
                current_workflows.Add(key,this_workflow);
            }

            if ((page == 2) && !current_workflows_no_durations.ContainsKey(key))
            {
               if (stage == 1 || stage == 3)
                    end_date_time = start_date_time;
                else start_date_time = end_date_time;

                //Save this workflow to the database
                current_workflow_id = Database.SobekCM_Database.Tracking_Save_New_Workflow(itemID, current_selected_user.UserName, equipment, start_date_time, end_date_time, stage, start_event_num, end_event_num, start_end_event_num);

                //Add the workflow id obtained after saving to the database 
                this_workflow.WorkflowID = current_workflow_id;

                key = current_workflow_id.ToString();

                //Add this to the dictionary
                current_workflows_no_durations.Add(key, this_workflow);
            }

            //Save the dictionaries back to the session
               HttpContext.Current.Session["Tracking_Current_Workflows"] = current_workflows;
               HttpContext.Current.Session["Tracking_Current_Workflows_No_Duration"] = current_workflows_no_durations;

        }


        /// <summary> Get the item BibID, VID, title from the ItemID </summary>
        /// <param name="item_ID"></param>
        private void Get_Bib_VID_from_ItemID(int item_ID)
        {
        
            DataRow temp = Database.SobekCM_Database.Tracking_Get_Item_Info_from_ItemID(item_ID);
            BibID = temp["BibID"].ToString();
            VID = temp["VID"].ToString();
            title = temp["Title"].ToString();
            if (String.IsNullOrEmpty(BibID) || String.IsNullOrEmpty(VID))
                error_message = "No matching item found for this ItemID!";

        }


        /// <summary> Validate the checksum on the barcode value </summary>
        /// <param name="encoded_ItemID">The itemID in Base-26 format</param>
        /// <param name="Stage">Indicates the event boundary</param>
        /// <param name="checksum_string">The checksum value generated for this barcode</param>
        /// <returns>Returns TRUE if the checksum is valid, else FALSE</returns>
        private bool Is_Valid_Checksum(string encoded_ItemID, string Stage, string checksum_string)
        {
            bool is_valid_checksum = true;
            int event_num=0;
            int thisItemID = 0;
            int thisChecksumValue = 0;
            
            Int32.TryParse(Stage, out event_num);
            thisItemID = Int_from_Base26(encoded_ItemID);
            thisChecksumValue = Int_from_Base26(checksum_string);

            if (thisChecksumValue != (thisItemID + event_num)%26)
                is_valid_checksum = false;
 
            return is_valid_checksum;
        }

        /// <summary>Get the itemID from the encoded ID in the barcode </summary>
        /// <param name="encoded_ItemID"></param>
        /// <returns></returns>
        private bool Get_Item_Info_from_Barcode(string encoded_ItemID)
        {
            bool result = true;
            itemID = Int_from_Base26(encoded_ItemID);
            if (String.IsNullOrEmpty(itemID.ToString()))
                result = false;
            return result;
        }

       
        /// <summary> Converts a Base-26 value to the Base-10 equivalent </summary>
        /// <param name="number">The number in Base-26</param>
        /// <returns>The converted Base-10 equivalent</returns>
        private  int Int_from_Base26(String number)
        {
            int convertedNumber = 0;
            if (!String.IsNullOrEmpty(number))
            {
                convertedNumber = (number[0] - 'A');
                for (int i = 1; i < number.Length; i++)
                {
                    convertedNumber *= 26;
                    convertedNumber += (number[i] - 'A');
                }
            }
            return convertedNumber;
        }


        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'Track Item'</value>
        public override string Web_Title
        {
            get
            {
                return "Item Tracking";
            }
        }

        
        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the individual metadata elements are added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Write_HTML", "Do nothing");

            //Include the js files
			Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.color-2.1.1.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.timers.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/sobekcm_track_item.js\" ></script>");
            Output.WriteLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/jquery-ui.css\" />");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/timeentry/jquery.timeentry.min.js\"></script>");
            Output.WriteLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/timeentry/jquery.timeentry.css\"></script>");
 
        }


        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Add_Controls", "");
         //   base.Add_Controls(MainPlaceHolder, Tracer);

            string barcode_row_style = String.Empty;
            string manual_row_style = String.Empty;

            StringBuilder builder = new StringBuilder(2000);
            builder.AppendLine("<!-- Track_Item_MySobekViewer.Add_Controls -->");
            builder.AppendLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/SobekCM_MySobek.css\" /> ");
            builder.AppendLine("  <link rel=\"stylesheet\" type=\"text/css\" href=\"" + currentMode.Base_URL + "default/SobekCM_Admin.css\" /> ");

            builder.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery-ui-1.10.3.custom.min.js\"></script>");
            builder.AppendLine("<script type=\"text/javascript\" src=\"" + currentMode.Base_URL + "default/scripts/jquery/jquery.color-2.1.1.js\"></script>");

            //Set the JavaScript global page value based on the currently selected tab
            builder.AppendLine("<script type=\"text/javascript\">");
            builder.AppendLine("setCurrentTab(" + page + ");");
            builder.AppendLine("</script>");

            //Add the main title
            builder.AppendLine("<div class=\"SobekHomeText\">");
            builder.AppendLine("    <br />");
            builder.AppendLine("    <h1>Item Tracking</h1>");
            builder.AppendLine("  </div>");
            builder.AppendLine();

            //Add the hidden variables
            builder.AppendLine("<!-- Hidden field is used for postbacks to add new form elements (i.e., new page, etc..) -->");
            builder.AppendLine("<input type=\"hidden\" id=\"Track_Item_behaviors_request\" name=\"Track_Item_behaviors_request\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"Track_Item_hidden_value\" name=\"Track_Item_hidden_value\"value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"TI_entry_type\" name=\"TI_entry_type\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_BibID\" name=\"hidden_BibID\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_VID\" name=\"hidden_VID\" value=\"\" />");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_event_num\" name=\"hidden_event_num\" value=\"\" />");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_equipment\" name=\"hidden_equipment\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_selected_username\" name=\"hidden_selected_username\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"tracking_new_page\" name=\"tracking_new_page\" value=\"\"/>");
            builder.AppendLine("<input type=\"hidden\" id=\"hidden_itemID\" name=\"hidden_itemID\" value=\"\"/>");
  

            //Start the User, Equipment info table
            builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>User and Equipment</h2></span>");
            builder.AppendLine("<table class=\"sbkTi_table\">");
            builder.AppendLine("<tr>");
            builder.AppendLine("          <td>Scanned/Processed by:</td>");
            builder.AppendLine("          <td><select id=\"ddlUserStart\" name=\"ddlUserStart\" onchange=\"ddlUser_Changed(this.id);\">");

            //Add the list of users to the dropdown list
            foreach (KeyValuePair<string, User_Object> thisUser in user_list)
            {
                if (thisUser.Key == current_selected_user.UserName)
                    builder.AppendLine("<option value=\"" + thisUser.Key + "\" selected>" + thisUser.Value.Full_Name + "</option>");
                else
                {
                    builder.AppendLine("<option value=\"" + thisUser.Key + "\">" + thisUser.Value.Full_Name + "</option>");
                }
            }
            builder.AppendLine("</td>");
            builder.AppendLine("           <td>Equipment used:</td>");
            builder.AppendLine("           <td><select name=\"ddlEquipmentStart\" id=\"ddlEquipmentStart\" onchange=\"ddlEquipment_Changed(this.id);\">");

  
            //Add the list of scanners to the dropdown list
            foreach (string thisScanner in scanners_list)
            {
                if(thisScanner==equipment)
                    builder.AppendLine("<option value=\"" + thisScanner + "\" selected>"+thisScanner+"</option>");
                else
                    builder.AppendLine("<option value=\"" + thisScanner + "\">" + thisScanner + "</option>");
            }
            builder.AppendLine("</select></td>");
            builder.AppendLine("</tr>");
            builder.AppendLine("</table>");


            //Start the Item Information Table
            string bibid = (String.IsNullOrEmpty(BibID)) ? String.Empty : BibID;
            string vid = (String.IsNullOrEmpty(VID)) ? String.Empty : VID;


            // Start the outer tab container
            builder.AppendLine("  <div id=\"tabContainer\" class=\"fulltabs\">");

 
                builder.AppendLine("    <div class=\"tabs\">");
                builder.AppendLine("      <ul>");

                const string DURATION = "Track with Duration";
                const string SINGLE_POINT = "Track without Duration";

                //Draw all the page tabs for this form

                builder.AppendLine("    <li id=\"tabHeader_1\" onclick=\"save_item_tracking('1');\">" + DURATION + "</li>");
                builder.AppendLine("     <li id=\"tabHeader_2\" onclick=\"save_item_tracking('2');\">" + SINGLE_POINT + "</li>");

                builder.AppendLine("</ul>");
                builder.AppendLine("</div>");
            

               //Start the divs for the content of the tabs
                 builder.AppendLine("    <div class=\"tabscontent\">");

                 #region Content of Tab1 - Tracking with duration
                 
                 builder.AppendLine("    	<div class=\"tabpage\" id=\"tabpage_1\">");
                //Start the item information table

                builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Item Information</h2></span>");
                builder.AppendLine("<table class=\"sbkTi_table\">");

                //Add the item info section
                if (hidden_request == "read_manual_entry")
                {
                    builder.AppendLine("<tr><td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType\" id=\"rb_barcode\" value=0 onclick=\"rbEntryTypeChanged(this.value);\">Barcode Entry</td></tr>");
                    barcode_row_style = "style=\"margin-left:200px\";";

                    builder.AppendLine("<tr id=\"tblrow_Barcode\" " + barcode_row_style + "><td></td><td>Scan barcode here:</td>");
                    builder.AppendLine("         <td colspan=\"3\"><input type=\"text\" id=\"txtScannedString\" name=\"txtScannedString\" autofocus onchange=\"BarcodeStringTextbox_Changed(this.value);\"/></td>");
                    builder.AppendLine("<td>");
                    builder.AppendLine("<div id=\"divAddButton_barcode\" style=\"float:right;\">");
                    builder.AppendLine("    <button title=\"Add new tracking entry\" class=\"sbkMySobek_RoundButton\" onclick=\"Add_new_entry_barcode(); return false;\">ADD</button>");
                    builder.AppendLine("</div></td></tr>");

                    builder.AppendLine("<td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType\" id=\"rb_manual\" value=1 checked onclick=\"rbEntryTypeChanged(this.value);\">Manual Entry</td></tr>");
                    builder.AppendLine("<tr id=\"tblrow_Manual1\" " + manual_row_style + "><td></td><td>BibID:</td><td><input type=\"text\" id=\"txtBibID\" value=\"" + bibid + "\" /></td>");
                    builder.AppendLine("         <td>VID:</td><td><input type=\"text\" id=\"txtVID\" value=\"" + vid + "\" /></td>");
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr id=\"tblrow_Manual2\" " + manual_row_style + ">");
                    builder.AppendLine("<td></td><td>Event:</td><td><select id=\"ddlManualEvent\" name=\"ddlManualEvent\">");
                    builder.AppendLine("                                       <option value=\"1\" selected>Start Scanning</option>");
                    builder.AppendLine("                                        <option value=\"2\">End Scanning</option>");
                    builder.AppendLine("                                        <option value=\"3\">Start Processing</option>");
                    builder.AppendLine("                                        <option value=\"4\">End Processing</option></select>");
                    builder.AppendLine("</td>");

                    //Call the JavaScript functions to apply the appropriate CSS class for the disabled row(s)
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_SetCSSClass('tblrow_Barcode');</script>");
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_RemoveCSSClass('tblrow_Manual1');</script>");
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_RemoveCSSClass('tblrow_Manual2');</script>");

                }
                else
                {
                    builder.AppendLine("<tr><td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType\" id=\"rb_barcode\" value=0 checked onclick=\"rbEntryTypeChanged(this.value);\">Barcode Entry</td></tr>");
                    manual_row_style = "style=\"margin-left:200px\";";

                    builder.AppendLine("<tr id=\"tblrow_Barcode\" " + barcode_row_style + "><td></td><td>Scan barcode here:</td>");
                    builder.AppendLine("         <td colspan=\"3\"><input type=\"text\" id=\"txtScannedString\" name=\"txtScannedString\" autofocus onchange=\"BarcodeStringTextbox_Changed(this.value);\"/></td>");
                    builder.AppendLine("<td>");
                    builder.AppendLine("<div id=\"divAddButton_barcode\" style=\"float:right;\">");
                    builder.AppendLine("    <button title=\"Add new tracking entry\" class=\"sbkMySobek_RoundButton\" onclick=\"Add_new_entry_barcode(); return false;\">ADD</button>");
                    builder.AppendLine("</div></td></tr>");

                    builder.AppendLine("<tr><td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType\" id=\"rb_manual\" value=1 onclick=\"rbEntryTypeChanged(this.value);\">Manual Entry</td></tr>");
                    builder.AppendLine("<tr id=\"tblrow_Manual1\" " + manual_row_style + "><td></td><td>BibID:</td><td><input type=\"text\" id=\"txtBibID\" value=\"" + bibid + "\" /></td>");
                    builder.AppendLine("         <td>VID:</td><td><input type=\"text\" id=\"txtVID\" value=\"" + vid + "\" /></td>");
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr id=\"tblrow_Manual2\" " + manual_row_style + ">");
                    builder.AppendLine("<td></td><td>Event:</td><td><select id=\"ddlManualEvent\" name=\"ddlManualEvent\">");
                    builder.AppendLine("                                       <option value=\"1\" selected>Start Scanning</option>");
                    builder.AppendLine("                                        <option value=\"2\">End Scanning</option>");
                    builder.AppendLine("                                        <option value=\"3\">Start Processing</option>");
                    builder.AppendLine("                                        <option value=\"4\">End Processing</option></select>");
                    builder.AppendLine("</td>");

                    //Call the JavaScript functions to apply the appropriate CSS class for the disabled row(s)
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_SetCSSClass('tblrow_Manual1');</script>");
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_SetCSSClass('tblrow_Manual2');</script>");

                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_RemoveCSSClass('tblrow_Barcode');</script>");

                }

                //Set the appropriate value in the workflow dropdown 
                if (stage >= 1)
                {
                    builder.AppendLine("<script type=\"text/javascript\">SetDropdown_Selected(" + stage + ");</script>");
                }

                builder.AppendLine("<td>");
                builder.AppendLine("<div id=\"divAddButton\" style=\"float:right;\">");
                builder.AppendLine("    <button title=\"Add new tracking entry\" class=\"sbkMySobek_RoundButton\" onclick=\"Add_new_entry(); return false;\">ADD</button>");
                builder.AppendLine("</div></td></tr>");

                builder.AppendLine("</table>");

                //End the item information table


               //If a new event has been scanned/entered, then display this table
               if (!String.IsNullOrEmpty(bibid) && !String.IsNullOrEmpty(vid) && !(error_message.Length > 0))
              {

                string selected_text_scanning = String.Empty;
                string selected_text_processing = String.Empty;
                string currentTime = DateTime.Now.ToString("");

                if (page == 1)
                {
                    //Start the Tracking Info section
                    builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Add Tracking Information</h2></span>");
                    builder.AppendLine("<span id = \"TI_NewEntrySpan\" class=\"sbkTi_TrackingEntrySpanMouseOut\" onmouseover=\"return entry_span_mouseover(this.id);\" onmouseout=\"return entry_span_mouseout(this.id);\">");
                    builder.AppendLine("<table class=\"sbkTi_table\">");

                    //If the user is trying to close an unopened entry, display an error message
                    if (close_error == true)
                    {
                        builder.AppendLine("<tr><td colspan=\"4\">");
                        builder.AppendLine("<span style=\"color:red;\">There is no matching open workflow to close!</span>");
                        builder.AppendLine("</td></tr>");
                    }

                    builder.AppendLine("<tr >");
                    builder.AppendLine("<td>Item: </td><td>" + bibid + ":" + vid + "</td>");

                    builder.AppendLine("<td>Title: </td><td>" + title + "</td>");
                    builder.AppendLine("</tr>");

                    builder.AppendLine("<tr><td>Workflow:</td>");
                    builder.AppendLine("         <td><select id=\"ddlEvent" + current_workflow_id + "\" name=\"ddlEvent" + current_workflow_id + "\"> disabled");
                    builder.AppendLine("                  <option value=\"1\" " + selected_text_scanning + ">Scanning</option>");
                    builder.AppendLine("                  <option value=\"2\"" + selected_text_processing + ">Processing</option></select>");
                    builder.AppendLine("         </td>");
                    builder.AppendLine("         <td>Date:</td>");
                    string currentDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                    //Set this as a JQuery datepicker field
                    //builder.AppendLine("<script type=\"text/javascript\">$(function(){$(\"#txtStartDate" + current_workflow_id + "\").datepicker();});</script>");
                    builder.AppendLine("         <td><input type=\"text\"  name=\"txtStartDate" + current_workflow_id + "\" id=\"txtStartDate" + current_workflow_id + "\" value=\"" + currentDate + "\" /> </td>");
                    builder.AppendLine("<script type=\"text/javascript\">setDatePicker(\"txtStartDate" + current_workflow_id + "\");</script>");
                    builder.AppendLine("</tr>");

           

                    //Add the Start and End Times
                    builder.AppendLine("<tr>");
                    builder.AppendLine("<td>Start Time:</td>");
                    if (close_error == true)
                    {
                       // builder.AppendLine("<td><input type=\"time\" name=\"txtStartTime" + current_workflow_id + "\" class=\"sbkTi_ErrorBox\" id=\"txtStartTime" + current_workflow_id + "\" value = \"" + start_Time + "\"/></td>");
                        builder.AppendLine("<td><input type=\"text\" name=\"txtStartTime" + current_workflow_id + "\" class=\"sbkTi_ErrorBox\" id=\"txtStartTime" + current_workflow_id + "\" value = \"" + start_Time + "\"/></td>");
                        builder.AppendLine("<script type=\"text/javascript\">setTimePicker(\"txtStartTime"+current_workflow_id+"\");</script>");

                    }
                    else
                    {
                       // builder.AppendLine("<td><input type=\"time\" name=\"txtStartTime" + current_workflow_id + "\" id=\"txtStartTime" + current_workflow_id + "\" value = \"" + start_Time + "\"/></td>");
                        builder.AppendLine("<td><input type=\"text\" name=\"txtStartTime" + current_workflow_id + "\" id=\"txtStartTime" + current_workflow_id + "\" value = \"" + start_Time + "\"/></td>");
                        builder.AppendLine("<script type=\"text/javascript\">setTimePicker(\"txtStartTime" + current_workflow_id + "\");</script>");
                    }
                    builder.AppendLine("<td>End Time:</td>");
                    builder.AppendLine("<td><input type=\"text\" name=\"txtEndTime" + current_workflow_id + "\" id=\"txtEndTime" + current_workflow_id + "\" value = \"" + end_Time + "\"/></td></tr>");
                    builder.AppendLine("<script type=\"text/javascript\">setTimePicker(\"txtStartTime" + current_workflow_id + "\");</script>");

                    builder.AppendLine("<tr><td colspan=\"4\"><span style=\"float:right;\">");
                    builder.AppendLine("    <button title=\"Save changes\" class=\"sbkMySobek_RoundButton\" onclick=\"save_workflow('" + current_workflow_id + "',' " + itemID + "'); \">SAVE</button>");
                    builder.AppendLine("    <button title=\"Delete this workflow\" class=\"sbkMySobek_RoundButton\" onclick=\"delete_workflow(" + current_workflow_id + "); \">DELETE</button>");
                    builder.AppendLine("</span></td></tr>");

                    //End this table
                    builder.AppendLine("</table>");

                    builder.AppendLine("</span>");
                }

                //If there are any previously opened and unclosed workflows for this item
                if (open_workflows_from_DB != null && open_workflows_from_DB.Rows.Count > 0)
                {

                    //builder.AppendLine("<tr><th>Item</th><th>Workflow</th><th>Date</th><th>Start Time</th><th>End Time</th><th>User</th><th>Equipment</th></tr>");
                    foreach (DataRow row in open_workflows_from_DB.Rows)
                    {
                        string thisWorkflowID = row["ProgressID"].ToString();
                        //If this is a "close" workflow event, display all open and unclosed workflows from before today
                        if (stage == 2 || stage == 4)
                        {
                            if (row["DateStarted"]!=null && Convert.ToDateTime(row["DateStarted"]).ToString("yyyy-MM-dd") == DateTime.Now.ToString("yyyy-MM-dd"))
                                continue;
                        }

                        string this_date_started = (row["DateStarted"]==null) ? "" : Convert.ToDateTime(row["DateStarted"]).ToString("MM/dd/yyyy");

                        builder.AppendLine("<span id=\"TI_NewEntry_duplicate_Span" + row["ProgressID"] + "\"  class=\"sbkTi_TrackingEntrySpanMouseOut\"  onmouseover=\"return entry_span_mouseover(this.id);\" onmouseout=\"return entry_span_mouseout(this.id);\">");
                        builder.AppendLine("<table class=\"sbkTi_table\" >");

                        builder.AppendLine("<tr><td colspan=\"4\">");
                        builder.AppendLine("<span style=\"color:red;\">You have a previously opened workflow from " + this_date_started + "! </span>");

                        builder.AppendLine("</td></tr>");


                        //Start the table
                        builder.AppendLine("<tr >");
                        builder.AppendLine("<td>Item: </td><td>" + BibID + ":" + VID + "</td>");

                        builder.AppendLine("<td>Title: </td><td>" + title + "</td>");
                        builder.AppendLine("</tr>");

                        string this_workflow = row["WorkFlowName"].ToString();
                        if (this_workflow == "Scanning")
                        {
                            selected_text_scanning = "selected";
                            selected_text_processing = "";
                        }
                        else if (this_workflow == "Processing")
                        {
                            selected_text_scanning = "";
                            selected_text_processing = "selected";
                        }
                        builder.AppendLine("<tr><td>Workflow:</td>");
                        builder.AppendLine("         <td><select id=\"ddlEvent" + thisWorkflowID + "\" name=\"ddlEvent" + thisWorkflowID + "\"> disabled");
                        builder.AppendLine("                  <option value=\"1\" " + selected_text_scanning + ">Scanning</option>");
                        builder.AppendLine("                  <option value=\"2\"" + selected_text_processing + ">Processing</option></select>");
                        builder.AppendLine("         </td>");
                        builder.AppendLine("         <td>Date:</td>");

                        builder.AppendLine("         <td><input type=\"text\" name=\"txtStartDate" + thisWorkflowID + "\" id=\"txtStartDate" + thisWorkflowID + "\" value=\"" + Convert.ToDateTime(row["DateStarted"]).ToString("yyyy-MM-dd") + "\" /> </td>");
                        builder.AppendLine("<script type=\"text/javascript\">setDatePicker(\"txtStartDate" + thisWorkflowID + "\");</script>");
                        builder.AppendLine("</tr>");

                        //Add the Start and End Times
                        builder.AppendLine("<tr>");
                        builder.AppendLine("<td>Start Time:</td>");
                        builder.AppendLine("<td><input type=\"text\" name=\"txtStartTime" + thisWorkflowID + "\" id=\"txtStartTime" + thisWorkflowID + "\" value = \"" + Convert.ToDateTime(row["DateStarted"]).ToString("HH:mm") + "\"/></td>");
                        builder.AppendLine("<script type=\"text/javascript\">setTimePicker(\"txtStartTime" + thisWorkflowID + "\");</script>");

                        builder.AppendLine("<td>End Time:</td>");
                        builder.AppendLine("<td><input type=\"text\" name=\"txtEndTime" + thisWorkflowID + "\" id=\"txtEndTime" + thisWorkflowID + "\" class=\"sbkTi_ErrorBox\" value = \"" + end_Time + "\"/></td></tr>");
                        builder.AppendLine("<script type=\"text/javascript\">setTimePicker(\"txtStartTime" + thisWorkflowID + "\");</script>");

                        builder.AppendLine("<tr><td colspan=\"4\"><span style=\"float:right;\">");
                        builder.AppendLine("    <button title=\"Save changes\" class=\"sbkMySobek_RoundButton\" onclick=\"save_workflow('" + thisWorkflowID + "','" + row["ItemID"] + "'); \">SAVE</button>");
                        builder.AppendLine("    <button title=\"Delete this workflow\" class=\"sbkMySobek_RoundButton\" onclick=\"delete_workflow(" + thisWorkflowID + "); \">DELETE</button>");
                        builder.AppendLine("</span></td></tr>");

                        //End this table
                        builder.AppendLine("</table>");
                        builder.AppendLine("</span>");


                    }

                }
            }
            //Add the current History table
                    if (current_workflows != null && current_workflows.Count > 0)
                    {
                        builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Current Work History</h2></span>");
                        builder.AppendLine("<table id=\"sbkTi_tblCurrentTracking\" class=\"sbkSaav_Table\">");
                        builder.AppendLine("<tr><th>Item</th><th>Workflow</th><th>Date</th><th>Start Time</th><th>End Time</th><th>User</th><th>Equipment</th></tr>");
                        foreach (KeyValuePair<string, Tracking_Workflow> thisPair in current_workflows)
                        {
                            Tracking_Workflow this_workflow = thisPair.Value;
                            string workflow_text = String.Empty;
                            if (this_workflow.Workflow_type == 1 || this_workflow.Workflow_type == 2)
                                workflow_text = "Scanning";
                            else
                            {
                                workflow_text = "Processing";
                            }
                            builder.AppendLine("<tr>");
                            builder.AppendLine("<td title=\"" + this_workflow.Title + "\">" + this_workflow.BibID + ":" + this_workflow.VID + "</td>");
                            builder.AppendLine("<td>" + workflow_text + "</td>");
                            builder.AppendLine("<td>" + Convert.ToDateTime(this_workflow.Date).ToString("MM/dd/yyyy") + "</td>");
                            builder.AppendLine("<td>" + (String.IsNullOrEmpty(this_workflow.StartTime) ? "-" : Convert.ToDateTime(this_workflow.StartTime).ToString("hh:mm tt")) + "</td>");
                            builder.AppendLine("<td>" + (String.IsNullOrEmpty(this_workflow.EndTime) ? "-" : Convert.ToDateTime(this_workflow.EndTime).ToString("hh:mm tt")) + "</td>");
                            builder.AppendLine("<td>" + this_workflow.thisUser.UserName + "</td>");
                            builder.AppendLine("<td>" + this_workflow.Equipment + "</td>");
                            builder.AppendLine("</tr>");
                        }

                        builder.AppendLine("</table>");


                    }

                    //Add the Save and Done buttons
                    builder.AppendLine("<div id=\"divButtons\" style=\"float:right;\">");
                    builder.AppendLine("    <button title=\"Save changes\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">SAVE</button>");
                    builder.AppendLine("    <button title=\"Save all changes and exit\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">DONE</button>");
                    builder.AppendLine("</div");
                    builder.AppendLine("<br/><br/>");

   //             }

                //Close the inner tab div
                builder.AppendLine("</div>");

  
                 #endregion



                #region Second tab - tracking without duration


                builder.AppendLine("    	<div class=\"tabpage\" id=\"tabpage_2\">");
                //Start the item information table

                builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Item Information</h2></span>");
                builder.AppendLine("<table class=\"sbkTi_table\">");

                //Add the item info section
                if (hidden_request == "read_manual_entry")
                {
                    builder.AppendLine("<tr><td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType2\" id=\"rb_barcode\" value=0  onclick=\"rbEntryType2Changed(this.value);\">Barcode Entry</td></tr>");
                    barcode_row_style = "style=\"margin-left:200px\";";

                    builder.AppendLine("<tr id=\"tblrow2_Barcode\" " + barcode_row_style + "><td></td><td>Scan barcode here:</td>");
                    builder.AppendLine("         <td colspan=\"3\"><input type=\"text\" id=\"txtScannedString2\" name=\"txtScannedString2\" autofocus onchange=\"BarcodeStringTextbox2_Changed(this.value);\"/></td>");
                    builder.AppendLine("<td>");
                    builder.AppendLine("<div id=\"divAddButton2_barcode\" style=\"float:right;\">");
                    builder.AppendLine("    <button title=\"Add new tracking entry\" class=\"sbkMySobek_RoundButton\" onclick=\"Add_new_entry_barcode2(); return false;\">ADD</button>");
                    builder.AppendLine("</div></td></tr>");

                    builder.AppendLine("<td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType2\" id=\"rb_manual\" value=1 checked onclick=\"rbEntryType2Changed(this.value);\">Manual Entry</td></tr>");
                    builder.AppendLine("<tr id=\"tblrow2_Manual1\" " + manual_row_style + "><td></td><td>BibID:</td><td><input type=\"text\" id=\"txtBibID2\"  value=\"" + bibid + "\" /></td>");
                    builder.AppendLine("         <td>VID:</td><td><input type=\"text\" id=\"txtVID2\" value=\"" + vid + "\" /></td>");
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr id=\"tblrow2_Manual2\" " + manual_row_style + ">");
                    builder.AppendLine("<td></td><td>Event:</td><td><select id=\"ddlManualEvent2\" name=\"ddlManualEvent2\">");
                    builder.AppendLine("                                       <option value=\"1\" selected>Start Scanning</option>");
                    builder.AppendLine("                                        <option value=\"2\">End Scanning</option>");
                    builder.AppendLine("                                        <option value=\"3\">Start Processing</option>");
                    builder.AppendLine("                                        <option value=\"4\">End Processing</option></select>");
                    builder.AppendLine("</td>");

                    //Call the JavaScript functions to apply the appropriate CSS class for the disabled row(s)
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_SetCSSClass('tblrow2_Barcode');</script>");
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_RemoveCSSClass('tblrow2_Manual1');</script>");
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_RemoveCSSClass('tblrow2_Manual2');</script>");

                }
                else
                {
                    builder.AppendLine("<tr><td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType2\" id=\"rb_barcode\" value=0 checked onclick=\"rbEntryType2Changed(this.value);\"/>Barcode Entry</td></tr>");
                    manual_row_style = "style=\"margin-left:200px\";";

                    builder.AppendLine("<tr id=\"tblrow2_Barcode\" " + barcode_row_style + "><td></td><td>Scan barcode here:</td>");
                    builder.AppendLine("         <td colspan=\"3\"><input type=\"text\" id=\"txtScannedString2\" name=\"txtScannedString2\" autofocus onchange=\"BarcodeStringTextbox_Changed(this.value);\"/></td>");
                    builder.AppendLine("<td>");
                    builder.AppendLine("<div id=\"divAddButton_barcode\" style=\"float:right;\">");
                    builder.AppendLine("    <button title=\"Add new tracking entry\" class=\"sbkMySobek_RoundButton\" onclick=\"Add_new_entry_barcode2(); return false;\">ADD</button>");
                    builder.AppendLine("</div></td></tr>");

                    builder.AppendLine("<tr><td colspan=\"100%\"><input type=\"radio\" name=\"rbEntryType2\" id=\"rb_manual\" value=1 onclick=\"rbEntryType2Changed(this.value);\"/>Manual Entry</td></tr>");
                    builder.AppendLine("<tr id=\"tblrow2_Manual1\" " + manual_row_style + "><td></td><td>BibID:</td><td><input type=\"text\" id=\"txtBibID2\" value=\"" + bibid + "\" /></td>");
                    builder.AppendLine("         <td>VID:</td><td><input type=\"text\" id=\"txtVID2\" value=\"" + vid + "\" /></td>");
                    builder.AppendLine("</tr>");
                    builder.AppendLine("<tr id=\"tblrow2_Manual2\" " + manual_row_style + ">");
                    builder.AppendLine("<td></td><td>Event:</td><td><select id=\"ddlManualEvent2\" name=\"ddlManualEvent2\">");
                    builder.AppendLine("                                       <option value=\"1\" selected>Start Scanning</option>");
                    builder.AppendLine("                                        <option value=\"2\">End Scanning</option>");
                    builder.AppendLine("                                        <option value=\"3\">Start Processing</option>");
                    builder.AppendLine("                                        <option value=\"4\">End Processing</option></select>");
                    builder.AppendLine("</td>");

                    //Call the JavaScript functions to apply the appropriate CSS class for the disabled row(s)
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_SetCSSClass('tblrow2_Manual1');</script>");
                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_SetCSSClass('tblrow2_Manual2');</script>");

                    builder.AppendLine("<script type=\"text/javascript\">DisableRow_RemoveCSSClass('tblrow2_Barcode');</script>");

                }

                //Set the appropriate value in the workflow dropdown 
                if (stage >= 1)
                {
                    builder.AppendLine("<script type=\"text/javascript\">SetDropdown_Selected(" + stage + ");</script>");
                }

                builder.AppendLine("<td>");
                builder.AppendLine("<div id=\"divAddButton\" style=\"float:right;\">");
                builder.AppendLine("    <button title=\"Add new tracking entry\" class=\"sbkMySobek_RoundButton\" onclick=\"Add_new_entry(); return false;\">ADD</button>");
                builder.AppendLine("</div></td></tr>");

                builder.AppendLine("</table>");

                //End the item information table


                //If a new event has been scanned/entered, then display this table
                if (!String.IsNullOrEmpty(bibid) && !String.IsNullOrEmpty(vid) && !(error_message.Length > 0))
            {

                string selected_text_scanning = String.Empty;
                string selected_text_processing = String.Empty;
                string currentTime = DateTime.Now.ToString("");


                //Start the Tracking Info section
                if (page == 2)
                {
                    builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Add Tracking Information</h2></span>");
                    builder.AppendLine("<span id = \"TI_NewEntrySpan\" class=\"sbkTi_TrackingEntrySpanMouseOut\" onmouseover=\"return entry_span_mouseover(this.id);\" onmouseout=\"return entry_span_mouseout(this.id);\">");
                    builder.AppendLine("<table class=\"sbkTi_table\">");
                    builder.AppendLine("<tr >");
                    builder.AppendLine("<td>Item: </td><td>" + bibid + ":" + vid + "</td>");

                    builder.AppendLine("<td>Title: </td><td>" + title + "</td>");
                    builder.AppendLine("</tr>");

                    builder.AppendLine("<tr><td>Workflow:</td>");
                    builder.AppendLine("         <td><select id=\"ddlEvent2\" name=\"ddlEvent2\"> disabled");
                    builder.AppendLine("                  <option value=\"1\" " + selected_text_scanning + ">Scanning</option>");
                    builder.AppendLine("                  <option value=\"2\"" + selected_text_processing + ">Processing</option></select>");
                    builder.AppendLine("         </td>");
                    builder.AppendLine("         <td>Date:</td>");
                    string currentDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
                    builder.AppendLine("         <td><input type=\"text\" name=\"txtStartDate2\" id=\"txtStartDate2\" value=\"" + currentDate + "\" /> </td>");
                    builder.AppendLine("<script type=\"text/javascript\">setDatePicker(\"txtStartDate2\");</script>");
                    builder.AppendLine("</tr>");

                    builder.AppendLine("<tr><td colspan=\"4\"><span style=\"float:right;\">");
                    builder.AppendLine("    <button title=\"Save changes\" class=\"sbkMySobek_RoundButton\" onclick=\"save_workflow('" + current_workflow_id + "','" + itemID + "'); return false;\">SAVE</button>");
                    builder.AppendLine("    <button title=\"Delete this workflow\" class=\"sbkMySobek_RoundButton\" onclick=\"delete_workflow(); return false;\">DELETE</button>");
                    builder.AppendLine("</span></td></tr>");

                    //End this table
                    builder.AppendLine("</table>");

                    builder.AppendLine("</span>");
                }

            }
            //Add the current History table
                    if (current_workflows_no_durations != null && current_workflows_no_durations.Count > 0)
                    {
                        builder.AppendLine("<span class=\"sbkTi_HomeText\"><h2>Current Work History</h2></span>");
                        builder.AppendLine("<table id=\"sbkTi_tblCurrentTracking2\" class=\"sbkSaav_Table\">");
                        builder.AppendLine("<tr><th>Item</th><th>Workflow</th><th>Date</th><th>User</th><th>Equipment</th></tr>");
                        foreach (KeyValuePair<string, Tracking_Workflow> thisPair in current_workflows_no_durations)
                        {
                            Tracking_Workflow this_workflow = thisPair.Value;
                            string workflow_text = String.Empty;
                            if (this_workflow.Workflow_type == 1 || this_workflow.Workflow_type == 2)
                                workflow_text = "Scanning";
                            else
                            {
                                workflow_text = "Processing";
                            }
                            builder.AppendLine("<tr>");
                            builder.AppendLine("<td title=\"" + this_workflow.Title + "\">" + this_workflow.BibID + ":" + this_workflow.VID + "</td>");
                            builder.AppendLine("<td>" + workflow_text + "</td>");
                            builder.AppendLine("<td>" + Convert.ToDateTime(this_workflow.Date).ToString("MM/dd/yyyy") + "</td>");
                 //           builder.AppendLine("<td>" + (String.IsNullOrEmpty(this_workflow.EndTime) ? "-" : Convert.ToDateTime(this_workflow.EndTime).ToString("hh:mm tt")) + "</td>");
                            builder.AppendLine("<td>" + this_workflow.thisUser.UserName + "</td>");
                            builder.AppendLine("<td>" + this_workflow.Equipment + "</td>");
                            builder.AppendLine("</tr>");
                        }

                        builder.AppendLine("</table>");


                    }

                    //Add the Save and Done buttons
                    builder.AppendLine("<div id=\"divButtons\" style=\"float:right;\">");
           //         builder.AppendLine("    <button title=\"Save changes\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">SAVE</button>");
                    builder.AppendLine("    <button title=\"Save all changes and exit\" class=\"sbkMySobek_RoundButton\" onclick=\"save(); return false;\">DONE</button>");
                    builder.AppendLine("</div");
                    builder.AppendLine("<br/><br/>");

       //         }

                //Close the inner tab div
                builder.AppendLine("</div>");

                builder.AppendLine("</div>");

            

#endregion



                builder.AppendLine("</div>");

            //Close the outer tab container
            builder.AppendLine("</div>");


            //Close the main div
            builder.AppendLine("</div>");

            builder.AppendLine("<br/><br/>");
           
            //Add the script to get the tab functionality
            builder.AppendLine("  <script>");
            builder.AppendLine("    $(document).ready(function(){");
            builder.AppendLine("      $(\"#tabContainer\").acidTabs();");
            builder.AppendLine("    });");
            builder.AppendLine("  </script>");

            
            LiteralControl control1 = new LiteralControl(builder.ToString());
          
            MainPlaceHolder.Controls.Add(control1);


     }




//A tracking workflow object which holds all the details of the current workflow 
  protected class Tracking_Workflow
  {
      public int WorkflowID { get; set; }

      public int Workflow_type { get; set; }

      public string Date { get; set; }

      public string StartTime { get; set; }

      public User_Object thisUser { get; set; }

      public int itemID { get; set; }

      public string EndTime { get; set; }

      public string BibID { get; set; }

      public string VID { get; set; }

      public string Title { get; set; }

      public string Equipment { get; set; }

      public bool Saved { get; set; }

      public Tracking_Workflow()
      {
          
      }
  }

    }
}
