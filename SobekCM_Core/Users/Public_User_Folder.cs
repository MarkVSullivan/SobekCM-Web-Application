#region Using directives

using System;
using System.Runtime.Serialization;

#endregion

namespace SobekCM.Core.Users
{
    /// <summary> This represents the public folder for a user for display by any user. </summary>
    [DataContract]
    public class Public_User_Folder
    {
        /// <summary> Readonly fields contains the email address for the owner of this folder </summary>
        [DataMember]
        public readonly string Email;

        /// <summary> Readonly fields contains the first name for the owner of this folder </summary>
        [DataMember]
        public readonly string FirstName;

        /// <summary> Readonly fields contains the description for this folder </summary>
        [DataMember]
        public readonly string FolderDescription;

        /// <summary> Readonly fields contains the name of this folder </summary>
        [DataMember]
        public readonly string FolderName;

        /// <summary> Readonly fields contains the last name for the owner of this folder </summary>
        [DataMember]
        public readonly string LastName;

        /// <summary> Display name for the owner of this public folder </summary>
        [DataMember]
        public readonly string Name;

        /// <summary> Readonly fields contains the nickname for the owner of this folder </summary>
        [DataMember]
        public readonly string Nickname;

        /// <summary> Readonly fields contains the primary key for this user folder </summary>
        [DataMember]
        public readonly int UserFolderID;

        /// <summary> Readonly fields contains the primary key for this owner of this folder </summary>
        [DataMember]
        public readonly int UserID;

        /// <summary> Readonly fields contains flag indicating if this folder is public </summary>
        [DataMember]
        public readonly bool IsPublic;

        /// <summary> Constructor for a new instance of the Public_User_Folder class </summary>
        /// <param name="UserFolderID"> Primary key for this user folder </param>
        /// <param name="UserID"> Primary key for the owner of this folder </param>
        /// <param name="FolderName"> Name of this folder</param>
        /// <param name="FolderDescription"> Description of this folder</param>
        /// <param name="FirstName"> First name of the owner of this folder </param>
        /// <param name="LastName"> Last name of the owner of this folder </param>
        /// <param name="Nickname"> Nickname of the owner of this folder </param>
        /// <param name="Email"> Email address for the owner of this folder</param>
        /// <param name="IsPublic"> Flag indicates if this folder is actually public</param>
        /// <remarks> This constructor is used when a folder is reqeuested publicly which then turns out to be private</remarks>
        public Public_User_Folder(int UserFolderID, string FolderName, string FolderDescription, int UserID, string FirstName, string LastName, string Nickname, string Email, bool IsPublic )
        {
            this.UserFolderID = UserFolderID;
            this.FolderName = FolderName;
            this.FolderDescription = FolderDescription;
            this.UserID = UserID;
            this.FirstName = FirstName;
            this.LastName = LastName;
            this.Nickname = Nickname;
            this.Email = Email;
            this.IsPublic = IsPublic;

            if ( Nickname.Length > 0 )
            {
                Name = Nickname + " " + LastName;
            }
            else
            {
                Name = FirstName + " " + LastName;
            }
        }

        /// <summary> Constructor for a new instance of the Public_User_Folder class </summary>
        /// <param name="IsPublic"> Flag indicates if this folder is actually public</param>
        /// <remarks> This constructor is used when a folder is reqeuested publicly which then turns out to be private</remarks>
        public Public_User_Folder(bool IsPublic)
        {
            UserFolderID = -1;
            FolderName = String.Empty;
            FolderDescription = String.Empty;
            UserID = -1;
            FirstName = String.Empty;
            LastName = string.Empty;
            Nickname = string.Empty;
            Email = String.Empty;
            this.IsPublic = IsPublic;
        }
    }
}
