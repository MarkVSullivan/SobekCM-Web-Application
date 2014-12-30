#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Resource_Object.OAI.Reader
{
    /// <summary> Basic information about an OAI-PMH repository which allows for 
    /// harvesting of OAI-PMH records either directly into a SobekCM library or
    /// into METS files for loading </summary>
    /// <remarks> Most of the information for this repository object comes from querying 
    /// the repository with Identity, ListSets, ListMetadataPrefixes. </remarks>
    public class OAI_Repository_Information
    {
        private List<string> metadataformats;
        private List<KeyValuePair<string, string>> sets;

        /// <summary> Constructor for a new instance of OAI_Repository_Information class </summary>
        /// <param name="URL"> URL for the OAI-PMH repository </param>
        public OAI_Repository_Information(string URL)
        {
            Harvested_URL = URL;
            Is_Valid = true;
            sets = new List<KeyValuePair<string, string>>();
            metadataformats = new List<string>();
        }

        /// <summary> Earliest data stamp for resources within this repository </summary>
        public string Earliest_Date_Stamp { get; set; }

        /// <summary> Information on how deleted records are handled in this repository </summary>
        public string Deleted_Record { get; set; }

        /// <summary> Information on granularity in this repository </summary>
        public string Granularity { get; set; }

        /// <summary> URL from which all the data was harvested for this repository  </summary>
        public string Harvested_URL { get; set; }

        /// <summary> Base URL as identified by the repository </summary>
        public string Base_URL { get; set; }

        /// <summary> Protocol version statement for this repository ( version 1 or 2 ) </summary>
        public string Protocol_Version { get; set; }

        /// <summary> Name of the repository, as identified by the respository </summary>
        public string Name { get; set; }

        /// <summary> Administrative email address for the repository  </summary>
        public string Admin_Email { get; set; }

        /// <summary> Identifier for the repository, as identified by the repository </summary>
        public string Repository_Identifier { get; set; }

        /// <summary> Identifier delimiter from the repositories identify response </summary>
        public string Delimiter { get; set; }

        /// <summary> Sample identifier from the repositories identify response </summary>
        public string Sample_Identifier { get; set; }

        /// <summary> Flag indicates if the repository appears valid, based on the identify response </summary>
        public bool Is_Valid { get; set; }

        /// <summary> Gets the list of all sets which are a part of this repository </summary>
        public List<KeyValuePair<string, string>> Sets
        {
            get { return sets; }
        }

        /// <summary> Gets the list of metadata formats which this repository supports </summary>
        public List<string> Metadata_Formats
        {
            get { return metadataformats; }
        }

        /// <summary> Adds a new metadata set to this repository information object </summary>
        /// <param name="New_Set"> Set code and name as KeyValuePair </param>
        public void Add_Set(KeyValuePair<string, string> New_Set)
        {
            sets.Add(New_Set);
        }

        /// <summary> Adds a metadata format to this repository information object </summary>
        /// <param name="Format">  </param>
        public void Add_Metadata_Format(string Format)
        {
            metadataformats.Add(Format);
        }
    }
}