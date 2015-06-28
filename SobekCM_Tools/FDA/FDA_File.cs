#region Using directives

using System;
using System.Collections.Generic;
using System.Xml;

#endregion

namespace SobekCM.Tools.FDA
{
    /// <summary> Class stores all the information about a file which was submitted
    /// to the FDA. </summary>
    /// <remarks> This class is used within a complete <see cref="FDA_Report_Data" /> object and is not generally used alone.</remarks>
    /// <example> For examples, see the example under the <see cref="FDA_Report_Data" /> class.</example>
    public class FDA_File
    {
        private readonly List<FDA_File_Warning> warnings;

        /// <summary> Constructor creates a new instance of the FDA_File class </summary>
        public FDA_File()
        {
            // Initialize all values
            Name = String.Empty;
            MD5_Checksum = String.Empty;
            SHA1_Checksum = String.Empty;
            Preservation = String.Empty;
            ID = String.Empty;
            Event = String.Empty;
            Size = -1;
            warnings = new List<FDA_File_Warning>();
        }

        /// <summary> Constructor creates a new instance of the FDA_File class </summary>
        /// <param name="ID">ID for this file in the FDA</param>
        /// <param name="Name">Name (or path) of the file</param>
        /// <param name="Size">Size of the file</param>
        /// <param name="MD5_Checksum">MD5 checksum for the file</param>
        /// <param name="SHA1_Checksum">SHA-1 checksum for the file</param>
        /// <param name="Preservation">Preservation level applied to this file</param>
        public FDA_File(string ID, string Name, long Size, string MD5_Checksum, string SHA1_Checksum, string Preservation)
        {
            // Set all values
            this.Name = Name;
            this.MD5_Checksum = MD5_Checksum;
            this.SHA1_Checksum = SHA1_Checksum;
            this.Preservation = Preservation;
            this.ID = ID;
            this.Size = Size;
            Event = String.Empty;
            warnings = new List<FDA_File_Warning>();
        }

        /// <summary> Gets or sets the ID for this file in the FDA </summary>
        public string ID { get; set; }

        /// <summary> Gets or sets the name (or path) for this file in the FDA </summary>
        public string Name { get; set; }

        /// <summary> Gets or sets the MD5 checksum result for this file in the FDA </summary>
        public string MD5_Checksum { get; set; }

        /// <summary> Gets or sets the SHA-1 checksum result for this file in the FDA </summary>
        public string SHA1_Checksum { get; set; }

        /// <summary> Gets or sets the preservation level for this file in the FDA </summary>
        public string Preservation { get; set; }

        /// <summary> Gets or sets the text of any event linked to this file </summary>
        public string Event { get; set; }

        /// <summary> Gets or sets the size of this file in the FDA </summary>
        public long Size { get; set; }

        /// <summary> Gets or sets the source XML node for this file from the FDA report </summary>
        /// <remarks>This is used when creating a new, more compact version of the FDA Ingest Report</remarks>
        public XmlNode XML_Node { get; set; }

        /// <summary> Gets the collection of warnings linked to this file </summary>
        public List<FDA_File_Warning> Warnings
        {
            get { return warnings; }
        }

        /// <summary> Add a new warning to this file </summary>
        /// <param name="Code"> Warning code for this file-level warning </param>
        /// <param name="Text"> Warning text for this file-level warning </param>
        public void Add_Warning(string Code, string Text)
        {
            warnings.Add(new FDA_File_Warning(Code, Text));
        }
    }
}
