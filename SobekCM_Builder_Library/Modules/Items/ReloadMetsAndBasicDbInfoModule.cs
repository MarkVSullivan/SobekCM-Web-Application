#region Using directives

using System;
using SobekCM.Library.Database;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module checks ... </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class ReloadMetsAndBasicDbInfoModule : abstractSubmissionPackageModule
    {
        /// <summary>  </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            // Load the METS file
            if (!Resource.Load_METS())
            {
                OnError("Error reading most recent METS file from " + Resource.BibID + ":" + Resource.VID, Resource.BibID + ":" + Resource.VID, String.Empty, Resource.BuilderLogId);
                return false;
            }

            // Add thumbnail, aggregation informaiton, and dark/access information from the database 
            if (!Resource.NewPackage)
            {
                SobekCM_Database.Add_Minimum_Builder_Information(Resource.Metadata);
            }
            else
            {
                // Check for any access/restriction/embargo date in the RightsMD section
                RightsMD_Info rightsInfo = Resource.Metadata.Get_Metadata_Module(GlobalVar.PALMM_RIGHTSMD_METADATA_MODULE_KEY) as RightsMD_Info;
                if ((rightsInfo != null) && (rightsInfo.hasData))
                {
                    switch (rightsInfo.Access_Code)
                    {
                        case RightsMD_Info.AccessCode_Enum.Campus:
                            // Was there an embargo date?
                            if (rightsInfo.Has_Embargo_End)
                            {
                                if (DateTime.Compare(DateTime.Now, rightsInfo.Embargo_End) < 0)
                                {
                                    Resource.Metadata.Behaviors.IP_Restriction_Membership = 1;
                                }
                            }
                            else
                            {
                                Resource.Metadata.Behaviors.IP_Restriction_Membership = 1;
                            }
                            break;

                        case RightsMD_Info.AccessCode_Enum.Private:
                            // Was there an embargo date?
                            if (rightsInfo.Has_Embargo_End)
                            {
                                if (DateTime.Compare(DateTime.Now, rightsInfo.Embargo_End) < 0)
                                {
                                    Resource.Metadata.Behaviors.Dark_Flag = true;
                                }
                            }
                            else
                            {
                                Resource.Metadata.Behaviors.Dark_Flag = true;
                            }
                            break;
                    }
                }
            }

            return true;
        }
    }
}
