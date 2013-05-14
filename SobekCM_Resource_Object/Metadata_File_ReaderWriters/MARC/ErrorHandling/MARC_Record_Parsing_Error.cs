#region License and Copyright

//          SobekCM MARC Library ( Version 1.2 )
//          
//          Copyright (2005-2012) Mark Sullivan. ( Mark.V.Sullivan@gmail.com )
//          
//          This file is part of SobekCM MARC Library.
//          
//          SobekCM MARC Library is free software: you can redistribute it and/or modify
//          it under the terms of the GNU Lesser Public License as published by
//          the Free Software Foundation, either version 3 of the License, or
//          (at your option) any later version.
//            
//          SobekCM MARC Library is distributed in the hope that it will be useful,
//          but WITHOUT ANY WARRANTY; without even the implied warranty of
//          MERCHANTABILITY or FITNESS FOR A PARTICULAR PURPOSE.  See the
//          GNU Lesser Public License for more details.
//            
//          You should have received a copy of the GNU Lesser Public License
//          along with SobekCM MARC Library.  If not, see <http://www.gnu.org/licenses/>.

#endregion

#region Using directives

using System;

#endregion

namespace SobekCM.Resource_Object.MARC.ErrorHandling
{
    /// <summary> Enumeration for the basic error types which need to be attached
    /// to a MARC record during parsing (mostly from a MARC21 exchange file)  </summary>
    public enum MARC_Record_Parsing_Error_Type_Enum : byte
    {
        /// <summary> Unknown type of error occurred </summary>
        UNKNOWN = 0,

        /// <summary> A directory and field length mismatch was discovered and could not be handled </summary>
        Directory_Field_Mismatch_Unhandled,

        /// <summary> The end of stream was unexpectedly encountered while parsing a record  </summary>
        Unexpected_End_Of_Stream_Encountered,

        /// <summary> The directory for a MARC21 record appears invalid </summary>
        Invalid_Directory_Encountered
    }

    /// <summary> Class stores basic error or error information which may 
    /// occur during processing </summary>
    public class MARC_Record_Parsing_Error : IEquatable<MARC_Record_Parsing_Error>
    {
        /// <summary> Any additional information about an error </summary>
        /// <remarks> This is different then the generic text for the Error; this is 
        /// ADDITIONAL information which may be saved for Error analysis </remarks>
        public readonly string Error_Details;

        /// <summary> Type of this error </summary>
        public readonly MARC_Record_Parsing_Error_Type_Enum Error_Type;

        /// <summary> Constructor for a new instance of the MARC_Record_Parsing_Error class </summary>
        /// <param name="Error_Type"> Type of this error </param>
        /// <param name="Error_Details"> Any additional information about an error </param>
        public MARC_Record_Parsing_Error(MARC_Record_Parsing_Error_Type_Enum Error_Type, string Error_Details)
        {
            this.Error_Type = Error_Type;
            this.Error_Details = Error_Details;
        }

        /// <summary> Constructor for a new instance of the MARC_Record_Parsing_Error class </summary>
        /// <param name="Error_Type"> Type of this error </param>
        public MARC_Record_Parsing_Error(MARC_Record_Parsing_Error_Type_Enum Error_Type)
        {
            this.Error_Type = Error_Type;
            Error_Details = String.Empty;
        }

        #region IEquatable<MARC_Record_Parsing_Error> Members

        /// <summary> Tests to see if this Error type is identical to another
        /// Error type </summary>
        /// <param name="other"> Other Error to check for type match </param>
        /// <returns> TRUE if the two Errors are the same type, otherwise FALSE </returns>
        public bool Equals(MARC_Record_Parsing_Error other)
        {
            return Error_Type == other.Error_Type;
        }

        #endregion
    }
}