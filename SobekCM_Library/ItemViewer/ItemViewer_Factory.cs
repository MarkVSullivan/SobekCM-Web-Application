#region Using directives

using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Library.ItemViewer.Viewers;

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
        /// <param name="viewObject"> View object from the digital resource object </param>
        /// <param name="Resource_Type">Resource type often impacts how an item viewer renders</param>
        /// <returns> Genereated item viewer class for rendering the particular view of a digital resource
        /// via HTML. </returns>
        public static abstractItemViewer Get_Viewer(View_Object viewObject, string Resource_Type )
        {
            switch (viewObject.View_Type)
            {
                case View_Enum.ALL_VOLUMES:
                    return new MultiVolumes_ItemViewer();

                case View_Enum.CITATION:
                    return new Citation_ItemViewer();

                case View_Enum.DOWNLOADS:
                    return new Download_ItemViewer();

                case View_Enum.FEATURES:
                    return new Feature_ItemViewer();

                case View_Enum.FLASH:
                    return new Flash_ItemViewer(viewObject.Label, 0);

                case View_Enum.GOOGLE_MAP:
                    return new Google_Map_ItemViewer();

                case View_Enum.HTML:
                    return new HTML_ItemViewer(viewObject.Attributes, viewObject.Label);

                case View_Enum.HTML_MAP:
                    string[] html_map_splitter = viewObject.Attributes.Split(";".ToCharArray());
                    if (html_map_splitter.Length >= 2)
                    {
                        return new HTML_Map_ItemViewer(html_map_splitter[0], html_map_splitter[1], viewObject.Label);
                    }
                    break;

                case View_Enum.JPEG:
                    abstractItemViewer jpegViewer = new JPEG_ItemViewer( viewObject.Attributes );
                    jpegViewer.FileName = viewObject.FileName;
                    return jpegViewer;

                case View_Enum.JPEG2000:
                    abstractItemViewer jpeg2000Viewer = new JP2_ItemViewer(Resource_Type, viewObject.Attributes);
                    jpeg2000Viewer.FileName = viewObject.FileName;
                    return jpeg2000Viewer;

                case View_Enum.RELATED_IMAGES:
                    return new Related_Images_ItemViewer(viewObject.Label);

                case View_Enum.SEARCH:
                    return new Text_Search_ItemViewer();
                    
                case View_Enum.STREETS:
                    return new Street_ItemViewer();

                case View_Enum.TEXT:
                    abstractItemViewer textViewer = new Text_ItemViewer();
                    textViewer.FileName = viewObject.FileName;
                    return textViewer;

                case View_Enum.TOC:
                    return new TOC_ItemViewer();

                case View_Enum.PDF:
                    return new PDF_ItemViewer(viewObject.FileName);

                case View_Enum.EAD_DESCRIPTION:
                    return new EAD_Description_ItemViewer();

                case View_Enum.EAD_CONTAINER_LIST:
                    return new EAD_Container_List_ItemViewer();

                case View_Enum.PAGE_TURNER:
                    return new GnuBooks_PageTurner_ItemViewer();

                case View_Enum.YOUTUBE_VIDEO:
                    return new YouTube_Embedded_Video_ItemViewer();

                case View_Enum.TRACKING:
                    return new Tracking_ItemViewer();
            }

            return null;
        }
    }
}
