using System;
using System.Collections.Generic;
using System.Text;

namespace SobekCM.Bib_Package
{
    /// <summary> Helper base class to assist with writing data in XML </summary>
    [Serializable]
    public class XML_Writing_Base_Type
    {
        /// <summary> Constructor for a new instance of the XML_Writing_Base_Type class </summary>
        public XML_Writing_Base_Type()
        {

        }

        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        public static string Convert_String_To_XML_Safe_Static(string element)
        {
            if (element == null)
                return string.Empty;

            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        /// <summary> Converts a basic string into an XML-safe string </summary>
        /// <param name="element"> Element data to convert </param>
        /// <returns> Data converted into an XML-safe string</returns>
        internal string Convert_String_To_XML_Safe(string element)
        {
            if (element == null)
                return string.Empty;

            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }
        
        /// <summary> Writes a data value in Greenstone XML format </summary>
        /// <param name="value"> Data value </param>
        /// <param name="metadata_name"> Name of the related metadata field </param>
        /// <param name="indent"> Indent for formatting the resultant metadata file </param>
        /// <returns> Greenstone XML formatted string </returns>
        internal string To_GSA(string value, string metadata_name, string indent)
        {
            if (( value == null ) || ( value.Length == 0))
                return String.Empty;

            return indent + "<Metadata name=\"" + metadata_name + "\">" + Convert_String_To_XML_Safe(value).Replace("[","").Replace("]","") + "</Metadata>\r\n";
        }

    }
}
