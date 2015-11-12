#region Using directives

using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Core;
using SobekCM.Engine_Library.Email;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;

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
        public static bool Send_Email( string Recepient_List, string CcList, string Comments, string User_Name, string SobekCM_Instance_Name, SobekCM_Item Item, bool HTML_Format, string URL, int UserID )
        {
            if (HTML_Format)
            {
                return HTML_Send_Email(Recepient_List, CcList, Comments, User_Name, SobekCM_Instance_Name, Item, URL, UserID) || Text_Send_Email(Recepient_List, CcList, Comments, User_Name, SobekCM_Instance_Name, Item, URL, UserID);
            }
            
            return Text_Send_Email(Recepient_List, CcList, Comments, User_Name, SobekCM_Instance_Name, Item, URL, UserID);
        }


        private static bool HTML_Send_Email(string Recepient_List, string CcList, string Comments, string User_Name, string SobekCM_Instance_Name, SobekCM_Item Item, string URL, int UserID )
        {
            try
            {
                // Collect the titles
                List<string> uniform_titles = new List<string>();
                List<string> alternative_titles = new List<string>();
                List<string> translated_titles = new List<string>();
                List<string> abbreviated_titles = new List<string>();
                if (Item.Bib_Info.Other_Titles_Count > 0)
                {
                    foreach (Title_Info thisTitle in Item.Bib_Info.Other_Titles)
                    {
                        switch (thisTitle.Title_Type)
                        {
                            case Title_Type_Enum.UNSPECIFIED:
                            case Title_Type_Enum.Alternative:
                                alternative_titles.Add(thisTitle.ToString());
                                break;

                            case Title_Type_Enum.Uniform:
                                uniform_titles.Add(thisTitle.ToString());
                                break;

                            case Title_Type_Enum.Translated:
                                translated_titles.Add(thisTitle.ToString());
                                break;

                            case Title_Type_Enum.Abbreviated:
                                abbreviated_titles.Add(thisTitle.ToString());
                                break;
                        }
                    }
                }

                List<string> subjects = new List<string>();
                List<string> hierGeo = new List<string>();
                if (Item.Bib_Info.Subjects_Count > 0)
                {
                    foreach (Subject_Info thisSubject in Item.Bib_Info.Subjects)
                    {
                        switch (thisSubject.Class_Type)
                        {
                            case Subject_Info_Type.Hierarchical_Spatial:
                                hierGeo.Add(thisSubject.ToString());
                                break;

                            default:
                                subjects.Add(thisSubject.ToString());
                                break;
                        }
                    }
                }

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
                    messageBuilder.AppendLine("<tr valign=\"top\"><td><a href=\"" + URL + "\"><img src=\"" + UI_ApplicationCache_Gateway.Settings.Servers.Image_URL + Item.Web.AssocFilePath.Replace("\\", "/") + "/" + Item.Behaviors.Main_Thumbnail + "\" alt=\"BLOCKED THUMBNAIL IMAGE\" border=\"1px\" /></a></td>\n");

                messageBuilder.AppendLine("<td>");
                messageBuilder.AppendLine("<table style=\"font-family:Arial, Helvetica, sans-serif; font-size:smaller;\">");
                messageBuilder.AppendLine("<tr><td>Title:</td><td><a href=\"" + URL + "\"><b>" + Item.Bib_Info.Main_Title + "</b></a></td></tr>");

                if (( Item.Bib_Info.hasSeriesTitle ) && ( Item.Bib_Info.SeriesTitle.Title.Length > 0))
                {
                    messageBuilder.AppendLine("<tr><td>Series Title:</td><td>" + Item.Bib_Info.SeriesTitle + "</td></tr>\n");
                }

                bool first_data = true;
                foreach (string title in uniform_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.AppendLine("<tr><td>Uniform Title:\t" + title + "</td></tr>\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + title + "</td></tr>\n");
                    }
                }

                first_data = true;
                foreach (string title in alternative_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.AppendLine("<tr><td>Alternate Title:</td><td>" + title + "</td></tr>\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + title.Replace("<i>", "") + "</td></tr>\n");
                    }
                }

                first_data = true;
                foreach (string title in translated_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.AppendLine("<tr><td>Translated Title:</td><td>" + title + "</td></tr>\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + title + "</td></tr>\n");
                    }
                }

                first_data = true;
                foreach (string title in abbreviated_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.AppendLine("<tr><td>Abbreviated Title:</td><td>" + title + "</td></tr>\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + title + "</td></tr>\n");
                    }
                }

                if (( Item.Bib_Info.hasMainEntityName ) && ( Item.Bib_Info.Main_Entity_Name.hasData))
                {
                    messageBuilder.AppendLine("<tr><td>Creator:</td><td>" + Item.Bib_Info.Main_Entity_Name + "</td></tr>\n");
                    first_data = false;
                }

                if (Item.Bib_Info.Names_Count > 0)
                {
                    foreach (Name_Info thisName in Item.Bib_Info.Names)
                    {
                        if (first_data)
                        {
                            messageBuilder.AppendLine("<tr><td>Creator:</td><td>" + thisName + "</td></tr>\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + thisName + "</td></tr>\n");
                        }
                    }
                }
                if (Item.Bib_Info.Publishers_Count > 0)
                {
                    first_data = true;
                    foreach (Publisher_Info thisPublisher in Item.Bib_Info.Publishers)
                    {
                        if (first_data)
                        {
                            messageBuilder.AppendLine("<tr><td>Publisher:</td><td>" + thisPublisher + "</td></tr>\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + thisPublisher + "</td></tr>\n");
                        }

                    }
                }
                if (Item.Bib_Info.Origin_Info.Date_Issued.Length > 0)
                {
                    messageBuilder.AppendLine("<tr><td>Date:</td><td>" + Item.Bib_Info.Origin_Info.Date_Issued + "</td></tr>\n");
                }
                else
                {
                    if (Item.Bib_Info.Origin_Info.MARC_DateIssued.Length > 0)
                    {
                        messageBuilder.AppendLine("<tr><td>Date:</td><td>" + Item.Bib_Info.Origin_Info.MARC_DateIssued + "</td></tr>\n");
                    }
                }
                if (Item.Bib_Info.Original_Description.Extent.Length > 0)
                {
                    messageBuilder.AppendLine("<tr><td>Description:</td><td>" + Item.Bib_Info.Original_Description.Extent + " ( " + Item.Bib_Info.SobekCM_Type + " )</td></tr>\n");
                }
                else
                {
                    messageBuilder.AppendLine("<tr><td>Description:</td><td>" + Item.Bib_Info.SobekCM_Type_String + "</td></tr>\n");
                }
                if (subjects.Count > 0)
                {
                    first_data = true;
                    foreach (string thisSubject in subjects)
                    {
                        if (first_data)
                        {
                            messageBuilder.AppendLine("<tr><td>Subject:</td><td>" + thisSubject + "</td></tr>\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + thisSubject + "</td></tr>\n");
                        }
                    }
                }

                if (Item.Bib_Info.Genres_Count > 0)
                {
                    first_data = true;
                    foreach (Genre_Info thisGenre in Item.Bib_Info.Genres)
                    {
                        if (first_data)
                        {
                            messageBuilder.AppendLine("<tr><td>Genre:</td><td>" + thisGenre + "</td></tr>\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + thisGenre + "</td></tr>\n");
                        }
                    }
                }

                if (hierGeo.Count > 0)
                {
                    first_data = true;
                    foreach (string thisSubject in hierGeo)
                    {
                        if (first_data)
                        {
                            messageBuilder.AppendLine("<tr><td>Spatial Coverage:</td><td>" + thisSubject + "</td></tr>\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.AppendLine("<tr><td>&nbsp;</td><td>" + thisSubject + "</td></tr>\n");
                        }
                    }
                }

                if (Item.Bib_Info.Access_Condition.Text.Length > 0)
                {
                    messageBuilder.AppendLine("<tr><td>Rights:</td><td>" + Item.Bib_Info.Access_Condition.Text + "</td></tr>\n");
                }
                messageBuilder.AppendLine("</table>");
                messageBuilder.AppendLine("</td></tr></table>");

                messageBuilder.AppendLine("</span>\n");

                string[] email_recepients = Recepient_List.Split(";,".ToCharArray());
                string subject = Item.Bib_Info.Main_Title.ToString().Replace("&quot;", "\"");
                if (Item.Bib_Info.Main_Title.Title.Length > 40)
                {
                    subject = Item.Bib_Info.Main_Title.ToString().Substring(0, 35).Replace("&quot;", "\"") + "...";
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

        private static bool Text_Send_Email(string Recepient_List, string CcList, string Comments, string User_Name, string SobekCM_Instance_Name, SobekCM_Item Item, string URL, int UserID )
        {
            try
            {
                // Collect the titles
                List<string> uniform_titles = new List<string>();
                List<string> alternative_titles = new List<string>();
                List<string> translated_titles = new List<string>();
                List<string> abbreviated_titles = new List<string>();
                if (Item.Bib_Info.Other_Titles_Count > 0)
                {
                    foreach (Title_Info thisTitle in Item.Bib_Info.Other_Titles)
                    {
                        switch (thisTitle.Title_Type)
                        {
                            case Title_Type_Enum.UNSPECIFIED:
                            case Title_Type_Enum.Alternative:
                                alternative_titles.Add(thisTitle.ToString());
                                break;

                            case Title_Type_Enum.Uniform:
                                uniform_titles.Add(thisTitle.ToString());
                                break;

                            case Title_Type_Enum.Translated:
                                translated_titles.Add(thisTitle.ToString());
                                break;

                            case Title_Type_Enum.Abbreviated:
                                abbreviated_titles.Add(thisTitle.ToString());
                                break;
                        }
                    }
                }

                List<string> subjects = new List<string>();
                List<string> hierGeo = new List<string>();
                if (Item.Bib_Info.Subjects_Count > 0)
                {
                    foreach (Subject_Info thisSubject in Item.Bib_Info.Subjects)
                    {
                        switch (thisSubject.Class_Type)
                        {
                            case Subject_Info_Type.Hierarchical_Spatial:
                                hierGeo.Add(thisSubject.ToString());
                                break;

                            default:
                                subjects.Add(thisSubject.ToString());
                                break;
                        }
                    }
                }

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
                messageBuilder.Append("\tURL:\t\t\t" + URL + "\n");
                messageBuilder.Append("\tTitle:\t\t" + Item.Bib_Info.Main_Title.ToString().Replace("&quot;", "\"") + "\n");

                if (( Item.Bib_Info.hasSeriesTitle ) && ( Item.Bib_Info.SeriesTitle.Title.Length > 0))
                {
                    messageBuilder.Append("\tSeries Title:\t" + Item.Bib_Info.SeriesTitle.ToString().Replace("&quot;", "\"") + "\n");
                }

                bool first_data = true;
                foreach (string title in uniform_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.Append("\tUniform Title:\t" + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.Append("\t\t\t\t" + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                    }
                }

                first_data = true;
                foreach (string title in alternative_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.Append("\tAlternate Title:\t" + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.Append("\t\t\t\t" + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                    }
                }

                first_data = true;
                foreach (string title in translated_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.Append("\tTranslated Title: " + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.Append("\t\t\t\t" + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                    }
                }

                first_data = true;
                foreach (string title in abbreviated_titles)
                {
                    if (first_data)
                    {
                        messageBuilder.Append("\tAbbreviated Title:" + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                        first_data = false;
                    }
                    else
                    {
                        messageBuilder.Append("\t\t\t\t" + title.Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                    }
                }

                if ((Item.Bib_Info.hasMainEntityName) && (Item.Bib_Info.Main_Entity_Name.hasData))
                {
                    messageBuilder.Append("\tCreator:\t\t" + Item.Bib_Info.Main_Entity_Name.ToString().Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                    first_data = false;
                }

                if (Item.Bib_Info.Names_Count > 0)
                {
                    foreach (Name_Info thisName in Item.Bib_Info.Names)
                    {
                        if (first_data)
                        {
                            messageBuilder.Append("\tCreator:\t\t" + thisName.ToString().Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.Append("\t\t\t\t" + thisName.ToString().Replace("<i>", "").Replace("</i>", "").Replace("&quot;", "\"") + "\n");
                        }
                    }
                }
                if (Item.Bib_Info.Publishers_Count > 0)
                {
                    first_data = true;
                    foreach (Publisher_Info thisPublisher in Item.Bib_Info.Publishers)
                    {
                        if (first_data)
                        {
                            messageBuilder.Append("\tPublisher:\t\t" + thisPublisher + "\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.Append("\t\t\t\t" + thisPublisher + "\n");
                        }

                    }
                }
                if (Item.Bib_Info.Origin_Info.Date_Issued.Length > 0)
                {
                    messageBuilder.Append("\tDate:\t\t\t" + Item.Bib_Info.Origin_Info.Date_Issued + "\n");
                }
                else
                {
                    if (Item.Bib_Info.Origin_Info.MARC_DateIssued.Length > 0)
                    {
                        messageBuilder.Append("\tDate:\t\t\t" + Item.Bib_Info.Origin_Info.MARC_DateIssued + "\n");
                    }
                }
                if (Item.Bib_Info.Original_Description.Extent.Length > 0)
                {
                    messageBuilder.Append("\tDescription:\t" + Item.Bib_Info.Original_Description.Extent + " ( " + Item.Bib_Info.SobekCM_Type_String + " )\n");
                }
                else
                {
                    messageBuilder.Append("\tDescription:\t" + Item.Bib_Info.SobekCM_Type_String + "\n");
                }
                if (subjects.Count > 0)
                {
                    first_data = true;
                    foreach (string thisSubject in subjects)
                    {
                        if (first_data)
                        {
                            messageBuilder.Append("\tSubject:\t\t" + thisSubject.Replace("<i>", "").Replace("</i>", "") + "\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.Append("\t\t\t\t" + thisSubject.Replace("<i>", "").Replace("</i>", "") + "\n");
                        }
                    }
                }

                if (Item.Bib_Info.Genres_Count > 0)
                {
                    first_data = true;
                    foreach (Genre_Info thisGenre in Item.Bib_Info.Genres)
                    {
                        if (first_data)
                        {
                            messageBuilder.Append("\tGenre:\t\t" + thisGenre.ToString().Replace("<i>", "").Replace("</i>", "") + "\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.Append("\t\t\t\t" + thisGenre.ToString().Replace("<i>", "").Replace("</i>", "") + "\n");
                        }
                    }
                }

                if (hierGeo.Count > 0)
                {
                    first_data = true;
                    foreach (string thisSubject in hierGeo)
                    {
                        if (first_data)
                        {
                            messageBuilder.Append("\tSpatial Coverage: " + thisSubject.Replace("<i>", "").Replace("</i>", "") + "\n");
                            first_data = false;
                        }
                        else
                        {
                            messageBuilder.Append("\t\t\t\t" + thisSubject.Replace("<i>", "").Replace("</i>", "") + "\n");
                        }
                    }
                }

                if (Item.Bib_Info.Access_Condition.Text.Length > 0)
                {
                    messageBuilder.Append("\tRights:\t\t" + Item.Bib_Info.Access_Condition.Text + "\n");
                }


                messageBuilder.Append("</span>\n");

                string[] email_recepients = Recepient_List.Split(";,".ToCharArray());
                string subject = Item.Bib_Info.Main_Title.ToString().Replace("&quot;", "\"");
                if (Item.Bib_Info.Main_Title.Title.Length > 40)
                {
                    subject = Item.Bib_Info.Main_Title.ToString().Substring(0, 35).Replace("&quot;", "\"") + "...";
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
    }
}
