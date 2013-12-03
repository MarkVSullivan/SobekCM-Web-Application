using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace SobekCM.Builder
{
    public class HTML_XML_Text_Extractor
    {

        /// <summary> Extracts the full text from a HTML file and writes to a file </summary>
        /// <param name="HTML_In_Name">Full path to the HTML file</param>
        /// <param name="Text_Out_Name">Output file name for the extracted text </param>
        /// <returns>TRUE if successful, otherwise FALSE</returns>
        public static bool Extract_Text(string HTML_In_Name, string Text_Out_Name)
        {
            StreamWriter outFile = null;
            StreamReader inFile = null;

            bool isXML = false;
            if (HTML_In_Name.ToUpper().IndexOf(".XML") > 0)
                isXML = true;

            try
            {
                // Create the resulting file for the text
                outFile = new StreamWriter(Text_Out_Name, false, System.Text.Encoding.UTF8);

                // Open a reader to the HTML file
                inFile = new StreamReader(HTML_In_Name);
                
                // If this is XML, we will need to keep track of all used tags and attributes
                List<string> included_tags = new List<string>();
                StringBuilder tagBuilder = new StringBuilder();

                // Build this line by line
                StringBuilder lineBuilder = new StringBuilder();

                // Step through each character
                int bracket_depth = 0;
                string line = inFile.ReadLine();
                bool building_tag = false;
                char lastChar = ' ';
                while (line != null)
                {
                    foreach (char thisChar in line)
                    {
                        switch (thisChar)
                        {
                            case '<':
                                bracket_depth++;
                                if (isXML)
                                {
                                    if (tagBuilder.Length > 0)
                                        tagBuilder.Remove(0, tagBuilder.Length);
                                }
                                break;

                            case '>':
                                bracket_depth--;
                                if (isXML)
                                {
                                    if (tagBuilder.Length > 0)
                                    {                      
                                        string possibletag = tagBuilder.ToString();
                                        tagBuilder.Remove(0, tagBuilder.Length);
                                        if (!included_tags.Contains(possibletag))
                                        {
                                            lineBuilder.Append(possibletag + " ");
                                            included_tags.Add(possibletag);
                                        }
                                    }
                                }
                                break;

                            case '/':
                            case '?':
                                if (bracket_depth == 0)
                                {
                                    lineBuilder.Append(thisChar);
                                    lastChar = thisChar;
                                }
                                break;

                            case ' ':
                            case '=':
                            case '"':
                                if (bracket_depth > 0)
                                {
                                    if (isXML)
                                    {
                                        if (tagBuilder.Length > 0)
                                        {
                                            string possibletag = tagBuilder.ToString();
                                            tagBuilder.Remove(0, tagBuilder.Length);
                                            if (!included_tags.Contains(possibletag))
                                            {
                                                lineBuilder.Append(possibletag + " ");
                                                included_tags.Add(possibletag);
                                            }
                                        }
                                    }
                                }
                                else
                                {
                                    if ((thisChar != ' ') || (lastChar != ' '))
                                    {
                                        lineBuilder.Append(thisChar);
                                        lastChar = thisChar;
                                    }
                                }
                                break;

                            default:
                                if (bracket_depth > 0)
                                {
                                    if (isXML)
                                    {
                                        tagBuilder.Append(thisChar);
                                    }
                                }
                                else
                                {
                                    if ((thisChar != ' ') || (lastChar != ' '))
                                    {
                                        lineBuilder.Append(thisChar);
                                        lastChar = thisChar;
                                    }
                                }
                                break;

                        }
                    }

                    // Add this line
                    if (lineBuilder.Length > 0)
                    {
                        string possibleLine = lineBuilder.ToString().Replace("&nbsp;"," ").Trim();
                        if (possibleLine.Length > 0)
                        {
                            outFile.WriteLine(possibleLine.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&gt;", ">").Replace("&lt;", "<"));
                        }
                        lineBuilder.Remove(0, lineBuilder.Length);
                    }

                    // Get the next line
                    line = inFile.ReadLine();
                }

                // Add this line
                if (lineBuilder.Length > 0)
                {
                    string possibleLine = lineBuilder.ToString().Replace("&nbsp;", " ").Trim();
                    if (possibleLine.Length > 0)
                    {
                        outFile.WriteLine(possibleLine.Replace("&amp;", "&").Replace("&quot;", "\"").Replace("&gt;",">").Replace("&lt;","<"));
                    }
                    lineBuilder.Remove(0, lineBuilder.Length);
                }

                return true;
            }
            catch 
            {

            }
            finally
            {
                if (outFile != null) outFile.Close();
                if (inFile != null) inFile.Close();
            }

            return false;
        }

    }
}
