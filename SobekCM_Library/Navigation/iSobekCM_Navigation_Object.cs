namespace SobekCM.Library.Navigation
{
	/// <summary> iSobekCM_Navigation_Object is the interface which any 
	/// Navigation Object must implement</summary>
	public interface iSobekCM_Navigation_Object
	{
		/// <summary> Returns the URL to redirect the user's browser, based on the current
		/// mode and specifics for this mode. </summary>
		/// <returns> String to be attached to the end of the main application name to redirect
		/// the current user's browser.  </returns>
		string Redirect_URL();

		/// <summary> Returns the URL to redirect the user's browser, based on the current
		/// mode and specifics for this mode. </summary>
		/// <param name="Item_View_Code">Item view code to display</param>
		/// <param name="Include_URL_Opts"> Flag indicates whether to include URL opts or not </param>
		/// <returns> String to be attached to the end of the main application name to redirect
		/// the current user's browser.  </returns>
		string Redirect_URL(string Item_View_Code, bool Include_URL_Opts );

		/// <summary> Returns the URL options the user has currently set </summary>
		/// <returns>URL options</returns>
		string URL_Options();
	}

}
