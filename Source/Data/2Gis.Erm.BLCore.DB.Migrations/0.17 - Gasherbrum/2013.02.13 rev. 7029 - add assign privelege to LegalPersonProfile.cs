using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7029, "Добавляем сущностную привелегию смены куратора профилю юр. лица")]
    public sealed class Migration7029 : TransactedMigration
    {
        private const int LegalPersonEntityId = 147;
        private const int LegalPersonProfileEntityId = 219;
        private const int AssignOperationId = 524288;

        #region Текст запроса

        private const string Query = @"
DECLARE @LegalPersonEntityId int = {0} DECLARE @AssignProfilePrivelegeId int
IF(NOT EXISTS(SELECT * FROM [Security].[Privileges] where EntityType = {1} AND Operation = {2}))
 INSERT INTO [Security].[Privileges] (EntityType, Operation) VALUES ({1}, {2})
SET @AssignProfilePrivelegeId = (SELECT TOP 1 Id FROM [Security].[Privileges] where EntityType = {1} AND Operation = {2})
DECLARE @AssignLegalPersonPrivelegeId int
SET @AssignLegalPersonPrivelegeId = (SELECT TOP 1 Id FROM [Security].[Privileges] where EntityType = @LegalPersonEntityId AND Operation = {2})
IF(NOT EXISTS(SELECT * FROM [Security].[RolePrivileges] where PrivilegeId =  @AssignProfilePrivelegeId))
INSERT INTO [Security].RolePrivileges(PrivilegeId, RoleId,Mask, Priority, CreatedOn, CreatedBy)
SELECT @AssignProfilePrivelegeId, RoleId, Mask, 0, GETUTCDATE(), 1 FROM [Security].RolePrivileges 
WHERE PrivilegeId = @AssignLegalPersonPrivelegeId ";

        #endregion

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.ExecuteNonQuery(string.Format(Query, LegalPersonEntityId, LegalPersonProfileEntityId, AssignOperationId));
        }
    }
}
