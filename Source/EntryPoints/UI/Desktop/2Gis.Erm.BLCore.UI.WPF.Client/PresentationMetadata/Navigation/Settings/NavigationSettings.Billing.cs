using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Common;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Hierarchy;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Resources.Accessors;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Navigation.Settings
{
    public static partial class NavigationSettings
    {
        public readonly static HierarchyElement Billing =
            HierarchyElement.Config
                .Title.Resource(() => ErmConfigLocalization.NavAreaBilling)
                .Icon.Resource(Images.Navigation.NavAreaBilling)
                .Childs(
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupOrders)
                        .Icon.Resource(Images.Navigation.NavGroupOrders)
                        .Handler.ShowGrid(OrderAggregate.Order.AsEntityName(), null, null),
                    //HierarchyElement.Config
                    //    .Title.Resource(() => ErmConfigLocalization.NavGroupLegalPersons)
                    //    .Icon.Resource(Images.Navigation.NavGroupLegalPersons)
                    //    .Handler.ShowGrid(LegalPersonAggregate.LegalPerson.AsEntityName(), null, null),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.EnMDeal)
                        .Icon.Resource(Images.Navigation.NavGroupDeals)
                        .Handler.ShowGrid(DealAggregate.Deal.AsEntityName(), null, null),
                    HierarchyElement.Config
                        .Title.Resource(() => ErmConfigLocalization.NavGroupAccounts)
                        .Icon.Resource(Images.Navigation.NavGroupAccounts)
                        .Handler.ShowGrid(AccountAggregate.Account.AsEntityName(), null, null));
    }
}

 