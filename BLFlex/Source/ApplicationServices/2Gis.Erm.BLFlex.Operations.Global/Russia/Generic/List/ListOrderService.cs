using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Russia.Orders;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.DTO;
using DoubleGis.Erm.BLQuerying.API.Operations.Listing.List.Metadata;
using DoubleGis.Erm.BLQuerying.Operations.Listing.List.Infrastructure;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

using NuClear.Model.Common.Entities;
using NuClear.Storage;
using NuClear.Storage.Specifications;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.List
{
    public sealed class ListOrderService : ListEntityDtoServiceBase<Order, ListOrderDto>, IRussiaAdapted
    {
        private readonly ISecurityServiceUserIdentifier _userIdentifierService;
        private readonly IQuery _query;
        private readonly FilterHelper _filterHelper;

        public ListOrderService(ISecurityServiceUserIdentifier userIdentifierService,
                                IQuery query,
                                FilterHelper filterHelper)
        {
            _userIdentifierService = userIdentifierService;
            _query = query;
            _filterHelper = filterHelper;
        }

        protected override IRemoteCollection List(QuerySettings querySettings)
        {
            var query = _query.For<Order>();

            bool forSubordinates;
            if (querySettings.TryGetExtendedProperty("ForSubordinates", out forSubordinates))
            {
                query = _filterHelper.ForSubordinates(query);
            }

            var selectExpression = OrderSpecifications.Select.OrdersForGridView();

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

                        // ДгппИд элемента номенклатуры "пакет "Дополнительный"" нужен для костыля-исключения на 2+2 месяца (до Июля)
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

                        var onApprovalState = OrderState.OnApproval.ToString();
                        var rejectedState = OrderState.Rejected.ToString();

                        var orderTypeId = EntityType.Instance.Order().Id;
                        var loqQuery = _query.For<ActionsHistoryDetail>().Where(x => x.ActionsHistory.EntityType == orderTypeId &&
                                                                                     x.PropertyName == "WorkflowStepId" &&
                                                                                     x.OriginalValue == onApprovalState &&
                                                                                     x.ModifiedValue == rejectedState)
                                             .Select(x => x.ActionsHistory.EntityId);

                        return x => loqQuery.Contains(x.Id);
                    });

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.Client()))
            {
                return query.Where(x => x.Firm.ClientId == querySettings.ParentEntityId || x.LegalPerson.ClientId == querySettings.ParentEntityId)
                            .Filter(_filterHelper, withoutAdvertisementFilter, dummyAdvertisementsFilter, rejectedByMeFilter)
                            .Select(selectExpression)
                            .Distinct()
                            .QuerySettings(_filterHelper, querySettings);
            }

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.LegalPerson()))
            {
                return query.Where(x => x.LegalPersonId == querySettings.ParentEntityId)
                            .Filter(_filterHelper, withoutAdvertisementFilter, dummyAdvertisementsFilter, rejectedByMeFilter)
                            .Select(selectExpression)
                            .Distinct()
                            .QuerySettings(_filterHelper, querySettings);
            }

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.Account()))
            {
                return query.Where(x => x.AccountId == querySettings.ParentEntityId)
                            .Filter(_filterHelper, withoutAdvertisementFilter, dummyAdvertisementsFilter, rejectedByMeFilter)
                            .Select(selectExpression)
                            .Distinct()
                            .QuerySettings(_filterHelper, querySettings);
            }

            if (querySettings.ParentEntityName.Equals(EntityType.Instance.Firm()))
            {
                return query.Where(x => x.FirmId == querySettings.ParentEntityId)
                            .Filter(_filterHelper, withoutAdvertisementFilter, dummyAdvertisementsFilter, rejectedByMeFilter)
                            .Select(selectExpression)
                            .Distinct()
                            .QuerySettings(_filterHelper, querySettings);
            }

            return query.Filter(_filterHelper, withoutAdvertisementFilter, dummyAdvertisementsFilter, rejectedByMeFilter)
                        .Select(selectExpression)
                        .Distinct()
                        .QuerySettings(_filterHelper, querySettings);
        }

        protected override void Transform(ListOrderDto dto)
        {
            dto.OwnerName = _userIdentifierService.GetUserInfo(dto.OwnerCode).DisplayName;           
        }
    }
}