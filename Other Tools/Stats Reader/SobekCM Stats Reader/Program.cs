#region Using directives

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using SobekCM.Library.Email;

#endregion

namespace SobekCM_Stats_Reader
{
    internal class Program
    {

        [STAThread]
        private static void Main(string[] args)
        {

            // Set the visual rendering
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            Control.CheckForIllegalCrossThreadCalls = false;

            // Set some defaults for round buttons
            Round_Button.Inactive_Border_Color = Color.DarkGray;
            Round_Button.Inactive_Text_Color = Color.White;
            Round_Button.Inactive_Fill_Color = Color.DarkGray;
            Round_Button.Mouse_Down_Border_Color = Color.Gray;
            Round_Button.Mouse_Down_Text_Color = Color.White;
            Round_Button.Mouse_Down_Fill_Color = Color.Gray;
            Round_Button.Active_Border_Color = Color.FromArgb(25, 68, 141);
            Round_Button.Active_Fill_Color = Color.FromArgb(25, 68, 141);
            Round_Button.Active_Text_Color = Color.White;


            //int usage_email_year = 2013;
            //int usage_email_month = 10;
            //DialogResult result = MessageBox.Show("Are you sure you want to send usage emails for " + usage_email_month + " / " +
            //                                      usage_email_year + "?", "Confirm Email Request", MessageBoxButtons.YesNoCancel, MessageBoxIcon.Question);
            //if (result == DialogResult.Yes)
            //{
            //    int emails_sent = Send_Usage_Emails(usage_email_year, usage_email_month);
            //    MessageBox.Show("Sent " + emails_sent + " usage stats emails.  ", "COMPLETE", MessageBoxButtons.OK, MessageBoxIcon.Information);
            //}
            //return;
            




            Stats_Setup_Form setupForm = new Stats_Setup_Form();
            setupForm.ShowDialog();


            





            Console.WriteLine("COMPLETE");
        }

        public static int Send_Usage_Emails(int year, int month)
        {
            // Get the database connection from the app settings for this app 
            string database_source = "lib-sobekdb\\SobekCM"; //ConfigurationSettings.AppSettings["Database_Source"];
            string database_name = "SobekDB"; //ConfigurationSettings.AppSettings["Database_Name"];

            string database_string = "data source=" + database_source + ";initial catalog=" + database_name +
                                     ";integrated security=Yes;";
            SobekCM.Library.Database.SobekCM_Database.Connection_String = database_string;

            //SobekCM.Library.Usage_Stats_Email_Helper.Send_Individual_Usage_Email(935, "Brian Keith", "marsull@uflib.ufl.edu", 2011, 9, 10, "http://ufdc.ufl.edu/");


            // Get the list of all users linked to items
            DataTable usersLinkedToItems = SobekCM.Library.Database.SobekCM_Database.Get_Users_Linked_To_Items(null);

            // Step through each row and pull data about the usage stats for this user
            int emails_sent = 0;
            foreach (DataRow thisRow in usersLinkedToItems.Rows)
            {
                System.Threading.Thread.Sleep(1000);

                // Pull out the user information from this row
                string firstName = thisRow[0].ToString();
                string lastName = thisRow[1].ToString();
                string nickName = thisRow[2].ToString();
                string userName = thisRow[3].ToString();
                int userid = Convert.ToInt32(thisRow[4]);
                string email = thisRow[5].ToString();

                // Compose the name
                string name = firstName + " " + lastName;
                if (nickName.Length > 0)
                    name = nickName + " " + lastName;

                // Try to compose and send the email.  The email will only be sent if there was 
                // some total usage this month, as well as total
                if (Usage_Stats_Email_Helper.Send_Individual_Usage_Email(userid, name, email, year, month, 10, "http://ufdc.ufl.edu/"))
                    emails_sent++;
            }

            return emails_sent;
        }



    }
}