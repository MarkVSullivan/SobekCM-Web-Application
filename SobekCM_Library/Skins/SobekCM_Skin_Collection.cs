#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Linq;

#endregion

namespace SobekCM.Library.Skins
{
	/// <summary> Collection of all the HTML skin data needed, including the fully built default skins and 
    /// the raw data to create the other HTML skins required to fulfil a user's request </summary>
	public class SobekCM_Skin_Collection
	{
		private readonly Dictionary<string, SobekCM_Skin_Object> defaultSkins;

	    /// <summary> Constructor for a new instance of the SobekCM_Skin_Collection class  </summary>
		public SobekCM_Skin_Collection()
		{
            defaultSkins = new Dictionary<string, SobekCM_Skin_Object>();
		}

        /// <summary> Constructor for a new instance of the SobekCM_Skin_Collection class  </summary>
        /// <param name="Interface_Table"> Datatable with the interface details for all valid HTML skins from the database </param>
        /// <remarks> This datatable is retrieved from the database by calling 
        /// the <see cref="Database.SobekCM_Database.Get_All_Web_Skins"/> method</remarks>
        public SobekCM_Skin_Collection(DataTable Interface_Table)
        {
            defaultSkins = new Dictionary<string, SobekCM_Skin_Object>();
            Skin_Table = Interface_Table;
        }

	    /// <summary> Datatable which has the information about every valid HTML skin from the database </summary>
	    /// <remarks> This is passed in during construction of this object.  This datatable was retrieved by calling 
	    /// the <see cref="Database.SobekCM_Database.Get_All_Web_Skins"/> method during 
	    /// application startup when the <see cref="SobekCM_Skin_Collection_Builder.Populate_Default_Skins"/> method is called. </remarks>
	    internal DataTable Skin_Table { get; set; }

	    /// <summary> Get the number of default skins already constructed in this collection </summary>
        public int Count
        {
            get { return defaultSkins.Count; }
        }

	    /// <summary> Address a single skin from this Collection, by skin code </summary>
        /// <param name="Skin_Language_Code"> Code to retrieve this skin ( [SKIN CODE] + '_' + [LANGUAGE CODE] , i.e., 'ufdc', 'dloc_fr', etc.. )</param>
        /// <returns> Existing HTML skin, or NULL </returns>
		public SobekCM_Skin_Object this[string Skin_Language_Code ]
		{
			get {
			    return defaultSkins.ContainsKey(Skin_Language_Code.ToLower()) ? defaultSkins[Skin_Language_Code.ToLower()] : null;
			}
		}

	    /// <summary> Clears all the default interfaces </summary>
	    public void Clear()
	    {
	        defaultSkins.Clear();
	    }

        /// <summary> Returns the ordered list of all skin codes </summary>
        public ReadOnlyCollection<string> Ordered_Skin_Codes
        {
            get
            {
                List<string> allSkinCodes = new List<string>();
                foreach (DataRow thisRow in Skin_Table.Rows)
                {
                    allSkinCodes.Add(thisRow[0].ToString());
                }
                return new ReadOnlyCollection<string>(allSkinCodes);
            }
        }

	    /// <summary> Datarow matching the provided skin code </summary>
	    /// <param name="Skin_Code"> Code for the HTML skin information to retrieve </param>
	    /// <returns> Row from a database query with basic information about the skin to build ( codes, override flags, banner link ), or NULL </returns>
	    /// <remarks> The datarow for this method is from the datatable passed in during construction of this object.  This datatable was retrieved by calling 
	    /// the <see cref="Database.SobekCM_Database.Get_All_Web_Skins"/> method during 
	    /// application startup when the <see cref="SobekCM_Skin_Collection_Builder.Populate_Default_Skins"/> method is called. </remarks>
	    public DataRow Skin_Row(string Skin_Code )
	    {
	        DataRow[] selectedRows = Skin_Table.Select("WebSkinCode = '" + Skin_Code + "'");
	        return selectedRows.Length > 0 ? selectedRows[0] : null;
	    }

	    /// <summary> Add a new HTML skin to this collection, to be retained as long as the application is active </summary>
		/// <param name="NewSkin"> New HTML skin to retain in this collection </param>
        public void Add(SobekCM_Skin_Object NewSkin)
		{
			// Add to the hashtable
            if (NewSkin.Language_Code.Length == 0)
            {
                defaultSkins[NewSkin.Skin_Code.ToLower()] = NewSkin;
            }
            else
            {
                defaultSkins[NewSkin.Skin_Code.ToLower() + "_" + NewSkin.Language_Code.ToLower()] = NewSkin;
            }
		}

        /// <summary> Add a new HTML skin to this collection, to be retained as long as the application is active </summary>
        /// <param name="Skin_Code"> Code for this new HTML skin</param>
        /// <param name="Base_Skin_Code"> Code for the base HTML skin which this new skin derives from</param>
        /// <param name="CSS_Style"> Additional CSS Stylesheet to be included for this new HTML skin</param>
        /// <returns> Newly constructed <see cref="SobekCM_Skin_Object"/> object </returns>
        public SobekCM_Skin_Object Add(string Skin_Code, string Base_Skin_Code, string CSS_Style)
		{
            // Create the new skin object
            SobekCM_Skin_Object newSkin = new SobekCM_Skin_Object(Skin_Code, Base_Skin_Code, CSS_Style);

			// Add to the hashtable
            defaultSkins[Skin_Code.ToLower()] = newSkin;

            // Return the new, built skin object
            return newSkin;
		}

        /// <summary> Add a new HTML skin to this collection, to be retained as long as the application is active </summary>
        /// <param name="Skin_Code"> Code for this new HTML skin</param>
        /// <param name="Base_Skin_Code"> Code for the base HTML skin which this new skin derives from</param>
        /// <param name="CSS_Style"> Additional CSS Stylesheet to be included for this new HTML skin</param>
        /// <param name="Banner_HTML"> Code for the banner to use, if this is set to override the banner</param>
        /// <returns> Newly constructed <see cref="SobekCM_Skin_Object"/> object </returns>
        public SobekCM_Skin_Object Add(string Skin_Code, string Base_Skin_Code, string CSS_Style, string Banner_HTML)
		{
            // Create the new skin object
            SobekCM_Skin_Object newSkin = new SobekCM_Skin_Object(Skin_Code, Base_Skin_Code, CSS_Style, Banner_HTML);

			// Add to the hashtable
            defaultSkins[Skin_Code.ToLower()] =  newSkin;

            // Return the new, built skin object
            return newSkin;
		}

        /// <summary> Removes a HTML skin from this collection </summary>
        /// <param name="Skin_Code"> Code the HTML skin to remove </param>
        /// <returns> TRUE if the skin was found and removed, otherwise FALSE </returns>
        public bool Remove(string Skin_Code)
        {
			if (!defaultSkins.ContainsKey(Skin_Code.ToLower()))
                return false;

			defaultSkins.Remove(Skin_Code.ToLower());
            return true;
        }
	}
}
