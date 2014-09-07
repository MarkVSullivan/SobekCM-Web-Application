using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ServiceStack.Text;
using SobekCM.Resource_Object;

namespace JSON_Serializer_Tester
{
    class Program
    {
        static void Main(string[] args)
        {
            SobekCM_Item item = new SobekCM_Item();
            item = SobekCM_Item.Read_METS(@"\\sob-file01\ContentFiles\auraria\AA\00\00\00\03\00001\AA00000003_00001.mets.xml");

            SobekCM.Resource_Object.Divisions.SobekCM_File_Info thisFile = item.Divisions.Files[0];

            ServiceStack.Text.JsonSerializer<SobekCM_Item> serializer = new JsonSerializer<SobekCM_Item>();

            DateTime startTime = DateTime.Now;



            string itemAsJson = serializer.SerializeToString(item);

            Console.WriteLine("Wrote JSON in: " + DateTime.Now.Subtract(startTime).TotalMilliseconds + " ms");


            SobekCM_Item item2 = new SobekCM_Item();
            item2 = SobekCM_Item.Read_METS(@"\\sob-file01\ContentFiles\auraria\AA\00\00\00\02\00001\AA00000002_00001.mets.xml");

            DateTime startTime2 = DateTime.Now;
            string itemAsJson2 = serializer.SerializeToString(item2);
            Console.WriteLine("Wrote JSON2 in: " + DateTime.Now.Subtract(startTime2).TotalMilliseconds + " ms");

            StreamWriter writer = new StreamWriter("item.json");
            writer.WriteLine(itemAsJson);
            writer.Flush();
            writer.Close();

            Console.ReadKey();
        }
    }
}
