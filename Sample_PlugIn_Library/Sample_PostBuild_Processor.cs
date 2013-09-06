using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Sample_PlugIn_Library
{
    class Sample_PostBuild_Processor : SobekCM.Library.Builder.iBuilder_PostBuild_Process
    {
        public void PostProcess(SobekCM.Resource_Object.SobekCM_Item Built_Object, string Directory)
        {
            throw new NotImplementedException();
        }
    }
}
