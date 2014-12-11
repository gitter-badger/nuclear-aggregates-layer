using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Themes;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Theme =
            CardMetadata.For<Theme>()
                        .MainAttribute<Theme, IThemeViewModel>(x => x.Name)
                        .Actions
                        .Attach(UiElementMetadata.Config.CreateAction<Theme>(),
                                UiElementMetadata.Config.UpdateAction<Theme>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CreateAndCloseAction<Theme>(),
                                UiElementMetadata.Config.UpdateAndCloseAction<Theme>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.RefreshAction<Theme>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.AdditionalActions(UiElementMetadata.Config
                                                                                            .Name.Static("SetDefaultTheme")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlSetDefaultTheme)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.SetDefaultTheme")

                                                                               // COMMENT {all, 01.12.2014}: А зачем права на создание? 
                                                                                            .AccessWithPrivelege<Theme>(EntityAccessTypes.Create)
                                                                                            .AccessWithPrivelege<Theme>(EntityAccessTypes.Update)
                                                                                            .Operation.NonCoupled<SetAsDefaultThemeIdentity>(),

                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("UnSetDefaultTheme")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlUnSetDefaultTheme)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.UnSetDefaultTheme")

                                                                               // COMMENT {all, 01.12.2014}: А зачем права на создание? 
                                                                                            .AccessWithPrivelege<Theme>(EntityAccessTypes.Create)
                                                                                            .AccessWithPrivelege<Theme>(EntityAccessTypes.Update)
                                                                                            .Operation.NonCoupled<SetAsDefaultThemeIdentity>()),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab(),
                                            UiElementMetadata.Config
                                                             .Name.Static("ThemeOrganizationUnit")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelThemeOrganizationUnit)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.ThemeOrganizationUnit)
                                                             .FilterToParent()
                                                             .AppendapleEntity<OrganizationUnit>(),
                                            UiElementMetadata.Config
                                                             .Name.Static("ThemeCategory")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelThemeCategory)
                                                             .LockOnNew()
                                                             .DisableOn<IThemeViewModel>(x => x.OrganizationUnitCount == 0)
                                                             .Handler.ShowGridByConvention(EntityName.ThemeCategory)
                                                             .FilterToParent()
                                                             .AppendapleEntity<Category>());
    }
}