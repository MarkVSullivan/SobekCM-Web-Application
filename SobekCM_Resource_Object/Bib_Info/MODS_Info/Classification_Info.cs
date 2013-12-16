#region Using directives

using System;
using System.IO;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary>Stores the information about any classifications associated with this digital resource. </summary>
    /// <remarks>This class extends the <see cref="XML_Node_Base_Type"/> class for writing to XML </remarks>
    [Serializable]
    public class Classification_Info : XML_Node_Base_Type, IEquatable<Classification_Info>
    {
        private string authority;
        private string classification;
        private string displayLabel;
        private string edition;

        /// <summary> Constructor for a new instance of the Classification_Info class </summary>
        public Classification_Info()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the Classification_Info class </summary>
        public Classification_Info(string Classification, string Authority)
        {
            classification = Classification;
            authority = Authority;
        }

        /// <summary> Gets or sets this classification term </summary>
        public string Classification
        {
            get { return classification ?? String.Empty; }
            set { classification = value; }
        }

        /// <summary> Gets or sets the display label for this classification term </summary>
        public string Display_Label
        {
            get { return displayLabel ?? String.Empty; }
            set { displayLabel = value; }
        }

        /// <summary> Gets or sets the authority for this classification term </summary>
        public string Authority
        {
            get { return authority ?? String.Empty; }
            set { authority = value; }
        }

        /// <summary> Gets or sets the edition for this classification term </summary>
        public string Edition
        {
            get { return edition ?? String.Empty; }
            set { edition = value; }
        }

        #region IEquatable<Classification_Info> Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Classification_Info other)
        {
            if (classification == other.Classification)
                return true;
            else
                return false;
        }

        #endregion

        /// <summary> Writes this classification as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="returnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS(TextWriter returnValue)
        {
            if (String.IsNullOrEmpty(classification))
                return;

            returnValue.Write("<mods:classification");
            base.Add_ID(returnValue);
            if (!String.IsNullOrEmpty(authority))
                returnValue.Write(" authority=\"" + authority + "\"");
            if (!String.IsNullOrEmpty(displayLabel))
                returnValue.Write(" displayLabel=\"" + base.Convert_String_To_XML_Safe(displayLabel) + "\"");
            if (!String.IsNullOrEmpty(edition))
                returnValue.Write(" edition=\"" + base.Convert_String_To_XML_Safe(edition) + "\"");
            returnValue.Write(">" + base.Convert_String_To_XML_Safe(classification) + "</mods:classification>\r\n");
        }

        internal MARC_Field to_MARC_Tag()
        {
            if (String.IsNullOrEmpty(classification))
                return null;

            MARC_Field returnValue = new MARC_Field();
            returnValue.Indicators = "  ";
            returnValue.Tag = 84;


            string authority_builder = String.Empty;
            if (!String.IsNullOrEmpty(authority))
            {
                switch (authority.ToLower())
                {
                    case "lcc":
                        returnValue.Tag = 50;
                        if (!String.IsNullOrEmpty(displayLabel))
                            authority_builder = "|3 " + displayLabel;
                        returnValue.Indicator1 = ' ';
                        returnValue.Indicator2 = '4';
                        break;

                    case "ddc":
                        returnValue.Tag = 82;
                        if (!String.IsNullOrEmpty(edition))
                            authority_builder = "|2 " + edition;
                        break;

                    case "udc":
                        returnValue.Tag = 80;
                        break;

                    case "nlm":
                        returnValue.Tag = 60;
                        break;

                    case "sudocs":
                        returnValue.Tag = 86;
                        returnValue.Indicator1 = '0';
                        break;

                    case "candocs":
                        returnValue.Tag = 86;
                        returnValue.Indicator1 = '1';
                        break;

                    default:
                        if (!String.IsNullOrEmpty(authority))
                            authority_builder = "|2 " + authority;
                        break;
                }
            }
            returnValue.Control_Field_Value = "|a " + classification + authority_builder;
            return returnValue;
        }
    }
}