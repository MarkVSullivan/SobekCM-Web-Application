using System.Collections.Generic;
using SobekCM.Core.Settings;

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    public interface iSchedulableModule
    {
        List<string> Arguments { get; set; }

        void DoWork( InstanceWide_Settings Settings );

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
