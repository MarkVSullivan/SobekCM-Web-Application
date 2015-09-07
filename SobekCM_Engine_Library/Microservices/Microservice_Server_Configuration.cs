#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> Overall configuration for the microservices exposed by this data layer </summary>
    public class Microservice_Server_Configuration
    {
        /// <summary> Get the endpoint configuration, based on the requested path </summary>
        /// <param name="Paths"> Requested URL paths </param>
        /// <returns> Matched enpoint configuration, otherwise NULL </returns>
        public Microservice_Endpoint Get_Endpoint(List<string> Paths)
        {
            if (RootPaths.ContainsKey(Paths[0]))
            {
                Microservice_Path path = RootPaths[Paths[0]];
                Paths.RemoveAt(0);

                do
                {
                    // Did we find an endpoint?
                    if (path.IsEndpoint)
                    {
                        return (Microservice_Endpoint) path;
                    }

                    // Look to the next part of the path
                    if (Paths.Count > 0)
                    {
                        if (!path.Children.ContainsKey(Paths[0]))
                        {
                            return null;
                        }
                        
                        path = path.Children[Paths[0]];
                        Paths.RemoveAt(0);
                    }
                    else
                    {
                        return null;
                    }

                } while ( true );
            }
  
            return null;
        }


        /// <summary> Constructor for a new instance of the Microservices_Config class </summary>
        public Microservice_Server_Configuration()
        {
            RootPaths = new Dictionary<string, Microservice_Path>();
            Components = new List<Microservice_Component>();
            RestrictionRanges = new List<Microservice_RestrictionRange>();
        }

        /// <summary> Collection of all the root paths/endpoints (defined hierarchically) </summary>
        public Dictionary<string, Microservice_Path> RootPaths { get; private set; }

        /// <summary> List of all the components specified in the configuration file </summary>
        public List<Microservice_Component> Components { get; private set; }

        /// <summary> List of all the possible restriction ranges ( each defined by multiple 
        /// possible IP address ranges ) in the configuration file </summary>
        public List<Microservice_RestrictionRange> RestrictionRanges { get; private set; }

        /// <summary> Any error associated with reading the configuration file into this object </summary>
        public string Error { get; internal set; }

        /// <summary> Clears all of the data loaded into this configuration </summary>
        public void ClearAll()
        {
            RootPaths.Clear();
            Components.Clear();
            RestrictionRanges.Clear();
            Error = null;
        }


        #region Code to save this configuration to a XML file

        ///// <summary> Save this quality control configuration to a XML config file </summary>
        ///// <param name="FilePath"> File/path for the resulting XML config file </param>
        ///// <returns> TRUE if successful, otherwise FALSE </returns>
        //public bool Save_To_Config_File(string FilePath)
        //{
        //    bool returnValue = true;
        //    StreamWriter writer = null;
        //    try
        //    {
        //        // Start the output file
        //        writer = new StreamWriter(FilePath, false, Encoding.UTF8);
        //        writer.WriteLine("<?xml version=\"1.0\" encoding=\"utf-8\"?>");
        //        writer.WriteLine("<Config>");
        //        writer.WriteLine("\t<Engine>");

        //        // Add the mapping
        //        if ((RootPaths != null) && (RootPaths.Count > 0))
        //        {
        //            writer.WriteLine("\t\t<Mapping>");

        //            foreach ( KeyValuePair<string, Microservice_Path> path in RootPaths)
        //            {
        //                if ((path.Value.Children == null) || (path.Value.Children.Count == 0))
        //                {
        //                    writer.WriteLine("\t\t\t<Path Segment=\"" + path.Value.Segment + "\" />");
        //                }
        //                else
        //                {
        //                    writer.WriteLine("\t\t\t<Path Segment=\"" + path.Value.Segment + "\">");
        //                    foreach (KeyValuePair<string, Microservice_Path> childPath in path.Value.Children)
        //                    {
        //                        add_path_endpoint(writer, "\t\t\t\t", childPath.Key, childPath.Value);
        //                    }
        //                    writer.WriteLine("\t\t\t</Path>");
        //                }
        //            }

        //            writer.WriteLine("\t\t</Mapping>");
        //        }

        //        // Add the components
        //        if ((Components != null) && (Components.Count > 0))
        //        {
        //            writer.WriteLine("\t\t<Components>");

        //            foreach ( Microservice_Component component in Components )
        //            {
        //                writer.Write("\t\t\t<Component ID=\"" + component.ID + "\"");
        //                if ( !String.IsNullOrEmpty(component.Assembly))
        //                    writer.Write(" Assembly=\"" + component.Assembly + "\"");
        //                writer.Write(" Namespace=\"" + component.Namespace + "\"");
        //                writer.Write(" Class=\"" + component.Class + "\"");
        //                writer.WriteLine(" />");
        //            }

        //            writer.WriteLine("\t\t</Components>");
        //        }

        //        // Add the logic portions
        //        if ((RestrictionRanges != null) && (RestrictionRanges.Count > 0))
        //        {
        //            writer.WriteLine("\t\t<RestrictionRanges>");

        //            foreach ( Microservice_RestrictionRange range in RestrictionRanges)
        //            {
        //                if ((range.IpRanges != null) && (range.IpRanges.Count > 0))
        //                {
        //                    writer.Write("\t\t\t<Range ID=\"" + range.ID + "\"");
        //                    if (!String.IsNullOrEmpty(range.Label))
        //                        writer.Write(" Label=\"" + range.Label + "\"");
        //                    writer.WriteLine(">");

        //                    foreach (Microservice_IpRange ipRange in range.IpRanges)
        //                    {
        //                        writer.Write("\t\t\t\t<IpRange");
        //                        if (!String.IsNullOrEmpty(ipRange.Label))
        //                            writer.Write(" Label=\"" + ipRange.Label + "\"");
        //                        writer.Write(" Start=\"" + ipRange.StartIp + "\"");
        //                        if (!String.IsNullOrEmpty(ipRange.EndIp))
        //                            writer.Write(" End=\"" + ipRange.EndIp + "\"");
        //                        writer.WriteLine(" />");
        //                    }

        //                    writer.WriteLine("\t\t\t</Range>");
        //                }
        //            }

        //            writer.WriteLine("\t\t</RestrictionRanges>");
        //        }

        //        writer.WriteLine("\t</Microservices>");
        //        writer.WriteLine("</Config>");
        //        writer.Flush();
        //        writer.Close();
        //    }
        //    catch 
        //    {
        //        returnValue = false;
        //    }
        //    finally
        //    {
        //        if (writer != null)
        //            writer.Close();
        //    }

        //    return returnValue;
        //}


        //private void add_path_endpoint(StreamWriter writer, string indent, string Segment, Microservice_Path pathOrEndpoint)
        //{
        //    if (pathOrEndpoint.IsEndpoint)
        //    {
        //        Microservice_Endpoint endpoint = (Microservice_Endpoint) pathOrEndpoint;
        //        if ((endpoint.Component != null) && (!String.IsNullOrEmpty(endpoint.Segment)) && (!String.IsNullOrEmpty(endpoint.Method)))
        //        {
        //            writer.Write(indent + "<Endpoint Segment=\"" + endpoint.Segment + "\" ComponentID=\"" + endpoint.Component.ID + "\" Method=\"" + endpoint.Method + "\" Enabled=\"" + endpoint.Enabled.ToString().ToLower() + "\"");
        //            if ( endpoint.Protocol == Microservice_Endpoint_Protocol_Enum.JSON )
        //                writer.Write(" Protocol=\"JSON\"");
        //            else
        //                writer.Write(" Protocol=\"PROTOBUF\"");

        //            if (endpoint.RequestType == Microservice_Endpoint_RequestType_Enum.GET)
        //                writer.Write(" RequestType=\"GET\"");
        //            else
        //                writer.Write(" RequestType=\"POST\"");

        //            if ((endpoint.RestrictionRanges != null) && (endpoint.RestrictionRanges.Count > 0))
        //            {
        //                writer.Write(" RestrictionRangeID=\"");
        //                bool first = true;
        //                foreach (Microservice_RestrictionRange range in endpoint.RestrictionRanges)
        //                {
        //                    if (first)
        //                    {
        //                        first = false;
        //                        writer.Write(range.ID);
        //                    }
        //                    else
        //                    {
        //                        writer.Write(" " + range.ID);
        //                    }
        //                }
        //                writer.Write("\"");
        //            }

        //            if ((String.IsNullOrEmpty(endpoint.Description)) && (String.IsNullOrEmpty(endpoint.Arguments)) && (String.IsNullOrEmpty(endpoint.Returns)) && (endpoint.RequestType == Microservice_Endpoint_RequestType_Enum.GET ))
        //                writer.WriteLine(" />");
        //            else
        //            {
        //                writer.WriteLine(">");
        //                if (!String.IsNullOrEmpty(endpoint.Description))
        //                    writer.WriteLine(indent + "\t<Description>" + Convert_String_To_XML_Safe(endpoint.Description) + "</Description>");
        //                if (!String.IsNullOrEmpty(endpoint.Arguments))
        //                    writer.WriteLine(indent + "\t<Arguments>" + Convert_String_To_XML_Safe(endpoint.Arguments) + "</Arguments>");
        //                if (!String.IsNullOrEmpty(endpoint.Returns))
        //                    writer.WriteLine(indent + "\t<Returns>" + Convert_String_To_XML_Safe(endpoint.Returns) + "</Returns>");

        //                writer.WriteLine(indent + "</Endpoint>");
        //            }
        //        }
        //    }
        //    else
        //    {
        //        if ((pathOrEndpoint.Children == null) || (pathOrEndpoint.Children.Count == 0))
        //        {
        //            writer.WriteLine(indent + "<Path Segment=\"" + Segment + "\" />");
        //        }
        //        else
        //        {
        //            writer.WriteLine(indent + "<Path Segment=\"" + Segment + "\">");
        //            foreach (KeyValuePair<string, Microservice_Path> childPath in pathOrEndpoint.Children)
        //            {
        //                add_path_endpoint(writer, indent + "\t", childPath.Key, childPath.Value);
        //            }
        //            writer.WriteLine(indent + "</Path>");
        //        }
        //    }
        //}

        ///// <summary> Converts a basic string into an XML-safe string </summary>
        ///// <param name="element"> Element data to convert </param>
        ///// <returns> Data converted into an XML-safe string</returns>
        //private static string Convert_String_To_XML_Safe(string element)
        //{
        //    if (element == null)
        //        return string.Empty;

        //    string xml_safe = element;
        //    int i = xml_safe.IndexOf("&");
        //    while (i >= 0)
        //    {
        //        if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
        //            (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
        //        {
        //            xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
        //        }

        //        i = xml_safe.IndexOf("&", i + 1);
        //    }
        //    return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        //}

        #endregion
    }
}