using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata Limit =
            CardMetadata.For<Limit>()
                        .WithDefaultIcon()
                        .Actions
                        .Attach(ToolbarElements.Create<Limit>(),
                                ToolbarElements.Update<Limit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.CreateAndClose<Limit>(),
                                ToolbarElements.UpdateAndClose<Limit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<Limit>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Additional(ToolbarElements.Limits.Approve()
                                                                          .DisableOn<ILimitViewModel>(x => x.IsNew,
                                                                                                      x => x.Status == LimitStatus.Approved,
                                                                                                      x => x.Status == LimitStatus.Rejected),
                                                           ToolbarElements.Limits.Reject()
                                                                          .DisableOn<ILimitViewModel>(x => x.IsNew,
                                                                                                      x => x.Status == LimitStatus.Approved,
                                                                                                      x => x.Status == LimitStatus.Rejected),
                                                           ToolbarElements.Limits.Open()
                                                                          .DisableOn<ILimitViewModel>(x => x.IsNew,
                                                                                                      x => x.Status == LimitStatus.Opened),
                                                           ToolbarElements.Limits.Recalculate()
                                                                          .DisableOn<ILimitViewModel>(x => x.IsNew)),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close());
    }
}