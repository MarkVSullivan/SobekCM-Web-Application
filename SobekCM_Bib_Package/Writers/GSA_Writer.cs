using System;
using System.IO;
using System.Collections;
using System.Text;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;

namespace SobekCM.Bib_Package.Writers
{
	/// <summary>Writer creates a Greenstone Archive format XML file </summary>
    /// <remarks>This writer is used by the SobekCM Builder when processing incoming bibliographic packages.<br /> <br />
	/// Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
	public class GSA_Writer
	{
		/// <summary> Constructor for the GSA_Writer class </summary>
		static GSA_Writer()
		{
			// Empty static writer
		}

        /// <summary> Saves this bibliographic resource in Greenstone Archive format XML </summary>
        /// <param name="thisBib">Bibliographic Package to save in GSA form</param>
        public static void Save(SobekCM_Item thisBib)
        {
            Save(thisBib, thisBib.Source_Directory);
        }

		/// <summary> Saves this bibliographic resource in Greenstone Archive format XML </summary>
		/// <param name="thisBib">Bibliographic Package to save in GSA form</param>
        /// <param name="Text_File_Directory">Directory in which all of the full text pages exist</param>
		public static void Save( SobekCM_Item thisBib, string Text_File_Directory )
		{
			// Create the filename to save
			string FileName = thisBib.Divisions.Source_Directory + "/doc.xml";
			if ( FileName[0] == '/' )
			{
				FileName = FileName.Substring(1);
			}

			// Write to a string builder first, then we'll dump to the file
			StringBuilder results = new StringBuilder();
            string indent = "    ";

			// Add the XML declaration
			results.Append( "<?xml version=\"1.0\" encoding=\"UTF-8\" standalone=\"no\"?>\r\n" );

			// Add the document type declaration
			results.Append( "<!DOCTYPE Archive SYSTEM \"http://greenstone.org/dtd/Archive/1.0/Archive.dtd\">\r\n");

			// Start the description section
			results.Append( "<Archive>\r\n" );
			results.Append( "<Section>\r\n");
			results.Append( "  <Description>\r\n" );

			// Add the administrative metadata first
            results.Append("    <Metadata name=\"gsdldoctype\">indexed_doc</Metadata>\r\n");
            results.Append("    <Metadata name=\"gsdlthistype\">Paged</Metadata>\r\n");
            results.Append("    <Metadata name=\"Identifier\">" + thisBib.BibID + thisBib.VID + "</Metadata>\r\n");
            results.Append("    <Metadata name=\"assocfilepath\">" + thisBib.SobekCM_Web.File_Root + "</Metadata>\r\n");

            // Add the number of pages
            results.Append("    <Metadata name=\"NumPages\">" + thisBib.Divisions.Page_Count + "</Metadata>\r\n");
            
			// Add the bibliographic metadata next
            results.Append(thisBib.Bib_Info.GSA_Descriptive_Metadata(thisBib.Divisions.Table_Of_Contents_XML, true).Replace("&lt;", "&amp;lt;").Replace("&gt;", "&amp;gt;"));

            // Add the oral interview parts
            if (thisBib.hasOralHistoryInformation )
            {
                results.Append(thisBib.Oral_Info.GSA_Interview_Metadata.Replace("&lt;", "&amp;lt;").Replace("&gt;", "&amp;gt;"));
            }

            // Add the performing arts part
            if (thisBib.hasPerformingArtsInformation )
            {
                results.Append(thisBib.Performing_Arts_Info.GSA_Performing_Arts_Metadata.Replace("&lt;", "&amp;lt;").Replace("&gt;", "&amp;gt;"));
            }

            // Add the aggregation codes
            if (thisBib.SobekCM_Web.Aggregation_Count > 0)
            {
                foreach (SobekCM.Bib_Package.SobekCM_Info.Aggregation_Info aggregation in thisBib.SobekCM_Web.Aggregations)
                {
                    string altCollection = aggregation.Code;
                    results.Append(To_GSA(altCollection.ToLower(), "sobekcm.AggregationCode", indent));
                }
            }
            if (thisBib.Bib_Info.Source.Code.Length > 0)
            {
                if (thisBib.Bib_Info.Source.Code.ToLower()[0] != 'i')
                    results.Append(To_GSA("i" + thisBib.Bib_Info.Source.Code, "sobekcm.AggregationCode", indent));
                else
                    results.Append(To_GSA(thisBib.Bib_Info.Source.Code, "sobekcm.AggregationCode", indent));
            }
            if (( thisBib.Bib_Info.hasLocationInformation ) && (thisBib.Bib_Info.Location.Holding_Code.Length > 0))
            {
                if (thisBib.Bib_Info.Location.Holding_Code.ToLower()[0] != 'i')
                    results.Append(To_GSA("i" + thisBib.Bib_Info.Location.Holding_Code, "sobekcm.AggregationCode", indent));
                else
                    results.Append(To_GSA(thisBib.Bib_Info.Location.Holding_Code, "sobekcm.AggregationCode", indent));
            }

			// Close out the description part of this main section
			results.Append( "  </Description>\r\n");
            results.Append("  <Content>aaaaa</Content>\r\n");

			// Add all the file portions
			results.Append( thisBib.Divisions.GSA_Detailed_File_Section( thisBib.BibID, thisBib.VID, false, Text_File_Directory ));

			// Close out the whole document
			results.Append( "</Section>\r\n" );
			results.Append( "</Archive>\r\n");

			try
			{
				// Write this to the file
				StreamWriter writer = new StreamWriter( FileName, false, System.Text.Encoding.UTF8 );
				writer.Write( results.ToString() );
				writer.Flush();
				writer.Close();
			}
			catch (Exception ee )
			{
				throw new ApplicationException("Error caught while writing the GSA file.\n\n" + ee.Message );
			}
		}

        private static string Convert_String_To_XML_Safe(string element)
        {
            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        private static string To_GSA(string value, string metadata_name, string indent)
        {
            if (value.Length == 0)
                return String.Empty;

            return indent + "<Metadata name=\"" + metadata_name + "\">" + Convert_String_To_XML_Safe(value).Replace("[", "").Replace("]", "") + "</Metadata>\r\n";
        }
	}
}
