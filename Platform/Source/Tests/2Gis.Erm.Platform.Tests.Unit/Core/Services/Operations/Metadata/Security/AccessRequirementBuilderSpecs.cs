using System;

using DoubleGis.Erm.Platform.Core.Metadata.Security;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Limit;

using FluentAssertions;

using Machine.Specifications;

using NuClear.Model.Common;
using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

namespace DoubleGis.Erm.Platform.Tests.Unit.Core.Services.Operations.Metadata.Security
{
    [Subject(typeof(AccessRequirementBuilder))]
    class AccessRequirementBuilderSpecs
    {
        class AccessRequirementBuilderContext<TIdentity>
            where TIdentity : IdentityBase<TIdentity>, new()
        {
            protected static OperationAccessRequirement<TIdentity> _actionPassedParameter;
            protected static IEntityType[] _expectedEntityNames;
            protected static IOperationAccessRequirement _result;
            protected static Action<OperationAccessRequirement<TIdentity>> _storePassedParameterAction;

            Establish context = () => { _storePassedParameterAction = r => _actionPassedParameter = r; };

            protected static void SetExpectedEntityNames(params IEntityType[] entityNames)
            {
                _expectedEntityNames = entityNames;
            }
        }

        [Subject(typeof(AccessRequirementBuilder))]
        class When_build_requirement_for_identity : AccessRequirementBuilderContext<CloseLimitIdenitity>
        {
            Establish context = () => SetExpectedEntityNames(EntityType.Instance.None());

            Because of = () => _result = AccessRequirementBuilder.ForOperation(_storePassedParameterAction);

            Behaves_like<AccessRequirementBuilderBehaviors<CloseLimitIdenitity>> identity_only_requirement;
        }

        [Subject(typeof(AccessRequirementBuilder))]
        class When_build_requirement_for_entity_specific_operation : AccessRequirementBuilderContext<CopyIdentity>
        {
            Establish context = () => SetExpectedEntityNames(typeof(Bill).AsEntityName());

            Because of = () => _result = AccessRequirementBuilder.ForOperation<CopyIdentity, Bill>(_storePassedParameterAction);

            Behaves_like<AccessRequirementBuilderBehaviors<CopyIdentity>> entity_requirement;
        }

        [Subject(typeof(AccessRequirementBuilder))]
        class When_build_requirement_for_entities_specific_operation : AccessRequirementBuilderContext<CopyIdentity>
        {
            Establish context = () => SetExpectedEntityNames(typeof(Bill).AsEntityName(), typeof(Client).AsEntityName());

            Because of = () => _result = AccessRequirementBuilder.ForOperation<CopyIdentity, Bill, Client>(_storePassedParameterAction);

            Behaves_like<AccessRequirementBuilderBehaviors<CopyIdentity>> double_entities_requirement;
        }

        /// <summary>
        ///     Проверка передачи требования делегату и соответсвия списка имен сущностей.
        /// </summary>
        [Behaviors]
        class AccessRequirementBuilderBehaviors<TIdentity>
            where TIdentity : IdentityBase<TIdentity>, new()
        {
            protected static OperationAccessRequirement<TIdentity> _actionPassedParameter;
            protected static IEntityType[] _expectedEntityNames;
            protected static IOperationAccessRequirement _result;

            It should_have_same_result_and_action_passed_parameter = () => _result.Should().BeSameAs(_actionPassedParameter);

            It should_create_requirement_for_expected_entities = () =>
                {
                    var sti = ((IOperationAccessRequirement)_actionPassedParameter).StrictOperationIdentity;

                    sti.Entities.Should().BeEquivalentTo(_expectedEntityNames);
                    sti.OperationIdentity.Should().BeSameAs(OperationIdentityBase<TIdentity>.Instance);
                };
        }
    }
}