using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Xml;

namespace SobekCM.Bib_Package.OAI
{
    /// <summary> Static class is used to query an OAI-PMH repository for information about the repository or
    /// records within that repository </summary>
    public class OAI_Repository_Stream_Reader
    {
        /// <summary> Gets the continuing list of records from an OAI-PMH repository, from a resumption token </summary>
        /// <param name="OAI_URL">URL for the OAI repository</param>
        /// <param name="Resumption_Token"> Recent resumption token from that repository </param>
        /// <returns> List of all the records from the repository </returns>
        public static OAI_Repository_Records_List List_Records(string OAI_URL, string Resumption_Token )
        {
            try
            {
                // Create the return value
                OAI_Repository_Records_List returnValue = new OAI_Repository_Records_List();

                // Get the URL
                string URL = OAI_URL + "?verb=ListRecords&resumptionToken=" + Resumption_Token;

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                // Read the stream and output the list of records
                read_list_of_records(resStream, returnValue);

                return returnValue;
            }
            catch (Exception ee)
            {
                Debug.WriteLine(ee.Message);
                return null;
            }
        }

        /// <summary> Gets the list of records from an OAI-PMH repository </summary>
        /// <param name="OAI_URL"> URL for the OAI-PMH repository </param>
        /// <param name="Set"> Name of the OAI-PMH set to pull records from </param>
        /// <param name="MetadataPrefix"> Metadata type to request </param>
        /// <returns> List of the records, or NULL if an error occrured </returns>
        public static OAI_Repository_Records_List List_Records(string OAI_URL, string Set, string MetadataPrefix )
        {
            try
            {
                // Create the return value
                OAI_Repository_Records_List returnValue = new OAI_Repository_Records_List();

                // Get the URL
                string URL = OAI_URL + "?verb=ListRecords&set=" + Set + "&metadataPrefix=" + MetadataPrefix;
                if ( Set.Length == 0 )
                    URL = OAI_URL + "?verb=ListRecords&metadataPrefix=" + MetadataPrefix;

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(URL);

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                // Read the stream and output the list of records
                read_list_of_records(resStream, returnValue);

                return returnValue;
            }
            catch ( Exception ee )
            {
                return null;
            }
        }

        private static void read_list_of_records(Stream resStream, OAI_Repository_Records_List returnValue)
        {
            // Try to read the XML
            XmlTextReader r = new XmlTextReader(resStream);

            bool inRecord = false;
            OAI_Repository_DublinCore_Record thisRecord = new OAI_Repository_DublinCore_Record();
            while (r.Read())
            {
                if (inRecord)
                {
                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name)
                        {

                            case "identifier":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.OAI_Identifier = r.Value.Trim();
                                }
                                break;

                            case "datestamp":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Datestamp = r.Value.Trim();
                                }
                                break;

                            case "dc:title":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Title(r.Value.Trim());
                                }
                                break;

                            case "dc:creator":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Creator(r.Value.Trim());
                                }
                                break;

                            case "dc:subject":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Subject(r.Value.Trim());
                                }
                                break;

                            case "dc:description":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Description(r.Value.Trim());
                                }
                                break;

                            case "dc:publisher":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Publisher(r.Value.Trim());
                                }
                                break;

                            case "dc:contributor":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Contributor(r.Value.Trim());
                                }
                                break;

                            case "dc:date":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Date(r.Value.Trim());
                                }
                                break;

                            case "dc:type":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Type(r.Value.Trim());
                                }
                                break;

                            case "dc:format":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Format(r.Value.Trim());
                                }
                                break;

                            case "dc:source":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Source(r.Value.Trim());
                                }
                                break;

                            case "dc:identifier":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Identifier(r.Value.Trim());
                                }
                                break;

                            case "dc:language":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Language(r.Value.Trim());
                                }
                                break;

                            case "dc:relation":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Relation(r.Value.Trim());
                                }
                                break;

                            case "dc:coverage":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Coverage(r.Value.Trim());
                                }
                                break;

                            case "dc:rights":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    thisRecord.Add_Rights(r.Value.Trim());
                                }
                                break;
                        }
                    }
                    else if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "record"))
                    {
                        inRecord = false;
                        returnValue.Add_Record(thisRecord);
                    }
                }
                else
                {
                    if ((r.NodeType == XmlNodeType.Element) && (r.Name == "record"))
                    {
                        inRecord = true;
                        thisRecord = new OAI_Repository_DublinCore_Record();
                    }

                    if ((r.NodeType == XmlNodeType.Element) && (r.Name == "resumptionToken"))
                    {
                        r.Read();
                        if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                        {
                            returnValue.Resumption_Token = r.Value;
                        }
                        break;
                    }
                }
            }
        }

        public static OAI_Repository_Information Identify(string OAI_URL)
        {
            // Create the return object 
            OAI_Repository_Information returnValue = new OAI_Repository_Information(OAI_URL);

            try
            {

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(OAI_URL + "?verb=Identify");

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                // Try to read the XML
                XmlTextReader r = new XmlTextReader(resStream);

                bool inOaiResponse = false;
                while (r.Read())
                {
                    if (inOaiResponse)
                    {
                        if (r.NodeType == XmlNodeType.Element)
                        {
                            switch (r.Name)
                            {
                                case "repositoryName":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Name = r.Value;
                                    }
                                    break;

                                case "adminEmail":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Admin_Email = r.Value;
                                    }
                                    break;

                                case "baseURL":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Base_URL = r.Value;
                                    }
                                    break;

                                case "protocolVersion":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Protocol_Version = r.Value;
                                    }
                                    break;

                                case "earliestDatestamp":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Earliest_Date_Stamp = r.Value;
                                    }
                                    break;

                                case "deletedRecord":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Deleted_Record = r.Value;
                                    }
                                    break;

                                case "granularity":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Granularity = r.Value;
                                    }
                                    break;

                                case "repositoryIdentifier":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Repository_Identifier = r.Value;
                                    }
                                    break;

                                case "delimiter":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Delimiter = r.Value;
                                    }
                                    break;

                                case "sampleIdentifier":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        returnValue.Sample_Identifier = r.Value;
                                    }
                                    break;
                            }
                        }
                    }
                    else
                    {
                        if ((r.NodeType == XmlNodeType.Element) && (r.Name == "OAI-PMH"))
                            inOaiResponse = true;
                    }
                }
            }
            catch
            {
                returnValue.Is_Valid = false;
            }

            return returnValue;
        }

        public static bool List_Sets( OAI_Repository_Information Repository )
        {
            try
            {

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Repository.Harvested_URL + "?verb=ListSets");

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                // Try to read the XML
                XmlTextReader r = new XmlTextReader(resStream);

                // Sort the list for display purposes
                SortedList<string, KeyValuePair<string, string>> sorter = new SortedList<string, KeyValuePair<string, string>>();

                bool inSet = false;
                string setName = String.Empty;
                string setSpec = String.Empty;
                while (r.Read())
                {
                    if (inSet)
                    {
                        if (r.NodeType == XmlNodeType.Element)
                        {
                            switch (r.Name)
                            {
                                case "setSpec":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        setSpec = r.Value;
                                    }
                                    break;

                                case "setName":
                                    r.Read();
                                    if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                    {
                                        setName = r.Value;
                                    }
                                    break;
                            }
                        }
                        else if ((r.NodeType == XmlNodeType.EndElement) && (r.Name == "set"))
                        {
                            if ((setSpec.Length > 0) && (setName.Length > 0))
                            {
                                KeyValuePair<string, string> newSet = new KeyValuePair<string, string>(setSpec, setName);
                                sorter.Add(setSpec, newSet);
                            }
                            setSpec = String.Empty;
                            setName = String.Empty;
                            inSet = false;
                        }
                    }
                    else
                    {
                        if ((r.NodeType == XmlNodeType.Element) && (r.Name == "set"))
                        {
                            inSet = true;
                            setSpec = String.Empty;
                            setName = String.Empty;
                        }
                    }
                }

                // Now, add each set to the repository
                foreach( string key in sorter.Keys )
                {
                    Repository.Add_Set(sorter[key]);
                }
            }
            catch
            {
                return false;
            }

            return true;
        }

        public static bool List_Metadata_Formats(OAI_Repository_Information Repository)
        {
            try
            {

                // prepare the web page we will be asking for
                HttpWebRequest request = (HttpWebRequest)WebRequest.Create(Repository.Harvested_URL + "?verb=ListMetadataFormats");

                // execute the request
                HttpWebResponse response = (HttpWebResponse)request.GetResponse();

                // we will read data via the response stream
                Stream resStream = response.GetResponseStream();

                // Try to read the XML
                XmlTextReader r = new XmlTextReader(resStream);

                while (r.Read())
                {
                    if (r.NodeType == XmlNodeType.Element)
                    {
                        switch (r.Name)
                        {
                            case "metadataPrefix":
                                r.Read();
                                if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                                {
                                    Repository.Add_Metadata_Format(r.Value);
                                }
                                break;
                        }
                    }
                }
            }
            catch
            {
                return false;
            }

            return true;
        }
    }
}
