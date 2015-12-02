#region Using directives

using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Data.SqlClient;
using System.Text;
using System.Text.RegularExpressions;
using System.Web;
using SobekCM.Core.Configuration.Engine;
using SobekCM.Tools;

#endregion

namespace SobekCM.Engine_Library.Endpoints
{
    /// <summary> Custom services endpoint is used to demonstrate how to add new endpoints to the 
    /// SobekCM system that are not part of the main system </summary>
    public class CustomServices : EndpointBase
    {
        // <summary> Register a user for the conference </summary>
        /// <param name="Response"></param>
        /// <param name="UrlSegments"></param>
        /// <param name="Protocol"></param>
        /// <param name="RequestForm"></param>
        /// <param name="IsDebug"></param>
        public void Register_User(HttpResponse Response, List<string> UrlSegments, Microservice_Endpoint_Protocol_Enum Protocol, NameValueCollection RequestForm, bool IsDebug)
        {
            Custom_Tracer tracer = new Custom_Tracer();

            // Add a trace
            tracer.Add_Trace("CustomServices.Register_User");

            // collect the values
            string eventName = null;
            string lastName = null;
            string firstName = null;
            string email = null;


            // Collect all the errors
            StringBuilder requiredValues = new StringBuilder();

            // Get and validate the required EVENT posted request string
            if (String.IsNullOrEmpty(RequestForm["event"]))
            {
                requiredValues.Append("\n\tEvent");
            }
            else
            {
                eventName = RequestForm["event"];
            }

            // Get and validate the required LAST NAME posted request string
            if (String.IsNullOrEmpty(RequestForm["lastName"]))
            {
                requiredValues.Append("\n\tLast Name");
            }
            else
            {
                lastName = RequestForm["lastName"];
            }

            // Get and validate the required FIRST NAME posted request string
            if (String.IsNullOrEmpty(RequestForm["firstName"]))
            {
                requiredValues.Append("\n\tFirst Name");
            }
            else
            {
                firstName = RequestForm["firstName"];
            }

            // Get and validate the required EMAIL posted request string
            if (String.IsNullOrEmpty(RequestForm["email"]))
            {
                requiredValues.Append("\n\tEmail");
            }
            else
            {
                email = RequestForm["email"];
            }

            // If we are missing some required fields, return a comprehensive error
            if (requiredValues.Length > 0)
            {
                Response.Output.Write("The following fields are required: \n" + requiredValues + "\n\nPlease enter the missing data and select REGISTER again.");
                Response.StatusCode = 400;
                return;
            }

            // Ensure the email address appears to be valid
            const string VALID_EMAIL_PATTERN = @"^\s*[\w\-\+_']+(\.[\w\-\+_']+)*\@[A-Za-z0-9]([\w\.-]*[A-Za-z0-9])?\.[A-Za-z][A-Za-z\.]*[A-Za-z]$";
            Regex emailCheck = new Regex(VALID_EMAIL_PATTERN, RegexOptions.IgnoreCase);
            if (( email != null ) && (!emailCheck.IsMatch(email)))
            {
                Response.Output.Write("Provided email address is not in the correct format.\n\nPlease correct the email address and select REGISTER again.");
                Response.StatusCode = 400;
                return;
            }

            // Get the optional values
            string phone = RequestForm["phone"] ?? String.Empty;
            string topics = RequestForm["topics"] ?? String.Empty;

            // Create the SQL connection
            using (SqlConnection sqlConnect = new SqlConnection("data source=SOB-SQL01\\SOBEK2;initial catalog=sobekrepository;integrated security=Yes;"))
            {
                try
                {
                    sqlConnect.Open();
                }
                catch
                {
                    Response.Output.Write("Unable to open connection to the database.");
                    Response.StatusCode = 500;
                    return;
                }

                // Create the SQL command
                SqlCommand sqlCommand = new SqlCommand("LOCAL_AddEventRegistration", sqlConnect)
                {
                    CommandType = CommandType.StoredProcedure
                };

                // Add the parameters
                sqlCommand.Parameters.AddWithValue("EventID", eventName);
                sqlCommand.Parameters.AddWithValue("FirstName", firstName);
                sqlCommand.Parameters.AddWithValue("LastName", lastName);
                sqlCommand.Parameters.AddWithValue("EmailAddress", email);
                sqlCommand.Parameters.AddWithValue("Phone", phone);
                sqlCommand.Parameters.AddWithValue("Notes", String.Empty);
                sqlCommand.Parameters.AddWithValue("Topics", topics);

                // Run the command itself
                try
                {
                    sqlCommand.ExecuteNonQuery();
                }
                catch
                {
                    Response.Output.Write("Error executing procedure to add your registration.");
                    Response.StatusCode = 500;
                    return;
                }

                // Close the connection (not technical necessary since we put the connection in the
                // scope of the using brackets.. it would dispose itself anyway)
                try
                {
                    sqlConnect.Close();
                }
                catch
                {
                    Response.Output.Write("Unable to close connection to the database.");
                    Response.StatusCode = 500;
                    return;
                }
            }
            
            Response.Output.Write("{\"message\":\"You have been registered\",\"success\":true}");
            Response.StatusCode = 200;
        }
    }
}
