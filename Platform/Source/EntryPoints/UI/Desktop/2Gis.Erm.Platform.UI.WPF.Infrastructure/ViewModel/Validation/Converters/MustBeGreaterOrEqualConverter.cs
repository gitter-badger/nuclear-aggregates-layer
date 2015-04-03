using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Platform.UI.WPF.Infrastructure.CustomTypeProvider;

using FluentValidation.Validators;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters
{
    public sealed class MustBeGreaterOrEqualConverter : Feature2ValidatorConverter<MustBeGreaterOrEqualConverter, MustBeGreaterOrEqualPropertyFeature>
    {
        protected override IPropertyValidator CreateValidator()
        {
            return new PredicateValidator(CheckValue);
        }

        protected override ErrorDescription GetErrorDescription()
        {
            string propertyKey = Settings.TargetPropertyMetadata.Name;
            Type propertyResourceType = typeof(MetadataResources);
            var localizeFeature = Settings.TargetPropertyMetadata.Features<DisplayNameLocalizedFeature>().SingleOrDefault();
            if (localizeFeature != null)
            {
                propertyKey = localizeFeature.ResourceKey;
            }

            return new ErrorDescription(Settings.ResourceKey, Settings.ErrorMessageResourceManagerType, propertyKey, propertyResourceType, new object[0]);
        }

        private bool CheckValue(object instanceToValidate, object propertyToMatchValue, PropertyValidatorContext propertyValidatorContext)
        {
            if (instanceToValidate == null)
            {
                throw new ArgumentNullException("instanceToValidate");
            }

            if (propertyToMatchValue == null)
            {
                return false;
            }

            // полагаемся на тот факт, что имеем дело с динамической вью-моделью, и требуемые свойства реализуют IComparable
            var vm = (IDynamicPropertiesContainer)instanceToValidate;

            object targetPropertyValue = vm.GetPropertyValue(Settings.PropertyToMatchName) as IComparable;

            return ((IComparable)propertyToMatchValue).CompareTo(targetPropertyValue) >= 0;
        }
    }
}