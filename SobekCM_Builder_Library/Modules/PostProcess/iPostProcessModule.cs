#region Using directives

using System.Collections.Generic;
using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.PostProcess
{
    /// <summary> Interface class that all post-process builder modules MUST implement </summary>
    public interface iPostProcessModule
    {
        /// <summary> Arguments passed in to this module, used to determine process details </summary>
        List<string> Arguments { get; set; }

        /// <summary> Method performs the work of the post-process builder module </summary>
        /// <param name="AggregationsAffected"> List of aggregations affected during the last process of incoming digital resources </param>
        /// <param name="ProcessedItems"> List of all items just processed (or reprocessed) </param>
        /// <param name="DeletedItems"> List of all delete requests just processed </param>
        /// <param name="Settings"> Instance-wide settings which may be required for this process </param>
        void DoWork( List<string> AggregationsAffected, List<BibVidStruct> ProcessedItems, List<BibVidStruct> DeletedItems, InstanceWide_Settings Settings );

        /// <summary> Event is fired when an error occurs during processing </summary>
        event ModuleErrorLoggingDelegate Error;

        /// <summary> Event is fired to report progress during processing </summary>
        event ModuleStandardLoggingDelegate Process;
    }
}
