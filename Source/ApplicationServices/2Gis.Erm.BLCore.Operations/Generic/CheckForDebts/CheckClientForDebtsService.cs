using DoubleGis.Erm.BLCore.Aggregates;
using DoubleGis.Erm.BLCore.Aggregates.Clients;
using DoubleGis.Erm.BLCore.Aggregates.Common.Generics;
using DoubleGis.Erm.BLCore.API.Operations.Generic.CheckForDebts;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Generic.CheckForDebts
{
    public class CheckClientForDebtsService : ICheckGenericEntityForDebtsService<Client>
    {
        private readonly IUserContext _userContext;
        private readonly IClientRepository _clientRepository;

        public CheckClientForDebtsService(IUserContext userContext, IClientRepository clientRepository)
        {
            _userContext = userContext;
            _clientRepository = clientRepository;
        }

        public CheckForDebtsResult CheckForDebts(long entityId)
        {
            try
            {
                var checkAggregateForDebtsRepository = _clientRepository as ICheckAggregateForDebtsRepository<Client>;
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