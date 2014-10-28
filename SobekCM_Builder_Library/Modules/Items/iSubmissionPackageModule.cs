using System.Collections.Generic;
using SobekCM.Core.Settings;

namespace SobekCM.Builder_Library.Modules.Items
{
    
    public interface iSubmissionPackageModule
    {
        List<string> Arguments { get; set; }

        InstanceWide_Settings Settings { get; set; }

        void DoWork(Incoming_Digital_Resource Resource);

        void ReleaseResources();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
