using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.Platform.API.Core.Notifications;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Crosscutting.EmailResolvers
{
    public class UserProfileEmployeeEmailResolveStrategy : IEmployeeEmailResolveStrategy
    {
        private readonly IUserRepository _userRepository;
        private readonly ITracer _tracer;

        public UserProfileEmployeeEmailResolveStrategy(IUserRepository userRepository, ITracer tracer)
        {
            _userRepository = userRepository;
            _tracer = tracer;
        }

        #region Implementation of IEmployeeEmailResolveStrategy

        public bool TryResolveEmail(long employeeUserCode, out string email)
        {
            email = null;

            var userProfile = _userRepository.GetProfileForUser(employeeUserCode);
            if (userProfile == null)
            {
                _tracer.Error("Can't find profile by user id: " + employeeUserCode);
                return false;
            }

            if (string.IsNullOrEmpty(userProfile.Email))
            {
                _tracer.Warn("Email is empty in user profile with id: " + userProfile.Id);
                return false;
            }

            email = userProfile.Email;

            return true;
        }

        #endregion
    }
}
