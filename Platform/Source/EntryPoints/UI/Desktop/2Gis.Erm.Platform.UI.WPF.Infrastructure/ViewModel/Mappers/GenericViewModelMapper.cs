using System.Linq;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.From;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.To;

using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers
{
    public sealed class GenericViewModelMapper<TEntityDto> : IViewModelMapper<TEntityDto> 
        where TEntityDto : class, IDomainEntityDto, new()
    {
        private readonly IConvertToDtoStrategy<TEntityDto>[] _convertToDtoStrategies;
        private readonly IConvertFromDtoStrategy<TEntityDto>[] _convertFromDtoStrategies;

        public GenericViewModelMapper(
// ReSharper disable ParameterTypeCanBeEnumerable.Local
            IConvertToDtoStrategy<TEntityDto>[] toDtoConvertStrategies,
            IConvertFromDtoStrategy<TEntityDto>[] fromDtoConvertStrategies
// ReSharper restore ParameterTypeCanBeEnumerable.Local
            )
        {
            _convertToDtoStrategies = toDtoConvertStrategies;
            _convertFromDtoStrategies = fromDtoConvertStrategies;
        }

        public TEntityDto ToDto(IUseCase useCase, IViewModel sourceViewModel)
        {
            var targetDto = new TEntityDto();

            // TODO ? возможно стоит передавать контекст, где отмечать обработанные свойства dto, 
            // тогда возможно было бы проверять, все ли свойства обработанны
            foreach (var strategy in _convertToDtoStrategies.Where(s => s.CanConvertToDto(targetDto, useCase, sourceViewModel)))
            {
                strategy.ConvertToDto(targetDto, useCase, sourceViewModel);
            }

            return targetDto;
        }

        public IViewModel FromDto(TEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel)
        {
            // TODO ? возможно стоит передавать контекст, где отмечать обработанные свойства dto, 
            // тогда возможно было бы проверять, все ли свойства обработанны
            foreach (var strategy in _convertFromDtoStrategies.Where(s => s.CanConvertFromDto(sourceDto, useCase, targetViewModel)))
            {
                strategy.ConvertFromDto(sourceDto, useCase, targetViewModel);
            }

            return targetViewModel;
        }
        
        IViewModel IViewModelMapper.FromDto(IDomainEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel)
        {
            return FromDto((TEntityDto)sourceDto, useCase, targetViewModel);
        }

        IDomainEntityDto IViewModelMapper.ToDto(IUseCase useCase, IViewModel sourceViewModel)
        {
            return ToDto(useCase, sourceViewModel);
        }
    }
}