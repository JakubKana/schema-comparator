using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace DBSchemaComparator.App.Comparator
{
    public class ScriptParser
    {


        public static string[] GetMSScriptArray(string script)
        {
            var parsed = Regex.Split(script, "GO\n");

            return parsed;
        }

    }
}
