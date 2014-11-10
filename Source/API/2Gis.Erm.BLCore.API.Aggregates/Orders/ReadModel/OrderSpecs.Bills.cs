using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel
{
    public static partial class OrderSpecs
    {
        public static class Bills
        {
            public static class Select
            {
                public static ISelectSpecification<Bill, OrderPrintValidationDto> OrderPrintValidationDto()
                {
                    return new SelectSpecification<Bill, OrderPrintValidationDto>(
                        x => new OrderPrintValidationDto
                                 {
                                     LegalPersonId = x.Order.LegalPersonId,
                                     LegalPersonProfileId = x.Order.LegalPersonProfileId,
                                     BranchOfficeOrganizationUnitId = x.Order.BranchOfficeOrganizationUnitId,
                                 });
                }
            }
        }
    }
}