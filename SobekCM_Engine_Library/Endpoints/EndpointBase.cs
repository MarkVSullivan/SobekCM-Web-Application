#region Using references

using System.Runtime.Serialization;
using System.Runtime.Serialization.Formatters.Binary;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Engine_Library.Microservices;

#endregion

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Base engine endpoint has helper method for serialization of the return object </summary>
    public abstract class EndpointBase
    {
        /// <summary> Serialize the return object, according to the protocol requested </summary>
        /// <param name="ReturnValue"> Return object to serialize </param>
        /// <param name="Response"> HTTP Response to write result to </param>
        /// <param name="Protocol"> Requested protocol type </param>
        /// <param name="CallbackJsonP"> Callback function for JSON-P </param>
        protected void Serialize(object ReturnValue, HttpResponse Response, Microservice_Endpoint_Protocol_Enum Protocol, string CallbackJsonP)
        {
            if (ReturnValue == null)
                return;

            switch (Protocol)
            {
                case Microservice_Endpoint_Protocol_Enum.JSON:
                    JSON.Serialize(ReturnValue, Response.Output, Options.ISO8601ExcludeNulls);
                    break;

                case Microservice_Endpoint_Protocol_Enum.PROTOBUF:
                    Serializer.Serialize(Response.OutputStream, ReturnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.JSON_P:
                    Response.Output.Write(CallbackJsonP + "(");
                    JSON.Serialize(ReturnValue, Response.Output, Options.ISO8601ExcludeNullsJSONP);
                    Response.Output.Write(");");
                    break;

                case Microservice_Endpoint_Protocol_Enum.XML:
                    System.Xml.Serialization.XmlSerializer x = new System.Xml.Serialization.XmlSerializer(ReturnValue.GetType());
                    x.Serialize(Response.Output, ReturnValue);
                    break;

                case Microservice_Endpoint_Protocol_Enum.BINARY:
                    IFormatter binary = new BinaryFormatter();
                    binary.Serialize(Response.OutputStream, ReturnValue);
                    break;
            }
        }
    }
}
