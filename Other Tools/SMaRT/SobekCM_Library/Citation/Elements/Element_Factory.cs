#region Using directives

using System;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Factory which generates each element object, depending on the provided element type and subtype </summary>
    public class Element_Factory
    {
        /// <summary> Gets the element object associated with the provided type </summary>
        /// <param name="Type"> Type for the element to retrieve </param>
        /// <returns>Correct element object which implements the <see cref="abstract_Element"/> class. </returns>
        public static abstract_Element getElement( string Type )
        {
            return getElement( Element_Type_Convertor.ToType( Type ), String.Empty );
        }

        /// <summary> Gets the element object associated with the provided type and subtype </summary>
        /// <param name="Type"> Type for the element to retrieve </param>
        /// <param name="SubType"> Subtype for the element to retrieve </param>
        /// <returns>Correct element object which implements the <see cref="abstract_Element"/> class. </returns>
        public static abstract_Element getElement( string Type, string SubType )
        {
            return getElement( Element_Type_Convertor.ToType( Type ), SubType );
        }

        /// <summary> Gets the element object associated with the provided type and subtype </summary>
        /// <param name="Type"> Type for the element to retrieve </param>
        /// <param name="SubType"> Subtype for the element to retrieve </param>
        /// <returns>Correct element object which implements the <see cref="abstract_Element"/> class. </returns>
        public static abstract_Element getElement( Element_Type Type, string SubType )
        {
            switch( Type )
            {
                case Element_Type.Abstract:
                    if (SubType == "simple")
                        return new Abstract_Summary_Element();
                    return new Abstract_Complex_Element();

                case Element_Type.Acquisition:
                    return new Acquisition_Note_Element();

                case Element_Type.Additional_Work_Needed:
                    return new Additional_Work_Needed_Element();

                case Element_Type.Aggregations:
                    return new Aggregations_Element();

                case Element_Type.Attribution:
                    return new Attribution_Element();

                case Element_Type.BibID:
                    return new BibID_Element();

                case Element_Type.Born_Digital:
                    return new Born_Digital_Element();

                case Element_Type.Catalog_Record_Number:
                    return new Catalog_Record_Number_Element();

                case Element_Type.Classification:
                    return new Classification_Element();

                case Element_Type.Container:
                    return new Container_Element();

                case Element_Type.Contributor:
                    return new Contributor_Element();

                case Element_Type.Coordinates:
                    return new Coordinates_Point_Element();

                case Element_Type.Creator:
                    switch (SubType)
                    {
                        case "simple":
                            return new Creator_Element();
                        case "complex":
                            return new Creator_Complex_Element();
                        case "fixed_role":
                            return new Creator_Fixed_Role_Element();
                        default:
                            return new Name_Form_Element();
                    }

                case Element_Type.CreatorNotes:
                    return new Creator_Notes_Element();

                case Element_Type.Dark_Flag:
                    return new Dark_Flag_Element();

                case Element_Type.Date:
                    return new Date_Element();

                case Element_Type.DateCopyrighted:
                    return new Date_Copyrighted_Element();

                case Element_Type.DescriptionStandard:
                    return new Description_Standard_Element();

                case Element_Type.Disposition_Advice:
                    return new Disposition_Advice_Element();

                case Element_Type.Donor:
                    return new Donor_Element();

                case Element_Type.Download:
                    return new Downloads_Element();

                case Element_Type.EAD:
                    return new EAD_Form_Element();

                case Element_Type.Edition:
                    return new Edition_Element();

                case Element_Type.EmbeddedVideo:
                    return new EmbeddedVideo_Element();

                case Element_Type.EncodingLevel:
                    return new Encoding_Level_Element();

                case Element_Type.FAST_Subject:
                    return new FAST_Subject_Element();

                case Element_Type.Format:
                    return new Format_Element();

                case Element_Type.Frequency:
                    return new Frequency_Element();

                case Element_Type.Genre:
                    return new Genre_Element();

                case Element_Type.Group_Title:
                    return new Group_Title_Element();

                case Element_Type.Holding:
                    return new Holding_Element();

                case Element_Type.Identifier:
                    if (SubType == "fixed_type")
                        return new Identifier_Fixed_Type_Element();
                    return new Identifier_Element();

                case Element_Type.Language:
                    if (SubType == "select")
                        return new Language_Select_Element();
                    return new Language_Element();

                case Element_Type.LOM_Aggregation_Level:
                    return new LOM_AggregationLevel_Element();

                case Element_Type.LOM_Context:
                    return new LOM_Context_Element();

                case Element_Type.LOM_Difficulty_Level:
                    return new LOM_DifficultyLevel_Element();

                case Element_Type.LOM_Intended_End_User_Role:
                    return new LOM_IntendedUser_Element();

                case Element_Type.LOM_Interactivity_Level:
                    return new LOM_InteractivityLevel_Element();

                case Element_Type.LOM_Interactivity_Type:
                    return new LOM_InteractivityType_Element();

                case Element_Type.LOM_Learning_Resource_Type:
                    return new LOM_ResourceType_Element();

                case Element_Type.LOM_Status:
                    return new LOM_Status_Element();

                case Element_Type.LOM_Typical_Age_Range:
                    return new LOM_TypicalAgeRange_Element();

                case Element_Type.MainThumbnail:
                    return new Main_Thumbnail_Element();

                case Element_Type.Manufacturer:
                    if (SubType == "simple")
                        return new Manufacturer_Element();
                    return new Manufacturer_Complex_Element();

                case Element_Type.Material_Received_Date:
                    return new Material_Received_Date();

                case Element_Type.Note:
                    if (SubType == "simple")
                        return new Note_Element();
                    return new Note_Complex_Element();

                case Element_Type.OCLC_Record_Number:
                    return new OCLC_Record_Number_Element();

                case Element_Type.OtherURL:
                    if (SubType == "simple")
                        return new Other_URL_Element();
                    return new Other_URL_Form_Element();


                case Element_Type.Primary_Identifier:
                    return new Primary_Alt_Identifier_Element();

                case Element_Type.Publication_Status:
                    return new Publication_Status_Element();


                case Element_Type.Publication_Place:
                    return new Publication_Place_Element();

                case Element_Type.Publisher:
                    if (SubType == "simple")
                        return new Publisher_Element();
                    return new Publisher_Complex_Element();

                case Element_Type.RecordOrigin:
                    return new Record_Origin_Element();

                case Element_Type.RecordStatus:
                    return new RecordStatus_Element();

                case Element_Type.RelatedItem:
                    return new Related_Item_Form_Element();

                case Element_Type.Rights:
                    switch (SubType)
                    {
                        case "ir":
                            return new IR_Rights_Element();
                        default:
                            return new Rights_Element();
                    }

                case Element_Type.Scale:
                    return new Scale_Element();

                case Element_Type.SerialHierarchy:
                    switch (SubType)
                    {
                        case "panel":
                            return new Serial_Hierarchy_Panel_Element();
                        default:
                            return new Serial_Hierarchy_Form_Element();
                    }

                case Element_Type.Source:
                    return new Source_Element();

                case Element_Type.Spatial:
                    if (SubType == "simple")
                        return new Spatial_Coverage_Element();
                    return new Hierarchical_Spatial_Form_Element();

                case Element_Type.Subject:
                    switch (SubType)
                    {
                        case "simple":
                            return new Subject_Element();

                        case "scheme":
                            return new Subject_Scheme_Element();

                        default:
                            return new Subject_Keyword_Standard_Form_Element();

                    }

                case Element_Type.TargetAudience:
                    return new Target_Audience_Element();

                case Element_Type.Temporal:
                    if (SubType == "simple")
                        return new Temporal_Coverage_Element();
                    return new Temporal_Complex_Element();

                case Element_Type.Tickler:
                    return new Tickler_Element();

                case Element_Type.Title:
                    if (SubType == "simple")
                        return new Title_Main_Element();
                    return new Title_Main_Form_Element();

                case Element_Type.Title_Other:
                    if ( SubType == "simple" )
                        return new Other_Title_Element();
                    return new Other_Title_Form_Element();

                case Element_Type.Tracking_Box:
                    return new Tracking_Box_Element();

                case Element_Type.Type:
                    switch (SubType)
                    {
                        case "ir":
                            return new IR_Type_Element();
                        case "simple":
                            return new Type_Element();
                        default:
                            return new Type_Format_Form_Element();
                    }

                case Element_Type.VID:
                    return new VID_Element();

                case Element_Type.Viewer:
                    return new Viewer_Element();

                case Element_Type.Visibility:
                    return new Visibility_Element();

                case Element_Type.VRA_CulturalContext:
                    return new VRA_CulturalContext_Element();

                case Element_Type.VRA_Inscription:
                    return new VRA_Inscription_Element();

                case Element_Type.VRA_Material:
                    return new VRA_Material_Element();

                case Element_Type.VRA_Measurement:
                    return new VRA_Measurement_Element();

                case Element_Type.VRA_StateEdition:
                    return new VRA_StateEdition_Element();

                case Element_Type.VRA_StylePeriod:
                    return new VRA_StylePeriod_Element();

                case Element_Type.VRA_Technique:
                    return new VRA_Technique_Element();

                case Element_Type.Web_Skin:
                    return new Web_Skin_Element();

                case Element_Type.Wordmark:
                    return new Wordmark_Element();

                default:
                    return null;
            }
        }
    }
}
