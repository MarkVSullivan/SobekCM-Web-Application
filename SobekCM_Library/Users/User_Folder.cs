#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.Users
{
    /// <summary> Represents a single folder in the folder hierarchy for this user </summary>
    public class User_Folder
    {
        /// <summary> Readonly field stores the primary key to this folder </summary>
        public readonly int Folder_ID;

        /// <summary> Readonly field stores the name of this folder </summary>
        public readonly string Folder_Name;

        private readonly SortedList<string,User_Folder> children;

        /// <summary> Constructor for a new instance of the User_Folder class </summary>
        /// <param name="Folder_Name"> Name of this folder </param>
        /// <param name="Folder_ID"> Primary key to this folder </param>
        public User_Folder( string Folder_Name, int Folder_ID )
        {
            this.Folder_Name = Folder_Name;
            this.Folder_ID = Folder_ID;
            children = new SortedList<string,User_Folder>();

            isPublic = false;
        }

        /// <summary> Gets the folder name with some of the special characters encoded for HTML  </summary>
        public string Folder_Name_Encoded
        {
            get
            {
                return Folder_Name.Replace("\"", "%22").Replace("'", "%27").Replace("=", "%3D").Replace("&", "%26");
            }
        }

        /// <summary> Read-only collection of the children folders under this folder </summary>
        public ReadOnlyCollection<User_Folder> Children
        {
            get { return new ReadOnlyCollection<User_Folder>(children.Values); }
        }

        /// <summary> Flag indicates if this folder is public </summary>
        public bool isPublic { get; set; }

        /// <summary> Gets the number of children folders under this folder </summary>
        public int Child_Count
        {
            get { return children.Count; }
        }
        /// <summary> Adds a new folder as a child under this folder </summary>
        /// <param name="Child_Folder_Name"> Name of the new child folder </param>
        /// <param name="Child_Folder_ID"> Primary key for child folder </param>
        /// <returns> Child folder object </returns>
        internal User_Folder Add_Child_Folder(string Child_Folder_Name, int Child_Folder_ID)
        {
            User_Folder returnValue = new User_Folder(Child_Folder_Name, Child_Folder_ID);
            children.Add(Child_Folder_Name, returnValue);
            return returnValue;
        }

        /// <summary> Adds a new folder as a child under this folder </summary>
        /// <param name="Child_Folder"> Completely built child folder object </param>
        internal void Add_Child_Folder( User_Folder Child_Folder )
        {
            children.Add(Child_Folder.Folder_Name, Child_Folder);
        }
    }
}
