using System;
using System.Reflection;

using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

using NuClear.Model.Common.Entities.Aspects;

using Omu.ValueInjecter;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.To.Concrete
{
    public sealed class GenericToDtoFromDynamicViewModelStrategy<TEntityDto> : IConvertToDtoStrategy<TEntityDto>
        where TEntityDto : class, IDomainEntityDto, new()
    {
        private readonly Type _dtoType = typeof(TEntityDto);

        public bool CanConvertToDto(TEntityDto targetDto, IUseCase useCase, IViewModel sourceViewModel)
        {
            return sourceViewModel is IDynamicPropertiesContainer;
        }

        public void ConvertToDto(TEntityDto targetDto, IUseCase useCase, IViewModel sourceviewModel)
        {
            var propertiesContainer = sourceviewModel as IDynamicPropertiesContainer;
            if (propertiesContainer == null)
            {
                return;
            }

            for (int index = 0; index < PropertyInfosStorage.GetProps(_dtoType).Count; index++)
            {
                var property = PropertyInfosStorage.GetProps(_dtoType)[index];
                PropertyInfo sourceViewModelPropertyInfo;
                if (!propertiesContainer.TryGetDynamicPropertyInfo(property.Name, out sourceViewModelPropertyInfo))
                {
                    continue;
                }

                if (!property.PropertyType.IsAssignableFrom(sourceViewModelPropertyInfo.PropertyType))
                {
                    continue;
                }

                property.SetValue(targetDto, propertiesContainer.GetDynamicPropertyValue(property.Name));
            }
        }
    }
}