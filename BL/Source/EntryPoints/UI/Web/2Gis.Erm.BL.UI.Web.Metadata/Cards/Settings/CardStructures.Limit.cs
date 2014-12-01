using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Limit =
            CardMetadata.For<Limit>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(UiElementMetadata.Config.SaveAction<Limit>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.SaveAndCloseAction<Limit>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.RefreshAction<Limit>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.AdditionalActions( // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("ApproveLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlApproveLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.ApproveLimit"),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("RejectLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlRejectLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.RejectLimit"),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("OpenLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlOpenLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.OpenLimit"),

                                                                           UiElementMetadata.Config
                                                                                            .Name.Static("RecalculateLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlRecalculateLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.RecalculateLimit")
                                                                                            .AccessWithPrivelege(FunctionalPrivilegeName.LimitRecalculation)),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CloseAction());
    }
}