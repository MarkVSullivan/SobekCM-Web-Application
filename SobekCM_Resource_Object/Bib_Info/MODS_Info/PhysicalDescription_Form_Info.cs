#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Stores information about the Designation of a particular presentation of a resource, including the physical form or medium of material for a resource </summary>
    public class PhysicalDescription_Form_Info
    {
        private string authority;
        private string form;
        private string type;

        /// <summary> Constructor for a new instance of the PhysicalDescription_Form_Info class </summary>
        public PhysicalDescription_Form_Info()
        {
            // Do nothing here
        }

        /// <summary> Constructor for a new instance of the PhysicalDescription_Form_Info class </summary>
        /// <param name="Form"> Designation of a particular presentation of a resource, including the physical form or medium of material for a resource</param>
        /// <param name="Authority"> Name of any authority used, if this is selected from an authority list </param>
        /// <param name="Type"> May be used to specify whether the form concerns materials or techniques, for example </param>
        public PhysicalDescription_Form_Info(string Form, string Authority, string Type)
        {
            authority = Authority;
            form = Form;
            type = Type;
        }

        /// <summary>  Designation of a particular presentation of a resource, including the physical form or medium of material for a resource </summary>
        public string Form
        {
            get { return form ?? String.Empty; }
            set { form = value; }
        }

        /// <summary> Name of any authority used, if this is selected from an authority list </summary>
        public string Authority
        {
            get { return authority ?? String.Empty; }
            set { authority = value; }
        }

        /// <summary> May be used to specify whether the form concerns materials or techniques, for example </summary>
        public string Type
        {
            get { return type ?? String.Empty; }
            set { type = value; }
        }

        internal bool hasData
        {
            get
            {
                if (!String.IsNullOrEmpty(form))
                    return true;
                else
                    return false;
            }
        }

        /// <summary> Clears all the data held in this form object </summary>
        public void Clear()
        {
            authority = null;
            form = null;
            type = null;
        }
    }
}