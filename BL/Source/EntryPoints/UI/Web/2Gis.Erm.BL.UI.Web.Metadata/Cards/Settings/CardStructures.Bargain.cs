using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        // TODO {y.baranihin, 26.11.2014}: Заполнить MainAttribute
        // .MainAttribute<Bargain, BargainViewModel>(x => x.Number)
        public static readonly CardMetadata Bargain =
            CardMetadata.For<Bargain>()
                        
                        .Actions
                            .Attach(UiElementMetadata.Config.SaveAction<Bargain>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.SaveAndCloseAction<Bargain>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.RefreshAction <Bargain>(),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.PrintActions(UiElementMetadata.Config
                                                                                           .Name.Static("PrintBargainAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .LockOnNew()
                                                                                           .Handler.Name("scope.PrintBargain"),
                                                                          UiElementMetadata.Config
                                                                                           .Name.Static("PrintNewSalesModelBargainAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintNewSalesModelBargainAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .LockOnNew()
                                                                                           .Handler.Name("scope.PrintNewSalesModelBargain"),
                                                                          UiElementMetadata.Config
                                                                                           .Name.Static("PrintBargainProlongationAgreementAction")
                                                                                           .Title.Resource(() => ErmConfigLocalization.ControlPrintBargainProlongationAgreementAction)
                                                                                           .ControlType(ControlType.TextButton)
                                                                                           .LockOnNew()
                                                                                           .Handler.Name("scope.PrintBargainProlongationAgreement")),
                                    UiElementMetadata.Config.SplitterAction(),
                                    UiElementMetadata.Config.CloseAction())
                        .RelatedItems
                            .Name("Information")
                            .Title(() => ErmConfigLocalization.CrdRelInformationHeader)
                            .Attach(UiElementMetadata.Config.ContentTab(),
                                    UiElementMetadata.Config
                                                     .Name.Static("BargainFiles")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelBargainFiles)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.BargainFile),
                                    UiElementMetadata.Config
                                                     .Name.Static("Orders")
                                                     .Title.Resource(() => ErmConfigLocalization.CrdRelOrders)
                                                     .LockOnNew()
                                                     .Handler.ShowGridByConvention(EntityName.Order));
    }
}