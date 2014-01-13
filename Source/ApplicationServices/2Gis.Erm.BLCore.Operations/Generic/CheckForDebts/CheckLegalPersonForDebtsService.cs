﻿using DoubleGis.Erm.BLCore.Aggregates;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.CheckForDebts
{
    public class CheckLegalPersonForDebtsService : ICheckGenericEntityForDebtsService<LegalPerson>
    {
        private readonly IUserContext _userContext;
        private readonly ILegalPersonRepository _legalPersonRepository;

        public CheckLegalPersonForDebtsService(IUserContext userContext, ILegalPersonRepository legalPersonRepository)
        {
            _userContext = userContext;
            _legalPersonRepository = legalPersonRepository;
        }

        public CheckForDebtsResult CheckForDebts(long entityId)
        {
            try
            {
                var checkAggregateForDebtsRepository = _legalPersonRepository as ICheckAggregateForDebtsRepository<LegalPerson>;
                checkAggregateForDebtsRepository.CheckForDebts(entityId, _userContext.Identity.Code, false);
                return new CheckForDebtsResult();
            }
            catch (ProcessAccountsWithDebtsException exception)
            {
                return new CheckForDebtsResult
                    {
                        DebtsExist = true,
                        Message = exception.Message
                    };
            }
        }
    }
}