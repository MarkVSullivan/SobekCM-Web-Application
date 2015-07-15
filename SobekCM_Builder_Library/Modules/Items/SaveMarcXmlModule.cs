#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    /// <summary> Item-level submission package module saves a MarcXML file within the digital resource folder </summary>
    /// <remarks> This class implements the <see cref="abstractSubmissionPackageModule" /> abstract class and implements the <see cref="iSubmissionPackageModule" /> interface. </remarks>
    public class SaveMarcXmlModule : abstractSubmissionPackageModule
    {
        /// <summary> Saves a MarcXML file within the digital resource folder </summary>
        /// <param name="Resource"> Incoming digital resource object </param>
        /// <returns> TRUE if processing can continue, FALSE if a critical error occurred which should stop all processing </returns>
        public override bool DoWork(Incoming_Digital_Resource Resource)
        {
            try
            {
                // Set the image location
                Resource.Metadata.Web.Image_Root = Settings.Image_URL + Resource.Metadata.Web.File_Root.Replace("\\", "/");
                Resource.Metadata.Web.Set_BibID_VID(Resource.Metadata.BibID, Resource.Metadata.VID);

                // Create the options dictionary used when saving information to the database, or writing MarcXML
                Dictionary<string, object> options = new Dictionary<string, object>();
                if (Engine_ApplicationCache_Gateway.Settings.MarcGeneration != null)
                {
                    options["MarcXML_File_ReaderWriter:MARC Cataloging Source Code"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Cataloging_Source_Code;
                    options["MarcXML_File_ReaderWriter:MARC Location Code"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Location_Code;
                    options["MarcXML_File_ReaderWriter:MARC Reproduction Agency"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Agency;
                    options["MarcXML_File_ReaderWriter:MARC Reproduction Place"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.Reproduction_Place;
                    options["MarcXML_File_ReaderWriter:MARC XSLT File"] = Engine_ApplicationCache_Gateway.Settings.MarcGeneration.XSLT_File;
                }
                options["MarcXML_File_ReaderWriter:System Name"] = Engine_ApplicationCache_Gateway.Settings.System_Name;
                options["MarcXML_File_ReaderWriter:System Abbreviation"] = Engine_ApplicationCache_Gateway.Settings.System_Abbreviation;

                // Save the marc xml file
                MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                string errorMessage;
                if (!marcWriter.Write_Metadata(Resource.Metadata.Source_Directory + "\\marc.xml", Resource.Metadata, options, out errorMessage))
                {
                    OnError("Error while saving the MarcXML : " + errorMessage, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                }

            }
            catch (Exception ee)
            {
                OnError("Exception caught while saving the MarcXML : " + ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
            }

            return true;
        }
    }
}
