using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.To
{
    public interface IConvertToDtoStrategy<TEntityDto> : IConvertDtoStrategy<TEntityDto>
        where TEntityDto : class, IDomainEntityDto, new()
    {
        bool CanConvertToDto(TEntityDto targetDto, IUseCase useCase, IViewModel sourceViewModel);
        void ConvertToDto(TEntityDto targetDto, IUseCase useCase, IViewModel sourceviewModel);
    }
}