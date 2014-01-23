using System;
using System.Reflection;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.MSpec.Tests.Unit
{
    public class UnitySutFactorySpecs
    {
        public class When_create
        {
            private Establish context = () =>
                {
                    Target = new UnitySutFactory<SimpleClass>();
                };

            private Because of = () =>
                {
                    Result = Target.Create();
                };

            private It should_instantiate_target_class = () => Result.Should().NotBeNull();

            private static SimpleClass Result { get; set; }
            private static UnitySutFactory<SimpleClass> Target { get; set; }

            private class SimpleClass
            {
            }
        }

        public class When_dependency_added
        {
            Establish context = () =>
                {
                    Target = new UnitySutFactory<TestDependentClass>();
                    _expectedDependency = Mock.Of<IDependency>();

                    Target.AddDependency(_expectedDependency);
                };

            private Because of = () =>
            {
                Result = Target.Create().Dependency;
            };

            private It should_resolve_dependency = () => Result.Should().Be(_expectedDependency);

            private static IDependency _expectedDependency;
            private static IDependency Result { get; set; }
            private static UnitySutFactory<TestDependentClass> Target { get; set; }
        }
    }
}