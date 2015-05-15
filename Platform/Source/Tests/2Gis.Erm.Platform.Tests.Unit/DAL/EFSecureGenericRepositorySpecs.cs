using System;
using System.ServiceModel.Security;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.DAL.EntityFramework;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes.Repositories;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    public class EFSecureGenericRepositorySpecs
    {
        [Tags("DAL")]
        [Subject(typeof(EFSecureGenericRepository<>))]
        public abstract class SecureRepositoryContext<TEntity> where TEntity : class, IEntity, IEntityKey, ICuratedEntity
        {
            protected static Exception _exception;

            Establish context = () =>
                {
                    EntityAccessServiceMock = new Mock<ISecurityServiceEntityAccessInternal>();
                    SecureRepository = new EFSecureGenericRepository<TEntity>(new StubUserContext(),
                                                                              new StubGenericRepository<TEntity>(),
                                                                              EntityAccessServiceMock.Object);
                };

            protected static Mock<ISecurityServiceEntityAccessInternal> EntityAccessServiceMock { get; private set; }
            protected static EFSecureGenericRepository<TEntity> SecureRepository { get; private set; }
        }

        [Tags("DAL")]
        [Subject(typeof(EFSecureGenericRepository<>))]
        public abstract class SecureRepositoryWithSkipEntityAccessCheckContext<TEntity> where TEntity : class, IEntity, IEntityKey, ICuratedEntity
        {
            protected static Exception _exception;

            Establish context = () => SecureRepository = new EFSecureGenericRepository<TEntity>(new StubUserContext(true),
                                                                                                new StubGenericRepository<TEntity>(),
                                                                                                Mock.Of<ISecurityServiceEntityAccessInternal>());

            protected static EFSecureGenericRepository<TEntity> SecureRepository { get; private set; }
        }

        [Behaviors]
        class ShouldNotThrowExceptionBehavior
        {
            protected static Exception _exception;

            It should_not_throw_exception = () => _exception.Should().Be(null);
        }

        [Behaviors]
        class ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown
        {
            protected static Exception _exception;

            It exception_of_type_SecurityAccessDeniedException_should_be_thrown = () => _exception.Should().BeOfType<SecurityAccessDeniedException>();
        }

        class When_call_Add_without_create_entity_access_type : SecureRepositoryContext<Order>
        {
            Because of = () => _exception = Catch.Exception(() => SecureRepository.Add(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Update_without_update_entity_access_type : SecureRepositoryContext<Order>
        {
            Because of = () => _exception = Catch.Exception(() => SecureRepository.Update(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Delete_without_delete_entity_access_type : SecureRepositoryContext<Order>
        {
            Because of = () => _exception = Catch.Exception(() => SecureRepository.Delete(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Add_with_create_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                            Moq.It.IsAny<EntityAccessTypes>(),
                                                                                            Moq.It.IsAny<long>(),
                                                                                            Moq.It.IsAny<long?>(),
                                                                                            Moq.It.IsAny<long>(),
                                                                                            Moq.It.IsAny<long?>()))
                                                         .Returns(EntityAccessTypes.Create);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Add(new Order()));

            Behaves_like<ShouldNotThrowExceptionBehavior> should_not_throw_exception_spec;
        }

        class When_call_Update_with_update_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.Update);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Update(new Order()));

            Behaves_like<ShouldNotThrowExceptionBehavior> should_not_throw_exception_spec;
        }

        class When_call_Delete_with_delete_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.Delete);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Delete(new Order()));

            Behaves_like<ShouldNotThrowExceptionBehavior> should_not_throw_exception_spec;
        }

        class When_call_Add_without_Update_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.Update);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Add(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Update_without_Delete_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.Delete);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Update(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Delete_without_Create_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.Create);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Delete(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Add_without_Assign_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.Assign);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Add(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Update_without_Append_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.Append);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Update(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Delete_without_None_entity_access_type : SecureRepositoryContext<Order>
        {
            Establish context = () => EntityAccessServiceMock.Setup(x => x.RestrictEntityAccess(EntityType.Instance.Order(),
                                                                                                Moq.It.IsAny<EntityAccessTypes>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>(),
                                                                                                Moq.It.IsAny<long>(),
                                                                                                Moq.It.IsAny<long?>()))
                                                             .Returns(EntityAccessTypes.None);

            Because of = () => _exception = Catch.Exception(() => SecureRepository.Delete(new Order()));

            Behaves_like<ExceptionOfTypeSecurityAccessDeniedExceptionShouldBeThrown> exception_of_type_SecurityAccessDeniedException_should_be_thrown_spec;
        }

        class When_call_Add_without_create_entity_access_type_and_with_enabled_entity_access_check : SecureRepositoryWithSkipEntityAccessCheckContext<Order>
        {
            Because of = () => _exception = Catch.Exception(() => SecureRepository.Add(new Order()));

            Behaves_like<ShouldNotThrowExceptionBehavior> should_not_throw_exception_spec;
        }

        class When_call_Update_without_update_entity_access_type_and_with_enabled_entity_access_check : SecureRepositoryWithSkipEntityAccessCheckContext<Order>
        {
            Because of = () => _exception = Catch.Exception(() => SecureRepository.Update(new Order()));

            Behaves_like<ShouldNotThrowExceptionBehavior> should_not_throw_exception_spec;
        }

        class When_call_Delete_without_delete_entity_access_type_and_with_enabled_entity_access_check : SecureRepositoryWithSkipEntityAccessCheckContext<Order>
        {
            Because of = () => _exception = Catch.Exception(() => SecureRepository.Delete(new Order()));

            Behaves_like<ShouldNotThrowExceptionBehavior> should_not_throw_exception_spec;
        }
    }
}