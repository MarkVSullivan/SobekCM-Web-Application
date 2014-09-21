using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Core.Users
{
    /// <summary> Represents a single user which is part of a user group. This contains only the very
    /// basic user information </summary>   
    [DataContract]
    public class User_Group_Member
    {
        /// <summary> User's email address </summary>
        [DataMember(EmitDefaultValue = false)]
        public readonly string Email;

        /// <summary> Returns the user's full name in [first name last name] order</summary>
        [DataMember]
        public readonly string FullName;

        /// <summary> UserID (or primary key) to this user from the database </summary>
        [DataMember]
        public readonly int UserID;

        /// <summary> SobekCM username for this user </summary>
        [DataMember]
        public readonly string UserName;

        /// <summary> Constructor for a new instance of the User_Group_Member class </summary>
        /// <param name="UserName">SobekCM username for this user</param>
        /// <param name="FullName">Returns the user's full name in [first name last name] order</param>
        /// <param name="Email">User's email address</param>
        /// <param name="UserID">serID (or primary key) to this user from the database</param>
        public User_Group_Member(string UserName, string FullName, string Email, int UserID)
        {
            this.UserName = UserName;
            this.FullName = FullName;
            this.Email = Email;
            this.UserID = UserID;
        }
    }
}
