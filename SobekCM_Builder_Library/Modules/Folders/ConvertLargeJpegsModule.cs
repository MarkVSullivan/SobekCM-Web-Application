using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder_Library.Modules.Folders
{
    /// <summary> Folder-level builder module checks to see if there are extra large JPEG images in the folder that should be pre-converted
    /// to JPEG2000s and smaller JPEGs, usually by converting to TIFF first. </summary>
    /// <remarks> This class implements the <see cref="abstractFolderModule" /> abstract class and implements the <see cref="iFolderModule" /> interface. </remarks>
    public class ConvertLargeJpegsModule : abstractFolderModule
    {
        /// <summary> Check if there are very large JPEGs which should be converted to TIFF for JPEG2000 and smaller JPEG creation </summary>
        /// <param name="BuilderFolder"> Builder folder upon which to perform all work </param>
        /// <param name="IncomingPackages"> List of valid incoming packages, which may be modified by this process </param>
        /// <param name="Deletes"> List of valid deletes, which may be modifyed by this process </param>
        public override void DoWork(Actionable_Builder_Source_Folder BuilderFolder, List<Incoming_Digital_Resource> IncomingPackages, List<Incoming_Digital_Resource> Deletes)
        {
            throw new NotImplementedException();
        }
    }
}
