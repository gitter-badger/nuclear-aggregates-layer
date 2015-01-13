using System;

using DoubleGis.Erm.BLCore.DB.Migrations.Properties;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;
using DoubleGis.Erm.Platform.Migration.MW;

namespace DoubleGis.Erm.BLCore.DB.Migrations._2._1
{
    [Migration(201412291720, "ERM-5524:Не отображаются созданные встречи в гриде Календаря","a.pashkin")]
    public class Migration201412291720:IContextedMigration<IActivityMigrationContext>
    {
        public void Apply(IActivityMigrationContext context)
        {
            try
            {
                context.Connection.BeginTransaction();
                var queryString = BuildSql(Resources.InsertMissingOrganizers_201412291720, context.CrmDatabaseName);
                context.Connection.ExecuteNonQuery(queryString);                
                context.Connection.CommitTransaction();
            }
            catch (Exception)
            {
                context.Connection.RollBackTransaction();
                throw;
            }
        }

        public void Revert(IActivityMigrationContext context)
        {
            throw new NotImplementedException();
        }

        private static string BuildSql(String script, String crmDbName)
        {
            return String.Format(script, crmDbName);
        }

       
    }
}
