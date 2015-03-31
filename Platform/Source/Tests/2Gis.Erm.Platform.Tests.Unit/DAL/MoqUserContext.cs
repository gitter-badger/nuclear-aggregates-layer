using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;

using Moq;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL
{
    /// <summary>
    ///     Моковая реализация <see cref="IUserContext" /> на основе Moq.
    /// </summary>
    public class MoqUserContext : Mock<IUserContext>
    {
        public const long UserCode = 42;

        public MoqUserContext()
        {
            MockIdentity = new Mock<IUserIdentity>();
            MockIdentity.SetupGet(i => i.Code).Returns(UserCode);

            SetupGet(u => u.Identity).Returns(MockIdentity.Object);
        }

        public Mock<IUserIdentity> MockIdentity { get; private set; }

        /// <summary>
        /// Настроить возвращаемое значение для мока <see cref="MockIdentity"/> при запросе <see cref="IUserIdentity.SkipEntityAccessCheck"/>.
        /// </summary>
        /// <param name="checkAccess">Возвращаемое значение.</param>
        public void SkipEntityAccess(bool checkAccess)
        {
            MockIdentity.SetupGet(i => i.SkipEntityAccessCheck).Returns(checkAccess);
        }
    }
}