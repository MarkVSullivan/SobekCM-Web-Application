#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    /// <summary> INFO file reader/writer </summary>
    /// <remarks>INFO files were used in the DLC for a time to hold the structural information and file information</remarks>
    public class INFO_File_ReaderWriter : iMetadata_File_ReaderWriter
    {
        #region iMetadata_File_ReaderWriter Members

        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        /// <value> This property always returns TRUE </value>
        public bool canRead
        {
            get { return true; }
        }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        /// <value> This property always return FALSE </value>
        public bool canWrite
        {
            get { return false; }
        }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Dublin Core, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'Custom INFO Metadata'</value>
        public string Metadata_Type_Name
        {
            get { return "Custom INFO Metadata"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., DC, MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'INFO'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "INFO"; }
        }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error outpt message
            Error_Message = String.Empty;

            // Get the directory
            string directory = (new FileInfo(MetadataFilePathName)).Directory.ToString();
            Return_Package.Source_Directory = directory;

            // Clear any division information which already exists in the package
            Return_Package.Divisions.Clear();

            // The INFO file is just a DataSet written out to XML use C#.
            // This makes this initial read VERY easy
            DataSet readInfo = new DataSet();
            try
            {
                readInfo.ReadXml(MetadataFilePathName);
            }
            catch (Exception ee)
            {
                Error_Message = "Error reading legacy INFO file: " + ee.Message;
                return false;
            }

            // Do some error checking first
            if ((readInfo == null) || (readInfo.Tables.Count < 3))
            {
                Error_Message = "Error reading legacy INFO page: insufficient tables present";
                return false;
            }

            // If there is bibliographic data in the DataSet which is not present
            // in the bib package, assign it.
            if (readInfo.Tables["Volume"].Rows.Count > 0)
            {
                if ((Return_Package.BibID.Length == 0) && (readInfo.Tables["Volume"].Rows[0]["Bib_ID"] != DBNull.Value))
                {
                    Return_Package.BibID = readInfo.Tables["Volume"].Rows[0]["Bib_ID"].ToString();
                }
                if (Return_Package.VID.Length == 0)
                {
                    Return_Package.VID = "00001";
                }
                if ((Return_Package.Bib_Info.Main_Title.Title.Length == 0) && (readInfo.Tables["Volume"].Rows[0]["Title"] != DBNull.Value))
                {
                    Return_Package.Bib_Info.Main_Title.Title = readInfo.Tables["Volume"].Rows[0]["Title"].ToString();
                }
            }

            // Get the divisions and pages tables
            DataTable divisions = readInfo.Tables["Division"];
            DataTable pages = readInfo.Tables["Page"];

            return true;

            //// Step through and add each division, and add it
            //ushort order = 1;
            //foreach( DataRow thisRow in divisions.Rows )
            //{
            //    // Add this division

            //    Return_Package.Divisions.Add_Division( String.Empty, "D" + thisRow["Division_Number"], thisRow["Type"].ToString(), thisRow["Type"].ToString(), order++ );


            //}

            //// Now, add each page to the appropriate divisions
            //string label = String.Empty;
            //string fileName;
            //string[] files;
            //string sequence;
            //foreach( DataRow thisRow in pages.Rows )
            //{
            //    // Find the label for this
            //    if ( thisRow["Feature"] == DBNull.Value )
            //    {
            //        label = String.Empty;
            //    }
            //    else
            //    {
            //        // Get the feature name first then
            //        label = thisRow["Feature"].ToString();

            //        // Is there a number for this?
            //        if (( thisRow["Number"] != DBNull.Value ) && ( thisRow["Number"].ToString().ToUpper() != "UNNUMBERED" ))
            //        {
            //            label = label + " " + thisRow["Number"].ToString();
            //        }
            //    }

            //    // Add the page division
            //    sequence = thisRow["Sequence"].ToString();
            //    Return_Package.Divisions.Add_Division( "D" + thisRow["Division_Number"], "P" + sequence, "Page", label, Convert.ToUInt16( thisRow["Sequence"] ) );

            //    // Get all the matching files		
            //    fileName = thisRow["FileName"].ToString();
            //    files = Directory.GetFiles( directory, fileName + ".*" );

            //    // Add each file 
            //    foreach( string thisFile in files )
            //    {
            //        // Add this file itself
            //        fileName = (( new FileInfo( thisFile )).Name );
            //        if ( fileName.ToUpper().IndexOf(".TIF") > 0 )
            //        {
            //            Return_Package.Divisions.Add_File( "F" + sequence, fileName, "G" + sequence );
            //            Return_Package.Divisions.Add_Page_File_Link( "P" + sequence, "F" + sequence );
            //        }
            //        if ( fileName.ToUpper().IndexOf(".TXT") > 0 )
            //        {
            //            Return_Package.Divisions.Add_File( "T" + sequence, fileName, "G" + sequence );
            //            Return_Package.Divisions.Add_Page_File_Link( "P" + sequence, "T" + sequence );
            //        }
            //        if ( fileName.ToUpper().IndexOf(".JPG") > 0 )
            //        {
            //            Return_Package.Divisions.Add_File( "J" + sequence, fileName, "G" + sequence );
            //            Return_Package.Divisions.Add_Page_File_Link( "P" + sequence, "J" + sequence );
            //        }
            //        if ( fileName.ToUpper().IndexOf(".JP2") > 0 )
            //        {
            //            Return_Package.Divisions.Add_File( "E" + sequence, fileName, "G" + sequence );
            //            Return_Package.Divisions.Add_Page_File_Link( "P" + sequence, "E" + sequence );
            //        }
            //    }
            //}
        }

        /// <summary> Reads metadata from an open stream and saves to the provided item/package </summary>
        /// <param name="Input_Stream"> Open stream to read metadata from </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(Stream Input_Stream, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        /// <summary> Writes the formatted metadata from the provided item to a file </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to write</param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(string MetadataFilePathName, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}