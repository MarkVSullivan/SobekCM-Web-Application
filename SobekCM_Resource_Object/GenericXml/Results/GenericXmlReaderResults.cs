using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.GenericXml.Results
{
    public class GenericXmlReaderResults
    {
        public List<MappedValue> MappedValues;

        public List<UnmappedValue> UnmappedValues;

        public GenericXmlReaderResults()
        {
            MappedValues = new List<MappedValue>();
            UnmappedValues = new List<UnmappedValue>();
        }
    }
}
