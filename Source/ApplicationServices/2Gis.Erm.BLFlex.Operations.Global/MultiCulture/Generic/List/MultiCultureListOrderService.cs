using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Multiculture.Orders;
using DoubleGis.Erm.BLFlex.API.Operations.Global.MultiCulture.Operations.Generic.List;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.MultiCulture.Generic.List
{
    public sealed class MultiCultureListOrderService : ListEntityDtoServiceBase<Order, MultiCultureListOrderDto>, ICzechAdapted, ICyprusAdapted, IChileAdapted, IUkraineAdapted, IEmiratesAdapted, IKazakhstanAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IFinder _finder;
        private readonly FilterHelper _filterHelper;

        public MultiCultureListOrderService(ISecurityServiceUserIdentifier userIdentifierService,
                                      IFinder finder,
                                      FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _finder = finder;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _finder.FindAll<Order>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var selectExpression = OrderSpecifications.Select.OrdersForMulticultureGridView().Selector;

            var dummyAdvertisementsFilter = querySettings.CreateForExtendedProperty<Order, bool>(
                "WithDummyValues",
                withDummyValues =>
                {
                    if (!withDummyValues)
                    {
                        return null;
                    }

                    var dummyAdvertisementIds =
                        query.SelectMany(
                            x =>
                            x.OrderPositions.Where(y => !x.IsDeleted && x.IsActive)
                                .SelectMany(y => y.OrderPositionAdvertisements.Select(z => z.Advertisement.AdvertisementTemplate.DummyAdvertisementId)))
                            .Where(x => x != null)
                            .Distinct()
                            .ToArray();

                    return
                        x =>
                        x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                            .Any(y => y.OrderPositionAdvertisements.Any(z => dummyAdvertisementIds.Contains(z.AdvertisementId)));
                });

            var withoutAdvertisementFilter = querySettings.CreateForExtendedProperty<Order, bool>(
                "WithoutAdvertisement",
                withoutAdvertisement =>
                {
                    if (!withoutAdvertisement)
                    {
                        return null;
                    }

                    // ������ �������� ������������ "����� "��������������"" ����� ��� �������-���������� �� 2+2 ������ (�� ����)
                    const int additionalPackageDgppId = 11572;

                    return x => x.OrderPositions.Where(y => y.IsActive && !y.IsDeleted)
                                    .Select(z => new
                                    {
                                        RequiredPositionFails =
                                                 new[] { z.PricePosition.Position }
                                                 .Union(z.PricePosition.Position.ChildPositions
                                                            .Where(w => w.IsActive && !w.IsDeleted)
                                                            .Select(w => w.ChildPosition))
                                                 .Where(w => w.AdvertisementTemplate.IsAdvertisementRequired)
                                                 .Select(w => new
                                                 {
                                                     OpaIsEmpty = z.OrderPositionAdvertisements.All(p => p.PositionId != w.Id)
                                                                  &&
                                                                  z.PricePosition.Position.ChildPositions.All(
                                                                      m => m.MasterPosition.DgppId != additionalPackageDgppId),

                                                     AdvertisementIsRequired = z.OrderPositionAdvertisements.Where(p => p.PositionId == z.Id)
                                                              .Any(p => p.AdvertisementId == null),
                                                 })
                                    })
                                    .Any(y => y.RequiredPositionFails.Any(z => z.OpaIsEmpty || z.AdvertisementIsRequired));
                });

            var rejectedByMeFilter = querySettings.CreateForExtendedProperty<Order, bool>(
                "RejectedByMe",
                rejectedByMe =>
                {
                    if (!rejectedByMe)
                    {
                        return null;
                    }

                    var onApprovalState = ((int)OrderState.OnApproval).ToString();
                    var rejectedState = ((int)OrderState.Rejected).ToString();

                    var loqQuery = _finder.Find<ActionsHistoryDetail>(
                        x =>
                        x.ActionsHistory.EntityType == (int)EntityName.Order && x.PropertyName == "WorkflowStepId" &&
                        x.OriginalValue == onApprovalState && x.ModifiedValue == rejectedState)
                        .Select(x => x.ActionsHistory.EntityId);

                    return x => loqQuery.Contains(x.Id);
                });

            switch (querySettings.ParentEntityName)
            {
                case EntityName.Client:
                    return query
                        .Where(x => x.Firm.ClientId == querySettings.ParentEntityId || x.LegalPerson.ClientId == querySettings.ParentEntityId)
                        .Filter(_filterHelper
                        , withoutAdvertisementFilter
                        , dummyAdvertisementsFilter
                        , rejectedByMeFilter)
                        .Select(selectExpression)
                        .Distinct()
                        .QuerySettings(_filterHelper, querySettings);
                case EntityName.LegalPerson:
                    return query
                        .Where(x => x.LegalPersonId == querySettings.ParentEntityId)
                        .Filter(_filterHelper
                        , withoutAdvertisementFilter
                        , dummyAdvertisementsFilter
                        , rejectedByMeFilter)
                        .Select(selectExpression)
                        .Distinct()
                        .QuerySettings(_filterHelper, querySettings);
                case EntityName.Account:
                    return query
                        .Where(x => x.AccountId == querySettings.ParentEntityId)
                        .Filter(_filterHelper
                        , withoutAdvertisementFilter
                        , dummyAdvertisementsFilter
                        , rejectedByMeFilter)
                        .Select(selectExpression)
                        .Distinct()
                        .QuerySettings(_filterHelper, querySettings);
                case EntityName.Firm:
                    return query
                        .Where(x => x.FirmId == querySettings.ParentEntityId)
                        .Filter(_filterHelper
                        , withoutAdvertisementFilter
                        , dummyAdvertisementsFilter
                        , rejectedByMeFilter)
                        .Select(selectExpression)
                        .Distinct()
                        .QuerySettings(_filterHelper, querySettings);
                default:
                    return query
                        .Filter(_filterHelper
                        , withoutAdvertisementFilter
                        , dummyAdvertisementsFilter
                        , rejectedByMeFilter)
                        .Select(selectExpression)
                        .Distinct()
                        .QuerySettings(_filterHelper, querySettings);
            }
        }

        protected override void Transform(MultiCultureListOrderDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;
        }
    }
}