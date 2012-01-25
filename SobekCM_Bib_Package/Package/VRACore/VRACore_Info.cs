using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;

namespace SobekCM.Bib_Package.VRACore
{
    /// <summary> Stores the VRACore-specific data elements for describing visual materials within
    /// the cultural context of their production </summary>
    public class VRACore_Info
    {
        private List<string> culturalContext;
        private List<string> inscription;
        private List<VRACore_Materials_Info> materials;
        private List<VRACore_Measurement_Info> measurements;
        private List<string> stateEdition;
        private List<string> stylePeriod;
        private List<string> technique;

        /// <summary> Constructor for a new instance of the VRACore_Info class </summary>
        public VRACore_Info()
        {
            // Do nothing
        }

        #region Methods and properties related to CULTURAL CONTEXTS 

        /// <summary> Gets the number of cultural contexts associated with this visual resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Cultural_Contexts"/> property.  Even if 
        /// there are no cultural contexts, the Cultural_Contexts property creates a readonly collection to pass back out.</remarks>
        public int Cultural_Context_Count
        {
            get
            {
                if (culturalContext == null)
                    return 0;
                else
                    return culturalContext.Count;
            }
        }

        /// <summary> Gets the collection of cultural contexts related with this visual resource </summary>
        /// <remarks> You should check the count of cultural contexts first using the <see cref="Cultural_Context_Count"/> property before using this property.
        /// Even if there are no contexts, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Cultural_Contexts
        {
            get
            {
                if (culturalContext == null)
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }
                else
                {
                    return new ReadOnlyCollection<string>(culturalContext);
                }
            }
        }

        /// <summary> Clears the list of all cultural contexts associated with this visual resource </summary>
        public void Clear_Cultural_Contexts()
        {
            if (culturalContext != null)
                culturalContext.Clear();
        }

        /// <summary> Adds a new cultural contexts to be associated with this visual resource </summary>
        /// <param name="Cultural_Context"> New cultural context </param>
        public void Add_Cultural_Context(string Cultural_Context)
        {
            if (Cultural_Context.Trim().Length > 0)
            {
                if (culturalContext == null)
                    culturalContext = new List<string>();

                culturalContext.Add(Cultural_Context.Trim());
            }
        }

        #endregion

        #region Methods and properties related to INSCRIPTIONS

        /// <summary> Gets the number of inscriptions associated with this visual resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Inscriptions"/> property.  Even if 
        /// there are no inscriptions, the Inscriptions property creates a readonly collection to pass back out.</remarks>
        public int Inscription_Count
        {
            get
            {
                if (inscription == null)
                    return 0;
                else
                    return inscription.Count;
            }
        }

        /// <summary> Gets the collection of inscriptions related with this visual resource </summary>
        /// <remarks> You should check the count of inscriptions first using the <see cref="Inscription_Count"/> property before using this property.
        /// Even if there are no inscriptions, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Inscriptions
        {
            get
            {
                if (inscription == null)
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }
                else
                {
                    return new ReadOnlyCollection<string>(inscription);
                }
            }
        }

        /// <summary> Clears the list of all inscriptions associated with this visual resource </summary>
        public void Clear_Inscriptions()
        {
            if (inscription != null)
                inscription.Clear();
        }

        /// <summary> Adds a new inscriptions to be associated with this visual resource </summary>
        /// <param name="Inscription"> New inscription </param>
        public void Add_Inscription(string Inscription)
        {
            if (Inscription.Trim().Length > 0)
            {
                if (inscription == null)
                    inscription = new List<string>();

                inscription.Add(Inscription.Trim());
            }
        }

        #endregion

        #region Methods and properties related to MATERIALS

        /// <summary> Gets the number of materials associated with this visual resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Materials"/> property.  Even if 
        /// there are no materials, the Materials property creates a readonly collection to pass back out.</remarks>
        public int Material_Count
        {
            get
            {
                if (materials == null)
                    return 0;
                else
                    return materials.Count;
            }
        }

        /// <summary> Gets the collection of materials related with this visual resource </summary>
        /// <remarks> You should check the count of materials first using the <see cref="Material_Count"/> property before using this property.
        /// Even if there are no materials, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<VRACore_Materials_Info> Materials
        {
            get
            {
                if (materials == null)
                {
                    return new ReadOnlyCollection<VRACore_Materials_Info>(new List<VRACore_Materials_Info>());
                }
                else
                {
                    return new ReadOnlyCollection<VRACore_Materials_Info>(materials);
                }
            }
        }

        /// <summary> Clears the list of all materials associated with this visual resource </summary>
        public void Clear_Materials()
        {
            if (materials != null)
                materials.Clear();
        }

        /// <summary> Adds a new materials to be associated with this visual resource </summary>
        /// <param name="Material"> New material </param>
        public void Add_Material(VRACore_Materials_Info Material)
        {
            if (materials == null)
                materials = new List<VRACore_Materials_Info>();

            materials.Add(Material);
        }

        /// <summary> Adds a new materials to be associated with this visual resource </summary>
        /// <param name="Materials">Substance(s) of which a work or an image is composed</param>
        /// <param name="Type">Type of materials described here, such as ( medium, support, etc.. )</param>
        public void Add_Material(string Materials, string Type)
        {
            if (materials == null)
                materials = new List<VRACore_Materials_Info>();

            materials.Add( new VRACore_Materials_Info(Materials, Type));
        }

        #endregion

        #region Methods and properties related to MEASUREMENTS

        /// <summary> Gets the number of measurements associated with this visual resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Measurements"/> property.  Even if 
        /// there are no measurements, the Measurements property creates a readonly collection to pass back out.</remarks>
        public int Measurement_Count
        {
            get
            {
                if (measurements == null)
                    return 0;
                else
                    return measurements.Count;
            }
        }

        /// <summary> Gets the collection of measurements related with this visual resource </summary>
        /// <remarks> You should check the count of measurements first using the <see cref="Measurement_Count"/> property before using this property.
        /// Even if there are no measurements, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<VRACore_Measurement_Info> Measurements
        {
            get
            {
                if (measurements == null)
                {
                    return new ReadOnlyCollection<VRACore_Measurement_Info>(new List<VRACore_Measurement_Info>());
                }
                else
                {
                    return new ReadOnlyCollection<VRACore_Measurement_Info>(measurements);
                }
            }
        }

        /// <summary> Clears the list of all measurements associated with this visual resource </summary>
        public void Clear_Measurements()
        {
            if (measurements != null)
                measurements.Clear();
        }

        /// <summary> Adds a new measurements to be associated with this visual resource </summary>
        /// <param name="Measurement"> New measurement </param>
        public void Add_Measurement(VRACore_Measurement_Info Measurement)
        {
            if (measurements == null)
                measurements = new List<VRACore_Measurement_Info>();

            measurements.Add(Measurement);
        }

        /// <summary> Adds a new measurements to be associated with this visual resource </summary>
        /// <param name="Measurements">The physical size, shape, scale, dimensions, or format of the Work or Image</param>
        /// <param name="Units">Units for the included measurement(s)</param>
        public void Add_Measurement(string Measurements, string Units)
        {
            if (measurements == null)
                measurements = new List<VRACore_Measurement_Info>();

            measurements.Add( new VRACore_Measurement_Info(Measurements, Units ));
        }

        #endregion

        #region Methods and properties related to STATE/EDITION

        /// <summary> Gets the number of state/editions associated with this visual resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="State_Editions"/> property.  Even if 
        /// there are no state/editions, the State_Editions property creates a readonly collection to pass back out.</remarks>
        public int State_Edition_Count
        {
            get
            {
                if (stateEdition == null)
                    return 0;
                else
                    return stateEdition.Count;
            }
        }

        /// <summary> Gets the collection of state/editions related with this visual resource </summary>
        /// <remarks> You should check the count of state/editions first using the <see cref="State_Edition_Count"/> property before using this property.
        /// Even if there are no state/editions, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> State_Editions
        {
            get
            {
                if (stateEdition == null)
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }
                else
                {
                    return new ReadOnlyCollection<string>(stateEdition);
                }
            }
        }

        /// <summary> Clears the list of all state/editions associated with this visual resource </summary>
        public void Clear_State_Editions()
        {
            if (stateEdition != null)
                stateEdition.Clear();
        }

        /// <summary> Adds a new state/edition to be associated with this visual resource </summary>
        /// <param name="State_Edition"> New state/edition </param>
        public void Add_State_Edition(string State_Edition)
        {
            if (State_Edition.Trim().Length > 0)
            {
                if (stateEdition == null)
                    stateEdition = new List<string>();

                stateEdition.Add(State_Edition.Trim());
            }
        }

        #endregion

        #region Methods and properties related to STYLE/PERIOD

        /// <summary> Gets the number of style/periods associated with this visual resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Style_Periods"/> property.  Even if 
        /// there are no style/periods, the Style_Periods property creates a readonly collection to pass back out.</remarks>
        public int Style_Period_Count
        {
            get
            {
                if (stylePeriod == null)
                    return 0;
                else
                    return stylePeriod.Count;
            }
        }

        /// <summary> Gets the collection of style/periods related with this visual resource </summary>
        /// <remarks> You should check the count of style/periods first using the <see cref="Style_Period_Count"/> property before using this property.
        /// Even if there are no style/periods, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Style_Periods
        {
            get
            {
                if (stylePeriod == null)
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }
                else
                {
                    return new ReadOnlyCollection<string>(stylePeriod);
                }
            }
        }

        /// <summary> Clears the list of all style/periods associated with this visual resource </summary>
        public void Clear_Style_Periods()
        {
            if (stylePeriod != null)
                stylePeriod.Clear();
        }

        /// <summary> Adds a new style/periods to be associated with this visual resource </summary>
        /// <param name="Style_Period"> New style/period </param>
        public void Add_Style_Period(string Style_Period)
        {
            if (Style_Period.Trim().Length > 0)
            {
                if (stylePeriod == null)
                    stylePeriod = new List<string>();

                stylePeriod.Add(Style_Period.Trim());
            }
        }

        #endregion

        #region Methods and properties related to TECHNIQUE

        /// <summary> Gets the number of techniques associated with this visual resource </summary>
        /// <remarks> This should be used rather than the Count property of the <see cref="Techniques"/> property.  Even if 
        /// there are no techniques, the Techniques property creates a readonly collection to pass back out.</remarks>
        public int Technique_Count
        {
            get
            {
                if (technique == null)
                    return 0;
                else
                    return technique.Count;
            }
        }

        /// <summary> Gets the collection of techniques related with this visual resource </summary>
        /// <remarks> You should check the count of techniques first using the <see cref="Technique_Count"/> property before using this property.
        /// Even if there are no techniques, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Techniques
        {
            get
            {
                if (technique == null)
                {
                    return new ReadOnlyCollection<string>(new List<string>());
                }
                else
                {
                    return new ReadOnlyCollection<string>(technique);
                }
            }
        }

        /// <summary> Clears the list of all techniques associated with this visual resource </summary>
        public void Clear_Techniques()
        {
            if (technique != null)
                technique.Clear();
        }

        /// <summary> Adds a new techniques to be associated with this visual resource </summary>
        /// <param name="Technique"> New technique </param>
        public void Add_Technique(string Technique)
        {
            if (Technique.Trim().Length > 0)
            {
                if (technique == null)
                    technique = new List<string>();

                technique.Add(Technique.Trim());
            }
        }

        #endregion

        /// <summary> Flag indicates if this object holds any data in the subfields </summary>
        public bool hasData
        {
            get
            {
                if ((culturalContext != null) && (culturalContext.Count > 0))
                    return true;

                if ((inscription != null) && (inscription.Count > 0))
                    return true;

                if ((materials != null) && (materials.Count > 0))
                    return true;

                if ((measurements != null) && (measurements.Count > 0))
                    return true;

                if ((stateEdition != null) && (stateEdition.Count > 0))
                    return true;

                if ((stylePeriod != null) && (stylePeriod.Count > 0))
                    return true;

                if ((technique != null) && (technique.Count > 0))
                    return true;

                return false;
            }
        }

    }
}
