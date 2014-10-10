#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Builder_Library.Modules.Folders
{
    public interface iFolderModule
    {
        void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes);

        void ReleaseResources();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
