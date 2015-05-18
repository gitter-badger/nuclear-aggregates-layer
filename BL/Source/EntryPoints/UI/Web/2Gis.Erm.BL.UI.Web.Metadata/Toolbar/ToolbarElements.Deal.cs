using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Deals
        {
            public static UIElementMetadataBuilder CloseDeal()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                        .Name.Static("CloseDeal")
                                        .Title.Resource(() => ErmConfigLocalization.ControlCloseDeal)
                                        .ControlType(ControlType.TextButton)
                                        .LockOnInactive()
                                        .JSHandler("CloseDeal");
            }

            public static UIElementMetadataBuilder Reopen()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                        .Name.Static("ReopenDeal")
                                        .Title.Resource(() => ErmConfigLocalization.ControlReopenDeal)
                                        .ControlType(ControlType.TextButton)
                                        .JSHandler("ReopenDeal");
            }

            // TODO {all, 23.12.2014}: У фирмы тоже есть смена клиента. Кнопки визуально отличаются надписями. Вероятно, их можно объединить.
            public static UIElementMetadataBuilder ChangeClient()
            {
                return

                    // COMMENT {all, 28.11.2014}: а как же безопасность?
                    UIElementMetadata.Config
                                        .Name.Static("ChangeDealClient")
                                        .Title.Resource(() => ErmConfigLocalization.ControlChangeDealClient)
                                        .ControlType(ControlType.TextButton)
                                        .LockOnInactive()
                                        .JSHandler("ChangeDealClient")
                                        .Operation.SpecificFor<ChangeClientIdentity, Deal>();
            }
        }
    }
}
