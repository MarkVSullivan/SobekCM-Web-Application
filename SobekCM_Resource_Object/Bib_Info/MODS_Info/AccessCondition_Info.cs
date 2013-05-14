#region Using directives

using System;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary>Stores the information about any access conditions or rights associated with this digital resource. </summary>
    /// <remarks>This class extends the <see cref="XML_Node_Base_Type"/> class for writing to XML </remarks>
    [Serializable]
    public class AccessCondition_Info : XML_Node_Base_Type
    {
        private string displayLabel;
        private string language;
        private string rights_text;
        private string rights_type;

        /// <summary> Constructor creates an empty access condition object </summary>
        public AccessCondition_Info()
        {
            // Do nothing
        }

        /// <summary>Gets or set information about restrictions imposed on access to a resource</summary>
        public string Text
        {
            get { return rights_text ?? String.Empty; }
            set { rights_text = value; }
        }

        /// <summary>Gets or sets the uncontrolled type of access condition.</summary>
        /// <remarks>There is no controlled list of types for accessCondition defined.  Suggested values are: restriction on access, and use and reproduction.</remarks>
        public string Type
        {
            get { return rights_type ?? "restrictions on use"; }
            set { rights_type = value; }
        }

        /// <summary>Gets or sets the additional text associated with the access conditions which is necessary for display.</summary>
        public string Display_Label
        {
            get { return displayLabel ?? "Rights"; }
            set { displayLabel = value; }
        }

        /// <summary> Gets or sets the language of this accessCondition text </summary>
        public string Language
        {
            get { return language ?? String.Empty; }
            set { language = value; }
        }

        /// <summary> Clear all the data associated with this access condition object </summary>
        public void Clear()
        {
            rights_text = null;
            rights_type = null;
            language = null;
            displayLabel = null;
        }

        /// <summary> Writes this access condition as MODS to a writer writing to a stream ( either a file or web response stream )</summary>
        /// <param name="returnValue"> Writer to the MODS building stream </param>
        internal void Add_MODS(TextWriter returnValue)
        {
            if (String.IsNullOrEmpty(rights_text))
                return;

            returnValue.Write("<mods:accessCondition");
            base.Add_ID(returnValue);
            if (!String.IsNullOrEmpty(rights_type))
                returnValue.Write(" type=\"" + base.Convert_String_To_XML_Safe(rights_type) + "\"");
            if (!String.IsNullOrEmpty(displayLabel))
                returnValue.Write(" displayLabel=\"" + base.Convert_String_To_XML_Safe(displayLabel) + "\"");
            if (!String.IsNullOrEmpty(language))
                returnValue.Write(" lang=\"" + language + "\"");
            returnValue.Write(">" + base.Convert_String_To_XML_Safe(rights_text) + "</mods:accessCondition>\r\n");
        }
    }
}