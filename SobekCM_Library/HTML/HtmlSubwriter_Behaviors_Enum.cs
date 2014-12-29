namespace SobekCM.Library.HTML
{
    /// <summary> Enumeration is used to allow individual HtmlSubwriters 
    /// (and viewers under those subwriters) to bubble up specific behaviors
    /// to the main HTMl writer and specific subwriter </summary>
    public enum HtmlSubwriter_Behaviors_Enum : byte
    {
		/// <summary> Do not show the internal header </summary>
        Suppress_Internal_Header,

		/// <summary> Do not show ANY header </summary>
        Suppress_Header,

		/// <summary> Do not show ANY footer </summary>
        Suppress_Footer,

		/// <summary> Do not show the banner </summary>
        Suppress_Banner,

		/// <summary> Do not put the container tags around the resultant page </summary>
        Item_Subwriter_NonWindowed_Mode,

		/// <summary> Suppress any possible bottom pagination in an item view </summary>
        Item_Subwriter_Suppress_Bottom_Pagination,
        
		/// <summary> Suppress the title bar for this item view </summary>
        Item_Subwriter_Suppress_Titlebar,

		/// <summary> Suppress displaying the item menu for this item view (item viewer likely adds one) </summary>
        Item_Subwriter_Suppress_Item_Menu,

		/// <summary> Suppress any possible left navigation/wordmarks bar in this item view </summary>
        Item_Subwriter_Suppress_Left_Navigation_Bar,

		/// <summary> This item view REQUIRES a left navigation bar, even if there is no other indicator
		/// that there should be one </summary>
        Item_Subwriter_Requires_Left_Navigation_Bar,

		/// <summary> This item requires the FULL Jquery UI library, not just the very small one
		/// generally utilized </summary>
		/// <value> This is more about SUPPRESSING the default minimum one in the header, since the
		/// item viewer could write the jquery reference in the HTML head itself </value>
		Item_Subwriter_Full_JQuery_UI,

		/// <summary> This mySobek viewer should mimic the item subwriter, since it mostly
		/// allows a logged on user to edit something about a single item </summary>
        MySobek_Subwriter_Mimic_Item_Subwriter,

        /// <summary> Used by aggregation viewers to tell the aggregation writer to NOT write out the home page text </summary>
        Aggregation_Suppress_Home_Text
    }
}
