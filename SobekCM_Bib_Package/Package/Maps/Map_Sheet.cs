using System;

namespace SobekCM.Bib_Package.Maps
{
	/// <summary> Information about a single map sheet from a map set item  </summary>
    [Serializable]
	public class Map_Sheet
	{
		private int index;
		private long sheetid;
		private string filePtr;
		private string file;
        private int db_sheetid;

        /// <summary> Constructor for a new instance of the Map_Sheet class </summary>
		public Map_Sheet()
		{
			index = -1;
			sheetid = -1;
			filePtr = String.Empty;
			file = String.Empty;
            db_sheetid = -1;
		}

        /// <summary> Constructor for a new instance of the Map_Sheet class </summary>
        /// <param name="SheetID"> Primary key for this map sheet in relation to the map set item </param>
        /// <param name="Index"> Index of this sheet within the entire set </param>
        /// <param name="FilePtr"> File pointer string </param>
        /// <param name="File"> File name </param>
		public Map_Sheet( long SheetID, int Index, string FilePtr, string File )
		{
			sheetid = SheetID;
			index = Index;
			filePtr = FilePtr;
			file = File;
            db_sheetid = -1;
		}

        /// <summary> Primary key for this sheet from the authority database </summary>
        public int Database_Sheet_ID
        {
            get {   return db_sheetid; }
            set {   db_sheetid = value;  }
        }

        /// <summary> Primary key for this map sheet in relation to the map set item </summary>
		public long SheetID
		{
			get	{	return sheetid;			}
			set	{	sheetid = value;		}
		}

        /// <summary>  Index of this sheet within the entire set </summary>
		public int Index
		{
			get	{	return index;		}
			set	{	index = value;		}
		}

        /// <summary>  File pointer string  </summary>
		public string FilePtr
		{
			get	{	return filePtr;		}
			set	{	filePtr = value;	}
		}

        /// <summary> File name </summary>
		public string File
		{
			get	{	return file;		}
			set	{	file = value;		}
		}
	}
}
