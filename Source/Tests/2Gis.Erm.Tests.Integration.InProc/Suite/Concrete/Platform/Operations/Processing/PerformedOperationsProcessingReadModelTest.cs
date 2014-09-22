using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel.DTOs;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary.MsCRM;
using DoubleGis.Erm.Platform.API.Core.Settings.ConnectionStrings;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing
{
    [UseCase(Duration = UseCaseDuration.VeryLong)]
    public sealed class PerformedOperationsProcessingReadModelTest : IIntegrationTest
    {
        private const string ProdDbConnectionString = "Data Source=uk-sql20\\erm;Initial Catalog=ErmRU;Integrated Security=True";
        private const int BatchSize = 1000;

        private static readonly XNamespace SqlServerNamespace = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";
        private static readonly IMessageFlow SourceMessageFlow = PrimaryReplicate2MsCRMPerformedOperationsFlow.Instance;
        private static readonly DateTime IgnoreOperationsPrecedingDate = DateTime.Now.Date.AddDays(-5);

        private readonly IConnectionStringSettings _connectionStringSettings;
        private readonly IFinder _finder;
        private readonly IPerformedOperationsProcessingReadModel _readModel;
        private readonly IProducedQueryLogContainer _producedQueryLogContainer;
        private readonly IUseCaseTuner _useCaseTuner;

        public PerformedOperationsProcessingReadModelTest(
            IConnectionStringSettings connectionStringSettings,
            IFinder finder,
            IPerformedOperationsProcessingReadModel readModel,
            IProducedQueryLogContainer producedQueryLogContainer,
            IUseCaseTuner useCaseTuner)
        {
            _connectionStringSettings = connectionStringSettings;
            _finder = finder;
            _readModel = readModel;
            _producedQueryLogContainer = producedQueryLogContainer;
            _useCaseTuner = useCaseTuner;
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
                    _connectionStringSettings.GetConnectionString(ConnectionStringName.Erm);
                    //ProdDbConnectionString;
            }
        }

        private bool GetOperationsForPrimaryProcessingUsingReadModel()
        {
            _useCaseTuner.AlterDuration<PerformedOperationsProcessingReadModelTest>();

            _producedQueryLogContainer.Reset();
            var operations = _readModel.GetOperationsForPrimaryProcessing(SourceMessageFlow, IgnoreOperationsPrecedingDate, BatchSize);
            var operationsForPrimaryProcessingByReadModel = _producedQueryLogContainer.Queries.Single();

            _producedQueryLogContainer.Reset();
            var operationsAlternative = GetOperationsForPrimaryProcessingAlternative(SourceMessageFlow, IgnoreOperationsPrecedingDate, BatchSize);
            var operationsForPrimaryProcessingByAlternativeMethod = _producedQueryLogContainer.Queries.Single();

            using (new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                ExecuteLoggedQueries(new[] { operationsForPrimaryProcessingByReadModel, operationsForPrimaryProcessingByAlternativeMethod });
            }

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
            var performedOperations = _finder.Find(OperationSpecs.Performed.Find.AfterDate(oldestOperationBoundaryDate));
            var processingTargetUseCases =
                _finder.Find(OperationSpecs.PrimaryProcessings.Find.ByFlowIds(new[] { sourceMessageFlow.Id }))
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