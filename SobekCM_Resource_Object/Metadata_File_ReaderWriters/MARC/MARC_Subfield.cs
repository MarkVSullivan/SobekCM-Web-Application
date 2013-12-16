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

namespace SobekCM.Resource_Object.MARC
{
    /// <summary> Holds the data about a single subfield in a <see cref="MARC_Field"/>. <br /> <br /> </summary>
    public class MARC_Subfield
    {
        /// <summary> Constructor for a new instance the MARC_Subfield class </summary>
        /// <param name="Subfield_Code"> Code for this subfield in the MARC record </param>
        /// <param name="Data"> Data stored for this subfield </param>
        public MARC_Subfield(char Subfield_Code, string Data)
        {
            // Save the parameters
            this.Subfield_Code = Subfield_Code;
            this.Data = Data;
        }

        /// <summary> Gets the MARC subfield code associated with this data  </summary>
        public char Subfield_Code { get; private set; }

        /// <summary> Gets the data associated with this MARC subfield  </summary>
        public string Data { get; set; }

        /// <summary> Returns this MARC Subfield as a string </summary>
        /// <returns> Subfield in format '|x data'.</returns>
        public override string ToString()
        {
            return "|" + Subfield_Code + " " + Data;
        }
    }
}