#region Using directives

using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using SobekCM.Core.Configuration;
using SobekCM.Core.Navigation;

#endregion

namespace SobekCM.Core.Aggregations
{
    /// <summary> Class is used to compare two aggregation objects and is generally used to determine
    /// has been updated within an item aggreation during administrative work </summary>
    public class Complete_Item_Aggregation_Comparer
    {
        /// <summary> Compares the two aggregation objects and returns a list of differences for saving as milestones
        /// during aggregation curatorial or administrative work </summary>
        /// <param name="Base"> Base item aggregation </param>
        /// <param name="Compared"> New item aggregation object to compare to the base </param>
        /// <returns> List of changes between the two aggregation objects </returns>
        public static List<string> Compare(Complete_Item_Aggregation Base, Complete_Item_Aggregation Compared )
        {

            // TODO: Facet comparison below needs to look up the name of the facet,
            // TODO: rather than just showing the primary key to the facet

            List<string> changes = new List<string>();

            // code
            if (!Base.Code.Equals(Compared.Code))
            {
                changes.Add("Changed code ( '" + Base.Code + "' --> '" + Compared.Code + "' )");
            }

            // parents
            List<string> base_parents = new List<string>();
            List<string> compared_parents = new List<string>();
            if (Base.Parents != null)
            {
                foreach (Item_Aggregation_Related_Aggregations parentAggr in Base.Parents)
                {
                    // Look in compared for a match
                    if ((Compared.Parents == null) || (Compared.Parents.All(CompareAggr => String.Compare(parentAggr.Code, CompareAggr.Code, StringComparison.InvariantCultureIgnoreCase) != 0)))
                    {
                        base_parents.Add(parentAggr.Code);
                    }
                        
                }
            }
            if (Compared.Parents != null)
            {
                foreach (Item_Aggregation_Related_Aggregations parentAggr in Compared.Parents)
                {
                    // Look in base for a match
                    if ((Base.Parents == null) || (Base.Parents.All(CompareAggr => String.Compare(parentAggr.Code, CompareAggr.Code, StringComparison.InvariantCultureIgnoreCase) != 0)))
                    {
                        compared_parents.Add(parentAggr.Code);
                    }
                }
            }
            if (base_parents.Count > 0)
            {
                if (base_parents.Count == 1)
                {
                    changes.Add("Removed parent " + base_parents[0] );
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Removed parents " + base_parents[0]);
                    for (int i = 1; i < base_parents.Count; i++)
                        builder.Append(", " + base_parents[i]);
                    changes.Add(builder.ToString());
                }
            }
            if (compared_parents.Count > 0)
            {
                if (compared_parents.Count == 1)
                {
                    changes.Add("Added parent " + compared_parents[0]);
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Added parents " + compared_parents[0]);
                    for (int i = 1; i < compared_parents.Count; i++)
                        builder.Append(", " + compared_parents[i]);
                    changes.Add(builder.ToString());
                }
            }

            // name
            if (!Base.Name.Equals(Compared.Name))
            {
                changes.Add("Changed name ( '" + Base.Name + "' --> '" + Compared.Name + "' )");
            }

            // short name
            if (!Base.ShortName.Equals(Compared.ShortName))
            {
                changes.Add("Changed short name ( '" + Base.ShortName + "' --> '" + Compared.ShortName + "' )");
            }

            // description
            if (!Base.Description.Equals(Compared.Description))
            {
                changes.Add("Changed description");
            }

            // type
            if (!Base.Type.Equals(Compared.Type))
            {
                changes.Add("Changed type ( '" + Base.Type + "' --> '" + Compared.Type + "' )");
            }

            // email address
            compare_nullable_strings(Base.Contact_Email, Compared.Contact_Email, "contact email", changes);

            // external link
            compare_nullable_strings(Base.External_Link, Compared.External_Link, "external link", changes);

            // Hidden flag
            if ( Base.Hidden != Compared.Hidden )
            {
                if ( Compared.Hidden )
                    changes.Add("Removed from parent home page");
                else
                    changes.Add("Added to parent home page");
            }

            // active flag
            if (Base.Active != Compared.Active)
            {
                if (Compared.Active)
                    changes.Add("Aggregation activated");
                else
                    changes.Add("Aggregation deactivated");
            }

            // thematic headings
            if ( Base.Thematic_Heading == null )
            {
                if ( Compared.Thematic_Heading != null )
                {
                    changes.Add("Added to thematic heading '" + Compared.Thematic_Heading.Text + "'");
                }
            }
            else
            {
                if ( Compared.Thematic_Heading == null )
                {
                    changes.Add("Removed from thematic heading '" + Base.Thematic_Heading.Text + "'");
                }
                else
                {
                    if (Base.Thematic_Heading.ID != Compared.Thematic_Heading.ID)
                    {
                        changes.Add("Changed thematic heading ( '" + Base.Thematic_Heading.Text + "' --> '" + Compared.Thematic_Heading.Text + "' )");
                    }
                }
            }

            // web skin
            List<string> base_skins = new List<string>();
            List<string> compared_skins = new List<string>();
            if (Base.Web_Skins != null)
            {
                foreach( string thisSkin in Base.Web_Skins)
                {
                    // Look in compared for a match
                    if ((Compared.Web_Skins == null) || (Compared.Web_Skins.All(CompareSkin => String.Compare(thisSkin, CompareSkin, StringComparison.InvariantCultureIgnoreCase) != 0)))
                    {
                        base_skins.Add(thisSkin);
                    }                       
                }
            }
            if (Compared.Web_Skins != null)
            {
                foreach ( string thisSkin in Compared.Web_Skins)
                {
                    // Look in base for a match
                    if ((Base.Web_Skins == null) || (Base.Web_Skins.All(CompareSkin => String.Compare(thisSkin, CompareSkin, StringComparison.InvariantCultureIgnoreCase) != 0)))
                    {
                        compared_skins.Add(thisSkin); 
                    }
                }
            }
            if (base_skins.Count > 0)
            {
                if (base_skins.Count == 1)
                {
                    changes.Add("Removed web skin " + base_skins[0]);
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Removed web skins " + base_skins[0]);
                    for (int i = 1; i < base_skins.Count; i++)
                        builder.Append(", " + base_skins[i]);
                    changes.Add(builder.ToString());
                }
            }
            if (compared_skins.Count > 0)
            {
                if (compared_skins.Count == 1)
                {
                    changes.Add("Added web skin " + compared_skins[0]);
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Added web skins " + compared_skins[0]);
                    for (int i = 1; i < compared_skins.Count; i++)
                        builder.Append(", " + compared_skins[i]);
                    changes.Add(builder.ToString());
                }
            }

            // default web skin
            compare_nullable_strings(Base.Default_Skin, Compared.Default_Skin, "default web skin", changes);

            // custom css
            if (String.IsNullOrEmpty(Base.CSS_File))
            {
                if (!String.IsNullOrEmpty(Compared.CSS_File))
                {
                    changes.Add("Enabled the aggregation-level stylesheet");
                }
            }
            else
            {
                if (String.IsNullOrEmpty(Compared.CSS_File))
                {
                    changes.Add("Disabled the aggregation-level stylesheet");
                }
            }

            // home pages (multilingual)
            List<Web_Language_Enum> removedLanguages = new List<Web_Language_Enum>();
            List<Web_Language_Enum> addedLanguages = new List<Web_Language_Enum>();
            if (Base.Home_Page_File_Dictionary != null)
            {
                foreach ( KeyValuePair<Web_Language_Enum, Complete_Item_Aggregation_Home_Page> thisHomePage in Base.Home_Page_File_Dictionary)
                {
                    // Look in compared for a match
                    if ((Compared.Home_Page_File_Dictionary == null) || (Compared.Home_Page_File_Dictionary.All(CompareHomePage => thisHomePage.Key != CompareHomePage.Key)))
                    {
                        removedLanguages.Add(thisHomePage.Key);
                    }                       
                }
            }
            if (Compared.Home_Page_File_Dictionary != null)
            {
                foreach (KeyValuePair<Web_Language_Enum, Complete_Item_Aggregation_Home_Page> thisHomePage in Compared.Home_Page_File_Dictionary)
                {
                    // Look in base for a match
                    if ((Base.Home_Page_File_Dictionary == null) || (Base.Home_Page_File_Dictionary.All(CompareHomePage => thisHomePage.Key != CompareHomePage.Key)))
                    {
                        addedLanguages.Add(thisHomePage.Key);
                    }                       
                }
            }
            if (removedLanguages.Count > 0)
            {
                if (removedLanguages.Count == 1)
                {
                    changes.Add("Removed " + Web_Language_Enum_Converter.Enum_To_Name(removedLanguages[0]) + " home page");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Removed " + Web_Language_Enum_Converter.Enum_To_Name(removedLanguages[0]));
                    for (int i = 1; i < removedLanguages.Count; i++)
                        builder.Append(", " + removedLanguages[i]);
                    changes.Add(builder + " home pages");
                }
            }
            if (addedLanguages.Count > 0)
            {
                if (addedLanguages.Count == 1)
                {
                    changes.Add("Added " + Web_Language_Enum_Converter.Enum_To_Name(addedLanguages[0]) + " home page");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Added " + Web_Language_Enum_Converter.Enum_To_Name(removedLanguages[0]));
                    for (int i = 1; i < addedLanguages.Count; i++)
                        builder.Append(", " + addedLanguages[i]);
                    changes.Add(builder + " home pages");
                }
            }


            // banners (multilingual)
            removedLanguages.Clear();
            addedLanguages.Clear();
            if (Base.Banner_Dictionary != null)
            {
                foreach (KeyValuePair<Web_Language_Enum, string> thisBanner in Base.Banner_Dictionary)
                {
                    // Look in compared for a match
                    bool match = false;
                    if (Compared.Banner_Dictionary != null)
                    {
                        foreach (KeyValuePair<Web_Language_Enum, string> compareBanner in Compared.Banner_Dictionary)
                        {
                            if (thisBanner.Key == compareBanner.Key)
                            {
                                match = true;

                                // Now, compare the source file as well
                                if (String.Compare(thisBanner.Value, compareBanner.Value, StringComparison.InvariantCultureIgnoreCase) != 0)
                                {
                                    changes.Add("Changed " + Web_Language_Enum_Converter.Enum_To_Name(thisBanner.Key) + " banner source file (" + thisBanner.Value + " --> " + compareBanner.Value + ")");
                                }

                                break;
                            }
                        }
                    }
                    if (!match)
                        removedLanguages.Add(thisBanner.Key);
                }
            }
            if (Compared.Banner_Dictionary != null)
            {
                foreach (KeyValuePair<Web_Language_Enum, string> thisBanner in Compared.Banner_Dictionary)
                {
                    // Look in base for a match
                    if ((Base.Banner_Dictionary == null) || (Base.Banner_Dictionary.All(CompareBanner => thisBanner.Key != CompareBanner.Key)))
                    {
                        addedLanguages.Add(thisBanner.Key);
                    }
                }
            }
            if (removedLanguages.Count > 0)
            {
                if (removedLanguages.Count == 1)
                {
                    changes.Add("Removed " + Web_Language_Enum_Converter.Enum_To_Name(removedLanguages[0]) + " banner");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Removed " + Web_Language_Enum_Converter.Enum_To_Name(removedLanguages[0]));
                    for (int i = 1; i < removedLanguages.Count; i++)
                        builder.Append(", " + removedLanguages[i]);
                    changes.Add(builder + " banners");
                }
            }
            if (addedLanguages.Count > 0)
            {
                if (addedLanguages.Count == 1)
                {
                    changes.Add("Added " + Web_Language_Enum_Converter.Enum_To_Name(addedLanguages[0]) + " banner");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Added " + Web_Language_Enum_Converter.Enum_To_Name(addedLanguages[0]));
                    for (int i = 1; i < addedLanguages.Count; i++)
                        builder.Append(", " + addedLanguages[i]);
                    changes.Add(builder + " banners");
                }
            }

            // search types
            List<Search_Type_Enum> removedSearches = new List<Search_Type_Enum>();
            List<Search_Type_Enum> addedSearches = new List<Search_Type_Enum>();
            if (Base.Search_Types != null)
            {
                foreach (Search_Type_Enum thisSearch in Base.Search_Types)
                {
                    // Look in compared for a match
                    if ((Compared.Search_Types == null) || (Compared.Search_Types.All(CompareSearch => thisSearch != CompareSearch)))
                    {
                        removedSearches.Add(thisSearch);
                    }
                }
            }
            if (Compared.Search_Types != null)
            {
                foreach (Search_Type_Enum thisSearch in Compared.Search_Types)
                {
                    // Look in base for a match
                    if ((Base.Search_Types == null) || (Base.Search_Types.All(CompareSearch => thisSearch != CompareSearch)))
                    {
                        addedSearches.Add(thisSearch);
                    }
                }
            }
            if (removedSearches.Count > 0)
            {
                if (removedSearches.Count == 1)
                {
                    changes.Add("Removed " + removedSearches[0].ToString() + " search");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Removed " + removedSearches[0].ToString());
                    for (int i = 1; i < removedSearches.Count; i++)
                        builder.Append(", " + removedSearches[i].ToString());
                    changes.Add(builder + " searches");
                }
            }
            if (addedSearches.Count > 0)
            {
                if (addedSearches.Count == 1)
                {
                    changes.Add("Added " + addedSearches[0] + " search");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Added " + addedSearches[0]);
                    for (int i = 1; i < addedSearches.Count; i++)
                        builder.Append(", " + addedSearches[i]);
                    changes.Add(builder + " searches");
                }
            }

            // other display types? ( all/new item browse, map browse )

            // map search default area
            if (Base.Map_Search != Compared.Map_Search )
            {
                changes.Add("Changed default map area display");
            }

            // facets
            List<short> addedFacets = new List<short>();
            List<short> removedFacets = new List<short>();
            if (Base.Facets != null)
            {
                foreach (short thisFacet in Base.Facets)
                {
                    // Look in compared for a match
                    if ((Compared.Facets == null) || (Compared.Facets.All(CompareFacet => thisFacet != CompareFacet)))
                    {
                        removedFacets.Add(thisFacet);
                    }
                }
            }
            if (Compared.Facets != null)
            {
                foreach (short thisFacet in Compared.Facets)
                {
                    // Look in base for a match
                    if ((Base.Facets == null) || (Base.Facets.All(CompareFacet => thisFacet != CompareFacet)))
                    {
                        addedFacets.Add(thisFacet);
                    }
                }
            }
            if (removedFacets.Count > 0)
            {
                if (removedFacets.Count == 1)
                {
                    changes.Add("Removed facet " + removedFacets[0]);
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Removed facets " + removedFacets[0]);
                    for (int i = 1; i < removedFacets.Count; i++)
                        builder.Append(", " + removedFacets[i]);
                    changes.Add(builder.ToString());
                }
            }
            if (addedFacets.Count > 0)
            {
                if (addedFacets.Count == 1)
                {
                    changes.Add("Added facet " + addedFacets[0]);
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Added facets " + addedFacets[0]);
                    for (int i = 1; i < addedFacets.Count; i++)
                        builder.Append(", " + addedFacets[i]);
                    changes.Add(builder.ToString());
                }
            }

            // result views
            List<Result_Display_Type_Enum> removedResultsDisplay = new List<Result_Display_Type_Enum>();
            List<Result_Display_Type_Enum> addedResultsDisplays = new List<Result_Display_Type_Enum>();
            if (Base.Result_Views != null)
            {
                foreach (Result_Display_Type_Enum thisSearch in Base.Result_Views)
                {
                    // Look in compared for a match
                    if ((Compared.Result_Views == null) || (Compared.Result_Views.All(CompareSearch => thisSearch != CompareSearch)))
                    {
                        removedResultsDisplay.Add(thisSearch);
                    }
                }
            }
            if (Compared.Search_Types != null)
            {
                foreach (Result_Display_Type_Enum thisSearch in Compared.Result_Views)
                {
                    // Look in base for a match
                    if ((Base.Result_Views == null) || (Base.Result_Views.All(CompareSearch => thisSearch != CompareSearch)))
                    {
                        addedResultsDisplays.Add(thisSearch);
                    }
                }
            }
            if (removedResultsDisplay.Count > 0)
            {
                if (removedResultsDisplay.Count == 1)
                {
                    changes.Add("Removed " + removedResultsDisplay[0].ToString() + " result display type");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Removed " + removedResultsDisplay[0].ToString());
                    for (int i = 1; i < removedResultsDisplay.Count; i++)
                        builder.Append(", " + removedResultsDisplay[i].ToString());
                    changes.Add(builder + " result display types");
                }
            }
            if (addedResultsDisplays.Count > 0)
            {
                if (addedResultsDisplays.Count == 1)
                {
                    changes.Add("Added " + addedResultsDisplays[0] + " result display type");
                }
                else
                {
                    StringBuilder builder = new StringBuilder("Added " + addedResultsDisplays[0]);
                    for (int i = 1; i < addedResultsDisplays.Count; i++)
                        builder.Append(", " + addedResultsDisplays[i]);
                    changes.Add(builder + " result display types");
                }
            }

            // default browse
            compare_nullable_strings(Base.Default_BrowseBy, Compared.Default_BrowseBy, "default browse", changes);

            // metadata browse


            // oai-pmh flag
            if (Base.OAI_Enabled != Compared.OAI_Enabled)
            {
                if (Compared.OAI_Enabled)
                    changes.Add("OAI-PMH enabled");
                else
                    changes.Add("OAI-PMH disabled");
            }

            // oai-pmh description
            compare_nullable_strings(Base.OAI_Metadata, Compared.OAI_Metadata, "additional OAI-PMH metadata", changes);

            // child pages


            return changes;
        }

        private static void compare_nullable_strings(string A, string B, string Type, List<string> Logger)
        {
            if (String.IsNullOrEmpty(A))
            {
                if (!String.IsNullOrEmpty(B))
                {
                    Logger.Add("Added " + Type + " ( '" + B + "' )");
                }
            }
            else
            {
                if (String.IsNullOrEmpty(B))
                {
                    Logger.Add("Removed " + Type + " ( '" + A + "' )");
                }
                else
                {
                    if (A != B)
                    {
                        Logger.Add("Changed " + Type + " ( '" + A + "' --> '" + B + "' )");
                    }
                }
            }
        }
    }
}
