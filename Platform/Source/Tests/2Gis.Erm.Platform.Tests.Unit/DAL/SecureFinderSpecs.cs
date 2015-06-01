using System.Linq;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Obsolete;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Storage.Core;
using NuClear.Storage.Futures.Queryable;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class SecureFinderSpecs
    {
        class FindBySelectorContext : FindBySpecContext
        {
            protected static SelectSpecification<Deal, Deal> _selectSpec;

            Establish context = () =>
                {
                    _selectSpec = new SelectSpecification<Deal, Deal>(e => e);
                };
        }

        class FindBySpecContext : SecureFinderContext
        {
            protected static FindSpecification<Deal> _findSpec;

            Establish context = () =>
                {
                    _findSpec = new FindSpecification<Deal>(e => true);
                    var readableDomainContextMock = new Mock<IReadableDomainContext>();
                    readableDomainContextMock.Setup(x => x.GetQueryableSource<Deal>()).Returns(_finderEntities.Cast<Deal>());
                    ReadableDomainContextProvider.Setup(f => f.Get()).Returns(readableDomainContextMock.Object);
                };
        }

        /// <summary>
        ///     Поведение, когда проверка доступа ВКЛЮЧЕНА.
        /// </summary>
        [Behaviors]
        class RestrictAccessBehavior
        {
            protected static IQueryable _restrictedEntities;
            protected static IQueryable _result;

            It should_return_same_result_as_SecurityAccess = () => _result.Should().Contain(_restrictedEntities, "Коллекции элементов должны совпадать.");
        }

        class SecureFinderContext
        {
            protected static IQueryable _finderEntities;
            protected static IQueryable _restrictedEntities;
            protected static IQueryable _result;
            protected static Mock<ISecurityServiceEntityAccessInternal> _securityAccess;

            static MockUserContext _mockUserContext;

            Establish context = () =>
                {
                    _finderEntities = CreateEntities();

                    ReadableDomainContextProvider = new Mock<IReadableDomainContextProvider>();

                    _securityAccess = new Mock<ISecurityServiceEntityAccessInternal>();

                    _mockUserContext = new MockUserContext();

                    Target = new SecureFinder(ReadableDomainContextProvider.Object,
                                              _mockUserContext.Object,
                                              _securityAccess.Object);
                };

            protected static Mock<IReadableDomainContextProvider> ReadableDomainContextProvider { get; private set; }
            protected static SecureFinder Target { get; private set; }

            protected static void SetUpRestrictQuery()
            {
                SkipEntityAccess(false);

                _restrictedEntities = CreateEntities();
                _securityAccess.Setup(s => s.RestrictQuery(_finderEntities, typeof(Deal).AsEntityName(), _mockUserContext.Object.Identity.Code))
                               .Returns(_restrictedEntities);
            }

            protected static void SkipEntityAccess(bool checkAccess)
            {
                _mockUserContext.SkipEntityAccess(checkAccess);
            }

            static IQueryable<Deal> CreateEntities()
            {
                return new[] { new Deal(), new Deal() }.AsQueryable();
            }
        }

        /// <summary>
        ///     Поведение, когда проверка доступа ОТКЛЮЧЕНА.
        /// </summary>
        [Behaviors]
        class SkipEntityAccessCheckBehavior
        {
            protected static IQueryable _finderEntities;
            protected static IQueryable _result;

            It should_return_same_result_as_Finder = () => _result.Should().Contain(_finderEntities, "Коллекции элементов должны совпадать.");
        }

        /// <summary>
        ///     Запрос по спецификации поиска, с ограничением вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_Find_by_specification_access_check : FindBySpecContext
        {
            Establish context = () => SetUpRestrictQuery();

            Because of = () => _result = new IncapsulationBreakingQueryableSequence<Deal>(Target.Find(_findSpec)).Queryable;

            Behaves_like<RestrictAccessBehavior> restrict_query;
        }

        /// <summary>
        ///     Запрос по спецификации поиска и отбора, с ограничением вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_Find_by_specification_and_selector_access_check : FindBySelectorContext
        {
            Establish context = () => SetUpRestrictQuery();

            Because of = () => _result = new IncapsulationBreakingQueryableSequence<Deal>(Target.Find(_findSpec).Map(x => x.Select(_selectSpec))).Queryable;

            Behaves_like<RestrictAccessBehavior> restrict_query;
        }

        /// <summary>
        ///     Запрос по спецификации поиска и отбора без ограничения вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_Find_by_specification_and_selector_skip_access_check : FindBySelectorContext
        {
            Establish context = () => SkipEntityAccess(true);

            Because of = () => _result = new IncapsulationBreakingQueryableSequence<Deal>(Target.Find(_findSpec).Map(x => x.Select(_selectSpec))).Queryable;

            Behaves_like<SkipEntityAccessCheckBehavior> skip_entity_access;
        }

        /// <summary>
        ///     Запрос по спецификации без ограничения вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_Find_by_specification_skip_access_check : FindBySpecContext
        {
            Establish context = () => SkipEntityAccess(true);

            Because of = () => _result = new IncapsulationBreakingQueryableSequence<Deal>(Target.Find(_findSpec)).Queryable;

            Behaves_like<SkipEntityAccessCheckBehavior> skip_entity_access;
        }
    }
}