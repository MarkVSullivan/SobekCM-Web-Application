#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Reflection;
using System.Web;
using SobekCM.Engine_Library.IpRangeUtilities;

#endregion

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

    /// <summary> Enumeration indicates the type of protocol utilized by this endpoint</summary>
    public enum Microservice_Endpoint_Protocol_Enum : byte
    {
        /// <summary> Output of this endpoint is a binary stream </summary>
        BINARY,

        /// <summary> Nothing is output ( serialized, deserialized ) but the engine places the object
        /// within the (shared) cache </summary>
        CACHE,

        /// <summary> Output of this endpoint is standard JSON </summary>
        JSON,

        /// <summary> Output of this endpoint is JSON-P </summary>
        JSON_P,

        /// <summary> Output of this endpoint is Protocol Buffer octet-stream </summary>
        PROTOBUF,

        /// <summary> Serve the object, via SOAP </summary>
        SOAP,

        /// <summary> Output in XML format </summary>
        XML
    }

    /// <summary> Class defines an microservice endpoint within a collection of path or URI segments </summary>
    public class Microservice_Endpoint : Microservice_Path
    {
        private MethodInfo methodInfo;
        private object restApiObject;
        private IpRangeSetV4 rangeTester;

        /// <summary> Invoke the method in the class specified for this endpoint, from the configuration XML file </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="RequestForm"></param>
        /// <param name="IsDebug"></param>
        public void Invoke(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, NameValueCollection RequestForm, bool IsDebug )
        {
            if ((methodInfo == null) || (restApiObject == null))
            {
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
                Type restApiClassType = dllAssembly.GetType(Component.Namespace + "." + Component.Class);
                restApiObject = Activator.CreateInstance(restApiClassType);

                methodInfo = restApiClassType.GetMethod(Method);
            }

            // Invokation is different, dependingon whether this is a PUT or POST
            if ( RequestType == Microservice_Endpoint_RequestType_Enum.GET )
                methodInfo.Invoke(restApiObject, new object[] { Response, UrlSegments, QueryString, Protocol, IsDebug });
            else
                methodInfo.Invoke(restApiObject, new object[] { Response, UrlSegments, Protocol, RequestForm });
        }


        /// <summary> Constructor for a new instance of the Microservice_Endpoint class </summary>
        public Microservice_Endpoint()
        {
            Enabled = true;
            RequestType = Microservice_Endpoint_RequestType_Enum.GET;
            Protocol = Microservice_Endpoint_Protocol_Enum.JSON;
        }

        /// <summary> Protocol which this endpoint utilizes ( JSON or Protocol Buffer ) </summary>
        public Microservice_Endpoint_Protocol_Enum Protocol { get; internal set; }

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

        /// <summary> Check to see if this endpoint can be invoked from this IP address </summary>
        /// <returns> TRUE if permitted, otherwise FALSE </returns>
        public bool AccessPermitted( string IpAddress )
        {
            // If no restriction exists, return TRUE
            if ((RestrictionRanges == null) || (RestrictionRanges.Count == 0))
                return true;

            // Was the comparison set built?
            if (rangeTester == null)
            {
                rangeTester = new IpRangeSetV4();
                foreach (Microservice_RestrictionRange thisRangeSet in RestrictionRanges)
                {
                    foreach (Microservice_IpRange thisRange in thisRangeSet.IpRanges)
                    {
                        if ( !String.IsNullOrEmpty(thisRange.EndIp))
                            rangeTester.AddIpRange(thisRange.StartIp, thisRange.EndIp);
                        else
                            rangeTester.AddIpRange(thisRange.StartIp);
                    }
                }
            }

            // You can always acess from the same machine (first may only be relevant in Visual Studio while debugging)
            if ((IpAddress == "::1") || (IpAddress == "127.0.0.1"))
                return true;

            // Now, test the IP against the tester
            return rangeTester.Contains(new ComparableIpAddress(IpAddress));
        }
    }
}