using System;
using System.Data;
using SobekCM.Bib_Package.Bib_Info;

namespace SobekCM.Bib_Package.Divisions
{
	/// <summary> Abstract class representing a single tree node in a structural map tree associated with a digital resource </summary>
	/// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    [Serializable]
	public abstract class abstract_TreeNode : XML_Writing_Base_Type
	{
		/// <summary> Main label for this node, if there is one </summary>
		protected string label;

		/// <summary> Stores the id of the type of this division </summary>
		protected string type;

		/// <summary> Constructor for a new instance of the abstrac_TreeNode class </summary>
		/// <param name="Type">Node type</param>
		/// <param name="Label">Node label</param>
		public abstract_TreeNode( string Type, string Label )
		{
			// Save the parameters
			this.type = Type;
			this.label = Label;
		}

		/// <summary> Constructor for an empty instance of the abstrac_TreeNode class </summary>
		public abstract_TreeNode( )
		{
			// Save the parameters
			this.type = String.Empty;;
			this.label = String.Empty;
		}

		/// <summary> Label for this node, if there is one </summary>
		public virtual string Label
		{
			get	{	return label;	}
			set	{	label = value;	}
		}

		/// <summary> Gets and sets the type of this node </summary>
		public string Type
		{
			get	{	return type;	}
			set	{	type = value;	}
		}

        /// <summary> Gets the flag indicating if this is a page node </summary>
		public abstract bool Page { get; }
	}
}