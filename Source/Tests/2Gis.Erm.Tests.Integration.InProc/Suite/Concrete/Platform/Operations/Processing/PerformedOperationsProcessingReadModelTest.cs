using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using System.Linq;
using System.Transactions;
using System.Xml.Linq;

using DoubleGis.Erm.Platform.API.Aggregates.SimplifiedModel.PerformedOperations.ReadModel;
using DoubleGis.Erm.Platform.API.Core.Messaging.Flows;
using DoubleGis.Erm.Platform.API.Core.Operations.Processing.Primary;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure;
using DoubleGis.Erm.Tests.Integration.InProc.Suite.Infrastructure.Fakes;

using FluentAssertions;

namespace DoubleGis.Erm.Tests.Integration.InProc.Suite.Concrete.Platform.Operations.Processing
{
    public sealed class PerformedOperationsProcessingReadModelTest : IIntegrationTest
    {
        private const string ProdDbConnectionString = "Data Source=uk-sql20\\erm;Initial Catalog=ErmRU;Integrated Security=True";
        private const int BatchSize = 1000;

        private static readonly XNamespace SqlServerNamespace = "http://schemas.microsoft.com/sqlserver/2004/07/showplan";
        private static readonly IMessageFlow SourceMessageFlow = AllPerformedOperationsFlow.Instance;
        private static readonly DateTime IgnoreOperationsPrecedingDate = DateTime.Now.Date.AddDays(-5);

        private readonly IFinder _finder;
        private readonly IPerformedOperationsProcessingReadModel _readModel;
        private readonly IProducedQueryLogContainer _producedQueryLogContainer;

        public PerformedOperationsProcessingReadModelTest(IFinder finder,
                                                          IPerformedOperationsProcessingReadModel readModel,
                                                          IProducedQueryLogContainer producedQueryLogContainer)
        {
            _finder = finder;
            _readModel = readModel;
            _producedQueryLogContainer = producedQueryLogContainer;
        }

        public ITestResult Execute()
        {
            return Result
                .When(GetOperationsForPrimaryProcessingUsingReadModel())
                .Then(result => result.Should().BeTrue())
                .Then(result => result.Status == TestResultStatus.Succeeded);
        }

        private bool GetOperationsForPrimaryProcessingUsingReadModel()
        {
            _producedQueryLogContainer.Reset();
            var operations = _readModel.GetOperationsForPrimaryProcessing(SourceMessageFlow, IgnoreOperationsPrecedingDate, BatchSize);
            var operationsForPrimaryProcessingQuerySyntax = _producedQueryLogContainer.Queries.Single();
            
            _producedQueryLogContainer.Reset();
            var fluentSyntaxQueryResult = ExecuteFluentSyntaxQuery();
            var operationsForPrimaryProcessingFluentSyntax = _producedQueryLogContainer.Queries.Single();

            using (new TransactionScope(TransactionScopeOption.RequiresNew))
            {
                ExecuteLoggedQueries(new[] { operationsForPrimaryProcessingQuerySyntax, operationsForPrimaryProcessingFluentSyntax });
            }

            return operations.Count() == fluentSyntaxQueryResult.Count();
        }

        private void ExecuteLoggedQueries(IEnumerable<string> queries)
        {
            foreach (var query in queries)
            {
                string planXml;
                using (var connection = new SqlConnection(ProdDbConnectionString))
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

                Console.WriteLine("Query executed on PROD environment. Cost: {0}", cost != null ? cost.ToString() : "N/A");
            }
        }

        private IEnumerable<IEnumerable<PerformedBusinessOperation>> ExecuteFluentSyntaxQuery()
        {
            var defaultUseCaseId = new Guid("00000000-0000-0000-0000-000000000000");

            var performedUseCases =
                _finder.FindAll<PerformedBusinessOperation>()
                       .Where(o => o.UseCaseId != defaultUseCaseId && o.Date > IgnoreOperationsPrecedingDate && o.Parent == null);

            var processedOperations =
                _finder.FindAll<PerformedOperationPrimaryProcessing>()
                       .Where(o => o.MessageFlowId == SourceMessageFlow.Id && o.Date > IgnoreOperationsPrecedingDate);

            var query = performedUseCases
                .GroupJoin(
                    processedOperations,
                    performedOperation => performedOperation.Id,
                    processedOperation => processedOperation.Id,
                    (performedOperation, performedOperationProcessings) => new
                        {
                            TargetOperation = performedOperation,
                            Processing = performedOperationProcessings.FirstOrDefault()
                        })
                .Where(performedOperationInfo => performedOperationInfo.Processing == null)
                .Select(performedOperationInfo => performedOperationInfo.TargetOperation)
                .GroupBy(
                    pbo => pbo.UseCaseId,
                    (guid, pbos) => new
                        {
                            Date = pbos.Min(operation => operation.Date),
                            UseCaseId = guid
                        })
                .OrderBy(x => x.Date)
                .Take(BatchSize)
                .GroupJoin(
                    _finder.FindAll<PerformedBusinessOperation>().Where(o => o.UseCaseId != defaultUseCaseId && o.Date > IgnoreOperationsPrecedingDate),
                    targetUseCase => targetUseCase.UseCaseId,
                    performedOperation => performedOperation.UseCaseId,
                    (targetUseCase, operations) => new
                        {
                            Operations = operations,
                            Date = targetUseCase.Date
                        })
                .OrderBy(useCase => useCase.Date)
                .Select(useCase => useCase.Operations);

            return query.ToArray();
        }
    }
}