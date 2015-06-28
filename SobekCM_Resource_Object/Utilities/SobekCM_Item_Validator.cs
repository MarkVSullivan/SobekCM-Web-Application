#region Using directives

using System.Collections.Generic;
using System.Data;
using System.Text.RegularExpressions;
using SobekCM.Resource_Object.Behaviors;
using SobekCM.Resource_Object.Bib_Info;

#endregion

namespace SobekCM.Resource_Object.Utilities
{
    /// <summary> Class is used to perform some basic validation on a <see cref="SobekCM_Item" />. </summary>
    public class SobekCM_Item_Validator
    {
        /// <summary> List of all projects or aggregations to validate an item against </summary>
        public static DataTable Project_Codes;

        /// <summary> Gets a flag indicating if the provided string appears to be in bib id format </summary>
        /// <param name="test_string"> string to check for bib id format </param>
        /// <returns> TRUE if this string appears to be in bib id format, otherwise FALSE </returns>
        public static bool is_bibid_format(string test_string)
        {
            // Must be 10 characters long to start with
            if (test_string.Length != 10)
                return false;

            // Use regular expressions to check format
            Regex myReg = new Regex("[A-Z]{2}[A-Z|0-9]{4}[0-9]{4}");
            return myReg.IsMatch(test_string.ToUpper());
        }

        /// <summary> Validate that the SobekCM Item obeys certain necessary requirements</summary>
        /// <param name="thisBib">SobekCM Item object</param>
        /// <param name="Validation_Errors">List of critical validation errors as output of validation process</param>
        /// <returns>TRUE if successful (may have warnings though) or FALSE if validation failed</returns>
        public static bool Validate_SobekCM_Item(SobekCM_Item thisBib, List<string> Validation_Errors)
        {
            // remove any messages in the collection
            Validation_Errors.Clear();

            // check if Bib ID field exists            
            if (thisBib.BibID.Length > 0)
            {
                // Is the Bib ID value in the correct format?
                if (!is_bibid_format(thisBib.BibID))
                    Validation_Errors.Add("Invalid Bib ID (" + thisBib.BibID + ");");
            }

            // check if Title field exists
            if (thisBib.Bib_Info.Main_Title.ToString().Length == 0)
                Validation_Errors.Add("No Title");


            // check if Material Type field exists
            if (thisBib.Bib_Info.Type.MODS_Type == TypeOfResource_MODS_Enum.UNKNOWN)
                Validation_Errors.Add("No Material Type");

            // check if Source Institution Code field exists
            if (thisBib.Bib_Info.Source.Code.Length == 0)
                Validation_Errors.Add("No Institution Code");

            // Check that all the project codes are valid
            if (Project_Codes != null)
            {
                // Check the primary code
                if (thisBib.Behaviors.Aggregation_Count > 0)
                {
                    foreach (Aggregation_Info aggregation in thisBib.Behaviors.Aggregations)
                    {
                        string altCode = aggregation.Code;
                        if (Project_Codes.Select("itemcode = '" + altCode.Replace("'", "") + "'").Length == 0)
                        {
                            Validation_Errors.Add("Invalid aggregation '" + altCode + "'");
                        }
                    }
                }
            }

            // step through the external record numbers
            if (thisBib.Bib_Info.Identifiers_Count > 0)
            {
                foreach (Identifier_Info thisIdentifier in thisBib.Bib_Info.Identifiers)
                {
                    // look for specific identifiers to validate
                    switch (thisIdentifier.Type.ToUpper())
                    {
                            // ALEPH
                        case "ALEPHBIBNUM":
                        case "ALEPH":
                            if ((thisIdentifier.Identifier.Length > 0) && (thisIdentifier.Identifier.Length > 10))
                                Validation_Errors.Add("Aleph value cannot exceed 10 characters");
                            break;

                            // OCLC
                        case "OCLC":
                            if ((thisIdentifier.Identifier.Length > 0) && (thisIdentifier.Identifier.Length > 9))
                                Validation_Errors.Add("OCLC value cannot exceed 9 characters");
                            break;

                            // ISBN
                        case "ISBN":

                            // ISBN field
                            if ((thisIdentifier.Identifier.Length > 0) && (thisIdentifier.Identifier.Length > 50))
                                Validation_Errors.Add("ISBN value cannot exceed 50 characters");

                            break;

                            // ISSN
                        case "ISSN":

                            // ISSN field
                            if ((thisIdentifier.Identifier.Length > 0) && (thisIdentifier.Identifier.Length > 50))
                                Validation_Errors.Add("ISSN value cannot exceed 50 characters");

                            break;

                            // LCCN
                        case "LCCN":

                            // LCCN field
                            if ((thisIdentifier.Identifier.Length > 0) && (thisIdentifier.Identifier.Length > 50))
                                Validation_Errors.Add("LCCN value cannot exceed 50 characters");

                            break;
                    }
                }
            }

            // Are ther any errors?
            if (Validation_Errors.Count > 0)
                // failed validation
                return false;
            else
                // passed validation
                return true;
        }
    }
}