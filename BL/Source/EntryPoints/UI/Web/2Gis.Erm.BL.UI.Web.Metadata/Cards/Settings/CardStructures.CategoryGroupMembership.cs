using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.Operations.Generic;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata CategoryGroupMembership =
            CardMetadata.Config
                        .For(EntityName.CategoryGroupMembership)
                        .Actions
                            .Attach(UiElementMetadata.Config
                                                     .Name.Static("Save")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlSave)
                                                     .ControlType(ControlType.ImageButton)
                                                     .LockOnInactive()
                                                     .Handler.Name("scope.Save")
                                                     .Icon.Path("Save.gif")
                                                     .AccessWithPrivelege(EntityAccessTypes.Update, EntityName.OrganizationUnit),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config
                                                     .Name.Static("SaveAndClose")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlSaveAndClose)
                                                     .ControlType(ControlType.ImageButton)
                                                     .LockOnInactive()
                                                     .Handler.Name("scope.SaveAndClose")
                                                     .Icon.Path("Save.gif")
                                                     .AccessWithPrivelege(EntityAccessTypes.Update, EntityName.OrganizationUnit)
                                                     .Operation.NonCoupled<CloseIdentity>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Refresh")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlRefresh)
                                                     .ControlType(ControlType.TextImageButton)
                                                     .Handler.Name("scope.refresh")
                                                     .Icon.Path("Refresh.gif"),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config
                                                     .Name.Static("ViewCategoryGroups")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlViewCategoryGroups)
                                                     .ControlType(ControlType.TextImageButton)
                                                     .Handler.Name("scope.ViewCategoryGroups")
                                                     .Icon.Path("en_ico_16_Category.gif"),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction());
    }
}