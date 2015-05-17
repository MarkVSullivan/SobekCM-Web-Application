using System.Collections.Specialized;
using System.Linq;

namespace SobekCM.Engine_Library
{
    public static class ExtensionMethods
    {
        public static NameValueCollection Copy(this NameValueCollection nvc)
        {
            NameValueCollection collection = new NameValueCollection();
            foreach (string k in nvc.AllKeys)
                collection.Add(k, nvc[k]);

            return collection;
        }
    }
}
