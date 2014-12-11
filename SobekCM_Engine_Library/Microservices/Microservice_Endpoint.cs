using System.Collections.Generic;

namespace SobekCM.Engine_Library.Microservices
{
    /// <summary> Enumeration indicates the type of HTTP request expected for this microservice endpoint </summary>
    public enum Microservice_Endpoint_RequestType_Enum : byte
    {
        /// <summary> Endpoint is called with a standard HTTP get, perhaps including arguments in the command line </summary>
        GET,

        /// <summary> Endpoint is called with a HTTP post call, including some object in the body of the post request </summary>
        POST
    }

    /// <summary> Class defines an microservice endpoint within a collection of path or URI segments </summary>
    public class Microservice_Endpoint : Microservice_Path
    {
        /// <summary> Constructor for a new instance of the Microservice_Endpoint class </summary>
        public Microservice_Endpoint()
        {
            Enabled = true;
            RequestType = Microservice_Endpoint_RequestType_Enum.GET;
        }

        /// <summary> Component defines the class which is used to fulfil the request </summary>
        public Microservice_Component Component { get; internal set; }

        /// <summary> Method within the class specified by the component that shuld be called to fulfil the request </summary>
        public string Method { get; internal set; }

        /// <summary> Flag indicates if this endpoint is enabled or disabled </summary>
        public bool Enabled { get; internal set; }

        /// <summary> If this endpoint is restricted to some IP ranges, this is the list of restriction ranges that
        /// can access this endpoint </summary>
        public List<Microservice_RestrictionRange> RestrictionRanges { get; internal set; }

        /// <summary> Description of this microservice endpoint, for automated creation of documentation </summary>
        public string Description { get; internal set; }

        /// <summary> Request type expected for this endpoint ( either a GET or a POST ) </summary>
        public Microservice_Endpoint_RequestType_Enum RequestType { get; internal set; }

        /// <summary> Description of the arguments that should be passed in to this endpoint </summary>
        /// <remarks> If no arguments are needed, the value of NONE should appear within this tag in the configuration file </remarks>
        public string Arguments { get; internal set; }

        /// <summary> Description of the type of object (usually specifying at least the class of the object) that is returned in the JSON or XML </summary>
        public string Returns { get; internal set; }

        /// <summary> Flag indicates if this path actually defines a single endpoint </summary>
        /// <remarks> This always returns 'TRUE' in this class </remarks>
        public override bool IsEndpoint
        {
            get { return true; }
        }
    }
}