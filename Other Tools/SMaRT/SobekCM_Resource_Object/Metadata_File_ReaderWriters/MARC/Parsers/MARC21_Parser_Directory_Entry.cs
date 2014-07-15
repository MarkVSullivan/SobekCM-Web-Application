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

namespace SobekCM.Resource_Object.MARC.Parsers
{
    /// <summary> Structure stores the basic information about a directory entry when parsing
    ///  a MARC21 record </summary>
    /// <remarks> This is only used while actually parsing the MARC21 record </remarks>
    internal struct MARC21_Parser_Directory_Entry
    {
        /// <summary> Length of the variable field, including indicators, subfield codes, data, and the field terminator.  </summary>
        public readonly short Field_Length;

        /// <summary> Starting character position of the variable field relative to the Base address of data (Leader/12-16) of the record. </summary>
        public readonly short Starting_Position;

        /// <summary> Three ASCII numeric or ASCII alphabetic characters (upper case or lower case, but not both) that identify an associated variable field. </summary>
        public readonly short Tag;

        /// <summary> Creates a new instance of the MARC21_Parser_Directory_Entry structure  </summary>
        /// <param name="Tag_Value"> Three ASCII numeric or ASCII alphabetic characters (upper case or lower case, but not both) that identify an associated variable field.</param>
        /// <param name="Length_Value"> Length of the variable field, including indicators, subfield codes, data, and the field terminator. </param>
        /// <param name="Starting_Position_Value"> Starting character position of the variable field relative to the Base address of data (Leader/12-16) of the record. </param>
        public MARC21_Parser_Directory_Entry(short Tag_Value, short Length_Value, short Starting_Position_Value)
        {
            Tag = Tag_Value;
            Field_Length = Length_Value;
            Starting_Position = Starting_Position_Value;
        }
    }
}