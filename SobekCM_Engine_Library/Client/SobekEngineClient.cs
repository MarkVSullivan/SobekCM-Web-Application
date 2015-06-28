#region Using directives

using System;
using SobekCM.Core.MicroservicesClient;

#endregion

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the endpoints exposed by the SobekCM engine and consumed by the application </summary>
    /// <remarks> This will not have access to the endpoints that are provided ONLY for use by the end user.  This
    /// only contains the endpoints that are actually consumed by the web UI. </remarks>
    public static class SobekEngineClient
    {
        /// <summary> Static constructor for the SobekEngineClient class </summary>
        static SobekEngineClient()
        {
            Config_Read_Attempted = false;
        }

        /// <summary> Flag indicates if an attempt was made to read the microservices configuration file </summary>
        public static bool Config_Read_Attempted { get; private set; }

        /// <summary> Flag indicates if an error was encountered while reading the microsservices configuration file </summary>
        public static string Config_Read_Error { get; private set; }

        /// <summary> Read the microservices configuration file </summary>
        /// <param name="ConfigFile"> File ( including path )</param>
        /// <param name="SystemBaseUrl"></param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This also sets the <see cref="Config_Read_Attempted"/> flag to TRUE and set the <see cref="Config_Read_Error"/> flag </remarks>
        public static bool Read_Config_File(string ConfigFile, string SystemBaseUrl)
        {
            MicroservicesClient_Configuration configObj = MicroservicesClient_Config_Reader.Read_Config(ConfigFile, SystemBaseUrl);
            Config_Read_Attempted = true;
            if (String.IsNullOrEmpty(configObj.Error))
            {
                Aggregations = new SobekEngineClient_AggregationEndpoints(configObj);
                WebSkins = new SobekEngineClient_WebSkinEndpoints(configObj);
                Items = new SobekEngineClient_ItemEndpoints(configObj);
                Search = new SobekEngineClient_SearchEndpoints(configObj);
                WebContent = new SobekEngineClient_WebContentEndpoints(configObj);
                Navigation = new SobekEngineClient_NavigationEndpoints(configObj);

                return true;
            }

            Config_Read_Error = configObj.Error;
            return false;
        }

        /// <summary> Aggregation-related endpoints exposed by the SobekCM engine </summary>
        public static SobekEngineClient_AggregationEndpoints Aggregations { get; private set; }

        /// <summary> Web skin-related endpoints exposed by the SobekCM engine </summary>
        public static SobekEngineClient_WebSkinEndpoints WebSkins { get; private set; }

        /// <summary> Item-related endpoints exposed by the SobekCM engine </summary>
        public static SobekEngineClient_ItemEndpoints Items { get; private set; }

        /// <summary> Search-related endpoints exposed by the SobekCM engine </summary>
        public static SobekEngineClient_SearchEndpoints Search { get; private set; }

        /// <summary> Web Content-related endpoints exposed by the SobekCM engine </summary>
        public static SobekEngineClient_WebContentEndpoints WebContent { get; private set; }

        /// <summary> Navigation-related endpoints exposed by the SobekCM engine </summary>
        public static SobekEngineClient_NavigationEndpoints Navigation { get; private set; }

    }
}
