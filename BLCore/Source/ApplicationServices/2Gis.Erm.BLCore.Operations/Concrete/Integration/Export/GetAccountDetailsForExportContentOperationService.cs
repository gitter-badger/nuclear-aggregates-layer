using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;
using System.Xml.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Integration.Export;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.AccountDetails.Dto;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Common.Utils.Data;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.AccountDetail;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Integration.Export
{
    public sealed class GetAccountDetailsForExportContentOperationService : IGetAccountDetailsForExportContentOperationService
    {
        private static readonly Encoding CyrillicEncoding = Encoding.GetEncoding(1251);
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly IGetDebitsInfoInitialForExportOperationService _getDebitsInfoInitialForExportOperationService;
        private readonly IValidateLegalPersonsForExportOperationService _validateLegalPersonsForExportOperationService;
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;
        private readonly IAccountReadModel _accountReadModel;

        public GetAccountDetailsForExportContentOperationService(
            IGlobalizationSettings globalizationSettings,
            IGetDebitsInfoInitialForExportOperationService getDebitsInfoInitialForExportOperationService,
            IValidateLegalPersonsForExportOperationService validateLegalPersonsForExportOperationService,
            ILegalPersonReadModel legalPersonReadModel,
            IOperationScopeFactory operationScopeFactory,
            IAccountReadModel accountReadModel)
        {
            _globalizationSettings = globalizationSettings;
            _getDebitsInfoInitialForExportOperationService = getDebitsInfoInitialForExportOperationService;
            _validateLegalPersonsForExportOperationService = validateLegalPersonsForExportOperationService;
            _legalPersonReadModel = legalPersonReadModel;
            _operationScopeFactory = operationScopeFactory;
            _accountReadModel = accountReadModel;
        }

        public IEnumerable<IntegrationResponse> Get(DateTime startPeriodDate, DateTime endPeriodDate, IEnumerable<long> organizationUnitIds)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<GetAccountDetailsForExportContentIdentity>())
            {
                var organizationUnitsWithoutWithdrawalOperation =
                    _accountReadModel.GetOrganizationUnitsWithNoSuccessfulLastWithdrawal(organizationUnitIds,
                                                                                         new TimePeriod(startPeriodDate, endPeriodDate));

                if (organizationUnitsWithoutWithdrawalOperation.Any())
                {
                    // TODO {all, 16.02.2015}: перенести в ресурсник для нелокализуемых строк
                    throw new ExportAccountDetailsBeforeWithdrawalOperationException(string.Format("Для ({0}) за период {1} - {2} нет успешной операции списания",
                                                                                                   string.Join(",", organizationUnitsWithoutWithdrawalOperation),
                                                                                                   startPeriodDate.ToShortDateString(),
                                                                                                   endPeriodDate.ToShortDateString()));
                }

                var legalPersonsToValidateByOrganizationUnit = _legalPersonReadModel.GetLegalPersonDtosToValidate(organizationUnitIds, startPeriodDate, endPeriodDate);
                var errorsByOrganizationUnit = legalPersonsToValidateByOrganizationUnit
                    .ToDictionary(legalPersonsToValidate => legalPersonsToValidate.Key,
                                  legalPersonsToValidate =>
                                  _validateLegalPersonsForExportOperationService.Validate(legalPersonsToValidate.Value.DistinctBy(y => y.LegalPersonId)));

                var organizationUnitWithoutErrorsIds =
                    organizationUnitIds.Except(errorsByOrganizationUnit.Where(x => x.Value.Any(y => y.IsBlockingError)).Select(x => x.Key)).ToArray();

                var debitsInfoInitialsByOrganizationUnit = _getDebitsInfoInitialForExportOperationService.Get(startPeriodDate,
                                                                                                              endPeriodDate,
                                                                                                              organizationUnitWithoutErrorsIds);

                scope.Complete();

                return organizationUnitIds.Select(organizationUnitId =>
                                                  ConstructResponse(errorsByOrganizationUnit[organizationUnitId].ToArray(),
                                                                    debitsInfoInitialsByOrganizationUnit.ContainsKey(organizationUnitId)
                                                                        ? debitsInfoInitialsByOrganizationUnit[organizationUnitId]
                                                                        : null));
            }
        }

        private static DataTable GetErrorsDataTable(IEnumerable<LegalPersonValidationForExportErrorDto> errors)
        {
            const int AttributesCount = 4;
            var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
            for (var i = 0; i < AttributesCount; i++)
            {
                dataTable.Columns.Add(string.Empty);
            }

            foreach (var error in errors.OrderByDescending(x => x.IsBlockingError))
            {
                dataTable.Rows.Add(error.LegalPersonId, error.SyncCode1C, error.IsBlockingError ? BLResources.BlockingError : BLResources.NonBlockingError, error.ErrorMessage);
            }

            return dataTable;
        }

        private IntegrationResponse ConstructResponse(IReadOnlyCollection<LegalPersonValidationForExportErrorDto> errors,                                                      
                                                      DebitsInfoInitialDto debitsInfoInitial)
        {
            var errorsFileName = string.Format("ExportErrors_{0}.csv", DateTime.Today.ToShortDateString());
            var response = new IntegrationResponse
                               {
                                   BlockingErrorsAmount = errors.Count(x => x.IsBlockingError),
                                   NonBlockingErrorsAmount = errors.Count(x => !x.IsBlockingError)
                               };

            var errorContent = GetErrorsDataTable(errors).ToCsv(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator);
            if (errors.Any(x => x.IsBlockingError))
            {
                response.FileName = errorsFileName;
                response.ContentType = MediaTypeNames.Application.Octet;
                response.Stream = new MemoryStream(CyrillicEncoding.GetBytes(errorContent));

                return response;
            }

            var streamDictionary = new Dictionary<string, Stream>();
            if (errors.Any())
            {
                streamDictionary.Add(errorsFileName, new MemoryStream(CyrillicEncoding.GetBytes(errorContent)));
            }

            var debitsStream = new MemoryStream(Encoding.UTF8.GetBytes(debitsInfoInitial.ToXElement().ToString(SaveOptions.None)));
            streamDictionary.Add("DebitsInfoInitial_" + DateTime.Today.ToShortDateString() + ".xml", debitsStream);

            response.FileName = "Acts.zip";
            response.ContentType = MediaTypeNames.Application.Zip;
            response.Stream = streamDictionary.ZipStreamDictionary();
            
            // Лажа, но повторил, как было.
            response.ProcessedWithoutErrors = debitsInfoInitial.Debits.Count() - errors.Count;

            return response;
        }
    }
}