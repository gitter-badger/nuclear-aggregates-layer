using System;
using System.Linq;

using DoubleGis.Erm.BL.Aggregates.Deals;
using DoubleGis.Erm.BL.API.Common.Enums;
using DoubleGis.Erm.BL.API.Operations.Concrete.AfterSaleService;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Common;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Concrete.AfterSaleService
{
    public class UpdateAfterSaleServiceOperationServiceTest : UseModelEntityTestBase<Deal>
    {
        private readonly DateTime _dateTime = DateTime.UtcNow.AddMonths(-2);
        private readonly IUpdateAfterSaleServiceOperationService _updateAfterSaleServiceOperationService;

        public UpdateAfterSaleServiceOperationServiceTest(IAppropriateEntityProvider<Deal> appropriateEntityProvider,
                                                          IUpdateAfterSaleServiceOperationService updateAfterSaleServiceOperationService)
            : base(appropriateEntityProvider)
        {
            _updateAfterSaleServiceOperationService = updateAfterSaleServiceOperationService;
        }

        protected override FindSpecification<Deal> ModelEntitySpec
        {
            get
            {
                var number = _dateTime.ToAbsoluteReleaseNumber();
                return Specs.Find.ActiveAndNotDeleted<Deal>() &&
                       new FindSpecification<Deal>(
                           d =>
                           d.AfterSaleServiceActivities.Any(a => a.AbsoluteMonthNumber == number && a.AfterSaleServiceType == (byte)AfterSaleServiceType.ASS1));
            }
        }

        protected override OrdinaryTestResult ExecuteWithModel(Deal modelEntity)
        {
            return Result.When(() => _updateAfterSaleServiceOperationService.Update(modelEntity.ReplicationCode, _dateTime, AfterSaleServiceType.ASS1))
                         .Then(result => result.Succeeded.Should().BeTrue());
        }
    }
}