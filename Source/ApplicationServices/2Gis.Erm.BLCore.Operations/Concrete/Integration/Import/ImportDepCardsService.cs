using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportDepCardsService : IImportDepCardsService
    {
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IFirmReadModel _firmReadModel;
        private readonly ICreateAggregateRepository<DepCard> _createAggregateService;
        private readonly IUpdateAggregateRepository<DepCard> _updateAggregateService;

        public ImportDepCardsService(IOperationScopeFactory scopeFactory,
                                     IFirmReadModel firmReadModel,
                                     ICreateAggregateRepository<DepCard> createAggregateService,
                                     IUpdateAggregateRepository<DepCard> updateAggregateService)
        {
            _scopeFactory = scopeFactory;
            _firmReadModel = firmReadModel;
            _createAggregateService = createAggregateService;
            _updateAggregateService = updateAggregateService;
        }

        public void Import(IEnumerable<DepCard> depCardsToImport)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ImportDepCardIdentity>())
            {
                var existingCardsDictionary = _firmReadModel.GetDepCards(depCardsToImport.Select(x => x.Id));
                var cardsToImportDictionary = depCardsToImport.ToDictionary(x => x.Id);

                var idsToUpdate = cardsToImportDictionary.Keys.Intersect(existingCardsDictionary.Keys).ToArray();
                var idsToInsert = cardsToImportDictionary.Keys.Except(existingCardsDictionary.Keys).ToArray();

                if (idsToUpdate.Any())
                {
                    foreach (var id in idsToUpdate)
                    {
                        var existingCard = existingCardsDictionary[id];
                        existingCard.IsHiddenOrArchived = cardsToImportDictionary[id].IsHiddenOrArchived;
                        _updateAggregateService.Update(existingCard);
                        scope.Updated<DepCard>(id);
                    }
                }

                if (idsToInsert.Any())
                {
                    foreach (var id in idsToInsert)
                    {
                        var newCard = new DepCard { Id = id, IsHiddenOrArchived = cardsToImportDictionary[id].IsHiddenOrArchived };
                        _createAggregateService.Create(newCard);
                        scope.Added<DepCard>(id);
                    }
                }

                scope.Complete();
            }
        }
    }
}