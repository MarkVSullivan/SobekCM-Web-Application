#region Using directives

using System;
using System.IO;
using SobekCM.Library.Settings;

#endregion

namespace SobekCM.Builder_Library.Modules.Items
{
    public class ConvertOfficeFilesToPdfModule : abstractSubmissionPackageModule
    {
        public override void DoWork(Incoming_Digital_Resource Resource)
        {
            string resourceFolder = Resource.Resource_Folder;

            // Should we try to convert office files?
            if (InstanceWide_Settings_Singleton.Settings.Convert_Office_Files_To_PDF)
            {
                try
                {
                    // Preprocess each Powerpoint document to PDF
                    string[] ppt_files = Directory.GetFiles(resourceFolder, "*.ppt*");
                    foreach (string thisPowerpoint in ppt_files)
                    {
                        // Get the fileinfo and the name
                        FileInfo thisPowerpointInfo = new FileInfo(thisPowerpoint);
                        string filename = thisPowerpointInfo.Name.Replace(thisPowerpointInfo.Extension, "");

                        // Does a PDF version exist for this item?
                        string pdf_version = resourceFolder + "\\" + filename + ".pdf";
                        if (!File.Exists(pdf_version))
                        {
                            int conversion_error = Word_Powerpoint_to_PDF_Converter.Powerpoint_To_PDF(thisPowerpoint, pdf_version);
                            switch (conversion_error)
                            {
                                case 1:
                                    OnError("Error converting PPT to PDF: Can't open input file", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;

                                case 2:
                                    OnError("Error converting PPT to PDF: Can't create output file", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;

                                case 3:
                                    OnError("Error converting PPT to PDF: Converting failed", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;

                                case 4:
                                    OnError("Error converting PPT to PDF: MS Office not installed", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;
                            }
                        }
                    }

                    // Preprocess each Word document to PDF
                    string[] doc_files = Directory.GetFiles(resourceFolder, "*.doc*");
                    foreach (string thisWordDoc in doc_files)
                    {
                        // Get the fileinfo and the name
                        FileInfo thisWordDocInfo = new FileInfo(thisWordDoc);
                        string filename = thisWordDocInfo.Name.Replace(thisWordDocInfo.Extension, "");

                        // Does a PDF version exist for this item?
                        string pdf_version = resourceFolder + "\\" + filename + ".pdf";
                        if (!File.Exists(pdf_version))
                        {
                            int conversion_error = Word_Powerpoint_to_PDF_Converter.Word_To_PDF(thisWordDoc, pdf_version);
                            switch (conversion_error)
                            {
                                case 1:
                                    OnError("Error converting Word DOC to PDF: Can't open input file", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;

                                case 2:
                                    OnError("Error converting Word DOC to PDF: Can't create output file", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;

                                case 3:
                                    OnError("Error converting Word DOC to PDF: Converting failed", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;

                                case 4:
                                    OnError("Error converting Word DOC to PDF: MS Office not installed", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                                    break;
                            }
                        }
                    }
                }
                catch (Exception ee)
                {
                    string startupPath = System.Reflection.Assembly.GetExecutingAssembly().CodeBase;

                    StreamWriter errorWriter = new StreamWriter( Path.Combine(startupPath, "Logs\\error.log"), true);
                    errorWriter.WriteLine("Message: " + ee.Message);
                    errorWriter.WriteLine("Stack Trace: " + ee.StackTrace);
                    errorWriter.Flush();
                    errorWriter.Close();

                    OnError("Unknown error converting office files to PDF", Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                    OnError(ee.Message, Resource.BibID + ":" + Resource.VID, Resource.METS_Type_String, Resource.BuilderLogId);
                }
            }
        }


    }
}
