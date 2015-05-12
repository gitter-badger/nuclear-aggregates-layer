using System;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.EAV;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage;
using NuClear.Storage.Core;
using NuClear.Storage.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class FinderSpecs
    {
        class FindByFinderSpecBase : FinderSpecBase
        {
            Establish context = () =>
                {
                    Finded = Mock.Of<IEntity>();
                    ReadDomainContext.Setup(r => r.GetQueryableSource<IEntity>()).Returns(new[] { Finded, Mock.Of<IEntity>() }.AsQueryable());
                };

            protected static IEntity Finded { get; private set; }
            protected static IQueryable<IEntity> Result { get; set; }

            protected static void ResultShouldOnlyContain(IEntity expected)
            {
                Result.Should().OnlyContain(i => i.Equals(expected), "Результат запроса должен содержать только один ожидаемый элемент.");
            }
        }

        class FindBySpecificationFinderSpecBase : FindByFinderSpecBase
        {
            Establish context = () =>
                {
                    FindSpecification = new Mock<IFindSpecification<IEntity>>();
                    FindSpecification.SetupGet(f => f.Predicate).Returns(e => e.Equals(Finded));
                };

            protected static Mock<IFindSpecification<IEntity>> FindSpecification { get; private set; }
        }

        class FinderSpecBase
        {
            Establish context = () =>
                {
                    ReadDomainContext = new Mock<IReadDomainContext>();

                    var readDomainContextProvider = new Mock<IReadDomainContextProvider>();
                    readDomainContextProvider.Setup(r => r.Get()).Returns(ReadDomainContext.Object);

                    Target = new Finder(readDomainContextProvider.Object);
                };

            protected static Mock<IReadDomainContext> ReadDomainContext { get; private set; }
            protected static Finder Target { get; private set; }
        }

        /// <summary>
        ///     Запрос всех объектов по типу, должен перенаправлять вызов к IReadDomainContext.GetQueryableSource(Type).
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(Finder))]
        class When_FindAll_by_Type : FinderSpecBase
        {
            static readonly Type ExpectedType = typeof(object);

            Because of = () => ((IQuery)Target).For(ExpectedType);

            It should_call_GetQueryableSource_with_expectedType =
                () => ReadDomainContext.Verify(r => r.GetQueryableSource(ExpectedType), Times.Once(), "Ожидался запрос к GetQueryableSource по типу.");
        }

        /// <summary>
        ///     Запрос всех объектов по типу параметра, должен перенаправлять вызов к IReadDomainContext.GetQueryableSource'T().
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(Finder))]
        class When_FindAll_by_Type_as_generic : FinderSpecBase
        {
            Because of = () => ((IQuery)Target).For<IEntity>();

            It should_call_GetQueryableSource_with_expectedType =
                () =>
                ReadDomainContext.Verify(r => r.GetQueryableSource<IEntity>(), Times.Once(), "Ожидался запрос к GetQueryableSource по типу generic параметра.");
        }

        /// <summary>
        ///     Запрос объектов по спецификации поиска, должен вренуть только один объект удовлетворяющий спецификации.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(Finder))]
        class When_Find_by_findSpecification : FindBySpecificationFinderSpecBase
        {
            Because of = () => Result = Target.Find(FindSpecification.Object);

            It should_return_finded_as_result = () => ResultShouldOnlyContain(Finded);
        }

        /// <summary>
        ///     Запрос объектов по спецификации поиска и выбора, должен вренуть один выбранный элемент.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(Finder))]
        class When_Find_by_find_and_select_specification : FindBySpecificationFinderSpecBase
        {
            static Mock<ISelectSpecification<IEntity, IEntity>> _selectSpecification;
            static IEntity _selected;

            Establish context = () =>
                {
                    _selected = Mock.Of<IEntity>();

                    _selectSpecification = new Mock<ISelectSpecification<IEntity, IEntity>>();
                    _selectSpecification.SetupGet(s => s.Selector).Returns(e => e.Equals(Finded) ? _selected : (IEntity)null);
                };

            Because of = () => Result = Target.Find<IEntity, IEntity>(_selectSpecification.Object, FindSpecification.Object);

            It should_return_selected_as_result = () => ResultShouldOnlyContain(_selected);
        }

        /// <summary>
        ///     Запрос объектов по лямбда выражению, должен вренуть один выбранный элемент.
        /// </summary>
        [Tags("DAL")]
        [Subject(typeof(Finder))]
        class When_Find_by_lambda_expression : FindByFinderSpecBase
        {
            Because of = () => Result = Target.Find<IEntity>(e => e.Equals(Finded));

            It should_return_finded_as_result = () => ResultShouldOnlyContain(Finded);
        }

        [Tags("DAL")]
        [Subject(typeof(Finder))]
        class When_queryable_returned
        {
            static IFinder Finder;
            static IQueryable<IEntity> Result;

            Establish context = () => Finder = new ConsistentFinderDecorator(new Finder(CreateReadDomainContextProvider()), null, null, null);
            Because of = () => Result = Finder.Find(CreateSpecification());
            It should_return_restricted_queryable = () => (Result is WrappedQuery).Should().BeTrue();

            static IFindSpecification<IEntity> CreateSpecification()
            {
                var spec = Mock.Of<IFindSpecification<IEntity>>();
                Mock.Get(spec)
                    .SetupGet(x => x.Predicate)
                    .Returns(e => true);

                return spec;
            }

            static IReadDomainContextProvider CreateReadDomainContextProvider()
            {
                var domainContext = Mock.Of<IReadDomainContext>();
                Mock.Get(domainContext)
                    .Setup(x => x.GetQueryableSource(Moq.It.IsAny<Type>()))
                    .Returns(new object[0].AsQueryable());
                
                Mock.Get(domainContext)
                    .Setup(x => x.GetQueryableSource<IEntity>())
                    .Returns(new IEntity[0].AsQueryable());

                var domainContextProvider = Mock.Of<IReadDomainContextProvider>();
                Mock.Get(domainContextProvider)
                    .Setup(provider => provider.Get())
                    .Returns(domainContext);

                return domainContextProvider;
            }

        }
    }
}