using System;
using System.Collections.Generic;
using System.Globalization;

using Microsoft.Practices.Unity;

using NuClear.Metamodeling.Validators;
using NuClear.ResourceUtilities;
using NuClear.ResourceUtilities.Legacy;

namespace DoubleGis.Erm.BLCore.DI.Config
{
    public static partial class CommonBootstrapper
    {
        // FIXME {all, 27.11.2013}: избавится от такой реализации проверки - после рефакторинга massprocessors, заменить на massprocessor
        public static void EnsureResourceEntriesUniqueness(this IEnumerable<Type> resourseManagerHostTypes, params CultureInfo[] targetCultures)
        {
            var availableResources = resourseManagerHostTypes.EvaluateAvailableResources(true, targetCultures);

            string report;
            if (availableResources.TryGetDuplicatedResourceEntry(out report))
            {
                throw new InvalidOperationException("Resources usage conventions violated. Duplicated resource entries detected. " + report);
            }
        }

        public static IUnityContainer EnsureMetadataCorrectness(this IUnityContainer container)
        {
            var suite = container.Resolve<IMetadataValidatorsSuite>();
            suite.Validate();

            return container;
        }
    }
}
