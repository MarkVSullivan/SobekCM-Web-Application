using System.Collections.Generic;

namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> Class represents a single segment of a microservice endpoint's URI, including all child segments or endpoints  </summary>
    public class Microservice_Path
    {
        /// <summary> Single portion of a URI specifying a microservice endpoint </summary>
        public string Segment { get; internal set; }

        /// <summary> Collection of child segments or endpoints, indexed by the next segment of the URI </summary>
        public Dictionary<string, Microservice_Path> Children { get; internal set; }

        /// <summary> Flag indicates if this path actually defines a single endpoint </summary>
        /// <remarks> This always returns 'FALSE' in this class, although a child class may override this property </remarks>
        public virtual bool IsEndpoint { get { return false;  } }
    }
}