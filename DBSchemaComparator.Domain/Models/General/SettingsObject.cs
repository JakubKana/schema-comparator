using System.Collections.Generic;

namespace DBSchemaComparator.Domain.Models.General
{
    public class SettingsObject
    {
        public List<DatabaseConnection> DatabaseConnections { get; set; }
        public string ResultPath { get; set; }
    }
}
