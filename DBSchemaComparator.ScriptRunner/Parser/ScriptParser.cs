using System;
using System.Linq;
using System.Text.RegularExpressions;
using DBSchemaComparator.Domain.Infrastructure;
using NLog;
using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Parser
{
    public class ScriptParser
    {
        private static readonly Logger Logger = LogManager.GetCurrentClassLogger();

        public bool ExecuteTransactionScript(string[] scriptArray, Database db)
        {

            db.EnableAutoSelect = false;
            try
            {
                Logger.Info("Executing transaction script.");
                db.BeginTransaction();

                //var script = "\ncreate procedure[dbo].[Companies_contact_by_Company_ID] @@Company_ID int as --------------------something------------------ select * from Companies_contact_view where Company_ID = @@Company_ID";
                // var result = Database.Execute(script);

                foreach (var s in scriptArray)
                {
                    Logger.Debug("Executing command", s);
                    
                    var script = Extensions.RemoveBeginingNewLine(Extensions.NormalizeParameters(s));
                    //var result = Database.Execute(script);
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
