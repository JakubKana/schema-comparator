using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DBSchemaComparator.App.Comparator
{
    public class ScriptParser
    {






        public static string[] GetMSScriptArray(string script)
        {
            var parsed = script.Split(Convert.ToChar("GO"));

            return parsed;
        }



    }
}
