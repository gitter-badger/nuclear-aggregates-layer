using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

using Microsoft.SqlServer.Management.Smo;

namespace DoubleGis.Erm.DB.Migration.Impl._0._18
{
    [Migration(7835, "Создание справочника проектов")]
    public sealed class Migration7835 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddProjectsTable(context);
            AddPrivelege(context);
        }

        private void AddProjectsTable(IMigrationContext context)
        {
            var table = context.Database.Tables[ErmTableNames.Projects.Name, ErmTableNames.Projects.Schema];
            if (table != null)
            {
                return;
            }

            table = new Table(context.Database, ErmTableNames.Projects.Name, ErmTableNames.Projects.Schema);

            table.Columns.Add(new Column(table, "Id", DataType.BigInt) { Nullable = false, Identity = true, IdentitySeed = 1, IdentityIncrement = 1 });
            table.CreateField("Code", DataType.Int, false);
            table.CreateField("OrganizationUnitId", DataType.BigInt, true);
            table.CreateField("NameLat", DataType.NVarChar(160), true); // 160 - это пальцем в небо; письмо в НК написал, чтобы уточнить размер
            table.CreateField("DisplayName", DataType.NVarChar(160), false);
            table.CreateField("DefaultLangId", DataType.Int, false);
            table.CreateField("IsActive", DataType.Bit, false);
            table.CreateField("CreatedBy", DataType.Int, false);
            table.CreateField("CreatedOn", DataType.DateTime2(2), false);
            table.CreateField("ModifiedBy", DataType.Int, true);
            table.CreateField("ModifiedOn", DataType.DateTime2(2), true);
            table.CreateField("Timestamp", DataType.Timestamp, true);

            table.Create();
            table.CreatePrimaryKey();
            table.CreateForeignKey("OrganizationUnitId", ErmTableNames.OrganizationUnits, "Id");
        }

        private void AddPrivelege(IMigrationContext context)
        {
            const int projectEntityCode = 158;
            const int readOperationCode = 1;
            const int editOperationCode = 2;
            const long readOperationId = 667;
            const long editOperationId = 668;

            const string query = @"if not exists(select * from [Security].[Privileges] where EntityType = {0} and Operation = {1}) 
                                   insert into [Security].[Privileges] (Id, EntityType, Operation) values ({3}, {0}, {1})
                                   if not exists(select * from [Security].[Privileges] where EntityType = {0} and Operation = {2}) 
                                   insert into [Security].[Privileges] (Id, EntityType, Operation) values ({4}, {0}, {2})";

            context.Connection.ExecuteNonQuery(string.Format(query, projectEntityCode, readOperationCode, editOperationCode, readOperationId, editOperationId));
        }
    }
}
