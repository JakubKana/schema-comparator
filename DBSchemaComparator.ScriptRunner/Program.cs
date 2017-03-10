using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Models.General;
using NLog;

namespace DBSchemaComparator.ScriptRunner
{
    class Program
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
        static void Main(string[] args)
        {
            if (args.Length == 3)
            {
                 
            }
            {
                Logger.Info(new ArgumentException(),"Invalid input argument count.");
                Environment.Exit((int)ExitCodes.InvalidArguments);
            }

            while (ar)



        }
    }
}
