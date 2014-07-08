using System;
using System.Collections.Generic;

using DoubleGis.Erm.Platform.API.Core.Identities;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Core.Operations.Logging.Transports.DB;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;
using DoubleGis.Erm.Qds.Etl.Extract.EF;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.Qds.Etl.Tests.Unit.Extract.EF
{
    class PboEntityLinkBuilderSpecs
    {
        [Subject(typeof(PboEntityLinkBuilder))]
        class When_create_links_for_not_supported_changes_type : PboEntityLinkBuilderContext
        {
            Because of = () => CatchedEx = Catch.Exception(() => Target.CreateEntityLinks(Mock.Of<IChangeDescriptor>()));

            It should_throw_not_supported_exception = () => CatchedEx.Should().BeOfType<NotSupportedException>();

            static Exception CatchedEx;
        }

        [Subject(typeof(PboEntityLinkBuilder))]
        class When_build_links_for_performed_business_operation : PboEntityLinkBuilderContext
        {
            Establish context = () =>
                {
                    ExpectedEntityName = EntityName.Client;
                    ExpectedId = 42;

                    _testDescr = new PboChangeDescriptor(new PerformedBusinessOperation
                                    {
                                        Context = "xmlContext",
                                        Operation = 1,
                                        Descriptor = 2
                                    });

                    SetupContextParserReferenceForOperation(_testDescr.Operation, ExpectedEntityName, ExpectedId);
                };

            Because of = () => Result = Target.CreateEntityLinks(_testDescr);

            It should_return_appropriate_entity_name_and_id =
                () => Result.Should().OnlyContain(e => e.Name == ExpectedEntityName && e.Id == ExpectedId,
                    "Ожидалась запись с информацией об объекте: тип сущности и идентификатор.");

            static IEnumerable<EntityLink> Result;
            static long ExpectedId;
            static EntityName ExpectedEntityName;
            static PboChangeDescriptor _testDescr;
        }

        [Subject(typeof(PboEntityLinkBuilder))]
        class When_pbo_with_multiple_changes_created_by_operation_logger
        {
            Establish context = () =>
            {
                int operationId = 300;
                var opIdentity = new Mock<INonCoupledOperationIdentity>();
                opIdentity.SetupGet(oi => oi.Id).Returns(operationId);

                var opScope = new Mock<IOperationScope>();
                opScope.SetupGet(os => os.OperationIdentity)
                       .Returns(new StrictOperationIdentity(opIdentity.Object, EntitySet.Create.NonCoupled));

                var operationScopeNode = new OperationScopeNode(opScope.Object.Id, opScope.Object.IsRoot, opScope.Object.OperationIdentity);

                var repository = new Mock<IRepository<PerformedBusinessOperation>>();
                repository.Setup(r => r.Add(Moq.It.IsAny<PerformedBusinessOperation>()))
                          .Callback((PerformedBusinessOperation pbo) => _testPbo = pbo);

                var scopeChanges = operationScopeNode.ChangesContext;
                scopeChanges.Added<Client>(new long[] { _expectedClientIdFirst, _expectedClientIdSecond });
                scopeChanges.Updated<Order>(new long[] { _expectedOrderId });
                scopeChanges.Deleted<Firm>(new long[] { _expectedFirmId });

                var trackedUseCase2PboConverter = new TrackedUseCase2PerfomedBusinessOperationsConverter(Mock.Of<IIdentityProvider>());
                var dbLoggingStrategy = new DirectDBLoggingStrategy(trackedUseCase2PboConverter, repository.Object);
                var operationLogger = new OperationLogger(new IOperationLoggingStrategy[] { dbLoggingStrategy }, new NullLogger());
                operationLogger.Log(operationScopeNode);

                var operationIdentityRegistry = new Mock<IOperationIdentityRegistry>();
                operationIdentityRegistry.Setup(oir => oir.GetIdentity(operationId)).Returns(opIdentity.Object);

                Target = new PboEntityLinkBuilder(new OldOperationContextParser(operationIdentityRegistry.Object));
            };

            Because of = () => Result = Target.CreateEntityLinks(new PboChangeDescriptor(_testPbo));

            It should_parse_entity_type_and_id = () =>
            {
                ResultShouldContain(EntityName.Client, _expectedClientIdFirst);
                ResultShouldContain(EntityName.Client, _expectedClientIdSecond);
                ResultShouldContain(EntityName.Order, _expectedOrderId);
                ResultShouldContain(EntityName.Firm, _expectedFirmId);
            };

            static void ResultShouldContain(EntityName name, long id)
            {
                Result.Should().Contain(r => r.Name == name && r.Id == id);
            }

            const int _expectedClientIdFirst = 3;
            const int _expectedClientIdSecond = 33;
            const int _expectedOrderId = 22;
            const int _expectedFirmId = 111;
            static PerformedBusinessOperation _testPbo;

            static IEnumerable<EntityLink> Result { get; set; }
            static PboEntityLinkBuilder Target { get; set; }
        }

        class PboEntityLinkBuilderContext
        {
            Establish context = () =>
                {
                    Operations = new List<PerformedBusinessOperation>();
                    ContextParser = new Mock<IOldOperationContextParser>();
                    Target = new PboEntityLinkBuilder(ContextParser.Object);
                };

            protected static PerformedBusinessOperation AddPerformedBusinessOperation(string opContext, int operation, int descriptor)
            {
                var performedBusinessOperation = new PerformedBusinessOperation
                    {
                        Context = opContext,
                        Operation = operation,
                        Descriptor = descriptor
                    };

                Operations.Add(performedBusinessOperation);

                return performedBusinessOperation;
            }

            protected static void SetupContextParserReferenceForOperation(PerformedBusinessOperation pbo, EntityName name, long id)
            {
                var parsedOp = new Dictionary<StrictOperationIdentity, IEnumerable<long>>
                    {
                        { new StrictOperationIdentity(Mock.Of<IOperationIdentity>(), new EntitySet(name)), new[] { id } }
                    };

                ContextParser.Setup(p => p.GetGroupedIdsFromContext(pbo.Context, pbo.Operation, pbo.Descriptor))
                    .Returns(parsedOp);
            }

            protected static List<PerformedBusinessOperation> Operations { get; private set; }
            protected static Mock<IOldOperationContextParser> ContextParser { get; private set; }
            protected static PboEntityLinkBuilder Target { get; private set; }
        }
    }
}
