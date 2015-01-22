using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.From
{
    public interface IConvertFromDtoStrategy<TEntityDto> : IConvertDtoStrategy<TEntityDto>
        where TEntityDto : class, IDomainEntityDto, new()
    {
        bool CanConvertFromDto(TEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel);
        void ConvertFromDto(TEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel);
    }
}