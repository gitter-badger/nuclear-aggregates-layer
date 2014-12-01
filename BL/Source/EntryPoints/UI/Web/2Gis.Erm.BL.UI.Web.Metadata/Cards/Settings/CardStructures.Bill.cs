using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Bill =
            CardMetadata.For<Bill>()
                        .MainAttribute<Bill, IBillViewModel>(x => x.BillNumber)                
                        .Actions
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
                                                                                           .Handler.Name("scope.PrintBill"))
                                                                                           .LockOnNew()
                                                                                           .Operation.SpecificFor<PrintIdentity, Bill>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction());
    }
}