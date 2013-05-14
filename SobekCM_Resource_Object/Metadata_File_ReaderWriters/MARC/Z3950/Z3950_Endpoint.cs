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

namespace SobekCM.Resource_Object.MARC
{
    /// <summary> Class stores information regarding a Z39.50 endpoint associated with this user </summary>
    public class Z3950_Endpoint
    {
        /// <summary> Constructor for a new instance of the Z39.50 endpoint object </summary>
        /// <param name="Name"> Arbitrary name associated with this Z39.50 endpoint by the user </param>
        /// <param name="URI"> URI / URL for the connection to the Z39.50 endpoint </param>
        /// <param name="Port"> Port for the connection to the Z39.50 endpoint </param>
        /// <param name="Database_Name"> Name of the database within the Z39.50 endpoint  </param>
        public Z3950_Endpoint(string Name, string URI, uint Port, string Database_Name)
        {
            this.Name = Name;
            this.URI = URI;
            this.Port = Port;
            this.Database_Name = Database_Name;
            Username = String.Empty;
            Password = String.Empty;
            Save_Password_Flag = false;
        }

        /// <summary> Constructor for a new instance of the Z39.50 endpoint object </summary>
        public Z3950_Endpoint()
        {
            Name = String.Empty;
            URI = String.Empty;
            Port = 0;
            Database_Name = String.Empty;
            Username = String.Empty;
            Password = String.Empty;
            Save_Password_Flag = false;
        }

        /// <summary> Constructor for a new instance of the Z39.50 endpoint object </summary>
        /// <param name="Name"> Arbitrary name associated with this Z39.50 endpoint by the user </param>
        /// <param name="URI"> URI / URL for the connection to the Z39.50 endpoint </param>
        /// <param name="Port"> Port for the connection to the Z39.50 endpoint </param>
        /// <param name="Database_Name"> Name of the database within the Z39.50 endpoint  </param>
        /// <param name="Username"> Username for the connection to the endpoint, if one is needed </param>
        public Z3950_Endpoint(string Name, string URI, uint Port, string Database_Name, string Username)
        {
            this.Name = Name;
            this.URI = URI;
            this.Port = Port;
            this.Database_Name = Database_Name;
            this.Username = Username;
            Password = String.Empty;
            Save_Password_Flag = false;
        }

        /// <summary> Arbitrary name associated with this Z39.50 endpoint by the user</summary>
        public string Name { get; set; }

        /// <summary> Port for the connection to the Z39.50 endpoint </summary>
        public uint Port { get; set; }

        /// <summary> Name of the database within the Z39.50 endpoint </summary>
        public string Database_Name { get; set; }

        /// <summary> URI / URL for the connection to the Z39.50 endpoint </summary>
        public string URI { get; set; }

        /// <summary> Username for the connection to the endpoint, if one is needed </summary>
        public string Username { get; set; }

        /// <summary> Password for the connection to the endpoint, if one is needed </summary>
        public string Password { get; set; }

        /// <summary> Flag indicates if the password should be saved for this connection to the user's 
        /// personal settings </summary>
        public bool Save_Password_Flag { get; set; }

        /// <summary> Create a copy of this object </summary>
        /// <returns> Copy of this object with all the same data </returns>
        public Z3950_Endpoint Copy()
        {
            Z3950_Endpoint copyPoint = new Z3950_Endpoint(Name, URI, Port, Database_Name, Username);
            copyPoint.Password = Password;
            copyPoint.Save_Password_Flag = Save_Password_Flag;
            return copyPoint;
        }
    }
}