using System;

namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Infrastructure
{
    // TODO {a.tukaev, 05.05.2014}: Все же думаю, более расширяемым будет иметь отдельное описание (читай - маппинг, метаданные), чем примешивать бизнес-инфу в .net-тип
    // IImportMetadataProvider - не для этого?
    // COMMENT {d.ivanov, 07.05.2014}: Для этого, но предполагается наличие нескольких реестров; не используя инфраструктуру метаданных, проще пока сделать именно так
    // COMMENT {a.tukaev, 08.05.2014}: Потом важно не забыть
    public sealed class ServiceBusObjectDescriptionAttribute : Attribute
    {
        private readonly string _objectName;
        private readonly ServiceBusObjectProcessingOrder _processingOrder = ServiceBusObjectProcessingOrder.Last;

        public ServiceBusObjectDescriptionAttribute(string objectName, ServiceBusObjectProcessingOrder processingOrder)
        {
            _objectName = objectName;
            _processingOrder = processingOrder;
        }

        public ServiceBusObjectDescriptionAttribute(string objectName)
        {
            _objectName = objectName;
        }

        public string ObjectName
        {
            get { return _objectName; }
        }

        public ServiceBusObjectProcessingOrder ProcessingOrder
        {
            get { return _processingOrder; }
        }
    }
}