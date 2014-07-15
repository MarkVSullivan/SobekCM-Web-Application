#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Threading;
using Microsoft.Practices.ServiceLocation;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Solr;
using SolrNet;

#endregion

namespace SobekCM.Library.Solr
{
    /// <summary> Controller class is used for indexing documents within a SobekCM library or single item aggregation within a SobekCM library </summary>
    public class Solr_Controller
    {
        /// <summary> Indexes all the items within a SobekCM library or a single item aggregation within a SobekCM library </summary>
        /// <param name="SolrDocumentUrl"> URL for the solr/lucene core used for searching for a single document within the library </param>
        /// <param name="SolrPageUrl"> URL for the solr/lucene core used for searching within a single document for matching pages </param>
        /// <param name="File_Location"> Location where all resource files are located </param>
        /// <param name="Collection"> Code the item aggreagtion to index, or empty string to index the entire library </param>
        public static void Index_Collection(string SolrDocumentUrl, string SolrPageUrl, string File_Location, string Collection)
        {
            // Initialize the document-level Solr/Lucene worker and add the solr url
            Startup.Init<SolrDocument>(SolrDocumentUrl);
            var solrDocumentWorker = ServiceLocator.Current.GetInstance<ISolrOperations<SolrDocument>>();

            // Initialize the page-level Solr/Lucene worker and add the solr url
            Startup.Init<SolrPage>(SolrPageUrl);
            var solrPageWorker = ServiceLocator.Current.GetInstance<ISolrOperations<SolrPage>>();

            // Get the start time
            DateTime startTime = DateTime.Now;

            // Get the list of all items in this collection
            int itemcount = 1;
            int sincelastcommit = 0;
            DataSet items = SobekCM_Database.Simple_Item_List(Collection);
            List<SolrDocument> index_files = new List<SolrDocument>();
            List<SolrPage> index_pages = new List<SolrPage>();

            // Temporarily write each bib:vid
            StreamWriter bibVidWriter = new StreamWriter("bib_vid_list.txt");
            foreach (DataRow thisRow in items.Tables[0].Rows)
            {
                string bibid = thisRow[0].ToString();
                string vid = thisRow[1].ToString();
                bibVidWriter.WriteLine(bibid + ":" + vid);
            }
            bibVidWriter.Flush();
            bibVidWriter.Close();

            // Temporarily log this
            StreamWriter logWriter = new StreamWriter("log" + DateTime.Now.Year + DateTime.Now.Month.ToString().PadLeft(2, '0') + DateTime.Now.Day.ToString().PadLeft(2, '0') + ".txt", false);


            // Step through each row
            foreach (DataRow thisRow in items.Tables[0].Rows)
            {
                string bibid = thisRow[0].ToString();
                string vid = thisRow[1].ToString();


                    string directory = File_Location + bibid.Substring(0, 2) + "\\" + bibid.Substring(2, 2) + "\\" + bibid.Substring(4, 2) + "\\" + bibid.Substring(6, 2) + "\\" + bibid.Substring(8) + "\\" + vid.PadLeft(5, '0');
                    string metsFile = directory + "\\" + bibid + "_" + vid + ".mets.xml";
                    if ((Directory.Exists(directory)) && (File.Exists(metsFile)))
                    {
                        Console.WriteLine(itemcount.ToString() + @":" + bibid + @":" + vid);

                        // Read a METS file
                        SobekCM_Item item = SobekCM_Item.Read_METS(metsFile);

                        // Only continue if this is not NULL
                        if (item != null)
                        {
                            logWriter.WriteLine(itemcount.ToString() + ":" + bibid + ":" + vid);

                            // Pull some data from the database
                            DataSet itemInfoSet = Database.SobekCM_Database.Get_Item_Information(bibid, vid, true, null);
                            if ((itemInfoSet != null) && (itemInfoSet.Tables[0].Rows.Count > 0))
                            {
                                DataRow itemRow = itemInfoSet.Tables[0].Rows[0];

                                // Copy over the serial hierarchy
                                item.Behaviors.Serial_Info.Clear();
                                string level1_text = itemRow["Level1_Text"].ToString();
                                if (level1_text.Length > 0)
                                {
                                    item.Behaviors.Serial_Info.Add_Hierarchy(0, Convert.ToInt32(itemRow["Level1_Index"]), level1_text);
                                    string level2_text = itemRow["Level2_Text"].ToString();
                                    if (level2_text.Length > 0)
                                    {
                                        item.Behaviors.Serial_Info.Add_Hierarchy(0, Convert.ToInt32(itemRow["Level2_Index"]), level2_text);
                                        string level3_text = itemRow["Level3_Text"].ToString();
                                        if (level1_text.Length > 0)
                                        {
                                            item.Behaviors.Serial_Info.Add_Hierarchy(0, Convert.ToInt32(itemRow["Level3_Index"]), level3_text);
                                        }
                                    }
                                }

                                // Copy the main thumbnail
                                item.Behaviors.Main_Thumbnail = itemRow["MainThumbnailFile"].ToString();
                                long aleph = Convert.ToInt64(itemRow["ALEPH_Number"]);
                                long oclc = Convert.ToInt64(itemRow["OCLC_Number"]);
                                if (aleph > 1)
                                {
                                    item.Bib_Info.ALEPH_Record = aleph.ToString();
                                }
                                if (oclc > 1)
                                {
                                    item.Bib_Info.OCLC_Record = oclc.ToString();
                                }

                                // Set the aggregations
                                item.Behaviors.Clear_Aggregations();
                                foreach (DataRow thisAggrRow in itemInfoSet.Tables[1].Rows)
                                {
                                    string code = thisAggrRow["Code"].ToString();
                                    string name = thisAggrRow["Name"].ToString();
                                    item.Behaviors.Add_Aggregation(code, name);
                                }
                            }

                            // Add this document to the list of documents to index
                            index_files.Add(new SolrDocument(item, directory));

                            // Index five documents at a time, since this could be alot of pages at a time
                            if (index_files.Count > 4)
                            {
                                logWriter.Flush();

                                // Add to document index
                                logWriter.WriteLine("ADDING TO DOCUMENT INDEX");
                                Console.WriteLine(@"Adding to Lucene/Solr Document Index");

                                bool document_success = false;
                                int document_attempts = 0;
                                while (!document_success)
                                {
                                    try
                                    {
                                        solrDocumentWorker.Add(index_files);
                                        document_success = true;
                                    }
                                    catch (Exception)
                                    {
                                        if (document_attempts > 5)
                                        {
                                            throw;
                                        }
                                        document_attempts++;
                                        logWriter.WriteLine("ERROR " + document_attempts);
                                        Console.WriteLine(@"ERROR " + document_attempts);
                                        Thread.Sleep(document_attempts * 1000);

                                    }
                                }

                                // Add each page to be indexed
                                foreach (SolrDocument document in index_files)
                                {
                                    index_pages.AddRange(document.Solr_Pages);
                                }

                                // Add to page index
                                logWriter.WriteLine("ADDING TO PAGE INDEX");
                                Console.WriteLine(@"Adding to Lucene/Solr Page Index");

                                bool page_success = false;
                                int page_attempts = 0;
                                while (!page_success)
                                {
                                    try
                                    {
                                        solrPageWorker.Add(index_pages);
                                        page_success = true;
                                    }
                                    catch (Exception)
                                    {
                                        if (page_attempts > 5)
                                        {
                                            throw;
                                        }
                                        page_attempts++;
                                        logWriter.WriteLine("ERROR " + page_attempts);
                                        Console.WriteLine(@"ERROR " + page_attempts);
                                        Thread.Sleep(page_attempts * 1000);
                                    }
                                }

                                // Clear the documents and pages
                                index_files.Clear();
                                index_pages.Clear();

                                if (sincelastcommit > 500)
                                {
                                    logWriter.WriteLine("DOCUMENT COMMIT ( " + DateTime.Now.ToString() + " )");
                                    Console.WriteLine(@"Comitting Changes to Lucene/Solr Document Index ( {0} )", DateTime.Now.ToString());
                                    try
                                    {
                                        solrDocumentWorker.Commit();
                                    }
                                    catch 
                                    {
                                        logWriter.WriteLine("ERROR CAUGHT DURING COMMIT ( " + DateTime.Now.ToString() + " )");
                                        Console.WriteLine(@"Error caught during document commit ( {0} )", DateTime.Now.ToString());
                                        Thread.Sleep(10 * 60 * 1000);
                                    }

                                    logWriter.WriteLine("PAGE COMMIT ( " + DateTime.Now.ToString() + " )");
                                    Console.WriteLine(@"Comitting Changes to Lucene/Solr Page Index ( {0} )", DateTime.Now.ToString());
                                    try
                                    {
                                        solrPageWorker.Commit();
                                    }
                                    catch
                                    {
                                        logWriter.WriteLine("ERROR CAUGHT DURING COMMIT ( " + DateTime.Now.ToString() + " )");
                                        Console.WriteLine(@"Error caught during document commit ( {0} )", DateTime.Now.ToString());
                                        Thread.Sleep(10 * 60 * 1000);
                                    }
                                    sincelastcommit = 0;

                                    //if (commitssinceoptimize >= 5)
                                    //{
                                    //    logWriter.WriteLine("DOCUMENT OPTIMIZE ( " + DateTime.Now.ToString() + " )");
                                    //    Console.WriteLine("Optimizing Lucene/Solr Document Index ( " + DateTime.Now.ToString() + " )");
                                    //    try
                                    //    {
                                    //        solrDocumentWorker.Optimize();
                                    //    }
                                    //    catch (Exception ee)
                                    //    {
                                    //        logWriter.WriteLine("ERROR CAUGHT DURING OPTIMIZE ( " + DateTime.Now.ToString() + " )");
                                    //        Console.WriteLine("Error caught during document optimize ( " + DateTime.Now.ToString() + " )");
                                    //        Thread.Sleep(10 * 60 * 1000);
                                    //    }

                                    //    logWriter.WriteLine("PAGE OPTIMIZE ( " + DateTime.Now.ToString() + " )");
                                    //    Console.WriteLine("Optimizing  Lucene/Solr Page Index ( " + DateTime.Now.ToString() + " )");
                                    //    try
                                    //    {
                                    //        solrPageWorker.Optimize();
                                    //    }
                                    //    catch (Exception ee)
                                    //    {
                                    //        logWriter.WriteLine("ERROR CAUGHT DURING OPTIMIZE ( " + DateTime.Now.ToString() + " )");
                                    //        Console.WriteLine("Error caught during document optimize ( " + DateTime.Now.ToString() + " )");
                                    //        Thread.Sleep(10 * 60 * 1000);
                                    //    }

                                    //    commitssinceoptimize = 0;
                                    //}

                                }
                            }
                        }
                        sincelastcommit++;
                    }

                itemcount++;
            }

            if (index_files.Count > 0)
            {
                logWriter.Flush();

                // Add to document index
                Console.WriteLine(@"Adding to Lucene/Solr Document Index");
                solrDocumentWorker.Add(index_files);

                // Add each page to be indexed
                foreach (SolrDocument document in index_files)
                {
                    index_pages.AddRange(document.Solr_Pages);
                }

                // Add to page index
                Console.WriteLine(@"Adding to Lucene/Solr Page Index");
                solrPageWorker.Add(index_pages);

                // Clear the documents and pages
                index_files.Clear();
                index_pages.Clear();
            }

            // Comit the changes to the solr/lucene index
            logWriter.WriteLine("DOCUMENT COMMIT ( " + DateTime.Now.ToString() + " )");
            Console.WriteLine(@"Comitting Changes to Lucene/Solr Document Index ( {0} )", DateTime.Now.ToString());
            try
            {
                solrDocumentWorker.Commit();
            }
            catch 
            {
                logWriter.WriteLine("ERROR CAUGHT DURING COMMIT ( " + DateTime.Now.ToString() + " )");
                Console.WriteLine(@"Error caught during document commit ( {0} )", DateTime.Now.ToString());
                Thread.Sleep(10 * 60 * 1000);
            }

            logWriter.WriteLine("PAGE COMMIT ( " + DateTime.Now.ToString() + " )");
            Console.WriteLine(@"Comitting Changes to Lucene/Solr Page Index ( {0} )", DateTime.Now.ToString());
            try
            {
                solrPageWorker.Commit();
            }
            catch
            {
                logWriter.WriteLine("ERROR CAUGHT DURING COMMIT ( " + DateTime.Now.ToString() + " )");
                Console.WriteLine(@"Error caught during document commit ( {0} )", DateTime.Now.ToString());
                Thread.Sleep(10 * 60 * 1000);
            }

            logWriter.WriteLine("Final document optimize");
            Console.WriteLine(@"Final document optimize");
            try
            {
                solrDocumentWorker.Optimize();
            }
            catch(Exception)
            {
                // Do not do anything here.  It may throw an exception when it runs very longs
            }
            Thread.Sleep(30 * 60 * 1000);

            logWriter.WriteLine("Final page optimize");
            Console.WriteLine(@"Final page optimize");
            try
            {
                solrPageWorker.Optimize();
            }
            catch (Exception)
            {
                // Do not do anything here.  It may throw an exception when it runs very longs
            }
            Thread.Sleep(30 * 60 * 1000);

            // Add final meessage
            Console.WriteLine(@"Process Complete at {0}", DateTime.Now.ToString());
            Console.WriteLine(@"Process Started at {0}", startTime.ToString());
            Console.WriteLine();
            Console.WriteLine(@"Enter any key to exit:");
            Console.ReadKey();

            logWriter.Flush();
            logWriter.Close();
        }

        /// <summary> Indexes a single digital resource within a SobekCM library </summary>
        /// <param name="SolrDocumentUrl"> URL for the solr/lucene core used for searching for a single document within the library </param>
        /// <param name="SolrPageUrl"> URL for the solr/lucene core used for searching within a single document for matching pages </param>
        /// <param name="Resource"> Digital resource to index</param>
        /// <param name="Include_Text"> Flag indicates whether to look for and include full text </param>
        public static void Update_Index(string SolrDocumentUrl, string SolrPageUrl, SobekCM_Item Resource, bool Include_Text )
        {
            // Create the solr workers
            var solrDocumentWorker = Solr_Operations_Cache<SolrDocument>.GetSolrOperations(SolrDocumentUrl);
            var solrPageWorker = Solr_Operations_Cache<SolrPage>.GetSolrOperations(SolrPageUrl);            

            // Get the list of all items in this collection
            List<SolrDocument> index_files = new List<SolrDocument>();
            List<SolrPage> index_pages = new List<SolrPage>();

            // Add this document to the list of documents to index
            index_files.Add(new SolrDocument(Resource, Resource.Source_Directory));

            bool document_success = false;
            int document_attempts = 0;
            while (!document_success)
            {
                try
                {
                    solrDocumentWorker.Add(index_files);
                    document_success = true;
                }
                catch (Exception)
                {
                    if (document_attempts > 5)
                    {
                        throw;
                    }
                    document_attempts++;
                    Console.WriteLine(@"ERROR {0}", document_attempts);
                    Thread.Sleep(document_attempts * 1000);
                }
            }

            // Add each page to be indexed
            foreach (SolrDocument document in index_files)
            {
                index_pages.AddRange(document.Solr_Pages);
            }


            bool page_success = false;
            int page_attempts = 0;
            while (!page_success)
            {
                try
                {
                    solrPageWorker.Add(index_pages);
                    page_success = true;
                }
                catch (Exception)
                {
                    if (page_attempts > 5)
                    {
                        throw;
                    }
                    page_attempts++;
                    Thread.Sleep(page_attempts * 1000);
                }
            }

            // Comit the changes to the solr/lucene index
            try
            {
                solrDocumentWorker.Commit();
            }
            catch
            {
                Thread.Sleep(10 * 60 * 1000);
            }

            try
            {
                solrPageWorker.Commit();
            }
            catch
            {
                Thread.Sleep(10 * 60 * 1000);
            }
        }

        /// <summary> Deletes an existing resource from both solr/lucene core indexes </summary>
        /// <param name="SolrDocumentUrl"> URL for the solr/lucene core used for searching for a single document within the library </param>
        /// <param name="SolrPageUrl"> URL for the solr/lucene core used for searching within a single document for matching pages </param>
        /// <param name="BibID"> Bibliographic identifier for the item to remove from the solr/lucene indexes </param>
        /// <param name="VID"> Volume identifer for the item to remove from the solr/lucene indexes </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public static bool Delete_Resource_From_Index(string SolrDocumentUrl, string SolrPageUrl, string BibID, string VID)
        {
            try
            {
                // Create the solr workers
                var solrDocumentWorker = Solr_Operations_Cache<SolrDocument>.GetSolrOperations(SolrDocumentUrl);
                var solrPageWorker = Solr_Operations_Cache<SolrPage>.GetSolrOperations(SolrPageUrl);

                // For the object, we can use the unique identifier
                solrDocumentWorker.Delete(BibID + ":" + VID);

                // For the pages, we need to search by id
                solrPageWorker.Delete( new SolrQuery("did:\"" + BibID + ":" + VID + "\""));

                // Comit the changes to the solr/lucene index
                try
                {
                    solrDocumentWorker.Commit();
                }
                catch
                {
                    Thread.Sleep(10 * 60 * 1000);
                }

                try
                {
                    solrPageWorker.Commit();
                }
                catch
                {
                    Thread.Sleep(10 * 60 * 1000);
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary> Optimize the solr/lucene core used for searching for a single document </summary>
        /// <param name="SolrDocumentUrl"> URL for the solr/lucene core used for searching for a single document within the library </param>
        public static void Optimize_Document_Index(string SolrDocumentUrl)
        {
            // Create the solr worker
            var solrDocumentWorker = Solr_Operations_Cache<SolrDocument>.GetSolrOperations(SolrDocumentUrl);

            try
            {
                solrDocumentWorker.Optimize();
            }
            catch (Exception)
            {
                // Do not do anything here.  It may throw an exception when it runs very longs
            }
        }

        /// <summary> Optimize the solr/lucene core used for searching within a single document </summary>
        /// <param name="SolrPageUrl"> URL for the solr/lucene core used for searching within a single document for matching pages </param>
        public static void Optimize_Page_Index(string SolrPageUrl)
        {
            // Create the solr worker
            var solrPageWorker = Solr_Operations_Cache<SolrPage>.GetSolrOperations(SolrPageUrl);

            try
            {
                solrPageWorker.Optimize();
            }
            catch (Exception)
            {
                // Do not do anything here.  It may throw an exception when it runs very longs
            }
        }
    }
}
