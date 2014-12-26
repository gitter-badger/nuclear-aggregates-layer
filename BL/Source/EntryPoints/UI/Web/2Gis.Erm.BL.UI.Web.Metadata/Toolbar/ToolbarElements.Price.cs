using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Price;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.ControlTypes;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Toolbar
{
    public sealed partial class ToolbarElements
    {
        public static class Prices
        {
            public static UIElementMetadataBuilder Publish()
            {
                return UIElementMetadata.Config
                                        .Name.Static("PublishPrice")
                                        .Title.Resource(() => ErmConfigLocalization.ControlPublishPrice)
                                        .Icon.Path(Icons.Icons.Toolbar.Refresh)
                                        .ControlType(ControlType.TextImageButton)
                                        .JSHandler("Publish")
                                        .LockOnInactive()
                                        .LockOnNew()

                                        // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                        .AccessWithPrivelege<Price>(EntityAccessTypes.Create)
                                        .AccessWithPrivelege<Price>(EntityAccessTypes.Update)
                                        .Operation.NonCoupled<PublishPriceIdentity>();
            }

            public static UIElementMetadataBuilder Unpublish()
            {
                return UIElementMetadata.Config
                                        .Name.Static("UnpublishPrice")
                                        .Title.Resource(() => ErmConfigLocalization.ControlUnpublishPrice)
                                        .Icon.Path(Icons.Icons.Toolbar.Refresh)
                                        .ControlType(ControlType.TextImageButton)
                                        .JSHandler("Unpublish")
                                        .LockOnNew()

                                        // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                        .AccessWithPrivelege<Price>(EntityAccessTypes.Create)
                                        .AccessWithPrivelege<Price>(EntityAccessTypes.Update)
                                        .Operation.NonCoupled<UnpublishPriceIdentity>();
            }

            public static UIElementMetadataBuilder Copy()
            {
                return UIElementMetadata.Config
                                        .Name.Static("CopyPrice")
                                        .Title.Resource(() => ErmConfigLocalization.ControlCopyPrice)
                                        .Icon.Path(Icons.Icons.Toolbar.Refresh)
                                        .ControlType(ControlType.TextImageButton)
                                        .JSHandler("Copy")
                                        .LockOnNew()

                                        // COMMENT {all, 01.12.2014}: а зачем права на создание?
                                        .AccessWithPrivelege<Price>(EntityAccessTypes.Create)
                                        .AccessWithPrivelege<Price>(EntityAccessTypes.Update)
                                        .Operation.NonCoupled<CopyPriceIdentity>();
            }
        }
    }
}
