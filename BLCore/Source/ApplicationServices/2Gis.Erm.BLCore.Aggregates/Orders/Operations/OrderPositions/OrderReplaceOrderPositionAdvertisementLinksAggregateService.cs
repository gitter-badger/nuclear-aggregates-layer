using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.Operations.OrderPositions;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.OrderPosition;

using NuClear.Storage.Writings;

namespace DoubleGis.Erm.BLCore.Aggregates.Orders.Operations.OrderPositions
{
    public sealed class OrderReplaceOrderPositionAdvertisementLinksAggregateService : IOrderReplaceOrderPositionAdvertisementLinksAggregateService
    {
        private readonly IRepository<OrderPositionAdvertisement> _orderPositionAdvertisementRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public OrderReplaceOrderPositionAdvertisementLinksAggregateService(
            IRepository<OrderPositionAdvertisement> orderPositionAdvertisementRepository,
            IIdentityProvider identityProvider,
            IOperationScopeFactory scopeFactory)
        {
            _orderPositionAdvertisementRepository = orderPositionAdvertisementRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Replace(IEnumerable<OrderPositionAdvertisement> replacedLinks, IEnumerable<AdvertisementLinkDescriptor> newLinks)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ReplaceOrderPositionAdvertisementLinksIdentity>())
            {
                foreach (var replacedLink in replacedLinks)
                {
                    _orderPositionAdvertisementRepository.Delete(replacedLink);
                    scope.Deleted<OrderPositionAdvertisement>(replacedLink.Id);
                }

                foreach (var newLink in newLinks)
                {
                    var linkEntity = new OrderPositionAdvertisement
                                         {
                                             OrderPositionId = newLink.OrderPositionId,
                                             PositionId = newLink.PositionId,
                                             AdvertisementId = newLink.AdvertisementId,
                                             FirmAddressId = newLink.FirmAddressId,
                                             CategoryId = newLink.CategoryId,
                                             ThemeId = newLink.ThemeId
                                         };

                    _identityProvider.SetFor(linkEntity);
                    _orderPositionAdvertisementRepository.Add(linkEntity);
                    scope.Added<OrderPositionAdvertisement>(linkEntity.Id);
                }

                _orderPositionAdvertisementRepository.Save();
                scope.Complete();
            }
        }
    }
}
