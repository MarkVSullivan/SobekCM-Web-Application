#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using SobekCM.Core.Aggregations;
using SobekCM.Core.Settings;
using SobekCM.Engine_Library.Aggregations;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Database;
using SobekCM.Library;

using SobekCM.Library.Database;
using SobekCM.UI_Library;

#endregion

namespace SobekCM.Builder_Library.Modules.PostProcess
{
    public class BuildAggregationBrowsesModule : abstractPostProcessModule
    {
        public override void DoWork(List<string> AggregationsAffected, List<BibVidStruct> ProcessedItems, List<BibVidStruct> DeletedItems, InstanceWide_Settings Settings)
        {
            if (AggregationsAffected.Count == 0)
                return;

            long updatedId = OnProcess("....Performing some aggregation update functions", "Aggregation Updates", String.Empty, String.Empty, -1);

            // Create the new statics page builder
            // IN THIS CASE, WE DO NEED TO SET THE SINGLETON, SINCE THIS CALLS THE LIBRARIES
            /// TODO: Start to pull this page directly from the web server, not builder here
            Engine_ApplicationCache_Gateway.Settings = Settings;
            Static_Pages_Builder staticBuilder = new Static_Pages_Builder(Settings.Application_Server_URL, Settings.Static_Pages_Location, Settings.Application_Server_Network);

            // Step through each aggregation with new items
            foreach (string thisAggrCode in AggregationsAffected)
            {
                // Some aggregations can be excluded
                if ((thisAggrCode != "IUF") && (thisAggrCode != "ALL") && (thisAggrCode.Length > 1))
                {
                    // Get the display aggregation code (lower leading 'i')
                    string display_code = thisAggrCode;
                    if (display_code[0] == 'I')
                        display_code = 'i' + display_code.Substring(1);

                    // Get this item aggregations
                    Complete_Item_Aggregation aggregationCompleteObj = Engine_Database.Get_Item_Aggregation(thisAggrCode, false, false, null);
                    Item_Aggregation aggregationObj = Item_Aggregation_Utilities.Get_Item_Aggregation( aggregationCompleteObj, Engine_ApplicationCache_Gateway.Settings.Default_UI_Language, null);

                    // Get the list of items for this aggregation
                    DataSet aggregation_items = Engine_Database.Simple_Item_List(thisAggrCode, null);

                    // Create the XML list for this aggregation
                    OnProcess("........Building XML item list for " + display_code, "Aggregation Updates", String.Empty, String.Empty, updatedId);
                    try
                    {
                        string aggregation_list_file = Settings.Static_Pages_Location + "\\" + thisAggrCode.ToLower() + ".xml";
                        if (File.Exists(aggregation_list_file))
                            File.Delete(aggregation_list_file);
                        aggregation_items.WriteXml(aggregation_list_file, XmlWriteMode.WriteSchema);
                    }
                    catch (Exception ee)
                    {
                        OnError("........Error in building XML list for " + display_code + " on " + Settings.Static_Pages_Location + "\n" + ee.Message, String.Empty, String.Empty, updatedId);
                    }

                    OnProcess("........Building RSS feed for " + display_code, "Aggregation Updates", String.Empty, String.Empty, updatedId);
                    try
                    {
                        staticBuilder.Create_RSS_Feed(thisAggrCode.ToLower(), Settings.Local_Log_Directory, aggregationObj.Name, aggregation_items);
                        try
                        {
                            File.Copy(Settings.Local_Log_Directory + thisAggrCode.ToLower() + "_rss.xml", Settings.Static_Pages_Location + "\\rss\\" + thisAggrCode.ToLower() + "_rss.xml", true);
                            File.Copy(Settings.Local_Log_Directory + thisAggrCode.ToLower() + "_short_rss.xml", Settings.Static_Pages_Location + "\\rss\\" + thisAggrCode.ToLower() + "_short_rss.xml", true);
                        }
                        catch (Exception ee)
                        {
                            OnError("........Error in copying RSS feed for " + display_code + " to " + Settings.Static_Pages_Location + "\n" + ee.Message, String.Empty, String.Empty, updatedId);
                        }
                    }
                    catch (Exception ee)
                    {
                        OnError("........Error in building RSS feed for " + display_code + "\n" + ee.Message, String.Empty, String.Empty, updatedId);
                    }

                    OnProcess("........Building static HTML browse page of links for " + display_code, "Aggregation Updates", String.Empty, String.Empty, updatedId);
                    try
                    {
                        staticBuilder.Build_All_Browse(aggregationObj, aggregation_items);
                        try
                        {
                            File.Copy(Settings.Local_Log_Directory + thisAggrCode.ToLower() + "_rss.xml", Settings.Static_Pages_Location + "\\rss\\" + thisAggrCode.ToLower() + "_rss.xml", true);
                            File.Copy(Settings.Local_Log_Directory + thisAggrCode.ToLower() + "_short_rss.xml", Settings.Static_Pages_Location + "\\rss\\" + thisAggrCode.ToLower() + "_short_rss.xml", true);
                        }
                        catch (Exception ee)
                        {
                            OnError("........Error in copying RSS feed for " + display_code + " to " + Settings.Static_Pages_Location + "\n" + ee.Message, String.Empty, String.Empty, updatedId);
                        }
                    }
                    catch (Exception ee)
                    {
                        OnError("........Error in building RSS feed for " + display_code + "\n" + ee.Message, String.Empty, String.Empty, updatedId);
                    }


                }
            }

            // Build the full instance-wide XML and RSS here as well
            Recreate_Library_XML_and_RSS(updatedId, staticBuilder);
        }

        private void Recreate_Library_XML_and_RSS(long Builderid, Static_Pages_Builder StaticBuilder )
        {
            // Update the RSS Feeds and Item Lists for ALL 
            // Build the simple XML result for this build
            OnProcess("........Building XML list for all digital resources", "Aggregation Updates", String.Empty, String.Empty, Builderid);
            try
            {
                DataSet simple_list = Engine_Database.Simple_Item_List(String.Empty, null);
                if (simple_list != null)
                {
                    try
                    {
                        string aggregation_list_file = Settings.Static_Pages_Location + "\\all.xml";
                        if (File.Exists(aggregation_list_file))
                            File.Delete(aggregation_list_file);
                        simple_list.WriteXml(aggregation_list_file, XmlWriteMode.WriteSchema);
                    }
                    catch (Exception ee)
                    {
                        OnError("........Error in building XML list for all digital resources on " + Settings.Static_Pages_Location + "\n" + ee.Message, String.Empty, String.Empty, Builderid);
                    }
                }
            }
            catch (Exception ee)
            {
                OnError("........Error in building XML list for all digital resources\n" + ee.Message, String.Empty, String.Empty, Builderid);
            }

            // Create the RSS feed for all ufdc items
            try
            {
                OnProcess("........Building RSS feed for all digital resources", "Aggregation Updates", String.Empty, String.Empty, Builderid);
                DataSet complete_list = Engine_Database.Simple_Item_List(String.Empty, null);

                StaticBuilder.Create_RSS_Feed("all", Settings.Local_Log_Directory, "All Items", complete_list);
                try
                {
                    File.Copy(Settings.Local_Log_Directory + "all_rss.xml", Settings.Static_Pages_Location + "\\rss\\all_rss.xml", true);
                    File.Copy(Settings.Local_Log_Directory + "all_short_rss.xml", Settings.Static_Pages_Location + "\\rss\\all_short_rss.xml", true);
                }
                catch (Exception ee)
                {
                    OnError("........Error in copying RSS feed for all digital resources to " + Settings.Static_Pages_Location + "\n" + ee.Message, String.Empty, String.Empty, Builderid);
                }
            }
            catch (Exception ee)
            {
                OnError("........Error in building RSS feed for all digital resources\n" + ee.Message, String.Empty, String.Empty, Builderid);
            }
        }
    }
}
