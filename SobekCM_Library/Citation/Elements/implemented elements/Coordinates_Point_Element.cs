#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Library.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Element allows simple entry of the coordinates (latitude/longitude) for an item </summary>
    /// <remarks> This class extends the <see cref="textBox_TextBox_Element"/> class. </remarks>
    public class Coordinates_Point_Element : textBox_TextBox_Element
    {
        /// <summary> Constructor for a new instance of the Coordinates_Point_Element class </summary>
        public Coordinates_Point_Element()
            : base("Coordinates:", "coordinate_point")
        {
            first_label = "Latitude";
            second_label = "Longitude";
            Repeatable = true;
            Display_SubType = "point";
            Type = Element_Type.Coordinates;
        }


        /// <summary> Renders the HTML for this element </summary>
        /// <param name="Output"> Textwriter to write the HTML for this element </param>
        /// <param name="Bib"> Object to populate this element from </param>
        /// <param name="Skin_Code"> Code for the current skin </param>
        /// <param name="isMozilla"> Flag indicates if the current browse is Mozilla Firefox (different css choices for some elements)</param>
        /// <param name="popup_form_builder"> Builder for any related popup forms for this element </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        /// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
        public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool isMozilla, StringBuilder popup_form_builder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL )
        {
            // Check that an acronym exists
            if (Acronym.Length == 0)
            {
                const string defaultAcronym = "Enter latitude and longitude for a coordinate related to this item";
                switch (CurrentLanguage)
                {
                    case Web_Language_Enum.English:
                        Acronym = defaultAcronym;
                        break;

                    case Web_Language_Enum.Spanish:
                        Acronym = defaultAcronym;
                        break;

                    case Web_Language_Enum.French:
                        Acronym = defaultAcronym;
                        break;

                    default:
                        Acronym = defaultAcronym;
                        break;
                }
            }

            List<string> latitudes = new List<string>();
            List<string> longitudes = new List<string>();

            // GEt the geospatial metadata module
            GeoSpatial_Information geoInfo = Bib.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            if (geoInfo != null)
            {
                if (geoInfo.hasData)
                {
                    for (int i = 0; i < geoInfo.Point_Count; i++)
                    {
                        latitudes.Add(geoInfo.Points[i].Latitude.ToString());
                        longitudes.Add(geoInfo.Points[i].Longitude.ToString());
                    }
                }
            }

            render_helper(Output, latitudes, longitudes, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);

        }

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <remarks> This clears any preexisting coordinate points </remarks>
        public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
        {
            // GEt the geospatial metadata module
            GeoSpatial_Information geoInfo = Bib.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            if (geoInfo != null)
            {
                geoInfo.Clear_Points();
            }
        }

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
        public override void Save_To_Bib(SobekCM_Item Bib)
        {
            List<Coordinate_Point> points = new List<Coordinate_Point>();

            string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
            string latitude = String.Empty;
            foreach (string thisKey in getKeys)
            {
                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_first") == 0)
                {
                    latitude = HttpContext.Current.Request.Form[thisKey];
                }

                if (thisKey.IndexOf(html_element_name.Replace("_", "") + "_second") != 0) continue;

                string longitude = HttpContext.Current.Request.Form[thisKey];
                if ((latitude.Length > 0) && ( longitude.Length > 0 ))
                {
                    double latitude_double, longitude_double;
                    if ((Double.TryParse(latitude, out latitude_double)) && (Double.TryParse(longitude, out longitude_double)))
                        points.Add(new Coordinate_Point(latitude_double, longitude_double));
                    latitude = String.Empty;
                }
            }

            // GEt the geospatial metadata module
            if (points.Count > 0)
            {
                GeoSpatial_Information geoInfo = Bib.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
                if (geoInfo == null)
                {
                    geoInfo = new GeoSpatial_Information();
                    Bib.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo);
                }

                foreach (Coordinate_Point thisPoint in points)
                {
                    geoInfo.Add_Point( thisPoint );
                }
            }
        }
    }
}



