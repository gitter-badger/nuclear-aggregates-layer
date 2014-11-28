using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Accounts;
using DoubleGis.Erm.BLCore.API.Aggregates.BranchOffices.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Common.Enums;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.Platform.API.Core.Settings.Globalization;
using DoubleGis.Erm.Platform.Common.Utils;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    public class ValidateAccountDetailsFrom1CHandler : RequestHandler<ValidateAccountDetailsFrom1CRequest, ValidateAccountDetailsFrom1CResponse>
    {
        private readonly IBranchOfficeReadModel _branchOfficeReadModel;
        private readonly ILocalizationSettings _localizationSettings;
        private readonly ILegalPersonRepository _legalPersonRepository;
        private readonly IAccountRepository _accountRepository;

        private readonly Dictionary<string, BranchOfficeOrganizationUnit> _branchOfficeOrganizationUnitsBy1CCode;
        private readonly Dictionary<long, BranchOfficeOrganizationUnit> _branchOfficeOrganizationUnitsById;

        public ValidateAccountDetailsFrom1CHandler(
            ILocalizationSettings localizationSettings,
            IBranchOfficeReadModel branchOfficeReadModel,
            ILegalPersonRepository legalPersonRepository,
            IAccountRepository accountRepository)
        {
            _localizationSettings = localizationSettings;
            _branchOfficeReadModel = branchOfficeReadModel;
            _legalPersonRepository = legalPersonRepository;
            _accountRepository = accountRepository;
            _branchOfficeOrganizationUnitsBy1CCode = new Dictionary<string, BranchOfficeOrganizationUnit>();
            _branchOfficeOrganizationUnitsById = new Dictionary<long, BranchOfficeOrganizationUnit>();
        }

        protected override ValidateAccountDetailsFrom1CResponse Handle(ValidateAccountDetailsFrom1CRequest request)
        {
            var result = new ValidateAccountDetailsFrom1CResponse();

            var targetEncoding = _localizationSettings.ApplicationCulture.ToDefaultAnsiEncoding();
            var rows = AccountDetailsFrom1CHelper.ParseStreamAsRows(request.InputStream, targetEncoding);
            if (rows.Length == 0)
            {
                result.Errors.Add(string.Format("Файл [{0}] пуст", request.FileName));
                return result;
            }

            AccountDetailsFrom1CHelper.CsvHeader csvHeader;
            if (!AccountDetailsFrom1CHelper.CsvHeader.TryParse(rows[0], _localizationSettings.ApplicationCulture, out csvHeader))
            {
                result.Errors.Add(string.Format("Неверный формат заголовка файла [{0}]", request.FileName));
                return result;
            }

            // юр. лицо отделения организации, которое было выбрано на форме
            BranchOfficeOrganizationUnit selectedBranchOfficeOrganizationUnit;
            if (_branchOfficeOrganizationUnitsById.ContainsKey(request.BranchOfficeOrganizationUnitId))
            {
                selectedBranchOfficeOrganizationUnit = _branchOfficeOrganizationUnitsById[request.BranchOfficeOrganizationUnitId];
            }
            else
            {
                selectedBranchOfficeOrganizationUnit = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(request.BranchOfficeOrganizationUnitId);
                _branchOfficeOrganizationUnitsById.Add(selectedBranchOfficeOrganizationUnit.Id, selectedBranchOfficeOrganizationUnit);
            }

            var contributionType = _branchOfficeReadModel.GetOrganizationUnitContributionType(selectedBranchOfficeOrganizationUnit.OrganizationUnitId);
            if (!_branchOfficeOrganizationUnitsBy1CCode.ContainsKey(selectedBranchOfficeOrganizationUnit.SyncCode1C))
            {
                _branchOfficeOrganizationUnitsBy1CCode.Add(selectedBranchOfficeOrganizationUnit.SyncCode1C, selectedBranchOfficeOrganizationUnit);
            }

            if (contributionType == ContributionTypeEnum.Franchisees)
            {
                if (selectedBranchOfficeOrganizationUnit.SyncCode1C != csvHeader.BranchOfficeOrganizationUnit1CCode)
                {
                    result.Errors.Add(string.Format(
                        "Код 1С выбранного юр. лица отделения организации ({0}) не совпадает с кодом в заголовке файла импорта ({1})",
                        selectedBranchOfficeOrganizationUnit.SyncCode1C,
                        csvHeader.BranchOfficeOrganizationUnit1CCode));
                    return result;
                }
            }

            for (int i = 1; i < rows.Length; i++)
            {
                AccountDetailsFrom1CHelper.CsvRow row;
                if (!AccountDetailsFrom1CHelper.CsvRow.TryParse(rows[i], _localizationSettings.ApplicationCulture, out row))
                {
                    result.Errors.Add("Неверный формат строки " + i);
                    continue;
                }

                if (row.BranchOfficeOrganizationUnit1CCode.Length == 0 || row.LegalPerson1CCode.Length == 0)
                {
                    AddErrorFieldValueIsMissing(result, i, "Код договора");
                    continue;
                }

                if (contributionType == ContributionTypeEnum.Franchisees)
                {
                    if (row.BranchOfficeOrganizationUnit1CCode != csvHeader.BranchOfficeOrganizationUnit1CCode)
                    {
                        result.Errors.Add(string.Format(
                            "Код 1С юр. лица отделения организации ({0}) в строке {1} не совпадает с кодом в заголовке файла импорта ({2})",
                            row.BranchOfficeOrganizationUnit1CCode,
                            i,
                            csvHeader.BranchOfficeOrganizationUnit1CCode));
                        continue;
                    }
                }

                BranchOfficeOrganizationUnit branchOfficeOrganizationUnit;

                if (_branchOfficeOrganizationUnitsBy1CCode.ContainsKey(row.BranchOfficeOrganizationUnit1CCode))
                {
                    branchOfficeOrganizationUnit = _branchOfficeOrganizationUnitsBy1CCode[row.BranchOfficeOrganizationUnit1CCode];
                }
                else
                {
                    branchOfficeOrganizationUnit = _branchOfficeReadModel.GetBranchOfficeOrganizationUnit(row.BranchOfficeOrganizationUnit1CCode);
                    _branchOfficeOrganizationUnitsBy1CCode.Add(row.BranchOfficeOrganizationUnit1CCode, branchOfficeOrganizationUnit);
                }

                if (branchOfficeOrganizationUnit == null)
                {
                    result.Errors.Add(string.Format("Активное юридическое лицо отделения организации с кодом '{0}' не найдено. Строка {1}.", row.BranchOfficeOrganizationUnit1CCode, i));
                    continue;
                }

                var accountsByLegalPerson1CCode = _accountRepository.GetAccountsByLegalPerson(row.LegalPerson1CCode).ToArray();

                // Костыльное отключение проверки для филиала. К Июлю они должны быть включены
                if (contributionType == ContributionTypeEnum.Franchisees)
                {
                    if (accountsByLegalPerson1CCode.Length > 1)
                    {
                        result.Errors.Add(string.Format("Найдено более одного лицевого счета с кодом '{0}'. Строка {1}.", row.LegalPerson1CCode, i));
                        continue;
                    }

                    if (accountsByLegalPerson1CCode.Length < 1)
                    {
                        result.Errors.Add(string.Format("Не найдено ни одного лицевого счета с кодом '{0}'. Строка {1}.", row.LegalPerson1CCode, i));
                        continue;
                    }

                    var accountByLegalPerson1CCode = accountsByLegalPerson1CCode[0];

                    if (accountByLegalPerson1CCode.BranchOfficeOrganizationUnitId != request.BranchOfficeOrganizationUnitId)
                    {
                        result.Errors.Add(
                            string.Format(
                                "В строке {0} юр. лицо отделения организации лицевого счета не совпадает с юр. лицом отделения организации, указанным в форме",
                                i));
                    }
                }

                var legalPersons =
                    _legalPersonRepository.FindLegalPersons(row.LegalPerson1CCode, branchOfficeOrganizationUnit.Id).ToArray();

                if (legalPersons.Length > 1)
                {
                    result.Errors.Add(string.Format("В строке {0} по коду договора найдено более 1 значения.", i));
                }

                if (legalPersons.Length == 0)
                {
                    result.Errors.Add(string.Format("В строке {0} значение в поле 'Код договора' не найдено в Базе Данных", i));
                    continue;
                }

                var legalPerson = legalPersons[0];
                var legalPersonType = legalPerson.LegalPersonTypeEnum;

                var isNaturalPerson = legalPersonType == LegalPersonType.NaturalPerson;
                if (string.IsNullOrEmpty(row.InnOrPassportSeries))
                {
                    AddErrorFieldValueIsMissing(result, i, isNaturalPerson ? "Серия паспорта" : "ИНН");
                }

                if (string.IsNullOrEmpty(row.KppOrPassportNumber) && legalPersonType != LegalPersonType.Businessman)
                {
                    AddErrorFieldValueIsMissing(result, i, isNaturalPerson ? "Номер паспорта" : "КПП");
                }

                if (legalPersonType == LegalPersonType.LegalPerson || legalPersonType == LegalPersonType.Businessman)
                {
                    if (legalPerson.Inn != row.InnOrPassportSeries && !string.IsNullOrEmpty(row.InnOrPassportSeries))
                    {
                        result.Errors.Add(
                            string.Format("В строке {0} не совпадает ИНН, найденное в БД и указанное в файле.", i));
                    }
                }

                if (legalPersonType == LegalPersonType.LegalPerson)
                {
                    if (legalPerson.Kpp != row.KppOrPassportNumber && !string.IsNullOrEmpty(row.KppOrPassportNumber))
                    {
                        result.Errors.Add(
                            string.Format("В строке {0} не совпадает КПП, найденное в БД и указанное в файле.", i));
                    }
                }

                if (legalPersonType == LegalPersonType.NaturalPerson)
                {
                    if (legalPerson.PassportNumber != row.KppOrPassportNumber && !string.IsNullOrEmpty(row.KppOrPassportNumber))
                    {
                        result.Errors.Add(
                            string.Format("В строке {0} не совпадает номер паспорта, найденный в БД и указанный в файле.", i));
                        continue;
                    }

                    if (legalPerson.PassportSeries != row.InnOrPassportSeries && !string.IsNullOrEmpty(row.InnOrPassportSeries))
                    {
                        result.Errors.Add(
                            string.Format("В строке {0} не совпадает серия паспорта, найденная в БД и указанная в файле.", i));
                        continue;
                    }
                }

                // Костыльное отключение проверки для филиала. К Июлю они должны быть включены
                if (contributionType == ContributionTypeEnum.Franchisees)
                {
                    LegalPerson[] legalPersonsByRequisites;
                    switch (legalPersonType)
                    {
                        case LegalPersonType.LegalPerson:
                            {
                                legalPersonsByRequisites =
                                    _legalPersonRepository.FindLegalPersonsByInnAndKpp(row.InnOrPassportSeries, row.KppOrPassportNumber).ToArray();
                                break;
                            }

                        case LegalPersonType.Businessman:
                            {
                                legalPersonsByRequisites =
                                    _legalPersonRepository.FindBusinessmenByInn(row.InnOrPassportSeries).ToArray();
                                break;
                            }

                        case LegalPersonType.NaturalPerson:
                            {
                                legalPersonsByRequisites =
                                    _legalPersonRepository.FindNaturalPersonsByPassport(row.InnOrPassportSeries, row.KppOrPassportNumber).ToArray();
                                break;
                            }

                        default:
                            throw new ArgumentOutOfRangeException();
                    }

                    if (legalPersonsByRequisites.Length > 1)
                    {
                        result.Errors.Add(
                            string.Format("По реквизитам в строке {0} найдено более одного юр. лица.", i));
                        continue;
                    }

                    if (legalPersonsByRequisites.Length < 1)
                    {
                        result.Errors.Add(
                            string.Format("По реквизитам в строке {0} не найдено ни одного активного юр. лица.", i));
                        continue;
                    }

                    if (legalPersonsByRequisites.Length > 0)
                    {
                        var legalPersonByRequisites = legalPersonsByRequisites[0];
                        var accountByLegalPerson1CCode = accountsByLegalPerson1CCode[0];
                        if (accountByLegalPerson1CCode.LegalPersonId != legalPersonByRequisites.Id)
                        {
                            result.Errors.Add(
                                string.Format("В строке {0} юр. лицо клиента лицевого счета не совпадает с юр. лицом, найденным по реквизитам", i));
                        }
                    }

                    if (row.OperationDate < csvHeader.Period.Start)
                    {
                        result.Errors.Add(
                            string.Format("В строке {0} дата операции меньше, чем начало периода, указанного в заголовке", i));
                    }

                    if (row.OperationDate > csvHeader.Period.End)
                    {
                        result.Errors.Add(
                            string.Format("В строке {0} дата операции больше, чем конец периода, указанного в заголовке", i));
                    }
                }
            }

            _branchOfficeOrganizationUnitsBy1CCode.Clear();
            _branchOfficeOrganizationUnitsById.Clear();
            return result;
        }

        private static void AddErrorFieldValueIsMissing(ValidateAccountDetailsFrom1CResponse result, int lineNumber, string fieldName)
        {
            result.Errors.Add(string.Format("В строке {0} отсутствует значение в поле '{1}'", lineNumber, fieldName));
        }
    }
}
