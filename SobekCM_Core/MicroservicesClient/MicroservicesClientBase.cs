#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Net;
using System.Text;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.MicroservicesClient
{
    /// <summary> Base class used for all microservice clients that includes helper methods </summary>
    public abstract class MicroservicesClientBase
    {
        /// <summary> Configuration information for microservice client endpoints </summary>
        protected static MicroservicesClient_Configuration Config;

        /// <summary> Constructor for a new instance of the MicroservicesClientBase class </summary>
        /// <param name="ConfigFile"> Location for the configuration file to read </param>
        /// <param name="SystemBaseUrl"> System base URL </param>
        protected MicroservicesClientBase(string ConfigFile, string SystemBaseUrl )
        {
            if ( Config == null )
                Config = MicroservicesClient_Config_Reader.Read_Config(ConfigFile, SystemBaseUrl);
        }

        /// <summary> Constructor for a new instance of the MicroservicesClientBase class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        protected MicroservicesClientBase(MicroservicesClient_Configuration ConfigObj)
        {
            Config = ConfigObj;
        }

        /// <summary> Directly set the microservice configuration </summary>
        /// <param name="EndpointConfig"> Configuration of all the endpoint information </param>
        public static void Set_Endpoints(MicroservicesClient_Configuration EndpointConfig)
        {
            Config = EndpointConfig;
        }

        #region Helper methods

        /// <summary> Gets the endpoint information from the microservices client configuration </summary>
        /// <param name="Key"> Lookup key for this configuration information </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Requested endpoint client configuration, or throws an ApplicationException </returns>
        /// <exception cref="ApplicationException"> If the key is not present in the configuration, an exception is thrown with the message
        /// 'No microservice endpoint defined in the client application for key'. </exception>
        protected MicroservicesClient_Endpoint GetEndpointConfig(string Key, Custom_Tracer Tracer )
        {
            MicroservicesClient_Endpoint endpoint = Config[Key];
            if (endpoint == null)
            {
                if ( Tracer != null )
                    Tracer.Add_Trace("MicroservicesClientBase.GetEndpointConfig", "No microservice endpoint defined in the client application for key '" + Key + "'", Custom_Trace_Type_Enum.Error); 
                throw new ApplicationException("No microservice endpoint defined in the client application for key '" + Key + "'");
            }

            return endpoint;
        }

        /// <summary> Deserialize an object from a remote microservice URI (Generic method) </summary>
        /// <typeparam name="T"> Type of object to deserialize from the URI response </typeparam>
        /// <param name="MicroserviceUri"> URI for the remote microservice to call </param>
        /// <param name="MicroserviceProtocol"> Protocol to use for the deserialization ( i.e., JSON, Protocol buffer, etc.. ) </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> An object of the type requested, from the serializing effort </returns>
        /// <remarks> This only works for simple GET requests at the moment, as no object is POSTed to the remote microservice URL </remarks>
        protected T Deserialize<T>(string MicroserviceUri, Microservice_Endpoint_Protocol_Enum MicroserviceProtocol, Custom_Tracer Tracer )
        {
            try
            {
                // Add a trace
                if ( Tracer != null )
                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Microservice endpoint call: [GET] " + MicroserviceUri );

                // Create the request for the remote microservice, by URI
                WebRequest request = WebRequest.Create(MicroserviceUri);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = "GET";

                // Send the request and (hopefully) get the response
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();

                // If the datastream is null, some unknown exception occurred here.. (seems like an exception should already have been thrown though)
                if (dataStream == null)
                {
                    if (Tracer != null)
                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Unable to get the response stream from the web response while connecting to microservice URL", Custom_Trace_Type_Enum.Error);
                    throw new ApplicationException("Unable to get the response stream from the web response while connecting to microservice URL ( '" + MicroserviceUri + " ').");
                }

                // Deserialize the resulting datastream
                T returnValue;
                if (MicroserviceProtocol == Microservice_Endpoint_Protocol_Enum.JSON)
                {
                    // Deserialize using the JIL JSON library 
                    TextReader reader = new StreamReader(dataStream);
                    string json = reader.ReadToEnd();


                    returnValue = JSON.Deserialize<T>(json);
                    reader.Close();
                }
                else
                {
                    // Deserialize using the Protocol buffer-net library
                    returnValue = Serializer.Deserialize<T>(dataStream);
                }

                // Close the datastreams and response
                dataStream.Close();
                response.Close();

                return returnValue;
            }
            catch (NotSupportedException ee)
            {
                if (ee.Message == "The URI prefix is not recognized.")
                {
                    if (Tracer != null)
                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Microservice URL is not a supported format due to invalid URI prefix", Custom_Trace_Type_Enum.Error );
                    throw new ApplicationException("Microservice URL ( '" + MicroserviceUri + "' ) is not a supported format due to invalid URI prefix.", ee);
                }

                throw;
            }
            catch (UriFormatException ee)
            {
                if (Tracer != null)
                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Microservice URL is invalid format", Custom_Trace_Type_Enum.Error);
                throw new ApplicationException("Microservice URL ( '" + MicroserviceUri + "' ) is invalid format.", ee);
            }
            catch (WebException ee)
            {
                switch (ee.Status)
                {
                    case WebExceptionStatus.ConnectFailure:
                        if (Tracer != null)
                            Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Connection failure while connecting to microservice URL: " + ee.Message, Custom_Trace_Type_Enum.Error);
                        throw new ApplicationException("Connection failure while connecting to microservice URL ( '" + MicroserviceUri + "' ): " + ee.Message, ee);

                    case WebExceptionStatus.ProtocolError:
                        using (HttpWebResponse response = (HttpWebResponse)ee.Response)
                        {

                            switch (response.StatusCode)
                            {
                                case HttpStatusCode.NotImplemented:
                                    if (Tracer != null)
                                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "No matching endpoint implemented for microservice URL", Custom_Trace_Type_Enum.Error);
                                    throw new ApplicationException("No matching endpoint implemented for microservice URL ( '" + MicroserviceUri + "' )", ee);

                                case HttpStatusCode.BadRequest:
                                    // Deserialize the resulting web fault object
                                    Stream responseStream = response.GetResponseStream();
                                    if (responseStream != null)
                                    {
                                        TextReader reader = new StreamReader(responseStream);
                                        string responsemsg = reader.ReadToEnd();

                                        if (Tracer != null)
                                            Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Bad request sent to microservice URL: " + responsemsg, Custom_Trace_Type_Enum.Error);
                                        throw new ApplicationException("Bad request sent to microservice URL ( '" + MicroserviceUri + "' ): " + responsemsg, ee);
                                    }
                                    if (Tracer != null)
                                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Bad request sent to microservice URL", Custom_Trace_Type_Enum.Error);
                                    throw new ApplicationException("Bad request sent to microservice URL ( '" + MicroserviceUri + "' )", ee);


                                default:
                                    if (Tracer != null)
                                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Protocol error returned from microservice URL: " + ee.Message, Custom_Trace_Type_Enum.Error);
                                    throw new ApplicationException("Protocol error returned from microservice URL ( '" + MicroserviceUri + "' ): " + ee.Message, ee);
                            }
                        }

                    case WebExceptionStatus.Timeout:
                        if (Tracer != null)
                            Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Timeout experienced while waiting for response from microservice URL", Custom_Trace_Type_Enum.Error);
                        throw new ApplicationException("Timeout experienced while waiting for response from microservice URL ( '" + MicroserviceUri + "' ).", ee);

                    default:
                        if (Tracer != null)
                            Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Unexpected web exception connecting to microservice URL: " + ee.Message, Custom_Trace_Type_Enum.Error);
                        throw new ApplicationException("Unexpected web exception connecting to microservice URL ( '" + MicroserviceUri + "' ): " + ee.Message, ee);
                }
            }
            catch (DeserializationException ee)
            {
                if (Tracer != null)
                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Error deserializing the JSON response from microservice URL into " + typeof(T) + ".  (" + ee.Message + ")", Custom_Trace_Type_Enum.Error);
                throw new ApplicationException("Error deserializing the JSON response from microservice URL ( '" + MicroserviceUri + "' ) into " + typeof(T) + ".  (" + ee.Message + ")", ee);
            }
            catch (ProtoException ee)
            {
                if (Tracer != null)
                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Error deserializing the Protocol Buffer response from microservice URL into " + typeof(T) + ".  (" + ee.Message + ")", Custom_Trace_Type_Enum.Error);
                throw new ApplicationException("Error deserializing the Protocol Buffer response from microservice URL ( '" + MicroserviceUri + "' ) into " + typeof(T) + ".  (" + ee.Message + ")", ee);
            }

            // Other exceptions are thrown
        }

        /// <summary> Deserialize an object from a remote microservice URI (Generic method) </summary>
        /// <typeparam name="T"> Type of object to deserialize from the URI response </typeparam>
        /// <param name="MicroserviceUri"> URI for the remote microservice to call </param>
        /// <param name="MicroserviceProtocol"> Protocol to use for the deserialization ( i.e., JSON, Protocol buffer, etc.. ) </param>
        /// <param name="PostData"> Data that should be posted to the microservice endpoint for this request </param>
        /// <param name="Tracer">  Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> An object of the type requested, from the serializing effort </returns>
        /// <remarks> This only works for simple GET requests at the moment, as no object is POSTed to the remote microservice URL </remarks>
        protected T Deserialize<T>(string MicroserviceUri, Microservice_Endpoint_Protocol_Enum MicroserviceProtocol, List<KeyValuePair<string, string>> PostData, string VerbMethod, Custom_Tracer Tracer )
        {
            try
            {
                // Add a trace
                Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Microservice endpoint call: [" + VerbMethod + "] " + MicroserviceUri);

                // Create the request for the remote microservice, by URI
                WebRequest request = WebRequest.Create(MicroserviceUri);
                request.Credentials = CredentialCache.DefaultCredentials;
                request.Method = VerbMethod;
                request.ContentType = "application/x-www-form-urlencoded";

                // Build and encode the post data
                NameValueCollection outgoingQueryString = HttpUtility.ParseQueryString(String.Empty);
                if (PostData != null)
                {
                    foreach (KeyValuePair<string, string> thisFieldData in PostData)
                    {
                        outgoingQueryString.Add(thisFieldData.Key, thisFieldData.Value);
                    }
                }
                byte[] byteArray = Encoding.UTF8.GetBytes(outgoingQueryString.ToString());

                // Set the ContentLength property of the WebRequest.
                request.ContentLength = byteArray.Length;

                // Get the request stream and add the post data
                using (Stream requestStream = request.GetRequestStream())
                {
                    // Write the data to the request stream.
                    requestStream.Write(byteArray, 0, byteArray.Length);

                    // Close the Stream object.
                    requestStream.Close();
                }

                // Send the request and (hopefully) get the response
                WebResponse response = request.GetResponse();
                Stream dataStream = response.GetResponseStream();

                // If the datastream is null, some unknown exception occurred here.. (seems like an exception should already have been thrown though)
                if (dataStream == null)
                {
                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Unable to get the response stream from the web response while connecting to microservice URL", Custom_Trace_Type_Enum.Error);
                    throw new ApplicationException("Unable to get the response stream from the web response while connecting to microservice URL ( '" + MicroserviceUri + " ').");
                }

                // Deserialize the resulting datastream
                T returnValue;
                if (MicroserviceProtocol == Microservice_Endpoint_Protocol_Enum.JSON)
                {
                    // Deserialize using the JIL JSON library 
                    TextReader reader = new StreamReader(dataStream);
                    string json = reader.ReadToEnd();


                    returnValue = JSON.Deserialize<T>(json);
                    reader.Close();
                }
                else
                {
                    // Deserialize using the Protocol buffer-net library
                    returnValue = Serializer.Deserialize<T>(dataStream);
                }

                // Close the datastreams and response
                dataStream.Close();
                response.Close();

                return returnValue;
            }
            catch (NotSupportedException ee)
            {
                if (ee.Message == "The URI prefix is not recognized.")
                {
                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Microservice URL is not a supported format due to invalid URI prefix", Custom_Trace_Type_Enum.Error);
                    throw new ApplicationException("Microservice URL ( '" + MicroserviceUri + "' ) is not a supported format due to invalid URI prefix.", ee);
                }

                throw;
            }
            catch (UriFormatException ee)
            {
                Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Microservice URL is invalid format", Custom_Trace_Type_Enum.Error);
                throw new ApplicationException("Microservice URL ( '" + MicroserviceUri + "' ) is invalid format.", ee);
            }
            catch (WebException ee)
            {
                switch (ee.Status)
                {
                    case WebExceptionStatus.ConnectFailure:
                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Connection failure while connecting to microservice URL: " + ee.Message, Custom_Trace_Type_Enum.Error);
                        throw new ApplicationException("Connection failure while connecting to microservice URL ( '" + MicroserviceUri + "' ): " + ee.Message, ee);

                    case WebExceptionStatus.ProtocolError:
                        using (HttpWebResponse response = (HttpWebResponse)ee.Response)
                        {

                            switch (response.StatusCode)
                            {
                                case HttpStatusCode.NotImplemented:
                                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "No matching endpoint implemented for microservice URL", Custom_Trace_Type_Enum.Error);
                                    throw new ApplicationException("No matching endpoint implemented for microservice URL ( '" + MicroserviceUri + "' )", ee);

                                case HttpStatusCode.BadRequest:
                                    // Deserialize the resulting web fault object
                                    Stream responseStream = response.GetResponseStream();
                                    if (responseStream != null)
                                    {
                                        TextReader reader = new StreamReader(responseStream);
                                        string responsemsg = reader.ReadToEnd();

                                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Bad request sent to microservice URL: " + responsemsg, Custom_Trace_Type_Enum.Error);
                                        throw new ApplicationException("Bad request sent to microservice URL ( '" + MicroserviceUri + "' ): " + responsemsg, ee);
                                    }
                                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Bad request sent to microservice URL", Custom_Trace_Type_Enum.Error);
                                    throw new ApplicationException("Bad request sent to microservice URL ( '" + MicroserviceUri + "' )", ee);


                                default:
                                    Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Protocol error returned from microservice URL: " + ee.Message, Custom_Trace_Type_Enum.Error);
                                    throw new ApplicationException("Protocol error returned from microservice URL ( '" + MicroserviceUri + "' ): " + ee.Message, ee);
                            }
                        }

                    case WebExceptionStatus.Timeout:
                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Timeout experienced while waiting for response from microservice URL", Custom_Trace_Type_Enum.Error);
                        throw new ApplicationException("Timeout experienced while waiting for response from microservice URL ( '" + MicroserviceUri + "' ).", ee);

                    default:
                        Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Unexpected web exception connecting to microservice URL: " + ee.Message, Custom_Trace_Type_Enum.Error);
                        throw new ApplicationException("Unexpected web exception connecting to microservice URL ( '" + MicroserviceUri + "' ): " + ee.Message, ee);
                }
            }
            catch (DeserializationException ee)
            {
                Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Error deserializing the JSON response from microservice URL into " + typeof(T) + ".  (" + ee.Message + ")", Custom_Trace_Type_Enum.Error);
                throw new ApplicationException("Error deserializing the JSON response from microservice URL ( '" + MicroserviceUri + "' ) into " + typeof(T) + ".  (" + ee.Message + ")", ee);
            }
            catch (ProtoException ee)
            {
                Tracer.Add_Trace("MicroservicesClientBase.Deserialize", "Error deserializing the Protocol Buffer response from microservice URL into " + typeof(T) + ".  (" + ee.Message + ")", Custom_Trace_Type_Enum.Error);
                throw new ApplicationException("Error deserializing the Protocol Buffer response from microservice URL ( '" + MicroserviceUri + "' ) into " + typeof(T) + ".  (" + ee.Message + ")", ee);
            }
            
            // Other exceptions are thrown
        }

        #endregion

        // ReSharper disable UnusedMember.Local

        private object ExampleGetMethod(int PrimaryKey, Custom_Tracer Tracer )
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("ConfigUrl1", Tracer);

            Tracer.Add_Trace("ExamplePostMethod", "Calling microservice endpoint at: " + endpoint.URL);

            // Call out to the endpoint and return the deserialized object
            return Deserialize<object>(String.Format(endpoint.URL, PrimaryKey), endpoint.Protocol, Tracer);
        }

        private string ExamplePostMethod(string UserId, object RemoveObject, Custom_Tracer Tracer)
        {
            // Get the endpoint
            MicroservicesClient_Endpoint endpoint = GetEndpointConfig("ConfigUrl1", Tracer);

            // Create the post data
            List<KeyValuePair<string, string>> postData = new List<KeyValuePair<string, string>>
            {
                new KeyValuePair<string, string>("UserId", UserId), 
                new KeyValuePair<string, string>("RemoveObject", JSON.Serialize(RemoveObject))
            };

            Tracer.Add_Trace("ExamplePostMethod", "Calling microservice endpoint at: " + endpoint.URL );

            // Call out to the endpoint and return the deserialized object
            return Deserialize<string>(endpoint.URL, endpoint.Protocol, postData, "POST", Tracer);
        }

        // ReSharper restore UnusedMember.Local
    }
}
