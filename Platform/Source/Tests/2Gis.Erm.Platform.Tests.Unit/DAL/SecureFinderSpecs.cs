using System;
using System.Linq;
using System.Linq.Expressions;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class SecureFinderSpecs
    {
        class FindAllSecureFinderContext : SecureFinderContext
        {
            Establish context = () =>
                {
                    ExpectedType = typeof(Deal);
                    Finder.Setup(f => f.For(ExpectedType)).Returns(_finderEntities);
                };

            protected static Type ExpectedType { get; set; }
        }

        class FindByExprContext : SecureFinderContext
        {
            static Expression<Func<Deal, bool>> _expression;

            Establish context = () =>
                {
                    _expression = e => true;
                    Finder.Setup(f => f.Find(_expression)).Returns(_finderEntities.Cast<Deal>());
                };

            Because of = () => _result = Target.Find(_expression);
        }

        class FindBySelectorContext : FindBySpecContext
        {
            protected static ISelectSpecification<Deal, Deal> _selectSpec;

            Establish context = () =>
                {
                    var selectSpecMock = new Mock<ISelectSpecification<Deal, Deal>>();
                    selectSpecMock.SetupGet(s => s.Selector).Returns(e => e);

                    _selectSpec = selectSpecMock.Object;
                };
        }

        class FindBySpecContext : SecureFinderContext
        {
            protected static IFindSpecification<Deal> _findSpec;

            Establish context = () =>
                {
                    var specMock = new Mock<IFindSpecification<Deal>>();
                    specMock.SetupGet(s => s.Predicate).Returns(e => true);
                    _findSpec = specMock.Object;
                    Finder.Setup(f => f.Find(_findSpec)).Returns(_finderEntities.Cast<Deal>());
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

            static MoqUserContext _moqUserContext;

            Establish context = () =>
                {
                    _finderEntities = CreateEntities();

                    Finder = new Mock<IFinder>();

                    _securityAccess = new Mock<ISecurityServiceEntityAccessInternal>();

                    _moqUserContext = new MoqUserContext();

                    Target = new SecureFinder(Finder.Object,
                                              _moqUserContext.Object,
                                              _securityAccess.Object);
                };

            protected static Mock<IFinder> Finder { get; private set; }
            protected static SecureFinder Target { get; private set; }

            protected static void SetUpRestrictQuery()
            {
                SkipEntityAccess(false);

                _restrictedEntities = CreateEntities();
                _securityAccess.Setup(s => s.RestrictQuery(_finderEntities, typeof(Deal).AsEntityName(), _moqUserContext.Object.Identity.Code))
                               .Returns(_restrictedEntities);
            }

            protected static void SkipEntityAccess(bool checkAccess)
            {
                _moqUserContext.SkipEntityAccess(checkAccess);
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
        ///     Запрос всех по типу, с ограничением вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_FindAll_by_Type_access_check : FindAllSecureFinderContext
        {
            Establish context = () => SetUpRestrictQuery();

            Because of = () => _result = Target.For(ExpectedType);

            Behaves_like<RestrictAccessBehavior> restrict_query;
        }

        /// <summary>
        ///     Запрос всех по типу без ограничения вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_FindAll_by_Type_skip_access_check : FindAllSecureFinderContext
        {
            Establish context = () => SkipEntityAccess(true);

            Because of = () => _result = Target.For(ExpectedType);

            Behaves_like<SkipEntityAccessCheckBehavior> skip_entity_access;
        }

        /// <summary>
        ///     Запрос по выражению, с ограничением вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_Find_by_expression_access_check : FindByExprContext
        {
            Establish context = () => SetUpRestrictQuery();

            Behaves_like<RestrictAccessBehavior> restrict_query;
        }

        /// <summary>
        ///     Запрос по выражению без ограничения вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_Find_by_expression_skip_access_check : FindByExprContext
        {
            Establish context = () => SkipEntityAccess(true);

            Behaves_like<SkipEntityAccessCheckBehavior> skip_entity_access;
        }

        /// <summary>
        ///     Запрос по спецификации поиска, с ограничением вызова.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(SecureFinder))]
        class When_Find_by_specification_access_check : FindBySpecContext
        {
            Establish context = () => SetUpRestrictQuery();

            Because of = () => _result = Target.Find(_findSpec);

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

            Because of = () => _result = Target.Find(_selectSpec, _findSpec);

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

            Because of = () => _result = Target.Find(_selectSpec, _findSpec);

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

            Because of = () => _result = Target.Find(_findSpec);

            Behaves_like<SkipEntityAccessCheckBehavior> skip_entity_access;
        }
    }
}