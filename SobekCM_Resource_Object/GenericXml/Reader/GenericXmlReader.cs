using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object.GenericXml.Mapping;
using SobekCM.Resource_Object.GenericXml.Results;

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

        public List<GenericXmlPathValue> GetValues(string XmlFile, string MappingFile )
        {
            // Open the mapping file first
            GenericXmlMappingSet set = GenericXmlMappingSet.Read(MappingFile);

            // Return value
            List<GenericXmlPathValue> returnValue = new List<GenericXmlPathValue>();
            Stack<GenericXmlNode> currentStack = new Stack<GenericXmlNode>();

            try
            {
                // Get the XML file content, stripped of display tags
                string content = cleaned_xml_string_from_file(XmlFile, set);

                // Streams used for reading
                XmlTextReader readerXml = new XmlTextReader(new StringReader(content));

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

        public List<GenericXmlPathValue> GetMappedValues(string XmlFile, string MappingFile)
        {
            // Open the mapping file first
            GenericXmlMappingSet set = GenericXmlMappingSet.Read(MappingFile);

            // Return value
            List<GenericXmlPathValue> returnValue = new List<GenericXmlPathValue>();
            Stack<GenericXmlNode> currentStack = new Stack<GenericXmlNode>();

            try
            {
                // Get the XML file content, stripped of display tags
                string content = cleaned_xml_string_from_file(XmlFile, set);

                // Streams used for reading
                XmlTextReader readerXml = new XmlTextReader(new StringReader(content));

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
                        recursive_get_mapped_values(readerXml.ReadSubtree(), currentStack, returnValue, set);

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

        #region Methods processes the XML file using the generic xml mapping

        public GenericXmlReaderResults ProcessFile(string XmlFile, string MappingFile)
        {
            // Open the mapping file first
            GenericXmlMappingSet set = GenericXmlMappingSet.Read(MappingFile);

            // Return value
            GenericXmlReaderResults returnValue = new GenericXmlReaderResults();

            // Stack to keep all the parent nodes
            Stack<GenericXmlNode> currentStack = new Stack<GenericXmlNode>();

            try
            {
                // Get the XML file content, stripped of display tags
                string content = cleaned_xml_string_from_file(XmlFile, set);

                // Streams used for reading
                XmlTextReader readerXml = new XmlTextReader(new StringReader(content));

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
                        recursive_process_file(readerXml.ReadSubtree(), currentStack, set, returnValue);

                        // Since this node was handled, pop it off the stack
                        currentStack.Pop();

                    }
                }

            }
            catch (Exception ee)
            {
                returnValue.ErrorMessage = ee.Message;
            }

            return returnValue;
        }

        private void recursive_process_file(XmlReader readerXml, Stack<GenericXmlNode> currentStack, GenericXmlMappingSet set, GenericXmlReaderResults returnValue)
        {
            // Create the list of nodes to this point from the stack
            List<GenericXmlNode> currentReverseList = currentStack.ToList();

            // THis is now on THIS element, so move to the next, unless there were none
            if (!readerXml.Read()) return;

            // Create the path for this
            GenericXmlPath currentPath = new GenericXmlPath();
            for (int j = currentReverseList.Count - 1; j >= 0; j--)
            {
                currentPath.PathNodes.Add(currentReverseList[j]);
            }

            //// First, handle any attributes
            //if (readerXml.HasAttributes)
            //{
            //    // Create the list of nodes to this point
            //    for (int i = 0; i < readerXml.AttributeCount; i++)
            //    {
            //        // Move to this attribute
            //        readerXml.MoveToAttribute(i);

            //        // Create the path for this
            //        GenericXmlPath thisPath = new GenericXmlPath();
            //        for (int j = currentReverseList.Count - 1; j >= 0; j--)
            //        {
            //            thisPath.PathNodes.Add(currentReverseList[j]);
            //        }
            //        thisPath.AttributeName = readerXml.Name;

            //        // Get the value for this path
            //        string attributeValue = readerXml.Value;

            //        // Does mapping exist for this path?
            //        PathMappingInstructions instructions = set.Get_Matching_Path_Instructions(thisPath);

            //        // If instructions, this was mapped
            //        if ((instructions != null) && (!String.IsNullOrEmpty(instructions.SobekMapping)))
            //        {
            //            MappedValue thisMappedValue = new MappedValue();
            //            thisMappedValue.Mapping = instructions.SobekMapping;
            //            thisMappedValue.Path = thisPath;
            //            thisMappedValue.Value = attributeValue;

            //            returnValue.MappedValues.Add(thisMappedValue);
            //        }
            //    }

            //    // Move back to the main element after handing the attributes
            //    readerXml.MoveToElement();
            //}





            // Now, iterate through all the child elements that may exist
            while (readerXml.Read())
            {
                //if (readerXml.NodeType == XmlNodeType.Text)
                //{
                //    // Create the path for this
                //    GenericXmlPath thisPath = new GenericXmlPath();
                //    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                //    {
                //        thisPath.PathNodes.Add(currentReverseList[j]);
                //    }

                //    // Does mapping exist for this path?
                //    PathMappingInstructions instructions = set.Get_Matching_Path_Instructions(thisPath);

                //    // Get the value for this path
                //    string textValue = readerXml.Value;

                //    // If instructions, this was mapped
                //    if ((instructions != null) && ( !String.IsNullOrEmpty(instructions.SobekMapping)))
                //    {
                //        MappedValue thisMappedValue = new MappedValue();
                //        thisMappedValue.Mapping = instructions.SobekMapping;
                //        thisMappedValue.Path = thisPath;
                //        thisMappedValue.Value = textValue;

                //        returnValue.MappedValues.Add(thisMappedValue);
                //    }
                //}

                bool text_found = false;
                if (readerXml.NodeType == XmlNodeType.Element)
                {
                    // Create the node for this top-level element
                    string nodeName = readerXml.Name;
                    GenericXmlNode topNode = new GenericXmlNode { NodeName = nodeName };

                    // Add this to the stack
                    currentStack.Push(topNode);

                    if (topNode.NodeName == "text")
                        text_found = true;

                    // It may be that mapping exists right here at this level
                    // Create the path for this
                    currentReverseList = currentStack.ToList();
                    GenericXmlPath thisPath = new GenericXmlPath();
                    for (int j = currentReverseList.Count - 1; j >= 0; j--)
                    {
                        thisPath.PathNodes.Add(currentReverseList[j]);
                    }

                    // Does mapping exist for this path?
                    if (set.Contains_Path(thisPath))
                    {
                        // Collect the attributes FIRST 
                        List<Tuple<string, string>> attributes = null;
                        if (readerXml.HasAttributes)
                        {
                            attributes = new List<Tuple<string, string>>();
                            readerXml.MoveToFirstAttribute();
                            attributes.Add(new Tuple<string, string>(readerXml.Name, readerXml.Value));
                            while (readerXml.MoveToNextAttribute())
                            {
                                attributes.Add(new Tuple<string, string>(readerXml.Name, readerXml.Value));
                            }
                            readerXml.MoveToElement();
                        }

                        // Does mapping exist for this path?
                        PathMappingInstructions instructions = set.Get_Matching_Path_Instructions(thisPath);

                        // If instructions, this was mapped
                        if ((instructions != null) && (!String.IsNullOrEmpty(instructions.SobekMapping)) && (instructions.IgnoreSubTree))
                        {
                            MappedValue thisMappedValue = new MappedValue();
                            thisMappedValue.Mapping = instructions.SobekMapping;
                            thisMappedValue.Path = thisPath;

                            if (instructions.RetainInnerXmlTags)
                            {
                                thisMappedValue.Value = readerXml.ReadInnerXml();
                            }
                            else
                            {
                                StringBuilder builder = new StringBuilder();
                                XmlReader innerReader = readerXml.ReadSubtree();
                                while (innerReader.Read())
                                {
                                    if (innerReader.NodeType == XmlNodeType.Text)
                                    {
                                        builder.Append(innerReader.Value);
                                    }
                                }
                                thisMappedValue.Value = builder.ToString();
                            }

                            returnValue.MappedValues.Add(thisMappedValue);

                            //// Actually, there is something here about skipping this
                            //readerXml.Skip();
                        }
                        else
                        {
                            // Recursively read this and all children
                            recursive_process_file(readerXml.ReadSubtree(), currentStack, set, returnValue);
                        }

                        // Now, handle the attributes
                        if ((attributes != null) && (attributes.Count > 0))
                        {
                            foreach (Tuple<string, string> attribute in attributes)
                            {
                                // Does mapping exist for this path?
                                PathMappingInstructions attrInstructions = set.Get_Matching_Path_Instructions(thisPath, attribute.Item1);

                                // If instructions, this was mapped
                                if ((attrInstructions != null) && (!String.IsNullOrEmpty(attrInstructions.SobekMapping)))
                                {
                                    MappedValue thisMappedValue = new MappedValue();
                                    thisMappedValue.Mapping = attrInstructions.SobekMapping;
                                    thisMappedValue.Path = thisPath;
                                    thisMappedValue.Path.AttributeName = attribute.Item1;
                                    thisMappedValue.Value = attribute.Item2;

                                    returnValue.MappedValues.Add(thisMappedValue);
                                }
                            }
                        }
                    }

                    // Since this node was handled, pop it off the stack
                    currentStack.Pop();
                }
            }
        }

        #endregion

        /// <summary> Read the full contents of the XML file, removing any display-only tags from the content </summary>
        /// <param name="XmlFile"></param>
        /// <param name="mappingSet"></param>
        /// <returns></returns>
        public string cleaned_xml_string_from_file(string XmlFile, GenericXmlMappingSet mappingSet)
        {
            // Just get the full text to work with
            string fullText = File.ReadAllText(XmlFile).Replace("\r\n", " ").Replace("\n", " ");

            // If there are no display tags to remove, done..
            if ((mappingSet.DisplayTagsToIgnore == null) || (mappingSet.DisplayTagsToIgnore.Count == 0))
                return fullText;

            // Open the content and read it into the StringBuilder
            StringBuilder builder = new StringBuilder((int) (fullText.Length*1.1));

            // The cleaning of the tags will not include the <text> portion of any metadata file
            int text_tag_index = -1;
            int text_tag_index1 = fullText.IndexOf("<text ");
            int text_tag_index2 = fullText.IndexOf("<text>");
            if (text_tag_index1 > 0)
                text_tag_index = text_tag_index1;
            if (text_tag_index2 > text_tag_index)
                text_tag_index = text_tag_index2;
            if (text_tag_index == -1)
                text_tag_index = fullText.Length;

            // Step through and remove the END tags
            int index = 0;
            int next_index = 0;
            bool start_tag = false;
            string next_tag = String.Empty;
            do
            {
                // Look for the next match
                next_index = fullText.Length;
                foreach (string displayTag in mappingSet.DisplayTagsToIgnore)
                {
                    // Look for the start tag first, including a space after for attributes or something
                    int this_index = fullText.IndexOf("<" + displayTag + " ", index, StringComparison.OrdinalIgnoreCase);
                    if (( this_index > 0 ) && (this_index < next_index))
                    {
                        next_index = this_index;
                    }

                    // Look for the start tag first, without any space or attributes
                    this_index = fullText.IndexOf("<" + displayTag + ">", index, StringComparison.OrdinalIgnoreCase);
                    if ((this_index > 0) && (this_index < next_index))
                    {
                        next_index = this_index;
                    }

                    // Look for the close tag
                    this_index = fullText.IndexOf("</" + displayTag + ">", index, StringComparison.OrdinalIgnoreCase);
                    if ((this_index > 0) && (this_index < next_index))
                    {
                        next_index = this_index;
                    }
                }

                // Was a match NOT found?
                if (next_index == fullText.Length)
                {
                    builder.Append(fullText.Substring(index));
                    break;
                }

                // The text tag index is another end criteria
                if (next_index > text_tag_index)
                {
                    builder.Append(fullText.Substring(index));
                    break;
                }
               
                // If a match was found, start by copying over the GOOD data
                builder.Append(fullText.Substring(index, next_index - index));

                // Now, find the end of this tag before continuing
                index = fullText.IndexOf(">", next_index, StringComparison.OrdinalIgnoreCase) + 1;

            } while (index < fullText.Length);

            return builder.ToString();
        }
    }
}
