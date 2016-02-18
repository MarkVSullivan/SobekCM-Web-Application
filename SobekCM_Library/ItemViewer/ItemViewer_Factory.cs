#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Core.Navigation;
using SobekCM.Core.Users;
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


        public static void Configure_ItemViewers()
        {
            // First, built the list of all prototypers
            List<iItemViewerPrototyper> allPrototypers = new List<iItemViewerPrototyper>();
            allPrototypers.Add(new Citation_MARC_ItemViewer_Prototyper());
            allPrototypers.Add(new Citation_Standard_ItemViewer_Prototyper());
            allPrototypers.Add(new Downloads_ItemViewer_Prototyper());
            allPrototypers.Add(new EmbeddedVideo_ItemViewer_Prototyper());
            allPrototypers.Add(new Flash_ItemViewer_Prototyper());
            allPrototypers.Add(new Google_Map_ItemViewer_Prototyper());
            allPrototypers.Add(new HTML_ItemViewer_Prototyper());
            allPrototypers.Add(new JPEG_ItemViewer_Prototyper());
            allPrototypers.Add(new JPEG2000_ItemViewer_Prototyper());
            allPrototypers.Add(new ManageMenu_ItemViewer_Prototyper());
            allPrototypers.Add(new Metadata_Links_ItemViewer_Prototyper());
            allPrototypers.Add(new MultiVolumes_ItemViewer_Prototyper());
            allPrototypers.Add(new PDF_ItemViewer_Prototyper());
            allPrototypers.Add(new Related_Images_ItemViewer_Prototyper());
            allPrototypers.Add(new Usage_Stats_ItemViewer_Prototyper());
            allPrototypers.Add(new Video_ItemViewer_Prototyper());

            // Define the two dictionaries
            viewerCodeToItemViewerPrototyper = new Dictionary<string, iItemViewerPrototyper>( StringComparer.OrdinalIgnoreCase );
            viewTypeToItemViewerPrototyper = new Dictionary<string, iItemViewerPrototyper>( StringComparer.OrdinalIgnoreCase );

            // Copy from the list to the two dictionaries
            foreach (iItemViewerPrototyper thisPrototyper in allPrototypers)
            {
                viewerCodeToItemViewerPrototyper[thisPrototyper.ViewerCode] = thisPrototyper;
                viewTypeToItemViewerPrototyper[thisPrototyper.ViewerType] = thisPrototyper;
            }
        }

        /// <summary> Gets the viewer code (used in URLs and such) for a specific view type,
        /// or NULL if the current instance doesn't support that viewer type </summary>
        /// <param name="ViewType"> Standard type of the viewer to find </param>
        /// <returns> Viewer code (used in URLs and such) for a specific view type,
        /// or NULL if the current instance doesn't support that viewer type </returns>
        public static string ViewCode_From_ViewType(string ViewType)
        {
            if (!viewTypeToItemViewerPrototyper.ContainsKey(ViewType))
                return null;

            return viewTypeToItemViewerPrototyper[ViewType].ViewerCode;
        }

        /// <summary> Gets the standard viewer type from a viewer code,
        /// or NULL if the current instance doesn't support that viewer code </summary>
        /// <param name="ViewCode"> Viewer code to look for viewer </param>
        /// <returns> Viewer type from a viewer code,
        /// or NULL if the current instance doesn't support that viewer code </returns>
        public static string ViewType_From_ViewCode(string ViewCode)
        {
            if (!viewerCodeToItemViewerPrototyper.ContainsKey(ViewCode))
                return null;

            return viewerCodeToItemViewerPrototyper[ViewCode].ViewerType;
        }



        /// <summary> Accepts a simple <see cref="View_Object"/> from the digital resource object and returns
        /// the appropriate item viewer object which extends the <see cref="SobekCM.Library.ItemViewer.Viewers.abstractItemViewer_OLD"/>
        /// class for rendering the item to the web via HTML.</summary>
        /// <param name="ViewObject"> View object from the digital resource object </param>
        /// <param name="Resource_Type">Resource type often impacts how an item viewer renders</param>
        /// <param name="Current_Object">Current resource object </param>
        /// <param name="Current_User">Currently session's user </param>
        /// <param name="Current_Mode"> Navigation object with all the information about the current request </param>
        /// <returns> Genereated item viewer class for rendering the particular view of a digital resource
        /// via HTML. </returns>
        public static abstractItemViewer_OLD Get_Viewer(View_Object ViewObject, string Resource_Type, SobekCM_Item Current_Object, User_Object Current_User, Navigation_Object Current_Mode )
        {
            switch (ViewObject.View_Type)
            {
                case View_Enum.ALL_VOLUMES:
                    return new MultiVolumes_ItemViewer_OLD();

                case View_Enum.CITATION:
                    return new Citation_ItemViewer();

                case View_Enum.DOWNLOADS:
                    return new Download_ItemViewer_OLD();

				case View_Enum.DATASET_CODEBOOK:
					return new Dataset_CodeBook_ItemViewer();

				case View_Enum.DATASET_REPORTS:
					return new Dataset_Reports_ItemViewer();

				case View_Enum.DATASET_VIEWDATA:
					return new Dataset_ViewData_ItemViewer();

				case View_Enum.EAD_DESCRIPTION:
					return new EAD_Description_ItemViewer();

				case View_Enum.EAD_CONTAINER_LIST:
					return new EAD_Container_List_ItemViewer();

				case View_Enum.EMBEDDED_VIDEO:
					return new EmbeddedVideo_ItemViewer_OLD();

                case View_Enum.FEATURES:
                    return new Feature_ItemViewer();

                case View_Enum.FLASH:
                    return new Flash_ItemViewer_OLD(ViewObject.Label, 0);

                case View_Enum.GOOGLE_COORDINATE_ENTRY:
                    return new Google_Coordinate_Entry_ItemViewer(Current_User, Current_Object, Current_Mode);

                case View_Enum.GOOGLE_MAP:
                    return new Google_Map_ItemViewer();

                case View_Enum.GOOGLE_MAP_BETA:
                    return new Google_Map_ItemViewer_Beta();

                case View_Enum.HTML:
                    return new HTML_ItemViewer_OLD(ViewObject.Attributes, ViewObject.Label);

                case View_Enum.JPEG:
                    abstractItemViewer_OLD jpegViewer = new JPEG_ItemViewer_OLD( ViewObject.Attributes );
                    jpegViewer.FileName = ViewObject.FileName;
                    return jpegViewer;

				case View_Enum.JPEG_TEXT_TWO_UP:
		            abstractItemViewer_OLD jpegTextViewer = new JPEG_Text_Two_Up_ItemViewer(ViewObject.Attributes);
					jpegTextViewer.FileName = ViewObject.FileName;
					return jpegTextViewer;

                case View_Enum.JPEG2000:
					if ( UI_ApplicationCache_Gateway.Settings.Servers.JP2ServerType == "Built-In IIPImage")
					{
						abstractItemViewer_OLD newJp2Viewer = new JPEG2000_ItemViewer_OLD();
						newJp2Viewer.FileName = ViewObject.FileName;
						return newJp2Viewer;
					}
                    return null;

				case View_Enum.MANAGE:
					return new ManageMenu_ItemViewer_OLD(Current_Object, Current_User, Current_Mode);

				case View_Enum.PAGE_TURNER:
						return new GnuBooks_PageTurner_ItemViewer();

				case View_Enum.PDF:
                        return new PDF_ItemViewer_OLD(ViewObject.FileName, Current_Mode);

				case View_Enum.QUALITY_CONTROL:
						return new QC_ItemViewer(Current_Object, Current_User, Current_Mode);

                case View_Enum.RELATED_IMAGES:
                    return new Related_Images_ItemViewer();

                case View_Enum.SEARCH:
                    return new Text_Search_ItemViewer();
                    
                case View_Enum.STREETS:
                    return new Street_ItemViewer();

				case View_Enum.TEST:
					return new Test_ItemViewer();

                case View_Enum.TEXT:
                    abstractItemViewer_OLD textViewer = new Text_ItemViewer();
                    textViewer.FileName = ViewObject.FileName;
                    return textViewer;

                case View_Enum.TOC:
                    return new TOC_ItemViewer();

				case View_Enum.TRACKING:
					return new Tracking_ItemViewer();

                case View_Enum.TRACKING_SHEET:
                    return new TrackingSheet_ItemViewer(Current_Object, Current_User, Current_Mode);

				case View_Enum.YOUTUBE_VIDEO:
					return new YouTube_Embedded_Video_ItemViewer();

                case View_Enum.VIDEO:
                    return new Video_ItemViewer_OLD(Current_Object);

            }

            return null;
        }
    }
}
