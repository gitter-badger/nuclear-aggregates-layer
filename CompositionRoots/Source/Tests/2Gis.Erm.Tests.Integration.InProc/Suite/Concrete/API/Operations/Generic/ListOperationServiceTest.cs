using System.Collections.Generic;

using DoubleGis.Erm.BLCore.API.Operations;
using DoubleGis.Erm.BLCore.API.Operations.Generic.List;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.BDD;

using FluentAssertions;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class ListOperationServiceTest : BDDIntegrationTestBase<ListOperationServiceTest.ListingContext, OrdinaryTestResult>
    {
        public class ListingContext : IBDDTestArgs
        {
            public string GivenDescription {get; set; }
            public SearchListModel SearchModel { get; set; }
        }

        private readonly IListEntityService _clientListingGenericEntityService;

        public ListOperationServiceTest(IOperationServicesManager operationServicesManager)
        {
            _clientListingGenericEntityService = operationServicesManager.GetListEntityService(EntityType.Instance.AccountDetail());
        }

        protected override IEnumerable<BDDTestRunConfig<ListingContext, OrdinaryTestResult>> Given()
        {
            return new[] { new BDDTestRunConfig<ListingContext, OrdinaryTestResult>
                {

                    Args = new ListingContext
                        {
                            GivenDescription = "Listing of accountDetails, skip 0, take 40, with asc sorting by Id",
                            SearchModel = new SearchListModel { Start = 0, Limit = 40, Sort = "Id ASC" }
                        }
                } };
        }

        protected override OrdinaryTestResult When(ListingContext arg)
        {
            return _clientListingGenericEntityService.List(arg.SearchModel) != null 
                        ? OrdinaryTestResult.As.Succeeded 
                        : OrdinaryTestResult.As.Failed;
        }

        protected override void Then(ListingContext args, OrdinaryTestResult result)
        {
            result.Status.Should().Be(TestResultStatus.Succeeded);
        }
    }
}