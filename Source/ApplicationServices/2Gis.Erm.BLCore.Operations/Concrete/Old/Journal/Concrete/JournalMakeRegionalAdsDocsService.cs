using System.Linq;

using DoubleGis.Erm.BLCore.API.MoDi.PrintRegional;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Orders.PrintForms;
using DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Journal.Concrete
{
    public static class JournalMakeRegionalAdsDocsProperties
    {
        public const string MakeRegionalAdsDocsRequest = "MakeRegionalAdsDocsRequest";
        public const string MoneyDistributionResponse = "MoneyDistributionResponse";
    }

    // FIXME {all, 23.10.2013}: нужен рефакторинг - реализовать нормальный специфический контракт, т.к. нет никакого смысла, что то сливать в обобщенный контейнер данных, а потом оттуда выколупывать эти данные, ещё и используя строки как Keys
    public sealed class JournalMakeRegionalAdsDocsService : IJournalMakeRegionalAdsDocsService
    {
        private readonly IRepository<RegionalAdvertisingSharing> _regionalAdvertisingSharingRepository;
        private readonly IIdentityProvider _identityProvider;

        public JournalMakeRegionalAdsDocsService(IRepository<RegionalAdvertisingSharing> regionalAdvertisingSharingRepository, IIdentityProvider identityProvider)
        {
            _regionalAdvertisingSharingRepository = regionalAdvertisingSharingRepository;
            _identityProvider = identityProvider;
        }

        public void WriteJournalEntry(IPropertyBag propertyBag)
        {
            var request = (MakeRegionalAdsDocsRequest)propertyBag.GetProperty(JournalMakeRegionalAdsDocsProperties.MakeRegionalAdsDocsRequest);
            var response = (PrintRegionalOrdersResponse)propertyBag.GetProperty(JournalMakeRegionalAdsDocsProperties.MoneyDistributionResponse);
            if (request == null || response == null)
            {
                return;
            }

            foreach (var responseItem in response.Items.Where(x => x != null))
            {
                var regionalAdvertisingSharing = new RegionalAdvertisingSharing
                {
                    BeginDistributionDate = request.StartPeriodDate,

                    SourceOrganizationUnitId = responseItem.SourceOrganizationUnitId,
                    SourceBranchOfficeOrganizationUnitId = responseItem.SourceBranchOfficeOrganizationUnitId,

                    DestOrganizationUnitId = responseItem.DestOrganizationUnitId,
                    DestBranchOfficeOrganizationUnitId = responseItem.DestBranchOfficeOrganizationUnitId,

                    TotalAmount = responseItem.TotalAmount,
                };

                foreach (var orderId in responseItem.FirmWithOrders.SelectMany(x => x.OrderIds))
                {
                    // FIXME {all, 26.07.2013}: C POCO такая тема не катит 
                    var ordersRegionalAdvertisingSharing = new OrdersRegionalAdvertisingSharing
                    {
                        OrderId = orderId,
                        RegionalAdvertisingSharing = regionalAdvertisingSharing
                    };

                    _identityProvider.SetFor(ordersRegionalAdvertisingSharing);
                    regionalAdvertisingSharing.OrdersRegionalAdvertisingSharings.Add(ordersRegionalAdvertisingSharing);
                }

                _identityProvider.SetFor(regionalAdvertisingSharing);
                _regionalAdvertisingSharingRepository.Add(regionalAdvertisingSharing);
            }

            _regionalAdvertisingSharingRepository.Save();
        }
    }
}
