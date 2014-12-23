using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Price =
            CardMetadata.For<Price>()
                        .MainAttribute<Price, IPriceViewModel>(x => x.Name)
                        .WithRelatedItems(UIElementMetadata.Config.ContentTab("en_ico_16_Price.gif"),
                                            UIElementMetadata.Config
                                                             .Name.Static("PricePosition")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelPricePosition)
                                                             .Icon.Path("en_ico_16_PricePosition.gif")
                                                             .Handler.ShowGridByConvention(EntityName.PricePosition)
                                                             .LockOnNew()
                                                             .FilterToParent())
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<Price>(),
                                UIElementMetadata.Config.UpdateAction<Price>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<Price>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<Price>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<Price>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config
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

                                UIElementMetadata.Config
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

                                UIElementMetadata.Config
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

                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction());
    }
}