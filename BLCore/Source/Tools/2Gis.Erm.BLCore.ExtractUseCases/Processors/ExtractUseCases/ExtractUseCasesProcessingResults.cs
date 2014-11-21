using DoubleGis.Erm.BLCore.ExtractUseCases.UseCases;

namespace DoubleGis.Erm.BLCore.ExtractUseCases.Processors.ExtractUseCases
{
    public class ExtractUseCasesProcessingResults
    {
        public UseCase[] ProcessedUseCases { get; set; }
        public string[] NotMappedHandlers { get; set; }
        public ProcessingErrors Errors { get; set; }
    }
}