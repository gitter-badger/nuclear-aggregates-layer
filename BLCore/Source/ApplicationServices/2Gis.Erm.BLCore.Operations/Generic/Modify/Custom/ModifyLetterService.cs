using System.Collections.Generic;
using System.Transactions;

using DoubleGis.Erm.BLCore.API.Aggregates.Activities;
using DoubleGis.Erm.BLCore.API.Aggregates.Activities.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Read;
using DoubleGis.Erm.Platform.DAL.Transactions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Activity;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLCore.Operations.Generic.Modify.Custom
{
    public sealed class ModifyLetterService : IModifyBusinessModelEntityService<Letter>
    {
        private readonly ILetterReadModel _readModel;
        private readonly IBusinessModelEntityObtainer<Letter> _activityObtainer;

        private readonly IActivityReadService _activityReadService;

        private readonly ICreateLetterAggregateService _createOperationService;
        private readonly IUpdateLetterAggregateService _updateOperationService;

        public ModifyLetterService(
            ILetterReadModel readModel,
            IBusinessModelEntityObtainer<Letter> obtainer,
            IActivityReadService activityReadService,
            ICreateLetterAggregateService createOperationService,
            IUpdateLetterAggregateService updateOperationService)
        {
            _readModel = readModel;
            _activityObtainer = obtainer;
            _activityReadService = activityReadService;
            _createOperationService = createOperationService;
            _updateOperationService = updateOperationService;
        }

        public long Modify(IDomainEntityDto domainEntityDto)
        {
            var letterDto = (LetterDomainEntityDto)domainEntityDto;
            var letter = _activityObtainer.ObtainBusinessModelEntity(domainEntityDto);

            _activityReadService.CheckIfAnyEntityReferencesContainsReserve(letterDto.RegardingObjects);
            _activityReadService.CheckIfEntityReferencesContainsReserve(letterDto.RecipientRef);            

            using (var transaction = new TransactionScope(TransactionScopeOption.Required, DefaultTransactionOptions.Default))
            {
                IEnumerable<LetterRegardingObject> oldRegardingObjects = null;
                LetterSender oldSender = null;
                LetterRecipient oldRecipient = null;
                if (letter.IsNew())
                {
                    _createOperationService.Create(letter);
                }
                else
                {
                    _updateOperationService.Update(letter);
                    oldRegardingObjects = _readModel.GetRegardingObjects(letter.Id);
                    oldSender = _readModel.GetSender(letter.Id);
                    oldRecipient = _readModel.GetRecipient(letter.Id);
                }

                _updateOperationService.ChangeRegardingObjects(letter,
                                                               oldRegardingObjects,
                                                               letter.ReferencesIfAny<Letter, LetterRegardingObject>(letterDto.RegardingObjects));
                _updateOperationService.ChangeSender(letter, oldSender, letter.ReferencesIfAny<Letter, LetterSender>(letterDto.SenderRef));
                _updateOperationService.ChangeRecipient(letter, oldRecipient, letter.ReferencesIfAny<Letter, LetterRecipient>(letterDto.RecipientRef));

                transaction.Complete();

                return letter.Id;
            }
        }
    }
}
