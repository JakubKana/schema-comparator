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


        public static string[] GetMsScriptArray(string script)
        {
        
           

            //Split by GO statement
            var parsed = Regex.Split(script, "GO\n");
            //Remove comments
            Predicate<string> filter = FindComments;

            var results = Array.FindAll(parsed, filter);
            var remains = parsed.Except(results).ToArray();
            if (remains.Last() == string.Empty)
            {
                remains = remains.Take(remains.Length - 1).ToArray();
            }
            
            return remains;
        }

        private static bool FindComments(string obj)
        {
            var result = Regex.IsMatch(obj, "^(\n)*/\\*\\*\\*\\*\\*\\*");
            return result;
        }

    }
}
