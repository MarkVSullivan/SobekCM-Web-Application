#region Using directives

using System;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.MARC;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> A geographic name given in a hierarchical form relating to the resource. </summary>
    [Serializable]
    public class Subject_Info_HierarchicalGeographic : Subject_Info
    {
        private string area;
        private string city, citysection;
        private string continent, country;
        private string county;
        private string island;
        private string province, region, state, territory;

        /// <summary> Constructor for a new instance of the hierarchical geographic subject  </summary>
        public Subject_Info_HierarchicalGeographic()
        {
            // Do nothing
        }

        /// <summary> Gets flag indicating if there is any data in this subject object </summary>
        public bool hasData
        {
            get
            {
                if ((!String.IsNullOrEmpty(continent)) || (!String.IsNullOrEmpty(country)) || (!String.IsNullOrEmpty(province)) || (!String.IsNullOrEmpty(region)) || (!String.IsNullOrEmpty(state)) ||
                    (!String.IsNullOrEmpty(territory)) || (!String.IsNullOrEmpty(county)) || (!String.IsNullOrEmpty(city)) || (!String.IsNullOrEmpty(island)) || (!String.IsNullOrEmpty(area)) || (!String.IsNullOrEmpty(citysection)))
                    return true;


                return false;
            }
        }

        /// <summary> Gets or sets the continent level of this hierarchical geographic subject </summary>
        public string Continent
        {
            get { return continent ?? String.Empty; }
            set { continent = value; }
        }

        /// <summary> Gets or sets the country level of this hierarchical geographic subject </summary>
        public string Country
        {
            get { return country ?? String.Empty; }
            set { country = value; }
        }

        /// <summary> Gets or sets the province level of this hierarchical geographic subject </summary>
        public string Province
        {
            get { return province ?? String.Empty; }
            set { province = value; }
        }

        /// <summary> Gets or sets the region level of this hierarchical geographic subject </summary>
        public string Region
        {
            get { return region ?? String.Empty; }
            set { region = value; }
        }

        /// <summary> Gets or sets the state level of this hierarchical geographic subject </summary>
        public string State
        {
            get { return state ?? String.Empty; }
            set { state = value; }
        }

        /// <summary> Gets or sets the territory level of this hierarchical geographic subject </summary>
        public string Territory
        {
            get { return territory ?? String.Empty; }
            set { territory = value; }
        }

        /// <summary> Gets or sets the county level of this hierarchical geographic subject </summary>
        public string County
        {
            get { return county ?? String.Empty; }
            set { county = value; }
        }

        /// <summary> Gets or sets the city level of this hierarchical geographic subject </summary>
        public string City
        {
            get { return city ?? String.Empty; }
            set { city = value; }
        }

        /// <summary> Gets or sets the city section level of this hierarchical geographic subject </summary>
        public string CitySection
        {
            get { return citysection ?? String.Empty; }
            set { citysection = value; }
        }

        /// <summary> Gets or sets the island level of this hierarchical geographic subject </summary>
        public string Island
        {
            get { return island ?? String.Empty; }
            set { island = value; }
        }

        /// <summary> Gets or sets the area level of this hierarchical geographic subject </summary>
        public string Area
        {
            get { return area ?? String.Empty; }
            set { area = value; }
        }

        internal string Spatial_XML
        {
            get
            {
                StringBuilder returnValue = new StringBuilder();

                if (!String.IsNullOrEmpty(continent))
                {
                    returnValue.Append(continent);
                }

                if (!String.IsNullOrEmpty(country))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + country);
                    else
                        returnValue.Append(country);
                }

                if (!String.IsNullOrEmpty(province))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + province);
                    else
                        returnValue.Append(province);
                }

                if (!String.IsNullOrEmpty(region))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + region);
                    else
                        returnValue.Append(region);
                }

                if (!String.IsNullOrEmpty(state))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + state);
                    else
                        returnValue.Append(state);
                }

                if (!String.IsNullOrEmpty(territory))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + territory);
                    else
                        returnValue.Append(territory);
                }

                if (!String.IsNullOrEmpty(county))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + county);
                    else
                        returnValue.Append(county);
                }

                if (!String.IsNullOrEmpty(city))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + city);
                    else
                        returnValue.Append(city);
                }

                if (!String.IsNullOrEmpty(citysection))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + citysection);
                    else
                        returnValue.Append(citysection);
                }

                if (!String.IsNullOrEmpty(island))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + island);
                    else
                        returnValue.Append(island);
                }

                if (!String.IsNullOrEmpty(area))
                {
                    if (returnValue.Length > 0)
                        returnValue.Append(" -- " + area);
                    else
                        returnValue.Append(area);
                }

                string toString = returnValue.ToString();
                if (toString.Length > 0)
                {
                    return base.Convert_String_To_XML_Safe(toString);
                }
                else
                {
                    return String.Empty;
                }
            }
        }

        /// <summary> Indicates this is the Hierarchical Spatial subclass of Subject_Info </summary>
        public override Subject_Info_Type Class_Type
        {
            get { return Subject_Info_Type.Hierarchical_Spatial; }
        }

        /// <summary> Writes this hierarchical subject as a simple string </summary>
        /// <returns> This hierarchical subject returned as a simple string </returns>
        public override string ToString()
        {
            return Spatial_XML;
        }

        internal override void Add_MODS(TextWriter results)
        {
            if (!hasData)
                return;

            results.Write("<mods:subject");
            base.Add_ID(results);
            if (!String.IsNullOrEmpty(language))
                results.Write(" lang=\"" + language + "\"");
            if (!String.IsNullOrEmpty(authority))
                results.Write(" authority=\"" + authority + "\"");
            results.Write(">\r\n<mods:hierarchicalGeographic>\r\n");

            if (!String.IsNullOrEmpty(continent))
            {
                results.Write("<mods:continent>" + base.Convert_String_To_XML_Safe(continent) + "</mods:continent>\r\n");
            }

            if (!String.IsNullOrEmpty(country))
            {
                results.Write("<mods:country>" + base.Convert_String_To_XML_Safe(country) + "</mods:country>\r\n");
            }

            if (!String.IsNullOrEmpty(province))
            {
                results.Write("<mods:province>" + base.Convert_String_To_XML_Safe(province) + "</mods:province>\r\n");
            }

            if (!String.IsNullOrEmpty(state))
            {
                results.Write("<mods:state>" + base.Convert_String_To_XML_Safe(state) + "</mods:state>\r\n");
            }

            if (!String.IsNullOrEmpty(territory))
            {
                results.Write("<mods:territory>" + base.Convert_String_To_XML_Safe(territory) + "</mods:territory>\r\n");
            }

            if (!String.IsNullOrEmpty(county))
            {
                results.Write("<mods:county>" + base.Convert_String_To_XML_Safe(county) + "</mods:county>\r\n");
            }

            if (!String.IsNullOrEmpty(city))
            {
                results.Write("<mods:city>" + base.Convert_String_To_XML_Safe(city) + "</mods:city>\r\n");
            }

            if (!String.IsNullOrEmpty(citysection))
            {
                results.Write("<mods:citySection>" + base.Convert_String_To_XML_Safe(citysection) + "</mods:citySection>\r\n");
            }

            if (!String.IsNullOrEmpty(region))
            {
                results.Write("<mods:region>" + base.Convert_String_To_XML_Safe(region) + "</mods:region>\r\n");
            }

            if (!String.IsNullOrEmpty(island))
            {
                results.Write("<mods:island>" + base.Convert_String_To_XML_Safe(island) + "</mods:island>\r\n");
            }

            if (!String.IsNullOrEmpty(area))
            {
                results.Write("<mods:area>" + base.Convert_String_To_XML_Safe(area) + "</mods:area>\r\n");
            }

            results.Write("</mods:hierarchicalGeographic>\r\n</mods:subject>\r\n");
        }


        internal override MARC_Field to_MARC_HTML()
        {
            MARC_Field returnValue = new MARC_Field();

            if ((String.IsNullOrEmpty(country)) && (String.IsNullOrEmpty(city)) && (String.IsNullOrEmpty(county)) && (String.IsNullOrEmpty(province)) && (String.IsNullOrEmpty(territory)) && (String.IsNullOrEmpty(citysection)))
            {
                if (!String.IsNullOrEmpty(area))
                {
                    returnValue.Tag = 752;
                    returnValue.Control_Field_Value = "|g " + area + ".";
                }
                if (!String.IsNullOrEmpty(region))
                {
                    returnValue.Tag = 752;
                    returnValue.Control_Field_Value = "|g " + region + ".";
                }
                return null;
            }

            // No indicators
            returnValue.Indicators = "  ";

            //if ((id.IndexOf("662") < 0) && (id.IndexOf("752") < 0) && (area.Length > 0)  && (country.Length == 0) && (city.Length == 0) && (county.Length == 0) && (province.Length == 0) && (territory.Length == 0) && ( citysection.Length == 0 ))
            //{
            //    returnValue.Tag = "752";
            //    returnValue.Field = "|g " + area;
            //}
            //else
            //{
            // Set the tag
            returnValue.Tag = 752;
            if (id.IndexOf("SUBJ662") == 0)
            {
                returnValue.Tag = 662;
            }

            StringBuilder fieldBuilder = new StringBuilder();
            if (!String.IsNullOrEmpty(country))
            {
                fieldBuilder.Append("|a " + country + " ");
            }

            if (!String.IsNullOrEmpty(state))
            {
                fieldBuilder.Append("|b " + state + " ");
            }
            else
            {
                if (!String.IsNullOrEmpty(territory))
                {
                    fieldBuilder.Append("|b " + territory + " ");
                }
                else if (!String.IsNullOrEmpty(province))
                {
                    fieldBuilder.Append("|b " + province + " ");
                }
            }
            if (!String.IsNullOrEmpty(county))
            {
                fieldBuilder.Append("|c " + county + " ");
            }
            if (!String.IsNullOrEmpty(city))
            {
                fieldBuilder.Append("|d " + city + " ");
            }
            if (!String.IsNullOrEmpty(citysection))
            {
                fieldBuilder.Append("|f " + citysection + " ");
            }
            if (!String.IsNullOrEmpty(area))
            {
                fieldBuilder.Append("|g " + area + " ");
            }
            if (!String.IsNullOrEmpty(island))
            {
                fieldBuilder.Append("|g " + island + " ");
            }

            if (fieldBuilder.Length > 2)
            {
                fieldBuilder.Remove(fieldBuilder.Length - 1, 1);
                fieldBuilder.Append(". ");
            }

            if (!String.IsNullOrEmpty(authority))
                fieldBuilder.Append("|2 " + authority + " ");

            returnValue.Control_Field_Value = fieldBuilder.ToString().Trim();
            //}


            return returnValue;
        }

        /// <summary> Writes this hierarchical subject as a simple string </summary>
        /// <param name="Include_Scheme">Flag indicates if the scheme should be included</param>
        /// <returns> This hierarchical subject returned as a simple string </returns>
        public override string ToString(bool Include_Scheme)
        {
            return Spatial_XML;
        }
    }
}