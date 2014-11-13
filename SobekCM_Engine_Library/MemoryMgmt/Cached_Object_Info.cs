#region Using directives

using System;

#endregion

namespace SobekCM.Engine.MemoryMgmt
{
    /// <summary> Stored basic information about a cached object </summary>
    public struct Cached_Object_Info
    {
        /// <summary> Key under which this object is stored </summary>
        public string Object_Key;

        /// <summary> Type of object </summary>
        public Type Object_Type;

        /// <summary> Basic information about a cached object </summary>
        /// <param name="Object_Key"> Key under which this object is stored </param>
        /// <param name="Object_Type"> Type of object </param>
        public Cached_Object_Info(string Object_Key, Type Object_Type)
        {
            this.Object_Key = Object_Key;
            this.Object_Type = Object_Type;
        }
    }
}
