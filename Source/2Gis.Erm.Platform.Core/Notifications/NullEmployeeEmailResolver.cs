using DoubleGis.Erm.Core.Services.Notifications;

namespace DoubleGis.Erm.BL.Services.Notifications
{
    // 2+: \Platform\Source\2Gis.Erm.Platform.Core\Notifications\NullEmployeeEmailResolver.cs
    public class NullEmployeeEmailResolver : IEmployeeEmailResolver
    {
        public bool TryResolveEmail(long employeeUserCode, out string email)
        {
            email = null;
            return true;
        }
    }
}