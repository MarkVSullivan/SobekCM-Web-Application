using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;

namespace SobekCM.Builder.Modules.Folders
{
    public interface iFolderModule
    {
        void DoWork(Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes);

        void ReleaseResources();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
