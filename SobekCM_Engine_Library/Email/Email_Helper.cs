#region Using directives

using System;
using System.Net.Mail;
using SobekCM.Core;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;

#endregion

namespace SobekCM.Engine_Library.Email
{
    /// <summary> Class is used to send emails according to this instance of SobekCM's settings </summary>
    public static class Email_Helper
    {
        /// <summary> Sends an email using the currently selected email method ( i.e., direct SMTP or database mail ) </summary>
        /// <param name="ToAddress"> To address for the email to send </param>
        /// <param name="Subject"> Subject line for the email to send </param>
        /// <param name="Body"> Body of the email to send  </param>
        /// <param name="isHtml"> If set to <c>true</c>, send the email as HTML format (vs. text format) </param>
        /// <param name="InstanceName"> Name of the current SobekCM instance </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool SendEmail(string ToAddress, string Subject, string Body, bool isHtml, string InstanceName)
        {
            EmailInfo newEmail = new EmailInfo {RecipientsList = ToAddress, Subject = Subject, Body = Body, isHTML = isHtml};

            if (String.IsNullOrEmpty(Engine_ApplicationCache_Gateway.Settings.EmailDefaultFromDisplay))
            {
                newEmail.FromAddress = InstanceName + " <" + Engine_ApplicationCache_Gateway.Settings.EmailDefaultFromAddress + ">";
            }
            else
            {
                newEmail.FromAddress = Engine_ApplicationCache_Gateway.Settings.EmailDefaultFromDisplay + " <" + Engine_ApplicationCache_Gateway.Settings.EmailDefaultFromAddress + ">";
 
            }

            string ignore_error_msg;

            return SendEmail(newEmail, out ignore_error_msg);
        }

        /// <summary> Send email according to this instance's email settings </summary>
        /// <param name="Email"> Complete information for this email </param>
        /// <param name="Error"> [OUT] Any error message encountered </param>
        /// <returns> TRUE if successfully queued, otherwise FALSE </returns>
        public static bool SendEmail(EmailInfo Email, out string Error )
        {
            Error = String.Empty;
            int replyId = -1;
            if (Email.ReplayToEmailID.HasValue)
                replyId = Email.ReplayToEmailID.Value;
            int userId = -1;
            if (Email.UserID.HasValue)
                userId = Email.UserID.Value;

            if (Engine_ApplicationCache_Gateway.Settings.EmailMethod == Email_Method_Enum.MsSqlDatabaseMail)
            {
                try
                {
                    // Send database email
                    Engine_Database.Send_Database_Email(Email.RecipientsList, Email.Subject, Email.Body, Email.FromAddress, Email.ReplyTo, Email.isHTML, Email.isContactUs, replyId, userId);
                }
                catch (Exception ee)
                {
                    Error = ee.Message;
                    return false;
                }
            }
            else
            {
                try
                {
                    string email_name = Engine_ApplicationCache_Gateway.Settings.EmailDefaultFromDisplay;
                    string email_address = Engine_ApplicationCache_Gateway.Settings.EmailDefaultFromAddress;
                    if (!String.IsNullOrEmpty(Email.FromAddress))
                    {
                        string[] splitter = Email.FromAddress.Trim().Split("<>".ToCharArray());
                        if (splitter.Length == 2)
                        {
                            email_name = splitter[0].Trim();
                            email_address = splitter[1].Trim();
                        }
                        else
                        {
                            email_address = Email.FromAddress;
                        }
                    }

                    MailMessage myMail = new MailMessage(Email.FromAddress, Email.RecipientsList)
                    {
                        Subject = Email.Subject,
                        Body = Email.Body
                    };


                    if (!String.IsNullOrEmpty(email_name))
                    {
                        myMail.Sender = new MailAddress(email_address, email_name);
                    }

                    // Mail this
                    SmtpClient client = new SmtpClient(Engine_ApplicationCache_Gateway.Settings.EmailSmtpServer, Engine_ApplicationCache_Gateway.Settings.EmailSmtpPort);
                    client.Send(myMail);

                    // Log this in the database
                    Engine_Database.Log_Sent_Email(Email.FromAddress, Email.RecipientsList, Email.Subject, Email.Body, Email.isHTML, Email.isContactUs, -1);

                }
                catch (Exception ee)
                {
                    Error = ee.Message;
                    return false;
                }
            }

            return true;
        }
    }
}
