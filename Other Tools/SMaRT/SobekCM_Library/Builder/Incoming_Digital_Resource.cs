#region Using directives

using System;
using System.Collections.Generic;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using SobekCM.Resource_Object;
using SobekCM.Resource_Object.Bib_Info;
using SobekCM.Resource_Object.Database;
using SobekCM.Resource_Object.Divisions;
using SobekCM.Resource_Object.Metadata_File_ReaderWriters;

#endregion

namespace SobekCM.Library.Builder
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

        private SobekCM_Item bibPackage;
        private string bibid;
        private string fileRoot;
        private string metsfile;
        private DateTime packageTime;
        private string resourceFolder;
        private readonly Builder_Source_Folder sourceFolder;
        private Incoming_Digital_Resource_Type type;
        private string vid;

        /// <summary> Constructor for a new instance of the Incoming_Digital_Resource class </summary>
        /// <param name="Resource_Folder"> Folder for this incoming digital resource </param>
        /// <param name="Source_Folder"> Parent source folder </param>
        public Incoming_Digital_Resource(string Resource_Folder, Builder_Source_Folder Source_Folder )
        {
            type = Incoming_Digital_Resource_Type.UNKNOWN;
            resourceFolder = Resource_Folder;
            sourceFolder = Source_Folder;

            // Set some defaults
            bibid = String.Empty;
            vid = String.Empty;
            packageTime = DateTime.Now;

            fileRoot = "collect/image_files/";
        }

        /// <summary> Returns the object which contains all the metadata (bibliographic, structural, administrative) for the digital resource </summary>
        public SobekCM_Item Metadata
        {
            get
            {
                return bibPackage;
            }
        }

        /// <summary> Returns the information about the original source folder for this incoming digital resource </summary>
        public Builder_Source_Folder Source_Folder
        {
            get
            {
                return sourceFolder;
            }
        }

        /// <summary> Gets the file hashtable to allow checking for the file object from the METS
        /// file by the name of the file </summary>
        public Dictionary<string, SobekCM_File_Info> File_Hashtable
        {
            get
            {
                Dictionary<string, SobekCM_File_Info> returnValue = new Dictionary<string, SobekCM_File_Info>();
                // Now, step through each file in this mets and look for attributes in the other
                foreach (SobekCM_File_Info thisFile in bibPackage.Divisions.Files)
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
                if (bibPackage != null)
                {
                    bibPackage.Web.File_Root = value;
                }
                fileRoot = value;
            }
            get
            {
                return fileRoot;
            }
        }

        /// <summary> Gets or sets the size (in MBs) of all the files and metadata for this resource </summary>
        public double DiskSpace_KB
        {
            get
            {
                return bibPackage.DiskSize_KB;
            }
            set
            {
                bibPackage.DiskSize_KB = value;
            }
        }

        #region IComparable<Incoming_Digital_Resource> Members

        /// <summary> Compares this incoming digital resource to another digital resource for sorting purposes </summary>
        /// <param name="other"> Incoming digital resource object to compare this to </param>
        /// <returns> Value indicating how to sort these </returns>
        /// <remarks> This sort is a simple compare based on bibliographic identifier and volume identifier </remarks>
        public int CompareTo(Incoming_Digital_Resource other)
        {
            string thisObjectId = bibid + "_" + vid;
            string thatObjectId = other.BibID + "_" + other.VID;

            return thisObjectId.CompareTo(thatObjectId);
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
                bibPackage = SobekCM_Item.Read_METS(Source_File);

                // If null was returned, this failed
                if (bibPackage == null)
                    return false;

                // TEMPORARY
                foreach (Identifier_Info thisIdentifier in bibPackage.Bib_Info.Identifiers)
                {
                    if (thisIdentifier.Type == "accn")
                        thisIdentifier.Type = "accession number";
                }

                // Save the BibID and VID.  If a VID already existed here, and not in the METS,
                // assign that to the METS.  (For example, when '00001' can be assumed
                bibid = bibPackage.BibID;
                if (bibPackage.VID.Length > 0)
                    vid = bibPackage.VID;
                else if ( !String.IsNullOrEmpty(vid))
                    bibPackage.VID = vid;

                switch (bibPackage.METS_Header.RecordStatus_Enum)
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
            bibPackage = null;
        }

        /// <summary> Load all the file attributes into this wrapper class </summary>
        /// <param name="Directory"> Directory to find the metadata and files </param>
        /// <remarks> This reads the width and height of all the page image files and stores this 
        /// in the internal SobekCM_Item object </remarks>
        public void Load_File_Attributes( string Directory )
        {
            Load_File_Attributes(Directory, Directory);
        }

        /// <summary> Load all the file attributes into this wrapper class </summary>
        /// <param name="Metadata_Location"> Directory to find the metadata for this package </param>
        /// <param name="File_Location"> Directory to fine the files for this package </param>
        /// <remarks> This reads the width and height of all the page image files and stores this 
        /// in the internal SobekCM_Item object </remarks>
        public void Load_File_Attributes(string Metadata_Location, string File_Location )
        {
            // First, check to see if there is an existing service METS
            if (( Metadata_Location.Length > 0 ) && (Directory.Exists(Metadata_Location)) && (File.Exists(Metadata_Location + "/" + bibid + "_" + vid + ".mets.xml")))
            {
                try
                {
                    SobekCM_Item serviceMETS = SobekCM_Item.Read_METS(Metadata_Location + "/" + bibid + "_" + vid + ".mets.xml");

                    // Create a hashtable of all the files in the service METS and tep through each file in this mets and look for attributes in the other
                    Dictionary<string, SobekCM_File_Info> serviceMetsFiles = serviceMETS.Divisions.Files.ToDictionary(thisFile => thisFile.System_Name);

                    // Now, step through each file in this mets and look for attributes in the other
                    foreach (SobekCM_File_Info thisFile in bibPackage.Divisions.Files)
                    {
                        // Is there a match?
                        if (serviceMetsFiles.ContainsKey(thisFile.System_Name))
                        {
                            // Get the match
                            SobekCM_File_Info serviceFile = serviceMetsFiles[thisFile.System_Name];

                            // Copy the data over
                            thisFile.Width = serviceFile.Width;
                            thisFile.Height = serviceFile.Height;
                            thisFile.System_Name = serviceFile.System_Name;
                        }
                    }
                }
                catch(Exception)
                {
                    // No need to do anything here.. can still function without this information being saved
                }
            }

            // Now, just look for the data being present in each file
            if (Directory.Exists(File_Location))
            {
                foreach (SobekCM_File_Info thisFile in bibPackage.Divisions.Files)
                {
                    // Is this a jpeg?
                    if (thisFile.System_Name.ToUpper().IndexOf(".JPG") > 0)
                    {
                        if ( thisFile.System_Name.ToUpper().IndexOf("THM.JPG") < 0 )
                            Compute_Jpeg_Attributes(thisFile, File_Location);
                    }

                    // Is this a jpeg2000?
                    if (thisFile.System_Name.ToUpper().IndexOf("JP2") > 0 )
                    {
                        Compute_Jpeg2000_Attributes(thisFile, File_Location);
                    }
                }
            }
        }

        /// <summary> Computes the attributes (width, height) for a JPEG file </summary>
        /// <param name="JPEG_File"> METS SobekCM_File_Info object for this jpeg file </param>
        /// <param name="File_Location"> Location where this file exists </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> The attribute information is computed and then stored in the provided METS SobekCM_File_Info object </remarks>
        public bool Compute_Jpeg_Attributes(SobekCM_File_Info JPEG_File, string File_Location)
        {
            // If the width and height are already determined, done!
            if ((JPEG_File.Width > 0) && (JPEG_File.Height > 0))
                return true;

            // Does this file exist?
            if (File.Exists(File_Location + "/" + JPEG_File.System_Name))
            {
                try
                {
                    // Get the height and width of this JPEG file
                    Bitmap image = (Bitmap)Image.FromFile(File_Location + "/" + JPEG_File.System_Name);
                    JPEG_File.Width = (ushort) image.Width;
                    JPEG_File.Height = (ushort) image.Height;
                    image.Dispose();
                    return true;
                }
                catch
                {
                    return false;
                }
            }

            return false;
        }

        /// <summary> Computes the attributes (width, height) for a JPEG2000 file </summary>
        /// <param name="JPEG2000_File"> METS SobekCM_File_Info object for this jpeg2000 file </param>
        /// <param name="File_Location"> Location where this file exists </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> The attribute information is computed and then stored in the provided METS SobekCM_File_Info object </remarks>
        public bool Compute_Jpeg2000_Attributes(SobekCM_File_Info JPEG2000_File, string File_Location)
        {
            // If the width and height are already determined, done!
            if ((JPEG2000_File.Width > 0) && (JPEG2000_File.Height > 0) && (JPEG2000_File.System_Name.Length > 0))
                return true;

            // Does this file exist?
            if (File.Exists(File_Location + "/" + JPEG2000_File.System_Name))
            {
                return get_attributes_from_jpeg2000(JPEG2000_File, File_Location + "/" + JPEG2000_File.System_Name);
            }

            if ((JPEG2000_File.System_Name.Length > 0) && (File.Exists(JPEG2000_File.System_Name)))
            {
                return get_attributes_from_jpeg2000(JPEG2000_File, JPEG2000_File.System_Name);
            }

            return false;
        }

        private bool get_attributes_from_jpeg2000(SobekCM_File_Info JPEG2000_File, string file)
        {
            try
            {
                // Get the height and width of this JPEG file
                FileStream reader = new FileStream(file, FileMode.Open, FileAccess.Read);
                int[] previousValues = new[] { 0, 0, 0, 0 };
                int bytevalue = reader.ReadByte();
                int count = 1;
                while (bytevalue != -1)
                {
                    // Move this value into the array
                    previousValues[0] = previousValues[1];
                    previousValues[1] = previousValues[2];
                    previousValues[2] = previousValues[3];
                    previousValues[3] = bytevalue;

                    // Is this IHDR?
                    if ((previousValues[0] == 105) && (previousValues[1] == 104) &&
                        (previousValues[2] == 100) && (previousValues[3] == 114))
                    {
                        break;
                    }
                    
                    // Is this the first four bytes and does it match the output from Kakadu 3-2?
                    if ((count == 4) && (previousValues[0] == 255) && (previousValues[1] == 79) &&
                        (previousValues[2] == 255) && (previousValues[3] == 81))
                    {
                        reader.ReadByte();
                        reader.ReadByte();
                        reader.ReadByte();
                        reader.ReadByte();
                        break;
                    }
                    
                    // Read the next byte
                    bytevalue = reader.ReadByte();
                    count++;
                }

                // Now, read ahead for the height and width
                JPEG2000_File.Height = (ushort) ((((((reader.ReadByte() * 256) + reader.ReadByte()) * 256) + reader.ReadByte()) * 256) + reader.ReadByte());
                JPEG2000_File.Width = (ushort) ((((((reader.ReadByte() * 256) + reader.ReadByte()) * 256) + reader.ReadByte()) * 256) + reader.ReadByte());
                reader.Close();

                return true;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary> Saves the SobekCM Service METS file for this incoming digital resource </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_SobekCM_Service_METS()
        {
            try
            {

                bibPackage.Save_SobekCM_METS();
                return true;
            }
            catch 
            {
                return false;
            }
        }

        /// <summary> Saves the citation-only METS file for this incoming digital resource </summary>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        /// <remarks> This is used for displaying the full citation information in the browse/search FULL VIEW </remarks>
        public bool Save_Citation_METS()
        {
            try
            {
                bibPackage.Save_Citation_Only_METS();
                return true;
            }
            catch
            {
                return false;
            }
        }

        /// <summary> Creates the static HTML file for this incoming digital resource </summary>
        /// <param name="staticBuilder"> Builder object helps to build the static pages </param>
        /// <returns> The name (including directory) for the resultant static html page </returns>
        public string Save_Static_HTML(Static_Pages_Builder staticBuilder)
        {
            try
            {
                staticBuilder.Item_List.Add_SobekCM_Item(bibPackage);
                string filename = Resource_Folder + "\\" + bibPackage.BibID + "_" + bibPackage.VID + ".html";
                staticBuilder.Create_Item_Citation_HTML(bibPackage.BibID, bibPackage.VID, filename, resourceFolder);

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
                string greenstoneLink = SobekCM_Library_Settings.Image_URL;
                bibPackage.Web.Image_Root = greenstoneLink + bibPackage.Web.File_Root.Replace("\\", "/");
                if (bibPackage.Web.Image_Root.IndexOf("/" + vid) < 0)
                    bibPackage.Web.Image_Root = bibPackage.Web.Image_Root + "/" + vid;
                bibPackage.Web.Set_BibID_VID(bibPackage.BibID, bibPackage.VID);


                List<string> collectionnames = new List<string>();
                // Get the collection names
                if ((bibPackage.Behaviors.Aggregation_Count > 0) && ( Collection_Codes != null ))
                {
                    collectionnames.AddRange(from aggregation in bibPackage.Behaviors.Aggregations select aggregation.Code into altCollection select Collection_Codes.Select("collectioncode = '" + altCollection + "'") into altCode where altCode.Length > 0 select altCode[0]["ShortName"].ToString());
                }

                // Save the marc xml file
                MarcXML_File_ReaderWriter marcWriter = new MarcXML_File_ReaderWriter();
                string Error_Message;
                Dictionary<string, object> options = new Dictionary<string, object>();
                options["MarcXML_File_ReaderWriter:Additional_Tags"] = bibPackage.MARC_Sobek_Standard_Tags(collectionnames, true, SobekCM_Library_Settings.System_Name, SobekCM_Library_Settings.System_Abbreviation);
                return marcWriter.Write_Metadata(bibPackage.Source_Directory + "\\marc.xml", bibPackage, options, out Error_Message);

            }
            catch
            {
                return false;
            }
        }

        /// <summary> Saves this item to the SobekCM database </summary>
        /// <param name="Item_List"> Item list datatable for pulling out the original creation date for this resource </param>
        /// <param name="New_Item"> Flag indicates this is an entirely new item </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Save_to_Database(DataTable Item_List, bool New_Item)
        {
            if (bibPackage == null)
                return false;

            try
            {

                // save the bib package to the SobekCM database
                bool existed = false;
                DateTime createTime = packageTime;
                DataRow[] check = Item_List.Select("BibID='" + bibid + "' and VID='" + vid + "'");
                if (check.Length > 0)
                {
                    existed = true;
                    if (Item_List.Columns.Contains("CreateDate"))
                        createTime = Convert.ToDateTime(check[0]["CreateDate"]);
                }

                // Set the file root again
                bibPackage.Web.File_Root = fileRoot;
                SobekCM_Database.Save_Digital_Resource(bibPackage, createTime, existed);

                // Save the behaviors if this is a new item
                if (!existed)
                {
                    // Some work here just in case the METS is missing stuff, or has old data

                    // Make sure not set to UFDC as only web skin by default (used to list UFDC on all METS files )
                    if ((bibPackage.Behaviors.Web_Skin_Count == 1) && (bibPackage.Behaviors.Web_Skins[0].ToUpper().Trim() == "UFDC"))
                        bibPackage.Behaviors.Clear_Web_Skins();

                    // Now, save the behaviors for this item
                    SobekCM_Database.Save_Behaviors(bibPackage, false, false);
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

        private void delete_directory(string directory)
        {
            string[] files = Directory.GetFiles(directory);
            foreach (string thisFile in files)
            {
                File.Delete(thisFile);
            }

            string[] subdirs = Directory.GetDirectories(directory);
            foreach (string thisSubDir in subdirs)
            {
                delete_directory(thisSubDir);
            }

            Directory.Delete(directory);
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
                return SobekCM_Library_Settings.System_Base_URL + "/" + bibPackage.BibID + "/" + bibPackage.VID;
            }
        }

        /// <summary> Gets the age in TICKS of this resource folder and files </summary>
        /// <remarks> This is read from the last write time for the folder </remarks>
        public long Age_in_Ticks
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
                if (metsFiles.Length == 0)
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

        ///// <summary> Gets the METS Record status associated with this incoming digital resource, as an enumerational value  </summary>
        //public METS_Record_Status METS_Type
        //{
        //    get
        //    {
        //        return metsType;
        //    }
        //    set
        //    {
        //        metsType = value;
        //    }
        //}

        /// <summary> Gets the METS Record status associated with this incoming digital resource, as a string </summary>
        public string METS_Type_String
        {
            get
            {
                if (bibPackage == null)
                    return "NULL";

                switch (bibPackage.METS_Header.RecordStatus_Enum)
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
        /// <param name="destination_directory"> New location for this incoming digital resource </param>
        /// <returns> TRUE if successful, otherwise FALSE </returns>
        public bool Move(string destination_directory)
        {
            try
            {
                // Make sure the destination directory exists
                if (!Directory.Exists(destination_directory))
                    Directory.CreateDirectory(destination_directory);

                // Determine if this directory needs to be flattened.
                DirectoryInfo dirInfo = new DirectoryInfo(resourceFolder);
                string destFolder = destination_directory + dirInfo.Name;
                string parentDirectory = String.Empty;

                // Does this directory appear to be a VID folder?
                if (dirInfo.Name.ToUpper().Replace("VID", "").Length == 5)
                {
                    // Is the parent directory 10 characters long, which would imply a Bib ID?
                    string bibidCheck = Directory.GetParent(resourceFolder).Name;
                    if (bibidCheck.Length == 10)
                    {
                        parentDirectory = Directory.GetParent(resourceFolder).FullName;
                        destFolder = destination_directory + bibidCheck + "_" + dirInfo.Name.ToUpper().Replace("VID", "");
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
                if (parentDirectory.Length > 0)
                {
                    if ((Directory.GetDirectories(parentDirectory).Length == 0) && (Directory.GetFiles(parentDirectory).Length == 0))
                    {
                        try
                        {
                            Directory.Delete(parentDirectory);
                        }
                        catch(Exception)
                        {
                            // If unable to delete the directory, not the worst thing
                        }
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
