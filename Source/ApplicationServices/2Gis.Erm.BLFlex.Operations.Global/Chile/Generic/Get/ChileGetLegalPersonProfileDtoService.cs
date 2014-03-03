using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Model.Entities.DTOs;
using DoubleGis.Erm.BLFlex.Operations.Global.Shared;
using DoubleGis.Erm.Core.Exceptions;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Chile.Generic.Get
{
    public class ChileGetLegalPersonProfileDtoService : GetDomainEntityDtoServiceBase<LegalPersonProfile>, IChileAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly ILegalPersonReadModel _legalPersonProfileReadModel;
        private readonly IBankReadModel _bankReadModel;

        public ChileGetLegalPersonProfileDtoService(IUserContext userContext, 
            ISecureFinder finder,
            ILegalPersonReadModel legalPersonProfileReadModel, 
            IBankReadModel bankReadModel)
            : base(userContext)
        {
            _finder = finder;
            _legalPersonProfileReadModel = legalPersonProfileReadModel;
            _bankReadModel = bankReadModel;
        }

        protected override IDomainEntityDto<LegalPersonProfile> GetDto(long entityId)
        {
            var profile = _legalPersonProfileReadModel.GetLegalPersonProfile(entityId);
            if (profile == null)
            {
                throw new EntityNotFoundException(typeof(LegalPersonProfile), entityId);
            }

            var profilePart = profile.Parts.OfType<LegalPersonProfilePart>().SingleOrDefault();
            if (profilePart == null)
            {
                throw new EntityNotFoundException(typeof(LegalPersonProfile), entityId);
            }

            // COMMENT {all, 12.02.2014}: Скажите мне, это нормально, что приходится пользоваться ReadModel и Finder для получения названий?
            var bankName = profilePart.BankId.HasValue ? _bankReadModel.GetBank(profilePart.BankId.Value).Name : string.Empty;
            var legalPersonName = _finder.Find(Specs.Find.ById<LegalPerson>(profile.LegalPersonId)).Single().LegalName;

            return new ChileLegalPersonProfileDomainEntityDto
                {
                    Id = profile.Id,
                    Name = profile.Name,
                    LegalPersonRef = new EntityReference { Id = profile.LegalPersonId, Name = legalPersonName },
                    DocumentsDeliveryAddress = profile.DocumentsDeliveryAddress,
                    RecipientName = profile.RecipientName,
                    PersonResponsibleForDocuments = profile.PersonResponsibleForDocuments,
                    DocumentsDeliveryMethod = (DocumentsDeliveryMethod)profile.DocumentsDeliveryMethod,
                    EmailForAccountingDocuments = profile.EmailForAccountingDocuments,
                    AdditionalEmail = profile.AdditionalEmail,
                    PostAddress = profile.PostAddress,
                    PaymentMethod = profile.PaymentMethod == null
                                        ? PaymentMethod.Undefined
                                        : (PaymentMethod)profile.PaymentMethod,
                    AccountType = profilePart.AccountType,
                    AccountNumber = profile.AccountNumber,
                    BankRef = new EntityReference { Id = profilePart.BankId, Name = bankName },
                    AdditionalPaymentElements = profile.AdditionalPaymentElements,
                    RepresentativeName = profile.ChiefNameInNominative,
                    RepresentativePosition = profile.PositionInNominative,
                    RepresentativeRut = profile.ChilePart().RepresentativeRut,
                    Phone = profile.Phone,
                    OperatesOnTheBasisInGenitive = profile.OperatesOnTheBasisInGenitive == null
                                                                     ? OperatesOnTheBasisType.Undefined
                                                       : (OperatesOnTheBasisType)profile.OperatesOnTheBasisInGenitive,
                    RepresentativeDocumentIssuedOn = profilePart.RepresentativeAuthorityDocumentIssuedOn,
                    RepresentativeDocumentIssuedBy = profilePart.RepresentativeAuthorityDocumentIssuedBy,

                    IsMainProfile = profile.IsMainProfile,
                    OwnerRef = new EntityReference { Id = profile.OwnerCode, Name = null },
                    CreatedByRef = new EntityReference { Id = profile.CreatedBy, Name = null },
                    CreatedOn = profile.CreatedOn,
                    IsActive = profile.IsActive,
                    IsDeleted = profile.IsDeleted,
                    ModifiedByRef = new EntityReference { Id = profile.ModifiedBy, Name = null },
                    ModifiedOn = profile.ModifiedOn,
                    Timestamp = profile.Timestamp
                };
        }

        protected override IDomainEntityDto<LegalPersonProfile> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            if (parentEntityId == null)
            {
                throw new ArgumentException("parentId");
            }

            return _finder.Find<LegalPerson>(x => x.Id == parentEntityId)
                          .Select(legalPerson => new ChileLegalPersonProfileDomainEntityDto
                              {
                                  LegalPersonRef = new EntityReference { Id = parentEntityId.Value, Name = legalPerson.LegalName },
                                  PaymentMethod = PaymentMethod.BankTransaction,
                                  DocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined,
                              })
                          .Single();
        }
    }
}