using System;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllEntities
{
    public class EntitiesProcessingResultsKey : IContextKey<EntitiesProcessingResults>
    {
        private static readonly Lazy<EntitiesProcessingResultsKey> KeyInstance = new Lazy<EntitiesProcessingResultsKey>();

        public static EntitiesProcessingResultsKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}