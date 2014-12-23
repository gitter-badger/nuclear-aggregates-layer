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
        public static readonly CardMetadata Theme =
            CardMetadata.For<Theme>()
                        .MainAttribute<Theme, IThemeViewModel>(x => x.Name)
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
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                          UIElementMetadata.Config
                                                           .Name.Static("ThemeOrganizationUnit")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelThemeOrganizationUnit)
                                                           .LockOnNew()
                                                           .Handler.ShowGridByConvention(EntityName.ThemeOrganizationUnit)
                                                           .FilterToParent()
                                                           .AppendapleEntity<OrganizationUnit>(),
                                          UIElementMetadata.Config
                                                           .Name.Static("ThemeCategory")
                                                           .Title.Resource(() => ErmConfigLocalization.CrdRelThemeCategory)
                                                           .LockOnNew()
                                                           .DisableOn<IThemeViewModel>(x => x.OrganizationUnitCount == 0)
                                                           .Handler.ShowGridByConvention(EntityName.ThemeCategory)
                                                           .FilterToParent()
                                                           .AppendapleEntity<Category>());
    }
}