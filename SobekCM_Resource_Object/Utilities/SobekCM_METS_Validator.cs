#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Drawing;
using System.IO;
using System.Net;
using System.Text;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Divisions;

#endregion

namespace SobekCM.Resource_Object.Utilities
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
        private const string verifyCheckSumType = "MD5";
        private string iconBaseUrl = String.Empty;
        private string packageDirName = String.Empty;
        private SobekCM_Item thisBibPackage;
        private StringBuilder validationErrors;

        //public event New_Wordmark_Added_Delegate New_Wordmark_Added;

        /// <summary> Constructor for a new instance of the SobekCM_METS_Validator class</summary>
        public SobekCM_METS_Validator()
        {
            validationErrors = new StringBuilder();
            thisBibPackage = new SobekCM_Item();
        }

        /// <summary>Constructor for a new instance of the SobekCM_METS_Validator class</summary>
        /// <param name="thisPackage">Bibliographic package to validate </param>
        public SobekCM_METS_Validator(SobekCM_Item thisPackage)
        {
            validationErrors = new StringBuilder();
            thisBibPackage = thisPackage;
        }

        /// <summary>Constructor for a new instance of the SobekCM_METS_Validator class</summary>
        /// <param name="iconBaseUrl">Base URL to validate</param>
        public SobekCM_METS_Validator(string iconBaseUrl)
        {
            validationErrors = new StringBuilder();
            thisBibPackage = new SobekCM_Item();
            this.iconBaseUrl = iconBaseUrl;
        }

        /// <summary>Check if mets file exists for a SobekCM package.  </summary>
        /// <param name="dirName">Directory name</param>
        /// <returns>TRUE if METS exists, otherwise FALSE</returns>
        public bool METS_Existence_Check(string dirName)
        {
            validationErrors = new StringBuilder();
            string[] metsFile = Directory.GetFiles(dirName, "*.mets");
            string[] metsFile2 = Directory.GetFiles(dirName, "*.mets.xml");
            if ((metsFile.Length == 0) && (metsFile2.Length == 0))
            {
                validationErrors.Append("Cannot find valid METS file!" + "\n");
                return false;
            }
            if ((metsFile.Length + metsFile2.Length) > 1)
            {
                validationErrors.Append("There are more than one METS file!" + "\n");
                return false;
            }
            else
                return true;
        }

        /// <summary> Check the Bibliographic package agaist the SobekCM METS standard</summary>
        /// <param name="bibPackagetoCheck">Package to validate</param>
        /// <param name="packageDir">Directory for the package</param>
        /// <returns>TRUE is succesful, otherwise FALSE</returns>
        public bool SobekCM_Standard_Check(SobekCM_Item bibPackagetoCheck, string packageDir)
        {
            bool returnVal = true;
            validationErrors = new StringBuilder();
            thisBibPackage = bibPackagetoCheck;
            packageDirName = packageDir;

            // check the BibID is in the right format
            if (SobekCM_Item.is_bibid_format(thisBibPackage.Bib_Info.BibID) == false)
            {
                validationErrors.Append("Invalid BibID" + "\n");
                returnVal = false;
            }
            // check the VID is in the right format
            if (SobekCM_Item.is_vids_format(thisBibPackage.Bib_Info.VID) == false)
            {
                validationErrors.Append("Invalid VID" + "\n");
                returnVal = false;
            }
            // check the title, it is a required field
            if (thisBibPackage.Bib_Info.Main_Title.Title.Length == 0)
            {
                validationErrors.Append("Item title is required but not supplied!" + "\n");
                returnVal = false;
            }

            // check if the objid is the combination of BibID_VID
            if (thisBibPackage.METS_Header.ObjectID != thisBibPackage.Bib_Info.BibID + "_" + thisBibPackage.Bib_Info.VID)
            {
                validationErrors.Append("The METS OBJID does not match its BibID and VID" + "\n");
                returnVal = false;
            }

            // Validate the folder matches the package object id
            DirectoryInfo dirInfo = new DirectoryInfo(packageDir);
            string dirName = dirInfo.Name;
            if (dirName.Length < 16)
            {
                dirName = dirInfo.Parent.Name + "_" + dirName;
            }
            if ( !String.Equals(dirName, thisBibPackage.METS_Header.ObjectID, StringComparison.InvariantCultureIgnoreCase))
            {
                validationErrors.Append("The folder name and the METS OBJID do not match" + "\n");
                returnVal = false;
            }

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
            Uri uriOfImage = new Uri(iconUrl);
            Image objImage = null;
            WebRequest objRequest = HttpWebRequest.Create(uriOfImage);
            // Return true unless you found otherwise
            try
            {
                objImage =
                    Image.FromStream(objRequest.GetResponse().GetResponseStream());
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
            validationErrors = new StringBuilder();
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
                        validationErrors.Append(thisFile.System_Name +
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
                                validationErrors.Append("The checksum for file: " + thisFile.System_Name +
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
            get { return validationErrors.ToString(); }
        }

        /// <summary> Gets and sets the base icon URL used to check for image existence </summary>
        public string Icon_Base_Url
        {
            get { return iconBaseUrl; }
            set { iconBaseUrl = value; }
        }

        #endregion
    }
}