#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using SobekCM.Library.Citation.Elements;

#endregion

namespace SobekCM.Library.Citation.Template
{
    /// <summary> Stores the data about a single template panel (within a template page) in a <see cref="Template"/> object </summary>
	public class Template_Panel
	{
        private readonly List<abstract_Element> elements;

        /// <summary> Constructor for a new instance of the Template_Panel class </summary>
		public Template_Panel()
		{
			// Set some defaults
            elements = new List<abstract_Element>();
            Title = String.Empty;
		}

        /// <summary> Constructor for a new instance of the Template_Panel class </summary>
		/// <param name="Title"> Default title for this panel </param>
		public Template_Panel( string Title )
		{
			// Set some defaults
            this.Title = Title;
            elements = new List<abstract_Element>();
		}

        /// <summary> Read-only collection of element objects contained within this template panel </summary>
        public ReadOnlyCollection<abstract_Element> Elements
        {
            get
            {
                return new ReadOnlyCollection<abstract_Element>(elements);
            }
        }

        /// <summary> Title for this template panel </summary>
        public string Title { get; set; }

        /// <summary> Adds a new metadata element to the collection of elements within this template panel  </summary>
        /// <param name="newElement"> New metadata element to add to this panel </param>
        internal void Add_Element(abstract_Element newElement)
        {
            elements.Add(newElement);
        }
	}
}
