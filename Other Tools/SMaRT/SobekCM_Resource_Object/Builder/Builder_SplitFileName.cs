#region Using directives

using System;
using System.Collections.Specialized;
using System.Text;

#endregion

namespace SobekCM.Resource_Object.Builder
{
    /// <summary> Class is used to split a file name for sorting and ranging </summary>
    internal class Builder_SplitFileName
    {
        private string[] nameParts;
        private string origName;

        /// <summary> Constructor for a new instance of this class </summary>
        /// <param name="fileName"> Original file name </param>
        public Builder_SplitFileName(string fileName)
        {
            // Save the filename
            origName = fileName;

            // Create the collections for building the parts
            StringCollection parts = new StringCollection();
            StringBuilder partBuilder = new StringBuilder();

            // Check the first digit of the filename
            bool lastDigit = Char.IsDigit(fileName[0]);
            partBuilder.Append(fileName[0]);
            bool thisDigit;

            // Step through the remaining characters
            for (int i = 1; i < fileName.Length; i++)
            {
                // Underscore and dash always seperate parts
                if ((fileName[i] == '_') || (fileName[i] == '-'))
                {
                    // Only add this part, if it has length already
                    if (partBuilder.Length > 0)
                    {
                        parts.Add(partBuilder.ToString());
                        partBuilder.Remove(0, partBuilder.Length);
                    }
                }
                else
                {
                    // Check the next character to see if it is a number or not
                    thisDigit = Char.IsDigit(fileName[i]);

                    // If this is the same type as before, this continues the same part
                    if (thisDigit == lastDigit)
                    {
                        partBuilder.Append(fileName[i]);
                    }
                    else
                    {
                        // If there is already a part being built, make that its own part
                        if (partBuilder.Length > 0)
                        {
                            parts.Add(partBuilder.ToString());
                            partBuilder.Remove(0, partBuilder.Length);
                        }

                        // Start the next part
                        partBuilder.Append(fileName[i]);
                    }

                    // Save the digit flag
                    lastDigit = thisDigit;
                }
            }

            // Add the last part, if there was one being built
            if (partBuilder.Length > 0)
            {
                parts.Add(partBuilder.ToString());
            }

            // Fill the string array with all the parts
            nameParts = new string[parts.Count];
            for (int i = 0; i < parts.Count; i++)
                nameParts[i] = parts[i];
        }

        /// <summary> Gets the first part of this split name </summary>
        public string[] NameParts
        {
            get { return nameParts; }
        }

        /// <summary> Gets the original file name </summary>
        public string Orignal_Name
        {
            get { return origName; }
        }
    }
}