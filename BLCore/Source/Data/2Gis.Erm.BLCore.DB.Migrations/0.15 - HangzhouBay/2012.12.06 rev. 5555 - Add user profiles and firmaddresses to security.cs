using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5555, "Добавляем в безопасность сущности UserProfile, FirmContact")]
    public sealed class Migration5555 : TransactedMigration
    {
        protected override void ApplyOverride(IMigrationContext context)
        {
            AddEntities(context);
        }

        private static void AddEntities(IMigrationContext context)
        {
            context.Database.ExecuteNonQuery(@"
DECLARE @EntityTypes TABLE (EntityType INT NOT NULL)
INSERT INTO @EntityTypes VALUES
(165), -- firm contact
(211) -- user profile

DECLARE @currentEntityType INT
SELECT @currentEntityType = MIN(EntityType) FROM @EntityTypes
WHILE @currentEntityType IS NOT NULL
BEGIN
	
	DECLARE @PrivilegeIds TABLE (Id INT NOT NULL)
	DECLARE @currentId INT

	-- read
	DELETE FROM @PrivilegeIds
	INSERT INTO Security.Privileges (EntityType, Operation) OUTPUT inserted.Id INTO @PrivilegeIds VALUES
	(@currentEntityType, 1)

	SELECT @currentId = MIN(Id) FROM @PrivilegeIds
	WHILE @currentId IS NOT NULL
	BEGIN
	
        -- assign privileges to roles
	    INSERT INTO Security.RolePrivileges (RoleId, PrivilegeId, Priority, Mask, CreatedBy, CreatedOn) VALUES
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Системный администратор'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Директор филиала'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Администратор по учету продаж'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Руководитель группы продаж'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Менеджер отдела продаж'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Настройщик системы'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Специалист отдела производства'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Специалист по маркетингу'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'АУП Москва'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Сервис-менеджер'), @currentId, 4, 16, 1, GETUTCDATE()),
	    ((SELECT Id FROM Security.Roles WHERE Name = 'Сотрудник УК'), @currentId, 4, 16, 1, GETUTCDATE())

		SELECT @currentId = MIN(Id) FROM @PrivilegeIds WHERE Id > @currentId
	END

	SELECT @currentEntityType = MIN(EntityType) FROM @EntityTypes WHERE EntityType > @currentEntityType
END");
        }
    }
}