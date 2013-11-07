using System;
using System.Linq;

using DoubleGis.Erm.BL.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Russia.Generic.Get
{
    public class GetLegalPersonProfileDtoService : GetDomainEntityDtoServiceBase<LegalPersonProfile>, IRussiaAdapted
    {
        private readonly ISecureFinder _finder;

        public GetLegalPersonProfileDtoService(IUserContext userContext, ISecureFinder finder) : base(userContext)
        {
            _finder = finder;
        }

        protected override IDomainEntityDto<LegalPersonProfile> GetDto(long entityId)
        {
            return _finder.Find<LegalPersonProfile>(x => x.Id == entityId)
                          .Select(entity => new LegalPersonProfileDomainEntityDto
                              {
                                  Id = entity.Id,
                                  Name = entity.Name,
                                  AdditionalEmail = entity.AdditionalEmail,
                                  ChiefNameInGenitive = entity.ChiefNameInGenitive,
                                  ChiefNameInNominative = entity.ChiefNameInNominative,
                                  DocumentsDeliveryAddress = entity.DocumentsDeliveryAddress,
                                  DocumentsDeliveryMethod = (DocumentsDeliveryMethod)entity.DocumentsDeliveryMethod,
                                  LegalPersonRef = new EntityReference { Id = entity.LegalPersonId, Name = entity.LegalPerson.LegalName },
                                  PositionInNominative = entity.PositionInNominative,
                                  PositionInGenitive = entity.PositionInGenitive,
                                  OperatesOnTheBasisInGenitive = entity.OperatesOnTheBasisInGenitive == null
                                                                            ? OperatesOnTheBasisType.Underfined
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
                                  BankName = entity.BankName,
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

        protected override IDomainEntityDto<LegalPersonProfile> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            if (parentEntityId == null)
            {
                throw new ArgumentException("parentId");
            }

            return _finder.Find<LegalPerson>(x => x.Id == parentEntityId)
                          .Select(legalPerson => new LegalPersonProfileDomainEntityDto
                              {
                                  LegalPersonRef = new EntityReference { Id = parentEntityId.Value, Name = legalPerson.LegalName },
                                  LegalPersonType = (LegalPersonType)legalPerson.LegalPersonTypeEnum,
                                  DocumentsDeliveryMethod = DocumentsDeliveryMethod.PostOnly,
                              })
                          .Single();
        }
    }
}