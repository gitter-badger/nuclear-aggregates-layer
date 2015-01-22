using System;
using System.Reflection;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

using NuClear.Model.Common.Entities.Aspects;

using Omu.ValueInjecter;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.From.Concrete
{
    public sealed class GenericFromDtoToDynamicViewModelStrategy<TEntityDto> : IConvertFromDtoStrategy<TEntityDto>
        where TEntityDto : class, IDomainEntityDto, new()
    {
        private readonly Type _dtoType = typeof(TEntityDto);

        public bool CanConvertFromDto(TEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel)
        {
            return targetViewModel is IDynamicPropertiesContainer;
        }

        public void ConvertFromDto(TEntityDto sourceDto, IUseCase useCase, IViewModel targetViewModel)
        {
            var propertiesContainer = targetViewModel as IDynamicPropertiesContainer;
            if (propertiesContainer == null)
            {
                return;
            }

            for (int index = 0; index < PropertyInfosStorage.GetProps(_dtoType).Count; index++)
            {
                var property = PropertyInfosStorage.GetProps(_dtoType)[index];
                PropertyInfo targetViewModelPropertyInfo;
                if (!propertiesContainer.TryGetDynamicPropertyInfo(property.Name, out targetViewModelPropertyInfo))
                {
                    continue;
                }

                if (!targetViewModelPropertyInfo.PropertyType.IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }

                propertiesContainer.SetDynamicPropertyValue(property.Name, property.GetValue(sourceDto));
            }
        }
    }
}
