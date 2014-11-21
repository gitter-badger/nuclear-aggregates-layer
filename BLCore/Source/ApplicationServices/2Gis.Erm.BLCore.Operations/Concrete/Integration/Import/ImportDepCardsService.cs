using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import
{
    public class ImportDepCardsService : IImportDepCardsService
    {
        private readonly IBulkCreateDepCardAggregateService _createAggregateService;
        private readonly IFirmReadModel _firmReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IBulkUpdateDepCardAggregateService _updateAggregateService;

        public ImportDepCardsService(IBulkCreateDepCardAggregateService createAggregateService,
                                     IFirmReadModel firmReadModel,
                                     IBulkUpdateDepCardAggregateService updateAggregateService,
                                     IOperationScopeFactory scopeFactory)
        {
            _createAggregateService = createAggregateService;
            _firmReadModel = firmReadModel;
            _updateAggregateService = updateAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<DepCard> depCardsToImport)
        {
            using (var scope = _scopeFactory.CreateNonCoupled<ImportDepCardIdentity>())
            {
                var cardsToImportDictionary = depCardsToImport.ToDictionary(x => x.Id);
                var existingCardsDictionary = _firmReadModel.GetDepCards(cardsToImportDictionary.Keys);
                
                var idsToUpdate = cardsToImportDictionary.Keys.Intersect(existingCardsDictionary.Keys).ToArray();
                var idsToInsert = cardsToImportDictionary.Keys.Except(existingCardsDictionary.Keys).ToArray();

                if (idsToUpdate.Any())
                {
                    var depCardsToUpdate = new List<DepCard>();
                    foreach (var id in idsToUpdate)
                    {
                        var existingCard = existingCardsDictionary[id];
                        existingCard.IsHiddenOrArchived = cardsToImportDictionary[id].IsHiddenOrArchived;

                        depCardsToUpdate.Add(existingCard);
                    }

                    _updateAggregateService.Update(depCardsToUpdate);
                }

                if (idsToInsert.Any())
                {
                    var depCardsToCreate =
                        idsToInsert.Select(id => new DepCard { Id = id, IsHiddenOrArchived = cardsToImportDictionary[id].IsHiddenOrArchived })
                                   .ToArray();

                    _createAggregateService.Create(depCardsToCreate);
                }

                scope.Complete();
            }
        }
    }
}