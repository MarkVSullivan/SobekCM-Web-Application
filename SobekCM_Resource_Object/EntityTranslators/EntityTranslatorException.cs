using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Resource_Object.EntityTranslators
{
    [Serializable]
    public class EntityTranslatorException : Exception
    {
        public EntityTranslatorException() : base() { }
        public EntityTranslatorException(string message) : base(message) { }
        public EntityTranslatorException(string message, Exception innerException) : base(message, innerException) { }
    }
}
