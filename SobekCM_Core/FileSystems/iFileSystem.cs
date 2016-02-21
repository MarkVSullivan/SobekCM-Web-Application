using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.BriefItem;

namespace SobekCM.Core.FileSystems
{
    public interface iFileSystem
    {
        


        /// <summary> Read to the end of a (text-based) file and return the contents </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <param name="FileName"> Name of the file to open, and read </param>
        /// <returns> Full contexts of the text-based file </returns>
        string ReadToEnd(BriefItemInfo DigitalResource, string FileName);


        /// <summary> Return the WEB uri for a digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <returns> URI for the web resource </returns>
        string Resource_Web_Uri(BriefItemInfo DigitalResource);

        /// <summary> Return a flag if the file specified exists within the digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <param name="FileName"> Filename to check for</param>
        /// <returns> URI for the web resource </returns>
        bool FileExists(BriefItemInfo DigitalResource, string FileName);

        /// <summary> Return the NETWORK uri for a digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <returns> URI for the network resource </returns>
        /// <remarks> This makes some presumptions on the type of system in the background </remarks>
        string Resource_Network_Uri(BriefItemInfo DigitalResource);

        /// <summary> [TEMPORARY] Get the associated file path (which is essentially the part of the 
        /// path that appears UNDER the root imaging spot </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <returns> Part of the file path, derived from the BibID and VID </returns>
        string AssociFilePath(BriefItemInfo DigitalResource);

    }
}
