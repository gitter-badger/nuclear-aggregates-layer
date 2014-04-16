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
            const long LegalPersonProfileId1 = 3;
            const long LegalPersonProfileId2 = 3;
            const long InvalidLegalPersonProfileId = 666;

            public static Order Order(bool isProfileSpecified, int profileCount)
            {
                var profiles = new [] { LegalPersonProfileId1, LegalPersonProfileId2 }
                    .Take(profileCount)
                    .Select(LegalPersonProfile)
                    .ToArray();

                var profileId = profiles.Any() ? profiles.First().Id : InvalidLegalPersonProfileId;

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

        private static class ProfileSpecified
        {
            public interface IArgument
            {
                bool IsProfileSpecified { get; }
            }

            public class True : IArgument
            {
                public bool IsProfileSpecified { get { return true; } }
            }

            public class False : IArgument
            {
                public bool IsProfileSpecified { get { return false; } }
            }
        }

        private static class ProfileCount
        {
            public interface IArgument
            {
                int ProfileCount { get; }
            }

            public class NoOne : IArgument
            {
                public int ProfileCount { get { return 0; } }
            }

            public class One : IArgument
            {
                public int ProfileCount { get { return 1; } }
            }

            public class MoreThanOne : IArgument
            {
                public int ProfileCount { get { return 2; } }
            }
        }

        private static class PrintOrder
        {
            public interface IArgument
            {
                PrintOrderType PrintOrderType { get; }
            }

            public class True : IArgument
            {
                public PrintOrderType PrintOrderType { get { return PrintOrderType.PrintOrder; } }
            }

            public class False : IArgument
            {
                public PrintOrderType PrintOrderType { get { return PrintOrderType.PrintReferenceInformation; } }
            }
        }
        
        private abstract class Context
        {
            protected static Order Order;
            protected static IsChooseProfileNeededHelper Helper;
            protected static IsChooseProfileNeededHelper.ChooseProfileDialogState Result;
        }

        private abstract class Context<TProfileSpecified, TPrintOrderType, TProfileCount> : Context
            where TProfileSpecified : ProfileSpecified.IArgument, new()
            where TProfileCount : ProfileCount.IArgument, new()
            where TPrintOrderType : PrintOrder.IArgument, new()
        {
            Establish context = () =>
                {
                    Order = Create.Order(new TProfileSpecified().IsProfileSpecified, new TProfileCount().ProfileCount);
                    Helper = new IsChooseProfileNeededHelper(Create.OrderReadModel(Order), Create.LegalPersonRepository(Order));
                };

            Because of = () => Result = Helper.GetChooseProfileDialogState(Order.Id, new TPrintOrderType().PrintOrderType);
        }

        class When_OrderHasNoProfile_NoProfiles_PrintOrder : Context<ProfileSpecified.False, PrintOrder.True, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasNoProfile_SingleProfile_PrintOrder : Context<ProfileSpecified.False, PrintOrder.True, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasNoProfile_MultipleProfiles_PrintOrder : Context<ProfileSpecified.False, PrintOrder.True, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_NoProfile_PrintOrder : Context<ProfileSpecified.True, PrintOrder.True, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_SingleProfile_PrintOrder : Context<ProfileSpecified.True, PrintOrder.True, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasProfile_MultipleProfiles_PrintOrder : Context<ProfileSpecified.True, PrintOrder.True, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        } 

        class When_OrderHasNoProfile_NoProfiles_PrintNotOrder : Context<ProfileSpecified.False, PrintOrder.False, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasNoProfile_SingleProfile_PrintNotOrder : Context<ProfileSpecified.False, PrintOrder.False, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasNoProfile_MultipleProfiles_PrintNotOrder : Context<ProfileSpecified.False, PrintOrder.False, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_NoProfile_PrintNotOrder : Context<ProfileSpecified.True, PrintOrder.False, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_SingleProfile_PrintNotOrder : Context<ProfileSpecified.True, PrintOrder.False, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasProfile_MultipleProfiles_PrintNotOrder : Context<ProfileSpecified.True, PrintOrder.False, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        } 
    }
}
