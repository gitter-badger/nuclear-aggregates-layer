﻿using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes;

using FluentAssertions;

using NuClear.Storage;
using NuClear.Storage.UseCases;
using NuClear.Tracing.API;

using IConnectionStringSettings = NuClear.Storage.ConnectionStrings.IConnectionStringSettings;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class PerformedOperationsProcessingReadModelTest : IIntegrationTest
    {
        private const string ProdDbConnectionString = "Data Source=uk-sql20\\erm;Initial Catalog=ErmRU;Integrated Security=True";
        private const int BatchSize = 1500;

        private static readonly XNamespace SqlServerNamespace = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";
        private static readonly IMessageFlow SourceMessageFlow = PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance;

        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IQuery _query;
        private readonly IPerformedOperationsProcessingReadModel _readModel;
        private readonly IProducedQueryLogContainer _producedQueryLogContainer;
        private readonly IUseCaseTuner _useCaseTuner;
        private readonly ITracer _tracer;

        public PerformedOperationsProcessingReadModelTest(
            IConnectionStringSettings connectionStringSettings,
            IQuery query,
            IPerformedOperationsProcessingReadModel readModel,
            IProducedQueryLogContainer producedQueryLogContainer,
            IUseCaseTuner useCaseTuner,
            ITracer tracer)
        {
            _connectionStringSettings = connectionStringSettings;
            _query = query;
            _readModel = readModel;
            _producedQueryLogContainer = producedQueryLogContainer;
            _useCaseTuner = useCaseTuner;
            _tracer = tracer;
        }

        public ITestResult Execute()
        {
            return Result
                .When(GetOperationsForPrimaryProcessingUsingReadModel())
                .Then(result => result.Should().BeTrue())
                .Then(result => result.Status == TestResultStatus.Succeeded);
        }

        private string TargetEnvironmentConnectionString
        {
            get 
            { 
                return 
                    _connectionStringSettings.GetConnectionString(ErmConnectionStringIdentity.Instance);
                    //ProdDbConnectionString;
            }
        }

        private bool GetOperationsForPrimaryProcessingUsingReadModel()
        {
            _useCaseTuner.AlterDuration<PerformedOperationsProcessingReadModelTest>();

            var sourceFlowState = _readModel.GetPrimaryProcessingFlowState(SourceMessageFlow);
            var oldestOperationBoundaryWithOffset = (sourceFlowState != null ? sourceFlowState.OldestProcessingTargetCreatedDate : DateTime.UtcNow).AddHours(-2);

            _producedQueryLogContainer.Reset();
            var operations = _readModel.GetOperationsForPrimaryProcessing(SourceMessageFlow, oldestOperationBoundaryWithOffset, BatchSize);
            var operationsForPrimaryProcessingByReadModelQuery = _producedQueryLogContainer.Queries.Single();

            /*
            _producedQueryLogContainer.Reset();
            var operationsAlternative = GetOperationsForPrimaryProcessingAlternative(SourceMessageFlow, oldestOperationBoundaryWithOffset, BatchSize);
            var operationsForPrimaryProcessingByAlternativeMethodQuery = _producedQueryLogContainer.Queries.Single();

            using (new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                ExecuteLoggedQueries(new[] { operationsForPrimaryProcessingByReadModelQuery, operationsForPrimaryProcessingByAlternativeMethodQuery });
            }*/

            return operations != null;
        }

        private void ExecuteLoggedQueries(IEnumerable<string> queries)
        {
            foreach (var query in queries)
            {
                string planXml;
                using (var connection = new SqlConnection(TargetEnvironmentConnectionString))
                {
                    using (var command = new SqlCommand { Connection = connection })
                    {
                        connection.Open();

                        command.CommandText = "SET SHOWPLAN_XML ON";
                        command.ExecuteNonQuery();

                        command.CommandText = query;
                        planXml = (string)command.ExecuteScalar();

                        command.CommandText = "SET SHOWPLAN_XML OFF";
                        command.ExecuteNonQuery();
                    }
                }

                double? cost = null;
                var xelement = XElement.Load(new StringReader(planXml));
                var stmtSimpleNode = xelement.Descendants(SqlServerNamespace + "StmtSimple").FirstOrDefault();
                if (stmtSimpleNode != null)
                {
                    cost = (double)stmtSimpleNode.Attribute("StatementSubTreeCost");
                }

                Console.WriteLine("Query executed on target environment. Cost: {0}", cost != null ? cost.ToString() : "N/A");
            }
        }

        private IReadOnlyList<DBPerformedOperationsMessage> GetOperationsForPrimaryProcessingAlternative(
            IMessageFlow sourceMessageFlow,
            DateTime oldestOperationBoundaryDate,
            int maxUseCaseCount)
        {
            var performedOperations = _query.For(OperationSpecs.Performed.Find.AfterDate(oldestOperationBoundaryDate));
            var processingTargetUseCases = _query.For(OperationSpecs.PrimaryProcessings.Find.ByFlowId(sourceMessageFlow.Id))
                       .OrderBy(targetUseCase => targetUseCase.CreatedOn)
                       .Take(maxUseCaseCount);

            return (from targetUseCase in processingTargetUseCases
                    join performedOperation in performedOperations on targetUseCase.UseCaseId equals performedOperation.UseCaseId
                        into useCaseOperations
                    orderby targetUseCase.CreatedOn
                    select new DBPerformedOperationsMessage
                        {
                            TargetUseCase = targetUseCase,
                            Operations = useCaseOperations
                        })
                    .ToList();
        }
    }
}