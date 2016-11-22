using System;
using System.Collections.Generic;
using System.Linq;
using SobekCM.Core.Aggregations;
using SobekCM.Core.BriefItem;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;

namespace SobekCM.Engine_Library.Items.BriefItems.Mappers
{
    /// <summary> Maps all the behavior information from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Behaviors_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            // Ensure the behaviors object exists
            if (New.Behaviors == null) New.Behaviors = new BriefItem_Behaviors();

            // Copy aggregation codes over
            if (Original.Behaviors.Aggregation_Code_List != null)
            {
                New.Behaviors.Aggregation_Code_List = Original.Behaviors.Aggregation_Code_List.ToList();

                // Add the collections as well
                foreach (string aggrCode in Original.Behaviors.Aggregation_Code_List)
                {
                    Item_Aggregation_Related_Aggregations thisAggr = Engine_ApplicationCache_Gateway.Codes[aggrCode];
                    if ((thisAggr != null) && (thisAggr.Active))
                    {
                        New.Add_Description("Aggregation", thisAggr.Name).Add_URI(Engine_ApplicationCache_Gateway.Settings.Servers.Base_URL + thisAggr.Code);
                    }
                }
            }

            // Copy over the source and holing
            New.Behaviors.Holding_Location_Aggregation = Original.Bib_Info.HoldingCode;
            if (Original.Bib_Info.Source != null)
                New.Behaviors.Source_Institution_Aggregation = Original.Bib_Info.Source.Code;

            // Copy the behavior information
            New.Behaviors.Dark_Flag = Original.Behaviors.Dark_Flag;
            New.Behaviors.Embedded_Video = Original.Behaviors.Embedded_Video;
            New.Behaviors.GroupTitle = Original.Behaviors.GroupTitle;
            New.Behaviors.GroupType = Original.Behaviors.GroupType;
            New.Behaviors.IP_Restriction_Membership = Original.Behaviors.IP_Restriction_Membership;
            New.Behaviors.Single_Use = Original.Behaviors.CheckOut_Required;
            New.Behaviors.Main_Thumbnail = Original.Behaviors.Main_Thumbnail;
            New.Behaviors.Full_Text_Searchable = Original.Behaviors.Text_Searchable;

            // Copy over the viewers
            foreach (View_Object origView in Original.Behaviors.Views)
            {
                New.Behaviors.Viewers.Add(new BriefItem_BehaviorViewer(origView.View_Type, origView.MenuOrder, origView.Exclude, origView.Label ));
            }

            // Copy over the wordmarks
            if (Original.Behaviors.Wordmark_Count > 0)
            {
                New.Behaviors.Wordmarks = new List<string>();
                foreach (Wordmark_Info origWordmark in Original.Behaviors.Wordmarks)
                {
                    New.Behaviors.Wordmarks.Add(origWordmark.Code);
                }
            }

            // Copy over the citation set, if it exists
            if ( !String.IsNullOrEmpty(Original.Behaviors.CitationSet))
                New.Behaviors.CitationSet = Original.Behaviors.CitationSet;

            // Copy over all the loose settings, if they exist
            if ((Original.Behaviors.Settings != null) && (Original.Behaviors.Settings.Count > 0))
            {
                foreach (Tuple<string, string> setting in Original.Behaviors.Settings)
                {
                    New.Behaviors.Add_Setting(setting.Item1, setting.Item2);
                }
            }

            return true;
        }

    }
}
