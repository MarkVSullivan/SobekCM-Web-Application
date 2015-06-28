#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Controlled list of name types associated with a <see cref="Name_Info"/> object. </summary>
    public enum Name_Info_Type_Enum
    {
        /// <summary> Unknown type of name </summary>
        UNKNOWN = -1,

        /// <summary> Name of a personal person </summary>
        Personal = 1,

        /// <summary> Name of a corporate entity </summary>
        Corporate,

        /// <summary> Name of a conference </summary>
        Conference
    }

    /// <summary> Controlled list of name role term types associated with a <see cref="Name_Info_Role"/> object. </summary>
    public enum Name_Info_Role_Type_Enum
    {
        /// <summary> Unspecified name role type </summary>
        UNSPECIFIED = -1,

        /// <summary> Name role is a code </summary>
        Code = 1,

        /// <summary> Name role is textual </summary>
        Text
    }

    #region Name_Info_Role class

    /// <summary> Class used to store all the information about the roles associated with a <see cref="Name_Info"/> object. </summary>
    [Serializable]
    public class Name_Info_Role
    {
        private string authority;
        private string role;
        private Name_Info_Role_Type_Enum roleType;

        /// <summary> Constructor for a new instance of the Name_Info_Role class </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Authority">Authority for this role term</param>
        /// <param name="Role_Type">Type of role</param>
        public Name_Info_Role(string Role, string Authority, Name_Info_Role_Type_Enum Role_Type)
        {
            roleType = Role_Type;
            authority = Authority;
            role = Role;
        }

        /// <summary> Constructor for a new instance of the Name_Info_Role class </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Authority">Authority for this role term</param>
        public Name_Info_Role(string Role, string Authority)
        {
            role = Role;
            authority = Authority;
            roleType = Name_Info_Role_Type_Enum.UNSPECIFIED;
        }

        /// <summary> Constructor for a new instance of the Name_Info_Role class </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Role_Type">Type of role</param>
        public Name_Info_Role(string Role, Name_Info_Role_Type_Enum Role_Type)
        {
            role = Role;
            roleType = Role_Type;
        }

        /// <summary> Gets or sets the type of role term indicated ( text or code )</summary>
        public Name_Info_Role_Type_Enum Role_Type
        {
            get { return roleType; }
            set { roleType = value; }
        }

        /// <summary> Gets or sets the name of the authority for this role text/code </summary>
        /// <remarks>Currently, only accepted authority is 'marcrelator' </remarks>
        public string Authority
        {
            get { return authority ?? String.Empty; }
            set { authority = value; }
        }

        /// <summary> Gets or sets the actual term for this role </summary>
        public string Role
        {
            get { return role; }
            set { role = value; }
        }
    }

    #endregion

    #region Name_Info class

    /// <summary>The name of a person, organization, or event (conference, meeting, etc.) associated in some way with the resource. </summary>
    /// <remarks>This class extends the <see cref="XML_Node_Base_Type"/> class for writing to XML </remarks>
    [Serializable]
    public class Name_Info : XML_Node_Base_Type
    {
        private string affiliation;
        private string dates;
        private string description;
        private string display_form;
        private string family_name;
        private string full_name;
        private string given_name;
        private bool main_entity;
        private Name_Info_Type_Enum name_type;

        private string terms_of_address;

        /// <summary> Constructor creates a new instance of the Name_Info class</summary>
        public Name_Info()
        {
            name_type = Name_Info_Type_Enum.UNKNOWN;
            main_entity = false;

            Roles = new List<Name_Info_Role>();
        }

        /// <summary>Constructor creates a new instance of the Name_Info class</summary>
        /// <param name="Full_Name">Full name of this entity</param>
        /// <param name="Role">Role term for this entity's relationship with the resource</param>
        public Name_Info(string Full_Name, string Role)
        {
            name_type = Name_Info_Type_Enum.UNKNOWN;
            main_entity = false;

            full_name = Full_Name;
            Roles = new List<Name_Info_Role>();
            if (Role.Length > 0)
            {
                Roles.Add(new Name_Info_Role(Role, Name_Info_Role_Type_Enum.Text));
            }
        }

        #region Public properties

        /// <summary> Gets or sets the type of name </summary>
        /// <remarks>Possible types are personal, corporate, conference, or unspecified</remarks>
        public Name_Info_Type_Enum Name_Type
        {
            get { return name_type; }
            set { name_type = value; }
        }

        /// <summary> Gets or sets the full name of this entity </summary>
        public string Full_Name
        {
            get { return full_name ?? String.Empty; }
            set { full_name = value; }
        }

        /// <summary> Gets or sets the given (first) names associated with this entity </summary>
        public string Given_Name
        {
            get { return given_name ?? String.Empty; }
            set { given_name = value; }
        }

        /// <summary> Gets or sets the family (last) name associated with this entity </summary>
        public string Family_Name
        {
            get { return family_name ?? String.Empty; }
            set { family_name = value; }
        }

        /// <summary> Gets or sets the dates associated with this name entity </summary>
        public string Dates
        {
            get { return dates ?? String.Empty; }
            set { dates = value; }
        }

        /// <summary> Gets or sets the terms of address associated with this entity </summary>
        public string Terms_Of_Address
        {
            get { return terms_of_address ?? String.Empty; }
            set { terms_of_address = value; }
        }

        /// <summary> Gets or sets additional information necessary for displaying this name entity </summary>
        public string Display_Form
        {
            get { return display_form ?? String.Empty; }
            set { display_form = value; }
        }

        /// <summary> Gets or sets the simple affiliation associated with this entity </summary>
        /// <remarks>For more details, use the Affiliation_Info object</remarks>
        public string Affiliation
        {
            get { return affiliation ?? String.Empty; }
            set { affiliation = value; }
        }

        /// <summary> Gets or sets any related description for this name entity </summary>
        public string Description
        {
            get { return description ?? String.Empty; }
            set { description = value; }
        }

        /// <summary> Gets the collection of roles for this entity </summary>
        public List<Name_Info_Role> Roles { get; private set; }

        /// <summary> Gets value specifying if there is data for this name object </summary>
        public bool hasData
        {
            get
            {
                return (!String.IsNullOrEmpty(full_name)) || (!String.IsNullOrEmpty(given_name)) || (!String.IsNullOrEmpty(family_name));
            }
        }

        #endregion

        #region Public Methods

        /// <summary> Add a new role to this name entity </summary>
        /// <param name="Role">Text of the role</param>
        public void Add_Role(string Role)
        {
            Add_Role(Role, String.Empty);
        }

        /// <summary> Add a new role to this name entity </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Authority">Authority for this role term</param>
        public void Add_Role(string Role, string Authority)
        {
            if ((Authority.Length == 0) || (Authority.ToLower() == "marcrelator"))
            {
                if (Role.Trim().Length <= 3)
                {
                    string text_from_code = Name_Info_MarcRelator_Code.Text_From_Code(Role);
                    if (text_from_code.Length > 0)
                    {
                        Roles.Add(new Name_Info_Role(text_from_code, String.Empty, Name_Info_Role_Type_Enum.Text));
                        Roles.Add(new Name_Info_Role(Role, "marcrelator", Name_Info_Role_Type_Enum.Code));
                    }
                    else
                    {
                        Roles.Add(new Name_Info_Role(Role, String.Empty, Name_Info_Role_Type_Enum.Code));
                    }
                }
                else
                {
                    string code_from_text = Name_Info_MarcRelator_Code.Code_From_Text(Role);
                    if (code_from_text.Length > 0)
                    {
                        Roles.Add(new Name_Info_Role(code_from_text, "marcrelator", Name_Info_Role_Type_Enum.Code));
                        Roles.Add(new Name_Info_Role(Role, String.Empty, Name_Info_Role_Type_Enum.Text));
                    }
                    else
                    {
                        Roles.Add(new Name_Info_Role(Role, String.Empty, Name_Info_Role_Type_Enum.Text));
                    }
                }
            }
            else
            {
                Roles.Add(new Name_Info_Role(Role, Authority));
            }
        }


        /// <summary> Add a new role to this name entity </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Authority">Authority for this role term</param>
        /// <param name="Role_Type">Type of role</param>
        public void Add_Role(string Role, string Authority, Name_Info_Role_Type_Enum Role_Type)
        {
            Roles.Add(new Name_Info_Role(Role, Authority, Role_Type));
        }

        #endregion

        /// <summary> Get the string with all the roles for this user </summary>
        public string Role_String
        {
            get
            {
                if ((Roles != null) && (Roles.Count > 0))
                {
                    List<string> temp_role_text = new List<string>();
                    foreach (Name_Info_Role thisRole in Roles)
                    {
                        if (((thisRole.Role_Type == Name_Info_Role_Type_Enum.Text) || (thisRole.Role_Type == Name_Info_Role_Type_Enum.UNSPECIFIED)) && ((thisRole.Role.ToUpper() != "MAIN ENTITY") && (thisRole.Role.ToUpper() != "CREATOR")))
                            temp_role_text.Add(thisRole.Role);
                    }
                    if (temp_role_text.Count == 0)
                        return String.Empty;

                    if (temp_role_text.Count == 1)
                        return " ( <i>" + Convert_String_To_XML_Safe(temp_role_text[0]) + "</i> )";

                    StringBuilder nameBuilder = new StringBuilder();
                    bool role_started = false;
                    foreach (string thisRole in temp_role_text)
                    {
                        if (!role_started)
                        {
                            nameBuilder.Append(" ( <i>" + Convert_String_To_XML_Safe(thisRole));
                            role_started = true;
                        }
                        else
                        {
                            nameBuilder.Append(", " + Convert_String_To_XML_Safe(thisRole));
                        }
                    }
                    if (role_started)
                        nameBuilder.Append("</i> )");
                    return nameBuilder.ToString();
                }
                return String.Empty;
            }
        }

        /// <summary> Flag indicates if this is the main entity / primary author associated with a digital resource </summary>
        public bool Main_Entity
        {
            get { return main_entity; }
            set { main_entity = value; }
        }

        /// <summary> Clears all the data associated with this item </summary>
        public void Clear()
        {
            name_type = Name_Info_Type_Enum.UNKNOWN;
            full_name = null;
            given_name = null;
            family_name = null;
            dates = null;
            terms_of_address = null;
            display_form = null;
            affiliation = null;
            description = null;
            main_entity = false;

            Roles.Clear();
        }

        /// <summary> Returns this name as a general string for display purposes </summary>
        /// <returns> This object in string format </returns>
        /// <remarks> By default, this includes the role(s) in the string </remarks>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary> Returns this identifier as a general string for display purposes </summary>
        /// <param name="IncludeRole"> Flag indicates whether to include the role(s) in this string </param>
        /// <returns> This object in string format </returns>
        public string ToString(bool IncludeRole)
        {
            StringBuilder nameBuilder = new StringBuilder();
            if (!String.IsNullOrEmpty(full_name))
            {
                nameBuilder.Append(Convert_String_To_XML_Safe(full_name.Replace("|", " -- ")));
            }
            else
            {
                if (!String.IsNullOrEmpty(family_name))
                {
                    if (!String.IsNullOrEmpty(given_name))
                    {
                        nameBuilder.Append(Convert_String_To_XML_Safe(family_name) + ", " + Convert_String_To_XML_Safe(given_name));
                    }
                    else
                    {
                        nameBuilder.Append(Convert_String_To_XML_Safe(family_name));
                    }
                }
                else
                {
                    nameBuilder.Append(!String.IsNullOrEmpty(given_name) ? Convert_String_To_XML_Safe(given_name) : "unknown");
                }
            }
            if (!String.IsNullOrEmpty(display_form))
                nameBuilder.Append(" ( " + Convert_String_To_XML_Safe(display_form) + " )");
            if (!String.IsNullOrEmpty(dates))
                nameBuilder.Append(", " + Convert_String_To_XML_Safe(dates));
            if ((IncludeRole) && (Roles != null))
            {
                bool role_started = false;
                foreach (Name_Info_Role thisRole in Roles)
                {
                    if (((thisRole.Role_Type == Name_Info_Role_Type_Enum.Text) || (thisRole.Role_Type == Name_Info_Role_Type_Enum.UNSPECIFIED)) && ((thisRole.Role.ToUpper() != "MAIN ENTITY") && (thisRole.Role.ToUpper() != "CREATOR")))
                    {
                        if (!role_started)
                        {
                            nameBuilder.Append(" ( <i>" + Convert_String_To_XML_Safe(thisRole.Role));
                            role_started = true;
                        }
                        else
                        {
                            nameBuilder.Append(", " + Convert_String_To_XML_Safe(thisRole.Role));
                        }
                    }
                }
                if (role_started)
                    nameBuilder.Append("</i> )");
            }
            return nameBuilder.ToString();
        }

        /// <summary> Writes this location as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="MainEntityFlag"> Flag indicates if this is the main entity / primary author associated with a digital resource </param>
        /// <param name="ReturnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS(bool MainEntityFlag, TextWriter ReturnValue)
        {
            ReturnValue.Write("<mods:name");
            switch (name_type)
            {
                case Name_Info_Type_Enum.Personal:
                    ReturnValue.Write(" type=\"personal\"");
                    break;

                case Name_Info_Type_Enum.Conference:
                    ReturnValue.Write(" type=\"conference\"");
                    break;

                case Name_Info_Type_Enum.Corporate:
                    ReturnValue.Write(" type=\"corporate\"");
                    break;
            }
            Add_ID(ReturnValue);
            ReturnValue.Write(">\r\n");

            if (!String.IsNullOrEmpty(full_name))
            {
                ReturnValue.Write("<mods:namePart>" + Convert_String_To_XML_Safe(full_name) + "</mods:namePart>\r\n");
            }

            if (!String.IsNullOrEmpty(given_name))
            {
                ReturnValue.Write("<mods:namePart type=\"given\">" + Convert_String_To_XML_Safe(given_name) + "</mods:namePart>\r\n");
            }

            if (!String.IsNullOrEmpty(family_name))
            {
                ReturnValue.Write("<mods:namePart type=\"family\">" + Convert_String_To_XML_Safe(family_name) + "</mods:namePart>\r\n");
            }

            if (!String.IsNullOrEmpty(dates))
            {
                ReturnValue.Write("<mods:namePart type=\"date\">" + Convert_String_To_XML_Safe(dates) + "</mods:namePart>\r\n");
            }

            if (!String.IsNullOrEmpty(terms_of_address))
            {
                ReturnValue.Write("<mods:namePart type=\"termsOfAddress\">" + Convert_String_To_XML_Safe(terms_of_address) + "</mods:namePart>\r\n");
            }

            if (!String.IsNullOrEmpty(display_form))
            {
                ReturnValue.Write("<mods:displayForm>" + Convert_String_To_XML_Safe(display_form) + "</mods:displayForm>\r\n");
            }

            if (!String.IsNullOrEmpty(affiliation))
            {
                ReturnValue.Write("<mods:affiliation>" + Convert_String_To_XML_Safe(affiliation) + "</mods:affiliation>\r\n");
            }

            if (!String.IsNullOrEmpty(description))
            {
                ReturnValue.Write("<mods:description>" + Convert_String_To_XML_Safe(description) + "</mods:description>\r\n");
            }

            if (((Roles != null) && (Roles.Count > 0)) || (MainEntityFlag))
            {
                ReturnValue.Write("<mods:role>\r\n");
                if (MainEntityFlag)
                {
                    ReturnValue.Write("<mods:roleTerm>Main Entity</mods:roleTerm>\r\n");
                }
                if (Roles != null)
                {
                    foreach (Name_Info_Role thisRole in Roles)
                    {
                        ReturnValue.Write("<mods:roleTerm");
                        switch (thisRole.Role_Type)
                        {
                            case Name_Info_Role_Type_Enum.Code:
                                ReturnValue.Write(" type=\"code\"");
                                break;

                            case Name_Info_Role_Type_Enum.Text:
                                ReturnValue.Write(" type=\"text\"");
                                break;
                        }
                        if (thisRole.Authority.Length > 0)
                        {
                            ReturnValue.Write(" authority=\"" + thisRole.Authority + "\"");
                        }

                        ReturnValue.Write(">" + Convert_String_To_XML_Safe(thisRole.Role) + "</mods:roleTerm>\r\n");
                    }
                }
                ReturnValue.Write("</mods:role>\r\n");
            }

            ReturnValue.Write("</mods:name>\r\n");
        }

        /// <summary> Writes this abstract as a MARC tag for aggregation into a MARC record </summary>
        /// <param name="ExcludeRelatorCodes"> Flag indicates to exclude the relator codes </param>
        /// <returns> Built MARC tag </returns>
        internal MARC_Field to_MARC_HTML(bool ExcludeRelatorCodes)
        {
            MARC_Field returnValue = new MARC_Field();
            StringBuilder fieldBuilder = new StringBuilder();

            // Get list of role codes first
            List<string> role_codes = new List<string>();
            List<string> role_texts = new List<string>();

            // Collect the codes first
            if (Roles != null)
            {
                foreach (Name_Info_Role thisRole in Roles)
                {
                    if ((thisRole.Role_Type == Name_Info_Role_Type_Enum.Code) && (thisRole.Authority == "marcrelator"))
                    {
                        role_codes.Add(thisRole.Role.ToLower());
                    }
                }

                // Collect the text next
                foreach (Name_Info_Role thisRole in Roles)
                {
                    if (((thisRole.Role_Type == Name_Info_Role_Type_Enum.Text) || (thisRole.Role_Type == Name_Info_Role_Type_Enum.UNSPECIFIED)) && (thisRole.Role.ToUpper() != "MAIN ENTITY") && (thisRole.Role.ToUpper() != "CREATOR"))
                    {
                        string code_from_text = Name_Info_MarcRelator_Code.Code_From_Text(thisRole.Role);
                        if ((code_from_text.Length > 0) && (!ExcludeRelatorCodes))
                        {
                            if (!role_codes.Contains(code_from_text))
                            {
                                role_codes.Add(code_from_text);
                            }
                        }
                        else
                        {
                            role_texts.Add(thisRole.Role.ToLower());
                        }
                    }
                }
            }

            switch (name_type)
            {
                case Name_Info_Type_Enum.Corporate:

                    returnValue.Indicators = "2 ";

                    if (!String.IsNullOrEmpty(full_name))
                    {
                        if (full_name.IndexOf(" -- ") > 0)
                        {
                            int corp_dash_index = full_name.IndexOf(" -- ");
                            fieldBuilder.Append("|a " + full_name.Substring(0, corp_dash_index) + " |b " + full_name.Substring(corp_dash_index + 4).Replace("|", "|b") + " ");
                        }
                        else
                        {
                            fieldBuilder.Append("|a " + full_name + " ");
                        }
                    }

                    if (!String.IsNullOrEmpty(description))
                        fieldBuilder.Append("|c " + description.Replace("|", "|c") + " ");

                    if (!String.IsNullOrEmpty(dates))
                        fieldBuilder.Append(", |d " + dates.Replace("|", "|d") + " ");

                    foreach (string thisRole in role_texts)
                    {
                        fieldBuilder.Append(", |e " + thisRole + " ");
                    }

                    if (!String.IsNullOrEmpty(affiliation))
                        fieldBuilder.Append("|u " + affiliation.Replace("|", "|u") + " ");

                    fieldBuilder.Append(". ");

                    if (!ExcludeRelatorCodes)
                    {
                        foreach (string thisRoleCode in role_codes)
                        {
                            fieldBuilder.Append("|4 " + thisRoleCode + " ");
                        }
                    }

                    returnValue.Control_Field_Value = fieldBuilder.ToString().Replace("  ", " ").Replace(" ,", ",").Replace(",,", ",").Replace("..", ".").Replace(" .", ".").Trim();
                    return returnValue;

                case Name_Info_Type_Enum.Conference:

                    returnValue.Indicators = "2 ";

                    if (!String.IsNullOrEmpty(full_name))
                    {
                        fieldBuilder.Append("|a " + full_name + " ");
                    }

                    if (!String.IsNullOrEmpty(description))
                        fieldBuilder.Append("|c " + description.Replace("|", "|c") + " ");

                    if (!String.IsNullOrEmpty(dates))
                        fieldBuilder.Append(", |d " + dates.Replace("|", "|d") + " ");

                    foreach (string thisRole in role_texts)
                    {
                        fieldBuilder.Append(", |j " + thisRole + " ");
                    }

                    if (!String.IsNullOrEmpty(affiliation))
                        fieldBuilder.Append("|u " + affiliation.Replace("|", "|u") + " ");

                    fieldBuilder.Append(". ");

                    if (!ExcludeRelatorCodes)
                    {
                        foreach (string thisRoleCode in role_codes)
                        {
                            fieldBuilder.Append("|4 " + thisRoleCode + " ");
                        }
                    }

                    returnValue.Control_Field_Value = fieldBuilder.ToString().Replace("  ", " ").Replace(" ,", ",").Replace(",,", ",").Replace("..", ".").Replace(" .", ".").Trim();
                    return returnValue;

                default:

                    // First, set the indicator and early name stuff
                    if ((Given_Name.Length == 0) && (Family_Name.Length == 0))
                    {
                        returnValue.Indicators = "  ";
                        string fullNameTesting = Full_Name.Trim();

                        fieldBuilder.Append("|a " + fullNameTesting + " ");

                        // If the full name has a comma just before the first space, 
                        // (and it is a personal name being here) than this must be
                        // in inverted order, so set the indicator accordingly.
                        int comma_index = fullNameTesting.IndexOf(",");
                        int space_index = fullNameTesting.IndexOf(" ");
                        if ((comma_index > 0) && (space_index > 0) && (space_index == comma_index + 1))
                        {
                            returnValue.Indicator1 = '1';
                        }
                    }

                    if ((Given_Name.Length > 0) && (Family_Name.Length > 0))
                    {
                        returnValue.Indicators = "1 ";
                        fieldBuilder.Append("|a " + Family_Name + ", " + Given_Name + " ");
                    }

                    if ((Given_Name.Length == 0) && (Family_Name.Length > 0))
                    {
                        returnValue.Indicators = "3 ";
                        fieldBuilder.Append("|a " + Family_Name + " ");
                    }

                    if ((Given_Name.Length > 0) && (Family_Name.Length == 0))
                    {
                        returnValue.Indicators = "0 ";
                        fieldBuilder.Append("|a " + Given_Name + " ");
                    }

                    if (!String.IsNullOrEmpty(terms_of_address))
                    {
                        if (terms_of_address.IndexOf(";") > 0)
                        {
                            string[] toa_splitter = terms_of_address.Split(";".ToCharArray());
                            fieldBuilder.Append("|b " + toa_splitter[0].Trim().Replace("|", "|b") + " ");
                            fieldBuilder.Append("|c " + toa_splitter[1].Trim().Replace("|", "|c") + " ");
                        }
                        else
                        {
                            fieldBuilder.Append("|c " + terms_of_address.Replace("|", "|c") + " ");
                        }
                    }

                    if (!String.IsNullOrEmpty(display_form))
                    {
                        fieldBuilder.Append("|q (" + display_form + ") ");
                    }

                    if (!String.IsNullOrEmpty(dates))
                    {
                        fieldBuilder.Append(", |d " + dates.Replace("|", "|d") + " ");
                    }

                    foreach (string thisRole in role_texts)
                    {
                        fieldBuilder.Append(", |e " + thisRole + " ");
                    }

                    if (!String.IsNullOrEmpty(description))
                    {
                        if (description.IndexOf(";") > 0)
                        {
                            string[] desc_splitter = description.Split(";".ToCharArray());
                            fieldBuilder.Append("|g " + desc_splitter[0].Trim().Replace("|", "|g") + " ");
                            fieldBuilder.Append("|j " + desc_splitter[1].Trim().Replace("|", "|j") + " ");
                        }
                        else
                        {
                            fieldBuilder.Append("|g " + description.Replace("|", "|g") + " ");
                        }
                    }

                    if (!String.IsNullOrEmpty(affiliation))
                    {
                        fieldBuilder.Append("|u " + affiliation.Replace("|", "|u") + " ");
                    }

                    fieldBuilder.Append(". ");


                    if (!ExcludeRelatorCodes)
                    {
                        foreach (string thisRoleCode in role_codes)
                        {
                            fieldBuilder.Append("|4 " + thisRoleCode + " ");
                        }
                    }

                    returnValue.Control_Field_Value = fieldBuilder.ToString().Replace("  ", " ").Replace(" ,", ",").Replace(",,", ",").Replace("..", ".").Replace(" .", ".").Trim();
                    return returnValue;
            }
        }
    }

    #endregion

    #region static Name_Info_MarcRelator_Code class

    /// <summary> MARC Relator code associated with a <see cref="Name_Info" /> object </summary>
    public class Name_Info_MarcRelator_Code
    {
        /// <summary> Convert from the role text to the corresponding MARC Relator Code </summary>
        /// <param name="Text"> Role text </param>
        /// <returns> MARC relator code, or the empty string if this does not match LC's controlled list</returns>
        public static string Code_From_Text(string Text)
        {
            string text_upper = Text.ToUpper();
            string code = String.Empty;
            switch (text_upper)
            {
                case "ADAPTER":
                    code = "adp";
                    break;
                case "AUTHOR OF AFTERWORD, COLOPHON, ETC.":
                    code = "aft";
                    break;
                case "ANIMATOR":
                    code = "anm";
                    break;
                case "ANNOTATOR":
                    code = "ann";
                    break;
                case "BIBLIOGRAPHIC ANTECEDENT":
                    code = "ant";
                    break;
                case "APPLICANT":
                    code = "app";
                    break;
                case "AUTHOR IN QUOTATIONS OR TEXT ABSTRACTS":
                    code = "aqt";
                    break;
                case "ARCHITECT":
                    code = "arc";
                    break;
                case "ARRANGER":
                    code = "arr";
                    break;
                case "ARTIST":
                    code = "art";
                    break;
                case "ASSIGNEE":
                    code = "asg";
                    break;
                case "ASSOCIATED NAME":
                    code = "asn";
                    break;
                case "ATTRIBUTED NAME":
                    code = "att";
                    break;
                case "AUCTIONEER":
                    code = "auc";
                    break;
                case "AUTHOR OF DIALOG":
                    code = "aud";
                    break;
                case "AUTHOR OF INTRODUCTION":
                    code = "aui";
                    break;
                case "AUTHOR OF SCREENPLAY":
                    code = "aus";
                    break;
                case "BINDING DESIGNER":
                    code = "bdd";
                    break;
                case "BOOKJACKET DESIGNER":
                    code = "bjd";
                    break;
                case "BOOK DESIGNER":
                    code = "bkd";
                    break;
                case "BOOK PRODUCER":
                    code = "bkp";
                    break;
                case "BINDER":
                    code = "bnd";
                    break;
                case "BOOKPLATE DESIGNER":
                    code = "bpd";
                    break;
                case "BOOKSELLER":
                    code = "bsl";
                    break;
                case "CONCEPTOR":
                    code = "ccp";
                    break;
                case "CHOREOGRAPHER":
                    code = "chr";
                    break;
                case "COLLABORATOR":
                    code = "clb";
                    break;
                case "CLIENT":
                    code = "cli";
                    break;
                case "CALLIGRAPHER":
                    code = "cll";
                    break;
                case "COLLOTYPER":
                    code = "clt";
                    break;
                case "COMMENTATOR":
                    code = "cmm";
                    break;
                case "COMPOSER":
                    code = "cmp";
                    break;
                case "COMPOSITOR":
                    code = "cmt";
                    break;
                case "CINEMATOGRAPHER":
                    code = "cng";
                    break;
                case "CONDUCTOR":
                    code = "cnd";
                    break;
                case "CENSOR":
                    code = "cns";
                    break;
                case "CONTESTANT -APPELLEE":
                    code = "coe";
                    break;
                case "COLLECTOR":
                    code = "col";
                    break;
                case "COMPILER":
                    code = "com";
                    break;
                case "CONTESTANT":
                    code = "cos";
                    break;
                case "CONTESTANT -APPELLANT":
                    code = "cot";
                    break;
                case "COVER DESIGNER":
                    code = "cov";
                    break;
                case "COPYRIGHT CLAIMANT":
                    code = "cpc";
                    break;
                case "COMPLAINANT-APPELLEE":
                    code = "cpe";
                    break;
                case "COPYRIGHT HOLDER":
                    code = "cph";
                    break;
                case "COMPLAINANT":
                    code = "cpl";
                    break;
                case "COMPLAINANT-APPELLANT":
                    code = "cpt";
                    break;
                case "CREATOR":
                    code = "cre";
                    break;
                case "CORRESPONDENT":
                    code = "crp";
                    break;
                case "CORRECTOR":
                    code = "crr";
                    break;
                case "CONSULTANT":
                    code = "csl";
                    break;
                case "CONSULTANT TO A PROJECT":
                    code = "csp";
                    break;
                case "COSTUME DESIGNER":
                    code = "cst";
                    break;
                case "CONTRIBUTOR":
                    code = "ctb";
                    break;
                case "CONTESTEE-APPELLEE":
                    code = "cte";
                    break;
                case "CARTOGRAPHER":
                    code = "ctg";
                    break;
                case "CONTRACTOR":
                    code = "ctr";
                    break;
                case "CONTESTEE":
                    code = "cts";
                    break;
                case "CONTESTEE-APPELLANT":
                    code = "ctt";
                    break;
                case "CURATOR":
                    code = "cur";
                    break;
                case "COMMENTATOR FOR WRITTEN TEXT":
                    code = "cwt";
                    break;
                case "DEFENDANT":
                    code = "dfd";
                    break;
                case "DEFENDANT-APPELLEE":
                    code = "dfe";
                    break;
                case "DEFENDANT-APPELLANT":
                    code = "dft";
                    break;
                case "DEGREE GRANTOR":
                    code = "dgg";
                    break;
                case "DISSERTANT":
                    code = "dis";
                    break;
                case "DELINEATOR":
                    code = "dln";
                    break;
                case "DANCER":
                    code = "dnc";
                    break;
                case "DONOR":
                    code = "dnr";
                    break;
                case "DEPICTED":
                    code = "dpc";
                    break;
                case "DEPOSITOR":
                    code = "dpt";
                    break;
                case "DRAFTSMAN":
                    code = "drm";
                    break;
                case "DIRECTOR":
                    code = "drt";
                    break;
                case "DESIGNER":
                    code = "dsr";
                    break;
                case "DISTRIBUTOR":
                    code = "dst";
                    break;
                case "DEDICATEE":
                    code = "dte";
                    break;
                case "DEDICATOR":
                    code = "dto";
                    break;
                case "DUBIOUS AUTHOR":
                    code = "dub";
                    break;
                case "EDITOR":
                    code = "edt";
                    break;
                case "ENGRAVER":
                    code = "egr";
                    break;
                case "ELECTROTYPER":
                    code = "elt";
                    break;
                case "ENGINEER":
                    code = "eng";
                    break;
                case "ETCHER":
                    code = "etr";
                    break;
                case "EXPERT":
                    code = "exp";
                    break;
                case "FACSIMILIST":
                    code = "fac";
                    break;
                case "FILM EDITOR":
                    code = "flm";
                    break;
                case "FORMER OWNER":
                    code = "fmo";
                    break;
                case "FIRST PART":
                    code = "fpy";
                    break;
                case "FUNDER":
                    code = "fnd";
                    break;
                case "FORGER":
                    code = "frg";
                    break;
                case "GRAPHIC TECHNICIAN":
                    code = "grt";
                    break;
                case "HONOREE":
                    code = "hnr";
                    break;
                case "HOST":
                    code = "hst";
                    break;
                case "ILLUSTRATOR":
                    code = "ill";
                    break;
                case "ILLUMINATOR":
                    code = "ilu";
                    break;
                case "INSCRIBER":
                    code = "ins";
                    break;
                case "INVENTOR":
                    code = "inv";
                    break;
                case "INSTRUMENTALIST":
                    code = "itr";
                    break;
                case "INTERVIEWEE":
                    code = "ive";
                    break;
                case "INTERVIEWER":
                    code = "ivr";
                    break;
                case "LIBRETTIST":
                    code = "lbt";
                    break;
                case "LIBELEE-APPELLEE":
                    code = "lee";
                    break;
                case "LIBELEE":
                    code = "lel";
                    break;
                case "LENDER":
                    code = "len";
                    break;
                case "LIBELEE-APPELLANT":
                    code = "let";
                    break;
                case "LIGHTING DESIGNER":
                    code = "lgd";
                    break;
                case "LIBELANT-APPELLEE":
                    code = "lie";
                    break;
                case "LIBELANT":
                    code = "lil";
                    break;
                case "LIBELANT-APPELLANT":
                    code = "lit";
                    break;
                case "LANDSCAPE ARCHITECT":
                    code = "lsa";
                    break;
                case "LICENSEE":
                    code = "lse";
                    break;
                case "LICENSOR":
                    code = "lso";
                    break;
                case "LITHOGRAPHER":
                    code = "ltg";
                    break;
                case "LYRICIST":
                    code = "lyr";
                    break;
                case "MANUFACTURER":
                    code = "mfr";
                    break;
                case "METADATA CONTACT":
                    code = "mdc";
                    break;
                case "MODERATOR":
                    code = "mod";
                    break;
                case "MONITOR":
                    code = "mon";
                    break;
                case "MARKUP EDITOR":
                    code = "mrk";
                    break;
                case "METAL-ENGRAVER":
                    code = "mte";
                    break;
                case "MUSICIAN":
                    code = "mus";
                    break;
                case "NARRATOR":
                    code = "nrt";
                    break;
                case "OPPONENT":
                    code = "opn";
                    break;
                case "ORIGINATOR":
                    code = "org";
                    break;
                case "ORGANIZER OF MEETING":
                    code = "orm";
                    break;
                case "OTHER":
                    code = "oth";
                    break;
                case "OWNER":
                    code = "own";
                    break;
                case "PATRON":
                    code = "pat";
                    break;
                case "PUBLISHING DIRECTOR":
                    code = "pbd";
                    break;
                case "PROOFREADER":
                    code = "pfr";
                    break;
                case "PLATEMAKER":
                    code = "plt";
                    break;
                case "PRINTER OF PLATES":
                    code = "pop";
                    break;
                case "PAPERMAKER":
                    code = "ppm";
                    break;
                case "PUPPETEER":
                    code = "ppt";
                    break;
                case "PROCESS CONTACT":
                    code = "prc";
                    break;
                case "PRODUCTION PERSONNEL":
                    code = "prd";
                    break;
                case "PERFORMER":
                    code = "prf";
                    break;
                case "PROGRAMMER":
                    code = "prg";
                    break;
                case "PRINTMAKER":
                    code = "prm";
                    break;
                case "PRODUCER":
                    code = "pro";
                    break;
                case "PRINTER":
                    code = "prt";
                    break;
                case "PATENT APPLICANT":
                    code = "pta";
                    break;
                case "PLAINTIFF -APPELLEE":
                    code = "pte";
                    break;
                case "PLAINTIFF":
                    code = "ptf";
                    break;
                case "PATENT HOLDER":
                    code = "pth";
                    break;
                case "PLAINTIFF-APPELLANT":
                    code = "ptt";
                    break;
                case "RUBRICATOR":
                    code = "rbr";
                    break;
                case "RECORDING ENGINEER":
                    code = "rce";
                    break;
                case "RECIPIENT":
                    code = "rcp";
                    break;
                case "REDACTOR":
                    code = "red";
                    break;
                case "RENDERER":
                    code = "ren";
                    break;
                case "RESEARCHER":
                    code = "res";
                    break;
                case "REVIEWER":
                    code = "rev";
                    break;
                case "REPORTER":
                    code = "rpt";
                    break;
                case "RESPONSIBLE PARTY":
                    code = "rpy";
                    break;
                case "RESPONDENT -APPELLEE":
                    code = "rse";
                    break;
                case "RESTAGER":
                    code = "rsg";
                    break;
                case "RESPONDENT":
                    code = "rsp";
                    break;
                case "RESPONDENT-APPELLANT":
                    code = "rst";
                    break;
                case "RESEARCH TEAM HEAD":
                    code = "rth";
                    break;
                case "RESEARCH TEAM MEMBER":
                    code = "rtm";
                    break;
                case "SCIENTIFIC ADVISOR":
                    code = "sad";
                    break;
                case "SCENARIST":
                    code = "sce";
                    break;
                case "SCULPTOR":
                    code = "scl";
                    break;
                case "SCRIBE":
                    code = "scr";
                    break;
                case "SECRETARY":
                    code = "sec";
                    break;
                case "SIGNER":
                    code = "sgn";
                    break;
                case "SINGER":
                    code = "sng";
                    break;
                case "SPEAKER":
                    code = "spk";
                    break;
                case "SPONSOR":
                    code = "spn";
                    break;
                case "SECOND PART":
                    code = "spy";
                    break;
                case "SURVEYOR":
                    code = "srv";
                    break;
                case "SET DESIGNER":
                    code = "std";
                    break;
                case "STORYTELLER":
                    code = "stl";
                    break;
                case "STANDARDS BODY":
                    code = "stn";
                    break;
                case "STEREOTYPER":
                    code = "str";
                    break;
                case "TEACHER":
                    code = "tch";
                    break;
                case "THESIS ADVISOR":
                    code = "ths";
                    break;
                case "TRANSCRIBER":
                    code = "trc";
                    break;
                case "TRANSLATOR":
                    code = "trl";
                    break;
                case "TYPE DESIGNER":
                    code = "tyd";
                    break;
                case "TYPOGRAPHER":
                    code = "tyg";
                    break;
                case "VIDEOGRAPHER":
                    code = "vdg";
                    break;
                case "VOCALIST":
                    code = "voc";
                    break;
                case "WRITER OF ACCOMPANYING MATERIAL":
                    code = "wam";
                    break;
                case "WOODCUTTER":
                    code = "wdc";
                    break;
                case "WOOD-ENGRAVER":
                    code = "wde";
                    break;
                case "WITNESS":
                    code = "wit";
                    break;
                case "AUTHOR":
                    code = "aut";
                    break;
                case "PHOTOGRAPHER":
                    code = "pht";
                    break;
                case "ACTOR":
                    code = "act";
                    break;
                case "PUBLISHER":
                    code = "pbl";
                    break;
            }
            return code;
        }

        /// <summary> Convert to the role text from the corresponding MARC Relator Code </summary>
        /// <param name="Code"> MARC Relator Code </param>
        /// <returns> Role text, or the empty string if this does not match LC's controlled list</returns>
        public static string Text_From_Code(string Code)
        {
            string code_upper = Code.ToUpper();
            string text = String.Empty;
            switch (code_upper)
            {
                case "ADP":
                    text = "Adapter ";
                    break;
                case "AFT":
                    text = "Author of afterword, colophon, etc. ";
                    break;
                case "ANM":
                    text = "Animator";
                    break;
                case "ANN":
                    text = "Annotator ";
                    break;
                case "ANT":
                    text = "Bibliographic antecedent ";
                    break;
                case "APP":
                    text = "Applicant ";
                    break;
                case "AQT":
                    text = "Author in quotations or text abstracts ";
                    break;
                case "ARC":
                    text = "Architect ";
                    break;
                case "ARR":
                    text = "Arranger ";
                    break;
                case "ART":
                    text = "Artist ";
                    break;
                case "ASG":
                    text = "Assignee ";
                    break;
                case "ASN":
                    text = "Associated name ";
                    break;
                case "ATT":
                    text = "Attributed name ";
                    break;
                case "AUC":
                    text = "Auctioneer ";
                    break;
                case "AUD":
                    text = "Author of dialog ";
                    break;
                case "AUI":
                    text = "Author of introduction ";
                    break;
                case "AUS":
                    text = "Author of screenplay ";
                    break;
                case "BDD":
                    text = "Binding designer ";
                    break;
                case "BJD":
                    text = "Bookjacket designer ";
                    break;
                case "BKD":
                    text = "Book designer ";
                    break;
                case "BKP":
                    text = "Book producer ";
                    break;
                case "BND":
                    text = "Binder ";
                    break;
                case "BPD":
                    text = "Bookplate designer ";
                    break;
                case "BSL":
                    text = "Bookseller ";
                    break;
                case "CCP":
                    text = "Conceptor ";
                    break;
                case "CHR":
                    text = "Choreographer ";
                    break;
                case "CLB":
                    text = "Collaborator ";
                    break;
                case "CLI":
                    text = "Client ";
                    break;
                case "CLL":
                    text = "Calligrapher ";
                    break;
                case "CLT":
                    text = "Collotyper ";
                    break;
                case "CMM":
                    text = "Commentator ";
                    break;
                case "CMP":
                    text = "Composer ";
                    break;
                case "CMT":
                    text = "Compositor ";
                    break;
                case "CNG":
                    text = "Cinematographer";
                    break;
                case "CND":
                    text = "Conductor ";
                    break;
                case "CNS":
                    text = "Censor ";
                    break;
                case "COE":
                    text = "Contestant -appellee ";
                    break;
                case "COL":
                    text = "Collector ";
                    break;
                case "COM":
                    text = "Compiler ";
                    break;
                case "COS":
                    text = "Contestant ";
                    break;
                case "COT":
                    text = "Contestant -appellant ";
                    break;
                case "COV":
                    text = "Cover designer ";
                    break;
                case "CPC":
                    text = "Copyright claimant ";
                    break;
                case "CPE":
                    text = "Complainant-appellee ";
                    break;
                case "CPH":
                    text = "Copyright holder ";
                    break;
                case "CPL":
                    text = "Complainant ";
                    break;
                case "CPT":
                    text = "Complainant-appellant ";
                    break;
                case "CRE":
                    text = "Creator ";
                    break;
                case "CRP":
                    text = "Correspondent ";
                    break;
                case "CRR":
                    text = "Corrector ";
                    break;
                case "CSL":
                    text = "Consultant ";
                    break;
                case "CSP":
                    text = "Consultant to a project ";
                    break;
                case "CST":
                    text = "Costume designer ";
                    break;
                case "CTB":
                    text = "Contributor ";
                    break;
                case "CTE":
                    text = "Contestee-appellee ";
                    break;
                case "CTG":
                    text = "Cartographer ";
                    break;
                case "CTR":
                    text = "Contractor ";
                    break;
                case "CTS":
                    text = "Contestee ";
                    break;
                case "CTT":
                    text = "Contestee-appellant ";
                    break;
                case "CUR":
                    text = "Curator ";
                    break;
                case "CWT":
                    text = "Commentator for written text ";
                    break;
                case "DFD":
                    text = "Defendant ";
                    break;
                case "DFE":
                    text = "Defendant-appellee ";
                    break;
                case "DFT":
                    text = "Defendant-appellant ";
                    break;
                case "DGG":
                    text = "Degree grantor ";
                    break;
                case "DIS":
                    text = "Dissertant ";
                    break;
                case "DLN":
                    text = "Delineator ";
                    break;
                case "DNC":
                    text = "Dancer ";
                    break;
                case "DNR":
                    text = "Donor ";
                    break;
                case "DPC":
                    text = "Depicted ";
                    break;
                case "DPT":
                    text = "Depositor ";
                    break;
                case "DRM":
                    text = "Draftsman ";
                    break;
                case "DRT":
                    text = "Director ";
                    break;
                case "DSR":
                    text = "Designer ";
                    break;
                case "DST":
                    text = "Distributor ";
                    break;
                case "DTE":
                    text = "Dedicatee ";
                    break;
                case "DTO":
                    text = "Dedicator ";
                    break;
                case "DUB":
                    text = "Dubious author ";
                    break;
                case "EDT":
                    text = "Editor ";
                    break;
                case "EGR":
                    text = "Engraver ";
                    break;
                case "ELT":
                    text = "Electrotyper ";
                    break;
                case "ENG":
                    text = "Engineer ";
                    break;
                case "ETR":
                    text = "Etcher ";
                    break;
                case "EXP":
                    text = "Expert ";
                    break;
                case "FAC":
                    text = "Facsimilist ";
                    break;
                case "FLM":
                    text = "Film editor ";
                    break;
                case "FMO":
                    text = "Former owner ";
                    break;
                case "FPY":
                    text = "First part ";
                    break;
                case "FND":
                    text = "Funder ";
                    break;
                case "FRG":
                    text = "Forger ";
                    break;
                case "GRT":
                    text = "Graphic technician ";
                    break;
                case "HNR":
                    text = "Honoree ";
                    break;
                case "HST":
                    text = "Host ";
                    break;
                case "ILL":
                    text = "Illustrator ";
                    break;
                case "ILU":
                    text = "Illuminator ";
                    break;
                case "INS":
                    text = "Inscriber ";
                    break;
                case "INV":
                    text = "Inventor ";
                    break;
                case "ITR":
                    text = "Instrumentalist ";
                    break;
                case "IVE":
                    text = "Interviewee ";
                    break;
                case "IVR":
                    text = "Interviewer ";
                    break;
                case "LBT":
                    text = "Librettist ";
                    break;
                case "LEE":
                    text = "Libelee-appellee ";
                    break;
                case "LEL":
                    text = "Libelee ";
                    break;
                case "LEN":
                    text = "Lender ";
                    break;
                case "LET":
                    text = "Libelee-appellant ";
                    break;
                case "LGD":
                    text = "Lighting designer";
                    break;
                case "LIE":
                    text = "Libelant-appellee ";
                    break;
                case "LIL":
                    text = "Libelant ";
                    break;
                case "LIT":
                    text = "Libelant-appellant ";
                    break;
                case "LSA":
                    text = "Landscape architect ";
                    break;
                case "LSE":
                    text = "Licensee ";
                    break;
                case "LSO":
                    text = "Licensor ";
                    break;
                case "LTG":
                    text = "Lithographer ";
                    break;
                case "LYR":
                    text = "Lyricist ";
                    break;
                case "MFR":
                    text = "Manufacturer";
                    break;
                case "MDC":
                    text = "Metadata contact ";
                    break;
                case "MOD":
                    text = "Moderator ";
                    break;
                case "MON":
                    text = "Monitor ";
                    break;
                case "MRK":
                    text = "Markup editor ";
                    break;
                case "MTE":
                    text = "Metal-engraver ";
                    break;
                case "MUS":
                    text = "Musician ";
                    break;
                case "NRT":
                    text = "Narrator ";
                    break;
                case "OPN":
                    text = "Opponent ";
                    break;
                case "ORG":
                    text = "Originator ";
                    break;
                case "ORM":
                    text = "Organizer of meeting ";
                    break;
                case "OTH":
                    text = "Other ";
                    break;
                case "OWN":
                    text = "Owner ";
                    break;
                case "PAT":
                    text = "Patron ";
                    break;
                case "PBD":
                    text = "Publishing director ";
                    break;
                case "PFR":
                    text = "Proofreader ";
                    break;
                case "PLT":
                    text = "Platemaker ";
                    break;
                case "POP":
                    text = "Printer of plates ";
                    break;
                case "PPM":
                    text = "Papermaker ";
                    break;
                case "PPT":
                    text = "Puppeteer";
                    break;
                case "PRC":
                    text = "Process contact ";
                    break;
                case "PRD":
                    text = "Production personnel ";
                    break;
                case "PRF":
                    text = "Performer ";
                    break;
                case "PRG":
                    text = "Programmer ";
                    break;
                case "PRM":
                    text = "Printmaker ";
                    break;
                case "PRO":
                    text = "Producer ";
                    break;
                case "PRT":
                    text = "Printer ";
                    break;
                case "PTA":
                    text = "Patent applicant ";
                    break;
                case "PTE":
                    text = "Plaintiff -appellee ";
                    break;
                case "PTF":
                    text = "Plaintiff ";
                    break;
                case "PTH":
                    text = "Patent holder ";
                    break;
                case "PTT":
                    text = "Plaintiff-appellant ";
                    break;
                case "RBR":
                    text = "Rubricator ";
                    break;
                case "RCE":
                    text = "Recording engineer ";
                    break;
                case "RCP":
                    text = "Recipient ";
                    break;
                case "RED":
                    text = "Redactor ";
                    break;
                case "REN":
                    text = "Renderer ";
                    break;
                case "RES":
                    text = "Researcher ";
                    break;
                case "REV":
                    text = "Reviewer ";
                    break;
                case "RPT":
                    text = "Reporter ";
                    break;
                case "RPY":
                    text = "Responsible party ";
                    break;
                case "RSE":
                    text = "Respondent -appellee ";
                    break;
                case "RSG":
                    text = "Restager";
                    break;
                case "RSP":
                    text = "Respondent ";
                    break;
                case "RST":
                    text = "Respondent-appellant ";
                    break;
                case "RTH":
                    text = "Research team head ";
                    break;
                case "RTM":
                    text = "Research team member ";
                    break;
                case "SAD":
                    text = "Scientific advisor ";
                    break;
                case "SCE":
                    text = "Scenarist ";
                    break;
                case "SCL":
                    text = "Sculptor ";
                    break;
                case "SCR":
                    text = "Scribe ";
                    break;
                case "SEC":
                    text = "Secretary ";
                    break;
                case "SGN":
                    text = "Signer ";
                    break;
                case "SNG":
                    text = "Singer ";
                    break;
                case "SPK":
                    text = "Speaker ";
                    break;
                case "SPN":
                    text = "Sponsor ";
                    break;
                case "SPY":
                    text = "Second part ";
                    break;
                case "SRV":
                    text = "Surveyor ";
                    break;
                case "STD":
                    text = "Set designer";
                    break;
                case "STL":
                    text = "Storyteller ";
                    break;
                case "STN":
                    text = "Standards body ";
                    break;
                case "STR":
                    text = "Stereotyper ";
                    break;
                case "TCH":
                    text = "Teacher ";
                    break;
                case "THS":
                    text = "Thesis advisor ";
                    break;
                case "TRC":
                    text = "Transcriber ";
                    break;
                case "TRL":
                    text = "Translator ";
                    break;
                case "TYD":
                    text = "Type designer ";
                    break;
                case "TYG":
                    text = "Typographer ";
                    break;
                case "VDG":
                    text = "Videographer";
                    break;
                case "VOC":
                    text = "Vocalist ";
                    break;
                case "WAM":
                    text = "Writer of accompanying material ";
                    break;
                case "WDC":
                    text = "Woodcutter ";
                    break;
                case "WDE":
                    text = "Wood-engraver ";
                    break;
                case "WIT":
                    text = "Witness ";
                    break;
                case "AUT":
                    text = "Author";
                    break;
                case "PHT":
                    text = "Photographer";
                    break;
                case "ACT":
                    text = "Actor";
                    break;
                case "PBL":
                    text = "Publisher";
                    break;
            }

            return text;
        }
    }

    #endregion
}