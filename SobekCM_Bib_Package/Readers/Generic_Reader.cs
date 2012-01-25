using System;
using System.IO;
using SobekCM.Bib_Package;

namespace SobekCM.Bib_Package.Readers
{
	/// <summary> Generic reader reads a metadata file by selecting the correct reader for the format. </summary>
	/// <remarks>Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class Generic_Reader
	{
		/// <summary> Constructor for a new instance of the Generic_Reader class </summary>
		public Generic_Reader()
		{
			// Empty constructor
		}

		/// <summary> Read the metadata file and enrich the existing bibliographic package </summary>
		/// <param name="Data_File">Generic (unspecified) metadata file</param>
		/// <param name="thisPackage">Bibliographic package to enrich</param>
		public void Read( string Data_File, SobekCM_Item thisPackage )
		{
			// Does this filename have '.info.xml' in it?
			if ( Data_File.ToUpper().IndexOf(".INFO.XML") > 0 )
			{
				INFO_Reader readInfo = new INFO_Reader();
				readInfo.Read_INFO( Data_File, thisPackage );
				return;
			}

			// Does this filename end in '.sgml' or '.sgm'?
			if ( Data_File.ToUpper().IndexOf(".SGM") > 0 )
			{
				SGML_Reader readInfo = new SGML_Reader();
				readInfo.Read_SGML( Data_File, thisPackage, false );
				return;
			}

			// Does this file have '.mets' in it?
			if (( Data_File.ToUpper().IndexOf(".METS") > 0 ) || ( Data_File.ToUpper().IndexOf(".PMETS") > 0 ))
			{
                METS_Reader readInfo = new METS_Reader();
				readInfo.Read_METS( Data_File, thisPackage );
				return;
			}

			// If it made it here, it may be METS or MXF
			if ( Data_File.ToUpper().IndexOf(".XML") > 0 )
			{
				// Read first couple lines
				StreamReader reader = new StreamReader( Data_File );
				string thisLine = reader.ReadLine();
				while ( thisLine != null )
				{
					// Is this MXF?
					if ( thisLine.ToUpper().Trim() == "<MXF>" )
					{
						// Close the current connection
						reader.Close();

						// Read in the MXF file
						MXF_Reader readInfo = new MXF_Reader();
						readInfo.Read_MXF( Data_File, thisPackage );
						return;
					}

					// Is this a METS declaration?
					if ( thisLine.ToUpper().IndexOf("<METS:") > 0 )
					{
						// Close the current connection
						reader.Close();

						// Read in the METS file
                        METS_Reader readInfo = new METS_Reader();
						readInfo.Read_METS( Data_File, thisPackage );
						return;
					}
	
					// Read the next line
					thisLine = reader.ReadLine();
				}
			}
		}

		/// <summary> Read an directory, so look for metadata files, read them, and return the new bibliographic package </summary>
		/// <param name="directory">Directory to look in</param>
		/// <returns>Newly built bibliographic package, or else NULL</returns>
		public SobekCM_Item Read_Directory( string directory )
		{
			// Make sure the directory exists
			if ( !Directory.Exists( directory ))
				return null;

			// Create the bibliographic object
			SobekCM_Item thisPackage = new SobekCM_Item();

			// Assign the VID and BIB if you can
			DirectoryInfo dir = new DirectoryInfo( directory );
			string dirString = dir.Name;

			// If this is length of 5, probably a VID folder
			if (( dirString.Length == 5 ) && ( Char.IsNumber( dirString[0] )) && 
				( Char.IsNumber( dirString[1] )) && ( Char.IsNumber( dirString[2] )) && 
				( Char.IsNumber( dirString[3] )) && ( Char.IsNumber( dirString[4] )))
			{
				// Save this VID
				thisPackage.VID = dirString;

				// Get the parent directory and find the bib id
				if (( dir.Parent != null ) && ( dir.Parent.Name.Length == 10 ))
				{
					thisPackage.BibID = dir.Parent.Name;
				}
			}
			else
			{
				// If this is length of 16, probably BIB_VID
				if (( dirString.Length == 16 ) && ( dirString.IndexOf("_") == 10 ))
				{
					string[] split = dirString.Split("_".ToCharArray() );
					thisPackage.BibID = split[0];
					thisPackage.VID = split[1];
				}
			}

			// Get the METS files first which could contain bibliographic data
			string[] files = Directory.GetFiles( directory, "*.mets" );
			
			// Step through each METS file
			foreach( string file in files )
			{
				Read( file, thisPackage );
			}
			
			// Get the XML files next which could contain bibliographic data
			files = Directory.GetFiles( directory, "*.xml" );
			
			// Step through each file
			foreach( string file in files )
			{
				Read( file, thisPackage );
			}

			// Get the SGML files next which could contain bibliographic data
			files = Directory.GetFiles( directory, "*.sgm" );
			
			// Step through each SGML file
			foreach( string file in files )
			{
				Read( file, thisPackage );
			}

			return thisPackage;
		}
	}
}
