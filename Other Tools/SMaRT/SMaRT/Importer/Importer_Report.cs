#region Using directives

using System;
using System.Data;
using SobekCM.Resource_Object;

#endregion

namespace SobekCM.Management_Tool.Importer
{
    public class Importer_Report
    {
        private DataTable report;

        public Importer_Report()
        {
            report = new DataTable("Importer_Bib_IDs");
            report.Columns.Add("ReceivingID", typeof(int));
            report.Columns.Add("BibID");
            report.Columns.Add("VID");
            report.Columns.Add("Comment");
            report.Columns.Add("Related_File");
            report.Columns.Add("Aleph");
            report.Columns.Add("OCLC");
            report.Columns.Add("Bib_Title");
            report.Columns.Add("Volume_Title");
            report.Columns.Add("Author");
            report.Columns.Add("Material_Type");
            report.Columns.Add("Project_Code");
        }

        public DataTable Data
        {
            get
            {
                return report;
            }
        }

        public DataRow Add_Item( SobekCM_Item bibPackage, string related_file, string comment )
        {
            // Create the new row
            DataRow newRow = report.NewRow();
            newRow["Comment"] = comment;
            newRow["Related_File"] = related_file;

            // Check if the data object is null
            if (bibPackage == null)
            {
                // Add this row to the table and return
                report.Rows.Add(newRow);
                return newRow;
            }

            newRow["ReceivingID"] = -1;
            newRow["BibID"] = bibPackage.BibID;
            newRow["VID"] = bibPackage.VID;
            newRow["Aleph"] = bibPackage.Bib_Info.ALEPH_Record;
            newRow["OCLC"] = bibPackage.Bib_Info.OCLC_Record;
            newRow["Bib_Title"] = bibPackage.Bib_Title;
            newRow["Volume_Title"] = bibPackage.Bib_Info.Main_Title.Title;
            newRow["Author"] = String.Empty;
            if ( bibPackage.Bib_Info.Main_Entity_Name.ToString().Replace("unknown","").Length > 0 )
            {
                newRow["Author"] = bibPackage.Bib_Info.Main_Entity_Name.ToString();
            }
            else
            {
                if ( bibPackage.Bib_Info.Names.Count > 0 )
                {
                    newRow["Author"] = bibPackage.Bib_Info.Names[0].ToString();
                }
            }
            newRow["Material_Type"] = bibPackage.Bib_Info.SobekCM_Type_String;
            newRow["Project_Code"] = bibPackage.Behaviors.Aggregation_Codes;
            report.Rows.Add(newRow);

            return newRow;
        }
    }
}
