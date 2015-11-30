using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.Database;
using SobekCM.Engine_Library.Items.BriefItems;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.OAI.Writer;

namespace SobekCM.Engine_Library.Configuration
{
    public class Configuration_Files_Reader
    {
        /// <summary> Refreshes the values from the database settings </summary>
        /// <returns> A fully builder instance-wide setting object </returns>
        public static InstanceWide_Configuration Build_Settings(string ConfigFileLocation, string Base_Directory, InstanceWide_Settings Settings)
        {
            InstanceWide_Configuration returnValue = new InstanceWide_Configuration();

            // Try to read the SHIBBOLETH configuration file
            if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_shibboleth.config"))
            {
                returnValue.Authentication.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(Base_Directory + "\\config\\user\\sobekcm_shibboleth.config");
            }
            else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_shibboleth.config"))
            {
                returnValue.Authentication.Shibboleth = Shibboleth_Configuration_Reader.Read_Config(Base_Directory + "\\config\\default\\sobekcm_shibboleth.config");
            }

            // Try to read the CONTACT FORM configuration file
            if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_contactform.config"))
            {
                returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(Base_Directory + "\\config\\user\\sobekcm_contactform.config");
            }
            else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_contactform.config"))
            {
                returnValue.ContactForm = ContactForm_Configuration_Reader.Read_Config(Base_Directory + "\\config\\default\\sobekcm_contactform.config");
            }

            // Try to read the QUALITY CONTROL configuration file
            //if (File.Exists(returnValue.Servers.Base_Directory + "\\config\\user\\sobekcm_qc.config"))
            //{
            //    QualityControl_Configuration.Read_Metadata_Configuration(returnValue.Servers.Base_Directory + "\\config\\user\\sobekcm_qc.config");
            //}
            //else if (File.Exists(returnValue.Servers.Base_Directory + "\\config\\default\\sobekcm_qc.config"))
            //{
            //    QualityControl_Configuration.Read_Metadata_Configuration(returnValue.Servers.Base_Directory + "\\config\\default\\sobekcm_qc.config");
            //}

            // Try to read the BRIEF ITEM MAPPING configuration file
            if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_brief_item_mapping.config"))
            {
                BriefItem_Factory.Read_Config(Base_Directory + "\\config\\user\\sobekcm_brief_item_mapping.config");
            }
            else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_brief_item_mapping.config"))
            {
                BriefItem_Factory.Read_Config(Base_Directory + "\\config\\default\\sobekcm_brief_item_mapping.config");
            }

            // Try to read the OAI-PMH configuration file
            if (File.Exists(Base_Directory + "\\config\\user\\sobekcm_oaipmh.config"))
            {
                returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(Base_Directory + "\\config\\user\\sobekcm_oaipmh.config", Settings.System.System_Name, Settings.System.System_Abbreviation, Settings.Email.System_Email);
            }
            else if (File.Exists(Base_Directory + "\\config\\default\\sobekcm_oaipmh.config"))
            {
                returnValue.OAI_PMH = OAI_PMH_Configuration_Reader.Read_Config(Base_Directory + "\\config\\default\\sobekcm_oaipmh.config", Settings.System.System_Name, Settings.System.System_Abbreviation, Settings.Email.System_Email);
            }

            // Load the OAI-PMH configuration file info into the OAI writer class ( in the resource object library )
            if (returnValue.OAI_PMH == null)
            {
                returnValue.OAI_PMH = new OAI_PMH_Configuration();
                returnValue.OAI_PMH.Set_Default();
            }

            OAI_PMH_Metadata_Writers.Clear();
            foreach (OAI_PMH_Metadata_Format thisWriter in returnValue.OAI_PMH.Metadata_Prefixes)
            {
                if (thisWriter.Enabled)
                {
                    OAI_PMH_Metadata_Writers.Add_Writer(thisWriter.Prefix, thisWriter.Assembly, thisWriter.Namespace, thisWriter.Class);
                }
            }

            return returnValue;
        }
    }
}
