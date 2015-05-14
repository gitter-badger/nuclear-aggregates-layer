using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Generics;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Tests.Unit.Core.Infrastructure;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.Model
{
    [Tags("Model")]
    [Subject(typeof(AccountRepository))]
    class When_assigning_an_account
    {
        static readonly Account Account = new Account { OwnerCode = 0 };
        static IAssignAggregateRepository<Account> _assignAccountRepository;

        Establish context = () =>
            {
                var finderMock = new Mock<ISecureFinder>();
                finderMock.Setup(x => x.Find(Moq.It.IsAny<FindSpecification<Account>>())).Returns(new[] { Account }.AsQueryable());

                _assignAccountRepository = new AccountRepository(null,
                                                                 null,
                                                                 null,
                                                                 finderMock.Object,
                                                                 null,
                                                                 null,
                                                                 Mock.Of<ISecureRepository<Account>>(),
                                                                 null,
                                                                 Mock.Of<ISecureRepository<Limit>>(),
                                                                 null,
                                                                 null,
                                                                 null,
                                                                 null,
                                                                 null,
                                                                 new StubOperationScopeFactory());

            };

        Because of = () => _assignAccountRepository.Assign(Account.Id, 10);

        It new_OwnerCode_should_be_set_ = () => Account.OwnerCode.Should().Be(10);
    }
}
