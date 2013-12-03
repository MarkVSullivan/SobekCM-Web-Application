#region License and Copyright

//          SobekCM MARC Library ( Version 1.2 )
//          
//          Copyright (2005-2012) Mark Sullivan. ( Mark.V.Sullivan@gmail.com )
//          
//          This file is part of SobekCM MARC Library.
//          
//          SobekCM MARC Library is free software: you can redistribute it and/or modify
//          it under the terms of the GNU Lesser Public License as published by
//          the Free Software Foundation, either version 3 of the License, or
//          (at your option) any later version.
//            
//          SobekCM MARC Library is distributed in the hope that it will be useful,
//          but WITHOUT ANY WARRANTY; without even the implied warranty of
//          MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//          GNU Lesser Public License for more details.
//            
//          You should have received a copy of the GNU Lesser Public License
//          along with SobekCM MARC Library.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region Using directives

using System;
using System.IO;
using SobekCM.Resource_Object.MARC.Parsers;
using Zoom.Net;
using Zoom.Net.YazSharp;

#endregion

namespace SobekCM.Resource_Object.MARC
{
    public class MARC_Record_Z3950_Retriever
    {
        public static MARC_Record Get_Record_By_Primary_Identifier(string Primary_Identifier, Z3950_Endpoint Z3950_Server, out string Message)
        {
            // Initially set the message to empty
            Message = String.Empty;

            // http://jai-on-asp.blogspot.com/2010/01/z3950-client-in-cnet-using-zoomnet-and.html
            // http://www.indexdata.com/yaz/doc/tools.html#PQF
            // http://www.loc.gov/z3950/agency/defns/bib1.html
            // http://www.assembla.com/code/wasolic/subversion/nodes/ZOOM.NET 
            // http://lists.indexdata.dk/pipermail/yazlist/2007-June/002080.html
            // http://fclaweb.fcla.edu/content/z3950-access-aleph

            const string prefix = "@attrset Bib-1 @attr 1=12 ";

            try
            {
                IConnection connection; //	zoom db connector
                IPrefixQuery query; //	zoom query
                IRecord record; //	one zoom record
                IResultSet records; //	collection of records	

                //	allocate MARC tools
                MARC21_Exchange_Format_Parser parser = new MARC21_Exchange_Format_Parser();

                //	establish connection
                connection = new Connection(Z3950_Server.URI, Convert.ToInt32(Z3950_Server.Port));
                connection.DatabaseName = Z3950_Server.Database_Name;

                // Any authentication here?
                if (Z3950_Server.Username.Length > 0)
                    connection.Username = Z3950_Server.Username;
                if (Z3950_Server.Password.Length > 0)
                    connection.Password = Z3950_Server.Password;

                // Set to USMARC
                connection.Syntax = RecordSyntax.USMARC;

                //	call the Z39.50 server
                query = new PrefixQuery(prefix + Primary_Identifier);
                records = connection.Search(query);

                // If the record count is not one, return a message
                if (records.Count != 1)
                {
                    if (records.Count == 0)
                        Message = "ERROR: No matching record found in Z39.50 endpoint";
                    else
                        Message = "ERROR: More than one matching record found in Z39.50 endpoint by primary identifier";
                    return null;
                }

                //	capture the byte stream
                record = records[0];
                MemoryStream ms = new MemoryStream(record.Content);

                //	display while debugging
                //MessageBox.Show(Encoding.UTF8.GetString(record.Content));

                try
                {
                    //	feed the record to the parser and add the 955
                    MARC_Record marcrec = parser.Parse(ms);
                    parser.Close();
                    return marcrec;
                }
                catch (Exception error)
                {
                    Message = "ERROR: Unable to parse resulting record into the MARC Record structure!\n\n" + error.Message;
                    return null;
                    //MessageBox.Show("Could not convert item " + Primary_Identifier + " to MARCXML.");
                    //Trace.WriteLine("Could not convert item " + Primary_Identifier   + " to MARCXML.");
                    //Trace.WriteLine("Error details: " + error.Message);
                }
            }
            catch (Exception error)
            {
                if (error.Message.IndexOf("The type initializer for 'Zoom.Net.Yaz") >= 0)
                {
                    Message = "ERROR: The Z39.50 libraries did not correctly initialize.\n\nThese libraries do not currently work in 64-bit environments.";
                }
                else
                {
                    Message = "ERROR: Unable to connect and search provided Z39.50 endpoint.\n\n" + error.Message;
                }

                return null;
                //MessageBox.Show("Could not convert item " + Primary_Identifier + " to MARCXML.");
                //Trace.WriteLine("Could not convert item " + Primary_Identifier   + " to MARCXML.");
                //Trace.WriteLine("Error details: " + error.Message);
            }
        }


        public static MARC_Record Get_Record(int Attribute_Number, string Search_Term, Z3950_Endpoint Z3950_Server, out string Message)
        {
            // Initially set the message to empty
            Message = String.Empty;

            // http://jai-on-asp.blogspot.com/2010/01/z3950-client-in-cnet-using-zoomnet-and.html
            // http://www.indexdata.com/yaz/doc/tools.html#PQF
            // http://www.loc.gov/z3950/agency/defns/bib1.html
            // http://www.assembla.com/code/wasolic/subversion/nodes/ZOOM.NET 
            // http://lists.indexdata.dk/pipermail/yazlist/2007-June/002080.html
            // http://fclaweb.fcla.edu/content/z3950-access-aleph

            string prefix = "@attrset Bib-1 @attr 1=" + Attribute_Number + " ";

            try
            {
                IConnection connection; //	zoom db connector
                IPrefixQuery query; //	zoom query
                IRecord record; //	one zoom record
                IResultSet records; //	collection of records	

                //	allocate MARC tools
                MARC21_Exchange_Format_Parser parser = new MARC21_Exchange_Format_Parser();

                //	establish connection
                connection = new Connection(Z3950_Server.URI, Convert.ToInt32(Z3950_Server.Port));
                connection.DatabaseName = Z3950_Server.Database_Name;

                // Any authentication here?
                if (Z3950_Server.Username.Length > 0)
                    connection.Username = Z3950_Server.Username;
                if (Z3950_Server.Password.Length > 0)
                    connection.Password = Z3950_Server.Password;

                // Set to USMARC
                connection.Syntax = RecordSyntax.USMARC;

                //	call the Z39.50 server
                query = new PrefixQuery(prefix + Search_Term);
                records = connection.Search(query);

                // If the record count is not one, return a message
                if (records.Count != 1)
                {
                    if (records.Count == 0)
                        Message = "ERROR: No matching record found in Z39.50 endpoint";
                    else
                        Message = "ERROR: More than one matching record found in Z39.50 endpoint by ISBN";
                    return null;
                }

                //	capture the byte stream
                record = records[0];
                MemoryStream ms = new MemoryStream(record.Content);

                //	display while debugging
                //MessageBox.Show(Encoding.UTF8.GetString(record.Content));

                try
                {
                    //	feed the record to the parser and add the 955
                    MARC_Record marcrec = parser.Parse(ms);
                    parser.Close();
                    return marcrec;
                }
                catch (Exception error)
                {
                    Message = "ERROR: Unable to parse resulting record into the MARC Record structure!\n\n" + error.Message;
                    return null;
                    //MessageBox.Show("Could not convert item " + Primary_Identifier + " to MARCXML.");
                    //Trace.WriteLine("Could not convert item " + Primary_Identifier   + " to MARCXML.");
                    //Trace.WriteLine("Error details: " + error.Message);
                }
            }
            catch (Exception error)
            {
                if (error.Message.IndexOf("The type initializer for 'Zoom.Net.Yaz") >= 0)
                {
                    Message = "ERROR: The Z39.50 libraries did not correctly initialize.\n\nThese libraries do not currently work in 64-bit environments.";
                }
                else
                {
                    Message = "ERROR: Unable to connect and search provided Z39.50 endpoint.\n\n" + error.Message;
                }

                return null;
                //MessageBox.Show("Could not convert item " + Primary_Identifier + " to MARCXML.");
                //Trace.WriteLine("Could not convert item " + Primary_Identifier   + " to MARCXML.");
                //Trace.WriteLine("Error details: " + error.Message);
            }
        }
    }
}