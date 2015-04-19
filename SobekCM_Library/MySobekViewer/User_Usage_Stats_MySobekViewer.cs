#region Using directives

using System;
using System.Data;
using System.IO;
using SobekCM.Library.Database;
using SobekCM.Library.UI;
using SobekCM.Tools;

#endregion

namespace SobekCM.Library.MySobekViewer
{
    class User_Usage_Stats_MySobekViewer : abstract_MySobekViewer
    {
        /// <summary> Constructor for a new instance of the User_Tags_MySobekViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public User_Usage_Stats_MySobekViewer(RequestCache RequestSpecificValues)  : base(RequestSpecificValues)
        {
            RequestSpecificValues.Tracer.Add_Trace("User_Usage_Stats_MySobekViewer.Constructor", String.Empty);
        }

        /// <summary> Title for the page that displays this viewer, this is shown in the search box at the top of the page, just below the banner </summary>
        public override string Web_Title
        {
            get
            {
                return "Item Usage Statistics";
            }
        }


        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            Tracer.Add_Trace("User_Usage_Stats_MySobekViewer.Write_HTML", String.Empty);

            string submode = RequestSpecificValues.Current_Mode.My_Sobek_SubMode;

            // Determine the date (month,year) for the usage stats to display
            int month = UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Month;
            int year = UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Year;
            if (submode.Length >= 6)
            {
                Int32.TryParse(submode.Substring(0, 4), out year);
                Int32.TryParse(submode.Substring(4, 2), out month);
            }

            char sort_value = 'a';
            string sort_term = "Title";
            if ((submode.Length == 1) || (submode.Length == 7))
            {
                switch (submode[submode.Length - 1])
                {
                    case 'b':
                        sort_value = 'b';
                        sort_term = "Total_Hits DESC";
                        break;

                    case 'c':
                        sort_value = 'c';
                        sort_term = "Total_Sessions DESC";
                        break;

                    case 'd':
                        sort_value = 'd';
                        sort_term = "Month_Hits DESC";
                        break;

                    case 'e':
                        sort_value = '3';
                        sort_term = "Month_Sessions DESC";
                        break; 
                }
            }

			Output.WriteLine("<h1>" + Web_Title + "</h1>");
			Output.WriteLine();

            Output.WriteLine("<div class=\"SobekText\">");
            Output.WriteLine("<br />");
            Output.WriteLine("<p>Below is a list of items associated with your account including usage statistics.  Total views and visits represents the total amount of usage since the item was added to the library and the monthly views and visits is the usage in the selected month.  For more information about these terms, see the <a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + "stats/usage/definitions\" target=\"_BLANK\">definitions on the main statistics page</a>.</p>");
            Output.WriteLine("<p>You may be the author, contributor, or associated with these items in some other way.</p>");
            Output.WriteLine("<p>To see statistics for a different month, change the selected month/year:");
            Output.WriteLine("<select name=\"date1_selector\" class=\"SobekStatsDateSelector\" onChange=\"window.location.href='" + RequestSpecificValues.Current_Mode.Base_URL + "my/stats/' + this.options[selectedIndex].value + '" + sort_value + "';\">");

            int select_month = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Month;
            int select_year = UI_ApplicationCache_Gateway.Stats_Date_Range.Earliest_Year;
            while ((select_month != UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Month) || (select_year != UI_ApplicationCache_Gateway.Stats_Date_Range.Latest_Year))
            {
                if ((month == select_month) && (year == select_year))
                {
                    Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\" selected=\"selected\" >" + Month_From_Int(select_month) + " " + select_year + "</option>");
                }
                else
                {
                    Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\">" + Month_From_Int(select_month) + " " + select_year + "</option>");
                }

                select_month++;
                if (select_month > 12)
                {
                    select_month = 1;
                    select_year++;
                }
            }
            if ((month == select_month) && (year == select_year))
            {
                Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\" selected=\"selected\" >" + Month_From_Int(select_month) + " " + select_year + "</option>");
            }
            else
            {
                Output.WriteLine("      <option value=\"" + select_year + select_month.ToString().PadLeft(2, '0') + "\">" + Month_From_Int(select_month) + " " + select_year + "</option>");
            }
            Output.WriteLine("    </select></p>");

            Output.WriteLine("<p>Select any column to re-sort this data.</p>");
            Output.WriteLine("<br />");

            // Get the item usage stats for this user on this month
            DataTable usageStats = SobekCM_Database.Get_User_Linked_Items_Stats(RequestSpecificValues.Current_User.UserID, month, year, Tracer);

            // Only continue if stats were returned
            if (usageStats != null)
            {
                // Use the data view
                DataView sortedView = new DataView(usageStats) {Sort = sort_term};

                // Add the table header
                Output.WriteLine("<center>");
                Output.WriteLine("<table border=\"0px\" cellspacing=\"0px\" class=\"statsTable\">");
                Output.WriteLine("  <tr align=\"left\" bgcolor=\"#0022a7\" >");

                string redirect_value = RequestSpecificValues.Current_Mode.Base_URL + "my/stats/" + year + month.ToString().PadLeft(2,'0');

                if (sort_value != 'a')
                {
                    Output.WriteLine("    <th align=\"left\"><a href=\"" + redirect_value + "a\"><span style=\"color: White\">TITLE</span></a></th>");
                }
                else
                {
                    Output.WriteLine("    <th align=\"left\"><span style=\"color: White\">TITLE</span></th>");
                }

                if (sort_value != 'b')
                {
                    Output.WriteLine("    <th align=\"center\"><a href=\"" + redirect_value + "b\"><span style=\"color: White\">TOTAL VIEWS</span></a></th>");
                }
                else
                {
                    Output.WriteLine("    <th align=\"center\"><span style=\"color: White\">TOTAL VIEWS</span></th>");
                }

                if (sort_value != 'c')
                {
                    Output.WriteLine("    <th align=\"center\"><a href=\"" + redirect_value + "c\"><span style=\"color: White\">TOTAL VISITS</span></a></th>");
                }
                else
                {
                    Output.WriteLine("    <th align=\"center\"><span style=\"color: White\">TOTAL VISITS</span></th>");
                }

                if (sort_value != 'd')
                {
                    Output.WriteLine("    <th align=\"center\"><a href=\"" + redirect_value + "d\"><span style=\"color: White\">MONTHLY VIEWS</span></a></th>");
                }
                else
                {
                    Output.WriteLine("    <th align=\"center\"><span style=\"color: White\">MONTHLY VIEWS</span></th>");
                }

                if (sort_value != 'e')
                {
                    Output.WriteLine("    <th align=\"center\"><a href=\"" + redirect_value + "e\"><span style=\"color: White\">MONTHLY VISITS</span></a></th>");
                }
                else
                {
                    Output.WriteLine("    <th align=\"center\"><span style=\"color: White\">MONTHLY VISITS</span></th>");
                }


                Output.WriteLine("  </tr>");

                // Keep track for a total row at the bottom
                int total_total_hits = 0;
                int total_total_sessions = 0;
                int total_month_hits = 0;
                int total_month_sessions = 0;

                // Display the stats for each item
                foreach (DataRowView thisRow in sortedView)
                {
                    string bibid = thisRow["BibID"].ToString();
                    string vid = thisRow["VID"].ToString();

                    Output.WriteLine("  <tr align=\"left\" >");
                    Output.WriteLine("    <td><a href=\"" + RequestSpecificValues.Current_Mode.Base_URL + bibid + "/" + vid + "/usage\" target=\"" + bibid+"_"+vid + "\">" + thisRow["Title"] + "</a></td>");
                    Output.WriteLine("    <td align=\"center\">" + thisRow["Total_Hits"] + "</td>");
                    Output.WriteLine("    <td align=\"center\">" + thisRow["Total_Sessions"] + "</td>");
                    Output.WriteLine("    <td align=\"center\">" + thisRow["Month_Hits"] + "</td>");
                    Output.WriteLine("    <td align=\"center\">" + thisRow["Month_Sessions"] + "</td>");
                    Output.WriteLine("  </tr>");
                    Output.WriteLine("  <tr><td bgcolor=\"#e7e7e7\" colspan=\"5\"></td></tr>");

                    // Also, add the values
                    total_total_hits += Convert.ToInt32(thisRow["Total_Hits"]);
                    total_total_sessions += Convert.ToInt32(thisRow["Total_Sessions"]);
                    total_month_hits += Convert.ToInt32(thisRow["Month_Hits"]);
                    total_month_sessions += Convert.ToInt32(thisRow["Month_Sessions"]);
                }

                Output.WriteLine("  <tr><td bgcolor=\"Black\" colspan=\"5\"></td></tr>");
                Output.WriteLine("  <tr align=\"left\" >");
                Output.WriteLine("    <td><strong>TOTAL</strong></td>");
                Output.WriteLine("    <td align=\"center\"><strong>" + total_total_hits + "</strong></td>");
                Output.WriteLine("    <td align=\"center\"><strong>" + total_total_sessions + "</strong></td>");
                Output.WriteLine("    <td align=\"center\"><strong>" + total_month_hits + "</strong></td>");
                Output.WriteLine("    <td align=\"center\"><strong>" + total_month_sessions + "</strong></td>");
                Output.WriteLine("  </tr>");



                Output.WriteLine("</table>");
                Output.WriteLine("</center>");
            }
            Output.WriteLine("<br /> <br />");
            Output.WriteLine("</div>");
        }

        private static string Month_From_Int(int Month_Int)
        {
            string monthString1 = "Invalid";
            switch (Month_Int)
            {
                case 1:
                    monthString1 = "January";
                    break;

                case 2:
                    monthString1 = "February";
                    break;

                case 3:
                    monthString1 = "March";
                    break;

                case 4:
                    monthString1 = "April";
                    break;

                case 5:
                    monthString1 = "May";
                    break;

                case 6:
                    monthString1 = "June";
                    break;

                case 7:
                    monthString1 = "July";
                    break;

                case 8:
                    monthString1 = "August";
                    break;

                case 9:
                    monthString1 = "September";
                    break;

                case 10:
                    monthString1 = "October";
                    break;

                case 11:
                    monthString1 = "November";
                    break;

                case 12:
                    monthString1 = "December";
                    break;
            }
            return monthString1;
        }

    }
}
