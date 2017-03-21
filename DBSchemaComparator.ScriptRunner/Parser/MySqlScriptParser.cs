using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.ScriptRunner.Deployment;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Parser
{
    
    public class MySqlScriptParser : IParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();
       

        public bool ExecuteTransactionScript(string[] scriptArray, Database db)
        {
            throw new NotImplementedException();
        }

        public string[] GetMsScriptArray(string script)
        {
            throw new NotImplementedException();
        }
    }
}
