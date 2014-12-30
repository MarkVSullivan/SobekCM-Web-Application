#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.OAI.Reader
{
    /// <summary> Collection of OAI-PMH records from the OAI-PMH repository </summary>
    public class OAI_Repository_Records_List
    {
        private List<OAI_Repository_DublinCore_Record> records;

        /// <summary> Constructor for a new instance of the OAI_Repository_Records_List class </summary>
        public OAI_Repository_Records_List()
        {
            records = new List<OAI_Repository_DublinCore_Record>();
        }

        /// <summary> Number of records contained within this object </summary>
        public int Count
        {
            get { return records.Count; }
        }

        /// <summary> Gets a particular record from within this collection of records </summary>
        /// <param name="i"> Index of the record within this collection </param>
        /// <returns> Record, or NULL if the record does not exist</returns>
        public OAI_Repository_DublinCore_Record this[int i]
        {
            get
            {
                if ((i < 0) || (i >= records.Count))
                    return null;
                return records[i];
            }
        }

        /// <summary> Gets or sets the resumption token, if one existed in the last pull of records </summary>
        public string Resumption_Token { get; set; }

        /// <summary> Adds a record to the collection of records </summary>
        /// <param name="New_Record"> New record to add </param>
        public void Add_Record(OAI_Repository_DublinCore_Record New_Record)
        {
            records.Add(New_Record);
        }
    }
}