using System;
using System.IO;
using System.Xml;

namespace SobekCM.Core.MicroservicesClient
{
    /// <summary> Read the configuration file for the client side of micro-services links </summary>
    public static class MicroservicesClient_Config_Reader
    {
        /// <summary> Static class is used to read the configuration file defining microservice endpoints </summary>
        /// <param name="ConfigFile"> Path and name of the configuration XML file to read </param>
        /// <param name="SystemBaseUrl"> System base URL </param>
        /// <returns> Fully configured microservices configuration object </returns>
        public static MicroservicesClient_Configuration Read_Config(string ConfigFile, string SystemBaseUrl )
        {
            MicroservicesClient_Configuration returnValue = new MicroservicesClient_Configuration();

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
                            case "microservicesclient":
                                read_microservices_client_details(readerXml.ReadSubtree(), returnValue, SystemBaseUrl);
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

        private static void read_microservices_client_details(XmlReader readerXml, MicroservicesClient_Configuration config, string SystemBaseUrl)
        {
            // Just step through the subtree of this
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    switch (readerXml.Name.ToLower())
                    {
                        case "add":
                            string key = String.Empty;
                            string url = String.Empty;
                            string protocol = "JSON";
                            if (readerXml.MoveToAttribute("Key"))
                                key = readerXml.Value;
                            if (readerXml.MoveToAttribute("URL"))
                                url = readerXml.Value.Replace("[BASEURL]", SystemBaseUrl);
                            if (readerXml.MoveToAttribute("Protocol"))
                                protocol = readerXml.Value;

                            if (( !String.IsNullOrEmpty(key)) && ( !String.IsNullOrEmpty(url)))
                                config.Add_Endpoint(key, url, protocol);
                            break;
                    }
                }
            }
        }
    }
}
