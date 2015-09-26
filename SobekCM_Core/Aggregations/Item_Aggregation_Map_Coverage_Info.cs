using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;
using ProtoBuf;

namespace SobekCM.Core.Aggregations
{
    /// <summary> Type of map coverage information included here </summary>
    public enum Item_Aggregation_Map_Coverage_Type_Enum : byte
    {
        /// <summary> Indicates the coordinate and zoom level information here is fixed by the
        /// administrator, and is not computed by materials within the aggregation or extent of results </summary>
        FIXED = 1,

        /// <summary> Indicates that the coordinate and zoom level information is computed
        /// from the collection of all materials within an aggregation </summary>
        COMPUTED = 2,

        /// <summary> Indicates that the fixed window should not be used and 
        /// the map should be zoomed to the extent of all included points </summary>
        /// <remarks> This is really only valid as such for the map browse option </remarks>
        EXTENT
    }

    /// <summary> Map coordinate and zoom (Google Maps) information for map search or map browse </summary>
    [Serializable, DataContract, ProtoContract]
    public class Item_Aggregation_Map_Coverage_Info
    {
        /// <summary> Constructor for a new instance of the Item_Aggregation_Map_Coverage_Info class </summary>
        public Item_Aggregation_Map_Coverage_Info()
        {
            // Do nothing - for serialization purposes   
        }

        /// <summary> Constructor for a new instance of the Item_Aggregation_Map_Coverage_Info class </summary>
        /// <param name="Type"> Type of map coverage, related to the coordinates and zoom ( i.e., fixed, computed, zoom to extent ) </param>
        /// <param name="ZoomLevel"> Google Maps zoom level for this display </param>
        /// <param name="Longitude"> Longitude for the center of the map to display </param>
        /// <param name="Latitude"> Latitude for the center of the map to display </param>
        public Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum Type, int ZoomLevel, decimal Longitude, decimal Latitude)
        {
            this.Type = Type;
            this.ZoomLevel = ZoomLevel;
            this.Longitude = Longitude;
            this.Latitude = Latitude;
        }

        /// <summary> Constructor for a new instance of the Item_Aggregation_Map_Coverage_Info class </summary>
        /// <param name="Type"> Type of map coverage, related to the coordinates and zoom ( i.e., fixed, computed, zoom to extent ) </param>
        public Item_Aggregation_Map_Coverage_Info(Item_Aggregation_Map_Coverage_Type_Enum Type)
        {
            this.Type = Type;
        }

        /// <summary> Type of map coverage, related to the coordinates and zoom ( i.e., fixed, computed, zoom to extent ) </summary>
        [DataMember(Name = "type")]
        [XmlAttribute("type")]
        [ProtoMember(1)]
        public Item_Aggregation_Map_Coverage_Type_Enum Type { get; set; }

        /// <summary> Google Maps zoom level for this display </summary>
        [DataMember(Name = "zoom")]
        [XmlAttribute("zoom")]
        [ProtoMember(2)]
        public int? ZoomLevel { get; set; }

        /// <summary> Longitude for the center of the map to display </summary>
        [DataMember(Name = "longitude")]
        [XmlAttribute("longitude")]
        [ProtoMember(3)]
        public decimal? Longitude { get; set; }

        /// <summary> Latitude for the center of the map to display </summary>
        [DataMember(Name = "latitude")]
        [XmlAttribute("latitude")]
        [ProtoMember(4)]
        public decimal? Latitude { get; set; }

        /// <summary> Method suppresses XML Serialization of the ZoomLevel flag property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeZoomLevel()
        {
            return ZoomLevel.HasValue;
        }

        /// <summary> Method suppresses XML Serialization of the Longitude flag property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLongitude()
        {
            return Longitude.HasValue;
        }

        /// <summary> Method suppresses XML Serialization of the Latitude flag property if it is NULL </summary>
        /// <returns> TRUE if the property should be serialized, otherwise FALSE </returns>
        public bool ShouldSerializeLatitude()
        {
            return Latitude.HasValue;
        }

        /// <summary> Return a copy of this map coverage information </summary>
        /// <returns> Copy of this coverage info </returns>
        public Item_Aggregation_Map_Coverage_Info Copy()
        {
            return new Item_Aggregation_Map_Coverage_Info
            {
                Type = Type, 
                Latitude = Latitude, 
                Longitude = Longitude, 
                ZoomLevel = ZoomLevel
            };
        }
    }
}
