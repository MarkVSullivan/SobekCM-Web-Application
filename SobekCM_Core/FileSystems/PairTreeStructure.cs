using System;
using System.IO;
using SobekCM.Core.BriefItem;

namespace SobekCM.Core.FileSystems
{
    /// <summary> Utility supports the pair tree file / directory structure which 
    /// the SObekCM instance uses by default to organize the digital resources </summary>
    public class PairTreeStructure : iFileSystem
    {
        private string rootNetworkUri;
        private string rootWebUri;
        private char pathSeperator;

        /// <summary> Constructor for a new instance of the <see cref="PairTreeStructure"/> class </summary>
        /// <param name="RootNetworkUri"> Root network location for the digital resource files </param>
        /// <param name="RootWebUri"> Root web URL for the digital resource files folder </param>
        public PairTreeStructure(string RootNetworkUri, string RootWebUri )
        {
            rootNetworkUri = RootNetworkUri;
            rootWebUri = RootWebUri;

            // Set the environmental default
            pathSeperator = Path.PathSeparator;
        }


        /// <summary> Read to the end of a (text-based) file and return the contents </summary>
        /// <param name="DigitalResource"> The digital resource object </param>
        /// <param name="FileName"> Name of the file to open, and read </param>
        /// <returns> Full contexts of the text-based file </returns>
        public string ReadToEnd(BriefItemInfo DigitalResource, string FileName)
        {
            string fullFilePath = Path.Combine(resource_network_uri(DigitalResource.BibID, DigitalResource.VID), FileName);
            return File.ReadAllText(fullFilePath);
        }


        private string resource_network_uri(string BibID, string VID)
        {
            return Path.Combine(rootNetworkUri, BibID.Substring(0, 2) + pathSeperator + BibID.Substring(2, 2) + pathSeperator + BibID.Substring(4, 2) + pathSeperator + BibID.Substring(6, 2) + pathSeperator + BibID.Substring(8, 2), VID);
        }
    }
}
