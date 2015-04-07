namespace SobekCM.Builder_Library.Tools
{
    /// <summary> Class is used to convert Word and Powerpoint files to PDF </summary>
    public static class Word_Powerpoint_to_PDF_Converter
    {
        /// <summary> Convert a Microsoft Word file to a PDF </summary>
        /// <param name="Word_In_File"> Input file (word) </param>
        /// <param name="PDF_Out_File"> Outpuf file (pdf) </param>
        /// <returns>An error value</returns>
        public static int Word_To_PDF( string Word_In_File, string PDF_Out_File )
        {
            SautinSoft.UseOffice u = new SautinSoft.UseOffice();

            //Prepare UseOffice .Net, loads MS Word in memory
            int ret = u.InitWord();
            u.Serial = "10006108851";

            //Return values:
            //0 - Loading successfully
            //1 - Can't load MS Word® library in memory (returned as 4)

            if (ret == 1)
                return 4;

            if (Word_In_File.ToUpper().IndexOf(".DOCX") > 0)
            {
                //Converting
                ret = u.ConvertFile(Word_In_File, PDF_Out_File, SautinSoft.UseOffice.eDirection.DOCX_to_PDF);
            }
            else
            {
                //Converting
                ret = u.ConvertFile(Word_In_File, PDF_Out_File, SautinSoft.UseOffice.eDirection.DOC_to_PDF);
            }

            //Release MS Word from memory
            u.CloseWord();

            //0 - Converting successfully
            //1 - Can't open input file. Check that you are using full local path to input file, URL and relative path are not supported
            //2 - Can't create output file. Please check that you have permissions to write by this path or probably this path already used by another application
            //3 - Converting failed, please contact with our Support Team
            //4 - MS Office isn't installed. The component requires that any of these versions of MS Office should be installed: 2000, XP, 2003, 2007 or 2010
            return ret;
        }

        /// <summary> Convert a Microsoft Powerpoint file to a PDF </summary>
        /// <param name="Powerpoint_In_File"> Input file (powerpoint) </param>
        /// <param name="PDF_Out_File"> Outpuf file (pdf) </param>
        /// <returns>An error value</returns>
        public static int Powerpoint_To_PDF(string Powerpoint_In_File, string PDF_Out_File)
        {
            SautinSoft.UseOffice u = new SautinSoft.UseOffice();

            //Prepare UseOffice .Net, loads MS Powerpoint in memory
            int ret = u.InitPowerPoint();
            u.Serial = "10006108851";

            //Return values:
            //0 - Loading successfully
            //1 - Can't load MS Powerpoint® library in memory (returned as 4)

            if (ret == 1)
                return 4;

            if (Powerpoint_In_File.ToUpper().IndexOf(".PPTX") > 0)
            {
                //Converting
                ret = u.ConvertFile(Powerpoint_In_File, PDF_Out_File, SautinSoft.UseOffice.eDirection.PPTX_to_PDF);
            }
            else
            {
                //Converting
                ret = u.ConvertFile(Powerpoint_In_File, PDF_Out_File, SautinSoft.UseOffice.eDirection.PPT_to_PDF);
            }

            //Release MS Powerpoint from memory
            u.ClosePowerPoint();

            //0 - Converting successfully
            //1 - Can't open input file. Check that you are using full local path to input file, URL and relative path are not supported
            //2 - Can't create output file. Please check that you have permissions to write by this path or probably this path already used by another application
            //3 - Converting failed, please contact with our Support Team
            //4 - MS Office isn't installed. The component requires that any of these versions of MS Office should be installed: 2000, XP, 2003, 2007 or 2010
            return ret;
        }
    }
}
