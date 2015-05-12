using System.Linq;

using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Storage.Core;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class ReadDomainContextProxySpecs
    {
        static CachingReadDomainContext _readDomainContextCachingProxy;

        [Tags("DAL")]
        [Subject(typeof(CachingReadDomainContext))]
        class When_call_GetQueryableSource_for_entity_from_Erm_scope
        {
            static IQueryable<ErmScopeEntity1> _query;

            Establish context = () => _readDomainContextCachingProxy = new CachingReadDomainContext(new StubDomainContextFactory(),
                                                                                                         new StubDomainContextMetadataProvider());

            Because of = () => _query = _readDomainContextCachingProxy.GetQueryableSource<ErmScopeEntity1>();

            It should_return_empty_domain_context = () => _query.Should().BeEmpty();
            It domain_context_should_be_for_exactly_entity = () => _query.ToArray().Should().BeEquivalentTo(new ErmScopeEntity1[0]);
        }

        [Tags("DAL")]
        [Subject(typeof(CachingReadDomainContext))]

        class When_call_GetQueryableSource_for_entity_from_Security_scope
        {
            static IReadDomainContext _readDomainContext;
            static IQueryable<SecurityScopeEntity1> _query;

            Establish context = () => _readDomainContext = new CachingReadDomainContext(new StubDomainContextFactory(),
                                                                                             new StubDomainContextMetadataProvider());

            Because of = () => _query = _readDomainContext.GetQueryableSource<SecurityScopeEntity1>();

            It should_return_empty_domain_context = () => _query.Should().BeEmpty();
            It domain_context_should_be_for_Secutity_scope_entity = () => _query.ToArray().Should().BeEquivalentTo(new SecurityScopeEntity1[0]);
            It domain_context_should_not_be_for_some_other_scope_entity = () => _query.ToArray().Should().NotBeEquivalentTo(new ErmScopeEntity1[0]);
        }

        [Tags("DAL")]
        [Subject(typeof(CachingReadDomainContext))]
        class When_call_GetQueryableSource_for_types_from_the_same_scope
        {
            static Mock<IReadDomainContextFactory> _readDomainContextFactoryMock;

            Establish context = () =>
                {
                    _readDomainContextFactoryMock = new Mock<IReadDomainContextFactory>();
                    _readDomainContextFactoryMock.Setup(x => x.Create(Moq.It.IsAny<DomainContextMetadata>())).Returns(new StubDomainContext()).Verifiable();

                    _readDomainContextCachingProxy = new CachingReadDomainContext(_readDomainContextFactoryMock.Object,
                                                                                       new StubDomainContextMetadataProvider());
                };

            Because of = () =>
                {
                    _readDomainContextCachingProxy.GetQueryableSource<ErmScopeEntity1>();
                    _readDomainContextCachingProxy.GetQueryableSource<ErmScopeEntity2>();
                    _readDomainContextCachingProxy.GetQueryableSource<SecurityScopeEntity1>();
                    _readDomainContextCachingProxy.GetQueryableSource<SecurityScopeEntity2>();
                };

            It should_always_return_the_same_domain_context_for_entities_from_the_same_scope = 
                () => _readDomainContextFactoryMock.Verify(x => x.Create(Moq.It.IsAny<DomainContextMetadata>()), Times.Exactly(2));
        }

        [Tags("DAL")]
        [Subject(typeof(CachingReadDomainContext))]
        class When_Disposing
        {
            static Mock<IReadDomainContext> _readDomainContextMock;
            
            Establish context = () =>
                {
                    _readDomainContextMock = new Mock<IReadDomainContext>();
                    _readDomainContextMock.Setup(x => x.Dispose()).Verifiable();

                    var readDomainContextFactoryMock = new Mock<IReadDomainContextFactory>();
                    readDomainContextFactoryMock.Setup(x => x.Create(Moq.It.IsAny<DomainContextMetadata>())).Returns(_readDomainContextMock.Object);

                    _readDomainContextCachingProxy = new CachingReadDomainContext(readDomainContextFactoryMock.Object,
                                                                                       new StubDomainContextMetadataProvider());
                };

            Because of = () =>
                {
                    _readDomainContextCachingProxy.GetQueryableSource<ErmScopeEntity1>();
                    _readDomainContextCachingProxy.GetQueryableSource<SecurityScopeEntity1>();

                    _readDomainContextCachingProxy.Dispose();
                };

            It should_dispose_all_read_domain_contexts_from_the_all_scopes = () => _readDomainContextMock.Verify(x => x.Dispose(), Times.Exactly(2));
        }
    }
}