using System;

using DoubleGis.Erm.Platform.API.Core.UseCases.Context;

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