#region Using directives

using System.Collections.Specialized;

#endregion

namespace SobekCM.Engine_Library
{
    /// <summary> Class holds all extension methods added for use within the engine library </summary>
    public static class ExtensionMethods
    {
        /// <summary> Extension method allows NameValueCollections to make a deep copy of themselves </summary>
        /// <param name="Nvc"> This NameValueCollection object to copy </param>
        /// <returns> Copied NameValueCollection collection </returns>
        public static NameValueCollection Copy(this NameValueCollection Nvc)
        {
            NameValueCollection collection = new NameValueCollection();
            foreach (string k in Nvc.AllKeys)
                collection.Add(k, Nvc[k]);

            return collection;
        }
    }
}
