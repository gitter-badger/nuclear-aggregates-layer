using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Advertisement;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Firm =
            CardMetadata.For<Firm>()
                        .MainAttribute<Firm, IFirmViewModel>(x => x.Name)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<Firm>(),
                                UIElementMetadata.Config.UpdateAction<Firm>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<Firm>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<Firm>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<Firm>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.AdditionalActions(UIElementMetadata.Config.ChangeOwnerAction<Firm>(),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("ChangeFirmClient")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeFirmClient)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.ChangeFirmClient")
                                                                                            .Operation.SpecificFor<ChangeClientIdentity, Firm>(),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("ChangeTerritory")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlChangeTerritory)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.ChangeTerritory")
                                                                                            .Operation.SpecificFor<ChangeTerritoryIdentity, Firm>(),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("AssignWhiteListedAd")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlAssignWhiteListedAd)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.AssignWhiteListedAd")
                                                                                            .Operation.NonCoupled<SelectAdvertisementToWhitelistIdentity>(),

                                                                           UIElementMetadata.Config.QualifyAction<Firm>()),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction())
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab(),
                                            UIElementMetadata.Config
                                                             .Name.Static("FirmAddresses")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAddresses)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.FirmAddress)
                                                             .FilterToParent(),

                                            UIElementMetadata.Config
                                                             .Name.Static("FirmCategories")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelCategories)
                                                             .Icon.Path("en_ico_16_Category.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.CategoryFirmAddress)
                                                             .FilterToParent(),

                                            UIElementMetadata.Config
                                                             .Name.Static("FirmAdvertisements")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelFirmAdvertisements)
                                                             .Icon.Path("en_ico_16_Advertisement.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Advertisement)
                                                             .FilterToParent(),

                                            UIElementMetadata.Config
                                                             .Name.Static("Orders")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                             .Icon.Path("en_ico_16_Order.gif")
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.Order)
                                                             .FilterToParent(),

                                            UIElementMetadata.Config
                                                             .Name.Static("Actions")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                                             .Icon.Path("en_ico_16_Action.gif")
                                                             .Handler.ShowGridByConvention(EntityName.Activity)
                                                             .FilterToParents()
                                                             .LockOnNew());
    }
}