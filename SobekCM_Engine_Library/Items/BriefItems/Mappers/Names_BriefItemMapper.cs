using System;
using System.Text;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the names ( i.e., creator, contributor, etc.. ) from the METS-based 
    /// SobekCM_Item object to the BriefItem, used for most the public functions of the front-end </summary>
    public class Names_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {

            // Add the main entity first
            if (Original.Bib_Info.hasMainEntityName) 
            {
                // Is this a conference?
                if (Original.Bib_Info.Main_Entity_Name.Name_Type == Name_Info_Type_Enum.conference)
                {
                    if (!String.IsNullOrWhiteSpace(Original.Bib_Info.Main_Entity_Name.Full_Name))
                        New.Add_Description("Conference", Original.Bib_Info.Main_Entity_Name.ToString());
                }
                else
                {
                    // Build the full name and info
                    Name_Info thisName = Original.Bib_Info.Main_Entity_Name;
                    StringBuilder nameBuilder = new StringBuilder();
                    if ( !String.IsNullOrWhiteSpace(thisName.Full_Name))
                    {
                        nameBuilder.Append(thisName.Full_Name.Replace("|", " -- "));
                    }
                    else
                    {
                        if ( !String.IsNullOrWhiteSpace(thisName.Family_Name))
                        {
                            if (!String.IsNullOrWhiteSpace(thisName.Given_Name))
                            {
                                nameBuilder.Append(thisName.Family_Name + ", " + thisName.Given_Name);
                            }
                            else
                            {
                                nameBuilder.Append(thisName.Family_Name);
                            }
                        }
                        else
                        {
                            nameBuilder.Append(!String.IsNullOrWhiteSpace(thisName.Given_Name) ? thisName.Given_Name : "unknown");
                        }
                    }

                    // Add the display form and dates
                    if (thisName.Display_Form.Length > 0)
                        nameBuilder.Append(" ( " + thisName.Display_Form + " )");
                    if (thisName.Dates.Length > 0)
                        nameBuilder.Append(", " + thisName.Dates);

                    // Add with the sub-roles as well
                    string roles = thisName.Role_String;
                    if (!String.IsNullOrWhiteSpace(roles))
                        New.Add_Description("Creator", nameBuilder.ToString()).SubTerm = roles;
                    else
                        New.Add_Description("Creator", nameBuilder.ToString());
                }
            }

            // Add all the other names attached
            if (Original.Bib_Info.Names_Count > 0)
            {
                foreach (Name_Info thisName in Original.Bib_Info.Names)
                {
                    // Is this a conference?
                    if (thisName.Name_Type == Name_Info_Type_Enum.conference)
                    {
                        if (!String.IsNullOrWhiteSpace(thisName.Full_Name))
                            New.Add_Description("Conference", thisName.ToString());
                    }
                    else
                    {
                        // Build the full name and info
                        StringBuilder nameBuilder = new StringBuilder();
                        if (!String.IsNullOrWhiteSpace(thisName.Full_Name))
                        {
                            nameBuilder.Append(thisName.Full_Name.Replace("|", " -- "));
                        }
                        else
                        {
                            if (!String.IsNullOrWhiteSpace(thisName.Family_Name))
                            {
                                if (!String.IsNullOrWhiteSpace(thisName.Given_Name))
                                {
                                    nameBuilder.Append(thisName.Family_Name + ", " + thisName.Given_Name);
                                }
                                else
                                {
                                    nameBuilder.Append(thisName.Family_Name);
                                }
                            }
                            else
                            {
                                nameBuilder.Append(!String.IsNullOrWhiteSpace(thisName.Given_Name) ? thisName.Given_Name : "unknown");
                            }
                        }

                        // Add the display form and dates
                        if (thisName.Display_Form.Length > 0)
                            nameBuilder.Append(" ( " + thisName.Display_Form + " )");
                        if (thisName.Dates.Length > 0)
                            nameBuilder.Append(", " + thisName.Dates);

                        // Add with the sub-roles as well
                        string roles = thisName.Role_String;
                        if (!String.IsNullOrWhiteSpace(roles))
                            New.Add_Description("Creator", nameBuilder.ToString()).SubTerm = roles;
                        else
                            New.Add_Description("Creator", nameBuilder.ToString());
                    }
                }
            }

            return true;
        }
    }
}
