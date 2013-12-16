#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Data;
using System.Data.SqlClient;
using System.Text;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.VRACore
{
    /// <summary> Stores the VRACore-specific data elements for describing visual materials within
    /// the cultural context of their production </summary>
    [Serializable]
    public class VRACore_Info : iMetadata_Module
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

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'VRACore'</value>
        public string Module_Name
        {
            get { return "VRACore"; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get
            {
                List<KeyValuePair<string, string>> metadataTerms = new List<KeyValuePair<string, string>>();

                // Add all the cultural contexts
                if (Cultural_Context_Count > 0)
                {
                    foreach (string culturalContext in Cultural_Contexts)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Cultural Context", culturalContext));
                    }
                }

                // Add all the inscriptions
                if (Inscription_Count > 0)
                {
                    foreach (string inscription in Inscriptions)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Inscription", inscription));
                    }
                }

                // Add all the material information
                if (Material_Count > 0)
                {
                    foreach (VRACore_Materials_Info materialInfo in  Materials)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Material", materialInfo.Materials));
                    }
                }

                // Add all the measurement information
                if (Measurement_Count > 0)
                {
                    foreach (VRACore_Measurement_Info measurement in Measurements)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Measurements", measurement.Measurements));
                    }
                }

                // Add all the style/period information
                if (Style_Period_Count > 0)
                {
                    foreach (string stylePeriod in  Style_Periods)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Style Period", stylePeriod));
                    }
                }

                // Add all the technique information
                if (Technique_Count > 0)
                {
                    foreach (string technique in Techniques)
                    {
                        metadataTerms.Add(new KeyValuePair<string, string>("Technique", technique));
                    }
                }

                return metadataTerms;
            }
        }

        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently calls the SobekCM_Save_Item_VRACore_Extensions stored procedure to save
        /// these values in the display table portion of the database </remarks>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            // Get the material display information
            StringBuilder materialDisplayBuilder = new StringBuilder();
            if (Material_Count > 0)
            {
                foreach (VRACore_Materials_Info materials in  Materials)
                {
                    if (materialDisplayBuilder.Length > 0)
                        materialDisplayBuilder.Append(", ");
                    materialDisplayBuilder.Append(materials.Materials);
                }
            }

            // Get the measurement display information
            string measurements = String.Empty;
            if (Measurement_Count > 0)
            {
                measurements = Measurements[0].Measurements;
            }

            // Get the style period display information
            StringBuilder stylePeriodDisplayBuilder = new StringBuilder();
            if (Style_Period_Count > 0)
            {
                foreach (string stylePeriod in Style_Periods)
                {
                    if (stylePeriodDisplayBuilder.Length > 0)
                        stylePeriodDisplayBuilder.Append(", ");
                    stylePeriodDisplayBuilder.Append(stylePeriod);
                }
            }

            // Get the technique display information
            StringBuilder techniqueDisplayBuilder = new StringBuilder();
            if (Technique_Count > 0)
            {
                foreach (string technique in Techniques)
                {
                    if (techniqueDisplayBuilder.Length > 0)
                        techniqueDisplayBuilder.Append(", ");
                    techniqueDisplayBuilder.Append(technique);
                }
            }

            // If there are no values to save, return
            if ((techniqueDisplayBuilder.Length == 0) && (stylePeriodDisplayBuilder.Length == 0) && (measurements.Length == 0) && (materialDisplayBuilder.Length == 0))
                return true;

            // Open the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection(DB_ConnectionString))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch (Exception ex)
                {
                    Error_Message = "Error opening connection to the database from VRACore_Info metadata module : " + ex.Message;
                    return false;
                }

                // Create the sql command / stored procedure
                SqlCommand cmd = new SqlCommand("SobekCM_Save_Item_VRACore_Extensions");
                cmd.CommandType = CommandType.StoredProcedure;
                cmd.Connection = sqlConnect;

                // Add the parameters
                cmd.Parameters.AddWithValue("@itemid", ItemID);
                cmd.Parameters.AddWithValue("@Material_Display", materialDisplayBuilder.ToString());
                cmd.Parameters.AddWithValue("@Measurement_Display", measurements);
                cmd.Parameters.AddWithValue("@StylePeriod_Display", stylePeriodDisplayBuilder.ToString());
                cmd.Parameters.AddWithValue("@Technique_Display", techniqueDisplayBuilder.ToString());

                // Execute the non-query SQL stored procedure
                try
                {
                    // Run the command
                    cmd.ExecuteNonQuery();
                }
                catch (Exception ex)
                {
                    Error_Message = "Error saving values from VRACore_Info metadata module : " + ex.Message;
                    return false;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch (Exception ex)
                {
                    Error_Message = "Error closing the connection to the database from VRACore_Info metadata module : " + ex.Message;
                    return false;
                }
            }

            return true;
        }



        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        #endregion

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

            materials.Add(new VRACore_Materials_Info(Materials, Type));
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

            measurements.Add(new VRACore_Measurement_Info(Measurements, Units));
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