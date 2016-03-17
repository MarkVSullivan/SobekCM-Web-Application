#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml;
using SobekCM.Core.BriefItem;
using SobekCM.Core.Configuration;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Engine_Library.Items.BriefItems.Mappers;
using SobekCM.Resource_Object;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Class is used to create the brief item object, from the original SobekCM_Item read
    /// from the METS file </summary>
    public static class BriefItem_Factory
    {
        private static Dictionary<string, List<IBriefItemMapper>> mappingSets;
        private static Dictionary<string, IBriefItemMapper> mappingObjDictionary;


        /// <summary> Create the BriefItemInfo from a full METS-based SobekCM_Item object,
        /// using the default mapping set </summary>
        /// <param name="Original"> Original METS-based object to use </param>
        /// <param name="Tracer"> Custom tracer to record general process flow </param>
        /// <returns> Completely built BriefItemInfo object from the METS-based SobekCM_Item object </returns>
        public static BriefItemInfo Create(SobekCM_Item Original, Custom_Tracer Tracer)
        {
            // Determine the mapping set
            return Create(Original, Engine_ApplicationCache_Gateway.Configuration.BriefItemMapping.DefaultSetName, Tracer);
        }

        /// <summary> Create the BriefItemInfo from a full METS-based SobekCM_Item object,
        /// using the default mapping set </summary>
        /// <param name="Original"> Original METS-based object to use </param>
        /// <param name="MappingSetId"> Name of the mapping set to use (if there are more than one)</param>
        /// <param name="Tracer"> Custom tracer to record general process flow </param>
        /// <returns> Completely built BriefItemInfo object from the METS-based SobekCM_Item object </returns>
        public static BriefItemInfo Create(SobekCM_Item Original, string MappingSetId, Custom_Tracer Tracer )
        {
            // Try to get the brief mapping set
            List<IBriefItemMapper> mappingSet = get_mapping_set(MappingSetId);

            // Create the mostly empty new brief item
            Tracer.Add_Trace("BriefItem_Factory.Create", "Create the mostly empty new brief item");
            BriefItemInfo newItem = new BriefItemInfo
            {
                BibID = Original.BibID,
                VID = Original.VID,
                Title = Original.Bib_Info.Main_Title.Title
            };

            // Build the new item using the selected mapping set
            Tracer.Add_Trace("BriefItem_Factory.Create", "Use the set of mappers to map data to the brief item");
            foreach (IBriefItemMapper thisMapper in mappingSet)
            {
                Tracer.Add_Trace("BriefItem_Factory.Create", "...." + thisMapper.GetType().ToString().Replace("SobekCM.Engine_Library.Items.BriefItems.Mappers.",""));
                thisMapper.MapToBriefItem(Original, newItem);
            }

            return newItem;
        }

        private static IBriefItemMapper get_or_create_mapper(string MapperAssembly, string MapperClass, out string ErrorMessage)
        {
            ErrorMessage = String.Empty;

            // Ensure this is built
            if (mappingObjDictionary == null)
                mappingObjDictionary = new Dictionary<string, IBriefItemMapper>();

            // Was this already created (for a different mapping set)?
            if (mappingObjDictionary.ContainsKey(MapperAssembly + "." + MapperClass))
                return mappingObjDictionary[MapperAssembly + "." + MapperClass];

            // Look for the standard classes, just to avoid having to use reflection
            // for these that are built right into the system
            if (String.IsNullOrEmpty(MapperAssembly))
            {
                IBriefItemMapper thisModule = null;
                switch (MapperClass)
                {
                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Abstracts_BriefItemMapper":
                        thisModule = new Abstracts_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Affiliations_BriefItemMapper":
                        thisModule = new Affiliations_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Aggregations_BriefItemMapper":
                        thisModule = new Aggregations_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Behaviors_BriefItemMapper":
                        thisModule = new Behaviors_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Classifications_BriefItemMapper":
                        thisModule = new Classifications_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Containers_BriefItemMapper":
                        thisModule = new Containers_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Dates_BriefItemMapper":
                        thisModule = new Dates_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Donor_BriefItemMapper":
                        thisModule = new Donor_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Edition_BriefItemMapper":
                        thisModule = new Edition_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Files_BriefItemMapper":
                        thisModule = new Files_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Frequency_BriefItemMapper":
                        thisModule = new Frequency_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Genres_BriefItemMapper":
                        thisModule = new Genres_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.GeoSpatial_BriefItemMapper":
                        thisModule = new GeoSpatial_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Holding_Location_BriefItemMapper":
                        thisModule = new Holding_Location_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Identifiers_BriefItemMapper":
                        thisModule = new Identifiers_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.InternalComments_BriefItemMapper":
                        thisModule = new InternalComments_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.InternalVarious_BriefItemMapper":
                        thisModule = new InternalVarious_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Languages_BriefItemMapper":
                        thisModule = new Languages_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.LearningObjectMetadata_BriefItemMapper":
                        thisModule = new LearningObjectMetadata_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Links_BriefItemMapper":
                        thisModule = new Links_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Manufacturers_BriefItemMapper":
                        thisModule = new Manufacturers_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Names_BriefItemMapper":
                        thisModule = new Names_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Notes_BriefItemMapper":
                        thisModule = new Notes_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Physical_Description_BriefItemMapper":
                        thisModule = new Physical_Description_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Publisher_BriefItemMapper":
                        thisModule = new Publisher_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Related_Items_BriefItemMapper":
                        thisModule = new Related_Items_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.ResourceType_BriefItemMapper":
                        thisModule = new ResourceType_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Rights_BriefItemMapper":
                        thisModule = new Rights_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Rights_MD_BriefItemMapper":
                        thisModule = new Rights_MD_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Source_Institution_BriefItemMapper":
                        thisModule = new Source_Institution_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Standard_BriefItemMapper":
                        thisModule = new Standard_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Subjects_BriefItemMapper":
                        thisModule = new Subjects_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Target_Audience_BriefItemMapper":
                        thisModule = new Target_Audience_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Temporal_Coverage_BriefItemMapper":
                        thisModule = new Temporal_Coverage_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Thesis_Dissertation_BriefItemMapper":
                        thisModule = new Thesis_Dissertation_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Titles_BriefItemMapper":
                        thisModule = new Titles_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.User_Tags_BriefItemMapper":
                        thisModule = new User_Tags_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.User_Tags_Internal_BriefItemMapper":
                        thisModule = new User_Tags_Internal_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.VRACore_BriefItemMapper":
                        thisModule = new VRACore_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Web_BriefItemMapper":
                        thisModule = new Web_BriefItemMapper();
                        break;

                    case "SobekCM.Engine_Library.Items.BriefItems.Mappers.Zoological_Taxonomy_BriefItemMapper":
                        thisModule = new Zoological_Taxonomy_BriefItemMapper();
                        break;
                }

                // Was this a match?
                if (thisModule != null)
                {
                    // Add to the dictionary to avoid looking this up again
                    mappingObjDictionary[MapperAssembly + "." + MapperClass] = thisModule;

                    // Return this standard IBriefItemMapper
                    return thisModule;
                }
            }

            // Try to retrieve this from the assembly using reflection
            object itemAsObj = Get_Mapper(MapperAssembly, MapperClass, out ErrorMessage);
            if ((itemAsObj == null) && (ErrorMessage.Length > 0))
            {
                return null;
            }


            // Ensure this implements the IBriefItemMapper class 
            IBriefItemMapper itemAsItem = itemAsObj as IBriefItemMapper;
            if (itemAsItem == null)
            {
                ErrorMessage = MapperClass + " loaded from assembly but does not implement the IBriefItemMapper interface!";
                return null;
            }


            // Add to the dictionary to avoid looking this up again
            mappingObjDictionary[MapperAssembly + "." + MapperClass] = itemAsItem;

            // Return this custom IBriefItemMapper
            return itemAsItem;
        }

        /// <summary> Clears the built instances of the mapping objects here, which forces the mappers
        /// to be rebuilt the next time it is called </summary>
        public static void Clear()
        {
            if (mappingSets != null) mappingSets.Clear();
            mappingSets = null;
            if (mappingObjDictionary != null) mappingObjDictionary.Clear();
            mappingObjDictionary = null;
        }

        private static List<IBriefItemMapper> get_mapping_set(string mappingSet)
        {
            // First, look in the dictionary, if it exists
            if ((mappingSets != null) && (mappingSets.ContainsKey(mappingSet)))
                return mappingSets[mappingSet];

            // Ensure the dictionary exists
            if (mappingSets == null)
                mappingSets = new Dictionary<string, List<IBriefItemMapper>>(StringComparer.OrdinalIgnoreCase);

            // Is this a valid mapping set id?
            BriefItemMapping_Set set = Engine_ApplicationCache_Gateway.Configuration.BriefItemMapping.GetMappingSet(mappingSet);
            if (set == null)
                return null;

            // build error messages
            StringBuilder errormessages = new StringBuilder();

            // Build this return list
            List<IBriefItemMapper> returnValue = new List<IBriefItemMapper>();
            foreach (BriefItemMapping_Mapper mappingConfig in set.Mappings)
            {
                // Build the mapper
                string errorMessage;
                IBriefItemMapper mapper = get_or_create_mapper(mappingConfig.Assembly, mappingConfig.Class, out errorMessage);
                if ( mapper != null )
                    returnValue.Add(mapper);
            }

            // Now, set to the dictionary
            mappingSets[mappingSet] = returnValue;

            // Return
            return returnValue;
        } 

        private static object Get_Mapper(string MapperAssembly, string MapperClass, out string ErrorMessage)
        {
            ErrorMessage = String.Empty;

            try
            {
                // Using reflection, create an object from the class namespace/name 
                Assembly dllAssembly = Assembly.GetExecutingAssembly();
                if (!String.IsNullOrEmpty(MapperAssembly))
                {
                    dllAssembly = Assembly.LoadFrom(MapperAssembly);
                }

                Type readerWriterType = dllAssembly.GetType(MapperClass);
                return Activator.CreateInstance(readerWriterType);
            }
            catch (Exception ee)
            {
                ErrorMessage = "Unable to load class from assembly. ( " + MapperClass + " ) : " + ee.Message;
                return null;
            }
        }
    }
}
