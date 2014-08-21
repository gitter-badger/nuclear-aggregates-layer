using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ModifyPhonecallService : IModifyBusinessModelEntityService<Phonecall>
    {
        private readonly IActivityReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<Phonecall> _activityObtainer;
        private readonly ICreateAggregateRepository<Phonecall> _createOperationService;
        private readonly IUpdatePhonecallAggregateService _updateOperationService;
        private readonly IUpdateRegardingObjectAggregateService<Phonecall> _updateRegardingObjectService;

        public ModifyPhonecallService(
            IActivityReadModel readModel,
            IBusinessModelEntityObtainer<Phonecall> obtainer,
            ICreateAggregateRepository<Phonecall> createOperationService,
            IUpdatePhonecallAggregateService updateOperationService,
            IUpdateRegardingObjectAggregateService<Phonecall> updateRegardingObjectService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
            _updateRegardingObjectService = updateRegardingObjectService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var phonecallDto = (PhonecallDomainEntityDto)domainEntityDto;
            var phonecall = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            IEnumerable<RegardingObject<Phonecall>> oldRegardingObjects;
            if (phonecall.IsNew())
            {
                _createOperationService.Create(phonecall);
                oldRegardingObjects = null;
            }
            else
            {
                _updateOperationService.Update(phonecall);
                oldRegardingObjects = _readModel.GetRegardingObjects<Phonecall>(phonecall.Id);
            }

            _updateRegardingObjectService.ChangeRegardingObjects(
                oldRegardingObjects,
                new[]
                    {
                        phonecall.ReferenceIfAny(EntityName.Client, phonecallDto.ClientRef.Id),
                        phonecall.ReferenceIfAny(EntityName.Contact, phonecallDto.ContactRef.Id),
                        phonecall.ReferenceIfAny(EntityName.Deal, phonecallDto.DealRef.Id),
                        phonecall.ReferenceIfAny(EntityName.Firm, phonecallDto.FirmRef.Id)
                    }.Where(x => x != null));

            return phonecall.Id;
        }
    }
}