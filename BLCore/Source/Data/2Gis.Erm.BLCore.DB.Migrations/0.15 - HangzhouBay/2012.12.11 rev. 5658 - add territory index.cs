using DoubleGis.Erm.BLCore.DB.Migrations.Shared;
using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5658, "Добавляем индекс на таблицу Territories")]
    public sealed class Migration5658 : TransactedMigration
    {
        const string IndexName = "IX_Territories_IsDeleted_IsActive_Incl_Id_DgppId_Name_OrgId";
        protected override void ApplyOverride(IMigrationContext context)
        {

            #region Текст запроса

            var query = @"CREATE NONCLUSTERED INDEX [IX_Territories_IsActive_Incl_Id_DgppId_Name_OrgId] ON [BusinessDirectory].[Territories] 
( 
[IsActive] ASC 
) 
INCLUDE ( [Id], 
[DgppId], 
[Name], 
[OrganizationUnitId]) WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, DROP_EXISTING = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO 
";
           
            #endregion

            var table = context.Database.Tables["Territories", ErmSchemas.BusinessDirectory];

            var index = table.Indexes[IndexName];
            if (index != null)
            {
                index.Drop();
            }

            context.Connection.ExecuteNonQuery(query);
        }
    }
}
