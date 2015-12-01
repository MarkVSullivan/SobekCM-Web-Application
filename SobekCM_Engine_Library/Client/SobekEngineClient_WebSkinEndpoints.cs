#region Using directives

using System;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.Skins;
using SobekCM.Engine_Library.Endpoints;
using SobekCM.Tools;

#endregion

namespace SobekCM.Core.Client
{
    /// <summary> Gateway to all the web skin-related endpoints exposed by the SobekCM engine </summary>
    public class SobekEngineClient_WebSkinEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_WebSkinEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_WebSkinEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }

        /// <summary> Get the complete web skin, by skin code </summary>
        /// <param name="SkinCode"> Code for the web skin </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Complete web skin object </returns>
        public Complete_Web_Skin_Object Get_Complete_Web_Skin(string SkinCode, Custom_Tracer Tracer)
        {
            return WebSkinServices.get_complete_web_skin(SkinCode, Tracer);
        }

        /// <summary> Get the language-specific web skin, by skin code and language </summary>
        /// <param name="SkinCode"> Code for the web skin </param>
        /// <param name="RequestedLanguage"> Requested language for the web skin code </param>
        /// <param name="DefaultLanguage"> Default UI language for the instance </param>
        /// <param name="Cache_On_Build"> Flag indicates whether to use the cache </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        /// <returns> Language-specific web skin object </returns>
        public Web_Skin_Object Get_LanguageSpecific_Web_Skin(string SkinCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, bool Cache_On_Build, Custom_Tracer Tracer)
        {
            // If no interface yet, look in the cache
            if ((SkinCode != "new") && (Cache_On_Build))
            {
                Web_Skin_Object htmlSkin = CachedDataManager.WebSkins.Retrieve_Skin(SkinCode, Web_Language_Enum_Converter.Enum_To_Code(RequestedLanguage), Tracer);
                if (htmlSkin != null)
                {
                    if (Tracer != null)
                    {
                        Tracer.Add_Trace("SobekEngineClient_WebSkinEndpoints.Get_LanguageSpecific_Web_Skin", "Web skin '" + SkinCode + "' found in cache");
                    }
                    return htmlSkin;
                }
            }

            // If still not interface, build one
            Web_Skin_Object new_skin = WebSkinServices.get_web_skin(SkinCode, RequestedLanguage, DefaultLanguage, Tracer);

            // Look in the web skin row and see if it should be kept around, rather than momentarily cached
            if ((new_skin != null) && ( String.IsNullOrEmpty(new_skin.Exception)))
            {
                if (Cache_On_Build)
                {
                    // Momentarily cache this web skin object
                    CachedDataManager.WebSkins.Store_Skin(SkinCode, Web_Language_Enum_Converter.Enum_To_Code(RequestedLanguage), new_skin, Tracer);
                }
            }

            return new_skin;
        }

        /// <summary> Returns the URL to be used by the HTML editing library for retrieving the list of uploads for a web skin </summary>
        public string WebSkin_Uploaded_Files_URL
        {
            get
            {
                return Config["Get_WebSkin_Uploaded_Files"] == null ? null : Config["Get_WebSkin_Uploaded_Files"].URL;
            }
        }
    }
}
