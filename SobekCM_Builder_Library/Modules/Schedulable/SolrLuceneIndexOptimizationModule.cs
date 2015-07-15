#region Using directives

using SobekCM.Core.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Schedulable
{
    /// <summary> Schedulable builder module optimizes the solr/lucene indexes for this instance </summary>
    /// <remarks> This class implements the <see cref="abstractSchedulableModule" /> abstract class and implements the <see cref="iSchedulableModule" /> interface. </remarks>
    public class SolrLuceneIndexOptimizationModule : abstractSchedulableModule
    {
        /// <summary> Optimizes the solr/lucene indexes for this instance </summary>
        /// <param name="Settings"> Instance-wide settings which may be required for this process </param>
        public override void DoWork(InstanceWide_Settings Settings)
        {
            //// Initiate a solr/lucene index optimization since we are done loading for a while
            //if (DateTime.Now.Day % 2 == 0)
            //{
            //	if (InstanceWide_Settings_Singleton.Settings.Document_Solr_Index_URL.Length > 0)
            //	{
            //		Console.WriteLine("Initiating Solr/Lucene document index optimization");
            //		Solr_Controller.Optimize_Document_Index(InstanceWide_Settings_Singleton.Settings.Document_Solr_Index_URL);
            //	}
            //}
            //else
            //{
            //	if (InstanceWide_Settings_Singleton.Settings.Page_Solr_Index_URL.Length > 0)
            //	{
            //		Console.WriteLine("Initiating Solr/Lucene page index optimization");
            //		Solr_Controller.Optimize_Page_Index(InstanceWide_Settings_Singleton.Settings.Page_Solr_Index_URL);
            //	}
            //}
            //// Sleep for twenty minutes to end this (the index rebuild might take some time)
            //Thread.Sleep(1000 * 20 * 60);
        }
    }
}
