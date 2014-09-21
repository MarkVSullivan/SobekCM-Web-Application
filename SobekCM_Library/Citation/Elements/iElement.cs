#region Using directives

using System.IO;
using System.Text;
using System.Xml;
using SobekCM.Resource_Object;
using SobekCM.Library.Application_State;
using SobekCM.Library.Configuration;
using SobekCM.Core.Users;

#endregion

namespace SobekCM.Library.Citation.Elements
{
	/// <summary> Interface which all metadata elements must implement  </summary>
	interface iElement
	{
		/// <summary> Element type for this class </summary>
		Element_Type Type { get; }

		/// <summary> Display subtype for this element </summary>
		string Display_SubType { get; }

		/// <summary> Flag indicates if this is being used as a constant field, or if data can be entered by the user </summary>
		bool isConstant { get; set; }

		/// <summary> Flag indicating this allows repeatability </summary>
		bool Repeatable { get; set; }

		/// <summary> Flag indicating this is mandatory </summary>
		bool Mandatory { get; set; }

		/// <summary> Returns TRUE if this element has some data </summary>
		bool HasData { get; }

		/// <summary> Title of this element in the current language </summary>
	    string Title { get; }

	    /// <summary> Reads from the template XML format </summary>
        /// <param name="xmlReader"> Current template xml configuration reader </param>
		void Read_XML( XmlTextReader xmlReader );

        /// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
        /// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
		void Prepare_For_Save( SobekCM_Item Bib, User_Object Current_User );

        /// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
        /// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
		void Save_To_Bib( SobekCM_Item Bib );

        /// <summary> Saves the constants to the bib id </summary>
        /// <param name="Bib"> Object into which to save this element's constant data </param>
        void Save_Constant_To_Bib(SobekCM_Item Bib);

		/// <summary> Renders the HTML for this element </summary>
        /// <param name="Output"> Textwriter to write the HTML for this element </param>
		/// <param name="Bib"> Object to populate this element from </param>
        /// <param name="Skin_Code"> Code for the current skin </param>
        /// <param name="IsMozilla"> Flag indicates if the current browse is Mozilla Firefox (different css choices for some elements)</param>
        /// <param name="PopupFormBuilder"> Builder for any related popup forms for this element </param>
        /// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
        /// <param name="CurrentLanguage"> Current user-interface language </param>
        /// <param name="Translator"> Language support object which handles simple translational duties </param>
        /// <param name="Base_URL"> Base URL for the current request </param>
        void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL );
	}
}
