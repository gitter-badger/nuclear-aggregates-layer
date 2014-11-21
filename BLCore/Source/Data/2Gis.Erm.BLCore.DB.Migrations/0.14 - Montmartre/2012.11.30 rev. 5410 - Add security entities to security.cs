using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(5410, "Добавляем в безопасность сущности User, Role, Department")]
    public sealed class Migration5410 : TransactedMigration
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
(51), -- department
(53), -- user
(54) -- role

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

    -- all roles can read
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

	-- other privileges
	DELETE FROM @PrivilegeIds
	INSERT INTO Security.Privileges (EntityType, Operation) OUTPUT inserted.Id INTO @PrivilegeIds VALUES
	(@currentEntityType, 2),
	(@currentEntityType, 4),
	(@currentEntityType, 16),
	(@currentEntityType, 32),
	(@currentEntityType, 65536),
	(@currentEntityType, 262144),
	(@currentEntityType, 524288)

	SELECT @currentId = MIN(Id) FROM @PrivilegeIds
	WHILE @currentId IS NOT NULL
	BEGIN
	
        -- 3 roles can do other things
		INSERT INTO Security.RolePrivileges (RoleId, PrivilegeId, Priority, Mask, CreatedBy, CreatedOn) VALUES
		((SELECT Id FROM Security.Roles WHERE Name = 'Системный администратор'), @currentId, 4, 16, 1, GETUTCDATE()),
		((SELECT Id FROM Security.Roles WHERE Name = 'Настройщик системы'), @currentId, 4, 16, 1, GETUTCDATE()),
		((SELECT Id FROM Security.Roles WHERE Name = 'Сотрудник УК'), @currentId, 4, 16, 1, GETUTCDATE())

		SELECT @currentId = MIN(Id) FROM @PrivilegeIds WHERE Id > @currentId
	END

	SELECT @currentEntityType = MIN(EntityType) FROM @EntityTypes WHERE EntityType > @currentEntityType
END");
        }
    }
}