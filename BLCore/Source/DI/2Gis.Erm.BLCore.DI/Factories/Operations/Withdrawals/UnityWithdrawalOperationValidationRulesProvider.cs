using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals.ValidationRules;

using Microsoft.Practices.Unity;

namespace DoubleGis.Erm.BLCore.DI.Factories.Operations.Withdrawals
{
    public sealed class UnityWithdrawalOperationValidationRulesProvider : IWithdrawalOperationValidationRulesProvider
    {
        private readonly IUnityContainer _container;

        public UnityWithdrawalOperationValidationRulesProvider(IUnityContainer container)
        {
            _container = container;
        }

        public IEnumerable<IWithdrawalOperationValidationRule> GetValidationRules()
        {
            return new[]
                       {
                           ResolveRule<WithdrawalOperationAccessValidationRule>(),
                           ResolveRule<PeriodValidationRule>(),
                           ResolveRule<WithdrawalOperationWorkflowValidationRule>(),
                           ResolveRule<LocksExistenceValidationRule>(),
                           ResolveRule<LegalPersonsValidationRule>(),
                       };
        }

        private IWithdrawalOperationValidationRule ResolveRule<Trule>() where Trule : IWithdrawalOperationValidationRule
        {
            return (IWithdrawalOperationValidationRule)_container.Resolve(typeof(Trule));
        }
    }
}