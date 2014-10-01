using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder.Modules.PostProcess
{
    public interface iPostProcessModule
    {

        void DoWork( List<string> AggregationsAffected, List<BibVidStruct> ProcessedItems, List<BibVidStruct> DeletedItems );

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
