using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardMetadatas
    {
        public static readonly CardMetadata AdvertisementElement =
            CardMetadata.For<AdvertisementElement>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(UIElementMetadata.Config.CreateAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UIElementMetadata.Config.UpdateAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UIElementMetadata.Config
                                                 .Name.Static("ResetToDraft")
                                                 .Title.Resource(() => MetadataResources.ControlResetToDraft)
                                                 .ControlType(ControlType.TextButton)
                                                 .Handler.Name("scope.ResetToDraft")
                                                 .DisableOn<IAdvertisementElementViewModel>(x => x.CanUserChangeStatus,
                                                                                            x => x.DisableEdit,
                                                                                            x => x.Status == AdvertisementElementStatusValue.Draft)
                                                 .HideOn<IAdvertisementElementViewModel>(x => !x.NeedsValidation)
                                                 .Operation.NonCoupled<ChangeAdvertisementElementStatusIdentity>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config
                                                 .Name.Static("SaveAndVerify")
                                                 .Title.Resource(() => MetadataResources.ControlSaveAndVerify)
                                                 .ControlType(ControlType.TextButton)
                                                 .LockOnInactive()
                                                 .Handler.Name("scope.SaveAndVerify")
                                                 .Icon.Path("Save.gif")
                                                 .HideOn<IAdvertisementElementViewModel>(x => !x.NeedsValidation)
                                                 .DisableOn<IAdvertisementElementViewModel>(x => x.Status != AdvertisementElementStatusValue.Draft)
                                                 .AccessWithPrivelege<AdvertisementElement>(EntityAccessTypes.Create)
                                                 .AccessWithPrivelege<AdvertisementElement>(EntityAccessTypes.Update)
                                                 .Operation.SpecificFor<CreateIdentity, AdvertisementElement>()
                                                 .Operation.SpecificFor<UpdateIdentity, AdvertisementElement>()
                                                 .Operation.NonCoupled<ChangeAdvertisementElementStatusIdentity>(),
                                UIElementMetadata.Config.CreateAndCloseAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UIElementMetadata.Config.UpdateAndCloseAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.RefreshAction<AdvertisementElement>(),
                                UIElementMetadata.Config.SplitterAction(),
                                UIElementMetadata.Config.CloseAction());
    }
}