using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Library.TEI
{
    /// <summary> Class stored the basic information for the system about the TEI plug-in, such as
    /// the names of all the XSLT, CSS, and Mapping files included in the instance </summary>
    public class TEI_Configuration
    {
        /// <summary> Sorted list of the XSLT files, without the extension </summary>
        public List<string> XSLT_Files { get; set; } 

        /// <summary> Sorted list of the CSS files, without the extension </summary>
        public List<string> CSS_Files { get; set; } 

        /// <summary> Sorted list of the mapping files, without the extension </summary>
        public List<string> Mapping_Files { get; set; } 

        /// <summary> Constructor for a new instance of the TEI_Configuration object </summary>
        /// <param name="PlugIn_Directory"> Directory for the TEI plugin-in </param>
        public TEI_Configuration(string PlugIn_Directory)
        {
            // Declare the collections first
            XSLT_Files = new List<string>();
            CSS_Files = new List<string>();
            Mapping_Files = new List<string>();

            // Get the list of existing XSLT files
            string xslt_directory = Path.Combine(PlugIn_Directory, "xslt");
            SortedList<string, string> xslt_files_sorted = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
            if (Directory.Exists(xslt_directory))
            {
                // Collect all the XSLT files
                string[] xslt_files = Directory.GetFiles(xslt_directory, "*.xslt");

                // Sort the XSLT filenames (without extensions)
                foreach (string thisFile in xslt_files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(thisFile);
                    xslt_files_sorted[fileName] = fileName;
                }

                // Add the sorted names to the list
                XSLT_Files.AddRange(xslt_files_sorted.Values);
            }

            // Get the list of existing CSS files
            string css_directory = Path.Combine(PlugIn_Directory, "css");
            SortedList<string, string> css_files_sorted = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
            if (Directory.Exists(css_directory))
            {
                // Collect all the CSS files
                string[] css_files = Directory.GetFiles(css_directory, "*.css");

                // Sort the CSS filenames (without extensions)
                foreach (string thisFile in css_files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(thisFile);
                    css_files_sorted[fileName] = fileName;
                }

                // Add the sorted names to the list
                CSS_Files.AddRange(css_files_sorted.Values);
            }

            // Get the list of existing mapping files
            string mapping_directory = Path.Combine(PlugIn_Directory, "mapping");
            SortedList<string, string> mapping_files_sorted = new SortedList<string, string>(StringComparer.OrdinalIgnoreCase);
            if (Directory.Exists(mapping_directory))
            {
                // Collect all the mapping files
                string[] mapping_files = Directory.GetFiles(mapping_directory, "*.xml");

                // Sort the mapping filenames (without extensions)
                foreach (string thisFile in mapping_files)
                {
                    string fileName = Path.GetFileNameWithoutExtension(thisFile);
                    mapping_files_sorted[fileName] = fileName;
                }

                // Add the sorted names to the list
                Mapping_Files.AddRange(mapping_files_sorted.Values);
            }
        }
    }
}
