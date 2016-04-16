using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.BriefItem;

namespace SobekCM.Core.FileSystems
{
    /// <summary> Class provides uniform access to the sobek file system 
    /// which contains all the digital resources </summary>
    public static class SobekFileSystem
    {
        private static iFileSystem fileSystem;

        /// <summary> Initializes the specified file system and sets the uris for the
        /// necessary access for the file system </summary>
        /// <param name="RootNetworkUri">The root network URI.</param>
        /// <param name="RootWebUri">The root web URI.</param>
        public static void Initialize(string RootNetworkUri, string RootWebUri)
        {
            fileSystem = new PairTreeStructure(RootNetworkUri, RootWebUri);
        }

        /// <summary> Read to the end of a (text-based) file and return the contents </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <param name="FileName"> Name of the file to open, and read </param>
        /// <returns> Full contexts of the text-based file </returns>
        public static string ReadToEnd(BriefItemInfo DigitalResource, string FileName)
        {
            return fileSystem.ReadToEnd(DigitalResource, FileName);
        }

        /// <summary> Return the WEB uri for a digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <returns> URI for the web resource </returns>
        public static string Resource_Web_Uri(BriefItemInfo DigitalResource)
        {
            return fileSystem.Resource_Web_Uri(DigitalResource);
        }

        /// <summary> Return the WEB uri for a file within the digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <param name="FileName"> Name of the resource file </param>
        /// <returns> URI for the web resource </returns>
        public static string Resource_Web_Uri(BriefItemInfo DigitalResource, string FileName )
        {
            return fileSystem.Resource_Web_Uri(DigitalResource, FileName);
        }

        /// <summary> Return the NETWORK uri for a digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <returns> URI for the network resource </returns>
        /// <remarks> This makes some presumptions on the type of system in the background </remarks>
        public static string Resource_Network_Uri(BriefItemInfo DigitalResource)
        {
            return fileSystem.Resource_Network_Uri(DigitalResource);
        }

        /// <summary> Return the NETWORK uri for a digital resource </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for a title within a SobekCM instance </param>
        /// <param name="VID"> Volume identifier (VID) for an item within a SobekCM title </param>
        /// <returns> URI for the network resource </returns>
        /// <remarks> This makes some presumptions on the type of system in the background </remarks>
        public static string Resource_Network_Uri(string BibID, string VID)
        {
            return fileSystem.Resource_Network_Uri(BibID, VID);
        }

        /// <summary> Return the NETWORK uri for a single file in the digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <param name="FileName"> Filename to get network URI for</param>
        /// <returns> URI for the network resource </returns>
        /// <remarks> This makes some presumptions on the type of system in the background </remarks>
        public static string Resource_Network_Uri(BriefItemInfo DigitalResource, string FileName)
        {
            return fileSystem.Resource_Network_Uri(DigitalResource, FileName);
        }

        /// <summary> Return the NETWORK uri for a single file in the digital resource </summary>
        /// <param name="BibID"> Bibliographic identifier (BibID) for a title within a SobekCM instance </param>
        /// <param name="VID"> Volume identifier (VID) for an item within a SobekCM title </param>
        /// <param name="FileName"> Filename to get network URI for</param>
        /// <returns> URI for the network resource </returns>
        /// <remarks> This makes some presumptions on the type of system in the background </remarks>
        public static string Resource_Network_Uri(string BibID, string VID, string FileName)
        {
            return fileSystem.Resource_Network_Uri(BibID, VID, FileName);
        }

        /// <summary> Return a flag if the file specified exists within the digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <param name="FileName"> Filename to check for</param>
        /// <returns> URI for the web resource </returns>
        public static bool FileExists(BriefItemInfo DigitalResource, string FileName)
        {
            return fileSystem.FileExists(DigitalResource, FileName);
        }

        /// <summary> [TEMPORARY] Get the associated file path (which is essentially the part of the 
        /// path that appears UNDER the root imaging spot </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <returns> Part of the file path, derived from the BibID and VID </returns>
        public static string AssociFilePath(BriefItemInfo DigitalResource)
        {
            return fileSystem.AssociFilePath(DigitalResource);
        }

        /// <summary> Gets the list of all the files associated with this digital resource </summary>
        /// <param name="DigitalResource"> The digital resource object  </param>
        /// <returns> List of the file information for this digital resource, or NULL if this does not exist somehow </returns>
        public static List<SobekFileSystem_FileInfo> GetFiles(BriefItemInfo DigitalResource)
        {
            return fileSystem.GetFiles(DigitalResource);
        }

    }
}