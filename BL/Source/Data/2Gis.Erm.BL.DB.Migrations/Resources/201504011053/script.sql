begin tran

-- Изменяю текущие стабильные индексы, чтобы соттветствовали бизнесу и было место для дефолтного третьего.
ALTER TABLE BusinessDirectory.CategoryOrganizationUnits NOCHECK CONSTRAINT FK_CategoryOrganizationUnits_CategoryGroups

update BusinessDirectory.CategoryGroups
set ModifiedBy = 1,
	ModifiedOn = GETUTCDATE(),
	Id = CASE WHEN Id = 1 THEN 5
		WHEN Id = 2 THEN 4
		WHEN Id = 3 THEN 2      
		WHEN Id = 4 THEN 1
	END
	
update BusinessDirectory.CategoryOrganizationUnits
set ModifiedBy = 1,
	ModifiedOn = GETUTCDATE(),
	CategoryGroupId = CASE WHEN CategoryGroupId = 1 THEN 5
		WHEN CategoryGroupId = 2 THEN 4
		WHEN CategoryGroupId = 3 THEN 2      
		WHEN CategoryGroupId = 4 THEN 1
	END

ALTER TABLE BusinessDirectory.CategoryOrganizationUnits CHECK CONSTRAINT FK_CategoryOrganizationUnits_CategoryGroups

-- Добавляю дефолтную (ранее) третью группу
insert into BusinessDirectory.CategoryGroups(Id, CategoryGroupName, GroupRate, IsActive, IsDeleted, OwnerCode, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn)
values(3, '100%', 1, 1, 0, 1, 1, GETUTCDATE(), 1, GETUTCDATE())

-- Рубрикам третьео уровня принудительно проставляю третью ценовую категорию
update CategoryOrganizationUnits
set CategoryGroupId = 3
from BusinessDirectory.CategoryOrganizationUnits
    inner join BusinessDirectory.Categories on Categories.Id = CategoryOrganizationUnits.CategoryId
where Categories.Level = 3
	and CategoryOrganizationUnits.CategoryGroupId is null

commit tran