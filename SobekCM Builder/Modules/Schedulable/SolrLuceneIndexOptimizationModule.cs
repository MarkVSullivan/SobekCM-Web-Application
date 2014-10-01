using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Builder.Modules.Schedulable
{
    public class SolrLuceneIndexOptimizationModule : abstractSchedulableModule
    {
        public override void DoWork()
        {
            //// Initiate a solr/lucene index optimization since we are done loading for a while
            //if (DateTime.Now.Day % 2 == 0)
            //{
            //	if (SobekCM_Library_Settings.Document_Solr_Index_URL.Length > 0)
            //	{
            //		Console.WriteLine("Initiating Solr/Lucene document index optimization");
            //		Solr_Controller.Optimize_Document_Index(SobekCM_Library_Settings.Document_Solr_Index_URL);
            //	}
            //}
            //else
            //{
            //	if (SobekCM_Library_Settings.Page_Solr_Index_URL.Length > 0)
            //	{
            //		Console.WriteLine("Initiating Solr/Lucene page index optimization");
            //		Solr_Controller.Optimize_Page_Index(SobekCM_Library_Settings.Page_Solr_Index_URL);
            //	}
            //}
            //// Sleep for twenty minutes to end this (the index rebuild might take some time)
            //Thread.Sleep(1000 * 20 * 60);
        }
    }
}
