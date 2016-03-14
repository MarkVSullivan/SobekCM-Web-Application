#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Xml;
using SobekCM.Core.BriefItem;
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
        private static string defaultSetId;
        private static readonly Dictionary<string, List<IBriefItemMapper>> mappingSets;


        /// <summary> Static constructor for the BriefItem_Factory static class </summary>
        static BriefItem_Factory()
        {
            // Create the mapping set dictionary
            mappingSets = new Dictionary<string, List<IBriefItemMapper>>( StringComparer.OrdinalIgnoreCase );
        }

        /// <summary> Create the BriefItemInfo from a full METS-based SobekCM_Item object,
        /// using the default mapping set </summary>
        /// <param name="Original"> Original METS-based object to use </param>
        /// <param name="Tracer"> Custom tracer to record general process flow </param>
        /// <returns> Completely built BriefItemInfo object from the METS-based SobekCM_Item object </returns>
        public static BriefItemInfo Create(SobekCM_Item Original, Custom_Tracer Tracer)
        {
            // Determine the mapping set
            string mappingSet = defaultSetId;
            if (((String.IsNullOrEmpty(mappingSet)) || ( !mappingSets.ContainsKey(mappingSet))) && ( mappingSets.Count > 0 ))
            {
                Tracer.Add_Trace("BriefItem_Factory.Create", "Default mapping set is not present.  Will use (arbitrarily) first set instead.");

                mappingSet = mappingSets.Keys.First();
            }

            // If still no mapping set, return NULL
            if ((String.IsNullOrEmpty(mappingSet)) || (!mappingSets.ContainsKey(mappingSet)))
            {
                Tracer.Add_Trace("BriefItem_Factory.Create", "No suitable mapping set could be found.  Returning NULL");
                return null;
            }

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
            List<IBriefItemMapper> mappers = mappingSets[mappingSet];
            foreach (IBriefItemMapper thisMapper in mappers)
            {
                Tracer.Add_Trace("BriefItem_Factory.Create", "...." + thisMapper.GetType().ToString().Replace("SobekCM.Engine_Library.Items.BriefItems.Mappers.", ""));
                thisMapper.MapToBriefItem(Original, newItem);
            }

            return newItem;
        }

        /// <summary> Create the BriefItemInfo from a full METS-based SobekCM_Item object,
        /// using the default mapping set </summary>
        /// <param name="Original"> Original METS-based object to use </param>
        /// <param name="MappingSetId"> Name of the mapping set to use (if there are more than one)</param>
        /// <param name="Tracer"> Custom tracer to record general process flow </param>
        /// <returns> Completely built BriefItemInfo object from the METS-based SobekCM_Item object </returns>
        public static BriefItemInfo Create(SobekCM_Item Original, string MappingSetId, Custom_Tracer Tracer )
        {
            // Ensure the mapping set exists
            if (!mappingSets.ContainsKey(MappingSetId))
            {
                Tracer.Add_Trace("BriefItem_Factory.Create", "Requested MappingSetID '" + MappingSetId + "' not found.  Will use default set instead.");

                // Just try to use the default
                MappingSetId = defaultSetId;

                // Determine the mapping set
                if (((String.IsNullOrEmpty(MappingSetId)) || (!mappingSets.ContainsKey(MappingSetId))) && (mappingSets.Count > 0))
                {
                    Tracer.Add_Trace("BriefItem_Factory.Create", "Default mapping set is not present either.  Will use (arbitrarily) first set instead.");

                    MappingSetId = mappingSets.Keys.First();
                }

                // If still no mapping set, return NULL
                if ((String.IsNullOrEmpty(MappingSetId)) || (!mappingSets.ContainsKey(MappingSetId)))
                {
                    Tracer.Add_Trace("BriefItem_Factory.Create", "No suitable mapping set could be found.  Returning NULL");
                    return null;
                }
            }

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
            List<IBriefItemMapper> mappers = mappingSets[MappingSetId];
            foreach (IBriefItemMapper thisMapper in mappers)
            {
                Tracer.Add_Trace("BriefItem_Factory.Create", "...." + thisMapper.GetType().ToString().Replace("SobekCM.Engine_Library.Items.BriefItems.Mappers.",""));
                thisMapper.MapToBriefItem(Original, newItem);
            }

            return newItem;
        }


        /// <summary> If there was an error while reading the configuration,
        /// the error will be placed here for displaying later </summary>
        public static string Read_Config_Error { get; private set; }

        /// <summary> Read the configuration file for the brief item mapping sets </summary>
        /// <param name="ConfigFile"> Path and name of the configuration file to read </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Read_Config(string ConfigFile)
        {
            // Initialize the config error read
            Read_Config_Error = String.Empty;

            // Streams used for reading
            Stream readerStream = null;
            XmlTextReader readerXml = null;

            // During this process, small objects ( IBriefItemMappers ) which contain no data
            // but implement the mapping method will be created.  This dictionary helps to ensure
            // each one is created only once.
            Dictionary<string, IBriefItemMapper> mappingObjDictionary = new Dictionary<string, IBriefItemMapper>();

            try
            {
                // Open a link to the file
                readerStream = new FileStream(ConfigFile, FileMode.Open, FileAccess.Read);

                // Open a XML reader connected to the file
                readerXml = new XmlTextReader(readerStream);

                while (readerXml.Read())
                {
                    if (readerXml.NodeType == XmlNodeType.Element)
                    {
                        switch (readerXml.Name.ToLower())
                        {
                            case "mappingset":
                                // Get the ID for this mapping set
                                string id = String.Empty;
                                if (readerXml.MoveToAttribute("ID"))
                                    id = readerXml.Value.Trim();

                                // Was this indicated as the default set?
                                if (readerXml.MoveToAttribute("Default"))
                                {
                                    if (String.Compare(readerXml.Value, "true", StringComparison.OrdinalIgnoreCase) == 0)
                                    {
                                        if (id.Length > 0)
                                            defaultSetId = id;
                                        else
                                        {
                                            defaultSetId = "DEFAULT";
                                            id = "DEFAULT";
                                        }
                                    }
                                }

                                // Read the set here
                                readerXml.MoveToElement();
                                List<IBriefItemMapper> mapSet = read_mappingset_details(readerXml.ReadSubtree(), mappingObjDictionary);

                                // Save in the dictionary of mapping sets
                                if (id.Length > 0)
                                {
                                    mappingSets[id] = mapSet;
                                }
                                break;
                        }
                    }
                }
            }
            catch (Exception ee)
            {
                Read_Config_Error = ee.Message;
            }
            finally
            {
                if (readerXml != null)
                {
                    readerXml.Close();
                }
                if (readerStream != null)
                {
                    readerStream.Close();
                }
            }

            return ( Read_Config_Error.Length == 0 );
        }

        private static List<IBriefItemMapper> read_mappingset_details(XmlReader ReaderXml, Dictionary<string, IBriefItemMapper> MappingObjDictionary)
        {
            // Create the empty return value
            List<IBriefItemMapper> returnValue = new List<IBriefItemMapper>();

            // Just step through the subtree of this
            while (ReaderXml.Read())
            {
                if (ReaderXml.NodeType == XmlNodeType.Element)
                {
                    switch (ReaderXml.Name.ToLower())
                    {
                        case "mapper":
                            // Read all the data for this mapper class
                            string mapperAssembly = String.Empty;
                            string mapperClass = String.Empty;
                            if (ReaderXml.MoveToAttribute("Assembly"))
                                mapperAssembly = ReaderXml.Value.Trim();
                            if (ReaderXml.MoveToAttribute("Class"))
                                mapperClass = ReaderXml.Value.Trim();

                            // Was this enabled?
                            bool enabled = true;
                            if (ReaderXml.MoveToAttribute("Default"))
                            {
                                if (String.Compare(ReaderXml.Value, "false", StringComparison.OrdinalIgnoreCase) == 0)
                                {
                                    enabled = false;
                                }
                            }

                            // Add this (if enabled) to the list of mappers
                            if (enabled)
                            {
                                string error;
                                IBriefItemMapper mapper = get_or_create_mapper(mapperAssembly, mapperClass, MappingObjDictionary, out error );
                                returnValue.Add(mapper);
                            }


                            break;
                    }
                }
            }

            return returnValue;
        }

        private static IBriefItemMapper get_or_create_mapper(string MapperAssembly, string MapperClass, Dictionary<string, IBriefItemMapper> MappingObjDictionary, out string ErrorMessage)
        {
            ErrorMessage = String.Empty;

            // Was this already created (for a different mapping set)?
            if (MappingObjDictionary.ContainsKey(MapperAssembly + "." + MapperClass))
                return MappingObjDictionary[MapperAssembly + "." + MapperClass];

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
                    MappingObjDictionary[MapperAssembly + "." + MapperClass] = thisModule;

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
            MappingObjDictionary[MapperAssembly + "." + MapperClass] = itemAsItem;

            // Return this custom IBriefItemMapper
            return itemAsItem;
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
