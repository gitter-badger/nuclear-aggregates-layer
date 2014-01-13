using System.Collections.Generic;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.FindEditableEntityControllers
{
    public class EntityControllerProcessingResults
    {
        public IDictionary<string, EntityControllerDescriptor> ProcessedControllers { get; set; }
        public IDictionary<string, EntityControllerDescriptor> InvalidProcessedControllers { get; set; }

        public int FoundControllers { get; set; }
        public int InvalidControllers { get; set; }
    }
}