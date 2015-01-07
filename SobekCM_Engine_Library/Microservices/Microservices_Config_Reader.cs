using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> Static class is used to read the configuration file defining microservice endpoints </summary>
    public static class Microservices_Config_Reader
    {
        /// <summary> Static class is used to read the configuration file defining microservice endpoints </summary>
        /// <param name="ConfigFile"> Path and name of the configuration XML file to read </param>
        /// <returns> Fully configured microservices configuration object </returns>
        public static Microservices_Configuration Read_Config(string ConfigFile)
        {
            Microservices_Configuration returnValue = new Microservices_Configuration();

            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            try
            {
                // Open a link to the file
                readerStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Read);

                // Open a XML reader connected to the file
                readerXml = new XmlTextReader(readerStream);

                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        switch (readerXml.Name.ToLower())
                        {
                            case "microservices":
                                read_microservices_details(readerXml.ReadSubtree(), returnValue);
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                returnValue.Error = ee.Message;
            }
            finally
            {
                if (readerXml != null)
                {
                    readerXml.Close();
                }
                if (readerStream != null)
                {
                    readerStream.Close();
                }
            }

            return returnValue;
        }

        private static void read_microservices_details(XmlReader readerXml, Microservices_Configuration config)
        {
            // Collections used to hold the objects read from the XML before they are all connected (due to references 
            // within the XML file to other portions )
            Dictionary<string, Microservice_Component> components = new Dictionary<string, Microservice_Component>();
            Dictionary<string, Microservice_RestrictionRange> ranges = new Dictionary<string, Microservice_RestrictionRange>();
            List<Microservice_Endpoint> allEndpoints = new List<Microservice_Endpoint>();
            Dictionary<Microservice_Endpoint, string> endpointToComponentDictionary = new Dictionary<Microservice_Endpoint, string>();
            Dictionary<Microservice_Endpoint, string> endpointToRestrictionDictionary = new Dictionary<Microservice_Endpoint, string>();

            // Just step through the subtree of this
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "mapping":
                            read_microservices_details_mapping(readerXml.ReadSubtree(), config, allEndpoints, endpointToComponentDictionary, endpointToRestrictionDictionary, null);
                            break;

                        case "components":
                            read_microservices_details_components(readerXml.ReadSubtree(), config, components);
                            break;

                        case "restrictionranges":
                            read_microservices_details_restrictionranges(readerXml.ReadSubtree(), config, ranges);
                            break;
                    }
                }
            }

            // Now that everything has been read here, connect the objects together
            foreach (Microservice_Endpoint endpoint in allEndpoints)
            {
                // Connect the component to the endpoint
                string componentid = endpointToComponentDictionary[endpoint];
                if (components.ContainsKey(componentid))
                {
                    endpoint.Component = components[componentid];
                }
                
                // Connect the restriction ranges to the endpoints, if not already done
                if (endpointToRestrictionDictionary.ContainsKey(endpoint))
                {
                    string restrictionid = endpointToRestrictionDictionary[endpoint];
                    string[] restrictions = restrictionid.Split(" ".ToCharArray());
                    foreach (string thisRestriction in restrictions)
                    {
                        if (ranges.ContainsKey(thisRestriction))
                        {
                            Microservice_RestrictionRange rangeObj = ranges[thisRestriction];
                            if ( endpoint.RestrictionRanges == null )
                                endpoint.RestrictionRanges = new List<Microservice_RestrictionRange>();
                            endpoint.RestrictionRanges.Add(rangeObj);
                        }
                    }

                }
            }
        }

        private static void read_microservices_details_mapping(XmlReader readerXml, Microservices_Configuration config, List<Microservice_Endpoint> allEndpoints, Dictionary<Microservice_Endpoint, string> endpointToComponentDictionary, Dictionary<Microservice_Endpoint, string> endpointToRestrictionDictionary, Microservice_Path parentSegment )
        {
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "path":
                            Microservice_Path path = new Microservice_Path();
                            if (readerXml.MoveToAttribute("Segment"))
                            {
                                path.Segment = readerXml.Value.Trim();

                                if (parentSegment == null)
                                    config.RootPaths[path.Segment] = path;
                                else
                                {
                                    if ( parentSegment.Children == null )
                                        parentSegment.Children = new Dictionary<string, Microservice_Path>();
                                    parentSegment.Children[path.Segment] = path;
                                }

                                readerXml.MoveToElement();
                                XmlReader subTreeReader = readerXml.ReadSubtree();
                                subTreeReader.Read();
                                read_microservices_details_mapping(subTreeReader, config, allEndpoints, endpointToComponentDictionary, endpointToRestrictionDictionary, path);
                            }
                            break;

                        case "endpoint":
                            Microservice_Endpoint endpoint = new Microservice_Endpoint();
                            string componentid = String.Empty;
                            string restrictionid = String.Empty;
                            if (readerXml.MoveToAttribute("Segment"))
                                endpoint.Segment = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("ComponentID"))
                                componentid = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Method"))
                                endpoint.Method = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Enabled"))
                            {
                                if (String.Compare(readerXml.Value.Trim(), "false", true) == 0)
                                    endpoint.Enabled = false;
                            }
                            if (readerXml.MoveToAttribute("RestrictionRangeID"))
                                restrictionid = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Protocol"))
                            {
                                if (String.Compare(readerXml.Value.Trim(), "PROTOBUF", true) == 0)
                                    endpoint.Protocol = Microservice_Endpoint_Protocol_Enum.PROTOBUF;
                                else
                                    endpoint.Protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                            }

                            readerXml.MoveToElement();
                            read_microservices_details_endpoint(readerXml.ReadSubtree(), endpoint);

                            if ((componentid.Length > 0) && (endpoint.Segment.Length > 0) && (endpoint.Method.Length > 0))
                            {
                                if (parentSegment != null)
                                {
                                    if (parentSegment.Children == null)
                                        parentSegment.Children = new Dictionary<string, Microservice_Path>();
                                    parentSegment.Children[endpoint.Segment] = endpoint;
                                    allEndpoints.Add(endpoint);
                                    endpointToComponentDictionary[endpoint] = componentid;
                                    endpointToRestrictionDictionary[endpoint] = restrictionid;
                                }
                            }
                            break;
                    }
                }
            }
        }

        private static void read_microservices_details_endpoint(XmlReader readerXml, Microservice_Endpoint endpoint)
        {
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "description":
                            readerXml.Read();
                            endpoint.Description = readerXml.Value.Trim();
                            break;

                        case "requesttype":
                            readerXml.Read();
                            string requesttype = readerXml.Value.Trim();
                            switch (requesttype.ToLower())
                            {
                                case "get":
                                    endpoint.RequestType = Microservice_Endpoint_RequestType_Enum.GET;
                                    break;

                                case "post":
                                    endpoint.RequestType = Microservice_Endpoint_RequestType_Enum.POST;
                                    break;
                            }
                            break;

                        case "arguments":
                            readerXml.Read();
                            endpoint.Arguments = readerXml.Value.Trim();
                            break;

                        case "returns":
                            readerXml.Read();
                            endpoint.Returns = readerXml.Value.Trim();
                            break;
                    }
                }
            }
        }

        private static void read_microservices_details_components(XmlReader readerXml, Microservices_Configuration config, Dictionary<string, Microservice_Component> components)
        {
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "component":
                            Microservice_Component component = new Microservice_Component();
                            if (readerXml.MoveToAttribute("ID"))
                                component.ID = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Assembly"))
                                component.Assembly = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Namespace"))
                                component.Namespace = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Class"))
                                component.Class = readerXml.Value.Trim();
                            if ((!String.IsNullOrEmpty(component.ID)) && (!String.IsNullOrEmpty(component.Class)) && (!String.IsNullOrEmpty(component.Namespace)))
                            {
                                components[component.ID] = component;
                                config.Components.Add(component);
                            }
                            break;
                    }
                }
            }
        }

        private static void read_microservices_details_restrictionranges(XmlReader readerXml, Microservices_Configuration config, Dictionary<string, Microservice_RestrictionRange> ranges)
        {
            Microservice_RestrictionRange range = null;

            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "range":
                            range = new Microservice_RestrictionRange();

                            if (readerXml.MoveToAttribute("ID"))
                                range.ID = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Label"))
                                range.Label = readerXml.Value.Trim();
                            break;

                        case "iprange":
                            Microservice_IpRange singleIpRange = new Microservice_IpRange();
                            if (readerXml.MoveToAttribute("Label"))
                                singleIpRange.Label = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("Start"))
                                singleIpRange.StartIp = readerXml.Value.Trim();
                            if (readerXml.MoveToAttribute("End"))
                                singleIpRange.EndIp = readerXml.Value.Trim();
                            if ( singleIpRange.StartIp.Length > 0 )
                                range.IpRanges.Add(singleIpRange);
                            break;
                    }

                }
                else if (readerXml.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Compare(readerXml.Name, "range", true) == 0)
                    {
                        if ((range != null) && (!String.IsNullOrEmpty(range.ID)))
                        {
                            ranges[range.ID] = range;
                            config.RestrictionRanges.Add(range);
                        }
                        range = null;
                    }
                }
            }
        }
    }
}
