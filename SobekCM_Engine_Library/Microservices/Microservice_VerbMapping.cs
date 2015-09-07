using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using SobekCM.Engine_Library.IpRangeUtilities;

namespace SobekCM.Engine_Library.Microservices
{
    public class Microservice_VerbMapping
    {
        private MethodInfo methodInfo;
        private object restApiObject;

        private IpRangeSetV4 rangeTester;

        /// <summary> Protocol which this endpoint utilizes ( JSON or Protocol Buffer ) </summary>
        public Microservice_Endpoint_Protocol_Enum Protocol { get; internal set; }

        /// <summary> Component defines the class which is used to fulfil the request </summary>
        public Microservice_Component Component { get; internal set; }

        /// <summary> Method within the class specified by the component that should be called to fulfil the request </summary>
        public string Method { get; internal set; }

        /// <summary> Flag indicates if this endpoint is enabled or disabled </summary>
        public bool Enabled { get; internal set; }

        /// <summary> If this endpoint is restricted to some IP ranges, this is the list of restriction ranges that
        /// can access this endpoint </summary>
        public List<Microservice_RestrictionRange> RestrictionRanges { get; internal set; }

        /// <summary> Request type expected for this endpoint ( either a GET or a POST ) </summary>
        public Microservice_Endpoint_RequestType_Enum RequestType { get; internal set; }

        /// <summary> Constructor for a new instance of the Microservice_VerbMapping class </summary>
        /// <param name="Method"> Method within the class specified by the component that should be called to fulfil the request </param>
        /// <param name="Enabled"> Flag indicates if this endpoint is enabled or disabled </param>
        /// <param name="Protocol"> Protocol which this endpoint utilizes ( JSON or Protocol Buffer ) </param>
        /// <param name="RequestType"> Request type expected for this endpoint ( either a GET or a POST ) </param>
        public Microservice_VerbMapping(string Method, bool Enabled, Microservice_Endpoint_Protocol_Enum Protocol, Microservice_Endpoint_RequestType_Enum RequestType)
        {
            this.Method = Method;
            this.Enabled = Enabled;
            this.Protocol = Protocol;
            this.RequestType = RequestType;
        }

        /// <summary> Invoke the method in the class specified for this endpoint, from the configuration XML file </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="QueryString"></param>
        /// <param name="RequestForm"></param>
        /// <param name="IsDebug"></param>
        public void Invoke(HttpResponse Response, List<string> UrlSegments, NameValueCollection QueryString, NameValueCollection RequestForm, bool IsDebug)
        {
            if ((methodInfo == null) || (restApiObject == null))
            {
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
               // Type restApiClassType = dllAssembly.GetType(Component.Namespace + "." + Component.Class);

                Type restApiClassType = dllAssembly.GetType("SobekCM.Engine_Library.Endpoints." + Component.Class);
                restApiObject = Activator.CreateInstance(restApiClassType);

                methodInfo = restApiClassType.GetMethod(Method);
            }

            // Invokation is different, dependingon whether this is a PUT or POST
            if (RequestType == Microservice_Endpoint_RequestType_Enum.GET)
                methodInfo.Invoke(restApiObject, new object[] { Response, UrlSegments, QueryString, Protocol, IsDebug });
            else
                methodInfo.Invoke(restApiObject, new object[] { Response, UrlSegments, Protocol, RequestForm });
        }

        /// <summary> Check to see if this endpoint can be invoked from this IP address </summary>
        /// <returns> TRUE if permitted, otherwise FALSE </returns>
        public bool AccessPermitted(string IpAddress)
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
                        if (!String.IsNullOrEmpty(thisRange.EndIp))
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
