namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> Code component which provides methods to fulfill microservice endpoint requests </summary>
    public class Microservice_Component
    {
        /// <summary> Identifier for this component, which is referenced within the configuration file to specify this component </summary>
        public string ID { get; internal set; }

        /// <summary> Assembly to load for this class, if this is an external assembly </summary>
        public string Assembly { get; internal set; }

        /// <summary> Namespace for this clas, within the assembly </summary>
        public string Namespace { get; internal set; }

        /// <summary> Name of the class which fulfills a microservice endpoint requests </summary>
        public string Class { get; internal set; }
    }
}