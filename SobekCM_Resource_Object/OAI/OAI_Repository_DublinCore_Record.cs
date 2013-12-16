#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Resource_Object.OAI
{
    /// <summary> Single record from Dublin Core metadata from an OAI-PMH repository </summary>
    public class OAI_Repository_DublinCore_Record
    {
        private List<string> contributors;
        private List<string> coverages;
        private List<string> creators;
        private List<string> dates;
        private string datestamp;
        private List<string> descriptions;
        private List<string> formats;
        private List<string> identifiers;
        private List<string> languages;
        private string link;
        private string oai_identifier;
        private List<string> publishers;
        private List<string> relations;
        private List<string> rights;
        private List<string> sources;
        private List<string> subjects;
        private List<string> titles;
        private List<string> types;

        public OAI_Repository_DublinCore_Record()
        {
        }

        public string OAI_Identifier
        {
            get { return oai_identifier; }
            set { oai_identifier = value; }
        }

        public string Datestamp
        {
            get { return datestamp; }
            set { datestamp = value; }
        }

        public string Link
        {
            get { return link; }
            set { link = value; }
        }

        public bool hasTitles
        {
            get
            {
                if ((titles != null) && (titles.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Titles
        {
            get
            {
                if (titles == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(titles);
            }
        }

        public bool hasCreators
        {
            get
            {
                if ((creators != null) && (creators.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Creators
        {
            get
            {
                if (creators == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(creators);
            }
        }

        public bool hasSubjects
        {
            get
            {
                if ((subjects != null) && (subjects.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Subjects
        {
            get
            {
                if (subjects == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(subjects);
            }
        }

        public bool hasDescriptions
        {
            get
            {
                if ((descriptions != null) && (descriptions.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Descriptions
        {
            get
            {
                if (descriptions == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(descriptions);
            }
        }

        public bool hasPublishers
        {
            get
            {
                if ((publishers != null) && (publishers.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Publishers
        {
            get
            {
                if (publishers == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(publishers);
            }
        }

        public bool hasContributors
        {
            get
            {
                if ((contributors != null) && (contributors.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Contributors
        {
            get
            {
                if (contributors == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(contributors);
            }
        }

        public bool hasDates
        {
            get
            {
                if ((dates != null) && (dates.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Dates
        {
            get
            {
                if (dates == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(dates);
            }
        }

        public bool hasTypes
        {
            get
            {
                if ((types != null) && (types.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Types
        {
            get
            {
                if (types == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(types);
            }
        }

        public bool hasFormats
        {
            get
            {
                if ((formats != null) && (formats.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Formats
        {
            get
            {
                if (formats == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(formats);
            }
        }

        public bool hasIdentifiers
        {
            get
            {
                if ((identifiers != null) && (identifiers.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Identifiers
        {
            get
            {
                if (identifiers == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(identifiers);
            }
        }

        public bool hasSources
        {
            get
            {
                if ((sources != null) && (sources.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Sources
        {
            get
            {
                if (sources == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(sources);
            }
        }

        public bool hasLanguages
        {
            get
            {
                if ((languages != null) && (languages.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Languages
        {
            get
            {
                if (languages == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(languages);
            }
        }

        public bool hasRelations
        {
            get
            {
                if ((relations != null) && (relations.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Relations
        {
            get
            {
                if (relations == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(relations);
            }
        }

        public bool hasCoverages
        {
            get
            {
                if ((coverages != null) && (coverages.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Coverages
        {
            get
            {
                if (coverages == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(coverages);
            }
        }

        public bool hasRights
        {
            get
            {
                if ((rights != null) && (rights.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        public ReadOnlyCollection<string> Rights
        {
            get
            {
                if (rights == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(rights);
            }
        }

        public void Add_Title(string New_Title)
        {
            if (titles == null)
                titles = new List<string>();
            if (!titles.Contains(New_Title))
                titles.Add(New_Title);
        }

        public void Add_Creator(string New_Creator)
        {
            if (creators == null)
                creators = new List<string>();
            if (!creators.Contains(New_Creator))
                creators.Add(New_Creator);
        }

        public void Add_Subject(string New_Subject)
        {
            if (subjects == null)
                subjects = new List<string>();
            if (!subjects.Contains(New_Subject))
                subjects.Add(New_Subject);
        }

        public void Add_Description(string New_Description)
        {
            if (descriptions == null)
                descriptions = new List<string>();
            if (!descriptions.Contains(New_Description))
                descriptions.Add(New_Description);
        }

        public void Add_Publisher(string New_Publisher)
        {
            if (publishers == null)
                publishers = new List<string>();
            if (!publishers.Contains(New_Publisher))
                publishers.Add(New_Publisher);
        }

        public void Add_Contributor(string New_Contributor)
        {
            if (contributors == null)
                contributors = new List<string>();
            if (!contributors.Contains(New_Contributor))
                contributors.Add(New_Contributor);
        }

        public void Add_Date(string New_Date)
        {
            if (dates == null)
                dates = new List<string>();
            if (!dates.Contains(New_Date))
                dates.Add(New_Date);
        }

        public void Add_Type(string New_Type)
        {
            if (types == null)
                types = new List<string>();
            if (!types.Contains(New_Type))
                types.Add(New_Type);
        }

        public void Add_Format(string New_Format)
        {
            if (formats == null)
                formats = new List<string>();
            if (!formats.Contains(New_Format))
                formats.Add(New_Format);
        }

        public void Add_Identifier(string New_Identifier)
        {
            if (identifiers == null)
                identifiers = new List<string>();
            if (!identifiers.Contains(New_Identifier))
                identifiers.Add(New_Identifier);
        }

        public void Add_Source(string New_Source)
        {
            if (sources == null)
                sources = new List<string>();
            if (!sources.Contains(New_Source))
                sources.Add(New_Source);
        }

        public void Add_Language(string New_Language)
        {
            if (languages == null)
                languages = new List<string>();
            if (!languages.Contains(New_Language))
                languages.Add(New_Language);
        }

        public void Add_Relation(string New_Relation)
        {
            if (relations == null)
                relations = new List<string>();
            if (!relations.Contains(New_Relation))
                relations.Add(New_Relation);
        }

        public void Add_Coverage(string New_Coverage)
        {
            if (coverages == null)
                coverages = new List<string>();
            if (!coverages.Contains(New_Coverage))
                coverages.Add(New_Coverage);
        }

        public void Add_Rights(string New_Rights)
        {
            if (rights == null)
                rights = new List<string>();
            if (!rights.Contains(New_Rights))
                rights.Add(New_Rights);
        }
    }
}