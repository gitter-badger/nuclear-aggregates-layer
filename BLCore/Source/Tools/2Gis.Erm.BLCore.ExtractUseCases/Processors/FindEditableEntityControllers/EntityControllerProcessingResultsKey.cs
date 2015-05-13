using System;

using NuClear.Storage.UseCases;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindEditableEntityControllers
{
    public class EntityControllerProcessingResultsKey : IContextKey<EntityControllerProcessingResults>
    {
        private static readonly Lazy<EntityControllerProcessingResultsKey> KeyInstance = new Lazy<EntityControllerProcessingResultsKey>();

        public static EntityControllerProcessingResultsKey Instance
        {
            get
            {
                return KeyInstance.Value;
            }
        }
    }
}