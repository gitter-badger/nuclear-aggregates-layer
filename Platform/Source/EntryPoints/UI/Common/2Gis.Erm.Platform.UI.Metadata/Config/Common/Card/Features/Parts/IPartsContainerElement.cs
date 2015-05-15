using NuClear.Metamodeling.Elements;

namespace DoubleGis.Erm.Platform.UI.Metadata.Config.Common.Card.Features.Parts
{
    // TODO {all, 20.11.2014}: Перекликается с реализацией для WPF. При возобновлении работы над WPF. Код нужно будет как-то объединить.
    public interface IPartsContainerElement : IMetadataElementAspect
    {
        bool HasParts { get; }
        IPartFeature[] Parts { get; }
    }
}
