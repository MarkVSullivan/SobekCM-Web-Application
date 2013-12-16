using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Library.Builder
{
    public static class SobekCM_Builder_Configuration_Details
    {
        private static List<iBuilder_PostBuild_Process> postBuildProcesses;

        static SobekCM_Builder_Configuration_Details()
        {
            postBuildProcesses = new List<iBuilder_PostBuild_Process>();
        }

        public static List<iBuilder_PostBuild_Process> PostBuild_Processes
        {
            get
            {
                return postBuildProcesses;
            }
        }
    }
}
