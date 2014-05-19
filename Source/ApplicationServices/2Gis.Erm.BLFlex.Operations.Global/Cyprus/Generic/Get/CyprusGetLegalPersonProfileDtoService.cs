using System;
using System.Linq;

using DoubleGis.Erm.BLCore.API.Aggregates.LegalPersons.ReadModel;
using DoubleGis.Erm.BLCore.Operations.Generic.Get;
using DoubleGis.Erm.Platform.API.Security.UserContext;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Entities;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;
using DoubleGis.Erm.Platform.Model.Metadata.Globalization;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Get
{
    public class CyprusGetLegalPersonProfileDtoService : GetDomainEntityDtoServiceBase<LegalPersonProfile>, ICyprusAdapted
    {
        private readonly ISecureFinder _finder;
        private readonly ILegalPersonReadModel _legalPersonReadModel;

        public CyprusGetLegalPersonProfileDtoService(IUserContext userContext, ISecureFinder finder, ILegalPersonReadModel legalPersonReadModel)
            : base(userContext)
        {
            _finder = finder;
            _legalPersonReadModel = legalPersonReadModel;
        }

        protected override IDomainEntityDto<LegalPersonProfile> GetDto(long entityId)
        {
            return _legalPersonReadModel.GetLegalPersonProfileDto<LegalPersonProfileDomainEntityDto>(entityId);
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
                          .Select(legalPerson => new LegalPersonProfileDomainEntityDto
                              {
                                  LegalPersonRef = new EntityReference { Id = parentEntityId.Value, Name = legalPerson.LegalName },
                                  LegalPersonType = (LegalPersonType)legalPerson.LegalPersonTypeEnum,
                                  DocumentsDeliveryMethod = DocumentsDeliveryMethod.PostOnly,
                                  PaymentMethod = PaymentMethod.Undefined
                              })
                          .Single();
        }
    }
}