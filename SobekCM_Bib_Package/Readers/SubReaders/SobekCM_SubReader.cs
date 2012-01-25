using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Bib_Package.Divisions;

namespace SobekCM.Bib_Package.Readers.SubReaders
{
    /// <summary> Subreader reads a SobekCM-compliant section of XML and stores the data in the provided digital resource object </summary>
    public class SobekCM_SubReader
    {

        #region Read all the SobekCM information

        /// <summary> Reads the SobekCM-compliant DMD Section and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the SobekCM data </param>
        /// <param name="package"> Digital resource object to save the data to </param>
        /// <param name="deprecatedDownloads"> List of downloads read from the deprecated SobekCM donwloads tags </param>
        /// <param name="sobekcm_namespace"> Namespace utilized for this material in the XML file being read </param>
        public static void Read_SobekCM_DMD_Sec(XmlTextReader r, SobekCM_Item package, string sobekcm_namespace, List<Download_Info_DEPRECATED> deprecatedDownloads )
        {
            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "METS:mdWrap") || (r.Name == "mdWrap")))
                    return;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name.Replace(sobekcm_namespace + ":", ""))
                    {
                        case "Collection.Primary":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                string code = r.Value;
                                if (code.ToUpper() == "FLAP")
                                    code = "AERIALS";
                                if (code.ToUpper() == "MAP")
                                    code = "MAPS";
                                package.SobekCM_Web.Add_Aggregation(code);
                            }
                            break;

                        case "Collection.Alternate":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                string code = r.Value;
                                if (code.ToUpper() == "FLAP")
                                    code = "AERIALS";
                                if (code.ToUpper() == "MAP")
                                    code = "MAPS";
                                package.SobekCM_Web.Add_Aggregation(code);
                            }
                            break;

                        case "SubCollection":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.SobekCM_Web.Add_Aggregation(r.Value);
                            break;

                        case "Aggregation":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.SobekCM_Web.Add_Aggregation(r.Value);
                            break;

                        case "MainPage":
                            if (r.MoveToAttribute("pagename"))
                                package.SobekCM_Web.Main_Page.PageName = r.Value;
                            if (r.MoveToAttribute("previous"))
                            {
                                try
                                {
                                    package.SobekCM_Web.Main_Page.Previous_Page_Exists = Convert.ToBoolean(r.Value);
                                }
                                catch { }
                            }
                            if (r.MoveToAttribute("next"))
                            {
                                try
                                {
                                    package.SobekCM_Web.Main_Page.Next_Page_Exists = Convert.ToBoolean(r.Value);
                                }
                                catch { }
                            }
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.SobekCM_Web.Main_Page.FileName = r.Value;
                            break;

                        case "MainThumbnail":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.SobekCM_Web.Main_Thumbnail = r.Value;
                            break;

                        case "EncodingLevel":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.Bib_Info.EncodingLevel = r.Value;
                            break;

                        case "Icon":
                        case "Wordmark":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.SobekCM_Web.Add_Wordmark(r.Value);
                            break;

                        case "Download":
                            Download_Info_DEPRECATED newDownload = new Download_Info_DEPRECATED();
                            if (r.MoveToAttribute("label"))
                                newDownload.Label = r.Value;
                            while (r.Read())
                            {
                                if (r.NodeType == XmlNodeType.Element)
                                    break;

                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Download"))
                                    break;
                            }
                            switch (r.Name.Replace(sobekcm_namespace + ":", ""))
                            {
                                case "name":
                                    r.Read();
                                    if (r.NodeType == XmlNodeType.Text)
                                    {
                                        newDownload.FileName = r.Value;
                                        deprecatedDownloads.Add(newDownload);
                                    }
                                    break;

                                case "url":
                                    if (r.MoveToAttribute("type"))
                                        newDownload.FormatCode = r.Value;
                                    if (r.MoveToAttribute("size"))
                                    {
                                        try
                                        {
                                            newDownload.Size_MB = (float)Convert.ToDouble(r.Value);
                                        }
                                        catch { }
                                    }
                                    r.Read();
                                    if (r.NodeType == XmlNodeType.Text)
                                    {
                                        newDownload.URL = r.Value;
                                        deprecatedDownloads.Add(newDownload);
                                    }
                                    break;

                                case "fptr":
                                    if (r.MoveToAttribute("FILEID"))
                                    {
                                        newDownload.File_ID = r.Value;
                                        deprecatedDownloads.Add(newDownload);
                                    }
                                    break;
                            }
                            break;

                        case "URL":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.Bib_Info.Location.PURL = r.Value;
                            break;

                        case "GUID":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.SobekCM_Web.GUID = r.Value;
                            break;

                        case "NotifyEmail":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                package.SobekCM_Web.NotifyEmail = r.Value;
                            }
                            break;

                        case "BibID":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.Bib_Info.BibID = r.Value;
                            break;

                        case "VID":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                                package.Bib_Info.VID = r.Value;
                            break;

                        case "Affiliation":
                            Affiliation_Info newAffiliation = new Affiliation_Info();
                            if (r.MoveToAttribute("nameid"))
                                newAffiliation.Name_Reference = r.Value;
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Affiliation"))
                                {
                                    if (newAffiliation.hasData)
                                    {
                                        package.Bib_Info.Add_Affiliation(newAffiliation);
                                        break;
                                    }
                                }

                                if (r.NodeType == XmlNodeType.Element)
                                {
                                    switch (r.Name.Replace(sobekcm_namespace + ":", ""))
                                    {
                                        case "AffiliationTerm":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.Term = r.Value;
                                            break;

                                        case "University":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.University = r.Value;
                                            break;

                                        case "Campus":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.Campus = r.Value;
                                            break;

                                        case "College":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.College = r.Value;
                                            break;

                                        case "Unit":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.Unit = r.Value;
                                            break;

                                        case "Department":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.Department = r.Value;
                                            break;

                                        case "Institute":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.Institute = r.Value;
                                            break;

                                        case "Center":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.Center = r.Value;
                                            break;

                                        case "Section":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.Section = r.Value;
                                            break;

                                        case "SubSection":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                                newAffiliation.SubSection = r.Value;
                                            break;
                                    }
                                }
                            }
                            break;

                        case "Coordinates":
                            read_coordinates_info(r, package, sobekcm_namespace);
                            break;

                        case "Holding":
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Holding"))
                                    break;

                                if ((r.NodeType == XmlNodeType.Element) && (r.Name == sobekcm_namespace + ":statement"))
                                {
                                    if (r.MoveToAttribute("code"))
                                        package.Bib_Info.Location.Holding_Code = r.Value;
                                    r.Read();
                                    if (r.NodeType == XmlNodeType.Text)
                                    {
                                        package.Bib_Info.Location.Holding_Name = r.Value;
                                        break;
                                    }
                                }
                            }
                            break;

                        case "Source":
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Source"))
                                    break;

                                if ((r.NodeType == XmlNodeType.Element) && (r.Name == sobekcm_namespace + ":statement"))
                                {
                                    if (r.MoveToAttribute("code"))
                                        package.Bib_Info.Source.Code = r.Value;
                                    r.Read();
                                    if (r.NodeType == XmlNodeType.Text)
                                    {
                                        package.Bib_Info.Source.Statement = r.Value;
                                        break;
                                    }
                                }
                            }
                            break;

                        case "Temporal":
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Temporal"))
                                {
                                    break;
                                }

                                if ((r.NodeType == XmlNodeType.Element) && (r.Name == sobekcm_namespace + ":period"))
                                {
                                    Temporal_Info newTemporal = new Temporal_Info();
                                    if (r.MoveToAttribute("start"))
                                    {
                                        try
                                        {
                                            newTemporal.Start_Year = Convert.ToInt32(r.Value);
                                        }
                                        catch { }
                                    }

                                    if (r.MoveToAttribute("end"))
                                    {
                                        try
                                        {
                                            newTemporal.End_Year = Convert.ToInt32(r.Value);
                                        }
                                        catch { }
                                    }

                                    r.Read();
                                    if (r.NodeType == XmlNodeType.Text)
                                    {
                                        newTemporal.TimePeriod = r.Value;
                                        package.Bib_Info.Add_Temporal_Subject(newTemporal);
                                    }
                                }
                            }
                            break;

                        case "Type":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                package.Bib_Info.SobekCM_Type_String = r.Value.Trim();
                            }
                            break;

                        case "SortDate":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                try
                                {
                                    package.Bib_Info.SortDate = Convert.ToInt32(r.Value);
                                }
                                catch
                                {

                                }
                            }
                            break;

                        case "SortTitle":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                package.Bib_Info.SortTitle = r.Value;
                            }
                            break;

                        case "Manufacturer":
                            Publisher_Info thisManufacturer = null;
                            string manufacturer_id = String.Empty;
                            if (r.MoveToAttribute("ID"))
                                manufacturer_id = r.Value;
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Manufacturer"))
                                {
                                    if (thisManufacturer != null)
                                    {
                                        thisManufacturer.ID = manufacturer_id;
                                    }
                                    break;
                                }

                                if (r.NodeType == XmlNodeType.Element)
                                {
                                    switch (r.Name.Replace(sobekcm_namespace + ":", ""))
                                    {
                                        case "Name":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                            {
                                                thisManufacturer = package.Bib_Info.Add_Manufacturer(r.Value);
                                            }
                                            break;

                                        case "PlaceTerm":
                                            if (thisManufacturer != null)
                                            {
                                                if ((r.MoveToAttribute("type")) && (r.Value == "code"))
                                                {
                                                    if (r.MoveToAttribute("authority"))
                                                    {
                                                        switch (r.Value)
                                                        {
                                                            case "marccountry":
                                                                r.Read();
                                                                if (r.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisManufacturer.Add_Place(String.Empty, r.Value, String.Empty);
                                                                }
                                                                break;

                                                            case "iso3166":
                                                                r.Read();
                                                                if (r.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisManufacturer.Add_Place(String.Empty, String.Empty, r.Value);
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    r.Read();
                                                    if (r.NodeType == XmlNodeType.Text)
                                                    {
                                                        thisManufacturer.Add_Place(r.Value);
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;

                        case "Publisher":
                            Publisher_Info thisPublisher = null;
                            string publisher_id = String.Empty;
                            if (r.MoveToAttribute("ID"))
                                publisher_id = r.Value;
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Publisher"))
                                {
                                    if (thisPublisher != null)
                                    {
                                        thisPublisher.ID = publisher_id;
                                    }
                                    break;
                                }

                                if (r.NodeType == XmlNodeType.Element)
                                {
                                    switch (r.Name.Replace(sobekcm_namespace + ":", ""))
                                    {
                                        case "Name":
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                            {
                                                thisPublisher = package.Bib_Info.Add_Publisher(r.Value);
                                            }
                                            break;

                                        case "PlaceTerm":
                                            if (thisPublisher != null)
                                            {
                                                if ((r.MoveToAttribute("type")) && (r.Value == "code"))
                                                {
                                                    if (r.MoveToAttribute("authority"))
                                                    {
                                                        switch (r.Value)
                                                        {
                                                            case "marccountry":
                                                                r.Read();
                                                                if (r.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisPublisher.Add_Place(String.Empty, r.Value, String.Empty);
                                                                }
                                                                break;

                                                            case "iso3166":
                                                                r.Read();
                                                                if (r.NodeType == XmlNodeType.Text)
                                                                {
                                                                    thisPublisher.Add_Place(String.Empty, String.Empty, r.Value);
                                                                }
                                                                break;
                                                        }
                                                    }
                                                }
                                                else
                                                {
                                                    r.Read();
                                                    if (r.NodeType == XmlNodeType.Text)
                                                    {
                                                        thisPublisher.Add_Place(r.Value);
                                                    }
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;


                        case "part:performingArts":
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "part:performingArts"))
                                    break;

                                if (r.NodeType == XmlNodeType.Element)
                                {
                                    switch (r.Name)
                                    {
                                        case "part:Performer":
                                            string occupation = String.Empty;
                                            string lifespan = String.Empty;
                                            string title = String.Empty;
                                            string sex = String.Empty;
                                            if (r.MoveToAttribute("occupation"))
                                                occupation = r.Value;
                                            if (r.MoveToAttribute("lifespan"))
                                                lifespan = r.Value;
                                            if (r.MoveToAttribute("title"))
                                                title = r.Value;
                                            if (r.MoveToAttribute("sex"))
                                                sex = r.Value;
                                            r.Read();
                                            if (r.NodeType == XmlNodeType.Text)
                                            {
                                                Performer newPerformer = package.Performing_Arts_Info.Add_Performer(r.Value);
                                                newPerformer.Occupation = occupation;
                                                newPerformer.Title = title;
                                                newPerformer.Sex = sex;
                                                newPerformer.LifeSpan = lifespan;
                                            }
                                            break;

                                        case "part:Performance":
                                            if (r.MoveToAttribute("date"))
                                                package.Performing_Arts_Info.Performance_Date = r.Value;
                                            r.MoveToElement();
                                            if (!r.IsEmptyElement)
                                            {
                                                r.Read();
                                                if (r.NodeType == XmlNodeType.Text)
                                                {
                                                    package.Performing_Arts_Info.Performance = r.Value;
                                                }
                                            }
                                            break;
                                    }
                                }
                            }
                            break;

                        case "SerialHierarchy":
                            int serial_level = -1;
                            int serial_order = -1;
                            string serial_display = String.Empty;
                            if (r.MoveToAttribute("level"))
                            {
                                try
                                {
                                    serial_level = Convert.ToInt32(r.Value);
                                }
                                catch { }
                            }
                            if (r.MoveToAttribute("order"))
                            {
                                try
                                {
                                    serial_order = Convert.ToInt32(r.Value);
                                }
                                catch { }
                            }
                            r.MoveToElement();
                            if (!r.IsEmptyElement)
                            {
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    serial_display = r.Value;
                            }
                            if ((serial_display.Length == 0) && (serial_order > 0))
                                serial_display = serial_order.ToString();
                            if ((serial_display.Length > 0) && (serial_order > 0))
                                package.Serial_Info.Add_Hierarchy(serial_level, serial_order, serial_display);
                            break;

                        case "Container":
                            int container_level = -1;
                            string container_type = String.Empty;
                            string container_name = String.Empty;
                            if (r.MoveToAttribute("level"))
                            {
                                try
                                {
                                    container_level = Convert.ToInt32(r.Value);
                                }
                                catch { }
                            }
                            if (r.MoveToAttribute("type"))
                            {
                                container_type = r.Value;
                            }
                            r.MoveToElement();
                            if (!r.IsEmptyElement)
                            {
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                {
                                    container_name = r.Value;

                                    package.Bib_Info.Add_Container(container_type, container_name, container_level);
                                }
                            }
                            break;

                        case "oral:Interviewee":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                package.Oral_Info.Interviewee = r.Value;
                            }
                            break;

                        case "oral:Interviewer":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                package.Oral_Info.Interviewer = r.Value;
                            }
                            break;
                    }
                }
            }
        }

        private static void read_coordinates_info(XmlTextReader r, SobekCM_Item package, string sobekcm_namespace )
        {
            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Coordinates"))
                {
                    return;
                }

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name.Replace(sobekcm_namespace + ":", ""))
                    {
                        case "KML":
                            if (!r.IsEmptyElement)
                            {
                                r.Read();
                                if (r.NodeType == XmlNodeType.Text)
                                    package.Bib_Info.Coordinates.KML_Reference = r.Value;
                            }
                            break;
                        case "Point":
                            package.Bib_Info.Coordinates.Add_Point(read_point(r));
                            break;

                        case "Line":
                            Coordinate_Line newLine = new Coordinate_Line();
                            if (r.MoveToAttribute("label"))
                                newLine.Label = r.Value;
                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Line"))
                                {
                                    package.Bib_Info.Coordinates.Add_Line(newLine);
                                    break;
                                }

                                if ((r.NodeType == XmlNodeType.Element) && (r.Name == sobekcm_namespace + ":Point"))
                                {
                                    newLine.Add_Point(read_point(r));
                                }
                            }
                            break;

                        case "Polygon":
                            Coordinate_Polygon newPolygon = new Coordinate_Polygon();
                            if (r.MoveToAttribute("label"))
                                newPolygon.Label = r.Value;
                            if (r.MoveToAttribute("ID"))
                                newPolygon.ID = r.Value;
                            if (r.MoveToAttribute("pageSeq"))
                            {
                                try
                                {
                                    newPolygon.Page_Sequence = Convert.ToUInt16(r.Value);
                                }
                                catch { }
                            }

                            while (r.Read())
                            {
                                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Polygon"))
                                {
                                    package.Bib_Info.Coordinates.Add_Polygon(newPolygon);
                                    break;
                                }

                                if (r.NodeType == XmlNodeType.Element)
                                {
                                    if (r.Name == sobekcm_namespace + ":Edge")
                                    {
                                        while (r.Read())
                                        {
                                            if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Edge"))
                                            {
                                                break;
                                            }

                                            if ((r.NodeType == XmlNodeType.Element) && (r.Name == sobekcm_namespace + ":Point"))
                                            {
                                                newPolygon.Add_Edge_Point(read_point(r));
                                            }
                                        }
                                    }

                                    if (r.Name == sobekcm_namespace + ":Internal")
                                    {
                                        while (r.Read())
                                        {
                                            if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == sobekcm_namespace + ":Internal"))
                                            {
                                                break;
                                            }

                                            if ((r.NodeType == XmlNodeType.Element) && (r.Name == sobekcm_namespace + ":Point"))
                                            {
                                                newPolygon.Add_Inner_Point(read_point(r));
                                            }
                                        }
                                    }
                                }
                            }
                            break;
                    }
                }
            }
        }

        private static Coordinate_Point read_point(XmlTextReader r)
        {
            try
            {
                Coordinate_Point newPoint = new Coordinate_Point();
                if (r.MoveToAttribute("latitude"))
                    newPoint.Latitude = Convert.ToDouble(r.Value.Replace("°", ""));
                if (r.MoveToAttribute("longitude"))
                    newPoint.Longitude = Convert.ToDouble(r.Value.Replace("°", ""));
                if (r.MoveToAttribute("altitude"))
                {
                    try
                    {
                        newPoint.Altitude = (long)Convert.ToDouble(r.Value);
                    }
                    catch { }
                }
                if (r.MoveToAttribute("order"))
                {
                    try
                    {
                        newPoint.Order_From_XML_Read = Convert.ToInt32(r.Value);
                    }
                    catch { }
                }
                if (r.MoveToAttribute("label"))
                    newPoint.Label = r.Value;
                return newPoint;
            }
            catch
            {
                return null;
            }
        }

        #endregion

        #region Read all the SobekCM Map information

        /// <summary> Reads the SobekCM-compliant Map Section and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the SobekCM data </param>
        /// <param name="package"> Digital resource object to save the data to </param>
        public static void Read_SobekCM_Map_DMD_Sec(XmlTextReader r, SobekCM_Item package)
        {
            // Try to get the attributes from here first
            if (r.MoveToAttribute("id"))
                package.Map.MapID = r.Value;

            // Step through each field
            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "map:ufdc_map"))
                    return;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "map:indexes":
                            read_map_indexes(r, package);
                            break;

                        case "map:entities":
                            read_map_entities(r, package);
                            break;

                        case "map:sheets":
                            read_map_sheets(r, package);
                            break;
                    }
                }
            }
        }

        private static void read_map_indexes(XmlTextReader r, SobekCM_Item package)
        {
            string title = String.Empty;
            string file = String.Empty;
            string html = String.Empty;
            string type = String.Empty;
            string id_string = String.Empty;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement)
                {
                    switch (r.Name)
                    {
                        case "map:indexes":
                            return;

                        case "map:image":
                            try
                            {
                                long id = Convert.ToInt64(id_string.ToUpper().Replace("INDE", "").Replace("X", ""));
                                package.Map.New_Index(id, title, file, html, type);
                            }
                            catch { }

                            // Clear the variables
                            title = String.Empty;
                            file = String.Empty;
                            html = String.Empty;
                            type = String.Empty;
                            id_string = String.Empty;
                            break;
                    }
                }
                else if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "map:image":
                            if (r.MoveToAttribute("type"))
                                type = r.Value;
                            if (r.MoveToAttribute("id"))
                                id_string = r.Value;
                            break;

                        case "map:title":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                title = r.Value;
                            }
                            break;

                        case "map:file":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                file = r.Value;
                            }
                            break;

                        case "map:html":
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                html = r.Value;
                            }
                            break;
                    }
                }
            }
        }

        private static void read_map_entities(XmlTextReader r, SobekCM_Item package)
        {
            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "map:entities"))
                {
                    return;
                }

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "map:street":
                            read_map_street(r, package);
                            break;

                        case "map:feature":
                            read_map_feature(r, package);
                            break;

                        case "map:person":
                            read_map_person(r, package);
                            break;

                        case "map:corporation":
                            read_map_corporation(r, package);
                            break;
                    }
                }
            }
        }

        private static void read_map_street(XmlTextReader r, SobekCM_Item package)
        {
            string id_string = String.Empty;
            string name = String.Empty;
            if (r.MoveToAttribute("id"))
                id_string = r.Value;
            if (r.MoveToAttribute("name"))
                name = r.Value;

            // Determine the street id
            long streetid = -1;
            try
            {
                streetid = Convert.ToInt64(id_string.Replace("STR", "").Replace("E", "").Replace("T", ""));
            }
            catch
            {
            }

            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "map:street"))
                    return;

                if ((r.NodeType == XmlNodeType.Element) && (r.Name == "map:segment"))
                {
                    long sheetid = -1;
                    long start = -1;
                    long end = -1;
                    string direction = String.Empty;
                    string side = String.Empty;
                    string desc = String.Empty;

                    for (int i = 0; i < r.AttributeCount; i++)
                    {
                        r.MoveToAttribute(i);
                        switch (r.Name)
                        {
                            case "sheetid":
                                try
                                {
                                    sheetid = Convert.ToInt64(r.Value.Replace("MS", "").Replace("SHEET", ""));
                                }
                                catch
                                {
                                }
                                break;

                            case "side":
                                side = r.Value;
                                break;

                            case "direction":
                                direction = r.Value;
                                break;

                            case "start":
                                try
                                {
                                    start = Convert.ToInt64(r.Value.Trim());
                                }
                                catch
                                {
                                }
                                break;

                            case "end":
                                try
                                {
                                    start = Convert.ToInt64(r.Value.Trim());
                                }
                                catch
                                {
                                }
                                break;
                        }
                    }

                    r.MoveToElement();
                    if (!r.IsEmptyElement)
                    {
                        r.Read();
                        desc = r.Value;
                    }

                    // Add this street segment information
                    if (sheetid > 0)
                    {
                        package.Map.Add_Street(streetid, sheetid, name, desc, direction, start, end, side);
                    }
                }
            }
        }

        private static void read_map_feature(XmlTextReader r, SobekCM_Item package)
        {
            string id_string = String.Empty;
            string name = String.Empty;
            string type = String.Empty;
            if (r.MoveToAttribute("id"))
                id_string = r.Value;
            if (r.MoveToAttribute("name"))
                name = r.Value;
            if (r.MoveToAttribute("type"))
                type = r.Value;

            // Determine the feature id
            long featid = -1;
            try
            {
                featid = Convert.ToInt64(id_string.Replace("FEAT", "").Replace("U", "").Replace("R", "").Replace("E", ""));
            }
            catch
            {
            }

            // Add this feature
            Maps.Map_Info_Tables.FeatureRow thisFeature = package.Map.Add_Feature(featid, name, type);

            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "map:feature"))
                    return;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "map:desc":
                            thisFeature.Description = r.Value;
                            break;

                        case "map:coordinates":
                            for (int i = 0; i < r.AttributeCount; i++)
                            {
                                r.MoveToAttribute(i);
                                switch (r.Name)
                                {
                                    case "units":
                                        thisFeature.Units = r.Value;
                                        break;

                                    case "latitude":
                                        thisFeature.Latitude = r.Value;
                                        break;

                                    case "longitude":
                                        thisFeature.Longitude = r.Value;
                                        break;
                                }
                            }
                            break;

                        case "map:sheetref":
                            long x = -1;
                            long y = -1;
                            long sheetid = -1;
                            for (int i = 0; i < r.AttributeCount; i++)
                            {
                                r.MoveToAttribute(i);
                                switch (r.Name)
                                {
                                    case "x":
                                        try
                                        {
                                            x = Convert.ToInt64(r.Value);
                                        }
                                        catch { }
                                        break;

                                    case "y":
                                        try
                                        {
                                            y = Convert.ToInt64(r.Value);
                                        }
                                        catch { }
                                        break;

                                    case "sheetid":
                                        try
                                        {
                                            sheetid = Convert.ToInt64(r.Value.Replace("MS", "").Replace("SHEET", ""));
                                        }
                                        catch { }
                                        break;
                                }
                            }

                            if (sheetid > 0)
                            {
                                package.Map.Add_Feature_Sheet_Link(thisFeature.FeatureID, sheetid, x, y);
                            }
                            break;

                        case "map:corpref":
                            if (r.MoveToAttribute("corpid"))
                            {
                                try
                                {
                                    long corpid = Convert.ToInt64(r.Value.Replace("COR", "").Replace("P", ""));
                                    package.Map.Add_Feature_Corp_Link(thisFeature.FeatureID, corpid);
                                }
                                catch
                                {

                                }
                            }
                            break;

                        case "map:persid":
                            string reftype = String.Empty;
                            if (r.MoveToAttribute("reftype"))
                                reftype = r.Value;
                            if (r.MoveToAttribute("persid"))
                            {
                                try
                                {
                                    long persid = Convert.ToInt64(r.Value.Replace("PER", "").Replace("S", "").Replace("O", "").Replace("N", ""));
                                    package.Map.Add_Feature_Person_Link(thisFeature.FeatureID, persid, reftype);
                                }
                                catch
                                {

                                }
                            }
                            break;
                    }
                }
            }
        }

        private static void read_map_person(XmlTextReader r, SobekCM_Item package)
        {
            if (r.MoveToAttribute("id"))
            {
                try
                {
                    long personid = Convert.ToInt64(r.Value.Replace("PER", "").Replace("S", "").Replace("O", "").Replace("N", ""));

                    while (r.Read())
                    {
                        if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "map:person"))
                            return;

                        if ((r.NodeType == XmlNodeType.Element) && (r.Name == "map:persname") && (!r.IsEmptyElement))
                        {
                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                package.Map.New_Person(personid, r.Value);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private static void read_map_corporation(XmlTextReader r, SobekCM_Item package)
        {
            if (r.MoveToAttribute("id"))
            {
                try
                {
                    long corpid = Convert.ToInt64(r.Value.Replace("COR", "").Replace("P", ""));

                    string type = String.Empty;
                    string primary_name = String.Empty;
                    List<string> alternate_names = new List<string>();

                    while (r.Read())
                    {
                        if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "map:corporation"))
                        {
                            if ((corpid > 0) && (primary_name.Length > 0))
                            {
                                Maps.Map_Corporation thisCorp = package.Map.New_Corporation(corpid, primary_name);
                                foreach (string altName in alternate_names)
                                    thisCorp.Add_Alt_Name(altName);
                            }
                            return;
                        }

                        if ((r.NodeType == XmlNodeType.Element) && (r.Name == "map:corpname") && (!r.IsEmptyElement))
                        {
                            if (r.MoveToAttribute("type"))
                                type = r.Value;
                            else
                                type = String.Empty;

                            r.Read();
                            if (r.NodeType == XmlNodeType.Text)
                            {
                                if ((type.Length == 0) || (type == "primary"))
                                    primary_name = r.Value;
                                else
                                    alternate_names.Add(r.Value);
                            }
                        }
                    }
                }
                catch { }
            }
        }

        private static void read_map_sheets(XmlTextReader r, SobekCM_Item package)
        {
            string id_string = String.Empty;
            string file = String.Empty;

            while (r.Read())
            {
                if (r.NodeType == XmlNodeType.EndElement)
                {
                    switch (r.Name)
                    {
                        case "map:sheets":
                            return;

                        case "map:sheet":
                            try
                            {
                                long id = Convert.ToInt64(id_string.ToUpper().Replace("MS", ""));
                                package.Map.New_Sheet(id, 0, file, String.Empty);
                            }
                            catch { }

                            // Clear the variables
                            id_string = String.Empty;
                            file = String.Empty;
                            break;
                    }
                }
                else if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "map:sheet":
                            if (r.MoveToAttribute("id"))
                                id_string = r.Value;
                            break;

                        case "map:fileref":
                            if (r.MoveToAttribute("fileid"))
                                file = r.Value;
                            break;
                    }
                }
            }
        }

        #endregion

        #region Read the SobekCM technical file information

        /// <summary> Reads the SobekCM-compliant File Section and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the SobekCM data </param>
        /// <param name="package"> Digital resource object to save the data to </param>
        /// <param name="sobekcm_namespace"> Namespace utilized for this material in the XML file being read </param>
        /// <param name="files_by_fileid"> Dictionary of all files by file id.  This can be empty to be filled here, or can contain basic information from the SobeKCM file technical data section </param>
        public static void Read_SobekCM_File_Sec(XmlTextReader r, SobekCM_Item package, string sobekcm_namespace, Dictionary<string, SobekCM_File_Info> files_by_fileid )
        {
            string fileid = String.Empty;

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((r.NodeType == XmlNodeType.EndElement) && (r.Name ==  sobekcm_namespace + ":FileInfo"))
                    return;

                // get the right division information based on node type
                switch (r.NodeType)
                {
                    case XmlNodeType.EndElement:
                        if (r.Name == sobekcm_namespace + ":FileInfo")
                            return;
                        break;

                    case XmlNodeType.Element:
                        if ((r.Name == sobekcm_namespace + ":File") && (r.HasAttributes) && (r.MoveToAttribute("fileid")))
                        {
                            fileid = r.Value;

                            // Save this information
                            SobekCM_File_Info existingFile = null;
                            if (!files_by_fileid.ContainsKey(fileid))
                            {
                                existingFile = new SobekCM_File_Info(String.Empty);
                                files_by_fileid[fileid] = existingFile;
                            }
                            else
                            {
                                existingFile = files_by_fileid[fileid];
                            }

                            try
                            {
                                if (r.MoveToAttribute("width"))
                                    existingFile.Width = Convert.ToUInt16(r.Value);

                                if (r.MoveToAttribute("height"))
                                    existingFile.Height = Convert.ToUInt16(r.Value);
                            }
                            catch
                            {
                            }

                            if (r.MoveToAttribute("servicecopy"))
                                existingFile.Service_Copy = r.Value;
                        }
                        break;
                }
            } while (r.Read());
        }

        #endregion

    }
}
