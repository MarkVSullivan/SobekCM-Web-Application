#region Using directives
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
#endregion

namespace SobekCM.Library.ItemViewer.Viewers
{
    public class TrackingSheet_ItemViewer : abstractItemViewer
    {
        private SobekCM_Item track_item;

        /// <summary>
        /// Constructor for the Tracking Sheet ItemViewer
        /// </summary>
        /// <param name="Current_Object"></param>
        /// <param name="Current_User"></param>
        /// <param name="Current_Mode"></param>
        public TrackingSheet_ItemViewer(SobekCM_Item Current_Object, User_Object Current_User, SobekCM_Navigation_Object Current_Mode)
        {
            CurrentMode = Current_Mode;
            CurrentUser = Current_User;

            //Assign the current resource object to track_item
            track_item = Current_Object;

            // If there is no user, send to the login
            if (CurrentUser == null)
            {
                CurrentMode.Mode = Display_Mode_Enum.My_Sobek;
                CurrentMode.My_Sobek_Type = My_Sobek_Type_Enum.Logon;
                CurrentMode.Redirect();
                return;
            }

            // If the user cannot edit this item, go back
            if (!CurrentUser.Can_Edit_This_Item(Current_Object))
            {
                CurrentMode.ViewerCode = String.Empty;
                CurrentMode.Redirect();
                return;
            }
        }


        /// <summary> Stream to which to write the HTML for this subwriter  </summary>
        /// <param name="Output"> Response stream for the item viewer to write directly to </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Main_Viewer_Section(TextWriter Output, Custom_Tracer Tracer)
        {
            if (Tracer != null)
            {
                Tracer.Add_Trace("TrackingSheet_ItemViewer.Write_Main_Viewer_Section", "");
            }

            Output.WriteLine("\t\t<!-- TRACKING SHEET VIEWER OUTPUT -->");

            //Start the table
            Output.WriteLine("<table>");
            Output.WriteLine("<tr><td>"+track_item.BibID+" : "+track_item.VID+"</td></tr>");

            Output.WriteLine("</table>");


            //End the table

        }


        /// <summary> Gets the type of item viewer this object represents </summary>
        /// <value> This property always returns the enumerational value <see cref="ItemViewer_Type_Enum.Tracking_Sheet"/>. </value>
        public override ItemViewer_Type_Enum ItemViewer_Type
        {
            get { return ItemViewer_Type_Enum.Tracking_Sheet; }
        }


        /// <summary> Flag indicates if this view should be overriden if the item is checked out by another user </summary>
        /// <remarks> This always returns the value TRUE for this viewer </remarks>
        public override bool Override_On_Checked_Out
        {
            get
            {
                return true;
            }
        }

        /// <summary> Width for the main viewer section to adjusted to accomodate this viewer</summary>
        /// <value> This always returns the value -1</value>
        public override int Viewer_Width
        {
            get
            {
                return -1;
            }
        }

        /// <summary> Height for the main viewer section to adjusted to accomodate this viewer</summary>
        public override int Viewer_Height
        {
            get
            {
                return -1;
            }
        }

    }
}
