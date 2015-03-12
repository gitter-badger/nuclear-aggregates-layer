namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    /// <summary>
    /// Идентификаторы сервисов интеграции, использующиеся в таблице [Shared].[BusinessOperationServices]
    /// </summary>
    public enum IntegrationService
    {
        Undefined = 0,
        ExportFlowFinancialDataLegalEntity = 3,
        ExportFlowOrdersAdvMaterial = 4,
        ExportFlowOrdersOrder = 5,
        ExportFlowOrdersResource = 6,
        ExportFlowOrdersTheme = 7,
        ExportFlowOrdersThemeBranch = 8,
        ImportFirmAddressNames = 9,
        ExportFlowFinancialDataClient = 11,
        ExportFlowPriceListsPriceList = 12,
        ExportFlowPriceListsPriceListPosition = 13,
        ExportFlowOrdersInvoice = 14,
        ExportFlowNomenclaturesNomenclatureElement = 15,
        ExportFlowNomenclaturesNomenclatureElementRelation = 16,
        ExportFlowDeliveryDataLetterSendRequest = 17,
        ExportFlowOrdersDenialReason = 18
    }
}
