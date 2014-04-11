using System;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers.Helpers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
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
        class IsChooseProfileNeededHelperContext
        {
            protected const long OrderId = 1;

            protected static Order Order;

            protected static IOrderReadModel OrderReadModel;
            protected static ILegalPersonRepository LegalPersonRepository;
            protected static IsChooseProfileNeededHelper Helper;

            Establish context = () =>
                {
                    Order = new Order { Id = OrderId };
                    OrderReadModel = Mock.Of<IOrderReadModel>(x => x.GetOrder(OrderId) == Order);

                    LegalPersonRepository = Mock.Of<ILegalPersonRepository>();

                    Helper = new IsChooseProfileNeededHelper(OrderReadModel, LegalPersonRepository);
                };

            protected static void CheckAllNeededPrintOrderTypes(Action<PrintOrderType> shouldAction, PrintOrderType[] PrintOrderTypesToExclude)
            {
                var printOrderTypes = Enum.GetValues(typeof(PrintOrderType)).Cast<PrintOrderType>().Where(x => !PrintOrderTypesToExclude.Contains(x));

                foreach (var printOrderType in printOrderTypes)
                {
                    shouldAction(printOrderType);
                }
            }
        }

        class OrderHasNoProfileButHasLegalPersonContext : IsChooseProfileNeededHelperContext
        {
            protected const long LegalPersonId = 2;

            protected static LegalPerson LegalPerson;
            protected static LegalPersonWithProfiles LegalPersonWithProfiles;

            Establish context = () =>
                {
                    Order.LegalPersonId = LegalPersonId;

                    LegalPerson = new LegalPerson { Id = LegalPersonId };
                    LegalPersonWithProfiles = new LegalPersonWithProfiles
                        {
                            LegalPerson = LegalPerson
                        };

                    Mock.Get(LegalPersonRepository).Setup(x => x.GetLegalPersonWithProfiles(LegalPersonId)).Returns(LegalPersonWithProfiles);
                };
        }

        class OrderHasLegalPersonAndProfileContext : OrderHasNoProfileButHasLegalPersonContext
        {
            protected const long LegalPersonProfileId = 3;

            Establish context = () => { Order.LegalPersonProfileId = LegalPersonProfileId; };
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(IsChooseProfileNeededHelper))]
        class When_order_has_legal_person_with_single_profile : OrderHasNoProfileButHasLegalPersonContext
        {
            const long LegalPersonProfileId = 3;

            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            static LegalPersonProfile LegalPersonProfile;

            Establish context = () =>
                {
                    LegalPersonProfile = new LegalPersonProfile { Id = LegalPersonProfileId };
                    LegalPersonWithProfiles.Profiles = new[] { LegalPersonProfile };
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(OrderId, 0); 

            It should_returns_not_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeFalse();
            It should_returns_order_profile_id = () => Result.LegalPersonProfileId.Should().Be(LegalPersonProfileId);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(IsChooseProfileNeededHelper))]
        class When_order_has_natural_legal_person_with_many_profiles : OrderHasNoProfileButHasLegalPersonContext
        {
            static PrintOrderType[] PrintOrderTypesToExclude =
                {
                    PrintOrderType.PrintOrder, 
                    PrintOrderType.PrintBargain, 
                    PrintOrderType.PrintReferenceInformation
                };

            Establish context = () =>
                {
                    LegalPersonWithProfiles.Profiles = new[] { new LegalPersonProfile(), new LegalPersonProfile() };
                    LegalPerson.LegalPersonTypeEnum = (int)LegalPersonType.NaturalPerson;
                };

            It should_returns_not_needed_choose_profile = () => CheckAllNeededPrintOrderTypes(
                printOrderType =>
                    {
                        var result = Helper.GetChooseProfileDialogState(OrderId, printOrderType);
                        result.IsChooseProfileNeeded.Should().BeFalse();
                        result.LegalPersonProfileId.Should().Be(null);
                    }, 
                PrintOrderTypesToExclude);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(IsChooseProfileNeededHelper))]
        class When_order_has_not_legal_person_and_profile : IsChooseProfileNeededHelperContext
        {
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Because of = () => Result = Helper.GetChooseProfileDialogState(OrderId, 0);

            It should_returns_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeTrue();
            It should_returns_null_profile_id = () => Result.LegalPersonProfileId.Should().Be(null);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(IsChooseProfileNeededHelper))]
        class When_print_not_order_and_order_has_profile : IsChooseProfileNeededHelperContext
        {
            const long ProfileId = 2;
            static PrintOrderType[] PrintOrderTypesToExclude = { PrintOrderType.PrintOrder };

            Establish context = () => Order.LegalPersonProfileId = ProfileId;

            It should_choose_profile_not_needed = () => CheckAllNeededPrintOrderTypes(
                printOrderType =>
                    {
                        var result = Helper.GetChooseProfileDialogState(OrderId, printOrderType);
                        result.IsChooseProfileNeeded.Should().BeFalse();
                        result.LegalPersonProfileId.Should().Be(ProfileId);
                    }, 
                PrintOrderTypesToExclude);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(IsChooseProfileNeededHelper))]
        class When_print_order_with_single_legal_profile : OrderHasLegalPersonAndProfileContext
        {
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;
            static LegalPersonProfile LegalPersonProfile;

            Establish context = () =>
                {
                    LegalPersonProfile = new LegalPersonProfile { Id = LegalPersonProfileId };
                    LegalPersonWithProfiles.Profiles = new[] { LegalPersonProfile };
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(OrderId, PrintOrderType.PrintOrder);

            It should_returns_not_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeFalse();
            It should_returns_order_profile_id = () => Result.LegalPersonProfileId.Should().Be(LegalPersonProfileId);
        }

        class PrintBillContext : OrderHasNoProfileButHasLegalPersonContext
        {
            protected const long BillId = 8;
            Establish context = () => Mock.Get(OrderReadModel).Setup(x => x.GetOrderByBill(BillId)).Returns(Order);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(IsChooseProfileNeededHelper))]
        class When_print_bill_and_legal_person_has_single_profile : PrintBillContext
        {
            protected const long LegalPersonProfileId = 3;
            static LegalPersonProfile LegalPersonProfile;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () => 
            {
                LegalPersonProfile = new LegalPersonProfile { Id = LegalPersonProfileId };
                LegalPersonWithProfiles.Profiles = new[] { LegalPersonProfile };
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(BillId);

            It should_returns_not_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeFalse();
            It should_returns_order_profile_id = () => Result.LegalPersonProfileId.Should().Be(LegalPersonProfileId);
        }

        [Tags("ControllerHelper")]
        [Subject(typeof(IsChooseProfileNeededHelper))]
        class When_print_bill_and_legal_person_has_many_profile : PrintBillContext
        {
            protected const long LegalPersonProfileId_1 = 3;
            protected const long LegalPersonProfileId_2 = 4;

            static LegalPersonProfile LegalPersonProfile_1;
            static LegalPersonProfile LegalPersonProfile_2;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                LegalPersonProfile_1 = new LegalPersonProfile { Id = LegalPersonProfileId_1 };
                LegalPersonProfile_2 = new LegalPersonProfile { Id = LegalPersonProfileId_2 };
                LegalPersonWithProfiles.Profiles = new[] { LegalPersonProfile_1, LegalPersonProfile_2 };
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(BillId);

            It should_returns_need_choose_profile = () => Result.IsChooseProfileNeeded.Should().BeTrue();
            It should_returns_order_profile_id = () => Result.LegalPersonProfileId.Should().Be(null);
        }
    }
}