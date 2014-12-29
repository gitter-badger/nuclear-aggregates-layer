using System;
using System.Linq.Expressions;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

using Humanizer;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.RelatedItems
{
    public static partial class RelatedItem
    {
        public static UIElementMetadataBuilder ContentTab(string icon)
        {
            return UIElementMetadata.Config
                                    .Name.Static("ContentTab")
                                    .Title.Resource(() => ErmConfigLocalization.CrdRelInformation)
                                    .Icon.Path(icon);
        }

        public static UIElementMetadataBuilder ContentTab()
        {
            return ContentTab(Icons.Icons.Entity.DefaultSmall);
        }

        public static UIElementMetadataBuilder ActivitiesGrid()
        {
            return EntityGrid(EntityName.Activity.ToString().Pluralize(), EntityName.Activity, () => ErmConfigLocalization.CrdRelErmActions)
                .Icon.Path(Icons.Icons.Entity.Small(EntityName.Activity))
                .FilterToParents();
        }

        public static UIElementMetadataBuilder ChildrenGrid<TKey>(EntityName entity, Expression<Func<TKey>> resourceKeyExpression)
        {
            return EntityGrid("Children", entity, resourceKeyExpression).FilterToParent();
        }

        public static UIElementMetadataBuilder EntityGrid<TKey>(EntityName entity, Expression<Func<TKey>> resourceKeyExpression)
        {
            return EntityGrid(entity.ToString().Pluralize(), entity, resourceKeyExpression).FilterToParent();
        }

        public static UIElementMetadataBuilder EntityGrid<TKey>(EntityName entity, string iconPath, Expression<Func<TKey>> resourceKeyExpression)
        {
            return EntityGrid(entity, resourceKeyExpression).Icon.Path(iconPath);
        }

        private static UIElementMetadataBuilder EntityGrid<TKey>(string name, EntityName entity, Expression<Func<TKey>> resourceKeyExpression)
        {
            return UIElementMetadata.Config
                                    .Name.Static(name)
                                    .Title.Resource(resourceKeyExpression)
                                    .LockOnNew()
                                    .Handler.ShowGridByConvention(entity);
        }
    }
}
