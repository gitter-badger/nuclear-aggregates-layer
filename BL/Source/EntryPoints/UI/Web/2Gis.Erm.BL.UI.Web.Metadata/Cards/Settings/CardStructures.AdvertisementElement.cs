using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards;
using DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements;
using DoubleGis.Erm.Platform.UI.Metadata.UiElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Settings
{
    public static partial class CardStructures
    {
        public static readonly CardMetadata AdvertisementElement =
            CardMetadata.For<AdvertisementElement>()
                        .MainAttribute(x => x.Id)
                        .Actions
                        .Attach(UiElementMetadata.Config.CreateAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UiElementMetadata.Config.UpdateAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UiElementMetadata.Config
                                                 .Name.Static("ResetToDraft")
                                                 .Title.Resource(() => MetadataResources.ControlResetToDraft)
                                                 .ControlType(ControlType.TextButton)
                                                 .Handler.Name("scope.ResetToDraft")
                                                 .DisableOn<IAdvertisementElementViewModel>(x => x.CanUserChangeStatus,
                                                                                            x => x.DisableEdit,
                                                                                            x => x.Status == AdvertisementElementStatusValue.Draft)
                                                 .HideOn<IAdvertisementElementViewModel>(x => !x.NeedsValidation)
                                                 .Operation.NonCoupled<ChangeAdvertisementElementStatusIdentity>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config
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
                                UiElementMetadata.Config.CreateAndCloseAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UiElementMetadata.Config.UpdateAndCloseAction<AdvertisementElement>()
                                                 .HideOn<IAdvertisementElementViewModel>(x => x.NeedsValidation),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.RefreshAction<AdvertisementElement>(),
                                UiElementMetadata.Config.SplitterAction(),
                                UiElementMetadata.Config.CloseAction());
    }
}