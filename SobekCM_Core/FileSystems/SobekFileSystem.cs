﻿using System;
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


    }
}