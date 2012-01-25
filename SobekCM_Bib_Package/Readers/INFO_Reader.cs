using System;
using System.IO;
using System.Data;
using SobekCM.Bib_Package;

namespace SobekCM.Bib_Package.Readers
{
	/// <summary>Reader reads INFO metadata files </summary>
	/// <remarks>INFO files were used in the DLC for a time to hold the structural information and file information <br /> <br />
	/// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class INFO_Reader
	{
		/// <summary> Constructor for a new instance of the INFO_Reader class </summary>
		public INFO_Reader()
		{
			// Empty constructor
		}

		/// <summary> Read the metadata file and enrich the existing bibliographic package </summary>
		/// <param name="INFO_File">INFO metadata file</param>
		/// <param name="thisPackage">Bibliographic package to enrich</param>
		public void Read_INFO( string INFO_File, SobekCM_Item thisPackage )
		{
			// Get the directory
			string directory = (new FileInfo( INFO_File )).Directory.ToString();
			thisPackage.Source_Directory = directory;

			// Clear any division information which already exists in the package
			thisPackage.Divisions.Clear();

			// The INFO file is just a DataSet written out to XML use C#.
			// This makes this initial read VERY easy
			DataSet readInfo = new DataSet();
			try
			{
				readInfo.ReadXml( INFO_File );
			}
			catch
			{
				return;
			}

			// Do some error checking first
			if (( readInfo == null ) || ( readInfo.Tables.Count < 3 ))
				return;

			// If there is bibliographic data in the DataSet which is not present
			// in the bib package, assign it.
			if ( readInfo.Tables["Volume"].Rows.Count > 0 )
			{
				if (( thisPackage.BibID.Length == 0 ) && ( readInfo.Tables["Volume"].Rows[0]["Bib_ID"] != DBNull.Value ))
				{
					thisPackage.BibID = readInfo.Tables["Volume"].Rows[0]["Bib_ID"].ToString();
				}
				if ( thisPackage.VID.Length == 0 )
				{
					thisPackage.VID = "00001";
				}
				if (( thisPackage.Bib_Info.Main_Title.Title.Length == 0 ) && ( readInfo.Tables["Volume"].Rows[0]["Title"] != DBNull.Value ))
				{
					thisPackage.Bib_Info.Main_Title.Title = readInfo.Tables["Volume"].Rows[0]["Title"].ToString();
				}
			}

			// Get the divisions and pages tables
			DataTable divisions = readInfo.Tables["Division"];
			DataTable pages = readInfo.Tables["Page"];

            //// Step through and add each division, and add it
            //ushort order = 1;
            //foreach( DataRow thisRow in divisions.Rows )
            //{
            //    // Add this division
               
            //    thisPackage.Divisions.Add_Division( String.Empty, "D" + thisRow["Division_Number"], thisRow["Type"].ToString(), thisRow["Type"].ToString(), order++ );

                
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
            //    thisPackage.Divisions.Add_Division( "D" + thisRow["Division_Number"], "P" + sequence, "Page", label, Convert.ToUInt16( thisRow["Sequence"] ) );

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
            //            thisPackage.Divisions.Add_File( "F" + sequence, fileName, "G" + sequence );
            //            thisPackage.Divisions.Add_Page_File_Link( "P" + sequence, "F" + sequence );
            //        }
            //        if ( fileName.ToUpper().IndexOf(".TXT") > 0 )
            //        {
            //            thisPackage.Divisions.Add_File( "T" + sequence, fileName, "G" + sequence );
            //            thisPackage.Divisions.Add_Page_File_Link( "P" + sequence, "T" + sequence );
            //        }
            //        if ( fileName.ToUpper().IndexOf(".JPG") > 0 )
            //        {
            //            thisPackage.Divisions.Add_File( "J" + sequence, fileName, "G" + sequence );
            //            thisPackage.Divisions.Add_Page_File_Link( "P" + sequence, "J" + sequence );
            //        }
            //        if ( fileName.ToUpper().IndexOf(".JP2") > 0 )
            //        {
            //            thisPackage.Divisions.Add_File( "E" + sequence, fileName, "G" + sequence );
            //            thisPackage.Divisions.Add_Page_File_Link( "P" + sequence, "E" + sequence );
            //        }
            //    }
			//}
		}
	}
}
