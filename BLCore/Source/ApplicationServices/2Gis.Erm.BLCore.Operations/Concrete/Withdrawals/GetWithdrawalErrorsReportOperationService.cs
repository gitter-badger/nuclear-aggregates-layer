using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.OrganizationUnits.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Withdrawals;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.Withdrawal;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Withdrawals
{
    public sealed class GetWithdrawalErrorsReportOperationService : IGetWithdrawalErrorsCsvReportOperationService
    {
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly IOrganizationUnitReadModel _organizationUnitReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public GetWithdrawalErrorsReportOperationService(IGlobalizationSettings globalizationSettings, IOrganizationUnitReadModel organizationUnitReadModel, IOperationScopeFactory operationScopeFactory)
        {
            _globalizationSettings = globalizationSettings;
            _organizationUnitReadModel = organizationUnitReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public WithdrawalErrorsReport GetErrorsReport(IDictionary<long, WithdrawalProcessingResult> resultsWithErrors, TimePeriod period, AccountingMethod accountingMethod)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetWithdrawalErrorsCsvReportIdentity>())
            {
                var organizationUnitNames = _organizationUnitReadModel.GetNames(resultsWithErrors.Select(x => x.Key));
                var dataTable = new DataTable();
                dataTable.Columns.Add(MetadataResources.OrganizationUnit);
                dataTable.Columns.Add(BLResources.Error);

                foreach (var withdrawalProcessingResult in resultsWithErrors)
                {
                    foreach (var message in withdrawalProcessingResult.Value.ProcessingMessages)
                    {
                        dataTable.Rows.Add(organizationUnitNames[withdrawalProcessingResult.Key], message.Text);
                    }
                }

                var csvReportContent = dataTable.ToCsvEscaped(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator, true);
                var reportContent = Encoding.UTF8.GetPreamble().Concat(Encoding.UTF8.GetBytes(csvReportContent)).ToArray();

                scope.Complete();

                return new WithdrawalErrorsReport
                {
                    ReportContent = reportContent,
                    ReportFileName = string.Format("WithdrawalReport{0:dd-MM-yy}_{1:dd-MM-yy}_{2}.csv", period.Start, period.End, accountingMethod),
                    ContentType = "text/csv",
                };
            }
        }
    }
}
