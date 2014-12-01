using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Bargain =
            CardMetadata.For<Bargain>()
                        .MainAttribute<Bargain, IBargainViewModel>(x => x.Number)                
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Bargain>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Bargain>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Bargain>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.PrintActions(UiElementMetadata.Config
                                                                                           .Name.Static("PrintBargainAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .LockOnNew()
                                                                                           .Handler.Name("scope.PrintBargain")
                                                                                           .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                                          UiElementMetadata.Config
                                                                                           .Name.Static("PrintNewSalesModelBargainAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .LockOnNew()
                                                                                           .Handler.Name("scope.PrintNewSalesModelBargain")
                                                                                           .Operation.SpecificFor<PrintIdentity, Bargain>(),
                                                                          UiElementMetadata.Config
                                                                                           .Name.Static("PrintBargainProlongationAgreementAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainProlongationAgreementAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .LockOnNew()
                                                                                           .Handler.Name("scope.PrintBargainProlongationAgreement")),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction())
                        .ConfigRelatedItems(
                                    UiElementMetadata.Config
                                                     .Name.Static("BargainFiles")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelBargainFiles)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.BargainFile)
                                                     .FilterToParent(),
                                    UiElementMetadata.Config
                                                     .Name.Static("Orders")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Order)
                                                     .FilterToParent());
    }
}