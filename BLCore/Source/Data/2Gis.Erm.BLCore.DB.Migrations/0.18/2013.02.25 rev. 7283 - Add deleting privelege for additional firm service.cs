using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(7283, "Добавляем разрешение на удаление для сущности AdditionalFirmServices")]
    public sealed class Migration7283 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddEntities(context);
        }

        private static void AddEntities(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
DECLARE @PrivilegeIds TABLE (Id INT NOT NULL)

INSERT INTO Security.Privileges (EntityType, Operation)
OUTPUT inserted.Id INTO @PrivilegeIds
VALUES (220, 65536) -- delete for AdditionalFirmService

declare @currentId int
SELECT @currentId = Id FROM @PrivilegeIds

-- assign privileges to roles
INSERT INTO Security.RolePrivileges (RoleId, PrivilegeId, Priority, Mask, CreatedBy, CreatedOn) VALUES
((SELECT Id FROM Security.Roles WHERE Name = 'Системный администратор'), @currentId, 4, 16, 1, GETUTCDATE())
");
        }
    }
}