using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.HotClient;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Exporters;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Operations.RequestResponse;
using DoubleGis.Erm.Platform.Common.Logging;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity;

using FluentAssertions;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Export
{
    // TODO {d.ivanov, 25.11.2013}: должен лечь в 2Gis.Erm.BLCore.Tests.Unit\BL\Export\HotClientRequestOperationsExporterSpecs.cs
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1400:AccessModifierMustBeDeclared", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1306:FieldNamesMustBeginWithLowerCaseLetter", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1305:FieldNamesMustNotUseHungarianNotation", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    public static class HotClientRequestOperationsExporterSpecs
    {
        public const int PackageSize = 10;

        public abstract class BasicContext
        {
            protected static HotClientRequestOperationsExporter Exporter;
            protected static ICommonLog Logger;
            protected static IPublicService PublicService;
            protected static IOperationContextParser OperationContextParser;
            protected static FlowDescription FlowDescription;

            Establish context = () =>
                {
                    Logger = SetupLogger();
                    PublicService = SetupPublicService();
                    OperationContextParser = SetupOperationContextParser();

                    Exporter = new HotClientRequestOperationsExporter(Logger, PublicService, OperationContextParser);

                    FlowDescription = new FlowDescription();
                    
                };

            private static IOperationContextParser SetupOperationContextParser()
            {
                var operationContexParser = Mock.Of<IOperationContextParser>();
                var operationIdentity = new StrictOperationIdentity(Mock.Of<IOperationIdentity>(), new EntitySet(EntityName.None));

                Mock.Get(operationContexParser)
                    .Setup(x => x.GetGroupedIdsFromContext(Moq.It.IsAny<string>(), Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                    .Returns(new Dictionary<StrictOperationIdentity, IEnumerable<long>> { { operationIdentity, new[] { 1L } } });

                return operationContexParser;
            }

            private static IPublicService SetupPublicService()
            {
                var publicService = Mock.Of<IPublicService>();
                return publicService;
            }

            private static ICommonLog SetupLogger()
            {
                var logger = Mock.Of<ICommonLog>();
                return logger;
            }
        }

        public class WhenTryingToProcessMessages : BasicContext
        {
            protected static IEnumerable<PerformedBusinessOperation> PerformedBusinessOperations;

            protected static IEnumerable<IExportableEntityDto> FailedEntites;
            protected static Exception Exception;

            Because of = () =>
            {
                try
                {
                    Exporter.ExportOperations(FlowDescription, PerformedBusinessOperations, PackageSize, out FailedEntites);
                    Exception = null;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                }
            };
        }

        public sealed class WhenTryingToProcessMessagesAndFailedFithException : WhenTryingToProcessMessages
        {
            Establish context = () =>
            {
                Mock.Get(PublicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<Request>()))
                    .Throws<Exception>();

                PerformedBusinessOperations = new[]
                        {
                            new PerformedBusinessOperation {Id = 1},
                            new PerformedBusinessOperation {Id = 2},
                            new PerformedBusinessOperation {Id = 3},
                        };

            };

            It should_not_throw_exception = () => Exception.Should().BeNull();

            It shold_try_to_export_each_operation = () =>
                Mock.Get(PublicService).Verify(x => x.Handle(Moq.It.IsAny<Request>()), Times.Exactly(PerformedBusinessOperations.Count()));

            It should_move_operations_to_failed = () => FailedEntites.Should().HaveCount(PerformedBusinessOperations.Count());
        }

        public sealed class WhenTryingToProcessMessagesAndFailedFithStatus : WhenTryingToProcessMessages
        {
            Establish context = () =>
            {
                Mock.Get(PublicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<Request>()))
                    .Returns(new CreateHotClientResponse { Success = false });

                PerformedBusinessOperations = new[]
                        {
                            new PerformedBusinessOperation {Id = 1},
                            new PerformedBusinessOperation {Id = 2},
                            new PerformedBusinessOperation {Id = 3},
                        };

            };

            It should_not_throw_exception = () => Exception.Should().BeNull();

            It shold_try_to_export_each_operation = () =>
                Mock.Get(PublicService).Verify(x => x.Handle(Moq.It.IsAny<Request>()), Times.Exactly(PerformedBusinessOperations.Count()));

            private It should_move_operations_to_failed = () => FailedEntites.Should().HaveCount(PerformedBusinessOperations.Count());
        }

        public sealed class WhenTryingToProcessMessagesAndSucceeded : WhenTryingToProcessMessages
        {
            Establish context = () =>
            {
                Mock.Get(PublicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<Request>()))
                    .Returns(new CreateHotClientResponse { Success = true });

                PerformedBusinessOperations = new[]
                        {
                            new PerformedBusinessOperation {Id = 1},
                            new PerformedBusinessOperation {Id = 2},
                            new PerformedBusinessOperation {Id = 3},
                        };

            };

            It should_not_throw_exception = () => Exception.Should().BeNull();

            It shold_try_to_export_each_operation = () =>
                Mock.Get(PublicService).Verify(x => x.Handle(Moq.It.IsAny<Request>()), Times.Exactly(PerformedBusinessOperations.Count()));

            It should_not_move_operations_to_failed = () => FailedEntites.Should().BeEmpty();
        } 

        public class WhenTryingToProcessFailedQueue : BasicContext
        {
            protected static IEnumerable<ExportFailedEntity> FailedEntities;

            protected static IEnumerable<IExportableEntityDto> SucceededEntites;
            protected static Exception Exception;

            Establish context = () =>
                {
                    FailedEntities = new[]
                        {
                            new ExportFailedEntity { Id = 1 },
                            new ExportFailedEntity { Id = 2 },
                            new ExportFailedEntity { Id = 3 },
                        };

                };
          
            Because of = () =>
            {
                try
                {
                    Exporter.ExportFailedEntities(FlowDescription, FailedEntities, PackageSize, out SucceededEntites);
                    Exception = null;
                }
                catch (Exception ex)
                {
                    Exception = ex;
                }
            };
        }

        public sealed class WhenTryingToProcessFailedQueueAndFailedFithException : WhenTryingToProcessFailedQueue
        {
            Establish context = () =>
            {
                Mock.Get(PublicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<Request>()))
                    .Throws<Exception>();
            };

            It should_not_throw_exception = () => Exception.Should().BeNull();

            It shold_try_to_export_each_operation = () =>
                Mock.Get(PublicService).Verify(x => x.Handle(Moq.It.IsAny<Request>()), Times.Exactly(FailedEntities.Count()));

            It should_move_operations_to_failed = () => SucceededEntites.Should().BeEmpty();
        }

        public sealed class WhenTryingToProcessFailedQueueAndFailedFithStatus : WhenTryingToProcessFailedQueue
        {
            Establish context = () =>
            {
                Mock.Get(PublicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<Request>()))
                    .Returns(new CreateHotClientResponse { Success = false });
            };

            It should_not_throw_exception = () => Exception.Should().BeNull();

            It shold_try_to_export_each_operation = () =>
                Mock.Get(PublicService).Verify(x => x.Handle(Moq.It.IsAny<Request>()), Times.Exactly(FailedEntities.Count()));

            private It should_move_operations_to_failed = () => SucceededEntites.Should().BeEmpty();
        }

        public sealed class WhenTryingToProcessFailedQueueAndSucceeded : WhenTryingToProcessFailedQueue
        {
            Establish context = () =>
            {
                Mock.Get(PublicService)
                    .Setup(x => x.Handle(Moq.It.IsAny<Request>()))
                    .Returns(new CreateHotClientResponse { Success = true });
            };

            It should_not_throw_exception = () => Exception.Should().BeNull();

            It shold_try_to_export_each_operation = () =>
                Mock.Get(PublicService).Verify(x => x.Handle(Moq.It.IsAny<Request>()), Times.Exactly(FailedEntities.Count()));

            It should_not_move_operations_to_failed = () => SucceededEntites.Should().HaveCount(FailedEntities.Count());
        } 
    }
}
