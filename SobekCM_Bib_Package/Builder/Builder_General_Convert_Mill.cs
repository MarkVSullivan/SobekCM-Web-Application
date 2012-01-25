using System;
using System.Text.RegularExpressions;

namespace SobekCM.Bib_Package.Builder
{
	/// <summary> Enum specified the roman numerals </summary>
	internal enum Roman_numerals 
	{ 
		I = 1,
		V = 5,
		X = 10,
		L = 50,
		C= 100,
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

		public static int Convert_Roman_To_Numbers(char thisChar)
		{		
			switch ( thisChar.ToString().ToUpper())
			{
				case "I": return 1;
				case "V": return 5;
				case "X": return 10;
				case "L": return 50;
				case "C": return 100;
				case "D": return 500;
				case "M": return 1000;
				default: return 0;
			}
		}
		public static int Convert_Roman_To_Numbers( string romanNum)
		{
			romanNum = romanNum.ToUpper();
			int returnNum = 0;
			int Biggest = 0;
			int thisNum = 0;
			for (int i = romanNum.Length-1; i >=0; i-- )
			{
				thisNum = Builder_General_Convert_Mill.Convert_Roman_To_Numbers(romanNum[i]);

				////				if ((int)Enum.Parse.typeof(Roman_numerals),romanNum[i].ToString() < Biggest))
				if ( thisNum < Biggest)
					returnNum = returnNum - thisNum;
				else
				{
					returnNum = returnNum + thisNum;
					Biggest = thisNum;
				}
			}
			return returnNum;
		}

		public static File_String_Type Convert_FileString_To_Type(string fileName, bool checkRoman)
		{
			if (fileName.ToUpper() == "COVER")
				return File_String_Type.COVER;
			if ( fileName.ToUpper() == "COPYRIGHT" )
				return File_String_Type.COPYRIGHT;
			if ((checkRoman) && ( IsRomanNumerals(fileName)))
				return File_String_Type.ROMAN_NUMERALS;
			if (IsNumeric(fileName))
				return File_String_Type.NUMBERS;
			// Default
			return File_String_Type.LETTERS;
		}


		private static bool IsRomanNumerals(string strToTest)
		{
			return (Regex.Match(strToTest, @"^[ivxlcdm]+$", System.Text.RegularExpressions.RegexOptions.IgnoreCase).Success);
////			return (!Regex.IsMatch(strToTest,@"^[^ivxlcdm]$",System.Text.RegularExpressions.RegexOptions.IgnoreCase));
		}

		private static bool IsNumeric(string strToTest)
		{
			try
			{
				Convert.ToInt32(strToTest);
				return true;
			}
			catch
			{
				return false;
			}
			//return (! Regex.IsMatch(strToTest,@"^[^0-9]$") );
		}	
	}
}
