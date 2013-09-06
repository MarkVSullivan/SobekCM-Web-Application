using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SobekCM.Resource_Object;

namespace SobekCM.Library.Builder
{
    public interface iBuilder_PreBuild_Process
    {
        void PreValidateProcess(SobekCM_Item Built_Object, string Directory);
    }
}
