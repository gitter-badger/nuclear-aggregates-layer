namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export
{
    /// <summary>
    /// Идентификаторы сервисов интеграции, использующиеся в таблице [Shared].[BusinessOperationServices]
    /// </summary>
    public enum IntegrationService
    {
        // todo {a.rechkalov}: добавить контроль того, что одинаковые номера не используются для разных сервисов одновременно
        // comment {y.baranihin, 2013-08-12}: острой необходимости в такой проверке нет, предалгаю заменить на todo
        // TODO {all, 23.08.2013}: Нужно переименовать этот enum и соответствующую таблицу в IntegrationFlowType
        // comment {all, 19.09.2012}: Хотелось бы избежать слова "flow" в названии таблицы и enum
        Undefined = 0,
        ExportFlowCardExtensionsCardCommercial = 2,
        ExportFlowFinancialDataLegalEntity = 3,
        ExportFlowOrdersAdvMaterial = 4,
        ExportFlowOrdersOrder = 5,
        ExportFlowOrdersResource = 6,
        ExportFlowOrdersTheme = 7,
        ExportFlowOrdersThemeBranch = 8,
        ImportFirmAddressNames = 9,
        CreateHotClientTask = 10,
        ExportFlowFinancialDataClient = 11,
    }
}
