using System;
using System.Data;
using System.Data.Objects;

using DoubleGis.Erm.Platform.API.Core.Settings.CRM;
using DoubleGis.Erm.Platform.API.Core.UseCases.Context;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.UseCases.Context;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;
using SaveOptions = DoubleGis.Erm.Platform.DAL.SaveOptions;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class EFDomainContextSpecs
    {
        const string DefaultContextName = "Erm";

        static EFDomainContext _domainContext;

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        public abstract class EFDomainContextMockContext
        {
            Establish context = () =>
            {
                MsCrmSettingsMock = new Mock<IMsCrmSettings>();
                MsCrmSettingsMock.Setup(x => x.EnableReplication).Returns(true);
                
                ObjectContextMock  = new Mock<IObjectContext>();

                _domainContext = new EFDomainContext(Mock.Of<IProcessingContext>(),
                                                     DefaultContextName,
                                                     ObjectContextMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     MsCrmSettingsMock.Object,
                                                     Mock.Of<ICommonLog>());
            };

            protected static Mock<IMsCrmSettings> MsCrmSettingsMock { get; private set; }
            protected static Mock<IObjectContext> ObjectContextMock { get; private set; }
        }

        class When_call_SaveChanges_for_added_entities : EFDomainContextMockContext
        {
            static Deal _deal;

            Establish context = () =>
                {
                    _deal = new Deal { ReplicationCode = Guid.Empty };

                    ObjectContextMock.Setup(p => p.GetObjectStateEntries(Moq.It.IsAny<EntityState>()))
                                     .Returns(new[] { new EFEntityStateEntry(_deal, EntityState.Added) });
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

                    ObjectContextMock.Setup(p => p.GetObjectStateEntries(Moq.It.IsAny<EntityState>()))
                                     .Returns(new[] { new EFEntityStateEntry(_deal, EntityState.Modified) });
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

                    ObjectContextMock.Setup(p => p.GetObjectStateEntries(Moq.It.IsAny<EntityState>()))
                                     .Returns(new[]
                                         {
                                             new EFEntityStateEntry(advertisement, EntityState.Added), 
                                             new EFEntityStateEntry(deal, EntityState.Added),
                                             new EFEntityStateEntry(order, EntityState.Modified),
                                             new EFEntityStateEntry(orderPosition, EntityState.Deleted),
                                             new EFEntityStateEntry(firm, EntityState.Unchanged)
                                         })
                                     .Verifiable();
                };

            Because of = () => _domainContext.SaveChanges(SaveOptions.None);

            It replicate_deal_stored_proc_should_be_called_once = () =>
                ObjectContextMock.Verify(x => x.ExecuteFunction(Moq.It.Is<string>(y => y == "Erm.ReplicateDeal"), Moq.It.IsAny<ObjectParameter[]>()),
                                         Times.Once());

            It replicate_order_stored_proc_should_be_called_once = () =>
                ObjectContextMock.Verify(x => x.ExecuteFunction(Moq.It.Is<string>(y => y == "Erm.ReplicateOrder"), Moq.It.IsAny<ObjectParameter[]>()),
                                         Times.Once());

            It replicate_order_position_stored_proc_should_be_called_once = () =>
                ObjectContextMock.Verify(x => x.ExecuteFunction(Moq.It.Is<string>(y => y == "Erm.ReplicateOrderPosition"), Moq.It.IsAny<ObjectParameter[]>()),
                                         Times.Once());

            It should_be_two_replicate_stored_procs_called = () =>
                ObjectContextMock.Verify(x => x.ExecuteFunction(Moq.It.IsAny<string>(), Moq.It.IsAny<ObjectParameter[]>()),
                                         Times.Exactly(3));
        }

        class When_call_SaveChanges_with_disabled_EnableReplication_param : EFDomainContextMockContext
        {
            Establish context = () =>
                {
                    MsCrmSettingsMock.Setup(x => x.EnableReplication).Returns(false);

                    var deal = new Deal();
                    var order = new Order();
                    var orderPosition = new OrderPosition();

                    ObjectContextMock.Setup(p => p.GetObjectStateEntries(Moq.It.IsAny<EntityState>()))
                                     .Returns(new[]
                                         {
                                             new EFEntityStateEntry(deal, EntityState.Added),
                                             new EFEntityStateEntry(order, EntityState.Modified),
                                             new EFEntityStateEntry(orderPosition, EntityState.Deleted)
                                         });
                };

            Because of = () => _domainContext.SaveChanges(SaveOptions.None);

            It any_replicate_stored_procs_should_not_be_called = () => 
                ObjectContextMock.Verify(x => x.ExecuteFunction(Moq.It.IsAny<string>(), Moq.It.IsAny<ObjectParameter[]>()), Times.Never());
        }

        [Tags("DAL")]
        [Subject(typeof(EFDomainContext))]
        class When_Disposing_with_NullPendingChangesHandlingStrategy
        {
            static Exception _exception;

            Establish context = () =>
                {
                    var objectContextMock = new Mock<IObjectContext>();
                    objectContextMock.Setup(x => x.GetObjectStateEntries(Moq.It.IsAny<EntityState>()))
                                     .Returns(new[] { new EFEntityStateEntry(null, Moq.It.IsAny<EntityState>(), false) });

                    _domainContext = new EFDomainContext(new ProcessingContext(),
                                                         DefaultContextName,
                                                         objectContextMock.Object,
                                                         new NullPendingChangesHandlingStrategy(),
                                                         Mock.Of<IMsCrmSettings>(),
                                                         Mock.Of<ICommonLog>());
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
                var objectContextMock = new Mock<IObjectContext>();
                objectContextMock.Setup(x => x.GetObjectStateEntries(Moq.It.IsAny<EntityState>()))
                                 .Returns(new[] { new EFEntityStateEntry(null, Moq.It.IsAny<EntityState>(), false) });

                _domainContext = new EFDomainContext(new ProcessingContext(),
                                                     DefaultContextName,
                                                     objectContextMock.Object,
                                                     new ForcePendingChangesHandlingStrategy(), 
                                                     Mock.Of<IMsCrmSettings>(),
                                                     Mock.Of<ICommonLog>());
            };

            Because of = () => _exception = Catch.Exception(() => _domainContext.Dispose());

            It exception_of_type_PendingChangesNotHandledException_should_be_thrown = () => _exception.Should().BeOfType<PendingChangesNotHandledException>();
        }
    }
}