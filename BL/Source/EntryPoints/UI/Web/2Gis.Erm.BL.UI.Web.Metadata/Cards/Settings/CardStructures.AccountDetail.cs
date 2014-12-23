using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AccountDetail =
            CardMetadata.For<AccountDetail>()
                        .MainAttribute(x => x.Id)
                        .Actions.Attach(UIElementMetadata.Config.CreateAction<AccountDetail>(),
                                        UIElementMetadata.Config.UpdateAction<AccountDetail>(),
                                        UIElementMetadata.Config.CreateAndCloseAction<AccountDetail>(),
                                        UIElementMetadata.Config.UpdateAndCloseAction<AccountDetail>(),
                                        UIElementMetadata.Config.RefreshAction<AccountDetail>(),
                                        UIElementMetadata.Config.CloseAction());
    }
}