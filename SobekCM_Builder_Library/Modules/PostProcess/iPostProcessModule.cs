#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Builder_Library.Modules.PostProcess
{
    public interface iPostProcessModule
    {

        void DoWork( List<string> AggregationsAffected, List<BibVidStruct> ProcessedItems, List<BibVidStruct> DeletedItems );

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
