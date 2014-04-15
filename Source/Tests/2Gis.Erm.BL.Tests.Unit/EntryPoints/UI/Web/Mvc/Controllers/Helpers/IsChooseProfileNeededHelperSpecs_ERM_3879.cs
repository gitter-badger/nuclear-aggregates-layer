using System.Linq;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers.Helpers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BL.Tests.Unit.EntryPoints.UI.Web.Mvc.Controllers.Helpers
{
    class IsChooseProfileNeededHelperSpecs_ERM_3879
    {

        static class Create
        {
            const long OrderId = 1;
            const long LegalPersonId = 2;

            public static Order Order(bool isProfileSpecified, int profileCount)
            {
                var profiles = new long[] { 3, 4, 5 }
                    .Take(profileCount)
                    .Select(LegalPersonProfile)
                    .ToArray();

                var profileId = profiles.Any() ? profiles.First().Id : 666;

                return new Order
                    {
                        Id = OrderId,
                        LegalPersonId = LegalPersonId,
                        LegalPerson = new LegalPerson
                            {
                                Id = LegalPersonId,
                                LegalPersonProfiles = profiles,
                            },
                        LegalPersonProfileId = isProfileSpecified ? (long?)profileId : null
                    };
            }

            private static LegalPersonProfile LegalPersonProfile(long id)
            {
                return new LegalPersonProfile { Id = id };
            }

            public static IOrderReadModel OrderReadModel(Order order)
            {
                var mock = Mock.Of<IOrderReadModel>();
                Mock.Get(mock).Setup(model => model.GetOrder(Moq.It.IsAny<long>())).Returns(order);
                var x = mock.GetOrder(123);
                return mock;
            }

            public static ILegalPersonRepository LegalPersonRepository(Order order)
            {
                var mock = Mock.Of<ILegalPersonRepository>();
                Mock.Get(mock).Setup(model => model.GetLegalPersonWithProfiles(Moq.It.IsAny<long>())).Returns(new LegalPersonWithProfiles 
                {
                    LegalPerson = order.LegalPerson,
                    Profiles = order.LegalPerson.LegalPersonProfiles
                });
                return mock;
            }
        }

        #region Order
        class When_OrderHasNoProfile_NoProfiles_PrintOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                Order = Create.Order(isProfileSpecified: false, profileCount: 0);
                Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintOrder);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasNoProfile_SingleProfile_PrintOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
                {
                    Order = Create.Order(isProfileSpecified: false, profileCount: 1);
                    Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintOrder);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasNoProfile_MultipleProfiles_PrintOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                Order = Create.Order(isProfileSpecified: false, profileCount: 2);
                Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintOrder);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_NoProfile_PrintOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                Order = Create.Order(isProfileSpecified: true, profileCount: 0);
                Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintOrder);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_SingleProfile_PrintOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                Order = Create.Order(isProfileSpecified: true, profileCount: 1);
                Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintOrder);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasProfile_MultipleProfiles_PrintOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
                {
                    Order = Create.Order(isProfileSpecified: true, profileCount: 2);
                    Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintOrder);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        } 
        #endregion

        #region PrintReferenceInformation
        class When_OrderHasNoProfile_NoProfiles_PrintNotOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
                {
                    Order = Create.Order(isProfileSpecified: false, profileCount: 0);
                    Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintReferenceInformation);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasNoProfile_SingleProfile_PrintNotOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
                {
                    Order = Create.Order(isProfileSpecified: false, profileCount: 1);
                    Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintReferenceInformation);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasNoProfile_MultipleProfiles_PrintNotOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
                {
                    Order = Create.Order(isProfileSpecified: false, profileCount: 2);
                    Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintReferenceInformation);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_NoProfile_PrintNotOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                Order = Create.Order(isProfileSpecified: true, profileCount: 0);
                Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintReferenceInformation);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_SingleProfile_PrintNotOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                Order = Create.Order(isProfileSpecified: true, profileCount: 1);
                Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintReferenceInformation);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasProfile_MultipleProfiles_PrintNotOrder
        {
            static Order Order;
            static IsChooseProfileNeededHelper Helper;
            static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;

            Establish context = () =>
            {
                Order = Create.Order(isProfileSpecified: true, profileCount: 2);
                Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
            };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, PrintOrderType.PrintReferenceInformation);

            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        } 
        #endregion
    }
}
