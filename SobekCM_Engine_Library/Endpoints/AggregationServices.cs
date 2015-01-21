using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using Jil;
using ProtoBuf;
using SobekCM.Core.Aggregations;
using SobekCM.Engine_Library.Microservices;
using SobekCM.Tools;

namespace SobekCM.Engine_Library.Endpoints
{
    public class AggregationServices
    {
        public void GetAggregationByCode(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol)
        {
            
            if (UrlSegments.Count > 0)
            {
                Custom_Tracer tracer = new Custom_Tracer();

                string AggrCode = UrlSegments[0];

                Item_Aggregation returnValue = Aggregations.Item_Aggregation_Utilities.Get_Item_Aggregation(AggrCode, String.Empty, null, false, false, tracer);


                if (Protocol == Microservice_Endpoint_Protocol_Enum.JSON)
                {
                    JSON.Serialize(returnValue, Response.Output, Options.ISO8601ExcludeNulls);
                }
                else
                {
                    Serializer.Serialize(Response.OutputStream, returnValue);
                }

                Response.End();
            }
        }
    }
}
