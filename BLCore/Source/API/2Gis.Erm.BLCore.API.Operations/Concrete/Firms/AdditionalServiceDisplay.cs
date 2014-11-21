namespace DoubleGis.Erm.BLCore.API.Operations.Concrete.Firms
{
    public enum AdditionalServiceDisplay
    {
        // не отображать (в БД храним false)
        DoNotDisplay = 0,

        // отображать (в БД храним true)
        Display = 1,

        // значение по умолчанию (в БД не храним)
        Default,

        // зависит от адреса (в БД не храним)
        DependsOnAddress,
    }
}
