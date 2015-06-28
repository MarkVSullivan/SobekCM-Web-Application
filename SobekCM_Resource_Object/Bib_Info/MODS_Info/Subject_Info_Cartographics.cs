#region Using directives

using System;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Subject information which includes cartographic data indicating spatial coverage </summary>
    [Serializable]
    public class Subject_Info_Cartographics : Subject_Info
    {
        private string coordinates;
        private string projection;
        private string scale;

        /// <summary> Constuctor for a new instance of the Subject_Info_Cartographics class </summary>
        public Subject_Info_Cartographics()
        {
            // Do nothing
        }

        /// <summary> Gets or sets the scale for this cartographic material </summary>
        public string Scale
        {
            get { return scale ?? String.Empty; }
            set { scale = value; }
        }

        /// <summary> Gets or sets the simple coordinates for this cartographic material </summary>
        public string Coordinates
        {
            get { return coordinates ?? String.Empty; }
            set { coordinates = value; }
        }

        /// <summary> Gets or sets the projection for this cartographic material </summary>
        public string Projection
        {
            get { return projection ?? String.Empty; }
            set { projection = value; }
        }

        /// <summary> Indicates this is the Cartographics subclass of Subject_Info </summary>
        public override Subject_Info_Type Class_Type
        {
            get { return Subject_Info_Type.Cartographics; }
        }

        /// <summary> Writes this cartographic subject as a simple string </summary>
        /// <returns> This cartographic subject returned as a simple string </returns>
        public override string ToString()
        {
            return String.Empty;
        }

        /// <summary> Writes this cartographic subject as a simple string </summary>
        /// <param name="Include_Scheme">Flag indicates if the scheme should be included</param>
        /// <returns> This cartographic subject returned as a simple string </returns>
        public override string ToString(bool Include_Scheme)
        {
            return ToString();
        }

        internal override void Add_MODS(TextWriter Results)
        {
            if ((String.IsNullOrEmpty(scale)) && (String.IsNullOrEmpty(coordinates)) && (String.IsNullOrEmpty(projection)))
                return;

            Results.Write("<mods:subject");
            Add_ID(Results);
            if (!String.IsNullOrEmpty(language))
                Results.Write(" lang=\"" + language + "\"");
            if (!String.IsNullOrEmpty(authority))
                Results.Write(" authority=\"" + authority + "\"");

            Results.Write(">\r\n");
            if (!String.IsNullOrEmpty(coordinates))
            {
                Results.Write("<mods:cartographics>\r\n");
                Results.Write("<mods:coordinates>" + coordinates + "</mods:coordinates>\r\n");
                Results.Write("</mods:cartographics>\r\n");
            }

            if ((!String.IsNullOrEmpty(scale)) || (!String.IsNullOrEmpty(projection)))
            {
                Results.Write("<mods:cartographics>\r\n");
                if (!String.IsNullOrEmpty(scale))
                    Results.Write("<mods:scale>" + Convert_String_To_XML_Safe(scale) + "</mods:scale>\r\n");

                if (!String.IsNullOrEmpty(projection))
                    Results.Write("<mods:projection>" + Convert_String_To_XML_Safe(projection) + "</mods:projection>\r\n");
                Results.Write("</mods:cartographics>\r\n");
            }
            Results.Write("</mods:subject>\r\n");
        }

        internal override MARC_Field to_MARC_HTML()
        {
            MARC_Field returnValue = new MARC_Field { Tag = 255 };

            // Set the tag
            if ((id.IndexOf("SUBJ") == 0) && (id.Length >= 7))
            {
                string possible_tag = id.Substring(4, 3);
                try
                {
                    int possible_tag_number = Convert.ToInt16(possible_tag);
                    returnValue.Tag = possible_tag_number;
                }
                catch
                {
                }
            }

            StringBuilder builder = new StringBuilder(50);
            if (!String.IsNullOrEmpty(scale))
                builder.Append("|a " + scale + " ");
            if (!String.IsNullOrEmpty(projection))
                builder.Append("|b " + projection + " ");
            if (!String.IsNullOrEmpty(coordinates))
                builder.Append("|c " + coordinates + " ");
            returnValue.Control_Field_Value = builder.ToString().Trim();

            return returnValue;
        }
    }
}