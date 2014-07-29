using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public interface ICardViewModelFactory
    {
        TViewModel Create<TViewModel>(IUseCase useCase, EntityName entityName, long entityId) where TViewModel : class, ICardViewModel;
        ICardViewModel Create(IUseCase useCase, EntityName entityName, long entityId);
    }
}
