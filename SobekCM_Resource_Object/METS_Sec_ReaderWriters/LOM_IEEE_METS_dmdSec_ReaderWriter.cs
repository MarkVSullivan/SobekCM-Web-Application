#region Using directives

using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using SobekCM.Resource_Object.Metadata_Modules;
using SobekCM.Resource_Object.Metadata_Modules.LearningObjects;

#endregion

namespace SobekCM.Resource_Object.METS_Sec_ReaderWriters
{
    /// <summary> Class is used to read the IEEE-LOM learning object metadata from within a 
    /// SobekCM METS file </summary>
    public class LOM_IEEE_METS_dmdSec_ReaderWriter : XML_Writing_Base_Type, iPackage_dmdSec_ReaderWriter
    {
        private const string lom_namespace = "lom:";
        private const string indent = "  ";

        #region iPackage_dmdSec_ReaderWriter Members

        /// <summary> Flag indicates if this active reader/writer will write a dmdSec </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns> TRUE if the package has data to be written, otherwise fALSE </returns>
        public bool Include_dmdSec(SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            LearningObjectMetadata lomInfo = METS_Item.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if ((lomInfo == null) || (!lomInfo.hasData))
                return false;
            return true;
        }

        /// <summary> Flag indicates if this active reader/writer needs to append schema reference information
        /// to the METS XML header by analyzing the contents of the digital resource item </summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> TRUE if the schema should be attached, otherwise fALSE </returns>
        public bool Schema_Reference_Required_Package(SobekCM_Item METS_Item)
        {
            return true;
        }

        /// <summary> Returns the schema namespace (xmlns) information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema namespace info for the METS header</returns>
        public string[] Schema_Namespace(SobekCM_Item METS_Item)
        {
            return new string[] { "lom=\"http://sobekrepository.org/schemas/sobekcm_lom\"" };
        }

        /// <summary> Returns the schema location information to be written in the XML/METS Header</summary>
        /// <param name="METS_Item"> Package with all the metadata to save</param>
        /// <returns> Formatted schema location for the METS header</returns>
        public string[] Schema_Location(SobekCM_Item METS_Item)
        {
            return new string[0];
        }

        #endregion

        #region Method to write the learning object metadata in LOM-IEEE XML format within a dmdSec

        /// <summary> Writes the dmdSec for the entire package to the text writer </summary>
        /// <param name="Output_Stream">Stream to which the formatted text is written </param>
        /// <param name="METS_Item">Package with all the metadata to save</param>
        /// <param name="Options"> Dictionary of any options which this METS section writer may utilize</param>
        /// <returns>TRUE if successful, otherwise FALSE </returns>
        public bool Write_dmdSec(TextWriter Output_Stream, SobekCM_Item METS_Item, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists and has data
            LearningObjectMetadata lomInfo = METS_Item.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if ((lomInfo == null) || (!lomInfo.hasData))
                return true;

            // Start the LOM node
            Output_Stream.WriteLine( "<" + lom_namespace + "lom>");

            // Add any aggregation level
            if (lomInfo.AggregationLevel != AggregationLevelEnum.UNDEFINED)
            {
                Output_Stream.WriteLine(indent + "<" + lom_namespace + "general>");
                Output_Stream.Write(indent + indent + "<" + lom_namespace + "aggregationLevel>");
                switch ( lomInfo.AggregationLevel )
                {
                    case AggregationLevelEnum.level1:
                        Output_Stream.Write("1");
                        break;

                    case AggregationLevelEnum.level2:
                        Output_Stream.Write("2");
                        break;

                    case AggregationLevelEnum.level3:
                        Output_Stream.Write("3");
                        break;

                    case AggregationLevelEnum.level4:
                        Output_Stream.Write("4");
                        break;
                }
                Output_Stream.WriteLine("</" + lom_namespace + "aggregationLevel>");
                Output_Stream.WriteLine(indent + "</" + lom_namespace + "general>");
            }

            // Add ny lifecycle / status
            if (lomInfo.Status != StatusEnum.UNDEFINED)
            {
                Output_Stream.WriteLine(indent + "<" + lom_namespace + "lifecycle>");
                Output_Stream.Write(indent + indent + "<" + lom_namespace + "status>");
                switch (lomInfo.Status)
                {
                    case StatusEnum.draft:
                        Output_Stream.Write("draft");
                        break;

                    case StatusEnum.final:
                        Output_Stream.Write("final");
                        break;

                    case StatusEnum.revised:
                        Output_Stream.Write("revised");
                        break;

                    case StatusEnum.unavailable:
                        Output_Stream.Write("uavailable");
                        break;
                }
                Output_Stream.WriteLine("</" + lom_namespace + "status>");
                Output_Stream.WriteLine(indent + "</" + lom_namespace + "lifecycle>");
            }

            // Add technical / requirements
            if (lomInfo.SystemRequirements.Count > 0)
            {
                Output_Stream.WriteLine(indent + "<" + lom_namespace + "technical>");

                foreach (LOM_System_Requirements thisRequirement in lomInfo.SystemRequirements)
                {
                    // Must have a type and name
                    if (( thisRequirement.RequirementType == RequirementTypeEnum.UNDEFINED ) || ( String.IsNullOrWhiteSpace( thisRequirement.Name.Value)))
                        continue;

                    // Start to write this requirement
                    Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "requirement>");
                    Output_Stream.WriteLine(indent + indent + indent + "<" + lom_namespace + "orcomposite>");

                    // Add the requirement type
                    switch ( thisRequirement.RequirementType )
                    {
                        case RequirementTypeEnum.browser:
                            Output_Stream.WriteLine( indent + indent + indent + indent + "<" + lom_namespace + "type>browser</" + lom_namespace + "type>");
                            break;

                        case RequirementTypeEnum.hardware:
                            Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "type>hardware</" + lom_namespace + "type>");
                            break;

                        case RequirementTypeEnum.operating_system:
                            Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "type>operating system</" + lom_namespace + "type>");
                            break;

                        case RequirementTypeEnum.software:
                            Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "type>software</" + lom_namespace + "type>");
                            break;
                    }

                    // Add the name
                    if ( !String.IsNullOrWhiteSpace( thisRequirement.Name.Source ))
                        Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "name source=\"" + Convert_String_To_XML_Safe(thisRequirement.Name.Source) + "\">" + Convert_String_To_XML_Safe(thisRequirement.Name.Value) + "</" + lom_namespace + "name>");
                    else
                        Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "name>" + Convert_String_To_XML_Safe(thisRequirement.Name.Value) + "</" + lom_namespace + "name>");


                    // Add the minimum version, if one exists
                    if (!String.IsNullOrWhiteSpace(thisRequirement.MinimumVersion))
                    {
                        Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "minimumversion>" + Convert_String_To_XML_Safe(thisRequirement.MinimumVersion) + "</" + lom_namespace + "minimumversion>");
                    }

                    // Add the maximum version, if one exists
                    if (!String.IsNullOrWhiteSpace(thisRequirement.MaximumVersion))
                    {
                        Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "maximumversion>" + Convert_String_To_XML_Safe(thisRequirement.MaximumVersion) + "</" + lom_namespace + "maximumversion>");
                    }

                    Output_Stream.WriteLine(indent + indent + indent + "</" + lom_namespace + "orcomposite>");
                    Output_Stream.WriteLine(indent + indent + "</" + lom_namespace + "requirement>");
                }
                Output_Stream.WriteLine(indent + "</" + lom_namespace + "technical>");
            }

            // Add the educational part
            if ((lomInfo.InteractivityType != InteractivityTypeEnum.UNDEFINED) || (lomInfo.LearningResourceTypes.Count > 0) || (lomInfo.InteractivityLevel != InteractivityLevelEnum.UNDEFINED) ||
                (lomInfo.IntendedEndUserRoles.Count > 0) || (lomInfo.Contexts.Count > 0) || (lomInfo.TypicalAgeRanges.Count > 0) || (lomInfo.DifficultyLevel != DifficultyLevelEnum.UNDEFINED) ||
                (!String.IsNullOrWhiteSpace(lomInfo.TypicalLearningTime)))
            {
                Output_Stream.WriteLine(indent + "<" + lom_namespace + "educational>");

                // Add interactivity type
                if (lomInfo.InteractivityType != InteractivityTypeEnum.UNDEFINED)
                {
                    switch ( lomInfo.InteractivityType )
                    {
                        case InteractivityTypeEnum.active:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitytype>active</" + lom_namespace + "interactivitytype>");
                            break;

                        case InteractivityTypeEnum.expositive:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitytype>expositive</" + lom_namespace + "interactivitytype>");
                            break;

                        case InteractivityTypeEnum.mixed:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitytype>mixed</" + lom_namespace + "interactivitytype>");
                            break;
                    }
                }

                // Add the learning resource types
                foreach (LOM_VocabularyState thisType in lomInfo.LearningResourceTypes)
                {
                    if ( String.IsNullOrWhiteSpace( thisType.Source ))
                        Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "learningresourcetype>" + Convert_String_To_XML_Safe( thisType.Value ) + "</" + lom_namespace + "learningresourcetype>");
                    else
                        Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "learningresourcetype source=\"" + Convert_String_To_XML_Safe( thisType.Source ) + "\">" + Convert_String_To_XML_Safe(thisType.Value) + "</" + lom_namespace + "learningresourcetype>");
                 }

                // Add the interactivity level
                if (lomInfo.InteractivityLevel != InteractivityLevelEnum.UNDEFINED)
                {
                    switch (lomInfo.InteractivityLevel)
                    {
                        case InteractivityLevelEnum.very_high:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitylevel>very high</" + lom_namespace + "interactivitylevel>");
                            break;

                        case InteractivityLevelEnum.high:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitylevel>high</" + lom_namespace + "interactivitylevel>");
                            break;

                        case InteractivityLevelEnum.medium:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitylevel>medium</" + lom_namespace + "interactivitylevel>");
                            break;

                        case InteractivityLevelEnum.low:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitylevel>low</" + lom_namespace + "interactivitylevel>");
                            break;

                        case InteractivityLevelEnum.very_low:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "interactivitylevel>very low</" + lom_namespace + "interactivitylevel>");
                            break;
                    }
                }

                // Add the intended user role
                foreach (IntendedEndUserRoleEnum thisRole in lomInfo.IntendedEndUserRoles)
                {
                    switch (thisRole)
                    {
                        case IntendedEndUserRoleEnum.author:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "intendedenduserrole>author</" + lom_namespace + "intendedenduserrole>");
                            break;

                        case IntendedEndUserRoleEnum.learner:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "intendedenduserrole>learner</" + lom_namespace + "intendedenduserrole>");
                            break;

                        case IntendedEndUserRoleEnum.manager:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "intendedenduserrole>manager</" + lom_namespace + "intendedenduserrole>");
                            break;

                        case IntendedEndUserRoleEnum.teacher:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "intendedenduserrole>teacher</" + lom_namespace + "intendedenduserrole>");
                            break;
                    }
                }

                // Add the context information
                foreach (LOM_VocabularyState thisContext in lomInfo.Contexts)
                {
                    if (String.IsNullOrWhiteSpace(thisContext.Source))
                        Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "context>" + Convert_String_To_XML_Safe(thisContext.Value) + "</" + lom_namespace + "context>");
                    else
                        Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "context source=\"" + Convert_String_To_XML_Safe(thisContext.Source) + "\">" + Convert_String_To_XML_Safe(thisContext.Value) + "</" + lom_namespace + "context>");
                }

                // Add the typical age range information(s)
                if ( lomInfo.TypicalAgeRanges.Count > 0 )
                {
                    Output_Stream.WriteLine( indent + indent + "<" + lom_namespace + "typicalagerange>");

                    foreach( LOM_LanguageString thisRange in lomInfo.TypicalAgeRanges )
                    {
                        if (String.IsNullOrWhiteSpace(thisRange.Language))
                            Output_Stream.WriteLine(indent + indent + indent +"<" + lom_namespace + "langstring>" + Convert_String_To_XML_Safe(thisRange.Value) + "</" + lom_namespace +  "langstring>");
                        else
                            Output_Stream.WriteLine(indent + indent + indent + "<" + lom_namespace + "langstring lang=\"" + Convert_String_To_XML_Safe(thisRange.Language) + "\">" + Convert_String_To_XML_Safe(thisRange.Value) + "</" + lom_namespace + "langstring>");
                    }

                    Output_Stream.WriteLine( indent + indent + "</" + lom_namespace + "typicalagerange>");
                }

                // Add the difficulty inforamtion
                if (lomInfo.DifficultyLevel != DifficultyLevelEnum.UNDEFINED)
                {
                    switch (lomInfo.DifficultyLevel)
                    {
                        case DifficultyLevelEnum.very_difficult:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "difficulty>very difficult</" + lom_namespace + "difficulty>");
                            break;

                        case DifficultyLevelEnum.difficult:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "difficulty>difficult</" + lom_namespace + "difficulty>");
                            break;

                        case DifficultyLevelEnum.medium:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "difficulty>medium</" + lom_namespace + "difficulty>");
                            break;

                        case DifficultyLevelEnum.easy:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "difficulty>easy</" + lom_namespace + "difficulty>");
                            break;

                        case DifficultyLevelEnum.very_easy:
                            Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "difficulty>very easy</" + lom_namespace + "difficulty>");
                            break;
                    }
                }

                // Add the typical learning time
                if ( !String.IsNullOrWhiteSpace( lomInfo.TypicalLearningTime ))
                {
                    Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "typicallearningtime>");
                    Output_Stream.WriteLine(indent + indent + indent + "<" + lom_namespace + "duration>" + Convert_String_To_XML_Safe(lomInfo.TypicalLearningTime) + "</" + lom_namespace + "duration>");
                    Output_Stream.WriteLine(indent + indent + "</" + lom_namespace + "typicallearningtime>");
                }

                Output_Stream.WriteLine(indent + "</" + lom_namespace + "educational>");
            }

            // Add the classification information (repeatable)
            foreach( LOM_Classification thisClassification in lomInfo.Classifications )
            {
                Output_Stream.WriteLine(indent + "<" + lom_namespace + "classification>");

                // Add the optional purpose field
                if ( !String.IsNullOrWhiteSpace( thisClassification.Purpose.Value ))
                {
                    if ( !String.IsNullOrWhiteSpace( thisClassification.Purpose.Source ))
                        Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "purpose source=\"" + Convert_String_To_XML_Safe(thisClassification.Purpose.Source) + "\">" + Convert_String_To_XML_Safe( thisClassification.Purpose.Value ) + "</" + lom_namespace + "purpose>");
                    else
                        Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "purpose>" + Convert_String_To_XML_Safe(thisClassification.Purpose.Value) + "</" + lom_namespace + "purpose>");
                }

                // Add the taxon path(s)
                foreach( LOM_TaxonPath thisPath in thisClassification.TaxonPaths )
                {
                    Output_Stream.WriteLine(indent + indent + "<" + lom_namespace + "taxonpath>" );

                    // Add the source?
                    if ( thisPath.SourceNames.Count > 0 )
                    {
                        Output_Stream.WriteLine( indent + indent + indent + "<" + lom_namespace + "source>");

                        // Add the sources
                        foreach( LOM_LanguageString thisSource in thisPath.SourceNames )
                        {
                            if (String.IsNullOrWhiteSpace(thisSource.Language))
                                Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "langstring>" + Convert_String_To_XML_Safe(thisSource.Value) + "</" + lom_namespace + "langstring>");
                            else
                                Output_Stream.WriteLine(indent + indent + indent + indent + "<" + lom_namespace + "langstring lang=\"" + Convert_String_To_XML_Safe(thisSource.Language) + "\">" + Convert_String_To_XML_Safe(thisSource.Value) + "</" + lom_namespace + "langstring>");
                        }

                        Output_Stream.WriteLine( indent + indent + indent + "</" + lom_namespace + "source>");
                    }

                    // Add all the taxons
                    foreach( LOM_Taxon thisTaxon in thisPath.Taxons )
                    {
                        Output_Stream.WriteLine(indent + indent + indent + "<" + lom_namespace + "taxon>" );

                        // Add the ID
                        if ( !String.IsNullOrWhiteSpace( thisTaxon.ID ))
                        {
                            Output_Stream.WriteLine( indent + indent + indent + indent + "<" + lom_namespace + "id>" + Convert_String_To_XML_Safe( thisTaxon.ID) + "</" + lom_namespace + "id>");
                        }

                        // Add the entries
                        if ( thisTaxon.Entries.Count > 0 )
                        {
                            Output_Stream.WriteLine( indent + indent + indent + indent + "<" + lom_namespace + "entry>");

                            // Add the sources
                            foreach( LOM_LanguageString thisSource in thisTaxon.Entries )
                            {
                                if (String.IsNullOrWhiteSpace(thisSource.Language))
                                    Output_Stream.WriteLine(indent + indent + indent + indent + indent + indent + "<" + lom_namespace + "langstring>" + Convert_String_To_XML_Safe(thisSource.Value) + "</" + lom_namespace + "langstring>");
                                else
                                    Output_Stream.WriteLine(indent + indent + indent + indent + indent + indent + "<" + lom_namespace + "langstring lang=\"" + Convert_String_To_XML_Safe(thisSource.Language) + "\">" + Convert_String_To_XML_Safe(thisSource.Value) + "</" + lom_namespace + "langstring>");
                            }

                            Output_Stream.WriteLine( indent + indent + indent + indent + "</" + lom_namespace + "entry>");
                        }

                        Output_Stream.WriteLine(indent + indent + indent + "</" + lom_namespace + "taxon>" );
                    }

                    Output_Stream.WriteLine(indent + indent + "</" + lom_namespace + "taxonpath>" );
                }

                Output_Stream.WriteLine(indent + "</" + lom_namespace + "classification>");
            }

            // Finish the main lom node
            Output_Stream.WriteLine( "</" + lom_namespace + "lom>");
        
            return true;
        }

        #endregion

        #region Method(s) to read the learning object metadata from LOM-IEEE XML format from a dmdSec

        /// <summary> Reads the dmdSec at the current position in the XmlTextReader and associates it with the 
        /// entire package  </summary>
        /// <param name="Input_XmlReader"> Open XmlReader from which to read the metadata </param>
        /// <param name="Return_Package"> Package into which to read the metadata</param>
        /// <param name="Options"> Dictionary of any options which this METS section reader may utilize</param>
        /// <returns> TRUE if successful, otherwise FALSE</returns>
        public bool Read_dmdSec(XmlReader Input_XmlReader, SobekCM_Item Return_Package, Dictionary<string, object> Options)
        {
            // Ensure this metadata module extension exists
            LearningObjectMetadata lomInfo = Return_Package.Get_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY) as LearningObjectMetadata;
            if (lomInfo == null)
            {
                lomInfo = new LearningObjectMetadata();
                Return_Package.Add_Metadata_Module(GlobalVar.IEEE_LOM_METADATA_MODULE_KEY, lomInfo);
            }

            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && ((Input_XmlReader.Name == "METS:mdWrap") || (Input_XmlReader.Name == "mdWrap")))
                    return true;

                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if (( lom_namespace.Length > 0 ) && ( name.IndexOf( lom_namespace ) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        case "general":
                            read_general(Input_XmlReader.ReadSubtree(), lomInfo);
                            break;
                            
                        case "lifecycle":
                            read_lifecycle(Input_XmlReader.ReadSubtree(), lomInfo);
                            break;

                        case "technical":
                            read_technical(Input_XmlReader.ReadSubtree(), lomInfo);
                            break;

                        case "educational":
                            read_educational(Input_XmlReader.ReadSubtree(), lomInfo);
                            break;

                        case "classification":
                            read_classification(Input_XmlReader.ReadSubtree(), lomInfo);
                            break;
                       

                    }
                }


            } while (Input_XmlReader.Read());

            return true;
        }

        private void read_general( XmlReader Input_XmlReader, LearningObjectMetadata lomInfo )
        {
            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if ((lom_namespace.Length > 0) && (name.IndexOf(lom_namespace) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        case "aggregationlevel":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string aggregationLevelTemp = Input_XmlReader.Value.Trim();
                                switch (aggregationLevelTemp)
                                {
                                    case "1":
                                        lomInfo.AggregationLevel = AggregationLevelEnum.level1;
                                        break;

                                    case "2":
                                        lomInfo.AggregationLevel = AggregationLevelEnum.level2;
                                        break;

                                    case "3":
                                        lomInfo.AggregationLevel = AggregationLevelEnum.level3;
                                        break;

                                    case "4":
                                        lomInfo.AggregationLevel = AggregationLevelEnum.level4;
                                        break;
                                }
                            }
                            break;
                    }
                }
            } while (Input_XmlReader.Read());
        }


        private void read_lifecycle(XmlReader Input_XmlReader, LearningObjectMetadata lomInfo)
        {
            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if ((lom_namespace.Length > 0) && (name.IndexOf(lom_namespace) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        case "status":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string statusTemp = Input_XmlReader.Value.Trim();
                                switch (statusTemp)
                                {
                                    case "draft":
                                        lomInfo.Status = StatusEnum.draft;
                                        break;

                                    case "final":
                                        lomInfo.Status = StatusEnum.final;
                                        break;

                                    case "revised":
                                        lomInfo.Status = StatusEnum.revised;
                                        break;

                                    case "unavailable":
                                        lomInfo.Status = StatusEnum.unavailable;
                                        break;
                                }
                            }
                            break;
                    }
                }
            } while (Input_XmlReader.Read());
        }


        private void read_technical(XmlReader Input_XmlReader, LearningObjectMetadata lomInfo)
        {
            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if ((lom_namespace.Length > 0) && (name.IndexOf(lom_namespace) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        //case "requirement":
                        case "orcomposite":
                            read_requirement(Input_XmlReader.ReadSubtree(), lomInfo);
                            break;
                    }
                }
            } while (Input_XmlReader.Read());
        }

        private void read_requirement(XmlReader Input_XmlReader, LearningObjectMetadata lomInfo)
        {
            LOM_System_Requirements requirement = new LOM_System_Requirements();

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if ((lom_namespace.Length > 0) && (name.IndexOf(lom_namespace) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        case "type":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string typeTemp = Input_XmlReader.Value.ToLower().Trim();
                                switch( typeTemp )
                                {
                                    case "operating system":
                                        requirement.RequirementType = RequirementTypeEnum.operating_system;
                                        break;

                                    case "browser":
                                        requirement.RequirementType = RequirementTypeEnum.browser;
                                        break;

                                    case "software":
                                        requirement.RequirementType = RequirementTypeEnum.software;
                                        break;

                                    case "hardware":
                                        requirement.RequirementType = RequirementTypeEnum.hardware;
                                        break;
                                }
                            }
                            break;

                        case "name":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                requirement.Name.Value = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "minimumversion":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                requirement.MinimumVersion = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "maximumversion":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                requirement.MaximumVersion = Input_XmlReader.Value.Trim();
                            }
                            break;
                    }                   
                }
            } while (Input_XmlReader.Read());

            // Was a good requirement created?
            if ((requirement.RequirementType != RequirementTypeEnum.UNDEFINED) && (!String.IsNullOrWhiteSpace(requirement.Name.Value)))
                lomInfo.Add_SystemRequirements(requirement);
        }


        private void read_educational(XmlReader Input_XmlReader, LearningObjectMetadata lomInfo)
        {
            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if (( lom_namespace.Length > 0 ) && ( name.IndexOf( lom_namespace ) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        //case "requirement":
                        case "interactivitytype":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string interactivityTemp = Input_XmlReader.Value.Trim();
                                switch( interactivityTemp )
                                {
                                    case "active":
                                        lomInfo.InteractivityType = InteractivityTypeEnum.active;
                                        break;

                                    case "expositive":
                                        lomInfo.InteractivityType = InteractivityTypeEnum.expositive;
                                        break;

                                    case "mixed":
                                        lomInfo.InteractivityType = InteractivityTypeEnum.mixed;
                                        break;
                                }
                            }
                            break;

                        case "learningresourcetype":
                            LOM_VocabularyState learningResourceType = new LOM_VocabularyState();
                            if (Input_XmlReader.MoveToAttribute("source"))
                                learningResourceType.Source = Input_XmlReader.Value.Trim();
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                learningResourceType.Value = Input_XmlReader.Value.Trim();
                                lomInfo.Add_LearningResourceType(learningResourceType);
                            }
                            break;

                        case "interactivitylevel":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string interactivityLevelTemp = Input_XmlReader.Value.ToLower().Trim();
                                switch( interactivityLevelTemp )
                                {
                                    case "very low":
                                        lomInfo.InteractivityLevel = InteractivityLevelEnum.very_low;
                                        break;

                                    case "low":
                                        lomInfo.InteractivityLevel = InteractivityLevelEnum.low;
                                        break;

                                    case "medium":
                                        lomInfo.InteractivityLevel = InteractivityLevelEnum.medium;
                                        break;

                                    case "high":
                                        lomInfo.InteractivityLevel = InteractivityLevelEnum.high;
                                        break;

                                    case "very high":
                                        lomInfo.InteractivityLevel = InteractivityLevelEnum.very_high;
                                        break;
                                }
                            }
                            break;

                        case "intendedenduserrole":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string endUserRoleTemp = Input_XmlReader.Value.ToLower().Trim();
                                switch (endUserRoleTemp)
                                {
                                    case "teacher":
                                        lomInfo.Add_IntendedEndUserRole( IntendedEndUserRoleEnum.teacher);
                                        break;

                                    case "author":
                                        lomInfo.Add_IntendedEndUserRole( IntendedEndUserRoleEnum.author);
                                        break;

                                    case "learner":
                                        lomInfo.Add_IntendedEndUserRole( IntendedEndUserRoleEnum.learner);
                                        break;

                                    case "manager":
                                        lomInfo.Add_IntendedEndUserRole( IntendedEndUserRoleEnum.manager);
                                        break;
                                }
                            }
                            break;

                        case "context":
                            LOM_VocabularyState context = new LOM_VocabularyState();
                            if (Input_XmlReader.MoveToAttribute("source"))
                                context.Source = Input_XmlReader.Value.Trim();
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                context.Value = Input_XmlReader.Value.Trim();
                                lomInfo.Add_Context(context);
                            }
                            break;

                        case "typicalagerange":
                            // Need to gather all the language strings here
                            do
                            {
                                string subname = Input_XmlReader.Name.ToLower();
                                if (( lom_namespace.Length > 0 ) && ( subname.IndexOf( lom_namespace ) == 0))
                                    subname = subname.Substring(lom_namespace.Length);
                                
                                // Stop when done looping through the language strings
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (subname == "typicalagerange"))
                                    break;

                                // Start of a new language string?
                                if (( Input_XmlReader.NodeType == XmlNodeType.Element ) && ( subname == "langstring" ))
                                {
                                    LOM_LanguageString agerange = new LOM_LanguageString();
                                    if (Input_XmlReader.MoveToAttribute("lang"))
                                        agerange.Language = Input_XmlReader.Value.Trim(); 
                                    Input_XmlReader.Read();
                                    if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                                    {
                                        agerange.Value = Input_XmlReader.Value.Trim(); 
                                        lomInfo.Add_TypicalAgeRange(agerange);
                                    }
                                }
                            } while (Input_XmlReader.Read());
                            break;

                        case "difficulty":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                string difficultyTemp = Input_XmlReader.Value.ToLower().Trim();
                                switch (difficultyTemp)
                                {
                                    case "very easy":
                                        lomInfo.DifficultyLevel = DifficultyLevelEnum.very_easy;
                                        break;

                                    case "easy":
                                        lomInfo.DifficultyLevel = DifficultyLevelEnum.easy;
                                        break;

                                    case "medium":
                                        lomInfo.DifficultyLevel = DifficultyLevelEnum.medium;
                                        break;

                                    case "difficult":
                                        lomInfo.DifficultyLevel = DifficultyLevelEnum.difficult;
                                        break;

                                    case "very difficult":
                                        lomInfo.DifficultyLevel = DifficultyLevelEnum.very_difficult;
                                        break;
                                }
                            }
                            break;

                        case "typicallearningtime":
                            // Need to gather all the language strings here
                            do
                            {
                                string subname = Input_XmlReader.Name.ToLower();
                                if ((lom_namespace.Length > 0) && (subname.IndexOf(lom_namespace) == 0))
                                    subname = subname.Substring(lom_namespace.Length);

                                // Stop when done looping through the language strings
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (subname == "typicallearningtime"))
                                    break;

                                // Start of a new language string?
                                if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (subname == "duration"))
                                {
                                    Input_XmlReader.Read();
                                    if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                                    {
                                        lomInfo.TypicalLearningTime = Input_XmlReader.Value.Trim();
                                    }
                                }
                            } while (Input_XmlReader.Read());
                            break;
                    }
                }
            } while (Input_XmlReader.Read());
        }

        private void read_classification(XmlReader Input_XmlReader, LearningObjectMetadata lomInfo)
        {
            // Create the classification object
            LOM_Classification classification = new LOM_Classification();

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if ((lom_namespace.Length > 0) && (name.IndexOf(lom_namespace) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        case "purpose":
                            if (Input_XmlReader.MoveToAttribute("source"))
                                classification.Purpose.Source = Input_XmlReader.Value.Trim();
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                classification.Purpose.Value = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "taxonpath":
                            read_taxonpath(Input_XmlReader.ReadSubtree(), classification);
                            break;

                    }
                }
            } while (Input_XmlReader.Read());

            // Double check that there is some taxonomies here before adding this to the resource object
            bool added = false;
            foreach (LOM_TaxonPath thisPath in classification.TaxonPaths)
            {
                foreach (LOM_Taxon thisTaxon in thisPath.Taxons)
                {
                    if ((thisTaxon.ID.Length > 0) || (thisTaxon.Entries.Count > 0) && (thisTaxon.Entries[0].Value.Length > 0))
                    {
                        added = true;
                        lomInfo.Add_Classification(classification);
                        break;
                    }
                }
                if (added) break;
            }
        }

        private void read_taxonpath(XmlReader Input_XmlReader, LOM_Classification classification)
        {
            // Create the taxon path object
            LOM_TaxonPath taxonPath = new LOM_TaxonPath();

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if ((lom_namespace.Length > 0) && (name.IndexOf(lom_namespace) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        case "source":
                            // Need to gather all the language strings here
                            do
                            {
                                string subname = Input_XmlReader.Name.ToLower();
                                if ((lom_namespace.Length > 0) && (subname.IndexOf(lom_namespace) == 0))
                                    subname = subname.Substring(lom_namespace.Length);

                                // Stop when done looping through the language strings
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (subname == "source"))
                                    break;

                                // Start of a new language string?
                                if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (subname == "langstring"))
                                {
                                    LOM_LanguageString source = new LOM_LanguageString();
                                    if (Input_XmlReader.MoveToAttribute("lang"))
                                        source.Language = Input_XmlReader.Value.Trim();
                                    Input_XmlReader.Read();
                                    if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                                    {
                                        source.Value = Input_XmlReader.Value.Trim();
                                        taxonPath.Add_SourceName(source);
                                    }
                                }
                            } while (Input_XmlReader.Read());
                            break;

                        case "taxon":
                            read_taxon(Input_XmlReader.ReadSubtree(), taxonPath);
                            break;

                    }
                }
            } while (Input_XmlReader.Read());

            // Do a little checking here before adding it
            if ( taxonPath.Taxons.Count > 0 )
                classification.Add_TaxonPath(taxonPath);
        }

        private void read_taxon(XmlReader Input_XmlReader, LOM_TaxonPath taxonPath)
        {
            // Create the taxon path object
            LOM_Taxon taxon = new LOM_Taxon();

            // Loop through reading each XML node
            do
            {
                // get the right division information based on node type
                if (Input_XmlReader.NodeType == XmlNodeType.Element)
                {
                    string name = Input_XmlReader.Name.ToLower();
                    if ((lom_namespace.Length > 0) && (name.IndexOf(lom_namespace) == 0))
                        name = name.Substring(lom_namespace.Length);

                    switch (name)
                    {
                        case "id":
                            Input_XmlReader.Read();
                            if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                            {
                                taxon.ID = Input_XmlReader.Value.Trim();
                            }
                            break;

                        case "entry":
                            // Need to gather all the language strings here
                            do
                            {
                                string subname = Input_XmlReader.Name.ToLower();
                                if ((lom_namespace.Length > 0) && (subname.IndexOf(lom_namespace) == 0))
                                    subname = subname.Substring(lom_namespace.Length);

                                // Stop when done looping through the language strings
                                if ((Input_XmlReader.NodeType == XmlNodeType.EndElement) && (subname == "entry"))
                                    break;

                                // Start of a new language string?
                                if ((Input_XmlReader.NodeType == XmlNodeType.Element) && (subname == "langstring"))
                                {
                                    LOM_LanguageString entry = new LOM_LanguageString();
                                    if (Input_XmlReader.MoveToAttribute("lang"))
                                        entry.Language = Input_XmlReader.Value.Trim();
                                    Input_XmlReader.Read();
                                    if ((Input_XmlReader.NodeType == XmlNodeType.Text) && (Input_XmlReader.Value.Trim().Length > 0))
                                    {
                                        entry.Value = Input_XmlReader.Value.Trim();
                                        taxon.Add_Entry(entry);
                                    }
                                }
                            } while (Input_XmlReader.Read());
                            break;
                    }
                }
            } while (Input_XmlReader.Read());

            // Do a little checking here before adding it
            if ((taxon.ID.Length > 0 ) || ( taxon.Entries.Count > 0 ))
                taxonPath.Add_Taxon(taxon);
        }


        #endregion
    }
}
