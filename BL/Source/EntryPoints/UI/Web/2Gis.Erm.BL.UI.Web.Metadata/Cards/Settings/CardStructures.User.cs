using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Security;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata User =
            CardMetadata.For<User>()
                        .MainAttribute<User, IUserViewModel>(x => x.DisplayName)
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<User>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<User>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<User>(),
                                    UiElementMetadata.Config.AdditionalActions(
                                                                               // COMMENT {all, 01.12.2014}: а как же безопасность? 
                                                                               UiElementMetadata.Config
                                                                                                .Name.Static("ShowUserProfile")
                                                                                                .Title.Resource(() => ErmConfigLocalization.EnUserProfile)
                                                                                                .LockOnNew()
                                                                                                .ControlType(ControlType.TextButton)
                                                                                                .Handler.Name("scope.ProcessUserProfile")),
                                    UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config
                                                             .Name.Static("UserRole")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelUserRole)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.UserRole)
                                                             .FilterToParent()
                                                             .AppendapleEntity<Role>(),
                                            UiElementMetadata.Config
                                                             .Name.Static("UserTerritory")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelUserTerritory)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.UserTerritory)
                                                             .FilterToParent()
                                                             .AppendapleEntity<Territory>(),
                                            UiElementMetadata.Config
                                                             .Name.Static("UserOrganizationUnit")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelUserOrganizationUnit)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.UserOrganizationUnit)
                                                             .FilterToParent()
                                                             .AppendapleEntity<OrganizationUnit>());
    }
}