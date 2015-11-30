#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

#endregion

namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> Static class is used to read the configuration file defining microservice endpoints </summary>
    public static class Microservice_Server_Config_Reader
    {
        /// <summary> Static class is used to read the configuration file defining microservice endpoints </summary>
        /// <param name="ConfigFile"> Path and name of the configuration XML file to read </param>
        /// <returns> Fully configured microservices configuration object </returns>
        public static Microservice_Server_Configuration Read_Config(string ConfigFile)
        {
            Microservice_Server_Configuration returnValue = new Microservice_Server_Configuration();

            // Collections used to hold the objects read from the XML before they are all connected (due to references 
            // within the XML file to other portions )
            Dictionary<string, Microservice_Component> components = new Dictionary<string, Microservice_Component>();
            Dictionary<string, Microservice_RestrictionRange> ranges = new Dictionary<string, Microservice_RestrictionRange>();
            List<Microservice_Endpoint> allEndpoints = new List<Microservice_Endpoint>();
            Dictionary<Microservice_VerbMapping, string> endpointToComponentDictionary = new Dictionary<Microservice_VerbMapping, string>();
            Dictionary<Microservice_VerbMapping, string> endpointToRestrictionDictionary = new Dictionary<Microservice_VerbMapping, string>();

            // Read this file
            read_engine_file(ConfigFile, returnValue, allEndpoints, endpointToComponentDictionary, endpointToRestrictionDictionary, components, ranges);

            // Now that everything has been read here, connect the objects together
            foreach (Microservice_Endpoint endpoint in allEndpoints)
            {
                // Step through each applicable verb --> method mapping ( i.e., GET, POST, PUT, and DELETE )
                foreach (Microservice_VerbMapping verbmapping in endpoint.AllVerbMappings)
                {
                    // Connect the component to the endpoint
                    string componentid = endpointToComponentDictionary[verbmapping];
                    if (components.ContainsKey(componentid))
                    {
                        verbmapping.Component = components[componentid];
                    }

                    // Connect the restriction ranges to the endpoints, if not already done
                    if (endpointToRestrictionDictionary.ContainsKey(verbmapping))
                    {
                        string restrictionid = endpointToRestrictionDictionary[verbmapping];
                        string[] restrictions = restrictionid.Split(" ".ToCharArray());
                        foreach (string thisRestriction in restrictions)
                        {
                            if (ranges.ContainsKey(thisRestriction))
                            {
                                Microservice_RestrictionRange rangeObj = ranges[thisRestriction];
                                if (verbmapping.RestrictionRanges == null)
                                    verbmapping.RestrictionRanges = new List<Microservice_RestrictionRange>();
                                verbmapping.RestrictionRanges.Add(rangeObj);
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        /// <summary> Static class is used to read the configuration file defining microservice endpoints </summary>
        /// <param name="ConfigFiles"> Path and name of the configuration XML file to read </param>
        /// <returns> Fully configured microservices configuration object </returns>
        public static Microservice_Server_Configuration Read_Config(string[] ConfigFiles)
        {
            Microservice_Server_Configuration returnValue = new Microservice_Server_Configuration();

            // Collections used to hold the objects read from the XML before they are all connected (due to references 
            // within the XML file to other portions )
            Dictionary<string, Microservice_Component> components = new Dictionary<string, Microservice_Component>();
            Dictionary<string, Microservice_RestrictionRange> ranges = new Dictionary<string, Microservice_RestrictionRange>();
            List<Microservice_Endpoint> allEndpoints = new List<Microservice_Endpoint>();
            Dictionary<Microservice_VerbMapping, string> endpointToComponentDictionary = new Dictionary<Microservice_VerbMapping, string>();
            Dictionary<Microservice_VerbMapping, string> endpointToRestrictionDictionary = new Dictionary<Microservice_VerbMapping, string>();

            // Read these files
            foreach (string configFile in ConfigFiles)
            {
                read_engine_file(configFile, returnValue, allEndpoints, endpointToComponentDictionary, endpointToRestrictionDictionary, components, ranges);
            }

            // Now that everything has been read here, connect the objects together
            foreach (Microservice_Endpoint endpoint in allEndpoints)
            {
                // Step through each applicable verb --> method mapping ( i.e., GET, POST, PUT, and DELETE )
                foreach (Microservice_VerbMapping verbmapping in endpoint.AllVerbMappings)
                {
                    // Connect the component to the endpoint
                    string componentid = endpointToComponentDictionary[verbmapping];
                    if (components.ContainsKey(componentid))
                    {
                        verbmapping.Component = components[componentid];
                    }

                    // Connect the restriction ranges to the endpoints, if not already done
                    if (endpointToRestrictionDictionary.ContainsKey(verbmapping))
                    {
                        string restrictionid = endpointToRestrictionDictionary[verbmapping];
                        string[] restrictions = restrictionid.Split(" ".ToCharArray());
                        foreach (string thisRestriction in restrictions)
                        {
                            if (ranges.ContainsKey(thisRestriction))
                            {
                                Microservice_RestrictionRange rangeObj = ranges[thisRestriction];
                                if (verbmapping.RestrictionRanges == null)
                                    verbmapping.RestrictionRanges = new List<Microservice_RestrictionRange>();
                                verbmapping.RestrictionRanges.Add(rangeObj);
                            }
                        }
                    }
                }
            }

            return returnValue;
        }

        private static void read_engine_file(string ConfigFile, Microservice_Server_Configuration Config, List<Microservice_Endpoint> AllEndpoints, Dictionary<Microservice_VerbMapping, string> EndpointToComponentDictionary, Dictionary<Microservice_VerbMapping, string> EndpointToRestrictionDictionary, Dictionary<string, Microservice_Component> Components, Dictionary<string, Microservice_RestrictionRange> Ranges)
        {
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
                            case "engine":
                                if (readerXml.MoveToAttribute("ClearAll"))
                                {
                                    if (readerXml.Value.Trim().ToLower() == "true")
                                    {
                                        Config.ClearAll();
                                        AllEndpoints.Clear();
                                        Components.Clear();
                                        Ranges.Clear();
                                        EndpointToComponentDictionary.Clear();
                                        EndpointToRestrictionDictionary.Clear();
                                    }
                                    readerXml.MoveToElement();
                                }
                                read_engine_details(readerXml.ReadSubtree(), Config, AllEndpoints, EndpointToComponentDictionary, EndpointToRestrictionDictionary, Components, Ranges);
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Config.Error = ee.Message;
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
        }

        private static void read_engine_details(XmlReader ReaderXml, Microservice_Server_Configuration Config, List<Microservice_Endpoint> AllEndpoints, Dictionary<Microservice_VerbMapping, string> EndpointToComponentDictionary, Dictionary<Microservice_VerbMapping, string> EndpointToRestrictionDictionary, Dictionary<string, Microservice_Component> Components, Dictionary<string, Microservice_RestrictionRange> Ranges)
        {
            // Just step through the subtree of this
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "mapping":
                            read_microservices_details_mapping(ReaderXml.ReadSubtree(), Config, AllEndpoints, EndpointToComponentDictionary, EndpointToRestrictionDictionary, null);
                            break;

                        case "components":
                            read_microservices_details_components(ReaderXml.ReadSubtree(), Config, Components);
                            break;

                        case "restrictionranges":
                            read_microservices_details_restrictionranges(ReaderXml.ReadSubtree(), Config, Ranges);
                            break;
                    }
                }
            }
        }

        private static void read_microservices_details_mapping(XmlReader ReaderXml, Microservice_Server_Configuration Config, List<Microservice_Endpoint> AllEndpoints, Dictionary<Microservice_VerbMapping, string> EndpointToComponentDictionary, Dictionary<Microservice_VerbMapping, string> EndpointToRestrictionDictionary, Microservice_Path ParentSegment)
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "removeall":
                            if (ParentSegment != null)
                            {
                                if ( ParentSegment.Children != null )
                                    ParentSegment.Children.Clear();
                            }
                            else
                            {
                                Config.RootPaths.Clear();
                            }
                            break;

                        case "path":
                            if (ReaderXml.MoveToAttribute("Segment"))
                            {
                                Microservice_Path path;
                                string segment = ReaderXml.Value.Trim();

                                if (ParentSegment == null)
                                {
                                    if (Config.RootPaths.ContainsKey(segment.ToLower()))
                                        path = Config.RootPaths[segment.ToLower()];
                                    else
                                    {
                                        path = new Microservice_Path { Segment = segment };
                                        Config.RootPaths[segment.ToLower()] = path;
                                    }
                                }
                                else
                                {
                                    if (ParentSegment.Children == null)
                                        ParentSegment.Children = new Dictionary<string, Microservice_Path>(StringComparer.OrdinalIgnoreCase);

                                    if (ParentSegment.Children.ContainsKey(segment.ToLower()))
                                    {
                                        path = ParentSegment.Children[segment.ToLower()];
                                    }
                                    else
                                    {
                                        path = new Microservice_Path {Segment = segment};
                                        ParentSegment.Children[path.Segment] = path;
                                    }
                                    
                                }

                                ReaderXml.MoveToElement();
                                XmlReader subTreeReader = ReaderXml.ReadSubtree();
                                subTreeReader.Read();
                                read_microservices_details_mapping(subTreeReader, Config, AllEndpoints, EndpointToComponentDictionary, EndpointToRestrictionDictionary, path);
                            }
                            break;

                        case "complexendpoint":

                            

                            // Read the top-endpoint information, before getting to each verb mapping
                            bool disabled_at_top = false;
                            Microservice_Endpoint endpoint = new Microservice_Endpoint();
                            if (ReaderXml.MoveToAttribute("Segment"))
                                endpoint.Segment = ReaderXml.Value.Trim();
                            if ((ReaderXml.MoveToAttribute("Enabled")) && (String.Compare(ReaderXml.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase) == 0))
                                disabled_at_top = true;

                            // Now, read what remains
                            ReaderXml.MoveToElement();
                            XmlReader complexReader = ReaderXml.ReadSubtree();
                            complexReader.Read();
                            read_microservices_complex_endpoint_details(complexReader, EndpointToComponentDictionary, EndpointToRestrictionDictionary, endpoint, disabled_at_top );

                            // If a verb was mapped and there was a valid segment, add this
                            if ((!String.IsNullOrEmpty(endpoint.Segment)) && (endpoint.HasVerbMapping))
                            {
                                if (ParentSegment != null)
                                {
                                    // Add this endpoint
                                    if (ParentSegment.Children == null)
                                        ParentSegment.Children = new Dictionary<string, Microservice_Path>();
                                    ParentSegment.Children[endpoint.Segment] = endpoint;
                                    AllEndpoints.Add(endpoint);
                                }
                            }
                            break;

                        case "endpoint":
                            read_microservices_simple_endpoint_details(ReaderXml, AllEndpoints, EndpointToComponentDictionary, EndpointToRestrictionDictionary, ParentSegment);
                            break;
                    }
                }
            }
        }

        private static void read_microservices_complex_endpoint_details(XmlReader ReaderXml, Dictionary<Microservice_VerbMapping, string> EndpointToComponentDictionary, Dictionary<Microservice_VerbMapping, string> EndpointToRestrictionDictionary, Microservice_Endpoint Endpoint, bool DisabledAtTop )
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    if (String.Compare(ReaderXml.Name, "verbmapping", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        // Ensure verb is indicated first
                        if (ReaderXml.MoveToAttribute("Verb"))
                        {
                            Microservice_Endpoint_RequestType_Enum verb = Microservice_Endpoint_RequestType_Enum.ERROR;
                            switch (ReaderXml.Value.Trim().ToUpper())
                            {
                                case "DELETE":
                                    verb = Microservice_Endpoint_RequestType_Enum.DELETE;
                                    break;

                                case "GET":
                                    verb = Microservice_Endpoint_RequestType_Enum.GET;
                                    break;

                                case "POST":
                                    verb = Microservice_Endpoint_RequestType_Enum.POST;
                                    break;

                                case "PUT":
                                    verb = Microservice_Endpoint_RequestType_Enum.PUT;
                                    break;
                            }

                            // If a valid verb found, continue
                            if (verb != Microservice_Endpoint_RequestType_Enum.ERROR)
                            {
                                // Build the verb mapping
                                Microservice_VerbMapping verbMapping = new Microservice_VerbMapping(null, !DisabledAtTop, Microservice_Endpoint_Protocol_Enum.JSON, verb);
                                if (ReaderXml.MoveToAttribute("Method"))
                                    verbMapping.Method = ReaderXml.Value.Trim();
                                if ((!DisabledAtTop) && (ReaderXml.MoveToAttribute("Enabled")) && (String.Compare(ReaderXml.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase) == 0))
                                    verbMapping.Enabled = false;
                                if (ReaderXml.MoveToAttribute("Protocol"))
                                {
                                    switch (ReaderXml.Value.Trim().ToUpper())
                                    {
                                        case "JSON":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                                            break;

                                        case "JSON-P":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.JSON_P;
                                            break;

                                        case "PROTOBUF":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.PROTOBUF;
                                            break;

                                        case "SOAP":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.SOAP;
                                            break;

                                        case "XML":
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.XML;
                                            break;

                                        default:
                                            verbMapping.Protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                                            break;
                                    }
                                }

                                // Get the mapping to componentid and restriction id
                                string componentid = String.Empty;
                                string restrictionid = String.Empty;
                                if (ReaderXml.MoveToAttribute("ComponentID"))
                                    componentid = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("RestrictionRangeID"))
                                    restrictionid = ReaderXml.Value.Trim();

                                // If valid, add to this endpoint
                                if ((componentid.Length > 0) && ( !String.IsNullOrEmpty(verbMapping.Method)))
                                {
                                    // Add the verb mapping to the right spot
                                    switch (verbMapping.RequestType)
                                    {
                                        case Microservice_Endpoint_RequestType_Enum.DELETE:
                                            Endpoint.DeleteMapping = verbMapping;
                                            break;

                                        case Microservice_Endpoint_RequestType_Enum.GET:
                                            Endpoint.GetMapping = verbMapping;
                                            break;

                                        case Microservice_Endpoint_RequestType_Enum.POST:
                                            Endpoint.PostMapping = verbMapping;
                                            break;

                                        case Microservice_Endpoint_RequestType_Enum.PUT:
                                            Endpoint.PutMapping = verbMapping;
                                            break;

                                    }

                                    // Also save the link to component and restriction id
                                    EndpointToComponentDictionary[verbMapping] = componentid;
                                    EndpointToRestrictionDictionary[verbMapping] = restrictionid;
                                }
                            }
                        }
                    }
                }
            }
        }

        private static void read_microservices_simple_endpoint_details(XmlReader ReaderXml, List<Microservice_Endpoint> AllEndpoints, Dictionary<Microservice_VerbMapping, string> EndpointToComponentDictionary, Dictionary<Microservice_VerbMapping, string> EndpointToRestrictionDictionary, Microservice_Path ParentSegment)
        {
            Microservice_Endpoint endpoint = new Microservice_Endpoint();
            string componentid = String.Empty;
            string restrictionid = String.Empty;
            string method = String.Empty;
            bool enabled = true;
            Microservice_Endpoint_Protocol_Enum protocol = Microservice_Endpoint_Protocol_Enum.JSON;

            if (ReaderXml.MoveToAttribute("Segment"))
                endpoint.Segment = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("ComponentID"))
                componentid = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("Method"))
                method = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("Enabled"))
            {
                if (String.Compare(ReaderXml.Value.Trim(), "false", StringComparison.OrdinalIgnoreCase) == 0)
                    enabled = false;
            }
            if (ReaderXml.MoveToAttribute("RestrictionRangeID"))
                restrictionid = ReaderXml.Value.Trim();
            if (ReaderXml.MoveToAttribute("Protocol"))
            {
                switch (ReaderXml.Value.Trim().ToUpper())
                {
                    case "JSON":
                        protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                        break;

                    case "JSON-P":
                        protocol = Microservice_Endpoint_Protocol_Enum.JSON_P;
                        break;

                    case "PROTOBUF":
                        protocol = Microservice_Endpoint_Protocol_Enum.PROTOBUF;
                        break;

                    case "SOAP":
                        protocol = Microservice_Endpoint_Protocol_Enum.SOAP;
                        break;

                    case "XML":
                        protocol = Microservice_Endpoint_Protocol_Enum.XML;
                        break;

                    default:
                        protocol = Microservice_Endpoint_Protocol_Enum.JSON;
                        break;
                }
            }

            ReaderXml.MoveToElement();

            if ((componentid.Length > 0) && (endpoint.Segment.Length > 0) && (method.Length > 0))
            {
                if (ParentSegment != null)
                {
                    // Add this endpoint
                    if (ParentSegment.Children == null)
                        ParentSegment.Children = new Dictionary<string, Microservice_Path>();
                    ParentSegment.Children[endpoint.Segment] = endpoint;
                    AllEndpoints.Add(endpoint);

                    // Add the verb mapping defaulted to GET
                    endpoint.GetMapping = new Microservice_VerbMapping(method, enabled, protocol, Microservice_Endpoint_RequestType_Enum.GET);
                    EndpointToComponentDictionary[endpoint.GetMapping] = componentid;
                    EndpointToRestrictionDictionary[endpoint.GetMapping] = restrictionid;
                }
            }
        }

        private static void read_microservices_details_components(XmlReader ReaderXml, Microservice_Server_Configuration Config, Dictionary<string, Microservice_Component> Components)
        {
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "component":
                            string Namespace = String.Empty;
                            Microservice_Component component = new Microservice_Component();
                            if (ReaderXml.MoveToAttribute("ID"))
                                component.ID = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("Assembly"))
                                component.Assembly = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("Namespace"))
                                Namespace = ReaderXml.Value.Trim() + ".";
                            if (ReaderXml.MoveToAttribute("Class"))
                                component.Class = Namespace + ReaderXml.Value.Trim();
                            if ((!String.IsNullOrEmpty(component.ID)) && (!String.IsNullOrEmpty(component.Class)))
                            {
                                // If the key already existed, remove the old one as it will be replaced
                                if (Components.ContainsKey(component.ID))
                                {
                                    Config.Components.Remove(Components[component.ID]);
                                    Components.Remove(component.ID);
                                }
                                
                                // Add this new component
                                Components[component.ID] = component;
                                Config.Components.Add(component);
                                
                            }
                            break;
                    }
                }
            }
        }

        private static void read_microservices_details_restrictionranges(XmlReader ReaderXml, Microservice_Server_Configuration Config, Dictionary<string, Microservice_RestrictionRange> Ranges)
        {
            Microservice_RestrictionRange currentRange = null;

            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "range":
                            string rangeId = null;
                            if (ReaderXml.MoveToAttribute("ID"))
                                rangeId = ReaderXml.Value.Trim();

                            // Must have an ID to be valid
                            if (!String.IsNullOrEmpty(rangeId))
                            {
                                currentRange = null;

                                // Look for a matching range
                                foreach (Microservice_RestrictionRange range in Config.RestrictionRanges)
                                {
                                    if (range.ID == rangeId)
                                    {
                                        currentRange = range;
                                        break;
                                    }
                                }

                                // If no range, create the new one
                                if (currentRange == null)
                                {
                                    currentRange = new Microservice_RestrictionRange {ID = rangeId};
                                }

                                if (ReaderXml.MoveToAttribute("Label"))
                                    currentRange.Label = ReaderXml.Value.Trim();
                            }
                            else
                            {
                                // Missing ID in this range
                                currentRange = null;
                            }
                            break;

                        case "removeall":
                            if (currentRange != null)
                                currentRange.IpRanges.Clear();
                            break;

                        case "iprange":
                            if (currentRange != null)
                            {
                                Microservice_IpRange singleIpRange = new Microservice_IpRange();
                                if (ReaderXml.MoveToAttribute("Label"))
                                    singleIpRange.Label = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("Start"))
                                    singleIpRange.StartIp = ReaderXml.Value.Trim();
                                if (ReaderXml.MoveToAttribute("End"))
                                    singleIpRange.EndIp = ReaderXml.Value.Trim();
                                if (singleIpRange.StartIp.Length > 0)
                                    currentRange.IpRanges.Add(singleIpRange);
                            }
                            break;
                    }

                }
                else if (ReaderXml.NodeType == XmlNodeType.EndElement)
                {
                    if (String.Compare(ReaderXml.Name, "range", StringComparison.OrdinalIgnoreCase) == 0)
                    {
                        if ((currentRange != null) && (!String.IsNullOrEmpty(currentRange.ID)))
                        {
                            Ranges[currentRange.ID] = currentRange;
                            if (!Config.RestrictionRanges.Contains(currentRange))
                                Config.RestrictionRanges.Add(currentRange);
                        }
                        currentRange = null;
                    }
                }
            }
        }
    }
}
