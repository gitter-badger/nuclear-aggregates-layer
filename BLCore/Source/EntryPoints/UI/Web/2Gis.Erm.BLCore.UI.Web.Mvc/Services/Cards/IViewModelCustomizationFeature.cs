using System;

using NuClear.Metamodeling.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationFeature : IMetadataFeature
    {
        Type CustomizationType { get; }
        int Order { get; }
    }
}