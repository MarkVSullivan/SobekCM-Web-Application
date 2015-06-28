#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Text;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{
    /// <summary> Base class from which the standard subject, title as subject, and name as subject extend </summary>
    [Serializable]
    public abstract class Subject_Standard_Base : Subject_Info
    {
        /// <summary> Protected list of all the genre subject keywords in this complex subject object </summary>
        protected List<string> genres;

        /// <summary> Protected list of all the geographic subject keywords in this complex subject object </summary>
        protected List<string> geographics;

        /// <summary> Protected list of all the temporal subject keywords in this complex subject object </summary>
        protected List<string> temporals;

        /// <summary> Protected list of all the topical subject keywords in this complex subject object </summary>
        protected List<string> topics;

        /// <summary> Constructor for a new instance of the standard subject base class </summary>
        protected Subject_Standard_Base()
        {
            // Do nothing
        }

        /// <summary> Gets flag indicating if there is any data in this subject object </summary>
        public bool hasData
        {
            get
            {
                return ((topics != null) && (topics.Count > 0)) ||
                       ((geographics != null) && (geographics.Count > 0)) ||
                       ((temporals != null) && (temporals.Count > 0)) ||
                       ((genres != null) && (genres.Count > 0));
            }
        }

        /// <summary> Gets the number of topical subjects </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Topics"/> property.  Even if 
        /// there are no topical subjects, the Topics property creates a readonly collection to pass back out.</remarks>
        public int Topics_Count
        {
            get {
                return topics == null ? 0 : topics.Count;
            }
        }

        /// <summary> Gets the list of topical subjects </summary>
        /// <remarks> You should check the count of topical subjects first using the <see cref="Topics_Count"/> before using this property.
        /// Even if there are no topical subjects, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Topics
        {
            get {
                return topics == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(topics);
            }
        }

        /// <summary> Gets the number of geographic subjects </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Geographics"/> property.  Even if 
        /// there are no geographic subjects, the Geographics property creates a readonly collection to pass back out.</remarks>
        public int Geographics_Count
        {
            get {
                return geographics == null ? 0 : geographics.Count;
            }
        }

        /// <summary> Gets the list of geographic subjects </summary>
        /// <remarks> You should check the count of geographic subjects first using the <see cref="Geographics_Count"/> before using this property.
        /// Even if there are no geographic subjects, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Geographics
        {
            get {
                return geographics == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(geographics);
            }
        }

        /// <summary> Gets the number of temporal subjects </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Temporals"/> property.  Even if 
        /// there are no temporal subjects, the Temporals property creates a readonly collection to pass back out.</remarks>
        public int Temporals_Count
        {
            get {
                return temporals == null ? 0 : temporals.Count;
            }
        }

        /// <summary> Gets the list of temporal subjects </summary>
        /// <remarks> You should check the count of temporal subjects first using the <see cref="Temporals_Count"/> before using this property.
        /// Even if there are no temporal subjects, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Temporals
        {
            get {
                return temporals == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(temporals);
            }
        }

        /// <summary> Gets the number of genre subjects </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Genres"/> property.  Even if 
        /// there are no genre subjects, the Genres property creates a readonly collection to pass back out.</remarks>
        public int Genres_Count
        {
            get {
                return genres == null ? 0 : genres.Count;
            }
        }

        /// <summary> Gets the list of genre subjects </summary>
        /// <remarks> You should check the count of genre subjects first using the <see cref="Genres_Count"/> before using this property.
        /// Even if there are no genre subjects, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<string> Genres
        {
            get {
                return genres == null ? new ReadOnlyCollection<string>(new List<string>()) : new ReadOnlyCollection<string>(genres);
            }
        }

        /// <summary> Add a new topical subject keyword to this subject </summary>
        /// <param name="NewTerm">New subject term to add</param>
        public void Add_Topic(string NewTerm)
        {
            if (topics == null)
                topics = new List<string>();

            if (!topics.Contains(NewTerm))
                topics.Add(NewTerm);
        }

        /// <summary> Clear all the topical subject keywords from this subject </summary>
        public void Clear_Topics()
        {
            if (topics != null)
                topics.Clear();
        }

        /// <summary> Add a new geographic subject keyword to this subject </summary>
        /// <param name="NewTerm">New subject term to add</param>
        public void Add_Geographic(string NewTerm)
        {
            if (geographics == null)
                geographics = new List<string>();

            if (!geographics.Contains(NewTerm))
                geographics.Add(NewTerm);
        }

        /// <summary> Clear all geographic terms from this subject </summary>
        public void Clear_Geographics()
        {
            if (geographics != null)
                geographics.Clear();
        }

        /// <summary> Add a new temporal subject keyword to this subject </summary>
        /// <param name="NewTerm">New subject term to add</param>
        public void Add_Temporal(string NewTerm)
        {
            if (temporals == null)
                temporals = new List<string>();

            if (!temporals.Contains(NewTerm))
                temporals.Add(NewTerm);
        }

        /// <summary> Clear all the temporal subject keywords from this subject </summary>
        public void Clear_Temporals()
        {
            if (temporals != null)
                temporals.Clear();
        }

        /// <summary> Add a new genre subject keyword to this subject </summary>
        /// <param name="NewTerm">New subject term to add</param>
        public void Add_Genre(string NewTerm)
        {
            if (genres == null)
                genres = new List<string>();

            if (!genres.Contains(NewTerm))
                genres.Add(NewTerm);
        }

        /// <summary> Clear all the genre subject keywords from this subject </summary>
        public void Clear_Genres()
        {
            if (genres != null)
                genres.Clear();
        }

        /// <summary> Write the subject information out to string format</summary>
        /// <returns> This subject expressed as a string</returns>
        public string To_Base_String()
        {
            StringBuilder builder = new StringBuilder();

            if (topics != null)
            {
                foreach (string thisTopic in topics)
                {
                    {
                        if (builder.Length > 0)
                        {
                            builder.Append(" -- " + thisTopic);
                        }
                        else
                        {
                            builder.Append(thisTopic);
                        }
                    }
                }
            }

            if (genres != null)
            {
                foreach (string thisGenre in genres)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(" -- " + thisGenre);
                    }
                    else
                    {
                        builder.Append(thisGenre);
                    }
                }
            }

            if (geographics != null)
            {
                foreach (string thisGeographic in geographics)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(" -- " + thisGeographic);
                    }
                    else
                    {
                        builder.Append(thisGeographic);
                    }
                }
            }

            if (temporals != null)
            {
                foreach (string thisTemporal in temporals)
                {
                    if (builder.Length > 0)
                    {
                        builder.Append(" -- " + thisTemporal);
                    }
                    else
                    {
                        builder.Append(thisTemporal);
                    }
                }
            }

            return Convert_String_To_XML_Safe(builder.ToString());
        }

        /// <summary> Add the base MODS section, which includes the topic, genre, geographic, and temporal key words </summary>
        /// <param name="Results"> Stream to write the resulting MODS-formatted XML to</param>
        protected void Add_Base_MODS(TextWriter Results)
        {
            if (topics != null)
            {
                foreach (string thisElement in topics)
                {
                    Results.Write("<mods:topic>" + Convert_String_To_XML_Safe(thisElement) + "</mods:topic>\r\n");
                }
            }

            if (geographics != null)
            {
                foreach (string thisElement in geographics)
                {
                    Results.Write("<mods:geographic>" + Convert_String_To_XML_Safe(thisElement) + "</mods:geographic>\r\n");
                }
            }

            if (temporals != null)
            {
                foreach (string thisElement in temporals)
                {
                    Results.Write("<mods:temporal>" + Convert_String_To_XML_Safe(thisElement) + "</mods:temporal>\r\n");
                }
            }

            if (genres != null)
            {
                foreach (string thisElement in genres)
                {
                    Results.Write("<mods:genre>" + Convert_String_To_XML_Safe(thisElement) + "</mods:genre>\r\n");
                }
            }
        }
    }
}