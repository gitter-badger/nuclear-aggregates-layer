using System;

namespace DoubleGis.Erm.Platform.API.Core.UseCases.Context.Keys
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
