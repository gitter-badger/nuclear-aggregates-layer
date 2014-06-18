﻿using System.Web.Mvc;

using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Models.Russia;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Global.Services.Cards.Russia;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLFlex.Tests.Unit.EntryPoints.UI.Web.Mvc.Global.Services.Russia
{
    public class ClientViewModelCustomizationServiceSpecs
    {
        [Tags("BL")]
        [Tags("ClientViewModelCustomizationService")]
        [Subject(typeof(RussiaClientViewModelCustomizationService))]
        public abstract class MockContext
        {
            private Establish context = () =>
                {
                    var userContextMock = new Mock<IUserContext>();
                    userContextMock.SetupGet(x => x.Identity).Returns(() => new ErmUserIdentity(new UserInfo(1, "test", "test")));
                    UserContext = userContextMock.Object;

                    ViewModel = new ClientViewModel();
                };

            protected static IUserContext UserContext { get; set; }
            protected static ClientViewModel ViewModel { get; set; }
            protected static ISecurityServiceFunctionalAccess FunctionalAccessService { get; set; }
            protected static RussiaClientViewModelCustomizationService ViewModelCustomizationService { get; set; }
        }

        private class When_user_has_AdvertisementAgencyManagement_functional_privilege : MockContext
        {
            private Establish context = () =>
                {
                    var functionalAccessServicetMock = new Mock<ISecurityServiceFunctionalAccess>();
                    functionalAccessServicetMock.Setup(x => x.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, 1))
                                                .Returns(true);
                    FunctionalAccessService = functionalAccessServicetMock.Object;
                    ViewModelCustomizationService = new RussiaClientViewModelCustomizationService(FunctionalAccessService, UserContext);
                };

            private Because of =
                () =>
                ViewModelCustomizationService.CustomizeViewModel(ViewModel, Mock.Of<ModelStateDictionary>());

            private It should_be_true_for_CanEditIsAdvertisingAgency_property = () => ViewModel.CanEditIsAdvertisingAgency.Should().BeTrue();
        }

        private class When_user_hasnt_AdvertisementAgencyManagement_functional_privilege : MockContext
        {
            private Establish context = () =>
                {
                    var functionalAccessServicetMock = new Mock<ISecurityServiceFunctionalAccess>();
                    functionalAccessServicetMock.Setup(x => x.HasFunctionalPrivilegeGranted(FunctionalPrivilegeName.AdvertisementAgencyManagement, 1))
                                                .Returns(false);
                    FunctionalAccessService = functionalAccessServicetMock.Object;
                    ViewModelCustomizationService = new RussiaClientViewModelCustomizationService(FunctionalAccessService, UserContext);
                };

            private Because of =
                () =>
                ViewModelCustomizationService.CustomizeViewModel(ViewModel, Mock.Of<ModelStateDictionary>());

            private It should_be_false_for_CanEditIsAdvertisingAgency_property = () => ViewModel.CanEditIsAdvertisingAgency.Should().BeFalse();
        }
    }
}
