using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers
{
    public interface IViewModelMapper
    {
        IDomainEntityDto ToDto(IUseCase useCase, IViewModel sourceViewModel);
        IViewModel FromDto(IDomainEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel);
    }

    public interface IViewModelMapper<TEntityDto> : IViewModelMapper
        where TEntityDto : class, IDomainEntityDto, new()
    {
        new TEntityDto ToDto(IUseCase useCase, IViewModel sourceViewModel);
        IViewModel FromDto(TEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel);
    }
}
