using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Mime;
using System.Text;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.Accounts.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Users;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Common;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.API.Core.UseCases;
using DoubleGis.Erm.Platform.API.Security;
using DoubleGis.Erm.Platform.Common.Compression;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;

using Nuclear.Tracing.API;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    [UseCase(Duration = UseCaseDuration.Long)]
    [Obsolete("На текущий момент 1С читает поток юр. лиц из шины")]
    public sealed class ExportLegalPersonsHandler : RequestHandler<ExportLegalPersonsRequest, IntegrationResponse>
    {
        private static readonly Encoding CyrillicEncoding = Encoding.GetEncoding(1251);

        private readonly ISecurityServiceUserIdentifier _securityServiceUserIdentifier;
        private readonly ITracer _logger;
        private readonly IUserRepository _userRepository;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IAccountRepository _accountRepository;
        private readonly IGlobalizationSettings _globalizationSettings;
        private readonly IValidateLegalPersonsForExportOperationService _validateLegalPersonsForExportOperationService;

        public ExportLegalPersonsHandler(
            ISecurityServiceUserIdentifier securityServiceUserIdentifier,
            ITracer logger,
            ILegalPersonRepository legalPersonRepository,
            IUserRepository userRepository,
            IAccountRepository accountRepository,
            IGlobalizationSettings globalizationSettings,
            IValidateLegalPersonsForExportOperationService validateLegalPersonsForExportOperationService)
        {
            _securityServiceUserIdentifier = securityServiceUserIdentifier;
            _logger = logger;
            _legalPersonRepository = legalPersonRepository;
            _userRepository = userRepository;
            _accountRepository = accountRepository;
            _globalizationSettings = globalizationSettings;
            _validateLegalPersonsForExportOperationService = validateLegalPersonsForExportOperationService;
        }

        public static string ClearText(string input)
        {
            return !string.IsNullOrEmpty(input) ? input.Replace(';', ',') : input;
        }

        protected override IntegrationResponse Handle(ExportLegalPersonsRequest request)
        {
            if (request.OrganizationUnitId == null)
            {
                throw new ArgumentNullException("request.OrganizationUnitId");
            }

            var organizationUnitDetails = _userRepository.GetOrganizationUnitDetails(request.OrganizationUnitId.Value);

            var isMovedToCrm = organizationUnitDetails.Unit.ErmLaunchDate != null;

            if (!isMovedToCrm)
            {
                throw new NotificationException(BLResources.CantExportDataRelatedToNotMovedToCrmOrganizationUnit);
            }

            var legalPersonFor1CExportDtos =
                _legalPersonRepository.GetLegalPersonsForExportTo1C(request.OrganizationUnitId.Value, request.PeriodStart)
                                      .Distinct(new LegalPersonDtoEqualityComparer());

            if (!legalPersonFor1CExportDtos.Any())
            {
                throw new NotificationException(BLResources.ExportCouldnotFindLegalPersons);
            }

            _logger.InfoFormat("Начало проверки юр.лиц");
            var validationErrors =
                _validateLegalPersonsForExportOperationService.Validate(legalPersonFor1CExportDtos.Select(x =>
                                                                                                          new ValidateLegalPersonDto
            {
                                                                                                                  LegalPersonId = x.LegalPerson.Id,
                    SyncCode1C = x.LegalPersonSyncCode1C,
                                                                                                              }).ToArray());

            var notValidResponseLogBuilder = new StringBuilder();
            foreach (var item in validationErrors)
                                        {
                notValidResponseLogBuilder.AppendFormat("[{0}] [{1}] - {2}", item.LegalPersonId, item.SyncCode1C, item.ErrorMessage);
            }

            // write logs
            var logReportBuilder = new StringBuilder();
            var processedWithoutErrorsCount = legalPersonFor1CExportDtos.Count() - validationErrors.Count();

            logReportBuilder.AppendFormat("Обработано успешно юридических лиц - [{0}]", processedWithoutErrorsCount).Append(Environment.NewLine);
            logReportBuilder.AppendFormat("Неблокирующих ошибок - [{0}]:", validationErrors.Count(x => !x.IsBlockingError)).Append(Environment.NewLine);
            logReportBuilder.AppendFormat("Блокирующих ошибок - [{0}]:", validationErrors.Count(x => x.IsBlockingError)).Append(Environment.NewLine);
            logReportBuilder.Append(notValidResponseLogBuilder);
            _logger.InfoFormat(logReportBuilder.ToString());

            var legalPersons = legalPersonFor1CExportDtos.Select(x => x.LegalPerson);

            _legalPersonRepository.SyncWith1C(legalPersons.Where(x => !x.IsInSyncWith1C));

            var accounts = _accountRepository.GetAccountsForExortTo1C(request.OrganizationUnitId.Value);

            var blockingErrors = validationErrors.Where(x => x.IsBlockingError).Select(x => x.LegalPersonId).Distinct().ToArray();
            var legalPersonDtosToExport = legalPersonFor1CExportDtos.Where(x => !blockingErrors.Contains(x.LegalPerson.Id));
            var accountsDataTable = GetAccountsDataTable(accounts);
            var legalPersonsDataTable = GetLegalPersonsDataTable(legalPersonDtosToExport);
            var errorsDataTable = GetErrorsDataTable(validationErrors);

            return new IntegrationResponse
            {
                Stream = ZipDataTables(accountsDataTable, legalPersonsDataTable, errorsDataTable),
                ContentType = MediaTypeNames.Application.Zip,
                FileName = "Customers.zip",

                ProcessedWithoutErrors = processedWithoutErrorsCount,
                BlockingErrorsAmount = validationErrors.Count(x => x.IsBlockingError),
                NonBlockingErrorsAmount = validationErrors.Count(x => !x.IsBlockingError),
            };
        }

        private Stream ZipDataTables(DataTable accounts, DataTable legalPersons, DataTable errorsList)
        {
            var streamDictionary = new Dictionary<string, Stream>
            {
                {
                    "Accounts.csv",
                    CreateCsvStream(accounts)
                },
                {
                    "Customers.csv",
                    CreateCsvStream(legalPersons)
                }
            };

            if (errorsList != null)
            {
                streamDictionary.Add("ExportLegalPersonsErrors_" + DateTime.Today.ToShortDateString() + ".csv",
                                     CreateCsvStream((errorsList)));
            }

            return streamDictionary.ZipStreamDictionary();
        }

        private MemoryStream CreateCsvStream(DataTable accounts)
        {
            return new MemoryStream(CyrillicEncoding.GetBytes(accounts.ToCsv(_globalizationSettings.ApplicationCulture.TextInfo.ListSeparator)));
        }

        private static DataTable GetAccountsDataTable(IEnumerable<AccountFor1CExportDto> accounts)
        {
            const int attributesCount = 2;
            var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
            for (var i = 0; i < attributesCount; i++)
            {
                dataTable.Columns.Add(string.Empty); 
            }

            foreach (var account in accounts)
            {
                dataTable.Rows.Add(account.LegalPersonSyncCode1C, account.BranchOfficeOrganizationUnitSyncCode1C);
            }

            return dataTable;
        }

        private DataTable GetLegalPersonsDataTable(IEnumerable<LegalPersonFor1CExportDto> legalPersonDtos)
        {
            const int attributesCount = 13;
            var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
            for (var i = 0; i < attributesCount; i++)
            {
                dataTable.Columns.Add(string.Empty);
            }

            foreach (var legalPersonDto in legalPersonDtos)
            {
                var legalPerson = legalPersonDto.LegalPerson;
                var profile = legalPersonDto.Profile;
                dataTable.Rows.Add(
                    legalPersonDto.LegalPersonSyncCode1C,
                    ClearText(legalPerson.ShortName),
                    ClearText(legalPerson.LegalName),
                    legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson
                        ? ClearText(legalPerson.RegistrationAddress)
                        : ClearText(legalPerson.LegalAddress),
                    legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson ? ClearText(legalPerson.PassportSeries) : ClearText(legalPerson.Inn),
                    legalPerson.LegalPersonTypeEnum == LegalPersonType.NaturalPerson ? ClearText(legalPerson.PassportNumber) : ClearText(legalPerson.Kpp),
                    string.Concat(
                        profile.EmailForAccountingDocuments ?? string.Empty,
                        profile.EmailForAccountingDocuments != null && profile.AdditionalEmail != null ? "," : string.Empty,
                        profile.AdditionalEmail ?? string.Empty),
                    string.Format(
                        BLResources.ExportLegalPersonDelivaryFormattedInfo,
                        ClearText(profile.DocumentsDeliveryAddress),
                        ClearText(profile.Phone),
                        ClearText(profile.PersonResponsibleForDocuments)),
                    ClearText(profile.PostAddress),
                    ClearText(profile.RecipientName),
                    profile.DocumentsDeliveryMethod,
                    legalPerson.LegalPersonTypeEnum,
                    ClearText(_securityServiceUserIdentifier.GetUserInfo(legalPerson.OwnerCode).DisplayName));
            }

            return dataTable;
        }

        private static DataTable GetErrorsDataTable(IEnumerable<LegalPersonValidationForExportErrorDto> errors)
            {
            const int attributesCount = 4;
            var dataTable = new DataTable { Locale = CultureInfo.InvariantCulture };
            for (var i = 0; i < attributesCount; i++)
            {
                dataTable.Columns.Add(string.Empty);
            }

            foreach (var error in errors.Where(x => x.IsBlockingError))
            {
                dataTable.Rows.Add(error.LegalPersonId, error.SyncCode1C, BLResources.BlockingError, error.ErrorMessage);
            }

            foreach (var error in errors.Where(x => !x.IsBlockingError))
            {
                dataTable.Rows.Add(error.LegalPersonId, error.SyncCode1C, BLResources.NonBlockingError, error.ErrorMessage);
        }

            return dataTable;
        }

        private sealed class LegalPersonDtoEqualityComparer : IEqualityComparer<LegalPersonFor1CExportDto>
        {
            bool IEqualityComparer<LegalPersonFor1CExportDto>.Equals(LegalPersonFor1CExportDto x, LegalPersonFor1CExportDto y)
            {
                return x.LegalPerson.Id.Equals(y.LegalPerson.Id);
            }

            int IEqualityComparer<LegalPersonFor1CExportDto>.GetHashCode(LegalPersonFor1CExportDto obj)
            {
                return obj.LegalPerson.Id.GetHashCode();
            }
        }
    }
}
