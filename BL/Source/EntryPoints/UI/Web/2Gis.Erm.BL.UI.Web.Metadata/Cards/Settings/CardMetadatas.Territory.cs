using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Territory =
            CardMetadata.For<Territory>()
                        .MainAttribute<Territory, ITerritoryViewModel>(x => x.Name)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<Territory>(),
                                UIElementMetadata.Config.UpdateAction<Territory>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<Territory>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<Territory>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<Territory>(),
                                UIElementMetadata.Config.ActivateAction<Territory>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                            UIElementMetadata.Config
                                                             .Name.Static("Firm")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirms)
                                                             .Icon.Path("en_ico_16_Firm.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Firm)
                                                             .FilterToParent());
    }
}