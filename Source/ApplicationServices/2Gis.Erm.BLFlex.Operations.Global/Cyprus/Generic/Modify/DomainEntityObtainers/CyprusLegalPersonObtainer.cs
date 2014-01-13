using System.Linq;

using DoubleGis.Erm.BLCore.API.Operations.Generic.Modify.DomainEntityObtainers;
using DoubleGis.Erm.Platform.API.Core.Globalization;
using DoubleGis.Erm.Platform.DAL;
using DoubleGis.Erm.Platform.DAL.Specifications;
using DoubleGis.Erm.Platform.Model.Aggregates;
using DoubleGis.Erm.Platform.Model.Entities.DTOs;
using DoubleGis.Erm.Platform.Model.Entities.Enums;
using DoubleGis.Erm.Platform.Model.Entities.Erm;
using DoubleGis.Erm.Platform.Model.Entities.Interfaces;

namespace DoubleGis.Erm.BLFlex.Operations.Global.Cyprus.Generic.Modify.DomainEntityObtainers
{
    public sealed class CyprusLegalPersonObtainer : IBusinessModelEntityObtainer<LegalPerson>, IAggregateReadModel<LegalPerson>, ICyprusAdapted
    {
        private readonly IFinder _finder;

        public CyprusLegalPersonObtainer(IFinder finder)
        {
            _finder = finder;
        }

        public LegalPerson ObtainBusinessModelEntity(IDomainEntityDto domainEntityDto)
        {
            var dto = (LegalPersonDomainEntityDto)domainEntityDto;

            var legalPerson = dto.Id == 0
                                  ? new LegalPerson { IsActive = true }
                                  : _finder.Find(Specs.Find.ById<LegalPerson>(dto.Id)).Single();

            if (!dto.IsInSyncWith1C)
            {
                legalPerson.LegalPersonTypeEnum = (int)dto.LegalPersonTypeEnum;
                legalPerson.LegalName = dto.LegalName;
                legalPerson.ShortName = dto.ShortName;
                legalPerson.LegalAddress = dto.LegalAddress;
                legalPerson.Kpp = dto.Kpp;
                legalPerson.PassportNumber = dto.PassportNumber;
            }

            legalPerson.Inn = dto.LegalPersonTypeEnum == LegalPersonType.Businessman ? dto.BusinessmanInn : dto.Inn;
            legalPerson.VAT = dto.VAT;
            legalPerson.PassportSeries = dto.PassportSeries;
            legalPerson.OwnerCode = dto.OwnerRef.Id.Value;
            legalPerson.ClientId = dto.ClientRef.Id;
            legalPerson.Comment = dto.Comment;
            legalPerson.PassportIssuedBy = dto.PassportIssuedBy;
            legalPerson.RegistrationAddress = dto.RegistrationAddress;
            legalPerson.Timestamp = dto.Timestamp;
            legalPerson.CardNumber = dto.CardNumber;

            return legalPerson;
        }
    }
}