using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.EntityTypes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories;

using FluentAssertions;

using Machine.Specifications;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class UnitOfWorkScopeSpecs
    {
        static IUnitOfWorkScope _unitOfWorkScope;

        [Tags("DAL")]
        [Subject(typeof(UnitOfWorkScope))]
        public abstract class StubUnitOfWorkContext
        {
            Establish context = () =>
                {
                    Factories = new Dictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IAggregateRepository>>
                        {
                            {
                                typeof(IConcreteAggregateRepository),
                                (readContextProvider, modifiableContextProvider) =>
                                new ConcreteAggregateRepository(new StubFinder(readContextProvider),
                                                                new StubEntityRepository<ErmScopeEntity1>(readContextProvider,
                                                                                                          modifiableContextProvider),
                                                                new StubEntityRepository<ErmScopeEntity2>(readContextProvider,
                                                                                                          modifiableContextProvider))
                            }
                        };

                    UnitOfWork = new StubUnitOfWork(Factories,
                                                    new StubDomainContext(),
                                                    new StubDomainContextFactory(),
                                                    new NullPendingChangesHandlingStrategy(),
                                                    new NullLogger());

                    _unitOfWorkScope = UnitOfWork.CreateScope();
                };

            protected static IUnitOfWork UnitOfWork { get; private set; }
            protected static Dictionary<Type, Func<IReadDomainContextProvider, IModifiableDomainContextProvider, IAggregateRepository>> Factories { get; private set; }
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWorkScope))]
        class When_creating_with_null_as_IUnitOfWork_constructor_param
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() => new UnitOfWorkScope(null,
                                                                                              Moq.It.IsAny<IAggregateRepositoryForHostFactory>(),
                                                                                              Moq.It.IsAny<IPendingChangesHandlingStrategy>()));

            It exception_of_type_ArgumentNullException_should_be_thrown = () => _exception.Should().BeOfType<ArgumentNullException>();
        }

        [Tags("DAL")]
        [Subject(typeof(UnitOfWorkScope))]
        class When_creating_with_null_as_IAggregateRepositoryForHostFactory_constructor_param
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() => new UnitOfWorkScope(Moq.It.IsAny<IUnitOfWork>(),
                                                                                              null,
                                                                                              Moq.It.IsAny<IPendingChangesHandlingStrategy>()));

            It exception_of_type_ArgumentNullException_should_be_thrown = () => _exception.Should().BeOfType<ArgumentNullException>();
        }

        class When_created : StubUnitOfWorkContext
        {
            It scope_Id_should_be_not_equal_to_parent_unit_of_work_scope_Id = () => _unitOfWorkScope.ScopeId.Should().NotBe(((IDomainContextHost)UnitOfWork).ScopeId);
        }

        class When_creating_two_instances : StubUnitOfWorkContext
        {
            static IUnitOfWorkScope _unitOfWorkScope2;

            Because of = () => _unitOfWorkScope2 = UnitOfWork.CreateScope();

            It their_scope_Ids_should_be_not_equal = () => _unitOfWorkScope.ScopeId.Should().NotBe(_unitOfWorkScope2.ScopeId);
        }

        class When_call_CreateRepository_twice_for_the_same_aggregate_repository_interface : StubUnitOfWorkContext
        {
            static ConcreteAggregateRepository _repository1;
            static ConcreteAggregateRepository _repository2;

            Because of = () =>
                {
                    _repository1 = (ConcreteAggregateRepository)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();
                    _repository2 = (ConcreteAggregateRepository)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();
                };

            It instances_of_created_aggregate_repositories_should_be_different = () => _repository1.Should().NotBe(_repository2);

            It modifiable_domain_context_of_entity_repository_1_of_aggregate_repository_1_should_be_the_same_as_modifiable_domain_context_of_entity_repository_1_of_aggregate_repository_2
                    = () => _repository1.EntityRepositoryType1.UsedModifiableDomainContext.Should().Be(_repository2.EntityRepositoryType1.UsedModifiableDomainContext);

            It modifiable_domain_context_of_entity_repository_2_of_aggregate_repository_1_should_be_the_same_as_modifiable_domain_context_of_entity_repository_2_of_aggregate_repository_2
                    = () => _repository1.EntityRepositoryType2.UsedModifiableDomainContext.Should().Be(_repository2.EntityRepositoryType2.UsedModifiableDomainContext);
        }

        class When_call_CreateRepository_for_the_different_aggregate_repository_interfaces : StubUnitOfWorkContext
        {
            static ConcreteAggregateRepository1 _repository1;
            static ConcreteAggregateRepository2 _repository2;

            Establish context = () =>
                {
                    Factories.Add(typeof(IConcreteAggregateRepository1),
                                  (readContextProvider, modifiableContextProvider) =>
                                  new ConcreteAggregateRepository1(new StubFinder(readContextProvider),
                                                                   new StubEntityRepository<ErmScopeEntity1>(readContextProvider,
                                                                                                             modifiableContextProvider),
                                                                   new StubEntityRepository<ErmScopeEntity2>(readContextProvider,
                                                                                                             modifiableContextProvider)));
                    Factories.Add(typeof(IConcreteAggregateRepository2),
                                  (readContextProvider, modifiableContextProvider) =>
                                  new ConcreteAggregateRepository2(new StubFinder(readContextProvider),
                                                                   new StubEntityRepository<ErmScopeEntity2>(readContextProvider,
                                                                                                             modifiableContextProvider),
                                                                   new StubEntityRepository<ErmScopeEntity1>(readContextProvider,
                                                                                                             modifiableContextProvider)));
                };

            Because of = () =>
                {
                    _repository1 = (ConcreteAggregateRepository1)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository1>();
                    _repository2 = (ConcreteAggregateRepository2)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository2>();
                };

            It read_domain_context_of_entity_repository_1_of_aggregate_repository_1_should_be_the_same_as_read_domain_context_of_entity_repository_2_of_aggregate_repository_1
                    = () => _repository1.EntityRepositoryType1.UsedReadDomainContext.Should().Be(_repository1.EntityRepositoryType2.UsedReadDomainContext);

            It read_domain_context_of_entity_repository_1_of_aggregate_repository_2_should_be_the_same_as_read_domain_context_of_entity_repository_2_of_aggregate_repository_2
                    = () => _repository2.EntityRepositoryType1.UsedReadDomainContext.Should().Be(_repository2.EntityRepositoryType2.UsedReadDomainContext);

            It read_domain_context_of_entity_repository_1_of_aggregate_repository_1_should_be_the_same_as_read_domain_context_of_entity_repository_1_of_aggregate_repository_2
                    = () => _repository1.EntityRepositoryType1.UsedReadDomainContext.Should().Be(_repository2.EntityRepositoryType1.UsedReadDomainContext);

            It modifiable_domain_context_of_entity_repository_1_of_aggregate_repository_1_should_not_be_the_same_as_modifiable_domain_context_of_entity_repository_2_of_aggregate_repository_1
                    = () => _repository1.EntityRepositoryType1.UsedModifiableDomainContext.Should().NotBe(_repository1.EntityRepositoryType2.UsedModifiableDomainContext);

            It modifiable_domain_context_of_entity_repository_1_of_aggregate_repository_2_should_not_be_the_same_as_modifiable_domain_context_of_entity_repository_2_of_aggregate_repository_2
                    = () => _repository2.EntityRepositoryType1.UsedModifiableDomainContext.Should().NotBe(_repository2.EntityRepositoryType2.UsedModifiableDomainContext);

            It modifiable_domain_context_of_entity_repository_1_of_aggregate_repository_1_should_not_be_the_same_as_modifiable_domain_context_of_entity_repository_1_of_aggregate_repository_2
                    = () => _repository1.EntityRepositoryType1.UsedModifiableDomainContext.Should().NotBe(_repository2.EntityRepositoryType1.UsedModifiableDomainContext);
        }

        class When_entity_repository_changes_is_saved_but_Complete_is_not_called : StubUnitOfWorkContext
        {
            static ConcreteAggregateRepository _repository;

            Establish context = () =>
                {
                    _repository = (ConcreteAggregateRepository)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();

                    _repository.EntityRepositoryType1.Save();
                    _repository.EntityRepositoryType2.Save();
                };

            It modifiable_domain_context_of_entity_repository_1_changes_should_not_be_accepted =
                () => _repository.EntityRepositoryType1.UsedModifiableDomainContext.IsChangesAccepted.Should().Be(false);

            It modifiable_domain_context_of_entity_repository_2_changes_should_not_be_accepted =
                () => _repository.EntityRepositoryType2.UsedModifiableDomainContext.IsChangesAccepted.Should().Be(false);

            It modifiable_domain_context_of_entity_repository_1_changes_should_not_be_saved =
                () => _repository.EntityRepositoryType1.UsedModifiableDomainContext.IsChangesSaved.Should().Be(false);

            It modifiable_domain_context_of_entity_repository_2_changes_should_not_be_saved =
                () => _repository.EntityRepositoryType2.UsedModifiableDomainContext.IsChangesSaved.Should().Be(false);
        }

        class When_entity_repository_changes_is_saved_and_Complete_is_called : StubUnitOfWorkContext
        {
            static ConcreteAggregateRepository _repository;

            Establish context = () =>
                {
                    _repository = (ConcreteAggregateRepository)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();

                    _repository.EntityRepositoryType1.Save();
                    _repository.EntityRepositoryType2.Save();

                    _unitOfWorkScope.Complete();
                };

            It modifiable_domain_context_of_entity_repository_1_changes_should_be_accepted =
                () => _repository.EntityRepositoryType1.UsedModifiableDomainContext.IsChangesAccepted.Should().Be(true);

            It modifiable_domain_context_of_entity_repository_2_changes_should_be_accepted =
                () => _repository.EntityRepositoryType2.UsedModifiableDomainContext.IsChangesAccepted.Should().Be(true);

            It modifiable_domain_context_of_entity_repository_1_changes_should_be_saved =
                () => _repository.EntityRepositoryType1.UsedModifiableDomainContext.IsChangesSaved.Should().Be(true);

            It modifiable_domain_context_of_entity_repository_2_changes_should_be_saved =
                () => _repository.EntityRepositoryType2.UsedModifiableDomainContext.IsChangesSaved.Should().Be(true);
        }

        class When_call_Complete_on_disposed_object : StubUnitOfWorkContext
        {
            static Exception _exception;

            Establish context = () =>
                {
                    _unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();
                    _unitOfWorkScope.Dispose();
                };

            Because of = () => _exception = Catch.Exception(() => _unitOfWorkScope.Complete());

            It exception_of_type_ObjectDisposedException_should_be_thrown = () => _exception.Should().BeOfType<ObjectDisposedException>();
        }

        class When_Complete_is_called_in_empty_scope : StubUnitOfWorkContext
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() => _unitOfWorkScope.Complete());

            It exception_should_not_be_thrown = () => _exception.Should().Be(null);
        }

        class When_Disposing_twice : StubUnitOfWorkContext
        {
            static Exception _exception;

            Because of = () => _exception = Catch.Exception(() =>
                {
                    _unitOfWorkScope.Dispose();
                    _unitOfWorkScope.Dispose();
                });

            It exception_should_not_be_thrown = () => _exception.Should().BeNull();
        }

        class When_Disposing_whole_scope_with_aggregate_repository_created_in : StubUnitOfWorkContext
        {
            static ConcreteAggregateRepository _repository;

            Establish context = () =>
                {
                    _repository = (ConcreteAggregateRepository)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();

                    _repository.EntityRepositoryType1.Save();
                    _repository.EntityRepositoryType2.Save();
                };

            Because of = () => _unitOfWorkScope.Dispose();

            It modifiable_domain_context_of_entity_repository_1_should_be_disposed =
                () => _repository.EntityRepositoryType1.UsedModifiableDomainContext.IsDisposed.Should().Be(true);

            It modifiable_domain_context_of_entity_repository_2_should_be_disposed =
                () => _repository.EntityRepositoryType2.UsedModifiableDomainContext.IsDisposed.Should().Be(true);
        }

        class When_Disposing_with_NullPendingChangesHandlingStrategy_and_Complete_is_not_called : StubUnitOfWorkContext
        {
            static Exception _exception;

            Establish context = () =>
            {
                var repository = (ConcreteAggregateRepository)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();

                repository.EntityRepositoryType1.Save();
                repository.EntityRepositoryType2.Save();
            };
            
            Because of = () => _exception = Catch.Exception(() => _unitOfWorkScope.Dispose());

            It exception_should_not_be_thrown = () => _exception.Should().Be(null);
        }

        class When_Disposing_with_ForcePendingChangesHandlingStrategy_and_Complete_is_not_called : StubUnitOfWorkContext
        {
            static Exception _exception;

            Establish context = () =>
                {
                    IUnitOfWork unitOfWork = new StubUnitOfWork(Factories,
                                                        new StubDomainContext(),
                                                        new StubDomainContextFactory(),
                                                        new ForcePendingChangesHandlingStrategy(),
                                                        new NullLogger());

                    _unitOfWorkScope = unitOfWork.CreateScope();

                    var repository = (ConcreteAggregateRepository)_unitOfWorkScope.CreateRepository<IConcreteAggregateRepository>();

                    repository.EntityRepositoryType1.Save();
                    repository.EntityRepositoryType2.Save();
                };

            Because of = () => _exception = Catch.Exception(() => _unitOfWorkScope.Dispose());

            It exception_should_be_thrown = () => _exception.Should().BeOfType<PendingChangesNotHandledException>();
        }
    }
}