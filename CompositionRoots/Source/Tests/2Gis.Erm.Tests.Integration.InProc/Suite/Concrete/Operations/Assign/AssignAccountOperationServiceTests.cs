using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Security;

using DoubleGis.Erm.BLCore.API.Aggregates.Settings;
using DoubleGis.Erm.BLCore.Operations.Generic.Assign;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Operations.Assign
{ // ReSharper disable InconsistentNaming
    public class AssignAccountOperationServiceTest_AssignAccountWoDebts : IIntegrationTest
    {
        private const long OwnerCode = 1234;

        private readonly AssignAccountOperationService _os;
        private readonly IAppropriateEntityProvider<Account> _appropriateEntityProvider;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public AssignAccountOperationServiceTest_AssignAccountWoDebts(AssignAccountOperationService os,
                                                 IAppropriateEntityProvider<Account> appropriateEntityProvider,
                                                 IDebtProcessingSettings debtProcessingSettings)
        {
            _os = os;
            _appropriateEntityProvider = appropriateEntityProvider;
            _debtProcessingSettings = debtProcessingSettings;
        }

        public ITestResult Execute()
        {
            var initial = _appropriateEntityProvider.Get(TestSpecs.FindAccountWoDebts(OwnerCode, _debtProcessingSettings.MinDebtAmount),
                                                         TestSpecs.SelectAccountOwnerDto());
            _os.Assign(initial.Id, OwnerCode, false, false);

            var current = _appropriateEntityProvider.Get(Specs.Find.ById<Account>(initial.Id), TestSpecs.SelectAccountOwnerDto());

            current.AccountOwner.ShouldBeEquivalentTo(OwnerCode);
            current.LimitOwners.ShouldBeEquivalentTo(current.LimitOwners.Select(x => OwnerCode));

            return OrdinaryTestResult.As.Succeeded;
        }
    }

    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "Reviewed. Suppression is OK here.")]
    public class AssignAccountOperationServiceTest_AssignAccountWithDebts : IIntegrationTest
    {
        private const long OwnerCode = 1234;

        private readonly AssignAccountOperationService _os;
        private readonly IAppropriateEntityProvider<Account> _appropriateEntityProvider;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public AssignAccountOperationServiceTest_AssignAccountWithDebts(AssignAccountOperationService os,
                                                                        IAppropriateEntityProvider<Account> appropriateEntityProvider,
                                                                        IDebtProcessingSettings debtProcessingSettings)
        {
            _os = os;
            _appropriateEntityProvider = appropriateEntityProvider;
            _debtProcessingSettings = debtProcessingSettings;
        }

        public ITestResult Execute()
        {
            var initial = _appropriateEntityProvider.Get(TestSpecs.FindAccountWithDebts(OwnerCode, _debtProcessingSettings.MinDebtAmount), TestSpecs.SelectAccountOwnerDto());

            try
            {
                var result = _os.Assign(initial.Id, OwnerCode, false, false);
                return result.CanProceed ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
            }
            catch (SecurityException)
            {
                return OrdinaryTestResult.As.Ignored;
            }
        }
    }

    public class AssignAccountOperationServiceTest_AssignAccountWithDebtsWithBypass : IIntegrationTest
    {
        private const long OwnerCode = 1234;

        private readonly AssignAccountOperationService _os;
        private readonly IAppropriateEntityProvider<Account> _appropriateEntityProvider;
        private readonly IDebtProcessingSettings _debtProcessingSettings;

        public AssignAccountOperationServiceTest_AssignAccountWithDebtsWithBypass(AssignAccountOperationService os,
                                                                                  IAppropriateEntityProvider<Account> appropriateEntityProvider,
                                                                                  IDebtProcessingSettings debtProcessingSettings)
        {
            _os = os;
            _appropriateEntityProvider = appropriateEntityProvider;
            _debtProcessingSettings = debtProcessingSettings;
        }

        public ITestResult Execute()
        {
            var initial = _appropriateEntityProvider.Get(TestSpecs.FindAccountWithDebts(OwnerCode, _debtProcessingSettings.MinDebtAmount), TestSpecs.SelectAccountOwnerDto());

            try
            {
                var result = _os.Assign(initial.Id, OwnerCode, true, false);
                return result == null ? OrdinaryTestResult.As.Succeeded : OrdinaryTestResult.As.Failed;
            }
            catch (SecurityException)
            {
                return OrdinaryTestResult.As.Ignored;
            }
        }
    }

    public class AssignAccountOperationServiceTest_AssignAccountInReserve : IIntegrationTest
    {
        private const long OwnerCode = 1234;

        private readonly AssignAccountOperationService _os;
        private readonly IAppropriateEntityProvider<Account> _appropriateEntityProvider;

        public AssignAccountOperationServiceTest_AssignAccountInReserve(AssignAccountOperationService os,
                                                                        IAppropriateEntityProvider<Account> appropriateEntityProvider)
        {
            _os = os;
            _appropriateEntityProvider = appropriateEntityProvider;
        }

        public ITestResult Execute()
        {
            var initial = _appropriateEntityProvider.Get(TestSpecs.FindAccountInReserve(),
                                                         TestSpecs.SelectAccountOwnerDto());

            Action a = () => _os.Assign(initial.Id, OwnerCode, false, false);

            a.ShouldThrow<NotificationException>();

            return OrdinaryTestResult.As.Succeeded;
        }
    }

    public class AccountOwnerDto
    {
        public long AccountOwner { get; set; }
        public IEnumerable<long> LimitOwners { get; set; }
        public long Id { get; set; }
    }

    public class TestSpecs
    {
        public static FindSpecification<Account> FindAccountWoDebts(long ownerCode, decimal minDebtAmount)
        {
            var restrictedOwners = new[] { 27, ownerCode };
            return new FindSpecification<Account>(
                x =>
                x.IsActive && !x.IsDeleted && !restrictedOwners.Contains(x.OwnerCode) &&
                x.Limits.Any(y => y.IsActive && !y.IsDeleted) &&
                x.Limits.All(y => !restrictedOwners.Contains(y.OwnerCode)) && x.Balance - (x.Locks
                                                                                            .Where(y => y.IsActive && !y.IsDeleted)
                                                                                            .Sum(y => (decimal?)y.PlannedAmount) ?? 0) > minDebtAmount);
        }

        public static FindSpecification<Account> FindAccountWithDebts(long ownerCode, decimal minDebtAmount)
        {
            var restrictedOwners = new[] { 27, ownerCode };
            return new FindSpecification<Account>(
                x =>
                x.IsActive && !x.IsDeleted && !restrictedOwners.Contains(x.OwnerCode) &&
                x.Limits.Any(y => y.IsActive && !y.IsDeleted) &&
                x.Limits.All(y => !restrictedOwners.Contains(y.OwnerCode)) && x.Balance - (x.Locks
                                                                                            .Where(y => y.IsActive && !y.IsDeleted)
                                                                                            .Sum(y => (decimal?)y.PlannedAmount) ?? 0) <= minDebtAmount);
        }

        public static FindSpecification<Account> FindAccountInReserve()
        {
            return new FindSpecification<Account>(x => x.IsActive && !x.IsDeleted && x.OwnerCode == 27);
        }

        public static SelectSpecification<Account, AccountOwnerDto> SelectAccountOwnerDto()
        {
            return Specs.Select.Generic((Account x) => new AccountOwnerDto
                                                           {
                                                               Id = x.Id,
                                                               AccountOwner = x.OwnerCode,
                                                               LimitOwners = x.Limits.Where(y => y.IsActive && !y.IsDeleted).Select(y => y.OwnerCode)
                                                           });
        }
    }

    // ReSharper restore InconsistentNaming
}