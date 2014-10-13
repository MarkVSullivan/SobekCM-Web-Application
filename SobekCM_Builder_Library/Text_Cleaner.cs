#region Using directives

using System;
using System.IO;
using System.Text.RegularExpressions;

#endregion

namespace SobekCM.Builder_Library
{
    /// <summary> Class is used to clean incoming text files and remove offending additional spaces 
    /// or stand-alone punctuation </summary>
    public static class Text_Cleaner
    {
        /// <summary> Clean text file and remove offending additional spaces or stand-alone punctuation </summary>
        /// <param name="File"> Name of the file to scrub </param>
        public static void Clean_Text_File(string File)
        {
            StreamReader textReader = new StreamReader(File);
            string alltext = textReader.ReadToEnd();
            textReader.Close();

            alltext = alltext.Replace(" . ", " ");
            alltext = alltext.Replace(" , ", " ");
            alltext = alltext.Replace(" ! ", " ");
            alltext = alltext.Replace(" - ", " ");
            alltext = alltext.Replace(" _ ", " ");
            alltext = alltext.Replace(" \" ", " ");
            alltext = alltext.Replace(" * ", " ");
            alltext = alltext.Replace(" ' ", " ");

            for (int i = 0; i < 100; i++)
            {
                alltext = alltext.Replace("  ", " ");
            }

            StreamWriter textWriter = new StreamWriter(File, false);
            foreach (char thisChar in alltext)
            {
                int ascii = thisChar;
                if ((ascii < 169) && (ascii != 152) && (ascii != 158) && ( ascii != 127 ))
                {
                    textWriter.Write(thisChar);
                }
            }
            textWriter.Flush();
            textWriter.Close();
        }

        /// <summary> Checks to see if this file appears to have a SSN in the text, by performing
        /// a regular expression search for a match </summary>
        /// <param name="File"> Text file to check for SSN </param>
        /// <returns> The possible social security matching string </returns>
        public static string Has_SSN(string File)
        {
            const string SSN_REGEX_MATCHER = @"[/,,/.,/=,\s]([0-6]\d{2}|7[0-6]\d|77[0-2])(\s|\-)?(\d{2})\2(\d{4})[/,,/.,\s]";

            StreamReader textReader = new StreamReader(File);

            string line = textReader.ReadLine();
            while (line != null)
            {
                Match hasSSN = Regex.Match(line, SSN_REGEX_MATCHER);
                if (hasSSN.Success)
                {
                    string match_value = hasSSN.Value;
                    textReader.Close();
                    return match_value;
                }

                line = textReader.ReadLine();
            }

            textReader.Close();
            return String.Empty;
        }
    }
}
