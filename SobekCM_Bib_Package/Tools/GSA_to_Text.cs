using System;
using System.IO;

namespace SobekCM.Bib_Package.Tools
{
	/// <summary>Tool can be used to read the Greenstone Archive files and create the text files from the imbedded page text</summary>
	/// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class GSA_to_Text
	{
		/// <summary> Creates seperate text files from a GSA file with TEXT embedded. </summary>
		/// <param name="GSA_File"> Name and location of GSA file </param>
		/// <returns> TRUE if successful, otherwise FALSE </returns>
		public static bool Create_Text_Files( string GSA_File )
		{
			// Make sure the file exists
			if ( !File.Exists( GSA_File ))
			{
				return false;
			}

			try
			{

				// Get the directory
				string directory = ( new FileInfo( GSA_File )).DirectoryName;

				// Read this GSA file into a string
				StreamReader reader = new StreamReader( GSA_File );
			
				// Create the writer to be used for each text file
				StreamWriter writer = null;
				string fileName;

				// Loop through each line
				bool endFound = false;
				string thisLine = reader.ReadLine();

				// Get down to the section of the main part first
				while( thisLine != null )
				{
					if ( thisLine.IndexOf("<Section>") >= 0 )
					{
						break;
					}

					thisLine = reader.ReadLine();
				}

				// Now, move down to the NEXT section
				thisLine = reader.ReadLine();
				while( thisLine != null )
				{
					if ( thisLine.IndexOf("<Section>") >= 0 )
					{
						break;
					}

					thisLine = reader.ReadLine();
				}

				// Step through the rest of the GSA
				while( thisLine != null )
				{
					// Is this the line with the filename?
					if ( thisLine.IndexOf("<Metadata name=\"Screen\">") >= 0 )
					{
						// Get the filename
						fileName = thisLine.Substring( thisLine.IndexOf("<Metadata name=\"Screen\">") + 24 );
						fileName = fileName.Substring( 0, fileName.IndexOf("</Metadata>"));

						// Close the writer
						if ( writer != null )
						{
							writer.Flush();
							writer.Close();
						}

						// Open the reader for this text file
						writer = new StreamWriter( directory + "\\" + fileName.Replace(".jpg",".txt"), false );
					}

					// Does this start the line with all the text?
					if ( thisLine.IndexOf("<Content>") >= 0 )
					{
						endFound = false;
						while (( !endFound ) && ( thisLine != null ))
						{
							// Get rid of the first part of the line
							if ( thisLine.IndexOf("&lt;pre&gt;") > 0 )
							{
								// Get the start of the text
								thisLine = thisLine.Substring( thisLine.IndexOf("&lt;pre&gt;") + 11 );
							}

							// Drop the end of this line, if the end is here
							if ( thisLine.IndexOf("&lt;/pre&gt;") > 0 )
							{
								// Get the start of the text
								endFound = true;
								thisLine = thisLine.Substring( 0, thisLine.IndexOf("&lt;/pre&gt;") );
							}

							// Write this line to the writer
							writer.Write( thisLine + " ");

							// Get the next line
							thisLine = reader.ReadLine();
						}
					}

					// Get the next line
					thisLine = reader.ReadLine();
				}

				// Close the writer, if there is one still open
				if ( writer != null )
				{
					writer.Flush();
					writer.Close();
				}	
	
				return true;
			}
			catch
			{
				return false;
			}
		}
	}
}
