using NuClear.Security.API.UserContext;
using NuClear.Security.API.UserContext.Identity;
using NuClear.Security.API.UserContext.Profile;

using Moq;

namespace DoubleGis.Erm.Platform.Tests.Unit.DAL.Infrastructure.Fakes
{
    internal sealed class StubUserContext : IUserContext
    {
        public const int FakeCurrentUserCode = 10;

        public const int FakeAnotherUserCode = 11;

        private readonly bool _skipSecureCheck;

        public StubUserContext() : this(false)
        {
        }

        public StubUserContext(bool skipAccessCheck)
        {
            _skipSecureCheck = skipAccessCheck;
        }

        public IUserIdentity Identity
        {
            get
            {
                var userIdentity = new Mock<IUserIdentity>();
                userIdentity.Setup(x => x.Code).Returns(FakeCurrentUserCode);
                userIdentity.Setup(x => x.SkipEntityAccessCheck).Returns(_skipSecureCheck);
                return userIdentity.Object;
            }
        }

        public IUserProfile Profile
        {
            get { return Mock.Of<IUserProfile>(); }
        }
    }
}
