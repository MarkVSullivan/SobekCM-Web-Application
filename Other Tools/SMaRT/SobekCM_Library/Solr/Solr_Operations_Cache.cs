#region Using directives

using Microsoft.Practices.ServiceLocation;
using SolrNet;

#endregion

namespace SobekCM.Library.Solr
{
    internal static class Solr_Operations_Cache<T> where T : new() 
    { 
        private static ISolrOperations<T> solrOperations; 

        public static ISolrOperations<T> GetSolrOperations(string solrUrl) 
        { 
            if (solrOperations == null) 
            { 
                Startup.Init<T>(solrUrl); 
                solrOperations = ServiceLocator.Current.GetInstance<ISolrOperations<T>>(); 
            } 
            return solrOperations; 
        } 
    }
}
