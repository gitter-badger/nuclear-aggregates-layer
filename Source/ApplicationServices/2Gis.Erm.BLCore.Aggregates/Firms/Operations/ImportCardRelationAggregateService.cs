using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Cards;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Firms.Operations
{
    public class ImportCardRelationAggregateService : IImportCardRelationAggregateService
    {
        private readonly IRepository<CardRelation> _cardRelationGenericRepository;
        private readonly IFinder _finder;
        private readonly IOperationScopeFactory _scopeFactory;

        public ImportCardRelationAggregateService(IRepository<CardRelation> cardRelationGenericRepository, IFinder finder, IOperationScopeFactory scopeFactory)
        {
            _cardRelationGenericRepository = cardRelationGenericRepository;
            _finder = finder;
            _scopeFactory = scopeFactory;
        }

        public void ImportCardRelations(IEnumerable<CardRelationServiceBusDto> cardRelationServiceBusDtos)
        {
            foreach (var cardRelationDto in cardRelationServiceBusDtos)
            {
                ProcessCardRelationDto(cardRelationDto);
            }

            _cardRelationGenericRepository.Save();
        }

        private void ProcessCardRelationDto(CardRelationServiceBusDto cardRelationDto)
        {
            // create or update card relation
            var objectExists = true;
            var cardRelation = _finder.Find<CardRelation>(x => x.Id == cardRelationDto.Code).SingleOrDefault();
            if (cardRelation == null)
            {
                cardRelation = new CardRelation { Id = cardRelationDto.Code };
                objectExists = false;
            }

            cardRelation.DepCardCode = cardRelationDto.DepartmentCardCode;
            cardRelation.PosCardCode = cardRelationDto.PointOfServiceCardCode;
            cardRelation.OrderNo = cardRelationDto.DepartmentCardSortingPosition;
            cardRelation.IsDeleted = cardRelationDto.IsDeleted;

            if (objectExists)
            {
                using (var scope = _scopeFactory.CreateSpecificFor<UpdateIdentity, CardRelation>())
                {
                    _cardRelationGenericRepository.Update(cardRelation);
                    scope.Updated<CardRelation>(cardRelation.Id)
                         .Complete();
                }
            }
            else
            {
                using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, CardRelation>())
                {
                    _cardRelationGenericRepository.Add(cardRelation);
                    scope.Added<CardRelation>(cardRelation.Id)
                         .Complete();
                }
            }
        }
    }
}