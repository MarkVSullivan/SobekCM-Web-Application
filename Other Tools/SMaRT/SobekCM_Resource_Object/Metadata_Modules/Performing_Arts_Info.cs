#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

#endregion

namespace SobekCM.Resource_Object.Metadata_Modules
{

    #region Performer class

    /// <summary> Class encapsulates all the information about a single performer  </summary>
    public class Performer
    {
        private string lifespan;
        private string name;
        private string occupation;
        private string sex;
        private string title;

        /// <summary> Constructor creates a new instance of the Performer class </summary>
        public Performer()
        {
            // Do nothing
        }

        /// <summary> Constructor creates a new instance of the Performer class </summary>
        /// <param name="Name">Name of the performer</param>
        public Performer(string Name)
        {
            name = Name;
        }

        /// <summary> Gets and sets the lifespan of this performer </summary>
        public string LifeSpan
        {
            get { return lifespan ?? String.Empty; }
            set { lifespan = value; }
        }

        /// <summary> Gets and sets the title of this performer </summary>
        public string Title
        {
            get { return title ?? String.Empty; }
            set { title = value; }
        }

        /// <summary> Gets and sets the occupation of this performer </summary>
        public string Occupation
        {
            get { return occupation ?? String.Empty; }
            set { occupation = value; }
        }

        /// <summary> Gets and sets the sex of this performer </summary>
        public string Sex
        {
            get { return sex ?? String.Empty; }
            set { sex = value; }
        }

        /// <summary> Gets and sets the name of this performer </summary>
        public string Name
        {
            get { return name ?? String.Empty; }
            set { name = value; }
        }
    }

    #endregion

    /// <summary> Stores performing arts specific information for this resource </summary>
    /// <remarks> Object created by Mark V Sullivan (2006) for University of Florida's Digital Library Center.</remarks>
    [Serializable]
    public class Performing_Arts_Info : iMetadata_Module
    {
        private string date;
        private string performance;
        private List<Performer> performers;

        /// <summary> Constructor for a new instance of the Performing_Arts_Info class </summary>
        public Performing_Arts_Info()
        {
            // Do nothing
        }

        /// <summary> Gets flag indicating if this has any data </summary>
        internal bool hasData
        {
            get
            {
                if ((!String.IsNullOrEmpty(date)) || (!String.IsNullOrEmpty(performance)) || (Performers_Count > 0))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
        }

        /// <summary> Gets and sets the name of this performance </summary>
        public string Performance
        {
            get { return performance ?? String.Empty; }
            set { performance = value; }
        }

        /// <summary> Gets and sets the date of this performance </summary>
        public string Performance_Date
        {
            get { return date ?? String.Empty; }
            set { date = value; }
        }

        /// <summary> Gets the number of performers linked to this digital object </summary>
        public int Performers_Count
        {
            get
            {
                if (performers == null)
                    return 0;
                else
                    return performers.Count;
            }
        }

        /// <summary> Gets the collection of performers for this item </summary>
        public ReadOnlyCollection<Performer> Performers
        {
            get
            {
                if (performers == null)
                    return new ReadOnlyCollection<Performer>(new List<Performer>());
                else
                    return new ReadOnlyCollection<Performer>(performers);
            }
        }

        /// <summary> Add a new performer to the collection of performers associated with this item </summary>
        /// <param name="Name">Name of the performer</param>
        public Performer Add_Performer(string Name)
        {
            if (performers == null)
                performers = new List<Performer>();

            Performer newPerformer = new Performer(Name);
            performers.Add(newPerformer);
            return newPerformer;
        }

        #region Methods/Properties to implement the iMetadata_Module interface

        /// <summary> Name for this metadata module </summary>
        /// <value> This always returns 'PerformingArts' </value>
        public string Module_Name
        {
            get { return "PerformingArts"; }
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
    }
}