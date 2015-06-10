using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.Aggregates.Accounts.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.Operations;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Readings.Queryable;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.Model
{
    [Tags("Model")]
    [Subject(typeof(AccountRepository))]
    class When_assigning_an_account
    {
        static readonly AssignAccountDto Account = new AssignAccountDto {Account = new Account(), Limits = new Limit[0]};
        static IAssignAccountAggregateService _assignAccountRepository;

        Establish context = () =>
            {
                                    _assignAccountRepository = new AssignAccountAggregateService(Mock.Of<ISecureRepository<Account>>(),
                                                                 Mock.Of<ISecureRepository<Limit>>(),
                                                                                                 Mock.Of<IOperationScopeFactory>());
            };

        Because of = () => _assignAccountRepository.Assign(Account, 10);

        It new_OwnerCode_should_be_set_ = () => Account.Account.OwnerCode.Should().Be(10);
    }
}
