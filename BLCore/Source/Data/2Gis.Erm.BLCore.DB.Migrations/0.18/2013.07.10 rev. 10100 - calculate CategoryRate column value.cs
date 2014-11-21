using DoubleGis.Erm.Platform.Migration.Base;
using DoubleGis.Erm.Platform.Migration.Core;

namespace DoubleGis.Erm.DB.Migration.Impl
{
    [Migration(10100, "Расчёт CategoryRate для существующих заказов")]
    public class Migration10100 : TransactedMigration
    {
        private const string CreateFuntions = @"
CREATE FUNCTION dbo.GetFirmCategoryRate
(
	@firmId bigint,
	@organizationUnitId bigint
)
RETURNS decimal(19, 4)
AS
BEGIN
	DECLARE @result decimal(19, 4)

	select @result = max(CategoryGroups.GroupRate)
	from BusinessDirectory.FirmAddresses
		inner join BusinessDirectory.CategoryFirmAddresses on CategoryFirmAddresses.FirmAddressId = FirmAddresses.Id
		inner join BusinessDirectory.Categories on Categories.Id = CategoryFirmAddresses.CategoryId
		inner join BusinessDirectory.CategoryOrganizationUnits on CategoryOrganizationUnits.CategoryId = Categories.Id
		inner join BusinessDirectory.CategoryGroups on CategoryGroups.Id = CategoryOrganizationUnits.CategoryGroupId
	where FirmAddresses.FirmId = @firmId
		and CategoryOrganizationUnits.OrganizationUnitId = @organizationUnitId
		and CategoryGroups.IsActive = 1 and CategoryGroups.IsDeleted = 0
		and CategoryOrganizationUnits.IsActive = 1 and CategoryOrganizationUnits.IsDeleted = 0
		and Categories.IsActive = 1 and Categories.IsDeleted = 0
		and CategoryFirmAddresses.IsActive = 1 and CategoryFirmAddresses.IsDeleted = 0
		and FirmAddresses.IsActive = 1 and FirmAddresses.IsDeleted = 0

	if @result is not null RETURN @result
	RETURN 1
END
GO

CREATE FUNCTION dbo.GetOrderPositionCategoryRate
(
	@orderId bigint,
	@pricePositionId bigint
)
RETURNS decimal(19, 4)
AS
BEGIN
	DECLARE @applyCategoryRate bit, @destOrganizationUnitId int, @firmId int

	select @applyCategoryRate = PricePositions.RatePricePositions from Billing.PricePositions where PricePositions.Id = @pricePositionId

	if @applyCategoryRate = 1
	begin 
		select @destOrganizationUnitId = Orders.DestOrganizationUnitId, @firmId = Orders.FirmId from Billing.Orders where Orders.Id = @orderId
		return dbo.GetFirmCategoryRate(@firmId, @destOrganizationUnitId)
	end
	
	return 1
END
GO
";

        private const string DropFuntions = @"
DROP FUNCTION dbo.GetOrderPositionCategoryRate
GO

DROP FUNCTION dbo.GetFirmCategoryRate
GO
";

        private const string CalculateOrderPositionRates = @"
update Billing.OrderPositions
set CategoryRate = dbo.GetOrderPositionCategoryRate(OrderId, OrderPositions.PricePositionId)
go
";

        protected override void ApplyOverride(IMigrationContext context)
        {
            context.Connection.StatementTimeout = 600;

            context.Connection.ExecuteNonQuery(CreateFuntions);
            context.Connection.ExecuteNonQuery(CalculateOrderPositionRates);
            context.Connection.ExecuteNonQuery(DropFuntions);
        }
    }
}
