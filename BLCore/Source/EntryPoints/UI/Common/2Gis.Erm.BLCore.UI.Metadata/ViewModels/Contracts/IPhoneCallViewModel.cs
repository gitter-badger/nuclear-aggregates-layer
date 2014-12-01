using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPhonecallViewModel : IEntityViewModelAbstract<Phonecall>
    {
        string Header { get; set; }
    }
}
