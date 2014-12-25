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
        public static readonly CardMetadata AdvertisementElement =
            CardMetadata.For<AdvertisementElement>()
                        .WithDefaultIcon()
                        .Actions
                        .Attach(ToolbarElements.Create<AdvertisementElement>()
                                               .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                ToolbarElements.Update<AdvertisementElement>()
                                               .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                ToolbarElements.AdvertisementElements.ResetToDraft()
                                               .DisableOn<IAdvertisementElementViewModel>(x => x.CanUserChangeStatus,
                                                                                          x => x.DisableEdit,
                                                                                          x => x.Status == AdvertisementElementStatusValue.Draft)
                                               .HideOn<IAdvertisementElementViewModel>(x => !x.NeedsValidation),
                                ToolbarElements.Splitter(),
                                ToolbarElements.AdvertisementElements.SaveAndVerify()
                                               .HideOn<IAdvertisementElementViewModel>(x => !x.NeedsValidation)
                                               .DisableOn<IAdvertisementElementViewModel>(x => x.Status != AdvertisementElementStatusValue.Draft),
                                ToolbarElements.CreateAndClose<AdvertisementElement>()
                                               .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                ToolbarElements.UpdateAndClose<AdvertisementElement>()
                                               .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<AdvertisementElement>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close());
    }
}