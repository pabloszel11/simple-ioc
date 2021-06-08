using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCEngine
{
    public class LoggerClient
    {
        public string Execute(string message)
        {
            var factory = new LoggerFactory();
            var logger = factory.CreateLogger();

            return logger.Log(message);
        }
    }
}
