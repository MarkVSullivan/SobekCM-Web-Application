#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.LearningObjects
{
    /// <summary>  A taxonomic path in a specific classification system.  Each succeeding 
    /// level is a refinement of the preceeding level. ( IEEE-LOM 9.2 ) </summary>
    [Serializable]
    public class LOM_TaxonPath
    {
        private readonly List<LOM_Taxon> taxons;
        private readonly List<LOM_LanguageString> sourceNames;

        /// <summary> Constructor for a new instance of the LOM_TaxonPath class </summary>
        public LOM_TaxonPath()
        {
            sourceNames = new List<LOM_LanguageString>();
            taxons = new List<LOM_Taxon>();
        }

        /// <summary> The name of the classification system, possibly in multiple languages ( IEEE-LOM 9.2.1 ) </summary>
        public ReadOnlyCollection<LOM_LanguageString> SourceNames
        {
            get
            {
                return new ReadOnlyCollection<LOM_LanguageString>(sourceNames);
            }
        }

        /// <summary> Add a new source name of the classification system ( IEEE-LOM 9.2.1 ) </summary>
        /// <param name="SourceName"> Name of the source classification system </param>
        public void Add_SourceName( string SourceName )
        {
            sourceNames.Add( new LOM_LanguageString(SourceName, String.Empty ));
        }

        /// <summary> Add a new source name of the classification system ( IEEE-LOM 9.2.1 ) </summary>
        /// <param name="SourceName"> Name of the source classification system </param>
        /// <param name="Language"> Language of this new source name to add </param>
        public void Add_SourceName(string SourceName, string Language )
        {
            sourceNames.Add(new LOM_LanguageString(SourceName, Language));
        }

        /// <summary> Add a new source name of the classification system ( IEEE-LOM 9.2.1 ) </summary>
        /// <param name="Value"> Name and language of the source classification system </param>
        public void Add_SourceName( LOM_LanguageString Value)
        {
            sourceNames.Add( Value );
        }

        /// <summary> Clears the list of all the names of the classification system, in multiple languages ( IEEE-LOM 9.2.1 ) </summary>
        public void Clear_SourceNames()
        {
            sourceNames.Clear();
        }

        /// <summary> Gets all taxonomic terms within this taxonomic path.  Each succeeding level is a refinement of the preceeding level ( IEEE-LOM 9.2.2 ) </summary>
        public ReadOnlyCollection<LOM_Taxon> Taxons
        {
            get
            {
                return new ReadOnlyCollection<LOM_Taxon>(taxons);
            }
        }

        /// <summary> Add a new taxonomic term to this taxonomic path ( IEEE-LOM 9.2.2 ) </summary>
        /// <param name="Value"> New taxonomic term to add to this classification </param>
        public void Add_Taxon(LOM_Taxon Value)
        {
            taxons.Add(Value);
        }

        /// <summary> Clears the list of all the taxonomic terms in this taxonomic path ( IEEE-LOM 9.2.2 ) </summary>
        public void Clear_Taxons()
        {
            taxons.Clear();
        }
    }
}
