using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public interface ICardDocumentViewModelFactory
    {
        TViewModel Create<TViewModel>(IUseCase useCase, EntityName entityName, long entityId) where TViewModel : class, IViewModel;
        IViewModel Create(IUseCase useCase, EntityName entityName, long entityId);
    }
}