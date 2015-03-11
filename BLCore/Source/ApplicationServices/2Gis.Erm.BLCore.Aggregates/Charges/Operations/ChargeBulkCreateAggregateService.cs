using System;
using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Aggregates.Charges.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Dto.Billing;
using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BLCore.Aggregates.Charges.Operations
{
    public sealed class ChargeBulkCreateAggregateService : IChargeBulkCreateAggregateService
    {
        private readonly IRepository<Charge> _chargeRepository;
        private readonly IIdentityProvider _identityProvider;
        private readonly IOperationScopeFactory _scopeFactory;

        public ChargeBulkCreateAggregateService(IRepository<Charge> chargeRepository, IIdentityProvider identityProvider, IOperationScopeFactory scopeFactory)
        {
            _chargeRepository = chargeRepository;
            _identityProvider = identityProvider;
            _scopeFactory = scopeFactory;
        }

        public void Create(long projectId, DateTime startDate, DateTime endDate, IReadOnlyCollection<ChargeDto> chargesDtos, Guid sessionId)
        {
            using (var scope = _scopeFactory.CreateSpecificFor<BulkCreateIdentity, Charge>())
            {
                foreach (var chargeDto in chargesDtos)
                {
                    var charge = new Charge
                        {
                            OrderPositionId = chargeDto.OrderPositionId,
                            Amount = chargeDto.Amount,
                            ProjectId = projectId,
                            PeriodStartDate = startDate,
                            PeriodEndDate = endDate,
                            SessionId = sessionId
                        };

                    _identityProvider.SetFor(charge);
                    _chargeRepository.Add(charge);
                    scope.Added<Charge>(charge.Id);
                }

                _chargeRepository.Save();

                scope.Complete();
            }
        }
    }
}