using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc
{
    public static class CardMetadataExtensions
    {
        public static CardMetadataBuilder<Order> ConfigOrderToolbarWithSpecificPrintActions(this CardMetadataBuilder<Order> metadataBuilder, params UIElementMetadata[] printActions)
        {
            return metadataBuilder.Actions
                                  .Attach(UIElementMetadata.Config.CreateAction<Order>(),
                                          UIElementMetadata.Config.UpdateAction<Order>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.CreateAndCloseAction<Order>(),
                                          UIElementMetadata.Config.UpdateAndCloseAction<Order>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.RefreshAction<Order>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.PrintActions(printActions),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.AdditionalActions(UIElementMetadata.Config.OrderAdditionalActions()),
                                          UIElementMetadata.Config.CloseAction());
        }

        public static CardMetadataBuilder<Bargain> ConfigBargainToolbarWithSpecificPrintActions(this CardMetadataBuilder<Bargain> metadataBuilder,
                                                                                                params UIElementMetadata[] printActions)
        {
            return metadataBuilder.Actions
                                  .Attach(UIElementMetadata.Config.CreateAction<Bargain>(),
                                          UIElementMetadata.Config.UpdateAction<Bargain>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.CreateAndCloseAction<Bargain>(),
                                          UIElementMetadata.Config.UpdateAndCloseAction<Bargain>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.RefreshAction<Bargain>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.PrintActions(printActions),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.CloseAction());
        }

        public static CardMetadataBuilder<LegalPerson> ConfigLegalPersonToolbarWithSpecificAdditionalActions(this CardMetadataBuilder<LegalPerson> metadataBuilder, params UIElementMetadata[] additionalActions)
        {
            return metadataBuilder.Actions
                                  .Attach(UIElementMetadata.Config.CreateAction<LegalPerson>(),
                                          UIElementMetadata.Config.UpdateAction<LegalPerson>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.CreateAndCloseAction<LegalPerson>(),
                                          UIElementMetadata.Config.UpdateAndCloseAction<LegalPerson>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.RefreshAction<LegalPerson>(),
                                          UIElementMetadata.Config.AdditionalActions(additionalActions),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.CloseAction());
        }

        public static CardMetadataBuilder<Bill> ConfigBillToolbarWithPrinting(this CardMetadataBuilder<Bill> metadataBuilder)
        {
            return metadataBuilder.Actions
                                  .Attach(UIElementMetadata.Config.CreateAction<Bill>(),
                                          UIElementMetadata.Config.UpdateAction<Bill>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.CreateAndCloseAction<Bill>(),
                                          UIElementMetadata.Config.UpdateAndCloseAction<Bill>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.RefreshAction<Bill>(),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.PrintActions(UIElementMetadata.Config
                                                                                                 .Name.Static("PrintBillAction")
                                                                                                 .Title.Resource(() => ErmConfigLocalization.ControlPrintBillAction)
                                                                                                 .ControlType(ControlType.TextButton)
                                                                                                 .Handler.Name("scope.PrintBill")
                                                                                                 .LockOnNew()
                                                                                                 .Operation.SpecificFor<PrintIdentity, Bill>()),
                                          UIElementMetadata.Config.SplitterAction(),
                                          UIElementMetadata.Config.CloseAction());
        }
    }
}