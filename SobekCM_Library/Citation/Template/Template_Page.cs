#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.Citation.Template
{
	/// <summary> Stores the data about a single page in a <see cref="CompleteTemplate"/> object </summary>
	public class Template_Page
	{
	    private readonly List<Template_Panel> panels;

        /// <summary> Constructor for a new instance of the Template_Page class </summary>
		public Template_Page()
		{
			// Set some defaults
			Title = String.Empty;
            Instructions = String.Empty;
            panels = new List<Template_Panel>();
		}

        /// <summary> Constructor for a new instance of the Template_Page class </summary>
		/// <param name="Title"> Default title for this page </param>
		public Template_Page( string Title )
		{
			// Set some defaults
			this.Title = Title;
            Instructions = String.Empty;
            panels = new List<Template_Panel>();
		}

	    /// <summary> Read-only collection of <see cref="Template_Panel"/> objects held within this template page </summary>
		public ReadOnlyCollection<Template_Panel> Panels
		{
			get
			{
                return new ReadOnlyCollection<Template_Panel>(panels);
			}
		}

	    /// <summary> Title for this template page  </summary>
	    public string Title { get; set; }

	    /// <summary> Instructions for this template page  </summary>
	    public string Instructions { get; set; }

	    /// <summary> Adds a new template panel to the collection of panels contained within this template page </summary>
	    /// <param name="newPanel"> New template panel to add </param>
	    internal void Add_Panel( Template_Panel newPanel )
	    {
	        panels.Add(newPanel);
	    }
	}
}
