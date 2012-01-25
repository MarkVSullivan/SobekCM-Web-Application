using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Text;

namespace SobekCM.Bib_Package.Bib_Info
{
    /// <summary> Class contains the information about the publisher of a resource </summary>
    [Serializable]
    public class Publisher_Info : XML_Node_Base_Type, IEquatable<Publisher_Info>
    {
        private List<Origin_Info_Place> places;
        string name;

        /// <summary> Constructor create a new instance of the Publisher_Info class </summary>
        public Publisher_Info()
        {
            // Do nothing
        }

        /// <summary> Constructor create a new instance of the Publisher_Info class </summary>
        /// <param name="Name">Name of the publisher </param>
        public Publisher_Info( string Name )
        {
            name = Name;
        }

        /// <summary> Gets and sets the name of this publisher  </summary>
        public string Name
        {
            get { return name ?? String.Empty; }
            set { name = value; }
        }

        /// <summary> Gets the number of places associated with this publisher </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Places"/> property.  Even if 
        /// there are no places, the SubCollections property creates a readonly collection to pass back out.</remarks>
        public int Places_Count
        {
            get
            {
                if (places == null)
                    return 0;
                else
                    return places.Count;
            }
        }

        /// <summary>Gets the list of places associated with this publisher </summary>
        /// <remarks> You should check the count of places first using the <see cref="Places_Count"/> property before using this property.
        /// Even if there are no places, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Origin_Info_Place> Places
        {
            get
            {
                if (places == null)
                    return new ReadOnlyCollection<Origin_Info_Place>(new List<Origin_Info_Place>());
                else
                    return new ReadOnlyCollection<Origin_Info_Place>(places);
            }
        }

        /// <summary> Clears all places associated with this publisher </summary>
        public void Clear_Places()
        {
            if (places != null)
                places.Clear();
        }

        /// <summary> Add a new publication place </summary>
        /// <param name="Place_Text">Text of the publication place</param>
        public void Add_Place(string Place_Text)
        {
            if (places == null)
                places = new List<Origin_Info_Place>();

            places.Add(new Origin_Info_Place(Place_Text, String.Empty, String.Empty));
        }

        /// <summary> Add a new publication place </summary>
        /// <param name="Place_Text">Text of the publication place</param>
        /// <param name="Place_MarcCountry">Marc country code for the publication place</param>
        /// <param name="Place_ISO3166">ISO-3166 code for the publication place</param>
        public void Add_Place(string Place_Text, string Place_MarcCountry, string Place_ISO3166)
        {
            if (places == null)
                places = new List<Origin_Info_Place>();

            places.Add(new Origin_Info_Place(Place_Text, Place_MarcCountry, Place_ISO3166));
        }

        #region IEquatable Members

        /// <summary> Compares this object with another similarly typed object </summary>
        /// <param name="other">Similarly types object </param>
        /// <returns>TRUE if the two objects are sufficiently similar</returns>
        public bool Equals(Publisher_Info other)
        {
            if ( other.Name == Name )
                return true;

            return false;
        }

        #endregion  

        /// <summary> Write the publisher information out to string format</summary>
        /// <returns> This publisher expressed as a string</returns>
        public override string ToString()
        {
            StringBuilder builder = new StringBuilder();
            if ( !String.IsNullOrEmpty(name))
            {
                builder.Append(base.Convert_String_To_XML_Safe(name));
            }
            if (( places != null ) && ( places.Count > 0))
            {
                builder.Append(" ( ");
                foreach (Origin_Info_Place thisPlace in places)
                {
                    if (thisPlace.Place_Text.Length > 0)
                    {
                        builder.Append(base.Convert_String_To_XML_Safe(thisPlace.Place_Text) + ", " );
                    }
                }
                builder.Append(")");
            }
            return builder.ToString().Replace(", )", " )");
        }

        /// <summary> Writes this publisher as SobekCM-formatted XML </summary>
        /// <param name="sobekcm_namespace"> Namespace to use for the SobekCM custom schema ( usually 'sobekcm' )</param>
        /// <param name="results"> Stream to write this publisher as SobekCM-formatted XML</param>
        /// <param name="type"> Type indicates if this is a publisher or a manufacturer of the digital resource </param>
        internal void Add_SobekCM_Metadata(string sobekcm_namespace, string type, System.IO.TextWriter results)
        {
            if (!String.IsNullOrEmpty(name))
            {
                results.Write( "<" + sobekcm_namespace + ":" + type );
                base.Add_ID( results );
                results.WriteLine(">");

                results.WriteLine( "<" + sobekcm_namespace + ":Name>" + base.Convert_String_To_XML_Safe(name) + "</" + sobekcm_namespace + ":Name>");

                // Step through all the publication places
                if (places != null)
                {
                    foreach (Origin_Info_Place place in places)
                    {
                        if ((place.Place_ISO3166.Length > 0) || (place.Place_MarcCountry.Length > 0) || (place.Place_Text.Length > 0))
                        {
                            if (place.Place_Text.Length > 0)
                                results.WriteLine( "<" + sobekcm_namespace + ":PlaceTerm type=\"text\">" + base.Convert_String_To_XML_Safe(place.Place_Text) + "</" + sobekcm_namespace + ":PlaceTerm>");
                            if (place.Place_MarcCountry.Length > 0)
                                results.WriteLine( "<" + sobekcm_namespace + ":PlaceTerm type=\"code\" authority=\"marccountry\">" + place.Place_MarcCountry + "</" + sobekcm_namespace + ":PlaceTerm>");
                            if (place.Place_ISO3166.Length > 0)
                                results.WriteLine( "<" + sobekcm_namespace + ":PlaceTerm type=\"code\" authority=\"iso3166\">" + place.Place_ISO3166 + "</" + sobekcm_namespace + ":PlaceTerm>");
                        }
                    }
                }

                results.WriteLine( "</" + sobekcm_namespace + ":" + type + ">");
            }
        }
    }
}
