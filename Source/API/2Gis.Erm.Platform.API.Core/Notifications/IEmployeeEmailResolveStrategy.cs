namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public interface IEmployeeEmailResolveStrategy
    {
        bool TryResolveEmail(long employeeUserCode, out string email);
    }
}
