#region Using directives

using System.Collections.Generic;
using System.Text;

#endregion

namespace SobekCM.Library
{
    /// <summary> Class is used to highlight certain terms within a body of HTML text </summary>
    public class Text_Search_Term_Highlighter
    {
        private const string HIGHLIGHT_START = "<span class=\"texthighlight\">";
        private const string HIGHLIGHT_END = "</span>";
        private const int BACKWARDS_TAG_CHECK_LIMIT = 250;

        /// <summary> Checks the source html text for terms to highlight and returns the text, including any highlighting </summary>
        /// <param name="Source_Text"> Source text which may contain terms to highlight </param>
        /// <param name="Search_Term"> Term to highlight in the source text </param>
        /// <returns> Source text including highlights </returns>
        public static string Hightlight_Term_In_HTML(string Source_Text, string Search_Term)
        {
            List<string> terms = new List<string> {Search_Term};
            return Hightlight_Term_In_HTML(Source_Text, terms);
        }

        /// <summary> Checks the source html text for terms to highlight and returns the text, including any highlighting </summary>
        /// <param name="Source_Text"> Source text which may contain terms to highlight </param>
        /// <param name="Search_Terms"> Terms to highlight in the source text </param>
        /// <returns> Source text including highlights </returns>
        public static string Hightlight_Term_In_HTML(string Source_Text, List<string> Search_Terms)
        {
            return Hightlight_Term_In_HTML(Source_Text, Search_Terms, HIGHLIGHT_START, HIGHLIGHT_END);
        }


        /// <summary> Checks the source html text for terms to highlight and returns the text, including any highlighting </summary>
        /// <param name="Source_Text"> Source text which may contain terms to highlight </param>
        /// <param name="Search_Terms"> Terms to highlight in the source text </param>
        /// <param name="Highlight_Start">HTML to use at the beginning of a span to highlight</param>
        /// <param name="Highlight_End">HTML to use at the end of a span to highlight</param>
        /// <returns> Source text including highlights </returns>
        public static string Hightlight_Term_In_HTML(string Source_Text, List<string> Search_Terms, string Highlight_Start, string Highlight_End )
        {
            // Place the entire text into lower case
            string sourceLower = Source_Text.ToLower();

            // Step through and only keep search terms that exist along with first index
            List<string> terms = new List<string>(Search_Terms.Count);
            List<int> nextIndex = new List<int>(Search_Terms.Count);
            List<int> termLength = new List<int>(Search_Terms.Count);
            foreach (string searchTerm in Search_Terms)
            {
                string termLower = searchTerm.ToLower();
                int termIndex = sourceLower.IndexOf(termLower);
                if (termIndex < 0) continue;

                terms.Add(termLower);
                nextIndex.Add(termIndex);
                termLength.Add(termLower.Length);
            }

            // If no matches, we are done
            if (terms.Count == 0)
                return Source_Text;

            // Create the string builder to populate with the text as we build it
            StringBuilder builder = new StringBuilder(Source_Text.Length + 500);

            // Now, step through the entire text, looking for each term
            int current_start = 0;
            int tag_last_known_location = -1;
            bool tag_last_known_tag = false;
            while (terms.Count > 0)
            {
                // Find the next match
                int last_match = -1;
                int last_match_location = -1;
                if (terms.Count == 1)
                {
                    last_match_location = nextIndex[0];
                    last_match = 0;
                }
                else
                {
                    for (int i = 0; i < terms.Count; i++)
                    {
                        if ((last_match_location == -1) || (nextIndex[i] < last_match_location))
                        {
                            last_match_location = nextIndex[i];
                            last_match = i;
                        }
                    }
                }

                // Verify this is more than the current index (which would happen if the two
                // search terms contain each other, like "st augustine")
                if (last_match_location >= current_start)
                {
                    // Check if this match is actually in HTML tags
                    bool last_match_in_tag = false;
                    int tag_check_index = last_match_location;
                    int tag_check_counter = 0;
                    while ((tag_check_index >= 0) && (tag_check_counter < BACKWARDS_TAG_CHECK_LIMIT))
                    {
                        // Have we now encountered a point we previously checked from?
                        if (tag_check_index <= tag_last_known_location)
                        {
                            tag_last_known_location = last_match_location;
                            last_match_in_tag = tag_last_known_tag;
                            break;
                        }
                        
                        if (sourceLower[tag_check_index] == '>')
                        {
                            tag_last_known_location = last_match_location;
                            tag_last_known_tag = false;
                            break;
                        }

                        if (sourceLower[tag_check_index] == '<')
                        {
                            last_match_in_tag = true;
                            tag_last_known_location = last_match_location;
                            tag_last_known_tag = true;
                            break;
                        }

                        // Prepare for the next check
                        tag_check_counter++;
                        tag_check_index--;
                    }

                    // Add the string to the builder
                    builder.Append(Source_Text.Substring(current_start, last_match_location - current_start));
                    if (!last_match_in_tag)
                        builder.Append(Highlight_Start);
                    builder.Append(Source_Text.Substring(last_match_location, termLength[last_match]));
                    if (!last_match_in_tag)
                        builder.Append(Highlight_End);

                    // Set the current index correctly
                    current_start = last_match_location + termLength[last_match];
                }

                // Now, determine the next match for the last matched term
                int next_index = sourceLower.IndexOf(terms[last_match], current_start);
                if (next_index < 0)
                {
                    terms.RemoveAt(last_match);
                    termLength.RemoveAt(last_match);
                    nextIndex.RemoveAt(last_match);
                }
                else
                {
                    nextIndex[last_match] = next_index;
                }
            }

            // Add the remainder of the source
            builder.Append(Source_Text.Substring(current_start));

            // Return the built string
            return builder.ToString();
        }
    }
}
