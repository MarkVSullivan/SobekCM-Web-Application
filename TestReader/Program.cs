using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using SobekCM.Core.Configuration;
using SobekCM.Core.Settings;

namespace TestReader
{
    class Program
    {
        static void Main(string[] args)
        {
            // Collect the list of directories to look for and read all .xml files
            List<string> directories = new List<string>();

            // Add the default configuration directory first
            directories.Add(@"C:\GitRepository\SobekCM-Web-Application\SobekCM\config\default");

            // Add all of the plug-in foldersm but ensure they are sorted
            string plug_in_folder = @"C:\GitRepository\SobekCM-Web-Application\SobekCM\plugins";
            if (Directory.Exists(plug_in_folder))
            {
                // Get the list of subdirectories 
                string[] subdirs = Directory.GetDirectories(plug_in_folder);

                // Ensure it is sorted alphabetically
                SortedList<string, string> subdirs_sorted = new SortedList<string, string>();
                foreach (string thisSubDir in subdirs)
                {
                    // Get the directory name and add to the sorted list
                    string dirName = (new DirectoryInfo(thisSubDir)).Name;
                    subdirs_sorted.Add(dirName, thisSubDir);
                }

                // Now, add each folder correctly sorted
                foreach (string thisSubDir in subdirs_sorted.Values)
                {
                    directories.Add(thisSubDir);
                    if (Directory.Exists(Path.Combine(thisSubDir, "config")))
                        directories.Add(Path.Combine(thisSubDir, "config"));
                }
            }

            // Add the final user configuration directory last
            directories.Add(@"C:\GitRepository\SobekCM-Web-Application\SobekCM\config\user");

            InstanceWide_Settings settings = new InstanceWide_Settings();

            // Read the configuration files
            InstanceWide_Configuration config = SobekCM.Engine_Library.Configuration.Configuration_Files_Reader.Read_Config_Files(directories, settings);

            StringBuilder XmlSb = new StringBuilder();
            TextWriter writerXml = new StringWriter(XmlSb);

            // Write out the config file
            XmlSerializer x = new XmlSerializer(config.GetType());
            x.Serialize(writerXml, config);

            StreamWriter writer = new StreamWriter("output.xml", false);
            writer.Write(XmlSb);
            writer.Flush();
            writer.Close();

            Console.ReadLine();
        }
    }
}
