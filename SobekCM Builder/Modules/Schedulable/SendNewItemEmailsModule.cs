using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder.Modules.Schedulable
{
    public class SendNewItemEmailsModule : abstractSchedulableModule
    {
        public override void DoWork()
        {
            // NOTE: This was all brought over from the BulkLoader this way, it had been commented out ther as well.
            //       To get this to work, will need to do some development here, pull the list of impacted items, etc..

            //Add_NonError_To_Log("\t\tSending emails for new items");

            //// Get the list of institutions
            //DataTable institutions = SobekCM.Library.Database.SobekCM_Database.Get_All_Institutions();

            //// Step through each institution in the item list
            //foreach (string thisInstitution in items_by_location.Keys)
            //{
            //    // Determine the email address
            //    string email = String.Empty;
            //    string institution = String.Empty;

            //    // Determine if these are dLOC
            //    string software = "dLOC";
            //    string help_email = "dloc@fiu.edu";
            //    foreach (string html in items_by_location[thisInstitution])
            //    {
            //        if (html.IndexOf("dloc") < 0)
            //        {
            //            software = "UFDC";
            //            help_email = "ufdc@uflib.ufl.edu";
            //            break;
            //        }
            //    }

            //    if (thisInstitution.IndexOf("@") > 0)
            //    {
            //        email = thisInstitution;
            //        institution = "New " + software + " items";
            //    }
            //    else
            //    {
            //        // Search the list of institutions by the code
            //        DataRow[] selected = institutions.Select("InstitutionCode = '" + thisInstitution + "' and Load_Email_Notification=1");
            //        if (selected.Length > 0)
            //        {
            //            // Get the email and name of institution from the database
            //            email = selected[0]["Load_Email"].ToString();
            //            institution = "New " + software + " items for " + selected[0]["InstitutionName"].ToString();
            //        }
            //    }

            //    if (email.Length > 0)
            //    {
            //        // Get the name of the institution

            //        // Build the text for the email
            //        StringBuilder bodyBuilder = new StringBuilder();
            //        bodyBuilder.AppendLine("The following items are now available online in " + software + ".<br />");
            //        bodyBuilder.AppendLine("Los siguientes artículos, que usted mandó, están disponibles en línea a través de " + software + ".<br />");
            //        bodyBuilder.AppendLine("Les fichiers suivants ont été téléchargés sur le serveur " + software + ".<br />");
            //        bodyBuilder.AppendLine("<br />");
            //        bodyBuilder.AppendLine("<blockquote>");

            //        foreach (string html in items_by_location[thisInstitution])
            //        {
            //            bodyBuilder.AppendLine(html + "<br />");
            //        }

            //        bodyBuilder.AppendLine("</blockquote>");
            //        bodyBuilder.AppendLine("<br />");
            //        bodyBuilder.AppendLine("This is an automatic email.  Please do not respond to this email.  For any issues please contact " + help_email + ".<br />");
            //        bodyBuilder.AppendLine("Este correo se mandó automáticamente. Por favor no conteste este correo. Si usted tiene alguna pregunta o problema por favor mande un correo a " + help_email + ".<br />");
            //        bodyBuilder.AppendLine("Ceci est une réponse automatique. Veuillez ne pas répondre à ce message.  Envoyez-vos enquêtes directement à " + help_email + ".<br />");

            //        // Send this email
            //        try
            //        {
            //            System.Net.Mail.MailMessage myMail = new System.Net.Mail.MailMessage("ufdc_load@uflib.ufl.edu", email);
            //            myMail.Subject = institution;
            //            myMail.IsBodyHtml = true;
            //            myMail.Body = bodyBuilder.ToString();
            //            myMail.IsBodyHtml = true;

            //            // Mail this
            //            System.Net.Mail.SmtpClient client = new System.Net.Mail.SmtpClient("smtp.ufl.edu");
            //            client.Send(myMail);
            //        }
            //        catch (Exception ee)
            //        {
            //            string valu = "rerror";
            //        }
            //    }
            //}
        }
    }
}
