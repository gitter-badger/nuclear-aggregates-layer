using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Territory =
            CardMetadata.For<Territory>()
                        .MainAttribute<Territory, ITerritoryViewModel>(x => x.Name)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Territory>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Territory>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Territory>(),
                                    UiElementMetadata.Config.ActivateAction<Territory>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config
                                                             .Name.Static("Firm")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                             .Icon.Path("en_ico_16_Firm.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Firm)
                                                             .FilterToParent());
    }
}