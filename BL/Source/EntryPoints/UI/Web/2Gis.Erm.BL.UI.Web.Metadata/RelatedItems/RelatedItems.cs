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
            return UIElementMetadata.Config
                                    .Name.Static(EntityName.Activity.ToString().Pluralize())
                                    .Title.Resource(() => ErmConfigLocalization.CrdRelErmActions)
                                    .Icon.Path(Icons.Icons.Entity.Activity)
                                    .LockOnNew()
                                    .Handler.ShowGridByConvention(EntityName.Activity)
                                    .FilterToParents();
        }

        public static UIElementMetadataBuilder ChildrenGrid<TKey>(EntityName entity, Expression<Func<TKey>> resourceKeyExpression)
        {
            return UIElementMetadata.Config
                                    .Name.Static("Children")
                                    .Title.Resource(resourceKeyExpression)
                                    .LockOnNew()
                                    .Handler.ShowGridByConvention(entity)
                                    .FilterToParent();
        }

        public static UIElementMetadataBuilder EntityGrid<TKey>(EntityName entity, Expression<Func<TKey>> resourceKeyExpression)
        {
            return UIElementMetadata.Config
                                    .Name.Static(entity.ToString().Pluralize())
                                    .Title.Resource(resourceKeyExpression)
                                    .LockOnNew()
                                    .Handler.ShowGridByConvention(entity)
                                    .FilterToParent();
        }

        public static UIElementMetadataBuilder EntityGrid<TKey>(EntityName entity, string iconPath, Expression<Func<TKey>> resourceKeyExpression)
        {
            return UIElementMetadata.Config
                                    .Name.Static(entity.ToString().Pluralize())
                                    .Title.Resource(resourceKeyExpression)
                                    .Icon.Path(iconPath)
                                    .LockOnNew()
                                    .Handler.ShowGridByConvention(entity)
                                    .FilterToParent();
        }
    }
}
