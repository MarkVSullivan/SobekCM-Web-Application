using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace SobekCM.Library.Localization
{
    class Single_Class_Info
    {

        public string ClassName { get; set; }

        public List<Property_Info> Properties;

        public Single_Class_Info(string ClassName)
        {
            this.ClassName = ClassName;
            Properties = new List<Property_Info>();
        }
    }
}
