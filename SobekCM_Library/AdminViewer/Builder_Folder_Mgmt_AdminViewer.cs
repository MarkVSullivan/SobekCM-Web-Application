using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using SobekCM.Tools;

namespace SobekCM.Library.AdminViewer
{
    public class Builder_Folder_Mgmt_AdminViewer : abstract_AdminViewer
    {
        /// <summary> Constructor for a new instance of the Builder_Folder_Mgmt_AdminViewer class </summary>
        /// <param name="RequestSpecificValues"> All the necessary, non-global data specific to the current request </param>
        public Builder_Folder_Mgmt_AdminViewer(RequestCache RequestSpecificValues) : base(RequestSpecificValues)
        {

        }

        public override string Web_Title
        {
            get { throw new NotImplementedException(); }
        }

        public override string Viewer_Icon
        {
            get { throw new NotImplementedException(); }
        }

        public override void Write_HTML(TextWriter Output, Custom_Tracer Tracer)
        {
            throw new NotImplementedException();
        }
    }
}
