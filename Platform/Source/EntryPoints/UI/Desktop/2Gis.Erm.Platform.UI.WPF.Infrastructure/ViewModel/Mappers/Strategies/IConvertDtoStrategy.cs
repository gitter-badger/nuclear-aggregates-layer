using NuClear.Model.Common.Entities.Aspects;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies
{
    /// <summary>
    /// Маркерный интерфейс
    /// </summary>
    public interface IConvertDtoStrategy
    {
    }

    public interface IConvertDtoStrategy<TEntityDto> : IConvertDtoStrategy
        where TEntityDto : class, IDomainEntityDto, new()
    {
    }
}