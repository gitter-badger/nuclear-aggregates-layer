using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IOperationTypeViewModel : IEntityViewModelAbstract<OperationType>
    {
        string Name { get; set; }
    }
}