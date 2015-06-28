#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary>physicalDescription is a wrapper element that contains all subelements relating to 
    /// physical description information of the resource described. Data is input only within each 
    /// subelement. </summary>
    [Serializable]
    public class PhysicalDescription_Info : XML_Writing_Base_Type
    {
        private string extent;
        private PhysicalDescription_Form_Info form;
        private List<string> notes;

        /// <summary> Constructor creates a new instance of the PhysicalDescription_Info class </summary>
        public PhysicalDescription_Info()
        {
            // Do nothing
        }

        /// <summary> Gets or sets a statement of the number and specific material of the units
        /// of the resource that express physical extent </summary>
        public string Extent
        {
            get { return extent ?? String.Empty; }
            set { extent = value; }
        }

        /// <summary> Gets the number of notes in this physical description object </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Notes"/> property.  Even if 
        /// there are no physical description notes, the Notes property creates a readonly collection to pass back out.</remarks>
        public int Notes_Count
        {
            get {
                return notes == null ? 0 : notes.Count;
            }
        }

        /// <summary> Gets the collection of physical description notes </summary>
        /// <remarks> You should check the count of physical description notes first using the <see cref="Notes_Count"/> property before using this property.
        /// Even if there are no notes, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Notes
        {
            get {
                return notes == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(notes);
            }
        }

        /// <summary> Returns the form information associated with this physical description object </summary>
        public PhysicalDescription_Form_Info Form_Info
        {
            get { return form ?? (form = new PhysicalDescription_Form_Info()); }
        }

        /// <summary> Returns a flag that indicates if this physical description object contains
        /// form information </summary>
        public bool hasFormInformation
        {
            get {
                return (form != null) && (form.hasData);
            }
        }

        /// <summary> Returns flag which indicates this physical description has data </summary>
        internal bool hasData
        {
            get {
                return (!String.IsNullOrEmpty(extent)) || ((notes != null) && (notes.Count != 0)) || ((form != null) && (form.hasData));
            }
        }

        /// <summary> Clear all the information about the physical description of this item </summary>
        public void Clear()
        {
            extent = null;
            form = null;
            if (notes != null) notes.Clear();
        }

        /// <summary> Clear all the physical description notes </summary>
        public void Clear_Note()
        {
            if (notes != null)
                notes.Clear();
        }

        /// <summary> Add a physical description notes </summary>
        /// <param name="Note"> New note to add  </param>
        public void Add_Note(string Note)
        {
            if (notes == null)
                notes = new List<string>();

            notes.Add(Note);
        }

        internal void Add_MODS(TextWriter Results)
        {
            if ((String.IsNullOrEmpty(extent)) && ((notes == null) || (notes.Count == 0)) && ((form == null) || (!form.hasData)))
                return;

            Results.Write("<mods:physicalDescription>\r\n");

            if ((form != null) && (form.hasData))
            {
                Results.Write("<mods:form");
                if (form.Authority.Length > 0)
                    Results.Write(" authority=\"" + form.Authority + "\"");
                if (form.Type.Length > 0)
                    Results.Write(" type=\"" + form.Type + "\"");
                Results.Write(">" + Convert_String_To_XML_Safe(form.Form) + "</mods:form>\r\n");
            }

            if (!String.IsNullOrEmpty(extent))
                Results.Write("<mods:extent>" + Convert_String_To_XML_Safe(extent) + "</mods:extent>\r\n");

            if (notes != null)
            {
                foreach (string thisNote in notes)
                {
                    Results.Write("<mods:note>" + Convert_String_To_XML_Safe(thisNote) + "</mods:note>\r\n");
                }
            }

            Results.Write("</mods:physicalDescription>\r\n");
        }
    }
}