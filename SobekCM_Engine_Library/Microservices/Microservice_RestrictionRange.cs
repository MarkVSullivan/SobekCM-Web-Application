#region Using directives

using System.Collections.Generic;

#endregion

namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> Collection of IP ranges which can be used to limit access to a microservice endpoint </summary>
    public class Microservice_RestrictionRange
    {
        /// <summary> Constructor for a new instance of the Microservice_RestrictionRange class </summary>
        public Microservice_RestrictionRange()
        {
            IpRanges = new List<Microservice_IpRange>();
        }

        /// <summary> Identifier for this component, which is referenced within the configuration file to specify this component </summary>
        public string ID { get; internal set; }

        /// <summary> Descriptive label for this collection of IP addresses </summary>
        public string Label { get; internal set; }

        /// <summary> Collection of individual IP addresses or individual IP ranges </summary>
        public List<Microservice_IpRange> IpRanges { get; private set; }
    }
}