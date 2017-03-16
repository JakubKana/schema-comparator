using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Deployment
{
    public interface IDeployment
    {
        bool CheckDatabaseExists(string databaseName);
        void CreateDatabase(Database db, string dbName);
    }
}