using SobekCM.Core.Aggregations;
using SobekCM.Core.Configuration;
using SobekCM.Core.MemoryMgmt;
using SobekCM.Core.MicroservicesClient;
using SobekCM.Core.WebContent;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.Endpoints;
using SobekCM.Tools;

namespace SobekCM.Core.Client
{
    public class SobekEngineClient_AggregationEndpoints : MicroservicesClientBase
    {
        /// <summary> Constructor for a new instance of the SobekEngineClient_AggregationEndpoints class </summary>
        /// <param name="ConfigObj"> Fully constructed microservices client configuration </param>
        public SobekEngineClient_AggregationEndpoints(MicroservicesClient_Configuration ConfigObj) : base(ConfigObj)
        {
            // All work done in the base constructor
        }

        public Complete_Item_Aggregation Get_Complete_Aggregation(string AggregationCode, bool isRobot, Custom_Tracer Tracer)
        {
            return AggregationServices.get_complete_aggregation(AggregationCode, isRobot, Tracer);
        }

        public Item_Aggregation Get_Aggregation(string AggregationCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, bool isRobot, Custom_Tracer Tracer)
        {
            return AggregationServices.get_item_aggregation(AggregationCode, RequestedLanguage, DefaultLanguage, isRobot, Tracer);
        }

        public HTML_Based_Content Get_Aggregation_HTML_Child_Page(string AggregationCode, Web_Language_Enum RequestedLanguage, Web_Language_Enum DefaultLanguage, bool isRobot, string ChildPageCode, Custom_Tracer Tracer )
        {
            return AggregationServices.get_item_aggregation_html_child_page(AggregationCode, RequestedLanguage, DefaultLanguage, isRobot, ChildPageCode, Tracer);
        }
    }
}
