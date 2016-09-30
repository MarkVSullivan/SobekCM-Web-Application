using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Resource_Object.GenericXml.Reader;

namespace SobekCM.Resource_Object.GenericXml.Mapping
{
    public class GenericXmlMappingTreeNode
    {
        public GenericXmlNode Node;

        public Dictionary<string, GenericXmlMappingTreeNode> Children;

        public PathMappingInstructions Instructions;

        public GenericXmlMappingTreeNode()
        {
            Children = new Dictionary<string, GenericXmlMappingTreeNode>(StringComparer.OrdinalIgnoreCase);
        }


    }
}
