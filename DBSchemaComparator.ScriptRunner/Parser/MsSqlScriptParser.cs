using System;
using System.Linq;
using System.Text.RegularExpressions;
using DBSchemaComparator.Domain.Infrastructure;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Parser
{
    public class MsSqlScriptParser : IParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool ExecuteTransactionScript(string[] scriptArray, Database db)
        {

            db.EnableAutoSelect = false;
            try
            {
                Logger.Info("Executing transaction script.");
                db.BeginTransaction();

                foreach (var s in scriptArray)
                {
                    Logger.Debug("Executing command", s);

                    var script = Extensions.RemoveBeginingNewLine(Extensions.NormalizeParameters(s));

                    db.Execute(script);
                }

                Logger.Info("Transaction Successful.");

                db.CompleteTransaction();

                return true;
            }
            catch (Exception exception)
            {
                Logger.Info(exception, "Aborting transaction.");
                db.AbortTransaction();
                return false;
            }
        }

        public string[] GetScriptArray(string script)
        {
            //Split by GO statement
            var parsed = Regex.Split(script, "GO");
            //Remove comments
            Predicate<string> filter = FindComments;
            Predicate<string> filterEmpty = FindWhiteSpaces;

            var results = Array.FindAll(parsed, filter);
            var remains = parsed.Except(results).ToArray();
            var results2 = Array.FindAll(remains, filterEmpty);
            var remains2 = remains.Except(results2).ToArray();

            if (remains2.Last() == string.Empty)
            {
                remains2 = remains2.Take(remains2.Length - 1).ToArray();
            }            
            return remains2;
        }

        private static bool FindComments(string obj)
        {
            var result = Regex.IsMatch(obj, "^(\n)*(/\\*)+");
            return result;
        }

        private static bool FindWhiteSpaces(string obj)
        {
            var result = Regex.IsMatch(obj, "^(\\s)+$");

            return result;
        }
        
    }
}
