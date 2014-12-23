using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features.Resources;
using DoubleGis.Erm.Platform.UI.Metadata.UIElements;

namespace DoubleGis.Erm.BL.UI.Web.Metadata.Cards.Extensions
{
    public static class UIElementMetadataBuilderExtensions
    {
        public static UIElementMetadataBuilder ContentTab(this UIElementMetadataBuilder builder, string icon)
        {
            return builder.Name.Static("ContentTab")
                          .Title.Resource(() => ErmConfigLocalization.CrdRelInformation)
                          .Icon.Path(icon);
        }

        public static UIElementMetadataBuilder ContentTab(this UIElementMetadataBuilder builder)
        {
            return builder.ContentTab("en_ico_16_Default.gif");
        }

        public static UIElementMetadataBuilder AppendapleEntity<TEntity>(this UIElementMetadataBuilder builder)
            where TEntity : IEntity
        {
            return builder.WithFeatures(new AppendableEntityFeature(typeof(TEntity).AsEntityName()));
        }

        public static UIElementMetadataBuilder FilterToParent(this UIElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterToParentFeature());
        }

        public static UIElementMetadataBuilder FilterToParents(this UIElementMetadataBuilder builder)
        {
            return builder.WithFeatures(new FilterByParentsFeature());
        }

        public static UIElementMetadataBuilder ExtendedInfo(this UIElementMetadataBuilder builder, IResourceDescriptor extendedInfo)
        {
            return builder.WithFeatures(new ExtendedInfoFeature(extendedInfo));
        }
    }
}
