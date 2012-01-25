using System;
using System.IO;
using System.Collections;
using System.Collections.Specialized;


namespace SobekCM.Bib_Package.Tools
{
	/// <summary>Tool builds the Greenstone 'archives.inf' file used by the Greenstone collection building process</summary>
    /// <remarks>This writer is used by the SobekCM Builder before building a Greenstone collection.<br /> <br />
	/// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class Archive_Info_FileBuilder
	{
		/// <summary> Constructor for a new instance of the Archive_Info_FileBuilder class </summary>
		public Archive_Info_FileBuilder()
		{
			// Empty constructor
		}

		/// <summary> Step through the folder structure and create the 'archives.inf' file </summary>
		/// <param name="startDirectory"> Starting directory (archives folder in the Greenstone collection folder)</param>
		public void Create_Archives_File( string startDirectory )
		{
			// Step through the directories, and get a list of complete directory paths
			StringCollection dirs = new StringCollection();
			recursively_add_directories( startDirectory, dirs, String.Empty );

			// Step through and make the identifier list as well
			StringCollection ids = new StringCollection();
			foreach( string dir in dirs )
			{
				// Only continue if this is of the correct format
				ids.Add( dir.Substring(0,2) + dir[3] + dir[9] + dir[6] + dir[12] + dir[4] + dir[10] + dir[7] + dir[13] + dir.Substring(15,5));
			}

			// List the directories into a text file
			StreamWriter writer = new StreamWriter( startDirectory + "/archives.inf", false );
			for( int i = 0 ; i < dirs.Count; i++ )
			{
				writer.WriteLine( ids[i] + "\t" + dirs[i] + "doc.xml" );
			}
			writer.Flush();
			writer.Close();
		}

		private void recursively_add_directories( string directory, StringCollection dirs, string build_string )
		{
			// Get any subdirectories
			string[] subdirs = Directory.GetDirectories( directory );

			// Were there subdirectories?
			if ( subdirs.Length > 0 )
			{
				// Step through each one
				DirectoryInfo thisDirInfo;
				foreach( string thisDir in subdirs )
				{
					// Get the Directory information object
					thisDirInfo = new DirectoryInfo( thisDir );

					// Add this to the build string and call this subdirectory recursively
					recursively_add_directories( thisDir, dirs, build_string + thisDirInfo.Name + "/" );
				}
			}
			else
			{
				if ( build_string.Length == 21 )
				{
					dirs.Add( build_string );
				}
			}            
		}
	}
}
