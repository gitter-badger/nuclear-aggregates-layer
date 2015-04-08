using System;

using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;

namespace DoubleGis.Erm.BLCore.UI.Web.Mvc.Services.Cards
{
    public interface IViewModelCustomizationFeature : IMetadataFeature
    {
        Type CustomizationType { get; }
        int Order { get; }
    }
}