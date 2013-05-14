#region Using directives

using System;
using System.Collections.Generic;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Metadata_Modules;

#endregion

namespace SobekCM.Resource_Object.Divisions
{
    /// <summary> Abstract class representing a single tree node in a structural map tree associated with a digital resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center. </remarks>
    [Serializable]
    public abstract class abstract_TreeNode : MetadataDescribableBase
    {
        /// <summary> Main label for this node, if there is one </summary>
        protected string label;

        /// <summary> Stores the type of this division (i.e., Chapter, Page, etc..) </summary>
        protected string type;

        /// <summary> Stores the ID for this division within a METS file </summary>
        /// <remarks> This is not READ or used except during the METS writing process </remarks>
        protected string id;

        /// <summary> Constructor for a new instance of the abstrac_TreeNode class </summary>
        /// <param name="Type">Node type</param>
        /// <param name="Label">Node label</param>
        protected abstract_TreeNode(string Type, string Label)
        {
            // Save the parameters
            type = Type;
            label = Label;
        }

        /// <summary> Constructor for an empty instance of the abstrac_TreeNode class </summary>
        protected abstract_TreeNode()
        {
            // Save the parameters
            type = String.Empty;
            label = String.Empty;
        }

        /// <summary> Label for this node, if there is one </summary>
        public virtual string Label
        {
            get { return label; }
            set { label = value; }
        }

        /// <summary> Gets and sets the type of this node </summary>
        public string Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary> Gets and sets the ID of this node within the context of a larger METS file </summary>
        /// <remarks> This is not READ or used except during the METS writing process </remarks>
        internal string ID
        {
            get { return id ?? String.Empty; }
            set { id = value; }
        }

        /// <summary> Gets the flag indicating if this is a page node </summary>
        public abstract bool Page { get; }
    }
}