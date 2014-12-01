using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder_Library.Settings
{
    public static class Builder_Settings
    {
        static Builder_Settings()
        {
            Instance_Package_Limit = -1;
        }

        /// <summary> Maximum number of packages to process for each instance, before moving onto the 
        /// instance  </summary>
        /// <remarks> -1 is the default value and indicates no limit </remarks>
        public static int Instance_Package_Limit { get; set; }
    

    }
}
