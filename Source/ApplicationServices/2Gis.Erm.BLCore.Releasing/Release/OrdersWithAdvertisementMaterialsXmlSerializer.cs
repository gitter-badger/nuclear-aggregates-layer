using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;

using DoubleGis.Erm.BLCore.API.Aggregates.Orders.DTO.ForRelease;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

namespace DoubleGis.Erm.BLCore.Releasing.Release
{
    // TODO {all, 21.08.2013}: Сериализует в xml информацию по заказам, пока statefull реализация, чтобы иземения в использование xmlwriter были минимальными => снизить риски некорркетной схемы и т.п.
    // Однако, можно было бы передавать stream в который нужно сериализовать очередную порцию информации по заказам, в каждый вызов - препятствие пока - это использование XmlWriter, варианты:
    //  - переход к WriteRaw, вместо более верхнеуровневого Api xmlwriter
    //  - не использовать XmlWriter
    public sealed class OrdersWithAdvertisementMaterialsXmlSerializer : IOrdersWithAdvertisementMaterialsSerializer
    {
        private readonly Stream _stream;
        private readonly Action<long, Stream> _fileContentAccessor;
        private readonly int _organizationUnitDgppId;
        private readonly ICommonLog _logger;
        private readonly XmlWriter _xmlWriter;
        private bool _isBodyNotEmpty;

        public OrdersWithAdvertisementMaterialsXmlSerializer(
            Stream stream,
            Action<long, Stream> fileContentAccessor,
            int organizationUnitDgppId, 
            ICommonLog logger)
        {
            _stream = stream;
            _fileContentAccessor = fileContentAccessor;
            _organizationUnitDgppId = organizationUnitDgppId;
            _logger = logger;
            _xmlWriter = XmlWriter.Create(_stream, new XmlWriterSettings { Indent = true });

            WriteHeader();
        }

        public int Serialize(IEnumerable<OrderInfo> orderInfos)
        {
            if (!_isBodyNotEmpty)
            {
                WriteBodyStart();

                _isBodyNotEmpty = true;
            }

            int processedItemsCount = 0;

            foreach (var info in orderInfos)
            {
                try
                {
                    _logger.DebugFormatEx("Start serialization to xml info for order with id {0} and number {1}", info.Id, info.Number);
                    WriteBodyElement(info);
                    _logger.DebugFormatEx("Serialization to xml finished for order with id {0} and number {1}", info.Id, info.Number);
                }
                catch (Exception ex)
                {
                    _logger.ErrorFormatEx(ex, "Can't serialize to xml info for order with id {0} and number {1}", info.Id, info.Number);
                    throw;
                }
                ++processedItemsCount;
            }

            _xmlWriter.Flush();

            return processedItemsCount;
        }

        public void Complete()
        {
            if (_isBodyNotEmpty)
            {
                WriteBodyEnd();
            }

            WriteFooter();

            _xmlWriter.Flush();
        }

        private void WriteHeader()
        {
            _xmlWriter.WriteStartDocument();

            // exchange
            _xmlWriter.WriteStartElement("exchange");

            // header
            _xmlWriter.WriteStartElement("header");

            // mguid
            _xmlWriter.WriteStartElement("mguid");
            _xmlWriter.WriteValue(XmlConvert.ToString(Guid.NewGuid()));
            _xmlWriter.WriteEndElement();

            // createDate
            _xmlWriter.WriteStartElement("createDate");
            _xmlWriter.WriteValue(DateTime.UtcNow);
            _xmlWriter.WriteEndElement();

            // exportDate
            _xmlWriter.WriteStartElement("exportDate");
            _xmlWriter.WriteValue(DateTime.UtcNow.ToString("u"));
            _xmlWriter.WriteEndElement();

            // organizationUnitId
            _xmlWriter.WriteStartElement("organizationUnitId");
            _xmlWriter.WriteValue(_organizationUnitDgppId);
            _xmlWriter.WriteEndElement();

            // /header
            _xmlWriter.WriteEndElement();

            // flush written content
            _xmlWriter.Flush();
        }

        private void WriteBodyStart()
        {
            // <orders>
            _xmlWriter.WriteStartElement("orders");
            
            // flush written content
            _xmlWriter.Flush();
        }

        private void WriteBodyEnd()
        {
            // </orders>
            _xmlWriter.WriteEndElement();

            // flush written content
            _xmlWriter.Flush();
        }

        private void WriteFooter()
        {
            // exchange
            _xmlWriter.WriteEndElement();

            // root
            _xmlWriter.WriteEndDocument();

            // flush written content
            _xmlWriter.Flush();
        }

        private void WriteBodyElement(OrderInfo orderInfo)
        {
            var allPositions = orderInfo.Positions.Union(orderInfo.CompositePositions);

            // проверки выполняются до начала записи в xmlWriter
            if (null == orderInfo.StableFirmId)
            {
                throw new NotificationException(string.Format(BLResources.StableFirmIdNotSpecifiedForFirm, orderInfo.Id, orderInfo.Number));
            }

            if (allPositions.Any(p => !p.PlatformId.HasValue))
            {
                throw new NotificationException(string.Format(BLResources.OrderContainsPositionWithoutDgppId, orderInfo.Id, orderInfo.Number));
            }

            if (allPositions.Any(p => !p.ProductCategory.HasValue))
            {
                throw new NotificationException(string.Format(BLResources.OrderContainsPositionWithoutExportCodeForCategory, orderInfo.Id, orderInfo.Number));
            }

            if (allPositions.Any(p => !p.ProductType.HasValue))
            {
                throw new NotificationException(string.Format(BLResources.OrderContainsPositionWithoutExportCodeForType, orderInfo.Id, orderInfo.Number));
            }

            // <order>
            _xmlWriter.WriteStartElement("order");

            // id=""
            _xmlWriter.WriteAttributeString("id", orderInfo.Id.ToString());

            // number=""
            _xmlWriter.WriteAttributeString("number", orderInfo.Number);

            // status=""
            _xmlWriter.WriteAttributeString("status", ((OrderState)orderInfo.Status).ToString().ToLower());

            // createdOn=""
            _xmlWriter.WriteAttributeString("createdOn", orderInfo.CreatedOn.ToString("u"));

            // beginDistributionDate=""
            _xmlWriter.WriteAttributeString("beginDistributionDate", orderInfo.BeginDistributionDate.ToString("u"));

            // endDistributionDate=""
            _xmlWriter.WriteAttributeString("endDistributionDate", orderInfo.EndDistributionDate.ToString("u"));

            // stableFirmId=""
            _xmlWriter.WriteAttributeString("stableFirmId", orderInfo.StableFirmId.ToString());

            // curatorId=""
            _xmlWriter.WriteAttributeString("curatorId", orderInfo.CuratorId.ToString());

            // approverId=""
            _xmlWriter.WriteAttributeString("approverId", (orderInfo.ApproverId ?? 0).ToString());

            // <positions>
            _xmlWriter.WriteStartElement("positions");
            foreach (var position in allPositions)
            {
                // FIXME {all, 21.08.2013}: Подробнее см. описание к реализации детектора.
                if (!OldFormatAdvertisementMaterialDetector.IsValid(position))
                {
                    var msg = string.Format("Order position with id {0} has advertisiment in OLD format, that is currently unsupported. Before release 1.0 functional for supporting old format advertisement converting was removed.", position.Id);
                    _logger.FatalEx(msg);
                    throw new InvalidOperationException(msg);
                }

                // <position>
                _xmlWriter.WriteStartElement("position");

                // id=""
                _xmlWriter.WriteAttributeString("id", position.Id.ToString());

                // platformId=""
                _xmlWriter.WriteAttributeString("platformType", position.PlatformId.ToString());

                // productType=""
                _xmlWriter.WriteAttributeString("productType", position.ProductType.ToString());

                // productCategory=""
                _xmlWriter.WriteAttributeString("productCategory", position.ProductCategory.ToString());

                // <advMaterials>
                _xmlWriter.WriteStartElement("advMaterials");
                foreach (var advMaterial in position.AdvertisingMaterials)
                {
                    // <advMaterial>
                    _xmlWriter.WriteStartElement("advMaterial");
                    _xmlWriter.WriteAttributeString("id", advMaterial.Id.HasValue ? advMaterial.Id.ToString() : position.Id.ToString());
                    if (advMaterial.IsSelectedToWhiteList.HasValue)
                    {
                        _xmlWriter.WriteAttributeString("isWhitedListed",
                                                       advMaterial.IsSelectedToWhiteList.Value.ToString(System.Globalization.CultureInfo.InvariantCulture).ToLower());
                    }

                    if (advMaterial.StableAddrIds.Any() ||
                        advMaterial.StableRubrIds.Any() ||
                        advMaterial.RubrInAddrIds.Any() ||
                        advMaterial.ThemeIds.Any() ||
                        advMaterial.Elements.Any())
                    {
                        // <stableAddrIds>
                        _xmlWriter.WriteStartElement("stableAddrIds");
                        foreach (var stableAddrId in advMaterial.StableAddrIds)
                        {
                            // <stableAddrId>
                            _xmlWriter.WriteStartElement("stableAddrId");
                            _xmlWriter.WriteValue(stableAddrId);
                            _xmlWriter.WriteEndElement();
                        }

                        // </stableAddrIds>
                        _xmlWriter.WriteEndElement();

                        // <stableRubIds>
                        _xmlWriter.WriteStartElement("stableRubIds");
                        foreach (var stableRubrId in advMaterial.StableRubrIds)
                        {
                            // <stableRubId>
                            _xmlWriter.WriteStartElement("stableRubId");
                            _xmlWriter.WriteValue(stableRubrId);
                            _xmlWriter.WriteEndElement();
                        }

                        // </stableRubIds>
                        _xmlWriter.WriteEndElement();

                        // <rubInAddrIds>
                        _xmlWriter.WriteStartElement("stableRubInAddrIds");
                        foreach (var rubInAddrIds in advMaterial.RubrInAddrIds)
                        {
                            // <rubInAddrId>
                            _xmlWriter.WriteStartElement("stableRubInAddrId");

                            // <stableAddrId>
                            _xmlWriter.WriteStartElement("stableAddrId");
                            _xmlWriter.WriteValue(rubInAddrIds.FirmAddressId);
                            _xmlWriter.WriteEndElement();

                            // <stableRubId>
                            _xmlWriter.WriteStartElement("stableRubId");
                            _xmlWriter.WriteValue(rubInAddrIds.CategoryId);
                            _xmlWriter.WriteEndElement();

                            // </rubInAddrId>
                            _xmlWriter.WriteEndElement();
                        }

                        // </rubInAddrIds>
                        _xmlWriter.WriteEndElement();

                        // <themeIds>
                        _xmlWriter.WriteStartElement("themeIds");
                        foreach (var themeId in advMaterial.ThemeIds)
                        {
                            // <themeId>
                            _xmlWriter.WriteStartElement("themeId");
                            _xmlWriter.WriteValue(themeId);
                            _xmlWriter.WriteEndElement();
                        }

                        // </themeIds>
                        _xmlWriter.WriteEndElement();

                        // <advElements>
                        _xmlWriter.WriteStartElement("advElements");
                        foreach (var element in advMaterial.Elements)
                        {
                            string type = null;
                            if (!string.IsNullOrEmpty(element.Text))
                            {
                                type = "text";
                            }
                            else if (element.FileId.HasValue)
                            {
                                type = "blob";
                            }
                            else if (element.BeginDate.HasValue)
                            {
                                type = "timeperiod";
                            }

                            // <content>
                            _xmlWriter.WriteStartElement("content");
                            _xmlWriter.WriteAttributeString("id",
                                                           element.Id.ToString());
                            _xmlWriter.WriteAttributeString("exportCode",
                                                           element.ExportCode.ToString());
                            if (type != null)
                            {
                                _xmlWriter.WriteAttributeString("type", type);
                            }

                            if (!string.IsNullOrEmpty(element.Text))
                            {
                                _xmlWriter.WriteValue(element.Text);
                            }
                            else if (element.FileId.HasValue)
                            {
                                var memoryStream = new MemoryStream();
                                _fileContentAccessor(element.FileId.Value, memoryStream);

                                _xmlWriter.WriteValue(Convert.ToBase64String(memoryStream.ToArray()));
                            }
                            else if (element.BeginDate.HasValue)
                            {
                                _xmlWriter.WriteValue(string.Format("{0};{1}", element.BeginDate.Value.ToString("u"), element.EndDate.HasValue ? element.EndDate.Value.ToString("u") : string.Empty));
                            }

                            // </content>
                            _xmlWriter.WriteEndElement();
                        }

                        // </advElements>
                        _xmlWriter.WriteEndElement();
                    }

                    // </advMaterial>
                    _xmlWriter.WriteEndElement();
                }

                // </advMaterials>
                _xmlWriter.WriteEndElement();

                // </position>
                _xmlWriter.WriteEndElement();
            }

            // </positions>
            _xmlWriter.WriteEndElement();

            // </order>
            _xmlWriter.WriteEndElement();
        }

        #region Поддержка IDisposable

        private readonly object _disposeSync = new object();

        /// <summary>
        /// Флаг того что instance disposed
        /// </summary>
        private bool _isDisposed;

        /// <summary>
        /// Флаг того что instance disposed - потокобезопасный
        /// </summary>
        private bool IsDisposed
        {
            get
            {
                lock (_disposeSync)
                {
                    return _isDisposed;
                }
            }
        }

        /// <summary>
        /// Dispose
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
        }

        /// <summary>
        /// Внутренний dispose класса
        /// </summary>
        private void Dispose(bool disposing)
        {
            lock (_disposeSync)
            {
                if (_isDisposed)
                {
                    return;
                }

                if (disposing)
                {
                    _xmlWriter.Dispose();
                }

                // Free your own state (unmanaged objects).
                // Set large fields to null.
                _isDisposed = true;
            }
        }

        #endregion

        // FIXME {all, 21.08.2013}: это УГ оставлено для выявления у позиции заказа рекламных материалов старого формата (мигрировавших из ДГПП и т.д.). Через некоторое время после миграции Норильска (например 2-3 сборки) нужно удалить.
        // до подготовки версии 1.0 при сериализации данных по позиции заказа в XML применялся спец. конвертер, который для некотоорых позиций "конвертировал" данные по рекламным материалам из старого формата в новый
        // Т.к. все города были переведены с ДГПП на ERM, и постепенно заказы с позициями у которых реклама была в "старом формате" закончились, было решено удалить конвертер, оставив только детектор на случай внезапного появления таких заказов с позициями в "старом формате"
        // Для проверки отсутсвия действующих заказов с рекламой в старом формате использовался ниже приведенный SQL запрос (последние такие заказы были в январе и феврале 2013 года).
        // Также ниже есть удаленная реализация конвертера позиций заказа в новый формат.
        private static class OldFormatAdvertisementMaterialDetector
        {
            #region Удаленная реализация конвертера рекламных метериалов из старого формата в новый
            //public class AdvertisingMaterialConverter : IAdvertisingMaterialConverter
            //{
            //    private const int PriorityExportCode = 1;
            //    private const int BannerExportCode = 8;
            //    private readonly IFinder _finder;

            //    public AdvertisingMaterialConverter(IFinder finder)
            //    {
            //        _finder = finder;
            //    }

            //    public void Convert(OrderPositionInfo orderPositionInfo)
            //    {
            //        if (orderPositionInfo.ProductType != PriorityExportCode && orderPositionInfo.ProductType != BannerExportCode)
            //        {
            //            return;
            //        }

            //        foreach (var advertisingMaterialInfo in orderPositionInfo.AdvertisingMaterials)
            //        {
            //            if (advertisingMaterialInfo.StableRubrIds.Any())
            //            {
            //                continue;
            //            }

            //            var categoryDgppIds =
            //                (from orderPosition in _finder.Find(Specs.Find.ById<OrderPosition>(orderPositionInfo.Id))
            //                 from firmAddress in orderPosition.Order.Firm.FirmAddresses
            //                 from categoryFirmAddress in firmAddress.CategoryFirmAddresses
            //                 where categoryFirmAddress.IsActive && !categoryFirmAddress.IsDeleted &&
            //                       firmAddress.IsActive && !firmAddress.IsDeleted
            //                 select categoryFirmAddress.Category.DgppId).Distinct().ToArray();

            //            advertisingMaterialInfo.StableRubrIds = categoryDgppIds;
            //        }
            //    }
            //}
            #endregion

            /// <summary>
            /// Запрос для проверки наличия заказов с позициями, которые обрабатывались IAdvertisingMaterialConverter до релиза 1.0
            /// </summary>
            private const string SQLQueryForOrderPositionWithOldFormatAdvertisements =
@"Use [ERM]

declare @markerdate datetime
set @markerdate = '20130315'

SELECT o.*,op.*,opa.*
FROM [Billing].[Orders] o
inner join [Billing].[OrderPositions] op
	on o.Id = op.OrderId AND op.IsActive = 1 AND op.IsDeleted = 0 AND
	o.IsActive = 1 AND o.IsDeleted = 0 
	AND o.BeginDistributionDate <= @markerdate AND (
	--@markerdate <= o.EndDistributionDatePlan  OR 
	@markerdate <= o.EndDistributionDateFact
	) 
	AND (o.WorkflowStepId = 5 OR o.WorkflowStepId = 6)
inner join [Billing].[PricePositions] pp
	on op.PricePositionId = pp.Id
inner join [Billing].[Positions] pos
	on pp.PositionId = pos.Id
inner join [Billing].[PositionCategories] pc
	on pos.CategoryId = pc.Id AND (pc.ExportCode = 1 OR pc.ExportCode = 8)
left outer join [Billing].[OrderPositionAdvertisement] opa
	on op.Id = opa.OrderPositionId 
where opa.Id is not null AND (opa.FirmAddressId is not null OR opa.CategoryId is null)";

            public static bool IsValid(OrderPositionInfo positionInfo)
            {
                const int PriorityExportCode = 1;
                const int BannerExportCode = 8;

                if (positionInfo.ProductType != PriorityExportCode && positionInfo.ProductType != BannerExportCode)
                {
                    return true;
                }

                return positionInfo.AdvertisingMaterials.All(advertisingMaterialInfo => advertisingMaterialInfo.StableRubrIds.Any());
            }
        }
    }
}