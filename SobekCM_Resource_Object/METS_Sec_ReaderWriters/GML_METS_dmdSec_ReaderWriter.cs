#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.GeoSpatial;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
  public class GML_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter, iDivision_dmdSec_ReaderWriter
  {
    #region Properties and methods that implement iPackage_dmdSec_ReaderWriter

    /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
    /// <param name="Output_Stream">Stream to which the formatted text is written </param>
    /// <param name="METS_Item">Package with all the metadata to save</param>
    /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
    /// <returns>TRUE if successful, otherwise FALSE </returns>
    public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
    {
      // GEt the geo-spatial information if it exists
      GeoSpatial_Information geoInfo = METS_Item.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if ((geoInfo == null) || (!geoInfo.hasData))
        return true;

      return Write_Metadata_Section(Output_Stream, geoInfo, Options);
    }

    /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
    /// entire package  </summary>
    /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
    /// <param name="Return_Package"> Package into which to read the metadata</param>
    /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
    /// <returns> TRUE if successful, otherwise FALSE</returns>  
    public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
    {
      // Get the geo-spatial information if it exists or create a new one
      GeoSpatial_Information geoInfo = Return_Package.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if (geoInfo == null)
      {
        geoInfo = new GeoSpatial_Information();
        Return_Package.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo);
      }

      return Read_Metadata_Section(Input_XmlReader, geoInfo, Options);
    }

    /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
    /// <param name="METS_Item"> Package with all the metadata to save</param>
    /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
    /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
    public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
    {
      // GEt the geo-spatial information if it exists
      GeoSpatial_Information geoInfo = METS_Item.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if ((geoInfo == null) || (!geoInfo.hasData))
        return false;
      return true;
    }

    /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
    /// to the METS XML header by analyzing the contents of the digital resource item </summary>
    /// <param name="METS_Item"> Package with all the metadata to save</param>
    /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
    public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
    {
      // GEt the geo-spatial information if it exists
      GeoSpatial_Information geoInfo = METS_Item.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if ((geoInfo == null) || (!geoInfo.hasData))
        return false;
      return true;
    }

    #endregion

    #region Properties and methods that implement iDivision_dmdSec_ReaderWriter

    /// <summary> Writes the dmdSec for one subsection/division of the METS structure map </summary>
    /// <param name="Output_Stream">Stream to which the formatted text is written </param>
    /// <param name="MetsDivision">Division from the overall package with all the metadata to save</param>
    /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
    /// <returns>TRUE if successful, otherwise FALSE </returns>
    public bool Write_dmdSec(TextWriter Output_Stream, abstract_TreeNode MetsDivision, Dictionary<string, object> Options)
    {
      // GEt the geo-spatial information if it exists
      GeoSpatial_Information geoInfo = MetsDivision.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if ((geoInfo == null) || (!geoInfo.hasData))
        return true;

      return Write_Metadata_Section(Output_Stream, geoInfo, Options);
    }

    /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with 
    /// one subsection/division from the METS structure map </summary>
    /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
    /// <param name="MetsDivision"> Division from the overall package into which to read the metadata</param>
    /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
    /// <returns> TRUE if successful, otherwise FALSE</returns>
    public bool Read_dmdSec(XmlReader Input_XmlReader, abstract_TreeNode MetsDivision, Dictionary<string, object> Options)
    {
      // Get the geo-spatial information if it exists or create a new one
      GeoSpatial_Information geoInfo = MetsDivision.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if (geoInfo == null)
      {
        geoInfo = new GeoSpatial_Information();
        MetsDivision.Add_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY, geoInfo);
      }

      return Read_Metadata_Section(Input_XmlReader, geoInfo, Options);
    }

    /// <summary> Flag indicates if this active reader/writer will write a dmdSec for this node </summary>
    /// <param name="MetsDivision"> Division to check if a dmdSec will be written </param>
    /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
    /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
    public bool Include_dmdSec(abstract_TreeNode MetsDivision, Dictionary<string, object> Options)
    {
      // GEt the geo-spatial information if it exists
      GeoSpatial_Information geoInfo = MetsDivision.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if ((geoInfo == null) || (!geoInfo.hasData))
        return false;
      return true;
    }

    /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
    /// to the METS XML header by analyzing the contents of the division </summary>
    /// <param name="MetsDivision"> Division from the overall package into which to read the metadata</param>
    /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
    public bool Schema_Reference_Required_Division(abstract_TreeNode MetsDivision)
    {
      // GEt the geo-spatial information if it exists
      GeoSpatial_Information geoInfo = MetsDivision.Get_Metadata_Module(GlobalVar.GEOSPATIAL_METADATA_MODULE_KEY) as GeoSpatial_Information;
      if ((geoInfo == null) || (!geoInfo.hasData))
        return false;
      return true;
    }

    #endregion

    #region Methods shared by iPackage_dmdSec_ReaderWriter and iDivision_dmdSec_ReaderWriter

    /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
    /// <param name="METS_Item"> Package with all the metadata to save</param>
    /// <returns> Formatted schema namespace info for the METS header</returns>
    public string[] Schema_Namespace(SobekCM_Item METS_Item)
    {
      return new string[] { "gml=\"http://www.opengis.net/gml\"" };
    }

    /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
    /// <param name="METS_Item"> Package with all the metadata to save</param>
    /// <returns> Formatted schema location for the METS header</returns>
    public string[] Schema_Location(SobekCM_Item METS_Item)
    {
      return new string[] { };
    }

    #endregion

    #region Code to read the GML metadata section

    private bool Read_Metadata_Section(XmlReader Input_XmlReader, GeoSpatial_Information geoInfo, Dictionary<string, object> Options)
    {
      do // Loop through reading each XML node
      {
        // get the right division information based on node type
        if (Input_XmlReader.NodeType == XmlNodeType.Element) //if it is an element
        {
          switch (Input_XmlReader.Name) //get name of
          {
            case "gml:Point": //is a point
              // Read the feature label
              string pointLabel = String.Empty;
              if (Input_XmlReader.MoveToAttribute("label"))
                pointLabel = Input_XmlReader.Value;
              do
              {
                if (Input_XmlReader.NodeType == XmlNodeType.EndElement && Input_XmlReader.Name == "gml:Point") //check to see if end of element
                  break;
                if (Input_XmlReader.NodeType == XmlNodeType.Element) //if it is an element
                {
                  switch (Input_XmlReader.Name) //determine the name of that element
                  {
                    case "gml:Coordinates": //if it is the coordinates
                      Input_XmlReader.Read();
                      if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                      {
                        string result = Convert.ToString(Input_XmlReader.Value);
                        var items = result.Split(',');
                        double latitude = double.Parse(items[0]);
                        double longitude = double.Parse(items[1]);
                        geoInfo.Add_Point(latitude, longitude, pointLabel); //add to obj
                      }
                      break;
                  }
                }
              } while (Input_XmlReader.Read());
              break;

            case "gml:Line": //is a line
              // Read the feature label
              string lineLabel = String.Empty;
              if (Input_XmlReader.MoveToAttribute("label"))
                lineLabel = Input_XmlReader.Value;
              do
              {
                if (Input_XmlReader.NodeType == XmlNodeType.EndElement && Input_XmlReader.Name == "gml:Line") //check to see if end of element
                  break;
                if (Input_XmlReader.NodeType == XmlNodeType.Element) //if it is an element
                {
                  switch (Input_XmlReader.Name) //determine the name of that element
                  {
                    case "gml:Coordinates": //if it is the coordinates
                      Input_XmlReader.Read(); //2do: parse out lat and long
                      if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                      {
                        // Parse the string into a collection of doubles, which represents lats AND longs
                        List<double> latLongs = new List<double>();
                        string rValue = Input_XmlReader.Value + ' ';
                        StringBuilder coordinatePointBuilder = new StringBuilder();
                        for (int iterator = 0; iterator < rValue.Length; iterator++)
                        {
                          char rValueChar = rValue[iterator];
                          if ((Char.IsNumber(rValueChar)) || (rValueChar == '.') || (rValueChar == '-'))
                          {
                            coordinatePointBuilder.Append(rValueChar);
                          }
                          else
                          {
                            if (coordinatePointBuilder.Length > 0)
                            {
                              latLongs.Add(double.Parse(coordinatePointBuilder.ToString()));
                              coordinatePointBuilder.Remove(0, coordinatePointBuilder.Length);
                            }
                          }
                        }

                        // In pairs, assign new points to the line and add the line to the coordinate/item
                        Coordinate_Line newline = new Coordinate_Line();
                        int i = 0;
                        while ((i + 2) <= latLongs.Count)
                        {
                          string lineName = "line";
                          lineName += i;
                          newline.Add_Point(latLongs[i], latLongs[i + 1], lineName);
                          i += 2;
                        }
                        newline.Label = lineLabel;
                        geoInfo.Add_Line(newline);
                      }
                      break;
                  }
                }
              } while (Input_XmlReader.Read());
              break;

            case "gml:Polygon": //is polygon
              // Read the feature label
              string polygonLabel = String.Empty;
              double polygonRotation = 0;
              double circleRadius = 0;
              if (Input_XmlReader.MoveToAttribute("label"))
                polygonLabel = Input_XmlReader.Value;
              if (Input_XmlReader.MoveToAttribute("rotation"))
                polygonRotation = Convert.ToDouble(Input_XmlReader.Value);
              if (Input_XmlReader.MoveToAttribute("radius"))
                circleRadius = Convert.ToDouble(Input_XmlReader.Value);
              do
              {
                if (Input_XmlReader.NodeType == XmlNodeType.EndElement && Input_XmlReader.Name == "gml:Polygon") //check to see if end of element
                  break;
                if (Input_XmlReader.NodeType == XmlNodeType.Element) //if it is an element
                {
                  switch (Input_XmlReader.Name) //determine the name of that element
                  {
                    case "gml:Coordinates": //if it is the coordinates
                      Input_XmlReader.Read();
                      if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                      {
                        // Parse the string into a collection of doubles, which represents lats AND longs
                        List<double> latLongs = new List<double>();
                        string rValue = Input_XmlReader.Value + ' ';
                        StringBuilder coordinatePointBuilder = new StringBuilder();
                        for (int iterator = 0; iterator < rValue.Length; iterator++)
                        {
                          char rValueChar = rValue[iterator];
                          if ((Char.IsNumber(rValueChar)) || (rValueChar == '.') || (rValueChar == '-'))
                          {
                            coordinatePointBuilder.Append(rValueChar);
                          }
                          else
                          {
                            if (coordinatePointBuilder.Length > 0)
                            {
                              latLongs.Add(double.Parse(coordinatePointBuilder.ToString()));
                              coordinatePointBuilder.Remove(0, coordinatePointBuilder.Length);
                            }
                          }
                        }

                        // In pairs, assign new points to the polygon and add the polygon to the coordinate/item
                        Coordinate_Polygon newPoly = new Coordinate_Polygon();
                        int i = 0;
                        while ((i + 2) <= latLongs.Count)
                        {
                          newPoly.Add_Edge_Point(latLongs[i], latLongs[i + 1]);
                          i += 2;
                        }
                        newPoly.Label = polygonLabel;
                        newPoly.polygonRotation = polygonRotation;
                        geoInfo.Add_Polygon(newPoly);
                      }
                      break;
                  }
                }
              } while (Input_XmlReader.Read());
              break;
          }
        }
      } while (Input_XmlReader.Read());

      return true;
    }

    #endregion

    #region Code to write the GML metadata section

    private bool Write_Metadata_Section(TextWriter Output_Stream, GeoSpatial_Information geoInfo, Dictionary<string, object> Options)
    {

      Output_Stream.WriteLine("<gml:FeatureCollection>");

      //for points
      foreach (Coordinate_Point thisPoint in geoInfo.Points)
      {
        Output_Stream.WriteLine("<gml:featureMember>");
        if (thisPoint.Label.Length > 0)
          Output_Stream.WriteLine("<gml:Point label=\"" + Convert_String_To_XML_Safe(thisPoint.Label) + "\">");
        else
          Output_Stream.WriteLine("<gml:Point>");
        Output_Stream.Write("<gml:Coordinates>");
        Output_Stream.Write(thisPoint.Latitude + "," + thisPoint.Longitude + " ");
        Output_Stream.WriteLine("</gml:Coordinates>");
        Output_Stream.WriteLine("</gml:Point>");
        Output_Stream.WriteLine("</gml:featureMember>");
      }

      //for lines
      foreach (Coordinate_Line thisLine in geoInfo.Lines)
      {
        Output_Stream.WriteLine("<gml:featureMember>");
        if (thisLine.Label.Length > 0)
          Output_Stream.WriteLine("<gml:Line label=\"" + Convert_String_To_XML_Safe(thisLine.Label) + "\">");
        else
          Output_Stream.WriteLine("<gml:Line>");
        Output_Stream.Write("<gml:Coordinates>");
        foreach (Coordinate_Point thisPoint in thisLine.Points) //for each lat/long
        {
          Output_Stream.Write(thisPoint.Latitude + "," + thisPoint.Longitude + " ");
        }
        Output_Stream.WriteLine("</gml:Coordinates>");
        Output_Stream.WriteLine("</gml:Line>");
        Output_Stream.WriteLine("</gml:featureMember>");
      }

      //for polygons
      foreach (Coordinate_Polygon thisPolygon in geoInfo.Polygons) //for each polygon with coordinates
      {
        Output_Stream.WriteLine("<gml:featureMember>");
        if (thisPolygon.Label.Length > 0)
          Output_Stream.WriteLine("<gml:Polygon label=\"" + Convert_String_To_XML_Safe(thisPolygon.Label) + "\" rotation=\"" + thisPolygon.polygonRotation + "\" radius=\"" + thisPolygon.circleRadius + "\">");
        else
          Output_Stream.WriteLine("<gml:Polygon rotation=\"" + thisPolygon.polygonRotation + "\" radius=\"" + thisPolygon.circleRadius + "\">");
        Output_Stream.WriteLine("<gml:exterior>");
        Output_Stream.WriteLine("<gml:LinearRing>");
        Output_Stream.Write("<gml:Coordinates>");
        foreach (Coordinate_Point thisPoint in thisPolygon.Edge_Points) //for each lat/long
        {
          Output_Stream.Write(thisPoint.Latitude + "," + thisPoint.Longitude + " ");
        }
        Output_Stream.WriteLine("</gml:Coordinates>");
        Output_Stream.WriteLine("</gml:LinearRing>");
        Output_Stream.WriteLine("</gml:exterior>");
        Output_Stream.WriteLine("</gml:Polygon>");
        Output_Stream.WriteLine("</gml:featureMember>");
      }

      Output_Stream.WriteLine("</gml:FeatureCollection>");

      return true;
    }

    #endregion
  }
}