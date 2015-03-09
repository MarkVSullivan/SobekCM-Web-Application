using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace SobekCM.Engine_Library.JSON_Client_Helpers
{
    /// <summary> Class is used by the ImageBrowser plug-in within the HTML editor to display server-side images to include </summary>
    [Serializable, DataContract]
    public class UploadedFileFolderInfo
    {
        /// <summary> URL for the image to display for inclusion </summary>
        public string image { get; private set;  }

        /// <summary> Folder name under which this image should be included in the user interface </summary>
        public string folder { get; private set; }

        /// <summary> Constructor for a new instance of the UploadedFileFolderInfo class for the ImageBrowser 
        /// plug-in within the HTML editor to display server-side images to include  </summary>
        /// <param name="Image"> URL for the image to display for inclusion </param>
        /// <param name="Folder"> Folder name under which this image should be included in the user interface </param>
        public UploadedFileFolderInfo(string Image, string Folder)
        {
            image = Image;
            folder = Folder;
        }
    }
}
