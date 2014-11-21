using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public interface IGridViewModelFactory
    {
        IGridViewModel Create(IUseCase useCase, EntityName entityName);
    }
}
