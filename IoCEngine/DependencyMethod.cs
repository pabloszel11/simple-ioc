using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCEngine
{
    [AttributeUsage(AttributeTargets.Method)]
    public class DependencyMethod : Attribute
    {
    }
}
