using DoubleGis.Erm.BL.UI.Web.Metadata.Cards;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Handler.Concrete;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements.Features;

namespace DoubleGis.Erm.BL.UI.Web.Metadata
{
    public static class FeatureExtensions
    {
        public static string ToRequestUrl(this ShowGridHandlerFeature showGridHandler)
        {
            return string.Format("/Grid/View/{0}", showGridHandler.EntityName);
        }

        // Эта штука нужна только для тестирования. Потом Удалить.
        public static string ToDisabledExpression(this LockOnNewCardFeature feature)
        {
            return @"Ext.getDom(""Id"").value==0";
        }

        public static string ToExtendedInfo(this FilterToParentFeature feature)
        {
            return @"filterToParent=true";
        }

        public static string ToExtendedInfo(this FilterByParentsFeature feature)
        {
            return @"filterByParents=true";
        }
    }
}