using System.Diagnostics.CodeAnalysis;

using DoubleGis.Erm.BL.UI.Web.Mvc.Controllers.Helpers;
using DoubleGis.Erm.BL.UI.Web.Mvc.Models;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

using FluentAssertions;

using Machine.Specifications;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BL.Tests.Unit.EntryPoints.UI.Web.Mvc.Controllers.Helpers
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test")]
    class IsChooseProfileNeededHelperSpecs_ERM_3879
    {
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
            protected static ProfileChooseHelper Helper;
            protected static ProfileChooseHelper.ChooseProfileDialogState Result;
        }

        private abstract class Context<TProfileSpecified, TProfileCount> : Context
            where TProfileSpecified : ProfileSpecified.IArgument, new()
            where TProfileCount : ProfileCount.IArgument, new()
        {
            Establish context = () =>
                {
                    Order = Create.Order(new TProfileSpecified().IsProfileSpecified, new TProfileCount().ProfileCount);
                    Helper = new ProfileChooseHelper(Create.OrderReadModel(Order), Create.LegalPersonReadModel(Order), Create.SecurityServiceEntityAccess());
                };

            Because of = () => Result = Helper.GetChooseProfileDialogStateForOrder(Order.Id);
        }

        class When_OrderHasNoProfile_NoProfiles_PrintOrder : Context<ProfileSpecified.False, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasNoProfile_SingleProfile_PrintOrder : Context<ProfileSpecified.False, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasNoProfile_MultipleProfiles_PrintOrder : Context<ProfileSpecified.False, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_NoProfile_PrintOrder : Context<ProfileSpecified.True, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_SingleProfile_PrintOrder : Context<ProfileSpecified.True, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasProfile_MultipleProfiles_PrintOrder : Context<ProfileSpecified.True, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        } 

        class When_OrderHasNoProfile_NoProfiles_PrintNotOrder : Context<ProfileSpecified.False, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasNoProfile_SingleProfile_PrintNotOrder : Context<ProfileSpecified.False, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasNoProfile_MultipleProfiles_PrintNotOrder : Context<ProfileSpecified.False, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_NoProfile_PrintNotOrder : Context<ProfileSpecified.True, ProfileCount.NoOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        }

        class When_OrderHasProfile_SingleProfile_PrintNotOrder : Context<ProfileSpecified.True, ProfileCount.One>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(false);
        }

        class When_OrderHasProfile_MultipleProfiles_PrintNotOrder : Context<ProfileSpecified.True, ProfileCount.MoreThanOne>
        {
            It should_not_fail = () => Result.IsChooseProfileNeeded.Should().Be(true);
        } 
    }
}
