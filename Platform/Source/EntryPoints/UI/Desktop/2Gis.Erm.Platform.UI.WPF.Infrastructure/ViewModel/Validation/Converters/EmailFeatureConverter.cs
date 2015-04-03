using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.CommonFeatures;
using DoubleGis.Erm.Platform.Model.Metadata.Entities.PropertyFeatures;

using FluentValidation.Validators;

using NuClear.Metamodeling.Elements;
using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters
{
    public sealed class EmailFeatureConverter : Feature2ValidatorConverter<EmailFeatureConverter, EmailPropertyFeature>
    {
        protected override IPropertyValidator CreateValidator()
        {
            return new EmailValidator();
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
    }
}