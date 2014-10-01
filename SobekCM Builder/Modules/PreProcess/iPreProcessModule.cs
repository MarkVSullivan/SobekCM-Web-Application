using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder.Modules.PreProcess
{
    public interface iPreProcessModule
    {
        void DoWork();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
