using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace IoCEngine
{
    public class LoggerFactory
    {
        private static Func<ILogger> _provider;
        public ILogger CreateLogger()
        {
            if (_provider != null)
                return _provider();
            else
                throw new ArgumentException();
        }

        public static void SetProvider(Func<ILogger> provider)
        {
            _provider = provider;
        }
    }
}
