using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IAssociatedPositionsGroupViewModel : IEntityViewModelAbstract<AssociatedPositionsGroup>
    {
        string Name { get; set; }
    }
}