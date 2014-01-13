using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.PresentationMetadata.Cards
{
    public interface ICardStructuresProvider
    {
        bool TryGetDescriptor(EntityName entityName, out CardStructure descriptor);
        CardStructure[] Structures { get; }
    }
}
