﻿#region Using directives

using System;
using System.IO;
using System.Text;
using System.Web;
using SobekCM.Core.ApplicationState;
using SobekCM.Core.Configuration;
using SobekCM.Core.Configuration.Localization;
using SobekCM.Core.Users;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Library.Citation.Elements
{
	/// <summary> Element allows entry of the ETD graduation semester ( i.e., 'Fall 2012') for an item </summary>
	/// <remarks> This class extends the <see cref="simpleTextBox_Element"/> class. </remarks>
	public class ETD_GraduationSemester_Element : simpleTextBox_Element
	{
		/// <summary> Constructor for a new instance of the ETD_GraduationSemester_Element class </summary>
		public ETD_GraduationSemester_Element()
			: base("Graduation Semester", "etd_graduationsemester")
		{
			Repeatable = false;
		}

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
		/// <remarks> This simple element does not append any popup form to the popup_form_builder</remarks>
		public override void Render_Template_HTML(TextWriter Output, SobekCM_Item Bib, string Skin_Code, bool IsMozilla, StringBuilder PopupFormBuilder, User_Object Current_User, Web_Language_Enum CurrentLanguage, Language_Support_Info Translator, string Base_URL)
		{
			// Check that an acronym exists
			if (Acronym.Length == 0)
			{
				Acronym = "Enter the semester of the graduation (i.e., 'Fall 2012') related to this thesis/dissertation";
			}

			// Is there an ETD object?
			string valueToDisplay = string.Empty;
			Thesis_Dissertation_Info etdInfo = Bib.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;
			if (etdInfo != null)
				valueToDisplay = etdInfo.Graduation_Semester;

			render_helper(Output, valueToDisplay, Skin_Code, Current_User, CurrentLanguage, Translator, Base_URL);
		}

		/// <summary> Prepares the bib object for the save, by clearing any existing data in this element's related field(s) </summary>
		/// <param name="Bib"> Existing digital resource object which may already have values for this element's data field(s) </param>
		/// <param name="Current_User"> Current user, who's rights may impact the way an element is rendered </param>
		/// <remarks> This does nothing since there is only one value </remarks>
		public override void Prepare_For_Save(SobekCM_Item Bib, User_Object Current_User)
		{
			// Do nothing since there is only one value
		}

		/// <summary> Saves the data rendered by this element to the provided bibliographic object during postback </summary>
		/// <param name="Bib"> Object into which to save the user's data, entered into the html rendered by this element </param>
		public override void Save_To_Bib(SobekCM_Item Bib)
		{
			string[] getKeys = HttpContext.Current.Request.Form.AllKeys;
			foreach (string thisKey in getKeys)
			{
				if (thisKey.IndexOf(html_element_name.Replace("_", "")) == 0)
				{
					Thesis_Dissertation_Info etdInfo = Bib.Get_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY) as Thesis_Dissertation_Info;

					string value = HttpContext.Current.Request.Form[thisKey];
					if (value.Length > 0)
					{
						if (etdInfo == null)
						{
							etdInfo = new Thesis_Dissertation_Info();
							Bib.Add_Metadata_Module(GlobalVar.THESIS_METADATA_MODULE_KEY, etdInfo);
						}
						etdInfo.Graduation_Semester = value;
					}
					else
					{
						if (etdInfo != null)
							etdInfo.Graduation_Semester = String.Empty;
					}
					return;
				}
			}
		}
	}
}
