using System;

namespace DoubleGis.Erm.Platform.Common.Crosscutting
{
    public static class CrosscuttingIndicators
    {
        public static readonly Type Service = typeof(ICrosscuttingService);
        public static readonly Type InvariantSafeService = typeof(IInvariantSafeCrosscuttingService);
        public static readonly Type ServiceConsumer = typeof(ICrosscuttingServiceConsumer);

        public static bool IsCrosscuttingService(this Type checkingType)
        {
            return Service.IsAssignableFrom(checkingType);
        }

        public static bool IsCrosscuttingServiceInvariantSafe(this Type checkingType)
        {
            return InvariantSafeService.IsAssignableFrom(checkingType);
        }

        public static bool IsCrosscuttingServiceConsumer(this Type checkingType)
        {
            return ServiceConsumer.IsAssignableFrom(checkingType);
        }
    }
}
