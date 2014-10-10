namespace SobekCM.Builder_Library.Modules.Schedulable
{
    public interface iSchedulableModule
    {
        void DoWork();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
