using System;
using System.Linq;

using DoubleGis.Erm.BLCore.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Aggregates.SimplifiedModel.DictionaryEntity.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.BLFlex.Aggregates.Global.Chile.SimplifiedModel.ReadModel;
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
        private readonly ILegalPersonReadModel _legalPersonReadModel;
        private readonly IBankReadModel _bankReadModel;

        public ChileGetLegalPersonProfileDtoService(IUserContext userContext, 
            ISecureFinder finder,
            ILegalPersonReadModel legalPersonProfileReadModel, 
            IBankReadModel bankReadModel)
            : base(userContext)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonProfileReadModel;
            _bankReadModel = bankReadModel;
        }

        protected override IDomainEntityDto<LegalPersonProfile> GetDto(long entityId)
        {
            var profile = _legalPersonReadModel.GetLegalPersonProfile(entityId);
            if (profile == null)
            {
                throw new EntityNotFoundException(typeof(LegalPersonProfile), entityId);
            }

            var profilePart = profile.ChilePart();

            // COMMENT {all, 12.02.2014}: Скажите мне, это нормально, что приходится пользоваться ReadModel и Finder для получения названий?
            // COMMENT {a.rechkalov, 02.04.2014}: finder для получения названия больше не используется
            var bankName = profilePart.BankId.HasValue ? _bankReadModel.GetBank(profilePart.BankId.Value).Name : string.Empty;

            var dto = _legalPersonReadModel.GetLegalPersonProfileDto<ChileLegalPersonProfileDomainEntityDto>(entityId);

            dto.AccountType = profilePart.AccountType;
            dto.BankRef = new EntityReference { Id = profilePart.BankId, Name = bankName };
            dto.RepresentativeRut = profile.ChilePart().RepresentativeRut;
            dto.RepresentativeDocumentIssuedOn = profilePart.RepresentativeAuthorityDocumentIssuedOn;
            dto.RepresentativeDocumentIssuedBy = profilePart.RepresentativeAuthorityDocumentIssuedBy;

            return dto;
        }

        protected override IDomainEntityDto<LegalPersonProfile> CreateDto(long? parentEntityId, EntityName parentEntityName, string extendedInfo)
        {
            if (!parentEntityId.HasValue)
            {
                throw new ArgumentNullException("parentEntityId");
            }

            if (parentEntityName != EntityName.LegalPerson)
            {
                throw new ArgumentException("parentEntityName");
            }

            return _finder.Find(Specs.Find.ById<LegalPerson>(parentEntityId.Value))
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