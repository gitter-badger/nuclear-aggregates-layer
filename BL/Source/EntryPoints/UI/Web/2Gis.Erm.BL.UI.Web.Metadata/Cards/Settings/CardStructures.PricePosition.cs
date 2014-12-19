using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata PricePosition =
            CardMetadata.For<PricePosition>()
                        .MainAttribute<PricePosition, IPricePositionViewModel>(x => x.Position.Value)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<PricePosition>(),
                                UIElementMetadata.Config.UpdateAction<PricePosition>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<PricePosition>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<PricePosition>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<PricePosition>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.AdditionalActions(

                                                                           // COMMENT {all, 01.12.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("CopyPricePosition")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlCopyPricePosition)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .Handler.Name("scope.CopyPricePosition")
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Operation.NonCoupled<CopyPricePositionIdentity>()),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(UIElementMetadata.Config.ContentTab("en_ico_16_PricePosition.gif"),
                                            UIElementMetadata.Config
                                                             .Name.Static("AssociatedPositionsGroup")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelAssociatedPositionsGroup)
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.AssociatedPositionsGroup)
                                                             .FilterToParent(),
                                            UIElementMetadata.Config
                                                             .Name.Static("DeniedPosition")
                                                             .Title.Resource(() => ErmConfigLocalization.CrdRelDeniedPosition)
                                                             .ExtendedInfo(new TemplateDescriptor(
                                                                               new StaticStringResourceDescriptor("PositionId={0}&&PriceId={1}"),
                                                                               new PropertyDescriptor<IPricePositionViewModel>(x => x.Position.Key),
                                                                               new PropertyDescriptor<IPricePositionViewModel>(x => x.Price.Key)))
                                                             .LockOnNew()
                                                             .Handler.ShowGridByConvention(EntityName.DeniedPosition));
    }
}