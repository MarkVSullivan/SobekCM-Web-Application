using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Bib_Package;

namespace SobekCM.Bib_Package.Readers.SubReaders
{
    /// <summary> Subreader reads the Electronic Thesis and Disserations (ETD) section of XML and stores the data in the provided digital resource object </summary>
    public class Thesis_Dissertation_Reader
    {
        /// <summary> Reads the Electronic Thesis and Disserations (ETD) section of XML and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the daitss data </param>
        /// <param name="package"> Digital resource object to save the data to </param>
        public static void Read_ETD_Sec(XmlTextReader r, SobekCM_Item package)
        {
            // Loop through reading each XML node
            do
            {
                // If this is the end of this section, return
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "METS:mdWrap") || (r.Name == "mdWrap")))
                    return;

                // get the right division information based on node type
                if (r.NodeType == XmlNodeType.Element)
                {
                    string name = r.Name.ToLower();
                    if (name.IndexOf("palmm:") == 0)
                        name = name.Substring(6);

                    switch (name)
                    {
                        case "committeechair":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.ETD.Committee_Chair = r.Value.Trim();
                            }
                            break;

                        case "committeecochair":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.ETD.Committee_Co_Chair = r.Value.Trim();
                            }
                            break;

                        case "committeemember":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.ETD.Add_Committee_Member(r.Value);
                            }
                            break;

                        case "graduationdate":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                try
                                {
                                    package.ETD.Graduation_Date = Convert.ToDateTime(r.Value);
                                }
                                catch
                                {

                                }
                            }
                            break;

                        case "degree":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.ETD.Degree = r.Value;
                            }
                            break;

                        case "degreediscipline":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.ETD.Degree_Discipline = r.Value;
                            }
                            break;

                        case "degreegrantor":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.ETD.Degree_Grantor = r.Value;
                            }
                            break;

                        case "degreelevel":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                string temp = r.Value.ToLower();
                                if (temp == "doctorate")
                                    package.ETD.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Doctorate;
                                if (temp == "masters")
                                    package.ETD.Degree_Level = Thesis_Dissertation_Info.Thesis_Degree_Level_Enum.Masters;
                            }
                            break;
                    }
                }

            } while (r.Read());
        }

    }
}
