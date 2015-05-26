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
        [Tags("Storage")]
        [Subject(typeof(Finder))]
        class When_queryable_returned
        {
            static IFinder Finder;
            static IQueryable<IEntity> Result;

            Establish context = () => Finder = new ConsistentFinder(new Finder(CreateReadDomainContextProvider()), null, null, null);
            Because of = () => Result = Finder.Find(CreateSpecification());
            It should_return_restricted_queryable = () => (Result is WrappedQuery).Should().BeTrue();

            static FindSpecification<IEntity> CreateSpecification()
            {
                return new FindSpecification<IEntity>(x => true);
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