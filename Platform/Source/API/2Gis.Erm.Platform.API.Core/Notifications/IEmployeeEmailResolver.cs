namespace DoubleGis.Erm.Platform.API.Core.Notifications
{
    public interface IEmployeeEmailResolver
    {
        bool TryResolveEmail(long employeeUserCode, out string email);
    }
}
