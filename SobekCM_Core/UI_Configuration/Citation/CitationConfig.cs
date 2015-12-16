using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Core.UI_Configuration.Citation
{
    /// <summary> Configuration for the citation within SobekCM, including the elements, group of element,
    /// order, and other details for rendering the citation within SobekCM </summary>
    public class CitationConfig
    {
        /// <summary> Name of the default citation set </summary>
        public string DefaultCitationSet { get; set; }

        /// <summary> Collection of the citation sets for this citation configuration </summary>
        public List<CitationSet> CitationSets { get; set; }

        /// <summary> Constuctor for a new instance of the <see cref="CitationConfig"/> class. </summary>
        public CitationConfig()
        {
            CitationSets = new List<CitationSet>();
        }
    }
}
