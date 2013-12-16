#region Using directives

using System.Collections.Generic;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    /// <summary> Interface which all metadata file reader/writers must implement. </summary>
    /// <remarks>These are reader/writers which write the entire resource as a full metadata file 
    /// (although not each piece of the resource may be saved in every format).  These are different (but
    /// may depend on) the METS dmdSec and amdSec reader/writers which write just a single section
    /// of extensible metadata within a larger METS file.  </remarks>
    public interface iMetadata_File_ReaderWriter
    {
        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        bool canRead { get; }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        bool canWrite { get; }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Dublin Core, Greenstone file, etc.. ) </summary>
        string Metadata_Type_Name { get; }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        string Metadata_Type_Abbreviation { get; }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message);

        /// <summary> Reads metadata from an open stream and saves to the provided item/package </summary>
        /// <param name="Input_Stream"> Open stream to read metadata from </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        bool Read_Metadata(Stream Input_Stream, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message);

        /// <summary> Writes the formatted metadata from the provided item to a file </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to write</param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        bool Write_Metadata(string MetadataFilePathName, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message);

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message);
    }
}