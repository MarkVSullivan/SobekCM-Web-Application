using System;
using System.Collections.Generic;
using System.Text;
using SobekCM.Bib_Package.MARC;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Subject information which includes cartographic data indicating spatial coverage </summary>
    [Serializable]
    public class Subject_Info_Cartographics : Subject_Info
    {
        private string scale;
        private string coordinates;
        private string projection;

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
            get
            {
                return Subject_Info_Type.Cartographics;
            }
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

        internal override string To_GSA_DublinCore(string indent)
        {
            return String.Empty;
        }

        internal override string To_GSA_SobekCM(string indent)
        {
            return String.Empty;
        }

        internal override void Add_MODS( System.IO.TextWriter results)
        {
            if ((String.IsNullOrEmpty(scale)) && (String.IsNullOrEmpty(coordinates)) && (String.IsNullOrEmpty(projection)))
                return;

            results.Write( "<mods:subject");
            base.Add_ID(results);
            if (!String.IsNullOrEmpty(language))
                results.Write(" lang=\"" + language + "\"");
            if (!String.IsNullOrEmpty(authority))
                results.Write(" authority=\"" + authority + "\"");
            
            results.Write(">\r\n");
            if (!String.IsNullOrEmpty(coordinates))
            {
                results.Write( "<mods:cartographics>\r\n");
                results.Write( "<mods:coordinates>" + coordinates + "</mods:coordinates>\r\n");
                results.Write( "</mods:cartographics>\r\n");
            }

            if ((!String.IsNullOrEmpty(scale)) || (!String.IsNullOrEmpty(projection)))
            {
                results.Write( "<mods:cartographics>\r\n");
                if (!String.IsNullOrEmpty(scale))
                    results.Write( "<mods:scale>" + base.Convert_String_To_XML_Safe(scale) + "</mods:scale>\r\n");

                if (!String.IsNullOrEmpty(projection))
                    results.Write( "<mods:projection>" + base.Convert_String_To_XML_Safe(projection) + "</mods:projection>\r\n");
                results.Write( "</mods:cartographics>\r\n");            
            }
            results.Write( "</mods:subject>\r\n");
        }

        internal override MARC_Field to_MARC_HTML()
        {
            MARC_Field returnValue = new MARC_Field();

            // Set the tag
            returnValue.Tag = 255;
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
