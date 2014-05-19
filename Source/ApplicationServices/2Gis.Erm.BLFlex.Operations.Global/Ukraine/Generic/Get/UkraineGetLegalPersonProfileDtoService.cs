using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
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

namespace DoubleGis.Erm.BLFlex.Operations.Global.Ukraine.Generic.Get
{
    public class UkraineGetLegalPersonProfileDtoService : GetDomainEntityDtoServiceBase<LegalPersonProfile>, IUkraineAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public UkraineGetLegalPersonProfileDtoService(IUserContext userContext,
                                                      ISecureFinder finder,
                                                      ILegalPersonReadModel legalPersonProfileReadModel)
            : base(userContext)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonProfileReadModel;
        }

        protected override IDomainEntityDto<LegalPersonProfile> GetDto(long entityId)
        {
            var profile = _legalPersonReadModel.GetLegalPersonProfile(entityId);
            if (profile == null)
            {
                throw new EntityNotFoundException(typeof(LegalPersonProfile), entityId);
            }

            var profilePart = profile.UkrainePart();

            var dto = _legalPersonReadModel.GetLegalPersonProfileDto<UkraineLegalPersonProfileDomainEntityDto>(entityId);

            dto.Mfo = profilePart.Mfo;

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
                          .Select(legalPerson => new UkraineLegalPersonProfileDomainEntityDto
                              {
                                  LegalPersonRef = new EntityReference { Id = parentEntityId.Value, Name = legalPerson.LegalName },
                                  PaymentMethod = PaymentMethod.BankTransaction,
                                  DocumentsDeliveryMethod = DocumentsDeliveryMethod.Undefined,
                                  LegalPersonType = (LegalPersonType)legalPerson.LegalPersonTypeEnum,
                              })
                          .Single();
        }
    }
}