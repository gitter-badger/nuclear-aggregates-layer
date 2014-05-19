using System;
using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons;
using DoubleGis.Erm.BLCore.API.Operations.Concrete.Old.Integration.OneC;
using DoubleGis.Erm.BLCore.Common.Infrastructure.Handlers;
using DoubleGis.Erm.BLCore.Resources.Server.Properties;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Operations.Concrete.Old.Integration.OneC
{
    public sealed class ValidateLegalPersonsFor1CHandler : RequestHandler<ValidateLegalPersonsFor1CRequest, ValidateLegalPersonsResponse>
    {
        private static readonly Func<LegalPerson, string>[] BlockingValidators =
        {
            x => string.IsNullOrEmpty(x.LegalName) ? "Не указано юридическое название" : null,
            x => string.IsNullOrEmpty(x.ShortName) ? "Не указано короткое название" : null,
            x => x.LegalPersonTypeEnum != (int)LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.LegalAddress)
                        ? "Не указан юридический адрес"
                        : null,
            x => x.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.RegistrationAddress)
                        ? "Не указана прописка"
                        : null,
            x => (x.LegalPersonTypeEnum == (int)LegalPersonType.Businessman ||
                   x.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson) &&
                  string.IsNullOrEmpty(x.Inn)
                        ? "Не указан ИНН"
                        : null,
            x => (x.LegalPersonTypeEnum == (int)LegalPersonType.Businessman &&
                   !string.IsNullOrEmpty(x.Inn) && x.Inn.Replace(" ", string.Empty).Length != 12) ||
                  (x.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson &&
                   !string.IsNullOrEmpty(x.Inn) && x.Inn.Replace(" ", string.Empty).Length != 10)
                        ? "ИНН не соответствует формату"
                        : null,
            x => x.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson &&
                  string.IsNullOrEmpty(x.Kpp)
                        ? "Не указан КПП"
                        : null,
            x => (x.LegalPersonTypeEnum == (int)LegalPersonType.LegalPerson &&
                   !string.IsNullOrEmpty(x.Kpp) &&
                   x.Kpp.Replace(" ", string.Empty).Length != 9)
                        ? "КПП не соответствует формату"
                        : null,
            x => x.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.PassportSeries)
                        ? "Не указана серия паспорта"
                        : null,
            x => x.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson &&
                  !string.IsNullOrEmpty(x.PassportSeries) &&
                  x.PassportSeries.Replace(" ", string.Empty).Length != 4
                        ? "Серия паспорта не соответствуют формату"
                        : null,
            x => x.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson &&
                  string.IsNullOrEmpty(x.PassportNumber)
                        ? "Не указан номер паспорта"
                        : null,
            x => x.LegalPersonTypeEnum == (int)LegalPersonType.NaturalPerson &&
                  !string.IsNullOrEmpty(x.PassportNumber) &&
                  x.PassportNumber.Replace(" ", string.Empty).Length != 6
                        ? "Номер паспорта не соответствуют формату"
                        : null
        };

        private readonly ILegalPersonRepository _legalPersonRepository;

        public ValidateLegalPersonsFor1CHandler(ILegalPersonRepository legalPersonRepository)
        {
            _legalPersonRepository = legalPersonRepository;
        }

        protected override ValidateLegalPersonsResponse Handle(ValidateLegalPersonsFor1CRequest request)
        {
            var entitiesSyncCodes = request.Entities.Select(x => x.SyncCode1C);
            var nonUniqueCode1C = _legalPersonRepository.SelectNotUnique1CSyncCodes(entitiesSyncCodes);

            var blockingErrors = new List<ErrorDto>();
            var nonBlockingErrors = new List<ErrorDto>();

            foreach (var item in request.Entities)
            {
                ValidateSingleItem(item, nonUniqueCode1C, blockingErrors, nonBlockingErrors);
                }

            return new ValidateLegalPersonsResponse
            {
                BlockingErrors = blockingErrors,
                NonBlockingErrors = nonBlockingErrors,
            };
        }

        private void ValidateSingleItem(ValidateLegalPersonRequestItem request, string[] nonUniqueCodes1C, ICollection<ErrorDto> blockingErrors, ICollection<ErrorDto> nonBlockingErrors)
        {
            var blockingValidators = new Func<LegalPerson, string>[]
            {
                lp => string.IsNullOrEmpty(request.SyncCode1C)
                            ? "Не указан идентификатор 1С"
                            : null
            };

            var nonBlockingValidators = new Func<LegalPerson, string>[]
            {
                lp =>
                    {
                        var isSyncCode1CUnique = Array.IndexOf(nonUniqueCodes1C, request.SyncCode1C) == -1;
                        return !isSyncCode1CUnique
                                   ? string.Format("Код 1С [{0}] юр лица клиента не уникален", request.SyncCode1C)
                                   : null;
                    }
            };

            var profilesInfo = _legalPersonRepository.GetLegalPersonWithProfiles(request.Entity.Id);

            if (profilesInfo != null && !profilesInfo.Profiles.Any())
            {
                blockingErrors.Add(new ErrorDto
                {
                    LegalPersonId = request.Entity.Id,
                    SyncCode1C = request.SyncCode1C,
                    ErrorMessage = string.Concat("Проверка юр.лица с кодом '", request.Entity.Id, "': [Блокирующая ошибка] ", BLResources.MustMakeLegalPersonProfile)
                });
            }

            // validate blocking validators
            foreach (var validator in BlockingValidators.Concat(blockingValidators))
            {
                var errorMessage = validator(request.Entity);
                if (errorMessage == null)
                {
                    continue;
                }

                blockingErrors.Add(new ErrorDto
                                   {
                    LegalPersonId = request.Entity.Id,
                                       SyncCode1C = request.SyncCode1C,
                    ErrorMessage = string.Concat("Проверка юр.лица с кодом '", request.Entity.Id, "': [Блокирующая ошибка] ", errorMessage)
                });
            }

            // validate non-blocking validators
            foreach (var validator in nonBlockingValidators)
            {
                var errorMessage = validator(request.Entity);
                if (errorMessage == null)
                {
                    continue;
                }

                nonBlockingErrors.Add(new ErrorDto
                                 {
                    LegalPersonId = request.Entity.Id,
                                     SyncCode1C = request.SyncCode1C,
                    ErrorMessage = string.Concat("Проверка юр.лица с кодом '", request.Entity.Id, "': [Неблокирующая ошибка] ", errorMessage)
                });
            }
        }
    }
}
