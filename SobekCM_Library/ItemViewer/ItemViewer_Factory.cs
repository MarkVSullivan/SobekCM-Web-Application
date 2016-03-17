#region Using directives

using System;
using System.Collections.Generic;
using System.Reflection;
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
    }
}
