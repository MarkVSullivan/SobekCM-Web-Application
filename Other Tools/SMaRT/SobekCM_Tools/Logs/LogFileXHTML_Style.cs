#region Using directives

using System.Text;

#endregion

namespace SobekCM.Tools.Logs
{
	/// <summary> [ NOT YET FULLY IMPLEMENTED ] <br /> <br />
	/// LogFileXHTML_Style is a class used to hold information about a specific style used in the <see cref="LogFileXHTML"/> class. 
	/// <br /> <br /> </summary>
	/// <remarks> This class is created by calls to the <see cref="LogFileXHTML.AddNewStyle(string, string, bool)"/> method in the <see cref="LogFileXHTML"/> class. 
	/// <br /> <br />
	/// Object created by Mark V Sullivan (2003) for University of Florida's Digital Library Center.  </remarks> 
	public class LogFileXHTML_Style
	{

/*=========================================================================
 *		PRIVATE VARIABLE DECLARATIONS of the LogFileXHTML_Style Class 
 *=========================================================================*/

		/// <summary> Private bool variable stores flag which indicates if this style should have the actual log portion of the line emboldened. </summary>
		private readonly bool bold;

		/// <summary> Private string variable stores the information for the color of the font used for this style. </summary>
		private readonly string fontColor;

		/// <summary> Private string variable stores the information for the font family for this style. </summary>
		private readonly string fontFamily;

		/// <summary> Private string variable stores the information for the size of the font to use for this style. </summary>
		private readonly string fontSize;

		/// <summary> Private bool variable stores flag which indicates if the actual log information for these style should be italicized. </summary>
		private readonly bool italics;

		/// <summary> Private string variable stores the name of this style to be referenced in the body of the XHTML document. </summary>
		private readonly string name;

/*=========================================================================
 *		CONSTRUCTOR(S) of the LogFileXHTML_Style Class 
 *=========================================================================*/

		/// <summary> Constructor for the LogFileXHTML_Style class. </summary>
		/// <param name="bold"> Flag which indicates if this style should have the actual log portion of the line emboldened. </param>
		/// <param name="font_Color"> The information for the color of the font used for this style. </param>
		/// <param name="font_Size"> The information for the size of the font to use for this style. </param>
		/// <param name="italics"> Flag which indicates if the actual log information for these style should be italicized. </param>
		/// <param name="name"> The name of this style to be referenced in the body of the XHTML document. </param>
		public LogFileXHTML_Style( bool bold, string font_Color, string font_Size, bool italics, string name )
		{
			// Save all the properties to the private variables
			this.bold = bold;
			fontColor = font_Color;
			fontSize = font_Size;
			this.italics = italics;
			this.name = name;

			// Set the font family to the default of Aerial
			fontFamily = "Aerial";
		}

		/// <summary> Constructor for the LogFileXHTML_Style class which takes the style definition from
		/// HTML and parses each element for this object. </summary>
		/// <remarks> The input string must be of the exact form which the ToString() method creates. </remarks>
		/// <param name="style_definition"> Style sheet in HTML format, as output by ToString() </param>
		public LogFileXHTML_Style( string style_definition )
		{
			

		}


/*=========================================================================
 *		PUBLIC PROPERTIES of the LogFileXHTML_Style Class 
 *=========================================================================*/

		/// <summary> Gets flag which indicates if this style should have the actual log portion of the line emboldened. </summary>
		public bool Bold
		{
			get	{	return bold;	}
		}

		/// <summary> Gets the information for the color of the font used for this style. </summary>
		public string Font_Color
		{
			get	{	return fontColor;	}
		}

		/// <summary> Gets the information for the font family for this style. </summary>
		public string Font_Family
		{
			get	{	return fontFamily;	}
		}

		/// <summary> Gets the information for the size of the font to use for this style. </summary>
		public string Font_Size
		{
			get	{	return fontSize;	}
		}

		/// <summary> Gets flag which indicates if the actual log information for these style should be italicized. </summary>
		public bool Italics
		{
			get	{	return italics;	}
		}

		/// <summary> Gets the name of this style to be referenced in the body of the XHTML document. </summary>
		public string Name
		{
			get	{	return name;	}
		}

/*=========================================================================
 *		PUBLIC METHODS of the LogFileXHTML_Style Class 
 *=========================================================================*/

		/// <summary> Returns the Style Sheet definition line which defines this style. </summary>
		/// <returns> Style sheet definition XHTML line. </returns>
		public override string ToString()
		{
			// Add the style sheet components
			StringBuilder returnVal = new StringBuilder("     ." + name.ToLower() + " { color: \"" + fontColor + "\"; ");
			returnVal.Append("font-size: \"" + fontSize + "\"; font-family: \"" + fontFamily + "\"; ");

			// Add remarks for bold and italics
			if ( bold )
				returnVal.Append("<!strong> ");
			if ( italics )
				returnVal.Append("<!italics> ");

			// Add closing line
			returnVal.Append("}");

			// Return this line, converted to a regular string
			return returnVal.ToString();
		}

/*=========================================================================
 *		PRIVATE METHODS of the LogFileXHTML_Style Class 
 *=========================================================================*/

		// TO DO: Place any necessary private methods here

	}
}