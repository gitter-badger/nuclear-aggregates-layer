using System.Collections.Generic;
using System.Linq;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Deals;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

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
        private readonly IChangeDealStageOperationService _changeDealStageOperationService;

        public ModifyPhonecallService(
            IPhonecallReadModel readModel,
            IBusinessModelEntityObtainer<Phonecall> obtainer,
            IClientReadModel clientReadModel,
            IFirmReadModel firmReadModel,
            ICreatePhonecallAggregateService createOperationService,
            IUpdatePhonecallAggregateService updateOperationService,
            IChangeDealStageOperationService changeDealStageOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _clientReadModel = clientReadModel;
            _firmReadModel = firmReadModel;            
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
            _changeDealStageOperationService = changeDealStageOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var phonecallDto = (PhonecallDomainEntityDto)domainEntityDto;
            var phonecall = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            if (_firmReadModel.IsAnyReferencedFirmInReserve(phonecallDto.RegardingObjects))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForFirmInReserve);
            }

            if (_clientReadModel.IsAnyReferencedClientInReserve(phonecallDto.RegardingObjects))
            {
                throw new BusinessLogicException(BLResources.CannotSaveActivityForClientInReserve);
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

                _updateOperationService.ChangeRegardingObjects(phonecall,
                                                               oldRegardingObjects,
                                                               phonecall.ReferencesIfAny<Phonecall, PhonecallRegardingObject>(phonecallDto.RegardingObjects));
                _updateOperationService.ChangeRecipient(phonecall, oldRecipient, phonecall.ReferencesIfAny<Phonecall, PhonecallRecipient>(phonecallDto.RecipientRef));

                if (phonecall.Status == ActivityStatus.Completed)
                {
                    UpdateDealStage(phonecallDto);
                }

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
            var dealRef = appointmentDto.RegardingObjects.FirstOrDefault(x => x.EntityName == EntityName.Deal);
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

        private static DealStage ConvertToStage(ActivityPurpose purpose)
        {
            switch (purpose)
            {
                case ActivityPurpose.FirstCall:
                    return DealStage.CollectInformation;

                case ActivityPurpose.ProductPresentation:
                case ActivityPurpose.OpportunitiesPresentation:
                    return DealStage.HoldingProductPresentation;

                case ActivityPurpose.OfferApproval:
                case ActivityPurpose.DecisionApproval:
                    return DealStage.MatchAndSendProposition;

                default:
                    return DealStage.None;
            }
        }
    }
}