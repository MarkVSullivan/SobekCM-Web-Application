#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.Maps
{
    /// <summary> Personal name associated with a map sheet within a digital resource  </summary>
    [Serializable]
    public class Map_Person
    {
        private string name;
        private long personid;

        /// <summary> Constructor for a new instance of the Map_Person class </summary>
        public Map_Person()
        {
            personid = -1;
            name = String.Empty;
        }

        /// <summary> Constructor for a new instance of the Map_Person class </summary>
        /// <param name="PersonID"> Primary key for this personal name </param>
        /// <param name="Name"> Primary personal name for the personal name object </param>
        public Map_Person(long PersonID, string Name)
        {
            personid = PersonID;
            name = Name;
        }

        /// <summary> Primary key for this personal name object associated with a map sheet item </summary>
        public long PersonID
        {
            get { return personid; }
            set { personid = value; }
        }

        /// <summary> Primary personal name for this personal name object associated with a map sheet item </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }
    }
}