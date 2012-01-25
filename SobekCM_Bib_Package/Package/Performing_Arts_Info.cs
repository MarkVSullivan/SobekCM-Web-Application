using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SobekCM.Bib_Package
{
    #region Performer class

    /// <summary> Class encapsulates all the information about a single performer  </summary>
    public class Performer
    {
        private string lifespan;
        private string title;
        private string occupation;
        private string sex;
        private string name;

        /// <summary> Constructor creates a new instance of the Performer class </summary>
        public Performer()
        {
            // Do nothing
        }

        /// <summary> Constructor creates a new instance of the Performer class </summary>
        /// <param name="Name">Name of the performer</param>
        public Performer( string Name )
        {
            name = Name;
        }

        /// <summary> Gets and sets the lifespan of this performer </summary>
        public string LifeSpan
        {
            get  {  return lifespan ?? String.Empty;   }
            set  {  lifespan = value;  }
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
	public class Performing_Arts_Info : XML_Writing_Base_Type
	{
		private string performance;
		private string date;
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
				if (( !String.IsNullOrEmpty(date)) || ( !String.IsNullOrEmpty(performance)) || ( Performers_Count > 0 ))
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
			set	{	performance = value;		}
		}

		/// <summary> Gets and sets the date of this performance </summary>
		public string Performance_Date
		{
            get { return date ?? String.Empty; }
			set	{	date = value;		}
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

		/// <summary> Gets the Greenstone Archival format XML for the performing arts information associated with the resource </summary>
		internal string GSA_Performing_Arts_Metadata
		{
			get
			{
				StringBuilder result = new StringBuilder();
				string indent = "    ";

				// Write the data for each performer in this collection
                if (( performers != null ) && ( performers.Count > 0 ))
                {
                    foreach (Performer thisPerformer in performers)
                    {
                        result.Append(indent + "<Metadata name=\"sobekcm.PerformingArts^performer\">" + base.Convert_String_To_XML_Safe(thisPerformer.Name));

                        if (thisPerformer.LifeSpan.Length > 0)
                        {
                            result.Append(", " + thisPerformer.LifeSpan);
                        }
                        result.Append("</Metadata>\r\n");
                    }
                }

				// Add the performance place as its own metadata, if there is one
				if ( !String.IsNullOrEmpty(performance))
				{
					if ( !String.IsNullOrEmpty(date))
					{
                        result.Append(indent + "<Metadata name=\"sobekcm.PerformingArts^performance\">" + base.Convert_String_To_XML_Safe(performance) + " ( " + base.Convert_String_To_XML_Safe(date) + " )</Metadata>\r\n");
					}
					else
					{
                        result.Append(indent + "<Metadata name=\"sobekcm.PerformingArts^performance\">" + base.Convert_String_To_XML_Safe(performance) + "</Metadata>\r\n");
					}
				}

				// Close the creator tab
				return result.ToString();

			}
		}

		/// <summary> Gets the METS format XML for the performing arts information associated with the resource </summary>
        internal void Add_METS_Performing_Arts_Metadata( System.IO.TextWriter results)
        {
            if (!hasData)
                return;

            // Start this section
            results.Write( "<part:performingArts>\r\n");

            // Add all the performer data
            if ((performers != null) && (performers.Count > 0))
            {
                foreach (Performer thisPerformer in performers)
                {
                    results.Write( "<part:Performer");
                    if (thisPerformer.LifeSpan.Length > 0)
                        results.Write(" lifespan=\"" + base.Convert_String_To_XML_Safe(thisPerformer.LifeSpan) + "\"");
                    if (thisPerformer.Title.Length > 0)
                        results.Write(" title=\"" + base.Convert_String_To_XML_Safe(thisPerformer.Title) + "\"");
                    if (thisPerformer.Occupation.Length > 0)
                        results.Write(" occupation=\"" + base.Convert_String_To_XML_Safe(thisPerformer.Occupation) + "\"");
                    if (thisPerformer.Sex.Length > 0)
                        results.Write(" sex=\"" + thisPerformer.Sex + "\"");
                    results.Write(">" + base.Convert_String_To_XML_Safe(thisPerformer.Name) + "</part:Performer>\r\n");
                }
            }

            // Add the performance information
            if (( !String.IsNullOrEmpty(performance)) || ( !String.IsNullOrEmpty(date)))
            {
                string performanceName = base.Convert_String_To_XML_Safe(performance);
                if (performanceName.Length == 0)
                    performanceName = "Unknown";
                results.Write( "<part:Performance");
                if (!String.IsNullOrEmpty(date))
                    results.Write(" date=\"" + base.Convert_String_To_XML_Safe(date) + "\"");
                results.Write(">" + performanceName + "</part:Performance>\r\n");
            }

            //// End this section
            results.Write( "</part:performingArts>\r\n");
        }
	}
}
