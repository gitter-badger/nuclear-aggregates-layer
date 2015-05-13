using System;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindPublicServiceUseCases
{
    public class UseCaseProcessingResultsKey : IContextKey<UseCaseProcessingResults>
    {
        private static readonly Lazy<UseCaseProcessingResultsKey> KeyInstance = new Lazy<UseCaseProcessingResultsKey>();

        public static UseCaseProcessingResultsKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}