using System;
using System.IO;

using DoubleGis.Erm.BLCore.Aggregates.Orders.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Generic.File;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.Common.Logging;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    // FIXME {all, 21.08.2013}: длительность usecase прописана, но такие метаданные пока не обрабатываются никем (раньше их использовала requesthandlerfactory)
    [UseCase(Duration = UseCaseDuration.Long)] 
    public sealed class FileStorageEnsureOrderExportedStrategy : IEnsureOrderExportedStrategy
    {
        private const int TypicalOrdersStreamSizeBytes = 10000000;
        private const int ChunkSize = 20;

        private readonly IOrderReadModel _orderReadModel;
        private readonly IFileService _fileService;
        private readonly IOrdersWithAdvertisementMaterialsSerializerFactory _ordersWithAdvertisementMaterialsSerializerFactory;
        private readonly IPublishOrdersForReleaseToFileStorage _ordersForReleaseToFileStoragePublisher;
        private readonly ICommonLog _logger;

        public FileStorageEnsureOrderExportedStrategy(IOrderReadModel orderReadModel,
                                                      IFileService fileService,
                                                      IOrdersWithAdvertisementMaterialsSerializerFactory ordersWithAdvertisementMaterialsSerializerFactory,
                                                      IPublishOrdersForReleaseToFileStorage ordersForReleaseToFileStoragePublisher,
                                                      ICommonLog logger)
        {
            _orderReadModel = orderReadModel;
            _fileService = fileService;
            _ordersWithAdvertisementMaterialsSerializerFactory = ordersWithAdvertisementMaterialsSerializerFactory;
            _ordersForReleaseToFileStoragePublisher = ordersForReleaseToFileStoragePublisher;
            _logger = logger;
        }

        public bool IsExported(long releaseId, long organizationUnitId, int organizationUnitDgppId, TimePeriod period, bool isBeta)
        {
            _logger.InfoFormatEx("Starting ensure process that all orders are exported to file storage. " +
                                 "Release detail: id {0}, organization unit id {1}, period {2}, {3} release",
                                 releaseId,
                                 organizationUnitId,
                                 period,
                                 isBeta ? "beta" : "final");

            if (isBeta)
            {
                _logger.InfoFormatEx("Orders export to file storage will be skipped because of beta release mode");
                return true;
            }
            
            var ordersStream = new MemoryStream(TypicalOrdersStreamSizeBytes);
            var orderSerializer = _ordersWithAdvertisementMaterialsSerializerFactory.Create(ordersStream, FileContentAccessor, organizationUnitDgppId);

            var totalProcessedCount = 0;
            try
            {
                int currentProcessedCount;

                do
                {
                    var ordersChunk = _orderReadModel.GetOrderInfosForRelease(organizationUnitId, period, totalProcessedCount, ChunkSize);
                    currentProcessedCount = orderSerializer.Serialize(ordersChunk);

                    totalProcessedCount += currentProcessedCount;
                }
                while (currentProcessedCount > 0);

                orderSerializer.Complete();

                ordersStream.Position = 0;
                _ordersForReleaseToFileStoragePublisher.Publish(organizationUnitId, organizationUnitDgppId, period, ordersStream);
            }
            catch (Exception ex)
            {
                _logger.ErrorFormatEx(ex,
                                      "Can't finish ensure process that all orders are exported already to file storage. " +
                                      "Release detail: id {0}, organization unit id {1}, period {2}, final release",
                                      releaseId,
                                      organizationUnitId,
                                      period);
                return false;
            }
            finally
            {
                // в нижеследующем порядке Dispose нельзя менять порядок вызова
                // serializer использует в своей реализации XmlWriter, а XmlWriter в момент своего dispose видимо пытается слить оставшиеся изменения в stream и т.п.,
                // при этом если stream к этому моменту будет уже disposed - то получим ObjectDisposedException (в .NET 4.0. и новее, в 3.5 exception пожирался)
                // Т.о. хоть и не хорошо опираться на знание деталей реализации, стоящей за абстракцией, но stream нужно dispos, строго после serializer
                orderSerializer.Dispose();
                ordersStream.Dispose();
            }

            _logger.InfoFormatEx("Finished ensure process. Ensured that all orders are exported already to file storage. " +
                                 "Release detail: id {0}, organization unit id {1}, period {2}, final release",
                                 releaseId,
                                 organizationUnitId,
                                 period);

            return true;
        }

        private void FileContentAccessor(long fileId, Stream stream)
        {
            var content = _fileService.GetFileContent(fileId);
            content.CopyTo(stream);
        }
    }
}