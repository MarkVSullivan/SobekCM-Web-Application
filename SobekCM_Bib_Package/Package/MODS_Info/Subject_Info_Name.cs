using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Name used as a subject for a digital resource </summary>
    [Serializable]
    public class Subject_Info_Name : Subject_Standard_Base
    {
        private Name_Info nameInfo;

        /// <summary> Constructor for a new instance of the Subject_Info_Name class </summary>
        public Subject_Info_Name()
        {
            nameInfo = new Name_Info();
        }

        #region Public properties

        /// <summary> Gets the Name_Info object associated with this subject </summary>
        public Name_Info Name_Object
        {
            get { return nameInfo; }
        }

        /// <summary> Gets or sets the type of name subject </summary>
        /// <remarks>Possible types are personal, corporate, conference, or unspecified</remarks>
        public Name_Info_Type_Enum Name_Type
        {
            get { return nameInfo.Name_Type; }
            set { nameInfo.Name_Type = value; }
        }

        /// <summary> Gets or sets the ID associated with this name subject </summary>
        /// <remarks>This is used to link this name in the MDOS section to a hierarchical affiliation in the SobekCM section</remarks>
        public string NameID
        {
            get { return nameInfo.ID; }
            set { nameInfo.ID = value; }
        }

        /// <summary> Gets or sets the full name of this entity </summary>
        public string Full_Name
        {
            get { return nameInfo.Full_Name; }
            set { nameInfo.Full_Name = value; }
        }

        /// <summary> Gets or sets the given (first) names associated with this entity </summary>
        public string Given_Name
        {
            get { return nameInfo.Given_Name; }
            set { nameInfo.Given_Name = value; }
        }

        /// <summary> Gets or sets the family (last) name associated with this entity </summary>
        public string Family_Name
        {
            get { return nameInfo.Family_Name; }
            set { nameInfo.Family_Name = value; }
        }

        /// <summary> Gets or sets the dates associated with this name entity </summary>
        public string Dates
        {
            get { return nameInfo.Dates; }
            set { nameInfo.Dates = value; }
        }

        /// <summary> Gets or sets the terms of address associated with this entity </summary>
        public string Terms_Of_Address
        {
            get { return nameInfo.Terms_Of_Address; }
            set { nameInfo.Terms_Of_Address = value; }
        }

        /// <summary> Gets or sets additional information necessary for displaying this name entity </summary>
        public string Display_Form
        {
            get { return nameInfo.Display_Form; }
            set { nameInfo.Display_Form = value; }
        }

        /// <summary> Gets or sets the simple affiliation associated with this entity </summary>
        /// <remarks>For more details, use the Affiliation_Info object</remarks>
        public string Affiliation
        {
            get { return nameInfo.Affiliation; }
            set { nameInfo.Affiliation = value; }
        }

        /// <summary> Gets or sets any related description for this name entity </summary>
        public string Description
        {
            get { return nameInfo.Description; }
            set { nameInfo.Description = value; }
        }

        /// <summary> Gets the collection of roles for this entity </summary>
        public List<Name_Info_Role> Roles
        {
            get { return nameInfo.Roles; }
        }

        #endregion

        #region Public Methods

        /// <summary> Add a new role to this name entity </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Authority">Authority for this role term</param>
        /// <param name="Role_Type">Type of role</param>
        public void Add_Role(string Role, string Authority, Name_Info_Role_Type_Enum Role_Type)
        {
            nameInfo.Roles.Add(new Name_Info_Role(Role, Authority, Role_Type));
        }

        /// <summary> Add a new role to this name entity </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Authority">Authority for this role term</param>
        public void Add_Role(string Role, string Authority)
        {
            nameInfo.Roles.Add(new Name_Info_Role(Role, Authority));
        }

        /// <summary> Add a new role to this name entity </summary>
        /// <param name="Role">Text of the role</param>
        /// <param name="Role_Type">Type of role</param>
        public void Add_Role(string Role, Name_Info_Role_Type_Enum Role_Type)
        {
            nameInfo.Roles.Add(new Name_Info_Role(Role, Role_Type));
        }

        #endregion

        /// <summary> Indicates this is the Subject Name subclass of Subject_Info </summary>
        public override Subject_Info_Type Class_Type
        {
            get { return Subject_Info_Type.Name; }
        }

        internal void Set_Internal_Name(Name_Info NewName)
        {
            nameInfo = NewName;
        }

        /// <summary> Writes this name used as a subject as a simple string </summary>
        /// <returns> This name as a subject returned as a simple string </returns>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary> Writes this name used as a subject as a simple string </summary>
        /// <param name="Include_Scheme"> Flag indicates whether the role(s) should be included </param>
        /// <returns> This name as a subject returned as a simple string </returns>
        public override string ToString(bool Include_Scheme)
        {
            StringBuilder builder = new StringBuilder();
            if (nameInfo.Full_Name.Length > 0)
                builder.Append( nameInfo.ToString() );

            builder.Append(base.To_Base_String());

            if (Include_Scheme)
            {
                if ( !String.IsNullOrEmpty(authority ))
                    builder.Append(" ( " + authority + " )");
            }

            return builder.ToString();
        }

        internal override string To_GSA_DublinCore(string indent)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append(To_GSA(ToString(), "dc.Subject", indent));
            if (geographics != null)
            {
                foreach (string thisGeographic in geographics)
                    builder.Append(To_GSA(thisGeographic, "dc.Coverage^spatial", indent));
            }
            return builder.ToString();
        }

        internal override string To_GSA_SobekCM(string indent)
        {
            return String.Empty;
        }

        internal override void Add_MODS(System.IO.TextWriter results)
        {
            results.Write("<mods:subject");
            base.Add_ID(results);
            if (!String.IsNullOrEmpty(language))
                results.Write(" lang=\"" + language + "\"");
            if (!String.IsNullOrEmpty(authority))
                results.Write(" authority=\"" + authority + "\"");
            results.Write(">\r\n");

            nameInfo.Add_MODS( false, results);

            Add_Base_MODS( results );

            results.Write( "</mods:subject>\r\n");
        }

        internal override MARC_Field to_MARC_HTML()
        {
            MARC_Field returnValue = nameInfo.to_MARC_HTML(true);

            // Set the tag
            switch (nameInfo.Name_Type)
            {
                case Name_Info_Type_Enum.personal:
                    returnValue.Tag = 600;
                    break;

                case Name_Info_Type_Enum.corporate:
                    returnValue.Tag = 610;
                    break;

                case Name_Info_Type_Enum.conference:
                    returnValue.Tag = 611;
                    break;
            }

            // Add to the built field
            StringBuilder fieldBuilder = new StringBuilder(returnValue.Control_Field_Value + " ");

            if (geographics != null)
            {
                foreach (string geo in geographics)
                {
                    fieldBuilder.Append("|z " + geo + " ");
                }
            }

            if (temporals != null)
            {
                foreach (string temporal in temporals)
                {
                    fieldBuilder.Append("|y " + temporal + " ");
                }
            }

            if (topics != null)
            {
                foreach (string topic in topics)
                {
                    fieldBuilder.Append("|x " + topic + " ");
                }
            }

            if (genres != null)
            {
                foreach (string form in genres)
                {
                    fieldBuilder.Append("|v " + form + " ");
                }
            }
            
            base.Add_Source_Indicator(returnValue, fieldBuilder);

            // Add the relator codes last
            foreach (Name_Info_Role thisRole in nameInfo.Roles)
            {
                if ((thisRole.Role_Type == Name_Info_Role_Type_Enum.code) && (thisRole.Authority == "marcrelator"))
                {
                    fieldBuilder.Append("|4 " + thisRole.Role + " ");
                }
            }

            returnValue.Control_Field_Value = fieldBuilder.ToString().Trim();

            return returnValue;
        }

    }
}
