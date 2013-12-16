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
    /// <summary> Enumeration for the basic warning types which need to be attached
    /// to a MARC record during parsing (mostly from a MARC21 exchange file)  </summary>
    public enum MARC_Record_Parsing_Warning_Type_Enum : byte
    {
        /// <summary> Unknown type of warning (most likely not used, 
        /// as this would likely be an error instead ) </summary>
        UNKNOWN = 0,

        /// <summary> A directory and field length mismatch was discovered, but it 
        /// appears that it was able to be handled </summary>
        Directory_Field_Mismatch_Handled,

        /// <summary> Indicates that an alternate character set appears to have been present
        /// in a MARC8 character-encoded record </summary>
        Alternate_Character_Set_Present
    }

    /// <summary> Class stores basic warning or error information which may 
    /// occur during processing </summary>
    public class MARC_Record_Parsing_Warning : IEquatable<MARC_Record_Parsing_Warning>
    {
        /// <summary> Any additional information about a warning </summary>
        /// <remarks> This is different then the generic text for the warning; this is 
        /// ADDITIONAL information which may be saved for warning analysis </remarks>
        public readonly string Warning_Details;

        /// <summary> Type of this warning </summary>
        public readonly MARC_Record_Parsing_Warning_Type_Enum Warning_Type;

        /// <summary> Constructor for a new instance of the MARC_Record_Parsing_Warning class </summary>
        /// <param name="Warning_Type"> Type of this warning </param>
        /// <param name="Warning_Details"> Any additional information about a warning </param>
        public MARC_Record_Parsing_Warning(MARC_Record_Parsing_Warning_Type_Enum Warning_Type, string Warning_Details)
        {
            this.Warning_Type = Warning_Type;
            this.Warning_Details = Warning_Details;
        }

        /// <summary> Constructor for a new instance of the MARC_Record_Parsing_Warning class </summary>
        /// <param name="Warning_Type"> Type of this warning </param>
        public MARC_Record_Parsing_Warning(MARC_Record_Parsing_Warning_Type_Enum Warning_Type)
        {
            this.Warning_Type = Warning_Type;
            Warning_Details = String.Empty;
        }

        #region IEquatable<MARC_Record_Parsing_Warning> Members

        /// <summary> Tests to see if this warning type is identical to another
        /// warning type </summary>
        /// <param name="other"> Other warning to check for type match </param>
        /// <returns> TRUE if the two warnings are the same type, otherwise FALSE </returns>
        public bool Equals(MARC_Record_Parsing_Warning other)
        {
            return Warning_Type == other.Warning_Type;
        }

        #endregion
    }
}