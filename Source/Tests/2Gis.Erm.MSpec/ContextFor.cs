using Machine.Specifications;

using Moq;

namespace DoubleGis.Erm.MSpec
{
    public abstract class ContextFor<TSubjectUnderTest>
        where TSubjectUnderTest : class
    {
        private static ISutFactory<TSubjectUnderTest> _sutFactory;
        private static TSubjectUnderTest _sut;

        protected static TSubjectUnderTest SUT
        {
            get { return _sut ?? (_sut = _sutFactory.Create()); }
        }

        Establish context = () =>
        {
            _sutFactory = new UnitySutFactory<TSubjectUnderTest>();
        };

        Cleanup stuff = () =>
        {
            _sut = null;
            _sutFactory = null;
        };

        protected static T Resolve<T>()
        {
            return _sutFactory.Resolve<T>();
        }

        protected static void RegisterDependency<TDependency, TImpl>() where TImpl : TDependency
        {
            _sutFactory.RegisterDependency<TDependency, TImpl>();
        }

        protected static T AddDependency<T>(T dependency)
        {
            return _sutFactory.AddDependency(dependency);
        }

        protected static T AddStub<T>() where T : class
        {
            return AddDependency(Mock.Of<T>());
        }

        protected static Mock<T> IntroduceMoq<T>() where T : class
        {
            var mock = new Mock<T>();

            AddDependency(mock.Object);

            return mock;
        }
    }
}
