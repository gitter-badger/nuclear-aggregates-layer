using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates;
using NuClear.Model.Common.Entities.Aspects;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Tracing.API;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    class UnitOfWorkSpecs
    {
        static UnitOfWork _unitOfWork;

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        public abstract class StubUnitOfWorkContext
        {
            Establish context = () =>
                {
                    var factories = new Dictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IAggregateRepository>>
                        {
                            {
                                typeof(ConcreteAggregateRepository),
                                (readContextProvider, modifiableContextProvider) =>
                                new ConcreteAggregateRepository(new StubFinder(readContextProvider),
                                                                new StubEntityRepository<ErmScopeEntity1>(readContextProvider,
                                                                                                          modifiableContextProvider),
                                                                new StubEntityRepository<ErmScopeEntity2>(readContextProvider,
                                                                                                          modifiableContextProvider))
                            }
                        };

                _unitOfWork = new StubUnitOfWork(factories,
                                                 new StubDomainContext(),
                                                 new StubDomainContextFactory(),
                                                 new NullPendingChangesHandlingStrategy(),
                                                 new NullTracer());
            };
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        public abstract class MockUnitOfWorkContext
        {
            Establish context = () =>
                {
                    _unitOfWork = new MockUnitOfWork(
                        (x, y) =>
                            {
                                ReadDomainContextProvider = x;
                                ModifiableDomainContextProvider = y;
                            },
                        Mock.Of<IReadDomainContext>(),
                        Mock.Of<IModifiableDomainContextFactory>(),
                        Mock.Of<IPendingChangesHandlingStrategy>(),
                        Mock.Of<ITracer>());
                };

            protected static IReadDomainContextProvider ReadDomainContextProvider { get; private set; }
            protected static IModifiableDomainContextProvider ModifiableDomainContextProvider { get; private set; }
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_getting_ModifiableDomainContexts : StubUnitOfWorkContext
        {
            static IEnumerable<IModifiableDomainContext> _modifiableDomainContexts;

            Because of = () => _modifiableDomainContexts = ((IUnitOfWork)_unitOfWork).GetModifiableDomainContexts(Mock.Of<IDomainContextHost>());
            It contexts_should_be_empty = () => _modifiableDomainContexts.Should().BeEmpty();
        }

        class When_created : StubUnitOfWorkContext
        {
            Because of = () => { };
            It should_set_ScopeId = () => ((IDomainContextHost)_unitOfWork).ScopeId.Should().NotBe(Guid.Empty);
        }

        class When_call_CreateRepository_with_concrete_aggregate_repository_as_a_parameter : StubUnitOfWorkContext
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() =>
                {
                    var aggregatesLayerRuntimeFactory = _unitOfWork as IAggregatesLayerRuntimeFactory;
                    aggregatesLayerRuntimeFactory.CreateRepository(typeof(ConcreteAggregateRepository));
                });
            It exception_should_not_be_thrown = () => _exception.Should().BeNull();
        }

        class When_call_CreateRepository_with_a_type_that_is_not_aggregate_repository_as_a_parameter : StubUnitOfWorkContext
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() =>
                {
                    var aggregatesLayerRuntimeFactory = _unitOfWork as IAggregatesLayerRuntimeFactory;
                    aggregatesLayerRuntimeFactory.CreateRepository(typeof(StubFinder));
                });

            It exception_should_be_thrown = () => _exception.Should().NotBeNull();
            It exception_should_be_of_type_ArgumentException = () => _exception.Should().BeOfType<ArgumentException>();
        }

        class When_call_CreateRepository_with_interface_as_a_parameter : StubUnitOfWorkContext
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() =>
                {
                    var aggregatesLayerRuntimeFactory = _unitOfWork as IAggregatesLayerRuntimeFactory;
                    aggregatesLayerRuntimeFactory.CreateRepository(typeof(IEnumerable<>));
                });
            It exception_should_be_thrown = () => _exception.Should().NotBeNull();
            It exception_should_be_of_type_ArgumentException = () => _exception.Should().BeOfType<ArgumentException>();
        }

        class When_just_call_CreateRepository : MockUnitOfWorkContext
        {
            Because of = () => _unitOfWork.CreateRepository<IStubSimpleAggregateRepository>();

            It readDomainContextProvider_argument_should_be_of_type_ReadDomainContextProviderProxy =
                () => ReadDomainContextProvider.Should().BeOfType<ReadDomainContextProviderProxy>();

            It modifiableDomainContextProvider_argument_should_be_of_type_ModifiableDomainContextProviderProxy =
                () => ModifiableDomainContextProvider.Should().BeOfType<ModifiableDomainContextProviderProxy>();
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_CreateRepository_using_different_implemented_interfaces
        {
            static UnitOfWork _unitOfWork1;
            static UnitOfWork _unitOfWork2;
            
            Establish context = () =>
                {
                    _unitOfWork1 = new MockUnitOfWork(
                        (x, y) =>
                            {
                            },
                        Mock.Of<IReadDomainContext>(),
                        Mock.Of<IModifiableDomainContextFactory>(),
                        Mock.Of<IPendingChangesHandlingStrategy>(),
                        Mock.Of<ITracer>());
                    _unitOfWork2 = new MockUnitOfWork(
                        (x, y) =>
                            {
                            },
                        Mock.Of<IReadDomainContext>(),
                        Mock.Of<IModifiableDomainContextFactory>(),
                        Mock.Of<IPendingChangesHandlingStrategy>(),
                        Mock.Of<ITracer>());
                };

            Because of = () =>
                {
                    _unitOfWork1.CreateRepository<IStubSimpleAggregateRepository>();
                    ((IAggregateRepositoryForHostFactory)_unitOfWork2).CreateRepository<IStubSimpleAggregateRepository>(_unitOfWork2);
                };
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_Get_throught_IReadDomainContextProviderForHost_interface_without_scope_and_inside_scope
        {
            static IReadDomainContext _readDomainContext1;
            static IReadDomainContext _readDomainContext2;

            Establish context = () =>
                {
                    _unitOfWork = new MockUnitOfWork(Mock.Of<IReadDomainContext>(),
                                                     Mock.Of<IModifiableDomainContextFactory>(),
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     Mock.Of<ITracer>());
                };

            Because of = () =>
                {
                    _readDomainContext1 = ((IReadDomainContextProviderForHost)_unitOfWork).Get(_unitOfWork);
                    using (var scope = ((IUnitOfWork)_unitOfWork).CreateScope())
                    {
                        _readDomainContext2 = ((IReadDomainContextProviderForHost)_unitOfWork).Get(scope);
                    }
                };

            It read_domain_contexts_should_be_the_same = () => _readDomainContext1.Should().BeSameAs(_readDomainContext2);
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_Get_throught_IModifiableDomainContextProviderForHost_interface
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();

            Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Verifiable();

                    _unitOfWork = new MockUnitOfWork(Mock.Of<IReadDomainContext>(),
                                                     ModifiableDomainContextFactoryMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     Mock.Of<ITracer>());
                };

            Because of = () => ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<IEntity>(_unitOfWork);

            It modifiable_domain_context_factory_Create_method_should_be_called = () => ModifiableDomainContextFactoryMock.Verify();
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_Get_throught_IModifiableDomainContextProviderForHost_interface_twice_for_the_same_type
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
            static IModifiableDomainContext _modifiableDomainContext1;
            static IModifiableDomainContext _modifiableDomainContext2;

            Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Returns(new StubDomainContext()).Verifiable();

                    _unitOfWork = new MockUnitOfWork(Mock.Of<IReadDomainContext>(),
                                                     ModifiableDomainContextFactoryMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     Mock.Of<ITracer>());
                };

            Because of = () =>
                {
                    _modifiableDomainContext1 = ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<IEntity>(_unitOfWork);
                    _modifiableDomainContext2 = ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<IEntity>(_unitOfWork);
                };

            It modifiable_domain_context_factory_Create_method_should_be_called_only_once =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<IEntity>(), Times.Once());
            It resulting_modifiable_domain_contexts_should_be_the_same = () => _modifiableDomainContext1.Should().BeSameAs(_modifiableDomainContext2);
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_Get_throught_IModifiableDomainContextProviderForHost_interface_twice_for_the_same_type_in_different_scopes
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();

            Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Returns(new StubDomainContext()).Verifiable();

                    _unitOfWork = new MockUnitOfWork(Mock.Of<IReadDomainContext>(),
                                                     ModifiableDomainContextFactoryMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     Mock.Of<ITracer>());
                };

            Because of = () =>
                {
                    ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<IEntity>(_unitOfWork);
                    using (var scope = ((IUnitOfWork)_unitOfWork).CreateScope())
                    {
                        ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<IEntity>(scope);
                    }
                };

            It modifiable_domain_context_factory_Create_method_should_be_called_twice =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<IEntity>(), Times.Exactly(2));
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_Get_throught_IModifiableDomainContextProviderForHost_interface_twice_for_the_different_types
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();

            Establish context = () =>
            {
                ModifiableDomainContextFactoryMock.Setup(x => x.Create<ErmScopeEntity1>()).Verifiable();
                ModifiableDomainContextFactoryMock.Setup(x => x.Create<ErmScopeEntity2>()).Verifiable();

                _unitOfWork = new MockUnitOfWork(Mock.Of<IReadDomainContext>(),
                                                 ModifiableDomainContextFactoryMock.Object,
                                                 Mock.Of<IPendingChangesHandlingStrategy>(),
                                                 Mock.Of<ITracer>());
            };

            Because of = () =>
                {
                    ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<ErmScopeEntity1>(_unitOfWork);
                    ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<ErmScopeEntity2>(_unitOfWork);
                };

            It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity1 =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<ErmScopeEntity1>(), Times.Once());
            It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity2 =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<ErmScopeEntity2>(), Times.Once());
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_Get_throught_IModifiableDomainContextProviderForHost_interface_twice_for_the_different_types_in_different_scopes
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();

            Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<ErmScopeEntity1>()).Returns(new StubDomainContext()).Verifiable();
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<ErmScopeEntity2>()).Returns(new StubDomainContext()).Verifiable();

                    _unitOfWork = new MockUnitOfWork(Mock.Of<IReadDomainContext>(),
                                                     ModifiableDomainContextFactoryMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     Mock.Of<ITracer>());
                };

            Because of = () =>
                {
                    ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<ErmScopeEntity1>(_unitOfWork);
                    using (var scope = ((IUnitOfWork)_unitOfWork).CreateScope())
                    {
                        ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<ErmScopeEntity2>(scope);
                    }
                };

            It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity1 =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<ErmScopeEntity1>(), Times.Once());
            It modifiable_domain_context_factory_Create_method_should_be_called_only_once_for_ErmScopeEntity2 =
                () => ModifiableDomainContextFactoryMock.Verify(x => x.Create<ErmScopeEntity2>(), Times.Once());
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWork))]
        class When_call_GetModifiableDomainContexts_throught_IModifiableDomainContextProviderForHost_interface_twice
        {
            static readonly Mock<IModifiableDomainContextFactory> ModifiableDomainContextFactoryMock = new Mock<IModifiableDomainContextFactory>();
            static readonly StubDomainContext StubDomainContext = new StubDomainContext();

            static IEnumerable<IModifiableDomainContext> _modifiableDomainContexts1;
            static IEnumerable<IModifiableDomainContext> _modifiableDomainContexts2;

            Establish context = () =>
                {
                    ModifiableDomainContextFactoryMock.Setup(x => x.Create<IEntity>()).Returns(StubDomainContext).Verifiable();

                    _unitOfWork = new MockUnitOfWork(Mock.Of<IReadDomainContext>(),
                                                     ModifiableDomainContextFactoryMock.Object,
                                                     Mock.Of<IPendingChangesHandlingStrategy>(),
                                                     Mock.Of<ITracer>());
                };

            Because of = () =>
                {
                    ((IModifiableDomainContextProviderForHost)_unitOfWork).Get<IEntity>(_unitOfWork);

                    _modifiableDomainContexts1 = ((IUnitOfWork)_unitOfWork).GetModifiableDomainContexts(_unitOfWork);
                    _modifiableDomainContexts2 = ((IUnitOfWork)_unitOfWork).GetModifiableDomainContexts(_unitOfWork);
                };

            It result_1_should_be_of_array_type = () => _modifiableDomainContexts1.Should().BeOfType<IModifiableDomainContext[]>();
            It result_2_should_be_of_array_type = () => _modifiableDomainContexts2.Should().BeOfType<IModifiableDomainContext[]>();

            It result_1_should_not_be_empty = () => _modifiableDomainContexts1.Should().NotBeEmpty();
            It result_2_should_not_be_empty = () => _modifiableDomainContexts2.Should().NotBeEmpty();

            It result_1_should_not_be__the_same_as_result_2 = () => ((object)_modifiableDomainContexts1).Should().NotBeSameAs(_modifiableDomainContexts2);

            It stub_domain_context_should_be_as_element_in_result_1 = () => _modifiableDomainContexts1.Should().OnlyContain(x => x == StubDomainContext);
            It stub_domain_context_should_be_as_element_in_result_2 = () => _modifiableDomainContexts2.Should().OnlyContain(x => x == StubDomainContext);
        }
    }
}