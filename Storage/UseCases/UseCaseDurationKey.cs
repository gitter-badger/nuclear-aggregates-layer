using System;

namespace NuClear.Storage.UseCases
{
    public sealed class UseCaseDurationKey : IContextKey<UseCaseDuration>
    {
        private static readonly Lazy<UseCaseDurationKey> KeyInstance = new Lazy<UseCaseDurationKey>();

        public static UseCaseDurationKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}
