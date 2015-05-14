using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationsMetadataBuilder<TViewModel> : MetadataElementBuilder<ViewModelCustomizationsMetadataBuilder<TViewModel>, ViewModelCustomizationsMetadata>
        where TViewModel : IEntityViewModelBase
    {
        private readonly Type _entityType;
        private int _currentOrder;

        public ViewModelCustomizationsMetadataBuilder(Type entityType)
        {
            _entityType = entityType;
        }

        public ViewModelCustomizationsMetadataBuilder()
        {
            throw new NotSupportedException();
        }

        public ViewModelCustomizationsMetadataBuilder<TViewModel> Use<TCustomization>() where TCustomization : IViewModelCustomization<TViewModel>
        {
            AddFeatures(new ViewModelCustomizationFeature<TCustomization, TViewModel>());
            return this;
        }

        public ViewModelCustomizationsMetadataBuilder<TViewModel> UseOrdered<TCustomization>() where TCustomization : IViewModelCustomization<TViewModel>
        {
            _currentOrder++;
            AddFeatures(new ViewModelCustomizationFeature<TCustomization, TViewModel>(_currentOrder));
            return this;
        }

        protected override ViewModelCustomizationsMetadata Create()
        {
            return new ViewModelCustomizationsMetadata(_entityType, Features);
        }
    }
}