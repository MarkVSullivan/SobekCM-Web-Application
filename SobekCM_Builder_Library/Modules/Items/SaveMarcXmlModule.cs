#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Library.Settings;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class SaveMarcXmlModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            try
            {
                // Set the image location
                Resource.Metadata.Web.Image_Root = Settings.Image_URL + Resource.Metadata.Web.File_Root.Replace("\\", "/");
                Resource.Metadata.Web.Set_BibID_VID(Resource.Metadata.BibID, Resource.Metadata.VID);


                List<string> collectionnames = new List<string>();

                // Save the marc xml file
                MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                string errorMessage;
                Dictionary<string, object> options = new Dictionary<string, object>();
                options["MarcXML_File_ReaderWriter:Additional_Tags"] = Resource.Metadata.MARC_Sobek_Standard_Tags(collectionnames, true, Settings.System_Name, Settings.System_Abbreviation);
                if (!marcWriter.Write_Metadata(Resource.Metadata.Source_Directory + "\\marc.xml", Resource.Metadata, options, out errorMessage))
                {
                    OnError("Error while saving the MarcXML : " + errorMessage, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                }

            }
            catch (Exception ee)
            {
                OnError("Exception caught while saving the MarcXML : " + ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }
        }
    }
}
