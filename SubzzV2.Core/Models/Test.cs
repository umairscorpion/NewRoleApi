using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SubzzV2.Core.Models
{
    public class Test
    {
        public Test()
        {
            Permissions = new Test();
        }
        public Test Permissions { get; set; }
    }
}
