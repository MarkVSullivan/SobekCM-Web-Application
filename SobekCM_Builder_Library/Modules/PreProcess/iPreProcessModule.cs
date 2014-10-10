namespace SobekCM.Builder_Library.Modules.PreProcess
{
    public interface iPreProcessModule
    {
        void DoWork();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
