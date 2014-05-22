#region Using directives

using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Library.Users
{
    /// <summary> Represents a mySobek user group, which allows some permissions and information to be assigned
    /// to a collection of individual mySobek users.  </summary>
    public class User_Group
    {
        #region Private class members

        private readonly User_Editable_Collection aggregations;
        private readonly List<string> editableRegexes;
        private readonly List<string> defaultMetadataSets;
        private readonly List<string> templates;
        private readonly List<User_Group_Member> users;

        #endregion

        #region Constructor

        /// <summary> Constructor for a new instance of the User_Group class </summary>
        /// <param name="Name">Name for this SobekCM user group</param>
        /// <param name="Description">Description for this SobekCM user group</param>
        /// <param name="UserGroupID">UserGroupID (or primary key) to this user group from the database</param>
        public User_Group(string Name, string Description, int UserGroupID)
        {
            this.Name = Name;
            this.Description = Description;
            this.UserGroupID = UserGroupID;
            Can_Submit = false;
            Is_Internal_User = false;
            Is_System_Admin = false;
            templates = new List<string>();
            defaultMetadataSets = new List<string>();
            aggregations = new User_Editable_Collection();
            editableRegexes = new List<string>();
            users = new List<User_Group_Member>();
        }

        #endregion

        #region Public Properties

        /// <summary> Name for this SobekCM user group </summary>
        public string Name { get; set; }

        /// <summary> Description for this SobekCM user group </summary>
        public string Description { get; set; }

        /// <summary> UserGroupID (or primary key) to this user group from the database </summary>
        public int UserGroupID { get; set; }

        /// <summary> Simple flag indicates if this user group can submit items </summary>
        public bool Can_Submit { get; set; }

        /// <summary> Flag indicates if this is an internal user group </summary>
        /// <remarks>This grants access to various tracking elements in SobekCM</remarks>
        public bool Is_Internal_User { get; set; }

        /// <summary> Flag indicates if this user group has general admin rights over the entire system </summary>
        public bool Is_System_Admin { get; set; }

        /// <summary> Ordered list of submittal templates this user group has access to </summary>
        public ReadOnlyCollection<string> Templates
        {
            get { return new ReadOnlyCollection<string>(templates); }
        }

        /// <summary> Ordered list of default metadata sets this user group has access to </summary>
        public ReadOnlyCollection<string> Default_Metadata_Sets
        {
            get { return new ReadOnlyCollection<string>(defaultMetadataSets); }
        }

        /// <summary> List of item aggregations associated with this user group </summary>
        public ReadOnlyCollection<User_Editable_Aggregation> Aggregations
        {
            get { return aggregations.Collection; }
        }

        /// <summary> List of regular expressions for checking for edit by bibid </summary>
        public ReadOnlyCollection<string> Editable_Regular_Expressions
        {
            get { return new ReadOnlyCollection<string>(editableRegexes); }
        }

        /// <summary> Gets the list of users associated with this user group </summary>
        public ReadOnlyCollection<User_Group_Member> Users
        {
            get { return new ReadOnlyCollection<User_Group_Member>(users); }
        }

        #endregion

        #region Internal methods for modifying the collections of editable objects ( bibid, templates, projects, aggregations, etc..)

        /// <summary> Adds a user to the list of users which belong to this user group </summary>
        /// <param name="User"> Small user object which holds the very basic information about this user </param>
        internal void Add_User(User_Group_Member User)
        {
            users.Add(User);
        }

        /// <summary> Adds a user to the list of users which belong to this user group </summary>
        /// <param name="UserName">SobekCM username for this user</param>
        /// <param name="Full_Name">Returns the user's full name in [first name last name] order</param>
        /// <param name="Email">User's email address</param>
        /// <param name="UserID">serID (or primary key) to this user from the database</param>
        internal void Add_User(string UserName, string Full_Name, string Email, int UserID)
        {
            users.Add(new User_Group_Member(UserName, Full_Name, Email, UserID));
        }

        /// <summary> Add a new item aggregation to this user group's collection of item aggregations </summary>
        /// <param name="Code">Code for this user editable item aggregation</param>
        /// <param name="Aggregation_Name">Name for this user editable item aggregation </param>
        /// <param name="CanSelect">Flag indicates if this user can add items to this item aggregation</param>
        /// <param name="CanEditItems">Flag indicates if this user can edit any items in this item aggregation</param>
        /// <param name="IsCurator"> Flag indicates if this user is listed as the curator or collection manager for this given digital aggregation </param>
		internal void Add_Aggregation(string Code, string Aggregation_Name, bool CanSelect, bool CanEditMetadata, bool CanEditBehaviors, bool CanPerformQc, bool CanUploadFiles, bool CanChangeVisibility, bool CanDelete, bool IsCurator, bool IsAdmin)
        {
            aggregations.Add(Code, Aggregation_Name, CanSelect, CanEditMetadata, CanEditBehaviors, CanPerformQc, CanUploadFiles, CanChangeVisibility, CanDelete, IsCurator, false, IsAdmin, true );
        }

        /// <summary> Adds a template to the list of templates this user group can select </summary>
        /// <param name="Template">Code for this template</param>
        /// <remarks>This must match the name of one of the template XML files in the mySobek\templates folder</remarks>
        internal void Add_Template(string Template)
        {
            templates.Add(Template);
        }

        /// <summary> Adds a default metadata set to the list of sets this user group can select </summary>
        /// <param name="MetadataSet">Code for this default metadata set</param>
        /// <remarks>This must match the name of one of the project METS (.pmets) files in the mySobek\projects folder</remarks>
        internal void Add_Default_Metadata_Set(string MetadataSet)
        {
            defaultMetadataSets.Add(MetadataSet);
        }

        /// <summary> Adds a regular expression to this user group to determine which titles this user can edit </summary>
        /// <param name="Regular_Expression"> Regular expression used to compute if this user group can edit a title, by BibID</param>
        internal void Add_Editable_Regular_Expression(string Regular_Expression)
        {
            editableRegexes.Add(Regular_Expression);
        }

        #endregion
    }

    /// <summary> Represents a single user which is part of a user group. This contains only the very
    /// basic user information </summary>
    public class User_Group_Member
    {
        /// <summary> User's email address </summary>
        public readonly string Email;

        /// <summary> Returns the user's full name in [first name last name] order</summary>
        public readonly string Full_Name;

        /// <summary> UserID (or primary key) to this user from the database </summary>
        public readonly int UserID;

        /// <summary> SobekCM username for this user </summary>
        public readonly string UserName;

        /// <summary> Constructor for a new instance of the User_Group_Member class </summary>
        /// <param name="UserName">SobekCM username for this user</param>
        /// <param name="Full_Name">Returns the user's full name in [first name last name] order</param>
        /// <param name="Email">User's email address</param>
        /// <param name="UserID">serID (or primary key) to this user from the database</param>
        public User_Group_Member(string UserName, string Full_Name, string Email, int UserID)
        {
            this.UserName = UserName;
            this.Full_Name = Full_Name;
            this.Email = Email;
            this.UserID = UserID;
        }
    }
}
