using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc
{
    public static class CardMetadataExtensions
    {
        public static CardMetadataBuilder<Order> ConfigOrderToolbarWithSpecificPrintActions(this CardMetadataBuilder<Order> metadataBuilder, params UiElementMetadata[] printActions)
        {
            return metadataBuilder.Actions
                                  .Attach(UiElementMetadata.Config.SaveAction<Order>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.SaveAndCloseAction<Order>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.RefreshAction<Order>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.PrintActions(printActions),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.AdditionalActions(UiElementMetadata.Config.OrderAdditionalActions()),
                                          UiElementMetadata.Config.CloseAction());
        }

        public static CardMetadataBuilder<Bargain> ConfigBargainToolbarWithSpecificPrintActions(this CardMetadataBuilder<Bargain> metadataBuilder, params UiElementMetadata[] printActions)
        {
            return metadataBuilder.Actions
                                  .Attach(UiElementMetadata.Config.SaveAction<Bargain>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.SaveAndCloseAction<Bargain>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.RefreshAction<Bargain>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.PrintActions(printActions),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.CloseAction());
        }

        public static CardMetadataBuilder<LegalPerson> ConfigLegalPersonToolbarWithSpecificAdditionalActions(this CardMetadataBuilder<LegalPerson> metadataBuilder, params UiElementMetadata[] additionalActions)
        {
            return metadataBuilder.Actions
                                  .Attach(UiElementMetadata.Config.SaveAction<LegalPerson>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.SaveAndCloseAction<LegalPerson>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.RefreshAction<LegalPerson>(),
                                          UiElementMetadata.Config.AdditionalActions(additionalActions),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.CloseAction());
        }

        public static CardMetadataBuilder<Bill> ConfigBillToolbarWithPrinting(this CardMetadataBuilder<Bill> metadataBuilder)
        {
            return metadataBuilder.Actions
                                  .Attach(UiElementMetadata.Config.SaveAction<Bill>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.SaveAndCloseAction<Bill>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.RefreshAction<Bill>(),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.PrintActions(UiElementMetadata.Config
                                                                                                 .Name.Static("PrintBillAction")
                                                                                                 .Title.Resource(() => ErmConfigLocalization.ControlPrintBillAction)
                                                                                                 .ControlType(ControlType.TextButton)
                                                                                                 .Handler.Name("scope.PrintBill")
                                                                                                 .LockOnNew()
                                                                                                 .Operation.SpecificFor<PrintIdentity, Bill>()),
                                          UiElementMetadata.Config.SplitterAction(),
                                          UiElementMetadata.Config.CloseAction());
        }
    }
}