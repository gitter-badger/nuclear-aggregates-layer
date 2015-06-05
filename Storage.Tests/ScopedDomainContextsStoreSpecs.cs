using System.Collections.Generic;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities.Aspects;
using NuClear.Storage.Core;

using Storage.Tests.EntityTypes;
using Storage.Tests.Fakes;

using It = Machine.Specifications.It;

namespace Storage.Tests
{
    public class ScopedDomainContextsStoreSpecs
    {
        [Tags("Storage")]
        [Subject(typeof(ScopedDomainContextsStore))]
        class When_call_GetReadable_without_scope_and_inside_scope
        {
            static ScopedDomainContextsStore _scopedDomainContextsStore;
            static IDomainContextHost _domainContextHost;
            static IDomainContextsScope _domainContextHostScope;
            static IDomainContextsScopeFactory _domainContextsScopeFactory;

            static IReadableDomainContext _readableDomainContext1;
            static IReadableDomainContext _readableDomainContext2;

            Establish context = () =>
                    {
                        _scopedDomainContextsStore = new ScopedDomainContextsStore(new StubDomainContext(), new StubDomainContextFactory());
                        _domainContextHost = new DomainContextHost(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
                        _domainContextsScopeFactory = (IDomainContextsScopeFactory)_domainContextHost;
                    };

            Because of = () =>
            {
                _readableDomainContext1 = _scopedDomainContextsStore.GetReadable(_domainContextHost);
                using (_domainContextHostScope = _domainContextsScopeFactory.CreateScope())
                {
                    _readableDomainContext2 = _scopedDomainContextsStore.GetReadable(_domainContextHostScope);
                }
            };

            It read_domain_contexts_should_be_the_same = () => _readableDomainContext1.Should().BeSameAs(_readableDomainContext2);
            It scope_Ids_should_be_not_equal = () => _domainContextHost.ScopeId.Should().NotBe(_domainContextHostScope.ScopeId);
        }

        [Tags("Storage")]
        [Subject(typeof(ScopedDomainContextsStore))]
        class When_call_GetModifiable
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
            static ScopedDomainContextsStore _scopedDomainContextsStore;
            static IDomainContextHost _domainContextHost;

            Establish context = () =>
                    {
                        ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Verifiable();
                        _scopedDomainContextsStore = new ScopedDomainContextsStore(Mock.Of<IReadableDomainContext>(), ModifiableDomainContextFactoryMock.Object);
                        _domainContextHost = new DomainContextHost(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());

                    };

            Because of = () => _scopedDomainContextsStore.GetModifiable<IEntity>(_domainContextHost);

            It modifiable_domain_context_factory_Create_method_should_be_called = () => ModifiableDomainContextFactoryMock.Verify();
        }

        [Tags("Storage")]
        [Subject(typeof(ScopedDomainContextsStore))]
        class When_call_GetModifiable_twice_for_the_same_type
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
            static ScopedDomainContextsStore _scopedDomainContextsStore;
            static IDomainContextHost _domainContextHost;
            static IModifiableDomainContext _modifiableDomainContext1;
            static IModifiableDomainContext _modifiableDomainContext2;

            Establish context = () =>
                    {
                        ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Verifiable();
                        _scopedDomainContextsStore = new ScopedDomainContextsStore(Mock.Of<IReadableDomainContext>(), ModifiableDomainContextFactoryMock.Object);
                        _domainContextHost = new DomainContextHost(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
                    };

            Because of = () =>
            {
                _modifiableDomainContext1 = _scopedDomainContextsStore.GetModifiable<IEntity>(_domainContextHost);
                _modifiableDomainContext1 = _scopedDomainContextsStore.GetModifiable<IEntity>(_domainContextHost);
            };

            It modifiable_domain_context_factory_Create_method_should_be_called_only_once =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<IEntity>(), Times.Once());
            It resulting_modifiable_domain_contexts_should_be_the_same = () => _modifiableDomainContext1.Should().BeSameAs(_modifiableDomainContext2);
        }

        [Tags("Storage")]
        [Subject(typeof(ScopedDomainContextsStore))]
        class When_call_GetModifiable_twice_for_the_same_type_in_different_scopes
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
            static ScopedDomainContextsStore _scopedDomainContextsStore;
            static IDomainContextHost _domainContextHost;
            static IDomainContextsScopeFactory _domainContextsScopeFactory;

            Establish context = () =>
                    {
                        ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Verifiable();
                        _scopedDomainContextsStore = new ScopedDomainContextsStore(Mock.Of<IReadableDomainContext>(), ModifiableDomainContextFactoryMock.Object);
                        _domainContextHost = new DomainContextHost(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
                        _domainContextsScopeFactory = (IDomainContextsScopeFactory)_domainContextHost;
                    };

            private Because of = () =>
                                     {
                                         _scopedDomainContextsStore.GetModifiable<IEntity>(_domainContextHost);
                                         using (var scope = _domainContextsScopeFactory.CreateScope())
                                         {
                                             _scopedDomainContextsStore.GetModifiable<IEntity>(scope);
                                         }
                                     };

            It modifiable_domain_context_factory_Create_method_should_be_called_twice =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<IEntity>(), Times.Exactly(2));

            [Tags("Storage")]
            [Subject(typeof(ScopedDomainContextsStore))]
            class When_call_GetModifiable_twice_for_the_different_types
            {
                static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
                static ScopedDomainContextsStore _scopedDomainContextsStore;
                static IDomainContextHost _domainContextHost;

                Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<Entity1>()).Verifiable();
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<Entity2>()).Verifiable();

                    _scopedDomainContextsStore = new ScopedDomainContextsStore(Mock.Of<IReadableDomainContext>(), ModifiableDomainContextFactoryMock.Object);
                    _domainContextHost = new DomainContextHost(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
                };

                Because of = () =>
                {
                    _scopedDomainContextsStore.GetModifiable<Entity1>(_domainContextHost);
                    _scopedDomainContextsStore.GetModifiable<Entity2>(_domainContextHost);
                };

                It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity1 =
                    () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<Entity1>(), Times.Once());
                It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity2 =
                    () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<Entity2>(), Times.Once());
            }

            [Tags("Storage")]
            [Subject(typeof(ScopedDomainContextsStore))]
            class When_call_GetModifiable_twice_for_the_different_types_in_different_scopes
            {
                static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
                static ScopedDomainContextsStore _scopedDomainContextsStore;
                static IDomainContextHost _domainContextHost;
                static IDomainContextsScopeFactory _domainContextsScopeFactory;

                Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<Entity1>()).Returns(new StubDomainContext()).Verifiable();
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<Entity2>()).Returns(new StubDomainContext()).Verifiable();

                    _scopedDomainContextsStore = new ScopedDomainContextsStore(Mock.Of<IReadableDomainContext>(), ModifiableDomainContextFactoryMock.Object);
                    _domainContextHost = new DomainContextHost(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
                    _domainContextsScopeFactory = (IDomainContextsScopeFactory)_domainContextHost;
                };

                Because of = () =>
                {
                    _scopedDomainContextsStore.GetModifiable<Entity1>(_domainContextHost);
                    using (var scope = _domainContextsScopeFactory.CreateScope())
                    {
                        _scopedDomainContextsStore.GetModifiable<Entity2>(scope);
                    }
                };

                It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity1 =
                    () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<Entity1>(), Times.Once());
                It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity2 =
                    () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<Entity2>(), Times.Once());
            }

            [Tags("Storage")]
            [Subject(typeof(ScopedDomainContextsStore))]
            class When_call_DropModifiable_twice
            {
                static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
                static readonly StubDomainContext StubDomainContext = new StubDomainContext();
                static ScopedDomainContextsStore _scopedDomainContextsStore;
                static IDomainContextHost _domainContextHost;

                static IEnumerable<IModifiableDomainContext> _modifiableDomainContexts1;
                static IEnumerable<IModifiableDomainContext> _modifiableDomainContexts2;

                Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Returns(StubDomainContext).Verifiable();

                    _scopedDomainContextsStore = new ScopedDomainContextsStore(Mock.Of<IReadableDomainContext>(), ModifiableDomainContextFactoryMock.Object);
                    _domainContextHost = new DomainContextHost(_scopedDomainContextsStore, new NullPendingChangesHandlingStrategy());
                };

                Because of = () =>
                {
                    _scopedDomainContextsStore.GetModifiable<IEntity>(_domainContextHost);
                    _modifiableDomainContexts1 = _scopedDomainContextsStore.DropModifiable(_domainContextHost);
                    _modifiableDomainContexts1 = _scopedDomainContextsStore.DropModifiable(_domainContextHost);
                };

                It result_1_should_be_of_array_type = () => _modifiableDomainContexts1.Should().BeOfType<IModifiableDomainContext[]>();
                It result_2_should_be_of_array_type = () => _modifiableDomainContexts2.Should().BeOfType<IModifiableDomainContext[]>();

                It result_1_should_not_be_empty = () => _modifiableDomainContexts1.Should().NotBeEmpty();
                It result_2_should_not_be_empty = () => _modifiableDomainContexts2.Should().BeEmpty();

                It result_1_should_not_be__the_same_as_result_2 = () => ((object)_modifiableDomainContexts1).Should().NotBeSameAs(_modifiableDomainContexts2);

                It stub_domain_context_should_be_as_element_in_result_1 = () => _modifiableDomainContexts1.Should().OnlyContain(x => x == StubDomainContext);
            }
        }
    }
}