using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    public class Permissions_Reports_AdminViewer : abstract_AdminViewer
    {
        public Permissions_Reports_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {

        }

        public override string Web_Title
        {
            get { return "User Permissions"; }
        }

        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            // Nothin yet
        }
    }
}