using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindAllEntities
{
    public class EntitiesProcessingResults
    {
        public IDictionary<string, EntityDescriptor> ProcessedEntities { get; set; }
        public IDictionary<string, EntityDescriptor> InvalidProcessedEntities { get; set; }

        public int FoundEntities { get; set; }
        public int InvalidEntities { get; set; }
    }
}