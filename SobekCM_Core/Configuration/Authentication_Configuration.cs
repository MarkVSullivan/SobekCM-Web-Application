using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SobekCM.Core.Configuration
{
    public class Authentication_Configuration
    {
        public Authentication_Configuration()
        {
            Shibboleth = new Shibboleth_Configuration();
        }

        public Shibboleth_Configuration Shibboleth { get; set; }
    }
}
