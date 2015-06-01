using System;

using DoubleGis.Erm.BLCore.API.Aggregates.Firms.ReadModel;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.Orders
{
    public class CreateOrderOperationServiceTest : IIntegrationTest
    {
        private readonly IAppropriateEntityProvider<Currency> _currencyAppropriateEntityProvider;
        private readonly IAppropriateEntityProvider<OrganizationUnit> _organizationUnitAppropriateEntityProvider;
        private readonly IAppropriateEntityProvider<LegalPerson> _legalPersonAppropriateEntityProvider;
        private readonly IAppropriateEntityProvider<BranchOfficeOrganizationUnit> _branchOfficeOrganizationUnitAppropriateEntityProvider;
        private readonly IAppropriateEntityProvider<Firm> _firmAppropriateEntityProvider;
        private readonly IAppropriateEntityProvider<Bargain> _bargainAppropriateEntityProvider;
        private readonly IModifyBusinessModelEntityService<Order> _modifyEntityService;

        public CreateOrderOperationServiceTest(IAppropriateEntityProvider<Currency> currencyAppropriateEntityProvider,
                                               IAppropriateEntityProvider<OrganizationUnit> organizationUnitAppropriateEntityProvider,
                                               IAppropriateEntityProvider<LegalPerson> legalPersonAppropriateEntityProvider,
                                               IAppropriateEntityProvider<BranchOfficeOrganizationUnit> branchOfficeOrganizationUnitAppropriateEntityProvider,
                                               IAppropriateEntityProvider<Firm> firmAppropriateEntityProvider,
                                               IAppropriateEntityProvider<Bargain> bargainAppropriateEntityProvider,
                                               IModifyBusinessModelEntityService<Order> modifyEntityService)
        {
            _currencyAppropriateEntityProvider = currencyAppropriateEntityProvider;
            _organizationUnitAppropriateEntityProvider = organizationUnitAppropriateEntityProvider;
            _legalPersonAppropriateEntityProvider = legalPersonAppropriateEntityProvider;
            _branchOfficeOrganizationUnitAppropriateEntityProvider = branchOfficeOrganizationUnitAppropriateEntityProvider;
            _firmAppropriateEntityProvider = firmAppropriateEntityProvider;
            _bargainAppropriateEntityProvider = bargainAppropriateEntityProvider;
            _modifyEntityService = modifyEntityService;
        }

        public ITestResult Execute()
        {
            var currency = _currencyAppropriateEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Currency>());
            var organizationUnit = _organizationUnitAppropriateEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>());
            var legalPerson = _legalPersonAppropriateEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<LegalPerson>());
            var branchOfficeOrganizationUnit = _branchOfficeOrganizationUnitAppropriateEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<BranchOfficeOrganizationUnit>());
            var firm = _firmAppropriateEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Firm>() && FirmSpecs.Firms.Find.ByOrganizationUnit(organizationUnit.Id));
            var bargain = _bargainAppropriateEntityProvider.Get(Specs.Find.ActiveAndNotDeleted<Bargain>());
            
            var domainEntityDto = new OrderDomainEntityDto
                {
                    CurrencyRef = new EntityReference(currency.Id, currency.Name),
                    SourceOrganizationUnitRef = new EntityReference(organizationUnit.Id, organizationUnit.Name),
                    DestOrganizationUnitRef = new EntityReference(organizationUnit.Id, organizationUnit.Name),
                    LegalPersonRef = new EntityReference(legalPerson.Id, legalPerson.LegalName),
                    BranchOfficeOrganizationUnitRef = new EntityReference(branchOfficeOrganizationUnit.Id, branchOfficeOrganizationUnit.ShortLegalName),
                    FirmRef = new EntityReference(firm.Id, firm.Name),
                    PlatformRef = new EntityReference((long)PlatformEnum.Desktop, PlatformEnum.Desktop.ToString()),
                    InspectorRef = new EntityReference(firm.OwnerCode),
                    OwnerRef = new EntityReference(firm.OwnerCode),
                    BargainRef = new EntityReference(bargain.Id),
                    BeginDistributionDate = DateTime.Now.GetNextMonthFirstDate(),
                    WorkflowStepId = OrderState.OnRegistration,
                    OrderType = OrderType.Sale,
                    PaymentMethod = PaymentMethod.CashPayment,
                    PayableFact = 1000m,
                    PayablePlan = 1000m,
                    PayablePrice = 1180m,
                    VatPlan = 180m
                };

            return Result.When(_modifyEntityService.Modify(domainEntityDto))
                         .Then(result => result.Should().BeGreaterThan(0));
        }
    }
}
