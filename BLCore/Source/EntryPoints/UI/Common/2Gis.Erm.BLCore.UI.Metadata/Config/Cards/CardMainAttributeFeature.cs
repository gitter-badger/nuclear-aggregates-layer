using DoubleGis.Erm.Platform.Model.Metadata.Common.Elements.Aspects.Features;
using DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Cards
{
    public interface ICardMainAttributeFeature : ICardFeature
    {
        IPropertyDescriptor PropertyDescriptor { get; }
    }
}