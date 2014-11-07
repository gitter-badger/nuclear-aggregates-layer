using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationFeature<TCustomization> : IViewModelCustomizationFeature, IUniqueMetadataFeature where TCustomization : IViewModelCustomization
    {
        public ViewModelCustomizationFeature() : this(0)
        {
        }

        public ViewModelCustomizationFeature(int order)
        {
            CustomizationType = typeof(TCustomization);
            Order = order;
        }

        public Type CustomizationType { get; private set; }

        public int Order { get; private set; }
    }
}
