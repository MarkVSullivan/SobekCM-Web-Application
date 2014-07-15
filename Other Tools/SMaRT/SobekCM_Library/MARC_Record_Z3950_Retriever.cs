//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using Zoom.Net.YazSharp;
//using Zoom.Net;
//using System.Xml;


//namespace SobekCM.Library
//{
//    public class MARC_Record_Z3950_Retriever
//    {
//        public static void Test()
//        {

//            // http://jai-on-asp.blogspot.com/2010/01/z3950-client-in-cnet-using-zoomnet-and.html
//            // http://www.indexdata.com/yaz/doc/tools.html#PQF
//            // http://www.loc.gov/z3950/agency/defns/bib1.html
//            // http://www.assembla.com/code/wasolic/subversion/nodes/ZOOM.NET 
//            // http://lists.indexdata.dk/pipermail/yazlist/2007-June/002080.html
            

//            try
//            {
//                //// Information for connecting to ALEPH
//                //Connection ob1 = new Connection("uf.aleph.fcla.edu", 9845);
//                //ob1.DatabaseName = "UFU01PUB";

//                // Information for connecting to OCLC
//                Connection ob1 = new Connection("zcat.oclc.org", 210);
//                ob1.DatabaseName = "OLUCWorldCat";
//                ob1.Username = "100315087";
//                ob1.Password = "fugdlc";

//                //define the syntax type that will be required. Here i am defining XML viz MarcXml
//                ob1.Syntax = Zoom.Net.RecordSyntax.USMARC;

//                //Connect to the server
//                ob1.Connect();

//                //Declare your query
//                //string query = "@and \"Robinson\" \"Crusoe\"";

                

//                //Create the object for query. 
//                string query = "@attr 1=12 01381152";
//                Zoom.Net.YazSharp.PrefixQuery q = new PrefixQuery(query);

//                IResultSet results;
//                //perform search
//                results = (ResultSet)ob1.Search(q);

//                //Now iterate through to the results and get the xml of each record fetched and derive from it the needed values.

//                for (uint i = 0; i < results.Size; i++)
//                {
//                    string temp = Encoding.UTF8.GetString(results[i].Content);
//                    //This string is having the xml in string format. Convert it into the xml via XmlDocument
//                    XmlDocument doc = new XmlDocument();
//                    doc.LoadXml(temp);

//                    //perform the needful operations
//                    //............... 
//                    //...............
//                    //............... 
//                }

//            }
//            catch ( Exception ee )
//            {
//                bool error = true;
//            }
//        }
//    }
//}
