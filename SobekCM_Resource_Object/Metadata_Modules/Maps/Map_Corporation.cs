#region Using directives

using System;
using System.Collections.Specialized;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.Maps
{
    /// <summary> Contains all the information about a corporation which has some appearance on a map, such
    /// as the Sanborn maps </summary>
    [Serializable]
    public class Map_Corporation
    {
        private StringCollection altNames;
        private long corpid;
        private string primName;

        /// <summary> Constructor creates a new instance of the Map_Corporation class </summary>
        public Map_Corporation()
        {
            // Set some defaults
            corpid = -1;
            primName = String.Empty;
            altNames = new StringCollection();
        }

        /// <summary> Constructor creates a new instance of the Map_Corporation class </summary>
        /// <param name="CorpID"> Primary key for this corporation from the authority database </param>
        /// <param name="Primary_Name"> Primary name for this corporation</param>
        public Map_Corporation(long CorpID, string Primary_Name)
        {
            // Set some defaults
            corpid = CorpID;
            primName = Primary_Name;
            altNames = new StringCollection();
        }

        /// <summary> Primary key for this corporation from the authority database </summary>
        public long CorpID
        {
            get { return corpid; }
            set { corpid = value; }
        }

        /// <summary> Primary name for this corporation </summary>
        public string Primary_Name
        {
            get { return primName; }
            set { primName = value; }
        }

        /// <summary> Gets the number of alternate names associated with this corporation </summary>
        public int Alt_Name_Count
        {
            get { return altNames.Count; }
        }

        /// <summary> Clear all the alternate names associated with this corporation </summary>
        public void Clear_Alt_Names()
        {
            altNames.Clear();
        }

        /// <summary> Add an alternate name associated with this corporation </summary>
        /// <param name="New_Alt_Name"> New alternate name for this corporation </param>
        public void Add_Alt_Name(string New_Alt_Name)
        {
            string capped = New_Alt_Name.ToUpper().Trim();
            if ((primName.ToUpper() != capped) && (!altNames.Contains(New_Alt_Name.Trim())))
            {
                altNames.Add(New_Alt_Name);
            }
        }

        /// <summary> Gets an alternate name for this corporation by index </summary>
        /// <param name="index"> Index for the alternate name to retrieve </param>
        /// <returns> Alternate name requested by index </returns>
        public string Get_Alt_Name(int index)
        {
            return altNames[index];
        }
    }
}