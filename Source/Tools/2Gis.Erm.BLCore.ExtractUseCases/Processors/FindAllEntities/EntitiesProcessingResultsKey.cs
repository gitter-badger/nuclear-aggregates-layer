using System;

using DoubleGis.Erm.Platform.API.Core.UseCases.Context;

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