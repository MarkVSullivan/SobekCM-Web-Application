using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using SobekCM.Resource_Object.GenericXml.Mapping;

namespace SobekCM.Resource_Object.GenericXml.Reader
{
    public class GenericXmlReader
    {
        #region Methods retrieve all existing, unique paths

        public List<GenericXmlPath> GetExistingPaths(string XmlFile)
        {
            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            // Return value
            Dictionary<string, string> pathExistenceCheck = new Dictionary<string, string>();
            List<GenericXmlPath> returnValue = new List<GenericXmlPath>();
            Stack<GenericXmlNode> currentStack = new Stack<GenericXmlNode>();

            try
            {
                // Open a link to the file
                readerStream = new FileStream(XmlFile, FileMode.Open, FileAccess.Read);

                // Try to read the XML
                readerXml = new XmlTextReader(readerStream);

                // Step through the top-level elements
                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        // Create the node for this top-level element
                        string nodeName = readerXml.Name;
                        GenericXmlNode topNode = new GenericXmlNode {NodeName = nodeName};

                        // Add this to the stack
                        currentStack.Push(topNode);

                        // Recursively read this and all children
                        recursive_get_existing_paths(readerXml.ReadSubtree(), currentStack, pathExistenceCheck, returnValue);

                        // Since this node was handled, pop it off the stack
                        currentStack.Pop();

                    }
                }

            }
            catch (Exception ee)
            {
               // MessageBox.Show(ee.Message);
            }

            return returnValue;
        }

        private void recursive_get_existing_paths(XmlReader readerXml, Stack<GenericXmlNode> currentStack, Dictionary<string, string> pathExistenceCheck, List<GenericXmlPath> returnValue )
        {
            // Create the list of nodes to this point from the stack
            List<GenericXmlNode> currentReverseList = currentStack.ToList();


            // THis is now on THIS element, so move to the next, unless there were none
            if (!readerXml.Read()) return;

            // Check to see if this node has been added yet
            // Create the path for this
            GenericXmlPath currentPath = new GenericXmlPath();
            for (int j = currentReverseList.Count - 1; j >= 0; j--)
            {
                currentPath.PathNodes.Add(currentReverseList[j]);
            }

            // Was this already added?
            string currentPathString = currentPath.ToString();
            if (!pathExistenceCheck.ContainsKey(currentPathString))
            {
                pathExistenceCheck[currentPathString] = currentPathString;
                returnValue.Add(currentPath);
            }

            // First, handle any attributes
            if (readerXml.HasAttributes)
            {
                // Create the list of nodes to this point
                for( int i = 0 ; i < readerXml.AttributeCount ; i++ )
                {
                    // Move to this attribute
                    readerXml.MoveToAttribute(i);

                    // Create the path for this
                    GenericXmlPath thisPath = new GenericXmlPath();
                    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                    {
                        thisPath.PathNodes.Add(currentReverseList[j]);
                    }
                    thisPath.AttributeName = readerXml.Name;

                    // Was this already added?
                    string pathString = thisPath.ToString();
                    if (!pathExistenceCheck.ContainsKey(pathString))
                    {
                        pathExistenceCheck[pathString] = pathString;
                        returnValue.Add(thisPath);
                    }
                }

                // Move back to the main element after handing the attributes
                readerXml.MoveToElement();
            }

            //// THis is now on THIS element, so move to the next, unless there were none
            //if (!readerXml.Read()) return;



            //// Is there actual text for this element
            //if ( !String.IsNullOrEmpty(readerXml.Value))
            //{
            //    // Create the path for this
            //    GenericXmlReaderMapperPath thisPath = new GenericXmlReaderMapperPath();
            //    for (int j = currentReverseList.Count - 1; j >= 0; j--)
            //    {
            //        thisPath.PathNodes.Add(currentReverseList[j]);
            //    }

            //    // Was this already added?
            //    string pathString = thisPath.ToString();
            //    if (!pathExistenceCheck.ContainsKey(pathString))
            //    {
            //        pathExistenceCheck[pathString] = pathString;
            //        returnValue.Add(thisPath);
            //    }
            //}



            // Now, iterate through all the child elements that may exist
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Text)
                {
                    // Create the path for this
                    GenericXmlPath thisPath = new GenericXmlPath();
                    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                    {
                        thisPath.PathNodes.Add(currentReverseList[j]);
                    }

                    // Was this already added?
                    string pathString = thisPath.ToString();
                    if (!pathExistenceCheck.ContainsKey(pathString))
                    {
                        pathExistenceCheck[pathString] = pathString;
                        returnValue.Add(thisPath);
                    }
                }

                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    // Create the node for this top-level element
                    string nodeName = readerXml.Name;
                    GenericXmlNode topNode = new GenericXmlNode { NodeName = nodeName };

                    // Add this to the stack
                    currentStack.Push(topNode);

                    // Recursively read this and all children
                    recursive_get_existing_paths(readerXml.ReadSubtree(), currentStack, pathExistenceCheck, returnValue);

                    // Since this node was handled, pop it off the stack
                    currentStack.Pop();
                }
            }
        }

        #endregion

        #region Methods retrieve all path/value pairs that existing in the file

        public List<GenericXmlPathValue> GetValues(string XmlFile )
        {
            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            // Return value
            List<GenericXmlPathValue> returnValue = new List<GenericXmlPathValue>();
            Stack<GenericXmlNode> currentStack = new Stack<GenericXmlNode>();

            try
            {
                // Open a link to the file
                readerStream = new FileStream(XmlFile, FileMode.Open, FileAccess.Read);

                // Try to read the XML
                readerXml = new XmlTextReader(readerStream);

                // Step through the top-level elements
                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        // Create the node for this top-level element
                        string nodeName = readerXml.Name;
                        GenericXmlNode topNode = new GenericXmlNode { NodeName = nodeName };

                        // Add this to the stack
                        currentStack.Push(topNode);

                        // Recursively read this and all children
                        recursive_get_values(readerXml.ReadSubtree(), currentStack, returnValue);

                        // Since this node was handled, pop it off the stack
                        currentStack.Pop();

                    }
                }

            }
            catch (Exception ee)
            {
                // MessageBox.Show(ee.Message);
            }

            return returnValue;
        }

        private void recursive_get_values(XmlReader readerXml, Stack<GenericXmlNode> currentStack, List<GenericXmlPathValue> returnValue)
        {
            // Create the list of nodes to this point from the stack
            List<GenericXmlNode> currentReverseList = currentStack.ToList();


            // THis is now on THIS element, so move to the next, unless there were none
            if (!readerXml.Read()) return;

            // Check to see if this node has been added yet
            // Create the path for this
            GenericXmlPath currentPath = new GenericXmlPath();
            for (int j = currentReverseList.Count - 1; j >= 0; j--)
            {
                currentPath.PathNodes.Add(currentReverseList[j]);
            }

            //// Was this already added?
            //string currentPathString = currentPath.ToString();
            //if (!pathExistenceCheck.ContainsKey(currentPathString))
            //{
            //    pathExistenceCheck[currentPathString] = currentPathString;
            //    returnValue.Add(currentPath);
            //}

            // First, handle any attributes
            if (readerXml.HasAttributes)
            {
                // Create the list of nodes to this point
                for (int i = 0; i < readerXml.AttributeCount; i++)
                {
                    // Move to this attribute
                    readerXml.MoveToAttribute(i);

                    // Create the path for this
                    GenericXmlPath thisPath = new GenericXmlPath();
                    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                    {
                        thisPath.PathNodes.Add(currentReverseList[j]);
                    }
                    thisPath.AttributeName = readerXml.Name;

                    // Get the value for this path
                    string attributeValue = readerXml.Value;

                    // Add this path/value pair to the return value
                    returnValue.Add(new GenericXmlPathValue(thisPath, attributeValue));
                }

                // Move back to the main element after handing the attributes
                readerXml.MoveToElement();
            }

            //// THis is now on THIS element, so move to the next, unless there were none
            //if (!readerXml.Read()) return;



            //// Is there actual text for this element
            //if ( !String.IsNullOrEmpty(readerXml.Value))
            //{
            //    // Create the path for this
            //    GenericXmlReaderMapperPath thisPath = new GenericXmlReaderMapperPath();
            //    for (int j = currentReverseList.Count - 1; j >= 0; j--)
            //    {
            //        thisPath.PathNodes.Add(currentReverseList[j]);
            //    }

            //    // Was this already added?
            //    string pathString = thisPath.ToString();
            //    if (!pathExistenceCheck.ContainsKey(pathString))
            //    {
            //        pathExistenceCheck[pathString] = pathString;
            //        returnValue.Add(thisPath);
            //    }
            //}



            // Now, iterate through all the child elements that may exist
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Text)
                {
                    // Create the path for this
                    GenericXmlPath thisPath = new GenericXmlPath();
                    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                    {
                        thisPath.PathNodes.Add(currentReverseList[j]);
                    }

                    // Get the value for this path
                    string textValue = readerXml.Value;

                    // Add this path/value pair to the return value
                    returnValue.Add(new GenericXmlPathValue(thisPath, textValue));
                }

                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    // Create the node for this top-level element
                    string nodeName = readerXml.Name;
                    GenericXmlNode topNode = new GenericXmlNode { NodeName = nodeName };

                    // Add this to the stack
                    currentStack.Push(topNode);

                    // Recursively read this and all children
                    recursive_get_values(readerXml.ReadSubtree(), currentStack, returnValue);

                    // Since this node was handled, pop it off the stack
                    currentStack.Pop();
                }
            }
        }

        #endregion

        #region Methods retrieve all path/value pairs that existing in the file

        public List<GenericXmlPathValue> GetMappedValues(string XmlFile, GenericXmlMappingSet MappingSet)
        {
            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            // Return value
            List<GenericXmlPathValue> returnValue = new List<GenericXmlPathValue>();
            Stack<GenericXmlNode> currentStack = new Stack<GenericXmlNode>();

            try
            {
                // Open a link to the file
                readerStream = new FileStream(XmlFile, FileMode.Open, FileAccess.Read);

                // Try to read the XML
                readerXml = new XmlTextReader(readerStream);

                // Step through the top-level elements
                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        // Create the node for this top-level element
                        string nodeName = readerXml.Name;
                        GenericXmlNode topNode = new GenericXmlNode { NodeName = nodeName };

                        // Add this to the stack
                        currentStack.Push(topNode);

                        // Recursively read this and all children
                        recursive_get_mapped_values(readerXml.ReadSubtree(), currentStack, returnValue, MappingSet);

                        // Since this node was handled, pop it off the stack
                        currentStack.Pop();

                    }
                }

            }
            catch (Exception ee)
            {
               // MessageBox.Show(ee.Message);
            }

            return returnValue;
        }

        private void recursive_get_mapped_values(XmlReader readerXml, Stack<GenericXmlNode> currentStack, List<GenericXmlPathValue> returnValue, GenericXmlMappingSet MappingSet)
        {
            // Create the list of nodes to this point from the stack
            List<GenericXmlNode> currentReverseList = currentStack.ToList();


            // THis is now on THIS element, so move to the next, unless there were none
            if (!readerXml.Read()) return;

            // Check to see if this node has been added yet
            // Create the path for this
            GenericXmlPath currentPath = new GenericXmlPath();
            for (int j = currentReverseList.Count - 1; j >= 0; j--)
            {
                currentPath.PathNodes.Add(currentReverseList[j]);
            }

            //// Was this already added?
            //string currentPathString = currentPath.ToString();
            //if (!pathExistenceCheck.ContainsKey(currentPathString))
            //{
            //    pathExistenceCheck[currentPathString] = currentPathString;
            //    returnValue.Add(currentPath);
            //}

            // First, handle any attributes
            if (readerXml.HasAttributes)
            {
                // Create the list of nodes to this point
                for (int i = 0; i < readerXml.AttributeCount; i++)
                {
                    // Move to this attribute
                    readerXml.MoveToAttribute(i);

                    // Create the path for this
                    GenericXmlPath thisPath = new GenericXmlPath();
                    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                    {
                        thisPath.PathNodes.Add(currentReverseList[j]);
                    }
                    thisPath.AttributeName = readerXml.Name;

                    // Get the value for this path
                    string attributeValue = readerXml.Value;

                    // Add this path/value pair to the return value
                    returnValue.Add(new GenericXmlPathValue(thisPath, attributeValue));
                }

                // Move back to the main element after handing the attributes
                readerXml.MoveToElement();
            }

            //// THis is now on THIS element, so move to the next, unless there were none
            //if (!readerXml.Read()) return;



            //// Is there actual text for this element
            //if ( !String.IsNullOrEmpty(readerXml.Value))
            //{
            //    // Create the path for this
            //    GenericXmlReaderMapperPath thisPath = new GenericXmlReaderMapperPath();
            //    for (int j = currentReverseList.Count - 1; j >= 0; j--)
            //    {
            //        thisPath.PathNodes.Add(currentReverseList[j]);
            //    }

            //    // Was this already added?
            //    string pathString = thisPath.ToString();
            //    if (!pathExistenceCheck.ContainsKey(pathString))
            //    {
            //        pathExistenceCheck[pathString] = pathString;
            //        returnValue.Add(thisPath);
            //    }
            //}



            // Now, iterate through all the child elements that may exist
            while (readerXml.Read())
            {
                if (readerXml.NodeType == XmlNodeType.Text)
                {
                    // Create the path for this
                    GenericXmlPath thisPath = new GenericXmlPath();
                    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                    {
                        thisPath.PathNodes.Add(currentReverseList[j]);
                    }

                    // Get the value for this path
                    string textValue = readerXml.Value;

                    // Add this path/value pair to the return value
                    returnValue.Add(new GenericXmlPathValue(thisPath, textValue));
                }

                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    // Create the node for this top-level element
                    string nodeName = readerXml.Name;
                    GenericXmlNode topNode = new GenericXmlNode { NodeName = nodeName };

                    // Add this to the stack
                    currentStack.Push(topNode);

                    // Recursively read this and all children
                    recursive_get_values(readerXml.ReadSubtree(), currentStack, returnValue);

                    // Since this node was handled, pop it off the stack
                    currentStack.Pop();
                }
            }
        }

        #endregion
    }
}
