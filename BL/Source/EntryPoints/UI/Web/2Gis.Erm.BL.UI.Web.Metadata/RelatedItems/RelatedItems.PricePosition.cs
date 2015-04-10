using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.Model.Aspects.Entities;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using Humanizer;

using NuClear.Metamodeling.UI.Elements.Aspects.Features;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Handler;
using NuClear.Metamodeling.UI.Elements.Aspects.Features.Resources;
using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static class PricePosition
        {
            public static UIElementMetadataBuilder DeniedPositionsGrid()
            {
                return UIElementMetadata.Config
                                        .Name.Static(EntityType.Instance.DeniedPosition().Description.Pluralize())
                                        .Title.Resource(() => ErmConfigLocalization.CrdRelDeniedPosition)
                                        .ExtendedInfo(new TemplateDescriptor(
                                                          new StaticStringResourceDescriptor("PositionId={0}&&PriceId={1}"),
                                                          new PropertyDescriptor<IPositionAspect>(x => x.PositionId),
                                                          new PropertyDescriptor<IPriceAspect>(x => x.PriceId)))
                                        .LockOnNew()
                                        .Handler.ShowGridByConvention(EntityType.Instance.DeniedPosition());
            }
        }
    }
}
