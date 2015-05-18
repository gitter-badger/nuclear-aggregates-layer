using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel;
using DoubleGis.Erm.BL.API.Aggregates.SimplifiedModel.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.SimplifiedModel;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Entities.Aspects;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.Operations.Generic.Modify.SimplifiedModel
{
    public class ModifyDenialReasonOperationService : IModifySimplifiedModelEntityService<DenialReason>
    {
        private readonly ISimplifiedModelEntityObtainer<DenialReason> _obtainer;
        private readonly ICreateDenialReasonService _createService;
        private readonly IUpdateDenialReasonService _updateService;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IDenialReasonReadModel _denialReasonReadModel;

        public ModifyDenialReasonOperationService(ISimplifiedModelEntityObtainer<DenialReason> obtainer,
                                                  ICreateDenialReasonService createService,
                                                  IUpdateDenialReasonService updateService,
                                                  IOperationScopeFactory operationScopeFactory,
                                                  IDenialReasonReadModel denialReasonReadModel)
        {
            _obtainer = obtainer;
            _createService = createService;
            _updateService = updateService;
            _operationScopeFactory = operationScopeFactory;
            _denialReasonReadModel = denialReasonReadModel;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var dto = (DenialReasonDomainEntityDto)domainEntityDto;
            if (_denialReasonReadModel.IsThereDuplicateByName(dto.Id, dto.Name))
            {
                throw new DenialReasonNameIsNotUniqueException(string.Format(BLResources.DenialReasonNameIsNotUnique, dto.Name));
            }

            var entity = _obtainer.ObtainSimplifiedModelEntity(domainEntityDto);
            if (entity.IsNew())
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<CreateIdentity, DenialReason>())
                {
                    _createService.Create(entity);
                    operationScope.Added<DenialReason>(entity.Id);
                    operationScope.Complete();
                }
            }
            else
            {
                using (var operationScope = _operationScopeFactory.CreateSpecificFor<UpdateIdentity, DenialReason>())
                {
                    _updateService.Update(entity);
                    operationScope.Updated<DenialReason>(entity.Id);
                    operationScope.Complete();
                }
            }

            return entity.Id;
        }
    }
}