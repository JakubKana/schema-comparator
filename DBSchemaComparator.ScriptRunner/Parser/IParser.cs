using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Parser
{
    public interface IParser
    {
        bool ExecuteTransactionScript(string[] scriptArray, Database db);
        string[] GetScriptArray(string script);
    }
}