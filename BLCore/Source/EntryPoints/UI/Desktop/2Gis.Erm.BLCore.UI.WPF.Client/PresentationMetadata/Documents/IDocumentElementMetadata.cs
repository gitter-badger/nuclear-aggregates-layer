using NuClear.Metamodeling.Elements.Aspects.Conditions;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Documents
{
    public interface IDocumentElementMetadata
    {
        ICondition Condition { get; }
    }
}