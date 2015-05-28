using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;

using FluentAssertions;

using NuClear.Model.Common.Entities;
using NuClear.Storage;
using NuClear.Storage.Futures.Queryable;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.API.Operations.Generic
{
    public sealed class AppendOperationServiceTest : IIntegrationTest
    {
        private readonly IFinder _finder;
        private readonly IAppendGenericEntityService<OrganizationUnit, User> _appendGenericEntityService;

        public AppendOperationServiceTest(IFinder finder, IAppendGenericEntityService<OrganizationUnit, User> appendGenericEntityService)
        {
            _finder = finder;
            _appendGenericEntityService = appendGenericEntityService;
        }

        public ITestResult Execute()
        {
            var activeOrganizationUnits = 
                _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<OrganizationUnit>())
                    .Map(q => q.Select(ou => ou.Id))
                    .Many();

            var appendInfo =
                _finder
                    .Find(Specs.Find.ActiveAndNotDeleted<User>())
                    .Map(q => q.Select(u => new
                        {
                            User = u,
                            AlreadyAppended = u.UserOrganizationUnits.Select(x => x.OrganizationUnitId)
                        }))
                    .Many()
                    .Select(u => new
                        {
                            u.User,
                            NotAppended = activeOrganizationUnits.Where(x => !u.AlreadyAppended.Contains(x))
                        })
                    .First(u => u.NotAppended.Any());

            var appendParams = new AppendParams
                {
                    AppendedId = appendInfo.NotAppended.First(),
                    AppendedType = EntityType.Instance.OrganizationUnit(),
                    ParentId = appendInfo.User.Id,
                    ParentType = EntityType.Instance.User()
                };

            return Result
                .When(() => _appendGenericEntityService.Append(appendParams))
                .Then(result => result.Status.Should().Be(TestResultStatus.Succeeded));
        }
    }
}
