using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata Limit =
            CardMetadata.For<Limit>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<Limit>(),
                                UIElementMetadata.Config.UpdateAction<Limit>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CreateAndCloseAction<Limit>(),
                                UIElementMetadata.Config.UpdateAndCloseAction<Limit>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<Limit>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.AdditionalActions( // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("ApproveLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlApproveLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.ApproveLimit"),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("RejectLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlRejectLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.RejectLimit"),

                                                                           // COMMENT {all, 28.11.2014}: а как же безопасность?
                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("OpenLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlOpenLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .LockOnNew()
                                                                                            .Handler.Name("scope.OpenLimit"),

                                                                           UIElementMetadata.Config
                                                                                            .Name.Static("RecalculateLimit")
                                                                                            .Title.Resource(() => ErmConfigLocalization.ControlRecalculateLimit)
                                                                                            .ControlType(ControlType.TextButton)
                                                                                            .LockOnInactive()
                                                                                            .Handler.Name("scope.RecalculateLimit")
                                                                                            .AccessWithPrivelege(FunctionalPrivilegeName.LimitRecalculation)),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction());
    }
}