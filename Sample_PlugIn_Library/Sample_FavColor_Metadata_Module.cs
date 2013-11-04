using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SobekCM.Resource_Object.Metadata_Modules;

namespace Sample_PlugIn_Library
{
    public class Sample_FavColor_Metadata_Module : iMetadata_Module
    {
        private List<string> otherFavoriteColors;

        public Sample_FavColor_Metadata_Module()
        {
            otherFavoriteColors = new List<string>();
            bool no_error = true;
        }

        public string Absolute_Favorite_Color { get; set; }

        public List<string> Other_Favorite_Color
        {
            get
            {
                return otherFavoriteColors;
            }
        }

        /// <summary> Name for this metadata module </summary>
        public static string Module_Name_Static
        {
            get { return "MyFavColor"; }
        }

        /// <summary> Name for this metadata module </summary>
        public string Module_Name
        {
            get { return "MyFavColor"; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get 
            {
                // Create return value
                List<KeyValuePair<string, string>> returnVal = new List<KeyValuePair<string, string>>();

                // Add fav color
                if (!String.IsNullOrEmpty(Absolute_Favorite_Color))
                {
                    returnVal.Add(new KeyValuePair<string, string>("AbsoluteColorFav", Absolute_Favorite_Color));
                }

                // Add seconday favs
                foreach( string thisColor in otherFavoriteColors )
                {
                    returnVal.Add( new KeyValuePair<string,string>("OtherColorFav", thisColor));
                }

                return returnVal;
            }
        }


        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM.Resource_Object.SobekCM_Item BibObject, out string Error_Message)
        {
            Error_Message = String.Empty;

            return true;
        }

        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM.Resource_Object.SobekCM_Item BibObject, out string Error_Message)
        {
            Error_Message = String.Empty;

            return true;
        }

        public bool hasData
        {
            get
            {
                return !String.IsNullOrEmpty(Absolute_Favorite_Color) || otherFavoriteColors.Count > 0;
            }
        }

    }
}
