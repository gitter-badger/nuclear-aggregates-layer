using System;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

using FluentValidation.Validators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters
{
    public sealed class DigitsOnlyConverter : Feature2ValidatorConverter<DigitsOnlyConverter, DigitsOnlyPropertyFeature>
    {
        protected override IPropertyValidator CreateValidator()
        {
            return new PredicateValidator(CheckValue);
        }

        protected override ErrorDescription GetErrorDescription()
        {
            string propertyKey = Settings.TargetProperty.Name;
            Type propertyResourceType = typeof(MetadataResources);
            var localizeFeature = Settings.TargetProperty.Features.OfType<DisplayNameLocalizedFeature>().SingleOrDefault();
            if (localizeFeature != null)
            {
                propertyKey = localizeFeature.ResourceKey;
            }

            return new ErrorDescription(Settings.ResourceKey, Settings.ErrorMessageResourceManagerType, propertyKey, propertyResourceType, new object[0]);
        }

        private bool CheckValue(object instanceToValidate, object propertyValue, PropertyValidatorContext propertyValidatorContext)
        {
            if (propertyValue == null)
            {
                return true;
            }

            var propertyString = Convert.ToString(propertyValue);
            return propertyString.All(Char.IsDigit);
        }
    }
}