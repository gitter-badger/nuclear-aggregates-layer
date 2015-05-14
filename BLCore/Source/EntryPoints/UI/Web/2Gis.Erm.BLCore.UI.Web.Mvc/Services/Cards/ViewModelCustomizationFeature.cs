using System;

using DoubleGis.Erm.BLCore.UI.Web.Mvc.ViewModels;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public sealed class ViewModelCustomizationFeature<TCustomization, TViewModel> : IViewModelCustomizationFeature, IUniqueMetadataFeature 
        where TViewModel: IEntityViewModelBase
        where TCustomization : IViewModelCustomization<TViewModel>
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
