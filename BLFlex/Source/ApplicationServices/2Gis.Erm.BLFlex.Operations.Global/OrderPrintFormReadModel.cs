using System.Linq;

using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLFlex.Operations.Global
{
    public sealed class OrderPrintFormReadModel : IOrderPrintFormReadModel
    {
        private readonly IFinder _finder;

        public OrderPrintFormReadModel(IFinder finder)
        {
            _finder = finder;
        }

        public OrderRelationsDto GetOrderRelationsDto(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => new OrderRelationsDto
                              {
                                  BranchOfficeOrganizationUnitId = order.BranchOfficeOrganizationUnitId,
                                  OrderNumber = order.Number,
                                  CurrencyIsoCode = order.Currency.ISOCode,
                                  LegalPersonId = order.LegalPersonId,
                                  SourceOrganizationUnitId = order.SourceOrganizationUnitId,
                                  DestOrganizationUnitId = order.DestOrganizationUnitId,
                                  FirmId = order.FirmId,
                                  LegalPersonProfileId = order.LegalPersonProfileId,
                                  BranchOfficeId = order.BranchOfficeOrganizationUnit.BranchOfficeId,
                                  IsOrderWithDiscount = order.DiscountSum.HasValue && order.DiscountSum.Value > 0
                              })
                          .Single();
        }

        public decimal GetOrderDicount(long orderId)
        {
            var discount = _finder.Find(Specs.Find.ById<Order>(orderId))
                                  .Select(order => order.DiscountSum)
                                  .Single();

            return discount.HasValue ? discount.Value : 0m;
        }

        public ContributionTypeEnum GetOrderContributionType(long organizationUnitId)
        {
            var contributionType = _finder.Find(Specs.Find.ById<OrganizationUnit>(organizationUnitId))
                                          .SelectMany(x => x.BranchOfficeOrganizationUnits)
                                          .Where(x => x.IsActive && !x.IsDeleted && x.IsPrimary)
                                          .Select(x => x.BranchOffice.ContributionTypeId)
                                          .SingleOrDefault();

            return contributionType.HasValue ? (ContributionTypeEnum)contributionType : ContributionTypeEnum.NotSet;
        }

        public IQueryable<Bill> GetBillQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(x => x.Bills)
                          .Where(Specs.Find.ActiveAndNotDeleted<Bill>())
                          .OrderBy(y => y.PaymentDatePlan);
        }

        public IQueryable<OrderPosition> GetOrderPositionQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(order => order.OrderPositions)
                          .Where(orderPosition => orderPosition.IsActive && !orderPosition.IsDeleted && orderPosition.PayablePrice != 0m)
                          .OrderBy(orderPosition => orderPosition.Id);
        }

        public IQueryable<Order> GetOrderQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId));
        }

        public IQueryable<FirmAddress> GetFirmAddressQuery(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .SelectMany(x => x.Firm.FirmAddresses)
                          .Where(Specs.Find.ActiveAndNotDeleted<FirmAddress>())
                          .Where(x => !x.ClosedForAscertainment)
                          .OrderBy(y => y.SortingPosition);
        }

        public IQueryable<BranchOffice> GetBranchOfficeQuery(long orderId)
        {
            // COMMENT {all, 13.05.2014}: Тут Partable не опасен
            // COMMENT {a.rechkalov, 21.05.2014}: Отдавая наружу IQueryable нельзя быть в этом уверенным
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => order.BranchOfficeOrganizationUnit.BranchOffice);
        }

        public Bargain GetOrderBargain(long orderId)
        {
            return _finder.Find(Specs.Find.ById<Order>(orderId))
                          .Select(order => order.Bargain)
                          .Single();
        }
    }
}
