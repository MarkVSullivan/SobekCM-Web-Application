#region Using directives

using System;
using System.IO;
using System.Xml;
using SobekCM.Core.Client;
using SobekCM.Core.UI_Configuration.StaticResources;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Configuration
{
    /// <summary> Gateway to all of the static resources (images, javascript, stylesheets, and included libraries ) 
    /// used by the standard SobekCM web user interface </summary>
    public static class Static_Resources_Gateway
    {
        private static StaticResources_Configuration config;

        /// <summary> Static constructor for the Static_Resources class </summary>
        static Static_Resources_Gateway()
        {
            // Get the static resource configuration from the engine 
            Custom_Tracer tracer = new Custom_Tracer();
            config = SobekEngineClient.Admin.Get_Static_Resources_Configuration(tracer);
        }

        /// <summary> URL for the default resource '16px-feed-icon.svg.png' file ( http://cdn.sobekrepository.org/images/misc/16px-Feed-icon.svg.png by default)</summary>
        public static string Sixteen_Px_Feed_Img { get { return config.Sixteen_Px_Feed_Img; } }

        /// <summary> URL for the default resource 'add_geospatial_icon.png' file ( http://cdn.sobekrepository.org/images/misc/add_geospatial_icon.png by default)</summary>
        public static string Add_Geospatial_Img { get { return config.Add_Geospatial_Img; } }

        /// <summary> URL for the default resource 'add_volume_icon.png' file ( http://cdn.sobekrepository.org/images/misc/add_volume_icon.png by default)</summary>
        public static string Add_Volume_Img { get { return config.Add_Volume_Img; } }

        /// <summary> URL for the default resource 'admin_view.png' file ( http://cdn.sobekrepository.org/images/misc/admin_view.png by default)</summary>
        public static string Admin_View_Img { get { return config.Admin_View_Img; } }

        /// <summary> URL for the default resource 'admin_view_lg.png' file ( http://cdn.sobekrepository.org/images/misc/admin_view_lg.png by default)</summary>
        public static string Admin_View_Img_Large { get { return config.Admin_View_Img_Large; } }

        /// <summary> URL for the default resource 'aggregations.gif' file ( http://cdn.sobekrepository.org/images/misc/aggregations.gif by default)</summary>
        public static string Aggregations_Img { get { return config.Aggregations_Img; } }

        /// <summary> URL for the default resource 'aggregations_lg.png' file ( http://cdn.sobekrepository.org/images/misc/aggregations_lg.png by default)</summary>
        public static string Aggregations_Img_Large { get { return config.Aggregations_Img_Large; } }

        /// <summary> URL for the default resource 'ajax-loader.gif' file ( http://cdn.sobekrepository.org/images/mapedit/ajax-loader.gif by default)</summary>
        public static string Ajax_Loader_Img { get { return config.Ajax_Loader_Img; } }

        /// <summary> URL for the default resource 'aliases.png' file ( http://cdn.sobekrepository.org/images/misc/aliases.png by default)</summary>
        public static string Aliases_Img { get { return config.Aliases_Img; } }

        /// <summary> URL for the default resource 'aliases_small.png' file ( http://cdn.sobekrepository.org/images/misc/aliases_small.png by default)</summary>
        public static string Aliases_Img_Small { get { return config.Aliases_Img_Small; } }

        /// <summary> URL for the default resource 'aliases_large.png' file ( http://cdn.sobekrepository.org/images/misc/aliases_large.png by default)</summary>
        public static string Aliases_Img_Large { get { return config.Aliases_Img_Large; } }

        /// <summary> URL for the default resource 'arw05lt.gif' file ( http://cdn.sobekrepository.org/images/qc/ARW05LT.gif by default)</summary>
        public static string Arw05lt_Img { get { return config.Arw05lt_Img; } }

        /// <summary> URL for the default resource 'arw05rt.gif' file ( http://cdn.sobekrepository.org/images/qc/ARW05RT.gif by default)</summary>
        public static string Arw05rt_Img { get { return config.Arw05rt_Img; } }

        /// <summary> URL for the default resource 'bg1.png' file ( http://cdn.sobekrepository.org/images/mapedit/bg1.png by default)</summary>
        public static string Bg1_Img { get { return config.Bg1_Img; } }

        /// <summary> URL for the default resource 'big_bookshelf.gif' file ( http://cdn.sobekrepository.org/images/misc/big_bookshelf.gif by default)</summary>
        public static string Big_Bookshelf_Img { get { return config.Big_Bookshelf_Img; } }

        /// <summary> URL for the default resource 'blue.png' file ( http://cdn.sobekrepository.org/images/mapedit/mapIcons/blue.png by default)</summary>
        public static string Blue_Img { get { return config.Blue_Img; } }

        /// <summary> URL for the default resource 'blue-pin.png' file ( http://cdn.sobekrepository.org/images/mapsearch/blue-pin.png by default)</summary>
        public static string Blue_Pin_Img { get { return config.Blue_Pin_Img; } }

        /// <summary> URL for the default resource 'bookshelf.png' file ( http://cdn.sobekrepository.org/images/misc/bookshelf.png by default)</summary>
        public static string Bookshelf_Img { get { return config.Bookshelf_Img; } }

        /// <summary> URL for the default resource 'bookturner.js' file ( http://cdn.sobekrepository.org/includes/bookturner/1.0.0/bookturner.js by default)</summary>
        public static string Bookturner_Js { get { return config.Bookturner_Js; } }

        /// <summary> URL for the default resource 'brief_blue.png' file ( http://cdn.sobekrepository.org/images/mapsearch/brief_blue.png by default)</summary>
        public static string Brief_Blue_Img { get { return config.Brief_Blue_Img; } }

        /// <summary> URL for the default resource 'button_down_arrow.png' file ( http://cdn.sobekrepository.org/images/misc/button_down_arrow.png by default)</summary>
        public static string Button_Down_Arrow_Png { get { return config.Button_Down_Arrow_Png; } }

        /// <summary> URL for the default resource 'button_first_arrow.png' file ( http://cdn.sobekrepository.org/images/misc/button_first_arrow.png by default)</summary>
        public static string Button_First_Arrow_Png { get { return config.Button_First_Arrow_Png; } }

        /// <summary> URL for the default resource 'button_last_arrow.png' file ( http://cdn.sobekrepository.org/images/misc/button_last_arrow.png by default)</summary>
        public static string Button_Last_Arrow_Png { get { return config.Button_Last_Arrow_Png; } }

        /// <summary> URL for the default resource 'button_next_arrow.png' file ( http://cdn.sobekrepository.org/images/misc/button_next_arrow.png by default)</summary>
        public static string Button_Next_Arrow_Png { get { return config.Button_Next_Arrow_Png; } }

        /// <summary> URL for the default resource 'button_next_arrow2.png' file ( http://cdn.sobekrepository.org/images/misc/button_next_arrow2.png by default)</summary>
        public static string Button_Next_Arrow2_Png { get { return config.Button_Next_Arrow2_Png; } }

        /// <summary> URL for the default resource 'button_previous_arrow.png' file ( http://cdn.sobekrepository.org/images/misc/button_previous_arrow.png by default)</summary>
        public static string Button_Previous_Arrow_Png { get { return config.Button_Previous_Arrow_Png; } }

        /// <summary> URL for the default resource 'button_up_arrow.png' file ( http://cdn.sobekrepository.org/images/misc/button_up_arrow.png by default)</summary>
        public static string Button_Up_Arrow_Png { get { return config.Button_Up_Arrow_Png; } }

        /// <summary> URL for the default resource 'button-action1.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-action1.png by default)</summary>
        public static string Button_Action1_Png { get { return config.Button_Action1_Png; } }

        /// <summary> URL for the default resource 'button-action2.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-action2.png by default)</summary>
        public static string Button_Action2_Png { get { return config.Button_Action2_Png; } }

        /// <summary> URL for the default resource 'button-action3.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-action3.png by default)</summary>
        public static string Button_Action3_Png { get { return config.Button_Action3_Png; } }

        /// <summary> URL for the default resource 'button-base.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-base.png by default)</summary>
        public static string Button_Base_Png { get { return config.Button_Base_Png; } }

        /// <summary> URL for the default resource 'button-blocklot.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-blockLot.png by default)</summary>
        public static string Button_Blocklot_Png { get { return config.Button_Blocklot_Png; } }

        /// <summary> URL for the default resource 'button-cancel.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-cancel.png by default)</summary>
        public static string Button_Cancel_Png { get { return config.Button_Cancel_Png; } }

        /// <summary> URL for the default resource 'button-controls.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-controls.png by default)</summary>
        public static string Button_Controls_Png { get { return config.Button_Controls_Png; } }

        /// <summary> URL for the default resource 'button-converttooverlay.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-convertToOverlay.png by default)</summary>
        public static string Button_Converttooverlay_Png { get { return config.Button_Converttooverlay_Png; } }

        /// <summary> URL for the default resource 'button-drawcircle.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-drawCircle.png by default)</summary>
        public static string Button_Drawcircle_Png { get { return config.Button_Drawcircle_Png; } }

        /// <summary> URL for the default resource 'button-drawline.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-drawLine.png by default)</summary>
        public static string Button_Drawline_Png { get { return config.Button_Drawline_Png; } }

        /// <summary> URL for the default resource 'button-drawmarker.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-drawMarker.png by default)</summary>
        public static string Button_Drawmarker_Png { get { return config.Button_Drawmarker_Png; } }

        /// <summary> URL for the default resource 'button-drawpolygon.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-drawPolygon.png by default)</summary>
        public static string Button_Drawpolygon_Png { get { return config.Button_Drawpolygon_Png; } }

        /// <summary> URL for the default resource 'button-drawrectangle.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-drawRectangle.png by default)</summary>
        public static string Button_Drawrectangle_Png { get { return config.Button_Drawrectangle_Png; } }

        /// <summary> URL for the default resource 'button-hybrid.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-hybrid.png by default)</summary>
        public static string Button_Hybrid_Png { get { return config.Button_Hybrid_Png; } }

        /// <summary> URL for the default resource 'button-itemgetuserlocation.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-itemGetUserLocation.png by default)</summary>
        public static string Button_Itemgetuserlocation_Png { get { return config.Button_Itemgetuserlocation_Png; } }

        /// <summary> URL for the default resource 'button-itemplace.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-itemPlace.png by default)</summary>
        public static string Button_Itemplace_Png { get { return config.Button_Itemplace_Png; } }

        /// <summary> URL for the default resource 'button-itemreset.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-itemReset.png by default)</summary>
        public static string Button_Itemreset_Png { get { return config.Button_Itemreset_Png; } }

        /// <summary> URL for the default resource 'button-layercustom.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-layerCustom.png by default)</summary>
        public static string Button_Layercustom_Png { get { return config.Button_Layercustom_Png; } }

        /// <summary> URL for the default resource 'button-layerhybrid.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-layerHybrid.png by default)</summary>
        public static string Button_Layerhybrid_Png { get { return config.Button_Layerhybrid_Png; } }

        /// <summary> URL for the default resource 'button-layerreset.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-layerReset.png by default)</summary>
        public static string Button_Layerreset_Png { get { return config.Button_Layerreset_Png; } }

        /// <summary> URL for the default resource 'button-layerroadmap.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-layerRoadmap.png by default)</summary>
        public static string Button_Layerroadmap_Png { get { return config.Button_Layerroadmap_Png; } }

        /// <summary> URL for the default resource 'button-layersatellite.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-layerSatellite.png by default)</summary>
        public static string Button_Layersatellite_Png { get { return config.Button_Layersatellite_Png; } }

        /// <summary> URL for the default resource 'button-layerterrain.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-layerTerrain.png by default)</summary>
        public static string Button_Layerterrain_Png { get { return config.Button_Layerterrain_Png; } }

        /// <summary> URL for the default resource 'button-manageitem.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-manageItem.png by default)</summary>
        public static string Button_Manageitem_Png { get { return config.Button_Manageitem_Png; } }

        /// <summary> URL for the default resource 'button-manageoverlay.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-manageOverlay.png by default)</summary>
        public static string Button_Manageoverlay_Png { get { return config.Button_Manageoverlay_Png; } }

        /// <summary> URL for the default resource 'button-managepoi.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-managePOI.png by default)</summary>
        public static string Button_Managepoi_Png { get { return config.Button_Managepoi_Png; } }

        /// <summary> URL for the default resource 'button-overlayedit.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-overlayEdit.png by default)</summary>
        public static string Button_Overlayedit_Png { get { return config.Button_Overlayedit_Png; } }

        /// <summary> URL for the default resource 'button-overlaygetuserlocation.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-overlayGetUserLocation.png by default)</summary>
        public static string Button_Overlaygetuserlocation_Png { get { return config.Button_Overlaygetuserlocation_Png; } }

        /// <summary> URL for the default resource 'button-overlayplace.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-overlayPlace.png by default)</summary>
        public static string Button_Overlayplace_Png { get { return config.Button_Overlayplace_Png; } }

        /// <summary> URL for the default resource 'button-overlayreset.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-overlayReset.png by default)</summary>
        public static string Button_Overlayreset_Png { get { return config.Button_Overlayreset_Png; } }

        /// <summary> URL for the default resource 'button-overlayrotate.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-overlayRotate.png by default)</summary>
        public static string Button_Overlayrotate_Png { get { return config.Button_Overlayrotate_Png; } }

        /// <summary> URL for the default resource 'button-overlaytoggle.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-overlayToggle.png by default)</summary>
        public static string Button_Overlaytoggle_Png { get { return config.Button_Overlaytoggle_Png; } }

        /// <summary> URL for the default resource 'button-overlaytransparency.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-overlayTransparency.png by default)</summary>
        public static string Button_Overlaytransparency_Png { get { return config.Button_Overlaytransparency_Png; } }

        /// <summary> URL for the default resource 'button-pandown.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-panDown.png by default)</summary>
        public static string Button_Pandown_Png { get { return config.Button_Pandown_Png; } }

        /// <summary> URL for the default resource 'button-panleft.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-panLeft.png by default)</summary>
        public static string Button_Panleft_Png { get { return config.Button_Panleft_Png; } }

        /// <summary> URL for the default resource 'button-panreset.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-panReset.png by default)</summary>
        public static string Button_Panreset_Png { get { return config.Button_Panreset_Png; } }

        /// <summary> URL for the default resource 'button-panright.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-panRight.png by default)</summary>
        public static string Button_Panright_Png { get { return config.Button_Panright_Png; } }

        /// <summary> URL for the default resource 'button-panup.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-panUp.png by default)</summary>
        public static string Button_Panup_Png { get { return config.Button_Panup_Png; } }

        /// <summary> URL for the default resource 'button-poicircle.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiCircle.png by default)</summary>
        public static string Button_Poicircle_Png { get { return config.Button_Poicircle_Png; } }

        /// <summary> URL for the default resource 'button-poiedit.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiEdit.png by default)</summary>
        public static string Button_Poiedit_Png { get { return config.Button_Poiedit_Png; } }

        /// <summary> URL for the default resource 'button-poigetuserlocation.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiGetUserLocation.png by default)</summary>
        public static string Button_Poigetuserlocation_Png { get { return config.Button_Poigetuserlocation_Png; } }

        /// <summary> URL for the default resource 'button-poiline.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiLine.png by default)</summary>
        public static string Button_Poiline_Png { get { return config.Button_Poiline_Png; } }

        /// <summary> URL for the default resource 'button-poimarker.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiMarker.png by default)</summary>
        public static string Button_Poimarker_Png { get { return config.Button_Poimarker_Png; } }

        /// <summary> URL for the default resource 'button-poiplace.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiPlace.png by default)</summary>
        public static string Button_Poiplace_Png { get { return config.Button_Poiplace_Png; } }

        /// <summary> URL for the default resource 'button-poipolygon.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiPolygon.png by default)</summary>
        public static string Button_Poipolygon_Png { get { return config.Button_Poipolygon_Png; } }

        /// <summary> URL for the default resource 'button-poirectangle.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiRectangle.png by default)</summary>
        public static string Button_Poirectangle_Png { get { return config.Button_Poirectangle_Png; } }

        /// <summary> URL for the default resource 'button-poireset.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiReset.png by default)</summary>
        public static string Button_Poireset_Png { get { return config.Button_Poireset_Png; } }

        /// <summary> URL for the default resource 'button-poitoggle.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-poiToggle.png by default)</summary>
        public static string Button_Poitoggle_Png { get { return config.Button_Poitoggle_Png; } }

        /// <summary> URL for the default resource 'button-reset.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-reset.png by default)</summary>
        public static string Button_Reset_Png { get { return config.Button_Reset_Png; } }

        /// <summary> URL for the default resource 'button-roadmap.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-roadmap.png by default)</summary>
        public static string Button_Roadmap_Png { get { return config.Button_Roadmap_Png; } }

        /// <summary> URL for the default resource 'button-satellite.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-satellite.png by default)</summary>
        public static string Button_Satellite_Png { get { return config.Button_Satellite_Png; } }

        /// <summary> URL for the default resource 'button-save.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-save.png by default)</summary>
        public static string Button_Save_Png { get { return config.Button_Save_Png; } }

        /// <summary> URL for the default resource 'button-search.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-search.png by default)</summary>
        public static string Button_Search_Png { get { return config.Button_Search_Png; } }

        /// <summary> URL for the default resource 'button-terrain.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-terrain.png by default)</summary>
        public static string Button_Terrain_Png { get { return config.Button_Terrain_Png; } }

        /// <summary> URL for the default resource 'button-togglemapcontrols.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-toggleMapControls.png by default)</summary>
        public static string Button_Togglemapcontrols_Png { get { return config.Button_Togglemapcontrols_Png; } }

        /// <summary> URL for the default resource 'button-toggletoolbar.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-toggleToolbar.png by default)</summary>
        public static string Button_Toggletoolbar_Png { get { return config.Button_Toggletoolbar_Png; } }

        /// <summary> URL for the default resource 'button-toggletoolbox.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-toggleToolbox.png by default)</summary>
        public static string Button_Toggletoolbox_Png { get { return config.Button_Toggletoolbox_Png; } }

        /// <summary> URL for the default resource 'button-toolbox.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-toolbox.png by default)</summary>
        public static string Button_Toolbox_Png { get { return config.Button_Toolbox_Png; } }

        /// <summary> URL for the default resource 'button-usesearchaslocation.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-useSearchAsLocation.png by default)</summary>
        public static string Button_Usesearchaslocation_Png { get { return config.Button_Usesearchaslocation_Png; } }

        /// <summary> URL for the default resource 'button-zoomin.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-zoomIn.png by default)</summary>
        public static string Button_Zoomin_Png { get { return config.Button_Zoomin_Png; } }

        /// <summary> URL for the default resource 'button-zoomout.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-zoomOut.png by default)</summary>
        public static string Button_Zoomout_Png { get { return config.Button_Zoomout_Png; } }

        /// <summary> URL for the default resource 'button-zoomreset.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-zoomReset.png by default)</summary>
        public static string Button_Zoomreset_Png { get { return config.Button_Zoomreset_Png; } }

        /// <summary> URL for the default resource 'button-zoomreset2.png' file ( http://cdn.sobekrepository.org/images/mapedit/button-zoomReset2.png by default)</summary>
        public static string Button_Zoomreset2_Png { get { return config.Button_Zoomreset2_Png; } }

        /// <summary> URL for the default resource 'calendar_button.png' file ( http://cdn.sobekrepository.org/images/misc/calendar_button.png by default)</summary>
        public static string Calendar_Button_Img { get { return config.Calendar_Button_Img; } }

        /// <summary> URL for the default resource 'cancel.ico' file ( http://cdn.sobekrepository.org/images/qc/Cancel.ico by default)</summary>
        public static string Cancel_Ico { get { return config.Cancel_Ico; } }

        /// <summary> URL for the default resource 'cc_by.png' file ( http://cdn.sobekrepository.org/images/misc/cc_by.png by default)</summary>
        public static string Cc_By_Img { get { return config.Cc_By_Img; } }

        /// <summary> URL for the default resource 'cc_by_nc.png' file ( http://cdn.sobekrepository.org/images/misc/cc_by_nc.png by default)</summary>
        public static string Cc_By_Nc_Img { get { return config.Cc_By_Nc_Img; } }

        /// <summary> URL for the default resource 'cc_by_nc_nd.png' file ( http://cdn.sobekrepository.org/images/misc/cc_by_nc_nd.png by default)</summary>
        public static string Cc_By_Nc_Nd_Img { get { return config.Cc_By_Nc_Nd_Img; } }

        /// <summary> URL for the default resource 'cc_by_nc_sa.png' file ( http://cdn.sobekrepository.org/images/misc/cc_by_nc_sa.png by default)</summary>
        public static string Cc_By_Nc_Sa_Img { get { return config.Cc_By_Nc_Sa_Img; } }

        /// <summary> URL for the default resource 'cc_by_nd.png' file ( http://cdn.sobekrepository.org/images/misc/cc_by_nd.png by default)</summary>
        public static string Cc_By_Nd_Img { get { return config.Cc_By_Nd_Img; } }

        /// <summary> URL for the default resource 'cc_by_sa.png' file ( http://cdn.sobekrepository.org/images/misc/cc_by_sa.png by default)</summary>
        public static string Cc_By_Sa_Img { get { return config.Cc_By_Sa_Img; } }

        /// <summary> URL for the default resource 'cc_zero.png' file ( http://cdn.sobekrepository.org/images/misc/cc_zero.png by default)</summary>
        public static string Cc_Zero_Img { get { return config.Cc_Zero_Img; } }

        /// <summary> URL for the default resource 'chart.js' file ( http://cdn.sobekrepository.org/includes/chartjs/1.0.2/Chart.min.js by default)</summary>
        public static string Chart_Js { get { return config.Chart_Js; } }

        /// <summary> URL for the default resource 'chat.png' file ( http://cdn.sobekrepository.org/images/misc/chat.png by default)</summary>
        public static string Chat_Png { get { return config.Chat_Png; } }

        /// <summary> URL for the default resource 'checkmark.png' file ( http://cdn.sobekrepository.org/images/misc/checkmark.png by default)</summary>
        public static string Checkmark_Png { get { return config.Checkmark_Png; } }

        /// <summary> URL for the default resource 'checkmark2.png' file ( http://cdn.sobekrepository.org/images/misc/checkmark2.png by default)</summary>
        public static string Checkmark2_Png { get { return config.Checkmark2_Png; } }

        ///// <summary> URL for the default resource 'ckeditor.js' file ( http://cdn.sobekrepository.org/includes/ckeditor/4.4.7/ckeditor.js by default)</summary>
        //public static string Ckeditor_Js { get { return config.; } }

        /// <summary> URL for the default resource 'closed_folder.jpg' file ( http://cdn.sobekrepository.org/images/misc/closed_folder.jpg by default)</summary>
        public static string Closed_Folder_Jpg { get { return config.Closed_Folder_Jpg; } }

        /// <summary> URL for the default resource 'closed_folder_public.jpg' file ( http://cdn.sobekrepository.org/images/misc/closed_folder_public.jpg by default)</summary>
        public static string Closed_Folder_Public_Jpg { get { return config.Closed_Folder_Public_Jpg; } }

        /// <summary> URL for the default resource 'closed_folder_public_big.jpg' file ( http://cdn.sobekrepository.org/images/misc/closed_folder_public_big.jpg by default)</summary>
        public static string Closed_Folder_Public_Big_Jpg { get { return config.Closed_Folder_Public_Big_Jpg; } }

        /// <summary> URL for the default resource 'contentslider.js' file ( http://cdn.sobekrepository.org/includes/contentslider/2.4/contentslider.min.js by default)</summary>
        public static string Contentslider_Js { get { return config.Contentslider_Js; } }

        /// <summary> URL for the default resource 'dark_resource.png' file ( http://cdn.sobekrepository.org/images/misc/dark_resource.png by default)</summary>
        public static string Dark_Resource_Png { get { return config.Dark_Resource_Png; } }

        /// <summary> URL for the default resource 'delete_cursor.cur' file ( http://cdn.sobekrepository.org/images/qc/delete_cursor.cur by default)</summary>
        public static string Delete_Cursor_Cur { get { return config.Delete_Cursor_Cur; } }

        /// <summary> URL for the default resource 'delete_item_icon.png' file ( http://cdn.sobekrepository.org/images/misc/delete_item_icon.png by default)</summary>
        public static string Delete_Item_Icon_Png { get { return config.Delete_Item_Icon_Png; } }

        /// <summary> URL for the default resource 'digg_share.gif' file ( http://cdn.sobekrepository.org/images/misc/digg_share.gif by default)</summary>
        public static string Digg_Share_Gif { get { return config.Digg_Share_Gif; } }

        /// <summary> URL for the default resource 'digg_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/digg_share_h.gif by default)</summary>
        public static string Digg_Share_H_Gif { get { return config.Digg_Share_H_Gif; } }

        /// <summary> URL for the default resource 'dloc_banner_700.jpg' file ( http://cdn.sobekrepository.org/images/misc/dloc_banner_700.jpg by default)</summary>
        public static string Dloc_Banner_700_Jpg { get { return config.Dloc_Banner_700_Jpg; } }

        /// <summary> URL for the default resource 'drag1pg.ico' file ( http://cdn.sobekrepository.org/images/qc/DRAG1PG.ICO by default)</summary>
        public static string Drag1pg_Ico { get { return config.Drag1pg_Ico; } }

        /// <summary> URL for the default resource 'edit.gif' file ( http://cdn.sobekrepository.org/images/misc/edit.gif by default)</summary>
        public static string Edit_Gif { get { return config.Edit_Gif; } }

        /// <summary> URL for the default resource 'edit.png' file ( http://cdn.sobekrepository.org/images/mapedit/edit.png by default)</summary>
        public static string Edit_Mapedit_Img { get { return config.Edit_Mapedit_Img; } }

        /// <summary> URL for the default resource 'edit_behaviors_icon.png' file ( http://cdn.sobekrepository.org/images/misc/edit_behaviors_icon.png by default)</summary>
        public static string Edit_Behaviors_Icon_Png { get { return config.Edit_Behaviors_Icon_Png; } }

        /// <summary> URL for the default resource 'edit_hierarchy.png' file ( http://cdn.sobekrepository.org/images/misc/edit_hierarchy.png by default)</summary>
        public static string Edit_Hierarchy_Png { get { return config.Edit_Hierarchy_Png; } }

        /// <summary> URL for the default resource 'edit_metadata_icon.png' file ( http://cdn.sobekrepository.org/images/misc/edit_metadata_icon.png by default)</summary>
        public static string Edit_Metadata_Icon_Png { get { return config.Edit_Metadata_Icon_Png; } }

        /// <summary> URL for the default resource 'email.png' file ( http://cdn.sobekrepository.org/images/misc/email.png by default)</summary>
        public static string Email_Png { get { return config.Email_Png; } }

        /// <summary> URL for the default resource 'emptypage.jpg' file ( http://cdn.sobekrepository.org/images/bookturner/emptypage.jpg by default)</summary>
        public static string Emptypage_Jpg { get { return config.Emptypage_Jpg; } }

        /// <summary> URL for the default resource 'exit.gif' file ( http://cdn.sobekrepository.org/images/misc/exit.gif by default)</summary>
        public static string Exit_Gif { get { return config.Exit_Gif; } }

        /// <summary> URL for the default resource 'facebook_share.gif' file ( http://cdn.sobekrepository.org/images/misc/facebook_share.gif by default)</summary>
        public static string Facebook_Share_Gif { get { return config.Facebook_Share_Gif; } }

        /// <summary> URL for the default resource 'facebook_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/facebook_share_h.gif by default)</summary>
        public static string Facebook_Share_H_Gif { get { return config.Facebook_Share_H_Gif; } }

        /// <summary> URL for the default resource 'favorites_share.gif' file ( http://cdn.sobekrepository.org/images/misc/favorites_share.gif by default)</summary>
        public static string Favorites_Share_Gif { get { return config.Favorites_Share_Gif; } }

        /// <summary> URL for the default resource 'favorites_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/favorites_share_h.gif by default)</summary>
        public static string Favorites_Share_H_Gif { get { return config.Favorites_Share_H_Gif; } }

        /// <summary> URL for the default resource 'file_ai.png' file ( http://cdn.sobekrepository.org/images/misc/file_ai.png by default)</summary>
        public static string File_AI_Img { get { return config.File_AI_Img; } }

        /// <summary> URL for the default resource 'file_eps.png' file ( http://cdn.sobekrepository.org/images/misc/file_eps.png by default)</summary>
        public static string File_EPS_Img { get { return config.File_EPS_Img; } }

        /// <summary> URL for the default resource 'file_excel.png' file ( http://cdn.sobekrepository.org/images/misc/file_excel.png by default)</summary>
        public static string File_Excel_Img { get { return config.File_Excel_Img; } }

        /// <summary> URL for the default resource 'file_kml.png' file ( http://cdn.sobekrepository.org/images/misc/file_kml.png by default)</summary>
        public static string File_KML_Img { get { return config.File_KML_Img; } }

        /// <summary> URL for the default resource 'file_pdf.png' file ( http://cdn.sobekrepository.org/images/misc/file_pdf.png by default)</summary>
        public static string File_PDF_Img { get { return config.File_PDF_Img; } }

        /// <summary> URL for the default resource 'file_psd.png' file ( http://cdn.sobekrepository.org/images/misc/file_psd.png by default)</summary>
        public static string File_PSD_Img { get { return config.File_PSD_Img; } }

        /// <summary> URL for the default resource 'file_pub.png' file ( http://cdn.sobekrepository.org/images/misc/file_pub.png by default)</summary>
        public static string File_PUB_Img { get { return config.File_PUB_Img; } }

        /// <summary> URL for the default resource 'file_txt.png' file ( http://cdn.sobekrepository.org/images/misc/file_txt.png by default)</summary>
        public static string File_TXT_Img { get { return config.File_TXT_Img; } }

        /// <summary> URL for the default resource 'file_word.png' file ( http://cdn.sobekrepository.org/images/misc/file_word.png by default)</summary>
        public static string File_Word_Img { get { return config.File_Word_Img; } }

        /// <summary> URL for the default resource 'file_xml.png' file ( http://cdn.sobekrepository.org/images/misc/file_xml.png by default)</summary>
        public static string File_XML_Img { get { return config.File_XML_Img; } }

        /// <summary> URL for the default resource 'file_vsd.png' file ( http://cdn.sobekrepository.org/images/misc/file_vsd.png by default)</summary>
        public static string File_VSD_Img { get { return config.File_VSD_Img; } }

        /// <summary> URL for the default resource 'file_zip.png' file ( http://cdn.sobekrepository.org/images/misc/file_zip.png by default)</summary>
        public static string File_ZIP_Img { get { return config.File_ZIP_Img; } }

        /// <summary> URL for the default resource 'file_management_icon.png' file ( http://cdn.sobekrepository.org/images/misc/file_management_icon.png by default)</summary>
        public static string File_Management_Icon_Png { get { return config.File_Management_Icon_Png; } }

        /// <summary> URL for the default resource 'firewall.gif' file ( http://cdn.sobekrepository.org/images/misc/firewall.gif by default)</summary>
        public static string Firewall_Img { get { return config.Firewall_Img; } }

        /// <summary> URL for the default resource 'firewall.png' file ( http://cdn.sobekrepository.org/images/misc/firewall.png by default)</summary>
        public static string Firewall_Img_Small { get { return config.Firewall_Img_Small; } }

        /// <summary> URL for the default resource 'first2.png' file ( http://cdn.sobekrepository.org/images/bookturner/first2.png by default)</summary>
        public static string First2_Png { get { return config.First2_Png; } }

        /// <summary> URL for the default resource 'gears.png' file ( http://cdn.sobekrepository.org/images/misc/gears.png by default)</summary>
        public static string Gears_Img { get { return config.Gears_Img; } }

        /// <summary> URL for the default resource 'gears_small.png' file ( http://cdn.sobekrepository.org/images/misc/gears_small.png by default)</summary>
        public static string Gears_Img_Small { get { return config.Gears_Img_Small; } }

        /// <summary> URL for the default resource 'gears_large.png' file ( http://cdn.sobekrepository.org/images/misc/gears_large.png by default)</summary>
        public static string Gears_Img_Large { get { return config.Gears_Img_Large; } }

        /// <summary> URL for the default resource 'geo_blue.png' file ( http://cdn.sobekrepository.org/images/mapsearch/geo_blue.png by default)</summary>
        public static string Geo_Blue_Png { get { return config.Geo_Blue_Png; } }

        /// <summary> URL for the default resource 'get_adobe_reader.png' file ( http://cdn.sobekrepository.org/images/misc/get_adobe_reader.png by default)</summary>
        public static string Get_Adobe_Reader_Png { get { return config.Get_Adobe_Reader_Png; } }

        /// <summary> URL for the default resource 'getuserlocation.png' file ( http://cdn.sobekrepository.org/images/mapedit/getUserLocation.png by default)</summary>
        public static string Getuserlocation_Png { get { return config.Getuserlocation_Png; } }

        /// <summary> URL for the default resource 'gmaps-infobox.js' file ( http://cdn.sobekrepository.org/includes/gmaps-infobox/1.0/gmaps-infobox.js by default)</summary>
        public static string Gmaps_Infobox_Js { get { return config.Gmaps_Infobox_Js; } }

        /// <summary> URL for the default resource 'gmaps-markerwithlabel.js' file ( http://cdn.sobekrepository.org/includes/gmaps-markerwithlabel/1.9.1/gmaps-markerwithlabel-1.9.1.js by default)</summary>
        public static string Gmaps_MarkerwithLabel_Js { get { return config.Gmaps_MarkerwithLabel_Js; } }

        /// <summary> URL for the default resource 'go_button.png' file ( http://cdn.sobekrepository.org/images/misc/go_button.png by default)</summary>
        public static string Go_Button_Png { get { return config.Go_Button_Png; } }

        /// <summary> URL for the default resource 'go_gray.gif' file ( http://cdn.sobekrepository.org/images/misc/go_gray.gif by default)</summary>
        public static string Go_Gray_Gif { get { return config.Go_Gray_Gif; } }

        /// <summary> URL for the default resource 'google_share.gif' file ( http://cdn.sobekrepository.org/images/misc/google_share.gif by default)</summary>
        public static string Google_Share_Gif { get { return config.Google_Share_Gif; } }

        /// <summary> URL for the default resource 'google_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/google_share_h.gif by default)</summary>
        public static string Google_Share_H_Gif { get { return config.Google_Share_H_Gif; } }

        /// <summary> URL for the default resource 'help_button.jpg' file ( http://cdn.sobekrepository.org/images/misc/help_button.jpg by default)</summary>
        public static string Help_Button_Jpg { get { return config.Help_Button_Jpg; } }

        /// <summary> URL for the default resource 'help_button_darkgray.jpg' file ( http://cdn.sobekrepository.org/images/misc/help_button_darkgray.jpg by default)</summary>
        public static string Help_Button_Darkgray_Jpg { get { return config.Help_Button_Darkgray_Jpg; } }

        /// <summary> URL for the default resource 'hide_internal_header.png' file ( http://cdn.sobekrepository.org/images/misc/hide_internal_header.png by default)</summary>
        public static string Hide_Internal_Header_Png { get { return config.Hide_Internal_Header_Png; } }

        /// <summary> URL for the default resource 'hide_internal_header2.png' file ( http://cdn.sobekrepository.org/images/misc/hide_internal_header2.png by default)</summary>
        public static string Hide_Internal_Header2_Png { get { return config.Hide_Internal_Header2_Png; } }

        /// <summary> URL for the default resource 'home.png' file ( http://cdn.sobekrepository.org/images/misc/home.png by default)</summary>
        public static string Home_Png { get { return config.Home_Png; } }

        /// <summary> URL for the default resource 'home_button.gif' file ( http://cdn.sobekrepository.org/images/misc/home_button.gif by default)</summary>
        public static string Home_Button_Gif { get { return config.Home_Button_Gif; } }

        /// <summary> URL for the default resource 'home_folder.gif' file ( http://cdn.sobekrepository.org/images/misc/home_folder.gif by default)</summary>
        public static string Home_Folder_Gif { get { return config.Home_Folder_Gif; } }

        /// <summary> URL for the default resource 'html5shiv.js' file ( http://cdn.sobekrepository.org/includes/html5shiv/3.7.3/html5shiv.js by default)</summary>
        public static string Html5shiv_Js { get { return config.Html5shiv_Js; } }

        /// <summary> URL for the default resource 'icons-os.png' file ( http://cdn.sobekrepository.org/images/mapedit/icons-os.png by default)</summary>
        public static string Icons_Os_Png { get { return config.Icons_Os_Png; } }

        /// <summary> URL for the default resource 'item_count.png' file ( http://cdn.sobekrepository.org/images/misc/item_count.png by default)</summary>
        public static string Item_Count_Img { get { return config.Item_Count_Img; } }

        /// <summary> URL for the default resource 'item_count_lg.png' file ( http://cdn.sobekrepository.org/images/misc/item_count_lg.png by default)</summary>
        public static string Item_Count_Img_Large { get { return config.Item_Count_Img_Large; } }

        /// <summary> URL for the default resource 'jquery.color-2.1.1.js' file ( http://cdn.sobekrepository.org/includes/jquery-color/2.1.1/jquery.color-2.1.1.js by default)</summary>
        public static string Jquery_Color_2_1_1_Js { get { return config.Jquery_Color_2_1_1_Js; } }

        /// <summary> URL for the default resource 'jquery.datatables.js' file ( http://cdn.sobekrepository.org/includes/datatables/1.11.1/js/jquery.dataTables.min.js by default)</summary>
        public static string Jquery_Datatables_Js { get { return config.Jquery_Datatables_Js; } }

        /// <summary> URL for the default resource 'jquery.easing.1.3.js' file ( http://cdn.sobekrepository.org/includes/bookturner/1.0.0/jquery.easing.1.3.js by default)</summary>
        public static string Jquery_Easing_1_3_Js { get { return config.Jquery_Easing_1_3_Js; } }

        /// <summary> URL for the default resource 'jquery.hovercard.js' file ( http://cdn.sobekrepository.org/includes/jquery-hovercard/2.4/jquery.hovercard.min.js by default)</summary>
        public static string Jquery_Hovercard_Js { get { return config.Jquery_Hovercard_Js; } }

        /// <summary> URL for the default resource 'jquery.mousewheel.js' file ( http://cdn.sobekrepository.org/includes/jquery-mousewheel/3.1.3/jquery.mousewheel.js by default)</summary>
        public static string Jquery_Mousewheel_Js { get { return config.Jquery_Mousewheel_Js; } }

        /// <summary> URL for the default resource 'jquery.qtip.css' file ( http://cdn.sobekrepository.org/includes/jquery-qtip/2.2.0/jquery.qtip.min.css by default)</summary>
        public static string Jquery_Qtip_Css { get { return config.Jquery_Qtip_Css; } }

        /// <summary> URL for the default resource 'jquery.qtip.js' file ( http://cdn.sobekrepository.org/includes/jquery-qtip/2.2.0/jquery.qtip.min.js by default)</summary>
        public static string Jquery_Qtip_Js { get { return config.Jquery_Qtip_Js; } }

        /// <summary> URL for the default resource 'jquery-searchbox.css' file ( http://cdn.sobekrepository.org/includes/jquery-searchbox/1.0/jquery-searchbox.css by default)</summary>
        public static string Jquery_Searchbox_Css { get { return config.Jquery_Searchbox_Css; } }

        /// <summary> URL for the default resource 'jquery.timeentry.js' file ( http://cdn.sobekrepository.org/includes/timeentry/1.5.2/jquery.timeentry.min.js by default)</summary>
        public static string Jquery_Timeentry_Js { get { return config.Jquery_Timeentry_Js; } }

        /// <summary> URL for the default resource 'jquery.timers.js' file ( http://cdn.sobekrepository.org/includes/jquery-timers/1.2/jquery.timers.min.js by default)</summary>
        public static string Jquery_Timers_Js { get { return config.Jquery_Timers_Js; } }

        /// <summary> URL for the default resource 'jquery.uploadifive.js' file ( http://cdn.sobekrepository.org/includes/uploadifive/1.1.2/jquery.uploadifive.min.js by default)</summary>
        public static string Jquery_Uploadifive_Js { get { return config.Jquery_Uploadifive_Js; } }

        /// <summary> URL for the default resource 'jquery.uploadify.js' file ( http://cdn.sobekrepository.org/includes/uploadify/3.2.1/jquery.uploadify.min.js by default)</summary>
        public static string Jquery_Uploadify_Js { get { return config.Jquery_Uploadify_Js; } }

        /// <summary> URL for the default resource 'jquery-1.10.2.js' file ( http://cdn.sobekrepository.org/includes/jquery/1.10.2/jquery-1.10.2.min.js by default)</summary>
        public static string Jquery_1_10_2_Js { get { return config.Jquery_1_10_2_Js; } }

        /// <summary> URL for the default resource 'jquery-1.2.6.min.js' file ( http://cdn.sobekrepository.org/includes/bookturner/1.0.0/jquery-1.2.6.min.js by default)</summary>
        public static string Jquery_1_2_6_Min_Js { get { return config.Jquery_1_2_6_Min_Js; } }

        /// <summary> URL for the default resource 'jquery-json-2.4.js' file ( http://cdn.sobekrepository.org/includes/jquery-json/2.4/jquery-json-2.4.min.js by default)</summary>
        public static string Jquery_Json_2_4_Js { get { return config.Jquery_Json_2_4_Js; } }

        /// <summary> URL for the default resource 'jquery-knob.js' file ( http://cdn.sobekrepository.org/includes/jquery-knob/1.2.0/jquery-knob.js by default)</summary>
        public static string Jquery_Knob_Js { get { return config.Jquery_Knob_Js; } }

        /// <summary> URL for the default resource 'jquery-migrate-1.1.1.js' file ( http://cdn.sobekrepository.org/includes/jquery-migrate/1.1.1/jquery-migrate-1.1.1.min.js by default)</summary>
        public static string Jquery_Migrate_1_1_1_Js { get { return config.Jquery_Migrate_1_1_1_Js; } }

        /// <summary> URL for the default resource 'jquery-rotate.js' file ( http://cdn.sobekrepository.org/includes/jquery-rotate/2.2/jquery-rotate.js by default)</summary>
        public static string Jquery_Rotate_Js { get { return config.Jquery_Rotate_Js; } }

        /// <summary> URL for the default resource 'jquery-ui-1.10.1.js' file ( http://cdn.sobekrepository.org/includes/jquery-ui/1.10.1/jquery-ui-1.10.1.js by default)</summary>
        public static string Jquery_Ui_1_10_1_Js { get { return config.Jquery_Ui_1_10_1_Js; } }

        /// <summary> URL for the default resource 'jquery-ui-1.10.3.custom.js' file ( http://cdn.sobekrepository.org/includes/jquery-ui/1.10.3/jquery-ui-1.10.3.custom.min.js by default)</summary>
        public static string Jquery_Ui_1_10_3_Custom_Js { get { return config.Jquery_Ui_1_10_3_Custom_Js; } }

        /// <summary> URL for the default resource 'jquery-ui-1.10.3.draggable.js' file ( http://cdn.sobekrepository.org/includes/jquery-ui-draggable/1.10.3/jquery-ui-1.10.3.draggable.min.js by default)</summary>
        public static string Jquery_Ui_1_10_3_Draggable_Js { get { return config.Jquery_Ui_1_10_3_Draggable_Js; } }

        /// <summary> URL for the default resource 'jquery-ui.css' file ( http://cdn.sobekrepository.org/includes/jquery-ui/1.10.3/jquery-ui.css by default)</summary>
        public static string Jquery_Ui_Css { get { return config.Jquery_Ui_Css; } }

        /// <summary> URL for the default resource 'jsdatepick.min.1.3.js' file ( http://cdn.sobekrepository.org/includes/datepicker/1.3/jsDatePick.min.1.3.js by default)</summary>
        public static string Jsdatepick_Min_1_3_Js { get { return config.Jsdatepick_Min_1_3_Js; } }

        /// <summary> URL for the default resource 'jsdatepick_ltr.css' file ( http://cdn.sobekrepository.org/includes/datepicker/1.3/jsDatePick_ltr.css by default)</summary>
        public static string Jsdatepick_Ltr_Css { get { return config.Jsdatepick_Ltr_Css; } }

        /// <summary> URL for the default resource 'jstree.css' file ( http://cdn.sobekrepository.org/includes/jstree/3.0.9/themes/default/style.min.css by default)</summary>
        public static string Jstree_Css { get { return config.Jstree_Css; } }

        /// <summary> URL for the default resource 'jstree.js' file ( http://cdn.sobekrepository.org/includes/jstree/3.0.9/jstree.min.js by default)</summary>
        public static string Jstree_Js { get { return config.Jstree_Js; } }

        /// <summary> URL for the default resource 'keydragzoom_packed.js' file ( http://cdn.sobekrepository.org/includes/keydragzoom/1.0/keydragzoom_packed.js by default)</summary>
        public static string Keydragzoom_Packed_Js { get { return config.Keydragzoom_Packed_Js; } }

        /// <summary> URL for the default resource 'last2.png' file ( http://cdn.sobekrepository.org/images/bookturner/last2.png by default)</summary>
        public static string Last2_Png { get { return config.Last2_Png; } }

        /// <summary> URL for the default resource 'leftarrow.png' file ( http://cdn.sobekrepository.org/images/misc/leftarrow.png by default)</summary>
        public static string Leftarrow_Png { get { return config.Leftarrow_Png; } }

        /// <summary> URL for the default resource 'legend_nonselected_polygon.png' file ( http://cdn.sobekrepository.org/images/misc/legend_nonselected_polygon.png by default)</summary>
        public static string Legend_Nonselected_Polygon_Png { get { return config.Legend_Nonselected_Polygon_Png; } }

        /// <summary> URL for the default resource 'legend_point_interest.png' file ( http://cdn.sobekrepository.org/images/misc/legend_point_interest.png by default)</summary>
        public static string Legend_Point_Interest_Png { get { return config.Legend_Point_Interest_Png; } }

        /// <summary> URL for the default resource 'legend_red_pushpin.png' file ( http://cdn.sobekrepository.org/images/misc/legend_red_pushpin.png by default)</summary>
        public static string Legend_Red_Pushpin_Png { get { return config.Legend_Red_Pushpin_Png; } }

        /// <summary> URL for the default resource 'legend_search_area.png' file ( http://cdn.sobekrepository.org/images/misc/legend_search_area.png by default)</summary>
        public static string Legend_Search_Area_Png { get { return config.Legend_Search_Area_Png; } }

        /// <summary> URL for the default resource 'legend_selected_polygon.png' file ( http://cdn.sobekrepository.org/images/misc/legend_selected_polygon.png by default)</summary>
        public static string Legend_Selected_Polygon_Png { get { return config.Legend_Selected_Polygon_Png; } }

        /// <summary> URL for the default resource 'main_information.ico' file ( http://cdn.sobekrepository.org/images/qc/Main_Information.ICO by default)</summary>
        public static string Main_Information_Ico { get { return config.Main_Information_Ico; } }

        /// <summary> URL for the default resource 'manage_collection.png' file ( http://cdn.sobekrepository.org/images/misc/manage_collection.png by default)</summary>
        public static string Manage_Collection_Img { get { return config.Manage_Collection_Img; } }

        /// <summary> URL for the default resource 'map_drag_hand.gif' file ( http://cdn.sobekrepository.org/images/misc/map_drag_hand.gif by default)</summary>
        public static string Map_Drag_Hand_Gif { get { return config.Map_Drag_Hand_Gif; } }

        /// <summary> URL for the default resource 'map_point.gif' file ( http://cdn.sobekrepository.org/images/misc/map_point.gif by default)</summary>
        public static string Map_Tack_Img { get { return config.Map_Tack_Img; } }

        /// <summary> URL for the default resource 'map_point.png' file ( http://cdn.sobekrepository.org/images/misc/map_point.png by default)</summary>
        public static string Map_Point_Png { get { return config.Map_Point_Png; } }

        /// <summary> URL for the default resource 'map_polygon2.gif' file ( http://cdn.sobekrepository.org/images/misc/map_polygon2.gif by default)</summary>
        public static string Map_Polygon2_Gif { get { return config.Map_Polygon2_Gif; } }

        /// <summary> URL for the default resource 'map_rectangle2.gif' file ( http://cdn.sobekrepository.org/images/misc/map_rectangle2.gif by default)</summary>
        public static string Map_Rectangle2_Gif { get { return config.Map_Rectangle2_Gif; } }

        /// <summary> URL for the default resource 'mass_update_icon.png' file ( http://cdn.sobekrepository.org/images/misc/mass_update_icon.png by default)</summary>
        public static string Mass_Update_Icon_Png { get { return config.Mass_Update_Icon_Png; } }

        /// <summary> URL for the default resource 'metadata_browse_large.png' file ( http://cdn.sobekrepository.org/images/misc/metadata_browse_large.png by default)</summary>
        public static string Metadata_Browse_Img_Large { get { return config.Metadata_Browse_Img_Large; } }

        /// <summary> URL for the default resource 'metadata_browse.png' file ( http://cdn.sobekrepository.org/images/misc/metadata_browse.png by default)</summary>
        public static string Metadata_Browse_Img { get { return config.Metadata_Browse_Img; } }

        /// <summary> URL for the default resource 'minussign.png' file ( http://cdn.sobekrepository.org/images/misc/minussign.png by default)</summary>
        public static string Minussign_Png { get { return config.Minussign_Png; } }

        /// <summary> URL for the default resource 'missingimage.jpg' file ( http://cdn.sobekrepository.org/images/misc/MissingImage.jpg by default)</summary>
        public static string Missingimage_Jpg { get { return config.Missingimage_Jpg; } }

        /// <summary> URL for the default resource 'move_pages_cursor.cur' file ( http://cdn.sobekrepository.org/images/qc/move_pages_cursor.cur by default)</summary>
        public static string Move_Pages_Cursor_Cur { get { return config.Move_Pages_Cursor_Cur; } }

        /// <summary> URL for the default resource 'new_element.jpg' file ( http://cdn.sobekrepository.org/images/misc/new_element.jpg by default)</summary>
        public static string New_Element_Jpg { get { return config.New_Element_Jpg; } }

        /// <summary> URL for the default resource 'new_element_demo.jpg' file ( http://cdn.sobekrepository.org/images/misc/new_element_demo.jpg by default)</summary>
        public static string New_Element_Demo_Jpg { get { return config.New_Element_Demo_Jpg; } }

        /// <summary> URL for the default resource 'new_folder.jpg' file ( http://cdn.sobekrepository.org/images/misc/new_folder.jpg by default)</summary>
        public static string New_Folder_Jpg { get { return config.New_Folder_Jpg; } }

        /// <summary> URL for the default resource 'new_item_medium.png' file ( http://cdn.sobekrepository.org/images/misc/new_item_medium.png by default)</summary>
        public static string New_Item_Img { get { return config.New_Item_Img; } }

        /// <summary> URL for the default resource 'new_item_large.png' file ( http://cdn.sobekrepository.org/images/misc/new_item_large.png by default)</summary>
        public static string New_Item_Img_Large { get { return config.New_Item_Img_Large; } }

        /// <summary> URL for the default resource 'new_item_small.png' file ( http://cdn.sobekrepository.org/images/misc/new_item_small.png by default)</summary>
        public static string New_Item_Img_Small { get { return config.New_Item_Img_Small; } }

        /// <summary> URL for the default resource 'next.png' file ( http://cdn.sobekrepository.org/images/bookturner/next.png by default)</summary>
        public static string Next_Png { get { return config.Next_Png; } }

        /// <summary> URL for the default resource 'next2.png' file ( http://cdn.sobekrepository.org/images/bookturner/next2.png by default)</summary>
        public static string Next2_Png { get { return config.Next2_Png; } }

        /// <summary> URL for the default resource 'no_pages.jpg' file ( http://cdn.sobekrepository.org/images/qc/no_pages.jpg by default)</summary>
        public static string No_Pages_Jpg { get { return config.No_Pages_Jpg; } }

        /// <summary> URL for the default resource 'nocheckmark.png' file ( http://cdn.sobekrepository.org/images/misc/nocheckmark.png by default)</summary>
        public static string Nocheckmark_Png { get { return config.Nocheckmark_Png; } }

        /// <summary> URL for the default resource 'nothumb.jpg' file ( http://cdn.sobekrepository.org/images/misc/NoThumb.jpg by default)</summary>
        public static string Nothumb_Jpg { get { return config.Nothumb_Jpg; } }

        /// <summary> URL for the default resource 'open_folder.jpg' file ( http://cdn.sobekrepository.org/images/misc/open_folder.jpg by default)</summary>
        public static string Open_Folder_Jpg { get { return config.Open_Folder_Jpg; } }

        /// <summary> URL for the default resource 'open_folder_public.jpg' file ( http://cdn.sobekrepository.org/images/misc/open_folder_public.jpg by default)</summary>
        public static string Open_Folder_Public_Jpg { get { return config.Open_Folder_Public_Jpg; } }

        /// <summary> URL for the default resource 'pagenumbg.gif' file ( http://cdn.sobekrepository.org/images/bookturner/pageNumBg.gif by default)</summary>
        public static string Pagenumbg_Gif { get { return config.Pagenumbg_Gif; } }

        /// <summary> URL for the default resource 'plussign.png' file ( http://cdn.sobekrepository.org/images/misc/plussign.png by default)</summary>
        public static string Plussign_Png { get { return config.Plussign_Png; } }

        /// <summary> URL for the default resource 'pmets.gif' file ( http://cdn.sobekrepository.org/images/misc/pmets.gif by default)</summary>
        public static string Pmets_Img { get { return config.Pmets_Img; } }

        /// <summary> URL for the default resource 'point02.ico' file ( http://cdn.sobekrepository.org/images/qc/POINT02.ICO by default)</summary>
        public static string Point02_Ico { get { return config.Point02_Ico; } }

        /// <summary> URL for the default resource 'point04.ico' file ( http://cdn.sobekrepository.org/images/qc/POINT04.ICO by default)</summary>
        public static string Point04_Ico { get { return config.Point04_Ico; } }

        /// <summary> URL for the default resource 'point13.ico' file ( http://cdn.sobekrepository.org/images/qc/POINT13.ICO by default)</summary>
        public static string Point13_Ico { get { return config.Point13_Ico; } }

        /// <summary> URL for the default resource 'pointer_blue.gif' file ( http://cdn.sobekrepository.org/images/misc/pointer_blue.gif by default)</summary>
        public static string Pointer_Blue_Gif { get { return config.Pointer_Blue_Gif; } }

        /// <summary> URL for the default resource 'portal_large.png' file ( http://cdn.sobekrepository.org/images/misc/portal_large.png by default)</summary>
        public static string Portals_Img_Large { get { return config.Portals_Img_Large; } }

        /// <summary> URL for the default resource 'portal.png' file ( http://cdn.sobekrepository.org/images/misc/portal.png by default)</summary>
        public static string Portals_Img { get { return config.Portals_Img; } }

        /// <summary> URL for the default resource 'portals_small.png' file ( http://cdn.sobekrepository.org/images/misc/portals.png by default)</summary>
        public static string Portals_Img_Small { get { return config.Portals_Img_Small; } }

        /// <summary> URL for the default resource 'previous2.png' file ( http://cdn.sobekrepository.org/images/bookturner/previous2.png by default)</summary>
        public static string Previous2_Png { get { return config.Previous2_Png; } }

        /// <summary> URL for the default resource 'print.css' file ( http://cdn.sobekrepository.org/css/sobekcm-print/4.8.4/print.css by default)</summary>
        public static string Print_Css { get { return config.Print_Css; } }

        /// <summary> URL for the default resource 'printer.png' file ( http://cdn.sobekrepository.org/images/misc/printer.png by default)</summary>
        public static string Printer_Png { get { return config.Printer_Png; } }

        /// <summary> URL for the default resource 'private_items.png' file ( http://cdn.sobekrepository.org/images/misc/private_items.png by default)</summary>
        public static string Private_Items_Img { get { return config.Private_Items_Img; } }

        /// <summary> URL for the default resource 'private_items_lg.png' file ( http://cdn.sobekrepository.org/images/misc/private_items_lg.png by default)</summary>
        public static string Private_Items_Img_Large { get { return config.Private_Items_Img_Large; } }

        /// <summary> URL for the default resource 'private_resource_icon.png' file ( http://cdn.sobekrepository.org/images/misc/private_resource_icon.png by default)</summary>
        public static string Private_Resource_Img_Jumbo { get { return config.Private_Resource_Img_Jumbo; } }

        /// <summary> URL for the default resource 'public_resource_icon.png' file ( http://cdn.sobekrepository.org/images/misc/public_resource_icon.png by default)</summary>
        public static string Public_Resource_Img_Jumbo { get { return config.Public_Resource_Img_Jumbo; } }

        /// <summary> URL for the default resource 'qc_addfiles.png' file ( http://cdn.sobekrepository.org/images/qc/qc_addfiles.png by default)</summary>
        public static string Qc_Addfiles_Png { get { return config.Qc_Addfiles_Png; } }

        /// <summary> URL for the default resource 'qc_button_icon.png' file ( http://cdn.sobekrepository.org/images/misc/qc_button_icon.png by default)</summary>
        public static string Qc_Button_Img_Large { get { return config.Qc_Button_Img_Large; } }

        /// <summary> URL for the default resource 'rect_large.ico' file ( http://cdn.sobekrepository.org/images/qc/rect_large.ico by default)</summary>
        public static string Rect_Large_Ico { get { return config.Rect_Large_Ico; } }

        /// <summary> URL for the default resource 'rect_medium.ico' file ( http://cdn.sobekrepository.org/images/qc/rect_medium.ico by default)</summary>
        public static string Rect_Medium_Ico { get { return config.Rect_Medium_Ico; } }

        /// <summary> URL for the default resource 'rect_small.ico' file ( http://cdn.sobekrepository.org/images/qc/rect_small.ico by default)</summary>
        public static string Rect_Small_Ico { get { return config.Rect_Small_Ico; } }

        /// <summary> URL for the default resource 'red-pushpin.png' file ( http://cdn.sobekrepository.org/images/mapedit/mapIcons/red-pushpin.png by default)</summary>
        public static string Red_Pushpin_Png { get { return config.Red_Pushpin_Png; } }

        /// <summary> URL for the default resource 'refresh.png' file ( http://cdn.sobekrepository.org/images/misc/refresh.png by default)</summary>
        public static string Refresh_Img { get { return config.Refresh_Img; } }

        /// <summary> URL for the default resource 'refresh_small.png' file ( http://cdn.sobekrepository.org/images/misc/refresh_small.png by default)</summary>
        public static string Refresh_Img_Small { get { return config.Refresh_Img_Small; } }

        /// <summary> URL for the default resource 'refresh_large.png' file ( http://cdn.sobekrepository.org/images/misc/refresh_large.png by default)</summary>
        public static string Refresh_Img_Large { get { return config.Refresh_Img_Large; } }

        /// <summary> URL for the default resource 'refresh_folder.jpg' file ( http://cdn.sobekrepository.org/images/misc/refresh_folder.jpg by default)</summary>
        public static string Refresh_Folder_Jpg { get { return config.Refresh_Folder_Jpg; } }

        /// <summary> URL for the default resource 'removeicon.gif' file ( http://cdn.sobekrepository.org/images/mapsearch/removeIcon.gif by default)</summary>
        public static string Removeicon_Gif { get { return config.Removeicon_Gif; } }

        /// <summary> URL for the default resource 'restricted_resource_lg.png' file ( http://cdn.sobekrepository.org/images/misc/restricted_resource_lg.png by default)</summary>
        public static string Restricted_Resource_Img_Large { get { return config.Restricted_Resource_Img_Large; } }

        /// <summary> URL for the default resource 'restricted_resource_icon.png' file ( http://cdn.sobekrepository.org/images/misc/restricted_resource_icon.png by default)</summary>
        public static string Restricted_Resource_Img_Jumbo { get { return config.Restricted_Resource_Img_Jumbo; } }

        /// <summary> URL for the default resource 'return.gif' file ( http://cdn.sobekrepository.org/images/misc/return.gif by default)</summary>
        public static string Return_Img { get { return config.Return_Img; } }

        /// <summary> URL for the default resource 'rotation-clockwise.png' file ( http://cdn.sobekrepository.org/images/mapedit/rotation-clockwise.png by default)</summary>
        public static string Rotation_Clockwise_Png { get { return config.Rotation_Clockwise_Png; } }

        /// <summary> URL for the default resource 'rotation-counterclockwise.png' file ( http://cdn.sobekrepository.org/images/mapedit/rotation-counterClockwise.png by default)</summary>
        public static string Rotation_Counterclockwise_Png { get { return config.Rotation_Counterclockwise_Png; } }

        /// <summary> URL for the default resource 'rotation-reset.png' file ( http://cdn.sobekrepository.org/images/mapedit/rotation-reset.png by default)</summary>
        public static string Rotation_Reset_Png { get { return config.Rotation_Reset_Png; } }

        /// <summary> URL for the default resource 'save.ico' file ( http://cdn.sobekrepository.org/images/qc/Save.ico by default)</summary>
        public static string Save_Ico { get { return config.Save_Ico; } }

        /// <summary> URL for the default resource 'saved_searches.gif' file ( http://cdn.sobekrepository.org/images/misc/saved_searches.gif by default)</summary>
        public static string Saved_Searches_Img { get { return config.Saved_Searches_Img; } }

        /// <summary> URL for the default resource 'saved_searches_big.gif' file ( http://cdn.sobekrepository.org/images/misc/saved_searches_big.gif by default)</summary>
        public static string Saved_Searches_Img_Jumbo { get { return config.Saved_Searches_Img_Jumbo; } }

        /// <summary> URL for the default resource 'search.png' file ( http://cdn.sobekrepository.org/images/mapedit/search.png by default)</summary>
        public static string Search_Png { get { return config.Search_Png; } }

        /// <summary> URL for the default resource 'search_advanced.png' file ( http://cdn.sobekrepository.org/images/misc/search_advanced.png by default)</summary>
        public static string Search_Advanced_Img { get { return config.Search_Advanced_Img; } }

        /// <summary> URL for the default resource 'search_advanced_mimetype.png' file ( http://cdn.sobekrepository.org/images/misc/search_advanced_mimetype.png by default)</summary>
        public static string Search_Advanced_MimeType_Img { get { return config.Search_Advanced_MimeType_Img; } }

        /// <summary> URL for the default resource 'search_advanced_year_range.png' file ( http://cdn.sobekrepository.org/images/misc/search_advanced_year_range.png by default)</summary>
        public static string Search_Advanced_Year_Range_Img { get { return config.Search_Advanced_Year_Range_Img; } }

        /// <summary> URL for the default resource 'search_basic.png' file ( http://cdn.sobekrepository.org/images/misc/search_basic.png by default)</summary>
        public static string Search_Basic_Img { get { return config.Search_Basic_Img; } }

        /// <summary> URL for the default resource 'search_basic_mimetype.png' file ( http://cdn.sobekrepository.org/images/misc/search_basic_mimetype.png by default)</summary>
        public static string Search_Basic_MimeType_Img { get { return config.Search_Basic_MimeType_Img; } }

        /// <summary> URL for the default resource 'search_basic_year_range.png' file ( http://cdn.sobekrepository.org/images/misc/search_basic_year_range.png by default)</summary>
        public static string Search_Basic_Year_Range_Img { get { return config.Search_Basic_Year_Range_Img; } }

        /// <summary> URL for the default resource 'search_basic_year_range.png' file ( http://cdn.sobekrepository.org/images/misc/search_basic_with_fulltext.png by default)</summary>
        public static string Search_Basic_With_FullText_Img { get { return config.Search_Basic_With_FullText_Img; } }

        /// <summary> URL for the default resource 'search_full_text.png' file ( http://cdn.sobekrepository.org/images/misc/search_full_text.png by default)</summary>
        public static string Search_Full_Text_Img { get { return config.Search_Full_Text_Img; } }

        /// <summary> URL for the default resource 'search_fulltext_exclude_newspapers.png' file ( http://cdn.sobekrepository.org/images/misc/search_fulltext_exclude_newspapers.png by default)</summary>
        public static string Search_Full_Text_Exlude_Newspapers_Img { get { return config.Search_Full_Text_Exlude_Newspapers_Img; } }

        /// <summary> URL for the default resource 'search_map.png' file ( http://cdn.sobekrepository.org/images/misc/search_map.png by default)</summary>
        public static string Search_Map_Img { get { return config.Search_Map_Img; } }

        /// <summary> URL for the default resource 'search_newspaper.png' file ( http://cdn.sobekrepository.org/images/misc/search_newspaper.png by default)</summary>
        public static string Search_Newspaper_Img { get { return config.Search_Newspaper_Img; } }

        /// <summary> URL for the default resource 'settings.png' file ( http://cdn.sobekrepository.org/images/misc/settings.png by default)</summary>
        public static string Settings_Img { get { return config.Settings_Img; } }

        /// <summary> URL for the default resource 'settings_small.png' file ( http://cdn.sobekrepository.org/images/misc/settings_small.png by default)</summary>
        public static string Settings_Img_Small { get { return config.Settings_Img_Small; } }

        /// <summary> URL for the default resource 'settings_large.png' file ( http://cdn.sobekrepository.org/images/misc/settings_large.png by default)</summary>
        public static string Settings_Img_Large { get { return config.Settings_Img_Large; } }

        /// <summary> URL for the default resource 'show_internal_header.png' file ( http://cdn.sobekrepository.org/images/misc/show_internal_header.png by default)</summary>
        public static string Show_Internal_Header_Png { get { return config.Show_Internal_Header_Png; } }

        /// <summary> URL for the default resource 'skins.gif' file ( http://cdn.sobekrepository.org/images/misc/skins.gif by default)</summary>
        public static string Skins_Img { get { return config.Skins_Img; } }

        /// <summary> URL for the default resource 'skins.png' file ( http://cdn.sobekrepository.org/images/misc/skins.png by default)</summary>
        public static string Skins_Img_Small { get { return config.Skins_Img_Small; } }

        /// <summary> URL for the default resource 'skins_lg.png' file ( http://cdn.sobekrepository.org/images/misc/skins_lg.png by default)</summary>
        public static string Skins_Img_Large { get { return config.Skins_Img_Large; } }

        /// <summary> URL for the default resource 'sobekcm.css' file ( http://cdn.sobekrepository.org/css/sobekcm/4.8.4/SobekCM.min.css by default)</summary>
        public static string Sobekcm_Css { get { return config.Sobekcm_Css; } }

        /// <summary> URL for the default resource 'sobekcm_admin.css' file ( http://cdn.sobekrepository.org/css/sobekcm-admin/4.8.4/SobekCM_Admin.min.css by default)</summary>
        public static string Sobekcm_Admin_Css { get { return config.Sobekcm_Admin_Css; } }

        /// <summary> URL for the default resource 'sobekcm_admin.js' file ( http://cdn.sobekrepository.org/js/sobekcm-admin/4.8.4/sobekcm_admin.js by default)</summary>
        public static string Sobekcm_Admin_Js { get { return config.Sobekcm_Admin_Js; } }

        /// <summary> URL for the default resource 'sobekcm_bookturner.css' file ( http://cdn.sobekrepository.org/css/sobekcm-bookturner/4.8.4/SobekCM_BookTurner.css by default)</summary>
        public static string Sobekcm_Bookturner_Css { get { return config.Sobekcm_Bookturner_Css; } }

        /// <summary> URL for the default resource 'sobekcm_datatables.css' file ( http://cdn.sobekrepository.org/css/sobekcm-datatables/4.8.4/SobekCM_DataTables.css by default)</summary>
        public static string Sobekcm_Datatables_Css { get { return config.Sobekcm_Datatables_Css; } }

        /// <summary> URL for the default resource 'sobekcm_full.js' file ( http://cdn.sobekrepository.org/js/sobekcm-full/4.8.4/sobekcm_full.min.js by default)</summary>
        public static string Sobekcm_Full_Js { get { return config.Sobekcm_Full_Js; } }

        /// <summary> URL for the default resource 'sobekcm_item.css' file ( http://cdn.sobekrepository.org/css/sobekcm-item/4.8.4/SobekCM_Item.min.css by default)</summary>
        public static string Sobekcm_Item_Css { get { return config.Sobekcm_Item_Css; } }

        /// <summary> URL for the default resource 'sobekcm_map_search.js' file ( http://cdn.sobekrepository.org/js/sobekcm-map-editor/4.8.4/sobekcm_map_editor.js by default)</summary>
        public static string Sobekcm_Map_Editor_Js { get { return config.Sobekcm_Map_Editor_Js; } }

        /// <summary> URL for the default resource 'sobekcm_map_search.js' file ( http://cdn.sobekrepository.org/js/sobekcm-map/4.8.4/sobekcm_map_search.js by default)</summary>
        public static string Sobekcm_Map_Search_Js { get { return config.Sobekcm_Map_Search_Js; } }

        /// <summary> URL for the default resource 'sobekcm_map_tool.js' file ( http://cdn.sobekrepository.org/js/sobekcm-map/4.8.4/sobekcm_map_tool.js by default)</summary>
        public static string Sobekcm_Map_Tool_Js { get { return config.Sobekcm_Map_Tool_Js; } }

        /// <summary> URL for the default resource 'sobekcm_mapeditor.css' file ( http://cdn.sobekrepository.org/css/sobekcm-map/4.8.4/SobekCM_MapEditor.css by default)</summary>
        public static string Sobekcm_Mapeditor_Css { get { return config.Sobekcm_Mapeditor_Css; } }

        /// <summary> URL for the default resource 'sobekcm_mapsearch.css' file ( http://cdn.sobekrepository.org/css/sobekcm-map/4.8.4/SobekCM_MapSearch.css by default)</summary>
        public static string Sobekcm_Mapsearch_Css { get { return config.Sobekcm_Mapsearch_Css; } }

        /// <summary> URL for the default resource 'sobekcm_metadata.css' file ( http://cdn.sobekrepository.org/css/sobekcm-metadata/4.8.4/SobekCM_Metadata.min.css by default)</summary>
        public static string Sobekcm_Metadata_Css { get { return config.Sobekcm_Metadata_Css; } }

        /// <summary> URL for the default resource 'sobekcm_metadata.js' file ( http://cdn.sobekrepository.org/js/sobekcm-metadata/4.8.4/sobekcm_metadata.js by default)</summary>
        public static string Sobekcm_Metadata_Js { get { return config.Sobekcm_Metadata_Js; } }

        /// <summary> URL for the default resource 'sobekcm_mysobek.css' file ( http://cdn.sobekrepository.org/css/sobekcm-mysobek/4.8.4/SobekCM_MySobek.min.css by default)</summary>
        public static string Sobekcm_Mysobek_Css { get { return config.Sobekcm_Mysobek_Css; } }

        /// <summary> URL for the default resource 'sobekcm_print.css' file ( http://cdn.sobekrepository.org/css/sobekcm-print/4.8.4/SobekCM_Print.css by default)</summary>
        public static string Sobekcm_Print_Css { get { return config.Sobekcm_Print_Css; } }

        /// <summary> URL for the default resource 'sobekcm_qc.css' file ( http://cdn.sobekrepository.org/css/sobekcm-qc/4.8.4/SobekCM_QC.css by default)</summary>
        public static string Sobekcm_Qc_Css { get { return config.Sobekcm_Qc_Css; } }

        /// <summary> URL for the default resource 'sobekcm_qc.js' file ( http://cdn.sobekrepository.org/js/sobekcm-qc/4.8.4/sobekcm_qc.js by default)</summary>
        public static string Sobekcm_Qc_Js { get { return config.Sobekcm_Qc_Js; } }

        /// <summary> URL for the default resource 'sobekcm_stats.css' file ( http://cdn.sobekrepository.org/css/sobekcm-stats/4.8.4/SobekCM_Stats.css by default)</summary>
        public static string Sobekcm_Stats_Css { get { return config.Sobekcm_Stats_Css; } }

        /// <summary> URL for the default resource 'sobekcm_thumb_results.js' file ( http://cdn.sobekrepository.org/js/sobekcm-thumb-results/4.8.4/sobekcm_thumb_results.js by default)</summary>
        public static string Sobekcm_Thumb_Results_Js { get { return config.Sobekcm_Thumb_Results_Js; } }

        /// <summary> URL for the default resource 'sobekcm_track_item.js' file ( http://cdn.sobekrepository.org/js/sobekcm-track-item/4.8.4/sobekcm_track_item.js by default)</summary>
        public static string Sobekcm_Track_Item_Js { get { return config.Sobekcm_Track_Item_Js; } }

        /// <summary> URL for the default resource 'sobekcm_trackingsheet.css' file ( http://cdn.sobekrepository.org/css/sobekcm-tracking/4.8.4/SobekCM_TrackingSheet.css by default)</summary>
        public static string Sobekcm_Trackingsheet_Css { get { return config.Sobekcm_Trackingsheet_Css; } }

        /// <summary> URL for the default resource 'spinner.gif' file ( http://cdn.sobekrepository.org/images/misc/spinner.gif by default)</summary>
        public static string Spinner_Gif { get { return config.Spinner_Gif; } }

        /// <summary> URL for the default resource 'spinner_gray.gif' file ( http://cdn.sobekrepository.org/images/misc/spinner_gray.gif by default)</summary>
        public static string Spinner_Gray_Gif { get { return config.Spinner_Gray_Gif; } }

        /// <summary> URL for the default resource 'stumbleupon_share.gif' file ( http://cdn.sobekrepository.org/images/misc/stumbleupon_share.gif by default)</summary>
        public static string Stumbleupon_Share_Gif { get { return config.Stumbleupon_Share_Gif; } }

        /// <summary> URL for the default resource 'stumbleupon_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/stumbleupon_share_h.gif by default)</summary>
        public static string Stumbleupon_Share_H_Gif { get { return config.Stumbleupon_Share_H_Gif; } }

        /// <summary> URL for the default resource 'submitted_items.gif' file ( http://cdn.sobekrepository.org/images/misc/submitted_items.gif by default)</summary>
        public static string Submitted_Items_Gif { get { return config.Submitted_Items_Gif; } }

        /// <summary> URL for the default resource 'table_blue.png' file ( http://cdn.sobekrepository.org/images/mapsearch/table_blue.png by default)</summary>
        public static string Table_Blue_Png { get { return config.Table_Blue_Png; } }

        /// <summary> URL for the default resource 'thematic_heading.png' file ( http://cdn.sobekrepository.org/images/misc/thematic_heading.png by default)</summary>
        public static string Thematic_Heading_Img { get { return config.Thematic_Heading_Img; } }

        /// <summary> URL for the default resource 'thematic_heading.gif' file ( http://cdn.sobekrepository.org/images/misc/thematic_heading.gif by default)</summary>
        public static string Thematic_Heading_Img_Small { get { return config.Thematic_Heading_Img_Small; } }

        /// <summary> URL for the default resource 'thematic_heading_lg.png' file ( http://cdn.sobekrepository.org/images/misc/thematic_heading_lg.png by default)</summary>
        public static string Thematic_Heading_Img_Large { get { return config.Thematic_Heading_Img_Large; } }

        /// <summary> URL for the default resource 'thumb_blue.png' file ( http://cdn.sobekrepository.org/images/mapsearch/thumb_blue.png by default)</summary>
        public static string Thumb_Blue_Png { get { return config.Thumb_Blue_Png; } }

        /// <summary> URL for the default resource 'thumbnail_cursor.cur' file ( http://cdn.sobekrepository.org/images/qc/thumbnail_cursor.cur by default)</summary>
        public static string Thumbnail_Cursor_Cur { get { return config.Thumbnail_Cursor_Cur; } }

        /// <summary> URL for the default resource 'thumbnail_large.gif' file ( http://cdn.sobekrepository.org/images/misc/thumbnail_large.gif by default)</summary>
        public static string Thumbnail_Large_Gif { get { return config.Thumbnail_Large_Gif; } }

        /// <summary> URL for the default resource 'thumbs1.gif' file ( http://cdn.sobekrepository.org/images/misc/thumbs1.gif by default)</summary>
        public static string Thumbs1_Gif { get { return config.Thumbs1_Gif; } }

        /// <summary> URL for the default resource 'thumbs1_selected.gif' file ( http://cdn.sobekrepository.org/images/misc/thumbs1_selected.gif by default)</summary>
        public static string Thumbs1_Selected_Gif { get { return config.Thumbs1_Selected_Gif; } }

        /// <summary> URL for the default resource 'thumbs2.gif' file ( http://cdn.sobekrepository.org/images/misc/thumbs2.gif by default)</summary>
        public static string Thumbs2_Gif { get { return config.Thumbs2_Gif; } }

        /// <summary> URL for the default resource 'thumbs2_selected.gif' file ( http://cdn.sobekrepository.org/images/misc/thumbs2_selected.gif by default)</summary>
        public static string Thumbs2_Selected_Gif { get { return config.Thumbs2_Selected_Gif; } }

        /// <summary> URL for the default resource 'thumbs3.gif' file ( http://cdn.sobekrepository.org/images/misc/thumbs3.gif by default)</summary>
        public static string Thumbs3_Gif { get { return config.Thumbs3_Gif; } }

        /// <summary> URL for the default resource 'thumbs3_selected.gif' file ( http://cdn.sobekrepository.org/images/misc/thumbs3_selected.gif by default)</summary>
        public static string Thumbs3_Selected_Gif { get { return config.Thumbs3_Selected_Gif; } }

        /// <summary> URL for the default resource 'toolbar-toggle.png' file ( http://cdn.sobekrepository.org/images/mapedit/toolbar-toggle.png by default)</summary>
        public static string Toolbar_Toggle_Png { get { return config.Toolbar_Toggle_Png; } }

        /// <summary> URL for the default resource 'toolbox-close2.png' file ( http://cdn.sobekrepository.org/images/mapedit/toolbox-close2.png by default)</summary>
        public static string Toolbox_Close2_Png { get { return config.Toolbox_Close2_Png; } }

        /// <summary> URL for the default resource 'toolbox-icon.png' file ( http://cdn.sobekrepository.org/images/mapedit/toolbox-icon.png by default)</summary>
        public static string Toolbox_Icon_Png { get { return config.Toolbox_Icon_Png; } }

        /// <summary> URL for the default resource 'toolbox-maximize2.png' file ( http://cdn.sobekrepository.org/images/mapedit/toolbox-maximize2.png by default)</summary>
        public static string Toolbox_Maximize2_Png { get { return config.Toolbox_Maximize2_Png; } }

        /// <summary> URL for the default resource 'toolbox-minimize2.png' file ( http://cdn.sobekrepository.org/images/mapedit/toolbox-minimize2.png by default)</summary>
        public static string Toolbox_Minimize2_Png { get { return config.Toolbox_Minimize2_Png; } }

        /// <summary> URL for the default resource 'top_left.jpg' file ( http://cdn.sobekrepository.org/images/bookturner/top_left.jpg by default)</summary>
        public static string Top_Left_Jpg { get { return config.Top_Left_Jpg; } }

        /// <summary> URL for the default resource 'top_right.jpg' file ( http://cdn.sobekrepository.org/images/bookturner/top_right.jpg by default)</summary>
        public static string Top_Right_Jpg { get { return config.Top_Right_Jpg; } }

        /// <summary> URL for the default resource 'track2.gif' file ( http://cdn.sobekrepository.org/images/misc/track2.gif by default)</summary>
        public static string Track2_Gif { get { return config.Track2_Gif; } }

        /// <summary> URL for the default resource 'trash01.ico' file ( http://cdn.sobekrepository.org/images/qc/TRASH01.ICO by default)</summary>
        public static string Trash01_Ico { get { return config.Trash01_Ico; } }

        /// <summary> URL for the default resource 'twitter_share.gif' file ( http://cdn.sobekrepository.org/images/misc/twitter_share.gif by default)</summary>
        public static string Twitter_Share_Gif { get { return config.Twitter_Share_Gif; } }

        /// <summary> URL for the default resource 'twitter_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/twitter_share_h.gif by default)</summary>
        public static string Twitter_Share_H_Gif { get { return config.Twitter_Share_H_Gif; } }

        /// <summary> URL for the default resource 'ufdc_banner_700.jpg' file ( http://cdn.sobekrepository.org/images/misc/ufdc_banner_700.jpg by default)</summary>
        public static string Ufdc_Banner_700_Jpg { get { return config.Ufdc_Banner_700_Jpg; } }

        /// <summary> URL for the default resource 'ui-icons_ffffff_256x240.png' file ( http://cdn.sobekrepository.org/images/mapsearch/ui-icons_ffffff_256x240.png by default)</summary>
        public static string Ui_Icons_Ffffff_256X240_Png { get { return config.Ui_Icons_Ffffff_256X240_Png; } }

        /// <summary> URL for the default resource 'uploadifive.css' file ( http://cdn.sobekrepository.org/includes/uploadifive/1.1.2/uploadifive.css by default)</summary>
        public static string Uploadifive_Css { get { return config.Uploadifive_Css; } }

        /// <summary> URL for the default resource 'uploadify.css' file ( http://cdn.sobekrepository.org/includes/uploadify/3.2.1/uploadify.css by default)</summary>
        public static string Uploadify_Css { get { return config.Uploadify_Css; } }

        /// <summary> URL for the default resource 'uploadify.swf' file ( http://cdn.sobekrepository.org/includes/uploadify/3.2.1/uploadify.swf by default)</summary>
        public static string Uploadify_Swf { get { return config.Uploadify_Swf; } }

        /// <summary> URL for the default resource 'usage.png' file ( http://cdn.sobekrepository.org/images/misc/usage.png by default)</summary>
        public static string Usage_Img { get { return config.Usage_Img; } }

        /// <summary> URL for the default resource 'usage_lg.png' file ( http://cdn.sobekrepository.org/images/misc/usage_lg.png by default)</summary>
        public static string Usage_Img_Large { get { return config.Usage_Img_Large; } }

        /// <summary> URL for the default resource 'users.gif' file ( http://cdn.sobekrepository.org/images/misc/Users.gif by default)</summary>
        public static string Users_Img { get { return config.Users_Img; } }

        /// <summary> URL for the default resource 'users.png' file ( http://cdn.sobekrepository.org/images/misc/Users.png by default)</summary>
        public static string Users_Img_Small { get { return config.Users_Img_Small; } }

        /// <summary> URL for the default resource 'users_lg.png' file ( http://cdn.sobekrepository.org/images/misc/Users_lg.png by default)</summary>
        public static string Users_Img_Large { get { return config.Users_Img_Large; } }

        /// <summary> URL for the default resource 'icon_permission.png' file ( http://cdn.sobekrepository.org/images/misc/icon_permission.png by default)</summary>
        public static string User_Permission_Img { get { return config.User_Permission_Img; } }

        /// <summary> URL for the default resource 'user_permissions_lg.png' file ( http://cdn.sobekrepository.org/images/misc/user_permissions_lg.png by default)</summary>
        public static string User_Permission_Img_Large { get { return config.User_Permission_Img_Large; } }

        /// <summary> URL for the default resource 'view.ico' file ( http://cdn.sobekrepository.org/images/qc/View.ico by default)</summary>
        public static string View_Ico { get { return config.View_Ico; } }

        /// <summary> URL for the default resource 'view_work_log.png' file ( http://cdn.sobekrepository.org/images/misc/view_work_log.png by default)</summary>
        public static string View_Work_Log_Img { get { return config.View_Work_Log_Img; } }

        /// <summary> URL for the default resource 'view_work_log_icon.png' file ( http://cdn.sobekrepository.org/images/misc/view_work_log_icon.png by default)</summary>
        public static string View_Work_Log_Img_Large { get { return config.View_Work_Log_Img_Large; } }

        /// <summary> URL for the default resource 'warning.png' file ( http://cdn.sobekrepository.org/images/misc/warnging.png by default)</summary>
        public static string Warning_Img { get { return config.Warning_Img; } }

        /// <summary> URL for the default resource 'web_content_medium.png' file ( http://cdn.sobekrepository.org/images/misc/web_content_medium.png by default)</summary>
        public static string WebContent_Img { get { return config.WebContent_Img; } }

        /// <summary> URL for the default resource 'web_content_small.png' file ( http://cdn.sobekrepository.org/images/misc/web_content_small.png by default)</summary>
        public static string WebContent_Img_Small { get { return config.WebContent_Img_Small; } }

        /// <summary> URL for the default resource 'web_content_large.png' file ( http://cdn.sobekrepository.org/images/misc/web_content_large.png by default)</summary>
        public static string WebContent_Img_Large { get { return config.WebContent_Img_Large; } }

        /// <summary> URL for the default resource 'webcontent_history.png' file ( http://cdn.sobekrepository.org/images/misc/webcontent_history.png by default)</summary>
        public static string WebContent_History_Img { get { return config.WebContent_History_Img; } }

        /// <summary> URL for the default resource 'webcontent_history_small.png' file ( http://cdn.sobekrepository.org/images/misc/webcontent_history_small.png by default)</summary>
        public static string WebContent_History_Img_Small { get { return config.WebContent_History_Img_Small; } }

        /// <summary> URL for the default resource 'webcontent_history_large.png' file ( http://cdn.sobekrepository.org/images/misc/webcontent_history_large.png by default)</summary>
        public static string WebContent_History_Img_Large { get { return config.WebContent_History_Img_Large; } }

        /// <summary> URL for the default resource 'webcontent_usage.png' file ( http://cdn.sobekrepository.org/images/misc/webcontent_usage.png by default)</summary>
        public static string WebContent_Usage_Img { get { return config.WebContent_Usage_Img; } }

        /// <summary> URL for the default resource 'webcontent_usage_small.png' file ( http://cdn.sobekrepository.org/images/misc/webcontent_usage_small.png by default)</summary>
        public static string WebContent_Usage_Img_Small { get { return config.WebContent_Usage_Img_Small; } }

        /// <summary> URL for the default resource 'webcontent_usage_large.png' file ( http://cdn.sobekrepository.org/images/misc/webcontent_usage_large.png by default)</summary>
        public static string WebContent_Usage_Img_Large { get { return config.WebContent_Usage_Img_Large; } }

        /// <summary> URL for the default resource 'wizard.png' file ( http://cdn.sobekrepository.org/images/misc/wizard.png by default)</summary>
        public static string Wizard_Img { get { return config.Wizard_Img; } }

        /// <summary> URL for the default resource 'wizard_lg.png' file ( http://cdn.sobekrepository.org/images/misc/wizard_lg.png by default)</summary>
        public static string Wizard_Img_Large { get { return config.Wizard_Img_Large; } }

        /// <summary> URL for the default resource 'wordmarks.png' file ( http://cdn.sobekrepository.org/images/misc/wordmarks.png by default)</summary>
        public static string Wordmarks_Img { get { return config.Wordmarks_Img; } }

        /// <summary> URL for the default resource 'wordmarks_small.png' file ( http://cdn.sobekrepository.org/images/misc/wordmarks_small.png by default)</summary>
        public static string Wordmarks_Img_Small { get { return config.Wordmarks_Img_Small; } }

        /// <summary> URL for the default resource 'wordmarks_large.png' file ( http://cdn.sobekrepository.org/images/misc/wordmarks_large.png by default)</summary>
        public static string Wordmarks_Img_Large { get { return config.Wordmarks_Img_Large; } }

        /// <summary> URL for the default resource 'yahoo_share.gif' file ( http://cdn.sobekrepository.org/images/misc/yahoo_share.gif by default)</summary>
        public static string Yahoo_Share_Gif { get { return config.Yahoo_Share_Gif; } }

        /// <summary> URL for the default resource 'yahoo_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/yahoo_share_h.gif by default)</summary>
        public static string Yahoo_Share_H_Gif { get { return config.Yahoo_Share_H_Gif; } }

        /// <summary> URL for the default resource 'yahoobuzz_share.gif' file ( http://cdn.sobekrepository.org/images/misc/yahoobuzz_share.gif by default)</summary>
        public static string Yahoobuzz_Share_Gif { get { return config.Yahoobuzz_Share_Gif; } }

        /// <summary> URL for the default resource 'yahoobuzz_share_h.gif' file ( http://cdn.sobekrepository.org/images/misc/yahoobuzz_share_h.gif by default)</summary>
        public static string Yahoobuzz_Share_H_Gif { get { return config.Yahoobuzz_Share_H_Gif; } }

        /// <summary> URL for the default resource 'zoom_tool.cur' file ( http://cdn.sobekrepository.org/images/misc/zoom_tool.cur by default)</summary>
        public static string Zoom_Tool_Cur { get { return config.Zoom_Tool_Cur; } }

        /// <summary> URL for the default resource 'zoomin.png' file ( http://cdn.sobekrepository.org/images/bookturner/zoomin.png by default)</summary>
        public static string Zoomin_Png { get { return config.Zoomin_Png; } }

        /// <summary> URL for the default resource 'zoomout.png' file ( http://cdn.sobekrepository.org/images/bookturner/zoomout.png by default)</summary>
        public static string Zoomout_Png { get { return config.Zoomout_Png; } }

    }
}
