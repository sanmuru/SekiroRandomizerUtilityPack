using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SekiroRandomizer.Patch
{
    [AttributeUsage(AttributeTargets.Method)]
    class InjectTargetAttribute : Attribute
    {
        public string Namespace { get; set; }
        public string Type { get; set; }
        public string[] NestedTypes { get; set; }
        public string Member { get; set; }
    }
}
