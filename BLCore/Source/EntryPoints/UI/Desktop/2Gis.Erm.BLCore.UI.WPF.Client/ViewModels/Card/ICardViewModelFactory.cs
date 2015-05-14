using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using NuClear.Model.Common.Entities;

namespace DoubleGis.Erm.BLCore.UI.WPF.Client.ViewModels.Card
{
    public interface ICardViewModelFactory
    {
        TViewModel Create<TViewModel>(IUseCase useCase, IEntityType entityName, long entityId) where TViewModel : class, ICardViewModel;
        ICardViewModel Create(IUseCase useCase, IEntityType entityName, long entityId);
    }
}
