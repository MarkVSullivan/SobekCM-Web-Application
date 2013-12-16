using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Resource_Object.Configuration;
using SobekCM.Resource_Object;
using Sample_PlugIn_Library;

namespace ConsoleApplication1
{
    class Program
    {
        static void Main(string[] args)
        {
            SobekCM.Resource_Object.Database.SobekCM_Database.Connection_String = "Data Source=192.168.13.12;Initial Catalog=DPanther;User ID=sa;Password=g15r55d3db";
            Metadata_Configuration.Read_Metadata_Configuration("config\\sobekcm_metadata.config");

            SobekCM_Item newItem = SobekCM_Item.Read_METS("sample.xml");

            Sample_FavColor_Metadata_Module taxonInfo = newItem.Get_Metadata_Module(Sample_FavColor_Metadata_Module.Module_Name_Static) as Sample_FavColor_Metadata_Module;
            if (taxonInfo == null)
            {
                taxonInfo = new Sample_FavColor_Metadata_Module();
                newItem.Add_Metadata_Module(Sample_FavColor_Metadata_Module.Module_Name_Static, taxonInfo);
            }

            taxonInfo.Absolute_Favorite_Color = "mauve";
            taxonInfo.Other_Favorite_Color.Add("blue");
            taxonInfo.Other_Favorite_Color.Add("transparent");

            newItem.Save_METS("output.xml");

            SobekCM.Resource_Object.Database.SobekCM_Database.Save_Digital_Resource(newItem);

        }
    }
}
