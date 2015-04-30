using System;
using System.Linq;
using System.Reflection;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.Platform.API.Core.UseCases
{
    public static class UseCaseUtils
    {
        public static bool TryGetUseCaseDuration(this ICustomAttributeProvider processingAttributeHost, out UseCaseDuration useCaseDuration)
        {
            useCaseDuration = UseCaseDuration.None;

            var targetAttributeType = typeof(UseCaseAttribute);
            var useCaseAttributesArray = (UseCaseAttribute[])processingAttributeHost.GetCustomAttributes(targetAttributeType, false);
            if (!useCaseAttributesArray.Any())
            {
                return false;
            }

            if (useCaseAttributesArray.Length > 1)
            {
                throw new InvalidOperationException("Attribute " + targetAttributeType + " can't be specified more than once");
            }
            
            useCaseDuration = useCaseAttributesArray[0].Duration;
            return useCaseAttributesArray[0].Duration != UseCaseDuration.None;
        }

        public static void EnsureDurationConsidered(this IProcessingContext processingContext, UseCaseDuration consideringDuration)
        {
            var evaluatedDuration = processingContext.ContainsKey(UseCaseDurationKey.Instance)
                                        ? (UseCaseDuration)Math.Max((int)consideringDuration, (int)processingContext.GetValue(UseCaseDurationKey.Instance))
                                        : consideringDuration;

            processingContext.SetValue(UseCaseDurationKey.Instance, evaluatedDuration);
        }
    }
}
