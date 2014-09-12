using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.CardsForErm;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Processors
{
    public class ImportCardRelationForErmService : IImportCardRelationForErmService
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IOperationScopeFactory _scopeFactory;
        private readonly IBulkCreateCardRelationsAggregateService _bulkCreateCardRelationsAggregateService;
        private readonly IBulkUpdateCardRelationsAggregateService _bulkUpdateCardRelationsAggregateService;

        public ImportCardRelationForErmService(IFirmReadModel firmReadModel,
                                               IOperationScopeFactory scopeFactory,
                                               IBulkCreateCardRelationsAggregateService bulkCreateCardRelationsAggregateService,
                                               IBulkUpdateCardRelationsAggregateService bulkUpdateCardRelationsAggregateService)
        {
            _firmReadModel = firmReadModel;
            _scopeFactory = scopeFactory;
            _bulkCreateCardRelationsAggregateService = bulkCreateCardRelationsAggregateService;
            _bulkUpdateCardRelationsAggregateService = bulkUpdateCardRelationsAggregateService;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cardRelationForErmServiceBusDtos = dtos.Cast<CardRelationForErmServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardRelationForErmIdentity>())
            {
                var existingCardRelations = _firmReadModel.GetCardRelationsByIds(cardRelationForErmServiceBusDtos.Select(x => x.Code));
                var relationsToCreate = new List<CardRelation>();
                var relationsToUpdate = new List<CardRelation>();

                foreach (var cardRelationDto in cardRelationForErmServiceBusDtos)
                {
                    CardRelation targetRelation;
                    if (existingCardRelations.TryGetValue(cardRelationDto.Code, out targetRelation))
                    {
                        relationsToUpdate.Add(targetRelation);
                    }
                    else
                    {
                        targetRelation = new CardRelation
                                             {
                                                 Id = cardRelationDto.Code
                                             };

                        relationsToCreate.Add(targetRelation);
                    }

                    targetRelation.DepCardCode = cardRelationDto.Card2Code;
                    targetRelation.PosCardCode = cardRelationDto.Card1Code;
                    // в erm для простоты sorting position должен начинаться с 1
                    targetRelation.OrderNo = cardRelationDto.OrderNo + 1;
                    targetRelation.IsDeleted = cardRelationDto.IsDeleted;
                }

                if (relationsToCreate.Any())
                {
                    _bulkCreateCardRelationsAggregateService.Create(relationsToCreate);
                }

                if (relationsToUpdate.Any())
                {
                    _bulkUpdateCardRelationsAggregateService.Update(relationsToUpdate);
                }

                scope.Complete();
            }
        }
    }
}