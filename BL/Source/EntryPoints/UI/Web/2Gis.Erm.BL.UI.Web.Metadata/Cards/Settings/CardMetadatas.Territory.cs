using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
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
                        .Attach(ToolbarElements.Create<Territory>(),
                                ToolbarElements.Update<Territory>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Territory>(),
                                ToolbarElements.UpdateAndClose<Territory>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Territory>(),
                                ToolbarElements.Activate<Territory>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
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