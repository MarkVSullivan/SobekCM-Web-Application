namespace SobekCM.Library.Aggregations
{
	/// <summary> Information about a front-banner for an item aggregation </summary>
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
		/// <param name="Image_File"> Name of the image file used in this front banner</param>
		public Item_Aggregation_Front_Banner( string Image_File )
		{
			this.Image_File = Image_File;
			Banner_Type = Item_Aggregation_Front_Banner_Type.LEFT;
			Width = 550;
			Height = 230;
		}

		/// <summary> Name of the image file used in this front banner </summary>
		public string Image_File { get; set;  }

		/// <summary> Width of the special front banner image used for aggregations that show the highlighted
		/// item and the search box in the main banner at the top on the front page  </summary>
		public ushort Width { get; set; }

		/// <summary> Height of the special front banner image used for aggregations that show the highlighted
		/// item and the search box in the main banner at the top on the front page </summary>
		public ushort Height { get; set; }

		/// <summary>  Flag indicates type of front banner -- either FULL, LEFT, or RIGHT </summary>
		public Item_Aggregation_Front_Banner_Type Banner_Type { get; set; }
	}
}
