#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Resource_Object.OAI.Reader
{
    /// <summary> Single record from Dublin Core metadata from an OAI-PMH repository </summary>
    public class OAI_Repository_DublinCore_Record
    {
        private List<string> contributors;
        private List<string> coverages;
        private List<string> creators;
        private List<string> dates;
        private List<string> descriptions;
        private List<string> formats;
        private List<string> identifiers;
        private List<string> languages;
        private List<string> publishers;
        private List<string> relations;
        private List<string> rights;
        private List<string> sources;
        private List<string> subjects;
        private List<string> titles;
        private List<string> types;

        /// <summary> Constructor for a new instance of the  <see cref="OAI_Repository_DublinCore_Record"/> class </summary>
        public OAI_Repository_DublinCore_Record()
        {
            // Do nothing
        }

        /// <summary> OAI identifier for this record, should be unique, at least within the repository </summary>
        public string OAI_Identifier { get; set; }

        /// <summary> Datestamp when the record was last modified </summary>
        public string Datestamp { get; set; }

        /// <summary> Link for this record </summary>
        public string Link { get; set; }

        /// <summary> Flag indicates if this record has one or more TITLE elements </summary>
        public bool hasTitles
        {
            get {
                return (titles != null) && (titles.Count > 0);
            }
        }

        /// <summary> Collection of TITLE elements associated with this record </summary>
        public ReadOnlyCollection<string> Titles
        {
            get {
                return titles == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(titles);
            }
        }

        /// <summary> Flag indicates if this record has one or more CREATOR elements </summary>
        public bool hasCreators
        {
            get {
                return (creators != null) && (creators.Count > 0);
            }
        }

        /// <summary> Collection of CREATOR elements associated with this record </summary>
        public ReadOnlyCollection<string> Creators
        {
            get {
                return creators == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(creators);
            }
        }

        /// <summary> Flag indicates if this record has one or more SUBJECT elements </summary>
        public bool hasSubjects
        {
            get {
                return (subjects != null) && (subjects.Count > 0);
            }
        }

        /// <summary> Collection of SUBJECT elements associated with this record </summary>
        public ReadOnlyCollection<string> Subjects
        {
            get {
                return subjects == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(subjects);
            }
        }

        /// <summary> Flag indicates if this record has one or more DESCRIPTION elements </summary>
        public bool hasDescriptions
        {
            get {
                return (descriptions != null) && (descriptions.Count > 0);
            }
        }

        /// <summary> Collection of DESCRIPTION elements associated with this record </summary>
        public ReadOnlyCollection<string> Descriptions
        {
            get {
                return descriptions == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(descriptions);
            }
        }

        /// <summary> Flag indicates if this record has one or more PUBLISHER elements </summary>
        public bool hasPublishers
        {
            get {
                return (publishers != null) && (publishers.Count > 0);
            }
        }

        /// <summary> Collection of PUBLISHER elements associated with this record </summary>
        public ReadOnlyCollection<string> Publishers
        {
            get {
                return publishers == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(publishers);
            }
        }

        /// <summary> Flag indicates if this record has one or more CONTRIBUTOR elements </summary>
        public bool hasContributors
        {
            get {
                return (contributors != null) && (contributors.Count > 0);
            }
        }

        /// <summary> Collection of CONTRIBUTOR elements associated with this record </summary>
        public ReadOnlyCollection<string> Contributors
        {
            get {
                return contributors == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(contributors);
            }
        }

        /// <summary> Flag indicates if this record has one or more DATE elements </summary>
        public bool hasDates
        {
            get {
                return (dates != null) && (dates.Count > 0);
            }
        }

        /// <summary> Collection of DATES elements associated with this record </summary>
        public ReadOnlyCollection<string> Dates
        {
            get {
                return dates == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(dates);
            }
        }

        /// <summary> Flag indicates if this record has one or more TYPE elements </summary>
        public bool hasTypes
        {
            get {
                return (types != null) && (types.Count > 0);
            }
        }

        /// <summary> Collection of TYPE elements associated with this record </summary>
        public ReadOnlyCollection<string> Types
        {
            get {
                return types == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(types);
            }
        }

        /// <summary> Flag indicates if this record has one or more FORMAT elements </summary>
        public bool hasFormats
        {
            get {
                return (formats != null) && (formats.Count > 0);
            }
        }

        /// <summary> Collection of FORMAT elements associated with this record </summary>
        public ReadOnlyCollection<string> Formats
        {
            get {
                return formats == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(formats);
            }
        }

        /// <summary> Flag indicates if this record has one or more IDENTIFIER elements </summary>
        public bool hasIdentifiers
        {
            get {
                return (identifiers != null) && (identifiers.Count > 0);
            }
        }

        /// <summary> Collection of IDENTIFIER elements associated with this record </summary>
        public ReadOnlyCollection<string> Identifiers
        {
            get {
                return identifiers == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(identifiers);
            }
        }

        /// <summary> Flag indicates if this record has one or more SOURCE elements </summary>
        public bool hasSources
        {
            get {
                return (sources != null) && (sources.Count > 0);
            }
        }

        /// <summary> Collection of SOURCE elements associated with this record </summary>
        public ReadOnlyCollection<string> Sources
        {
            get {
                return sources == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(sources);
            }
        }

        /// <summary> Flag indicates if this record has one or more LANGUAGE elements </summary>
        public bool hasLanguages
        {
            get {
                return (languages != null) && (languages.Count > 0);
            }
        }

        /// <summary> Collection of LANGUAGE elements associated with this record </summary>
        public ReadOnlyCollection<string> Languages
        {
            get {
                return languages == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(languages);
            }
        }

        /// <summary> Flag indicates if this record has one or more RELATION elements </summary>
        public bool hasRelations
        {
            get {
                return (relations != null) && (relations.Count > 0);
            }
        }

        /// <summary> Collection of RELATION elements associated with this record </summary>
        public ReadOnlyCollection<string> Relations
        {
            get {
                return relations == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(relations);
            }
        }

        /// <summary> Flag indicates if this record has one or more COVERAGE elements </summary>
        public bool hasCoverages
        {
            get {
                return (coverages != null) && (coverages.Count > 0);
            }
        }

        /// <summary> Collection of COVERAGE elements associated with this record </summary>
        public ReadOnlyCollection<string> Coverages
        {
            get {
                return coverages == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(coverages);
            }
        }

        /// <summary> Flag indicates if this record has one or more RIGHTS elements </summary>
        public bool hasRights
        {
            get {
                return (rights != null) && (rights.Count > 0);
            }
        }

        /// <summary> Collection of RIGHTS elements associated with this record </summary>
        public ReadOnlyCollection<string> Rights
        {
            get {
                return rights == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(rights);
            }
        }

        /// <summary> Add a new title to this record </summary>
        /// <param name="New_Title"> New title to add </param>
        public void Add_Title(string New_Title)
        {
            if (titles == null)
                titles = new List<string>();
            if (!titles.Contains(New_Title))
                titles.Add(New_Title);
        }

        /// <summary> Add a new creator to this record </summary>
        /// <param name="New_Creator"> New creator to add </param>
        public void Add_Creator(string New_Creator)
        {
            if (creators == null)
                creators = new List<string>();
            if (!creators.Contains(New_Creator))
                creators.Add(New_Creator);
        }

        /// <summary> Add a new subject to this record </summary>
        /// <param name="New_Subject"> New subject to add </param>
        public void Add_Subject(string New_Subject)
        {
            if (subjects == null)
                subjects = new List<string>();
            if (!subjects.Contains(New_Subject))
                subjects.Add(New_Subject);
        }

        /// <summary> Add a new description to this record </summary>
        /// <param name="New_Description"> New description to add </param>
        public void Add_Description(string New_Description)
        {
            if (descriptions == null)
                descriptions = new List<string>();
            if (!descriptions.Contains(New_Description))
                descriptions.Add(New_Description);
        }

        /// <summary> Add a new publisher to this record </summary>
        /// <param name="New_Publisher"> New publisher to add </param>
        public void Add_Publisher(string New_Publisher)
        {
            if (publishers == null)
                publishers = new List<string>();
            if (!publishers.Contains(New_Publisher))
                publishers.Add(New_Publisher);
        }

        /// <summary> Add a new contributor to this record </summary>
        /// <param name="New_Contributor"> New contributor to add </param>
        public void Add_Contributor(string New_Contributor)
        {
            if (contributors == null)
                contributors = new List<string>();
            if (!contributors.Contains(New_Contributor))
                contributors.Add(New_Contributor);
        }

        /// <summary> Add a new date to this record </summary>
        /// <param name="New_Date"> New date to add </param>
        public void Add_Date(string New_Date)
        {
            if (dates == null)
                dates = new List<string>();
            if (!dates.Contains(New_Date))
                dates.Add(New_Date);
        }

        /// <summary> Add a new type to this record </summary>
        /// <param name="New_Type"> New type to add </param>
        public void Add_Type(string New_Type)
        {
            if (types == null)
                types = new List<string>();
            if (!types.Contains(New_Type))
                types.Add(New_Type);
        }

        /// <summary> Add a new format to this record </summary>
        /// <param name="New_Format"> New format to add </param>
        public void Add_Format(string New_Format)
        {
            if (formats == null)
                formats = new List<string>();
            if (!formats.Contains(New_Format))
                formats.Add(New_Format);
        }

        /// <summary> Add a new identifier to this record </summary>
        /// <param name="New_Identifier"> New identifier to add </param>
        public void Add_Identifier(string New_Identifier)
        {
            if (identifiers == null)
                identifiers = new List<string>();
            if (!identifiers.Contains(New_Identifier))
                identifiers.Add(New_Identifier);
        }

        /// <summary> Add a new source element to this record </summary>
        /// <param name="New_Source"> New source element to add </param>
        public void Add_Source(string New_Source)
        {
            if (sources == null)
                sources = new List<string>();
            if (!sources.Contains(New_Source))
                sources.Add(New_Source);
        }

        /// <summary> Add a new language to this record </summary>
        /// <param name="New_Language"> New language to add </param>
        public void Add_Language(string New_Language)
        {
            if (languages == null)
                languages = new List<string>();
            if (!languages.Contains(New_Language))
                languages.Add(New_Language);
        }

        /// <summary> Add a new related element to this record </summary>
        /// <param name="New_Relation"> New related element to add </param>
        public void Add_Relation(string New_Relation)
        {
            if (relations == null)
                relations = new List<string>();
            if (!relations.Contains(New_Relation))
                relations.Add(New_Relation);
        }

        /// <summary> Add a new coverage element to this record </summary>
        /// <param name="New_Coverage"> New coverage element to add </param>
        public void Add_Coverage(string New_Coverage)
        {
            if (coverages == null)
                coverages = new List<string>();
            if (!coverages.Contains(New_Coverage))
                coverages.Add(New_Coverage);
        }

        /// <summary> Add a new rights statement to this record </summary>
        /// <param name="New_Rights"> New rights statement to add </param>
        public void Add_Rights(string New_Rights)
        {
            if (rights == null)
                rights = new List<string>();
            if (!rights.Contains(New_Rights))
                rights.Add(New_Rights);
        }
    }
}