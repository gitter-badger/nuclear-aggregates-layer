using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.MSpec.Tests.Unit
{
    public class ContextForSpecs
    {
        public class When_Resolve_dependency_in_Establish : ContextFor<TestDependentClass>
        {
            Establish context = () =>
            {
                RegisterDependency<IDependency, TestImplDependency>();

                _result = Resolve<IDependency>();
            };

            Because of = () => { };

            private It should_return_mapped_type = () => _result.Should().BeOfType<TestImplDependency>();

            static IDependency _result;
            static IDependency _expectedDependency;
        }

        public class When_derived_reg_dependency_in_Establish : ContextFor<TestDependentClass>
        {
            Establish context = () =>
                {
                    RegisterDependency<IDependency, TestImplDependency>();

                    _result = SUT.Dependency;
                };

            Because of = () => { };

            private It should_resolve_dependncy_for_sut = () => _result.Should().BeOfType<TestImplDependency>();

            static IDependency _result;
            static IDependency _expectedDependency;
        }

        public class When_derived_Establish : ContextFor<TestDependentClass>
        {
            Establish context = () =>
                {
                    _expectedDependency = Mock.Of<IDependency>();

                    AddDependency(_expectedDependency);

                    _result = SUT.Dependency;
                };

            Because of = () => { };

            It should_init_SUT_property_with_dependencies = () => _result.Should().NotBeNull();

            static IDependency _result;
            static IDependency _expectedDependency;
        }

        public class When_IntroduceMoq_in_establish : ContextFor<TestDependentClass>
        {
            Establish context = () =>
            {
                _expectedDependency = IntroduceMoq<IDependency>().Object;

                _result = SUT.Dependency;
            };

            Because of = () => { };

            It should_init_SUT_property_with_Moq_dependency = () => _result.Should().Be(_expectedDependency);

            static IDependency _result;
            static IDependency _expectedDependency;
        }
    }
}