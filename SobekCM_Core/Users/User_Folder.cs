#region Using directives

using System.Collections.Generic;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Users
{
    /// <summary> Represents a single folder in the folder hierarchy for this user </summary>
    [DataContract]
    public class User_Folder
    {
        /// <summary> Readonly field stores the primary key to this folder </summary>
        [DataMember]
        public readonly int Folder_ID;

        /// <summary> Readonly field stores the name of this folder </summary>
        [DataMember]
        public readonly string Folder_Name;

        /// <summary> Constructor for a new instance of the User_Folder class </summary>
        /// <param name="Folder_Name"> Name of this folder </param>
        /// <param name="Folder_ID"> Primary key to this folder </param>
        public User_Folder( string Folder_Name, int Folder_ID )
        {
            this.Folder_Name = Folder_Name;
            this.Folder_ID = Folder_ID;

            IsPublic = false;
        }

        /// <summary> Gets the folder name with some of the special characters encoded for HTML  </summary>
        public string Folder_Name_Encoded
        {
            get
            {
                return Folder_Name.Replace("\"", "%22").Replace("'", "%27").Replace("=", "%3D").Replace("&", "%26");
            }
        }

        /// <summary> Collection of the children folders under this folder </summary>
        [DataMember(EmitDefaultValue = false)]
        public List<User_Folder> Children { get; set; }

        /// <summary> Flag indicates if this folder is public </summary>
        [DataMember]
        public bool IsPublic { get; set; }

        /// <summary> Gets the number of children folders under this folder </summary>
        public int Child_Count
        {
            get 
            {
                if (Children == null)
                    return 0;
                else
                    return Children.Count;
            }
        }

        /// <summary> Adds a new folder as a child under this folder </summary>
        /// <param name="Child_Folder_Name"> Name of the new child folder </param>
        /// <param name="Child_Folder_ID"> Primary key for child folder </param>
        /// <returns> Child folder object </returns>
        public User_Folder Add_Child_Folder(string Child_Folder_Name, int Child_Folder_ID)
        {
            User_Folder returnValue = new User_Folder(Child_Folder_Name, Child_Folder_ID);
            Add_Child_Folder(returnValue);
            return returnValue;
        }

        /// <summary> Adds a new folder as a child under this folder </summary>
        /// <param name="Child_Folder"> Completely built child folder object </param>
        public void Add_Child_Folder(User_Folder Child_Folder)
        {
            if (Children == null)
                Children = new List<User_Folder>();

            Children.Add(Child_Folder);
        }
    }
}
