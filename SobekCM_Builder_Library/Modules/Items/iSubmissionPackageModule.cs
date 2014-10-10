namespace SobekCM.Builder_Library.Modules.Items
{
    

    

    public interface iSubmissionPackageModule
    {

        void DoWork(Incoming_Digital_Resource Resource);

        void ReleaseResources();

        event ModuleErrorLoggingDelegate Error;

        event ModuleStandardLoggingDelegate Process;
    }
}
