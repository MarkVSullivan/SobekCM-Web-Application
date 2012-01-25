using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Contains information about a single finding guide container related to a digital resource (and ostensibly an EAD )</summary>
    [Serializable]
    public class Finding_Guide_Container : XML_Writing_Base_Type
    {
        private string type, name;
        private int level;

        /// <summary> Constructor for a new instance of a container used for creating finding guides </summary>
        /// <param name="Type">Type of container this represents (i.e., Box, Folder, etc..)</param>
        /// <param name="Name">Name of this container</param>
        /// <param name="Level">Level within the container list that this container resides</param>
        public Finding_Guide_Container(string Type, string Name, int Level)
        {
            name = Name;
            type = Type;
            level = Level;
        }

        /// <summary> Gets the name of this container </summary>
        public string Name
        {
            get { return name; }
            set { name = value; }
        }

        /// <summary> Gets the type of container this represents (i.e., Box, Folder, etc..)</summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary> Gets the level within the container list that this container resides </summary>
        public int Level
        {
            get { return level; }
            set { level = value; }
        }


        internal void toMETS(System.IO.TextWriter writer, string sobekcm_namespace)
        {
            if (name.Length == 0)
                return;

            writer.Write( "<" + sobekcm_namespace + ":Container");
            if (level > 0)
                writer.Write(" level=\"" + level + "\"");
            if (type.Length > 0)
                writer.Write(" type=\"" + base.Convert_String_To_XML_Safe(type) + "\"");
            writer.WriteLine(">" + base.Convert_String_To_XML_Safe(name) + "</" + sobekcm_namespace + ":Container>");
        }
    }
}
