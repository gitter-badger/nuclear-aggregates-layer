using System.Linq;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Core;

using Storage.Tests.EntityTypes;
using Storage.Tests.Fakes;

using It = Machine.Specifications.It;

namespace Storage.Tests
{
    public class CachingReadDomainContextSpecs
    {
        static CachingReadableDomainContext _readableDomainContextCachingProxy;

        [Tags("DAL")]
        [Subject(typeof(CachingReadableDomainContext))]
        class When_call_GetQueryableSource_for_entity_from_Erm_scope
        {
            static IQueryable<Entity1> _query;

            Establish context = () => _readableDomainContextCachingProxy = new CachingReadableDomainContext(new StubDomainContextFactory(),
                                                                                                         new StubDomainContextMetadataProvider());

            Because of = () => _query = _readableDomainContextCachingProxy.GetQueryableSource<Entity1>();

            It should_return_empty_domain_context = () => _query.Should().BeEmpty();
            It domain_context_should_be_for_exactly_entity = () => _query.ToArray().Should().BeEquivalentTo(new Entity1[0]);
        }

        [Tags("DAL")]
        [Subject(typeof(CachingReadableDomainContext))]
        class When_call_GetQueryableSource_for_types_from_the_same_scope
        {
            static Mock<IReadableDomainContextFactory> _readDomainContextFactoryMock;

            Establish context = () =>
                {
                    _readDomainContextFactoryMock = new Mock<IReadableDomainContextFactory>();
                    _readDomainContextFactoryMock.Setup(x => x.Create(Moq.It.IsAny<DomainContextMetadata>())).Returns(new StubDomainContext()).Verifiable();

                    _readableDomainContextCachingProxy = new CachingReadableDomainContext(_readDomainContextFactoryMock.Object,
                                                                                       new StubDomainContextMetadataProvider());
                };

            Because of = () =>
                {
                    _readableDomainContextCachingProxy.GetQueryableSource<Entity1>();
                    _readableDomainContextCachingProxy.GetQueryableSource<Entity2>();
                };

            It should_always_return_the_same_domain_context_for_entities_from_the_same_scope = 
                () => _readDomainContextFactoryMock.Verify(x => x.Create(Moq.It.IsAny<DomainContextMetadata>()), Times.Exactly(2));
        }

        [Tags("DAL")]
        [Subject(typeof(CachingReadableDomainContext))]
        class When_Disposing
        {
            static Mock<IReadableDomainContext> _readDomainContextMock;
            
            Establish context = () =>
                {
                    _readDomainContextMock = new Mock<IReadableDomainContext>();
                    _readDomainContextMock.Setup(x => x.Dispose()).Verifiable();

                    var readDomainContextFactoryMock = new Mock<IReadableDomainContextFactory>();
                    readDomainContextFactoryMock.Setup(x => x.Create(Moq.It.IsAny<DomainContextMetadata>())).Returns(_readDomainContextMock.Object);

                    _readableDomainContextCachingProxy = new CachingReadableDomainContext(readDomainContextFactoryMock.Object,
                                                                                       new StubDomainContextMetadataProvider());
                };

            Because of = () =>
                {
                    _readableDomainContextCachingProxy.GetQueryableSource<Entity1>();

                    _readableDomainContextCachingProxy.Dispose();
                };

            It should_dispose_all_read_domain_contexts_from_the_all_scopes = () => _readDomainContextMock.Verify(x => x.Dispose(), Times.Exactly(2));
        }
    }
}