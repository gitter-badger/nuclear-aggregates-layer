using System;
using System.Linq;

using DoubleGis.Erm.BL.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Enums;

using FluentValidation.Validators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters
{
    public sealed class CheckDateConverter : Feature2ValidatorConverter<CheckDateConverter, CheckDatePropertyFeature>
    {
        protected override IPropertyValidator CreateValidator()
        {
            return new PredicateValidator(CheckDate);
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

        private bool CheckDate(object instanceToValidate, object propertyValue, PropertyValidatorContext propertyValidatorContext)
        {
            if (propertyValue == null)
            {
                return false;
            }

            var date = ((DateTime)propertyValue).Date;
            return (Settings.CheckDayOfMonthType == CheckDayOfMonthType.FirstDay)
                       ? date == new DateTime(date.Year, date.Month, 1)
                       : date == new DateTime(date.Year, date.Month, 1).AddMonths(1).AddDays(-1);
        }
    }
}