#region Using directives

using System;
using System.Reflection;
using SobekCM.Core.UI_Configuration.TemplateElements;
using SobekCM.Library.Citation.Elements.implemented_elements;
using SobekCM.Library.UI;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Factory which generates each element object, depending on the provided element type and subtype </summary>
    public class Element_Factory
    {
        /// <summary> Gets the element object associated with the provided type </summary>
        /// <param name="Type"> Type for the element to retrieve </param>
        /// <returns> Correct element object which implements the <see cref="abstract_Element"/> class. </returns>
        public static abstract_Element getElement( string Type )
        {
           return getElement( Type, String.Empty );
        }

        /// <summary> Gets the element object associated with the provided type and subtype </summary>
        /// <param name="Type"> Type for the element to retrieve </param>
        /// <param name="SubType"> Subtype for the element to retrieve </param>
        /// <returns> Correct element object which implements the <see cref="abstract_Element"/> class. </returns>
        public static abstract_Element getElement(string Type, string SubType)
        {
            // Do a lookup for the basic configuration
            TemplateElementConfig config = UI_ApplicationCache_Gateway.Configuration.UI.TemplateElements.Get_Element_Configuration(Type.Replace("_", " "), SubType);

            // If this was null, there is no match in this system
            if (config == null)
                return null;

            // If there was no assembly listed, try to find a match in the existing template elements
            if (String.IsNullOrEmpty(config.Assembly))
            {
                // Was a namespace not included?  All elements in the base assemblies should have one
                string className = (config.Class.IndexOf(".") < 0) ? "SobekCM.Library.Citation.Elements." + config.Class : config.Class;

                // Look for a standard match
                switch (className)
                {
                    case "SobekCM.Library.Citation.Elements.Abstract_Complex_Element":
                        return new Abstract_Complex_Element();
                    case "SobekCM.Library.Citation.Elements.Abstract_Summary_Element":
                        return new Abstract_Summary_Element();
                    case "SobekCM.Library.Citation.Elements.Acquisition_Note_Element":
                        return new Acquisition_Note_Element();
                    case "SobekCM.Library.Citation.Elements.Additional_Work_Needed_Element":
                        return new Additional_Work_Needed_Element();
                    case "SobekCM.Library.Citation.Elements.Aggregations_Element":
                        return new Aggregations_Element();
                    case "SobekCM.Library.Citation.Elements.Attribution_Element":
                        return new Attribution_Element();
                    case "SobekCM.Library.Citation.Elements.BibID_Element":
                        return new BibID_Element();
                    case "SobekCM.Library.Citation.Elements.Born_Digital_Element":
                        return new Born_Digital_Element();
                    case "SobekCM.Library.Citation.Elements.Catalog_Record_Number_Element":
                        return new Catalog_Record_Number_Element();
                    case "SobekCM.Library.Citation.Elements.CitationSet_Element":
                        return new CitationSet_Element();
                    case "SobekCM.Library.Citation.Elements.Classification_Element":
                        return new Classification_Element();
                    case "SobekCM.Library.Citation.Elements.Container_Element":
                        return new Container_Element();
                    case "SobekCM.Library.Citation.Elements.Contributor_Element":
                        return new Contributor_Element();
                    case "SobekCM.Library.Citation.Elements.Coordinates_Point_Element":
                        return new Coordinates_Point_Element();
                    case "SobekCM.Library.Citation.Elements.Creator_Element":
                        return new Creator_Element();
                    case "SobekCM.Library.Citation.Elements.Creator_Complex_Element":
                        return new Creator_Complex_Element();
                    case "SobekCM.Library.Citation.Elements.Creator_Fixed_Role_Element":
                        return new Creator_Fixed_Role_Element();
                    case "SobekCM.Library.Citation.Elements.Name_Form_Element":
                        return new Name_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Creator_Notes_Element":
                        return new Creator_Notes_Element();
                    case "SobekCM.Library.Citation.Elements.Dark_Flag_Element":
                        return new Dark_Flag_Element();
                    case "SobekCM.Library.Citation.Elements.Date_Element":
                        return new Date_Element();
                    case "SobekCM.Library.Citation.Elements.Date_Copyrighted_Element":
                        return new Date_Copyrighted_Element();
                    case "SobekCM.Library.Citation.Elements.Description_Standard_Element":
                        return new Description_Standard_Element();
                    case "SobekCM.Library.Citation.Elements.Disposition_Advice_Element":
                        return new Disposition_Advice_Element();
                    case "SobekCM.Library.Citation.Elements.Donor_Element":
                        return new Donor_Element();
                    case "SobekCM.Library.Citation.Elements.EAD_Form_Element":
                        return new EAD_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Edition_Element":
                        return new Edition_Element();
                    case "SobekCM.Library.Citation.Elements.EmbeddedVideo_Element":
                        return new EmbeddedVideo_Element();
                    case "SobekCM.Library.Citation.Elements.Encoding_Level_Element":
                        return new Encoding_Level_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_CommitteeChair_Element":
                        return new ETD_CommitteeChair_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_CommitteeCoChair_Element":
                        return new ETD_CommitteeCoChair_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_CommitteeMember_Element":
                        return new ETD_CommitteeMember_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_Degree_Element":
                        return new ETD_Degree_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_DegreeDiscipline_Element":
                        return new ETD_DegreeDiscipline_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_DegreeDivision_Element":
                        return new ETD_DegreeDivision_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_DegreeGrantor_Element":
                        return new ETD_DegreeGrantor_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_DegreeLevel_Element":
                        return new ETD_DegreeLevel_Element();
                    case "SobekCM.Library.Citation.Elements.ETD_GraduationSemester_Element":
                        return new ETD_GraduationSemester_Element();
                    case "SobekCM.Library.Citation.Elements.FAST_Subject_Element":
                        return new FAST_Subject_Element();
                    case "SobekCM.Library.Citation.Elements.Format_Element":
                        return new Format_Element();
                    case "SobekCM.Library.Citation.Elements.Frequency_Element":
                        return new Frequency_Element();
                    case "SobekCM.Library.Citation.Elements.Genre_Element":
                        return new Genre_Element();
                    case "SobekCM.Library.Citation.Elements.Group_Title_Element":
                        return new Group_Title_Element();
                    case "SobekCM.Library.Citation.Elements.Holding_Element":
                        return new Holding_Element();
                    case "SobekCM.Library.Citation.Elements.Identifier_Element":
                        return new Identifier_Element();
                    case "SobekCM.Library.Citation.Elements.Identifier_Fixed_Type_Element":
                        return new Identifier_Fixed_Type_Element();
                    case "SobekCM.Library.Citation.Elements.Language_Element":
                        return new Language_Element();
                    case "SobekCM.Library.Citation.Elements.Language_Select_Element":
                        return new Language_Select_Element();
                    case "SobekCM.Library.Citation.Elements.LCCN_Element":
                        return new LCCN_Element();
                    case "SobekCM.Library.Citation.Elements.Literal_Element":
                        return new Literal_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_AggregationLevel_Element":
                        return new LOM_AggregationLevel_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_Context_Element":
                        return new LOM_Context_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_DifficultyLevel_Element":
                        return new LOM_DifficultyLevel_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_IntendedUser_Element":
                        return new LOM_IntendedUser_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_InteractivityLevel_Element":
                        return new LOM_InteractivityLevel_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_InteractivityType_Element":
                        return new LOM_InteractivityType_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_ResourceType_Element":
                        return new LOM_ResourceType_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_Status_Element":
                        return new LOM_Status_Element();
                    case "SobekCM.Library.Citation.Elements.LOM_TypicalAgeRange_Element":
                        return new LOM_TypicalAgeRange_Element();
                    case "SobekCM.Library.Citation.Elements.Main_Thumbnail_Element":
                        return new Main_Thumbnail_Element();
                    case "SobekCM.Library.Citation.Elements.Manufacturer_Complex_Element":
                        return new Manufacturer_Complex_Element();
                    case "SobekCM.Library.Citation.Elements.Manufacturer_Element":
                        return new Manufacturer_Element();
                    case "SobekCM.Library.Citation.Elements.Material_Received_Date":
                        return new Material_Received_Date();
                    case "SobekCM.Library.Citation.Elements.Note_Complex_Element":
                        return new Note_Complex_Element();
                    case "SobekCM.Library.Citation.Elements.Note_Element":
                        return new Note_Element();
                    case "SobekCM.Library.Citation.Elements.OCLC_Record_Number_Element":
                        return new OCLC_Record_Number_Element();
                    case "SobekCM.Library.Citation.Elements.Other_URL_Form_Element":
                        return new Other_URL_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Other_URL_Element":
                        return new Other_URL_Element();
                    case "SobekCM.Library.Citation.Elements.Primary_Alt_Identifier_Element":
                        return new Primary_Alt_Identifier_Element();
                    case "SobekCM.Library.Citation.Elements.Projects_Element":
                        return new Projects_Element();
                    case "SobekCM.Library.Citation.Elements.Publication_Status_Element":
                        return new Publication_Status_Element();
                    case "SobekCM.Library.Citation.Elements.Publication_Place_Element":
                        return new Publication_Place_Element();
                    case "SobekCM.Library.Citation.Elements.Publisher_Complex_Element":
                        return new Publisher_Complex_Element();
                    case "SobekCM.Library.Citation.Elements.Publisher_Element":
                        return new Publisher_Element();
                    case "SobekCM.Library.Citation.Elements.Record_Origin_Element":
                        return new Record_Origin_Element();
                    case "SobekCM.Library.Citation.Elements.RecordStatus_Element":
                        return new RecordStatus_Element();
                    case "SobekCM.Library.Citation.Elements.Related_Item_Form_Element":
                        return new Related_Item_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Rights_Element":
                        return new Rights_Element();
                    case "SobekCM.Library.Citation.Elements.IR_Rights_Element":
                        return new IR_Rights_Element();
                    case "SobekCM.Library.Citation.Elements.Scale_Element":
                        return new Scale_Element();
                    case "SobekCM.Library.Citation.Elements.Serial_Hierarchy_Form_Element":
                        return new Serial_Hierarchy_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Serial_Hierarchy_Panel_Element":
                        return new Serial_Hierarchy_Panel_Element();
                    case "SobekCM.Library.Citation.Elements.Source_Element":
                        return new Source_Element();
                    case "SobekCM.Library.Citation.Elements.Hierarchical_Spatial_Form_Element":
                        return new Hierarchical_Spatial_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Spatial_Coverage_Element":
                        return new Spatial_Coverage_Element();
                    case "SobekCM.Library.Citation.Elements.Subject_Keyword_Standard_Form_Element":
                        return new Subject_Keyword_Standard_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Subject_Scheme_Element":
                        return new Subject_Scheme_Element();
                    case "SobekCM.Library.Citation.Elements.Subject_Element":
                        return new Subject_Element();
                    case "SobekCM.Library.Citation.Elements.SuDOC_Element":
                        return new SuDOC_Element();
                    case "SobekCM.Library.Citation.Elements.Target_Audience_Element":
                        return new Target_Audience_Element();
                    case "SobekCM.Library.Citation.Elements.Temporal_Complex_Element":
                        return new Temporal_Complex_Element();
                    case "SobekCM.Library.Citation.Elements.Temporal_Coverage_Element":
                        return new Temporal_Coverage_Element();
                    case "SobekCM.Library.Citation.Elements.Tickler_Element":
                        return new Tickler_Element();
                    case "SobekCM.Library.Citation.Elements.Title_Main_Form_Element":
                        return new Title_Main_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Title_Main_Element":
                        return new Title_Main_Element();
                    case "SobekCM.Library.Citation.Elements.Other_Title_Form_Element":
                        return new Other_Title_Form_Element();
                    case "SobekCM.Library.Citation.Elements.Other_Title_Element":
                        return new Other_Title_Element();
                    case "SobekCM.Library.Citation.Elements.Tracking_Box_Element":
                        return new Tracking_Box_Element();
                    case "SobekCM.Library.Citation.Elements.Type_Format_Form_Element":
                        return new Type_Format_Form_Element();
                    case "SobekCM.Library.Citation.Elements.IR_Type_Element":
                        return new IR_Type_Element();
                    case "SobekCM.Library.Citation.Elements.Type_Element":
                        return new Type_Element();
                    case "SobekCM.Library.Citation.Elements.VID_Element":
                        return new VID_Element();
                    case "SobekCM.Library.Citation.Elements.Viewer_Element":
                        return new Viewer_Element();
                    case "SobekCM.Library.Citation.Elements.Visibility_Element":
                        return new Visibility_Element();
                    case "SobekCM.Library.Citation.Elements.VRA_CulturalContext_Element":
                        return new VRA_CulturalContext_Element();
                    case "SobekCM.Library.Citation.Elements.VRA_Inscription_Element":
                        return new VRA_Inscription_Element();
                    case "SobekCM.Library.Citation.Elements.VRA_Material_Element":
                        return new VRA_Material_Element();
                    case "SobekCM.Library.Citation.Elements.VRA_Measurement_Element":
                        return new VRA_Measurement_Element();
                    case "SobekCM.Library.Citation.Elements.VRA_StateEdition_Element":
                        return new VRA_StateEdition_Element();
                    case "SobekCM.Library.Citation.Elements.VRA_StylePeriod_Element":
                        return new VRA_StylePeriod_Element();
                    case "SobekCM.Library.Citation.Elements.VRA_Technique_Element":
                        return new VRA_Technique_Element();
                    case "SobekCM.Library.Citation.Elements.Web_Skin_Element":
                        return new Web_Skin_Element();
                    case "SobekCM.Library.Citation.Elements.Wordmark_Element":
                        return new Wordmark_Element();
                    case "SobekCM.Library.Citation.Elements.Zoological_Taxonomy_Form_Element":
                        return new Zoological_Taxonomy_Form_Element();
                }

                // If it made it here, there is no assembly, but it is an unexpected type.  
                // Just create it from the same assembly then
                try
                {
                    Assembly dllAssembly = Assembly.GetCallingAssembly();
                    Type elementType = dllAssembly.GetType(config.Class);
                    abstract_Element returnObj = (abstract_Element) Activator.CreateInstance(elementType);
                    return returnObj;
                }
                catch (Exception)
                {
                    // Not sure exactly what to do here, honestly
                    return null;
                }
            }

            // An assembly was indicated

            try
            {
                Assembly dllAssembly = Assembly.LoadFrom(config.Assembly);
                Type elementType = dllAssembly.GetType(config.Class);
                abstract_Element returnObj = (abstract_Element) Activator.CreateInstance(elementType);
                return returnObj;
            }
            catch (Exception)
            {
                // Not sure exactly what to do here, honestly
                return null;
            }
        }
    }
}
