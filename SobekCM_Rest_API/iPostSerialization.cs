using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Rest_API
{
    /// <summary> Interface is implemented for classes that require some additional configuration to occur around serialization events </summary>
    public interface iSerializationEvents
    {
        /// <summary> Class is called by the serializer after an item is unserialized </summary>
        void PostUnSerialization();
    }
}
