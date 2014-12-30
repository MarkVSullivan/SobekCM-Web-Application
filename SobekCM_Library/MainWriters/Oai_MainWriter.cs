#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.IO;
using System.Net;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SobekCM.Core.Navigation;
using SobekCM.Core.OAI;
using SobekCM.Library.Database;
using SobekCM.Tools;
using SobekCM.UI_Library;


#endregion

namespace SobekCM.Library.MainWriters
{
    /// <summary> Main writer writes browses in OAI-PMH format </summary>
    /// <remarks> This class extends the abstract class <see cref="abstractMainWriter"/>. </remarks>
    public class Oai_MainWriter: abstractMainWriter
    {
        private readonly DataTable oaiSets;
        private readonly NameValueCollection queryString;
        private readonly string url = UI_ApplicationCache_Gateway.Settings.System_Base_URL;
        private readonly List<string> validArgs;

        private const int RECORDS_PER_PAGE = 100;
        private const int IDENTIFIERS_PER_PAGE = 250;

        private const int WAY_FUTURE_YEAR = 2025;
        private const int WAY_PAST_YEAR = 1900;

        /// <summary> Constructor for a new instance of the Oai_MainWriter class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        /// <param name="Query_String"> URL Query string to parse for OAI-PMH verbs and other values </param>
        public Oai_MainWriter(NameValueCollection Query_String, RequestCache RequestSpecificValues) : base(RequestSpecificValues)
   
        {
            // Build list of valid arguments
            validArgs = new List<string>
                            {
                                "from",
                                "until",
                                "metadataPrefix",
                                "set",
                                "resumptionToken",
                                "identifier",
                                "verb",
                                "portal",
                                "urlrelative"
                            };

            // Load the list of OAI sets
            oaiSets = SobekCM_Database.Get_OAI_Sets();
            queryString = Query_String;

            // Set the response type
            HttpContext.Current.Response.ContentType = "text/xml";
        }

        /// <summary> Gets the enumeration of the type of main writer </summary>
        /// <value> This property always returns the enumerational value <see cref="SobekCM.UI_Library.Navigation.Writer_Type_Enum.OAI"/>. </value>
        public override Writer_Type_Enum Writer_Type { get { return Writer_Type_Enum.OAI; } }

        /// <summary> Perform all the work of adding text directly to the response stream back to the web user </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="Tracer"> Trace object keeps a list of each method executed and important milestones in rendering </param>
        public override void Write_Html(TextWriter Output, Custom_Tracer Tracer)
        {
            // Parse the request
            string verb = queryString["verb"];
            if (string.IsNullOrEmpty(verb))
            {
                Write_Error(Output, "verb=\"\"", "badVerb", "The verb argument is missing.");
                return;
            }

            // Make sure the verb is valid first
            if ((verb != "GetRecord") && (verb != "Identify") && (verb != "ListIdentifiers") &&
                (verb != "ListMetadataFormats") && (verb != "ListMetadataFormats") &&
                (verb != "ListRecords") && (verb != "ListSets"))
            {
                Write_Error(Output, "verb=\"\"", "badVerb", verb + " is not a legal OAI-PMH 2.0 verb.");
                return;
            }

            // Check each argument for basic validity
            foreach (string thisArg in queryString.AllKeys)
            {
                if (!validArgs.Contains(thisArg))
                {
                    Write_Error(Output, "verb=\"\"", "badArgument", thisArg + " is not a legal OAI-PMH 2.0 argument.");
                    return;
                }
            }

            // Switch result by verb
            switch (verb)
            {
                case "GetRecord":
                    string identifier = "";
                    string metadata = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "identifier") && (thisKey != "metadataPrefix") && (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is identifier, save it
                        if (thisKey == "identifier")
                            identifier = queryString["identifier"];

                        // If this is identifier, save it
                        if (thisKey == "metadataPrefix")
                            metadata = queryString["metadataPrefix"];
                    }

                    // Make sure both the identifier and metadata have data
                    if (identifier.Length == 0)
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "identifier required argument is missing.");
                        return;
                    }

                    // Make sure both the identifier and metadata have data
                    if (metadata.Length == 0)
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "metadataPrefix required argument is missing.");
                        return;
                    }

                    // Display the record and metadata
                    GetRecord(Output, identifier, metadata);
                    break;

                case "Identify":
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "portal") && ( thisKey != "urlrelative" ))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }
                    }
                    Identify(Output);
                    break;

                case "ListIdentifiers":
                    string fromLI = "";
                    string untilLI = "";
                    string setNameLI = "";
                    string metadataPrefixLI = "";
                    string tokenLI = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "from") && (thisKey != "until") &&
                            (thisKey != "metadataPrefix") && (thisKey != "set") && (thisKey != "resumptionToken") && 
                            (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is FROM, save it
                        if (thisKey == "from")
                            fromLI = queryString["from"];

                        // If this is UNTIL, save it
                        if (thisKey == "until")
                            untilLI = queryString["until"];

                        // If this is SET, save it
                        if (thisKey == "set")
                            setNameLI = queryString["set"];

                        // If this is METADATAPREFIX, save it
                        if (thisKey == "metadataPrefix")
                            metadataPrefixLI = queryString["metadataPrefix"];

                        // Check the resumption token
                        if (thisKey == "resumptionToken")
                        {
                            tokenLI = queryString["resumptionToken"];
                            if (tokenLI.Length < 26)
                            {
                                Write_Error(Output, "resumptionToken=\"" + tokenLI + "\" verb=\"ListIdentifiers\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + tokenLI);
                                return;
                            }
                        }
                    }

                    ListIdentifiers(Output, setNameLI, fromLI, untilLI, metadataPrefixLI, tokenLI);
                    break;

                case "ListMetadataFormats":
                    string identifierLMF = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "identifier") && (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is identifier, save it
                        if (thisKey == "identifier")
                            identifierLMF = queryString["identifier"];
                    }

                    ListMetadataFormats(Output, identifierLMF);
                    break;

                case "ListRecords":
                    string fromLR = "";
                    string untilLR = "";
                    string setNameLR = "";
                    string metadataPrefixLR = "";
                    string tokenLR = "";
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "from") && (thisKey != "until") &&
                            (thisKey != "metadataPrefix") && (thisKey != "set") && (thisKey != "resumptionToken") && 
                            (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // If this is FROM, save it
                        if (thisKey == "from")
                            fromLR = queryString["from"];

                        // If this is UNTIL, save it
                        if (thisKey == "until")
                            untilLR = queryString["until"];

                        // If this is SET, save it
                        if (thisKey == "set")
                            setNameLR = queryString["set"];

                        // If this is METADATAPREFIX, save it
                        if (thisKey == "metadataPrefix")
                            metadataPrefixLR = queryString["metadataPrefix"];

                        // Check the resumption token
                        if (thisKey == "resumptionToken")
                        {
                            tokenLR = queryString["resumptionToken"];
                            if (tokenLR.Length < 8)
                            {
                                Write_Error(Output, "resumptionToken=\"" + tokenLR + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + tokenLR);
                                return;
                            }
                        }
                    }

                    ListRecords(Output, setNameLR, fromLR, untilLR, metadataPrefixLR, tokenLR);
                    break;

                case "ListSets":
                    foreach (string thisKey in queryString.Keys)
                    {
                        // If this is invalid, throw the error
                        if ((thisKey != "verb") && (thisKey != "resumptionToken") && (thisKey != "portal") && (thisKey != "urlrelative"))
                        {
                            Write_Error(Output, "verb=\"\"", "badArgument", thisKey + " is not legal in this context.");
                            return;
                        }

                        // We will not be using resumptionToken
                        if (thisKey == "resumptionToken")
                        {
                            string tokenLS = queryString["resumptionToken"];
                            Write_Error(Output, "resumptionToken=\"" + tokenLS + "\" verb=\"ListSets\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + tokenLS);
                            return;
                        }
                    }
                    ListSets(Output);
                    break;

                default:
                    Write_Error(Output, "verb=\"" + verb + "\"", "badVerb", verb + " is not a legal OAI-PMH 2.0 verb.");
                    break;
            }
        }

        /// <summary> Lists all of the sets available for OAI-PMH harvesting </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        protected internal void ListSets(TextWriter Output)
        {
            // Start the response
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request verb=\"ListSets\">" + url + "</request>");
            Output.WriteLine("<ListSets>");

            // Display each set information
            foreach (DataRow thisRow in oaiSets.Rows)
            {
                Output.WriteLine("<set>");
                Output.WriteLine("<setSpec>" + thisRow["Code"].ToString().ToLower() + "</setSpec>");
                Output.WriteLine("<setName>" + thisRow["Name"].ToString().Replace("&","&amp;").Replace("\"", "&quot;") + "</setName>");
                Output.WriteLine("<setDescription>");
                Output.WriteLine("<oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">");
                Output.WriteLine("\t<dc:title>" + thisRow["Name"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;") + "</dc:title> ");
                Output.WriteLine("\t<dc:identifier>" + UI_ApplicationCache_Gateway.Settings.OAI_Resource_Identifier_Base + ":" + thisRow["Code"].ToString().ToLower() + "</dc:identifier>");
                Output.WriteLine("\t<dc:identifier>" + UI_ApplicationCache_Gateway.Settings.System_Base_URL + "?a=" + thisRow["Code"].ToString().ToLower() + "</dc:identifier>");
                if ( thisRow["Description"].ToString().Length > 0 )
                    Output.WriteLine("\t<dc:description>" + thisRow["Description"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;") + "</dc:description>");
                if (thisRow["OAI_Metadata"].ToString().Length > 0)
                {
                    Output.WriteLine("\t" + thisRow["OAI_Metadata"].ToString().Replace("&", "&amp;").Replace("\"", "&quot;"));
                }
                Output.WriteLine("</oai_dc:dc>");
                Output.WriteLine("</setDescription>");
                Output.WriteLine("</set>");
            }

            // Close the response
            Output.WriteLine("</ListSets>");
            Output.WriteLine("</OAI-PMH>");
        }
        /// <summary> Lists all the records associated with a certain OAI-PMH set </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="SetCode"> Code for the OAI-PMH set </param>
        /// <param name="From"> Date from which to return all the identifiers </param>
        /// <param name="Until"> Date to which to return all the identifiers </param>
        /// <param name="ResumptionToken"> Resumption token from the original query string, if one was provided </param>
        /// <param name="MetadataPrefix"> Prefix indicates the format for the metadata </param>
        protected internal void ListRecords(TextWriter Output, string SetCode, string From, 
            string Until, string MetadataPrefix, string ResumptionToken )
        {
            if (( ResumptionToken.Length > 0 ) && ( MetadataPrefix.Length == 0 ))
                MetadataPrefix = "oai_dc";

            // Make sure both the identifier and metadata have data
            if (MetadataPrefix.Length == 0)
            {
                Write_Error(Output, "verb=\"\"", "badArgument", "metadataPrefix required argument is missing.");
                return;
            }

            // Set the default dates and page first
            DateTime from_date = new DateTime(WAY_PAST_YEAR, 1, 1);
            DateTime until_date = new DateTime(WAY_FUTURE_YEAR, 1, 1);
            int current_page = 1;

            // Start to build the request XML
            StringBuilder request = new StringBuilder();
            
            // If there is a resumption token, that should be used to pull all information
            if (ResumptionToken.Length > 0)
            {
                // Add to the request
                request.Append("resumptionToken=\"" + ResumptionToken + "\" ");

                // Compute the state from the token
                try
                {
                    string page_string = ResumptionToken.Substring(0, 6);
                    SetCode = ResumptionToken.Substring(6 + UI_ApplicationCache_Gateway.Settings.OAI_Repository_Identifier.Length);

                    // Try to parse the page and dates now
                    if (!Int32.TryParse(page_string, out current_page))
                    {
                        Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                        return;
                    }

                    // Was there a from and until date included in the token?
                    if (SetCode.IndexOf(".") > 0)
                    {
                        int period_index = SetCode.IndexOf(".");
                        string from_string = SetCode.Substring(period_index + 1, 8);
                        string until_string = SetCode.Substring(period_index + 9, 8);
                        SetCode = SetCode.Substring(0, period_index);

                        // Before we can parse those dates, we need to format a bit for the .NET framework
                        from_string = from_string.Substring(4, 2) + "/" + from_string.Substring(6) + "/" + from_string.Substring(0, 4);
                        until_string = until_string.Substring(4, 2) + "/" + until_string.Substring(6) + "/" + until_string.Substring(0, 4);

                        // Try to parse the dates now
                        if ((!DateTime.TryParse(from_string, out from_date)) || (!DateTime.TryParse(until_string, out until_date)))
                        {
                            Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                            return;
                        }
                    }
                }
                catch
                {
                    Write_Error(Output, "resumptionToken=\"" + ResumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + ResumptionToken);
                    return;
                }
            }
            else
            {
                // Add to the request 
                if (From.Length > 0)
                    request.Append("from=\"" + From + "\" ");
                request.Append("metadataPrefix=\"" + MetadataPrefix + "\" ");
                if (SetCode.Length > 0)
                    request.Append("set=\"" + SetCode + "\" ");
                if (Until.Length > 0)
                    request.Append("until=\"" + Until + "\" ");

                // Check the format of FROM and UNTIL, if they are provided
                const string regexMatchString = "([0-9]{4})(-([0-9]{2})(-([0-9]{2})))";
                if (From.Length > 0)
                {
                    if ((From.Length != 10) || (!Regex.Match(From, regexMatchString).Success) || (!DateTime.TryParse(From, out from_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + From);
                        return;
                    }
                }
                if (Until.Length > 0)
                {
                    if ((Until.Length != 10) || (!Regex.Match(Until, regexMatchString).Success) || (!DateTime.TryParse(Until, out until_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + Until);
                        return;
                    }
                }
            }

            // Finish the request echo portion
            request.Append("verb=\"ListRecords\"");

            // Metadata prefix must currently be oai_dc
            if (MetadataPrefix != "oai_dc")
            {
                Write_Error(Output, request.ToString(), "cannotDisseminateFormat", MetadataPrefix + " is not supported by this repository.");
                return;
            }

            // Get the records
            List<OAI_Record> results = SobekCM_Database.Get_OAI_Data(SetCode, MetadataPrefix, from_date, until_date, RECORDS_PER_PAGE, current_page, true);
            if (results.Count == 0)
            {
                Write_Error(Output, request.ToString(), "noRecordsMatch", "There are no records matching the criteria indicated by this request's arguments.");
            }
            else
            {
                // Determine if a resumption token should be submitted after this.  This is easily
                // done since the database will attempt to return one additional record, if more records
                // exist
                bool more_records = false;
                if (results.Count > RECORDS_PER_PAGE)
                {
                    more_records = true;
                    results.RemoveAt(results.Count - 1);
                }

                // Write the list of records/identifiers
                Write_OAI_ListRecords(SetCode, from_date, until_date, ResumptionToken, request.ToString(), false, current_page, more_records, results, Output);
            }
        }

        /// <summary> Lists all the records associated with a single OAI-PMH set </summary>
        /// <param name="set_code"> Code for the OAI-PMH set </param>
        /// <param name="from"> Date from which to return all the identifiers </param>
        /// <param name="until"> Date to which to return all the identifiers </param>
        /// <param name="resumptionToken"> Resumption token from the original query string, if one was provided </param>
        /// <param name="request"> String that represents the original request </param>
        /// <param name="headers_only"> Flag indicates to just return the headers of each digital resource </param>
        /// <param name="Current_Page">Current page number within the set of results</param>
        /// <param name="More_Records"> Flag indicates if a resumption token should be issued</param>
        /// <param name="records"> List of all the records to return to the user </param>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <returns> TRUE if successfuly, otherwise FALSE</returns>
        protected internal void Write_OAI_ListRecords(string set_code, DateTime from, DateTime until,
            string resumptionToken, string request, bool headers_only, int Current_Page, bool More_Records, List<OAI_Record> records, TextWriter Output)
        {
            // If the first read was successful then write the header
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request " + request + ">" + url + "</request>");
            Output.WriteLine(headers_only ? "<ListIdentifiers>" : "<ListRecords>");

            // Step through each record
            foreach (OAI_Record thisRecord in records)
            {
                if (!headers_only)
                    Output.Write("<record>");
                Output.Write("<header><identifier>" + UI_ApplicationCache_Gateway.Settings.OAI_Resource_Identifier_Base + thisRecord.BibID + "</identifier><datestamp>" + thisRecord.Last_Modified_Date.Year + "-" + thisRecord.Last_Modified_Date.Month.ToString().PadLeft(2, '0') + "-" + thisRecord.Last_Modified_Date.Day.ToString().PadLeft(2, '0') + "</datestamp>");
                if (set_code.Length > 0)
                    Output.Write("<setSpec>" + UI_ApplicationCache_Gateway.Settings.OAI_Resource_Identifier_Base + set_code + "</setSpec>");
                Output.Write("</header>");
                if (!headers_only)
                {
                    Output.WriteLine("<metadata><oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">" + thisRecord.Record + "</oai_dc:dc></metadata></record>");
                }
            }

            // Should we provide a resumption token for possibly more records?
            if ( More_Records )
            {
                DateTime expDate = DateTime.Now.AddDays(1).ToUniversalTime();
                string expirationDateString = expDate.Year + "-" + expDate.Month.ToString().PadLeft(2, '0') + "-" + expDate.Day.ToString().PadLeft(2, '0') + "T" + expDate.Hour.ToString().PadLeft(2, '0') + ":" + expDate.Minute.ToString().PadLeft(2, '0') + ":00Z";
                string newResumptionToken = (Current_Page + 1).ToString().PadLeft(6, '0') + UI_ApplicationCache_Gateway.Settings.OAI_Repository_Identifier + set_code;
                if ((from.Year != WAY_PAST_YEAR) || (until.Year != WAY_FUTURE_YEAR))
                {
                    newResumptionToken = newResumptionToken + "." + from.Year + from.Month.ToString().PadLeft(2, '0') + from.Day.ToString().PadLeft(2, '0') + until.Year + until.Month.ToString().PadLeft(2, '0') + until.Day.ToString().PadLeft(2, '0');
                }
                Output.WriteLine("<resumptionToken expirationDate=\"" + expirationDateString + "\">" + newResumptionToken + "</resumptionToken>");
            }

            // Write the response
            Output.WriteLine(headers_only ? "</ListIdentifiers>" : "</ListRecords>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Lists all of the identifiers within a particular OAI-PMH set </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="set_code"> Code for the OAI-PMH set </param>
        /// <param name="from"> Date from which to return all the identifiers </param>
        /// <param name="until"> Date to which to return all the identifiers </param>
        /// <param name="metadataPrefix"> Prefix of the metadata fomat to return the identifier information</param>
        /// <param name="resumptionToken"> Resumption token from the original query string, if one was provided </param>
        protected internal void ListIdentifiers(TextWriter Output, string set_code, string from, string until, string metadataPrefix, string resumptionToken )
        {
            if ((resumptionToken.Length > 0) && (metadataPrefix.Length == 0))
                metadataPrefix = "oai_dc";

            // Make sure both the identifier and metadata have data
            if (metadataPrefix.Length == 0)
            {
                Write_Error(Output, "verb=\"\"", "badArgument", "metadataPrefix required argument is missing.");
                return;
            }

            // Set the default dates and page first
            DateTime from_date = new DateTime(1900, 1, 1);
            DateTime until_date = DateTime.Now.AddDays(1);
            int current_page = 1;

            // Start to build the request XML
            StringBuilder request = new StringBuilder();

            // If there is a resumption token, that should be used to pull all information
            if (resumptionToken.Length > 0)
            {
                // Add to the request
                request.Append("resumptionToken=\"" + resumptionToken + "\" ");

                // Compute the state from the token
                try
                {
                    string page_string = resumptionToken.Substring(0, 6);
                    set_code = resumptionToken.Substring(6 + UI_ApplicationCache_Gateway.Settings.OAI_Repository_Identifier.Length);

                    // Try to parse the page and dates now
                    if (!Int32.TryParse(page_string, out current_page))
                    {
                        Write_Error(Output, "resumptionToken=\"" + resumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + resumptionToken);
                        return;
                    }

                    // Was there a from and until date included in the token?
                    if (set_code.IndexOf(".") > 0)
                    {
                        int period_index = set_code.IndexOf(".");
                        string from_string = set_code.Substring(period_index + 1, 8);
                        string until_string = set_code.Substring(period_index + 9, 8);
                        set_code = set_code.Substring(0, period_index);

                        // Before we can parse those dates, we need to format a bit for the .NET framework
                        from_string = from_string.Substring(4, 2) + "/" + from_string.Substring(6) + "/" + from_string.Substring(0, 4);
                        until_string = until_string.Substring(4, 2) + "/" + until_string.Substring(6) + "/" + until_string.Substring(0, 4);

                        // Try to parse the dates now
                        if ((!DateTime.TryParse(from_string, out from_date)) || (!DateTime.TryParse(until_string, out until_date)))
                        {
                            Write_Error(Output, "resumptionToken=\"" + resumptionToken + "\" verb=\"ListRecords\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + resumptionToken);
                            return;
                        }
                    }
                }
                catch
                {
                    Write_Error(Output, "resumptionToken=\"" + resumptionToken + "\" verb=\"ListIdentifiers\"", "badResumptionToken", "no resumeAfter in resumptionToken:" + resumptionToken);
                    return;
                }
            }
            else
            {
                // Add to the request 
                if (from.Length > 0)
                    request.Append("from=\"" + from + "\" ");
                request.Append("metadataPrefix=\"" + metadataPrefix + "\" ");
                if (set_code.Length > 0)
                    request.Append("set=\"" + set_code + "\" ");
                if (until.Length > 0)
                    request.Append("until=\"" + until + "\" ");

                // Check the format of FROM and UNTIL, if they are provided
                const string regexMatchString = "([0-9]{4})(-([0-9]{2})(-([0-9]{2})))";
                if (from.Length > 0)
                {
                    if ((from.Length != 10) || (!Regex.Match(from, regexMatchString).Success) || (!DateTime.TryParse(from, out from_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + from);
                        return;
                    }
                }
                if (until.Length > 0)
                {
                    if ((until.Length != 10) || (!Regex.Match(until, regexMatchString).Success) || (!DateTime.TryParse(until, out until_date)))
                    {
                        Write_Error(Output, "verb=\"\"", "badArgument", "bad OAI date format:" + until);
                        return;
                    }
                }
            }

            // Finish the request echo portion
            request.Append("verb=\"ListIdentifiers\"");

            // Metadata prefix must currently be oai_dc
            if (metadataPrefix != "oai_dc")
            {
                Write_Error(Output, request.ToString(), "cannotDisseminateFormat", metadataPrefix + " is not supported by this repository.");
                return;
            }

            // Get the records
            List<OAI_Record> results = SobekCM_Database.Get_OAI_Data(set_code, metadataPrefix, from_date, until_date, IDENTIFIERS_PER_PAGE, current_page, false );
            if (results.Count == 0)
            {
                Write_Error(Output, request.ToString(), "noRecordsMatch", "There are no records matching the criteria indicated by this request's arguments.");
            }
            else
            {
                // Determine if a resumption token should be submitted after this.  This is easily
                // done since the database will attempt to return one additional record, if more records
                // exist
                bool more_records = false;
                if (results.Count > IDENTIFIERS_PER_PAGE)
                {
                    more_records = true;
                    results.RemoveAt(results.Count - 1);
                }

                // Write the list of records/identifiers
                Write_OAI_ListRecords(set_code, from_date, until_date, resumptionToken, request.ToString(), true, current_page, more_records, results, Output);
            }
        }

        /// <summary> Identifies the information about this OAI-PMH server </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        protected internal void Identify(TextWriter Output)
        {
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request verb=\"Identify\">" + url + "</request>");
            Output.WriteLine("<Identify>");
            Output.WriteLine("<repositoryName>" + UI_ApplicationCache_Gateway.Settings.OAI_Repository_Name + "</repositoryName>");
            Output.WriteLine("\t<baseURL>" + url + "</baseURL>");
            Output.WriteLine("\t<protocolVersion>2.0</protocolVersion>");
            Output.WriteLine("\t<adminEmail>" + UI_ApplicationCache_Gateway.Settings.System_Email + "</adminEmail>");
            Output.WriteLine("\t<earliestDatestamp>2005-12-15</earliestDatestamp>");
            Output.WriteLine("\t<deletedRecord>transient</deletedRecord>");
            Output.WriteLine("\t<granularity>YYYY-MM-DD</granularity>");
            Output.WriteLine("<description>");
            Output.WriteLine("\t<oai-identifier xmlns=\"http://www.openarchives.org/OAI/2.0/oai-identifier\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai-identifier http://www.openarchives.org/OAI/2.0/oai-identifier.xsd\">");
            Output.WriteLine("\t\t<scheme>oai</scheme>");
            Output.WriteLine("\t\t<repositoryIdentifier>" + UI_ApplicationCache_Gateway.Settings.OAI_Repository_Identifier + "</repositoryIdentifier>");
            Output.WriteLine("\t\t<delimiter>:</delimiter>");
            Output.WriteLine("\t\t<sampleIdentifier>" + UI_ApplicationCache_Gateway.Settings.OAI_Resource_Identifier_Base + "AB12345678</sampleIdentifier>");
            Output.WriteLine("\t</oai-identifier>");
            Output.WriteLine("</description>");
            Output.WriteLine("</Identify>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Gets the OAI-PMH m,etadata for a single digital resource, by identifier </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="identifier"> Identifier for the record to retrieve the metadata for </param>
        /// <param name="metadata_prefix"> Prefix for the metadata format to return the record </param>
        protected internal void GetRecord(TextWriter Output, string identifier, string metadata_prefix)
        {
            // Perform check that the identifier is valid
            bool valid = true;
            string record = String.Empty;

            // Metadata prefix must currently be oai_dc currently
            if (metadata_prefix != "oai_dc")
            {
                Write_Error(Output, "identifier=\"" + identifier + "\" metadataPrefix=\"" + metadata_prefix + "\" verb=\"GetRecord\"", "cannotDisseminateFormat", "Item " + identifier + " does not have metadata for " + metadata_prefix);
                return;
            }

            // Ensure the identifier is in basic correct form
            OAI_Record thisTitle = null;
            if (identifier.IndexOf( UI_ApplicationCache_Gateway.Settings.OAI_Resource_Identifier_Base ) != 0)
            {
                valid = false;
            }
            else
            {
                // Get the bib id and vid
                string bibid = identifier.Substring(UI_ApplicationCache_Gateway.Settings.OAI_Resource_Identifier_Base.Length);
                if (bibid.Length != 10)
                    valid = false;
                else
                {
                    // Pull the information for this item
                    thisTitle = SobekCM_Database.Get_OAI_Record(bibid, metadata_prefix);
                }
            }              

            // Throw error if invalid
            if ((!valid) || (thisTitle == null ))
            {
                Write_Error(Output, "identifier=\"" + identifier + "\" metadataPrefix=\"" + metadata_prefix + "\" verb=\"GetRecord\"", "idDoesNotExist", "identifier is not valid URI: " + identifier);
                return;
            }
            
            // Display the information
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request identifier=\"" + identifier + "\" metadataPrefix=\"" + metadata_prefix + "\" verb=\"GetRecord\">" + url + "</request>");
            Output.WriteLine("<GetRecord>");

            Output.Write("<record><header><identifier>" + UI_ApplicationCache_Gateway.Settings.OAI_Resource_Identifier_Base + thisTitle.BibID + "</identifier><datestamp>" + thisTitle.Last_Modified_Date.Year + "-" + thisTitle.Last_Modified_Date.Month.ToString().PadLeft(2, '0') + "-" + thisTitle.Last_Modified_Date.Day.ToString().PadLeft(2, '0') + "</datestamp></header>");
            Output.WriteLine("<metadata><oai_dc:dc xmlns:oai_dc=\"http://www.openarchives.org/OAI/2.0/oai_dc/\" xmlns:dc=\"http://purl.org/dc/elements/1.1/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/oai_dc/ http://www.openarchives.org/OAI/2.0/oai_dc.xsd\">" + thisTitle.Record + "</oai_dc:dc></metadata></record>");

            Output.WriteLine("</GetRecord>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Gets the metadata formats available for either the entire set or a single item, by identifier. </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="identifier"> Identifier to check for different possible identifiers </param>
        protected internal void ListMetadataFormats(TextWriter Output, string identifier)
        {
            // See if an individual was identified
            if (identifier.Length > 0)
            {
                // Perform check that the identifier is valid
                bool valid = true;

                // Throw error if invalid
                if (!valid)
                {
                    Write_Error(Output, "identifier=\"" + identifier + "\" verb=\"ListMetadataFormats\"", "idDoesNotExist", "identifier is not valid URI: " + identifier);
                    return;
                }
            }

            // Prepare response with DC as metadata format
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\" ?> ");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            if (identifier.Length > 0)
                Output.WriteLine("<request identifier=\"" + identifier + "\" verb=\"ListMetadataFormats\">" + url + "</request>");
            else
                Output.WriteLine("<request verb=\"ListMetadataFormats\">" + url + "</request>");
            Output.WriteLine("<ListMetadataFormats>");
            Output.WriteLine("<metadataFormat>");
            Output.WriteLine("<metadataPrefix>oai_dc</metadataPrefix>");
            Output.WriteLine("<schema>http://www.openarchives.org/OAI/2.0/oai_dc.xsd</schema>");
            Output.WriteLine("<metadataNamespace>http://www.openarchives.org/OAI/2.0/oai_dc/</metadataNamespace>");
            Output.WriteLine("</metadataFormat>");
            Output.WriteLine("</ListMetadataFormats>");
            Output.WriteLine("</OAI-PMH>");
        }

        /// <summary> Write the OAI-PMH error in the correct (OAI-PMH) format </summary>
        /// <param name="Output"> Stream to which to write the text for this main writer </param>
        /// <param name="request_verb"> Verb from the original request </param>
        /// <param name="error_code"> OAI-PMH Code for the error encountered </param>
        /// <param name="error_string"> String describes the problem encountered </param>
        protected internal void Write_Error(TextWriter Output, string request_verb, string error_code, string error_string)
        {
            Output.WriteLine("<?xml version=\"1.0\" encoding=\"UTF-8\"?>");
            Output.WriteLine("<?xml-stylesheet type=\"text/xsl\" href=\"oai2.xsl\" ?>");
            Output.WriteLine("<OAI-PMH xmlns=\"http://www.openarchives.org/OAI/2.0/\" xmlns:xsi=\"http://www.w3.org/2001/XMLSchema-instance\" xsi:schemaLocation=\"http://www.openarchives.org/OAI/2.0/ http://www.openarchives.org/OAI/2.0/OAI-PMH.xsd\">");
            Output.WriteLine("<responseDate>" + date_in_utc(DateTime.Now) + "</responseDate>");
            Output.WriteLine("<request " + request_verb + ">" + url + "</request>");
            Output.WriteLine("<error code=\"" + error_code + "\">" + error_string + "</error>");
            Output.WriteLine("</OAI-PMH>");
        }

        private string date_in_utc(DateTime date)
        {
            DateTime dateUTC = date.ToUniversalTime();
            return dateUTC.Year.ToString() + "-" + dateUTC.Month.ToString().PadLeft(2, '0') + "-" + dateUTC.Day.ToString().PadLeft(2, '0') + "T" + dateUTC.Hour.ToString().PadLeft(2, '0') + ":" + dateUTC.Minute.ToString().PadLeft(2, '0') + ":" + dateUTC.Second.ToString().PadLeft(2, '0') + "Z";
        }

        private String GetHtmlPage(string strURL)
        {
            try
            {
                // the html retrieved from the page
                String strResult;
                WebRequest objRequest = WebRequest.Create(strURL);
                WebResponse objResponse = objRequest.GetResponse();
                // the using keyword will automatically dispose the object 
                // once complete
                using (StreamReader sr =new StreamReader(objResponse.GetResponseStream()))
                {
                    strResult = sr.ReadToEnd();
                    // Close and clean up the StreamReader
                    sr.Close();
                }
                return strResult;
            }
            catch (Exception ee)
            {
                throw new ApplicationException("Error pulling html data '" + strURL + "'", ee);
            }
        }
    }
}
