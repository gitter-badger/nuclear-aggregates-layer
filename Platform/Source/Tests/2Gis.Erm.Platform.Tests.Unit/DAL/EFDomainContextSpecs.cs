using System;
using System.Data.Entity;
using System.Data.Entity.Core.Objects;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Core.UseCases.Context;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Replication.Metadata;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class EFDomainContextSpecs
    {
        const string DefaultContextName = "Erm";

        static IModifiableDomainContext _domainContext;

        static IMsCrmReplicationMetadataProvider _enabledReplicationMetadataProvider = new MsCrmReplicationMetadataProvider(EntityNameUtils.AllReplicated2MsCrmEntities);

        static IMsCrmReplicationMetadataProvider _disabledReplicationMetadataProvider = new MsCrmReplicationMetadataProvider(Enumerable.Empty<Type>());

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        public abstract class EFDomainContextMockContext
        {
            Establish context = () =>
            {
                
                ObjectContextMock  = new Mock<IDbContext>();

                _domainContext = new EFDomainContext(Mock.Of<IProcessingContext>(),
                                                     ObjectContextMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>());
            };

            protected static Mock<IDbContext> ObjectContextMock { get; private set; }
        }

        class When_call_SaveChanges_for_added_entities : EFDomainContextMockContext
        {
            static Deal _deal;

            Establish context = () =>
                {
                    _deal = new Deal { ReplicationCode = Guid.Empty };

                    ObjectContextMock.Setup(p => p.Entries())
                                     .Returns(new[] { new StubEntityEntry(_deal, EntityState.Added) });
                };

            Because of = () => _domainContext.SaveChanges();

            It entity_ReplicationCode_should_be_set = () => _deal.ReplicationCode.Should().NotBe(Guid.Empty);
        }

        class When_call_SaveChanges_for_modified_entities : EFDomainContextMockContext
        {
            static Deal _deal;
            static Guid _guid;

            Establish context = () =>
                {
                    _guid = Guid.NewGuid();
                    _deal = new Deal { ReplicationCode = _guid };

                    ObjectContextMock.Setup(p => p.Entries())
                                     .Returns(new[] { new StubEntityEntry(_deal, EntityState.Modified) });
                };

            Because of = () => _domainContext.SaveChanges();

            It entity_ReplicationCode_should_not_be_changed = () => _deal.ReplicationCode.Should().Be(_guid);
        }


        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        class When_Disposing_with_NullPendingChangesHandlingStrategy
        {
            static Exception _exception;

            Establish context = () =>
                {
                    var objectContextMock = new Mock<IDbContext>();
                    objectContextMock.Setup(x => x.Entries())
                                     .Returns(new[] { new StubEntityEntry(null, Moq.It.IsAny<EntityState>()) });

                    _domainContext = new EFDomainContext(new ProcessingContext(),
                                                         objectContextMock.Object,
                                                         new NullPendingChangesHandlingStrategy());
                };

            Because of = () => _exception = Catch.Exception(() => _domainContext.Dispose());

            It exception_should_not_be_thrown = () => _exception.Should().Be(null);
        }

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        class When_Disposing_with_ForcePendingChangesHandlingStrategy
        {
            static Exception _exception;

            Establish context = () =>
            {
                var objectContextMock = new Mock<IDbContext>();
                objectContextMock.Setup(x => x.HasChanges())
                                 .Returns(true);

                _domainContext = new EFDomainContext(new ProcessingContext(),
                                                     objectContextMock.Object,
                                                     new ForcePendingChangesHandlingStrategy());
            };

            Because of = () => _exception = Catch.Exception(() => _domainContext.Dispose());

            It exception_of_type_PendingChangesNotHandledException_should_be_thrown = () => _exception.Should().BeOfType<PendingChangesNotHandledException>();
        }
    }
}