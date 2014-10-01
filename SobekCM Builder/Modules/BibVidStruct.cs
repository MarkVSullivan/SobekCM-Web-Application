using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder.Modules
{
    public struct BibVidStruct
    {
        public readonly string BibID;

        public readonly string VID;

        public BibVidStruct(string BibID, string VID)
        {
            this.BibID = BibID;
            this.VID = VID;
        }
    }
}
