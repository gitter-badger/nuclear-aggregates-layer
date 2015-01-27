using System;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Services.Cards.Clients;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Clients.ReadModel;
using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;
using MessageType = DoubleGis.Erm.Platform.UI.Metadata.UIElements.MessageType;

namespace DoubleGis.Erm.BL.Tests.Unit.EntryPoints.UI.Web.Mvc.Services
{
    class WarnLinkToAdvAgencyExistsVmCustomizationSpecs
    {
        class When_client_view_model_allready_has_error_message_and_parent_adv_agency : ShouldNotChangeVmStateContext
        {
            Establish context = () =>
            {
                ExpectedMessageType = MessageType.CriticalError;
                ClientViewModel.MessageType = ExpectedMessageType;

                SetupReadModelToReturnClients("Some client");
            };

            Because of = () => Target.Customize(ClientViewModel, null);

            It should_not_change_message_type = () => ClientViewModel.MessageType.Should().Be(ExpectedMessageType);
            It should_not_change_message = () => ClientViewModel.Message.Should().Be(ExpectedMessage);
        }

        class When_client_hasnot_parent_links_to_advertising_agencies : ShouldNotChangeVmStateContext
        {
            Establish context = () =>
                {
                    ExpectedMessageType = MessageType.Info;
                    ClientViewModel.MessageType = ExpectedMessageType;
                };

            Because of = () => Target.Customize(ClientViewModel, null);

            It should_not_change_message_type = () => ClientViewModel.MessageType.Should().Be(ExpectedMessageType);
            It should_not_change_message = () => ClientViewModel.Message.Should().Be(ExpectedMessage);
        }

        class When_client_has_parent_links_to_advertising_agencies : WarnLinkToAdvAgencyExistsVmCustomizationContext
        {
            Establish context = () =>
                {
                    ExpectedNameOne = "Client one";
                    ExpectedNameTwo = "Client two";

                    SetupReadModelToReturnClients(ExpectedNameOne, ExpectedNameTwo);
                };

            Because of = () => Target.Customize(ClientViewModel, null);

            It should_set_warning_message_type = () => ClientViewModel.MessageType.Should().Be(MessageType.Warning);
            It should_contain_client_one_and_two_names = () => ClientViewModel.Message.Should()
                .Contain(ExpectedNameOne).And.Contain(ExpectedNameTwo);

            static string ExpectedNameOne;
            static string ExpectedNameTwo;
        }

        class ShouldNotChangeVmStateContext : WarnLinkToAdvAgencyExistsVmCustomizationContext
        {
            Establish context = () =>
            {
                ExpectedMessage = "Some message";
                ClientViewModel.Message = ExpectedMessage;
            };

            protected static string ExpectedMessage;
            protected static MessageType ExpectedMessageType;
        }

        [Tags("BL")]
        [Tags("WarnLinkToAdvAgencyExistsVmCustomization")]
        [Subject(typeof(WarnLinkToAdvAgencyExistsVmCustomization))]
        class WarnLinkToAdvAgencyExistsVmCustomizationContext
        {
            protected static WarnLinkToAdvAgencyExistsVmCustomization Target { get; private set; }
            protected static StubClientViewModel ClientViewModel { get; set; }
            protected static Mock<IClientReadModel> ClientReadModel { get; private set; }

            Establish context = () =>
                {
                    ClientReadModel = new Mock<IClientReadModel>();
                    ClientViewModel = new StubClientViewModel();
                    ClientViewModel.Id = 42;
                    Target = new WarnLinkToAdvAgencyExistsVmCustomization(ClientReadModel.Object);
                };

            protected static void SetupReadModelToReturnClients(params string[] clients)
            {
                long id = 1;
                SetupReadModelToReturnClients(clients.Select(c => new MasterClientDto { Id = id++, IsAdvertisingAgency = true, Name = c }).ToArray());
            }

            protected static void SetupReadModelToReturnClients(params MasterClientDto[] clients)
            {
                ClientReadModel.Setup(r => r.GetMasterAdvertisingAgencies(ClientViewModel.Id)).Returns(clients);
            }
        }

        public class StubClientViewModel : EntityViewModelBase<Client>
        {
            public override byte[] Timestamp { get; set; }
            public DateTime LastQualifyTime { get; set; }

            public override void LoadDomainEntityDto(IDomainEntityDto domainEntityDto)
            {
                throw new NotImplementedException();
            }

            public override IDomainEntityDto TransformToDomainEntityDto()
            {
                throw new NotImplementedException();
            }
        }
    }
}