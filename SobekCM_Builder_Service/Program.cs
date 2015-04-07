using System.ServiceProcess;

namespace SobekCM_Builder_Service
{
    static class Program
    {
        /// <summary> The main entry point for the application. </summary>
        static void Main()
        {
            ServiceBase[] ServicesToRun;
            ServicesToRun = new ServiceBase[] 
            { 
                new BuilderService() 
            };
            ServiceBase.Run(ServicesToRun);
        }
    }
}
