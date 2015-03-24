using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AdvertisementElement;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class AdvertisementElements
        {
            public static UIElementMetadataBuilder ResetToDraft()
            {
                return UIElementMetadata.Config
                                        .Name.Static("ResetToDraft")
                                        .Title.Resource(() => MetadataResources.ControlResetToDraft)
                                        .ControlType(ControlType.TextButton)
                                        .JSHandler("ResetToDraft")
                                        .Operation.NonCoupled<ResetAdvertisementElementToDraftIdentity>();
            }

            public static UIElementMetadataBuilder SaveAndVerify()
            {
                return UIElementMetadata.Config
                                        .Name.Static("SaveAndVerify")
                                        .Title.Resource(() => MetadataResources.ControlSaveAndVerify)
                                        .ControlType(ControlType.TextButton)
                                        .LockOnInactive()
                                        .JSHandler("SaveAndVerify")
                                        .Icon.Path(Icons.Icons.Toolbar.Save)
                                        .AccessWithPrivelege<AdvertisementElement>(EntityAccessTypes.Create)
                                        .AccessWithPrivelege<AdvertisementElement>(EntityAccessTypes.Update)
                                        .Operation.SpecificFor<CreateIdentity, AdvertisementElement>()
                                        .Operation.SpecificFor<UpdateIdentity, AdvertisementElement>()
                                        .Operation.NonCoupled<TransferAdvertisementElementToReadyForValidationIdentity>();
            }
        }
    }
}
