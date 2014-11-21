using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Processors.Concrete
{
    // TODO {all, 01.04.2014}: � ������ ���� ���������� ��� �������������, ��� ��� ����� ����� ������ ������� ���, ��� ��� ����� - ������� ��� �� �����������  
    // ������ �� ������ ��� �� ������ - ���� �� �������� ���������� � �������� ����������, �.�. ����� ���� ��� ������ ���������� ������������ ����� ���������������� ����������, 
    // ����� ��������, � ����� ������ � �.�. - ���� ��� �����, �.�. ��� ��� ���������� � ������ ���� + � ����������� ����� ������� ����������� ������� �� perfomedbusinessoperation (servicebus � �.�.)
    public sealed class InvoiceIntegrationProcessorOperationService : IntegrationProcessorOperationService<Order, ExportFlowOrdersInvoice>
    {
        public InvoiceIntegrationProcessorOperationService(
            IOperationsProcessingsStoreService<Order, ExportFlowOrdersInvoice> processingsStoreService,
            IOperationsExporter<Order, ExportFlowOrdersInvoice> operationsExporter) 
            : base(processingsStoreService, operationsExporter)
        {
        }
    }
}