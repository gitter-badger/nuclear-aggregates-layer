using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Dto;
using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Charges.Operations
{
    public sealed class ChargeCreateHistoryAggregateService : IChargeCreateHistoryAggregateService
    {
        private readonly IRepository<ChargesHistory> _chargesHistoryRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChargeCreateHistoryAggregateService(IRepository<ChargesHistory> chargesHistoryRepository, IIdentityProvider identityProvider, IOperationScopeFactory scopeFactory)
        {
            _chargesHistoryRepository = chargesHistoryRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Create(ChargesHistoryDto item)
        {
            var chargesHistory = new ChargesHistory
                {
                    ProjectId = item.ProjectId,
                    PeriodStartDate = item.PeriodStartDate,
                    PeriodEndDate = item.PeriodEndDate,
                    Status = (int)item.Status,
                    Comment = item.Comment,
                    Message = item.Message,
                    SessionId = item.SessionId
                };

            _identityProvider.SetFor(chargesHistory);

            using (var scope = _scopeFactory.CreateSpecificFor<CreateIdentity, ChargesHistory>())
            {
                _chargesHistoryRepository.Add(chargesHistory);
                _chargesHistoryRepository.Save();

                scope.Added<ChargesHistory>(chargesHistory.Id).Complete();
            }
        }
    }
}