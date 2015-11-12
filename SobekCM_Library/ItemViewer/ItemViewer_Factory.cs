#region Using directives

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
    /// which extends the <see cref="SobekCM.Library.ItemViewer.Viewers.abstractItemViewer"/> class.</summary>
    public class ItemViewer_Factory
    {
        /// <summary> Accepts a simple <see cref="View_Object"/> from the digital resource object and returns
        /// the appropriate item viewer object which extends the <see cref="SobekCM.Library.ItemViewer.Viewers.abstractItemViewer"/>
        /// class for rendering the item to the web via HTML.</summary>
        /// <param name="ViewObject"> View object from the digital resource object </param>
        /// <param name="Resource_Type">Resource type often impacts how an item viewer renders</param>
        /// <param name="Current_Object">Current resource object </param>
        /// <param name="Current_User">Currently session's user </param>
        /// <param name="Current_Mode"> Navigation object with all the information about the current request </param>
        /// <returns> Genereated item viewer class for rendering the particular view of a digital resource
        /// via HTML. </returns>
        public static abstractItemViewer Get_Viewer(View_Object ViewObject, string Resource_Type, SobekCM_Item Current_Object, User_Object Current_User, Navigation_Object Current_Mode )
        {
            switch (ViewObject.View_Type)
            {
                case View_Enum.ALL_VOLUMES:
                    return new MultiVolumes_ItemViewer();

                case View_Enum.CITATION:
                    return new Citation_ItemViewer();

                case View_Enum.DOWNLOADS:
                    return new Download_ItemViewer();

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
					return new EmbeddedVideo_ItemViewer();

                case View_Enum.FEATURES:
                    return new Feature_ItemViewer();

                case View_Enum.FLASH:
                    return new Flash_ItemViewer(ViewObject.Label, 0);

                case View_Enum.GOOGLE_COORDINATE_ENTRY:
                    return new Google_Coordinate_Entry_ItemViewer(Current_User, Current_Object, Current_Mode);

                case View_Enum.GOOGLE_MAP:
                    return new Google_Map_ItemViewer();

                case View_Enum.GOOGLE_MAP_BETA:
                    return new Google_Map_ItemViewer_Beta();

                case View_Enum.HTML:
                    return new HTML_ItemViewer(ViewObject.Attributes, ViewObject.Label);

                case View_Enum.JPEG:
                    abstractItemViewer jpegViewer = new JPEG_ItemViewer( ViewObject.Attributes );
                    jpegViewer.FileName = ViewObject.FileName;
                    return jpegViewer;

				case View_Enum.JPEG_TEXT_TWO_UP:
		            abstractItemViewer jpegTextViewer = new JPEG_Text_Two_Up_ItemViewer(ViewObject.Attributes);
					jpegTextViewer.FileName = ViewObject.FileName;
					return jpegTextViewer;

                case View_Enum.JPEG2000:
					if ( UI_ApplicationCache_Gateway.Settings.Servers.JP2ServerType == "Built-In IIPImage")
					{
						abstractItemViewer newJp2Viewer = new JPEG2000_ItemViewer();
						newJp2Viewer.FileName = ViewObject.FileName;
						return newJp2Viewer;
					}
                    return null;

				case View_Enum.MANAGE:
					return new ManageMenu_ItemViewer(Current_Object, Current_User, Current_Mode);

				case View_Enum.PAGE_TURNER:
						return new GnuBooks_PageTurner_ItemViewer();

				case View_Enum.PDF:
                        return new PDF_ItemViewer(ViewObject.FileName, Current_Mode);

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
                    abstractItemViewer textViewer = new Text_ItemViewer();
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
                    return new Video_ItemViewer(Current_Object);

            }

            return null;
        }
    }
}
