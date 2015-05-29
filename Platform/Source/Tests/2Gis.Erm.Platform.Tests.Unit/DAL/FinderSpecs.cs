using System;
using System.Linq;

using DoubleGis.Erm.Platform.DAL.EAV;
using DoubleGis.Erm.Platform.DAL.Obsolete;

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
        [Subject(typeof(ConsistentFinder))]
        class When_queryable_returned
        {
            static IFinder Finder;
            static IncapsulationBreakingQueryableFutureSequence<IEntity> Result;

            Establish context = () => Finder = new ConsistentFinder(CreateReadDomainContextProvider(), null, null, null);
            Because of = () => Result = (IncapsulationBreakingQueryableFutureSequence<IEntity>)Finder.Find(new FindSpecification<IEntity>(x => true));
            It should_return_restricted_queryable = () => (Result.Queryable is WrappedQuery).Should().BeTrue();

            static IReadableDomainContextProvider CreateReadDomainContextProvider()
            {
                var domainContext = Mock.Of<IReadableDomainContext>();
                Mock.Get(domainContext)
                    .Setup(x => x.GetQueryableSource(Moq.It.IsAny<Type>()))
                    .Returns(new object[0].AsQueryable());

                Mock.Get(domainContext)
                    .Setup(x => x.GetQueryableSource<IEntity>())
                    .Returns(new IEntity[0].AsQueryable());

                var domainContextProvider = Mock.Of<IReadableDomainContextProvider>();
                Mock.Get(domainContextProvider)
                    .Setup(provider => provider.Get())
                    .Returns(domainContext);

                return domainContextProvider;
            }
        } 
    }
}