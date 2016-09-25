using System;

namespace SobekCM.Core.XSLT
{
    /// <summary> Enumeration indicates which XSLT engine was used during the 
    /// XSLT transform process </summary>
    public enum XSLT_Transformer_Engine_Enum : byte
    {
        /// <summary> No engine successfully performed the transform </summary>
        NONE,

        /// <summary> Saxon-HE was used for the conversion </summary>
        Saxon,

        /// <summary> Native .NET libraries were utilized </summary>
        Native_dotNet
    }

    /// <summary> Return arguments from a XSLT transform request </summary>
    public class XSLT_Transformer_ReturnArgs
    {
        /// <summary> Flag indicates if the conversion was successful </summary>
        public bool Successful { get; set; }

        /// <summary> Engine that was used for the transform, assuming success </summary>
        public XSLT_Transformer_Engine_Enum Engine { get; set; }

        /// <summary> Version of the XSLT, if this was determined during the transform process </summary>
        public int XSLT_Version { get; set; }

        /// <summary> Error message if this was not successful </summary>
        public string ErrorMessage { get; set; }

        /// <summary> Exception, if one was thrown during processing </summary>
        public Exception InnerException { get; set; }

        /// <summary> If the output was not directed to a file, this returned the
        /// transformed XML from the source file </summary>
        public string TransformedString { get; set; }

        /// <summary> Number of milliseconds it took to perform the transform </summary>
        public double Milliseconds { get; set; }
    }
}
