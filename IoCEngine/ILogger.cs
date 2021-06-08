using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCEngine
{
    public interface ILogger
    {
        //Log returning changed string so I can demonstrate it works in tests
        public abstract string Log(string msg);
    }

    public class ConsoleLogger : ILogger
    {
        public string Log(string msg)
        {
            msg = "Log from ConsoleLogger on " + DateTime.Now + ": " + msg;
            Console.WriteLine(msg);
            return msg;
        }
    }

}
