using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Settings;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCardsForERM.Processors
{
    public class ImportCardForErmService : IImportCardForErmService
    {
        private readonly IImportCardForErmAggregateService _importCardsForErmAggregateService;
        private readonly IIntegrationLocalizationSettings _integrationLocalizationSettings;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCardForErmService(IOperationScopeFactory scopeFactory,
                                       IImportCardForErmAggregateService importCardsForErmAggregateService,
                                       IIntegrationLocalizationSettings integrationLocalizationSettings)
        {
            _scopeFactory = scopeFactory;
            _importCardsForErmAggregateService = importCardsForErmAggregateService;
            _integrationLocalizationSettings = integrationLocalizationSettings;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cardForErmServiceBusDtos = dtos.Cast<CardForErmServiceBusDto>().ToArray();

            var posCards = cardForErmServiceBusDtos.Where(x => x.Type == CardType.Pos).ToArray();
            var depCards = cardForErmServiceBusDtos.Where(x => x.Type == CardType.Dep).ToArray();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardForErmIdentity>())
            {
                _importCardsForErmAggregateService.ImportDepCards(depCards.Select(x => x.ToImportDepCardDto()));

                foreach (var depCard in depCards)
                {
                    _importCardsForErmAggregateService.ImportFirmContacts(depCard.Code, depCard.Contacts.Select(x => x.ToImportFirmContactDto()), true);
                }

                _importCardsForErmAggregateService.ImportFirmAddresses(posCards.Select(x => x.ToImportFirmAddressDto()),
                                                                       _integrationLocalizationSettings.RegionalTerritoryLocaleSpecificWord);

                foreach (var posCard in posCards)
                {
                    _importCardsForErmAggregateService.ImportFirmContacts(posCard.Code, posCard.Contacts.Select(x => x.ToImportFirmContactDto()), false);
                    _importCardsForErmAggregateService.ImportCategoryFirmAddresses(posCard.Code, posCard.Rubrics.Select(x => x.ToCategoryFirmAddressDto()));
                }

                scope.Updated<Firm>(cardForErmServiceBusDtos.Select(x => x.FirmCode).Distinct()).Complete();
            }
        }
    }
}