using SobekCM.Core.Configuration;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Skins;
using SobekCM.Engine_Library.Endpoints;
using SobekCM.Tools;

namespace SobekCM.Core.Client
{
    public class SobekEngineClient_WebSkinEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_WebSkinEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_WebSkinEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }

        public Complete_Web_Skin_Object Get_Complete_Web_Skin(string SkinCode, Custom_Tracer Tracer)
        {
            return WebSkinServices.get_complete_web_skin(SkinCode, Tracer);
        }

        public Web_Skin_Object Get_Aggregation(string SkinCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, Custom_Tracer Tracer)
        {
            return WebSkinServices.get_web_skin(SkinCode, RequestedLanguage, DefaultLanguage, Tracer);
        }


        public string WebSkin_Uploaded_Files_URL
        {
            get
            {
                return Config["Get_WebSkin_Uploaded_Files"] == null ? null : Config["Get_WebSkin_Uploaded_Files"].URL;
            }
        }
    }
}
