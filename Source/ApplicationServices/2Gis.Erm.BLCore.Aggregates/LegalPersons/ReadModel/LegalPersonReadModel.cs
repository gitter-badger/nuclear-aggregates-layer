using System.Collections.Generic;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.Common.DTO;
using DoubleGis.Erm.BLCore.API.Aggregates.Common.Specs;
using DoubleGis.Erm.BLCore.API.Aggregates.Dynamic.ReadModel;
using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;

namespace DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel
{
    // FIXME {all, 07.04.2014}: к именно текущему состоянию данного типа вопросов не очень много, но предлагаю пока взять timeout на дальнейшее масштабирование практики переопределения readmodel в зависимости от business model через наследование
    // т.к. у нас в приложении в перспективе будет на уровне метаданных полная информация в какой бизнесмодели какие свойства к каким сущностям подцепленны, возможно не придется этим заниматься на уровне императивного кода в readmodel
    // Сам подход partable (расширяемых, возможно лучше было использовать что-то вроде extensibility) сущностей был ориентирован на то, чтобы минимизировать необходимость в создании Chile***Service_Model и т.п. 
    // Итого - до согласования подхода работы с расширяемыми сущностями (EAV и т.п.) пока подход с abstract классом и business model specific подкласами заморожен.
    public abstract class LegalPersonReadModel : ILegalPersonReadModel
    {
        private readonly IFinder _finder;
        private readonly ISecureFinder _secureFinder;

        protected LegalPersonReadModel(IFinder finder, ISecureFinder secureFinder)
        {
            _finder = finder;
            _secureFinder = secureFinder;
        }

        public virtual LegalPerson GetLegalPerson(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId)).Single();
        }

        public virtual LegalPersonProfile GetLegalPersonProfile(long legalPersonProfileId)
        {
            return _finder.Find(Specs.Find.ById<LegalPersonProfile>(legalPersonProfileId)).Single();
            }

        public PaymentMethod? GetPaymentMethod(long legalPersonId)
        {
            return _finder.Find(LegalPersonSpecs.Profiles.Find.MainByLegalPersonId(legalPersonId))
                          .Select(x => (PaymentMethod?)x.PaymentMethod)
                          .SingleOrDefault();
        }

        // FIXME {all, 07.04.2014}: какое-то слишком абстрактное название дляметодов - readmodel это набор методов, являющихся wrapper над спецификациями, разной толщины - но все они usecase специфичны. Т.о. либо метод должен быть более конкретным, либо тип в которомон находиться более абстрактным
        public virtual IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(LegalPerson legalPerson)
        {
            return Enumerable.Empty<BusinessEntityInstanceDto>();
            }

        // FIXME {all, 07.04.2014}: какое-то слишком абстрактное название дляметодов - readmodel это набор методов, являющихся wrapper над спецификациями, разной толщины - но все они usecase специфичны. Т.о. либо метод должен быть более конкретным, либо тип в которомон находиться более абстрактным
        public virtual IEnumerable<BusinessEntityInstanceDto> GetBusinessEntityInstanceDto(LegalPersonProfile legalPersonProfile)
        {
            return Enumerable.Empty<BusinessEntityInstanceDto>();
        }

        public bool HasAnyLegalPersonProfiles(long legalPersonId)
        {
            return _finder.Find(Specs.Find.ById<LegalPerson>(legalPersonId)).Any(x => x.LegalPersonProfiles.Any());
        }

        public T GetLegalPersonDto<T>(long entityId)
            where T : LegalPersonDomainEntityDto, new()
        {
            return _secureFinder.Find<LegalPerson>(x => x.Id == entityId)
                          .Select(entity => new T
            {
                              Id = entity.Id,
                              LegalName = entity.LegalName,
                              ShortName = entity.ShortName,
                              LegalPersonTypeEnum = (LegalPersonType)entity.LegalPersonTypeEnum,
                              LegalAddress = entity.LegalAddress,
                              Inn = entity.Inn,
                              Kpp = entity.Kpp,
                              VAT = entity.VAT,
                              Ic = entity.Ic,
                              PassportSeries = entity.PassportSeries,
                              PassportNumber = entity.PassportNumber,
                              PassportIssuedBy = entity.PassportIssuedBy,
                              RegistrationAddress = entity.RegistrationAddress,
                              ClientRef = new EntityReference { Id = entity.ClientId, Name = entity.Client.Name },
                              IsInSyncWith1C = entity.IsInSyncWith1C,
                              ReplicationCode = entity.ReplicationCode,
                              Comment = entity.Comment,
                              OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                              CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                              CreatedOn = entity.CreatedOn,
                              IsActive = entity.IsActive,
                              IsDeleted = entity.IsDeleted,
                              ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                              ModifiedOn = entity.ModifiedOn,
                              HasProfiles = entity.LegalPersonProfiles.Any(),
                              CardNumber = entity.CardNumber,
                              Timestamp = entity.Timestamp
                          })
                          .Single();
        }

        public T GetLegalPersonProfileDto<T>(long entityId)
            where T : LegalPersonProfileDomainEntityDto, new()
        {
            return _secureFinder.Find<LegalPersonProfile>(x => x.Id == entityId)
                          .Select(entity => new T
        {
                              Id = entity.Id,
                              Name = entity.Name,
                              AdditionalEmail = entity.AdditionalEmail,
                              ChiefNameInGenitive = entity.ChiefNameInGenitive,
                              ChiefNameInNominative = entity.ChiefNameInNominative,
                              Registered = entity.Registered,
                              DocumentsDeliveryAddress = entity.DocumentsDeliveryAddress,
                              DocumentsDeliveryMethod = (DocumentsDeliveryMethod)entity.DocumentsDeliveryMethod,
                              LegalPersonRef = new EntityReference { Id = entity.LegalPersonId, Name = entity.LegalPerson.LegalName },
                              PositionInNominative = entity.PositionInNominative,
                              PositionInGenitive = entity.PositionInGenitive,
                              OperatesOnTheBasisInGenitive = entity.OperatesOnTheBasisInGenitive == null
                                                                        ? OperatesOnTheBasisType.Undefined
                                                                        : (OperatesOnTheBasisType)entity.OperatesOnTheBasisInGenitive,
                              CertificateDate = entity.CertificateDate,
                              CertificateNumber = entity.CertificateNumber,
                              BargainBeginDate = entity.BargainBeginDate,
                              BargainEndDate = entity.BargainEndDate,
                              BargainNumber = entity.BargainNumber,
                              WarrantyNumber = entity.WarrantyNumber,
                              WarrantyBeginDate = entity.WarrantyBeginDate,
                              WarrantyEndDate = entity.WarrantyEndDate,
                              PostAddress = entity.PostAddress,
                              EmailForAccountingDocuments = entity.EmailForAccountingDocuments,
                              LegalPersonType = (LegalPersonType)entity.LegalPerson.LegalPersonTypeEnum,
                              PaymentEssentialElements = entity.PaymentEssentialElements,
                              AdditionalPaymentElements = entity.AdditionalPaymentElements,
                              PaymentMethod = entity.PaymentMethod == null
                                                                        ? PaymentMethod.Undefined
                                                                        : (PaymentMethod)entity.PaymentMethod,
                              IBAN = entity.IBAN,
                              SWIFT = entity.SWIFT,
                              AccountNumber = entity.AccountNumber,
                              BankCode = entity.BankCode,
                              BankName = entity.BankName,
                              BankAddress = entity.BankAddress,
                              RegistrationCertificateDate = entity.RegistrationCertificateDate,
                              RegistrationCertificateNumber = entity.RegistrationCertificateNumber,
                              PersonResponsibleForDocuments = entity.PersonResponsibleForDocuments,
                              Phone = entity.Phone,
                              RecipientName = entity.RecipientName,
                              IsMainProfile = entity.IsMainProfile,
                              OwnerRef = new EntityReference { Id = entity.OwnerCode, Name = null },
                              CreatedByRef = new EntityReference { Id = entity.CreatedBy, Name = null },
                              CreatedOn = entity.CreatedOn,
                              IsActive = entity.IsActive,
                              IsDeleted = entity.IsDeleted,
                              ModifiedByRef = new EntityReference { Id = entity.ModifiedBy, Name = null },
                              ModifiedOn = entity.ModifiedOn,
                              Timestamp = entity.Timestamp
                          })
                          .Single();
        }
    }
}