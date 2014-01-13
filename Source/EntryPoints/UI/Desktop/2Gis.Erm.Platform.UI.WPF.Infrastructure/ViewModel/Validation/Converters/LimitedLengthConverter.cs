using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Metadata.Entities;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

using FluentValidation.Validators;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters
{
    public sealed class LimitedLengthConverter : Feature2ValidatorConverter<LimitedLengthConverter, LimitedLengthPropertyFeature>
    {
        protected override IPropertyValidator CreateValidator()
        {
            return new LengthValidator(Settings.MinLength, Settings.MaxLength);
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

            var resourceAndArgs = SelectResourceIdAndArgumentsForMessage();

            return new ErrorDescription(resourceAndArgs.Item1, typeof(BLResources), propertyKey, propertyResourceType, resourceAndArgs.Item2);
        }

        private Tuple<string, object[]> SelectResourceIdAndArgumentsForMessage()
        {
            string resourceId;
            object[] args;


            if (Settings.MinLength < 0)
            {
                resourceId = BLResources.StringLengthLocalizedAttribute_InvalidMaxLength;
                args = new object[0];
            }

            else if (Settings.MinLength == 0)
            {
                resourceId = BLResources.StringLengthLocalizedAttribute_ValidationError;
                args = new object[] { Settings.MaxLength };
            }

            else
            {
                args = new object[] { Settings.MaxLength, Settings.MinLength };
                resourceId = Settings.MaxLength == Settings.MinLength
                                 ? StaticReflection.GetMemberName(() => BLResources.StringLengthLocalizedAttribute_ValidationErrorEqualsLimitations)
                                 : StaticReflection.GetMemberName(() => BLResources.StringLengthLocalizedAttribute_ValidationErrorIncludingMinimum);
            }

            return Tuple.Create(resourceId, args);
        }
    }
}