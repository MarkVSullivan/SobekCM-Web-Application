using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Library.Builder;

namespace SobekCM.Builder.Modules.Items
{
    public class SaveServiceMetsModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            try
            {
                Resource.Metadata.Save_SobekCM_METS();
            }
            catch (Exception ee)
            {
                OnError("Exception caught while saving the SobekCM service METS : " + ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }
        }
    }
}
