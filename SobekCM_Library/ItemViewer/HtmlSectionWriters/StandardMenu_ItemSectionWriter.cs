using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using SobekCM.Core.BriefItem;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Library.HTML;
using SobekCM.Library.ItemViewer.Menu;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;

namespace SobekCM.Library.ItemViewer.HtmlSectionWriters
{
    /// <summary> Item HTML section writer adds the standard item menu to the item display </summary>
    public class StandardMenu_ItemSectionWriter : iItemSectionWriter
    {
        private static iItemMenuProvider menuProvider;

        /// <summary> Write the standard item menu to the item display html</summary>
        /// <param name="Output"> Stream to which to write </param>
        /// <param name="Prototyper"> Current item viewer prototyper </param>
        /// <param name="CurrentViewer"> Current item viewer which will be used to fill the primary part of the page </param>
        /// <param name="CurrentItem"> Current item which is being displayed </param>
        /// <param name="RequestSpecificValues"> Other, request specific values, such as the current mode, user, etc.. </param>
        /// <param name="Behaviors"> Behaviors for the current view and situation </param>
        public void Write_HTML(TextWriter Output, iItemViewerPrototyper Prototyper, iItemViewer CurrentViewer, BriefItemInfo CurrentItem, RequestCache RequestSpecificValues, List<HtmlSubwriter_Behaviors_Enum> Behaviors)
        {
            // First, check that the menu provider was created
            if (menuProvider == null)
            {
                // Was there some configuration information?
                if (UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.MainMenu != null)
                {
                    if ((UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.MainMenu.Class == "SobekCM.Library.ItemViewer.Menu.StandardItemMenuProvider") && (String.IsNullOrWhiteSpace(UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.MainMenu.Assembly)))
                        menuProvider = new StandardItemMenuProvider();
                    else
                    {
                        try
                        {
                            string assemblyName = UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.MainMenu.Assembly;
                            string assemblyFilePath = Engine_ApplicationCache_Gateway.Configuration.Extensions.Get_Assembly(assemblyName);
                            Assembly dllAssembly = Assembly.LoadFrom(assemblyFilePath);
                            Type readerWriterType = dllAssembly.GetType(UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.MainMenu.Class);
                            menuProvider = (iItemMenuProvider)Activator.CreateInstance(readerWriterType);
                        }
                        catch (Exception)
                        {
                            // Do nothing here... will be fixed in the next couple lines of code
                        }
                    }
                }

                // Finally, just set to the standard if there was a problem
                if (menuProvider == null)
                    menuProvider = new StandardItemMenuProvider();
            }

            // The item viewer can choose to override the standard item menu
            if ((!Behaviors.Contains(HtmlSubwriter_Behaviors_Enum.Item_Subwriter_Suppress_Item_Menu)) && (menuProvider != null))
            {
                // Determine the current mode
                string currentCode = RequestSpecificValues.Current_Mode.ViewerCode ?? String.Empty;
                if (Prototyper != null)
                    currentCode = Prototyper.ViewerCode;


                // Let the menu provider write the menu
                menuProvider.Add_Main_Menu(Output, currentCode, RequestSpecificValues.Flags.ItemRestrictedFromUserByIp, RequestSpecificValues.Flags.ItemCheckedOutByOtherUser, CurrentItem, RequestSpecificValues.Current_Mode, RequestSpecificValues.Current_User, RequestSpecificValues.Tracer);
            }
        }
    }
}
