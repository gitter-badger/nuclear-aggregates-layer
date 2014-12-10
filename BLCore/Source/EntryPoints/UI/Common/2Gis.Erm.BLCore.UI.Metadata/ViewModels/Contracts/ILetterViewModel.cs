using DoubleGis.Erm.Platform.Model.Entities.Activity;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface ILetterViewModel : IEntityViewModelAbstract<Letter>
    {
        string Title { get; set; }
    }
}
