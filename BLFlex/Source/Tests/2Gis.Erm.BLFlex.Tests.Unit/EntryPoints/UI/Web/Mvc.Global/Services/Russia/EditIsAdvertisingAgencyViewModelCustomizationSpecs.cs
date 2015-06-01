using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia.Clients;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Services.Russia
{
    class EditIsAdvertisingAgencyViewModelCustomizationSpecs
    {
        class When_user_has_AdvertisementAgencyManagement_functional_privilege : MockContext
        {
            Establish context = () => SetupHasFunctionalPrivilegeGranted(true);

            Because of = () => Target.Customize(ViewModel, null);

            It should_be_true_for_CanEditIsAdvertisingAgency_property = () => ViewModel.CanEditIsAdvertisingAgency.Should().BeTrue();
        }

        class When_user_hasnt_AdvertisementAgencyManagement_functional_privilege : MockContext
        {
            Establish context = () => SetupHasFunctionalPrivilegeGranted(false);

            Because of = () => Target.Customize(ViewModel, null);

            It should_be_false_for_CanEditIsAdvertisingAgency_property = () => ViewModel.CanEditIsAdvertisingAgency.Should().BeFalse();
        }

        [Tags("BL")]
        [Tags("EditIsAdvertisingAgencyViewModelCustomization")]
        [Subject(typeof(EditIsAdvertisingAgencyViewModelCustomization))]
        public abstract class MockContext
        {
            Establish context = () =>
                {
                    UserContext = new Mock<IUserContext>();
                    UserContext.SetupGet(x => x.Identity).Returns(() => new ErmUserIdentity(new UserInfo(1, "test", "test")));

                    FunctionalAccessService = new Mock<ISecurityServiceFunctionalAccess>();
                    ViewModel = new ClientViewModel();

                    Target = new EditIsAdvertisingAgencyViewModelCustomization(FunctionalAccessService.Object, UserContext.Object);
                };

            protected static void SetupHasFunctionalPrivilegeGranted(bool value)
            {
                FunctionalAccessService.Setup(x => x.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, 1))
                                       .Returns(value);
            }

            protected static Mock<IUserContext> UserContext { get; private set; }
            protected static Mock<ISecurityServiceFunctionalAccess> FunctionalAccessService { get; private set; }
            protected static ClientViewModel ViewModel { get; set; }
            protected static EditIsAdvertisingAgencyViewModelCustomization Target { get; private set; }
        }
    }
}