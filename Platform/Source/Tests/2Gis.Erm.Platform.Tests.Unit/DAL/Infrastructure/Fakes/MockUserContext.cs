using DoubleGis.Erm.Platform.API.Security.UserContext.Identity;

using Moq;

using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    /// <summary>
    ///     Моковая реализация <see cref="IUserContext" /> на основе Moq.
    /// </summary>
    public class MockUserContext : Mock<IUserContext>
    {
        public const long UserCode = 42;

        public MockUserContext()
        {
            MockIdentity = new Mock<IUserIdentity>();
            MockIdentitySecurityControl = MockIdentity.As<IUserIdentitySecurityControl>();
            MockIdentity.SetupGet(i => i.Code).Returns(UserCode);

            SetupGet(u => u.Identity).Returns(MockIdentity.Object);
        }

        public Mock<IUserIdentity> MockIdentity { get; private set; }
        public Mock<IUserIdentitySecurityControl> MockIdentitySecurityControl { get; private set; }

        /// <summary>
        /// Настроить возвращаемое значение для мока <see cref="MockIdentity"/> при запросе <see cref="IUserIdentity.SkipEntityAccessCheck"/>.
        /// </summary>
        /// <param name="checkAccess">Возвращаемое значение.</param>
        public void SkipEntityAccess(bool checkAccess)
        {
            MockIdentitySecurityControl.SetupGet(i => i.SkipEntityAccessCheck).Returns(checkAccess);
        }
    }
}