using System;

using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Qds.Docs;
using DoubleGis.Erm.Qds.Etl.Transform.Docs;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Transform.Docs
{
    class OrderGridDocMapperSpecs
    {
        [Subject(typeof(ClientGridDocMapper))]
        class When_update_by_unsupported_entity_type : OrderGridDocMapperContext
        {
            Because of = () => Result = Catch.Exception(() => Target.UpdateDocByEntity(new[] { new OrderGridDoc() }, Mock.Of<IEntityKey>()));

            It should_throw_not_supported_excpetion = () => Result.Should().BeOfType<NotSupportedException>();

            static Exception Result;
        }

        [Subject(typeof(OrderGridDocMapper))]
        class When_update_by_order : OrderGridDocMapperContext
        {
            Establish context = () =>
                {
                    ExpectedWorkflowStepName = "Some order state";
                    const int workflowStepId = (int)OrderState.Archive;

                    Document = new OrderGridDoc();
                    Order = new Order
                        {
                            Id = 42,
                            Number = "13579",
                            BeginDistributionDate = DateTime.Now,
                            EndDistributionDatePlan = DateTime.Now,
                            EndDistributionDateFact = DateTime.Now,
                            IsActive = true,
                            IsDeleted = true,
                            HasDocumentsDebt = 52,
                            ModifiedOn = DateTime.Now,
                            CreatedOn = DateTime.Now,
                            PayablePlan = 13.13m,
                            WorkflowStepId = workflowStepId,
                            AmountToWithdraw = 14.14m,
                            AmountWithdrawn = 15.15m,
                        };

                    EnumLocalizer.Setup(e => e.LocalizeFromId<OrderState>(workflowStepId)).Returns(ExpectedWorkflowStepName);
                };

            Because of = () => Target.UpdateDocByEntity(new[] { Document }, Order);

            It should_map_fields = () => Order.ShouldBeEquivalentTo(Document, options => options.ExcludingMissingProperties());
            It should_resolve_workflowstep_as_name = () => Document.WorkflowStep.Should().Be(ExpectedWorkflowStepName);

            static Order Order;
            static OrderGridDoc Document;
            static string ExpectedWorkflowStepName;
        }

        class OrderGridDocMapperContext
        {
            Establish context = () =>
                {
                    EnumLocalizer = new Mock<IEnumLocalizer>();
                    Target = new OrderGridDocMapper(EnumLocalizer.Object);
                };

            protected static Mock<IEnumLocalizer> EnumLocalizer { get; private set; }

            protected static OrderGridDocMapper Target { get; private set; }
        }
    }
}