using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Aggregates.Aliases;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Concrete.Hierarchy;

namespace DoubleGis.Erm.BL.UI.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Account =
            CardMetadata.For<Account>()
                        .Title.Resource(() => ErmConfigLocalization.EnAccounts)
                        .Icon.Path("en_ico_16_Account.gif")
                        .MainAttribute(x => x.Id)
                        .RelatedItems.Attach(HierarchyMetadata.Config
                                                              .Title.Resource(() => ErmConfigLocalization.CrdRelAccountDetails)
                                                              .Handler.ShowGridByConvention(OrderAggregate.Bill.AsEntityName(), "OrderId={Id}", "Id == 0"))
                        .Actions.Attach();
    }
}
