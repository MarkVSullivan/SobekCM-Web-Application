#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Web;
using SobekCM.Core;
using SobekCM.Core.BriefItem;
using SobekCM.Core.UI_Configuration.Citation;
using SobekCM.Engine_Library.Email;
using SobekCM.Library.UI;

#endregion

namespace SobekCM.Library.Email
{
    /// <summary> Helper class creates and sends the email when users 'share' a single digital resource </summary>
    public class Item_Email_Helper
    {
        /// <summary> Creates and sends the email when a user 'shares' a single digital resource </summary>
        /// <param name="Recepient_List"> Recepient list for this email </param>
        /// <param name="CcList"> CC list for this email </param>
        /// <param name="Comments"> Sender's comments to be included in the email </param>
        /// <param name="User_Name"> Name of the user that sent this email </param>
        /// <param name="SobekCM_Instance_Name"> Name of the current SobekCM instance (i.e., UDC, dLOC, etc..)</param>
        /// <param name="Item"> Digital resource to email </param>
        /// <param name="HTML_Format"> Tells if this should be sent as HMTL, otherwise it will be plain text </param>
        /// <param name="URL"> Direct URL for this item </param>
        /// <param name="UserID"> Primary key for the user that is sendig the email </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Send_Email( string Recepient_List, string CcList, string Comments, string User_Name, string SobekCM_Instance_Name, BriefItemInfo Item, bool HTML_Format, string URL, int UserID )
        {
            if (HTML_Format)
            {
                return HTML_Send_Email(Recepient_List, CcList, Comments, User_Name, SobekCM_Instance_Name, Item, URL, UserID) || Text_Send_Email(Recepient_List, CcList, Comments, User_Name, SobekCM_Instance_Name, Item, URL, UserID);
            }
            
            return Text_Send_Email(Recepient_List, CcList, Comments, User_Name, SobekCM_Instance_Name, Item, URL, UserID);
        }


        private static bool HTML_Send_Email(string Recepient_List, string CcList, string Comments, string User_Name, string SobekCM_Instance_Name, BriefItemInfo Item, string URL, int UserID)
        {
            try
            {
                StringBuilder messageBuilder = new StringBuilder();

                messageBuilder.AppendLine("<span style=\"font-family:Arial, Helvetica, sans-serif;\">");
                if (Comments.Length > 0)
                {
                    messageBuilder.AppendLine(User_Name + " wanted you to see this item on " + SobekCM_Instance_Name + " and included the following comments.<br /><br />\n");
                    messageBuilder.AppendLine(Comments.Replace("<", "(").Replace(">", ")").Replace("\"", "&quot;") + ".<br /><br />\n");
                }
                else
                {
                    messageBuilder.AppendLine(User_Name + " wanted you to see this item on " + SobekCM_Instance_Name + ".<br /><br />\n");
                }

                messageBuilder.AppendLine("<table cellspacing=\"0px\" cellpadding=\"0px\">\n");
                messageBuilder.AppendLine("<tr><td colspan=\"2\" style=\"background: black; color: white; font-family:Arial, Helvetica, sans-serif;\"><b>ITEM INFORMATION</b></td></tr>\n");

                // Include the thumbnail, if one exists
                if (String.IsNullOrEmpty(Item.Behaviors.Main_Thumbnail))
                    messageBuilder.AppendLine("<tr>");
                else
                    messageBuilder.AppendLine("<tr valign=\"top\"><td><a href=\"" + URL + "\"><img src=\"" + Item.Web.Source_URL.Replace("\\", "/") + "/" + Item.Behaviors.Main_Thumbnail + "\" alt=\"BLOCKED THUMBNAIL IMAGE\" border=\"1px\" /></a></td>\n");

                messageBuilder.AppendLine("<td>");
                messageBuilder.AppendLine("<table style=\"font-family:Arial, Helvetica, sans-serif; font-size:smaller;\">");



                // Step through the citation configuration here
                CitationSet citationSet = UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.Get_CitationSet("EMAIL");
                foreach (CitationFieldSet fieldsSet in citationSet.FieldSets)
                {
                    // Check to see if any of the values indicated in this field set exist
                    bool foundExistingData = false;
                    foreach (CitationElement thisField in fieldsSet.Elements)
                    {
                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = Item.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm != null) && (briefTerm.Values.Count > 0))
                        {
                            foundExistingData = true;
                            break;
                        }
                    }

                    // If no data was found to put in this field set, skip it
                    if (!foundExistingData)
                        continue;

                    // Step through all the fields in this field set and write them
                    foreach (CitationElement thisField in fieldsSet.Elements)
                    {
                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = Item.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm == null) || (briefTerm.Values.Count == 0))
                            continue;

                        // If they can all be listed one after the other do so now
                        if (!thisField.IndividualFields)
                        {
                            List<string> valueArray = new List<string>();
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {

                                if (String.IsNullOrEmpty(thisValue.Authority))
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value));
                                    }
                                    else
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Language + " )");
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + " )");
                                    }
                                    else
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )");
                                    }
                                }

                            }

                            // Now, add this to the citation HTML
                            Add_Citation_HTML_Rows(thisField.DisplayTerm, valueArray, messageBuilder);
                        }
                        else
                        {
                            // In this case, each individual value gets its own citation html row
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {
                                // Determine the label
                                string label = thisField.DisplayTerm;
                                if (thisField.OverrideDisplayTerm == CitationElement_OverrideDispayTerm_Enum.subterm)
                                {
                                    if (!String.IsNullOrEmpty(thisValue.SubTerm))
                                        label = thisValue.SubTerm;
                                }

                                if (String.IsNullOrEmpty(thisValue.Authority))
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        messageBuilder.Append(Single_Citation_HTML_Row(label, HttpUtility.HtmlEncode(thisValue.Value)));
                                    }
                                    else
                                    {
                                        messageBuilder.Append(Single_Citation_HTML_Row(label, HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Language + " )"));
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        messageBuilder.Append(Single_Citation_HTML_Row(label, HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + " )"));
                                    }
                                    else
                                    {
                                        messageBuilder.Append(Single_Citation_HTML_Row(label, HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )"));
                                    }
                                }

                            }
                        }
                    }
                }

                messageBuilder.AppendLine("</table>");
                messageBuilder.AppendLine("</td></tr></table>");

                messageBuilder.AppendLine("</span>\n");

                string[] email_recepients = Recepient_List.Split(";,".ToCharArray());
                string subject = Item.Title.Replace("&quot;", "\"");
                if (Item.Title.Length > 40)
                {
                    subject = Item.Title.Substring(0, 35).Replace("&quot;", "\"") + "...";
                }

                int error_count = 0;
                foreach (string thisReceipient in email_recepients)
                {
                    EmailInfo newEmail = new EmailInfo
                    {
                        Body = messageBuilder.ToString(),
                        isContactUs = false,
                        isHTML = true,
                        Subject = subject,
                        RecipientsList = thisReceipient,
                        FromAddress = SobekCM_Instance_Name + " <" + UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromAddress + ">",
                        UserID = UserID
                    };

                    if (! String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay))
                        newEmail.FromAddress = UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay + " <" + UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromAddress + ">";

                    if (CcList.Length > 0)
                        newEmail.RecipientsList = thisReceipient.Trim() + "," + CcList;

                    string error;
                    if (!Email_Helper.SendEmail(newEmail, out error))
                        error_count++;
                }
                return error_count <= 0;
            }
            catch
            {
                return false;
            }
        }

        private static void Add_Citation_HTML_Rows(string Row_Name, List<string> Values, StringBuilder Results)
        {
            // Only add if there is a value
            if (Values.Count <= 0) return;

            Results.Append("  <tr><td>" + Row_Name.ToUpper().Replace(" ", "_") + ": </td><td>");

            bool first = true;
            foreach (string thisValue in Values.Where(ThisValue => ThisValue.Length > 0))
            {
                if (first)
                {
                    Results.Append(thisValue);
                    first = false;
                }
                else
                {
                    Results.Append("<br />" + HttpUtility.HtmlEncode(thisValue));
                }
            }
            Results.AppendLine("</td></tr>");
        }

        private static string Single_Citation_HTML_Row(string Row_Name, string Value)
        {
            // Only add if there is a value
            if (Value.Length > 0)
            {
                return "<tr><td>" + Row_Name + ":</td><td>" + HttpUtility.HtmlEncode( Value ) + "</td></tr>" + Environment.NewLine;
            }
            return String.Empty;
        }

        private static bool Text_Send_Email(string Recepient_List, string CcList, string Comments, string User_Name, string SobekCM_Instance_Name, BriefItemInfo Item, string URL, int UserID )
        {
            try
            {
  
                StringBuilder messageBuilder = new StringBuilder();

                if (Comments.Length > 0)
                {
                    messageBuilder.Append(User_Name + " wanted you to see this item on " + SobekCM_Instance_Name + " and included the following comments:\n\n");
                    messageBuilder.Append("\"" + Comments + "\"\n\n");
                }
                else
                {
                    messageBuilder.Append(User_Name + " wanted you to see this item on " + SobekCM_Instance_Name + ".\n\n");
                }
                messageBuilder.Append("ITEM INFORMATION\n");
                messageBuilder.Append("------------------------------------------------------\n");


                // Step through the citation configuration here
                CitationSet citationSet = UI_ApplicationCache_Gateway.Configuration.UI.CitationViewer.Get_CitationSet("EMAIL");
                foreach (CitationFieldSet fieldsSet in citationSet.FieldSets)
                {
                    // Check to see if any of the values indicated in this field set exist
                    bool foundExistingData = false;
                    foreach (CitationElement thisField in fieldsSet.Elements)
                    {
                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = Item.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm != null) && (briefTerm.Values.Count > 0))
                        {
                            foundExistingData = true;
                            break;
                        }
                    }

                    // If no data was found to put in this field set, skip it
                    if (!foundExistingData)
                        continue;

                    // Step through all the fields in this field set and write them
                    foreach (CitationElement thisField in fieldsSet.Elements)
                    {
                        // Look for a match in the item description
                        BriefItem_DescriptiveTerm briefTerm = Item.Get_Description(thisField.MetadataTerm);

                        // If no match, just continue
                        if ((briefTerm == null) || (briefTerm.Values.Count == 0))
                            continue;

                        // If they can all be listed one after the other do so now
                        if (!thisField.IndividualFields)
                        {
                            List<string> valueArray = new List<string>();
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {

                                if (String.IsNullOrEmpty(thisValue.Authority))
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value));
                                    }
                                    else
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Language + " )");
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + " )");
                                    }
                                    else
                                    {
                                        valueArray.Add(HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )");
                                    }
                                }

                            }

                            // Now, add this to the citation HTML
                            Add_Citation_Text_Rows(thisField.DisplayTerm, valueArray, messageBuilder);
                        }
                        else
                        {
                            // In this case, each individual value gets its own citation html row
                            foreach (BriefItem_DescTermValue thisValue in briefTerm.Values)
                            {
                                // Determine the label
                                string label = thisField.DisplayTerm;
                                if (thisField.OverrideDisplayTerm == CitationElement_OverrideDispayTerm_Enum.subterm)
                                {
                                    if (!String.IsNullOrEmpty(thisValue.SubTerm))
                                        label = thisValue.SubTerm;
                                }

                                if (String.IsNullOrEmpty(thisValue.Authority))
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        messageBuilder.Append(Single_Citation_Text_Row(label, HttpUtility.HtmlEncode(thisValue.Value)));
                                    }
                                    else
                                    {
                                        messageBuilder.Append(Single_Citation_Text_Row(label, HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Language + " )"));
                                    }
                                }
                                else
                                {
                                    if (String.IsNullOrEmpty(thisValue.Language))
                                    {
                                        messageBuilder.Append(Single_Citation_Text_Row(label, HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + " )"));
                                    }
                                    else
                                    {
                                        messageBuilder.Append(Single_Citation_Text_Row(label, HttpUtility.HtmlEncode(thisValue.Value) + " ( " + thisValue.Authority + ", " + thisValue.Language + " )"));
                                    }
                                }

                            }
                        }
                    }
                }


                string[] email_recepients = Recepient_List.Split(";,".ToCharArray());
                string subject = Item.Title.Replace("&quot;", "\"");
                if (Item.Title.Length > 40)
                {
                    subject = Item.Title.Substring(0, 35).Replace("&quot;", "\"") + "...";
                }

                int error_count = 0;
                foreach (string thisReceipient in email_recepients)
                {
                    EmailInfo newEmail = new EmailInfo
                    {
                        Body = messageBuilder.ToString(),
                        isContactUs = false,
                        isHTML = false,
                        Subject = subject,
                        RecipientsList = thisReceipient,
                        FromAddress = SobekCM_Instance_Name + " <" + UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromAddress + ">",
                        UserID = UserID
                    };

                    if (!String.IsNullOrEmpty(UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay))
                        newEmail.FromAddress = UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromDisplay + " <" + UI_ApplicationCache_Gateway.Settings.Email.Setup.DefaultFromAddress + ">";


                    if (CcList.Length > 0)
                        newEmail.RecipientsList = thisReceipient.Trim() + "," + CcList;

                    string error;
                    if (!Email_Helper.SendEmail(newEmail, out error))
                        error_count++;
                }
                return error_count <= 0;
            }
            catch 
            {
                return false;
            }
        }

        private static void Add_Citation_Text_Rows(string Row_Name, List<string> Values, StringBuilder Results)
        {
            // Only add if there is a value
            if (Values.Count <= 0) return;

            Results.Append("\t" + Row_Name.ToUpper().Replace(" ", "_").PadRight(30, ' ') + ":  ");

            bool first = true;
            foreach (string thisValue in Values.Where(ThisValue => ThisValue.Length > 0))
            {
                if (first)
                {
                    Results.Append(thisValue);
                    first = false;
                }
                else
                {
                    Results.Append(Environment.NewLine + "\t" + String.Empty.PadRight(30, ' ') + "   " + HttpUtility.HtmlEncode(thisValue));
                }
            }
            Results.AppendLine();
        }

        private static string Single_Citation_Text_Row(string Row_Name, string Value)
        {
            // Only add if there is a value
            if (Value.Length > 0)
            {
                return "\t" + Row_Name.ToUpper().Replace(" ", "_").PadRight(30, ' ') + ":  " + HttpUtility.HtmlEncode(Value) + Environment.NewLine;
            }
            return String.Empty;
        }
    }
}
