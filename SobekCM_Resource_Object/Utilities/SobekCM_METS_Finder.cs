#region Using directives

using System;
using System.Data;
using System.IO;
using System.Text;

#endregion

namespace SobekCM.Resource_Object.Utilities
{
    /// <summary> METS finder looks for the most current METS file in a variety of workflow and service
    /// locations, pulling the latest file or eventually building the file from database information 
    /// if necessary </summary>
    public class SobekCM_METS_Finder
    {
        /// <summary> Returns the directory name for the indicated digital resource within a 
        /// SobekCM content folder pair-tree </summary>
        /// <param name="BaseDir">Base directory for the content folder </param>
        /// <param name="BibID"> BibID for the digital resource to locate </param>
        /// <param name="VID"> VID for the digital resource to locate </param>
        /// <returns> Full directory for the digital resource, under the content folder pair-tree structure </returns>
        public static string Resource_Folder(string BaseDir, string BibID, string VID)
        {
            string item_folder = BibID.Substring(0, 2) + Path.DirectorySeparatorChar + BibID.Substring(2, 2) + Path.DirectorySeparatorChar + BibID.Substring(4, 2) + Path.DirectorySeparatorChar +
                BibID.Substring(6, 2) + Path.DirectorySeparatorChar + BibID.Substring(8, 2) + Path.DirectorySeparatorChar + VID;

            return Path.Combine(BaseDir, item_folder);
        }

        /// <summary> Returns the name of the current METS file for the indicated digital resource within a 
        /// SobekCM content folder pair-tree </summary>
        /// <param name="BaseDir">Base directory for the content folder </param>
        /// <param name="BibID"> BibID for the digital resource to locate </param>
        /// <param name="VID"> VID for the digital resource to locate </param>
        /// <returns> Full qualified filename for the METS for the digital resource, under the content folder pair-tree structure </returns>
        public static string METS_File(string BaseDir, string BibID, string VID)
        {
            string item_folder = BibID.Substring(0, 2) + Path.DirectorySeparatorChar + BibID.Substring(2, 2) + Path.DirectorySeparatorChar + BibID.Substring(4, 2) + Path.DirectorySeparatorChar +
                BibID.Substring(6, 2) + Path.DirectorySeparatorChar + BibID.Substring(8, 2) + Path.DirectorySeparatorChar + VID;

            return Path.Combine(BaseDir, item_folder, BibID + "_" + VID + ".mets.xml");
        }



        private static string SOBEKCM_IMAGE_LOCATION = @"\\cns-uflib-ufdc\UFDC\RESOURCES\";
        private static string SOBEKCM_DROPBOX_LOCATION = @"\\cns-uflib-ufdc\UFDC\INCOMING\";
        private static string SOBEKCM_DATA_LOCATION = @"\\cns-uflib-ufdc\UFDC\DATA\";

        /// <summary> Find the most current METS file by looking in a variety of workflow
        /// and service locations, pulling the latest file or eventually building the file from
        /// database information if necessary </summary>
        /// <param name="BibID"> Bibliographic identifier for the item to find </param>
        /// <param name="VID"> Volume identifier for the item to find </param>
        /// <param name="Folder"> Folder in which to place the found or built METS file </param>
        /// <param name="VolumeInfoFromDB"> DataSet of data about this volume from the database (or NULL)</param>
        /// <returns> SobekCM Item object read from the METS file, or NULL if unable to perform the tasks </returns>
        public static SobekCM_Item Find_UFDC_METS(string BibID, string VID, string Folder, DataSet VolumeInfoFromDB)
        {
            SobekCM_Item bibPackage = null;
            bool mets_found = false;
            DateTime lastWriteDate = new DateTime(1900, 1, 1);

            // Look for pre-existing mets files in the inbound Folder(s)
            if (Directory.Exists(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "\\" + VID))
            {
                if (File.Exists(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml"))
                {
                    mets_found = true;
                    lastWriteDate = (new FileInfo(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml")).LastWriteTime;
                    if (Folder.Length > 0)
                    {
                        File.Copy(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml", Folder + "\\" + BibID + "_" + VID + ".mets", true);
                        bibPackage = SobekCM_Item.Read_METS(Folder + "\\" + BibID + "_" + VID + ".mets");
                    }
                    else
                    {
                        bibPackage = SobekCM_Item.Read_METS(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml");
                    }
                }
            }

            // Look for a flattened inbound mets file
            if ((!mets_found) && (Directory.Exists(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "_" + VID)))
            {
                if (File.Exists(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "_" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml"))
                {
                    mets_found = true;
                    lastWriteDate = (new FileInfo(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "_" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml")).LastWriteTime;

                    if (Folder.Length > 0)
                    {
                        File.Copy(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "_" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml", Folder + "\\" + BibID + "_" + VID + ".mets", true);
                        bibPackage = SobekCM_Item.Read_METS(Folder + "\\" + BibID + "_" + VID + ".mets");
                    }
                    else
                    {
                        bibPackage = SobekCM_Item.Read_METS(SOBEKCM_DROPBOX_LOCATION + "inbound\\" + BibID + "_" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml");
                    }
                }
            }

            // Look for a processing mets file
            if ((!mets_found) && (Directory.Exists(SOBEKCM_DROPBOX_LOCATION + "processing\\" + BibID + "\\" + VID)))
            {
                if (File.Exists(SOBEKCM_DROPBOX_LOCATION + "processing\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml"))
                {
                    mets_found = true;
                    lastWriteDate = (new FileInfo(SOBEKCM_DROPBOX_LOCATION + "processing\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml")).LastWriteTime;

                    if (Folder.Length > 0)
                    {
                        File.Copy(SOBEKCM_DROPBOX_LOCATION + "processing\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml", Folder + "\\" + BibID + "_" + VID + ".mets", true);
                        bibPackage = SobekCM_Item.Read_METS(Folder + "\\" + BibID + "_" + VID + ".mets");
                    }
                    else
                    {
                        bibPackage = SobekCM_Item.Read_METS(SOBEKCM_DROPBOX_LOCATION + "processing\\" + BibID + "\\" + VID + "\\" + BibID + "_" + VID + ".METS_Header.xml");
                    }
                }
            }


            // Get the directory for the files already loaded to UFDC
            string sobekcm_folder = BibID.Substring(0, 2) + "\\" + BibID.Substring(2, 2) + "\\" + BibID.Substring(4, 2) + "\\" + BibID.Substring(6, 2) + "\\" + BibID.Substring(8, 2) + "\\" + VID;
            if (Directory.Exists(SOBEKCM_IMAGE_LOCATION + sobekcm_folder))
            {
                string folders_and_mets = sobekcm_folder + "\\" + BibID + "_" + VID + ".METS_Header.xml";
                if (File.Exists(SOBEKCM_IMAGE_LOCATION + folders_and_mets))
                {
                    if (mets_found)
                    {
                        // If the METS was already found, still check for a NEWER one online
                        DateTime compareWriteDate = (new FileInfo(SOBEKCM_IMAGE_LOCATION + folders_and_mets)).LastWriteTime;
                        if (compareWriteDate.CompareTo(lastWriteDate) > 0)
                        {
                            if (Folder.Length > 0)
                            {
                                File.Copy(SOBEKCM_IMAGE_LOCATION + folders_and_mets, Folder + "\\" + BibID + "_" + VID + ".mets", true);
                                bibPackage = SobekCM_Item.Read_METS(Folder + "\\" + BibID + "_" + VID + ".mets");
                            }
                            else
                            {
                                bibPackage = SobekCM_Item.Read_METS(SOBEKCM_IMAGE_LOCATION + folders_and_mets);
                            }
                        }
                    }
                    else
                    {
                        mets_found = true;
                        if (Folder.Length > 0)
                        {
                            File.Copy(SOBEKCM_IMAGE_LOCATION + folders_and_mets, Folder + "\\" + BibID + "_" + VID + ".mets", true);
                            bibPackage = SobekCM_Item.Read_METS(Folder + "\\" + BibID + "_" + VID + ".mets");
                        }
                        else
                        {
                            bibPackage = SobekCM_Item.Read_METS(SOBEKCM_IMAGE_LOCATION + folders_and_mets);
                        }
                    }
                }
            }

            // Look in the DATA Folder
            if ((!mets_found) && (SOBEKCM_DATA_LOCATION.Length > 0))
            {
                string data_mets_folder = SOBEKCM_DATA_LOCATION + "METS\\" + BibID[0] + "\\" + BibID[1] + "\\" + BibID[2] + "\\" + BibID[3] + "\\" + BibID[4] + "\\" + BibID[5] + "\\" + BibID[6] + "\\" + BibID[7] + "\\" + BibID[8] + "\\" + BibID[9] + "\\" + VID;
                if (Directory.Exists(data_mets_folder))
                {
                    string data_mets_file = data_mets_folder + "\\" + BibID + "_" + VID + ".mets";
                    if (File.Exists(data_mets_file))
                    {
                        mets_found = true;
                        if (Folder.Length > 0)
                        {
                            File.Copy(data_mets_file, Folder + "\\" + BibID + "_" + VID + ".mets", true);
                            bibPackage = SobekCM_Item.Read_METS(Folder + "\\" + BibID + "_" + VID + ".mets");
                        }
                        else
                        {
                            bibPackage = SobekCM_Item.Read_METS(data_mets_file);
                        }
                    }
                }
            }

            // Look in the local folder
            if ((!mets_found) && (Folder.Length > 0))
            {
                string[] mets_files = Directory.GetFiles(Folder, "*.mets*");
                if (mets_files.Length > 0)
                {
                    bibPackage = SobekCM_Item.Read_METS(mets_files[0]);
                    if (bibPackage != null)
                        mets_found = true;
                }
            }

            // If still no METS, have to build one from the values provided and 
            // any existing MARC record in the DATA Folder
            if ((!mets_found) && (VolumeInfoFromDB != null))
            {
                bool recordCreated = false;
                DataRow itemRow = VolumeInfoFromDB.Tables[2].Rows[0];
                string materialType = itemRow["Type"].ToString();
                string title = itemRow["Title"].ToString();

                string publisher = String.Empty;
                string author = String.Empty;
                string donor = String.Empty;
                string pubDate = String.Empty;
                string alephNumber = String.Empty;
                string oclcNumber = String.Empty;

                if (itemRow["Publisher"] != DBNull.Value)
                    publisher = itemRow["Publisher"].ToString();
                if (itemRow["Author"] != DBNull.Value)
                    author = itemRow["Author"].ToString();
                if (itemRow["Donor"] != DBNull.Value)
                    donor = itemRow["Donor"].ToString();
                if (itemRow["PubDate"] != DBNull.Value)
                    pubDate = itemRow["PubDate"].ToString();
                if (itemRow["ALEPH_Number"] != DBNull.Value)
                    alephNumber = itemRow["ALEPH_Number"].ToString();
                if (itemRow["OCLC_Number"] != DBNull.Value)
                    oclcNumber = itemRow["OCLC_Number"].ToString();

                // Look for an OCLC record MarcXML file
                if (oclcNumber.Length > 1)
                {
                    oclcNumber = oclcNumber.PadLeft(8, '0');
                    StringBuilder oclcDirBuilder = new StringBuilder(SOBEKCM_DATA_LOCATION + "MARCXML\\OCLC\\");
                    foreach (char thisChar in oclcNumber)
                    {
                        oclcDirBuilder.Append(thisChar + "\\");
                    }
                    if ((Directory.Exists(oclcDirBuilder.ToString())) && (File.Exists(oclcDirBuilder.ToString() + oclcNumber + ".xml")))
                    {
                        // Read this in then
                        bibPackage = new SobekCM_Item();
                        recordCreated = true;
                        bibPackage.Read_From_MARC_XML(oclcDirBuilder.ToString() + oclcNumber + ".xml");
                        bibPackage.Bib_Info.Record.Record_Origin = "Imported from (OCLC)" + oclcNumber;
                    }
                }

                // Look for a local catalog record MarcXML file
                if ((!recordCreated) && (alephNumber.Length > 1))
                {
                    alephNumber = alephNumber.PadLeft(9, '0');
                    StringBuilder alephDirBuilder = new StringBuilder(SOBEKCM_DATA_LOCATION + "MARCXML\\");
                    foreach (char thisChar in alephNumber)
                    {
                        alephDirBuilder.Append(thisChar + "\\");
                    }
                    if ((Directory.Exists(alephDirBuilder.ToString())) && (File.Exists(alephDirBuilder.ToString() + alephNumber + ".xml")))
                    {
                        // Read this in then
                        bibPackage = new SobekCM_Item();
                        recordCreated = true;
                        bibPackage.Read_From_MARC_XML(alephDirBuilder.ToString() + alephNumber + ".xml");
                        bibPackage.Bib_Info.Record.Record_Origin = "Imported from (ALEPH)" + alephNumber;
                    }
                }

                // If not created, make a blank item
                if (!recordCreated)
                {
                    bibPackage = new SobekCM_Item();
                    bibPackage.Bib_Info.Record.Record_Origin = "Derived from the SobekCM databse";
                    bibPackage.Bib_Info.Main_Title.Title = title;
                    if (author.IndexOf("|") > 0)
                    {
                        string[] authors = author.Split("|".ToCharArray());
                        bibPackage.Bib_Info.Main_Entity_Name.Full_Name = authors[0].Replace("<i>", "").Replace("</i>", "");
                        for (int i = 1; i < authors.Length; i++)
                        {
                            bibPackage.Bib_Info.Add_Named_Entity(authors[i].Replace("<i>", "").Replace("</i>", ""));
                        }
                    }
                    else
                    {
                        bibPackage.Bib_Info.Main_Entity_Name.Full_Name = author.Replace("<i>", "").Replace("</i>", "");
                    }
                    if (donor.Length > 0)
                    {
                        bibPackage.Bib_Info.Donor.Full_Name = donor;
                    }
                    if (pubDate.Length > 0)
                    {
                        bibPackage.Bib_Info.Origin_Info.Date_Issued = pubDate;
                    }
                    if (alephNumber.Length > 1)
                    {
                        bibPackage.Bib_Info.Add_Identifier(alephNumber, "ALEPH");
                    }
                    if (oclcNumber.Length > 1)
                    {
                        bibPackage.Bib_Info.Add_Identifier(oclcNumber, "OCLC");
                    }
                    if (publisher.IndexOf("|") > 0)
                    {
                        string[] publishers = publisher.Split("|".ToCharArray());
                        for (int i = 0; i < publishers.Length; i++)
                        {
                            if (publishers[i] != "(multiple)")
                            {
                                bibPackage.Bib_Info.Add_Publisher(publishers[i]);
                            }
                        }
                    }
                    else
                    {
                        bibPackage.Bib_Info.Add_Publisher(publisher);
                    }
                }

                // Add the values needed for all the records originated this way
                bibPackage.BibID = BibID;
                bibPackage.VID = VID;
            }

            return bibPackage;
        }
    }
}