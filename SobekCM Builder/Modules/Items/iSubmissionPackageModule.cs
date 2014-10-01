using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;

namespace SobekCM.Builder.Modules.Items
{
    

    

    public interface iSubmissionPackageModule
    {

        void DoWork(Incoming_Digital_Resource Resource);

        void ReleaseResources();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
