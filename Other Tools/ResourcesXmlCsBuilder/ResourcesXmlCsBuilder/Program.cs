using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;

namespace ResourcesXmlCsBuilder
{
    class Program
    {
        static void Main(string[] args)
        {
            Default_Resources.Read_Config("sample.xml");
            Default_Resources.Read_Config("sample2.xml");



            // Get the list of all files
            Dictionary<string, List<string>> group_to_files = new Dictionary<string, List<string>>();
            SortedDictionary<string, string> files_and_links = new SortedDictionary<string, string>();
            StreamReader reader = new StreamReader("allFiles.txt");
            string line = reader.ReadLine();
            while (line != null)
            {
                string[] splitter = line.Split("\t".ToCharArray());
                string file = splitter[0].Replace("\"", "");
                string source = splitter[1].Replace("\"", "");

                string group = source.Substring(0, source.IndexOf("/"));

                if (group_to_files.ContainsKey(group))
                    group_to_files[group].Add(file);
                else
                {
                    group_to_files[group] = new List<string> {file};
                }


                if (!files_and_links.ContainsKey(file))
                {
                    files_and_links[file] = source;
                }

                line = reader.ReadLine();
            }
            reader.Close();

            // Write the output XML
            StreamWriter writer = new StreamWriter("output.xml");
            foreach (KeyValuePair<string, List<string>> grouping in group_to_files)
            {
                grouping.Value.Sort();
                Console.WriteLine(grouping.Key);

                writer.WriteLine("<grouping name=\"" + grouping.Key + "\">");

                foreach (string thisFile in grouping.Value)
                {
                    Console.WriteLine("\t" + thisFile);

                    writer.WriteLine("  <file key=\"" + thisFile.ToLower() + "\" source=\"[%BASEURL%]/" + files_and_links[thisFile] + "\" />");
                }
                writer.WriteLine("</grouping>");
            }
            writer.Flush();
            writer.Close();

            // Write the C# code portion
            StreamWriter codeWriter = new StreamWriter("Default_Resources.cs");
            codeWriter.WriteLine("namespace SobekCM.Library.Settings");
            codeWriter.WriteLine("{");
            codeWriter.WriteLine("\t/// <summary> Gateway to all of the static resources (images, javascript, stylesheets, and included libraries ) ");
            codeWriter.WriteLine("\t/// used by the standard SobekCM web user interface </summary>");
            codeWriter.WriteLine("\tpublic static class Default_Resources");
            codeWriter.WriteLine("\t{");

            // Write the constructor
            codeWriter.WriteLine("\t\t/// <summary> Static constructor for the Default_Resources class </summary>");
            codeWriter.WriteLine("\t\tstatic Default_Resources()");
            codeWriter.WriteLine("\t\t{");
            codeWriter.WriteLine("\t\t\t// Set the default values, using the CDN");
            // Write the properties
            foreach (KeyValuePair<string, string> thisFile in files_and_links)
            {
                codeWriter.WriteLine("\t\t\t" + file_to_property(thisFile.Key) + "=\"http://cdn.sobekrepository.org/" + thisFile.Value + "\";");
            }

            codeWriter.WriteLine("\t\t}");

            // Write the properties
            foreach (KeyValuePair<string, string> thisFile in files_and_links)
            {
                codeWriter.WriteLine("\t\t/// <summary> URL for the default resource '" + thisFile.Key.ToLower() + "' file ( http://cdn.sobekrepository.org/" + thisFile.Value + " by default)</summary>");
                codeWriter.WriteLine("\t\tpublic static string " + file_to_property(thisFile.Key) + " { get; private set; }");
                codeWriter.WriteLine();
            }

            // Add the method to save a value, by key
            codeWriter.WriteLine("\t\t/// <summary> Add a single file, with key and source </summary>");
            codeWriter.WriteLine("\t\t/// <param name=\"Key\"> Key for this file, from the default resources config file </param>");
            codeWriter.WriteLine("\t\t/// <param name=\"Source\"> Source (i.e., URL) for this file </param>");
            codeWriter.WriteLine("\t\tpublic static void Add_File(string Key, string Source)");
            codeWriter.WriteLine("\t\t{");
            codeWriter.WriteLine("\t\t\tswitch (Key)");
            codeWriter.WriteLine("\t\t\t{");

            // Write the properties
            foreach (KeyValuePair<string, string> thisFile in files_and_links)
            {
                codeWriter.WriteLine("\t\t\t\tcase \"" + thisFile.Key.ToLower() + "\":");
                codeWriter.WriteLine("\t\t\t\t\t" + file_to_property(thisFile.Key) + " = Source;");
                codeWriter.WriteLine("\t\t\t\t\tbreak;");
                codeWriter.WriteLine();
            }
            codeWriter.WriteLine("\t\t\t}");
            codeWriter.WriteLine("\t\t}");

            codeWriter.WriteLine("\t\t/// <summary> Read the indicated configuration file for these default statis resources </summary>");
            codeWriter.WriteLine("\t\t/// <param name=\"ConfigFile\"> Configuration file to read </param>");
            codeWriter.WriteLine("\t\t/// <returns> TRUE if successful, otherwise FALSE </returns>");
            codeWriter.WriteLine("\t\tpublic static bool Read_Config(string ConfigFile)");
            codeWriter.WriteLine("\t\t{");
            codeWriter.WriteLine("\t\t\t// Streams used for reading");
            codeWriter.WriteLine("\t\t\tStream readerStream = null;");
            codeWriter.WriteLine("\t\t\tXmlTextReader readerXml = null;");
            codeWriter.WriteLine("\t\t\tbool returnValue = true;");
            codeWriter.WriteLine("\t\t\tstring base_url = \"[%BASEURL%]\";");
            codeWriter.WriteLine();
            codeWriter.WriteLine("\t\t\ttry");
            codeWriter.WriteLine("\t\t\t{");
            codeWriter.WriteLine("\t\t\t\t// Open a link to the file");
            codeWriter.WriteLine("\t\t\t\treaderStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Read);");
            codeWriter.WriteLine();
            codeWriter.WriteLine("\t\t\t\t// Open a XML reader connected to the file");
            codeWriter.WriteLine("\t\t\t\treaderXml = new XmlTextReader(readerStream);");
            codeWriter.WriteLine();
            codeWriter.WriteLine("\t\t\t\twhile (readerXml.Read())");
            codeWriter.WriteLine("\t\t\t\t{");
            codeWriter.WriteLine("\t\t\t\t\tif (readerXml.NodeType == XmlNodeType.Element)");
            codeWriter.WriteLine("\t\t\t\t\t{");
            codeWriter.WriteLine("\t\t\t\t\t\tswitch (readerXml.Name.ToLower())");
            codeWriter.WriteLine("\t\t\t\t\t\t{");
            codeWriter.WriteLine("\t\t\t\t\t\t\tcase \"default_resources\":");
            codeWriter.WriteLine("\t\t\t\t\t\t\t\tif (readerXml.MoveToAttribute(\"baseUrl\")) base_url = readerXml.Value;");
            codeWriter.WriteLine("\t\t\t\t\t\t\t\tbreak;");
            codeWriter.WriteLine();
            codeWriter.WriteLine("\t\t\t\t\t\t\tcase \"file\":");
            codeWriter.WriteLine("\t\t\t\t\t\t\t\tstring key = (readerXml.MoveToAttribute(\"key\")) ? readerXml.Value.Trim() : null;");
            codeWriter.WriteLine("\t\t\t\t\t\t\t\tstring source = (readerXml.MoveToAttribute(\"source\")) ? readerXml.Value.Trim() : null;");
            codeWriter.WriteLine("\t\t\t\t\t\t\t\tif ((!String.IsNullOrEmpty(key)) && (!String.IsNullOrEmpty(source))) Add_File(key.ToLower(), source.Replace(\"[%BASEURL%]\", base_url));");
            codeWriter.WriteLine("\t\t\t\t\t\t\t\tbreak;");
            codeWriter.WriteLine("\t\t\t\t\t\t}");
            codeWriter.WriteLine("\t\t\t\t\t}");
            codeWriter.WriteLine("\t\t\t\t}");
            codeWriter.WriteLine("\t\t\t}");
            codeWriter.WriteLine("\t\t\tcatch");
            codeWriter.WriteLine("\t\t\t{");
            codeWriter.WriteLine("\t\t\t\treturnValue = false;");
            codeWriter.WriteLine("\t\t\t}");
            codeWriter.WriteLine("\t\t\tfinally");
            codeWriter.WriteLine("\t\t\t{");
            codeWriter.WriteLine("\t\t\t\tif (readerXml != null) readerXml.Close();");
            codeWriter.WriteLine("\t\t\t\tif (readerStream != null) readerStream.Close();");
            codeWriter.WriteLine("\t\t\t}");
            codeWriter.WriteLine();
            codeWriter.WriteLine("\t\t\treturn returnValue;");
            codeWriter.WriteLine("\t\t}");
            codeWriter.WriteLine("\t}");
            codeWriter.WriteLine("}");
            codeWriter.Flush();
            codeWriter.Close();

        }

        private static string file_to_property(string file)
        {
            TextInfo textInfo = new CultureInfo("en-US", false).TextInfo;

            string tempFile = file.ToLower().Replace(".", "_").Replace("-", "_").Replace("16", "Sixteen_");
            tempFile = tempFile.Replace("_", " ");
            tempFile = textInfo.ToTitleCase(tempFile); //War And Peace
            return tempFile.Replace(" ", "_");

            //return file.ToLower().Replace(".", "_").Replace("-", "_");
        }
    }
}
