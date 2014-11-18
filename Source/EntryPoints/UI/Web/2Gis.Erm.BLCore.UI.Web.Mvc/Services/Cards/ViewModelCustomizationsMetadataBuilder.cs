using System;

using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationsMetadataBuilder : MetadataElementBuilder<ViewModelCustomizationsMetadataBuilder, ViewModelCustomizationsMetadata>
    {
        private Type _entityType;
        private int _currentOrder;

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

        public ViewModelCustomizationsMetadataBuilder UseOrdered<TCustomization>() where TCustomization : IViewModelCustomization
        {
            _currentOrder++;
            AddFeatures(new ViewModelCustomizationFeature<TCustomization>(_currentOrder));
            return this;
        }

        protected override ViewModelCustomizationsMetadata Create()
        {
            return new ViewModelCustomizationsMetadata(_entityType, Features);
        }
    }
}