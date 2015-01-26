using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Resources.Accessors;

using NuClear.Metamodeling.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        public readonly static HierarchyMetadata Billing =
            HierarchyMetadata.Config
                .Title.Resource(() => ErmConfigLocalization.NavAreaBilling)
                .Icon.Resource(Images.Navigation.NavAreaBilling)
                .Childs(
                    HierarchyMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupOrders)
                        .Icon.Resource(Images.Navigation.NavGroupOrders)
                        .Handler.ShowGrid(OrderAggregate.Root, null, null),
                    //HierarchyElement.Config
                    //    .Title.Resource(() => ErmConfigLocalization.NavGroupLegalPersons)
                    //    .Icon.Resource(Images.Navigation.NavGroupLegalPersons)
                    //    .Handler.ShowGrid(LegalPersonAggregate.LegalPerson.AsEntityName(), null, null),
                    HierarchyMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.EnMDeal)
                        .Icon.Resource(Images.Navigation.NavGroupDeals)
                        .Handler.ShowGrid(DealAggregate.Root, null, null),
                    HierarchyMetadata.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupAccounts)
                        .Icon.Resource(Images.Navigation.NavGroupAccounts)
                        .Handler.ShowGrid(AccountAggregate.Root, null, null));
    }
}

 