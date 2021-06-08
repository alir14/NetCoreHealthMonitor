using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebApplication1.Model
{
    public class AppSetting
    {
        public HealthCheck HealthCheck { get; set; }
    }

    public class HealthCheck
    {
        public string SQLConnectionStringHealthCheck { get; set; }

        public string SQLConnectionStringMaster { get; set; }

        public string TestQuery { get; set; }

        public string URITodo { get; set; }

        public string URIUser { get; set; }

    }
}
