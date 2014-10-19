#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using SobekCM.Engine_Library.ApplicationState;
using SobekCM.Library;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;

#endregion

namespace SobekCM.Builder_Library
{
    /// <summary> Class is a wrapper around a SobekCM_Item digital object, which allows for commonly
    /// executed processes to assist with the SobekCM Builder </summary>
    public class Incoming_Digital_Resource : IComparable<Incoming_Digital_Resource>
    {
        #region Incoming_Digital_Resource_Type enum

        /// <summary> Enumerational value indicates the type of incoming digital resource </summary>
        /// <remarks> This is necessary, rather than depending on the METS Record Status type, 
        /// because folders could also be accepted with just files, without a METS file </remarks>
        public enum Incoming_Digital_Resource_Type
        {
            /// <summary> Type of resource is unknown, unrecognized, or not yet computed </summary>
            UNKNOWN = -1,

            /// <summary> Complete packages contain all the metadata and image resource files for loading into a SobekCM library </summary>
            COMPLETE_PACKAGE = 1,

            /// <summary> Partial packages have a METS file, but may not necessarily have every resource file, as some may already exist in the 
            /// SobekCM library's resource folder </summary>
            PARTIAL_PACKAGE,

            /// <summary> Metadata updates contain just the METS file with updated metadata for loading into the library </summary>
            METADATA_UPDATE,

            /// <summary> Bib-level METS files are generally deprecated </summary>
            BIB_LEVEL,

            /// <summary> A simple folder of files may not contain a METS file but will still be loaded into the library's resource folder </summary>
            FOLDER_OF_FILES,

            /// <summary> Delete METS will cause the digital resource in the library to be deleted, and the files to be moved to a special DELETED folder for review </summary>
            DELETE
        }

        #endregion

	    private string bibid;
        private string fileRoot;
        private string metsfile;
        private DateTime packageTime;
        private string resourceFolder;
	    private Incoming_Digital_Resource_Type type;
        private string vid;
	    private string metsTypeOverride;
 
        /// <summary> Constructor for a new instance of the Incoming_Digital_Resource class </summary>
        /// <param name="Resource_Folder"> Folder for this incoming digital resource </param>
        /// <param name="Source_Folder"> Parent source folder </param>
        public Incoming_Digital_Resource(string Resource_Folder, Actionable_Builder_Source_Folder Source_Folder )
        {
            type = Incoming_Digital_Resource_Type.UNKNOWN;
            resourceFolder = Resource_Folder;
            this.Source_Folder = Source_Folder;

            // Set some defaults
            bibid = String.Empty;
            vid = String.Empty;
            packageTime = DateTime.Now;
	        metsTypeOverride = String.Empty;
            NewImageFiles = new List<string>();
            NewPackage = false;

            fileRoot = "collect/image_files/";
        }

	    /// <summary> Returns the object which contains all the metadata (bibliographic, structural, administrative) for the digital resource </summary>
	    public SobekCM_Item Metadata { get; private set; }

	    /// <summary> Returns the information about the original source folder for this incoming digital resource </summary>
        public Actionable_Builder_Source_Folder Source_Folder { get; private set; }

		/// <summary> Primary key for the main builder log entry for this item </summary>
		public long BuilderLogId { get; set;  }

        /// <summary> List of new image files  </summary>
        public List<string> NewImageFiles { get; set; }

        /// <summary> Flag indicates if this a brand new item  </summary>
        public bool NewPackage { get; set; }

	    /// <summary> Gets the file hashtable to allow checking for the file object from the METS
        /// file by the name of the file </summary>
        public Dictionary<string, SobekCM_File_Info> File_Hashtable
        {
            get
            {
                Dictionary<string, SobekCM_File_Info> returnValue = new Dictionary<string, SobekCM_File_Info>();
                // Now, step through each file in this mets and look for attributes in the other
                foreach (SobekCM_File_Info thisFile in Metadata.Divisions.Files)
                {
                    returnValue[thisFile.System_Name] = thisFile;
                }
                return returnValue;
            }
        }

        /// <summary> Web file root for this incoming digital resource </summary>
        public string File_Root
        {
            set
            {
                if (Metadata != null)
                {
                    Metadata.Web.File_Root = value;
                }
                fileRoot = value;
            }
            get
            {
                return fileRoot;
            }
        }

        /// <summary> Gets or sets the size (in MBs) of all the files and metadata for this resource </summary>
        public double DiskSpaceMb
        {
            get
            {
                return Metadata.DiskSize_MB;
            }
            set
            {
                Metadata.DiskSize_MB = value;
            }
        }

        #region IComparable<Incoming_Digital_Resource> Members

        /// <summary> Compares this incoming digital resource to another digital resource for sorting purposes </summary>
        /// <param name="Other"> Incoming digital resource object to compare this to </param>
        /// <returns> Value indicating how to sort these </returns>
        /// <remarks> This sort is a simple compare based on bibliographic identifier and volume identifier </remarks>
        public int CompareTo(Incoming_Digital_Resource Other)
        {
            string thisObjectId = bibid + "_" + vid;
            string thatObjectId = Other.BibID + "_" + Other.VID;

            return String.Compare(thisObjectId, thatObjectId, StringComparison.OrdinalIgnoreCase);
        }

        #endregion

        /// <summary> Read the METS file and load the data into this object </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Load_METS()
        {
            return Load_METS(METS_File);
        }

        /// <summary> Read the METS file and load the data into this object </summary>
        /// <param name="Source_File"> METS source file to read </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Load_METS(string Source_File)
        {
            try
            {
                // Load the METS file
                Metadata = SobekCM_Item.Read_METS(Source_File);

                // If null was returned, this failed
                if (Metadata == null)
                    return false;

                // TEMPORARY
                foreach (Identifier_Info thisIdentifier in Metadata.Bib_Info.Identifiers)
                {
                    if (thisIdentifier.Type == "accn")
                        thisIdentifier.Type = "accession number";
                }

                // Save the BibID and VID.  If a VID already existed here, and not in the METS,
                // assign that to the METS.  (For example, when '00001' can be assumed
                bibid = Metadata.BibID;
                if (Metadata.VID.Length > 0)
                    vid = Metadata.VID;
                else if ( !String.IsNullOrEmpty(vid))
                    Metadata.VID = vid;

                switch (Metadata.METS_Header.RecordStatus_Enum)
                {
                    case METS_Record_Status.METADATA_UPDATE:
                        type = Incoming_Digital_Resource_Type.METADATA_UPDATE;
                        break;

                    case METS_Record_Status.COMPLETE:
                        type = Incoming_Digital_Resource_Type.COMPLETE_PACKAGE;
                        break;

                    case METS_Record_Status.PARTIAL:
                        type = Incoming_Digital_Resource_Type.PARTIAL_PACKAGE;
                        break;

                    case METS_Record_Status.DELETE:
                        type = Incoming_Digital_Resource_Type.DELETE;
                        break;

                    case METS_Record_Status.BIB_LEVEL:
                        type = Incoming_Digital_Resource_Type.BIB_LEVEL;
                        break;
                }

                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        /// <summary> Clear the internally wrapped SobekCM_Item object </summary>
        /// <remarks> This is called to clear the memory in use by the object </remarks>
        public void Clear_METS()
        {
            Metadata = null;
        }
 

        /// <summary> Saves the SobekCM Service METS file for this incoming digital resource </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_SobekCM_Service_METS()
        {
            try
            {

                Metadata.Save_SobekCM_METS();
                return true;
            }
            catch 
            {
                return false;
            }
        }


        /// <summary> Creates the static HTML file for this incoming digital resource </summary>
        /// <param name="StaticBuilder"> Builder object helps to build the static pages </param>
        /// <returns> The name (including directory) for the resultant static html page </returns>
        public string Save_Static_HTML(Static_Pages_Builder StaticBuilder)
        {
            try
            {
                if (!Directory.Exists(Resource_Folder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name))
                    Directory.CreateDirectory(Resource_Folder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name);

                string filename = Resource_Folder + "\\" + Engine_ApplicationCache_Gateway.Settings.Backup_Files_Folder_Name + "\\" + Metadata.BibID + "_" + Metadata.VID + ".html";
                StaticBuilder.Create_Item_Citation_HTML(Metadata, filename, resourceFolder);

                return filename;
            }
            catch
            {
                return String.Empty;
            }
        }

        /// <summary> Saves the MarcXML file, used for creating MARC feeds, for this incoming digital resource </summary>
        /// <param name="Collection_Codes"> Collection codes to include in the resultant MarcXML file </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_MARC_XML( DataTable Collection_Codes )
        {
            try
            {
                // Set the image location
                Metadata.Web.Image_Root = Engine_ApplicationCache_Gateway.Settings.Image_URL + Metadata.Web.File_Root.Replace("\\", "/");
                Metadata.Web.Set_BibID_VID(Metadata.BibID, Metadata.VID);


                List<string> collectionnames = new List<string>();
                // Get the collection names
                if ((Metadata.Behaviors.Aggregation_Count > 0) && ( Collection_Codes != null ))
                {
                    collectionnames.AddRange(from aggregation in Metadata.Behaviors.Aggregations select aggregation.Code into altCollection select Collection_Codes.Select("collectioncode = '" + altCollection + "'") into altCode where altCode.Length > 0 select altCode[0]["ShortName"].ToString());
                }

                // Save the marc xml file
                MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                string errorMessage;
                Dictionary<string, object> options = new Dictionary<string, object>();
                options["MarcXML_File_ReaderWriter:Additional_Tags"] = Metadata.MARC_Sobek_Standard_Tags(collectionnames, true, Engine_ApplicationCache_Gateway.Settings.System_Name, Engine_ApplicationCache_Gateway.Settings.System_Abbreviation);
                return marcWriter.Write_Metadata(Metadata.Source_Directory + "\\marc.xml", Metadata, options, out errorMessage);

            }
            catch
            {
                return false;
            }
        }

        /// <summary> Saves this item to the SobekCM database </summary>
         /// <param name="New_Item"> Flag indicates this is an entirely new item </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_to_Database(bool New_Item)
        {
            if (Metadata == null)
                return false;

            try
            {

                // save the bib package to the SobekCM database
                bool existed = !New_Item;
                DateTime createTime = packageTime;

                // Set the file root again
                Metadata.Web.File_Root = fileRoot;
                SobekCM_Database.Save_Digital_Resource(Metadata, createTime, existed);

                // Save the behaviors if this is a new item
                if (!existed)
                {
                    // Some work here just in case the METS is missing stuff, or has old data

                    // Make sure not set to UFDC as only web skin by default (used to list UFDC on all METS files )
                    if ((Metadata.Behaviors.Web_Skin_Count == 1) && (Metadata.Behaviors.Web_Skins[0].ToUpper().Trim() == "UFDC"))
                        Metadata.Behaviors.Clear_Web_Skins();

                    // Now, save the behaviors for this item
                    SobekCM_Database.Save_Behaviors(Metadata, false, false);
                }

                //// Set the suppress endeca flag
                //if ((New_Item) && (!bibPackage.Behaviors.Suppress_Endeca))
                //{
                //    SobekCM.Library.Database.SobekCM_Database.Set_Endeca_Flag(bibPackage.BibID, bibPackage.Behaviors.Suppress_Endeca);
                //}
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        #region Method to delete this package

        /// <summary> Deletes the folder and all content within the directory or subdirectories </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This does not process any delete against the resource in the database or indexes, but 
        /// rather just deletes the folder where the incoming digital resource is housed </remarks>
        public bool Delete()
        {
            try
            {
                delete_directory(resourceFolder);
                return true;
            }
            catch
            {
                return false;
            }
        }

        private static void delete_directory(string Directory)
        {
            string[] files = System.IO.Directory.GetFiles(Directory);
            foreach (string thisFile in files)
            {
	            try
	            {
					File.Delete(thisFile);
	            }
	            catch { }
            }

            string[] subdirs = System.IO.Directory.GetDirectories(Directory);
            foreach (string thisSubDir in subdirs)
            {
                delete_directory(thisSubDir);
            }

            System.IO.Directory.Delete(Directory);
        }

        #endregion

        #region Public Properties

        /// <summary> Gets the name of the resource folder itself, usually BibID_VID format </summary>
        public string Folder_Name
        {
            get { return (new DirectoryInfo(resourceFolder)).Name; }
        }

        /// <summary> Gets the complete directory information for the resource folder  </summary>
        public string Resource_Folder
        {
            get { return resourceFolder; }
            set { resourceFolder = value; }
        }

        /// <summary> Gets the time associated with this package, which is the usually the 
        /// last write/modify time for files within the folder </summary>
        public DateTime Package_Time
        {
            get { return packageTime; }
            set { packageTime = value; }
        }

        /// <summary> Gets the type of incoming digital resource request, (i.e., Metadata update, complete new package, delete, etc.. ) </summary>
        public Incoming_Digital_Resource_Type Resource_Type
        {
            get { return type; }
            set { type = value; }
        }

        /// <summary> Gets the Bibliographic Identifier (BibID) for this incoming digital resource </summary>
        public string BibID
        {
            get { return bibid; }
            set { bibid = value; }
        }

        /// <summary> Gets the Volume Identifier (VID) for this incoming digital resource </summary>
        public string VID
        {
            get { return vid; }
            set { vid = value; }
        }

        /// <summary> Gets the permanent link associated with this incoming digital resource </summary>
        public string Permanent_Link
        {
            get
            {
                return Engine_ApplicationCache_Gateway.Settings.System_Base_URL + "/" + Metadata.BibID + "/" + Metadata.VID;
            }
        }

        /// <summary> Gets the age in TICKS of this resource folder and files </summary>
        /// <remarks> This is read from the last write time for the folder </remarks>
        public long AgeInTicks
        {
            get { return DateTime.Now.Ticks - Directory.GetLastWriteTime(resourceFolder).Ticks; }
        }

        /// <summary> Returns the METS file name (and directory) associated with this incoming 
        /// digital resource </summary>
        public string METS_File
        {
            get
            {
                // If there is a METS file specified here, use that one
                if ( !String.IsNullOrEmpty(metsfile))
                {
                    if (File.Exists(resourceFolder + "\\" + metsfile))
                        return resourceFolder + "\\" + metsfile;
                }

                // If there is a bibid and vid, use those
                if ((!String.IsNullOrEmpty(bibid)) && (!String.IsNullOrEmpty(vid)))
                {
                    if (File.Exists(resourceFolder + "\\" + bibid + "_" + vid + ".mets.xml"))
                        return resourceFolder + "\\" + bibid + "_" + vid + ".mets.xml";
                }

                // Look for any .mets.xml file
                string[] metsFiles = Directory.GetFiles(resourceFolder, "*.mets.xml");
                if (metsFiles.Length > 0)
                    return metsFiles[0];

                // Finally, just use any old mets file
                string[] metsFiles2 = Directory.GetFiles(resourceFolder, "*.mets*");
                return metsFiles2.Length == 0 ? String.Empty : metsFiles2[0];
            }
            set
            {
                metsfile = value;
            }
        }

        /// <summary> Gets the METS Record status associated with this incoming digital resource, as a string </summary>
        public string METS_Type_String
        {
            get
            {
	            if (metsTypeOverride.Length > 0)
		            return metsTypeOverride;

                if (Metadata == null)
                    return "NULL";

                switch (Metadata.METS_Header.RecordStatus_Enum)
                {
                    case METS_Record_Status.BIB_LEVEL:
                        return "Bib Level";
                    case METS_Record_Status.METADATA_UPDATE:
                        return "Metadata Update";
                    case METS_Record_Status.COMPLETE:
                        return "Complete";
                    case METS_Record_Status.PARTIAL:
                        return "Partial";
                    case METS_Record_Status.DELETE:
                        return "Delete";
                    default:
                        return "unknown";
                }
            }
			set { metsTypeOverride = value; }
        }

        /// <summary> Flag indicates this is a METS only type package, which should not have any associated
        /// digital resource file, other than metadata </summary>
        public bool METS_Only_Package
        {
            get
            {
                // Has this already been determined?
                if (type != Incoming_Digital_Resource_Type.UNKNOWN)
                {
                    switch (type)
                    {
                        case Incoming_Digital_Resource_Type.METADATA_UPDATE:
                            return true;

                        default:
                            return false;
                    }
                }

                // Is there just a mets file?
                string[] files = Directory.GetFiles(resourceFolder);
                if ((files.Length == 1) && (files[0].ToUpper().IndexOf(".METS") > 0))
                {
                    // Only a METS file, but is this METS file a DELETE or METADATA_UPDATE?
                    int lineCount = 1;
                    StreamReader reader = new StreamReader(files[0]);
                    string line = reader.ReadLine();
                    while ((line != null) && (lineCount < 50))
                    {
                        if (line.ToUpper().IndexOf("RECORDSTATUS") >= 0)
                        {
                            if (line.ToUpper().IndexOf("METADATA_UPDATE") >= 0)
                            {
                                if (line.ToUpper().IndexOf("METADATA_UPDATE") > 0)
                                {
                                    type = Incoming_Digital_Resource_Type.METADATA_UPDATE;
                                }
                                return true;
                            }
                            
                            type = Incoming_Digital_Resource_Type.COMPLETE_PACKAGE;
                            return false;
                        }
                        
                        line = reader.ReadLine();
                        lineCount++;
                    }
                }

                type = Incoming_Digital_Resource_Type.COMPLETE_PACKAGE;
                return false;
            }
        }

        #endregion

        #region Method to move this package

        /// <summary> Moves the incoming digital resource folder, along with all files and subdirectories, to a new location </summary>
        /// <param name="DestinationDirectory"> New location for this incoming digital resource </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Move(string DestinationDirectory)
        {
            try
            {
                // Make sure the destination directory exists
                if (!Directory.Exists(DestinationDirectory))
                    Directory.CreateDirectory(DestinationDirectory);

                // Determine if this directory needs to be flattened.
                DirectoryInfo dirInfo = new DirectoryInfo(resourceFolder);
                string destFolder = DestinationDirectory + dirInfo.Name;

                // Does this directory appear to be a VID folder?
                if (dirInfo.Name.ToUpper().Replace("VID", "").Length == 5)
                {
                    // Is the parent directory 10 characters long, which would imply a Bib ID?
                    string bibidCheck = Directory.GetParent(resourceFolder).Name;
                    if (bibidCheck.Length == 10)
                    {
                        destFolder = DestinationDirectory + bibidCheck + "_" + dirInfo.Name.ToUpper().Replace("VID", "");
                    }
                    else if (bibidCheck.Length == 2)
                    {
                        // Put in special code for directories dropped in builder from resource folder
                        // That is, look for the pair-tree format
                        string check = bibidCheck;
                        int count = 0;
                        while ((Directory.GetParent(bibidCheck) != null) && (count < 4))
                        {
                            string parent = Directory.GetParent(bibidCheck).Name;
                            if (parent.Length != 2)
                            {
                                check = String.Empty;
                                break;
                            }

                            check = check + parent;
                            count++;
                        }

                        if (check.Length == 10)
                        {
                            destFolder = DestinationDirectory + check + "_" + dirInfo.Name.ToUpper().Replace("VID", "");
                         }
                    }
                }

                // If the destination directory exists, delete it
                if (Directory.Exists(destFolder))
                {
                    Directory.Delete(destFolder, true);
                }

                // Move this directory
                Directory.Move(resourceFolder, destFolder);
                resourceFolder = destFolder;

                // If the parent directory is empty, try to delete it
                string parentDir = resourceFolder;
                while ((Directory.GetParent(parentDir) != null) && (Directory.GetParent(parentDir).GetFiles().Length == 0))
                {
                    parentDir = Directory.GetParent(parentDir).FullName;
                    try
                    {
                        Directory.Delete(parentDir);
                    }
                    catch (Exception)
                    {
                        // If unable to delete the directory, not the worst thing
                    }
                }

                return true;
            }
            catch
            {
                return false;
            }
        }

        #endregion
    }
}
