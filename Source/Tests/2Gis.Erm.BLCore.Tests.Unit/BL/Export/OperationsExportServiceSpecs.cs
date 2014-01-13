using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.DAL.PersistenceServices.Export;
using DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export.Export;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

using Machine.Specifications;

using Moq;

using It = Machine.Specifications.It;

// ReSharper disable ConvertToLambdaExpression
namespace DoubleGis.Erm.BLCore.Tests.Unit.BL.Export
{
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "It's a test!")]
    public abstract class OperationsExportServiceSpecs
    {
        protected const int QueueSize = 42;
        protected const int BatchSize = 10;
        protected static IOperationsExportService exportService;
        protected static FlowDescription flow;

        protected static IExportableOperationsPersistenceService<EntityClass, ExportedEntityClass> persistenceService;
        protected static IOperationsExporter<EntityClass, ExportedEntityClass> operationsExporter;
        protected static List<ExportFailedEntity> recordsToProcess;
        protected static List<ExportFailedEntity> recordsWasRequested;
        protected static List<IExportableEntityDto> recordsWasRemovedFromQueue;

        private Establish context = () =>
        {
            persistenceService = Mock.Of<IExportableOperationsPersistenceService<EntityClass, ExportedEntityClass>>();
            operationsExporter = Mock.Of<IOperationsExporter<EntityClass, ExportedEntityClass>>();
            exportService = new OperationsExportService(persistenceService, operationsExporter);
            flow = new FlowDescription();


            recordsToProcess = Enumerable.Range(0, QueueSize).Select(i => new ExportFailedEntity { Id = i }).ToList();
            recordsWasRequested = new List<ExportFailedEntity>();
            recordsWasRemovedFromQueue = new List<IExportableEntityDto>();
        };

        private Because of = () => exportService.ExportFailedEntities(flow, BatchSize);

        public class EntityClass : IEntity, IEntityKey
        {
            public long Id { get; set; }
        }

        public class ExportedEntityClass : IEntity, IEntityKey
        {
            public long Id { get; set; }
        }

        public class OperationsExportService : OperationsExportService<EntityClass, ExportedEntityClass>
        {
            public OperationsExportService(IExportableOperationsPersistenceService<EntityClass, ExportedEntityClass> persistenceService,
                                           IOperationsExporter<EntityClass, ExportedEntityClass> operationsExporter)
                : base(persistenceService, operationsExporter)
            {
            }

            protected override ISelectSpecification<ExportedEntityClass, DateTime> ProcessingDateSpecification
            {
                get { return null; }
            }

            protected override void UpdateProcessedOperation(ExportedEntityClass processedOperationEntity)
            {
            }

            protected override ExportedEntityClass CreateProcessedOperation(PerformedBusinessOperation operation)
            {
                return null;
            }
        }

        public class ExportableEntityDto : IExportableEntityDto
        {
            public long Id { get; set; }
        }
    }

    [Tags("Export")]
    [Subject(typeof(OperationsExportService<,>))]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "It's a test!")]
    public class When_export_all_fails : OperationsExportServiceSpecs
    {
        private Establish context = () =>
        {
            Mock.Get(persistenceService)
                .Setup(service => service.GetFailedEntities(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns<int, int>((takeCount, skipCount) =>
                {
                    var result = recordsToProcess.Skip(skipCount).Take(takeCount).ToArray();
                    recordsWasRequested.AddRange(result);
                    return result;
                });

            // Чтобы метод не возвращал null для out параметра.
            IEnumerable<IExportableEntityDto> outResult = new List<ExportableEntityDto>();
            Mock.Get(operationsExporter)
                .Setup(exporter => exporter.ExportFailedEntities(Moq.It.IsAny<FlowDescription>(),
                                                                 Moq.It.IsAny<IEnumerable<ExportFailedEntity>>(),
                                                                 Moq.It.IsAny<int>(),
                                                                 out outResult));

            // Грязный хак. 
            // Поскольку используя moq ОЧЕНЬ непросто влиять на out параметры (https://groups.google.com/group/moqdisc/browse_thread/thread/4c7590e725151da9)
            // поэтому, зная, что метод RemoveFromFailureQueue вызывается после ExportFailedEntities и получает тот же самый объект, что вернулся из ExportFailedEntities
            // мы этот список наполняем в callback для RemoveFromFailureQueue.
            Mock.Get(persistenceService)
                .Setup(service => service.RemoveFromFailureQueue(Moq.It.IsAny<IEnumerable<IExportableEntityDto>>()))
                .Callback<IEnumerable<IExportableEntityDto>>(dtos =>
                {
                    recordsWasRemovedFromQueue.AddRange(dtos);

                    var list = (List<ExportableEntityDto>)dtos;
                    list.Clear();
                    list.AddRange(new ExportableEntityDto[0]);
                });
        };

        private It should_take_first_batch = () =>
        {
            Mock.Get(persistenceService).Verify(service => service.GetFailedEntities(Moq.It.IsAny<int>(), Moq.It.Is<int>(i => i == 0)));
        };

        private It should_take_each_record_once = () =>
        {
            var eachRecordProcessedOnce = recordsWasRequested.Count == recordsWasRequested.Distinct().Count();
            eachRecordProcessedOnce.ShouldBeTrue();
        };

        private It should_take_next_batch_in_the_same_job_iteration = () =>
        {
            Mock.Get(persistenceService).Verify(service => service.GetFailedEntities(Moq.It.IsAny<int>(), Moq.It.Is<int>(i => i == BatchSize)));
        };

        private It should_process_all_queue = () =>
        {
            recordsToProcess.All(entity => recordsWasRequested.Contains(entity)).ShouldBeTrue();
        };

        private It should_not_remove_any_record_from_queue = () =>
        {
            recordsToProcess.Count.ShouldEqual(QueueSize);
            recordsWasRemovedFromQueue.Count.ShouldEqual(0);
        };
    }

    [Tags("Export")]
    [Subject(typeof(OperationsExportService<,>))]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1401:FieldsMustBePrivate", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.NamingRules", "SA1310:FieldNamesMustNotContainUnderscore", Justification = "It's a test!")]
    [SuppressMessage("StyleCop.CSharp.MaintainabilityRules", "SA1402:FileMayOnlyContainASingleClass", Justification = "It's a test!")]
    public class When_export_all_success : OperationsExportServiceSpecs
    {
        private Establish context = () =>
        {
            // Продолжение грязного хака, связанного с out параметрами. 
            // Сначала мы узнаём сколько сущностей получил сервис экспорта, а затем сообщаем, что ровно столько было успешно экспортировано.
            int count = 0;
            Mock.Get(persistenceService)
                .Setup(service => service.GetFailedEntities(Moq.It.IsAny<int>(), Moq.It.IsAny<int>()))
                .Returns<int, int>((takeCount, skipCount) =>
                {
                    var result = recordsToProcess.Skip(skipCount).Take(takeCount).ToArray();
                    recordsWasRequested.AddRange(result);
                    count = result.Length;
                    return result;
                });

            // Чтобы метод не возвращал null для out параметра.
            IEnumerable<IExportableEntityDto> outResult = new List<ExportableEntityDto>();
            Mock.Get(operationsExporter)
                .Setup(exporter => exporter.ExportFailedEntities(Moq.It.IsAny<FlowDescription>(),
                                                                 Moq.It.IsAny<IEnumerable<ExportFailedEntity>>(),
                                                                 Moq.It.IsAny<int>(),
                                                                 out outResult));

            Mock.Get(persistenceService)
                .Setup(service => service.RemoveFromFailureQueue(Moq.It.IsAny<IEnumerable<IExportableEntityDto>>()))
                .Callback<IEnumerable<IExportableEntityDto>>(dtos =>
                {
                    recordsToProcess.RemoveAll(entity => recordsWasRequested.Contains(entity));

                    var list = (List<ExportableEntityDto>)dtos;
                    list.Clear();
                    list.AddRange(Enumerable.Range(0, count).Select(i => new ExportableEntityDto()));

                    recordsWasRemovedFromQueue.AddRange(dtos);
                });
        };

        private It should_remove_all_records_from_queue = () =>
        {
            recordsToProcess.Count.ShouldEqual(0);
            recordsWasRemovedFromQueue.Count.ShouldEqual(QueueSize);
        };
    }
}
// ReSharper restore ConvertToLambdaExpression
