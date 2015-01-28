
using System;
using SobekCM.Core.MicroservicesClient;

namespace SobekCM.Core.Client
{
    public static class SobekEngineClient
    {

        static SobekEngineClient()
        {
            Config_Read_Attempted = false;
        }

        public static bool Config_Read_Attempted { get; private set; }

        public static string Config_Read_Error { get; private set; }

        public static bool Read_Config_File(string ConfigFile)
        {
            MicroservicesClient_Configuration configObj = MicroservicesClient_Config_Reader.Read_Config(ConfigFile);
            Config_Read_Attempted = true;
            if (String.IsNullOrEmpty(configObj.Error))
            {
                Aggregations = new SobekEngineClient_AggregationEndpoints(configObj);
                WebSkins = new SobekEngineClient_WebSkinEndpoints(configObj);
                Items = new SobekEngineClient_ItemEndpoints(configObj);
                Search = new SobekEngineClient_SearchEndpoints(configObj);

                return true;
            }

            Config_Read_Error = configObj.Error;
            return false;
        }

        public static SobekEngineClient_AggregationEndpoints Aggregations { get; private set; }

        public static SobekEngineClient_WebSkinEndpoints WebSkins { get; private set; }

        public static SobekEngineClient_ItemEndpoints Items { get; private set; }

        public static SobekEngineClient_SearchEndpoints Search { get; private set; }

    }
}
