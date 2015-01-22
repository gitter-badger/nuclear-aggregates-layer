using System;
using System.Reflection;

using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.UI.Metadata.Indicators;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.Presentation.Controls.Lookup;
using DoubleGis.Erm.Platform.UI.WPF.Infrastructure.UseCases;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

using NuClear.Model.Common.Entities.Aspects;

using Omu.ValueInjecter;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Mappers.Strategies.To.Concrete
{
    public sealed class GenericLookupToDtoFromDynamicViewModelStrategy<TEntityDto> : IConvertToDtoStrategy<TEntityDto>
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

            var entityReferenceIndicator = typeof(EntityReference);
            var lookupIndicator = typeof(LookupViewModel);

            for (int index = 0; index < PropertyInfosStorage.GetProps(_dtoType).Count; index++)
            {
                var property = PropertyInfosStorage.GetProps(_dtoType)[index];
                if (!entityReferenceIndicator.IsAssignableFrom(property.PropertyType))
                {
                    continue;
                }

                PropertyInfo sourceViewModelPropertyInfo;
                if (!propertiesContainer.TryGetDynamicPropertyInfo(property.Name, out sourceViewModelPropertyInfo)
                    || !lookupIndicator.IsAssignableFrom(sourceViewModelPropertyInfo.PropertyType))
                {
                    continue;
                }

                var lookupViewModel = propertiesContainer.GetDynamicPropertyValue(property.Name) as LookupViewModel;
                if (lookupViewModel == null)
                {
                    continue;
                }

                property.SetValue(targetDto, LookupEntry.ToReference(lookupViewModel.SelectedItem));
            }
        }
    }
}