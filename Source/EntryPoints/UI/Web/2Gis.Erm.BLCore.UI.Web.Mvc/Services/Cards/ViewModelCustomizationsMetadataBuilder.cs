using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationsMetadataBuilder : MetadataElementBuilder<ViewModelCustomizationsMetadataBuilder, ViewModelCustomizationsMetada>
    {
        private Type _entityType;

        public ViewModelCustomizationsMetadataBuilder For<TEntity>() where TEntity : IEntity
        {
            _entityType = typeof(TEntity);
            return this;
        }

        public ViewModelCustomizationsMetadataBuilder Use<TCustomization>() where TCustomization : IViewModelCustomization
        {
            AddFeatures(new ViewModelCustomizationFeature<TCustomization>());
            return this;
        }

        public ViewModelCustomizationsMetadataBuilder UseWithOrder<TCustomization>(int order) where TCustomization : IViewModelCustomization
        {
            AddFeatures(new ViewModelCustomizationFeature<TCustomization>(order));
            return this;
        }

        protected override ViewModelCustomizationsMetada Create()
        {
            return new ViewModelCustomizationsMetada(_entityType, Features);
        }
    }
}