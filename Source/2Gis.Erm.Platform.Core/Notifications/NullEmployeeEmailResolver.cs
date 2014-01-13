using DoubleGis.Erm.Platform.API.Core.Notifications;

namespace DoubleGis.Erm.Platform.Core.Notifications
{
    public class NullEmployeeEmailResolver : IEmployeeEmailResolver
    {
        public bool TryResolveEmail(long employeeUserCode, out string email)
        {
            email = null;
            return true;
        }
    }
}