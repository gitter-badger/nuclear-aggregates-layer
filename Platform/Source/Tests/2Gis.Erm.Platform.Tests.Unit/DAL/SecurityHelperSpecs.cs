using System;
using System.ServiceModel.Security;

using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Entities.Aspects;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    [Tags("DAL")]
    [Subject(typeof(SecurityHelper))]
    public class SecurityHelperSpecs
    {
        class CheckRequestSecurityHelperContext : SecurityHelperContext
        {
            protected const EntityAccessTypes OperationType = EntityAccessTypes.Assign;

            Establish context = () => SkipEntityAccess(false);

            protected static void SetUpRestrictEntityAccess(EntityAccessTypes allowedOperation)
            {
                // COMMENT {f.zaharov, 03.10.2013}: Немного отформатировал код здесь, давай плз предерживаться такого стиля форматирования
                // DONE {d.ivanov, 03.10.2013}: Ок. Опять же это CleanCode, очень хочу автоматизировать этот процесс, если так и не смогу подогнать, то откажусь от него.
                _entityAccessService.Setup(e => e.RestrictEntityAccess(Entity.GetType().AsEntityName(),
                                                                       OperationType,
                                                                       _mockUserContext.Object.Identity.Code,
                                                                       Entity.Id,
                                                                       Entity.OwnerCode,
                                                                       ((ICuratedEntity)Entity).OldOwnerCode))
                                    .Returns(allowedOperation);
            }

            protected static void TargetCheckRequest()
            {
                Target.CheckRequest(OperationType, Entity);
            }
        }

        class IsEntityAccessTypeGrantedSecurityHelperContext : SecurityHelperContext
        {
            protected const EntityAccessTypes GrantedEntityAccess = EntityAccessTypes.Assign;

            Establish context = () => _entityAccessService
                                          .Setup(e => e.GetCommonEntityAccessForMetadata(Entity.GetType().AsEntityName(), _mockUserContext.Object.Identity.Code))
                                          .Returns(GrantedEntityAccess);
        }

        class SecurityHelperContext
        {
            protected static readonly Deal Entity = new Deal();

            protected static Mock<ISecurityServiceEntityAccessInternal> _entityAccessService;
            protected static MockUserContext _mockUserContext;

            Establish context = () =>
                {
                    _mockUserContext = new MockUserContext();

                    _entityAccessService = new Mock<ISecurityServiceEntityAccessInternal>();

                    Target = new SecurityHelper(_mockUserContext.Object, _entityAccessService.Object);
                };

            protected static SecurityHelper Target { get; private set; }

            protected static void SkipEntityAccess(bool checkAccess)
            {
                _mockUserContext.SkipEntityAccess(checkAccess);
            }
        }

        /// <summary>
        ///     Определяет поведение в случае отказа в доступе.
        /// </summary>
        [Behaviors]
        class ThrowSecurityAccessDeniedExceptionBehavior
        {
            protected static readonly Deal Entity;
            protected static EntityAccessTypes _deniedAccess;
            protected static Exception _exception;

            It should_fail_as_SecurityAccessDeniedException = () => _exception.Should().BeOfType<SecurityAccessDeniedException>();
            It should_fail_with_access_type = () => _exception.Message.Should().Contain(_deniedAccess.ToString());
            It should_fail_with_entity_type_name = () => _exception.Message.Should().Contain(Entity.GetType().Name);
        }

        /// <summary>
        ///     Вызов CheckRequest для разрешенной операции не должен падать.
        /// </summary>
        [Subject("CheckRequest")]
        class When_CheckRequest_and_entity_access_by_type_allowed : CheckRequestSecurityHelperContext
        {
            Establish context = () =>
                {
                    const EntityAccessTypes allowedOperation = OperationType;

                    SetUpRestrictEntityAccess(allowedOperation);
                };

            Because of = TargetCheckRequest;

            It should_not_fail = () => { };
        }

        /// <summary>
        ///     Вызов CheckRequest при огреничении доступа должен бросать исключение.
        /// </summary>
        [Subject("CheckRequest")]
        class When_CheckRequest_and_entity_access_by_type_restricted : CheckRequestSecurityHelperContext
        {
            protected static EntityAccessTypes _deniedAccess = OperationType;
            protected static Exception _exception;

            Establish context = () => SetUpRestrictEntityAccess(EntityAccessTypes.Delete);

            Because of = () => _exception = Catch.Exception(TargetCheckRequest);

            Behaves_like<ThrowSecurityAccessDeniedExceptionBehavior> fail;
        }

        /// <summary>
        ///     Запрос не должен быть отвергнут при отключенной проверке доступа к данным.
        /// </summary>
        [Subject("CheckRequest")]
        class When_CheckRequest_and_skip_entity_access : CheckRequestSecurityHelperContext
        {
            Establish context = () => SkipEntityAccess(true);

            Because of = TargetCheckRequest;

            // COMMENT {f.zaharov, 03.10.2013}: Здесь тоже отформатировал код
            // DONE {d.ivanov, 03.10.2013}: OK, но тут было бы удобнее отформатировать так чтобы код был виден, а еще круче оформить метод, вот так:
            It should_not_restrict_entity_access = () => VerifyRestrictEntityAccesNeverCalled();

            static void VerifyRestrictEntityAccesNeverCalled()
            {
                _entityAccessService.Verify(e => e.RestrictEntityAccess(Moq.It.IsAny<IEntityType>(),
                                                                        Moq.It.IsAny<EntityAccessTypes>(),
                                                                        Moq.It.IsAny<long>(),
                                                                        Moq.It.IsAny<long?>(),
                                                                        Moq.It.IsAny<long>(),
                                                                        Moq.It.IsAny<long?>()),
                                            Times.Never(),
                                            "Проверка должна быть отключена.");
            }
        }

        /// <summary>
        ///     Когда доступ по типу операции запрещен, должно быть выброшенно исключение.
        /// </summary>
        [Subject("IsEntityAccessTypeGranted")]
        class When_access_denied_by_entity_type : IsEntityAccessTypeGrantedSecurityHelperContext
        {
            protected static EntityAccessTypes _deniedAccess = EntityAccessTypes.Delete;
            protected static Exception _exception;

            Because of = () => _exception = Catch.Exception(() => Target.IsEntityAccessTypeGranted<Deal>(_deniedAccess));

            Behaves_like<ThrowSecurityAccessDeniedExceptionBehavior> fail;
        }

        /// <summary>
        ///     Не должен упасть, если операция разрешена.
        /// </summary>
        [Subject("IsEntityAccessTypeGranted")]
        class When_access_granted_by_entity_type : IsEntityAccessTypeGrantedSecurityHelperContext
        {
            Because of = () => Target.IsEntityAccessTypeGranted<Deal>(GrantedEntityAccess);

            It should_not_fail = () => { };
        }
    }
}