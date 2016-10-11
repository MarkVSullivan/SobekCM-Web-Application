using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.GenericXml.Results
{
    public class GenericXmlReaderResults
    {
        public List<MappedValue> MappedValues { get; set; }

        public List<UnmappedValue> UnmappedValues { get; set; }

        public string ErrorMessage { get; set; }

        public GenericXmlReaderResults()
        {
            MappedValues = new List<MappedValue>();
            UnmappedValues = new List<UnmappedValue>();
        }
    }
}
