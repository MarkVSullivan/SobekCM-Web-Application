using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Xml;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;
using SobekCM.Bib_Package.SobekCM_Info;
using SobekCM.Bib_Package.Divisions;

namespace SobekCM.Bib_Package.Readers.SubReaders
{
    /// <summary> Subreader reads a Dublin Core-compliant section of XML and stores the data in the provided digital resource object </summary>
    public class Dublin_Core_SubReader
    {
        /// <summary> Reads the Dublin Core-compliant section of XML and stores the data in the provided digital resource </summary>
        /// <param name="r"> XmlTextReader from which to read the dublin core data </param>
        /// <param name="package"> Digital resource object to save the data to </param>
        public static void Read_Dublin_Core_Info(XmlTextReader r, SobekCM_Item package )
        {
            while (r.Read())
            {
                if ((r.NodeType == XmlNodeType.EndElement) && ((r.Name == "METS:mdWrap") || (r.Name == "mdWrap")))
                    return;

                if (r.NodeType == XmlNodeType.Element)
                {
                    switch (r.Name)
                    {
                        case "dc:contributor":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Add_Named_Entity(r.Value.Trim(), "Contributor");
                            }
                            break;

                        case "dc:coverage":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                Subject_Info_Standard thisSubject = new  Subject_Info_Standard();
                                thisSubject.Add_Geographic( r.Value.Trim());
                                package.Bib_Info.Add_Subject(thisSubject);
                            }
                            break;

                        case "dc:creator":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                if (package.Bib_Info.Main_Entity_Name.hasData)
                                {
                                    package.Bib_Info.Add_Named_Entity(r.Value.Trim());
                                }
                                else
                                {
                                    package.Bib_Info.Main_Entity_Name.Full_Name = r.Value.Trim();
                                }
                            }
                            break;

                        case "dc:date":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Origin_Info.Date_Issued = r.Value.Trim();
                            }
                            break;

                        case "dc:description":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Add_Note(r.Value.Trim());
                            }
                            break;

                        case "dc:format":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Original_Description.Extent = r.Value.Trim();
                            }
                            break;

                        case "dc:identifier":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Add_Identifier(r.Value.Trim());
                            }
                            break;

                        case "dc:language":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Add_Language(r.Value.Trim());
                            }
                            break;

                        case "dc:publisher":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Add_Publisher(r.Value.Trim());
                            }
                            break;

                        case "dc:relation":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                Related_Item_Info newRelatedItem = new Related_Item_Info();
                                newRelatedItem.Main_Title.Title = r.Value.Trim();
                                package.Bib_Info.Add_Related_Item(newRelatedItem);
                            }
                            break;

                        case "dc:rights":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Access_Condition.Text = r.Value.Trim();
                            }
                            break;

                        case "dc:source":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Add_Note(r.Value, Note_Type_Enum.source);
                            }
                            break;

                        case "dc:subject":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                if (r.Value.IndexOf(";") > 0)
                                {
                                    string[] splitter = r.Value.Split(";".ToCharArray());
                                    foreach (string thisSplit in splitter)
                                    {
                                        package.Bib_Info.Add_Subject(thisSplit.Trim(), String.Empty);
                                    }
                                }
                                else
                                {
                                    package.Bib_Info.Add_Subject(r.Value.Trim(), String.Empty);
                                }
                            }
                            break;

                        case "dc:title":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && ( r.Value.Trim().Length > 0 ))
                            {
                                if (package.Bib_Info.Main_Title.Title.Length == 0)
                                {
                                    package.Bib_Info.Main_Title.Title = r.Value.Trim();
                                }
                                else
                                {
                                    package.Bib_Info.Add_Other_Title(r.Value.Trim(), Title_Type_Enum.alternative);
                                }
                            }
                            break;

                        case "dc:type":
                            r.Read();
                            if ((r.NodeType == XmlNodeType.Text) && (r.Value.Trim().Length > 0))
                            {
                                package.Bib_Info.Type.Add_Uncontrolled_Type(r.Value.Trim());
                            }
                            break;
                    }
                }
            }
        }
    }
}
