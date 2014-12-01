using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ITerritoryViewModel : IEntityViewModelAbstract<Territory>
    {
        string Name { get; set; }
    }
}