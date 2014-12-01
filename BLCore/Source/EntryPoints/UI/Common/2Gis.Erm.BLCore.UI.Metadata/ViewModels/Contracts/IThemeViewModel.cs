using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IThemeViewModel : IEntityViewModelAbstract<Theme>
    {
        string Name { get; set; }
        int OrganizationUnitCount { get; set; }
    }
}