#region Using directives

using System;
using System.Runtime.Serialization;
using ProtoBuf;

#endregion

namespace SobekCM.Core.Aggregations
{
	/// <summary> Information about a front-banner for an item aggregation </summary>   
    [Serializable, DataContract, ProtoContract]
	public class Item_Aggregation_Front_Banner
	{
		/// <summary> Type of front banner -- either FULL, LEFT, or RIGHT </summary>
		public enum Item_Aggregation_Front_Banner_Type : byte
		{	
			/// <summary> This is a full-width banner, and does not include
			/// the rotating highlights feature </summary>
			FULL,

			/// <summary> The banner sits to the left and the higlights sit 
			/// to the right </summary>
			LEFT,

			/// <summary> The banner sits to the right and the highlights sit
			/// to the left </summary>
			RIGHT
		}
		/// <summary> Constructor for a new instance of the Item_Aggregation_Front_Banner class </summary>
		/// <param name="File"> Name of the image file used in this front banner</param>
		public Item_Aggregation_Front_Banner( string File )
		{
			this.File = File;
			Type = Item_Aggregation_Front_Banner_Type.LEFT;
			Width = 550;
			Height = 230;
		}

		/// <summary> Name of the image file used in this front banner </summary>
        [DataMember(Name = "file"), ProtoMember(1)]
		public string File { get; set;  }

		/// <summary> Width of the special front banner image used for aggregationPermissions that show the highlighted
		/// item and the search box in the main banner at the top on the front page  </summary>
        [DataMember(Name = "width"), ProtoMember(2)]
		public ushort Width { get; set; }

		/// <summary> Height of the special front banner image used for aggregationPermissions that show the highlighted
		/// item and the search box in the main banner at the top on the front page </summary>
        [DataMember(Name = "height"), ProtoMember(3)]
		public ushort Height { get; set; }

		/// <summary>  Flag indicates type of front banner -- either FULL, LEFT, or RIGHT </summary>
        [DataMember(Name = "type"), ProtoMember(4)]
		public Item_Aggregation_Front_Banner_Type Type { get; set; }
	}
}
