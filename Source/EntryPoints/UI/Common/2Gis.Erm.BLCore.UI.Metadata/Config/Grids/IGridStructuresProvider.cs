using DoubleGis.Erm.Platform.Model.Entities;

namespace DoubleGis.Erm.BLCore.UI.Metadata.Config.Grids
{
    public interface IGridStructuresProvider
    {
        GridStructure[] Structures { get; }
        bool TryGetDescriptor(EntityName entityName, out GridStructure descriptor);
    }
}