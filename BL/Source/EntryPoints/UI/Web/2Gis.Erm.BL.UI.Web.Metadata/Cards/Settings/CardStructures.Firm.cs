using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Advertisement;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Firm =
            CardMetadata.For<Firm>()
                        .MainAttribute<Firm, IFirmViewModel>(x => x.Name)
                        .Actions
                        .Attach(UiElementMetadata.Config.SaveAction<Firm>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.SaveAndCloseAction<Firm>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.RefreshAction<Firm>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.AdditionalActions(UiElementMetadata.Config.ChangeOwnerAction<Firm>(),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("ChangeFirmClient")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeFirmClient)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.ChangeFirmClient")
                                                                                            .Operation.SpecificFor<ChangeClientIdentity, Firm>(),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("ChangeTerritory")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeTerritory)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.ChangeTerritory")
                                                                                            .Operation.SpecificFor<ChangeTerritoryIdentity, Firm>(),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("AssignWhiteListedAd")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlAssignWhiteListedAd)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.AssignWhiteListedAd")
                                                                                            .Operation.NonCoupled<SelectAdvertisementToWhitelistIdentity>(),

                                                                           UiElementMetadata.Config.QualifyAction<Firm>()),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UiElementMetadata.Config.ContentTab(),
                                            UiElementMetadata.Config
                                                             .Name.Static("FirmAddresses")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAddresses)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.FirmAddress)
                                                             .FilterToParent(),

                                            UiElementMetadata.Config
                                                             .Name.Static("FirmCategories")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelCategories)
                                                             .Icon.Path("en_ico_16_Category.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.CategoryFirmAddress)
                                                             .FilterToParent(),

                                            UiElementMetadata.Config
                                                             .Name.Static("FirmAdvertisements")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAdvertisements)
                                                             .Icon.Path("en_ico_16_Advertisement.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Advertisement)
                                                             .FilterToParent(),

                                            UiElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .Icon.Path("en_ico_16_Order.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Order)
                                                             .FilterToParent(),

                                            UiElementMetadata.Config
                                                             .Name.Static("ActivityHistory")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelActivityHistory)
                                                             .Icon.Path("en_ico_16_history.gif")
                                                             .LockOnNew(),

                                            UiElementMetadata.Config
                                                             .Name.Static("Actions")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                             .Icon.Path("en_ico_16_Action.gif")
                                                             .LockOnNew());
    }
}