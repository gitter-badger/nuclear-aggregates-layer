using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPriceViewModel : IEntityViewModelAbstract<Price>
    {
        string Name { get; set; }
        bool IsPublished { get; set; }
    }
}