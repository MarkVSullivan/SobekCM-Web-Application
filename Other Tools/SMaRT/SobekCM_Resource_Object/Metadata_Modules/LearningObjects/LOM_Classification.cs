using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SobekCM.Resource_Object.Metadata_Modules.LearningObjects
{
    /// <summary> This category describes where this learning object falls within a particular classification schema ( IEEE-LOM 9 ) </summary>
    /// <remarks> To define multiple classifications, there may be multiple instances of this category </remarks>
    [Serializable]
    public class LOM_Classification
    {
        private List<LOM_TaxonPath> taxonPaths;

        /// <summary> The purpose of classifying this learning object (i.e., discipline, idea, educational objective, skill level, etc..)  ( IEEE-LOM 9.1 ) </summary>
        public LOM_VocabularyState Purpose { get; set; }

        /// <summary> Constructor for a new instance of the LOM_Classification class </summary>
        public LOM_Classification()
        {
            taxonPaths = new List<LOM_TaxonPath>();
            Purpose = new LOM_VocabularyState();
        }

        /// <summary> Gets all taxonomic paths in this classification system.  Each succeeding level is a refinement of the preceeding level ( IEEE-LOM 9.2 ) </summary>
        public ReadOnlyCollection<LOM_TaxonPath> TaxonPaths
        {
            get
            {
                return new ReadOnlyCollection<LOM_TaxonPath>(taxonPaths);
            }
        }

        /// <summary> Add a new taxonomic path to this classification ( IEEE-LOM 9.2 ) </summary>
        /// <param name="Value"> New taxonomic path to add to this classifciation </param>
        public void Add_TaxonPath(LOM_TaxonPath Value)
        {
            taxonPaths.Add(Value);
        }

        /// <summary> Clears the list of all the taxonomic paths in this classification ( IEEE-LOM 9.2 ) </summary>
        public void Clear_TaxonPaths()
        {
            taxonPaths.Clear();
        }
    }
}
 