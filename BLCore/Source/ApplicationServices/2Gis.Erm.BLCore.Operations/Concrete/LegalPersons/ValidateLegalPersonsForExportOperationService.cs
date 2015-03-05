using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.LegalPersons;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.API.Core.Operations.Logging;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Identities.Operations.Identity.Specific.LegalPerson;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.LegalPersons
{
    public sealed class ValidateLegalPersonsForExportOperationService : IValidateLegalPersonsForExportOperationService
    {
        private static readonly Func<LegalPerson, string>[] BlockingValidators =
        {
            x => string.IsNullOrEmpty(x.LegalName) ? "Не указано юридическое название" : null,
            x => string.IsNullOrEmpty(x.ShortName) ? "Не указано короткое название" : null,
            x => x.LegalPersonTypeEnum != LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.LegalAddress)
                        ? "Не указан юридический адрес"
                        : null,
            x => x.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.RegistrationAddress)
                        ? "Не указана прописка"
                        : null,
            x => (x.LegalPersonTypeEnum == LegalPersonType.Businessman ||
                   x.LegalPersonTypeEnum == LegalPersonType.LegalPerson) &&
                  string.IsNullOrEmpty(x.Inn)
                        ? "Не указан ИНН"
                        : null,
            x => (x.LegalPersonTypeEnum == LegalPersonType.Businessman &&
                   !string.IsNullOrEmpty(x.Inn) && x.Inn.Replace(" ", string.Empty).Length != 12) ||
                  (x.LegalPersonTypeEnum == LegalPersonType.LegalPerson &&
                   !string.IsNullOrEmpty(x.Inn) && x.Inn.Replace(" ", string.Empty).Length != 10)
                        ? "ИНН не соответствует формату"
                        : null,
            x => x.LegalPersonTypeEnum == LegalPersonType.LegalPerson &&
                  string.IsNullOrEmpty(x.Kpp)
                        ? "Не указан КПП"
                        : null,
            x => (x.LegalPersonTypeEnum == LegalPersonType.LegalPerson &&
                   !string.IsNullOrEmpty(x.Kpp) &&
                   x.Kpp.Replace(" ", string.Empty).Length != 9)
                        ? "КПП не соответствует формату"
                        : null,
            x => x.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.PassportSeries)
                        ? "Не указана серия паспорта"
                        : null,
            x => x.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                  !string.IsNullOrEmpty(x.PassportSeries) &&
                  x.PassportSeries.Replace(" ", string.Empty).Length != 4
                        ? "Серия паспорта не соответствуют формату"
                        : null,
            x => x.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.PassportNumber)
                        ? "Не указан номер паспорта"
                        : null,
            x => x.LegalPersonTypeEnum == LegalPersonType.NaturalPerson &&
                  !string.IsNullOrEmpty(x.PassportNumber) &&
                  x.PassportNumber.Replace(" ", string.Empty).Length != 6
                        ? "Номер паспорта не соответствуют формату"
                        : null
        };

        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IOperationScopeFactory _operationScopeFactory;

        public ValidateLegalPersonsForExportOperationService(ILegalPersonReadModel legalPersonReadModel, IOperationScopeFactory operationScopeFactory)
        {
            _legalPersonReadModel = legalPersonReadModel;
            _operationScopeFactory = operationScopeFactory;
        }

        public IEnumerable<LegalPersonValidationForExportErrorDto> Validate(IEnumerable<ValidateLegalPersonDto> legalPersonsToValidate)
        {
            using (var scope = _operationScopeFactory.CreateNonCoupled<ValidateLegalPersonsForExportIdentity>())
            {
                var errors = new List<LegalPersonValidationForExportErrorDto>();
                var syncCodesByIds = legalPersonsToValidate.ToDictionary(x => x.LegalPersonId, y => y.SyncCode1C);

                var entitiesSyncCodes = legalPersonsToValidate.Select(x => x.SyncCode1C);
                var nonUniqueCode1C = _legalPersonReadModel.SelectNotUnique1CSyncCodes(entitiesSyncCodes);
                var legalPersons = _legalPersonReadModel.GetLegalPersonsWithProfileExistanceInfo(legalPersonsToValidate.Select(x => x.LegalPersonId).ToArray());

                errors.AddRange(legalPersonsToValidate.Where(x => string.IsNullOrWhiteSpace(x.SyncCode1C))
                                                              .Select(x => new LegalPersonValidationForExportErrorDto
                                                              {
                                                                  IsBlockingError = true,
                                                                  LegalPersonId = x.LegalPersonId,
                                                                  ErrorMessage = "Не указан идентификатор 1С"
                                                              }));

                errors.AddRange(legalPersons.Where(x => !x.LegalPersonHasProfiles)
                                            .Select(x => new LegalPersonValidationForExportErrorDto
                                            {
                                                IsBlockingError = true,
                                                LegalPersonId = x.LegalPerson.Id,
                                                ErrorMessage =
                                                    string.Format("Проверка юр.лица с кодом '{0}': [Блокирующая ошибка] {1}",
                                                                  x.LegalPerson.Id,
                                                                  BLResources.MustMakeLegalPersonProfile),
                                                SyncCode1C = syncCodesByIds[x.LegalPerson.Id]
                                            }));

                foreach (var legalPerson in legalPersons.Select(x => x.LegalPerson))
                {
                    errors.AddRange(BlockingValidators.Select(x => x(legalPerson))
                                                      .Where(x => x != null)
                                                      .Select(x => new LegalPersonValidationForExportErrorDto
                                                      {
                                                          IsBlockingError = true,
                                                          LegalPersonId = legalPerson.Id,
                                                          SyncCode1C = syncCodesByIds[legalPerson.Id],
                                                          ErrorMessage =
                                                              string.Format("Проверка юр.лица с кодом '{0}': [Блокирующая ошибка] {1}",
                                                                            legalPerson.Id,
                                                                            x)
                                                      }));
                }

                // Неблокирующие ошибки:
                errors.AddRange(legalPersonsToValidate.Where(x => nonUniqueCode1C.Contains(x.SyncCode1C))
                                                      .Select(x => new LegalPersonValidationForExportErrorDto
                                                      {
                                                          IsBlockingError = false,
                                                          LegalPersonId = x.LegalPersonId,
                                                          ErrorMessage = string.Format("Код 1С [{0}] юр лица клиента не уникален", x.SyncCode1C),
                                                          SyncCode1C = x.SyncCode1C
                                                      }));
                
                scope.Complete();
                return errors;   
            }
        }
    }
}