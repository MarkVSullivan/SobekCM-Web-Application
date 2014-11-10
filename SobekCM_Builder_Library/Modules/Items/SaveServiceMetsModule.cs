#region Using directives

using System;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class SaveServiceMetsModule : abstractSubmissionPackageModule
    {
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            try
            {
                Resource.Metadata.Save_SobekCM_METS();
            }
            catch (Exception ee)
            {
                OnError("Exception caught while saving the SobekCM service METS : " + ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                return false;
            }

            return true;
        }
    }
}
