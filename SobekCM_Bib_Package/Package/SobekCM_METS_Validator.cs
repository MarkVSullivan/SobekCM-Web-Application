using System;
using System.Collections.Generic;
using System.IO;
using System.Text;
using SobekCM.Bib_Package.Divisions;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.Database;
using System.Net;
using System.Net.Sockets;
using System.Data;
using System.Collections.Specialized;
using System.Text.RegularExpressions;
using System.Drawing;

namespace SobekCM.Bib_Package
{
    /// <summary> Delegate is used for the event which fires when a new wordmark is added 
    /// while loading an item into the library </summary>
    /// <param name="WordmarkID"> New wordmark ID </param>
    /// <param name="Name"> New wordmark code </param>
    /// <param name="URL"> URL for this wordmark </param>
    public delegate void New_Wordmark_Added_Delegate(int WordmarkID, string Name, string URL);

    /// <summary>Validates a METS file with SobekCM METS file standards</summary>
    /// <remarks> Object created by Ying Tang and Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    public class SobekCM_METS_Validator
    {
        private StringBuilder validationErrors;
        private SobekCM_Item thisBibPackage;
        private string packageDirName = String.Empty;
        private string iconBaseUrl = String.Empty;
        private const string verifyCheckSumType = "MD5";

        //public event New_Wordmark_Added_Delegate New_Wordmark_Added;

        /// <summary> Constructor for a new instance of the SobekCM_METS_Validator class</summary>
        public SobekCM_METS_Validator()
        {
            this.validationErrors = new StringBuilder();
            this.thisBibPackage = new SobekCM_Item();
        }

        /// <summary>Constructor for a new instance of the SobekCM_METS_Validator class</summary>
        /// <param name="thisPackage">Bibliographic package to validate </param>
        public SobekCM_METS_Validator(SobekCM_Item thisPackage)
        {
            this.validationErrors = new StringBuilder();
            thisBibPackage = thisPackage;
        }

        /// <summary>Constructor for a new instance of the SobekCM_METS_Validator class</summary>
        /// <param name="iconBaseUrl">Base URL to validate</param>
        public SobekCM_METS_Validator(string iconBaseUrl)
        {
            this.validationErrors = new StringBuilder();
            this.thisBibPackage = new SobekCM_Item();
            this.iconBaseUrl = iconBaseUrl;
        }

        /// <summary>Check if mets file exists for a SobekCM package.  </summary>
        /// <param name="dirName">Directory name</param>
        /// <returns>TRUE if METS exists, otherwise FALSE</returns>
        public bool METS_Existence_Check(string dirName)
        {
            this.validationErrors = new StringBuilder();
            string[] metsFile = Directory.GetFiles(dirName, "*.mets");
            string[] metsFile2 = Directory.GetFiles(dirName, "*.mets.xml");
            if ((metsFile.Length == 0) && (metsFile2.Length == 0))
            {
                this.validationErrors.Append("Cannot find valid METS file!" + "\n");
                return false;
            }
            if ((metsFile.Length + metsFile2.Length) > 1)
            {
                this.validationErrors.Append("There are more than one METS file!" + "\n");
                return false;
            }
            else
                return true;
        }

        /// <summary> Check the Bibliographic package agaist the UFDC METS standard</summary>
        /// <param name="bibPackagetoCheck">Package to validate</param>
        /// <param name="validateCheckSums">Flag indicates whether to check the checksums</param>
        /// <param name="packageDir">Directory for the package</param>
        /// <param name="Aggregation_Codes"> Aggregation codes to check against </param>
        /// <returns>TRUE is succesful, otherwise FALSE</returns>
        public bool UFDC_Standard_Check(SobekCM_Item bibPackagetoCheck, bool validateCheckSums, string packageDir, DataTable Aggregation_Codes)
        {
            bool returnVal = true;
            this.validationErrors = new StringBuilder();
            this.thisBibPackage = bibPackagetoCheck;
            this.packageDirName = packageDir;
            // check the BibID is in the right format
            if (SobekCM_Database.is_bibid_format(thisBibPackage.Bib_Info.BibID) == false)
            {
                this.validationErrors.Append("Invalid BibID" + "\n");
                returnVal = false;
            }
            // check the VID is in the right format
            if (SobekCM_Database.is_vids_format(thisBibPackage.Bib_Info.VID) == false)
            {
                this.validationErrors.Append("Invalid VID" + "\n");
                returnVal = false;

            }
            // check the title, it is a required field
            if (thisBibPackage.Bib_Info.Main_Title.Title.Length == 0)
            {
                this.validationErrors.Append("Item title is required but not supplied!" + "\n");
                returnVal = false;
            }

            //if (Aggregation_Codes != null)
            //{
            //    // check if the colleciton code is already in the SobekCM database
            //    foreach (string thisCollection in thisBibPackage.SobekCM_Web.Aggregations)
            //    {
            //        DataRow[] selected2 = Aggregation_Codes.Select("collectioncode = '" + thisCollection + "'");
            //        if (selected2.Length == 0)
            //        {
            //            this.validationErrors.Append("Invalid alternative collection code - " + thisCollection + "\n");
            //            returnVal = false;
            //        }
            //    }
            //}

            // check if the objid is the combination of BibID_VID
            if (thisBibPackage.METS.ObjectID != thisBibPackage.Bib_Info.BibID + "_" + thisBibPackage.Bib_Info.VID)
            {
                this.validationErrors.Append(" The METS OBJID does not match its BibID and VID" + "\n");
                returnVal = false;
            }


            ////			// check if subcollections are valid SobekCM collections
            ////			foreach( string thisCollection in thisBibPackage.Processing_Parameters.SubCollections)
            ////			{
            ////				if (SobekCM_Database.is_valid_collection(thisCollection) == false)
            ////				{
            ////					this.validationErrors.Append("Invalid sub-collection code - "+ thisCollection+ "\n");
            ////					returnVal = false;
            ////				}
            ////			}

            // check if record status is consistent with the SobekCM record
            //bool recordExist = true;
            //if (thisBibPackage.METS.RecordStatus == METS_Record_Status.NEW)
            //    recordExist = false;
            //if (recordExist != SobekCM_Database.does_item_exist(thisBibPackage.Bib_Info.BibID, thisBibPackage.Bib_Info.VID))
            //{
            //    this.validationErrors.Append("Record Status is not consistent with the record in SobekCM database!" + "\n");
            //    returnVal = false;
            //}

            // check if all files exist in the package and the MD5 checksum if the checksum flag is true		
            returnVal = returnVal && Check_Files(packageDir, validateCheckSums);


            // check if file sizes and MD5 checksums match with what indicate in the mets file 
            return returnVal;
        }


        /// <summary>Compare only the file group section between the new mets
        /// file and the old mets file.  Use the string comparision method to determine if the file 
        /// group sections have the same contents after removing all white spaces.</summary>
        /// <param name="newMetsFile">New METS file (METADATA_UPDATE)</param>
        /// <param name="oldMetsFile">Existing METS file</param>
        /// <returns>TRUE if valid, otherwise FALSE</returns>
        public bool METS_Update_Only_Check(string newMetsFile, string oldMetsFile)
        {
            // TEMPORARILY STOP THIS CHECK UNTIL THIS CHECK CAN BE REFINED
            return true;


            //Regex regex = new Regex(
            //    @"\s*",
            //    RegexOptions.IgnoreCase
            //    | RegexOptions.Multiline
            //    | RegexOptions.Compiled
            //    );
            //try
            //{

            //    this.validationErrors = new StringBuilder();

            //    StreamReader reader1 = new StreamReader(newMetsFile);
            //    string newMetsFileStr = reader1.ReadToEnd();
            //    reader1.Close();

            //    StreamReader reader2 = new StreamReader(oldMetsFile);
            //    string oldMetsFileStr = reader2.ReadToEnd();
            //    reader2.Close();

            //    int startIndex = newMetsFileStr.IndexOf("<METS:fileSec>");
            //    int endIndex = newMetsFileStr.LastIndexOf("</METS:fileSec>") + "</METS:fileSec>".Length;
            //    string newFileGroup = newMetsFileStr.Substring(startIndex, endIndex - startIndex);

            //    startIndex = oldMetsFileStr.IndexOf("<METS:fileSec>");
            //    endIndex = oldMetsFileStr.LastIndexOf("</METS:fileSec>") + "</METS:fileSec>".Length;
            //    string oldFileGroup = oldMetsFileStr.Substring(startIndex, endIndex - startIndex);

            //    string oldFileGroupStr = regex.Replace(newFileGroup, "");
            //    string newFileGroupStr = regex.Replace(oldFileGroup, "");

            //    if (oldFileGroupStr == newFileGroupStr)
            //        return true;
            //    else
            //    {
            //        this.validationErrors.Append("Files listed in this update METS file are different from the old METS file!");
            //        return false;
            //    }
            //}
            //catch
            //{
            //    this.validationErrors.Append("Error in comparing the new and old METS files!");
            //    return false;
            //}
        }

        #region Private Method

        /// <summary>Private method used to check if a icon file is a valid file</summary>
        /// <param name="iconUrl"></param>
        /// <returns></returns>
        private bool IsValidIconImage(string iconUrl)
        {
            System.Uri uriOfImage = new System.Uri(iconUrl);
            System.Drawing.Image objImage = null;
            System.Net.WebRequest objRequest = System.Net.HttpWebRequest.Create(uriOfImage);
            // Return true unless you found otherwise
            try
            {
                objImage =
                    System.Drawing.Image.FromStream(objRequest.GetResponse().GetResponseStream());
                return true;
            }
            catch
            {
                return false;
            }
        }


        /// <summary> Check if specified file exists in the search directory</summary>
        /// <param name="dirName">Directory name</param>
        /// <param name="matchCheckSums">Should there be checksum matching?</param>
        /// <returns>TRUE if successful, otherwise FALSE</returns>
        public bool Check_Files(string dirName, bool matchCheckSums)
        {
            bool returnVal = true;
            StringCollection fileToCheck = new StringCollection();
            fileToCheck.Add("jpg");
            fileToCheck.Add("txt");
            fileToCheck.Add("jp2");

            List<SobekCM_File_Info> packageFiles = thisBibPackage.Divisions.Files;
            foreach (SobekCM_File_Info thisFile in packageFiles)
            {
                if (fileToCheck.Contains(new FileInfo(thisFile.System_Name).Extension.Replace(".", "").Trim().ToLower()))
                {
                    if (File.Exists(dirName + "\\" + thisFile.System_Name) == false)
                    {
                        returnVal = false;
                        this.validationErrors.Append(thisFile.System_Name +
                            " is specified in the METS file but not included in the submission package!" + "\n");
                    }
                }

                // verify the checksums if necessary
                if (fileToCheck.Contains(new FileInfo(thisFile.System_Name).Extension.Replace(".", "").Trim().ToLower()))
                {
                    if (matchCheckSums)
                    {
                        if ((thisFile.Checksum_Type.Trim().ToUpper() == verifyCheckSumType) &&
                            (thisFile.Checksum.Length > 0))
                        {
                            string currentChecksum = (new FileMD5(dirName + "\\" + thisFile.System_Name)).Checksum;
                            if (thisFile.Checksum.Trim() != currentChecksum)
                            {
                                this.validationErrors.Append("The checksum for file: " + thisFile.System_Name +
                                    " does not match the checksum specified in the METS file!" + "\n");
                                returnVal = false;
                            }
                        }
                    }
                }
            }
            return returnVal;

        }
        #endregion

        #region public properties

        /// <summary> Gets the error string for the last METS file </summary>
        public string ValidationError
        {
            get { return this.validationErrors.ToString(); }
        }

        /// <summary> Gets and sets the base icon URL used to check for image existence </summary>
        public string Icon_Base_Url
        {
            get { return this.iconBaseUrl; }
            set { this.iconBaseUrl = value; }
        }


        #endregion

    }
}
