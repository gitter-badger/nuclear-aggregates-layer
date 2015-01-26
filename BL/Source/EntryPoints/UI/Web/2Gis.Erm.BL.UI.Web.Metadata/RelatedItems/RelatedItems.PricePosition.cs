using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.BLCore.UI.Metadata.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using Humanizer;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static class PricePosition
        {
            public static UIElementMetadataBuilder DeniedPositionsGrid()
            {
                return UIElementMetadata.Config
                                        .Name.Static(EntityName.DeniedPosition.ToString().Pluralize())
                                        .Title.Resource(() => ErmConfigLocalization.CrdRelDeniedPosition)
                                        .ExtendedInfo(new TemplateDescriptor(
                                                          new StaticStringResourceDescriptor("PositionId={0}&&PriceId={1}"),
                                                          new PropertyDescriptor<IPositionAspect>(x => x.PositionId),
                                                          new PropertyDescriptor<IPriceAspect>(x => x.PriceId)))
                                        .LockOnNew()
                                        .Handler.ShowGridByConvention(EntityName.DeniedPosition);
            }
        }
    }
}
