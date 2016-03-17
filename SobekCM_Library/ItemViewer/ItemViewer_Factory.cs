#region Using directives

using System;
using System.Collections.Generic;
using System.Reflection;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;
using SobekCM.Core.Settings;
using SobekCM.Core.UI_Configuration.Viewers;
using SobekCM.Core.Users;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Library.ItemViewer.Viewers;
using SobekCM.Library.UI;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Behaviors;

#endregion

namespace SobekCM.Library.ItemViewer
{
    /// <summary> Static class is a factory that generates and returns the requested item viewer object 
    /// which implements the <see cref="SobekCM.Library.ItemViewer.Viewers.iItemViewer"/> interface.</summary>
    public static class ItemViewer_Factory
    {
        private static Dictionary<string, iItemViewerPrototyper> viewerCodeToItemViewerPrototyper;
        private static Dictionary<string, iItemViewerPrototyper> viewTypeToItemViewerPrototyper;

        /// <summary> Configure the viewers for a single brief item to match this user interface settings </summary>
        /// <param name="BriefItem"> Brief item to adjust selected viewers </param>
        public static void Configure_Brief_Item_Viewers(BriefItemInfo BriefItem)
        {
            // Ensure the necessary dictionaries are built
            configureItemViewers();

            // If the brief item already has the UI built, skip this
            if ((BriefItem.UI != null) && (BriefItem.UI.Viewers_By_Priority != null) && (BriefItem.UI.Viewers_Menu_Order != null))
                return;

            // Now, add the UI object (if null) and set the values
            if (BriefItem.UI == null) BriefItem.UI = new BriefItem_UI();
            if (BriefItem.UI.Viewers_By_Priority == null) BriefItem.UI.Viewers_By_Priority = new List<string>();
            if (BriefItem.UI.Viewers_Menu_Order == null) BriefItem.UI.Viewers_Menu_Order = new List<string>();

            //// Step through each viewer included from the database and build lookup dictionary
            //Dictionary<string, string> dbViews = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase);
            //foreach (BriefItem_BehaviorViewer viewer in BriefItem.Behaviors.Viewers)
            //{
            //    dbViews[viewer.ViewerType] = viewer.ViewerType;
            //}

            //// Add any viewers that should be added


            // Use a sorted list to build the menu order
            SortedDictionary<float, string> menuOrderSort = new SortedDictionary<float, string>();

            // Step through each viewer included from the database
            foreach (BriefItem_BehaviorViewer viewer in BriefItem.Behaviors.Viewers)
            {
                // If this is an EXCLUDE viewer, just skip it here
                if (viewer.Excluded)
                    continue;

                // Verify a match in the UI configuration for the actual item viewers
                if (!viewTypeToItemViewerPrototyper.ContainsKey(viewer.ViewerType))
                    continue;

                // Get the item prototype object
                iItemViewerPrototyper protoTyper = viewTypeToItemViewerPrototyper[viewer.ViewerType];

                // Verify this prototyper believes it should be added
                if (protoTyper.Include_Viewer(BriefItem))
                {
                    // Add this view to the ordained views
                    BriefItem.UI.Viewers_By_Priority.Add(viewer.ViewerType);

                    // CHeck for collisions on the menu order
                    float menuOrder = viewer.MenuOrder;
                    while (menuOrderSort.ContainsKey(menuOrder))
                        menuOrder = menuOrder + .001f;

                    // Also add this to the menu order sorted list
                    menuOrderSort[menuOrder] = viewer.ViewerType;
                }
            }

            // Add the viewers back in menu order
            foreach (float thisKey in menuOrderSort.Keys)
            {
                BriefItem.UI.Viewers_Menu_Order.Add( menuOrderSort[thisKey]);
            }
        }


        private static void configureItemViewers()
        {
            // If already configured, do nothing
            if ((viewTypeToItemViewerPrototyper != null) && (viewTypeToItemViewerPrototyper.Count > 0) &&
                (viewerCodeToItemViewerPrototyper != null) && (viewerCodeToItemViewerPrototyper.Count > 0))
                return;

            // Make sure the dictionaries are not null
            if ( viewerCodeToItemViewerPrototyper == null ) viewerCodeToItemViewerPrototyper = new Dictionary<string, iItemViewerPrototyper>(StringComparer.OrdinalIgnoreCase);
            if (viewTypeToItemViewerPrototyper == null) viewTypeToItemViewerPrototyper = new Dictionary<string, iItemViewerPrototyper>(StringComparer.OrdinalIgnoreCase);

            // Step through all the potential item viewers prototypes in the dictionary
            foreach (ItemSubViewerConfig thisViewerConfig in UI_ApplicationCache_Gateway.Configuration.UI.WriterViewers.Items.Viewers)
            {
                // If this is not enabled, skip it
                if (!thisViewerConfig.Enabled)
                    continue;

                // Get the code and viewer type
                string code = thisViewerConfig.ViewerCode;
                string type = thisViewerConfig.ViewerType;

                // Build the prototyper
                iItemViewerPrototyper prototyper = configurePrototyper(thisViewerConfig.Assembly, thisViewerConfig.Class);

                // Add this to the dictionaries
                viewTypeToItemViewerPrototyper[type] = prototyper;
                viewerCodeToItemViewerPrototyper[code] = prototyper;
            }
        }

        private static iItemViewerPrototyper configurePrototyper( string assembly, string className )
        {
            // Was an assembly indicated
            if (!String.IsNullOrEmpty(assembly))
            {
                try
                {
                    Assembly dllAssembly = Assembly.LoadFrom(assembly);
                    Type readerWriterType = dllAssembly.GetType(className);
                    iItemViewerPrototyper returnObj = (iItemViewerPrototyper)Activator.CreateInstance(readerWriterType);
                    return returnObj;
                }
                catch (Exception)
                {
                    // Not sure exactly what to do here, honestly
                    return null;
                }
            }

            // Return a standard class
            switch (className)
            {
                case "SobekCM.Library.ItemViewer.Viewers.Citation_MARC_ItemViewer_Prototyper":
                    return new Citation_MARC_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Citation_Standard_ItemViewer_Prototyper":
                    return new Citation_Standard_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Downloads_ItemViewer_Prototyper":
                    return new Downloads_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.EmbeddedVideo_ItemViewer_Prototyper":
                    return new EmbeddedVideo_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Flash_ItemViewer_Prototyper":
                    return new Flash_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Google_Map_ItemViewer_Prototyper":
                    return new Google_Map_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.HTML_ItemViewer_Prototyper":
                    return new HTML_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.JPEG_ItemViewer_Prototyper":
                    return new JPEG_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.JPEG2000_ItemViewer_Prototyper":
                    return new JPEG2000_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.ManageMenu_ItemViewer_Prototyper":
                    return new ManageMenu_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Metadata_Links_ItemViewer_Prototyper":
                    return new Metadata_Links_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.MultiVolumes_ItemViewer_Prototyper":
                    return new MultiVolumes_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.PDF_ItemViewer_Prototyper":
                    return new PDF_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Related_Images_ItemViewer_Prototyper":
                    return new Related_Images_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Usage_Stats_ItemViewer_Prototyper":
                    return new Usage_Stats_ItemViewer_Prototyper();

                case "SobekCM.Library.ItemViewer.Viewers.Video_ItemViewer_Prototyper":
                    return new Video_ItemViewer_Prototyper();
            }

            return null;
        }

        /// <summary> Clears the dictionaries of item viewer prototypes, used when the cache
        /// is reset either manually or automatically </summary>
        /// <returns></returns>
        public static void Clear()
        {
            viewerCodeToItemViewerPrototyper.Clear();
            viewTypeToItemViewerPrototyper.Clear();
        }

        /// <summary> Gets the viewer code (used in URLs and such) for a specific view type,
        /// or NULL if the current instance doesn't support that viewer type </summary>
        /// <param name="ViewType"> Standard type of the viewer to find </param>
        /// <returns> Viewer code (used in URLs and such) for a specific view type,
        /// or NULL if the current instance doesn't support that viewer type </returns>
        public static string ViewCode_From_ViewType(string ViewType)
        {
            // Ensure the necessary dictionaries are built
            configureItemViewers();

            // If this not in the dictionary, return NULL
            if (!viewTypeToItemViewerPrototyper.ContainsKey(ViewType))
                return null;

            // return match
            return viewTypeToItemViewerPrototyper[ViewType].ViewerCode;
        }

        /// <summary> Gets the standard viewer type from a viewer code,
        /// or NULL if the current instance doesn't support that viewer code </summary>
        /// <param name="ViewerCode"> Viewer code to look for viewer </param>
        /// <returns> Viewer type from a viewer code,
        /// or NULL if the current instance doesn't support that viewer code </returns>
        public static string ViewType_From_ViewerCode(string ViewerCode)
        {
            // Ensure the necessary dictionaries are built
            configureItemViewers();

            // If this not in the dictionary, return NULL
            if (!viewerCodeToItemViewerPrototyper.ContainsKey(ViewerCode))
                return null;

            // return match
            return viewerCodeToItemViewerPrototyper[ViewerCode].ViewerType;
        }

        /// <summary> Accepts a viewer code (string) from the digital resource object and returns
        /// the appropriate item viewer object which extends the <see cref="SobekCM.Library.ItemViewer.Viewer.iItemViewerPriority"/>
        /// class for rendering items to the web via HTML.</summary>
        /// <param name="ViewerCode"> Viewer code to retrieve </param>
        /// <returns> Genereated item viewer class for rendering the particular view of a digital resource
        /// via HTML. </returns>
        public static iItemViewerPrototyper Get_Viewer_By_ViewerCode(string ViewerCode)
        {
            // Ensure the necessary dictionaries are built
            configureItemViewers();

            // Get this viewer, by viewer code
            if (viewerCodeToItemViewerPrototyper.ContainsKey(ViewerCode))
                return viewerCodeToItemViewerPrototyper[ViewerCode];

            return null;
        }


        /// <summary> Accepts a view type (string) from the digital resource object and returns
        /// the appropriate item viewer object which extends the <see cref="SobekCM.Library.ItemViewer.Viewer.iItemViewerPriority"/>
        /// class for rendering items to the web via HTML.</summary>
        /// <param name="ViewType"> Viewer code to retrieve </param>
        /// <returns> Genereated item viewer class for rendering the particular view of a digital resource
        /// via HTML. </returns>
        public static iItemViewerPrototyper Get_Viewer_By_ViewType( string ViewType )
        {
            // Ensure the necessary dictionaries are built
            configureItemViewers();

            // Get this viewer, by viewer code
            if (viewTypeToItemViewerPrototyper.ContainsKey(ViewType))
                return viewTypeToItemViewerPrototyper[ViewType];

            return null;
        }

        /// <summary> Gets the appropriate item viewer prototyper ( <see cref="SobekCM.Library.ItemViewer.Viewer.iItemViewerPriority"/> )
        /// class for an individual item, based on the requested viewer code </summary>
        /// <param name="CurrentItem">The current item.</param>
        /// <param name="ViewerCode">The viewer code.</param>
        /// <returns> Item viewer prototyper class, which can then be used to create the viewer, if applicable </returns>
        public static iItemViewerPrototyper Get_Item_Viewer(BriefItemInfo CurrentItem, string ViewerCode)
        {
            // Get the viewer type from the item
            string validType = CurrentItem.UI.Get_Viewer_Type(ViewerCode);

            // Now, return this prototype 
            return Get_Viewer_By_ViewType(validType);
        }
    }
}
