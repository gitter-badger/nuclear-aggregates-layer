using System;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.ExtractUseCases
{
    public class ExtractUseCasesKey : IContextKey<ExtractUseCasesProcessingResults>
    {
        private static readonly Lazy<ExtractUseCasesKey> KeyInstance = new Lazy<ExtractUseCasesKey>();

        public static ExtractUseCasesKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}