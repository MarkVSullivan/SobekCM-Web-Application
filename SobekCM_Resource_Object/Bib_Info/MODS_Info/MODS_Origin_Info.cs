#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;

#endregion

namespace SobekCM.Resource_Object.Bib_Info
{

    #region Origin_Info_Place class definition

    /// <summary> Contains information about a single origination location (publication place) for an item </summary>
    [Serializable]
    public class Origin_Info_Place
    {
        private string placeISO3166;
        private string placeMarcCountry;
        private string placeText;

        /// <summary> Constructor for a new instance of the Origin_Info_Place class </summary>
        /// <param name="Place_Text">Text of the publication place</param>
        /// <param name="Place_MarcCountry">MARC country code for the publication place</param>
        /// <param name="Place_ISO3166">ISO-3166 code for the publication place</param>
        public Origin_Info_Place(string Place_Text, string Place_MarcCountry, string Place_ISO3166)
        {
            placeText = Place_Text;
            placeMarcCountry = Place_MarcCountry;
            placeISO3166 = Place_ISO3166;
        }

        /// <summary> Text of the publication place </summary>
        public string Place_Text
        {
            get { return placeText; }
            set { placeText = value; }
        }

        /// <summary> MARC country code for the publication place </summary>
        public string Place_MarcCountry
        {
            get { return placeMarcCountry; }
            set { placeMarcCountry = value; }
        }

        /// <summary> ISO-3166 code for the publication place </summary>
        public string Place_ISO3166
        {
            get { return placeISO3166; }
            set { placeISO3166 = value; }
        }
    }

    #endregion

    #region Origin_Info_Frequency class definition

    /// <summary> Stored information about a frequency with which a resource was published or created </summary>
    [Serializable]
    public class Origin_Info_Frequency : XML_Writing_Base_Type
    {
        private string frequency_authority;
        private string frequency_term;

        /// <summary> Constructor for a new instance of the Origin_Info_Frequency class </summary>
        public Origin_Info_Frequency()
        {
            // Do nothing
        }

        /// <summary> Constructor for a new instance of the Origin_Info_Frequency class </summary>
        /// <param name="Term"> Frequency term such as 'monthly', 'daily', 'quarterly' </param>
        public Origin_Info_Frequency(string Term)
        {
            frequency_term = Term;
        }

        /// <summary> Constructor for a new instance of the Origin_Info_Frequency class </summary>
        /// <param name="Term"> Frequency term such as 'monthly', 'daily', 'quarterly' </param>
        /// <param name="Authority"> Controlled authority list from which this frequency term derives </param>
        public Origin_Info_Frequency(string Term, string Authority)
        {
            frequency_term = Term;
            frequency_authority = Authority;
        }

        /// <summary> Frequency term such as 'monthly', 'daily', 'quarterly' </summary>
        public string Term
        {
            get { return frequency_term ?? String.Empty; }
            set { frequency_term = value; }
        }

        /// <summary> Controlled authority list from which this frequency term derives </summary>
        public string Authority
        {
            get { return frequency_authority ?? String.Empty; }
            set { frequency_authority = value; }
        }
    }

    #endregion

    /// <summary>Information about the origin of the resource, including place of origin or publication, 
    /// publisher/originator, and dates associated with the resource</summary>
    [Serializable]
    public class MODS_Origin_Info : XML_Writing_Base_Type
    {
        private string dateCopyrighted;
        private string dateCreated;
        private string dateIssued;
        private string dateReprinted;
        private string edition;
        private List<Origin_Info_Frequency> frequencies;
        private List<Origin_Info_Issuance_Enum> issuances;
        private string marcDateIssued;
        private string marcDateIssuedEnd;
        private string marcDateIssuedStart;
        private List<Origin_Info_Place> places;
        private List<string> publishers;

        /// <summary> Constructor creates a new instance of the MODS_Origin_Info class </summary>
        public MODS_Origin_Info()
        {
            // Do nothing
        }

		/// <summary> Gets the first date discovered by going through all possible date fields </summary>
		/// <remarks> This checks, in this order, Date Issues, Date Created, MARC Date Issued, MARC Date Issued Start, Date Copyrighted</remarks>
	    public string Date_Check_All_Fields
	    {
		    get
		    {
				string pubdate = Date_Issued;
				if (pubdate.Length == 0)
					pubdate = Date_Created;
				if (pubdate.Length == 0)
					pubdate = MARC_DateIssued;
				if (pubdate.Length == 0)
					pubdate = MARC_DateIssued_Start;
				if (pubdate.Length == 0)
					pubdate = Date_Copyrighted;
			    return pubdate;
		    }
	    }

        /// <summary> Get the number of frequencies associated with this material </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Frequencies"/> property.  Even if 
        /// there are no frequencies, the Frequencies property creates a readonly collection to pass back out.</remarks>
        public int Frequencies_Count
        {
            get {
                return frequencies == null ? 0 : frequencies.Count;
            }
        }

        /// <summary> Gets the collection of frequencies </summary>
        /// <remarks> You should check the count of frequencies first using the <see cref="Frequencies_Count"/> property before using this property.
        /// Even if there are no frequencies, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Origin_Info_Frequency> Frequencies
        {
            get {
                return frequencies == null ? new ReadOnlyCollection<Origin_Info_Frequency>(new List<Origin_Info_Frequency>()) : new ReadOnlyCollection<Origin_Info_Frequency>(frequencies);
            }
        }

        /// <summary> Get the number of issuances associated with this material </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Issuances"/> property.  Even if 
        /// there are no issuances, the Issuances property creates a readonly collection to pass back out.</remarks>
        public int Issuances_Count
        {
            get {
                return issuances == null ? 0 : issuances.Count;
            }
        }

        /// <summary> Gets the collection of issuances </summary>
        /// <remarks> You should check the count of issuances first using the <see cref="Issuances_Count"/> property before using this property.
        /// Even if there are no issuances, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Origin_Info_Issuance_Enum> Issuances
        {
            get {
                return issuances == null ? new ReadOnlyCollection<Origin_Info_Issuance_Enum>(new List<Origin_Info_Issuance_Enum>()) : new ReadOnlyCollection<Origin_Info_Issuance_Enum>(issuances);
            }
        }

        /// <summary> Gets the number of publication places associated with this item </summary>
        /// <remarks>This should be used rather than the Count property of the <see cref="Places"/> property.  Even if 
        /// there are no publication places, the Places property creates a readonly collection to pass back out.</remarks>
        public int Places_Count
        {
            get {
                return places == null ? 0 : places.Count;
            }
        }

        /// <summary> Gets the collection of publication places </summary>
        /// <remarks> You should check the count of publication places first using the <see cref="Places_Count"/> property before using this property.
        /// Even if there are no publication places, this property creates a readonly collection to pass back out.</remarks>
        public ReadOnlyCollection<Origin_Info_Place> Places
        {
            get {
                return places == null ? new ReadOnlyCollection<Origin_Info_Place>(new List<Origin_Info_Place>()) : new ReadOnlyCollection<Origin_Info_Place>(places);
            }
        }

        /// <summary> Gets or sets the edition for this </summary>
        public string Edition
        {
            get { return edition ?? String.Empty; }
            set { edition = value; }
        }

        /// <summary> Get and sets the date this item was created </summary>
        public string Date_Created
        {
            get { return dateCreated ?? String.Empty; }
            set { dateCreated = value; }
        }

        /// <summary> Get and sets the date this item was issued/published </summary>
        public string Date_Issued
        {
            get { return dateIssued ?? String.Empty; }
            set { dateIssued = value; }
        }

        /// <summary> Get and sets the date this item was copyrighted </summary>
        public string Date_Copyrighted
        {
            get { return dateCopyrighted ?? String.Empty; }
            set { dateCopyrighted = value; }
        }

        /// <summary> Gets and sets the marc encoded date issued from the 260 |c </summary>
        public string MARC_DateIssued
        {
            get { return marcDateIssued ?? String.Empty; }
            set { marcDateIssued = value; }
        }

        /// <summary> Gets and sets the marc encoded start of date issued </summary>
        public string MARC_DateIssued_Start
        {
            get { return marcDateIssuedStart ?? String.Empty; }
            set { marcDateIssuedStart = value; }
        }

        /// <summary> Gets and sets the marc encoded end of date issued </summary>
        public string MARC_DateIssued_End
        {
            get
            {
                return marcDateIssuedEnd ?? String.Empty;
            }
            set { marcDateIssuedEnd = value; }
        }

        /// <summary> Gets and sets date the item was reprinted or reissued</summary>
        public string Date_Reprinted
        {
            get { return dateReprinted ?? String.Empty; }
            set { dateReprinted = value; }
        }

        #region Internal Methods and Properties

        /// <summary> Gets the number of publishers associated with this origination information </summary>
        internal int Publishers_Count
        {
            get {
                return publishers == null ? 0 : publishers.Count;
            }
        }

        /// <summary> Gets the collection of publisher names </summary>
        internal List<string> Publishers
        {
            get { return publishers; }
        }

        /// <summary> Flag indicates if there is data present in this origination information object </summary>
        internal bool hasData
        {
            get
            {
                // Add all the publishers names
                if ((publishers != null) && (publishers.Count > 0))
                    return true;

                // Step through all the publication places
                if (places != null)
                {
                    if (places.Any(Place => (Place.Place_ISO3166.Length > 0) || (Place.Place_MarcCountry.Length > 0) || (Place.Place_Text.Length > 0))) {
                        return true;
                    }
                }

                // Is the date issued exist?
                if ((!String.IsNullOrEmpty(dateIssued)) && (dateIssued != "-1"))
                    return true;

                // Does the create date exist?
                if ((!String.IsNullOrEmpty(dateCreated)) && (dateCreated != "-1"))
                    return true;

                // Does the marc date exist?
                if ((!String.IsNullOrEmpty(marcDateIssued)) && (marcDateIssued != "-1"))
                    return true;

                // Does the marc date start exist?
                if ((!String.IsNullOrEmpty(marcDateIssuedStart)) && (marcDateIssuedStart != "-1"))
                    return true;

                // Does the marc end date exist?
                if ((!String.IsNullOrEmpty(marcDateIssuedEnd)) && (marcDateIssuedEnd != "-1"))
                    return true;

                // Is the copyright date exist?
                if ((!String.IsNullOrEmpty(dateCopyrighted)) && (dateCopyrighted != "-1"))
                    return true;

                // Does the edition exist?
                if (!String.IsNullOrEmpty(edition))
                    return true;

                // Does reprint or reissue date exist?
                if (!String.IsNullOrEmpty(dateReprinted))
                    return true;

                // Check the frequency
                if ((frequencies != null) && (frequencies.Count > 0))
                    return true;

                // Check the issuance
                if ((issuances != null) && (issuances.Count > 0))
                    return true;

                return false;
            }
        }

        /// <summary> Add a new publication place </summary>
        /// <param name="Place_Text">Text of the publication place</param>
        public void Add_Place(string Place_Text)
        {
            if (Place_Text.Length > 0)
            {
                if (places == null)
                    places = new List<Origin_Info_Place>();

                places.Add(new Origin_Info_Place(Place_Text, String.Empty, String.Empty));
            }
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

        /// <summary> Inserts a new publication place at the beginning of the collection </summary>
        /// <param name="Place_Text">Text of the publication place</param>
        /// <param name="Place_MarcCountry">Marc country code for the publication place</param>
        /// <param name="Place_ISO3166">ISO-3166 code for the publication place</param>
        public void Insert_Place(string Place_Text, string Place_MarcCountry, string Place_ISO3166)
        {
            if (places == null)
                places = new List<Origin_Info_Place>();

            places.Insert(0, new Origin_Info_Place(Place_Text, Place_MarcCountry, Place_ISO3166));
        }

        /// <summary> Add a Publisher to this material </summary>
        /// <param name="Publisher_Name">Name of the publisher</param>
        internal void Add_Publisher(string Publisher_Name)
        {
            if (publishers == null)
                publishers = new List<string>();


            if (publishers.Any(Publisher => Publisher_Name == Publisher)) {
                return;
            }
            publishers.Add(Publisher_Name);
        }

        internal void Add_MODS(TextWriter Results)
        {
            // Return if there is no data
            if (!hasData)
                return;

            // Start this
            Results.Write("<mods:originInfo>\r\n");

            // Add all the publishers names
            if (publishers != null)
            {
                foreach (string thisPublisher in publishers)
                {
                    Results.Write("<mods:publisher>" + Convert_String_To_XML_Safe(thisPublisher) + "</mods:publisher>\r\n");
                }
            }

            // Step through all the publication places
            if (places != null)
            {
                foreach (Origin_Info_Place place in places)
                {
                    if ((place.Place_ISO3166.Length > 0) || (place.Place_MarcCountry.Length > 0) || (place.Place_Text.Length > 0))
                    {
                        Results.Write("<mods:place>\r\n");
                        if (place.Place_Text.Length > 0)
                            Results.Write("<mods:placeTerm type=\"text\">" + Convert_String_To_XML_Safe(place.Place_Text) + "</mods:placeTerm>\r\n");
                        if (place.Place_MarcCountry.Length > 0)
                            Results.Write("<mods:placeTerm type=\"code\" authority=\"marccountry\">" + place.Place_MarcCountry + "</mods:placeTerm>\r\n");
                        if (place.Place_ISO3166.Length > 0)
                            Results.Write("<mods:placeTerm type=\"code\" authority=\"iso3166\">" + place.Place_ISO3166 + "</mods:placeTerm>\r\n");
                        Results.Write("</mods:place>\r\n");
                    }
                }
            }

            // Is the date issued exist?
            if ((!String.IsNullOrEmpty(dateIssued)) && (dateIssued != "-1"))
                Results.Write("<mods:dateIssued>" + Convert_String_To_XML_Safe(dateIssued) + "</mods:dateIssued>\r\n");

            // Does the marc date exist?
            if (!String.IsNullOrEmpty(marcDateIssued))
                Results.Write("<mods:dateIssued encoding=\"marc\">" + Convert_String_To_XML_Safe(marcDateIssued) + "</mods:dateIssued>\r\n");

            // Does the marc start date exist?
            if (!String.IsNullOrEmpty(marcDateIssuedStart))
                Results.Write("<mods:dateIssued encoding=\"marc\" point=\"start\">" + marcDateIssuedStart + "</mods:dateIssued>\r\n");

            // Does the marc end date exist?
            if (!String.IsNullOrEmpty(marcDateIssuedEnd))
                Results.Write("<mods:dateIssued encoding=\"marc\" point=\"end\">" + marcDateIssuedEnd + "</mods:dateIssued>\r\n");

            // Is the date created exist?
            if ((!String.IsNullOrEmpty(dateCreated)) && (dateCreated != "-1"))
                Results.Write("<mods:dateCreated>" + Convert_String_To_XML_Safe(dateCreated) + "</mods:dateCreated>\r\n");

            // Is the copyright date exist?
            if ((!String.IsNullOrEmpty(dateCopyrighted)) && (dateCopyrighted != "-1"))
                Results.Write("<mods:copyrightDate>" + Convert_String_To_XML_Safe(dateCopyrighted) + "</mods:copyrightDate>\r\n");

            // Does the edition exist?
            if (!String.IsNullOrEmpty(edition))
                Results.Write("<mods:edition>" + Convert_String_To_XML_Safe(edition) + "</mods:edition>\r\n");

            // Does reprint or reissue date exist?
            if (!String.IsNullOrEmpty(dateReprinted))
                Results.Write("<mods:dateOther type=\"reprint\">" + Convert_String_To_XML_Safe(dateReprinted) + "</mods:dateOther>\r\n");

            // Add the frequency
            if (frequencies != null)
            {
                foreach (Origin_Info_Frequency frequency in frequencies)
                {
                    Results.Write("<mods:frequency");
                    if (frequency.Authority.Length > 0)
                    {
                        Results.Write(" authority=\"" + frequency.Authority + "\"");
                    }
                    Results.Write(">" + Convert_String_To_XML_Safe(frequency.Term) + "</mods:frequency>\r\n");
                }
            }

            // Add the issuances
            if (issuances != null)
            {
                foreach (Origin_Info_Issuance_Enum issuance in issuances)
                {
                    if (issuance != Origin_Info_Issuance_Enum.UNKNOWN)
                    {
                        Results.Write("<mods:issuance>");
                        switch (issuance)
                        {
                            case Origin_Info_Issuance_Enum.Continuing:
                                Results.Write("continuing");
                                break;

                            case Origin_Info_Issuance_Enum.Integrating_Resource:
                                Results.Write("integrating resource");
                                break;

                            case Origin_Info_Issuance_Enum.Monographic:
                                Results.Write("monographic");
                                break;

                            case Origin_Info_Issuance_Enum.Multipart_Monograph:
                                Results.Write("multipart monograph");
                                break;

                            case Origin_Info_Issuance_Enum.Serial:
                                Results.Write("serial");
                                break;

                            case Origin_Info_Issuance_Enum.Single_Unit:
                                Results.Write("single unit");
                                break;
                        }
                        Results.Write("</mods:issuance>\r\n");
                    }
                }
            }

            // End this
            Results.Write("</mods:originInfo>\r\n");
        }

        #endregion

        /// <summary> Clear all the data associated with this origin info object, except for the frequency information </summary>
        public void Clear_All_But_Frequencies()
        {
            if (publishers != null) publishers.Clear();
            if (places != null) places.Clear();

            dateIssued = null;
            marcDateIssued = null;
            marcDateIssuedEnd = null;
            marcDateIssuedStart = null;
            dateCopyrighted = null;
            dateReprinted = null;
            edition = null;
            dateCreated = null;
        }

        /// <summary> Clear the list of publishers and the list of places associated with this digital resource  </summary>
        public void Clear_Places_And_Publishers()
        {
            if (publishers != null) publishers.Clear();
        }

        /// <summary> Clear all the data associated with this origin info object </summary>
        public void Clear()
        {
            if (publishers != null) publishers.Clear();
            if (places != null) places.Clear();
            if (frequencies != null) frequencies.Clear();
            if (issuances != null) issuances.Clear();
            dateIssued = null;
            marcDateIssued = null;
            marcDateIssuedEnd = null;
            marcDateIssuedStart = null;
            dateCopyrighted = null;
            dateReprinted = null;
            edition = null;
            dateCreated = null;
        }

        /// <summary> Add a new frequency to this origination information </summary>
        /// <param name="Term"> Frequency term to add such as 'monthly', 'daily', 'quarterly' </param>
        public void Add_Frequency(string Term)
        {
            if (frequencies == null)
                frequencies = new List<Origin_Info_Frequency>();

            frequencies.Add(new Origin_Info_Frequency(Term));
        }

        /// <summary> Add a new frequency to this origination information </summary>
        /// <param name="Term"> Frequency term to add such as 'monthly', 'daily', 'quarterly' </param>
        /// <param name="Authority"> Controlled authority list from which this frequency term derives </param>
        public void Add_Frequency(string Term, string Authority)
        {
            if (frequencies == null)
                frequencies = new List<Origin_Info_Frequency>();

            frequencies.Add(new Origin_Info_Frequency(Term, Authority));
        }

        /// <summary> Add a new frequency to this origination information </summary>
        /// <param name="Frequency"> Frequency object to add such as 'monthly', 'daily', 'quarterly' </param>
        public void Add_Frequency(Origin_Info_Frequency Frequency)
        {
            if (frequencies == null)
                frequencies = new List<Origin_Info_Frequency>();

            frequencies.Add(Frequency);
        }

        /// <summary> Clear all the frequencies associated with this material </summary>
        public void Clear_Frequencies()
        {
            if (frequencies != null)
                frequencies.Clear();
        }

        /// <summary> Removes a frequency from the collection of frequencies </summary>
        /// <param name="Remove"> Frequency to remove </param>
        public void Remove_Frequency(Origin_Info_Frequency Remove)
        {
            if ((frequencies != null) && (frequencies.Contains(Remove)))
                frequencies.Remove(Remove);
        }

        /// <summary> Add a new issuance to this origination information </summary>
        /// <param name="Term"> Issuance term to add such as 'monthly', 'daily', 'quarterly' </param>
        public void Add_Issuance(string Term)
        {
            Origin_Info_Issuance_Enum newIssuanceEnum = Origin_Info_Issuance_Enum.UNKNOWN;
            switch (Term.ToLower())
            {
                case "continuing":
                    newIssuanceEnum = Origin_Info_Issuance_Enum.Continuing;
                    break;
                case "monographic":
                    newIssuanceEnum = Origin_Info_Issuance_Enum.Monographic;
                    break;
                case "single unit":
                    newIssuanceEnum = Origin_Info_Issuance_Enum.Single_Unit;
                    break;
                case "multipart monograph":
                    newIssuanceEnum = Origin_Info_Issuance_Enum.Multipart_Monograph;
                    break;
                case "serial":
                    newIssuanceEnum = Origin_Info_Issuance_Enum.Serial;
                    break;
                case "integrating resource":
                    newIssuanceEnum = Origin_Info_Issuance_Enum.Integrating_Resource;
                    break;
            }

            if (newIssuanceEnum != Origin_Info_Issuance_Enum.UNKNOWN)
            {
                if (issuances == null)
                    issuances = new List<Origin_Info_Issuance_Enum>();

                issuances.Add(newIssuanceEnum);
            }
        }

        /// <summary> Add a new issuance to this origination information </summary>
        /// <param name="Issuance"> Issuance object to add such as 'monthly', 'daily', 'quarterly' </param>
        public void Add_Issuance(Origin_Info_Issuance_Enum Issuance)
        {
            if (issuances == null)
                issuances = new List<Origin_Info_Issuance_Enum>();

            issuances.Add(Issuance);
        }

        /// <summary> Clear all the issuances associated with this material </summary>
        public void Clear_Issuances()
        {
            if (issuances != null)
                issuances.Clear();
        }

        /// <summary> Removes a place from the collection of publication places </summary>
        /// <param name="Remove"> Publication Place to remove </param>
        public void Remove_Place(Origin_Info_Place Remove)
        {
            if ((places != null) && (places.Contains(Remove)))
                places.Remove(Remove);
        }

        /// <summary> Clears all the publication places associated with this item </summary>
        public void Clear_Places()
        {
            if (places != null)
                places.Clear();
        }
    }
}