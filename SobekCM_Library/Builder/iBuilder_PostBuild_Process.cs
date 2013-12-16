using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SobekCM.Resource_Object;

namespace SobekCM.Library.Builder
{
    public interface iBuilder_PostBuild_Process
    {
        /// <summary> Perform any custom actions, after the item is completely built </summary>
        /// <param name="Built_Object"></param>
        /// <param name="Directory"></param>
        void PostProcess(SobekCM_Item Built_Object, string Directory);
    }
}
