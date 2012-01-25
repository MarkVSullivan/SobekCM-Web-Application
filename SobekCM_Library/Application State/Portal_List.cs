#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

#endregion

namespace SobekCM.Library.Application_State
{
    /// <summary> Collection of all the URL Portals into this SobekCM library </summary>
    public class Portal_List
    {
        private Portal defaultPortal;
        private readonly List<Portal> portals;

        /// <summary> Constructor for a new instance of the Portal_List class </summary>
        public Portal_List()
        {
            portals = new List<Portal>();
        }

        /// <summary> Gets a readonly collection of all the portals in this system </summary>
        public ReadOnlyCollection<Portal> All_Portals
        {
            get
            {
                return new ReadOnlyCollection<Portal>(portals);
            }
        }

        /// <summary> Gets and sets the default portal </summary>
        public Portal Default_Portal
        {
            get
            {
                if (defaultPortal != null)
                    return defaultPortal;

                return portals.Count > 0 ? portals[0] : null;
            }
            set { defaultPortal = value; }
        }

        /// <summary> Returns the number of URL Portals present in this collection </summary>
        public int Count
        {
            get { return portals.Count; }
        }

        /// <summary> Gets the URL portal for the current request, by the base url for the current request </summary>
        /// <param name="Base_URL"> Base URL for the current request against this library </param>
        /// <returns> The URL portal for the current request </returns>
        public Portal Get_Valid_Portal(string Base_URL)
        {
            if (Base_URL.Length == 0)
                return Default_Portal;

            foreach (Portal thisPortal in portals.Where(thisPortal => thisPortal.URL_Segment.Length > 0).Where(thisPortal => Base_URL.IndexOf(thisPortal.URL_Segment) >= 0))
            {
                return thisPortal;
            }

            return Default_Portal;
        }


        /// <summary> Create and add a new portal to this collection URL portals </summary>
        /// <param name="ID"> Primary key for this portal in the database </param>
        /// <param name="Name"> Name for the library when viewed through this portal</param>
        /// <param name="Abbreviation"> Abbreviation used for the library when viewed through this portal </param>
        /// <param name="Default_Aggregation"> Default aggregation, or 'all' if all aggregations are available </param>
        /// <param name="Default_Web_Skin"> Default web skin used when displayed through this portal </param>
        /// <param name="URL_Segment"> URL segment used to determine if a request comes from this portal </param>
        /// <param name="Base_PURL"> Base PURL to used when constructing a PURL for items within this portal, if it is different than the standard base URL </param>
        /// <returns> Built and added URL Portal for any additional work ( limiting by web skin or aggregations )</returns>
        public Portal Add_Portal( int ID, string Name, string Abbreviation, string Default_Aggregation, string Default_Web_Skin, string URL_Segment, string Base_PURL )
        {
            Portal returnValue = new Portal(ID, Name, Abbreviation, Default_Aggregation, Default_Web_Skin, URL_Segment, Base_PURL );
            portals.Add(returnValue);
            return returnValue;
        }

        /// <summary> Clears the list of all URL Portals into this library / cms </summary>
        public void Clear()
        {
            portals.Clear();
            defaultPortal = null;
        }
    }
}
