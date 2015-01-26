using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public interface ICardDocumentViewModelFactory
    {
        TViewModel Create<TViewModel>(IUseCase useCase, IEntityType entityName, long entityId) where TViewModel : class, IViewModel;
        IViewModel Create(IUseCase useCase, IEntityType entityName, long entityId);
    }
}