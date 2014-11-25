using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata
{
    public static class FeatureExtensions
    {
        public static string ToRequestUrl(this ShowGridHandlerFeature showGridHandler)
        {
            return string.Format("/Grid/View/{0}", showGridHandler.EntityName);
        }

        public static string ToDisabledExpression(this LockOnNewFeature feature)
        {
            return "Ext.getDom('Id').value==0";
        }
    }
}