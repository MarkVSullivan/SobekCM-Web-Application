#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.LearningObjects
{
    /// <summary> Particular term within a taxonomy.  A node that has a defined label or term ( IEEE-LOM 9.2.2 ) </summary>
    [Serializable]
    public class LOM_Taxon
    {
        private readonly List<LOM_LanguageString> entries;

        /// <summary> Identifier of the taxon, such as a number or letter combination provided by the source of the taxonomy ( IEEE-LOM 9.2.2.1 ) </summary>
        public string ID { get; set;  }

        /// <summary> Constructor for a new instance of the LOM_Taxon class </summary>
        public LOM_Taxon()
        {
            entries = new List<LOM_LanguageString>();
            ID = String.Empty;
        }

        /// <summary> Constructor for a new instance of the LOM_Taxon class </summary>
        /// <param name="ID"> Identifier of the taxon, such as a number or letter combination provided by the source of the taxonomy ( IEEE-LOM 9.2.2.1 ) </param>
        public LOM_Taxon( string ID )
        {
            entries = new List<LOM_LanguageString>();
            this.ID = ID;
        }

        /// <summary> Textual entry/label of the taxon in different languages ( IEEE-LOM 9.2.2.2 ) </summary>
        public ReadOnlyCollection<LOM_LanguageString> Entries
        {
            get
            {
                return new ReadOnlyCollection<LOM_LanguageString>(entries);
            }
        }

        /// <summary> Add a new taxonomic entry to this taxonomy ( IEEE-LOM 9.2.2.2 ) </summary>
        /// <param name="Entry"> New taxonomic term to add </param>
        public void Add_Entry(LOM_LanguageString Entry)
        {
            entries.Add(Entry);
        }

        /// <summary> Add a new taxonomic entry to this taxonomy ( IEEE-LOM 9.2.2.2 ) </summary>
        /// <param name="Entry"> New taxonomic term to add </param>
        public void Add_Entry(string Entry)
        {
            entries.Add(new LOM_LanguageString(Entry, String.Empty));
        }

        /// <summary> Add a new taxonomic entry to this taxonomy ( IEEE-LOM 9.2.2.2 ) </summary>
        /// <param name="Entry"> New taxonomic term to add </param>
        /// <param name="Language"> Language of this new taxonomic term </param>
        public void Add_Entry(string Entry, string Language )
        {
            entries.Add(new LOM_LanguageString(Entry, Language));
        }

        /// <summary> Clears the list of all the taxonomic entries ( IEEE-LOM 9.2.2.2 ) </summary>
        public void Clear_Entries()
        {
            entries.Clear();
        }
    }
}
