using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Prices.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Prices;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Create;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Create
{
    public sealed class CreateDeniedPositionOperationService : ICreateOperationService<DeniedPosition>
    {        
        private readonly IDeniedPositionsDuplicatesVerifier _deniedPositionsDuplicatesVerifier;
        private readonly IBusinessModelEntityObtainer<DeniedPosition> _deniedPositionObtainer;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly ICreateDeniedPositionsAggregateService _createDeniedPositionsAggregateService;

        public CreateDeniedPositionOperationService(IDeniedPositionsDuplicatesVerifier deniedPositionsDuplicatesVerifier,
                                                    IBusinessModelEntityObtainer<DeniedPosition> deniedPositionObtainer,
                                                    IOperationScopeFactory operationScopeFactory,
                                                    ICreateDeniedPositionsAggregateService createDeniedPositionsAggregateService)
        {            
            _deniedPositionsDuplicatesVerifier = deniedPositionsDuplicatesVerifier;
            _deniedPositionObtainer = deniedPositionObtainer;
            _operationScopeFactory = operationScopeFactory;
            _createDeniedPositionsAggregateService = createDeniedPositionsAggregateService;
        }

        public long Create(IDomainEntityDto entityDto)
        {
            using (var scope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DeniedPosition>())
            {
                var deniedPosition = _deniedPositionObtainer.ObtainBusinessModelEntity(entityDto);
                _deniedPositionsDuplicatesVerifier.VerifyForDuplicates(deniedPosition.PositionId, deniedPosition.PositionDeniedId, deniedPosition.PriceId);

                _createDeniedPositionsAggregateService.Create(deniedPosition.PriceId,
                                                              deniedPosition.PositionId,
                                                              new[]
                                                                  {
                                                                      new DeniedPositionToCreateDto
                                                                          {
                                                                              ObjectBindingType = deniedPosition.ObjectBindingType,
                                                                              PositionDeniedId = deniedPosition.PositionDeniedId
                                                                          }
                                                                  });

                scope.Complete();

                return deniedPosition.Id;
            }
        }
    }
}