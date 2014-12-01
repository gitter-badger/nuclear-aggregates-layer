using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.UI.Metadata.ViewModels.Contracts
{
    public interface IPrintFormTemplateViewModel : IEntityViewModelAbstract<PrintFormTemplate>
    {
        string FileName { get; set; }
    }
}
