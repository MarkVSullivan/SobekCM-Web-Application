using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using SobekCM.Bib_Package;
using SobekCM.Bib_Package.Bib_Info;

namespace SobekCM.Bib_Package.Writers.SubWriters
{
    /// <summary> Subwriter writes Dublin Core formatted XML for a given digital resource </summary>
    public class Dublin_Core_SubWriter : XML_Writing_Base_Type
    {
        /// <summary> Add the bibliographic information as simple Dublin Core to the output stream for a given digital resource </summary>
        /// <param name="Output"> Output stream for this metadata to be written to </param>
        /// <param name="thisBib"> Source digital resource </param>
        public static void Add_Simple_Dublin_Core(System.IO.TextWriter Output, SobekCM_Item thisBib )
        {
            // Add all the titles
            Output.WriteLine("<dc:title>" + thisBib.Bib_Info.Main_Title.ToString() + "</dc:title>");
            List<string> titles = new List<string>();
            titles.Add(thisBib.Bib_Info.Main_Title.ToString().Trim());
            if (thisBib.Bib_Info.Other_Titles_Count > 0)
            {
                foreach (Title_Info thisTitle in thisBib.Bib_Info.Other_Titles)
                {
                    if (!titles.Contains(thisTitle.ToString().Trim()))
                    {
                        Output.WriteLine("<dc:title>" + Convert_String_To_XML_Safe_Static(thisTitle.ToString()) + "</dc:title>");
                        titles.Add(thisTitle.ToString().Trim());
                    }
                }
            }

            // Series title maps to dc:relation
            if ((thisBib.Bib_Info.hasSeriesTitle) && (thisBib.Bib_Info.SeriesTitle.Title.Length > 0))
            {
                if (!titles.Contains(thisBib.Bib_Info.SeriesTitle.ToString().Trim()))
                {
                    Output.WriteLine("<dc:relation>" + Convert_String_To_XML_Safe_Static(thisBib.Bib_Info.SeriesTitle.ToString()) + "</dc:relation>");
                }
            }

            // Add all the creators
            List<string> contributors = new List<string>();
            if ((thisBib.Bib_Info.hasMainEntityName) && (thisBib.Bib_Info.Main_Entity_Name.Full_Name.Length > 0))
            {
                if ((thisBib.Bib_Info.Main_Entity_Name.Roles.Count == 0) || (thisBib.Bib_Info.Main_Entity_Name.Roles[0].Role.ToUpper() != "CONTRIBUTOR"))
                    Output.WriteLine("<dc:creator>" + Convert_String_To_XML_Safe_Static(thisBib.Bib_Info.Main_Entity_Name.ToString().Replace("<i>", "").Replace("</i>", "")) + "</dc:creator>");
                else
                    contributors.Add(thisBib.Bib_Info.Main_Entity_Name.ToString(false).Replace("<i>", "").Replace("</i>", ""));
            }
            if (thisBib.Bib_Info.Names_Count > 0)
            {
                foreach (Name_Info thisName in thisBib.Bib_Info.Names)
                {
                    if ((thisName.Roles.Count == 0) || (thisName.Roles[0].Role.ToUpper() != "CONTRIBUTOR"))
                        Output.WriteLine("<dc:creator>" + Convert_String_To_XML_Safe_Static(thisName.ToString().Replace("<i>", "").Replace("</i>", "")) + "</dc:creator>");
                    else
                        contributors.Add(thisName.ToString(false) .Replace("<i>", "").Replace("</i>", ""));
                }
            }

            // Add any collected contributors
            if (contributors.Count > 0)
            {
                foreach (string thisContributor in contributors)
                {
                    Output.WriteLine("<dc:contributor>" + Convert_String_To_XML_Safe_Static(thisContributor) + "</dc:contributor>");
                }
            }

            // Add the coverages (hierarchical geographic)
            if (thisBib.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in thisBib.Bib_Info.Subjects)
                {
                    if (thisSubject.Class_Type == Subject_Info_Type.Hierarchical_Spatial)
                    {
                        string subject_string = thisSubject.ToString();
                        if (subject_string.Length > 0)
                        {
                            Output.WriteLine("<dc:coverage>" + Convert_String_To_XML_Safe_Static(subject_string) + "</dc:coverage>");
                        }
                    }
                }
            }

            // Add the coverages (temporal)
            if (thisBib.Bib_Info.TemporalSubjects_Count > 0)
            {
                foreach (Temporal_Info thisTemporal in thisBib.Bib_Info.TemporalSubjects)
                {
                    if (thisTemporal.TimePeriod.Length > 0)
                    {
                        Output.WriteLine("<dc:coverage>" + Convert_String_To_XML_Safe_Static(thisTemporal.TimePeriod) + "</dc:coverage>");
                    }
                }
            }

            // Add the date issued
            if (thisBib.Bib_Info.Origin_Info.Date_Issued.Length > 0)
            {
                Output.WriteLine("<dc:date>" + Convert_String_To_XML_Safe_Static(thisBib.Bib_Info.Origin_Info.Date_Issued) + "</dc:date>");
            }

            // Add all descriptions/notes
            if ((thisBib.Bib_Info.Original_Description != null) && (thisBib.Bib_Info.Original_Description.Notes_Count > 0))
            {
                foreach (string physicalDescNote in thisBib.Bib_Info.Original_Description.Notes)
                {
                    Output.WriteLine("<dc:description>" + Convert_String_To_XML_Safe_Static(physicalDescNote) + "</dc:description>");
                }
            }
            if (thisBib.Bib_Info.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in thisBib.Bib_Info.Notes)
                {
                    if ( thisNote.Note_Type != Note_Type_Enum.source )  
                        Output.WriteLine("<dc:description>" + Convert_String_To_XML_Safe_Static(thisNote.ToString().Replace("<b>", "(").Replace("</b>", ") ")) + "</dc:description>");
                }
            }

            // Add the format
            if (thisBib.Bib_Info.Original_Description.Extent.Length > 0)
            {
                Output.WriteLine("<dc:format>" + Convert_String_To_XML_Safe_Static(thisBib.Bib_Info.Original_Description.Extent) + "</dc:format>");
            }

            // Add all the other identifiers
            if (thisBib.Bib_Info.Identifiers_Count > 0)
            {
                foreach (Identifier_Info thisIdentifier in thisBib.Bib_Info.Identifiers)
                {
                    Output.WriteLine("<dc:identifier>" + Convert_String_To_XML_Safe_Static(thisIdentifier.Identifier) + "</dc:identifier>");
                }
            }

            // Add the language information
            if (thisBib.Bib_Info.Languages_Count > 0)
            {
                foreach (Language_Info thisLanguage in thisBib.Bib_Info.Languages)
                {
                    if (thisLanguage.Language_Text.Length > 0)
                    {
                        Output.WriteLine("<dc:language>" + Convert_String_To_XML_Safe_Static(thisLanguage.Language_Text) + "</dc:language>");
                    }
                }
            }

            // Add the subjects
            if (thisBib.Bib_Info.Subjects_Count > 0)
            {
                foreach (Subject_Info thisSubject in thisBib.Bib_Info.Subjects)
                {
                    if (thisSubject.Class_Type != Subject_Info_Type.Hierarchical_Spatial)
                    {

                        if (thisSubject.Class_Type == Subject_Info_Type.Standard)
                        {
                            Subject_Info_Standard standSubj = (Subject_Info_Standard)thisSubject;
                            if (standSubj.Geographics_Count > 0)
                            {
                                string[] geographics = new string[standSubj.Geographics_Count];
                                standSubj.Geographics.CopyTo(geographics, 0);
                                standSubj.Clear_Geographics();
                                string subject_string = thisSubject.ToString();
                                if (subject_string.Length > 0)
                                {
                                    Output.WriteLine("<dc:subject>" + Convert_String_To_XML_Safe_Static(subject_string.Replace("<i>", "").Replace("</i>", "")) + "</dc:subject>");
                                }
                                foreach (string thisGeographic in geographics)
                                {
                                    Output.WriteLine("<dc:coverage>" + Convert_String_To_XML_Safe_Static(thisGeographic) + "</dc:coverage>");
                                    standSubj.Add_Geographic(thisGeographic);
                                }

                            }
                            else
                            {
                                string subject_string = thisSubject.ToString();
                                if (subject_string.Length > 0)
                                {
                                    Output.WriteLine("<dc:subject>" + Convert_String_To_XML_Safe_Static(subject_string.Replace("<i>", "").Replace("</i>", "")) + "</dc:subject>");
                                }
                            }
                        }
                        else
                        {
                            string subject_string = thisSubject.ToString();
                            if (subject_string.Length > 0)
                            {
                                Output.WriteLine("<dc:subject>" + Convert_String_To_XML_Safe_Static( subject_string.Replace("<i>", "").Replace("</i>", "")) + "</dc:subject>");
                            }
                        }
                    }

                }
            }



            // Add all the publishers from the origin info section
            if (thisBib.Bib_Info.Origin_Info.Publishers_Count > 0)
            {
                foreach (string publisher in thisBib.Bib_Info.Origin_Info.Publishers)
                {
                    Output.WriteLine("<dc:publisher>" + Convert_String_To_XML_Safe_Static(publisher) + "</dc:publisher>");
                }
            }

            // Add all publishers held in the more complete custom section
            if (thisBib.Bib_Info.Publishers.Count > 0)
            {
                foreach (Publisher_Info thisPublisher in thisBib.Bib_Info.Publishers)
                {
                    Output.WriteLine("<dc:publisher>" + Convert_String_To_XML_Safe_Static(thisPublisher.ToString()) + "</dc:publisher>");
                }
            }

            // Add the type
            string mods_type = thisBib.Bib_Info.Type.MODS_Type_String;
            if (mods_type.Length > 0)
            {
                Output.WriteLine("<dc:type>" + Convert_String_To_XML_Safe_Static(mods_type) + "</dc:type>");
            }
            if (thisBib.Bib_Info.Type.Uncontrolled_Types_Count > 0)
            {
                foreach (string thisType in thisBib.Bib_Info.Type.Uncontrolled_Types)
                {
                    Output.WriteLine("<dc:type>" + Convert_String_To_XML_Safe_Static(thisType) + "</dc:type>");

                }
            }

            // Add the relations
            if (thisBib.Bib_Info.RelatedItems_Count > 0)
            {
                foreach (Related_Item_Info thisRelatedItem in thisBib.Bib_Info.RelatedItems)
                {
                    if (thisRelatedItem.Main_Title.Title.Trim().Length > 0)
                    {
                        Output.WriteLine("<dc:relation>" + Convert_String_To_XML_Safe_Static(thisRelatedItem.Main_Title.Title) + "</dc:relation>");

                    }
                }
            }

            // Add the rights
            if (thisBib.Bib_Info.Access_Condition.Text.Length > 0)
            {
                Output.WriteLine("<dc:rights>" + Convert_String_To_XML_Safe_Static(thisBib.Bib_Info.Access_Condition.Text) + "</dc:rights>");
            }

            // Add the source note
            if (thisBib.Bib_Info.Notes_Count > 0)
            {
                foreach (Note_Info thisNote in thisBib.Bib_Info.Notes)
                {
                    if (thisNote.Note_Type == Note_Type_Enum.source)
                        Output.WriteLine("<dc:source>" + Convert_String_To_XML_Safe_Static(thisNote.Note) + "</dc:source>");
                }
            }


            //// Add the source information
            //if (thisBib.Bib_Info.Source.Statement.Length > 0)
            //{
            //    Output.WriteLine("<dc:source>" + thisBib.Bib_Info.Source.XML_Safe_Statement + "</dc:source>");
            //}






        }
    }
}
