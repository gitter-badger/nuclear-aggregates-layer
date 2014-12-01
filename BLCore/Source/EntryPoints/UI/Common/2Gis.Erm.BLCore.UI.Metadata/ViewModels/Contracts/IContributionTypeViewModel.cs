using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IContributionTypeViewModel : IEntityViewModelAbstract<ContributionType>
    {
        string Name { get; set; }
    }
}