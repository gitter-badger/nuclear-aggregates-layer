using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Notifications;

namespace DoubleGis.Erm.Platform.Core.Notifications
{
    public class EmployeeEmailResolver : IEmployeeEmailResolver
    {
        private readonly IEnumerable<IEmployeeEmailResolveStrategy> _emailResolveStrategies;

        public EmployeeEmailResolver(IEnumerable<IEmployeeEmailResolveStrategy> emailResolveStrategies)
        {
            _emailResolveStrategies = emailResolveStrategies;
        }
        
        public bool TryResolveEmail(long employee, out string email)
        {
            email = null;

            foreach (var strategy in _emailResolveStrategies)
            {
                string resolvedEmail = null;
                if (strategy.TryResolveEmail(employee, out resolvedEmail))
                {
                    email = resolvedEmail;
                    return true;
                }
            }

            return false;
        }
    }
}
