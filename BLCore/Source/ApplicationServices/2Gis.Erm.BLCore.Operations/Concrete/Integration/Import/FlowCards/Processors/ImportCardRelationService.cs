using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Import.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Firm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Import.FlowCards.Processors
{
    public class ImportCardRelationService : IImportCardRelationService
    {
        private readonly IFirmReadModel _firmReadModel;
        private readonly IBulkCreateCardRelationsAggregateService _bulkCreateCardRelationsAggregateService;
        private readonly IBulkUpdateCardRelationsAggregateService _bulkUpdateCardRelationsAggregateService;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCardRelationService(IFirmReadModel firmReadModel,
                                         IBulkCreateCardRelationsAggregateService bulkCreateCardRelationsAggregateService,
                                         IBulkUpdateCardRelationsAggregateService bulkUpdateCardRelationsAggregateService,
                                         IOperationScopeFactory scopeFactory)
        {
            _firmReadModel = firmReadModel;
            _bulkCreateCardRelationsAggregateService = bulkCreateCardRelationsAggregateService;
            _bulkUpdateCardRelationsAggregateService = bulkUpdateCardRelationsAggregateService;
            _scopeFactory = scopeFactory;
        }

        public void Import(IEnumerable<IServiceBusDto> dtos)
        {
            var cardRelationServiceBusDtos = dtos.Cast<CardRelationServiceBusDto>();

            using (var scope = _scopeFactory.CreateNonCoupled<ImportCardRelationIdentity>())
            {
                var existingCardRelations = _firmReadModel.GetCardRelationsByIds(cardRelationServiceBusDtos.Select(x => x.Code));
                var relationsToCreate = new List<CardRelation>();
                var relationsToUpdate = new List<CardRelation>();

                foreach (var cardRelationDto in cardRelationServiceBusDtos)
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