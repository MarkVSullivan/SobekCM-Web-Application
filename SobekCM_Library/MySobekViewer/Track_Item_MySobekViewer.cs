#region Using directives
using System;
using System.IO;
using System.Text;
using System.Web;
using System.Web.UI;
using System.Web.UI.WebControls;
using SobekCM.Library.Application_State;
using SobekCM.Library.Navigation;
using SobekCM.Library.Users;
using SobekCM.Resource_Object;
#endregion

namespace SobekCM.Library.MySobekViewer
{
    class Track_Item_MySobekViewer : abstract_MySobekViewer
    {
        private readonly SobekCM_Item item;

        /// <summary> Constructor for a new instance of the Track_Item_MySobekViewer class </summary>
        /// <param name="User"> Authenticated user information </param>
        /// <param name="Current_Mode"> Mode / navigation information for the current request</param>
        /// <param name="Current_Item"> Individual digital resource to be edited by the user </param>
        /// <param name="Code_Manager"> Code manager contains the list of all valid aggregation codes </param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        public Track_Item_MySobekViewer(User_Object User, SobekCM_Navigation_Object Current_Mode,
                                                SobekCM_Item Current_Item, Aggregation_Code_Manager Code_Manager,
                                                Custom_Tracer Tracer) 
            :  base(User)
          {
                    Tracer.Add_Trace("Track_Item_MySobekViewer.Constructor", String.Empty);

                    currentMode = Current_Mode;
                    item = Current_Item;

                    // If the user cannot edit this item, go back
                    if (!user.Can_Edit_This_Item(item))
                    {
                        currentMode.My_Sobek_Type = My_Sobek_Type_Enum.Home;
                        HttpContext.Current.Response.Redirect(currentMode.Redirect_URL());
                    }

            }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        /// <value> This returns the value 'Track Item'</value>
        public override string Web_Title
        {
            get
            {
                return "Track Item";
            }
        }

        /// <summary> Add the HTML to be displayed in the main SobekCM viewer area </summary>
        /// <param name="Output"> Textwriter to write the HTML for this viewer</param>
        /// <param name="Tracer">Trace object keeps a list of each method executed and important milestones in rendering</param>
        /// <remarks> This class does nothing, since the individual metadata elements are added as controls, not HTML </remarks>
        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Write_HTML", "Do nothing");
        }


        public override void Add_Controls(PlaceHolder MainPlaceHolder, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("Track_Item_MySobekViewer.Add_Controls", "");
         //   base.Add_Controls(MainPlaceHolder, Tracer);

            StringBuilder temp_builder = new StringBuilder(2000);
            temp_builder.AppendLine("<!-- Track_Item_MySobekViewer.Add_Controls -->");
            temp_builder.AppendLine("<div class=\"SobekHomeText\">");

            temp_builder.AppendLine("<span>Scanned barcode:<input type=\"text\" id=\"txtScannedString\" autofocus /></span>");
            temp_builder.AppendLine("<br/><br/>");

            //currentMode.My_Sobek_SubMode = current_submode;


            temp_builder.AppendLine("</div>");


            MainPlaceHolder.Controls.Add(new LiteralControl("</td>\n    <td width=\"20px\">&nbsp;</td>\n  </tr>\n</table>\n"));



            //StringBuilder builder = new StringBuilder(2000);
            //StringWriter output = new StringWriter(builder);

       


        }


    }
}
