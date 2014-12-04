using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Price =
            CardMetadata.For<Price>()
                        .MainAttribute<Price, IPriceViewModel>(x => x.Name)
                        .ConfigRelatedItems(UiElementMetadata.Config
                                                             .Name.Static("PricePosition")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelPricePosition)
                                                             .Icon.Path("en_ico_16_PricePosition.gif")
                                                             .Handler.ShowGridByConvention(EntityName.PricePosition)
                                                             .LockOnNew()
                                                             .FilterToParent())
                        .Actions
                            .Attach(UiElementMetadata.Config.ContentTab(),
                                    UiElementMetadata.Config.SaveAction<Price>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Price>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Price>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config
                                                     .Name.Static("PublishPrice")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlPublishPrice)
                                                     .Icon.Path("Refresh.gif")
                                                     .ControlType(ControlType.TextImageButton)
                                                     .Handler.Name("scope.Publish")
                                                     .LockOnInactive()
                                                     .LockOnNew()

                                                     // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                                     .AccessWithPrivelege<Price>(EntityAccessTypes.Create)
                                                     .AccessWithPrivelege<Price>(EntityAccessTypes.Update)
                                                     .Operation.NonCoupled<PublishPriceIdentity>(),

                                    UiElementMetadata.Config
                                                     .Name.Static("UnpublishPrice")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlUnpublishPrice)
                                                     .Icon.Path("Refresh.gif")
                                                     .ControlType(ControlType.TextImageButton)
                                                     .Handler.Name("scope.Unpublish")
                                                     .LockOnNew()

                                                     // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                                     .AccessWithPrivelege<Price>(EntityAccessTypes.Create)
                                                     .AccessWithPrivelege<Price>(EntityAccessTypes.Update)
                                                     .Operation.NonCoupled<UnpublishPriceIdentity>(),

                                    UiElementMetadata.Config
                                                     .Name.Static("CopyPrice")
                                                     .Title.Resource(() => ErmConfigLocalization.ControlCopyPrice)
                                                     .Icon.Path("Refresh.gif")
                                                     .ControlType(ControlType.TextImageButton)
                                                     .Handler.Name("scope.Copy")
                                                     .LockOnNew()

                                                     // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                                     .AccessWithPrivelege<Price>(EntityAccessTypes.Create)
                                                     .AccessWithPrivelege<Price>(EntityAccessTypes.Update)
                                                     .Operation.NonCoupled<CopyPriceIdentity>(),

                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction());
    }
}