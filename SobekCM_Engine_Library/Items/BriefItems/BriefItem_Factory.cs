using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Resource_Object;
using SobekCM.Rest_API.BriefItem;

namespace SobekCM.Engine_Library.Items.BriefItems
{
    /// <summary> Class is used to create the brief item object, from the original SobekCM_Item read
    /// from the METS file </summary>
    public static class BriefItem_Factory
    {
        private static List<IBriefItemMapper> mappers;

        static BriefItem_Factory()
        {
            mappers = new List<IBriefItemMapper>();
            mappers.Add(new Containers_BriefItemMapper());
            mappers.Add(new GeoSpatial_BriefItemMapper());
            mappers.Add(new LearningObjectMetadata_BriefItemMapper());
            mappers.Add(new Names_BriefItemMapper());
            mappers.Add(new Rights_MD_BriefItemMapper());
            mappers.Add(new Subjects_BriefItemMapper());
            mappers.Add(new TEMP_Citation_BriefItemMapper());
            mappers.Add(new Thesis_Dissertation_Info_BriefInfoMapper());
            mappers.Add(new Titles_BriefItemMapper());
            mappers.Add(new VRACore_BriefInfoMapper());
            mappers.Add(new Zoological_Taxonomy_Info_BriefInfoMapper());
        }

        public static BriefItemInfo Create(SobekCM_Item Original)
        {
            BriefItemInfo newItem = new BriefItemInfo();

            foreach (IBriefItemMapper thisMapper in mappers)
            {
                thisMapper.MapToBriefItem(Original, newItem);
            }


            return newItem;
        }
    }
}
