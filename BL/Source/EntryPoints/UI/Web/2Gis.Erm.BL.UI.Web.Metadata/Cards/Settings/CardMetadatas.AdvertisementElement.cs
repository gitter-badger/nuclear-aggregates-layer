using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
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
                                               .HideOn<IAdvertisementElementRequiresVerificationAspect>(x => x.NeedsValidation),
                                ToolbarElements.Update<AdvertisementElement>()
                                               .HideOn<IAdvertisementElementRequiresVerificationAspect>(x => x.NeedsValidation),
                                ToolbarElements.AdvertisementElements.ResetToDraft()
                                               .DisableOn<ISetReadOnlyAspect>(x => x.SetReadonly)
                                               .DisableOn<IAdvertisementElementVerificationAspect>(x => x.CanUserChangeStatus,
                                                                                                   x => x.Status == AdvertisementElementStatusValue.Draft)
                                               .HideOn<IAdvertisementElementRequiresVerificationAspect>(x => !x.NeedsValidation),
                                ToolbarElements.Splitter(),
                                ToolbarElements.AdvertisementElements.SaveAndVerify()
                                               .HideOn<IAdvertisementElementRequiresVerificationAspect>(x => !x.NeedsValidation)
                                               .DisableOn<IAdvertisementElementVerificationStateAspect>(x => x.Status != AdvertisementElementStatusValue.Draft),
                                ToolbarElements.CreateAndClose<AdvertisementElement>()
                                               .HideOn<IAdvertisementElementRequiresVerificationAspect>(x => x.NeedsValidation),
                                ToolbarElements.UpdateAndClose<AdvertisementElement>()
                                               .HideOn<IAdvertisementElementRequiresVerificationAspect>(x => x.NeedsValidation),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Refresh<AdvertisementElement>(),
                                ToolbarElements.Splitter(),
                                ToolbarElements.Close());
    }
}