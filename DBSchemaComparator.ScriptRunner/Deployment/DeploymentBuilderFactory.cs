using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using DBSchemaComparator.Domain.Database;
using DBSchemaComparator.Domain.Infrastructure;
using DBSchemaComparator.Domain.SqlBuilder;

namespace DBSchemaComparator.ScriptRunner.Deployment
{
    public class DeploymentBuilderFactory
    {

        public static IDeployment Create(DatabaseType dbType)
        {
            switch (dbType)
            {
                case DatabaseType.SqlServer:
                    return new MsSqlDeploy();
                case DatabaseType.MySql:
                    return new MySqlDeploy();
                case DatabaseType.Unsupported:
                    throw new NotSupportedException();
                default:
                    throw new NotSupportedException();
            }
        }
    }
}
