#region Using directives

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Text;
using System.Xml;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules.Maps
{
    /// <summary> Map information object holds all the related information about features, corporations, and personal
    /// names which are associated with sheets within a map set digital resource. </summary>
    [Serializable]
    public class Map_Info : iMetadata_Module
    {
        private readonly Hashtable corpHash;
        private string date;
        private readonly ArrayList indexCollection;
        private readonly Map_Info_Tables mainTbls;
        private string mapid;
        private string mets_id;
        private readonly Hashtable personHash;
        private readonly Hashtable sheetHash;

        /// <summary> Constructor for a new instance of the Map_Info class </summary>
        public Map_Info()
        {
            // Construct the DataSet 
            mainTbls = new Map_Info_Tables();

            // Construct the collections
            corpHash = new Hashtable();
            personHash = new Hashtable();
            sheetHash = new Hashtable();
            indexCollection = new ArrayList();

            // Start out with no mapid
            mapid = String.Empty;
            date = String.Empty;
            mets_id = String.Empty;
        }

        /// <summary> Primary key for this map information </summary>
        public string MapID
        {
            get { return mapid; }
            set { mapid = value; }
        }

        /// <summary> ID for the XML node when this is written into a METS file </summary>
        public string METS_ID
        {
            get { return mets_id; }
            set { mets_id = value; }
        }

        /// <summary> Date the cartographic material was rendered, or the year it is meant to represent </summary>
        public string Date
        {
            get { return date; }
            set { date = value; }
        }

        /// <summary> Flag indicates if there is data held within this map information object </summary>
        public bool hasData
        {
            get
            {
                if ((Features.Count > 0) || (Streets.Count > 0) || (personHash.Count > 0) || (corpHash.Count > 0) || (indexCollection.Count > 0))
                    return true;
                else
                    return false;
            }
        }

        #region MAP_INDEX methods and properties

        /// <summary> Gets the number of indexes associated with this digital resource </summary>
        public int Index_Count
        {
            get { return indexCollection.Count; }
        }

        /// <summary> Adds a new blank index object to this map information object and returns it </summary>
        /// <returns> Empty map index object added to this map object </returns>
        public Map_Index New_Index()
        {
            Map_Index returnValue = new Map_Index();
            indexCollection.Add(returnValue);
            return returnValue;
        }

        /// <summary> Adds a new index object to this map information object and returns it </summary>
        /// <param name="IndexID"> Primary key for this map index </param>
        /// <param name="Title"> Title of this map index  </param>
        /// <param name="Image_File"> Name of the image file for this map index </param>
        /// <param name="HTML_File"> Name of the html map file related to the image file </param>
        /// <param name="Type"> Type of index </param>
        /// <returns> Built map index object added to this map object </returns>
        public Map_Index New_Index(long IndexID, string Title, string Image_File, string HTML_File, string Type)
        {
            Map_Index returnValue = new Map_Index(IndexID, Title, Image_File, HTML_File, Type);
            indexCollection.Add(returnValue);
            return returnValue;
        }

        /// <summary> Gets map index information about this digital resource, by index </summary>
        /// <param name="index"> Index for the map index of interest within the larger collection </param>
        /// <returns> Map index object requested </returns>
        public Map_Index Get_Index(int index)
        {
            if ((index >= 0) && (index < indexCollection.Count))
            {
                return (Map_Index) indexCollection[index];
            }
            else
            {
                return null;
            }
        }

        #endregion

        #region MAP_CORPORATION methods and properties

        /// <summary> Gets the number of corporate names associated with this digital resource </summary>
        public int Corporation_Count
        {
            get { return corpHash.Count; }
        }

        /// <summary> Gets an array of all the corporate name objects associated with this map set </summary>
        public Map_Corporation[] All_Corporations
        {
            get
            {
                Map_Corporation[] allCorps = new Map_Corporation[corpHash.Count];
                int i = 0;
                foreach (DictionaryEntry thisItem in corpHash)
                {
                    allCorps[i++] = (Map_Corporation) thisItem.Value;
                }
                return allCorps;
            }
        }

        /// <summary> Gets a corporate name associated with this map, by primary key </summary>
        /// <param name="CorpID"> Primary key for the corporate name </param>
        /// <returns> Map corporation object, or NULL if the corporation is not associated with this item </returns>
        public Map_Corporation Get_Corporation(long CorpID)
        {
            if (corpHash.Contains(CorpID))
            {
                return (Map_Corporation) corpHash[CorpID];
            }
            else
            {
                return null;
            }
        }

        /// <summary> Adds a new empty corporate name to this map information </summary>
        /// <returns> Blank map corporate name added to this map </returns>
        public Map_Corporation New_Corporation()
        {
            // Calculate the next corp id by finding the largest current id
            long nextCorpID = 1;
            foreach (int thisCorpID in corpHash.Keys)
            {
                if (thisCorpID >= nextCorpID)
                    nextCorpID = thisCorpID + 1;
            }
            return New_Corporation(nextCorpID, String.Empty);
        }

        /// <summary> Adds a new corporate name to this map information </summary>
        /// <param name="CorpID"> Primary key to this corporate name, in relation to the map set </param>
        /// <returns> Map corporate body related to this map item </returns>
        public Map_Corporation New_Corporation(long CorpID)
        {
            return New_Corporation(CorpID, String.Empty);
        }

        /// <summary> Adds a new corporate name to this map information </summary>
        /// <param name="CorpID"> Primary key to this corporate name, in relation to the map set </param>
        /// <param name="Primary_Name"> Primary name for this corporate body </param>
        /// <returns> Map corporate body related to this map item </returns>
        public Map_Corporation New_Corporation(long CorpID, string Primary_Name)
        {
            Map_Corporation newCorp = new Map_Corporation(CorpID, Primary_Name);
            corpHash[CorpID] = newCorp;
            return newCorp;
        }

        #endregion

        #region MAP_PERSON methods and properties

        /// <summary> Number of personal names associated with this map item </summary>
        public int Person_Count
        {
            get { return personHash.Count; }
        }

        /// <summary> Array of all personal names associated with this digital resource </summary>
        public Map_Person[] All_People
        {
            get
            {
                Map_Person[] allPeople = new Map_Person[personHash.Count];
                int i = 0;
                foreach (DictionaryEntry thisItem in personHash)
                {
                    allPeople[i++] = (Map_Person) thisItem.Value;
                }
                return allPeople;
            }
        }

        /// <summary> Gets a personal name associated with this map, by primary key </summary>
        /// <param name="PersonID"> Primary key for the personal name </param>
        /// <returns> Map person object, or NULL if the person is not associated with this item </returns>
        public Map_Person Get_Person(long PersonID)
        {
            if (personHash.Contains(PersonID))
            {
                return (Map_Person) personHash[PersonID];
            }
            else
            {
                return null;
            }
        }

        /// <summary> Adds a new blank personal name object associated with this map item </summary>
        /// <returns> Blank map personal name added to this item </returns>
        public Map_Person New_Person()
        {
            // Calculate the next person id by finding the largest current id
            long nextPersonID = 1;
            foreach (int thisPersonID in personHash.Keys)
            {
                if (thisPersonID >= nextPersonID)
                    nextPersonID = thisPersonID + 1;
            }
            return New_Person(nextPersonID, String.Empty);
        }

        /// <summary> Adds a new personal name object associated with this map item </summary>
        /// <param name="PersonID"> Primary key for this personal name </param>
        /// <returns> Mostly empty map personal name added to this item </returns>
        public Map_Person New_Person(long PersonID)
        {
            return New_Person(PersonID, String.Empty);
        }


        /// <summary> Adds a new personal name object associated with this map item </summary>
        /// <param name="PersonID"> Primary key for this personal name </param>
        /// <param name="Primary_Name"> Primary personal name to associate with this map </param>
        /// <returns> Mostly empty map personal name added to this item </returns>
        public Map_Person New_Person(long PersonID, string Primary_Name)
        {
            Map_Person newPerson = new Map_Person(PersonID, Primary_Name);
            personHash[PersonID] = newPerson;
            return newPerson;
        }

        #endregion

        #region MAP_SHEET methods and properties

        /// <summary> Number of map sheets associated with this map set </summary>
        public int Sheet_Count
        {
            get { return sheetHash.Count; }
        }

        /// <summary> Array of all the map sheets associated with this map set </summary>
        public Map_Sheet[] All_Sheets
        {
            get
            {
                Map_Sheet[] allSheets = new Map_Sheet[sheetHash.Count];
                int i = 0;
                foreach (DictionaryEntry thisItem in sheetHash)
                {
                    allSheets[i++] = (Map_Sheet) thisItem.Value;
                }
                return allSheets;
            }
        }

        /// <summary> Gets a single map sheet by sheet id from this map item </summary>
        /// <param name="SheetID"> Primary key for the sheet of interest from this map set </param>
        /// <returns> Requested map sheet, or NULL if none exists </returns>
        public Map_Sheet Get_Sheet(long SheetID)
        {
            if (sheetHash.Contains(SheetID))
            {
                return (Map_Sheet) sheetHash[SheetID];
            }
            else
            {
                return null;
            }
        }

        /// <summary> Adds a new blank map sheet to the map information associated with this item </summary>
        /// <returns> New blank map sheet added to this item </returns>
        public Map_Sheet New_Sheet()
        {
            // Calculate the next sheet id by finding the largest current id
            long nextSheetID = 1;
            foreach (int thisSheetID in sheetHash.Keys)
            {
                if (thisSheetID >= nextSheetID)
                    nextSheetID = thisSheetID + 1;
            }
            return New_Sheet(nextSheetID, -1, String.Empty, String.Empty);
        }

        /// <summary> Add a new map sheet to this map-type item </summary>
        /// <param name="SheetID"> Primary key for this map sheet in relation to the map set item </param>
        /// <returns> Partially built and added map sheet object </returns>
        public Map_Sheet New_Sheet(long SheetID)
        {
            return New_Sheet(SheetID, -1, String.Empty, String.Empty);
        }

        /// <summary> Add a new map sheet to this map-type item </summary>
        /// <param name="SheetID"> Primary key for this map sheet in relation to the map set item </param>
        /// <param name="Index"> Index of this sheet within the entire set </param>
        /// <param name="FilePtr"> File pointer string </param>
        /// <param name="File"> File name </param>
        /// <returns> Fully built and added map sheet object </returns>
        public Map_Sheet New_Sheet(long SheetID, int Index, string FilePtr, string File)
        {
            Map_Sheet newSheet = new Map_Sheet(SheetID, Index, FilePtr, File);
            sheetHash[SheetID] = newSheet;
            return newSheet;
        }

        #endregion

        #region FEATURES methods and properties 

        /// <summary> Gets the strongly-typed datatable containing all the feature information </summary>
        public Map_Info_Tables.FeatureDataTable Features
        {
            get { return mainTbls.Feature; }
        }

        /// <summary> Adds a feature to this map set, to be linked with a sheet </summary>
        /// <param name="Name"> Name of this feature </param>
        /// <param name="Type"> Type of feature </param>
        /// <returns> Built information about this feature </returns>
        public Map_Info_Tables.FeatureRow Add_Feature(string Name, string Type)
        {
            // Calculate the next feature id
            long featureid = 1;
            foreach (Map_Info_Tables.FeatureRow thisFeature in mainTbls.Feature)
            {
                if (thisFeature.FeatureID >= featureid)
                    featureid = thisFeature.FeatureID + 1;
            }

            // Add and return the row
            return Add_Feature(featureid, Name, Type);
        }

        /// <summary> Adds a feature to this map set, to be linked with a sheet </summary>
        /// <param name="FeatureID"> Primary key for this feature, either unique in this map set or derived from a database </param>
        /// <param name="Name"> Name of this feature </param>
        /// <param name="Type"> Type of feature </param>
        /// <returns> Built information about this feature </returns>
        public Map_Info_Tables.FeatureRow Add_Feature(long FeatureID, string Name, string Type)
        {
            Map_Info_Tables.FeatureRow newFeatureRow = mainTbls.Feature.NewFeatureRow();
            newFeatureRow.FeatureID = FeatureID;
            newFeatureRow.Latitude = String.Empty;
            newFeatureRow.Longitude = String.Empty;
            newFeatureRow.Name = Name;
            newFeatureRow.Type = Type;
            newFeatureRow.Units = String.Empty;
            newFeatureRow.Description = String.Empty;
            mainTbls.Feature.Rows.Add(newFeatureRow);
            return newFeatureRow;
        }

        /// <summary> Adds a feature to this map set, to be linked with a sheet </summary>
        /// <param name="FeatureID"> Primary key for this feature, either unique in this map set or derived from a database </param>
        /// <param name="Name"> Name of this feature </param>
        /// <param name="Type"> Type of feature </param>
        /// <param name="Units"> Units of measure </param>
        /// <param name="Latitude"> Latitude of this feature </param>
        /// <param name="Longitude"> Longitude of this feature </param>
        /// <param name="Description"> Description of this feature </param>
        /// <returns> Built information about this feature </returns>
        public Map_Info_Tables.FeatureRow Add_Feature(long FeatureID, string Name, string Type, string Units, string Latitude, string Longitude, string Description)
        {
            Map_Info_Tables.FeatureRow newFeatureRow = mainTbls.Feature.NewFeatureRow();
            newFeatureRow.FeatureID = FeatureID;
            newFeatureRow.Latitude = Latitude;
            newFeatureRow.Longitude = Longitude;
            newFeatureRow.Name = Name;
            newFeatureRow.Type = Type;
            newFeatureRow.Units = Units;
            newFeatureRow.Description = Description;
            mainTbls.Feature.Rows.Add(newFeatureRow);
            return newFeatureRow;
        }

        /// <summary> Add a link between a feature in this map set and a sheet in the map set </summary>
        /// <param name="FeatureID"> Primary key to the feature </param>
        /// <param name="SheetID"> Primary key to the map sheet </param>
        /// <param name="X"> Horizontal location on the map sheet (in pixels) </param>
        /// <param name="Y"> Vertical location on the map sheet (in pixels) </param>
        public void Add_Feature_Sheet_Link(long FeatureID, long SheetID, long X, long Y)
        {
            Map_Info_Tables.FeatureRow featureRow = mainTbls.Feature.FindByFeatureID(FeatureID);
            if (featureRow != null)
            {
                mainTbls.Sheet_Link.AddSheet_LinkRow(featureRow, SheetID, X, Y);
            }
        }

        /// <summary> Adds a link between a feature in this map item and an existing personal name </summary>
        /// <param name="FeatureID"> Primary key for a feature in this map item </param>
        /// <param name="PersonID"> Primary key for a personal name in this map item </param>
        /// <param name="Type"> Type of link to establish </param>
        public void Add_Feature_Person_Link(long FeatureID, long PersonID, string Type)
        {
            Map_Info_Tables.FeatureRow featureRow = mainTbls.Feature.FindByFeatureID(FeatureID);
            if (featureRow != null)
            {
                mainTbls.Person_Link.AddPerson_LinkRow(featureRow, PersonID, Type);
            }
        }

        /// <summary> Adds a link between a feature in this map item and an existing corporate name </summary>
        /// <param name="FeatureID"> Primary key for a feature in this map item </param>
        /// <param name="CorpID"> Primary key for a corporate name in this map item </param>
        public void Add_Feature_Corp_Link(long FeatureID, long CorpID)
        {
            Map_Info_Tables.FeatureRow featureRow = mainTbls.Feature.FindByFeatureID(FeatureID);
            if (featureRow != null)
            {
                mainTbls.Corporation_Link.AddCorporation_LinkRow(featureRow, CorpID);
            }
        }

        #endregion

        #region STREETS methods and properties 

        /// <summary> Gets the strongly-typed datatable containing all the street information </summary>
        public Map_Info_Tables.StreetDataTable Streets
        {
            get { return mainTbls.Street; }
        }

        /// <summary> Adds information about a street which appears in this map set </summary>
        /// <param name="SheetID"> Primary key to the map sheet within this map set </param>
        /// <param name="Name"> Name of the street </param>
        /// <returns> Strongly-typed datatable row with all the street information </returns>
        public Map_Info_Tables.StreetRow Add_Street(long SheetID, string Name)
        {
            // Calculate the next streeet id
            long streetid = 1;
            foreach (Map_Info_Tables.StreetRow thisStreet in mainTbls.Street)
            {
                if (thisStreet.StreetID >= streetid)
                    streetid = thisStreet.StreetID + 1;
            }

            return Add_Street(streetid, SheetID, Name);
        }

        /// <summary> Adds information about a street which appears in this map set </summary>
        /// <param name="StreetID"> Primary key for this street within this map set </param>
        /// <param name="SheetID"> Primary key to the map sheet within this map set </param>
        /// <param name="Name"> Name of the street </param>
        /// <returns> Strongly-typed datatable row with all the street information </returns>
        public Map_Info_Tables.StreetRow Add_Street(long StreetID, long SheetID, string Name)
        {
            return Add_Street(StreetID, SheetID, Name, String.Empty, String.Empty, -1, -1, String.Empty);
        }

        /// <summary> Adds information about a street which appears in this map set </summary>
        /// <param name="StreetID"> Primary key for this street within this map set </param>
        /// <param name="SheetID"> Primary key to the map sheet within this map set </param>
        /// <param name="Name"> Name of the street </param>
        /// <param name="Description"> Description of this section of street </param>
        /// <param name="Direction"> Direction portion of the address ( i.e, East, North) </param>
        /// <param name="Start"> Address for the beginning of the section of street which appears on a map sheet </param>
        /// <param name="End"> Address for the end of the section of street which appears on a map sheet </param>
        /// <param name="Side"> Side of the street, if only one side is present </param>
        /// <returns> Strongly-typed datatable row with all the street information </returns>
        public Map_Info_Tables.StreetRow Add_Street(long StreetID, long SheetID, string Name, string Description, string Direction, long Start, long End, string Side)
        {
            Map_Info_Tables.StreetRow newStreetRow = mainTbls.Street.NewStreetRow();
            newStreetRow.Name = Name;
            newStreetRow.Description = Description;
            newStreetRow.Direction = Direction;
            newStreetRow.Start = Start;
            newStreetRow.End = End;
            newStreetRow.Side = Side;
            newStreetRow.StreetID = StreetID;
            newStreetRow.SheetID = SheetID;
            mainTbls.Street.Rows.Add(newStreetRow);
            return newStreetRow;
        }

        #endregion

        #region Method returns the information as string formatted as XML

        /// <summary> Returns all of the map information as string formatted as XML </summary>
        /// <param name="prefix"> Namespace to use for the schema referenced elements </param>
        /// <param name="include_map_id"> Flag indicates whether to include the map id</param>
        /// <returns> All of the map information, formatted as XML </returns>
        public string ToXML(string prefix, bool include_map_id)
        {
            // Start to build this data
            StringBuilder results = new StringBuilder();
            if ((include_map_id) && (mapid.Length > 0))
                results.Append("<" + prefix + "ufdc_map id=\"" + mapid + "\">\r\n");
            else
                results.Append("<" + prefix + "ufdc_map>\r\n");

            // Add index information
            if (indexCollection.Count > 0)
            {
                results.Append("<" + prefix + "indexes>\r\n");
                foreach (Map_Index index in indexCollection)
                {
                    results.Append("<" + prefix + "image");
                    if (index.Type.Length > 0)
                        results.Append(" type=\"" + index.Type + "\"");
                    if (index.IndexID > 0)
                        results.Append(" id=\"INDE" + index.IndexID + "\"");
                    results.Append(">\r\n");

                    if (index.Title.Length > 0)
                    {
                        results.Append("<" + prefix + "title>" + index.Title + "</" + prefix + "title>\r\n");
                    }

                    if (index.Image_File.Length > 0)
                    {
                        results.Append("<" + prefix + "file>" + index.Image_File + "</" + prefix + "file>\r\n");
                    }

                    if (index.HTML_File.Length > 0)
                    {
                        results.Append("<" + prefix + "html>" + index.HTML_File + "</" + prefix + "html>\r\n");
                    }

                    results.Append("</" + prefix + "image>\r\n");
                }
                results.Append("</" + prefix + "indexes>\r\n");
            }

            // Add all the entities
            if ((Features.Count > 0) || (Streets.Count > 0) || (corpHash.Count > 0) || (personHash.Count > 0))
            {
                results.Append("<" + prefix + "entities>\r\n");

                // Add all the features first
                DataView featureView = new DataView(Features);
                featureView.Sort = "FeatureID ASC";
                foreach (DataRowView thisFeatureView in featureView)
                {
                    // Get the feature row from the data view row
                    Map_Info_Tables.FeatureRow thisFeature = (Map_Info_Tables.FeatureRow) thisFeatureView.Row;

                    // Start the information on this feature
                    results.Append("<" + prefix + "feature id=\"FEAT" + thisFeature.FeatureID + "\"");
                    if (thisFeature.Name.Length > 0)
                        results.Append(" name=\"" + XML_Safe_Element(thisFeature.Name) + "\"");
                    if (thisFeature.Type.Length > 0)
                        results.Append(" type=\"" + thisFeature.Type.ToLower() + "\"");
                    results.Append(">\r\n");

                    // Add any corporate links
                    foreach (Map_Info_Tables.Corporation_LinkRow thisCorpLink in thisFeature.GetCorporation_LinkRows())
                    {
                        results.Append("<" + prefix + "corpref corpid=\"CORP" + thisCorpLink.CorpID + "\" />\r\n");
                    }

                    // Add any personal links
                    foreach (Map_Info_Tables.Person_LinkRow thisPersonLink in thisFeature.GetPerson_LinkRows())
                    {
                        if (thisPersonLink.Type.Length > 0)
                        {
                            results.Append("<" + prefix + "persref corpid=\"PERSP" + thisPersonLink.PersonID + "\" reftype=\"" + XML_Safe_Element(thisPersonLink.Type) + "\" />\r\n");
                        }
                        else
                        {
                            results.Append("<" + prefix + "persref persid=\"PERS" + thisPersonLink.PersonID + "\" />\r\n");
                        }
                    }

                    // Add location information
                    results.Append("<" + prefix + "location>\r\n");
                    if (thisFeature.Description.Length > 0)
                    {
                        results.Append("<" + prefix + "desc>" + thisFeature.Description + "</" + prefix + "desc>\r\n");
                    }
                    if ((thisFeature.Latitude.Length > 0) && (thisFeature.Longitude.Length > 0))
                    {
                        if (thisFeature.Units.Length > 0)
                        {
                            results.Append("<" + prefix + "coordinates units=\"" + thisFeature.Units + "\" latitude=\"" + thisFeature.Latitude + "\" longitude=\"" + thisFeature.Longitude + "\" />\r\n");
                        }
                        else
                        {
                            results.Append("<" + prefix + "coordinates latitude=\"" + thisFeature.Latitude + "\" longitude=\"" + thisFeature.Longitude + "\" />\r\n");
                        }
                    }
                    foreach (Map_Info_Tables.Sheet_LinkRow thisSheetLink in thisFeature.GetSheet_LinkRows())
                    {
                        if ((thisSheetLink.X > 0) && (thisSheetLink.Y > 0))
                        {
                            results.Append("<" + prefix + "sheetref sheetid=\"MS" + thisSheetLink.SheetID + "\" x=\"" + thisSheetLink.X + "\" y=\"" + thisSheetLink.Y + "\" />\r\n");
                        }
                        else
                        {
                            results.Append("<" + prefix + "sheetref sheetid=\"MS" + thisSheetLink.SheetID + "\" />\r\n");
                        }
                    }
                    results.Append("</" + prefix + "location>\r\n");

                    // Finish this feature
                    results.Append("</" + prefix + "feature>\r\n");
                }

                // Add all the streets next
                // Collect all the street ids
                ArrayList streetids = new ArrayList();
                foreach (Map_Info_Tables.StreetRow thisStreet in Streets)
                {
                    if (!streetids.Contains(thisStreet.StreetID))
                    {
                        streetids.Add(thisStreet.StreetID);
                    }
                }

                // Create a data view
                DataView streetView = new DataView(Streets);
                streetView.Sort = "StreetID ASC, SheetID ASC";
                foreach (long thisStreetID in streetids)
                {
                    // Start this street
                    streetView.RowFilter = "StreetID=" + thisStreetID;
                    bool start = false;
                    foreach (DataRowView thisStreetView in streetView)
                    {
                        // Get the feature row from the data view row
                        Map_Info_Tables.StreetRow thisStreet = (Map_Info_Tables.StreetRow) thisStreetView.Row;
                        if (!start)
                        {
                            results.Append("<" + prefix + "street id=\"STR" + thisStreetID + "\" name=\"" + XML_Safe_Element(thisStreet.Name) + "\">\r\n");
                            start = true;
                        }

                        results.Append("<" + prefix + "segment sheetid=\"MS" + thisStreet.SheetID + "\"");

                        if (thisStreet.Start >= 0)
                            results.Append(" start=\"" + thisStreet.Start + "\"");
                        if (thisStreet.End >= 0)
                            results.Append(" end=\"" + thisStreet.End + "\"");
                        if (thisStreet.Direction.Length > 0)
                            results.Append(" direction=\"" + XML_Safe_Element(thisStreet.Direction) + "\"");
                        if (thisStreet.Side.Length > 0)
                            results.Append(" side=\"" + XML_Safe_Element(thisStreet.Side) + "\"");

                        if (thisStreet.Description.Length > 0)
                        {
                            results.Append(">" + XML_Safe_Element(thisStreet.Description) + "</" + prefix + "segment>\r\n");
                        }
                        else
                        {
                            results.Append(" />\r\n");
                        }
                    }

                    if (start)
                    {
                        results.Append("</" + prefix + "street>\r\n");
                    }
                }

                // Add all the corporations
                Map_Corporation[] allCorps = All_Corporations;
                foreach (Map_Corporation thisCorp in allCorps)
                {
                    results.Append("<" + prefix + "corporation id=\"CORP" + thisCorp.CorpID + "\">\r\n");
                    results.Append("<" + prefix + "corpname type=\"primary\">" + XML_Safe_Element(thisCorp.Primary_Name) + "</" + prefix + "corpname>\r\n");
                    for (int i = 0; i < thisCorp.Alt_Name_Count; i++)
                    {
                        results.Append("<" + prefix + "corpname type=\"alternate\">" + XML_Safe_Element(thisCorp.Get_Alt_Name(i)) + "</" + prefix + "corpname>\r\n");
                    }
                    results.Append("</" + prefix + "corporation>\r\n");
                }

                // Add all the personal names
                Map_Person[] allPeople = All_People;
                foreach (Map_Person thisPerson in allPeople)
                {
                    results.Append("<" + prefix + "person id=\"PERS" + thisPerson.PersonID + "\">\r\n");
                    results.Append("<" + prefix + "persname>" + XML_Safe_Element(thisPerson.Name) + "</" + prefix + "persname>\r\n");
                    results.Append("</" + prefix + "person>\r\n");
                }
                results.Append("</" + prefix + "entities>\r\n");
            }

            // Add the SHEET information
            Map_Sheet[] allMaps = All_Sheets;
            if (allMaps.Length > 0)
            {
                results.Append("<" + prefix + "sheets>\r\n");

                foreach (Map_Sheet thisMap in allMaps)
                {
                    results.Append("<" + prefix + "sheet id=\"MS" + thisMap.SheetID + "\">\r\n");
                    results.Append("<" + prefix + "fileref fileid=\"" + thisMap.FilePtr + "\" />\r\n");
                    results.Append("</" + prefix + "sheet>\r\n");
                }

                results.Append("</" + prefix + "sheets>\r\n");
            }


            results.Append("</" + prefix + "ufdc_map>\r\n");

            // return the built string
            return results.ToString();
        }

        /// <summary> Gets the XML-safe value of this element </summary>
        public string XML_Safe_Element(string element)
        {
            string xml_safe = element;
            int i = xml_safe.IndexOf("&");
            while (i >= 0)
            {
                if ((i != xml_safe.IndexOf("&amp;", i)) && (i != xml_safe.IndexOf("&quot;", i)) &&
                    (i != xml_safe.IndexOf("&gt;", i)) && (i != xml_safe.IndexOf("&lt;", i)))
                {
                    xml_safe = xml_safe.Substring(0, i + 1) + "amp;" + xml_safe.Substring(i + 1);
                }

                i = xml_safe.IndexOf("&", i + 1);
            }
            return xml_safe.Replace("<", "&lt;").Replace(">", "&gt;").Replace("\"", "&quot;");
        }

        #endregion

        #region Methods to read from the XML format

        /// <summary> Reads all the map information from the main XML node </summary>
        /// <param name="Main_Node"> Main map node </param>
        /// <param name="Prefix"> Namespace which prefixes all element names referenced by a schema </param>
        public void Read_XML(XmlNode Main_Node, string Prefix)
        {
            // Read the attributes first
            foreach (XmlAttribute thisAttribute in Main_Node.Attributes)
            {
                if (thisAttribute.Name.ToLower() == "date")
                    date = thisAttribute.Value.Trim().ToUpper();
                if (thisAttribute.Name.ToLower() == "id")
                    mapid = thisAttribute.Value.Trim().ToUpper();
            }

            // Read the next nodes
            foreach (XmlNode mainChildNode in Main_Node.ChildNodes)
            {
                if (mainChildNode.Name.ToLower() == Prefix + "indexes")
                {
                    read_indexes(mainChildNode, Prefix);
                }

                if (mainChildNode.Name.ToLower() == Prefix + "entities")
                {
                    read_entities(mainChildNode, Prefix);
                }

                if (mainChildNode.Name.ToLower() == Prefix + "sheets")
                {
                    read_sheets(mainChildNode, Prefix);
                }
            }
        }

        private void read_indexes(XmlNode indexesNode, string Prefix)
        {
            // Read all the child nodes
            foreach (XmlNode indexNode in indexesNode.ChildNodes)
            {
                if (indexNode.Name.ToLower().Trim() == Prefix + "image")
                {
                    try
                    {
                        string title = String.Empty;
                        string file = String.Empty;
                        string html = String.Empty;
                        string type = String.Empty;
                        string id_string = String.Empty;

                        // Check the attributes
                        foreach (XmlAttribute thisAttribute in indexNode.Attributes)
                        {
                            if (thisAttribute.Name.ToLower() == "id")
                                id_string = thisAttribute.Value.Trim().ToUpper();
                            if (thisAttribute.Name.ToLower() == "type")
                                type = thisAttribute.Value.Trim().ToLower();
                        }

                        // Step through each child node of this
                        foreach (XmlNode subIndexNode in indexNode.ChildNodes)
                        {
                            string subIndexNodeName = subIndexNode.Name.ToLower();
                            if (subIndexNodeName == Prefix + "title")
                            {
                                title = subIndexNode.InnerText.Trim();
                            }
                            if (subIndexNodeName == Prefix + "file")
                            {
                                file = subIndexNode.InnerText.Trim();
                            }
                            if (subIndexNodeName == Prefix + "html")
                            {
                                html = subIndexNode.InnerText.Trim();
                            }
                        }

                        // Get the index value
                        long id = Convert.ToInt64(id_string.ToUpper().Replace("INDE", "").Replace("X", ""));

                        // Add this index
                        New_Index(id, title, file, html, type);
                    }
                    catch
                    {
                    }
                }
            }
        }

        private void read_entities(XmlNode entitiesNode, string Prefix)
        {
            // Read all the child nodes
            foreach (XmlNode entityNode in entitiesNode.ChildNodes)
            {
                try
                {
                    string name = entityNode.Name.ToLower();
                    if (name == Prefix + "street")
                    {
                        read_street(entityNode, Prefix);
                    }
                    if (name == Prefix + "feature")
                    {
                        read_feature(entityNode, Prefix);
                    }
                    if (name == Prefix + "person")
                    {
                        read_person(entityNode, Prefix);
                    }
                    if (name == Prefix + "corporation")
                    {
                        read_corporation(entityNode, Prefix);
                    }
                }
                catch
                {
                    return;
                }
            }
        }

        private void read_street(XmlNode streetNode, string Prefix)
        {
            // Check the attributes
            string id_string = String.Empty;
            string name = String.Empty;
            foreach (XmlAttribute thisAttribute in streetNode.Attributes)
            {
                if (thisAttribute.Name.ToLower() == "id")
                    id_string = thisAttribute.Value.Trim().ToUpper();
                if (thisAttribute.Name.ToLower() == "name")
                    name = thisAttribute.Value.Trim();
            }

            // Must have a name and an ID to continue
            long streetid = -1;
            try
            {
                streetid = Convert.ToInt64(id_string.Replace("STR", "").Replace("E", "").Replace("T", ""));
            }
            catch
            {
            }

            // Must have an id and name to continue
            if ((streetid < 0) || (name.Length == 0))
                return;

            // Now, step through each child row
            foreach (XmlNode segmentNode in streetNode.ChildNodes)
            {
                if (segmentNode.Name.ToLower().Trim() == Prefix + "segment")
                {
                    long sheetid = -1;
                    long start = -1;
                    long end = -1;
                    string direction = String.Empty;
                    string side = String.Empty;
                    string desc = String.Empty;

                    // Check the attributes
                    foreach (XmlAttribute thisAttribute in segmentNode.Attributes)
                    {
                        if (thisAttribute.Name.ToLower() == "sheetid")
                        {
                            try
                            {
                                sheetid = Convert.ToInt64(thisAttribute.Value.Replace("MS", "").Replace("SHEET", ""));
                            }
                            catch
                            {
                            }
                        }
                        if (thisAttribute.Name.ToLower() == "side")
                            side = thisAttribute.Value.Trim();
                        if (thisAttribute.Name.ToLower() == "direction")
                            direction = thisAttribute.Value.Trim();
                        if (thisAttribute.Name.ToLower() == "start")
                        {
                            try
                            {
                                start = Convert.ToInt64(thisAttribute.Value.Trim());
                            }
                            catch
                            {
                            }
                        }
                        if (thisAttribute.Name.ToLower() == "end")
                        {
                            try
                            {
                                end = Convert.ToInt64(thisAttribute.Value.Trim());
                            }
                            catch
                            {
                            }
                        }
                    }

                    // Get the description, if there was one
                    if ((segmentNode.InnerText != null) && (segmentNode.InnerText.Trim().Length > 0))
                        desc = segmentNode.InnerText.Trim();

                    // Add this street segment information
                    if (sheetid > 0)
                    {
                        Add_Street(streetid, sheetid, name, desc, direction, start, end, side);
                    }
                }
            }
        }

        private void read_feature(XmlNode featureNode, string Prefix)
        {
            // Check the attributes
            string id_string = String.Empty;
            string name = String.Empty;
            string type = String.Empty;
            foreach (XmlAttribute thisAttribute in featureNode.Attributes)
            {
                if (thisAttribute.Name.ToLower() == "id")
                    id_string = thisAttribute.Value.Trim().ToUpper();
                if (thisAttribute.Name.ToLower() == "name")
                    name = thisAttribute.Value.Trim();
                if (thisAttribute.Name.ToLower() == "type")
                    type = thisAttribute.Value.Trim();
            }

            // Must have a name and an ID to continue
            long featureid = -1;
            try
            {
                featureid = Convert.ToInt64(id_string.Replace("FEAT", "").Replace("U", "").Replace("R", "").Replace("E", ""));
            }
            catch
            {
            }

            // Must have an id and name to continue
            if ((featureid < 0) || (name.Length == 0))
                return;

            // Add this feature
            Map_Info_Tables.FeatureRow thisFeature = Add_Feature(featureid, name, type);

            // Now, step through each child row
            foreach (XmlNode childNode in featureNode.ChildNodes)
            {
                // Is this the location information?
                if (childNode.Name.ToLower().Trim() == Prefix + "location")
                {
                    // Step through each child under this
                    foreach (XmlNode locationNode in childNode.ChildNodes)
                    {
                        // Is this the description?
                        if ((locationNode.Name.ToLower().Trim() == Prefix + "desc") && (locationNode.InnerText != null))
                        {
                            thisFeature.Description = locationNode.InnerText.Trim();
                        }

                        // Is this the coordinates?
                        if (locationNode.Name.ToLower().Trim() == Prefix + "coordinates")
                        {
                            // Step through each attribute
                            foreach (XmlAttribute thisAttribute in locationNode.Attributes)
                            {
                                if (thisAttribute.Name.ToLower() == "units")
                                    thisFeature.Units = thisAttribute.Value.Trim();
                                if (thisAttribute.Name.ToLower() == "latitude")
                                    thisFeature.Latitude = thisAttribute.Value.Trim();
                                if (thisAttribute.Name.ToLower() == "longitude")
                                    thisFeature.Longitude = thisAttribute.Value.Trim();
                            }
                        }

                        // Are these sheet references?
                        if (locationNode.Name.ToLower().Trim() == Prefix + "sheetref")
                        {
                            // Step through each attribute
                            long x = -1;
                            long y = -1;
                            long sheetid = -1;
                            foreach (XmlAttribute thisAttribute in locationNode.Attributes)
                            {
                                if (thisAttribute.Name.ToLower() == "x")
                                {
                                    try
                                    {
                                        x = Convert.ToInt64(thisAttribute.Value.Trim());
                                    }
                                    catch
                                    {
                                    }
                                }
                                if (thisAttribute.Name.ToLower() == "y")
                                {
                                    try
                                    {
                                        y = Convert.ToInt64(thisAttribute.Value.Trim());
                                    }
                                    catch
                                    {
                                    }
                                }
                                if (thisAttribute.Name.ToLower() == "sheetid")
                                {
                                    try
                                    {
                                        string sheetid_string = thisAttribute.Value.Trim();
                                        sheetid_string = sheetid_string.Replace("MS", "").Replace("SHEET", "");
                                        sheetid = Convert.ToInt64(sheetid_string);
                                    }
                                    catch
                                    {
                                    }
                                }
                            }

                            // If there was a sheet id, add this link
                            if (sheetid > 0)
                            {
                                Add_Feature_Sheet_Link(thisFeature.FeatureID, sheetid, x, y);
                            }
                        }
                    }
                }

                // Is this a reference to a person?
                if (childNode.Name.ToLower().Trim() == Prefix + "persref")
                {
                    string reftype = String.Empty;
                    long personid = -1;
                    foreach (XmlAttribute thisAttribute in childNode.Attributes)
                    {
                        if (thisAttribute.Name.ToLower() == "reftype")
                        {
                            reftype = thisAttribute.Value.Trim();
                        }
                        if (thisAttribute.Name.ToLower() == "persid")
                        {
                            try
                            {
                                string person_string = thisAttribute.Value.Trim();
                                person_string = person_string.Replace("PER", "").Replace("S", "").Replace("O", "").Replace("N", "");
                                personid = Convert.ToInt64(person_string);
                            }
                            catch
                            {
                            }
                        }
                    }

                    // Add if there is a person id
                    if (personid > 0)
                    {
                        Add_Feature_Person_Link(thisFeature.FeatureID, personid, reftype);
                    }
                }

                // Is this a reference to a coorporation?
                if (childNode.Name.ToLower().Trim() == Prefix + "corpref")
                {
                    long corpid = -1;
                    foreach (XmlAttribute thisAttribute in childNode.Attributes)
                    {
                        if (thisAttribute.Name.ToLower() == "corpid")
                        {
                            try
                            {
                                string corp_string = thisAttribute.Value.Trim();
                                corp_string = corp_string.Replace("COR", "").Replace("P", "");
                                corpid = Convert.ToInt64(corp_string);
                            }
                            catch
                            {
                            }
                        }
                    }

                    // Add if there is a corp id
                    if (corpid > 0)
                    {
                        Add_Feature_Corp_Link(thisFeature.FeatureID, corpid);
                    }
                }
            }
        }

        private void read_person(XmlNode personNode, string Prefix)
        {
            // Check the attributes
            long personid = -1;
            foreach (XmlAttribute thisAttribute in personNode.Attributes)
            {
                if (thisAttribute.Name.ToLower() == "id")
                {
                    try
                    {
                        string person_string = thisAttribute.Value.Trim().ToUpper();
                        person_string = person_string.Replace("PER", "").Replace("S", "").Replace("O", "").Replace("N", "");
                        personid = Convert.ToInt64(person_string);
                    }
                    catch
                    {
                    }
                }
            }

            // Look for the name
            string name = String.Empty;
            foreach (XmlNode childNode in personNode.ChildNodes)
            {
                if (childNode.Name.ToLower().Trim() == Prefix + "persname")
                {
                    name = childNode.InnerText.Trim();
                }
            }

            // If there is a name and id, add this person
            if ((personid > 0) && (name.Length > 0))
            {
                New_Person(personid, name);
            }
        }

        private void read_corporation(XmlNode corpNode, string Prefix)
        {
            // Check the attributes
            long corpid = -1;
            foreach (XmlAttribute thisAttribute in corpNode.Attributes)
            {
                if (thisAttribute.Name.ToLower() == "id")
                {
                    try
                    {
                        string corp_string = thisAttribute.Value.Trim();
                        corp_string = corp_string.Replace("COR", "").Replace("P", "");
                        corpid = Convert.ToInt64(corp_string);
                    }
                    catch
                    {
                    }
                }
            }

            // Look for the primary name and alternate names
            string type = String.Empty;
            string primary_name = String.Empty;
            ArrayList alternate_names = new ArrayList();
            foreach (XmlNode childNode in corpNode.ChildNodes)
            {
                // Look at each corp name
                if (childNode.Name.ToLower().Trim() == Prefix + "corpname")
                {
                    foreach (XmlAttribute thisAttribute in childNode.Attributes)
                    {
                        if (thisAttribute.Name.ToLower() == "type")
                        {
                            type = thisAttribute.Value.Trim().ToLower();
                        }
                    }

                    string name = childNode.InnerText.Trim();

                    if (name.Length > 0)
                    {
                        if ((type.Length == 0) || (type == "primary"))
                        {
                            primary_name = name;
                        }
                        else
                        {
                            alternate_names.Add(name);
                        }
                    }
                }
            }

            // If there is a name and id, add this person
            if ((corpid > 0) && (primary_name.Length > 0))
            {
                Map_Corporation thisCorp = New_Corporation(corpid, primary_name);
                foreach (string altName in alternate_names)
                {
                    thisCorp.Add_Alt_Name(altName);
                }
            }
        }

        private void read_sheets(XmlNode sheetsNode, string Prefix)
        {
            // Read all the child nodes
            foreach (XmlNode indexNode in sheetsNode.ChildNodes)
            {
                if (indexNode.Name.ToLower().Trim() == Prefix + "sheet")
                {
                    try
                    {
                        string id_string = String.Empty;
                        string file = String.Empty;


                        // Check the attributes
                        foreach (XmlAttribute thisAttribute in indexNode.Attributes)
                        {
                            if (thisAttribute.Name.ToLower() == "id")
                                id_string = thisAttribute.Value.Trim().ToUpper();
                        }

                        // Step through each child node of this
                        foreach (XmlNode subIndexNode in indexNode.ChildNodes)
                        {
                            string subIndexNodeName = subIndexNode.Name.ToLower();
                            if (subIndexNodeName == Prefix + "fileref")
                            {
                                // Check the attributes
                                foreach (XmlAttribute thisAttribute in subIndexNode.Attributes)
                                {
                                    if (thisAttribute.Name.ToLower() == "fileid")
                                        file = thisAttribute.Value.Trim().ToUpper();
                                }
                            }
                        }

                        // Get the index value
                        long id = Convert.ToInt64(id_string.ToUpper().Replace("MS", ""));

                        // Add this index
                        New_Sheet(id, 0, file, String.Empty);
                    }
                    catch
                    {
                    }
                }
            }
        }

        #endregion

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns ''</value>
        public string Module_Name
        {
            get { return "SobekMapAuthority"; }
        }

        /// <summary> Gets the metadata search terms and values to be saved to the database
        /// to allow searching to occur over the data in this metadata module </summary>
        public List<KeyValuePair<string, string>> Metadata_Search_Terms
        {
            get { return null; }
        }

        /// <summary> Chance for this metadata module to perform any additional database work
        /// such as saving digital resource data into custom tables </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString"> Connection string for the current database </param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Save_Additional_Info_To_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        /// <summary> Chance for this metadata module to load any additional data from the 
        /// database when building this digital resource  in memory </summary>
        /// <param name="ItemID"> Primary key for this item within the SobekCM database </param>
        /// <param name="DB_ConnectionString">Connection string for the current database</param>
        /// <param name="BibObject"> Entire resource, in case there are dependencies between this module and somethingt in the full resource </param>
        /// <param name="Error_Message"> In the case of an error, this contains text of the error </param>
        /// <returns> TRUE if no error occurred, otherwise FALSE </returns>
        /// <remarks> This module currently  does no additional processing in this method </remarks>
        public bool Retrieve_Additional_Info_From_Database(int ItemID, string DB_ConnectionString, SobekCM_Item BibObject, out string Error_Message)
        {
            // Set the default error mesasge
            Error_Message = String.Empty;

            return true;
        }

        #endregion

        /// <summary> Clear all the information contained in this map information object </summary>
        public void Clear()
        {
            corpHash.Clear();
            personHash.Clear();
            sheetHash.Clear();
            indexCollection.Clear();
            mainTbls.Corporation_Link.Clear();
            mainTbls.Person_Link.Clear();
            mainTbls.Sheet_Link.Clear();
            mainTbls.Feature.Clear();
            mainTbls.Street.Clear();
        }
    }
}