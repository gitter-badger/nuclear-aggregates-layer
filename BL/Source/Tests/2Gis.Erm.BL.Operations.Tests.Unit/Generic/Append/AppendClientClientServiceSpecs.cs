using System;
using System.Collections.Generic;

using DoubleGis.Erm.BL.Operations.Generic.Append;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.Operations;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.Append;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;
using FluentAssertions.Primitives;

using Machine.Specifications;

using Moq;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity.Generic;
using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BL.Operations.Tests.Unit.Generic.Append
{
    class AppendClientClientServiceSpecs
    {
        [Subject(typeof(AppendClientClientService))]
        class When_client_link_exist : AppendParamsInvalidAppendClientClientServiceContext
        {
            Establish context = () => ReadModel.Setup(r => r.IsClientLinksExists(Params.ParentId, Params.AppendedId, false)).Returns(true);

            It should_throw_exception = () => ExceptionShouldBeOfType<ClientLinkAlreadyExistsException>();
            It should_not_call_create = () => Repository.Verify(r => r.Create(Moq.It.IsAny<ClientLink>(), Moq.It.IsAny<IEnumerable<DenormalizedClientLink>>()), Times.Never);
        }

        [Subject(typeof(AppendClientClientService))]
        class When_params_valid : AppendClientClientServiceContext
        {
            Establish context = () =>
                {
                    var denormalizedClientLinks = new[] { new DenormalizedClientLink(), };
                    ReadModel
                        .Setup(r => r.GetCurrentDenormalizationForClientLink(Moq.It.Is<long>(v => v == Params.ParentId.Value), Moq.It.Is<long>(v => v == Params.AppendedId.Value)))
                        .Returns(denormalizedClientLinks);

                    Repository.Setup(r => r.Create(Moq.It.IsAny<ClientLink>(), Moq.It.Is<IEnumerable<DenormalizedClientLink>>(v => v == denormalizedClientLinks)))
                              .Callback((ClientLink c, IEnumerable<DenormalizedClientLink> z) => CreatedClientLink = c);
                };

            Because of = () => Target.Append(Params);

            It should_create_link_in_repository = () =>
                {
                    CreatedClientLink.MasterClientId.Should().Be(Params.ParentId);
                    CreatedClientLink.ChildClientId.Should().Be(Params.AppendedId);
                    CreatedClientLink.IsDeleted.Should().BeFalse();
                };

            It should_update_scope_master_client = () => Scope.Verify(s => s.Updated<Client>(CreatedClientLink.MasterClientId), Times.Once);
            It should_update_scope_child_client = () => Scope.Verify(s => s.Updated<Client>(CreatedClientLink.ChildClientId), Times.Once);
            It should_update_scope_client_link = () => Scope.Verify(s => s.Updated<ClientLink>(CreatedClientLink.Id), Times.Once);

            static ClientLink CreatedClientLink;
        }

        [Subject(typeof(AppendClientClientService))]
        class When_parent_and_apend_id_same : AppendParamsInvalidAppendClientClientServiceContext
        {
            Establish context = () => Params.AppendedId = Params.ParentId;

            It should_throw_exception = () => ExceptionShouldBeOfType<SameIdsForEntitiesToLinkException>();
        }

        [Subject(typeof(AppendClientClientService))]
        class When_parent_type_not_specified : AppendParamsInvalidAppendClientClientServiceContext
        {
            Establish context = () => Params.ParentType.Equals(EntityType.Instance.Account());

            It should_throw_exception = () => ExceptionShouldBeOfType<InvalidEntityTypesForLinkingException>();
        }

        [Subject(typeof(AppendClientClientService))]
        class When_appended_type_not_specified : AppendParamsInvalidAppendClientClientServiceContext
        {
            Establish context = () => Params.AppendedType.Equals(EntityType.Instance.Account());

            It should_throw_exception = () => ExceptionShouldBeOfType<InvalidEntityTypesForLinkingException>();
        }

        [Subject(typeof(AppendClientClientService))]
        private class When_appended_id_not_specified : AppendParamsInvalidAppendClientClientServiceContext
        {
            Establish context = () => Params.AppendedId = null;

            It should_throw_exception = () => ExceptionShouldBeOfType<ParentOrChildIdsNotSpecifiedException>();
        }

        [Subject(typeof(AppendClientClientService))]
        private class When_parent_id_not_specified : AppendParamsInvalidAppendClientClientServiceContext
        {
            Establish context = () => Params.ParentId = null;

            It should_throw_exception = () => ExceptionShouldBeOfType<ParentOrChildIdsNotSpecifiedException>();
        }

        private class AppendParamsInvalidAppendClientClientServiceContext : AppendClientClientServiceContext
        {
            Because of = () => Result = Catch.Exception(() => Target.Append(Params));

            protected static AndWhichConstraint<ObjectAssertions, TException> ExceptionShouldBeOfType<TException>()
            {
                return Result.Should().NotBeNull().And.BeOfType<TException>();
            }

            protected static Exception Result;
        }

        class AppendClientClientServiceContext
        {
            private Establish context = () =>
                {
                    Params = new AppendParams
                        {
                            ParentId = 42,
                            ParentType = EntityType.Instance.Client(),

                            AppendedId = 24,
                            AppendedType = EntityType.Instance.Client(),
                        };

                    Scope = new Mock<IOperationScope>();
                    Scope.Setup(s => s.Updated<Client>(Params.ParentId.Value)).Returns(Scope.Object);
                    Scope.Setup(s => s.Updated<Client>(Params.AppendedId.Value)).Returns(Scope.Object);
                    Scope.Setup(s => s.Updated<ClientLink>(Moq.It.IsAny<long>())).Returns(Scope.Object);

                    var scopeFactory = new Mock<IOperationScopeFactory>();
                    scopeFactory.Setup(sf => sf.CreateSpecificFor<AppendIdentity, Client, Client>()).Returns(Scope.Object);

                    Repository = new Mock<ICreateClientLinkAggregateService>();
                    ReadModel = new Mock<IClientReadModel>();
                    ReadModel.Setup(mock => mock.GetClient(Moq.It.IsAny<long>())).Returns<long>(id => new Client { Id = id });

                    var userContext = new Mock<IUserContext>();
                    userContext.Setup(mock => mock.Identity).Returns(new NullUserIdentity());

                    var securityService = new Mock<ISecurityServiceEntityAccess>();
                    securityService.Setup(mock => mock.HasEntityAccess(Moq.It.IsAny<EntityAccessTypes>(),
                                                                       Moq.It.IsAny<IEntityType>(),
                                                                       Moq.It.IsAny<long>(),
                                                                       Moq.It.IsAny<long?>(),
                                                                       Moq.It.IsAny<long>(),
                                                                       Moq.It.IsAny<long?>()))
                                   .Returns(() => true);

                    Target = new AppendClientClientService(Repository.Object,
                                                           ReadModel.Object,
                                                           scopeFactory.Object,
                                                           securityService.Object,
                                                           userContext.Object);
                };

            protected static Mock<IOperationScope> Scope { get; private set; }

            protected static Mock<IClientReadModel> ReadModel { get; private set; }
            protected static Mock<ICreateClientLinkAggregateService> Repository { get; private set; }
            protected static AppendClientClientService Target { get; private set; }
            protected static AppendParams Params;
        }
    }
}