using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Title used as a subject for a digital resource </summary>
    [Serializable]
    public class Subject_Info_TitleInfo : Subject_Standard_Base
    {
        private Title_Info titleInfo;

        /// <summary> Constructor for a new instance of the title subject class </summary>
        public Subject_Info_TitleInfo()
        {
            titleInfo = new Title_Info();
        }

        #region Public properties

        /// <summary> Gets the Title_Info object associated with this subject </summary>
        public Title_Info Title_Object
        {
            get { return titleInfo; }
        }

        /// <summary> Gets or sets the type of this title </summary>
        public Title_Type_Enum Title_Type
        {
            get { return titleInfo.Title_Type; }
            set { titleInfo.Title_Type = value; }
        }

        /// <summary> Gets or sets the display label for this title </summary>
        public string Display_Label
        {
            get { return titleInfo.Display_Label; }
            set { titleInfo.Display_Label = value; }
        }

        /// <summary> Gets or sets the non sort portion of this title </summary>
        public string NonSort
        {
            get { return titleInfo.NonSort; }
            set { titleInfo.NonSort = value; }
        }

        /// <summary> Gets or sets the actual title portion of this title </summary>
        public string Title
        {
            get { return titleInfo.Title; }
            set { titleInfo.Title = value; }
        }

        /// <summary> Gets the title as XML-safe string </summary>
        internal string Title_XML
        {
            get { return titleInfo.Title_XML; }
        }

        /// <summary> Gets or sets the subtitle portion </summary>
        public string Subtitle
        {
            get { return titleInfo.Subtitle; }
            set { titleInfo.Subtitle = value; }
        }

        /// <summary> Gets the collection of part numbers for this title </summary>
        public ReadOnlyCollection<string> Part_Numbers
        {
            get { return titleInfo.Part_Numbers; }
        }

        /// <summary> Gets or sets the part names for this title </summary>
        public ReadOnlyCollection<string> Part_Names
        {
            get { return titleInfo.Part_Names; }
        }

        #endregion

        /// <summary> Indicates this is the title info subclass of Subject_Info </summary>
        public override Subject_Info_Type Class_Type
        {
            get { return Subject_Info_Type.TitleInfo; }
        }


        internal void Set_Internal_Title(Title_Info NewTitle)
        {
            titleInfo = NewTitle;
        }

        /// <summary> Write the subject information out to string format</summary>
        /// <returns> This subject expressed as a string</returns>
        /// <remarks> The scheme is included in this string</remarks>
        public override string ToString()
        {
            return ToString(true);
        }

        /// <summary> Write the subject information out to string format</summary>
        /// <param name="Include_Scheme"> Flag indicates whether the scheme should be included</param>
        /// <returns> This subject expressed as a string</returns>
        public override string ToString(bool Include_Scheme)
        {
            StringBuilder builder = new StringBuilder();
            builder.Append((titleInfo.NonSort + " " + titleInfo.Title + " " + titleInfo.Subtitle).Trim());

            builder.Append(base.To_Base_String());

            if (Include_Scheme)
            {
                if ( !String.IsNullOrEmpty(authority))
                    builder.Append(" ( " + authority + " )");
            }
            
            return Convert_String_To_XML_Safe(builder.ToString());
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
            if ( titleInfo.Title.Length == 0 )
                return;

            results.Write( "<mods:subject");
            base.Add_ID(results);
            if (!String.IsNullOrEmpty(language))
                results.Write(" lang=\"" + language + "\"");
            if (!String.IsNullOrEmpty(authority))
                results.Write(" authority=\"" + authority + "\"");
            results.Write(">\r\n");
            
            titleInfo.Add_MODS( results );

            base.Add_Base_MODS( results);

            results.Write( "</mods:subject>\r\n");
        }

        internal override MARC_Field to_MARC_HTML()
        {
            MARC_Field returnValue = titleInfo.to_MARC_HTML(630, 1, String.Empty, String.Empty);

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

            returnValue.Control_Field_Value = fieldBuilder.ToString().Trim();

            return returnValue;
        }
    }
}
