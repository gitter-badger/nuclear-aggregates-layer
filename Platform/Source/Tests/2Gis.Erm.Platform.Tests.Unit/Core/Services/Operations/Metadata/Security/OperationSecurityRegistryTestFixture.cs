using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.Platform.API.Core.Metadata.Security;
using DoubleGis.Erm.Platform.API.Security.EntityAccess;
using DoubleGis.Erm.Platform.API.Security.FunctionalAccess;
using DoubleGis.Erm.Platform.Core.Metadata.Security;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Generic;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Order;

using NuClear.Model.Common.Entities;
using NuClear.Model.Common.Operations.Identity;
using NuClear.Model.Common.Operations.Identity.Generic;

using NUnit.Framework;

namespace DoubleGis.Erm.Platform.Tests.Unit.Core.Services.Operations.Metadata.Security
{
    [TestFixture]
    [Category("Security")]
    public sealed class OperationSecurityRegistryTestFixture
    {
        [Test]
        public void AccessRequirement_OperationIdentity_MustBeCreatedAccordingToGenericTypes()
        {
            var requirement = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(x => { });
            Assert.AreEqual(requirement.StrictOperationIdentity, new StrictOperationIdentity(CreateIdentity.Instance, new EntitySet(EntityType.Instance.Order())));
        }

        [Test]
        public void AccessRequirement_OperationUsages_MustBeCreatedAccordingToGenericTypes()
        {
            var requirement = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
                x => x.UsesOperation<ActualizeOrderReleaseWithdrawalsIdentity>()
                      .UsesOperation<AppendIdentity, Theme>());

            var usedOperations = requirement.UsedOperations.ToArray();
            Assert.AreEqual(2, usedOperations.Length, "В требование было записано две операции");
            Assert.Contains(new StrictOperationIdentity(ActualizeOrderReleaseWithdrawalsIdentity.Instance, EntitySet.Create.NonCoupled), usedOperations);
            Assert.Contains(new StrictOperationIdentity(AppendIdentity.Instance, new EntitySet(EntityType.Instance.Theme())), usedOperations);
        }

        [Test]
        public void AccessRequirement_SimpleAccessRequirements_MustBeKept()
        {
            var requirement = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
                x => x.Require(EntityAccessTypes.Create, EntityType.Instance.Order())
                      .Require(EntityAccessTypes.Read, EntityType.Instance.Firm())
                      .Require(EntityAccessTypes.Update, EntityType.Instance.File())
                      .Require(FunctionalPrivilegeName.CascadeLegalPersonAssign));

            var requirements = requirement.Requirements.ToArray();
            Assert.AreEqual(4, requirements.Length, "В требование было записано 4 привилегии");
        }

        [Test]
        public void AccessRequirementReader_SelfDependency_MustDetectWhenItExists()
        {
            var requirementWithSelfDependency = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
                x => x.UsesOperation<CreateIdentity, Order>());

            var resolver = new OperationDependencyResolver(new[] { requirementWithSelfDependency });
            Assert.IsTrue(resolver.HasDependencyLoops, "Цикличные зависимости должны обнаруживаться, когда они есть");
        }

        [Test]
        public void AccessRequirementReader_CycleDependency_MustDetectWhenItExists()
        {
            var requirementWithCycleDependency1 = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
                x => x.UsesOperation<UpdateIdentity, Order>());

            var requirementWithCycleDependency2 = AccessRequirementBuilder.ForOperation<UpdateIdentity, Order>(
                x => x.UsesOperation<CreateIdentity, Order>());

            var resolver = new OperationDependencyResolver(new[] { requirementWithCycleDependency1, requirementWithCycleDependency2 });
            Assert.IsTrue(resolver.HasDependencyLoops, "Цикличные зависимости должны обнаруживаться, когда они есть");
        }

        [Test]
        public void AccessRequirementReader_SelfDependency_MustNotDetectWhenItDoesNotExists()
        {
            var requirementWithoutSelfDependency = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(x => { });

            var resolver = new OperationDependencyResolver(new[] { requirementWithoutSelfDependency });
            Assert.IsFalse(resolver.HasDependencyLoops, "Цикличные зависимости не должны обнаруживаться, когда их нет");
        }

        [Test]
        public void AccessRequirementReader_CycleDependency_MustNotDetectWhenItDoesNotExists()
        {
            var requirementWithoutCycleDependency1 = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(x => { });
            var requirementWithoutCycleDependency2 = AccessRequirementBuilder.ForOperation<UpdateIdentity, Order>(x => { });

            var resolver = new OperationDependencyResolver(new[] { requirementWithoutCycleDependency1, requirementWithoutCycleDependency2 });
            Assert.IsFalse(resolver.HasDependencyLoops, "Цикличные зависимости не должны обнаруживаться, когда их нет");
        }

        [Test]
        public void AccessRequirementReader_MustDeriveRequirementsFromChildOperations()
        {
            /*
             *     1
             *    / \
             *   2   3
             *       |
             *       4
             */
            var operation1 = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
                x => x.UsesOperation<CreateIdentity, Firm>()
                      .UsesOperation<UpdateIdentity, Firm>()
                      .Require(EntityAccessTypes.Create, EntityType.Instance.Order()));
            var operation2 = AccessRequirementBuilder.ForOperation<CreateIdentity, Firm>(
                x => x.Require(EntityAccessTypes.Create, EntityType.Instance.Firm()));
            var operation3 = AccessRequirementBuilder.ForOperation<UpdateIdentity, Firm>(
                x => x.UsesOperation<CreateIdentity, Theme>()
                      .Require(EntityAccessTypes.Update, EntityType.Instance.Firm()));
            var operation4 = AccessRequirementBuilder.ForOperation<CreateIdentity, Theme>(
                x => x.Require(EntityAccessTypes.Update, EntityType.Instance.Theme()));

            var resolver = new OperationDependencyResolver(new[] { operation1, operation2, operation3, operation4 });
            var requirements = resolver.GetFlattedRequirements(operation1.StrictOperationIdentity).ToArray();
            
            Assert.AreEqual(4, requirements.Length, "В списке должно получиться 4 привелегии доступа");
            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Create, EntityType.Instance.Order()), requirements, "Список должен содержать явно объявленную привелегию");
            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Create, EntityType.Instance.Firm()), requirements, "Список должен содержать выведенную из дочерней операции привелегию");
            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Update, EntityType.Instance.Firm()), requirements, "Список должен содержать выведенную из дочерней операции привелегию");
            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Update, EntityType.Instance.Theme()), requirements, "Список должен содержать выведенную из дочерней операции привелегию");
        }

        [Test]
        public void AccessRequirementReader_ParentOperationsMustNotInfluenceToChild()
        {
            /*
             *     1
             *    / \
             *   2   3
             *       |
             *       4
             */
            var operation1 = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(
                x => x.UsesOperation<CreateIdentity, Firm>()
                      .UsesOperation<UpdateIdentity, Firm>()
                      .Require(EntityAccessTypes.Create, EntityType.Instance.Order()));
            var operation2 = AccessRequirementBuilder.ForOperation<CreateIdentity, Firm>(
                x => x.Require(EntityAccessTypes.Create, EntityType.Instance.Firm()));
            var operation3 = AccessRequirementBuilder.ForOperation<UpdateIdentity, Firm>(
                x => x.UsesOperation<CreateIdentity, Theme>()
                      .Require(EntityAccessTypes.Update, EntityType.Instance.Firm()));
            var operation4 = AccessRequirementBuilder.ForOperation<CreateIdentity, Theme>(
                x => x.Require(EntityAccessTypes.Update, EntityType.Instance.Theme()));

            var resolver = new OperationDependencyResolver(new[] { operation1, operation2, operation3, operation4 });

            var requirementsForOperation2 = resolver.GetFlattedRequirements(operation2.StrictOperationIdentity).ToArray();
            var requirementsForOperation3 = resolver.GetFlattedRequirements(operation3.StrictOperationIdentity).ToArray();
            var requirementsForOperation4 = resolver.GetFlattedRequirements(operation4.StrictOperationIdentity).ToArray();

            Assert.AreEqual(1, requirementsForOperation2.Length, "В списке должно получиться 1 привелегия доступа");
            Assert.AreEqual(2, requirementsForOperation3.Length, "В списке должно получиться 2 привелегии доступа");
            Assert.AreEqual(1, requirementsForOperation4.Length, "В списке должно получиться 1 привелегия доступа");

            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Create, EntityType.Instance.Firm()), requirementsForOperation2, "Список должен содержать привелегию");
            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Update, EntityType.Instance.Firm()), requirementsForOperation3, "Список должен содержать привелегию");
            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Update, EntityType.Instance.Theme()), requirementsForOperation3, "Список должен содержать привелегию");
            Assert.Contains(new EntityAccessRequirement(EntityAccessTypes.Update, EntityType.Instance.Theme()), requirementsForOperation4, "Список должен содержать привелегию");
        }

        [Test]
        public void OperationDependencyResolver_ShouldReturnNullOnUnknownOperation()
        {
            var operation1 = AccessRequirementBuilder.ForOperation<CreateIdentity, Order>(x => { });

            var resolver = new OperationSecurityRegistryReader(new[] { operation1 });

            IEnumerable<IAccessRequirement> notUsed;
            var result = resolver.TryGetOperationRequirements(new StrictOperationIdentity(UpdateIdentity.Instance, new[] { EntityType.Instance.Firm() }.ToEntitySet()), out notUsed);
            Assert.IsFalse(result);
        }
    }
}
