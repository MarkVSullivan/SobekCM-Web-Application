#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Library.Application_State;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows entry of the creator(s) name and role for an item </summary>
    /// <remarks> This class extends the <see cref="textBox_ComboBox_Element"/> class. </remarks>
    public class Creator_Complex_Element : textBox_ComboBox_Element
    {
        /// <summary> Constructor for a new instance of the Creator_Complex_Element class </summary>
        public Creator_Complex_Element()
            : base("Creator", "complex_creator")
        {
            Repeatable = true;
            Type = Element_Type.Creator;
            Display_SubType = "complex";

            Add_Select_Item("", "");
            Add_Select_Item("Actor", "act");
            Add_Select_Item("Animator", "anm");
            Add_Select_Item("Architect", "arc");
            Add_Select_Item("Artist", "art");
            Add_Select_Item("Author, Primary", "aut_prim");
            Add_Select_Item("Author, Secondary", "aut_sec");
            Add_Select_Item("Binder", "bnd");
            Add_Select_Item("Calligrapher", "cll");
            Add_Select_Item("Cartographer", "ctg");
            Add_Select_Item("Choreographer", "chr");
            Add_Select_Item("Cinematographer", "cng");
            Add_Select_Item("Compiler", "com");
            Add_Select_Item("Composer", "cmp");
            Add_Select_Item("Conductor", "cnd");
            Add_Select_Item("Conference", "conference");
            Add_Select_Item("Consultant", "csl");
            Add_Select_Item("Contributor", "ctb");
            Add_Select_Item("Curator", "cur");
            Add_Select_Item("Degree grantor", "dgg");
            Add_Select_Item("Director", "drt");
            Add_Select_Item("Dissertant", "dis");
            Add_Select_Item("Designer", "dsr");
            Add_Select_Item("Editor", "edt");
            Add_Select_Item("Engineer", "eng");
            Add_Select_Item("Engraver", "egr");
            Add_Select_Item("Illustrator", "ill");
            Add_Select_Item("Interviewee", "ive");
            Add_Select_Item("Interviewer", "ivr");
            Add_Select_Item("Landscape architect", "lsa");
            Add_Select_Item("Lithographer", "ltg");
            Add_Select_Item("Manufacturer", "mfr");
            Add_Select_Item("Musician", "mus");
            Add_Select_Item("Narrator", "nrt");
            Add_Select_Item("Papermaker", "ppm");
            Add_Select_Item("Performer", "prf");
            Add_Select_Item("Photographer", "pht");
            Add_Select_Item("Programmer", "prg");
            Add_Select_Item("Printer", "prt");
            Add_Select_Item("Printmaker", "prm");
            Add_Select_Item("Producer", "pro");
            Add_Select_Item("Publisher", "pbl");
            Add_Select_Item("Puppeteer", "ppt");
            Add_Select_Item("Recipient", "rcp");
            Add_Select_Item("Researcher", "res");
            Add_Select_Item("Research team head", "rth");
            Add_Select_Item("Research team member", "rtm");
            Add_Select_Item("Reviewer", "rev");
            Add_Select_Item("Scientific advisor", "sad");
            Add_Select_Item("Sculptor", "scl");
            Add_Select_Item("Signer", "sgn");
            Add_Select_Item("Singer", "sng");
            Add_Select_Item("Speaker", "spk");
            Add_Select_Item("Sponsor", "spn");
            Add_Select_Item("Surveyor", "srv");
            Add_Select_Item("Stereotyper", "str");
            Add_Select_Item("Thesis advisor", "ths");
            Add_Select_Item("Transcriber", "trc");
            Add_Select_Item("Translator", "trl");
            Add_Select_Item("Woodcutter", "wdc");
            Add_Select_Item("Wood-engraver", "wde");
        }


        /// <summary> Renders the HTML for this element </summary>
        /// <param name="Output"> Textwriter to write the HTML for this element </param>
        /// <param name="Bib"> Object to populate this element from </param>
        /// <param name="Skin_Code"> Code for the current skin </param>
        /// <param name="isMozilla"> Flag indicates if the current browse is Mozilla Firefox (different css choices for some elements)</param>
        /// <param name="popup_form_builder"> Builder for any related popup forms for this element </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter each person or group which created this material. Personal names should be entered as [Family Name], [Given Name].";
                switch (CurrentLanguage)
                {
                    case Language_Enum.English:
                        Acronym = defaultAcronym;
                        break;

                    case Language_Enum.Spanish:
                        Acronym = defaultAcronym;
                        break;

                    case Language_Enum.French:
                        Acronym = defaultAcronym;
                        break;

                    default:
                        Acronym = defaultAcronym;
                        break;
                }
            }

            List<string> creator = new List<string>();
            List<string> roles = new List<string>();

            if (Bib.Bib_Info.hasMainEntityName )
            {
                creator.Add(Bib.Bib_Info.Main_Entity_Name.ToString(false));
                if (Bib.Bib_Info.Main_Entity_Name.Name_Type == Name_Info_Type_Enum.conference)
                {
                    roles.Add("Conference");
                }
                else
                {
                    if (Bib.Bib_Info.Main_Entity_Name.Roles.Count > 0)
                    {
                        bool found = false;
                        foreach (Name_Info_Role thisRole in Bib.Bib_Info.Main_Entity_Name.Roles)
                        {
                            if (thisRole.Role_Type == Name_Info_Role_Type_Enum.text)
                            {
                                roles.Add(thisRole.Role);
                                found = true;
                                break;
                            }
                        }
                        if (!found)
                            roles.Add(Bib.Bib_Info.Main_Entity_Name.Roles[0].Role);
                    }
                    else
                        roles.Add(String.Empty);
                }
            }

            foreach (Name_Info thisName in Bib.Bib_Info.Names)
            {
                if (thisName.hasData)
                {
                    creator.Add(thisName.ToString(false));
                    if (thisName.Name_Type == Name_Info_Type_Enum.conference)
                    {
                        roles.Add("Conference");
                    }
                    else
                    {
                        if (thisName.Roles.Count > 0)
                        {
                            bool found = false;
                            foreach (Name_Info_Role thisRole in thisName.Roles)
                            {
                                if (thisRole.Role_Type == Name_Info_Role_Type_Enum.text)
                                {
                                    roles.Add(thisRole.Role);
                                    found = true;
                                    break;
                                }
                            }
                            if (!found)
                                roles.Add(thisName.Roles[0].Role);
                        }
                        else
                        {
                            roles.Add(String.Empty);
                        }
                    }
                }
            }

            render_helper(Output, creator, roles, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears the main entity name and any other names associated with the digital resource </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            if ( Bib.Bib_Info.hasMainEntityName )
                Bib.Bib_Info.Main_Entity_Name.Clear();
            Bib.Bib_Info.Clear_Names();
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            string creator_text = String.Empty;

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_text") == 0)
                {
                    creator_text = HttpContext.Current.Request.Form[thisKey];
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_select") == 0)
                {
                    string creator_type = HttpContext.Current.Request.Form[thisKey].Trim();

                    if (creator_text.Trim().Length > 0)
                    {
                        int index = possible_select_items_value.IndexOf(creator_type);
                        if (index > 0)
                        {
                            string text_role = possible_select_items_text[index];
                            if (creator_type != text_role)
                            {
                                Bib.Bib_Info.Add_Named_Entity(creator_text, text_role, creator_type);
                            }
                            else
                            {
                                Bib.Bib_Info.Add_Named_Entity(creator_text, text_role);
                            }
                        }
                        else
                        {
                            Bib.Bib_Info.Add_Named_Entity(creator_text, String.Empty);
                        }                        
                    }

                    creator_text = String.Empty;
                }
            }
        }
    }
}


