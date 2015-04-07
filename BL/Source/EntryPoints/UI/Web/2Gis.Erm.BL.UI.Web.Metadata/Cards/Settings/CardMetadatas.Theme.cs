using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Theme =
            CardMetadata.For<Theme>()
                        .WithDefaultIcon()
                        .Actions
                        .Attach(ToolbarElements.Create<Theme>(),
                                ToolbarElements.Update<Theme>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Theme>(),
                                ToolbarElements.UpdateAndClose<Theme>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Theme>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.Themes.SetAsDefault(),
                                                           ToolbarElements.Themes.UnsetAsDefault()),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close())
                        .WithRelatedItems(RelatedItems.RelatedItem.ContentTab(),
                                          RelatedItems.RelatedItem
                                                      .EntityGrid(EntityName.ThemeOrganizationUnit, () => ErmConfigLocalization.CrdRelThemeOrganizationUnit)
                                                      .AppendapleEntity<OrganizationUnit>(),
                                          RelatedItems.RelatedItem
                                                      .EntityGrid(EntityName.ThemeCategory, () => ErmConfigLocalization.CrdRelThemeCategory)
                                                      .DisableOn<IOrganizationUnitCountAspect>(x => x.OrganizationUnitCount == 0)
                                                      .AppendapleEntity<Category>());
    }
}