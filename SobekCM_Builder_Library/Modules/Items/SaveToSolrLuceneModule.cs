#region Using directives

using System;
using SobekCM.Library.Settings;
using SobekCM.Library.Solr;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class SaveToSolrLuceneModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {

            // Save this to the Solr/Lucene database
            if (SobekCM_Library_Settings.Document_Solr_Index_URL.Length > 0)
            {
                try
                {
                    Solr_Controller.Update_Index(SobekCM_Library_Settings.Document_Solr_Index_URL, SobekCM_Library_Settings.Page_Solr_Index_URL, Resource.Metadata, true);
                }
                catch (Exception ee)
                {
                    OnError("Error saving data to the Solr/Lucene index.  The index may not reflect the most recent data in the METS.", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    OnError("Solr Error: " + ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);

                }
            }
        }
    }
}
