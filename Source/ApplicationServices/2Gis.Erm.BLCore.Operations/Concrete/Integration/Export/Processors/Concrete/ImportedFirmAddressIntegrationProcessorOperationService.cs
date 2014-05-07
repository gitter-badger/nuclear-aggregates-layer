using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Processors.Concrete
{
    // TODO {all, 01.04.2014}: в данном типе фактически нет необходимости, все что нужно может делать базовый тип, все что нужно - сделать его не абстрактным  
    // Почему же данный тип не удален - пока он оставлен фактически в качестве метаданных, т.е. такие типы как данный фактически обеспечивают явное документирование требований, 
    // какие сущности, в какие потоки и т.п. - пока это важно, т.к. ещё нет метаданных в другом виде + в перспективе будет жесткий рефакторинг реакции на perfomedbusinessoperation (servicebus и т.п.)
    public sealed class ImportedFirmAddressIntegrationProcessorOperationService : IntegrationProcessorOperationService<FirmAddress, ImportedFirmAddress>
    {
        public ImportedFirmAddressIntegrationProcessorOperationService(
            IOperationsProcessingsStoreService<FirmAddress, ImportedFirmAddress> processingsStoreService,
            IOperationsExporter<FirmAddress, ImportedFirmAddress> operationsExporter)
            : base(processingsStoreService, operationsExporter)
        {
        }
    }
}