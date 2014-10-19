#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Core.SiteMap;

#endregion

namespace SobekCM.Engine_Library.SiteMap
{
    /// <summary> Class is used to read an existing site map file and return the fully built <see cref="SobekCM_SiteMap" /> object </summary>
    public class SobekCM_SiteMap_Reader
    {
        /// <summary> Reads an existing site map file and returns the fully built <see cref="SobekCM_SiteMap" /> object </summary>
        /// <param name="SiteMap_File"> Sitemap file to read </param>
        /// <returns> Fully built sitemap object </returns>
        public static SobekCM_SiteMap Read_SiteMap_File(string SiteMap_File)
        {
            Stream reader = null;
            XmlTextReader nodeReader = null;
            SobekCM_SiteMap siteMap = null;
            int nodeValue = 1;

            Stack<SobekCM_SiteMap_Node> nodesStack = new Stack<SobekCM_SiteMap_Node>();

            try
            {
                // Create and open the readonly file stream
                reader = new FileStream(SiteMap_File, FileMode.Open, FileAccess.Read);

                // create the XML node reader
                nodeReader = new XmlTextReader(reader);

                // Read through the XML document
                while (nodeReader.Read())
                {
                    if (nodeReader.NodeType == XmlNodeType.Element)
                    {
                        // Handle the main sitemap tag
                        if (nodeReader.Name == "siteMap")
                        {
                            // This is the first node read, so it may have additional information
                            siteMap = new SobekCM_SiteMap();

                            // Look for the optional default breadcrumbs attribute
                            if (nodeReader.MoveToAttribute("default_breadcrumb"))
                            {
                                siteMap.Default_Breadcrumb = nodeReader.Value;
                            }

                            // Look for the optional width attribute
                            if (nodeReader.MoveToAttribute("width"))
                            {
                                short width;
                                Int16.TryParse(nodeReader.Value, out width);
                                siteMap.Width = width;
                            }

                            // Look for the optional url restriction attribute
                            if (nodeReader.MoveToAttribute("restrictedRobotUrl"))
                            {
                                siteMap.Restricted_Robot_URL = nodeReader.Value;
                            }
                        }

                        // Handle a new siteMapNode
                        if (nodeReader.Name == "siteMapNode")
                        {
                            string url = String.Empty;
                            string title = String.Empty;
                            string description = String.Empty;
                            bool empty = false;

                            // Before moving to any attributes, check to see if this is empty
                            if (nodeReader.IsEmptyElement)
                                empty = true;

                            // Step through the attributes
                            while (nodeReader.MoveToNextAttribute())
                            {
                                switch (nodeReader.Name)
                                {
                                    case "url":
                                        url = nodeReader.Value;
                                        break;

                                    case "title":
                                        title = nodeReader.Value;
                                        break;

                                    case "description":
                                        description = nodeReader.Value;
                                        break;
                                }
                            }

                            // Create the new node
                            SobekCM_SiteMap_Node newNode = new SobekCM_SiteMap_Node(url, title, description, nodeValue++);

                            // Add to the parent
                            if (nodesStack.Count == 0)
                            {
                                // This is the first node read so it should be the root node
                                if (siteMap != null) siteMap.RootNodes.Add(newNode);
                            }
                            else
                            {
                                nodesStack.Peek().Add_Child_Node(newNode);
                            }
                            
                            // Add this to the stack, at least until the end of this node is found 
                            // if this is not an empty element
                            if (!empty)
                                nodesStack.Push(newNode);
                        }
                    }
                    else if ((nodeReader.NodeType == XmlNodeType.EndElement) && ( nodeReader.Name == "siteMapNode" ))
                    {
                        nodesStack.Pop();
                    }
                }
            }
            finally
            {
                if (nodeReader != null)
                    nodeReader.Close();
                if (reader != null)
                    reader.Close();
            }

            return siteMap;
        }
    }
}
