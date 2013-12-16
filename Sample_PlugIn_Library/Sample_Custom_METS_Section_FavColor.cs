using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.METS_Sec_ReaderWriters;

namespace Sample_PlugIn_Library
{
    class Sample_Custom_METS_Section_FavColor : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            Sample_FavColor_Metadata_Module taxonInfo = METS_Item.Get_Metadata_Module(Sample_FavColor_Metadata_Module.Module_Name_Static) as Sample_FavColor_Metadata_Module;
            if ((taxonInfo == null) || (!taxonInfo.hasData))
                return true;

            
            Output_Stream.WriteLine("<MyFavColor>");

            if (!String.IsNullOrEmpty(taxonInfo.Absolute_Favorite_Color))
            {
                Output_Stream.WriteLine("<absoluteFavoriteColor>" + Convert_String_To_XML_Safe(taxonInfo.Absolute_Favorite_Color) + "</absoluteFavoriteColor>");
            }

            foreach (string thisCOlor in taxonInfo.Other_Favorite_Color)
            {
                Output_Stream.WriteLine("<additionalFavoriteColor>" + Convert_String_To_XML_Safe(thisCOlor) + "</additionalFavoriteColor>");
            }

            Output_Stream.WriteLine("</MyFavColor>");
            return true;
        }

        /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>  
        public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists
            Sample_FavColor_Metadata_Module taxonInfo = Return_Package.Get_Metadata_Module(Sample_FavColor_Metadata_Module.Module_Name_Static) as Sample_FavColor_Metadata_Module;
            if (taxonInfo == null)
            {
                taxonInfo = new Sample_FavColor_Metadata_Module();
                Return_Package.Add_Metadata_Module(Sample_FavColor_Metadata_Module.Module_Name_Static, taxonInfo);
            }

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && ((Input_XmlReader.Name == "METS:mdWrap") || (Input_XmlReader.Name == "mdWrap")))
                    return true;

                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();

                    switch (name)
                    {
                        case "absoluteFavoriteColor":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Absolute_Favorite_Color = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "additionalFavoriteColor":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxonInfo.Other_Favorite_Color.Add(Input_XmlReader.Value.Trim());
                            }
                            break;
                    }
                }
            } while (Input_XmlReader.Read());

            return true;

        }

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            Sample_FavColor_Metadata_Module taxonInfo = METS_Item.Get_Metadata_Module(Sample_FavColor_Metadata_Module.Module_Name_Static) as Sample_FavColor_Metadata_Module;
            if ((taxonInfo == null) || (!taxonInfo.hasData))
                return false;
            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            return false;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            string[] returnVal = new string[0];
            return returnVal;
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
                        string[] returnVal = new string[0];
            return returnVal;
        }
    }
}
