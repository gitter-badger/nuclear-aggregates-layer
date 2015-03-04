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

using Nuclear.Tracing.API;

using It = Machine.Specifications.It;
using SaveOptions = DoubleGis.Erm.Platform.DAL.SaveOptions;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class EFDomainContextSpecs
    {
        const string DefaultContextName = "Erm";

        static IModifiableDomainContext _domainContext;

        static IMsCrmReplicationMetadataProvider _enabledReplicationMetadataProvider = new MsCrmReplicationMetadataProvider(EntityNameUtils.AsyncReplicated2MsCrmEntities,
                                                                                                                            EntityNameUtils.AllReplicated2MsCrmEntities
                                                                                                                            .Except(
                                                                                                                                                                               EntityNameUtils
                                                                                                                                                                                   .AsyncReplicated2MsCrmEntities));

        static IMsCrmReplicationMetadataProvider _disabledReplicationMetadataProvider = new MsCrmReplicationMetadataProvider(Enumerable.Empty<Type>(), Enumerable.Empty<Type>());

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        public abstract class EFDomainContextMockContext
        {
            Establish context = () =>
            {
                
                ObjectContextMock  = new Mock<IDbContext>();

                _domainContext = new EFDomainContext(Mock.Of<IProcessingContext>(),
                                                     DefaultContextName,
                                                     ObjectContextMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     _enabledReplicationMetadataProvider,
                                                     Mock.Of<ITracer>());
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

            Because of = () => _domainContext.SaveChanges(SaveOptions.None);

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

            Because of = () => _domainContext.SaveChanges(SaveOptions.None);

            It entity_ReplicationCode_should_not_be_changed = () => _deal.ReplicationCode.Should().Be(_guid);
        }

        class When_call_SaveChanges_for_one_simple_and_four_replicable_entities : EFDomainContextMockContext
        {
            Establish context = () =>
                {
                    var advertisement = new Advertisement();
                    var deal = new Deal();
                    var order = new Order();
                    var orderPosition = new OrderPosition();
                    var firm = new Firm();

                    ObjectContextMock.Setup(p => p.Entries())
                                     .Returns(new[]
                                         {
                                             new StubEntityEntry(advertisement, EntityState.Added), 
                                             new StubEntityEntry(deal, EntityState.Added),
                                             new StubEntityEntry(order, EntityState.Modified),
                                             new StubEntityEntry(orderPosition, EntityState.Deleted),
                                             new StubEntityEntry(firm, EntityState.Unchanged)
                                         })
                                     .Verifiable();

                    _domainContext = new EFDomainContext(Mock.Of<IProcessingContext>(),
                                                     DefaultContextName,
                                                     ObjectContextMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     _enabledReplicationMetadataProvider,
                                                     Mock.Of<ITracer>());
                };

            Because of = () => _domainContext.SaveChanges(SaveOptions.None);

            It replicate_deal_stored_proc_should_be_called_once = () =>
                ObjectContextMock.Verify(x => x.ExecuteSql(Moq.It.Is<string>(y => y == "Erm.ReplicateDeal"), Moq.It.IsAny<object[]>()),
                                         Times.Once());

            It replicate_order_stored_proc_should_be_called_once = () =>
                ObjectContextMock.Verify(x => x.ExecuteSql(Moq.It.Is<string>(y => y == "Erm.ReplicateOrder"), Moq.It.IsAny<object[]>()),
                                         Times.Once());

            It replicate_order_position_stored_proc_should_be_called_once = () =>
                ObjectContextMock.Verify(x => x.ExecuteSql(Moq.It.Is<string>(y => y == "Erm.ReplicateOrderPosition"), Moq.It.IsAny<object[]>()),
                                         Times.Once());

            It should_be_two_replicate_stored_procs_called = () =>
                ObjectContextMock.Verify(x => x.ExecuteSql(Moq.It.IsAny<string>(), Moq.It.IsAny<object[]>()),
                                         Times.Exactly(3));
        }

        class When_call_SaveChanges_with_disabled_EnableReplication_param : EFDomainContextMockContext
        {

            Establish context = () =>
                {
                    var deal = new Deal();
                    var order = new Order();
                    var orderPosition = new OrderPosition();
               
                    ObjectContextMock.Setup(p => p.Entries())
                                     .Returns(new[]
                                         {
                                             new StubEntityEntry(deal, EntityState.Added),
                                             new StubEntityEntry(order, EntityState.Modified),
                                             new StubEntityEntry(orderPosition, EntityState.Deleted)
                                         });

                    _domainContext = new EFDomainContext(Mock.Of<IProcessingContext>(),
                                                         DefaultContextName,
                                                         ObjectContextMock.Object,
                                                         Mock.Of<IPendingChangesHandlingStrategy>(),
                                                         _disabledReplicationMetadataProvider,
                                                         Mock.Of<ITracer>());
                };

            Because of = () => _domainContext.SaveChanges(SaveOptions.None);

            It any_replicate_stored_procs_should_not_be_called = () =>
                ObjectContextMock.Verify(x => x.ExecuteSql(Moq.It.IsAny<string>(), Moq.It.IsAny<object[]>()), Times.Never());
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
                                                         DefaultContextName,
                                                         objectContextMock.Object,
                                                         new NullPendingChangesHandlingStrategy(),
                                                         _disabledReplicationMetadataProvider,
                                                         Mock.Of<ITracer>());
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
                                                     DefaultContextName,
                                                     objectContextMock.Object,
                                                     new ForcePendingChangesHandlingStrategy(),
                                                    _disabledReplicationMetadataProvider,
                                                     Mock.Of<ITracer>());
            };

            Because of = () => _exception = Catch.Exception(() => _domainContext.Dispose());

            It exception_of_type_PendingChangesNotHandledException_should_be_thrown = () => _exception.Should().BeOfType<PendingChangesNotHandledException>();
        }
    }
}