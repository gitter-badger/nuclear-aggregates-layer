using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ModifyPhonecallService : IModifyBusinessModelEntityService<Phonecall>
    {
        private readonly IPhonecallReadModel _readModel;

        private readonly IBusinessModelEntityObtainer<Phonecall> _activityObtainer;

        private readonly IClientReadModel _clientReadModel;

        private readonly IFirmReadModel _firmReadModel;        

        private readonly ICreatePhonecallAggregateService _createOperationService;

        private readonly IUpdatePhonecallAggregateService _updateOperationService;

        public ModifyPhonecallService(
            IPhonecallReadModel readModel,
            IBusinessModelEntityObtainer<Phonecall> obtainer,
            IClientReadModel clientReadModel,
            IFirmReadModel firmReadModel,
            ICreatePhonecallAggregateService createOperationService,
            IUpdatePhonecallAggregateService updateOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;            
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var phonecallDto = (PhonecallDomainEntityDto)domainEntityDto;
            if (phonecallDto.RegardingObjects == null || !phonecallDto.RegardingObjects.Any())
            {
                throw new BusinessLogicException(BLResources.NoRegardingObjectValidationError);
            }

            var phonecall = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);
            
            if (phonecallDto.RegardingObjects.HasReferenceInReserve(EntityType.Instance.Client(), _clientReadModel.IsClientInReserve))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForClientInReserve);
            }

            if (phonecallDto.RegardingObjects.HasReferenceInReserve(EntityType.Instance.Firm(), _firmReadModel.IsFirmInReserve))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForFirmInReserve);
            }

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<PhonecallRegardingObject> oldRegardingObjects;
                PhonecallRecipient oldRecipient;
                if (phonecall.IsNew())
                {
                    _createOperationService.Create(phonecall);
                    oldRegardingObjects = null;
                    oldRecipient = null;
                }
                else
                {
                    _updateOperationService.Update(phonecall);
                    oldRegardingObjects = _readModel.GetRegardingObjects(phonecall.Id);
                    oldRecipient = _readModel.GetRecipient(phonecall.Id);
                }

                _updateOperationService.ChangeRegardingObjects(
                    phonecall,
                                                               oldRegardingObjects,
                                                               phonecall.ReferencesIfAny<Phonecall, PhonecallRegardingObject>(phonecallDto.RegardingObjects));
                _updateOperationService.ChangeRecipient(phonecall, oldRecipient, phonecall.ReferencesIfAny<Phonecall, PhonecallRecipient>(phonecallDto.RecipientRef));

                transaction.Complete();

                return phonecall.Id;
            }
        }

        /// <summary>
        /// Tries to update the related deal stage if any.
        /// </summary>
        /// <remarks>
        /// See the specs on https://confluence.2gis.ru/pages/viewpage.action?pageId=48464616.
        /// </remarks>
        private void UpdateDealStage(PhonecallDomainEntityDto appointmentDto)
        {
            var dealRef = appointmentDto.RegardingObjects.FirstOrDefault(x => x.EntityTypeId.Equals(EntityType.Instance.Deal().Id));
            if (dealRef == null || !dealRef.Id.HasValue)
            {
                return;
            }

            var dealId = dealRef.Id.Value;
            var purpose = appointmentDto.Purpose;

            var newDealStage = ConvertToStage(purpose);
            if (newDealStage == DealStage.None)
            {
                return;
            }

            _changeDealStageOperationService.Change(dealId, newDealStage);
        }

        private static DealStage ConvertToStage(PhonecallPurpose purpose)
        {
            switch (purpose)
            {
                case PhonecallPurpose.FirstCall:
                    return DealStage.CollectInformation;

                case PhonecallPurpose.ProductPresentation:
                case PhonecallPurpose.OpportunitiesPresentation:
                    return DealStage.HoldingProductPresentation;

                case PhonecallPurpose.OfferApproval:
                case PhonecallPurpose.DecisionApproval:
                    return DealStage.MatchAndSendProposition;

                default:
                    return DealStage.None;
            }
        }
    }
}