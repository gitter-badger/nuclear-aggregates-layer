using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationFeature<TCustomization> : IViewModelCustomizationFeature, IUniqueMetadataFeature where TCustomization : IViewModelCustomization
    {
        private readonly Type _customizationType;

        public ViewModelCustomizationFeature()
        {
            _customizationType = typeof(TCustomization);
        }

        public Type CustomizationType
        {
            get { return _customizationType; }
        }
    }
}
