using System;

namespace IoCEngine
{
    [AttributeUsage(AttributeTargets.Constructor)]
    public class DependencyConstructor : Attribute
    {
    }
}
