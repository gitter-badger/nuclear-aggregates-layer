ALTER VIEW [BusinessDirectory].[vwTerritories]
AS
SELECT     Id, Name, OrganizationUnitId, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn
FROM         BusinessDirectory.Territories
WHERE     (IsActive = 1)
GO

ALTER view [Billing].[vwOrganizationUnits]
AS
Select Id, Name, Code, CreatedBy, CreatedOn, ModifiedBy, ModifiedOn from Billing.OrganizationUnits where IsActive=1 and IsDeleted=0
GO