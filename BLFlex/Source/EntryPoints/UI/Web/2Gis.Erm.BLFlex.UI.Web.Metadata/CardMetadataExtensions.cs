using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLFlex.UI.Web.Mvc.Toolbar;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BLFlex.UI.Web.Mvc
{
    public static class CardMetadataExtensions
    {
        public static CardMetadataBuilder<Order> MultiCultureConfigOrderToolbarWithSpecificPrintActions(this CardMetadataBuilder<Order> metadataBuilder,
                                                                                                        params UIElementMetadata[] printActions)
        {
            return metadataBuilder.Actions
                                  .Attach(ToolbarElements.Create<Order>(),
                                          ToolbarElements.Update<Order>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.CreateAndClose<Order>(),
                                          ToolbarElements.UpdateAndClose<Order>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Refresh<Order>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Print(printActions),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Additional(UIElementMetadata.Config.OrderAdditionalActions()),
                                          ToolbarElements.Close());
        }

        public static CardMetadataBuilder<Order> RussiConfigOrderToolbarWithSpecificPrintActions(this CardMetadataBuilder<Order> metadataBuilder,
                                                                                                 params UIElementMetadata[] printActions)
        {
            return metadataBuilder.Actions
                                  .Attach(ToolbarElements.Create<Order>(),
                                          ToolbarElements.Update<Order>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.CreateAndClose<Order>(),
                                          ToolbarElements.UpdateAndClose<Order>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Refresh<Order>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Print(printActions),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Additional(UIElementMetadata.Config.OrderAdditionalActions()),
                                          ToolbarElements.Splitter(),
                                          ToolbarElementsFlex.Orders.ManageDocumentsDebt(),
                                          ToolbarElements.Close());
        }

        public static CardMetadataBuilder<Bargain> ConfigBargainToolbarWithSpecificPrintActions(this CardMetadataBuilder<Bargain> metadataBuilder,
                                                                                                params UIElementMetadata[] printActions)
        {
            return metadataBuilder.Actions
                                  .Attach(ToolbarElements.Create<Bargain>(),
                                          ToolbarElements.Update<Bargain>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.CreateAndClose<Bargain>(),
                                          ToolbarElements.UpdateAndClose<Bargain>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Refresh<Bargain>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Print(printActions),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Close());
        }

        public static CardMetadataBuilder<LegalPerson> ConfigLegalPersonToolbarWithSpecificAdditionalActions(this CardMetadataBuilder<LegalPerson> metadataBuilder, params UIElementMetadata[] additionalActions)
        {
            return metadataBuilder.Actions
                                  .Attach(ToolbarElements.Create<LegalPerson>(),
                                          ToolbarElements.Update<LegalPerson>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.CreateAndClose<LegalPerson>(),
                                          ToolbarElements.UpdateAndClose<LegalPerson>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Refresh<LegalPerson>(),
                                          ToolbarElements.Additional(additionalActions),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Close());
        }

        public static CardMetadataBuilder<Bill> ConfigBillToolbarWithPrinting(this CardMetadataBuilder<Bill> metadataBuilder)
        {
            return metadataBuilder.Actions
                                  .Attach(ToolbarElements.Create<Bill>(),
                                          ToolbarElements.Update<Bill>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.CreateAndClose<Bill>(),
                                          ToolbarElements.UpdateAndClose<Bill>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Refresh<Bill>(),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Print(ToolbarElementsFlex.Bills.PrintBill()),
                                          ToolbarElements.Splitter(),
                                          ToolbarElements.Close());
        }
    }
}