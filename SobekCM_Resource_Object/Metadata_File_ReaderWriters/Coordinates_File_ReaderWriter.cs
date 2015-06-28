#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;

#endregion

namespace SobekCM.Resource_Object.Metadata_File_ReaderWriters
{
    /// <summary> Reader reads the coordinate information from a file </summary>
    public class Coordinates_File_ReaderWriter : iMetadata_File_ReaderWriter
    {
        #region iMetadata_File_ReaderWriter Members

        /// <summary> Flag indicates if this reader/writer can read from files and streams </summary>
        /// <value> This property always returns FALSE </value>
        public bool canRead
        {
            get { return false; }
        }

        /// <summary> Flag indicates if this reader/writer can write to files and streams </summary>
        /// <value> This property always return TRUE </value>
        public bool canWrite
        {
            get { return true; }
        }

        /// <summary> Full name which best describes the metadata format this reader/writer utilizes (i.e. Metadata Object Description Standard, Greenstone file, etc.. ) </summary>
        /// <value>This property always returns 'GML Coordinates'</value>
        public string Metadata_Type_Name
        {
            get { return "GML Coordinates"; }
        }

        /// <summary> Abbreviation for the metadata format utilized by this reader/writer (i.e., MODS, GSA, etc.. ) </summary>
        /// <value> This property always returns 'GML'</value>
        public string Metadata_Type_Abbreviation
        {
            get { return "GML"; }
        }

        /// <summary> Reads metadata from an existing metadata file and saves to the provided item/package </summary>
        /// <param name="MetadataFilePathName"> Path and name of the metadata file to read </param>
        /// <param name="Return_Package"> Package into which to read the metadata </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during reading </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Read_Metadata(string MetadataFilePathName, SobekCM_Item Return_Package, Dictionary<string, object> Options, out string Error_Message)
        {
            Stream reader = new FileStream(MetadataFilePathName, FileMode.Open, FileAccess.Read);
            Read_Metadata(reader, Return_Package, Options, out Error_Message);
            reader.Close();

            throw new NotImplementedException();
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
            // Determine if this is CSV or XML
            string extension = Path.GetExtension(MetadataFilePathName);
            if (( extension != null ) && (( extension.ToLower() == ".csv" ) || ( extension.ToLower() == ".txt" )))
            {
                // In this case, need to pass a new option to the stream writer, without altering the 
                // original options
                Dictionary<string, object> newOptions = new Dictionary<string, object>();
                foreach (string thisKey in Options.Keys)
                {
                    if (thisKey != "Coordinates_File_ReaderWriter:CSV_Style")
                        newOptions[thisKey] = Options[thisKey];
                }
                newOptions["Coordinates_File_ReaderWriter:CSV_Style"] = true;

                try
                {
                    StreamWriter results = new StreamWriter(MetadataFilePathName, false, Encoding.UTF8);
                    bool returnValue = Write_Metadata(results, Item_To_Save, newOptions, out Error_Message);
                    results.Flush();
                    results.Close();

                    return returnValue;
                }
                catch (Exception ee)
                {
                    Error_Message = "Error writing GML Coordinates metadata to file '" + MetadataFilePathName + ": " + ee.Message;
                    return false;
                }
            }
            
            try
            {
                StreamWriter results = new StreamWriter(MetadataFilePathName, false, Encoding.UTF8);
                bool returnValue = Write_Metadata(results, Item_To_Save, Options, out Error_Message);
                results.Flush();
                results.Close();

                return returnValue;
            }
            catch (Exception ee)
            {
                Error_Message = "Error writing GML Coordinates metadata to file '" + MetadataFilePathName + ": " + ee.Message;
                return false;
            }
        }

        /// <summary> Writes the formatted metadata from the provided item to a TextWriter (usually to an output stream) </summary>
        /// <param name="Output_Stream"></param>
        /// <param name="Item_To_Save"> Package with all the metadata to save </param>
        /// <param name="Options"> Dictionary of any options which this metadata reader/writer may utilize </param>
        /// <param name="Error_Message">[OUTPUT] Explanation of the error, if an error occurs during write </param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        /// <remarks>This writer accepts one option value.  'Coordinates_File_ReaderWriter:CSV_Style' is a boolean value which indicates
        /// if the Dublin Core metadata file should be written in CSV format.  (Default is FALSE).</remarks>
        public bool Write_Metadata(TextWriter Output_Stream, SobekCM_Item Item_To_Save, Dictionary<string, object> Options, out string Error_Message)
        {
            // Set default error output message
            Error_Message = String.Empty;

            // Determine if this is to write the GML XML, or the less often used CSV coordinate file
            bool csv_style = false;
            if (Options.ContainsKey("Coordinates_File_ReaderWriter:CSV_Style"))
            {
                bool.TryParse(Options["Coordinates_File_ReaderWriter:CSV_Style"].ToString(), out csv_style);
            }
            
            // Get the coordinates object
            GeoSpatial_Information geoInfo = Item_To_Save.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            if ((geoInfo == null) || (!geoInfo.hasData))
            {
                Error_Message = "No coordinate information to write.";
                return false;
            }

            // Call the appropriate writer
            return csv_style ? Write_CSVFILE(Output_Stream, Item_To_Save, Options) : Write_GMLFile(Output_Stream, Item_To_Save, Options);
        }

        #endregion

        #region Method to write the coordinates as a CSV file 

        //new CSV Writer
        private bool Write_CSVFILE(TextWriter CsvOutput, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            //2do confirm that this outputs a string of goodies ready to place in a CSV file

            // GEt the geo-spatial information if it exists
            GeoSpatial_Information geoInfo = METS_Item.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            if (geoInfo == null)
                return true;

            string imageURL = "http://ufdc.ufl.edu/"; //create imageURL
            imageURL += METS_Item.BibID;
            imageURL += "/";
            imageURL += METS_Item.VID;

            CsvOutput.WriteLine("BIBID, VID, Title, Date, ImageURL, Latitude1, Longitude1, Latitude2, Longitude2, Latitude3, Longitude3, Latitude4, Longitude4"); //output csv header //2do, make definable?

            //for points
            foreach (Coordinate_Point thisPoint in geoInfo.Points)
            {
                //iURL writer
                string csvImageURL = imageURL;

                CsvOutput.Write(METS_Item.BibID + ", " + METS_Item.VID + ", " + METS_Item.Bib_Title.Replace(",", " ") + ", " + METS_Item.Bib_Info.Origin_Info.Date_Issued + ", " + csvImageURL + ", ");
                CsvOutput.Write(thisPoint.Latitude + ", " + thisPoint.Longitude + ", ");
                CsvOutput.WriteLine(); //add line break
            }

            //for lines
            foreach (Coordinate_Line thisLine in geoInfo.Lines)
            {
                //URL writer
                string csvImageURL = imageURL;

                CsvOutput.Write(METS_Item.BibID + ", " + METS_Item.VID + ", " + METS_Item.Bib_Title.Replace(",", " ") + ", " + METS_Item.Bib_Info.Origin_Info.Date_Issued + ", " + csvImageURL + ", ");
                foreach (Coordinate_Point thisPoint in thisLine.Points) //for each lat/long
                {
                    CsvOutput.Write(thisPoint.Latitude + ", " + thisPoint.Longitude + ", ");
                }
                CsvOutput.WriteLine(); //add line break
            }

            //for polygons
            foreach (Coordinate_Polygon thisPolygon in geoInfo.Polygons) //for each polygon with coordinates
            {
                //URL writer
                string csvImageURL = imageURL;
                if (geoInfo.Polygon_Count > 1) //this fixes bug where imageURL could be set to 0
                {
                    csvImageURL += "/";
                    csvImageURL += thisPolygon.Page_Sequence;
                }

                CsvOutput.Write(METS_Item.BibID + ", " + METS_Item.VID + ", " + METS_Item.Bib_Title.Replace(",", " ") + ", " + METS_Item.Bib_Info.Origin_Info.Date_Issued + ", " + csvImageURL + ", ");
                foreach (Coordinate_Point thisPoint in thisPolygon.Edge_Points) //for each lat/long
                {
                    CsvOutput.Write(thisPoint.Latitude + ", " + thisPoint.Longitude + ", "); //csv
                }
                CsvOutput.WriteLine(); //add line break
            }

            return true;
        }

        #endregion

        #region Method to write the coordinates as GML 

        //new GML Writer
        private bool Write_GMLFile(TextWriter GmlOutput, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            //2do confirm that this outputs a string of goodies ready to place in a GML file

            // GEt the geo-spatial information if it exists
            GeoSpatial_Information geoInfo = METS_Item.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
            if (geoInfo == null)
                return true;

            string GMLSchemaURL = "http://www.opengis.net/gml"; //create header  //2do: add custom schema
            string imageURL = "http://ufdc.ufl.edu/"; //create imageURL
            imageURL += METS_Item.BibID;
            imageURL += "/";
            imageURL += METS_Item.VID;
            string featureCollectionURL = imageURL; //create collectionURL

            GmlOutput.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>"); //write header
            GmlOutput.WriteLine("<gml:FeatureCollection xmlns:gml=\"" + GMLSchemaURL + "\">");
            GmlOutput.WriteLine("<sobekCM:Collection URI=\"" + featureCollectionURL + "\">"); //write collectionURL

            //for points
            foreach (Coordinate_Point thisPoint in geoInfo.Points)
            {
                string featureMemberURL = imageURL;

                GmlOutput.WriteLine("<gml:featureMember>");
                GmlOutput.WriteLine("<sobekCM:Item URL=\"" + featureMemberURL + "\">");
                GmlOutput.WriteLine("<gml:Point>");
                GmlOutput.Write("<gml:Coordinates>");
                GmlOutput.Write(thisPoint.Latitude + "," + thisPoint.Longitude + " ");
                GmlOutput.Write("</gml:Coordinates>");
                GmlOutput.WriteLine(); //add line break
                GmlOutput.WriteLine("</gml:Point>");
                GmlOutput.WriteLine("</sobekCM:Item>");
                GmlOutput.WriteLine("</gml:featureMember>");

            }

            //for lines
            foreach (Coordinate_Line thisLine in geoInfo.Lines)
            {
                string featureMemberURL = imageURL;

                GmlOutput.WriteLine("<gml:featureMember>");
                GmlOutput.WriteLine("<sobekCM:Item URL=\"" + featureMemberURL + "\">");
                GmlOutput.WriteLine("<gml:boundedBy>");
                GmlOutput.WriteLine("<gml:Box>");
                GmlOutput.Write("<gml:Coordinates>");
                foreach (Coordinate_Point thisPoint in thisLine.Points) //for each lat/long
                {
                    GmlOutput.Write(thisPoint.Latitude + "," + thisPoint.Longitude + " ");
                }
                GmlOutput.Write("</gml:Coordinates>");
                GmlOutput.WriteLine(); //add line break
                GmlOutput.WriteLine("</gml:Box>");
                GmlOutput.WriteLine("</gml:boundedBy>");
                GmlOutput.WriteLine("</sobekCM:Item>");
                GmlOutput.WriteLine("</gml:featureMember>");
            }

            //for polygons
            foreach (Coordinate_Polygon thisPolygon in geoInfo.Polygons) //for each polygon with coordinates
            {
                string featureMemberURL = imageURL;
                featureMemberURL += "/";
                featureMemberURL += thisPolygon.Page_Sequence;

                GmlOutput.WriteLine("<gml:featureMember>");
                GmlOutput.WriteLine("<sobekCM:Item URL=\"" + featureMemberURL + "\">");
                GmlOutput.WriteLine("<gml:Polygon>");
                GmlOutput.WriteLine("<gml:exterior>");
                GmlOutput.WriteLine("<gml:LinearRing>");
                GmlOutput.Write("<gml:Coordinates>");
                foreach (Coordinate_Point thisPoint in thisPolygon.Edge_Points) //for each lat/long
                {
                    GmlOutput.Write(thisPoint.Latitude + "," + thisPoint.Longitude + " "); //gml
                }
                GmlOutput.Write("</gml:Coordinates>");
                GmlOutput.WriteLine(); //gml, add line break
                GmlOutput.WriteLine("</gml:LinearRing>");
                GmlOutput.WriteLine("</gml:exterior>");
                GmlOutput.WriteLine("</gml:Polygon>");
                GmlOutput.WriteLine("<sobekCM:/Item>");
                GmlOutput.WriteLine("</gml:featureMember>");
            }


            //send closing gml tags and close gml file
            GmlOutput.WriteLine("</sobekCM:Collection>");
            GmlOutput.WriteLine("</gml:FeatureCollection>");
            GmlOutput.Flush();
            GmlOutput.Close();

            return true;
        }

        #endregion

    }
}
