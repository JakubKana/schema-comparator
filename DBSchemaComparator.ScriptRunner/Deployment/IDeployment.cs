using PetaPoco;

namespace DBSchemaComparator.ScriptRunner.Deployment
{
    public interface IDeployment
    {
        bool CheckDatabaseExists(Database db, string databaseName);
        void CreateDatabase(Database db, string dbName);
        void DeleteDatabase(Database db, string dbName);
    }
}