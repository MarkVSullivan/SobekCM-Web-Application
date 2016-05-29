#region Using directives

using System;
using System.Text.RegularExpressions;

#endregion

namespace SobekCM_Resource_Database.Builder
{
    /// <summary> Enum specified the roman numerals </summary>
    internal enum RomanNumeralsEnum
    {
        I = 1,
        V = 5,
        X = 10,
        L = 50,
        C = 100,
        D = 500,
        M = 1000
    };

    internal enum File_String_Type
    {
        COPYRIGHT = 0,
        COVER,
        ROMAN_NUMERALS,
        NUMBERS,
        LETTERS
    }

    internal class Builder_General_Convert_Mill
    {
        static Builder_General_Convert_Mill()
        {
            // Empty static constructor
        }

        /// <summary> Convert the roman numeral character to decimal  </summary>
        /// <param name="ThisChar"> Roman numeral character </param>
        /// <returns> Roman numeral character, as decima </returns>
        public static int Convert_Roman_To_Numbers(char ThisChar)
        {
            switch (ThisChar.ToString().ToUpper())
            {
                case "I":
                    return 1;
                case "V":
                    return 5;
                case "X":
                    return 10;
                case "L":
                    return 50;
                case "C":
                    return 100;
                case "D":
                    return 500;
                case "M":
                    return 1000;
                default:
                    return 0;
            }
        }

        /// <summary> Converts a roman numeral string to the equivalent decimal value  </summary>
        /// <param name="RomanNum"> Roman number </param>
        /// <returns> Equivalent decimal value </returns>
        public static int Convert_Roman_To_Numbers(string RomanNum)
        {
            RomanNum = RomanNum.ToUpper();
            int returnNum = 0;
            int biggest = 0;
            for (int i = RomanNum.Length - 1; i >= 0; i--)
            {
                int thisNum = Convert_Roman_To_Numbers(RomanNum[i]);

                ////				if ((int)Enum.Parse.typeof(RomanNumeralsEnum),romanNum[i].ToString() < Biggest))
                if (thisNum < biggest)
                    returnNum = returnNum - thisNum;
                else
                {
                    returnNum = returnNum + thisNum;
                    biggest = thisNum;
                }
            }
            return returnNum;
        }

        /// <summary> Checks the new page name to determine if it is roman numerals, or something special  </summary>
        /// <param name="FileName"> Name of the page </param>
        /// <param name="CheckRoman"> Flag on whether to check for roman numerals or not </param>
        /// <returns> Information about how the file or page was named </returns>
        public static File_String_Type Convert_FileString_To_Type(string FileName, bool CheckRoman)
        {
            if (FileName.ToUpper() == "COVER")
                return File_String_Type.COVER;
            if (FileName.ToUpper() == "COPYRIGHT")
                return File_String_Type.COPYRIGHT;
            if ((CheckRoman) && (IsRomanNumerals(FileName)))
                return File_String_Type.ROMAN_NUMERALS;
            if (IsNumeric(FileName))
                return File_String_Type.NUMBERS;
            // Default
            return File_String_Type.LETTERS;
        }


        private static bool IsRomanNumerals(string StrToTest)
        {
            return (Regex.Match(StrToTest, @"^[ivxlcdm]+$", RegexOptions.IgnoreCase).Success);
////			return (!Regex.IsMatch(strToTest,@"^[^ivxlcdm]$",System.Text.RegularExpressions.RegexOptions.IgnoreCase));
        }

        private static bool IsNumeric(string StrToTest)
        {
                int test;
                return Int32.TryParse(StrToTest, out test);
        }
    }
}