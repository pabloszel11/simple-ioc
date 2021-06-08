using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCEngine.Exceptions
{
    public abstract class ContainerException : Exception
    {
    }

    public class CycleException : ContainerException { }
    public class MethodMissingException : ContainerException { }
}
