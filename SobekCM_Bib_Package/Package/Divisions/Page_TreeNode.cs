using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Drawing;

namespace SobekCM.Bib_Package.Divisions
{
	/// <summary> Page node in a strucutral map tree associated with a digital resource</summary>
	/// <remarks> This class extends the <see cref="abstract_TreeNode"/> class. <br /> <br /> 
	/// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    [Serializable]
	public class Page_TreeNode : abstract_TreeNode
	{
        private List<SobekCM_File_Info> files;

		/// <summary> Constructor creates an empty instance of the Page_TreeNode class </summary>
		public Page_TreeNode() : base( "Page", String.Empty )
		{
            files = new List<SobekCM_File_Info>();
		}

		/// <summary> Constructor creates a new instance of the Page_TreeNode class </summary>
		/// <param name="Label">Node label</param>
        public Page_TreeNode( string Label ) : base( "Page", Label)
		{
            files = new List<SobekCM_File_Info>();

			// If the label is just 'page'
			if ( Label.ToUpper() == "PAGE" )
			{
				label = String.Empty;
			}
		}

		/// <summary> Gets the flag indicating if this is a page node or not </summary>
		/// <value>Always returns 'TRUE'</value>
		public override bool Page
		{
			get	{	return true;	}
		}

		/// <summary> Gets the collection of files under this page </summary>
		/// <remarks> This returns a generic list which is a collection of <see cref="SobekCM_File_Info"/> objects. </remarks>
		public List<SobekCM_File_Info> Files
		{
			get	{	return files;	}
		}

        /// <summary> Returns this page information in Greenstone Archival format </summary>
        /// <param name="BibID">BibID for the parent digital resource</param>
        /// <param name="VID">Volume ID for the parent digital resource</param>
        /// <param name="Order">Order for this page in the current division</param>
        /// <param name="Directory">Directory in which to find the TEXT files</param>
        /// <param name="textDisplayable">Flag indicates if the text formatting should be retained</param>
        /// <returns>XML of for this page in Greenstone Archival format</returns>
        internal string toGSA(string BibID, string VID, ushort Order, string Directory, bool textDisplayable)
        {
            // Declare the stringbuilder and start the section
            StringBuilder result = new StringBuilder("<Section>\r\n  <Description>\r\n");

            // Use the page label, if there was one
            if (this.Label.Length > 0)
            {
                result.Append("    <Metadata name=\"Title\">" + Order + "_" + Label + "</Metadata>\r\n");
            }
            else
            {
                result.Append("    <Metadata name=\"Title\">" + Order + "</Metadata>\r\n");
            }

            // Add the volume level metadata here
            result.Append("    <Metadata name=\"sobekcm.BibID\">" + BibID + "</Metadata>\r\n");
            result.Append("    <Metadata name=\"sobekcm.VID\">" + VID + "</Metadata>\r\n");
            result.Append("    <Metadata name=\"FileFormat\">PagedImg</Metadata>\r\n");

            // Find any jpeg file
            string jpeg_file = String.Empty;
            foreach (SobekCM_File_Info thisFile in this.Files)
            {
                // Is this the jpeg?
                string file_extension = thisFile.File_Extension;
                if (((file_extension == "JPEG") || ( file_extension == "JPG" )) && (thisFile.System_Name.ToUpper().IndexOf("THM") < 0))
                {
                    // Write all the attributes for this
                    result.Append("    <Metadata name=\"ScreenType\">jpg</Metadata>\r\n");
                    result.Append("    <Metadata name=\"Screen\">" + thisFile.System_Name + "</Metadata>\r\n");
                    jpeg_file = thisFile.System_Name;
                    break;
                }
            }

            // End the description
            result.Append("  </Description> \r\n");

            // Set the default text message
            string text = "abcdefghijkl";

            // Find any text file
            if (Directory.Length > 0)
            {
                string text_file = String.Empty;
                foreach (SobekCM_File_Info thisFile in this.Files)
                {
                    // Is this the text?
                    string extension = thisFile.File_Extension;
                    if (( extension == "TXT" ) || ( extension == "TEXT" ))
                    {
                        if (File.Exists(Directory + "/" + thisFile.System_Name))
                        {
                            text_file = Directory + "/" + thisFile.System_Name;

                            // Break out since a text file was found
                            break;
                        }
                    }
                }

                // If no text file was found in the list, just look for a text file matching the jpeg name
                if ((text_file.Length == 0) && (jpeg_file.Length > 0))
                {
                    if (File.Exists(Directory + "/" + jpeg_file.ToUpper().Replace(".JPG", ".TXT")))
                    {
                        text_file = Directory + "/" + jpeg_file.ToUpper().Replace(".JPG", ".TXT");
                    }
                }

                // Try to read any text file
                if (text_file.Length > 0)
                {
                    // Get the text from this document
                    StreamReader reader = new StreamReader(text_file);
                    text = reader.ReadToEnd();

                    if (!textDisplayable)
                        text = text.Replace("&", "&amp;amp;").Replace("<", "&amp;lt;").Replace(">", "&amp;gt;").Replace("\"", "&amp;quot;").Replace("\r", "").Replace("\n", "");
                    else
                        text = text.Replace("&", "&amp;amp;").Replace("<", "&amp;lt;").Replace(">", "&amp;gt;").Replace("\"", "&amp;quot;").Replace("\r\n", "\n").Replace("\r", "");

                    reader.Close();
                }
            }

            if (text.Trim().Length == 0)
                text = "aaaaa";

            // Add the text porion to the GSA file
            result.Append("  <Content>" + text + "</Content>\r\n");

            // Close out this section
            result.Append("</Section>\r\n");

            // Return this built item
            return result.ToString();
        }
	}
}
