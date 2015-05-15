using System;
using System.Collections.Generic;
using System.Linq;

using FluentValidation.Validators;

using NuClear.Metamodeling.Domain.Entities;

namespace DoubleGis.Erm.Platform.UI.WPF.Infrastructure.ViewModel.Validation.Converters
{
    public static class ValidatorFeatureToValidatorsConverter
    {
        private static readonly ValidatorFactory[] Converters =
            {
                CheckDateConverter.TryConvert,
                DigitsOnlyConverter.TryConvert,
                EmailFeatureConverter.TryConvert,
                LimitedLengthConverter.TryConvert,
                MustBeGreaterOrEqualConverter.TryConvert,
                NonZeroConverter.TryConvert,
                RangePropertyConverter.TryConvert,
                RegularExpressionConverter.TryConvert,
                RequiredConverter.TryConvert
            };
        
        private delegate bool ValidatorFactory(IValidatablePropertyFeature validatablePropertyFeature, out IPropertyValidator validator);

        public static IEnumerable<IPropertyValidator> Convert(IEnumerable<IValidatablePropertyFeature> validatablePropertyFeatures)
        {
            foreach (var feature in validatablePropertyFeatures)
            {
                IPropertyValidator validator = null;
                if (!Converters.Any(c => c(feature, out validator)))
                {
                    throw new InvalidOperationException("Can't convert property validation feature to validator. Feature type: " + feature.GetType().Name);
                }

                yield return validator;
            }
        }
    }
}
