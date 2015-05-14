using Moq;

using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Security.API.UserContext.Profile;

namespace Storage.EntityFramework.Tests.Fakes
{
    internal sealed class StubUserContext : IUserContext
    {
        public const int FakeCurrentUserCode = 10;

        public const int FakeAnotherUserCode = 11;

        public IUserIdentity Identity
        {
            get
            {
                var userIdentity = new Mock<IUserIdentity>();
                userIdentity.Setup(x => x.Code).Returns(FakeCurrentUserCode);

                return userIdentity.Object;
            }
        }

        public IUserProfile Profile
        {
            get { return Mock.Of<IUserProfile>(); }
        }
    }
}
