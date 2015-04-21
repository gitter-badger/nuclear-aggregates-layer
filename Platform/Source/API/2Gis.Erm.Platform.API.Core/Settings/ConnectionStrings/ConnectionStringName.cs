namespace DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings
{
    /// <summary>
    /// Перечень имен для строк подключения к источникам данных
    /// </summary>
    public enum ConnectionStringName
    {
        None,
        Logging,
        Erm,
        ErmReports,
        CrmConnection,
        ErmValidation,
        ErmSearch,
        ErmPerformedOperationsServiceBus,
        QuartzJobStore
    }
}
