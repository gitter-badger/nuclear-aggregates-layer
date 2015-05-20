using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Grid
{
    public interface IGridViewModelFactory
    {
        IGridViewModel Create(IUseCase useCase, IEntityType entityName);
    }
}
