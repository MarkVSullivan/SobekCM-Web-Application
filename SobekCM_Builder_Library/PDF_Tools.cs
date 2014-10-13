#region Using directives

using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using iTextSharp.text.pdf;

#endregion

namespace SobekCM.Builder_Library
{
    /// <summary> Class contains some basic methods for preparing PDF files for loading into a SobekCM library </summary>
    /// <remarks> Portions of the PDF text extraction algorithm originally from Zoller (http://www.codeproject.com/KB/cs/PDFToText.aspx) </remarks>
    public static class PDF_Tools
    {
        #region Code for creating the main thumbnail for a PDF 

        /// <summary> Creates a main thumbnail from a PDF file </summary>
        /// <param name="Working_Directory"> Directory in which to do this work</param>
        /// <param name="PDF_In_Name">Name of the incoming PDF to create the thumbnail from</param>
        /// <param name="JPEG_Out_Name">Name of the resulting JPEG thumbnail file</param>
        /// <param name="Ghostscript_Exectuable">Path and file for the ghostcript executable</param>
        /// <param name="ImageMagick_Executable">Imagemagick executable file</param>
        /// <returns>TRUE if successful, otherwise FALSE</returns>
        public static bool Create_Thumbnail( string Working_Directory, string PDF_In_Name, string JPEG_Out_Name, string Ghostscript_Exectuable, string ImageMagick_Executable )
        {
            try
            {
                // Create the main JPEG for the first page of the PDF through ghostscript
                Process ghostScriptProcess = new Process {StartInfo = {FileName = Ghostscript_Exectuable, Arguments = "-q -dQUIET -dSAFER -dBATCH -dNOPAUSE -dNOPROMPT -dFirstPage=1 -dLastPage=1 -dMaxBitmap=500000000 -dGraphicsTextBits=1 -dTextAlphaBits=1 -sDEVICE=jpeg -sOutputFile=\"" + Working_Directory + "\\temp_builder.jpg\" \"" + PDF_In_Name + "\"", CreateNoWindow = true}};
                ghostScriptProcess.Start();
                ghostScriptProcess.WaitForExit(30000);                

                // Now, ensure the temp file was created
                if (!File.Exists(Working_Directory + "\\temp_builder.jpg"))
                    return false;

                // Run imagemagick to create the thumbnail
                Process imageMagickProcess = new Process {StartInfo = {FileName = ImageMagick_Executable, Arguments = "\"" + Working_Directory + "\\temp_builder.jpg\" -resize 150x250 \"" + JPEG_Out_Name + "\"", CreateNoWindow = true}};
                imageMagickProcess.Start();
                imageMagickProcess.WaitForExit(30000);

                // Now, delete the temporary JPEG
                File.Delete(Working_Directory + "\\temp_builder.jpg");

                // Make sure the resulting file exists
                return File.Exists(JPEG_Out_Name);
            }
            catch
            {
                try
                {
                    // Delete the temporary JPEG, if it exists
                    if (File.Exists(Working_Directory + "\\temp_builder.jpg"))
                        File.Delete( Working_Directory + "\\temp_builder.jpg" );
                }
                catch
                {
                }

                return false;
            }
        }


        #endregion

        #region Code for extracting the text from a PDF

        /// <summary> Extracts the full text from a PDF file and writes to a file </summary>
        /// <param name="PDF_In_Name">Full path to the pdf file</param>
        /// <param name="Text_Out_Name">Output file name for the extracted text </param>
        /// <returns>TRUE if successful, otherwise FALSE</returns>
        public static bool Extract_Text(string PDF_In_Name, string Text_Out_Name)
        {
            StreamWriter outFile = null;
            PdfReader reader = null;
            try
            {
                // Create a reader for the given PDF file
                reader = new PdfReader(PDF_In_Name);
                //outFile = File.CreateText(outFileName);
                outFile = new StreamWriter(Text_Out_Name, false, Encoding.UTF8);

                for (int page = 1; page <= reader.NumberOfPages; page++)
                {
                    try
                    {
                        string text_to_add = ExtractTextFromPDFBytes(reader.GetPageContent(page));

                        if (text_to_add.Trim().Length > 0)
                        {
                            outFile.WriteLine();
                            outFile.WriteLine("PAGE " + page);
                            outFile.WriteLine();
                            outFile.WriteLine(text_to_add);
                        }
                    }
                    catch
                    {

                    }
                }
                return true;
            }
            catch 
            {
                
            }
            finally
            {
                if (outFile != null) outFile.Close();
                if ( reader != null ) reader.Close();
            }

            return false;
        }

        #region ExtractTextFromPDFBytes

        /// <summary> This method processes an uncompressed Adobe (text) object and extracts text. </summary>
        /// <param name="Input">uncompressed</param>
        /// <returns> Full text from the PDF bytes </returns>
        private static string ExtractTextFromPDFBytes(byte[] Input)
        {
            // The number of characters to keep, when extracting text.
            const int NUMBER_OF_CHARS_TO_KEEP = 5;

            if (Input == null || Input.Length == 0) return "";

            // BT = Beginning of a text object operator 
            // ET = End of a text object operator
            // Td move to the start of next line
            //  5 Ts = superscript
            // -5 Ts = subscript
            string[] bt_array = { "BT" };
            string[] et_array = { "ET" };
            string[] tj_array = { "Tj" };
            string[] td_array = { "TD", "Td" };
            string[] special_array = { "'", "T*", "\"" };

            try
            {
                StringBuilder resultString = new StringBuilder();

                // Flag showing if we are we currently inside a text object
                bool inTextObject = false;

                // Flag showing if the next character is literal 
                // e.g. '\\' to get a '\' character or '\(' to get '('
                bool nextLiteral = false;

                // () Bracket nesting level. Text appears inside ()
                int bracketDepth = 0;

                char lastCharacter = ' ';

                // Keep previous chars to get extract numbers etc.:
                char[] previousCharacters = new char[NUMBER_OF_CHARS_TO_KEEP];
                for (int j = 0; j < NUMBER_OF_CHARS_TO_KEEP; j++) previousCharacters[j] = ' ';

                // When building ascii in a PDF, use the extended ascii encoding set and build the ascii
                int build_ascii = 0;
                int build_octal = 0;
                Encoding windows1252Encoder = Encoding.GetEncoding(1252);

                foreach (byte t in Input) 
                {
                    char c = (char)t;

                    // If we were building the ascii for a literal >128 ascii character, deal
                    // with that here and indicate done with literal
                    if ((build_ascii > 0) && (!Char.IsNumber(c)))
                    {
                        // In some PDFs we receive from born-digital newspapers, the Windows1252
                        // code section is repeated slightly higher, partly in reverse
                        if (build_ascii == 325) build_octal = 146;
                        if (build_ascii == 324) build_octal = 145;
                        if (build_ascii == 323) build_octal = 148;
                        if (build_ascii == 322) build_octal = 147;
                        if (build_ascii == 320) build_octal = 150;
                        if (build_ascii == 319) build_octal = 151;

                        if (build_octal == 213)
                            build_octal = 146;

                        if ((build_octal >= 145) && (build_octal <= 151))
                        {
                            char[] converted_chars = windows1252Encoder.GetChars(new byte[] { (byte)build_octal });
                            resultString.Append(converted_chars[0]);
                            lastCharacter = converted_chars[0];
                        }

                        build_ascii = 0;
                        build_octal = 0;
                        nextLiteral = false;
                    }

                    if (inTextObject)
                    {
                        // Position the text
                        if (bracketDepth == 0)
                        {
                            if (CheckToken(td_array, previousCharacters, NUMBER_OF_CHARS_TO_KEEP))
                            {
                                if ((resultString.Length > 1) && (resultString[resultString.Length - 2] == '-') && (resultString[resultString.Length - 1] == ' '))
                                {
                                    resultString.Remove(resultString.Length - 2, 2);
                                }
                                else
                                {
                                    if (lastCharacter != ' ')
                                    {
                                        lastCharacter = ' ';
                                        resultString.Append(" ");
                                    }
                                }
                            }
                            else
                            {
                                if (CheckToken(special_array, previousCharacters, NUMBER_OF_CHARS_TO_KEEP))
                                {
                                    if ((resultString.Length > 1) && (resultString[resultString.Length - 2] == '-') && (resultString[resultString.Length - 1] == ' '))
                                    {
                                        resultString.Remove(resultString.Length - 2, 2);
                                    }
                                    else
                                    {
                                        if (lastCharacter != ' ')
                                        {
                                            lastCharacter = ' ';
                                            resultString.Append(" ");
                                        }
                                    }
                                }
                                else
                                {
                                    if (CheckToken(tj_array, previousCharacters, NUMBER_OF_CHARS_TO_KEEP))
                                    {
                                        if ((resultString.Length > 1) && (resultString[resultString.Length - 2] == '-') && (resultString[resultString.Length - 1] == ' '))
                                        {
                                            resultString.Remove(resultString.Length - 2, 2);
                                        }
                                        else
                                        {
                                            if (lastCharacter != ' ')
                                            {
                                                lastCharacter = ' ';
                                                resultString.Append(" ");
                                            }
                                        }
                                    }
                                }
                            }
                        }

                        // End of a text object, also go to a new line.
                        if (bracketDepth == 0 &&
                            CheckToken(et_array, previousCharacters, NUMBER_OF_CHARS_TO_KEEP))
                        {

                            inTextObject = false;
                            if ((resultString.Length > 1) && (resultString[resultString.Length - 2] == '-') && (resultString[resultString.Length - 1] == ' '))
                            {
                                resultString.Remove(resultString.Length - 2, 2);
                            }
                            else
                            {
                                if (lastCharacter != ' ')
                                {
                                    lastCharacter = ' ';
                                    resultString.Append(" ");
                                }
                            }
                        }
                        else
                        {
                            // Start outputting text
                            if ((c == '(') && (bracketDepth == 0) && (!nextLiteral))
                            {
                                bracketDepth = 1;
                                //if (lastCharacter != ' ')
                                //{
                                //    lastCharacter = ' ';
                                //    resultString.Append(" ");
                                //}
                            }
                            else
                            {
                                // Stop outputting text
                                if ((c == ')') && (bracketDepth == 1) && (!nextLiteral))
                                {
                                    bracketDepth = 0;
                                }
                                else
                                {
                                    // Just a normal text character:
                                    if (bracketDepth == 1)
                                    {
                                        // Only print out next character no matter what. 
                                        // Do not interpret.
                                        if (c == '\\' && !nextLiteral)
                                        {
                                            nextLiteral = true;
                                            build_ascii = 0;
                                            build_octal = 0;
                                        }
                                        else
                                        {
                                            if ((nextLiteral) && (Char.IsNumber(c)))
                                            {
                                                build_octal = (build_octal * 8) + Convert.ToInt32(c.ToString());
                                                build_ascii = (build_ascii * 10) + Convert.ToInt32(c.ToString());
                                            }
                                            else
                                            {
                                                if (((c >= ' ') && (c <= '~')) || ((c >= 128) && (c < 255)))
                                                {
                                                    if ((resultString.Length > 1) && (resultString[resultString.Length - 2] == '-') && (resultString[resultString.Length - 1] == ' '))
                                                    {
                                                        resultString.Remove(resultString.Length - 2, 2);
                                                    }

                                                    // No point in putting in two spaces in a row
                                                    if ((c != ' ') || (lastCharacter != ' '))
                                                    {
                                                        lastCharacter = c;
                                                        resultString.Append(c);
                                                    }
                                                }

                                                nextLiteral = false;
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }

                    // Store the recent characters for 
                    // when we have to go back for a checking
                    for (int j = 0; j < NUMBER_OF_CHARS_TO_KEEP - 1; j++)
                    {
                        previousCharacters[j] = previousCharacters[j + 1];
                    }
                    previousCharacters[NUMBER_OF_CHARS_TO_KEEP - 1] = c;

                    // Start of a text object
                    if (!inTextObject && CheckToken(bt_array, previousCharacters, NUMBER_OF_CHARS_TO_KEEP))
                    {
                        if ((resultString.Length > 1) && (resultString[resultString.Length - 2] == '-') && (resultString[resultString.Length - 1] == ' '))
                        {
                            resultString.Remove(resultString.Length - 2, 2);
                        }
                        else
                        {
                            if (lastCharacter != ' ')
                            {
                                lastCharacter = ' ';
                                resultString.Append(" ");
                            }
                        }
                        inTextObject = true;
                    }
                }
                return resultString.ToString().Replace("Õ", "'").Replace("Ò", "\"").Replace("Ó", "\"").Replace("¥", "•");
            }
            catch
            {
                return "";
            }
        }

        #endregion

        #region CheckToken

        private static bool CheckToken(string[] Tokens, char[] Recent, int NumberOfCharsToKeep)
        {
            foreach (string token in Tokens)
            {
                if (token.Length > 1)
                {
                    if ((Recent[NumberOfCharsToKeep - 3] == token[0]) &&
                        (Recent[NumberOfCharsToKeep - 2] == token[1]) &&
                        ((Recent[NumberOfCharsToKeep - 1] == ' ') ||
                        (Recent[NumberOfCharsToKeep - 1] == 0x0d) ||
                        (Recent[NumberOfCharsToKeep - 1] == 0x0a)) &&
                        ((Recent[NumberOfCharsToKeep - 4] == ' ') ||
                        (Recent[NumberOfCharsToKeep - 4] == 0x0d) ||
                        (Recent[NumberOfCharsToKeep - 4] == 0x0a))
                        )
                    {
                        return true;
                    }
                }
                else
                {
                    if ((Recent[NumberOfCharsToKeep - 2] == token[0]) &&
                        ((Recent[NumberOfCharsToKeep - 1] == ' ') ||
                        (Recent[NumberOfCharsToKeep - 1] == 0x0d) ||
                        (Recent[NumberOfCharsToKeep - 1] == 0x0a)) &&
                        ((Recent[NumberOfCharsToKeep - 3] == ' ') ||
                        (Recent[NumberOfCharsToKeep - 3] == 0x0d) ||
                        (Recent[NumberOfCharsToKeep - 3] == 0x0a))
                        )
                    {
                        return true;
                    }
                }
            }
            return false;
        }


        #endregion

        #endregion

        /// <summary> Gets the number of pages within a PDF file </summary>
        /// <param name="Path"> Path and filename for the PDF to read </param>
        /// <returns> The number of pages within the PDF </returns>
        public static int Page_Count(string Path)
        {
            PdfReader pdf_file = null;
            int page_count;

            try
            {
                // open the file
                pdf_file = new PdfReader(Path);

                // read it's page count
                page_count = pdf_file.NumberOfPages;
            }
            catch
            {
                page_count = -1;
            }
            finally
            {
                // close the file.
                if (pdf_file != null)
                {
                    pdf_file.Close();
                }
            }
            
            // return the page count
            return page_count;
        }

    }
}
