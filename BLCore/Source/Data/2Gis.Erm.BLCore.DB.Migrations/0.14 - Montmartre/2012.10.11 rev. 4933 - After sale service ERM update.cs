using System.Text;

using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(4933, "Изменения по ППС в базе ERM.")]
    public class Migration4933 : TransactedMigration
    {
        private const int PrivilegeId = 605;

        protected override void ApplyOverride(IMigrationContext context)
        {
            if (!context.Database.Tables.Contains(ErmTableNames.AfterSaleServiceActivities.Name, ErmTableNames.AfterSaleServiceActivities.Schema))
            {
                Table afterSaleTable = new Table(context.Database, 
                    ErmTableNames.AfterSaleServiceActivities.Name, 
                    ErmTableNames.AfterSaleServiceActivities.Schema);

                Column idColumn = new Column(afterSaleTable, "Id", DataType.Int) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 };
                afterSaleTable.Columns.Add(idColumn);
                afterSaleTable.Columns.Add(new Column(afterSaleTable, "DealId", DataType.Int) { Nullable = false });
                afterSaleTable.Columns.Add(new Column(afterSaleTable, "AfterSaleServiceType", DataType.TinyInt) { Nullable = false });
                afterSaleTable.Columns.Add(new Column(afterSaleTable, "AbsoluteMonthNumber", DataType.Int) { Nullable = false });

                afterSaleTable.Create();
                afterSaleTable.CreatePrimaryKey("Id");

                ForeignKey fk = new ForeignKey(afterSaleTable, "FK_AfterSaleServiceActivities_Deals");
                fk.ReferencedTable = ErmTableNames.Deals.Name;
                fk.ReferencedTableSchema = ErmTableNames.Deals.Schema;
                fk.Columns.Add(new ForeignKeyColumn(fk, "DealId", "Id"));

                afterSaleTable.ForeignKeys.Add(fk);
                fk.Create();
            }

            AddFunctionalPrivilege(context);
        }

        private void AddFunctionalPrivilege(IMigrationContext context)
        {
            var commandQuery = new StringBuilder();

            commandQuery.Append("SET IDENTITY_INSERT [Security].[Privileges]")
                .Append("ON INSERT INTO [Security].[Privileges]( [Id], [EntityType], [Operation]) ")
                .Append(string.Format("VALUES( {0}, NULL, {1}) ", PrivilegeId, PrivilegeId))

                .Append("SET IDENTITY_INSERT [Security].[Privileges] OFF INSERT INTO [Security].[FunctionalPrivilegeDepths] ")
                .Append("( [PrivilegeId], [LocalResourceName], [Priority], [Mask]) ")
                .Append(string.Format("VALUES ({0}, 'FPrvDpthGranted', 1, 139) ", PrivilegeId));

            context.Connection.ExecuteNonQuery(commandQuery.ToString());
        }
    }
}
