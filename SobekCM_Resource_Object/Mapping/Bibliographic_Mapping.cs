using System;
using System.Collections.Generic;
using SobekCM.Resource_Object.Configuration;


namespace SobekCM.Resource_Object.Mapping
{
    /// <summary> Static class is used to map field/value pairs into a SobekCM digital resource </summary>
    public static class Bibliographic_Mapping
    {
        private static List<iBibliographicMapper> mappers;
        private static readonly Object mappersLock = new Object();

        /// <summary> Initializes the static class </summary>
        static Bibliographic_Mapping()
        {
            Set_Mappings();
        }

        #region Methods to configure and ensure this mapping is configured

        /// <summary> Sets the mapping configuration and builds the <see cref="iBibliographicMapper"/> 
        /// objects to handle the incoming field/value pairs </summary>
        /// <remarks> The configuration is loaded from the <see cref="ResourceObjectSettings" /> metadata
        /// configuration values. </remarks>
        public static void Set_Mappings()
        {
            Set_Mappings(ResourceObjectSettings.MetadataConfig.Mapping_Configs);
        }

        /// <summary> Sets the mapping configuration and builds the <see cref="iBibliographicMapper"/> 
        /// objects to handle the incoming field/value pairs </summary>
        /// <param name="MapperConfigs"> Mapper configuration values, used to determine which mappers to
        /// create, and in what order </param>
        public static void Set_Mappings( List<Metadata_Mapping_Config> MapperConfigs )
        {
            lock (mappersLock)
            {
                // Either initialize or clear the mapping
                if (mappers == null)
                    mappers = new List<iBibliographicMapper>();
                else
                    mappers.Clear();

                // Build the mapper
                foreach (Metadata_Mapping_Config config in MapperConfigs)
                {
                    mappers.Add(config.Get_Module());
                }
            }
        }

        /// <summary> Verified the <see cref="iBibliographicMapper"/> objects were loaded from the
        /// configuration, and loads if different </summary>
        /// <remarks> The configuration is loaded from the <see cref="ResourceObjectSettings" /> metadata
        /// configuration values. </remarks>
        public static void Verify_Initialization()
        {
            Verify_Initialization(ResourceObjectSettings.MetadataConfig.Mapping_Configs);
        }

        /// <summary> Verified the <see cref="iBibliographicMapper"/> objects were loaded from the
        /// configuration, and loads if different </summary>
        /// <param name="MapperConfigs"> Mapper configuration values, used to determine which mappers to
        /// create, and in what order </param>
        public static void Verify_Initialization(List<Metadata_Mapping_Config> MapperConfigs)
        {
            // Check that mappers is NULL, in which case we obviously need to set it
            bool needs_setting = (mappers == null) || (mappers.Count != MapperConfigs.Count);

            // Check each name
            if (!needs_setting)
            {
                for (int i = 0; i < MapperConfigs.Count; i++)
                {
                    string mapper_type = mappers[i].GetType().ToString();

                    if (!String.Equals(mapper_type, MapperConfigs[i].Code_Class, StringComparison.OrdinalIgnoreCase))
                    {
                        needs_setting = true;
                        break;
                    }
                }
            }

            // If this needs setting, then call the setting method
            Set_Mappings(MapperConfigs);
        }

        #endregion

        /// <summary> Adds a bit of data to a bibliographic package using the mapping </summary>
        /// <param name="Package">Bibliographic package to receive the data</param>
        /// <param name="Data">Text of the data</param>
        /// <param name="Field">Mapped field</param>
        /// <param name="Message"> [OUT] Message also indicates if the field was mapped, and which mapper found the match </param>
        /// <returns> TRUE if the field was mapped, FALSE if there was data and no mapping was found </returns>
        public static bool Add_Data(SobekCM_Item Package, string Data, string Field, out string Message )
        {
            // Try to map using the mappers in order
            foreach (iBibliographicMapper mapper in mappers)
            {
                // Did this map this piece of data?
                if (mapper.Add_Data(Package, Data, Field))
                {
                    Message = "Mapped '" + Field + "' using " + mapper.Name;
                    return true;
                }
            }

            // Apparently it was never mapped
            Message = "Unable to find a mapping for '" + Field + "'";
            return false;
        }

        /// <summary> Adds a bit of data to a bibliographic package using the mapping </summary>
        /// <param name="Package">Bibliographic package to receive the data</param>
        /// <param name="Data">Text of the data</param>
        /// <param name="Field">Mapped field</param>
        /// <returns> TRUE if the field was mapped, FALSE if there was data and no mapping was found </returns>
        public static bool Add_Data(SobekCM_Item Package, string Data, string Field )
        {
            // Try to map using the mappers in order
            foreach (iBibliographicMapper mapper in mappers)
            {
                // Did this map this piece of data?
                if (mapper.Add_Data(Package, Data, Field))
                {
                    return true;
                }
            }

            // Apparently it was never mapped
            return false;
        }
    }
}
