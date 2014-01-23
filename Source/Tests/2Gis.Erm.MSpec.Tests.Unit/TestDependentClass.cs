namespace DoubleGis.Erm.MSpec.Tests.Unit
{
    public class TestDependentClass
    {
        public TestDependentClass(IDependency dependency)
        {
            Dependency = dependency;
        }

        public IDependency Dependency { get; private set; }
    }
}