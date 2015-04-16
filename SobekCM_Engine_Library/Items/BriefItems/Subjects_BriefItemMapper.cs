using System;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Maps all the subjects from the METS-based SobekCM_Item object
    /// to the BriefItem, used for most the public functions of the front-end </summary>
    public class Subjects_BriefItemMapper : IBriefItemMapper
    {
        /// <summary> Map one or more data elements from the original METS-based object to the
        /// BriefItem object </summary>
        /// <param name="Original"> Original METS-based object </param>
        /// <param name="New"> New object to populate some data from the original </param>
        /// <returns> TRUE if successful, FALSE if an exception is encountered </returns>
        public bool MapToBriefItem(SobekCM_Item Original, BriefItemInfo New)
        {
            if (Original.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in Original.Bib_Info.Subjects)
                {
                    switch (thisSubject.Class_Type)
                    {
                        //case Subject_Info_Type.Hierarchical_Spatial:
                        //    Subject_Info_HierarchicalGeographic hieroSubj = (Subject_Info_HierarchicalGeographic)thisSubject;
                        //    StringBuilder spatial_builder = new StringBuilder();
                        //    if (hieroSubj.Continent.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(hieroSubj.Continent);
                        //    }
                        //    if (hieroSubj.Country.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.Country).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CO") + hieroSubj.Country + search_link_end);
                        //    }
                        //    if (hieroSubj.Province.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(hieroSubj.Province);
                        //    }
                        //    if (hieroSubj.Region.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(hieroSubj.Region);
                        //    }
                        //    if (hieroSubj.State.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.State).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "ST") + hieroSubj.State + search_link_end);
                        //    }
                        //    if (hieroSubj.Territory.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(hieroSubj.Territory);
                        //    }
                        //    if (hieroSubj.County.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.County).Replace(",", "").Replace("&", "".Replace(" ", "+"))).Replace("<%CODE%>", "CT") + hieroSubj.County + search_link_end);
                        //    }
                        //    if (hieroSubj.City.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(search_link.Replace("<%VALUE%>", HttpUtility.HtmlEncode(hieroSubj.City).Replace(",", "").Replace("&", "").Replace(" ", "+")).Replace("<%CODE%>", "CI") + hieroSubj.City + search_link_end);
                        //    }
                        //    if (hieroSubj.CitySection.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(hieroSubj.CitySection);
                        //    }
                        //    if (hieroSubj.Island.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(hieroSubj.Island);
                        //    }
                        //    if (hieroSubj.Area.Length > 0)
                        //    {
                        //        if (spatial_builder.Length > 0) spatial_builder.Append(" -- ");
                        //        spatial_builder.Append(hieroSubj.Area);
                        //    }
                        //    hierGeo.Add(spatial_builder.ToString());
                        //    break;

                        case Subject_Info_Type.Cartographics:
                            New.Add_Citation("Scale", ((Subject_Info_Cartographics) thisSubject).Scale);
                            break;

                        case Subject_Info_Type.Standard:
                        case Subject_Info_Type.Name:
                        case Subject_Info_Type.TitleInfo:
                            Subject_Standard_Base baseSubject = (Subject_Standard_Base)thisSubject;
                            if ((thisSubject.Class_Type == Subject_Info_Type.Standard) && (baseSubject.Genres_Count > 0) && (baseSubject.Topics_Count == 0) && (baseSubject.Temporals_Count == 0) && (baseSubject.Geographics_Count == 0) && (((Subject_Info_Standard)baseSubject).Occupations_Count == 0))
                            {
                                foreach (string thisGenre in baseSubject.Genres)
                                {
                                    if ( !String.IsNullOrWhiteSpace( baseSubject.Authority ))
                                    {
                                        New.Add_Citation("Genre", thisGenre).Authority = baseSubject.Authority;
                                    }
                                    else
                                    {
                                        New.Add_Citation("Genre", thisGenre);
                                    }
                                }
                            }
                            else
                            {
                                if (( !String.IsNullOrWhiteSpace(thisSubject.Authority)) && ( String.Compare(thisSubject.Authority, "NONE", StringComparison.InvariantCultureIgnoreCase) != 0 ))
                                {
                                    New.Add_Citation("Subjects / Keywords", thisSubject.ToString(false)).Authority = thisSubject.Authority;
                                }
                                else
                                {
                                    New.Add_Citation("Subjects / Keywords", thisSubject.ToString(false));
                                }
                            }
                            break;
                    }
                }
            }

            return true;
        }
    }
}
