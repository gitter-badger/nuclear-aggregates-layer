using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7146, "Помечаем фирмы на репликацию в CRM.")]
    public class Migration7146 : TransactedMigration, INonDefaultDatabaseMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            string query =
@"
DECLARE @FirmsToReplicate Table (Id int not null)
INSERT INTO @FirmsToReplicate
SELECT F.Id from 
[{0}].[BusinessDirectory].[Firms] F
INNER JOIN [{0}].[BusinessDirectory].[Territories] T ON F.TerritoryId = T.Id
			INNER JOIN [dbo].[Dg_firmExtensionBase] MF ON F.ReplicationCode=MF.[Dg_firmId]
			WHERE MF.[Dg_territory] <> T.ReplicationCode

UPDATE [{0}].[BusinessDirectory].[Firms] SET ModifiedOn = GETUTCDATE() WHERE Id in (SELECT Id FROM @FirmsToReplicate)";

            query = string.Format(query, context.ErmDatabaseName);

            context.Connection.ExecuteNonQuery(query);
        }

        public ErmConnectionStringKey ConnectionStringKey
        {
            get { return ErmConnectionStringKey.CrmDatabase; }
        }
    }
}
