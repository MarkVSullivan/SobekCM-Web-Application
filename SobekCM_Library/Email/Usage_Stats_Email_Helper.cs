#region Using directives

using System;
using System.Data;
using System.Text;
using SobekCM.Library.Database;

#endregion

namespace SobekCM.Library.Email
{
    /// <summary> Static helper class assists with sending usage statistics emails for
    /// the monthly emails regarding individual usage on items which are linked to users </summary>
    public class Usage_Stats_Email_Helper
    {
        private const string EMAIL_SUBJECT = "Usage statistics for your materials ( <%DATE%> )";

        private const string EMAIL_BODY = "<p>You are receiving this message because you are a contributor to a digital library or a collection supported by the UF Libraries, including the Institutional Repository (IR@UF), the UF Digital Collections (UFDC), the Digital Library of the Caribbean (dLOC), and many others. If you do not wish to receive future messages, please <a href=\"http://ufdc.ufl.edu/my/preferences\">edit your account preferences</a> online or send an email to <a href=\"mailto:ufdc@uflib.ufl.edu\">ufdc@uflib.ufl.edu</a>. </p>" + "<p><strong>Usage statistics for your materials ( <%DATE%> )</strong></p>" + "<p><%NAME%>,</p>" + "<p>Thank you for sharing materials that will be accessible online and for supporting worldwide open access to scholarly, creative, and other works.  This is a usage report for the shared materials.</p>" + "<p>Your items have been viewed <%TOTAL%> times since they were added and were viewed <%MONTHLY%> times this month</p>" + "<%ITEMS%>" + "<p><em><a href=\"http://ufdc.ufl.edu/my/stats/<%YEAR%><%MONTH%>d\">Click here to see the usage statistics for all of your items. &gt;&gt;</a></em></p>" + "<p>Thank you for sharing these materials.  Please contact us with any questions ( <a href=\"mailto:ufdc@uflib.ufl.edu\">ufdc@uflib.ufl.edu</a> or 352-273-2900).</p>";

        private const string TOO_MANY_ITEMS_MESSAGE = "<p>Below are the details for your top 10 items.  See the link below to view usage statistics for all <%COUNT%> of your items.</p>";

        /// <summary> Sends one email to a user from the system including 
        /// individual usage on items which are linked to the user </summary>
        /// <param name="UserID"> Primary key for whom to send the email </param>
        /// <param name="User_Name"> Name of the user to include in the email </param>
        /// <param name="User_Email"> Email address to use for this email </param>
        /// <param name="Year"> Year of statistics to highlight in the email </param>
        /// <param name="Month"> Month of statistics to highlight in the email </param>
        /// <param name="Number_Of_Items_To_Include"> Number of items to include in this email, in case the user has many, many items linked </param>
        /// <param name="Base_URL"> Base URL to use for links to these items </param>
        /// <returns> TRUE if succesful, otherwise FALSE </returns>
        public static bool Send_Individual_Usage_Email(int UserID, string User_Name, string User_Email, int Year, int Month, int Number_Of_Items_To_Include, string Base_URL )
        {
            try
            {
                // Get the item usage stats for this user on this month
                DataTable usageStats = SobekCM_Database.Get_User_Linked_Items_Stats(UserID, Month, Year, null);

                // Only continue if stats were returned
                if (usageStats != null)
                {
                    // Use the data view
                    DataView sortedView = new DataView(usageStats) {Sort = "Month_Hits DESC"};

                    // Keep track for a total row at the bottom
                    int total_total_hits = 0;
                    int total_month_hits = 0;

                    // Build the string here
                    StringBuilder itemStatsBuilder = new StringBuilder();

                    // Display the stats for each item
                    int item_count = 0;
                    foreach (DataRowView thisRow in sortedView)
                    {
                        if (item_count < Number_Of_Items_To_Include)
                        {
                            string bibid = thisRow["BibID"].ToString();
                            string vid = thisRow["VID"].ToString();

                            itemStatsBuilder.AppendLine("<strong><a href=\"" + Base_URL + bibid + "/" + vid + "\">" + thisRow["Title"] + "</a></strong><br />");
                            itemStatsBuilder.AppendLine("<ul>");
                            itemStatsBuilder.AppendLine("  <li>Permanent Link: <a href=\"" + Base_URL + bibid + "/" + vid + "\">" + Base_URL + bibid + "/" + vid + "</a></li>");
                            itemStatsBuilder.AppendLine("  <li>Views");
                            itemStatsBuilder.AppendLine("    <ul>");
                            itemStatsBuilder.AppendLine("      <li>" + Month_From_Int(Month) + " " + Year + ": " + thisRow["Month_Hits"] + " views");
                            itemStatsBuilder.AppendLine("      <li>Total to date ( since " + Convert.ToDateTime(thisRow["CreateDate"]).ToShortDateString() + " ): " + thisRow["Total_Hits"] + " views");
                            itemStatsBuilder.AppendLine("    </ul>");
                            itemStatsBuilder.AppendLine("  </li>");
                            itemStatsBuilder.AppendLine("</ul>");
                        }

                        // Also, add the values
                        total_total_hits += Convert.ToInt32(thisRow["Total_Hits"]);
                        total_month_hits += Convert.ToInt32(thisRow["Month_Hits"]);
                        item_count++;
                    }

                    // Put some notes if there were more than 10 items
                    if (item_count > Number_Of_Items_To_Include)
                        itemStatsBuilder.Insert(0, TOO_MANY_ITEMS_MESSAGE.Replace("<%COUNT%>", item_count.ToString()));

                    string email_body_user = EMAIL_BODY.Replace("<%TOTAL%>", Number_To_String(total_total_hits)).Replace("<%MONTHLY%>", Number_To_String(total_month_hits)).Replace("<%ITEMS%>", itemStatsBuilder.ToString()).Replace("<%DATE%>", Month_From_Int(Month) + " " + Year).Replace("<%NAME%>", User_Name).Replace("<%YEAR%>", Year.ToString()).Replace("<%MONTH%>", Month.ToString().PadLeft(2, '0')) + "<br /><br /><p>( " + Month_From_Int(Month) + " " + Year + " )</p>";

                    // Only send the email if there was actually usage this month though
                    if (total_month_hits > 0)
                    {
                        // Send this email
                        return SobekCM_Database.Send_Database_Email(User_Email, EMAIL_SUBJECT.Replace("<%DATE%>", Month_From_Int(Month) + " " + Year), email_body_user, true, false, -1);
                    }
                }

                // No actual error here, just turns out no items were linked to this user
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        private static string Number_To_String(int Input)
        {
            string inputAsString = Input.ToString();
            return (inputAsString.Length <= 3) ? inputAsString : inputAsString.Substring(0, inputAsString.Length - 3) + "," + inputAsString.Substring(inputAsString.Length - 3);
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
