using System;
using System.Text;
using System.Collections;
using System.IO;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Divisions;

namespace SobekCM.Bib_Package.Readers
{
	/// <summary>Reader reads SGML metadata files </summary>
	/// <remarks>SGML were used for a while to submit packages with full text to FCLA for loading into DLXS. <br /> <br />
	/// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class SGML_Reader
	{
        // TODO: Either get rid of the MXF_Reader or get it working again.  Changes in 2010 to the
        // division structure caused it to no longer function.

		/// <summary> Constructor for a new instance of the SGML_Reader class </summary>
		public SGML_Reader()
		{
			// Empty constructor
		}

		/// <summary> Read the metadata file and enrich the existing bibliographic package </summary>
		/// <param name="SGML_File">SGML metadata file</param>
		/// <param name="thisPackage">Bibliographic package to enrich</param>
		/// <param name="createTextFiles">Flag indicates whether to create new text files by copying the text out from the SGML files</param>
		public void Read_SGML( string SGML_File, SobekCM_Item thisPackage, bool createTextFiles )
		{
			// Create ths stringbuilder to hold all the lines of text, and writer to output
			StringBuilder textFileBldr = new StringBuilder();
			//StreamWriter textFileOut;

			// Get the directory
			string directory = (new FileInfo( SGML_File )).Directory.ToString();
			thisPackage.Source_Directory = directory;

			// Load the SGML File
			StreamReader reader = new StreamReader( SGML_File );

			// Clear all the old division information
			thisPackage.Divisions.Clear();

            //// Define the necessary variables for the work below
            //int fileid = 1, pageid = 1, indexOf;
            //ushort order = 1;
            //ushort depth = 1;
            //ushort divid = 1;
            //string fileName = String.Empty, subfilename, next_sub, type, lastDivID = String.Empty;
            //string[] splitter;
            //Division_TreeNode lastDivision = null;
            //abstract_TreeNode lastPage = null;
            //abstract_TreeNode[] treeNodes = new abstract_TreeNode[10];
            //bool lastPageHandled = false;

			// Loop through each line
			string next = reader.ReadLine();
            //while( next != null )
            //{
            //    // Trim this
            //    next = next.Trim();

            //    // Save this if this is either <HEAD, <PB , or <DIV
            //    if ( next.Length > 4 )
            //    {
            //        next_sub = next.Substring( 0, 4 );
            //        if ((( next_sub == "<HEA" ) || ( next_sub == "<PB " ) || ( next_sub == "<DIV" ) || ( next_sub == "</DI")) && ( next != "<HEADER>"))
            //        {
            //            // Is this a page/file definition?
            //            if ( next_sub == "<PB " )
            //            {
            //                // Close out the last page, if there was one
            //                if (( lastPage != null ) )
            //                {
            //                    if ( lastDivision != null )
            //                    {
            //                        // See if the page is linked to the last division yet
            //                        if ( !lastDivision.Nodes.Contains( lastPage ))
            //                        {
            //                            // Add link to the divisoin and page
            //                            lastDivision.Nodes.Add( lastPage );

            //                            // Remove this page from the root nodes, if it exists
            //                            if( thisPackage.Divisions.Root_Nodes.Contains( lastPage  ))
            //                            {
            //                                thisPackage.Divisions.Root_Nodes.Remove( lastPage );
            //                            }
            //                        }
            //                    }

            //                    // Also, if we are creating text files, do so here
            //                    if ( createTextFiles ) 
            //                    {
            //                        // If there is no text, use the canned phrase
            //                        if ( textFileBldr.Length == 0 )
            //                        {
            //                            textFileBldr.Append("This page contains no text.");
            //                        }

            //                        // Create the writer for this
            //                        textFileOut = new StreamWriter( thisPackage.Source_Directory + "/" + fileName + ".txt", false );

            //                        // Step through each character looking for illegal characters
            //                        int charASCII;
            //                        char character;
            //                        for( int i = 0 ; i < textFileBldr.Length ; i++ )
            //                        {
            //                            // get the character, and then the ascii
            //                            character = textFileBldr[ i ];
            //                            charASCII = (int) character;

            //                            // If this is illegal, change it
            //                            if (( charASCII < 31 ) && ( charASCII != 0 ) && ( charASCII != 9 ) && ( charASCII != 10 ) && ( charASCII != 13 ))
            //                            {
            //                                textFileBldr[ i ] = ' ';
            //                            }
            //                        }

            //                        // Now write this out to the file
            //                        textFileOut.Write( textFileBldr.ToString() );
            //                        textFileOut.Flush();
            //                        textFileOut.Close();

            //                        // Clear the text file builder
            //                        textFileBldr = new StringBuilder();

            //                        // Add this new text file to the package
            //                        thisPackage.Divisions.Add_File( "T" + (fileid - 1), fileName + ".txt", "G" + (pageid - 1) );
            //                        thisPackage.Divisions.Add_Page_File_Link( "P" + (pageid - 1), "T" + (fileid - 1) );
            //                    }
            //                }

            //                // Get the file name
            //                indexOf = next.IndexOf("REF=\"");
            //                fileName = next.Substring( indexOf + 5, next.IndexOf("\"", indexOf + 6 ) - indexOf - 5 );
            //                splitter = fileName.Split( "\\/.".ToCharArray() );
            //                fileName = splitter[ splitter.Length - 2 ];

            //                // Get the files 
            //                string[] files = Directory.GetFiles( directory, fileName + ".*" );

            //                // Add the page
            //                lastPage = thisPackage.Divisions.Add_Division( String.Empty, "P" + pageid, "PAGE", String.Empty, order++ );
            //                lastPageHandled = false;

            //                // Add each file 
            //                foreach( string thisFile in files )
            //                {
            //                    // Add this file itself
            //                    subfilename = (( new FileInfo( thisFile )).Name );
            //                    if ( subfilename.ToUpper().IndexOf(".TIF") > 0 )
            //                    {
            //                        thisPackage.Divisions.Add_File( "F" + fileid, subfilename, "G" + pageid );
            //                        thisPackage.Divisions.Add_Page_File_Link( "P" + pageid, "F" + fileid );
            //                    }
            //                    if (( subfilename.ToUpper().IndexOf(".TXT") > 0 ) && ( !createTextFiles ))
            //                    {
            //                        thisPackage.Divisions.Add_File( "T" + fileid, subfilename, "G" + pageid );
            //                        thisPackage.Divisions.Add_Page_File_Link( "P" + pageid, "T" + fileid );
            //                    }
            //                    if ( subfilename.ToUpper().IndexOf(".JPG") > 0 )
            //                    {
            //                        thisPackage.Divisions.Add_File( "J" + fileid, subfilename, "G" + pageid );
            //                        thisPackage.Divisions.Add_Page_File_Link( "P" + pageid, "J" + fileid );
            //                    }
            //                    if ( subfilename.ToUpper().IndexOf(".JP2") > 0 )
            //                    {
            //                        thisPackage.Divisions.Add_File( "E" + fileid, subfilename, "G" + pageid );
            //                        thisPackage.Divisions.Add_Page_File_Link( "P" + pageid, "E" + fileid );
            //                    }
            //                }

            //                // Increment the file and division count
            //                fileid++;
            //                pageid++;
            //            }

						
            //            // Is this the beginning of a division?
            //            if ( next_sub == "<DIV" )
            //            {
            //                // Get the TYPE, which is a required field
            //                type = "Chapter";
            //                if ( next.IndexOf("TYPE=") > 0 )
            //                {
            //                    int typeIndex = next.IndexOf("TYPE=") + 5;
            //                    type = next.Substring( typeIndex, next.IndexOf(">") - typeIndex ).Replace("\"","").Trim();
            //                }

            //                //// Create a new division, depending on which level this is
            //                //if ( depth < 0 )
            //                //{
            //                //    lastDivision = (Division_TreeNode) thisPackage.Divisions.Add_Division( String.Empty, "D" + divid, type, type, divid );
            //                //}
            //                //else
            //                //{
            //                //    lastDivision = (Division_TreeNode) thisPackage.Divisions.Add_Division( ((abstract_TreeNode) treeNodes[depth]).Node_ID, "D" + divid, type, type, divid );
            //                //}

            //                // Move to the next depth and div id
            //                depth++;
            //                divid++;

            //                // Save this in the list
            //                treeNodes[ depth ] = lastDivision;
            //            }

            //            // Is this the end of a division?
            //            if ( next_sub == "</DI")
            //            {
            //                // If there has been a page already, was it assigned to this division?
            //                if (( lastPage != null ) && ( lastDivision != null ))
            //                {
            //                    if (( !lastPageHandled  ) && ( !lastDivision.Nodes.Contains( lastPage )))
            //                    {
            //                        // Add this page to this division
            //                        lastDivision.Nodes.Add( lastPage );
            //                        lastPageHandled = true;

            //                        // Remove this page from the root nodes, if it exists
            //                        if( thisPackage.Divisions.Root_Nodes.Contains( lastPage ))
            //                        {
            //                            thisPackage.Divisions.Root_Nodes.Remove( lastPage );
            //                        }
            //                    }
            //                }

            //                // step back the depth one
            //                depth--;

            //                // Get the last division
            //                if ( depth >= 0 )
            //                {
            //                    lastDivision = (Division_TreeNode) treeNodes[ depth ];
            //                }
            //                else
            //                {
            //                    lastDivision = null;
            //                }
            //            }

            //            // Is this a HEAD tag for the last division?
            //            if (( next_sub == "<HEA" ) || ( next_sub == "<DIV" ))
            //            {
            //                int headStart = next.IndexOf("<HEAD>");
            //                int headEnd = next.IndexOf("</HEAD>");
            //                if (( headStart >= 0 ) && ( headEnd > 0 ) && ( headStart < headEnd ))
            //                {
            //                    lastDivision.Label = next.Substring( headStart + 6, headEnd - headStart - 6 );
            //                }
            //            }
            //        }
            //    }

            //    // If this is not a tag, and into a file, add this to the building
            //    // string builder to save, if we are saving text files
            //    if (( createTextFiles ) && ( next.Trim().Length > 0 ) && ( next.Trim()[0] != '<' ) && ( fileName.Length > 0 ))
            //    {
            //        // Add this line to the current text file bldr
            //        textFileBldr.Append( next + "\n" );
            //    }

            //    // Read the next line in 
            //    next = reader.ReadLine();
            //}

            //// Make sure the last page was assigned
            //if ( lastPage != null ) 
            //{
            //    if (( lastDivision != null ) && ( !lastDivision.Nodes.Contains( lastPage )) && ( !lastPageHandled ))
            //    {
            //        // Add this page to this division
            //        lastDivision.Nodes.Add( lastPage );

            //        // Remove this page from the root nodes, if it exists
            //        if( thisPackage.Divisions.Root_Nodes.Contains( lastPage ))
            //        {
            //            thisPackage.Divisions.Root_Nodes.Remove( lastPage );
            //        }
            //    }

            //    // Also, if we are creating text files, do so here
            //    if ( createTextFiles )
            //    {
            //        // If there is no text, use the canned phrase
            //        if ( textFileBldr.Length == 0 )
            //        {
            //            textFileBldr.Append("This page contains no text.");
            //        }

            //        // Create the writer for this
            //        textFileOut = new StreamWriter( thisPackage.Source_Directory + "/" + fileName + ".txt", false );

            //        // Step through each character looking for illegal characters
            //        int charASCII;
            //        char character;
            //        for( int i = 0 ; i < textFileBldr.Length ; i++ )
            //        {
            //            // get the character, and then the ascii
            //            character = textFileBldr[ i ];
            //            charASCII = (int) character;

            //            // If this is illegal, change it
            //            if (( charASCII < 31 ) && ( charASCII != 0 ) && ( charASCII != 9 ) && ( charASCII != 10 ) && ( charASCII != 13 ))
            //            {
            //                textFileBldr[ i ] = ' ';
            //            }
            //        }

            //        // Now write this out to the file
            //        textFileOut.Write( textFileBldr.ToString() );
            //        textFileOut.Flush();
            //        textFileOut.Close();

            //        // Add this new text file to the package
            //        thisPackage.Divisions.Add_File( "T" + (fileid - 1), fileName + ".txt", "G" + (pageid - 1) );
            //        thisPackage.Divisions.Add_Page_File_Link( "P" + (pageid - 1), "T" + (fileid - 1) );
            //    }
            //}

			// Close the reader
			reader.Close();
		}
	}
}
