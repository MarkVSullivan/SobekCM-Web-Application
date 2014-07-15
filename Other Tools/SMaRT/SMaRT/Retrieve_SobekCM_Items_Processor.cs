#region Using directives

using System;
using System.Data;
using System.IO;
using System.Net;

#endregion

namespace SobekCM.Management_Tool
{
    /// <summary> Enumeration defines the type of retrieval to be performed </summary>
    public enum Retrieval_Type_Enum
    {
        /// <summary> Retrieve the METS files for the selected items </summary>
        METS_Only = 1,

        /// <summary> Retrieve the complete package for the selected items </summary>
        Complete,

        /// <summary> Retrieve a MarcXML report with the information or all selected items </summary>
        MARC_XML
    }

    /// <summary> Delegate for the progress event during SobekCM item retrieval </summary>
    /// <param name="currentItem"> Index of the ccurrently being progressed item </param>
    public delegate void Retrieve_SobekCM_Items_Progress_Delegate( int currentItem );

    /// <summary> Delegate for the items complete event during SobekCM item retrieval </summary>
    /// <param name="errorCount"> Number of errors encountered </param>
    public delegate void Retrieve_SobekCM_Items_Complete_Delegate( int errorCount );

    /// <summary> Delegate for the individual file progress during a complete SobekCM item retrieval </summary>
    /// <param name="fileCount"> Number of files processed </param>
    /// <param name="maxFiles"> Complete number of files to be processed </param>
    public delegate void Retrieve_SobekCM_Items_File_Progress_Delegate(int fileCount, int maxFiles );

    /// <summary> Processor class pulls packages (or portions of packages) from a SobekCM library </summary>
    public class Retrieve_SobekCM_Items_Processor
    {
        private readonly string destination;
        private readonly DataTable itemList;
        private readonly Retrieval_Type_Enum retrievalType;

        /// <summary> Constructor for a new instance of the Retrieve_SobekCM_Items_Processor </summary>
        /// <param name="Item_List"> List of items to retrieve </param>
        /// <param name="Destination"> Destination (directory) for the retrieved items </param>
        /// <param name="Retrieval_Type"> Type of retrieval to perform ( i.e, METS, complete package, MarcXML report, etc.. ) </param>
        public Retrieve_SobekCM_Items_Processor(DataTable Item_List, string Destination, Retrieval_Type_Enum Retrieval_Type )
        {
            itemList = Item_List;
            destination = Destination;
            retrievalType = Retrieval_Type;
        }

        /// <summary> Event is fired as each package's retrieval completes </summary>
        public event Retrieve_SobekCM_Items_Progress_Delegate New_Progress;

        /// <summary> Event is fired when the retrieval process is complete </summary>
        public event Retrieve_SobekCM_Items_Complete_Delegate Progress_Complete;

        /// <summary> Event is fired as each individual file is processed when retrieving complete packages </summary>
        public event Retrieve_SobekCM_Items_File_Progress_Delegate File_Progress;

        /// <summary> Start processing this item retrieval request </summary>
        public void Start()
        {
            int current_item = 0;
            int error_count = 0;

            StreamWriter marcReportWriter = null;

            if (retrievalType == Retrieval_Type_Enum.MARC_XML)
            {
                marcReportWriter = new StreamWriter(destination + "\\marc_report.xml", false);

                marcReportWriter.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>");
                marcReportWriter.WriteLine("<collection xmlns=\"http://www.loc.gov/MARC21/slim\">");
            }

            foreach (DataRow thisRow in itemList.Rows)
            {
                if ((retrievalType == Retrieval_Type_Enum.METS_Only) || ( retrievalType == Retrieval_Type_Enum.MARC_XML ))
                {
                    string destination_folder = destination + "\\" + thisRow["Item_ID"].ToString().Replace("_", "\\");

                    // First, always try to download the METS file
                    string mets_file_url = thisRow["Web_Folder"] + "/" + thisRow["Item_ID"] + ".mets.xml";
                    string destination_file = destination_folder + "\\" + thisRow["Item_ID"] + ".mets.xml";
                    if (retrievalType == Retrieval_Type_Enum.MARC_XML)
                    {
                        mets_file_url = thisRow["Web_Folder"] + "/marc.xml";
                        destination_file = destination_folder + "\\marc.xml";
                    }
                    string mets_file_data = Get_Html_Page(mets_file_url);

                    if (mets_file_data.Length > 0)
                    {
                        try
                        {
                            // Create the folder
                            if (!Directory.Exists(destination_folder))
                                Directory.CreateDirectory(destination_folder);

                            // Save the METS file
                            StreamWriter writer = new StreamWriter(destination_file, false);
                            writer.Write(mets_file_data);
                            writer.Flush();
                            writer.Close();
                        }
                        catch 
                        {
                            error_count++;
                        }

                        // Build the marc report
                        if ((retrievalType == Retrieval_Type_Enum.MARC_XML) && ( marcReportWriter != null ))
                        {
                            marcReportWriter.Write(mets_file_data.Replace("</collection>", "").Replace("<?xml version=\"1.0\" encoding=\"UTF-8\" ?>","").Replace("<collection xmlns=\"http://www.loc.gov/MARC21/slim\">",""));
                        }
                    }
                    else
                    {
                        error_count++;
                    }
                }
                else
                {
                    try
                    {
                        // Try to find the resource folder for this
                        string resource_folder =thisRow["Network_Folder"].ToString();

                        if (!Directory.Exists(resource_folder))
                        {
                            error_count++;
                        }
                        else
                        {
                            // Create the folder for this
                            string destination_folder2 = destination + "\\" +thisRow["Item_ID"].ToString().Replace("_", "\\");

                            if (!Directory.Exists(destination_folder2))
                            {
                                Directory.CreateDirectory(destination_folder2);
                            }

                            // Copy all the files over from the standard storage
                            string[] files = Directory.GetFiles(resource_folder);
                            int file_count = 0;
                            foreach (string thisFile in files)
                            {
                                string new_file_name = destination_folder2 + "\\" + (new FileInfo(thisFile)).Name;
                                File.Copy(thisFile, new_file_name, true);

                                if (File_Progress != null)
                                {
                                    file_count++;
                                    File_Progress(file_count, files.Length);
                                }
                            }
                        }
                    }
                    catch 
                    {
                        error_count++;
                    }
                }

                // Fire the progress event
                current_item++;
                if (New_Progress != null)
                    New_Progress(current_item);
            }

            if ((retrievalType == Retrieval_Type_Enum.MARC_XML) && ( marcReportWriter != null ))
            {
                marcReportWriter.WriteLine("</collection>");
                marcReportWriter.Flush();
                marcReportWriter.Close();
            }

            // Fire the complete event
            if (Progress_Complete != null)
                Progress_Complete(error_count);
        }

        private string Get_Html_Page(string strURL)
        {
            try
            {
                // the html retrieved from the page
                WebRequest objRequest = WebRequest.Create(strURL);
                WebResponse objResponse = objRequest.GetResponse();
                Stream objStream = objResponse.GetResponseStream();

                if (objStream != null)
                {
                    // the using keyword will automatically dispose the object 
                    // once complete
                    string strResult;
                    using (StreamReader sr = new StreamReader(objStream))
                    {
                        strResult = sr.ReadToEnd();
                        // Close and clean up the StreamReader
                        sr.Close();
                    }
                    return strResult;
                }
                return String.Empty;
            }
            catch 
            {
                return String.Empty;
            }
        }
    }
}
