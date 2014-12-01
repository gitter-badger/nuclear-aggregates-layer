using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ICountryViewModel : IEntityViewModelAbstract<Country>
    {
        string Name { get; set; }
    }
}