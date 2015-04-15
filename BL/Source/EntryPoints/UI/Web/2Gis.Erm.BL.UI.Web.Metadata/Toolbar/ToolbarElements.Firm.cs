using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Advertisement;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Firms
        {
            public static UIElementMetadataBuilder AssignWhiteListedAd()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("AssignWhiteListedAd")
                                     .Title.Resource(() => ErmConfigLocalization.ControlAssignWhiteListedAd)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .JSHandler("AssignWhiteListedAd")
                                     .Operation.NonCoupled<SelectAdvertisementToWhitelistIdentity>();
            }

            // TODO {all, 23.12.2014}: У сделки тоже есть смена клиента. Кнопки визуально отличаются надписями. Вероятно, их можно объединить.
            public static UIElementMetadataBuilder ChangeClient()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                     .Name.Static("ChangeFirmClient")
                                     .Title.Resource(() => ErmConfigLocalization.ControlChangeFirmClient)
                                     .ControlType(ControlType.TextButton)
                                     .LockOnInactive()
                                     .JSHandler("ChangeFirmClient")
                                     .Operation.SpecificFor<ChangeClientIdentity, Firm>();
            }
        }
    }
}
