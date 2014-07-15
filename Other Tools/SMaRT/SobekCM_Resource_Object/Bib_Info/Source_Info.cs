#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Stores the source institution information about a digital resource </summary>
    /// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Source_Info : XML_Writing_Base_Type
    {
        private string code;
        private string statement;

        /// <summary> Constructor for an empty instance of the Source_Info class </summary>
        public Source_Info()
        {
            // Do nothing
        }

        /// <summary> Constructor a new instance of the Source_Info class </summary>
        /// <param name="Code">Source Institution Code</param>
        /// <param name="Statement">Source Institution Statement</param>
        public Source_Info(string Code, string Statement)
        {
            set_code(Code);
            statement = Statement;
        }

        /// <summary> Gets or sets the code associated with this source institution </summary>
        public string Code
        {
            get { return code ?? String.Empty; }
            set { set_code(value); }
        }

        /// <summary> Gets or sets the statement associated with this source institution </summary>
        public string Statement
        {
            get { return statement ?? String.Empty; }
            set { statement = value; }
        }

        /// <summary> Gets the source statement as a XML-encoded string </summary>
        internal string XML_Safe_Statement
        {
            get { return base.Convert_String_To_XML_Safe(statement); }
        }

        private void set_code(string newCode)
        {
            code = newCode;
            switch (code.ToUpper())
            {
                case "AM":
                case "IAM":
                    code = "FAMU";
                    break;

                case "CF":
                case "ICF":
                    code = "UCF";
                    break;

                case "FI":
                case "IFI":
                    code = "FIU";
                    break;

                case "FS":
                case "IFS":
                    code = "FSU";
                    break;

                case "MHM":
                case "IMHM":
                case "MH":
                case "IMH":
                    code = "MATHESON";
                    break;

                case "NF":
                case "INF":
                    code = "UNF";
                    break;

                case "UFL":
                case "IUFL":
                    code = "UF";
                    break;

                case "UOV":
                case "IUOV":
                    code = "UDO";
                    break;

                case "WT":
                case "IWT":
                    code = "WIDECAST";
                    break;

                case "UFSC":
                case "IUFSC":
                    code = "UFSPEC";
                    break;


                case "UFIR":
                case "IUFIR":
                    code = "UF";
                    break;

                case "WA":
                case "IWA":
                    code = "WC";
                    break;

                case "FMNH":
                case "IFMNH":
                    code = "FLMNH";
                    break;

                case "FC":
                case "IFC":
                    code = "FLAGLER";
                    break;
            }
        }

        /// <summary> Writes this source information as SobekCM-formatted XML </summary>
        /// <param name="sobekcm_namespace"> Namespace to use for the SobekCM custom schema ( usually 'sobekcm' )</param>
        /// <param name="results"> Stream to write this source information as SobekCM-formatted XML</param>
        internal void Add_SobekCM_Metadata(string sobekcm_namespace, TextWriter results)
        {
            if (!String.IsNullOrEmpty(statement))
            {
                if (!String.IsNullOrEmpty(code))
                {
                    results.Write("<" + sobekcm_namespace + ":Source>\r\n");
                    results.Write("<" + sobekcm_namespace + ":statement code=\"" + code + "\">" + base.Convert_String_To_XML_Safe(statement) + "</" + sobekcm_namespace + ":statement>\r\n");
                    results.Write("</" + sobekcm_namespace + ":Source>\r\n");
                }
                else
                {
                    results.Write("<" + sobekcm_namespace + ":Source>\r\n");
                    results.Write("<" + sobekcm_namespace + ":statement>" + base.Convert_String_To_XML_Safe(statement) + "</" + sobekcm_namespace + ":statement>\r\n");
                    results.Write("</" + sobekcm_namespace + ":Source>\r\n");
                }
            }
        }
    }
}