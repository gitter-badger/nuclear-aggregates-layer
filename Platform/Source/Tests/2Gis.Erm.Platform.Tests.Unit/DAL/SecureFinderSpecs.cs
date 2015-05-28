﻿using System.Linq;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Storage;
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
                    Finder.Setup(f => f.Find(_findSpec)).Returns(new QueryableFutureSequence<Deal>(_finderEntities.Cast<Deal>()));
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

                    Finder = new Mock<IFinder>();

                    _securityAccess = new Mock<ISecurityServiceEntityAccessInternal>();

                    _mockUserContext = new MockUserContext();

                    Target = new SecureFinder(Finder.Object,
                                              _mockUserContext.Object,
                                              _securityAccess.Object);
                };

            protected static Mock<IFinder> Finder { get; private set; }
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

            Because of = () => _result = Target.Find(_findSpec, _selectSpec);

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

            Because of = () => _result = Target.Find(_findSpec, _selectSpec);

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