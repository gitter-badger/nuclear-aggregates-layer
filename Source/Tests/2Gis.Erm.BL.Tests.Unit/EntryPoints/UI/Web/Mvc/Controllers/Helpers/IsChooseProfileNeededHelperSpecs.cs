using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers.Helpers;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BL.Tests.Unit.EntryPoints.UI.Web.Mvc.Controllers.Helpers
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test")]
    public class IsChooseProfileNeededHelperSpecs
    {
        abstract class IsChooseProfileNeededHelperContext
        {
            protected const long OrderId = 1;
            protected const long LegalPersonId = 2;
            protected const long LegalPersonProfileId = 3;

            protected static Order Order;
            protected static LegalPerson LegalPerson;
            protected static LegalPersonProfile[] LegalPersonProfiles;
            protected static LegalPersonWithProfiles LegalPersonWithProfiles;

            protected static IOrderReadModel OrderReadModel;
            protected static ILegalPersonReadModel LegalPersonReadModel;
            protected static ProfileChooseHelper Helper;

            Establish context = () =>
                {
                    Order = new Order { Id = OrderId };
                    OrderReadModel = Create.OrderReadModel(Order);

                    LegalPersonReadModel = Mock.Of<ILegalPersonReadModel>();

                    Helper = new ProfileChooseHelper(OrderReadModel, LegalPersonReadModel, Mock.Of<ISecurityServiceEntityAccess>());

                    Mock.Get(LegalPersonReadModel)
                        .Setup(x => x.GetLegalPersonProfileIds(LegalPersonId))
                        .Returns(() => LegalPersonProfiles.Select(p => p.Id));
                };
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(ProfileChooseHelper))]
        class When_order_has_legal_person_with_single_profile : IsChooseProfileNeededHelperContext
        {
            static ProfileChooseHelper.ChooseProfileDialogState Result;
            static LegalPersonProfile LegalPersonProfile;

            Establish context = () =>
                {
                    LegalPerson = new LegalPerson { Id = LegalPersonId };
                    Order.LegalPersonId = LegalPerson.Id;
                    LegalPersonProfile = new LegalPersonProfile { Id = LegalPersonProfileId };
                    LegalPersonProfiles = new[] { LegalPersonProfile };
                };

            Because of = () => Result = Helper.GetChooseProfileDialogStateForOrder(OrderId); 

            It should_returns_not_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeFalse();
            It should_returns_order_profile_id = () => Result.LegalPersonProfileId.Should().Be(LegalPersonProfileId);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(ProfileChooseHelper))]
        class When_order_has_not_legal_person_and_profile : IsChooseProfileNeededHelperContext
        {
            static ProfileChooseHelper.ChooseProfileDialogState Result;

            Because of = () => Result = Helper.GetChooseProfileDialogStateForOrder(OrderId);

            It should_returns_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeTrue();
            It should_returns_null_profile_id = () => Result.LegalPersonProfileId.Should().Be(null);
        }

        class PrintBillContext : IsChooseProfileNeededHelperContext
        {
            protected const long BillId = 8;

            private Establish context = () =>
                {
                    LegalPerson = new LegalPerson { Id = LegalPersonId };
                    Order.LegalPersonId = LegalPerson.Id;
                    Mock.Get(OrderReadModel).Setup(x => x.GetOrderByBill(BillId)).Returns(Order);
                };
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(ProfileChooseHelper))]
        class When_print_bill_and_legal_person_has_single_profile : PrintBillContext
        {
            static LegalPersonProfile LegalPersonProfile;
            static ProfileChooseHelper.ChooseProfileDialogState Result;

            Establish context = () => 
            {
                LegalPersonProfile = new LegalPersonProfile { Id = LegalPersonProfileId };
                LegalPersonProfiles = new[] { LegalPersonProfile };
            };

            Because of = () => Result = Helper.GetChooseProfileDialogStateForBill(BillId);

            It should_returns_not_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeFalse();
            It should_returns_order_profile_id = () => Result.LegalPersonProfileId.Should().Be(LegalPersonProfileId);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(ProfileChooseHelper))]
        class When_print_bill_and_legal_person_has_many_profile : PrintBillContext
        {
            protected const long LegalPersonProfileId_1 = 3;
            protected const long LegalPersonProfileId_2 = 4;

            static LegalPersonProfile LegalPersonProfile_1;
            static LegalPersonProfile LegalPersonProfile_2;
            static ProfileChooseHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                LegalPersonProfile_1 = new LegalPersonProfile { Id = LegalPersonProfileId_1 };
                LegalPersonProfile_2 = new LegalPersonProfile { Id = LegalPersonProfileId_2 };
                LegalPersonProfiles = new[] { LegalPersonProfile_1, LegalPersonProfile_2 };
            };

            Because of = () => Result = Helper.GetChooseProfileDialogStateForBill(BillId);

            It should_returns_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeTrue();
            It should_returns_order_profile_id = () => Result.LegalPersonProfileId.Should().Be(null);
        }
    }
}