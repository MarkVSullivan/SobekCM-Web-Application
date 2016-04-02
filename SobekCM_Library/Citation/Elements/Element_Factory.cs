#region Using directives

using System;
using SobekCM.Library.Citation.Elements.implemented_elements;

#endregion

namespace SobekCM.Library.Citation.Elements
{
    /// <summary> Factory which generates each element object, depending on the provided element type and subtype </summary>
    public class Element_Factory
    {
        /// <summary> Gets the element object associated with the provided type </summary>
        /// <param name="Type"> Type for the element to retrieve </param>
        /// <returns>Correct element object which implements the <see cref="abstract_Element"/> class. </returns>
        public static abstract_Element getElement( string Type )
        {
            return null;
           // return getElement( Element_Type_Convertor.ToType( Type ), String.Empty );
        }

        /// <summary> Gets the element object associated with the provided type and subtype </summary>
        /// <param name="Type"> Type for the element to retrieve </param>
        /// <param name="SubType"> Subtype for the element to retrieve </param>
        /// <returns>Correct element object which implements the <see cref="abstract_Element"/> class. </returns>
        public static abstract_Element getElement( string Type, string SubType )
        {
            return null;

            //return getElement( Element_Type_Convertor.ToType( Type ), SubType );
        }
    }
}
