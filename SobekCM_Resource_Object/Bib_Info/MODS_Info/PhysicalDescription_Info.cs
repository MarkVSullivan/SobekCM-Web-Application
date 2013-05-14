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
            get
            {
                if (notes == null)
                    return 0;
                else
                    return notes.Count;
            }
        }

        /// <summary> Gets the collection of physical description notes </summary>
        /// <remarks> You should check the count of physical description notes first using the <see cref="Notes_Count"/> property before using this property.
        /// Even if there are no notes, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Notes
        {
            get
            {
                if (notes == null)
                    return new ReadOnlyCollection<string>(new List<string>());
                else
                    return new ReadOnlyCollection<string>(notes);
            }
        }

        /// <summary> Returns the form information associated with this physical description object </summary>
        public PhysicalDescription_Form_Info Form_Info
        {
            get
            {
                if (form == null)
                    form = new PhysicalDescription_Form_Info();
                return form;
            }
        }

        /// <summary> Returns a flag that indicates if this physical description object contains
        /// form information </summary>
        public bool hasFormInformation
        {
            get
            {
                if ((form == null) || (!form.hasData))
                    return false;
                else
                    return true;
            }
        }

        /// <summary> Returns flag which indicates this physical description has data </summary>
        internal bool hasData
        {
            get
            {
                if ((String.IsNullOrEmpty(extent)) && ((notes == null) || (notes.Count == 0)) && ((form == null) || (!form.hasData)))
                    return false;
                else
                    return true;
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

        internal void Add_MODS(TextWriter results)
        {
            if ((String.IsNullOrEmpty(extent)) && ((notes == null) || (notes.Count == 0)) && ((form == null) || (!form.hasData)))
                return;

            results.Write("<mods:physicalDescription>\r\n");

            if ((form != null) && (form.hasData))
            {
                results.Write("<mods:form");
                if (form.Authority.Length > 0)
                    results.Write(" authority=\"" + form.Authority + "\"");
                if (form.Type.Length > 0)
                    results.Write(" type=\"" + form.Type + "\"");
                results.Write(">" + base.Convert_String_To_XML_Safe(form.Form) + "</mods:form>\r\n");
            }

            if (!String.IsNullOrEmpty(extent))
                results.Write("<mods:extent>" + base.Convert_String_To_XML_Safe(extent) + "</mods:extent>\r\n");

            if (notes != null)
            {
                foreach (string thisNote in notes)
                {
                    results.Write("<mods:note>" + base.Convert_String_To_XML_Safe(thisNote) + "</mods:note>\r\n");
                }
            }

            results.Write("</mods:physicalDescription>\r\n");
        }
    }
}