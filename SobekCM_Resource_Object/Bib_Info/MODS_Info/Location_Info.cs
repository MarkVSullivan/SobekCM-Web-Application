#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary>The "location" identifies the institution or repository holding the resource, 
    /// or a remote location in the form of a URL where it is available. </summary>
    /// <remarks>This class extends the <see cref="XML_Node_Base_Type"/> class for writing to XML </remarks>
    [Serializable]
    public class Location_Info : XML_Writing_Base_Type
    {
        private string ead_name;
        private string ead_url;
        private string holding_code;
        private string holding_name;
        private string other_url;
        private string other_url_label;
        private string other_url_note;
        private string purl;

        /// <summary> Constructor creates a new instance of the Location_Info class</summary>
        public Location_Info()
        {
            // Do nothing
        }

        /// <summary> Gets or sets the code for the holding location/institution </summary>
        public string Holding_Code
        {
            get { return holding_code ?? String.Empty; }
            set { set_code(value); }
        }

        /// <summary> Gets or sets the name for the holding location/institution </summary>
        public string Holding_Name
        {
            get { return holding_name ?? String.Empty; }
            set { holding_name = value; }
        }

        /// <summary> Gets or sets the (semi)permanent URL for this item in SobekCM </summary>
        public string PURL
        {
            get { return purl ?? String.Empty; }
            set { purl = value; }
        }

        /// <summary> Gets or sets the an additional link </summary>
        public string Other_URL
        {
            get { return other_url ?? String.Empty; }
            set { other_url = value; }
        }

        /// <summary> Gets or sets the display information for the additional link </summary>
        public string Other_URL_Display_Label
        {
            get { return other_url_label ?? String.Empty; }
            set { other_url_label = value; }
        }

        /// <summary> Gets or sets the note for the additional link </summary>
        public string Other_URL_Note
        {
            get { return other_url_note ?? String.Empty; }
            set { other_url_note = value; }
        }

        /// <summary> Gets or sets the URL for the EAD finding guide </summary>
        public string EAD_URL
        {
            get { return ead_url ?? String.Empty; }
            set { ead_url = value; }
        }

        /// <summary> Gets or sets the name for the EAD finding guide </summary>
        public string EAD_Name
        {
            get { return ead_name ?? String.Empty; }
            set { ead_name = value; }
        }

        private void set_code(string newCode)
        {
            holding_code = newCode;
            switch (holding_code.ToUpper())
            {
                case "AM":
                case "IAM":
                    holding_code = "FAMU";
                    break;

                case "CF":
                case "ICF":
                    holding_code = "UCF";
                    break;

                case "FI":
                case "IFI":
                    holding_code = "FIU";
                    break;

                case "FS":
                case "IFS":
                    holding_code = "FSU";
                    break;

                case "MHM":
                case "IMHM":
                case "MH":
                case "IMH":
                    holding_code = "MATHESON";
                    break;

                case "NF":
                case "INF":
                    holding_code = "UNF";
                    break;

                case "UFL":
                case "IUFL":
                    holding_code = "UF";
                    break;

                case "UOV":
                case "IUOV":
                    holding_code = "UDO";
                    break;

                case "WT":
                case "IWT":
                    holding_code = "WIDECAST";
                    break;

                case "UFSC":
                case "IUFSC":
                    holding_code = "UFSPEC";
                    break;

                case "UFIR":
                case "IUFIR":
                    holding_code = "UF";
                    break;

                case "WA":
                case "IWA":
                    holding_code = "WC";
                    break;

                case "FMNH":
                case "IFMNH":
                    holding_code = "FLMNH";
                    break;

                case "FC":
                case "IFC":
                    holding_code = "FLAGLER";
                    break;
            }
        }


        /// <summary> Clear all the location information about this object </summary>
        public void Clear()
        {
            purl = null;
            other_url = null;
            other_url_label = null;
            other_url_note = null;
            ead_url = null;
            ead_name = null;
        }

        /// <summary> Writes this location as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="returnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS(TextWriter returnValue)
        {
            if ((String.IsNullOrEmpty(holding_code)) && (String.IsNullOrEmpty(holding_name)) && (String.IsNullOrEmpty(other_url)) && (String.IsNullOrEmpty(purl)) && (String.IsNullOrEmpty(ead_url)))
                return;

            returnValue.Write("<mods:location>\r\n");

            if (!String.IsNullOrEmpty(holding_name))
                returnValue.Write("<mods:physicalLocation>" + base.Convert_String_To_XML_Safe(holding_name) + "</mods:physicalLocation>\r\n");

            if (!String.IsNullOrEmpty(holding_code))
                returnValue.Write("<mods:physicalLocation type=\"code\">" + holding_code + "</mods:physicalLocation>\r\n");

            if (!String.IsNullOrEmpty(purl))
                returnValue.Write("<mods:url access=\"object in context\">" + base.Convert_String_To_XML_Safe(purl) + "</mods:url>\r\n");

            if (!String.IsNullOrEmpty(other_url))
            {
                returnValue.Write("<mods:url");

                if (!String.IsNullOrEmpty(other_url_label))
                    returnValue.Write(" displayLabel=\"" + base.Convert_String_To_XML_Safe(other_url_label) + "\"");
                else
                    returnValue.Write(" displayLabel=\"Related URL\"");

                if (!String.IsNullOrEmpty(other_url_note))
                    returnValue.Write(" note=\"" + base.Convert_String_To_XML_Safe(other_url_note) + "\"");

                returnValue.Write(">" + base.Convert_String_To_XML_Safe(other_url) + "</mods:url>\r\n");
            }

            if (!String.IsNullOrEmpty(ead_url))
            {
                returnValue.Write("<mods:url displayLabel=\"Finding Guide\"");

                if (!String.IsNullOrEmpty(ead_name))
                    returnValue.Write(" note=\"" + base.Convert_String_To_XML_Safe(ead_name) + "\"");

                returnValue.Write(">" + base.Convert_String_To_XML_Safe(ead_url) + "</mods:url>\r\n");
            }

            returnValue.Write("</mods:location>\r\n");
        }
    }
}