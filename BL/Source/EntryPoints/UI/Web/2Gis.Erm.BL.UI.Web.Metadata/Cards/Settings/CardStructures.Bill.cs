using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Metadata.Models.Contracts;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Bill =
            CardMetadata.Config
                        .For<Bill>()
                        .MainAttribute<Bill, IBillViewModel>(x => x.BillNumber)                
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Bill>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Bill>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction<Bill>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.PrintActions(

                                                                          // COMMENT {all, 27.11.2014}: серьезно, можно распечатать еще не созданный счет? (Наверное, стоит добавить LockOnNew())
                                                                          UiElementMetadata.Config
                                                                                           .Name.Static("PrintBillAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintBillAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .Handler.Name("scope.PrintBill")),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction());
    }
}